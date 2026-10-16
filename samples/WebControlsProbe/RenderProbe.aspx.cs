using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// One rendering case (?component=TextBox&amp;case=TextBox.MaxLength=3) in a request, through the whole page life
    /// cycle: the component made and its properties set before it is added (as the page parser does with markup), in
    /// PreInit (a theme is set there, and a theme's skin overrides what the markup set); in the form, or in a naming
    /// container (Box). The page then shows what setting failed (probe-errors) and, written as text, the view state it
    /// saved (probe-state: the encoded __VIEWSTATE field differs by machine key, what it holds must not). An error later
    /// in the life cycle is written as its type and message (probe-failure), not as the server's error page.
    /// </summary>
    public partial class RenderProbe : Page
    {
        readonly List<string> failures = new List<string>();
        Control probed;
        RenderCase current;
        Func<object> data;
        Func<object> boundData;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            var component = ProbeComponent.Named(Request.QueryString["component"]);
            var parityCase = component == null ? null : component.Case(Request.QueryString["case"]);
            if (parityCase == null)
            {
                failures.Add("no case " + Request.QueryString["component"] + " / " + Request.QueryString["case"]);
                return;
            }
            if (parityCase.Context == "themed") Theme = "Probe";
            var control = component.Make();
            probed = control;
            current = parityCase;
            data = component.Data;
            boundData = component.BoundData;
            Action<Control> setup;
            if (parityCase.Context != null && component.ContextSetups.TryGetValue(parityCase.Context, out setup))
            {
                try { setup(control); }
                catch (Exception error) { failures.Add("[" + parityCase.Context + "] -> " + Describe(error)); }
            }
            foreach (var assignment in parityCase.Assignments)
            {
                try { assignment.Apply(control); }
                catch (Exception error) { failures.Add(assignment.Description + " -> " + Describe(error)); }
            }
            // A page has one server form and one server head: a case's form or head takes the place of the page's, with
            // what that held (the form: the probe's host and output; the head: its title).
            var replaced = control is HtmlForm ? form1 : control is HtmlHead ? (Control)FindHeader(this) : null;
            Control parent = Host;
            if (parityCase.Context == "naming container")
            {
                var box = new Box { ID = "box" };
                if (replaced != null) Replace(replaced, box);
                else Host.Controls.Add(box);
                parent = box;
            }
            try
            {
                if (replaced == null) parent.Controls.Add(component.Wrap == null ? control : component.Wrap(control));
                else
                {
                    while (replaced.HasControls()) control.Controls.Add(replaced.Controls[0]);
                    if (parent == Host) Replace(replaced, control);
                    else parent.Controls.Add(control);
                }
            }
            catch (Exception error) { failures.Add("Controls.Add -> " + Describe(error)); }
            if (component.Siblings != null)
                foreach (var sibling in component.Siblings()) parent.Controls.Add(sibling);
        }

        /// <summary>
        /// The component bound to the sample data, as a page binds in Load: a "[data-bound]" case's to ProbeData.Products();
        /// a data control's (its Data) every case, an "[empty]" case's to no data.
        /// </summary>
        /// <summary>A Substitution's callback (its MethodName): a static method of the page's class.</summary>
        public static string SubstitutionText(HttpContext context) => "substituted <b>" + context.Request.HttpMethod + "</b>";

        /// <summary>The control in the old one's place (its parent, its index).</summary>
        static void Replace(Control old, Control control)
        {
            var parent = old.Parent;
            var index = parent.Controls.IndexOf(old);
            parent.Controls.Remove(old);
            parent.Controls.AddAt(index, control);
        }

        /// <summary>The page's head (before Init, Page.Header is not yet set): the first HtmlHead of its controls.</summary>
        static HtmlHead FindHeader(Control control)
        {
            foreach (Control child in control.Controls)
            {
                if (child is HtmlHead head) return head;
                var found = FindHeader(child);
                if (found != null) return found;
            }
            return null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (current == null || (current.Context != "data-bound" && data == null)) return;
            try
            {
                object source = current.Context == "empty" ? new List<ProbeData.Product>() : data != null ? data() : boundData != null ? boundData() : ProbeData.Products();
                probed.GetType().GetProperty("DataSource").SetValue(probed, source, null);
                probed.DataBind();
            }
            catch (Exception error) { failures.Add("DataBind -> " + Describe(error)); }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Errors.Text = HttpUtility.HtmlEncode(string.Join("\n", failures));
        }

        protected override void SavePageStateToPersistenceMedium(object state)
        {
            StateText.Text = HttpUtility.HtmlEncode(ProbeState.Describe(state));
            base.SavePageStateToPersistenceMedium(state);
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.StatusCode = 500;
            Response.ContentType = "text/plain";
            Response.Write("probe-failure: " + Describe(error));
            Server.ClearError();
        }

        /// <summary>
        /// An exception as its type and message; an argument exception's parameter apart: the runtime words the message's
        /// suffix ("Parameter name: x" on .NET Framework, "(Parameter 'x')" on .NET), as tests/Parity.Core's Probe writes it.
        /// </summary>
        public static string Describe(Exception error)
        {
            var message = error.Message.Replace("\r\n", "\n");
            var argument = error as ArgumentException;
            if (argument != null && argument.ParamName != null)
            {
                message = Regex.Replace(message, @"\nParameter name: .*$", "");
                message = Regex.Replace(message, @" \(Parameter '[^']*'\)$", "");
                message += " [param " + argument.ParamName + "]";
            }
            return error.GetType().FullName + ": " + message;
        }
    }

    /// <summary>A naming container (its children's IDs and client IDs are prefixed with its own).</summary>
    public class Box : Panel, INamingContainer
    {
    }
}
