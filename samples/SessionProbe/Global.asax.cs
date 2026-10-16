using System;
using System.Collections.Generic;
using System.Web;

namespace SessionProbe
{
    /// <summary>The session's events, as the application sees them: each Session_Start and Session_End, in order.</summary>
    public class Global : HttpApplication
    {
        static readonly List<string> events = new List<string>();

        public static string Events()
        {
            lock (events) return string.Join(", ", events);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            lock (events) events.Add("Session_Start");
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // The ended session's items are still readable here (InProc).
            lock (events) events.Add("Session_End(" + Session.Count + " items)");
        }
    }
}
