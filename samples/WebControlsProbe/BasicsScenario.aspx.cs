using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The basic controls' scenario layer (scenarios/Label.json, CheckBox.json, RadioButton.json, Literal.json,
    /// LinkButton.json, HyperLink.json, Image.json, ImageButton.json, Panel.json, HiddenField.json, PlaceHolder.json, all
    /// on this page): each control plain, data-bound, set in code (kept in view state), set in code without view state,
    /// hidden and shown later, made dynamically; and a log of what the server saw (the request's number, kept in view
    /// state: a click the browser cancelled is no request; each event in the order raised, an image click's coordinates,
    /// the values two-way binding extracted, every control's value at the end). A post the server rejects is written as
    /// the exception's type and message.
    /// </summary>
    public partial class BasicsScenario : Page
    {
        readonly List<string> entries = new List<string>();
        Label dynamicLabel;
        Literal dynamicLiteral;
        CheckBox dynamicCheck;
        RadioButton dynamicRadio;
        HiddenField dynamicHidden;
        LinkButton dynamicLink;
        HyperLink dynamicHyperLink;
        Image dynamicImage;
        ImageButton dynamicImageButton;
        Panel dynamicPanel;
        TextBox textInHolder;

        public sealed class BindRow
        {
            public string Name { get; set; }
            public bool Flag { get; set; }
            public bool Choice { get; set; }
            public string Secret { get; set; }
        }

        protected string BoundText => "bound";

        protected bool BoundFlag => true;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Made on every request, before the posted data is loaded: they get their posted values and view state as ones in markup.
            dynamicLabel = new Label { ID = "lblDynamic" };
            dynamicLiteral = new Literal { ID = "litDynamic" };
            dynamicCheck = new CheckBox { ID = "chkDynamic", Text = "Dynamic" };
            dynamicCheck.CheckedChanged += Changed;
            dynamicRadio = new RadioButton { ID = "rdoDynamic", Text = "Dynamic", GroupName = "dynamic" };
            dynamicRadio.CheckedChanged += Changed;
            dynamicHidden = new HiddenField { ID = "hidDynamic" };
            dynamicHidden.ValueChanged += Changed;
            dynamicLink = new LinkButton { ID = "lnkDynamic", Text = "Dynamic link", CausesValidation = false };
            dynamicLink.Click += Clicked;
            dynamicHyperLink = new HyperLink { ID = "hypDynamic", NavigateUrl = "~/ButtonTarget.aspx" };
            dynamicImage = new Image { ID = "imgDynamic", ImageUrl = "~/Images/probe.png" };
            dynamicImageButton = new ImageButton { ID = "ibtnDynamic", ImageUrl = "~/Images/probe.png", AlternateText = "Dynamic image", CausesValidation = false };
            dynamicImageButton.Click += ImageClicked;
            dynamicPanel = new Panel { ID = "pnlDynamic" };
            dynamicPanel.Controls.Add(new Label { ID = "lblInDynamicPanel", Text = "In a dynamic panel" });
            foreach (var control in new Control[] { dynamicLabel, dynamicLiteral, dynamicCheck, dynamicRadio, dynamicHidden, dynamicLink, dynamicHyperLink, dynamicImage, dynamicImageButton, dynamicPanel })
                Dynamic.Controls.Add(control);
            textInHolder = new TextBox { ID = "txtInHolder" };
            phCreated.Controls.Add(new Literal { Text = "Created in a place holder: " });
            phCreated.Controls.Add(textInHolder);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack);
            if (IsPostBack) return;
            // Set in code on the first request only: kept by view state, lost without it.
            lblCode.Text = "set in code";
            lblNoViewState.Text = "set in code, no view state";
            lblHidden.Text = "set while hidden";
            litCode.Text = "set in code";
            litNoViewState.Text = "set in code, no view state";
            litHidden.Text = "set while hidden";
            chkNoViewState.Checked = true;
            chkHidden.Checked = true;
            rdoHidden.Checked = true;
            hidNoViewState.Value = "set in code, no view state";
            hidHidden.Value = "set while hidden";
            lnkHidden.Text = "Hidden link (set while hidden)";
            hypHidden.Text = "Hidden hyperlink";
            hypHidden.NavigateUrl = "~/ButtonTarget.aspx?hidden=1";
            imgCode.AlternateText = "set in code";
            imgHidden.AlternateText = "set while hidden";
            ibtnHidden.AlternateText = "Hidden image (set while hidden)";
            txtInHiddenPanel.Text = "set while hidden";
            lblInHiddenHolder.Text = "set while hidden";
            dynamicLabel.Text = "dynamic label";
            dynamicLiteral.Text = "dynamic literal";
            dynamicHidden.Value = "dynamic";
            dynamicHyperLink.Text = "dynamic hyperlink";
            dynamicImage.AlternateText = "dynamic image";
            rptItems.DataSource = new[] { "Pen", "Bag" };
            fvBasics.DataSource = new[] { new BindRow { Name = "Taro", Flag = true, Choice = false, Secret = "s1" } };
            DataBind();
        }

        protected void Changed(object sender, EventArgs e)
        {
            entries.Add("Changed " + Describe((Control)sender));
        }

        protected void Clicked(object sender, EventArgs e)
        {
            entries.Add("Click " + ((Control)sender).ID);
        }

        protected void ImageClicked(object sender, ImageClickEventArgs e)
        {
            entries.Add("Click " + ((Control)sender).ID + " at " + e.X + "," + e.Y);
        }

        protected void Commanded(object sender, CommandEventArgs e)
        {
            entries.Add("Command " + ((Control)sender).ID + ": " + e.CommandName + " " + e.CommandArgument);
        }

        protected void ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            entries.Add("ItemCommand " + ((Control)source).ID + " item " + e.Item.ItemIndex + ": " + e.CommandName + " " + e.CommandArgument + " (from " + ((Control)e.CommandSource).ID + ")");
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            foreach (var control in new Control[] { lblHidden, litHidden, chkHidden, rdoHidden, hidHidden, lnkHidden, hypHidden, imgHidden, ibtnHidden, pnlHidden, phHidden })
                control.Visible = true;
            entries.Add("Click btnShow");
        }

        protected void Updating(object sender, FormViewUpdateEventArgs e)
        {
            entries.Add("ItemUpdating: " + string.Join(", ", e.NewValues.Keys.Cast<object>().OrderBy(k => k.ToString(), StringComparer.Ordinal).Select(k => k + "=" + e.NewValues[k])));
            fvBasics.DataSource = new[] { new BindRow { Name = (string)e.NewValues["Name"], Flag = (bool)e.NewValues["Flag"], Choice = (bool)e.NewValues["Choice"], Secret = (string)e.NewValues["Secret"] } };
            fvBasics.DataBind();
        }

        protected void ModeChanging(object sender, FormViewModeEventArgs e)
        {
        }

        static string Describe(Control control)
        {
            var visible = control.Visible ? "" : " (hidden)";
            switch (control)
            {
                case CheckBox check: return check.ID + " Checked=" + check.Checked + visible;
                case HiddenField hidden: return hidden.ID + " Value=\"" + hidden.Value + "\"" + visible;
                case Label label: return label.ID + " Text=\"" + label.Text + "\"" + visible;
                case Literal literal: return literal.ID + " Text=\"" + literal.Text + "\"" + visible;
                case LinkButton link: return link.ID + " Text=\"" + link.Text + "\"" + visible;
                case HyperLink hyperLink: return hyperLink.ID + " Text=\"" + hyperLink.Text + "\" NavigateUrl=\"" + hyperLink.NavigateUrl + "\"" + visible;
                case Image image: return image.ID + " AlternateText=\"" + image.AlternateText + "\"" + visible;
                case TextBox text: return text.ID + " Text=\"" + text.Text + "\"" + visible;
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
                lblPlain, lblBound, lblCode, lblNoViewState, lblHidden, litBound, litCode, litNoViewState, litHidden,
                chkPlain, chkAuto, chkCauses, chkDisabled, chkBound, chkNoViewState, chkHidden, rdoSmall, rdoLarge, rdoAutoA, rdoAutoB, rdoCauses, rdoBound, rdoHidden,
                hidPlain, hidBound, hidNoViewState, hidHidden, lnkBound, lnkHidden, hypBound, hypHidden, imgBound, imgCode, imgHidden, ibtnBound, ibtnHidden,
                txtInPanel, txtInHiddenPanel, textInHolder, lblInHiddenHolder, litInBoundHolder,
                dynamicLabel, dynamicLiteral, dynamicCheck, dynamicRadio, dynamicHidden, dynamicLink, dynamicHyperLink, dynamicImage, dynamicImageButton,
            })
                entries.Add("State " + Describe(control));
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>Basic controls scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
