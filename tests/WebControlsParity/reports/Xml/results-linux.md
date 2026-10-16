# Xml パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 18:12
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 123 | 123 | 0 | 0 |
| ② HTML | 60 | 56 | 4 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 130 件中 130 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 12 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 51 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 13 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

| 層 | ケース | 行 | 理由 |
|---|---|---|---|
| render | <code>^Xml\.(DocumentSource&#124;TransformSource)=markup$</code> | <code>^probe-failure: System\.(ArgumentException: Illegal characters in path\.&#124;IO\.IOException: .*&#124;IO\.(FileNotFound&#124;DirectoryNotFound)Exception: .*&lt;b&gt;.*)$</code> | A file name with characters a path cannot have (&lt;, &gt;, &quot;): .NET Framework&#39;s file APIs check them first (ArgumentException &quot;Illegal characters in path.&quot;), .NET&#39;s leave it to the system (on Windows an IOException with the system&#39;s message; on Linux the name is valid, and the folder it names is not found). The runtime&#39;s, not System.Web&#39;s: both fail to read the file. |
| render | <code>^Xml(\[[^\]]+\])?\.(DocumentSource&#124;TransformSource)=missing$</code> | <code>^probe-failure: System\.IO\.FileNotFoundException: Could not find file &#39;\{site\}App_Data[\\/]none\.(xml&#124;xslt)&#39;\.$</code> | The file not found is named as the system writes paths: App_Data\none.xml on Windows, App_Data/none.xml on Linux. |

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>Xml.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Xml.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Xml.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Xml.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Xml.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Xml.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Xml.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Xml.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Xml.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Xml.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Xml.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Xml.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Xml.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Xml.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Xml.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Xml.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Xml.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Xml.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Xml.Api.Control.Context</code> | ✅ 一致 |
| <code>Xml.Api.Control.Controls</code> | ✅ 一致 |
| <code>Xml.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Xml.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Xml.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Xml.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Xml.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Xml.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Xml.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Xml.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Xml.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Xml.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Xml.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Xml.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Xml.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Xml.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Xml.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Xml.Api.Control.Events</code> | ✅ 一致 |
| <code>Xml.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Xml.Api.Control.Focus</code> | ✅ 一致 |
| <code>Xml.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Xml.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Xml.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Xml.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Xml.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Xml.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Xml.Api.Control.ID</code> | ✅ 一致 |
| <code>Xml.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Xml.Api.Control.Init</code> | ✅ 一致 |
| <code>Xml.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Xml.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Xml.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Xml.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Xml.Api.Control.Load</code> | ✅ 一致 |
| <code>Xml.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Xml.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Xml.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Xml.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Xml.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Xml.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Xml.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Xml.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Xml.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Xml.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Xml.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Xml.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Xml.Api.Control.Page</code> | ✅ 一致 |
| <code>Xml.Api.Control.Parent</code> | ✅ 一致 |
| <code>Xml.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Xml.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Xml.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Xml.Api.Control.Render</code> | ✅ 一致 |
| <code>Xml.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Xml.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Xml.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Xml.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Xml.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Xml.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Xml.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Xml.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Xml.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Xml.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Xml.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Xml.Api.Control.Site</code> | ✅ 一致 |
| <code>Xml.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Xml.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Xml.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Xml.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Xml.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Xml.Api.Control.Unload</code> | ✅ 一致 |
| <code>Xml.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Xml.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Xml.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Xml.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Xml.Api.Control.Visible</code> | ✅ 一致 |
| <code>Xml.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Xml.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Xml.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Xml.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Xml.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Xml.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Xml.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Xml.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Xml.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Xml.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Xml.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.#ctor</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.AddParsedSubObject</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.ClientID</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.Controls</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.CreateControlCollection</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.Document</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.DocumentContent</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.DocumentSource</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.EnableTheming</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.FindControl</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.Focus</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.GetDesignModeState</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.HasControls</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.Render</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.SkinID</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.Transform</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.TransformArgumentList</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.TransformSource</code> | ✅ 一致 |
| <code>Xml.Api.WebControls.Xml.XPathNavigator</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Xml.(case list)</code> | ✅ 一致 |
| <code>Xml.(default)</code> | ✅ 一致 |
| <code>Xml.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Xml.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Xml.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Xml.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Xml.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Xml.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Xml.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Xml.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Xml.DocumentContent=empty</code> | ✅ 一致 |
| <code>Xml.DocumentContent=invalid</code> | ✅ 一致 |
| <code>Xml.DocumentContent=items</code> | ✅ 一致 |
| <code>Xml.DocumentContent=japanese</code> | ✅ 一致 |
| <code>Xml.DocumentContent=markup</code> | ✅ 一致 |
| <code>Xml.DocumentContent=text</code> | ✅ 一致 |
| <code>Xml.DocumentSource=app-relative</code> | ✅ 一致 |
| <code>Xml.DocumentSource=app-relative+DocumentContent=items</code> | ✅ 一致 |
| <code>Xml.DocumentSource=empty</code> | ✅ 一致 |
| <code>Xml.DocumentSource=japanese</code> | ✅ 一致 |
| <code>Xml.DocumentSource=markup</code> | ⚠️ 既知の差分 |
| <code>Xml.DocumentSource=missing</code> | ⚠️ 既知の差分 |
| <code>Xml.DocumentSource=text</code> | ✅ 一致 |
| <code>Xml.EnableTheming=false</code> | ✅ 一致 |
| <code>Xml.EnableTheming=true</code> | ✅ 一致 |
| <code>Xml.EnableViewState=false</code> | ✅ 一致 |
| <code>Xml.EnableViewState=true</code> | ✅ 一致 |
| <code>Xml.ID=empty</code> | ✅ 一致 |
| <code>Xml.ID=japanese</code> | ✅ 一致 |
| <code>Xml.ID=markup</code> | ✅ 一致 |
| <code>Xml.ID=text</code> | ✅ 一致 |
| <code>Xml.SkinID=empty</code> | ✅ 一致 |
| <code>Xml.SkinID=japanese</code> | ✅ 一致 |
| <code>Xml.SkinID=markup</code> | ✅ 一致 |
| <code>Xml.SkinID=text</code> | ✅ 一致 |
| <code>Xml.TransformSource=app-relative</code> | ✅ 一致 |
| <code>Xml.TransformSource=app-relative+DocumentContent=items</code> | ✅ 一致 |
| <code>Xml.TransformSource=app-relative+DocumentSource=app-relative</code> | ✅ 一致 |
| <code>Xml.TransformSource=app-relative+EnableViewState=false</code> | ✅ 一致 |
| <code>Xml.TransformSource=app-relative+Visible=false</code> | ✅ 一致 |
| <code>Xml.TransformSource=empty</code> | ✅ 一致 |
| <code>Xml.TransformSource=japanese</code> | ✅ 一致 |
| <code>Xml.TransformSource=markup</code> | ⚠️ 既知の差分 |
| <code>Xml.TransformSource=missing</code> | ⚠️ 既知の差分 |
| <code>Xml.TransformSource=text</code> | ✅ 一致 |
| <code>Xml.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Xml.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Xml.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Xml.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Xml.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Xml.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Xml.Visible=false</code> | ✅ 一致 |
| <code>Xml.Visible=true</code> | ✅ 一致 |
| <code>Xml[naming container].(default)</code> | ✅ 一致 |
| <code>Xml[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Xml[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Xml[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Xml[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Xml[naming container].ID=text</code> | ✅ 一致 |
| <code>Xml[naming container].TransformSource=app-relative</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Xml [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| Xml [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| Xml [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| Xml [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| Xml [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| Xml [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| Xml [viewstate] a calendar&#39;s week chosen (several dates) | ✅ 一致 |
| Xml [viewstate] a calendar&#39;s next month | ✅ 一致 |
| Xml [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| Xml [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| Xml [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| Xml [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| Xml [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| Xml [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
