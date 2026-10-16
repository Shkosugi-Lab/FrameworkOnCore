# TableFooterRow パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:53
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 153 | 153 | 0 | 0 |
| ② HTML | 243 | 243 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 162 件中 162 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 32 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 118 | 1 | ✅ |
| ViewState の保存と復元 | 11 | 13 | ❌ 1 件 |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>TableFooterRow.Api.Control.#ctor</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Adapter</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ClientID</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Context</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Controls</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.DataBind</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Dispose</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Disposed</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Events</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.FindControl</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Focus</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.HasControls</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ID</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Init</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Load</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.OnInit</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Page</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Parent</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.PreRender</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Render</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Site</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.SkinID</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Unload</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ViewState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>TableFooterRow.Api.Control.Visible</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>TableFooterRow.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.TableFooterRow.#ctor</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.TableRow.#ctor</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.TableRow.Cells</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.TableRow.CreateControlCollection</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.TableRow.CreateControlStyle</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.TableRow.HorizontalAlign</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.TableRow.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.TableRow.TableSection</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.TableRow.VerticalAlign</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>TableFooterRow.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>TableFooterRow.(case list)</code> | ✅ 一致 |
| <code>TableFooterRow.(default)</code> | ✅ 一致 |
| <code>TableFooterRow.AccessKey=empty</code> | ✅ 一致 |
| <code>TableFooterRow.AccessKey=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.AccessKey=k</code> | ✅ 一致 |
| <code>TableFooterRow.AccessKey=markup</code> | ✅ 一致 |
| <code>TableFooterRow.AccessKey=text</code> | ✅ 一致 |
| <code>TableFooterRow.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>TableFooterRow.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>TableFooterRow.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>TableFooterRow.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>TableFooterRow.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>TableFooterRow.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>TableFooterRow.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>TableFooterRow.BackColor=#3366CC</code> | ✅ 一致 |
| <code>TableFooterRow.BackColor=Red</code> | ✅ 一致 |
| <code>TableFooterRow.BackColor=Transparent</code> | ✅ 一致 |
| <code>TableFooterRow.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>TableFooterRow.BorderColor=Red</code> | ✅ 一致 |
| <code>TableFooterRow.BorderColor=Transparent</code> | ✅ 一致 |
| <code>TableFooterRow.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>TableFooterRow.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>TableFooterRow.BorderStyle=Double</code> | ✅ 一致 |
| <code>TableFooterRow.BorderStyle=Groove</code> | ✅ 一致 |
| <code>TableFooterRow.BorderStyle=Inset</code> | ✅ 一致 |
| <code>TableFooterRow.BorderStyle=None</code> | ✅ 一致 |
| <code>TableFooterRow.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>TableFooterRow.BorderStyle=Outset</code> | ✅ 一致 |
| <code>TableFooterRow.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>TableFooterRow.BorderStyle=Solid</code> | ✅ 一致 |
| <code>TableFooterRow.BorderWidth=120px</code> | ✅ 一致 |
| <code>TableFooterRow.BorderWidth=2em</code> | ✅ 一致 |
| <code>TableFooterRow.BorderWidth=50%</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].AccessKey=empty</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].AccessKey=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].AccessKey=markup</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].AccessKey=text</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].AssociatedHeaderCellID=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BackColor=#3366CC</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BackColor=Red</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BackColor=Transparent</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderColor=#3366CC</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderColor=Red</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderColor=Transparent</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderStyle=Dashed</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderStyle=Dotted</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderStyle=Double</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderStyle=Groove</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderStyle=Inset</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderStyle=None</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderStyle=NotSet</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderStyle=Outset</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderStyle=Ridge</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderStyle=Solid</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderWidth=120px</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderWidth=2em</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].BorderWidth=50%</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ClientIDMode=Static</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ColumnSpan=-1</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ColumnSpan=0</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ColumnSpan=3</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].CssClass=empty</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].CssClass=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].CssClass=markup</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].CssClass=text</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].EnableTheming=false</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].EnableTheming=true</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].EnableViewState=false</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].EnableViewState=true</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Enabled=false</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Enabled=true</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Bold=false</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Bold=true</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Italic=false</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Italic=true</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Name=empty</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Name=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Name=markup</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Name=text</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Overline=false</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Overline=true</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Size=12pt</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Size=150%</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Size=Large</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Strikeout=false</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Strikeout=true</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Underline=false</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Font.Underline=true</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ForeColor=#3366CC</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ForeColor=Red</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ForeColor=Transparent</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Height=120px</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Height=2em</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Height=50%</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].HorizontalAlign=Center</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].HorizontalAlign=Left</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].HorizontalAlign=Right</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ID=empty</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ID=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ID=markup</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ID=text</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].RowSpan=-1</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].RowSpan=0</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].RowSpan=3</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].SkinID=empty</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].SkinID=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].SkinID=markup</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].SkinID=text</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].TabIndex=-1</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].TabIndex=0</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].TabIndex=3</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Text=empty</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Text=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Text=markup</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Text=text</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ToolTip=empty</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ToolTip=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ToolTip=markup</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ToolTip=text</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].VerticalAlign=Bottom</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].VerticalAlign=Middle</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].VerticalAlign=Top</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Visible=false</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Visible=true</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Width=120px</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Width=2em</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Width=50%</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Wrap=false</code> | ✅ 一致 |
| <code>TableFooterRow.Cells[1].Wrap=true</code> | ✅ 一致 |
| <code>TableFooterRow.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TableFooterRow.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TableFooterRow.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TableFooterRow.ClientIDMode=Static</code> | ✅ 一致 |
| <code>TableFooterRow.CssClass=empty</code> | ✅ 一致 |
| <code>TableFooterRow.CssClass=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.CssClass=markup</code> | ✅ 一致 |
| <code>TableFooterRow.CssClass=text</code> | ✅ 一致 |
| <code>TableFooterRow.CssClass=text+EnableViewState=false</code> | ✅ 一致 |
| <code>TableFooterRow.CssClass=text+Visible=false</code> | ✅ 一致 |
| <code>TableFooterRow.EnableTheming=false</code> | ✅ 一致 |
| <code>TableFooterRow.EnableTheming=true</code> | ✅ 一致 |
| <code>TableFooterRow.EnableViewState=false</code> | ✅ 一致 |
| <code>TableFooterRow.EnableViewState=true</code> | ✅ 一致 |
| <code>TableFooterRow.Enabled=false</code> | ✅ 一致 |
| <code>TableFooterRow.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>TableFooterRow.Enabled=true</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Bold=false</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Bold=true</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Italic=false</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Italic=true</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Name=empty</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Name=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Name=markup</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Name=text</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Overline=false</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Overline=true</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Size=12pt</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Size=150%</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Size=Large</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Strikeout=false</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Strikeout=true</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Underline=false</code> | ✅ 一致 |
| <code>TableFooterRow.Font.Underline=true</code> | ✅ 一致 |
| <code>TableFooterRow.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>TableFooterRow.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>TableFooterRow.ForeColor=Red</code> | ✅ 一致 |
| <code>TableFooterRow.ForeColor=Transparent</code> | ✅ 一致 |
| <code>TableFooterRow.Height=120px</code> | ✅ 一致 |
| <code>TableFooterRow.Height=2em</code> | ✅ 一致 |
| <code>TableFooterRow.Height=50%</code> | ✅ 一致 |
| <code>TableFooterRow.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>TableFooterRow.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>TableFooterRow.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>TableFooterRow.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>TableFooterRow.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>TableFooterRow.ID=empty</code> | ✅ 一致 |
| <code>TableFooterRow.ID=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.ID=markup</code> | ✅ 一致 |
| <code>TableFooterRow.ID=text</code> | ✅ 一致 |
| <code>TableFooterRow.SkinID=empty</code> | ✅ 一致 |
| <code>TableFooterRow.SkinID=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.SkinID=markup</code> | ✅ 一致 |
| <code>TableFooterRow.SkinID=text</code> | ✅ 一致 |
| <code>TableFooterRow.Style[color]=1em</code> | ✅ 一致 |
| <code>TableFooterRow.Style[color]=red</code> | ✅ 一致 |
| <code>TableFooterRow.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>TableFooterRow.Style[margin-left]=red</code> | ✅ 一致 |
| <code>TableFooterRow.TabIndex=-1</code> | ✅ 一致 |
| <code>TableFooterRow.TabIndex=0</code> | ✅ 一致 |
| <code>TableFooterRow.TabIndex=3</code> | ✅ 一致 |
| <code>TableFooterRow.TableSection=TableBody</code> | ✅ 一致 |
| <code>TableFooterRow.TableSection=TableBody+HorizontalAlign=Center</code> | ✅ 一致 |
| <code>TableFooterRow.TableSection=TableFooter</code> | ✅ 一致 |
| <code>TableFooterRow.TableSection=TableHeader</code> | ✅ 一致 |
| <code>TableFooterRow.ToolTip=empty</code> | ✅ 一致 |
| <code>TableFooterRow.ToolTip=japanese</code> | ✅ 一致 |
| <code>TableFooterRow.ToolTip=markup</code> | ✅ 一致 |
| <code>TableFooterRow.ToolTip=text</code> | ✅ 一致 |
| <code>TableFooterRow.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>TableFooterRow.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>TableFooterRow.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>TableFooterRow.VerticalAlign=Bottom</code> | ✅ 一致 |
| <code>TableFooterRow.VerticalAlign=Middle</code> | ✅ 一致 |
| <code>TableFooterRow.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>TableFooterRow.VerticalAlign=Top</code> | ✅ 一致 |
| <code>TableFooterRow.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>TableFooterRow.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>TableFooterRow.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>TableFooterRow.Visible=false</code> | ✅ 一致 |
| <code>TableFooterRow.Visible=true</code> | ✅ 一致 |
| <code>TableFooterRow.Width=120px</code> | ✅ 一致 |
| <code>TableFooterRow.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>TableFooterRow.Width=2em</code> | ✅ 一致 |
| <code>TableFooterRow.Width=50%</code> | ✅ 一致 |
| <code>TableFooterRow[naming container].(default)</code> | ✅ 一致 |
| <code>TableFooterRow[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TableFooterRow[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TableFooterRow[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TableFooterRow[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>TableFooterRow[naming container].ID=text</code> | ✅ 一致 |
| <code>TableFooterRow[themed].(default)</code> | ✅ 一致 |
| <code>TableFooterRow[themed].CssClass=text</code> | ✅ 一致 |
| <code>TableFooterRow[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>TableFooterRow[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| TableFooterRow [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| TableFooterRow [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| TableFooterRow [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| TableFooterRow [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| TableFooterRow [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| TableFooterRow [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| TableFooterRow [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| TableFooterRow [viewstate] a calendar&#39;s next month | ✅ 一致 |
| TableFooterRow [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| TableFooterRow [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| TableFooterRow [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| TableFooterRow [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| TableFooterRow [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| TableFooterRow [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
