# TableCell パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:52
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 157 | 157 | 0 | 0 |
| ② HTML | 144 | 144 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 166 件中 166 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 36 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 132 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 13 | ❌ 1 件 |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>TableCell.Api.Control.#ctor</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Adapter</code> | ✅ 一致 |
| <code>TableCell.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>TableCell.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>TableCell.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>TableCell.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>TableCell.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>TableCell.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ClientID</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Context</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Controls</code> | ✅ 一致 |
| <code>TableCell.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>TableCell.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>TableCell.Api.Control.DataBind</code> | ✅ 一致 |
| <code>TableCell.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>TableCell.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>TableCell.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>TableCell.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>TableCell.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Dispose</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Disposed</code> | ✅ 一致 |
| <code>TableCell.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>TableCell.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>TableCell.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>TableCell.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Events</code> | ✅ 一致 |
| <code>TableCell.Api.Control.FindControl</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Focus</code> | ✅ 一致 |
| <code>TableCell.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>TableCell.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>TableCell.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.HasControls</code> | ✅ 一致 |
| <code>TableCell.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ID</code> | ✅ 一致 |
| <code>TableCell.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Init</code> | ✅ 一致 |
| <code>TableCell.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>TableCell.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>TableCell.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Load</code> | ✅ 一致 |
| <code>TableCell.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>TableCell.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>TableCell.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>TableCell.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>TableCell.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>TableCell.Api.Control.OnInit</code> | ✅ 一致 |
| <code>TableCell.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>TableCell.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>TableCell.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>TableCell.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Page</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Parent</code> | ✅ 一致 |
| <code>TableCell.Api.Control.PreRender</code> | ✅ 一致 |
| <code>TableCell.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>TableCell.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Render</code> | ✅ 一致 |
| <code>TableCell.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>TableCell.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>TableCell.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>TableCell.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>TableCell.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Site</code> | ✅ 一致 |
| <code>TableCell.Api.Control.SkinID</code> | ✅ 一致 |
| <code>TableCell.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>TableCell.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>TableCell.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Unload</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ViewState</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>TableCell.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>TableCell.Api.Control.Visible</code> | ✅ 一致 |
| <code>TableCell.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>TableCell.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>TableCell.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>TableCell.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>TableCell.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>TableCell.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>TableCell.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>TableCell.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>TableCell.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>TableCell.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>TableCell.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>TableCell.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>TableCell.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.#ctor</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.AddAttributesToRender</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.AddParsedSubObject</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.AssociatedHeaderCellID</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.ColumnSpan</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.CreateControlStyle</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.HorizontalAlign</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.RenderContents</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.RowSpan</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.Text</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.VerticalAlign</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.TableCell.Wrap</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>TableCell.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>TableCell.(case list)</code> | ✅ 一致 |
| <code>TableCell.(default)</code> | ✅ 一致 |
| <code>TableCell.AccessKey=empty</code> | ✅ 一致 |
| <code>TableCell.AccessKey=japanese</code> | ✅ 一致 |
| <code>TableCell.AccessKey=k</code> | ✅ 一致 |
| <code>TableCell.AccessKey=markup</code> | ✅ 一致 |
| <code>TableCell.AccessKey=text</code> | ✅ 一致 |
| <code>TableCell.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>TableCell.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>TableCell.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>TableCell.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>TableCell.AssociatedHeaderCellID=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TableCell.AssociatedHeaderCellID=header</code> | ✅ 一致 |
| <code>TableCell.AssociatedHeaderCellID=header+Text=text</code> | ✅ 一致 |
| <code>TableCell.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>TableCell.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>TableCell.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>TableCell.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>TableCell.BackColor=#3366CC</code> | ✅ 一致 |
| <code>TableCell.BackColor=Red</code> | ✅ 一致 |
| <code>TableCell.BackColor=Transparent</code> | ✅ 一致 |
| <code>TableCell.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>TableCell.BorderColor=Red</code> | ✅ 一致 |
| <code>TableCell.BorderColor=Transparent</code> | ✅ 一致 |
| <code>TableCell.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>TableCell.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>TableCell.BorderStyle=Double</code> | ✅ 一致 |
| <code>TableCell.BorderStyle=Groove</code> | ✅ 一致 |
| <code>TableCell.BorderStyle=Inset</code> | ✅ 一致 |
| <code>TableCell.BorderStyle=None</code> | ✅ 一致 |
| <code>TableCell.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>TableCell.BorderStyle=Outset</code> | ✅ 一致 |
| <code>TableCell.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>TableCell.BorderStyle=Solid</code> | ✅ 一致 |
| <code>TableCell.BorderWidth=120px</code> | ✅ 一致 |
| <code>TableCell.BorderWidth=2em</code> | ✅ 一致 |
| <code>TableCell.BorderWidth=50%</code> | ✅ 一致 |
| <code>TableCell.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TableCell.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TableCell.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TableCell.ClientIDMode=Static</code> | ✅ 一致 |
| <code>TableCell.ColumnSpan=-1</code> | ✅ 一致 |
| <code>TableCell.ColumnSpan=0</code> | ✅ 一致 |
| <code>TableCell.ColumnSpan=3</code> | ✅ 一致 |
| <code>TableCell.ColumnSpan=3+RowSpan=3</code> | ✅ 一致 |
| <code>TableCell.CssClass=empty</code> | ✅ 一致 |
| <code>TableCell.CssClass=japanese</code> | ✅ 一致 |
| <code>TableCell.CssClass=markup</code> | ✅ 一致 |
| <code>TableCell.CssClass=text</code> | ✅ 一致 |
| <code>TableCell.CssClass=text+EnableViewState=false</code> | ✅ 一致 |
| <code>TableCell.CssClass=text+Visible=false</code> | ✅ 一致 |
| <code>TableCell.EnableTheming=false</code> | ✅ 一致 |
| <code>TableCell.EnableTheming=true</code> | ✅ 一致 |
| <code>TableCell.EnableViewState=false</code> | ✅ 一致 |
| <code>TableCell.EnableViewState=true</code> | ✅ 一致 |
| <code>TableCell.Enabled=false</code> | ✅ 一致 |
| <code>TableCell.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>TableCell.Enabled=true</code> | ✅ 一致 |
| <code>TableCell.Font.Bold=false</code> | ✅ 一致 |
| <code>TableCell.Font.Bold=true</code> | ✅ 一致 |
| <code>TableCell.Font.Italic=false</code> | ✅ 一致 |
| <code>TableCell.Font.Italic=true</code> | ✅ 一致 |
| <code>TableCell.Font.Name=empty</code> | ✅ 一致 |
| <code>TableCell.Font.Name=japanese</code> | ✅ 一致 |
| <code>TableCell.Font.Name=markup</code> | ✅ 一致 |
| <code>TableCell.Font.Name=text</code> | ✅ 一致 |
| <code>TableCell.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TableCell.Font.Overline=false</code> | ✅ 一致 |
| <code>TableCell.Font.Overline=true</code> | ✅ 一致 |
| <code>TableCell.Font.Size=12pt</code> | ✅ 一致 |
| <code>TableCell.Font.Size=150%</code> | ✅ 一致 |
| <code>TableCell.Font.Size=Large</code> | ✅ 一致 |
| <code>TableCell.Font.Strikeout=false</code> | ✅ 一致 |
| <code>TableCell.Font.Strikeout=true</code> | ✅ 一致 |
| <code>TableCell.Font.Underline=false</code> | ✅ 一致 |
| <code>TableCell.Font.Underline=true</code> | ✅ 一致 |
| <code>TableCell.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>TableCell.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>TableCell.ForeColor=Red</code> | ✅ 一致 |
| <code>TableCell.ForeColor=Transparent</code> | ✅ 一致 |
| <code>TableCell.Height=120px</code> | ✅ 一致 |
| <code>TableCell.Height=2em</code> | ✅ 一致 |
| <code>TableCell.Height=50%</code> | ✅ 一致 |
| <code>TableCell.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>TableCell.HorizontalAlign=Center+VerticalAlign=Top</code> | ✅ 一致 |
| <code>TableCell.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>TableCell.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>TableCell.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>TableCell.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>TableCell.ID=empty</code> | ✅ 一致 |
| <code>TableCell.ID=japanese</code> | ✅ 一致 |
| <code>TableCell.ID=markup</code> | ✅ 一致 |
| <code>TableCell.ID=text</code> | ✅ 一致 |
| <code>TableCell.RowSpan=-1</code> | ✅ 一致 |
| <code>TableCell.RowSpan=0</code> | ✅ 一致 |
| <code>TableCell.RowSpan=3</code> | ✅ 一致 |
| <code>TableCell.SkinID=empty</code> | ✅ 一致 |
| <code>TableCell.SkinID=japanese</code> | ✅ 一致 |
| <code>TableCell.SkinID=markup</code> | ✅ 一致 |
| <code>TableCell.SkinID=text</code> | ✅ 一致 |
| <code>TableCell.Style[color]=1em</code> | ✅ 一致 |
| <code>TableCell.Style[color]=red</code> | ✅ 一致 |
| <code>TableCell.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>TableCell.Style[margin-left]=red</code> | ✅ 一致 |
| <code>TableCell.TabIndex=-1</code> | ✅ 一致 |
| <code>TableCell.TabIndex=0</code> | ✅ 一致 |
| <code>TableCell.TabIndex=3</code> | ✅ 一致 |
| <code>TableCell.Text=empty</code> | ✅ 一致 |
| <code>TableCell.Text=japanese</code> | ✅ 一致 |
| <code>TableCell.Text=markup</code> | ✅ 一致 |
| <code>TableCell.Text=markup+Wrap=false</code> | ✅ 一致 |
| <code>TableCell.Text=text</code> | ✅ 一致 |
| <code>TableCell.ToolTip=empty</code> | ✅ 一致 |
| <code>TableCell.ToolTip=japanese</code> | ✅ 一致 |
| <code>TableCell.ToolTip=markup</code> | ✅ 一致 |
| <code>TableCell.ToolTip=text</code> | ✅ 一致 |
| <code>TableCell.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>TableCell.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>TableCell.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>TableCell.VerticalAlign=Bottom</code> | ✅ 一致 |
| <code>TableCell.VerticalAlign=Middle</code> | ✅ 一致 |
| <code>TableCell.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>TableCell.VerticalAlign=Top</code> | ✅ 一致 |
| <code>TableCell.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>TableCell.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>TableCell.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>TableCell.Visible=false</code> | ✅ 一致 |
| <code>TableCell.Visible=true</code> | ✅ 一致 |
| <code>TableCell.Width=120px</code> | ✅ 一致 |
| <code>TableCell.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>TableCell.Width=2em</code> | ✅ 一致 |
| <code>TableCell.Width=50%</code> | ✅ 一致 |
| <code>TableCell.Wrap=false</code> | ✅ 一致 |
| <code>TableCell.Wrap=true</code> | ✅ 一致 |
| <code>TableCell[naming container].(default)</code> | ✅ 一致 |
| <code>TableCell[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TableCell[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TableCell[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TableCell[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>TableCell[naming container].ID=text</code> | ✅ 一致 |
| <code>TableCell[themed].(default)</code> | ✅ 一致 |
| <code>TableCell[themed].CssClass=text</code> | ✅ 一致 |
| <code>TableCell[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>TableCell[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| TableCell [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| TableCell [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| TableCell [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| TableCell [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| TableCell [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| TableCell [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| TableCell [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| TableCell [viewstate] a calendar&#39;s next month | ✅ 一致 |
| TableCell [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| TableCell [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| TableCell [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| TableCell [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| TableCell [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| TableCell [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
