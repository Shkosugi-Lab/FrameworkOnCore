# TableHeaderCell パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:54
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 162 | 162 | 0 | 0 |
| ② HTML | 151 | 151 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 171 件中 171 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 39 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 139 | 1 | ✅ |
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
| <code>TableHeaderCell.Api.Control.#ctor</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Adapter</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ClientID</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Context</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Controls</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.DataBind</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Dispose</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Disposed</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Events</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.FindControl</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Focus</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.HasControls</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ID</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Init</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Load</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.OnInit</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Page</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Parent</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.PreRender</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Render</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Site</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.SkinID</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Unload</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ViewState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.Control.Visible</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.#ctor</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.AddAttributesToRender</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.AddParsedSubObject</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.AssociatedHeaderCellID</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.ColumnSpan</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.CreateControlStyle</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.HorizontalAlign</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.RenderContents</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.RowSpan</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.Text</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.VerticalAlign</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableCell.Wrap</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableHeaderCell.#ctor</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableHeaderCell.AbbreviatedText</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableHeaderCell.AddAttributesToRender</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableHeaderCell.CategoryText</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.TableHeaderCell.Scope</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>TableHeaderCell.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>TableHeaderCell.(case list)</code> | ✅ 一致 |
| <code>TableHeaderCell.(default)</code> | ✅ 一致 |
| <code>TableHeaderCell.AbbreviatedText=empty</code> | ✅ 一致 |
| <code>TableHeaderCell.AbbreviatedText=japanese</code> | ✅ 一致 |
| <code>TableHeaderCell.AbbreviatedText=markup</code> | ✅ 一致 |
| <code>TableHeaderCell.AbbreviatedText=text</code> | ✅ 一致 |
| <code>TableHeaderCell.AccessKey=empty</code> | ✅ 一致 |
| <code>TableHeaderCell.AccessKey=japanese</code> | ✅ 一致 |
| <code>TableHeaderCell.AccessKey=k</code> | ✅ 一致 |
| <code>TableHeaderCell.AccessKey=markup</code> | ✅ 一致 |
| <code>TableHeaderCell.AccessKey=text</code> | ✅ 一致 |
| <code>TableHeaderCell.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>TableHeaderCell.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>TableHeaderCell.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>TableHeaderCell.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>TableHeaderCell.AssociatedHeaderCellID=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TableHeaderCell.AssociatedHeaderCellID=header</code> | ✅ 一致 |
| <code>TableHeaderCell.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>TableHeaderCell.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>TableHeaderCell.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>TableHeaderCell.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>TableHeaderCell.BackColor=#3366CC</code> | ✅ 一致 |
| <code>TableHeaderCell.BackColor=Red</code> | ✅ 一致 |
| <code>TableHeaderCell.BackColor=Transparent</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderColor=Red</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderColor=Transparent</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderStyle=Double</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderStyle=Groove</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderStyle=Inset</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderStyle=None</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderStyle=Outset</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderStyle=Solid</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderWidth=120px</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderWidth=2em</code> | ✅ 一致 |
| <code>TableHeaderCell.BorderWidth=50%</code> | ✅ 一致 |
| <code>TableHeaderCell.CategoryText=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TableHeaderCell.CategoryText=Arial,MS-Gothic+Scope=Row</code> | ✅ 一致 |
| <code>TableHeaderCell.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TableHeaderCell.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TableHeaderCell.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TableHeaderCell.ClientIDMode=Static</code> | ✅ 一致 |
| <code>TableHeaderCell.ColumnSpan=-1</code> | ✅ 一致 |
| <code>TableHeaderCell.ColumnSpan=0</code> | ✅ 一致 |
| <code>TableHeaderCell.ColumnSpan=3</code> | ✅ 一致 |
| <code>TableHeaderCell.ColumnSpan=3+Wrap=false</code> | ✅ 一致 |
| <code>TableHeaderCell.CssClass=empty</code> | ✅ 一致 |
| <code>TableHeaderCell.CssClass=japanese</code> | ✅ 一致 |
| <code>TableHeaderCell.CssClass=markup</code> | ✅ 一致 |
| <code>TableHeaderCell.CssClass=text</code> | ✅ 一致 |
| <code>TableHeaderCell.CssClass=text+EnableViewState=false</code> | ✅ 一致 |
| <code>TableHeaderCell.CssClass=text+Visible=false</code> | ✅ 一致 |
| <code>TableHeaderCell.EnableTheming=false</code> | ✅ 一致 |
| <code>TableHeaderCell.EnableTheming=true</code> | ✅ 一致 |
| <code>TableHeaderCell.EnableViewState=false</code> | ✅ 一致 |
| <code>TableHeaderCell.EnableViewState=true</code> | ✅ 一致 |
| <code>TableHeaderCell.Enabled=false</code> | ✅ 一致 |
| <code>TableHeaderCell.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>TableHeaderCell.Enabled=true</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Bold=false</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Bold=true</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Italic=false</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Italic=true</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Name=empty</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Name=japanese</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Name=markup</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Name=text</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Overline=false</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Overline=true</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Size=12pt</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Size=150%</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Size=Large</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Strikeout=false</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Strikeout=true</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Underline=false</code> | ✅ 一致 |
| <code>TableHeaderCell.Font.Underline=true</code> | ✅ 一致 |
| <code>TableHeaderCell.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>TableHeaderCell.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>TableHeaderCell.ForeColor=Red</code> | ✅ 一致 |
| <code>TableHeaderCell.ForeColor=Transparent</code> | ✅ 一致 |
| <code>TableHeaderCell.Height=120px</code> | ✅ 一致 |
| <code>TableHeaderCell.Height=2em</code> | ✅ 一致 |
| <code>TableHeaderCell.Height=50%</code> | ✅ 一致 |
| <code>TableHeaderCell.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>TableHeaderCell.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>TableHeaderCell.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>TableHeaderCell.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>TableHeaderCell.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>TableHeaderCell.ID=empty</code> | ✅ 一致 |
| <code>TableHeaderCell.ID=japanese</code> | ✅ 一致 |
| <code>TableHeaderCell.ID=markup</code> | ✅ 一致 |
| <code>TableHeaderCell.ID=text</code> | ✅ 一致 |
| <code>TableHeaderCell.RowSpan=-1</code> | ✅ 一致 |
| <code>TableHeaderCell.RowSpan=0</code> | ✅ 一致 |
| <code>TableHeaderCell.RowSpan=3</code> | ✅ 一致 |
| <code>TableHeaderCell.Scope=Column</code> | ✅ 一致 |
| <code>TableHeaderCell.Scope=Column+AbbreviatedText=text</code> | ✅ 一致 |
| <code>TableHeaderCell.Scope=NotSet</code> | ✅ 一致 |
| <code>TableHeaderCell.Scope=Row</code> | ✅ 一致 |
| <code>TableHeaderCell.SkinID=empty</code> | ✅ 一致 |
| <code>TableHeaderCell.SkinID=japanese</code> | ✅ 一致 |
| <code>TableHeaderCell.SkinID=markup</code> | ✅ 一致 |
| <code>TableHeaderCell.SkinID=text</code> | ✅ 一致 |
| <code>TableHeaderCell.Style[color]=1em</code> | ✅ 一致 |
| <code>TableHeaderCell.Style[color]=red</code> | ✅ 一致 |
| <code>TableHeaderCell.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>TableHeaderCell.Style[margin-left]=red</code> | ✅ 一致 |
| <code>TableHeaderCell.TabIndex=-1</code> | ✅ 一致 |
| <code>TableHeaderCell.TabIndex=0</code> | ✅ 一致 |
| <code>TableHeaderCell.TabIndex=3</code> | ✅ 一致 |
| <code>TableHeaderCell.Text=empty</code> | ✅ 一致 |
| <code>TableHeaderCell.Text=japanese</code> | ✅ 一致 |
| <code>TableHeaderCell.Text=markup</code> | ✅ 一致 |
| <code>TableHeaderCell.Text=text</code> | ✅ 一致 |
| <code>TableHeaderCell.ToolTip=empty</code> | ✅ 一致 |
| <code>TableHeaderCell.ToolTip=japanese</code> | ✅ 一致 |
| <code>TableHeaderCell.ToolTip=markup</code> | ✅ 一致 |
| <code>TableHeaderCell.ToolTip=text</code> | ✅ 一致 |
| <code>TableHeaderCell.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>TableHeaderCell.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>TableHeaderCell.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>TableHeaderCell.VerticalAlign=Bottom</code> | ✅ 一致 |
| <code>TableHeaderCell.VerticalAlign=Middle</code> | ✅ 一致 |
| <code>TableHeaderCell.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>TableHeaderCell.VerticalAlign=Top</code> | ✅ 一致 |
| <code>TableHeaderCell.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>TableHeaderCell.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>TableHeaderCell.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>TableHeaderCell.Visible=false</code> | ✅ 一致 |
| <code>TableHeaderCell.Visible=true</code> | ✅ 一致 |
| <code>TableHeaderCell.Width=120px</code> | ✅ 一致 |
| <code>TableHeaderCell.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>TableHeaderCell.Width=2em</code> | ✅ 一致 |
| <code>TableHeaderCell.Width=50%</code> | ✅ 一致 |
| <code>TableHeaderCell.Wrap=false</code> | ✅ 一致 |
| <code>TableHeaderCell.Wrap=true</code> | ✅ 一致 |
| <code>TableHeaderCell[naming container].(default)</code> | ✅ 一致 |
| <code>TableHeaderCell[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TableHeaderCell[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TableHeaderCell[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TableHeaderCell[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>TableHeaderCell[naming container].ID=text</code> | ✅ 一致 |
| <code>TableHeaderCell[themed].(default)</code> | ✅ 一致 |
| <code>TableHeaderCell[themed].CssClass=text</code> | ✅ 一致 |
| <code>TableHeaderCell[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>TableHeaderCell[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| TableHeaderCell [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| TableHeaderCell [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| TableHeaderCell [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| TableHeaderCell [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| TableHeaderCell [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| TableHeaderCell [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| TableHeaderCell [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| TableHeaderCell [viewstate] a calendar&#39;s next month | ✅ 一致 |
| TableHeaderCell [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| TableHeaderCell [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| TableHeaderCell [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| TableHeaderCell [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| TableHeaderCell [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| TableHeaderCell [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
