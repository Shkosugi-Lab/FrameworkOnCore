using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace LocaleProbe
{
    /// <summary>
    /// Culture="auto:en-US" UICulture="auto": the cultures the browser's Accept-Language makes (the first language, its
    /// quality ignored; one .NET does not know: the default), the resources they find (App_GlobalResources and
    /// App_LocalResources, falling back by the culture's parents), and values written in them.
    /// </summary>
    public partial class Auto : Page
    {
        bool sent;

        protected void Sent(object sender, EventArgs e) => sent = true;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            var culture = Thread.CurrentThread.CurrentCulture;
            var ui = Thread.CurrentThread.CurrentUICulture;
            var lines = new[]
            {
                "Accept-Language = " + Request.Headers["Accept-Language"],
                "UserLanguages = " + (Request.UserLanguages == null ? "(null)" : string.Join("|", Request.UserLanguages)),
                "Culture = " + culture.Name + " (" + culture.LCID + ") neutral=" + culture.IsNeutralCulture,
                "UICulture = " + ui.Name + " neutral=" + ui.IsNeutralCulture + " parent=" + ui.Parent.Name,
                "Page.Culture = " + Culture + " / " + UICulture,
                "Only = " + HttpContext.GetGlobalResourceObject("Strings", "Only"),
                "Price = " + string.Format((string)HttpContext.GetGlobalResourceObject("Strings", "Price"), 1234.5m),
                "Note = " + GetLocalResourceObject("Note"),
                "Tip = " + lblTitle.ToolTip,
                "Date G = " + new DateTime(2026, 3, 18, 14, 5, 9).ToString(),
                "Date D = " + new DateTime(2026, 3, 18).ToString("D"),
                "Number N = " + 1234567.891m.ToString("N"),
                "Currency C = " + (-1234.5m).ToString("C"),
                "Percent P = " + 0.256m.ToString("P"),
                "Calendar = " + culture.Calendar.GetType().Name,
                "DisplayName = " + culture.DisplayName + " / " + new CultureInfo("fr-FR").DisplayName + " / " + CultureInfo.GetCultureInfo("zh-CHS").DisplayName,
                "EnglishName = " + culture.EnglishName + " / Native " + culture.NativeName,
                "Sent = " + sent,
            };
            Out.Text = HttpUtility.HtmlEncode(string.Join("\n", lines));
        }
    }
}