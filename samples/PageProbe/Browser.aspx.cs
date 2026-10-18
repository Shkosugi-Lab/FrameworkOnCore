using System;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace PageProbe
{
    /// <summary>
    /// The browser as the server knows it: Request.Browser (the .browser definitions matched against the user agent the
    /// test browser sends: its name, version, platform, what it supports), and the request's own description (method,
    /// accept types, languages, encodings, whether secure). Values that name this machine are left out.
    /// </summary>
    public partial class Browser : Page
    {
        protected override void Render(HtmlTextWriter writer)
        {
            var b = Request.Browser;
            var lines = new[]
            {
                "Browser=" + b.Browser, "Type=" + b.Type, "Id=" + b.Id, "Version=" + b.Version, "MajorVersion=" + b.MajorVersion,
                "MinorVersion=" + b.MinorVersion, "MinorVersionString=" + b.MinorVersionString, "Platform=" + b.Platform, "IsMobileDevice=" + b.IsMobileDevice,
                "Crawler=" + b.Crawler, "Cookies=" + b.Cookies, "Frames=" + b.Frames, "Tables=" + b.Tables, "JavaScript=" + b.EcmaScriptVersion,
                "JScriptVersion=" + b.JScriptVersion, "W3CDomVersion=" + b.W3CDomVersion, "MSDomVersion=" + b.MSDomVersion, "SupportsCss=" + b.SupportsCss,
                "SupportsCallback=" + b.SupportsCallback, "SupportsXmlHttp=" + b.SupportsXmlHttp,
                "TagWriter=" + b.TagWriter.FullName, "Win32=" + b.Win32, "Beta=" + b.Beta, "ScreenPixelsWidth=" + b.ScreenPixelsWidth,
                "PreferredRenderingType=" + b.PreferredRenderingType, "PreferredRequestEncoding=" + b.PreferredRequestEncoding, "IsColor=" + b.IsColor,
                "Browsers=" + string.Join(",", b.Browsers.Cast<string>()),
                "IsBrowser(Chrome)=" + b.IsBrowser("Chrome") + " IsBrowser(IE)=" + b.IsBrowser("IE"),
                "UserAgent has Chrome=" + (Request.UserAgent ?? "").Contains("Chrome"),
                "HttpMethod=" + Request.HttpMethod + " RequestType=" + Request.RequestType + " IsSecureConnection=" + Request.IsSecureConnection,
                "AcceptTypes=" + string.Join(",", Request.AcceptTypes ?? new string[0]),
                "UserLanguages=" + string.Join(",", Request.UserLanguages ?? new string[0]),
                "ContentEncoding=" + Request.ContentEncoding.WebName + " ContentType=[" + Request.ContentType + "] ContentLength=" + Request.ContentLength,
                "Url=" + Request.Url.PathAndQuery + " RawUrl=" + Request.RawUrl + " Path=" + Request.Path + " FilePath=" + Request.FilePath,
                "AppRelativeCurrentExecutionFilePath=" + Request.AppRelativeCurrentExecutionFilePath + " ApplicationPath=" + Request.ApplicationPath,
            };
            writer.Write("<!DOCTYPE html><html><head><title>Browser</title></head><body><pre id=\"out\">" + HttpUtility.HtmlEncode(string.Join("\n", lines)) + "</pre></body></html>");
        }
    }
}
