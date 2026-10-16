# TableRow パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 03:15
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 152 | 152 | 0 | 0 |
| ② HTML | 244 | 244 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 161 件中 161 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 32 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 119 | 1 | ✅ |
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
| <code>TableRow.Api.Control.#ctor</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Adapter</code> | ✅ 一致 |
| <code>TableRow.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>TableRow.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>TableRow.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>TableRow.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>TableRow.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>TableRow.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ClientID</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Context</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Controls</code> | ✅ 一致 |
| <code>TableRow.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>TableRow.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>TableRow.Api.Control.DataBind</code> | ✅ 一致 |
| <code>TableRow.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>TableRow.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>TableRow.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>TableRow.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>TableRow.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Dispose</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Disposed</code> | ✅ 一致 |
| <code>TableRow.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>TableRow.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>TableRow.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>TableRow.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Events</code> | ✅ 一致 |
| <code>TableRow.Api.Control.FindControl</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Focus</code> | ✅ 一致 |
| <code>TableRow.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>TableRow.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>TableRow.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.HasControls</code> | ✅ 一致 |
| <code>TableRow.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ID</code> | ✅ 一致 |
| <code>TableRow.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Init</code> | ✅ 一致 |
| <code>TableRow.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>TableRow.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>TableRow.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Load</code> | ✅ 一致 |
| <code>TableRow.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>TableRow.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>TableRow.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>TableRow.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>TableRow.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>TableRow.Api.Control.OnInit</code> | ✅ 一致 |
| <code>TableRow.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>TableRow.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>TableRow.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>TableRow.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Page</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Parent</code> | ✅ 一致 |
| <code>TableRow.Api.Control.PreRender</code> | ✅ 一致 |
| <code>TableRow.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>TableRow.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Render</code> | ✅ 一致 |
| <code>TableRow.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>TableRow.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>TableRow.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>TableRow.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>TableRow.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Site</code> | ✅ 一致 |
| <code>TableRow.Api.Control.SkinID</code> | ✅ 一致 |
| <code>TableRow.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>TableRow.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>TableRow.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Unload</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ViewState</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>TableRow.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>TableRow.Api.Control.Visible</code> | ✅ 一致 |
| <code>TableRow.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>TableRow.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>TableRow.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>TableRow.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>TableRow.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>TableRow.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>TableRow.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>TableRow.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>TableRow.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>TableRow.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>TableRow.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>TableRow.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>TableRow.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.TableRow.#ctor</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.TableRow.Cells</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.TableRow.CreateControlCollection</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.TableRow.CreateControlStyle</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.TableRow.HorizontalAlign</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.TableRow.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.TableRow.TableSection</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.TableRow.VerticalAlign</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>TableRow.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>TableRow.(case list)</code> | ✅ 一致 |
| <code>TableRow.(default)</code> | ✅ 一致 |
| <code>TableRow.AccessKey=empty</code> | ✅ 一致 |
| <code>TableRow.AccessKey=japanese</code> | ✅ 一致 |
| <code>TableRow.AccessKey=k</code> | ✅ 一致 |
| <code>TableRow.AccessKey=markup</code> | ✅ 一致 |
| <code>TableRow.AccessKey=text</code> | ✅ 一致 |
| <code>TableRow.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>TableRow.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>TableRow.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>TableRow.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>TableRow.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>TableRow.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>TableRow.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>TableRow.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>TableRow.BackColor=#3366CC</code> | ✅ 一致 |
| <code>TableRow.BackColor=Red</code> | ✅ 一致 |
| <code>TableRow.BackColor=Transparent</code> | ✅ 一致 |
| <code>TableRow.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>TableRow.BorderColor=Red</code> | ✅ 一致 |
| <code>TableRow.BorderColor=Transparent</code> | ✅ 一致 |
| <code>TableRow.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>TableRow.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>TableRow.BorderStyle=Double</code> | ✅ 一致 |
| <code>TableRow.BorderStyle=Groove</code> | ✅ 一致 |
| <code>TableRow.BorderStyle=Inset</code> | ✅ 一致 |
| <code>TableRow.BorderStyle=None</code> | ✅ 一致 |
| <code>TableRow.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>TableRow.BorderStyle=Outset</code> | ✅ 一致 |
| <code>TableRow.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>TableRow.BorderStyle=Solid</code> | ✅ 一致 |
| <code>TableRow.BorderWidth=120px</code> | ✅ 一致 |
| <code>TableRow.BorderWidth=2em</code> | ✅ 一致 |
| <code>TableRow.BorderWidth=50%</code> | ✅ 一致 |
| <code>TableRow.Cells[1].AccessKey=empty</code> | ✅ 一致 |
| <code>TableRow.Cells[1].AccessKey=japanese</code> | ✅ 一致 |
| <code>TableRow.Cells[1].AccessKey=markup</code> | ✅ 一致 |
| <code>TableRow.Cells[1].AccessKey=text</code> | ✅ 一致 |
| <code>TableRow.Cells[1].AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>TableRow.Cells[1].AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>TableRow.Cells[1].AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>TableRow.Cells[1].AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>TableRow.Cells[1].AssociatedHeaderCellID=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BackColor=#3366CC</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BackColor=Red</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BackColor=Transparent</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderColor=#3366CC</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderColor=Red</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderColor=Transparent</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderStyle=Dashed</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderStyle=Dotted</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderStyle=Double</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderStyle=Groove</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderStyle=Inset</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderStyle=None</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderStyle=NotSet</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderStyle=Outset</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderStyle=Ridge</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderStyle=Solid</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderWidth=120px</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderWidth=2em</code> | ✅ 一致 |
| <code>TableRow.Cells[1].BorderWidth=50%</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ClientIDMode=Static</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ColumnSpan=-1</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ColumnSpan=0</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ColumnSpan=3</code> | ✅ 一致 |
| <code>TableRow.Cells[1].CssClass=empty</code> | ✅ 一致 |
| <code>TableRow.Cells[1].CssClass=japanese</code> | ✅ 一致 |
| <code>TableRow.Cells[1].CssClass=markup</code> | ✅ 一致 |
| <code>TableRow.Cells[1].CssClass=text</code> | ✅ 一致 |
| <code>TableRow.Cells[1].EnableTheming=false</code> | ✅ 一致 |
| <code>TableRow.Cells[1].EnableTheming=true</code> | ✅ 一致 |
| <code>TableRow.Cells[1].EnableViewState=false</code> | ✅ 一致 |
| <code>TableRow.Cells[1].EnableViewState=true</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Enabled=false</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Enabled=true</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Bold=false</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Bold=true</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Italic=false</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Italic=true</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Name=empty</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Name=japanese</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Name=markup</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Name=text</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Overline=false</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Overline=true</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Size=12pt</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Size=150%</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Size=Large</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Strikeout=false</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Strikeout=true</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Underline=false</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Font.Underline=true</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ForeColor=#3366CC</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ForeColor=Red</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ForeColor=Transparent</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Height=120px</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Height=2em</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Height=50%</code> | ✅ 一致 |
| <code>TableRow.Cells[1].HorizontalAlign=Center</code> | ✅ 一致 |
| <code>TableRow.Cells[1].HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>TableRow.Cells[1].HorizontalAlign=Left</code> | ✅ 一致 |
| <code>TableRow.Cells[1].HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>TableRow.Cells[1].HorizontalAlign=Right</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ID=empty</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ID=japanese</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ID=markup</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ID=text</code> | ✅ 一致 |
| <code>TableRow.Cells[1].RowSpan=-1</code> | ✅ 一致 |
| <code>TableRow.Cells[1].RowSpan=0</code> | ✅ 一致 |
| <code>TableRow.Cells[1].RowSpan=3</code> | ✅ 一致 |
| <code>TableRow.Cells[1].SkinID=empty</code> | ✅ 一致 |
| <code>TableRow.Cells[1].SkinID=japanese</code> | ✅ 一致 |
| <code>TableRow.Cells[1].SkinID=markup</code> | ✅ 一致 |
| <code>TableRow.Cells[1].SkinID=text</code> | ✅ 一致 |
| <code>TableRow.Cells[1].TabIndex=-1</code> | ✅ 一致 |
| <code>TableRow.Cells[1].TabIndex=0</code> | ✅ 一致 |
| <code>TableRow.Cells[1].TabIndex=3</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Text=empty</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Text=japanese</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Text=markup</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Text=text</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ToolTip=empty</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ToolTip=japanese</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ToolTip=markup</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ToolTip=text</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>TableRow.Cells[1].VerticalAlign=Bottom</code> | ✅ 一致 |
| <code>TableRow.Cells[1].VerticalAlign=Middle</code> | ✅ 一致 |
| <code>TableRow.Cells[1].VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>TableRow.Cells[1].VerticalAlign=Top</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>TableRow.Cells[1].ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Visible=false</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Visible=true</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Width=120px</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Width=2em</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Width=50%</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Wrap=false</code> | ✅ 一致 |
| <code>TableRow.Cells[1].Wrap=true</code> | ✅ 一致 |
| <code>TableRow.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TableRow.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TableRow.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TableRow.ClientIDMode=Static</code> | ✅ 一致 |
| <code>TableRow.CssClass=empty</code> | ✅ 一致 |
| <code>TableRow.CssClass=japanese</code> | ✅ 一致 |
| <code>TableRow.CssClass=markup</code> | ✅ 一致 |
| <code>TableRow.CssClass=text</code> | ✅ 一致 |
| <code>TableRow.CssClass=text+EnableViewState=false</code> | ✅ 一致 |
| <code>TableRow.CssClass=text+Visible=false</code> | ✅ 一致 |
| <code>TableRow.EnableTheming=false</code> | ✅ 一致 |
| <code>TableRow.EnableTheming=true</code> | ✅ 一致 |
| <code>TableRow.EnableViewState=false</code> | ✅ 一致 |
| <code>TableRow.EnableViewState=true</code> | ✅ 一致 |
| <code>TableRow.Enabled=false</code> | ✅ 一致 |
| <code>TableRow.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>TableRow.Enabled=true</code> | ✅ 一致 |
| <code>TableRow.Font.Bold=false</code> | ✅ 一致 |
| <code>TableRow.Font.Bold=true</code> | ✅ 一致 |
| <code>TableRow.Font.Italic=false</code> | ✅ 一致 |
| <code>TableRow.Font.Italic=true</code> | ✅ 一致 |
| <code>TableRow.Font.Name=empty</code> | ✅ 一致 |
| <code>TableRow.Font.Name=japanese</code> | ✅ 一致 |
| <code>TableRow.Font.Name=markup</code> | ✅ 一致 |
| <code>TableRow.Font.Name=text</code> | ✅ 一致 |
| <code>TableRow.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TableRow.Font.Overline=false</code> | ✅ 一致 |
| <code>TableRow.Font.Overline=true</code> | ✅ 一致 |
| <code>TableRow.Font.Size=12pt</code> | ✅ 一致 |
| <code>TableRow.Font.Size=150%</code> | ✅ 一致 |
| <code>TableRow.Font.Size=Large</code> | ✅ 一致 |
| <code>TableRow.Font.Strikeout=false</code> | ✅ 一致 |
| <code>TableRow.Font.Strikeout=true</code> | ✅ 一致 |
| <code>TableRow.Font.Underline=false</code> | ✅ 一致 |
| <code>TableRow.Font.Underline=true</code> | ✅ 一致 |
| <code>TableRow.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>TableRow.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>TableRow.ForeColor=Red</code> | ✅ 一致 |
| <code>TableRow.ForeColor=Transparent</code> | ✅ 一致 |
| <code>TableRow.Height=120px</code> | ✅ 一致 |
| <code>TableRow.Height=2em</code> | ✅ 一致 |
| <code>TableRow.Height=50%</code> | ✅ 一致 |
| <code>TableRow.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>TableRow.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>TableRow.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>TableRow.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>TableRow.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>TableRow.ID=empty</code> | ✅ 一致 |
| <code>TableRow.ID=japanese</code> | ✅ 一致 |
| <code>TableRow.ID=markup</code> | ✅ 一致 |
| <code>TableRow.ID=text</code> | ✅ 一致 |
| <code>TableRow.SkinID=empty</code> | ✅ 一致 |
| <code>TableRow.SkinID=japanese</code> | ✅ 一致 |
| <code>TableRow.SkinID=markup</code> | ✅ 一致 |
| <code>TableRow.SkinID=text</code> | ✅ 一致 |
| <code>TableRow.Style[color]=1em</code> | ✅ 一致 |
| <code>TableRow.Style[color]=red</code> | ✅ 一致 |
| <code>TableRow.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>TableRow.Style[margin-left]=red</code> | ✅ 一致 |
| <code>TableRow.TabIndex=-1</code> | ✅ 一致 |
| <code>TableRow.TabIndex=0</code> | ✅ 一致 |
| <code>TableRow.TabIndex=3</code> | ✅ 一致 |
| <code>TableRow.TableSection=TableBody</code> | ✅ 一致 |
| <code>TableRow.TableSection=TableFooter</code> | ✅ 一致 |
| <code>TableRow.TableSection=TableHeader</code> | ✅ 一致 |
| <code>TableRow.TableSection=TableHeader+HorizontalAlign=Center</code> | ✅ 一致 |
| <code>TableRow.ToolTip=empty</code> | ✅ 一致 |
| <code>TableRow.ToolTip=japanese</code> | ✅ 一致 |
| <code>TableRow.ToolTip=markup</code> | ✅ 一致 |
| <code>TableRow.ToolTip=text</code> | ✅ 一致 |
| <code>TableRow.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>TableRow.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>TableRow.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>TableRow.VerticalAlign=Bottom</code> | ✅ 一致 |
| <code>TableRow.VerticalAlign=Middle</code> | ✅ 一致 |
| <code>TableRow.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>TableRow.VerticalAlign=Top</code> | ✅ 一致 |
| <code>TableRow.VerticalAlign=Top+Height=2em</code> | ✅ 一致 |
| <code>TableRow.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>TableRow.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>TableRow.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>TableRow.Visible=false</code> | ✅ 一致 |
| <code>TableRow.Visible=true</code> | ✅ 一致 |
| <code>TableRow.Width=120px</code> | ✅ 一致 |
| <code>TableRow.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>TableRow.Width=2em</code> | ✅ 一致 |
| <code>TableRow.Width=50%</code> | ✅ 一致 |
| <code>TableRow[naming container].(default)</code> | ✅ 一致 |
| <code>TableRow[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TableRow[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TableRow[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TableRow[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>TableRow[naming container].ID=text</code> | ✅ 一致 |
| <code>TableRow[themed].(default)</code> | ✅ 一致 |
| <code>TableRow[themed].CssClass=text</code> | ✅ 一致 |
| <code>TableRow[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>TableRow[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| TableRow [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| TableRow [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| TableRow [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| TableRow [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| TableRow [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| TableRow [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| TableRow [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| TableRow [viewstate] a calendar&#39;s next month | ✅ 一致 |
| TableRow [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| TableRow [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| TableRow [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| TableRow [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| TableRow [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| TableRow [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
