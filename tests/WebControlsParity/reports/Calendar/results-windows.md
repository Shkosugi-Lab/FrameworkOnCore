# Calendar パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 16:42
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-12, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 192 | 192 | 0 | 0 |
| ② HTML | 276 | 276 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 201 件中 201 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 122 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 263 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 9 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 4 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| ポストバックイベント | 0 | 4 | ✅ |
| 複数選択 | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>Calendar.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Calendar.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Calendar.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Calendar.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Calendar.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Calendar.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Calendar.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Context</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Controls</code> | ✅ 一致 |
| <code>Calendar.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Calendar.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Calendar.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Calendar.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Calendar.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Calendar.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Calendar.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Calendar.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Calendar.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Calendar.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Calendar.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Calendar.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Events</code> | ✅ 一致 |
| <code>Calendar.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Focus</code> | ✅ 一致 |
| <code>Calendar.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Calendar.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Calendar.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Calendar.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ID</code> | ✅ 一致 |
| <code>Calendar.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Init</code> | ✅ 一致 |
| <code>Calendar.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Calendar.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Calendar.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Load</code> | ✅ 一致 |
| <code>Calendar.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Calendar.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Calendar.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Calendar.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Calendar.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Calendar.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Calendar.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Calendar.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Calendar.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Calendar.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Page</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Parent</code> | ✅ 一致 |
| <code>Calendar.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Calendar.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Calendar.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Render</code> | ✅ 一致 |
| <code>Calendar.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Calendar.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Calendar.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Calendar.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Calendar.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Site</code> | ✅ 一致 |
| <code>Calendar.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Calendar.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Calendar.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Calendar.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Unload</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Calendar.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Calendar.Api.Control.Visible</code> | ✅ 一致 |
| <code>Calendar.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>Calendar.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>Calendar.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Calendar.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Calendar.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Calendar.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Calendar.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Calendar.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Calendar.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Calendar.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Calendar.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Calendar.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Calendar.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>Calendar.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.#ctor</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.Caption</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.CaptionAlign</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.CellPadding</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.CellSpacing</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.CreateControlCollection</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.DayHeaderStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.DayNameFormat</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.DayRender</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.DayStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.FirstDayOfWeek</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.HasWeekSelectors</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.LoadViewState</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.NextMonthText</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.NextPrevFormat</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.NextPrevStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.OnDayRender</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.OnPreRender</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.OnSelectionChanged</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.OnVisibleMonthChanged</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.OtherMonthDayStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.PrevMonthText</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.RaisePostBackEvent</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.Render</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.SaveViewState</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.SelectMonthText</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.SelectWeekText</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.SelectedDate</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.SelectedDates</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.SelectedDayStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.SelectionChanged</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.SelectionMode</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.SelectorStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.ShowDayHeader</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.ShowGridLines</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.ShowNextPrevMonth</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.ShowTitle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.TitleFormat</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.TitleStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.TodayDayStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.TodaysDate</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.TrackViewState</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.UseAccessibleHeader</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.VisibleDate</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.VisibleMonthChanged</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.Calendar.WeekendDayStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>Calendar.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Calendar.(case list)</code> | ✅ 一致 |
| <code>Calendar.(default)</code> | ✅ 一致 |
| <code>Calendar.AccessKey=empty</code> | ✅ 一致 |
| <code>Calendar.AccessKey=japanese</code> | ✅ 一致 |
| <code>Calendar.AccessKey=k</code> | ✅ 一致 |
| <code>Calendar.AccessKey=markup</code> | ✅ 一致 |
| <code>Calendar.AccessKey=text</code> | ✅ 一致 |
| <code>Calendar.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Calendar.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Calendar.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Calendar.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Calendar.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>Calendar.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>Calendar.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>Calendar.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>Calendar.BackColor=#3366CC</code> | ✅ 一致 |
| <code>Calendar.BackColor=Red</code> | ✅ 一致 |
| <code>Calendar.BackColor=Transparent</code> | ✅ 一致 |
| <code>Calendar.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>Calendar.BorderColor=Red</code> | ✅ 一致 |
| <code>Calendar.BorderColor=Transparent</code> | ✅ 一致 |
| <code>Calendar.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>Calendar.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>Calendar.BorderStyle=Double</code> | ✅ 一致 |
| <code>Calendar.BorderStyle=Groove</code> | ✅ 一致 |
| <code>Calendar.BorderStyle=Inset</code> | ✅ 一致 |
| <code>Calendar.BorderStyle=None</code> | ✅ 一致 |
| <code>Calendar.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>Calendar.BorderStyle=Outset</code> | ✅ 一致 |
| <code>Calendar.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>Calendar.BorderStyle=Solid</code> | ✅ 一致 |
| <code>Calendar.BorderWidth=120px</code> | ✅ 一致 |
| <code>Calendar.BorderWidth=2em</code> | ✅ 一致 |
| <code>Calendar.BorderWidth=50%</code> | ✅ 一致 |
| <code>Calendar.Caption=empty</code> | ✅ 一致 |
| <code>Calendar.Caption=japanese</code> | ✅ 一致 |
| <code>Calendar.Caption=markup</code> | ✅ 一致 |
| <code>Calendar.Caption=text</code> | ✅ 一致 |
| <code>Calendar.CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>Calendar.CaptionAlign=Left</code> | ✅ 一致 |
| <code>Calendar.CaptionAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.CaptionAlign=Right</code> | ✅ 一致 |
| <code>Calendar.CaptionAlign=Top</code> | ✅ 一致 |
| <code>Calendar.CellPadding=-1</code> | ✅ 一致 |
| <code>Calendar.CellPadding=0</code> | ✅ 一致 |
| <code>Calendar.CellPadding=3</code> | ✅ 一致 |
| <code>Calendar.CellSpacing=-1</code> | ✅ 一致 |
| <code>Calendar.CellSpacing=0</code> | ✅ 一致 |
| <code>Calendar.CellSpacing=3</code> | ✅ 一致 |
| <code>Calendar.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Calendar.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Calendar.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Calendar.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Calendar.CssClass=empty</code> | ✅ 一致 |
| <code>Calendar.CssClass=japanese</code> | ✅ 一致 |
| <code>Calendar.CssClass=markup</code> | ✅ 一致 |
| <code>Calendar.CssClass=text</code> | ✅ 一致 |
| <code>Calendar.DayHeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Calendar.DayHeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>Calendar.DayHeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Calendar.DayHeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Calendar.DayHeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>Calendar.DayHeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.DayHeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.DayHeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>Calendar.DayNameFormat=FirstLetter</code> | ✅ 一致 |
| <code>Calendar.DayNameFormat=FirstTwoLetters</code> | ✅ 一致 |
| <code>Calendar.DayNameFormat=Full</code> | ✅ 一致 |
| <code>Calendar.DayNameFormat=Short</code> | ✅ 一致 |
| <code>Calendar.DayNameFormat=Shortest</code> | ✅ 一致 |
| <code>Calendar.DayStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Calendar.DayStyle.CssClass=text</code> | ✅ 一致 |
| <code>Calendar.DayStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Calendar.DayStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Calendar.DayStyle.Height=120px</code> | ✅ 一致 |
| <code>Calendar.DayStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.DayStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.DayStyle.Wrap=true</code> | ✅ 一致 |
| <code>Calendar.EnableTheming=false</code> | ✅ 一致 |
| <code>Calendar.EnableTheming=true</code> | ✅ 一致 |
| <code>Calendar.EnableViewState=false</code> | ✅ 一致 |
| <code>Calendar.EnableViewState=true</code> | ✅ 一致 |
| <code>Calendar.Enabled=false</code> | ✅ 一致 |
| <code>Calendar.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>Calendar.Enabled=false+SelectionMode=DayWeekMonth</code> | ✅ 一致 |
| <code>Calendar.Enabled=true</code> | ✅ 一致 |
| <code>Calendar.FirstDayOfWeek=Default</code> | ✅ 一致 |
| <code>Calendar.FirstDayOfWeek=Friday</code> | ✅ 一致 |
| <code>Calendar.FirstDayOfWeek=Monday</code> | ✅ 一致 |
| <code>Calendar.FirstDayOfWeek=Monday+DayNameFormat=FirstTwoLetters</code> | ✅ 一致 |
| <code>Calendar.FirstDayOfWeek=Saturday</code> | ✅ 一致 |
| <code>Calendar.FirstDayOfWeek=Sunday</code> | ✅ 一致 |
| <code>Calendar.FirstDayOfWeek=Thursday</code> | ✅ 一致 |
| <code>Calendar.FirstDayOfWeek=Tuesday</code> | ✅ 一致 |
| <code>Calendar.FirstDayOfWeek=Wednesday</code> | ✅ 一致 |
| <code>Calendar.Font.Bold=false</code> | ✅ 一致 |
| <code>Calendar.Font.Bold=true</code> | ✅ 一致 |
| <code>Calendar.Font.Italic=false</code> | ✅ 一致 |
| <code>Calendar.Font.Italic=true</code> | ✅ 一致 |
| <code>Calendar.Font.Name=empty</code> | ✅ 一致 |
| <code>Calendar.Font.Name=japanese</code> | ✅ 一致 |
| <code>Calendar.Font.Name=markup</code> | ✅ 一致 |
| <code>Calendar.Font.Name=text</code> | ✅ 一致 |
| <code>Calendar.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>Calendar.Font.Overline=false</code> | ✅ 一致 |
| <code>Calendar.Font.Overline=true</code> | ✅ 一致 |
| <code>Calendar.Font.Size=12pt</code> | ✅ 一致 |
| <code>Calendar.Font.Size=150%</code> | ✅ 一致 |
| <code>Calendar.Font.Size=Large</code> | ✅ 一致 |
| <code>Calendar.Font.Strikeout=false</code> | ✅ 一致 |
| <code>Calendar.Font.Strikeout=true</code> | ✅ 一致 |
| <code>Calendar.Font.Underline=false</code> | ✅ 一致 |
| <code>Calendar.Font.Underline=true</code> | ✅ 一致 |
| <code>Calendar.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>Calendar.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>Calendar.ForeColor=Red</code> | ✅ 一致 |
| <code>Calendar.ForeColor=Transparent</code> | ✅ 一致 |
| <code>Calendar.Height=120px</code> | ✅ 一致 |
| <code>Calendar.Height=2em</code> | ✅ 一致 |
| <code>Calendar.Height=50%</code> | ✅ 一致 |
| <code>Calendar.ID=empty</code> | ✅ 一致 |
| <code>Calendar.ID=japanese</code> | ✅ 一致 |
| <code>Calendar.ID=markup</code> | ✅ 一致 |
| <code>Calendar.ID=text</code> | ✅ 一致 |
| <code>Calendar.NextMonthText=empty</code> | ✅ 一致 |
| <code>Calendar.NextMonthText=gt</code> | ✅ 一致 |
| <code>Calendar.NextMonthText=japanese</code> | ✅ 一致 |
| <code>Calendar.NextMonthText=markup</code> | ✅ 一致 |
| <code>Calendar.NextMonthText=text</code> | ✅ 一致 |
| <code>Calendar.NextPrevFormat=CustomText</code> | ✅ 一致 |
| <code>Calendar.NextPrevFormat=FullMonth</code> | ✅ 一致 |
| <code>Calendar.NextPrevFormat=FullMonth+DayNameFormat=Full</code> | ✅ 一致 |
| <code>Calendar.NextPrevFormat=ShortMonth</code> | ✅ 一致 |
| <code>Calendar.NextPrevStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Calendar.NextPrevStyle.CssClass=text</code> | ✅ 一致 |
| <code>Calendar.NextPrevStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Calendar.NextPrevStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Calendar.NextPrevStyle.Height=120px</code> | ✅ 一致 |
| <code>Calendar.NextPrevStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.NextPrevStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.NextPrevStyle.Wrap=true</code> | ✅ 一致 |
| <code>Calendar.OtherMonthDayStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Calendar.OtherMonthDayStyle.CssClass=text</code> | ✅ 一致 |
| <code>Calendar.OtherMonthDayStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Calendar.OtherMonthDayStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Calendar.OtherMonthDayStyle.ForeColor=Red+TitleStyle.BackColor=#3366CC</code> | ✅ 一致 |
| <code>Calendar.OtherMonthDayStyle.Height=120px</code> | ✅ 一致 |
| <code>Calendar.OtherMonthDayStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.OtherMonthDayStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.OtherMonthDayStyle.Wrap=true</code> | ✅ 一致 |
| <code>Calendar.PrevMonthText=empty</code> | ✅ 一致 |
| <code>Calendar.PrevMonthText=japanese</code> | ✅ 一致 |
| <code>Calendar.PrevMonthText=markup</code> | ✅ 一致 |
| <code>Calendar.PrevMonthText=text</code> | ✅ 一致 |
| <code>Calendar.SelectMonthText=empty</code> | ✅ 一致 |
| <code>Calendar.SelectMonthText=japanese</code> | ✅ 一致 |
| <code>Calendar.SelectMonthText=markup</code> | ✅ 一致 |
| <code>Calendar.SelectMonthText=text</code> | ✅ 一致 |
| <code>Calendar.SelectWeekText=empty</code> | ✅ 一致 |
| <code>Calendar.SelectWeekText=japanese</code> | ✅ 一致 |
| <code>Calendar.SelectWeekText=markup</code> | ✅ 一致 |
| <code>Calendar.SelectWeekText=text</code> | ✅ 一致 |
| <code>Calendar.SelectedDate=2020-01-20</code> | ✅ 一致 |
| <code>Calendar.SelectedDate=2020-01-20+EnableViewState=false</code> | ✅ 一致 |
| <code>Calendar.SelectedDate=2020-01-20+SelectedDayStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Calendar.SelectedDate=2020-01-20+Visible=false</code> | ✅ 一致 |
| <code>Calendar.SelectedDate=2020-02-29</code> | ✅ 一致 |
| <code>Calendar.SelectedDate=MinValue</code> | ✅ 一致 |
| <code>Calendar.SelectedDayStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Calendar.SelectedDayStyle.CssClass=text</code> | ✅ 一致 |
| <code>Calendar.SelectedDayStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Calendar.SelectedDayStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Calendar.SelectedDayStyle.Height=120px</code> | ✅ 一致 |
| <code>Calendar.SelectedDayStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.SelectedDayStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.SelectedDayStyle.Wrap=true</code> | ✅ 一致 |
| <code>Calendar.SelectionMode=Day</code> | ✅ 一致 |
| <code>Calendar.SelectionMode=DayWeek</code> | ✅ 一致 |
| <code>Calendar.SelectionMode=DayWeek+SelectedDate=2020-01-20</code> | ✅ 一致 |
| <code>Calendar.SelectionMode=DayWeekMonth</code> | ✅ 一致 |
| <code>Calendar.SelectionMode=DayWeekMonth+SelectWeekText=text+SelectMonthText=markup</code> | ✅ 一致 |
| <code>Calendar.SelectionMode=None</code> | ✅ 一致 |
| <code>Calendar.SelectionMode=None+SelectedDate=2020-01-20</code> | ✅ 一致 |
| <code>Calendar.SelectorStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Calendar.SelectorStyle.CssClass=text</code> | ✅ 一致 |
| <code>Calendar.SelectorStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Calendar.SelectorStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Calendar.SelectorStyle.Height=120px</code> | ✅ 一致 |
| <code>Calendar.SelectorStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.SelectorStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.SelectorStyle.Wrap=true</code> | ✅ 一致 |
| <code>Calendar.ShowDayHeader=false</code> | ✅ 一致 |
| <code>Calendar.ShowDayHeader=true</code> | ✅ 一致 |
| <code>Calendar.ShowGridLines=false</code> | ✅ 一致 |
| <code>Calendar.ShowGridLines=true</code> | ✅ 一致 |
| <code>Calendar.ShowGridLines=true+CellPadding=3+CellSpacing=0</code> | ✅ 一致 |
| <code>Calendar.ShowNextPrevMonth=false</code> | ✅ 一致 |
| <code>Calendar.ShowNextPrevMonth=false+TitleFormat=Month</code> | ✅ 一致 |
| <code>Calendar.ShowNextPrevMonth=true</code> | ✅ 一致 |
| <code>Calendar.ShowTitle=false</code> | ✅ 一致 |
| <code>Calendar.ShowTitle=false+ShowDayHeader=false</code> | ✅ 一致 |
| <code>Calendar.ShowTitle=true</code> | ✅ 一致 |
| <code>Calendar.SkinID=empty</code> | ✅ 一致 |
| <code>Calendar.SkinID=japanese</code> | ✅ 一致 |
| <code>Calendar.SkinID=markup</code> | ✅ 一致 |
| <code>Calendar.SkinID=text</code> | ✅ 一致 |
| <code>Calendar.Style[color]=1em</code> | ✅ 一致 |
| <code>Calendar.Style[color]=red</code> | ✅ 一致 |
| <code>Calendar.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>Calendar.Style[margin-left]=red</code> | ✅ 一致 |
| <code>Calendar.TabIndex=-1</code> | ✅ 一致 |
| <code>Calendar.TabIndex=0</code> | ✅ 一致 |
| <code>Calendar.TabIndex=3</code> | ✅ 一致 |
| <code>Calendar.TitleFormat=Month</code> | ✅ 一致 |
| <code>Calendar.TitleFormat=MonthYear</code> | ✅ 一致 |
| <code>Calendar.TitleStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Calendar.TitleStyle.CssClass=text</code> | ✅ 一致 |
| <code>Calendar.TitleStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Calendar.TitleStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Calendar.TitleStyle.Height=120px</code> | ✅ 一致 |
| <code>Calendar.TitleStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.TitleStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.TitleStyle.Wrap=true</code> | ✅ 一致 |
| <code>Calendar.TodayDayStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Calendar.TodayDayStyle.BackColor=Red+WeekendDayStyle.BackColor=#3366CC</code> | ✅ 一致 |
| <code>Calendar.TodayDayStyle.CssClass=text</code> | ✅ 一致 |
| <code>Calendar.TodayDayStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Calendar.TodayDayStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Calendar.TodayDayStyle.Height=120px</code> | ✅ 一致 |
| <code>Calendar.TodayDayStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.TodayDayStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.TodayDayStyle.Wrap=true</code> | ✅ 一致 |
| <code>Calendar.TodaysDate=2020-01-20</code> | ✅ 一致 |
| <code>Calendar.TodaysDate=2020-02-29</code> | ✅ 一致 |
| <code>Calendar.TodaysDate=MinValue</code> | ✅ 一致 |
| <code>Calendar.ToolTip=empty</code> | ✅ 一致 |
| <code>Calendar.ToolTip=japanese</code> | ✅ 一致 |
| <code>Calendar.ToolTip=markup</code> | ✅ 一致 |
| <code>Calendar.ToolTip=text</code> | ✅ 一致 |
| <code>Calendar.UseAccessibleHeader=false</code> | ✅ 一致 |
| <code>Calendar.UseAccessibleHeader=false+Caption=text+CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>Calendar.UseAccessibleHeader=true</code> | ✅ 一致 |
| <code>Calendar.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Calendar.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Calendar.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Calendar.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Calendar.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Calendar.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Calendar.Visible=false</code> | ✅ 一致 |
| <code>Calendar.Visible=true</code> | ✅ 一致 |
| <code>Calendar.VisibleDate=2020-01-20</code> | ✅ 一致 |
| <code>Calendar.VisibleDate=2020-02-29</code> | ✅ 一致 |
| <code>Calendar.VisibleDate=MinValue</code> | ✅ 一致 |
| <code>Calendar.WeekendDayStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Calendar.WeekendDayStyle.CssClass=text</code> | ✅ 一致 |
| <code>Calendar.WeekendDayStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Calendar.WeekendDayStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Calendar.WeekendDayStyle.Height=120px</code> | ✅ 一致 |
| <code>Calendar.WeekendDayStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.WeekendDayStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Calendar.WeekendDayStyle.Wrap=true</code> | ✅ 一致 |
| <code>Calendar.Width=120px</code> | ✅ 一致 |
| <code>Calendar.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>Calendar.Width=2em</code> | ✅ 一致 |
| <code>Calendar.Width=50%</code> | ✅ 一致 |
| <code>Calendar[naming container].(default)</code> | ✅ 一致 |
| <code>Calendar[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Calendar[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Calendar[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Calendar[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Calendar[naming container].ID=text</code> | ✅ 一致 |
| <code>Calendar[naming container].SelectionMode=DayWeekMonth</code> | ✅ 一致 |
| <code>Calendar[themed].(default)</code> | ✅ 一致 |
| <code>Calendar[themed].CssClass=text</code> | ✅ 一致 |
| <code>Calendar[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>Calendar[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Calendar [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| Calendar [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| Calendar [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| Calendar [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| Calendar [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| Calendar [postbackevent] a calendar&#39;s day chosen | ✅ 一致 |
| Calendar [postbackevent][multiselect] a calendar&#39;s week chosen (several dates) | ✅ 一致 |
| Calendar [postbackevent] a calendar&#39;s next month | ✅ 一致 |
| Calendar [postbackevent][dynamic] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| Calendar [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| Calendar [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| Calendar [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| Calendar [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| Calendar [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
