using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The list controls' scenario layer (scenarios/DropDownList.json, ListBox.json, CheckBoxList.json, RadioButtonList.json,
    /// BulletedList.json, all on this page): each mechanism's lists, and a log of what the server saw (the request's
    /// number, kept in view state: a postback the browser held back is no request; each SelectedIndexChanged and bullet
    /// click in the order raised, the values two-way binding extracted, every list's selection at the end, whether the
    /// page is valid). A post the server rejects (a value the list does not have, markup) is written as the exception's type.
    /// </summary>
    public partial class ListControlsScenario : Page
    {
        readonly List<string> entries = new List<string>();
        ListControl[] dynamicLists;

        public sealed class BindRow
        {
            public string Ddl { get; set; }
            public string Lst { get; set; }
            public string Rbl { get; set; }
            public string Cbl { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Made on every request, before the posted data is loaded; their items come back from view state.
            dynamicLists = new ListControl[]
            {
                new DropDownList { ID = "ddlDynamic" }, new ListBox { ID = "lstDynamic", Rows = 2 }, new CheckBoxList { ID = "cblDynamic", RepeatLayout = RepeatLayout.Flow },
                new RadioButtonList { ID = "rblDynamic", RepeatLayout = RepeatLayout.Flow }, new BulletedList { ID = "bltDynamic", DisplayMode = BulletedListDisplayMode.LinkButton },
            };
            foreach (var list in dynamicLists)
            {
                if (list is BulletedList) ((BulletedList)list).Click += BulletClicked;
                else list.SelectedIndexChanged += Changed;
                Dynamic.Controls.Add(list);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack);
            if (IsPostBack) return;
            ddlBound.DataSource = ProbeData.Products();
            ddlBound.DataBind();
            bltBound.DataSource = ProbeData.Products();
            bltBound.DataBind();
            foreach (var list in dynamicLists)
            {
                list.Items.Add(new ListItem("One", "1"));
                list.Items.Add(new ListItem("Two", "2"));
            }
            // Items added in code to hidden lists: they come back from view state when the lists are shown.
            foreach (var hidden in new ListControl[] { ddlHidden, lstHidden, cblHidden, rblHidden, bltHidden })
            {
                hidden.Items.Add(new ListItem("Kept 1", "k1"));
                hidden.Items.Add(new ListItem("Kept 2", "k2") { Selected = !(hidden is BulletedList) });
            }
            Bind(new BindRow { Ddl = "B2", Lst = "B2", Rbl = "B2", Cbl = "B2" });
        }

        void Bind(BindRow row)
        {
            fvProduct.DataSource = new[] { row };
            fvProduct.DataBind();
        }

        protected void Changed(object sender, EventArgs e)
        {
            entries.Add("SelectedIndexChanged " + Describe((ListControl)sender));
        }

        protected void BulletClicked(object sender, BulletedListEventArgs e)
        {
            entries.Add("Click " + ((Control)sender).ID + " index " + e.Index);
        }

        protected void ChecksValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = cblValidated.Items[0].Selected;
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            foreach (var hidden in new ListControl[] { ddlHidden, lstHidden, cblHidden, rblHidden, bltHidden }) hidden.Visible = true;
            entries.Add("Click btnShow");
        }

        protected void Updating(object sender, FormViewUpdateEventArgs e)
        {
            entries.Add("ItemUpdating: " + string.Join(", ", e.NewValues.Keys.Cast<object>().Select(k => k + "=" + e.NewValues[k])));
            Bind(new BindRow { Ddl = (string)e.NewValues["Ddl"], Lst = (string)e.NewValues["Lst"], Rbl = (string)e.NewValues["Rbl"], Cbl = (string)e.NewValues["Cbl"] });
        }

        protected void ModeChanging(object sender, FormViewModeEventArgs e)
        {
        }

        static string Describe(ListControl list)
        {
            string selected;
            try { selected = list.SelectedValue; }
            catch (NotSupportedException) { selected = "(not supported)"; }
            return list.ID + ": SelectedIndex=" + (list is BulletedList ? "-" : list.SelectedIndex.ToString()) + " SelectedValue=\"" + selected +
                   "\" selected [" + string.Join(",", list.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value)) + "] items " + list.Items.Count;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (IsPostBack)
            {
                string valid;
                try { valid = IsValid.ToString(); }
                catch (HttpException) { valid = "(not validated)"; }
                entries.Add("IsValid: " + valid);
            }
            foreach (var list in new ListControl[] { ddlSize, ddlAuto, ddlBound, ddlNoViewState, lstMulti, lstAuto, cblColors, cblAuto, rblSize, rblAuto, ddlValidated, lstValidated, cblValidated, ddlClient, lstClient, rblClient }.Concat(dynamicLists).Concat(new ListControl[] { ddlHidden, lstHidden, cblHidden, rblHidden, bltHidden, bltBound }))
                entries.Add("List " + Describe(list) + (list.Visible ? "" : " (hidden)"));
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>List controls scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
