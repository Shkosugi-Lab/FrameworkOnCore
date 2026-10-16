using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The data source controls' scenario layer (scenarios/ObjectDataSource.json, SqlDataSource.json, AccessDataSource.json,
    /// XmlDataSource.json, LinqDataSource.json, QueryExtender.json, ModelErrorMessage.json): grids and a details view bound
    /// to them, a row edited and updated, one deleted, one inserted, sorted and paged by the data source, filtered by a
    /// control's value (a where parameter, a query extender's search), a model error shown; an Access data source selected
    /// in code (it needs the Jet provider: what it says is logged); ones made in code; a log of the data source events.
    /// ?reset=1 puts the products back as they were (the session's, the in-memory database's).
    /// </summary>
    public partial class DataSourceScenario : Page
    {
        readonly List<string> entries = new List<string>();
        ObjectDataSource dynamicSource;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (Request.QueryString["reset"] == "1" && !IsPostBack)
            {
                ProductStore.Reset();
                ProbeDbFactory.Reset();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Made on every request: a data source and a grid bound to it by ID.
            dynamicSource = new ObjectDataSource { ID = "odsDynamic", TypeName = "WebControlsProbe.ProductStore", SelectMethod = "SelectSorted", SortParameterName = "sortExpression" };
            dynamicSource.Selected += ObjectSelected;
            Dynamic.Controls.Add(dynamicSource);
            Dynamic.Controls.Add(new GridView { ID = "gvDynamic", DataSourceID = "odsDynamic", AllowSorting = true });
            var xml = new XmlDataSource { ID = "xdsDynamic", DataFile = "~/App_Data/products.xml", XPath = "products/product[@price > 2]" };
            Dynamic.Controls.Add(xml);
            Dynamic.Controls.Add(new GridView { ID = "gvDynamicXml", DataSourceID = "xdsDynamic" });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack);
            if (IsPostBack) return;
            DataBind();
            // The Access data source: OLE DB's Jet provider, which the machine may not have.
            try
            {
                var rows = adsProducts.Select(DataSourceSelectArguments.Empty);
                entries.Add("AccessDataSource rows=" + (rows == null ? "null" : rows.Cast<object>().Count().ToString()));
            }
            // Without the provider (64-bit Windows has no Jet; ACE is installed apart) .NET Framework and the port on Windows
            // say so (InvalidOperationException); on Linux OLE DB itself is missing (PlatformNotSupportedException): the
            // same outcome, logged as one. Anything else, as it is.
            catch (Exception error) when (error is InvalidOperationException || error is PlatformNotSupportedException)
            {
                entries.Add("AccessDataSource: no OLE DB provider for Access files");
            }
            catch (Exception error)
            {
                entries.Add("AccessDataSource " + error.GetType().FullName);
            }
        }

        /// <summary>A data source event's sender: its view (the data source's ID is the view's owner's, not the view's).</summary>
        static string Source(object sender) => sender is Control control ? control.ID : sender.GetType().Name;

        static string Parameters(IDictionary values) =>
            values == null ? "" : string.Join(", ", values.Keys.Cast<object>().OrderBy(k => k.ToString(), StringComparer.Ordinal).Select(k => k + "=" + values[k]));

        protected void ObjectSelecting(object sender, ObjectDataSourceSelectingEventArgs e) => entries.Add("ObjectDataSource Selecting " + Source(sender) + " ExecutingSelectCount=" + e.ExecutingSelectCount);

        protected void ObjectSelected(object sender, ObjectDataSourceStatusEventArgs e) =>
            entries.Add("ObjectDataSource Selected " + Source(sender) + (e.Exception == null ? " rows=" + ((e.ReturnValue as IEnumerable)?.Cast<object>().Count() ?? -1) : " " + e.Exception.GetType().Name));

        protected void ObjectUpdating(object sender, ObjectDataSourceMethodEventArgs e) => entries.Add("ObjectDataSource Updating " + Parameters(e.InputParameters));

        protected void ObjectUpdated(object sender, ObjectDataSourceStatusEventArgs e) => entries.Add("ObjectDataSource Updated");

        protected void ObjectDeleted(object sender, ObjectDataSourceStatusEventArgs e) => entries.Add("ObjectDataSource Deleted");

        protected void SqlSelecting(object sender, SqlDataSourceSelectingEventArgs e) => entries.Add("SqlDataSource Selecting " + e.Command.CommandText + " SortExpression=" + e.Arguments.SortExpression);

        protected void SqlSelected(object sender, SqlDataSourceStatusEventArgs e) => entries.Add("SqlDataSource Selected rows=" + e.AffectedRows + (e.Exception == null ? "" : " " + e.Exception.GetType().Name + ": " + e.Exception.Message));

        static string Command(SqlDataSourceStatusEventArgs e) =>
            e.Command.CommandText + " (" + string.Join(", ", e.Command.Parameters.Cast<System.Data.Common.DbParameter>().Select(p => p.ParameterName + "=" + p.Value)) + ") rows=" + e.AffectedRows +
            (e.Exception == null ? "" : " " + e.Exception.GetType().Name + ": " + e.Exception.Message);

        protected void SqlUpdated(object sender, SqlDataSourceStatusEventArgs e) => entries.Add("SqlDataSource Updated " + Command(e));

        protected void SqlInserted(object sender, SqlDataSourceStatusEventArgs e) => entries.Add("SqlDataSource Inserted " + Command(e));

        protected void SqlDeleted(object sender, SqlDataSourceStatusEventArgs e) => entries.Add("SqlDataSource Deleted " + Command(e));

        protected void LinqSelecting(object sender, LinqDataSourceSelectEventArgs e) =>
            entries.Add("LinqDataSource Selecting " + string.Join(", ", e.WhereParameters.OrderBy(p => p.Key, StringComparer.Ordinal).Select(p => p.Key + "=" + p.Value)));

        protected void CheckModel(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtModelName.Text)) ModelState.AddModelError("Name", "The name <b>is</b> required.");
            entries.Add("Model valid=" + ModelState.IsValid);
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            // A data source control cannot be hidden (it renders nothing): a grid bound to one, hidden, is shown.
            gvHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            entries.Add("State " + ProductStore.Describe());
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>Data source scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
