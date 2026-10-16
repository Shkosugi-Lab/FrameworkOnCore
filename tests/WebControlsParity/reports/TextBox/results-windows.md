# TextBox パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 16:31
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 170 | 170 | 0 | 0 |
| ② HTML | 215 | 215 | 0 | 0 |
| ③ シナリオ | 13 | 13 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 179 件中 179 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 40 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 13 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 201 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 5 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 4 | 1 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| 変更イベント（*Changed、ServerChange） | 0 | 3 | ✅ |
| ReadOnly（ポストされた値を無視） | 3 | 1 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 2 | ✅ |
| AutoPostBack（クライアントスクリプトによるポストバック） | 7 | 3 | ✅ |
| CausesValidation / ValidationGroup | 8 | 2 | ✅ |
| 検証対象（サーバー側の検証） | 0 | 3 | ✅ |
| 検証対象（ブラウザ側の検証） | 0 | 1 | ✅ |
| 双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値） | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>TextBox.Api.Control.#ctor</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Adapter</code> | ✅ 一致 |
| <code>TextBox.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>TextBox.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>TextBox.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>TextBox.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>TextBox.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>TextBox.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ClientID</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Context</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Controls</code> | ✅ 一致 |
| <code>TextBox.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>TextBox.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>TextBox.Api.Control.DataBind</code> | ✅ 一致 |
| <code>TextBox.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>TextBox.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>TextBox.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>TextBox.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>TextBox.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Dispose</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Disposed</code> | ✅ 一致 |
| <code>TextBox.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>TextBox.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>TextBox.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>TextBox.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Events</code> | ✅ 一致 |
| <code>TextBox.Api.Control.FindControl</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Focus</code> | ✅ 一致 |
| <code>TextBox.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>TextBox.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>TextBox.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.HasControls</code> | ✅ 一致 |
| <code>TextBox.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ID</code> | ✅ 一致 |
| <code>TextBox.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Init</code> | ✅ 一致 |
| <code>TextBox.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>TextBox.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>TextBox.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Load</code> | ✅ 一致 |
| <code>TextBox.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>TextBox.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>TextBox.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>TextBox.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>TextBox.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>TextBox.Api.Control.OnInit</code> | ✅ 一致 |
| <code>TextBox.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>TextBox.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>TextBox.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>TextBox.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Page</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Parent</code> | ✅ 一致 |
| <code>TextBox.Api.Control.PreRender</code> | ✅ 一致 |
| <code>TextBox.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>TextBox.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Render</code> | ✅ 一致 |
| <code>TextBox.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>TextBox.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>TextBox.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>TextBox.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>TextBox.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Site</code> | ✅ 一致 |
| <code>TextBox.Api.Control.SkinID</code> | ✅ 一致 |
| <code>TextBox.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>TextBox.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>TextBox.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Unload</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ViewState</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>TextBox.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>TextBox.Api.Control.Visible</code> | ✅ 一致 |
| <code>TextBox.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>TextBox.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>TextBox.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>TextBox.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>TextBox.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>TextBox.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>TextBox.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>TextBox.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>TextBox.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>TextBox.Api.IEditableTextControl.TextChanged</code> | ✅ 一致 |
| <code>TextBox.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>TextBox.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>TextBox.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>TextBox.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>TextBox.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>TextBox.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>TextBox.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.#ctor</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.AddAttributesToRender</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.AddParsedSubObject</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.AutoCompleteType</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.AutoPostBack</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.CausesValidation</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.Columns</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.LoadPostData</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.MaxLength</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.OnPreRender</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.OnTextChanged</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.ReadOnly</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.Render</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.Rows</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.SaveViewState</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.TagKey</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.Text</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.TextChanged</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.TextMode</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.ValidationGroup</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.TextBox.Wrap</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>TextBox.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>TextBox.(case list)</code> | ✅ 一致 |
| <code>TextBox.(default)</code> | ✅ 一致 |
| <code>TextBox.AccessKey=empty</code> | ✅ 一致 |
| <code>TextBox.AccessKey=japanese</code> | ✅ 一致 |
| <code>TextBox.AccessKey=k</code> | ✅ 一致 |
| <code>TextBox.AccessKey=markup</code> | ✅ 一致 |
| <code>TextBox.AccessKey=text</code> | ✅ 一致 |
| <code>TextBox.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>TextBox.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>TextBox.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>TextBox.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>TextBox.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>TextBox.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>TextBox.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>TextBox.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=BusinessCity</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=BusinessCountryRegion</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=BusinessFax</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=BusinessPhone</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=BusinessState</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=BusinessStreetAddress</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=BusinessUrl</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=BusinessZipCode</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=Cellular</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=Company</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=Department</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=Disabled</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=DisplayName</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=Email</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=Enabled</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=FirstName</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=Gender</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=HomeCity</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=HomeCountryRegion</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=HomeFax</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=HomePhone</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=HomeState</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=HomeStreetAddress</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=HomeZipCode</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=Homepage</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=JobTitle</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=LastName</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=MiddleName</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=None</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=Notes</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=Office</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=Pager</code> | ✅ 一致 |
| <code>TextBox.AutoCompleteType=Search</code> | ✅ 一致 |
| <code>TextBox.AutoPostBack=false</code> | ✅ 一致 |
| <code>TextBox.AutoPostBack=true</code> | ✅ 一致 |
| <code>TextBox.AutoPostBack=true+CausesValidation=true</code> | ✅ 一致 |
| <code>TextBox.AutoPostBack=true+CausesValidation=true+ValidationGroup=text</code> | ✅ 一致 |
| <code>TextBox.AutoPostBack=true+Enabled=false</code> | ✅ 一致 |
| <code>TextBox.AutoPostBack=true+Visible=false</code> | ✅ 一致 |
| <code>TextBox.BackColor=#3366CC</code> | ✅ 一致 |
| <code>TextBox.BackColor=Red</code> | ✅ 一致 |
| <code>TextBox.BackColor=Transparent</code> | ✅ 一致 |
| <code>TextBox.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>TextBox.BorderColor=Red</code> | ✅ 一致 |
| <code>TextBox.BorderColor=Transparent</code> | ✅ 一致 |
| <code>TextBox.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>TextBox.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>TextBox.BorderStyle=Double</code> | ✅ 一致 |
| <code>TextBox.BorderStyle=Groove</code> | ✅ 一致 |
| <code>TextBox.BorderStyle=Inset</code> | ✅ 一致 |
| <code>TextBox.BorderStyle=None</code> | ✅ 一致 |
| <code>TextBox.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>TextBox.BorderStyle=Outset</code> | ✅ 一致 |
| <code>TextBox.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>TextBox.BorderStyle=Solid</code> | ✅ 一致 |
| <code>TextBox.BorderWidth=120px</code> | ✅ 一致 |
| <code>TextBox.BorderWidth=2em</code> | ✅ 一致 |
| <code>TextBox.BorderWidth=50%</code> | ✅ 一致 |
| <code>TextBox.CausesValidation=false</code> | ✅ 一致 |
| <code>TextBox.CausesValidation=true</code> | ✅ 一致 |
| <code>TextBox.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TextBox.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TextBox.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TextBox.ClientIDMode=Static</code> | ✅ 一致 |
| <code>TextBox.Columns=-1</code> | ✅ 一致 |
| <code>TextBox.Columns=0</code> | ✅ 一致 |
| <code>TextBox.Columns=3</code> | ✅ 一致 |
| <code>TextBox.Columns=3+Width=120px</code> | ✅ 一致 |
| <code>TextBox.CssClass=empty</code> | ✅ 一致 |
| <code>TextBox.CssClass=japanese</code> | ✅ 一致 |
| <code>TextBox.CssClass=markup</code> | ✅ 一致 |
| <code>TextBox.CssClass=text</code> | ✅ 一致 |
| <code>TextBox.EnableTheming=false</code> | ✅ 一致 |
| <code>TextBox.EnableTheming=true</code> | ✅ 一致 |
| <code>TextBox.EnableViewState=false</code> | ✅ 一致 |
| <code>TextBox.EnableViewState=true</code> | ✅ 一致 |
| <code>TextBox.Enabled=false</code> | ✅ 一致 |
| <code>TextBox.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>TextBox.Enabled=true</code> | ✅ 一致 |
| <code>TextBox.Font.Bold=false</code> | ✅ 一致 |
| <code>TextBox.Font.Bold=true</code> | ✅ 一致 |
| <code>TextBox.Font.Italic=false</code> | ✅ 一致 |
| <code>TextBox.Font.Italic=true</code> | ✅ 一致 |
| <code>TextBox.Font.Name=empty</code> | ✅ 一致 |
| <code>TextBox.Font.Name=japanese</code> | ✅ 一致 |
| <code>TextBox.Font.Name=markup</code> | ✅ 一致 |
| <code>TextBox.Font.Name=text</code> | ✅ 一致 |
| <code>TextBox.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TextBox.Font.Overline=false</code> | ✅ 一致 |
| <code>TextBox.Font.Overline=true</code> | ✅ 一致 |
| <code>TextBox.Font.Size=12pt</code> | ✅ 一致 |
| <code>TextBox.Font.Size=150%</code> | ✅ 一致 |
| <code>TextBox.Font.Size=Large</code> | ✅ 一致 |
| <code>TextBox.Font.Strikeout=false</code> | ✅ 一致 |
| <code>TextBox.Font.Strikeout=true</code> | ✅ 一致 |
| <code>TextBox.Font.Underline=false</code> | ✅ 一致 |
| <code>TextBox.Font.Underline=true</code> | ✅ 一致 |
| <code>TextBox.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>TextBox.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>TextBox.ForeColor=Red</code> | ✅ 一致 |
| <code>TextBox.ForeColor=Transparent</code> | ✅ 一致 |
| <code>TextBox.Height=120px</code> | ✅ 一致 |
| <code>TextBox.Height=2em</code> | ✅ 一致 |
| <code>TextBox.Height=50%</code> | ✅ 一致 |
| <code>TextBox.ID=empty</code> | ✅ 一致 |
| <code>TextBox.ID=japanese</code> | ✅ 一致 |
| <code>TextBox.ID=markup</code> | ✅ 一致 |
| <code>TextBox.ID=text</code> | ✅ 一致 |
| <code>TextBox.MaxLength=-1</code> | ✅ 一致 |
| <code>TextBox.MaxLength=0</code> | ✅ 一致 |
| <code>TextBox.MaxLength=3</code> | ✅ 一致 |
| <code>TextBox.ReadOnly=false</code> | ✅ 一致 |
| <code>TextBox.ReadOnly=true</code> | ✅ 一致 |
| <code>TextBox.ReadOnly=true+Enabled=false</code> | ✅ 一致 |
| <code>TextBox.Rows=-1</code> | ✅ 一致 |
| <code>TextBox.Rows=0</code> | ✅ 一致 |
| <code>TextBox.Rows=3</code> | ✅ 一致 |
| <code>TextBox.SkinID=empty</code> | ✅ 一致 |
| <code>TextBox.SkinID=japanese</code> | ✅ 一致 |
| <code>TextBox.SkinID=markup</code> | ✅ 一致 |
| <code>TextBox.SkinID=text</code> | ✅ 一致 |
| <code>TextBox.Style[color]=1em</code> | ✅ 一致 |
| <code>TextBox.Style[color]=red</code> | ✅ 一致 |
| <code>TextBox.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>TextBox.Style[margin-left]=red</code> | ✅ 一致 |
| <code>TextBox.TabIndex=-1</code> | ✅ 一致 |
| <code>TextBox.TabIndex=0</code> | ✅ 一致 |
| <code>TextBox.TabIndex=3</code> | ✅ 一致 |
| <code>TextBox.Text=empty</code> | ✅ 一致 |
| <code>TextBox.Text=japanese</code> | ✅ 一致 |
| <code>TextBox.Text=leading-newline</code> | ✅ 一致 |
| <code>TextBox.Text=lines</code> | ✅ 一致 |
| <code>TextBox.Text=markup</code> | ✅ 一致 |
| <code>TextBox.Text=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>TextBox.Text=spaces</code> | ✅ 一致 |
| <code>TextBox.Text=text</code> | ✅ 一致 |
| <code>TextBox.Text=text+Visible=false</code> | ✅ 一致 |
| <code>TextBox.TextMode=Color</code> | ✅ 一致 |
| <code>TextBox.TextMode=Date</code> | ✅ 一致 |
| <code>TextBox.TextMode=Date+Text=text</code> | ✅ 一致 |
| <code>TextBox.TextMode=DateTime</code> | ✅ 一致 |
| <code>TextBox.TextMode=DateTimeLocal</code> | ✅ 一致 |
| <code>TextBox.TextMode=Email</code> | ✅ 一致 |
| <code>TextBox.TextMode=Month</code> | ✅ 一致 |
| <code>TextBox.TextMode=MultiLine</code> | ✅ 一致 |
| <code>TextBox.TextMode=MultiLine+AutoPostBack=true</code> | ✅ 一致 |
| <code>TextBox.TextMode=MultiLine+MaxLength=3</code> | ✅ 一致 |
| <code>TextBox.TextMode=MultiLine+ReadOnly=true</code> | ✅ 一致 |
| <code>TextBox.TextMode=MultiLine+Rows=3+Columns=3</code> | ✅ 一致 |
| <code>TextBox.TextMode=MultiLine+Text=leading-newline</code> | ✅ 一致 |
| <code>TextBox.TextMode=MultiLine+Text=lines</code> | ✅ 一致 |
| <code>TextBox.TextMode=MultiLine+Text=markup</code> | ✅ 一致 |
| <code>TextBox.TextMode=MultiLine+Wrap=false</code> | ✅ 一致 |
| <code>TextBox.TextMode=Number</code> | ✅ 一致 |
| <code>TextBox.TextMode=Number+MaxLength=3</code> | ✅ 一致 |
| <code>TextBox.TextMode=Password</code> | ✅ 一致 |
| <code>TextBox.TextMode=Password+AutoCompleteType=Disabled</code> | ✅ 一致 |
| <code>TextBox.TextMode=Password+Text=text</code> | ✅ 一致 |
| <code>TextBox.TextMode=Phone</code> | ✅ 一致 |
| <code>TextBox.TextMode=Range</code> | ✅ 一致 |
| <code>TextBox.TextMode=Search</code> | ✅ 一致 |
| <code>TextBox.TextMode=SingleLine</code> | ✅ 一致 |
| <code>TextBox.TextMode=Time</code> | ✅ 一致 |
| <code>TextBox.TextMode=Url</code> | ✅ 一致 |
| <code>TextBox.TextMode=Week</code> | ✅ 一致 |
| <code>TextBox.ToolTip=empty</code> | ✅ 一致 |
| <code>TextBox.ToolTip=japanese</code> | ✅ 一致 |
| <code>TextBox.ToolTip=markup</code> | ✅ 一致 |
| <code>TextBox.ToolTip=text</code> | ✅ 一致 |
| <code>TextBox.ToolTip=text+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>TextBox.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>TextBox.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>TextBox.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>TextBox.ValidationGroup=empty</code> | ✅ 一致 |
| <code>TextBox.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>TextBox.ValidationGroup=markup</code> | ✅ 一致 |
| <code>TextBox.ValidationGroup=text</code> | ✅ 一致 |
| <code>TextBox.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>TextBox.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>TextBox.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>TextBox.Visible=false</code> | ✅ 一致 |
| <code>TextBox.Visible=true</code> | ✅ 一致 |
| <code>TextBox.Width=120px</code> | ✅ 一致 |
| <code>TextBox.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>TextBox.Width=2em</code> | ✅ 一致 |
| <code>TextBox.Width=50%</code> | ✅ 一致 |
| <code>TextBox.Wrap=false</code> | ✅ 一致 |
| <code>TextBox.Wrap=true</code> | ✅ 一致 |
| <code>TextBox[naming container].(default)</code> | ✅ 一致 |
| <code>TextBox[naming container].AutoPostBack=true</code> | ✅ 一致 |
| <code>TextBox[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TextBox[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TextBox[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TextBox[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>TextBox[naming container].ID=text</code> | ✅ 一致 |
| <code>TextBox[themed].(default)</code> | ✅ 一致 |
| <code>TextBox[themed].CssClass=text</code> | ✅ 一致 |
| <code>TextBox[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>TextBox[themed].SkinID=text</code> | ✅ 一致 |
| <code>TextBox[themed].Width=120px</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| TextBox [render][databind] first request | ✅ 一致 |
| TextBox [postdata][change][readonly][disabled][password][dynamic][requestvalidation] posted values | ✅ 一致 |
| TextBox [viewstate][change] posted again unchanged | ✅ 一致 |
| TextBox [viewstate][visible] hidden text box shown | ✅ 一致 |
| TextBox [autopostback][change] auto postback | ✅ 一致 |
| TextBox [autopostback][causesvalidation] auto postback, valid | ✅ 一致 |
| TextBox [autopostback][causesvalidation] auto postback, emptied | ✅ 一致 |
| TextBox [validation] group g1, empty | ✅ 一致 |
| TextBox [validation] group g1, not digits | ✅ 一致 |
| TextBox [validation] group g1, valid | ✅ 一致 |
| TextBox [clientvalidation] group g3, empty (no postback) | ✅ 一致 |
| TextBox [bind] two-way binding in a template | ✅ 一致 |
| TextBox [requestvalidation] markup posted | ✅ 一致 |

</details>
