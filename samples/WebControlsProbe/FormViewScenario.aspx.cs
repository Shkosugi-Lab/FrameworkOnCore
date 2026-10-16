using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The FormView's scenario layer (scenarios/FormView.json): a form view bound by the page (paging, modes, updating,
    /// inserting, deleting in its handlers), one bound by an ObjectDataSource (all of it automatic), an empty one, one
    /// without view state, a hidden one, a dynamic one; and a log of what the server saw (the request's number, each event
    /// with its keys and values, each form view's page and mode, the store's items).
    /// </summary>
    public partial class FormViewScenario : Page
    {
        readonly List<string> entries = new List<string>();
        FormView dynamicForm;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dynamicForm = new FormView { ID = "fvDynamic", ItemTemplate = new ProbeTemplate("dynamic"), AllowPaging = true };
            dynamicForm.PageIndexChanging += (sender, args) => { dynamicForm.PageIndex = args.NewPageIndex; BindDynamic(); };
            Dynamic.Controls.Add(dynamicForm);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack);
            if (IsPostBack) return;
            ProductStore.Reset();
            BindManual();
            fvEmpty.DataSource = new List<ProbeData.Product>();
            fvEmpty.DataBind();
            foreach (var form in new[] { fvNoViewState, fvHidden })
            {
                form.DataSource = ProbeData.Many().Take(2).ToList();
                form.DataBind();
            }
            BindDynamic();
        }

        void BindManual()
        {
            fvManual.DataSource = ProductStore.Items.ToList();
            fvManual.DataBind();
        }

        void BindDynamic()
        {
            dynamicForm.DataSource = ProbeData.Many().Take(3).ToList();
            dynamicForm.DataBind();
        }

        protected void ManualPaging(object sender, FormViewPageEventArgs e)
        {
            entries.Add("PageIndexChanging: NewPageIndex=" + e.NewPageIndex);
            fvManual.PageIndex = e.NewPageIndex;
            BindManual();
        }

        protected void ManualModeChanging(object sender, FormViewModeEventArgs e)
        {
            entries.Add("ModeChanging: NewMode=" + e.NewMode + " CancelingEdit=" + e.CancelingEdit);
            fvManual.ChangeMode(e.NewMode);
            BindManual();
        }

        protected void ManualUpdating(object sender, FormViewUpdateEventArgs e)
        {
            entries.Add("ItemUpdating: Keys=" + Values(e.Keys) + " OldValues=" + Values(e.OldValues) + " NewValues=" + Values(e.NewValues));
            var item = ProductStore.Items.First(p => p.Code == (string)e.Keys["Code"]);
            item.Name = (string)e.NewValues["Name"];
            item.Price = decimal.Parse((string)e.NewValues["Price"], NumberStyles.Number, CultureInfo.CurrentCulture);
            fvManual.ChangeMode(FormViewMode.ReadOnly);
            BindManual();
        }

        protected void ManualInserting(object sender, FormViewInsertEventArgs e)
        {
            entries.Add("ItemInserting: Values=" + Values(e.Values));
            ProductStore.Items.Add(new ProbeData.Product { Code = (string)e.Values["Code"], Name = (string)e.Values["Name"], Price = decimal.Parse((string)e.Values["Price"], NumberStyles.Number, CultureInfo.CurrentCulture) });
            fvManual.ChangeMode(FormViewMode.ReadOnly);
            BindManual();
        }

        protected void ManualDeleting(object sender, FormViewDeleteEventArgs e)
        {
            entries.Add("ItemDeleting: Keys=" + Values(e.Keys) + " Values=" + Values(e.Values));
            ProductStore.Items.RemoveAll(p => p.Code == (string)e.Keys["Code"]);
            BindManual();
        }

        protected void ItemCommand(object sender, FormViewCommandEventArgs e)
        {
            entries.Add("ItemCommand " + ((Control)sender).ID + ": " + e.CommandName + " " + e.CommandArgument);
        }

        protected void OdsUpdating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            entries.Add("ObjectDataSource Updating: " + Values(e.InputParameters));
        }

        protected void OdsInserting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            entries.Add("ObjectDataSource Inserting: " + Values(e.InputParameters));
        }

        protected void AutoUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            entries.Add("ItemUpdated: AffectedRows=" + e.AffectedRows + " Keys=" + Values(e.Keys) + " NewValues=" + Values(e.NewValues));
        }

        protected void AutoInserted(object sender, FormViewInsertedEventArgs e)
        {
            entries.Add("ItemInserted: AffectedRows=" + e.AffectedRows + " Values=" + Values(e.Values));
        }

        protected void AutoDeleted(object sender, FormViewDeletedEventArgs e)
        {
            entries.Add("ItemDeleted: AffectedRows=" + e.AffectedRows + " Keys=" + Values(e.Keys));
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            fvHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        static string Values(IDictionary values) =>
            "{" + string.Join(", ", values.Keys.Cast<object>().OrderBy(k => k.ToString(), StringComparer.Ordinal).Select(k => k + "=" + Convert.ToString(values[k], CultureInfo.InvariantCulture))) + "}";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            foreach (var form in new[] { fvManual, fvAuto, fvEmpty, fvNoViewState, fvHidden, dynamicForm })
                entries.Add("Form " + form.ID + ": PageIndex=" + form.PageIndex + " PageCount=" + form.PageCount + " CurrentMode=" + form.CurrentMode +
                            " DataKey=" + (form.DataKey == null || form.DataKey.Value == null ? "" : form.DataKey.Value) + (form.Visible ? "" : " (hidden)"));
            entries.Add("Store: " + ProductStore.Describe());
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>FormView scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
