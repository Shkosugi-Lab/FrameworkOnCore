using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.J
{
    public class TimerPage : NavigationProbe.ProbePage
    {
        protected System.Web.UI.Timer tm;

        protected int Ticks { get { return (int?)ViewState["ticks"] ?? 0; } set { ViewState["ticks"] = value; } }

        protected void Tick(object sender, EventArgs e)
        {
            Ticks = Ticks + 1;
            if (Ticks >= 3) tm.Enabled = false;
        }
    }
}
