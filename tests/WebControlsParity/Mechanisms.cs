using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace FrameworkOnCore.WebControlsParity
{
    /// <summary>
    /// The mechanisms of WebForms a component can take part in (README.md, the mechanism matrix): which apply to a component
    /// is read from its type (the interfaces it implements, its properties, its attributes), not chosen by hand; a mechanism
    /// is covered by the rendering cases whose names match Render and the scenario snapshots tagged [Tag]. A mechanism
    /// that applies and has neither is a gap the report shows.
    /// </summary>
    public sealed class Mechanism
    {
        public Mechanism(string tag, string name, string why, Func<Type, bool> applies, string render = null)
        {
            Tag = tag;
            Name = name;
            Why = why;
            Applies = applies;
            Render = render == null ? null : new Regex(render);
        }

        public string Tag { get; }
        public string Name { get; }
        /// <summary>Why it applies (what of the type says so).</summary>
        public string Why { get; }
        public Func<Type, bool> Applies { get; }
        /// <summary>The rendering cases that cover it, by name.</summary>
        public Regex Render { get; }

        static bool Has(Type type, string property) => type.GetProperty(property, BindingFlags.Public | BindingFlags.Instance) != null;
        static bool Is<T>(Type type) => typeof(T).IsAssignableFrom(type);

        public static readonly IReadOnlyList<Mechanism> All = new[]
        {
            new Mechanism("render", "プロパティ → HTML 描画", "すべてのコントロール", t => true, @"^[^\[]+\.[A-Za-z.]+(\[[^\]]+\])?="),
            new Mechanism("viewstate", "ViewState の保存と復元", "Control", t => true, @"(EnableViewState|ViewStateMode)="),
            new Mechanism("visible", "Visible=false（描画されず状態は保持）", "Control.Visible", t => true, @"Visible=false"),
            new Mechanism("naming", "名前付けコンテナと ClientIDMode", "Control.ClientIDMode", t => true, @"\[naming container\]"),
            new Mechanism("themes", "テーマとスキン（SkinID / EnableTheming）", "テーマを付けられる型（[Themeable]）", t => ThemeableAttribute.IsTypeThemeable(t), @"\[themed\]"),
            new Mechanism("dynamic", "動的に生成したコントロールのライフサイクル", "Control", t => true),
            new Mechanism("databind", "データバインド（DataBind / <%# %>）", "Control.DataBind", t => true),
            new Mechanism("disabled", "Enabled=false・Disabled=true（描画とポスト値の扱い）", "WebControl.Enabled、HtmlControl.Disabled", t => Is<WebControl>(t) || Has(t, "Disabled"), @"Enabled=false|Disabled=true"),
            new Mechanism("postdata", "ポストバックデータの取り込み", "IPostBackDataHandler", t => Is<IPostBackDataHandler>(t)),
            new Mechanism("change", "変更イベント（*Changed、ServerChange）", "IPostBackDataHandler と *Changed・ServerChange イベント",
                t => Is<IPostBackDataHandler>(t) && t.GetEvents().Any(e => e.Name.EndsWith("Changed", StringComparison.Ordinal) || e.Name == "ServerChange")),
            new Mechanism("readonly", "ReadOnly（ポストされた値を無視）", "ReadOnly プロパティ", t => Is<IPostBackDataHandler>(t) && Has(t, "ReadOnly"), @"ReadOnly=true"),
            new Mechanism("requestvalidation", "リクエスト検証（ValidateRequestMode）", "IPostBackDataHandler（ポスト値を読む）", t => Is<IPostBackDataHandler>(t), @"ValidateRequestMode="),
            new Mechanism("postbackevent", "ポストバックイベント", "IPostBackEventHandler", t => Is<IPostBackEventHandler>(t)),
            new Mechanism("command", "コマンドイベントと親へのバブル（CommandName）", "IButtonControl", t => Is<IButtonControl>(t), @"Command(Name|Argument)="),
            new Mechanism("submitbehavior", "送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）", "UseSubmitBehavior プロパティ", t => Has(t, "UseSubmitBehavior"), @"UseSubmitBehavior="),
            new Mechanism("clientclick", "ブラウザ側のクリック処理（OnClientClick）", "OnClientClick プロパティ", t => Has(t, "OnClientClick"), @"OnClientClick="),
            new Mechanism("crosspage", "クロスページポストバック（PostBackUrl）", "IButtonControl.PostBackUrl", t => Is<IButtonControl>(t), @"PostBackUrl="),
            new Mechanism("defaultbutton", "既定のボタン（Panel の DefaultButton）", "IButtonControl", t => Is<IButtonControl>(t)),
            new Mechanism("autopostback", "AutoPostBack（クライアントスクリプトによるポストバック）", "AutoPostBack プロパティ", t => Has(t, "AutoPostBack"), @"AutoPostBack=true"),
            new Mechanism("causesvalidation", "CausesValidation / ValidationGroup", "CausesValidation プロパティ", t => Has(t, "CausesValidation"), @"(CausesValidation|ValidationGroup)="),
            // Validated and bound by the value it posts: a control that posts none (BulletedList) has the attributes from ListControl only.
            new Mechanism("validation", "検証対象（サーバー側の検証）", "[ValidationProperty] と IPostBackDataHandler", t => t.GetCustomAttributes(typeof(ValidationPropertyAttribute), true).Length > 0 && Is<IPostBackDataHandler>(t)),
            new Mechanism("clientvalidation", "検証対象（ブラウザ側の検証）", "[ValidationProperty] と IPostBackDataHandler", t => t.GetCustomAttributes(typeof(ValidationPropertyAttribute), true).Length > 0 && Is<IPostBackDataHandler>(t)),
            new Mechanism("bind", "双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）","[ControlValueProperty] と IPostBackDataHandler", t => t.GetCustomAttributes(typeof(ControlValuePropertyAttribute), true).Length > 0 && Is<IPostBackDataHandler>(t)),
            new Mechanism("items", "項目（ListItem の Selected・Enabled・Text・Value と ViewState）", "ListControl、HtmlSelect", t => Is<ListControl>(t) || Is<HtmlSelect>(t), @"Items\["),
            new Mechanism("multiselect", "複数選択", "SelectionMode・Multiple プロパティ、CheckBoxList", t => Has(t, "SelectionMode") || Is<CheckBoxList>(t) || Is<HtmlSelect>(t), @"SelectionMode=Multiple|Multiple=true|Items\[0\]\.Selected=true\+Items\[2\]"),
            new Mechanism("repeat", "繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）", "IRepeatInfoUser", t => Is<IRepeatInfoUser>(t), @"Repeat(Layout|Columns|Direction)="),
            new Mechanism("eventvalidation", "ポスト値のイベント検証（一覧にない値・無効な項目の拒否）", "ListControl・HtmlSelect と IPostBackDataHandler", t => (Is<ListControl>(t) || Is<HtmlSelect>(t)) && Is<IPostBackDataHandler>(t)),
            new Mechanism("servervalidate", "サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）", "IValidator", t => Is<IValidator>(t)),
            new Mechanism("clientvalidate", "ブラウザ側の検証スクリプト（EnableClientScript）", "EnableClientScript プロパティ", t => Has(t, "EnableClientScript"), @"EnableClientScript="),
            new Mechanism("summary", "検証結果のまとめ（ValidationSummary）", "ValidationSummary", t => Is<ValidationSummary>(t)),
            new Mechanism("paging", "ページング", "AllowPaging プロパティ、IPageableItemContainer（ListView）、DataPager",
                t => Has(t, "AllowPaging") || t.GetInterface("IPageableItemContainer") != null || t.Name == "DataPager", @"AllowPaging=true|PageIndex=|CurrentPageIndex=|PageSize="),
            new Mechanism("sorting", "並べ替え", "AllowSorting プロパティ、Sorting イベント（ListView）", t => Has(t, "AllowSorting") || t.GetEvent("Sorting") != null, @"AllowSorting=true"),
            new Mechanism("inserting", "項目の挿入", "InsertItemTemplate・AutoGenerateInsertButton プロパティ", t => Has(t, "InsertItemTemplate") || Has(t, "AutoGenerateInsertButton"), @"Insert"),
            new Mechanism("editing", "行・項目の編集（編集・更新・取り消し・削除）", "EditIndex・EditItemIndex・DefaultMode プロパティ", t => Has(t, "EditIndex") || Has(t, "EditItemIndex") || (Has(t, "DefaultMode") && Is<CompositeDataBoundControl>(t)), @"EditIndex=|EditItemIndex=|DefaultMode="),
            new Mechanism("selection", "行・項目の選択", "データ表示コントロールの SelectedIndex", t => Has(t, "SelectedIndex") && (Is<CompositeDataBoundControl>(t) || Is<BaseDataList>(t) || t.Name == "ListView"), @"SelectedIndex="),
            new Mechanism("rowcommand", "行・項目のコマンド（RowCommand・ItemCommand へのバブル）", "*Command イベント（Command 以外）", t => t.GetEvents().Any(e => e.Name.EndsWith("Command", StringComparison.Ordinal) && e.Name != "Command")),
            new Mechanism("datakeys", "データキー（DataKeyNames・DataKeys）", "DataKeyNames プロパティ", t => Has(t, "DataKeyNames"), @"DataKeyNames="),
            new Mechanism("datasourceid", "データソースコントロールとの連携（DataSourceID）", "データ表示コントロール（Repeater・CompositeDataBoundControl・BaseDataList・ListView）",
                t => Is<Repeater>(t) || Is<CompositeDataBoundControl>(t) || Is<BaseDataList>(t) || t.Name == "ListView"),
            new Mechanism("templates", "テンプレート", "ITemplate のプロパティ", t => t.GetProperties().Any(p => p.PropertyType == typeof(ITemplate))),
            new Mechanism("datasource", "データソース連携（DataSource・DataTextField など）", "BaseDataBoundControl、HtmlSelect", t => Is<BaseDataBoundControl>(t) || Is<HtmlSelect>(t), @"\[(data-bound|fields|empty)\]"),
            new Mechanism("callback", "クライアントコールバック", "ICallbackEventHandler", t => Is<ICallbackEventHandler>(t)),
        };
    }
}
