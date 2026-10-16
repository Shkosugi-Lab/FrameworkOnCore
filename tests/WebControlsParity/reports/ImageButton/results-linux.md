# ImageButton パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 18:10
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 188 | 188 | 0 | 0 |
| ② HTML | 180 | 180 | 0 | 0 |
| ③ シナリオ | 10 | 10 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 197 件中 197 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 40 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 10 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 166 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 8 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 2 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 5 | 0 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 1 | ✅ |
| ポストバックイベント | 0 | 4 | ✅ |
| コマンドイベントと親へのバブル（CommandName） | 9 | 2 | ✅ |
| ブラウザ側のクリック処理（OnClientClick） | 9 | 1 | ✅ |
| クロスページポストバック（PostBackUrl） | 11 | 1 | ✅ |
| 既定のボタン（Panel の DefaultButton） | 0 | 1 | ✅ |
| CausesValidation / ValidationGroup | 11 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>ImageButton.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Context</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Controls</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Events</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Focus</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ID</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Init</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Load</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Page</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Parent</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Render</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Site</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Unload</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ImageButton.Api.Control.Visible</code> | ✅ 一致 |
| <code>ImageButton.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>ImageButton.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>ImageButton.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ImageButton.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ImageButton.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ImageButton.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ImageButton.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ImageButton.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ImageButton.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ImageButton.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ImageButton.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ImageButton.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ImageButton.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>ImageButton.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>ImageButton.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>ImageButton.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.IButtonControl.CausesValidation</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.IButtonControl.Click</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.IButtonControl.Command</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.IButtonControl.CommandArgument</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.IButtonControl.CommandName</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.IButtonControl.PostBackUrl</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.IButtonControl.Text</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.IButtonControl.ValidationGroup</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.Image.#ctor</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.Image.AddAttributesToRender</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.Image.AlternateText</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.Image.DescriptionUrl</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.Image.Enabled</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.Image.Font</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.Image.GenerateEmptyAlternateText</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.Image.ImageAlign</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.Image.ImageUrl</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.Image.RenderContents</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.Image.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.#ctor</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.AddAttributesToRender</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.CausesValidation</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.Click</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.Command</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.CommandArgument</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.CommandName</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.Enabled</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.GenerateEmptyAlternateText</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.GetPostBackOptions</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.LoadPostData</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.OnClick</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.OnClientClick</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.OnCommand</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.OnPreRender</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.PostBackUrl</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.RaisePostBackEvent</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.TagKey</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.Text</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.ImageButton.ValidationGroup</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>ImageButton.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ImageButton.(case list)</code> | ✅ 一致 |
| <code>ImageButton.(default)</code> | ✅ 一致 |
| <code>ImageButton.AccessKey=empty</code> | ✅ 一致 |
| <code>ImageButton.AccessKey=japanese</code> | ✅ 一致 |
| <code>ImageButton.AccessKey=k</code> | ✅ 一致 |
| <code>ImageButton.AccessKey=markup</code> | ✅ 一致 |
| <code>ImageButton.AccessKey=text</code> | ✅ 一致 |
| <code>ImageButton.AlternateText=empty</code> | ✅ 一致 |
| <code>ImageButton.AlternateText=japanese</code> | ✅ 一致 |
| <code>ImageButton.AlternateText=markup</code> | ✅ 一致 |
| <code>ImageButton.AlternateText=text</code> | ✅ 一致 |
| <code>ImageButton.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ImageButton.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ImageButton.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ImageButton.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ImageButton.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>ImageButton.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>ImageButton.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>ImageButton.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>ImageButton.BackColor=#3366CC</code> | ✅ 一致 |
| <code>ImageButton.BackColor=Red</code> | ✅ 一致 |
| <code>ImageButton.BackColor=Transparent</code> | ✅ 一致 |
| <code>ImageButton.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>ImageButton.BorderColor=Red</code> | ✅ 一致 |
| <code>ImageButton.BorderColor=Transparent</code> | ✅ 一致 |
| <code>ImageButton.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>ImageButton.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>ImageButton.BorderStyle=Double</code> | ✅ 一致 |
| <code>ImageButton.BorderStyle=Groove</code> | ✅ 一致 |
| <code>ImageButton.BorderStyle=Inset</code> | ✅ 一致 |
| <code>ImageButton.BorderStyle=None</code> | ✅ 一致 |
| <code>ImageButton.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>ImageButton.BorderStyle=Outset</code> | ✅ 一致 |
| <code>ImageButton.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>ImageButton.BorderStyle=Solid</code> | ✅ 一致 |
| <code>ImageButton.BorderWidth=120px</code> | ✅ 一致 |
| <code>ImageButton.BorderWidth=2em</code> | ✅ 一致 |
| <code>ImageButton.BorderWidth=50%</code> | ✅ 一致 |
| <code>ImageButton.CausesValidation=false</code> | ✅ 一致 |
| <code>ImageButton.CausesValidation=true</code> | ✅ 一致 |
| <code>ImageButton.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ImageButton.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ImageButton.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ImageButton.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ImageButton.CommandArgument=empty</code> | ✅ 一致 |
| <code>ImageButton.CommandArgument=japanese</code> | ✅ 一致 |
| <code>ImageButton.CommandArgument=markup</code> | ✅ 一致 |
| <code>ImageButton.CommandArgument=text</code> | ✅ 一致 |
| <code>ImageButton.CommandName=empty</code> | ✅ 一致 |
| <code>ImageButton.CommandName=japanese</code> | ✅ 一致 |
| <code>ImageButton.CommandName=markup</code> | ✅ 一致 |
| <code>ImageButton.CommandName=text</code> | ✅ 一致 |
| <code>ImageButton.CommandName=text+CommandArgument=markup</code> | ✅ 一致 |
| <code>ImageButton.CssClass=empty</code> | ✅ 一致 |
| <code>ImageButton.CssClass=japanese</code> | ✅ 一致 |
| <code>ImageButton.CssClass=markup</code> | ✅ 一致 |
| <code>ImageButton.CssClass=text</code> | ✅ 一致 |
| <code>ImageButton.DescriptionUrl=empty</code> | ✅ 一致 |
| <code>ImageButton.DescriptionUrl=japanese</code> | ✅ 一致 |
| <code>ImageButton.DescriptionUrl=markup</code> | ✅ 一致 |
| <code>ImageButton.DescriptionUrl=text</code> | ✅ 一致 |
| <code>ImageButton.EnableTheming=false</code> | ✅ 一致 |
| <code>ImageButton.EnableTheming=true</code> | ✅ 一致 |
| <code>ImageButton.EnableViewState=false</code> | ✅ 一致 |
| <code>ImageButton.EnableViewState=true</code> | ✅ 一致 |
| <code>ImageButton.Enabled=false</code> | ✅ 一致 |
| <code>ImageButton.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>ImageButton.Enabled=true</code> | ✅ 一致 |
| <code>ImageButton.Font.Bold=false</code> | ✅ 一致 |
| <code>ImageButton.Font.Bold=true</code> | ✅ 一致 |
| <code>ImageButton.Font.Italic=false</code> | ✅ 一致 |
| <code>ImageButton.Font.Italic=true</code> | ✅ 一致 |
| <code>ImageButton.Font.Name=empty</code> | ✅ 一致 |
| <code>ImageButton.Font.Name=japanese</code> | ✅ 一致 |
| <code>ImageButton.Font.Name=markup</code> | ✅ 一致 |
| <code>ImageButton.Font.Name=text</code> | ✅ 一致 |
| <code>ImageButton.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>ImageButton.Font.Overline=false</code> | ✅ 一致 |
| <code>ImageButton.Font.Overline=true</code> | ✅ 一致 |
| <code>ImageButton.Font.Size=12pt</code> | ✅ 一致 |
| <code>ImageButton.Font.Size=150%</code> | ✅ 一致 |
| <code>ImageButton.Font.Size=Large</code> | ✅ 一致 |
| <code>ImageButton.Font.Strikeout=false</code> | ✅ 一致 |
| <code>ImageButton.Font.Strikeout=true</code> | ✅ 一致 |
| <code>ImageButton.Font.Underline=false</code> | ✅ 一致 |
| <code>ImageButton.Font.Underline=true</code> | ✅ 一致 |
| <code>ImageButton.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>ImageButton.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>ImageButton.ForeColor=Red</code> | ✅ 一致 |
| <code>ImageButton.ForeColor=Transparent</code> | ✅ 一致 |
| <code>ImageButton.GenerateEmptyAlternateText=false</code> | ✅ 一致 |
| <code>ImageButton.GenerateEmptyAlternateText=true</code> | ✅ 一致 |
| <code>ImageButton.Height=120px</code> | ✅ 一致 |
| <code>ImageButton.Height=2em</code> | ✅ 一致 |
| <code>ImageButton.Height=50%</code> | ✅ 一致 |
| <code>ImageButton.ID=empty</code> | ✅ 一致 |
| <code>ImageButton.ID=japanese</code> | ✅ 一致 |
| <code>ImageButton.ID=markup</code> | ✅ 一致 |
| <code>ImageButton.ID=text</code> | ✅ 一致 |
| <code>ImageButton.ImageAlign=AbsBottom</code> | ✅ 一致 |
| <code>ImageButton.ImageAlign=AbsMiddle</code> | ✅ 一致 |
| <code>ImageButton.ImageAlign=Baseline</code> | ✅ 一致 |
| <code>ImageButton.ImageAlign=Bottom</code> | ✅ 一致 |
| <code>ImageButton.ImageAlign=Left</code> | ✅ 一致 |
| <code>ImageButton.ImageAlign=Middle</code> | ✅ 一致 |
| <code>ImageButton.ImageAlign=NotSet</code> | ✅ 一致 |
| <code>ImageButton.ImageAlign=Right</code> | ✅ 一致 |
| <code>ImageButton.ImageAlign=TextTop</code> | ✅ 一致 |
| <code>ImageButton.ImageAlign=Top</code> | ✅ 一致 |
| <code>ImageButton.ImageUrl=absolute</code> | ✅ 一致 |
| <code>ImageButton.ImageUrl=app-relative</code> | ✅ 一致 |
| <code>ImageButton.ImageUrl=app-relative+AlternateText=text</code> | ✅ 一致 |
| <code>ImageButton.ImageUrl=app-relative+EnableViewState=false</code> | ✅ 一致 |
| <code>ImageButton.ImageUrl=app-relative+Enabled=false</code> | ✅ 一致 |
| <code>ImageButton.ImageUrl=app-relative+Visible=false</code> | ✅ 一致 |
| <code>ImageButton.ImageUrl=empty</code> | ✅ 一致 |
| <code>ImageButton.ImageUrl=japanese</code> | ✅ 一致 |
| <code>ImageButton.ImageUrl=markup</code> | ✅ 一致 |
| <code>ImageButton.ImageUrl=text</code> | ✅ 一致 |
| <code>ImageButton.OnClientClick=confirm</code> | ✅ 一致 |
| <code>ImageButton.OnClientClick=confirm+CausesValidation=false</code> | ✅ 一致 |
| <code>ImageButton.OnClientClick=confirm+Enabled=false</code> | ✅ 一致 |
| <code>ImageButton.OnClientClick=confirm+ValidationGroup=text</code> | ✅ 一致 |
| <code>ImageButton.OnClientClick=empty</code> | ✅ 一致 |
| <code>ImageButton.OnClientClick=japanese</code> | ✅ 一致 |
| <code>ImageButton.OnClientClick=markup</code> | ✅ 一致 |
| <code>ImageButton.OnClientClick=text</code> | ✅ 一致 |
| <code>ImageButton.PostBackUrl=absolute</code> | ✅ 一致 |
| <code>ImageButton.PostBackUrl=app-relative</code> | ✅ 一致 |
| <code>ImageButton.PostBackUrl=app-relative+CausesValidation=false</code> | ✅ 一致 |
| <code>ImageButton.PostBackUrl=app-relative+Enabled=false</code> | ✅ 一致 |
| <code>ImageButton.PostBackUrl=app-relative+OnClientClick=confirm</code> | ✅ 一致 |
| <code>ImageButton.PostBackUrl=app-relative+ValidationGroup=text</code> | ✅ 一致 |
| <code>ImageButton.PostBackUrl=empty</code> | ✅ 一致 |
| <code>ImageButton.PostBackUrl=japanese</code> | ✅ 一致 |
| <code>ImageButton.PostBackUrl=markup</code> | ✅ 一致 |
| <code>ImageButton.PostBackUrl=text</code> | ✅ 一致 |
| <code>ImageButton.SkinID=empty</code> | ✅ 一致 |
| <code>ImageButton.SkinID=japanese</code> | ✅ 一致 |
| <code>ImageButton.SkinID=markup</code> | ✅ 一致 |
| <code>ImageButton.SkinID=text</code> | ✅ 一致 |
| <code>ImageButton.Style[color]=1em</code> | ✅ 一致 |
| <code>ImageButton.Style[color]=red</code> | ✅ 一致 |
| <code>ImageButton.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>ImageButton.Style[margin-left]=red</code> | ✅ 一致 |
| <code>ImageButton.TabIndex=-1</code> | ✅ 一致 |
| <code>ImageButton.TabIndex=0</code> | ✅ 一致 |
| <code>ImageButton.TabIndex=3</code> | ✅ 一致 |
| <code>ImageButton.ToolTip=empty</code> | ✅ 一致 |
| <code>ImageButton.ToolTip=japanese</code> | ✅ 一致 |
| <code>ImageButton.ToolTip=markup</code> | ✅ 一致 |
| <code>ImageButton.ToolTip=text</code> | ✅ 一致 |
| <code>ImageButton.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ImageButton.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ImageButton.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ImageButton.ValidationGroup=empty</code> | ✅ 一致 |
| <code>ImageButton.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>ImageButton.ValidationGroup=markup</code> | ✅ 一致 |
| <code>ImageButton.ValidationGroup=text</code> | ✅ 一致 |
| <code>ImageButton.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ImageButton.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ImageButton.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ImageButton.Visible=false</code> | ✅ 一致 |
| <code>ImageButton.Visible=true</code> | ✅ 一致 |
| <code>ImageButton.Width=120px</code> | ✅ 一致 |
| <code>ImageButton.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>ImageButton.Width=2em</code> | ✅ 一致 |
| <code>ImageButton.Width=50%</code> | ✅ 一致 |
| <code>ImageButton[naming container].(default)</code> | ✅ 一致 |
| <code>ImageButton[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ImageButton[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ImageButton[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ImageButton[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ImageButton[naming container].ID=text</code> | ✅ 一致 |
| <code>ImageButton[naming container].PostBackUrl=app-relative</code> | ✅ 一致 |
| <code>ImageButton[naming container].ValidationGroup=text</code> | ✅ 一致 |
| <code>ImageButton[themed].(default)</code> | ✅ 一致 |
| <code>ImageButton[themed].CssClass=text</code> | ✅ 一致 |
| <code>ImageButton[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>ImageButton[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ImageButton [render][databind][dynamic] first request | ✅ 一致 |
| ImageButton [postdata][postbackevent][command][viewstate] an image clicked: its coordinates, Click, then Command | ✅ 一致 |
| ImageButton [clientclick] OnClientClick returns false: no request | ✅ 一致 |
| ImageButton [causesvalidation][postbackevent] validating g1 (empty) | ✅ 一致 |
| ImageButton [command][databind] Command bubbled from a repeater item | ✅ 一致 |
| ImageButton [defaultbutton] Enter in a panel whose DefaultButton is the image | ✅ 一致 |
| ImageButton [dynamic][postbackevent] a dynamically created image button | ✅ 一致 |
| ImageButton [visible][viewstate][postbackevent] the image shown before raises Click | ✅ 一致 |
| ImageButton [requestvalidation] markup posted as a coordinate | ✅ 一致 |
| ImageButton [crosspage] cross-page postback (PostBackUrl) | ✅ 一致 |

</details>
