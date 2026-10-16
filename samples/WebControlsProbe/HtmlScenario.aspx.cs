using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The HTML server controls' scenario layer (scenarios/Html*.json, on this page; HtmlForm and HtmlHead made in code on
    /// HtmlFormScenario.aspx): each element plain, data-bound, set in code (kept in view state), set in code without view
    /// state, hidden and shown later, made dynamically; the inputs' posted values and ServerChange, the buttons' ServerClick
    /// (an image's coordinates), validation of the inputs on the server (g1) and in the browser (g2), a select's event
    /// validation; and a log of what the server saw (the request's number, kept in view state: a click the browser
    /// cancelled is no request; the events in the order raised; every control's value at the end). A post the server
    /// rejects is written as the exception's type and message.
    /// </summary>
    public partial class HtmlScenario : Page
    {
        readonly List<string> entries = new List<string>();
        readonly List<HtmlControl> madeInCode = new List<HtmlControl>();

        protected string BoundText => "bound";

        protected bool BoundFlag => true;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Made on every request, before the posted data is loaded: they get their posted values and view state as ones in markup.
            var text = new HtmlInputText { ID = "inpDynamicText" };
            text.ServerChange += Changed;
            var password = new HtmlInputPassword { ID = "inpDynamicPassword" };
            password.ServerChange += Changed;
            var hidden = new HtmlInputHidden { ID = "inpDynamicHidden" };
            hidden.ServerChange += Changed;
            var check = new HtmlInputCheckBox { ID = "inpDynamicCheck" };
            check.ServerChange += Changed;
            var radio = new HtmlInputRadioButton { ID = "inpDynamicRadio", Name = "dynamic" };
            radio.ServerChange += Changed;
            var email = new HtmlInputGenericControl("email") { ID = "inpDynamicEmail" };
            email.ServerChange += Changed;
            var area = new HtmlTextArea { ID = "txaDynamic" };
            area.ServerChange += Changed;
            var select = new HtmlSelect { ID = "selDynamic" };
            select.Items.Add(new ListItem("One", "1"));
            select.Items.Add(new ListItem("Two", "2"));
            select.ServerChange += Changed;
            var file = new HtmlInputFile { ID = "inpDynamicFile" };
            var button = new HtmlButton { ID = "btnDynamic", InnerText = "Dynamic button", CausesValidation = false };
            button.ServerClick += Clicked;
            var inputButton = new HtmlInputButton("button") { ID = "inpDynamicButton", Value = "Dynamic input button", CausesValidation = false };
            inputButton.ServerClick += Clicked;
            var submit = new HtmlInputSubmit { ID = "inpDynamicSubmit", Value = "Dynamic submit", CausesValidation = false };
            submit.ServerClick += Clicked;
            var reset = new HtmlInputReset { ID = "inpDynamicReset", Value = "Dynamic reset" };
            var image = new HtmlInputImage { ID = "inpDynamicImage", Src = "~/Images/probe.png", Alt = "Dynamic image", CausesValidation = false };
            image.ServerClick += ImageClicked;
            var anchor = new HtmlAnchor { ID = "ancDynamic", InnerText = "Dynamic anchor", CausesValidation = false };
            anchor.ServerClick += Clicked;
            var table = new HtmlTable { ID = "tblDynamic", Border = 1 };
            var row = new HtmlTableRow { ID = "trDynamic" };
            row.Cells.Add(new HtmlTableCell { ID = "tdDynamic" });
            table.Rows.Add(row);
            var video = new HtmlVideo { ID = "vidDynamic", Poster = "~/Images/probe.png" };
            video.Controls.Add(new HtmlSource { ID = "srcDynamic", Src = "~/Images/probe.png" });
            video.Controls.Add(new HtmlTrack { ID = "trkDynamic", Src = "~/Images/probe.png" });
            madeInCode.AddRange(new HtmlControl[]
            {
                new HtmlGenericControl("span") { ID = "genDynamic" }, anchor, new HtmlImage { ID = "imgDynamic", Src = "~/Images/probe.png" }, table,
                text, password, hidden, check, radio, email, area, select, file, button, inputButton, submit, reset, image,
                new HtmlIframe { ID = "ifrDynamic", Src = "~/Images/probe.png" }, new HtmlArea { ID = "areaDynamic", Href = "~/ButtonTarget.aspx" },
                new HtmlAudio { ID = "audDynamic", Src = "~/Images/probe.png" }, video, new HtmlEmbed { ID = "embDynamic", Src = "~/Images/probe.png" },
                new HtmlTitle { ID = "ttlDynamic", Text = "Dynamic title" },
            });
            foreach (var control in madeInCode) Dynamic.Controls.Add(control);
            // The head's own, made here too: a meta and a link (the page's head is made before the page's Init).
            var meta = new HtmlMeta { ID = "metaDynamic", Name = "keywords" };
            var link = new HtmlLink { ID = "lnkDynamic", Href = "~/Images/probe.css" };
            link.Attributes["rel"] = "stylesheet";
            Header.Controls.Add(meta);
            Header.Controls.Add(link);
            madeInCode.Add(meta);
            madeInCode.Add(link);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack);
            if (IsPostBack) return;
            // Set in code on the first request only: kept by view state, lost without it.
            genCode.InnerText = "set in code";
            genNoViewState.InnerText = "set in code, no view state";
            genHidden.InnerText = "set while hidden";
            ancHidden.InnerText = "Hidden anchor (set while hidden)";
            imgCode.Alt = "set in code";
            imgHidden.Alt = "set while hidden";
            tdCode.InnerText = "set in code";
            tdHiddenCell.InnerText = "Hidden cell (set while hidden)";
            var row = new HtmlTableRow();
            row.Cells.Add(new HtmlTableCell { InnerText = "row added in code" });
            tblCode.Rows.Add(row);
            inpTextNoViewState.Value = "set in code, no view state";
            inpTextHidden.Value = "set while hidden";
            Page.Title = "HTML controls scenario (set in code)";
            metaBound.Scheme = "set in code";
            selBound.DataSource = ProbeData.Products();
            foreach (var control in madeInCode)
            {
                switch (control)
                {
                    case HtmlInputControl input when !(input is HtmlInputFile || input is HtmlInputButton || input is HtmlInputImage): input.Value = "dynamic"; break;
                    case HtmlTextArea textArea: textArea.Value = "dynamic"; break;
                    case HtmlTitle title: title.Text = "Dynamic title (set in code)"; break;
                    case HtmlMeta meta: meta.Content = "dynamic"; break;
                    case HtmlTable table: table.Rows[0].Cells[0].InnerText = "dynamic cell"; break;
                    case HtmlContainerControl container when control is HtmlGenericControl || control is HtmlAnchor: container.InnerText = container.InnerText.Length == 0 ? "dynamic" : container.InnerText; break;
                    case HtmlImage image: image.Alt = "dynamic"; break;
                }
            }
            DataBind();
        }

        protected void Changed(object sender, EventArgs e)
        {
            entries.Add("ServerChange " + Describe((Control)sender));
        }

        protected void Clicked(object sender, EventArgs e)
        {
            entries.Add("ServerClick " + ((Control)sender).ID);
        }

        protected void ImageClicked(object sender, ImageClickEventArgs e)
        {
            entries.Add("ServerClick " + ((Control)sender).ID + " at " + e.X + "," + e.Y);
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            foreach (var control in new Control[] { genHidden, ancHidden, imgHidden, tdHiddenCell, tblHidden, inpTextHidden })
                control.Visible = true;
            entries.Add("Click btnShow");
        }

        static string Describe(Control control)
        {
            var visible = control.Visible ? "" : " (hidden)";
            switch (control)
            {
                case HtmlInputFile file:
                    var posted = file.PostedFile;
                    return file.ID + " PostedFile=" + (posted == null ? "null" : "\"" + System.IO.Path.GetFileName(posted.FileName) + "\" " + posted.ContentLength + " bytes \"" +
                        new System.IO.StreamReader(posted.InputStream).ReadToEnd() + "\"") + " Value=\"" + file.Value + "\"" + visible;
                case HtmlInputCheckBox check: return check.ID + " Checked=" + check.Checked + visible;
                case HtmlInputRadioButton radio: return radio.ID + " Checked=" + radio.Checked + " Value=\"" + radio.Value + "\"" + visible;
                case HtmlInputImage image: return image.ID + " Alt=\"" + image.Alt + "\"" + visible;
                case HtmlInputControl input: return input.ID + " Value=\"" + input.Value + "\"" + visible;
                case HtmlTextArea area: return area.ID + " Value=\"" + area.Value + "\"" + visible;
                case HtmlSelect select:
                    return select.ID + " SelectedIndex=" + select.SelectedIndex + " Value=\"" + select.Value + "\" Selected=[" +
                        string.Join(",", select.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value)) + "] Items=" + select.Items.Count + visible;
                case HtmlTable table: return table.ID + " Rows=" + table.Rows.Count + " [" + string.Join("|", table.Rows.Cast<HtmlTableRow>().SelectMany(r => r.Cells.Cast<HtmlTableCell>()).Select(c => c.InnerText)) + "]" + visible;
                case HtmlTableRow row: return row.ID + " Cells=" + row.Cells.Count + " Title=\"" + row.Attributes["title"] + "\"" + visible;
                case HtmlImage image: return image.ID + " Src=\"" + image.Src + "\" Alt=\"" + image.Alt + "\"" + visible;
                case HtmlTitle title: return title.ID + " Text=\"" + title.Text + "\"" + visible;
                case HtmlMeta meta: return meta.ID + " Name=\"" + meta.Name + "\" Content=\"" + meta.Content + "\" Scheme=\"" + meta.Scheme + "\"" + visible;
                case HtmlLink link: return link.ID + " Href=\"" + link.Href + "\"" + visible;
                case HtmlAnchor anchor: return anchor.ID + " HRef=\"" + anchor.HRef + "\" InnerText=\"" + anchor.InnerText + "\"" + visible;
                case HtmlArea area: return area.ID + " Href=\"" + area.Href + "\" Alt=\"" + area.Attributes["alt"] + "\"" + visible;
                case HtmlIframe frame: return frame.ID + " Src=\"" + frame.Src + "\"" + visible;
                case HtmlAudio audio: return audio.ID + " Src=\"" + audio.Src + "\"" + visible;
                case HtmlVideo video: return video.ID + " Src=\"" + video.Src + "\" Poster=\"" + video.Poster + "\"" + visible;
                case HtmlSource source: return source.ID + " Src=\"" + source.Src + "\"" + visible;
                case HtmlTrack track: return track.ID + " Src=\"" + track.Src + "\"" + visible;
                case HtmlEmbed embed: return embed.ID + " Src=\"" + embed.Src + "\"" + visible;
                case HtmlElement element: return element.ID + " lang=\"" + element.Attributes["lang"] + "\"" + visible;
                case HtmlForm form: return form.ID + " title=\"" + form.Attributes["title"] + "\"" + visible;
                case HtmlHead head: return head.ID + " Title=\"" + head.Title + "\" Controls=" + head.Controls.Count + visible;
                case HtmlContainerControl container: return container.ID + " InnerText=\"" + container.InnerText + "\" title=\"" + container.Attributes["title"] + "\"" + visible;
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
                elmPage, hdPage, ttlPage, metaBound, lnkBound, form1,
                genPlain, genBound, genCode, genNoViewState, genHidden, ancTarget, ancBound, ancHidden,
                imgPlain, imgBound, imgCode, imgHidden, ifrPlain, areaPlain, audPlain, vidPlain, srcPlain, trkPlain, embPlain,
                tblPlain, trPlain, trBound, tdPlain, tdCode, tdBound, tdHiddenCell, tblCode, tblHidden,
                inpText, inpTextBound, inpTextNoViewState, inpTextDisabled, inpTextHidden, inpPassword, inpHidden, inpHiddenBound,
                inpCheck, inpCheckBound, inpRadioSmall, inpRadioLarge, inpEmail, inpEmailBound, txaNote, txaBound, selSize, selMulti, selBound, inpFile,
                btnHtml, btnHtmlBound, inpButton, inpSubmit, inpSubmitBound, inpReset, inpResetBound, inpImage, inpImageBound,
                inpClient, inpClientPassword, inpClientEmail, txaClient, selClient, inpClientFile,
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
            Response.Write("<!DOCTYPE html><html><head><title>HTML controls scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
