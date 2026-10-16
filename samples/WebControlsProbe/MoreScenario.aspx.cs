using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The scenario layer of the table parts, FileUpload, MultiView and View, Localize, Substitution, Xml, AdRotator, Calendar,
    /// ImageMap and LiteralControl (scenarios/*.json naming this page): each control plain, data-bound, set in code (kept in
    /// view state, or not: a table's rows added in code, a literal control's text), hidden and shown later, made dynamically;
    /// a file posted, the views switched by their commands, a calendar's day and month chosen, an image map's hot spot
    /// clicked; and a log of what the server saw (the request's number, kept in view state; the events in the order raised;
    /// every control's state at the end). A post the server rejects is written as the exception's type and message.
    /// </summary>
    public partial class MoreScenario : Page
    {
        static readonly DateTime Today = new DateTime(2020, 1, 15);
        readonly List<string> entries = new List<string>();
        readonly List<Control> madeInCode = new List<Control>();
        LiteralControl literal;

        protected string BoundText => "bound";

        /// <summary>The Substitution's callback (its MethodName): a static method of the page's class.</summary>
        public static string SubstitutionText(HttpContext context) => "substituted <b>" + context.Request.HttpMethod + "</b>";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Today, for every calendar, the same on every run.
            foreach (var calendar in new[] { calPlain, calBound, calHidden }) calendar.TodaysDate = Today;
            // Made on every request, before the posted data is loaded.
            var table = new Table { ID = "tblDynamic", GridLines = GridLines.Both };
            var row = new TableHeaderRow { ID = "thrDynamic" };
            row.Cells.Add(new TableHeaderCell { ID = "thDynamic", Text = "Dynamic header" });
            table.Rows.Add(row);
            var body = new TableRow { ID = "trDynamic" };
            body.Cells.Add(new TableCell { ID = "tdDynamic" });
            table.Rows.Add(body);
            var footer = new TableFooterRow { ID = "tfrDynamic" };
            footer.Cells.Add(new TableCell { ID = "tdDynamicFoot", Text = "Dynamic foot" });
            table.Rows.Add(footer);
            var views = new MultiView { ID = "mvDynamic" };
            views.ActiveViewChanged += ViewChanged;
            foreach (var name in new[] { "One", "Two" })
            {
                var view = new View { ID = "vwDynamic" + name };
                view.Controls.Add(new Literal { Text = "Dynamic view " + name });
                view.Controls.Add(new Button { ID = "btnDynamic" + name, Text = "Next of " + name, CommandName = "NextView", CausesValidation = false });
                views.Views.Add(view);
            }
            views.ActiveViewIndex = 0;
            var calendar2 = new Calendar { ID = "calDynamic", TodaysDate = Today, VisibleDate = Today };
            calendar2.SelectionChanged += DateSelected;
            var map = new ImageMap { ID = "imapDynamic", ImageUrl = "~/Images/probe.png", HotSpotMode = HotSpotMode.PostBack };
            map.HotSpots.Add(new RectangleHotSpot { Left = 0, Top = 0, Right = 20, Bottom = 10, PostBackValue = "dynamic" });
            map.Click += HotSpotClicked;
            var rotator = new AdRotator { ID = "adDynamic", AdvertisementFile = "~/App_Data/ads.xml" };
            rotator.AdCreated += AdCreated;
            literal = new LiteralControl("dynamic literal") { ID = "litDynamic" };
            madeInCode.AddRange(new Control[]
            {
                table, new FileUpload { ID = "fupDynamic" }, views, new Localize { ID = "locDynamic", Text = "dynamic localize" },
                new Substitution { ID = "subDynamic", MethodName = "SubstitutionText" }, new Xml { ID = "xmlDynamic", DocumentSource = "~/App_Data/probe.xml" },
                rotator, calendar2, map, literal,
            });
            foreach (var control in madeInCode) Dynamic.Controls.Add(control);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack);
            if (IsPostBack) return;
            // Set in code on the first request only: kept by view state, or not.
            tdCode.Text = "set in code";
            tdHidden.Text = "set while hidden";
            var row = new TableRow();
            row.Cells.Add(new TableCell { Text = "row added in code" });
            tblCode.Rows.Add(row);
            locCode.Text = "set in code";
            locHidden.Text = "set while hidden";
            xmlCode.DocumentContent = "<items><item>set in code</item></items>";
            calPlain.VisibleDate = Today;
            calBound.VisibleDate = Today;
            adBound.DataSource = ProbeData.Many().Take(1).ToList();
            literal.Text = "dynamic literal (set in code)";
            ((TableRow)((Table)madeInCode[0]).Rows[1]).Cells[0].Text = "dynamic cell (set in code)";
            DataBind();
        }

        protected void ViewChanged(object sender, EventArgs e)
        {
            var views = (MultiView)sender;
            entries.Add("ActiveViewChanged " + views.ID + " ActiveViewIndex=" + views.ActiveViewIndex);
        }

        protected void ViewActivated(object sender, EventArgs e) => entries.Add("Activate " + ((Control)sender).ID);

        protected void ViewDeactivated(object sender, EventArgs e) => entries.Add("Deactivate " + ((Control)sender).ID);

        protected void AdCreated(object sender, AdCreatedEventArgs e) =>
            entries.Add("AdCreated " + ((Control)sender).ID + " ImageUrl=\"" + e.ImageUrl + "\" NavigateUrl=\"" + e.NavigateUrl + "\" AlternateText=\"" + e.AlternateText + "\" Properties=" + (e.AdProperties == null ? 0 : e.AdProperties.Count));

        protected void DateSelected(object sender, EventArgs e)
        {
            var calendar = (Calendar)sender;
            entries.Add("SelectionChanged " + calendar.ID + " " + string.Join(",", calendar.SelectedDates.Cast<DateTime>().Select(d => d.ToString("yyyy-MM-dd"))));
        }

        protected void MonthChanged(object sender, MonthChangedEventArgs e) =>
            entries.Add("VisibleMonthChanged " + ((Control)sender).ID + " " + e.PreviousDate.ToString("yyyy-MM") + " -> " + e.NewDate.ToString("yyyy-MM"));

        protected void DayRender(object sender, DayRenderEventArgs e)
        {
            if (e.Day.Date.Day == 20 && !e.Day.IsOtherMonth) e.Cell.Controls.Add(new LiteralControl(" *"));
        }

        protected void HotSpotClicked(object sender, ImageMapEventArgs e) => entries.Add("Click " + ((Control)sender).ID + ": " + e.PostBackValue);

        protected void ShowHidden(object sender, EventArgs e)
        {
            foreach (var control in new Control[] { tdHidden, tblHidden, fupHidden, mvHidden, locHidden, adHidden, calHidden, imapHidden })
                control.Visible = true;
            entries.Add("Click btnShow");
        }

        static string Describe(Control control)
        {
            var visible = control.Visible ? "" : " (hidden)";
            switch (control)
            {
                case Table table:
                    return table.ID + " Rows=" + table.Rows.Count + " [" + string.Join("|", table.Rows.Cast<TableRow>().Select(r => r.GetType().Name + ":" + string.Join(",", r.Cells.Cast<TableCell>().Select(c => c.Text)))) + "]" + visible;
                case TableCell cell: return cell.ID + " Text=\"" + cell.Text + "\"" + visible;
                case FileUpload upload:
                    // FileBytes reads the posted stream (once).
                    var bytes = upload.HasFile ? upload.FileBytes : null;
                    return upload.ID + " HasFile=" + upload.HasFile + " FileName=\"" + upload.FileName + "\" Files=" + upload.PostedFiles.Count +
                        (bytes == null ? "" : " " + bytes.Length + " bytes \"" + System.Text.Encoding.UTF8.GetString(bytes) + "\"") + visible;
                case MultiView views: return views.ID + " ActiveViewIndex=" + views.ActiveViewIndex + " Active=" + (views.GetActiveView() == null ? "-" : views.GetActiveView().ID) + visible;
                case Localize localize: return localize.ID + " Text=\"" + localize.Text + "\"" + visible;
                case Xml xml: return xml.ID + " DocumentContent=\"" + xml.DocumentContent + "\" DocumentSource=\"" + xml.DocumentSource + "\"" + visible;
                case AdRotator rotator: return rotator.ID + " AdvertisementFile=\"" + rotator.AdvertisementFile + "\"" + visible;
                case Calendar calendar:
                    return calendar.ID + " VisibleDate=" + calendar.VisibleDate.ToString("yyyy-MM-dd") + " SelectedDate=" + calendar.SelectedDate.ToString("yyyy-MM-dd") +
                        " Selected=" + calendar.SelectedDates.Count + visible;
                case ImageMap map: return map.ID + " HotSpots=" + map.HotSpots.Count + " AlternateText=\"" + map.AlternateText + "\"" + visible;
                case LiteralControl text: return text.ID + " Text=\"" + text.Text + "\"" + visible;
                case TextBox box: return box.ID + " Text=\"" + box.Text + "\"" + visible;
                case Label label: return label.ID + " Text=\"" + label.Text + "\"" + visible;
                default: return control.ID + visible;
            }
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
            foreach (var control in new Control[]
            {
                tblPlain, thBound, tdBound, tdCode, tdHidden, tblCode, tblHidden, fupPlain, fupMulti, fupDisabled, fupHidden, mvSteps, txtInFirst, lblInSecond, mvHidden,
                locPlain, locBound, locCode, locHidden, xmlPlain, xmlBound, xmlCode, adPlain, adBound, adHidden, calPlain, calBound, calHidden, imapPlain, imapBound, imapHidden,
            }.Concat(madeInCode))
                entries.Add("State " + Describe(control));
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>More controls scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
