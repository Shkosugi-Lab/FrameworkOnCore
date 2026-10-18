using System;
using System.Web;
using System.Web.UI;

namespace PageProbe
{
    /// <summary>
    /// What the page's scripts are made of (parity-scenario.json compares the HTML: compareSource): every
    /// ClientScriptManager registration (blocks, startup, include, array, hidden field, expando, submit statement), postback
    /// references, a client callback (ICallbackEventHandler) answered with the server's text, the default focus and
    /// button, scroll position. The browser runs them: the startup script writes into the page, the callback's answer too.
    /// </summary>
    public partial class Scripts : Page, ICallbackEventHandler, IPostBackEventHandler
    {
        string callbackArgument;

        protected void Page_Load(object sender, EventArgs e)
        {
            var cs = ClientScript;
            var type = GetType();
            cs.RegisterClientScriptBlock(type, "block", "function probeBlock() { return 'block <&> \"quoted\"'; }", true);
            cs.RegisterClientScriptBlock(type, "raw", "<script type=\"text/javascript\">var probeRaw = 1;</script>", false);
            cs.RegisterStartupScript(type, "startup", "document.getElementById('log').setAttribute('data-startup', probeBlock() + ' ' + probeArray.length);", true);
            cs.RegisterClientScriptInclude(type, "include", ResolveClientUrl("~/probe.js"));
            cs.RegisterArrayDeclaration("probeArray", "'a', 'b'");
            cs.RegisterArrayDeclaration("probeArray", "3");
            cs.RegisterHiddenField("probeHidden", "hidden & value");
            cs.RegisterExpandoAttribute(txtFocus.ClientID, "probe", "expando 'value'", true);
            cs.RegisterOnSubmitStatement(type, "submit", "document.getElementById('probeHidden').value += '!';");
            lnkManual.HRef = cs.GetPostBackClientHyperlink(this, "manual");
            var callback = cs.GetCallbackEventReference(this, "arg", "receiveServer", "context", "onCallbackError", true);
            cs.RegisterClientScriptBlock(type, "callback",
                "function callServer(arg, context) { " + callback + "; }" +
                "function receiveServer(result, context) { document.getElementById('callbackResult').innerHTML = result; }" +
                "function onCallbackError(message, context) { document.getElementById('callbackResult').innerHTML = 'error: ' + message; }", true);
            Log.Text = HttpUtility.HtmlEncode("Load IsPostBack=" + IsPostBack + " IsCallback=" + IsCallback + " hidden=" + Request.Form["probeHidden"]
                + " IsStartupScriptRegistered=" + cs.IsStartupScriptRegistered(type, "startup") + " IsClientScriptIncludeRegistered=" + cs.IsClientScriptIncludeRegistered(type, "include"));
            if (IsPostBack && !IsCallback) SetFocus(btnDefault);
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            ClientScript.ValidateEvent(UniqueID, eventArgument);
            Log.Text += HttpUtility.HtmlEncode("\nPage postback '" + eventArgument + "'");
        }

        // Registered while rendering (ASP.NET allows it then only); validated when the postback comes.
        protected override void Render(HtmlTextWriter writer)
        {
            ClientScript.RegisterForEventValidation(UniqueID, "manual");
            base.Render(writer);
        }

        protected void DefaultClicked(object sender, EventArgs e) { Log.Text += "\nDefault clicked"; }
        protected void ConfirmClicked(object sender, EventArgs e) { Log.Text += "\nConfirm clicked"; }

        public void RaiseCallbackEvent(string eventArgument) { callbackArgument = eventArgument; }

        public string GetCallbackResult() => "pong for '" + callbackArgument + "' (" + Request.HttpMethod + ", __CALLBACKID " + Request.Form["__CALLBACKID"] + ")";
    }
}
