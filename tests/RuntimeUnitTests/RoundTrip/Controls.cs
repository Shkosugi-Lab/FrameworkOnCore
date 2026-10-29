// The round-trip tests' controls: a page's tree of controls whose properties are set from a generated value (and hold it:
// StateControl), its view state saved (SaveViewStateRecursive) and loaded into a new tree on a postback
// (LoadViewStateRecursive), as a page does between two requests. Compiled into the .NET Framework 4.8 recorder and into the
// port's tests.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using FrameworkOnCore.Runtime.Tests.Fuzz;

namespace FrameworkOnCore.Runtime.Tests.RoundTrip
{
    /// <summary>An application's control that keeps a value in its view state.</summary>
    public class StateControl : WebControl
    {
        public object Value { get { return ViewState["v"]; } set { ViewState["v"] = value; } }
    }

    public static class ControlTree
    {
        const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>The tree: its controls without values (as a postback makes it before loading the view state), or with them.</summary>
        public static Panel Build(int seed, object value, bool withValues)
        {
            var root = new Panel { ID = "root" };
            var label = new Label { ID = "l" };
            var box = new TextBox { ID = "t" };
            var list = new DropDownList { ID = "d" };
            var check = new CheckBox { ID = "c" };
            var hidden = new HiddenField { ID = "h" };
            var link = new HyperLink { ID = "a" };
            var calendar = new Calendar { ID = "cal" };
            var state = new StateControl { ID = "s" };
            var controls = new Control[] { label, box, list, check, hidden, link, calendar, state };
            foreach (var c in controls) root.Controls.Add(c);
            if (!withValues) return root;
            Track(root);
            foreach (var c in controls) Track(c);
            var r = new Rng(0x436F6E74726F6CUL + (ulong)seed * 0x9E3779B97F4A7C15UL);
            label.Text = Values.Text(r);
            label.ForeColor = r.Chance(50) ? Color.FromArgb((int)r.Next()) : Color.FromKnownColor((KnownColor)(1 + r.Below(174)));
            label.Width = new Unit(r.Below(1000) / 4.0, (UnitType)(1 + r.Below(9)));
            label.ToolTip = Gen.Text(r, 3);
            label.CssClass = "c" + Gen.Text(r, 2);
            label.Font.Size = r.Chance(50) ? new FontUnit((FontSize)r.Below(11)) : FontUnit.Point(r.Below(72));
            label.Font.Bold = r.Chance(50);
            label.Font.Names = new[] { "Arial", "ＭＳ ゴシック" };
            label.Attributes["data-x"] = Gen.Text(r, 3);
            box.Text = Gen.Text(r, 3);
            box.MaxLength = r.Below(100);
            box.ReadOnly = r.Chance(50);
            box.TextMode = (TextBoxMode)r.Below(3);
            int items = r.Small(6);
            for (int i = 0; i < items; i++) list.Items.Add(new ListItem(Gen.Text(r, 2), Gen.Text(r, 2)) { Enabled = r.Chance(80) });
            if (items > 0) list.SelectedIndex = r.Below(items);
            check.Checked = r.Chance(50);
            check.Text = Gen.Text(r, 2);
            hidden.Value = Gen.Text(r, 4);
            link.NavigateUrl = "~/" + Gen.Path(r);
            link.Text = Gen.Text(r, 2);
            calendar.VisibleDate = new DateTime(2000 + r.Below(50), 1 + r.Below(12), 1);
            int dates = r.Small(3);
            for (int i = 0; i < dates; i++) calendar.SelectedDates.Add(new DateTime(2000 + r.Below(50), 1 + r.Below(12), 1 + r.Below(28)));
            calendar.SelectedDayStyle.BackColor = Color.FromArgb((int)r.Next());
            state.Value = value;
            state.BackColor = Color.Red;
            state.Height = Unit.Percentage(r.Below(1000) / 10.0);
            root.GroupingText = Gen.Text(r, 2);
            return root;
        }

        static void Track(Control c) => c.GetType().GetMethod("TrackViewState", Instance, null, Type.EmptyTypes, null).Invoke(c, null);

        /// <summary>The tree's view state as a page saves it (its controls' state: SaveViewStateRecursive).</summary>
        public static object Save(Control root) =>
            Formats.Invoke(typeof(Control).GetMethod("SaveViewStateRecursive", Instance), root, new object[] { ViewStateMode.Enabled });

        /// <summary>A new tree loaded with a view state on a postback (a page whose IsPostBack is true).</summary>
        public static Panel Load(object state)
        {
            var page = new Page();
            typeof(Page).GetField("_requestValueCollection", Instance).SetValue(page, new System.Collections.Specialized.NameValueCollection());
            typeof(Page).GetField("_isCrossPagePostBack", Instance).SetValue(page, true);
            var root = Build(0, null, false);
            page.Controls.Add(root);
            Formats.Invoke(typeof(Control).GetMethod("LoadViewStateRecursive", Instance), root, new[] { state });
            return root;
        }

        /// <summary>The tree's properties (what its view state keeps).</summary>
        public static string Show(Control root)
        {
            var sb = new StringBuilder("Tree{");
            var panel = (Panel)root;
            sb.Append("grouping=").Append(RoundTrip.Show.Of(panel.GroupingText));
            var label = (Label)root.Controls[0];
            sb.Append("; label=").Append(RoundTrip.Show.Of(new object[] { label.Text, label.ForeColor, label.Width, label.ToolTip, label.CssClass, label.Font.Size, label.Font.Bold,
                string.Join(",", label.Font.Names), label.Attributes["data-x"] }));
            var box = (TextBox)root.Controls[1];
            sb.Append("; box=").Append(RoundTrip.Show.Of(new object[] { box.Text, box.MaxLength, box.ReadOnly, box.TextMode.ToString() }));
            var list = (DropDownList)root.Controls[2];
            sb.Append("; list=").Append(RoundTrip.Show.Of(list.Items.Cast<ListItem>().Select(i => i.Text + "=" + i.Value + (i.Enabled ? "" : "/disabled")).ToArray()));
            var check = (CheckBox)root.Controls[3];
            sb.Append("; check=").Append(RoundTrip.Show.Of(new object[] { check.Checked, check.Text }));
            sb.Append("; hidden=").Append(RoundTrip.Show.Of(((HiddenField)root.Controls[4]).Value));
            var link = (HyperLink)root.Controls[5];
            sb.Append("; link=").Append(RoundTrip.Show.Of(new object[] { link.NavigateUrl, link.Text }));
            var calendar = (Calendar)root.Controls[6];
            sb.Append("; calendar=").Append(RoundTrip.Show.Of(new object[] { calendar.VisibleDate, calendar.SelectedDates.Cast<DateTime>().ToArray(), calendar.SelectedDayStyle.BackColor }));
            var state = (StateControl)root.Controls[7];
            sb.Append("; state=").Append(RoundTrip.Show.Of(new object[] { state.Value, state.BackColor, state.Height }));
            return sb.Append('}').ToString();
        }
    }
}
