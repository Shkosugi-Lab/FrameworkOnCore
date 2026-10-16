using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The DataList's scenario layer (scenarios/DataList.json): a data list bound by the page (selecting, editing, updating,
    /// deleting in its command handlers, its data keys), one bound by an ObjectDataSource, an empty one, one without view
    /// state, a hidden one, a dynamic one; and a log of what the server saw.
    /// </summary>
    public partial class DataListScenario : Page
    {
        readonly List<string> entries = new List<string>();
        DataList dynamicList;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dynamicList = new DataList { ID = "dlDynamic", ItemTemplate = new ProbeTemplate("dynamic"), RepeatColumns = 2 };
            Dynamic.Controls.Add(dynamicList);
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
            dlEmpty.DataSource = new List<ProbeData.Product>();
            dlEmpty.DataBind();
            foreach (var list in new[] { dlNoViewState, dlHidden, dynamicList })
            {
                list.DataSource = ProbeData.Many().Take(3).ToList();
                list.DataBind();
            }
        }

        void BindManual()
        {
            dlManual.DataSource = ProductStore.Items.Take(5).ToList();
            dlManual.DataBind();
        }

        protected void ItemCommand(object source, DataListCommandEventArgs e)
        {
            entries.Add("ItemCommand: " + e.CommandName + " " + e.CommandArgument + " item " + e.Item.ItemIndex + " (" + e.Item.ItemType + ")");
        }

        protected void EditCommand(object source, DataListCommandEventArgs e)
        {
            entries.Add("EditCommand: item " + e.Item.ItemIndex + " key " + dlManual.DataKeys[e.Item.ItemIndex]);
            dlManual.EditItemIndex = e.Item.ItemIndex;
            BindManual();
        }

        protected void UpdateCommand(object source, DataListCommandEventArgs e)
        {
            var name = ((TextBox)e.Item.FindControl("txtDlName")).Text;
            var key = (string)dlManual.DataKeys[e.Item.ItemIndex];
            entries.Add("UpdateCommand: item " + e.Item.ItemIndex + " key " + key + " name \"" + name + "\"");
            ProductStore.Items.First(p => p.Code == key).Name = name;
            dlManual.EditItemIndex = -1;
            BindManual();
        }

        protected void CancelCommand(object source, DataListCommandEventArgs e)
        {
            entries.Add("CancelCommand: item " + e.Item.ItemIndex);
            dlManual.EditItemIndex = -1;
            BindManual();
        }

        protected void DeleteCommand(object source, DataListCommandEventArgs e)
        {
            var key = (string)dlManual.DataKeys[e.Item.ItemIndex];
            entries.Add("DeleteCommand: item " + e.Item.ItemIndex + " key " + key);
            ProductStore.Items.RemoveAll(p => p.Code == key);
            BindManual();
        }

        protected void SelectionChanged(object sender, EventArgs e)
        {
            entries.Add("SelectedIndexChanged: SelectedIndex=" + dlManual.SelectedIndex + " SelectedValue=" + dlManual.SelectedValue);
            BindManual();
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            dlHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            foreach (var list in new[] { dlManual, dlAuto, dlEmpty, dlNoViewState, dlHidden, dynamicList })
                entries.Add("DataList " + list.ID + ": Items=" + list.Items.Count + " EditItemIndex=" + list.EditItemIndex + " SelectedIndex=" + list.SelectedIndex + (list.Visible ? "" : " (hidden)"));
            entries.Add("Store: " + ProductStore.Describe());
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>DataList scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
