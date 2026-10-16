using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace SessionProbe
{
    /// <summary>
    /// A page of the session (parity-scenario.json): stores an object that is not serializable, changes it in place,
    /// removes, clears, abandons; writes what the session holds and is (its mode, timeout, whether new, cookie), and what
    /// Global.asax saw (Session_Start, Session_End: the application's log).
    /// </summary>
    public partial class Default : Page
    {
        readonly List<string> entries = new List<string>();

        /// <summary>Not [Serializable]: InProc keeps the object itself.</summary>
        public sealed class Cart
        {
            public List<string> Items = new List<string>();
            public override string ToString() => "Cart [" + string.Join(",", Items) + "]";
        }

        protected void Store(object sender, EventArgs e)
        {
            Session["cart"] = new Cart { Items = { "pen" } };
            Session["count"] = (Session["count"] as int? ?? 0) + 1;
            Session["text"] = "stored";
            entries.Add("Click Store");
        }

        protected void Mutate(object sender, EventArgs e)
        {
            // Changed in place, not stored again.
            var cart = Session["cart"] as Cart;
            if (cart != null) cart.Items.Add("bag");
            entries.Add("Click Mutate (" + (cart == null ? "no cart" : "cart changed in place") + ")");
        }

        protected void Remove(object sender, EventArgs e)
        {
            Session.Remove("text");
            entries.Add("Click Remove");
        }

        protected void ClearAll(object sender, EventArgs e)
        {
            Session.Clear();
            entries.Add("Click Clear");
        }

        protected void AbandonSession(object sender, EventArgs e)
        {
            Session.Abandon();
            entries.Add("Click Abandon");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            entries.Insert(0, "IsPostBack=" + IsPostBack);
            entries.Add("Mode=" + Session.Mode + " Timeout=" + Session.Timeout + " IsCookieless=" + Session.IsCookieless + " IsReadOnly=" + Session.IsReadOnly + " IsNewSession=" + Session.IsNewSession);
            entries.Add("Same session as the last request: " + (Session.SessionID == (string)Application["last-id"]));
            Application["last-id"] = Session.SessionID;
            entries.Add("SessionID length: " + Session.SessionID.Length);
            entries.Add("Keys: " + string.Join(",", Session.Keys.Cast<string>()) + " (Count " + Session.Count + ")");
            foreach (string key in Session.Keys) entries.Add("  " + key + " = " + Session[key]);
            entries.Add("Request cookies: " + string.Join(",", Request.Cookies.AllKeys));
            entries.Add("Global: " + Global.Events());
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }
    }
}
