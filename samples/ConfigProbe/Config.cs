using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ConfigProbe
{
    /// <summary>What a page reads from its configuration, the same cases at the root and in Sub (Sub/web.config, the root's
    /// &lt;location path="Sub"&gt;): ConfigurationManager (the application's) and WebConfigurationManager (the request's path).</summary>
    public abstract class ConfigPage : ProbePage
    {
        /// <summary>A configuration error by its message without the path, the file's name and line.</summary>
        protected static object Failure(Func<object> read)
        {
            try
            {
                return read();
            }
            catch (ConfigurationException ex)
            {
                return "!" + ex.GetType().Name + ": " + ex.BareMessage + " (" + (ex.Filename == null ? "(no file)" : Path.GetFileName(ex.Filename)) + " line " + ex.Line + ")"
                    + (ex.InnerException == null ? "" : " inner " + ex.InnerException.GetType().Name);
            }
        }

        static string Values(NameValueCollection values) =>
            values == null ? "(null)" : string.Join("; ", values.AllKeys.Select(k => k + "=" + values[k]));

        static string Shop(ShopSection s) => s == null ? "(null)" :
            "name=" + s.Name + " max=" + s.MaxItems + " opened=" + s.Opened.ToString("yyyy-MM-dd HH:mm:ss") + " kind=" + s.Opened.Kind
            + " ratio=" + s.Ratio.ToString("R") + " shopKind=" + s.Kind + " timeout=" + s.Timeout
            + " products=" + string.Join(",", s.Products.Cast<ProductElement>().Select(p => p.Sku + ":" + p.Price))
            + " source=" + (s.ElementInformation.Source == null ? "(null)" : Path.GetFileName(s.ElementInformation.Source))
            + " line=" + s.ElementInformation.LineNumber + " readOnly=" + s.IsReadOnly()
            + " present=" + s.ElementInformation.Properties["maxItems"].ValueOrigin;

        protected void Common(string path)
        {
            Section("appSettings");
            Case("ConfigurationManager.AppSettings", () => Values(ConfigurationManager.AppSettings));
            Case("WebConfigurationManager.AppSettings", () => Values(WebConfigurationManager.AppSettings));
            Case("AppSettings[mode]", () => ConfigurationManager.AppSettings["mode"]);
            Case("Web AppSettings[MODE]", () => WebConfigurationManager.AppSettings["MODE"]);
            Case("AppSettings[dup]", () => ConfigurationManager.AppSettings["dup"]);
            Case("AppSettings[Missing]", () => ConfigurationManager.AppSettings["Missing"]);
            Case("AppSettings.Count", () => ConfigurationManager.AppSettings.Count);
            Case("AppSettings type", () => ConfigurationManager.AppSettings.GetType().Name);
            Case("AppSettingsReader Number int", () => new AppSettingsReader().GetValue("Number", typeof(int)));
            Case("AppSettingsReader Flag bool", () => new AppSettingsReader().GetValue("Flag", typeof(bool)));
            Case("AppSettingsReader Ratio double", () => new AppSettingsReader().GetValue("Ratio", typeof(double)));
            Case("AppSettingsReader Empty string", () => new AppSettingsReader().GetValue("Empty", typeof(string)));
            Case("AppSettingsReader Empty int", () => Failure(() => new AppSettingsReader().GetValue("Empty", typeof(int))));
            Case("AppSettingsReader Missing", () => Failure(() => new AppSettingsReader().GetValue("Missing", typeof(string))));

            Section("connectionStrings");
            Case("ConnectionStrings", () => string.Join(" | ", ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>()
                .Select(c => c.Name + ";" + c.ProviderName + ";" + c.ConnectionString)));
            Case("ConnectionStrings[main]", () => ConfigurationManager.ConnectionStrings["main"]?.ConnectionString);
            Case("ConnectionStrings[0].Name", () => ConfigurationManager.ConnectionStrings[0].Name);
            Case("Web ConnectionStrings.Count", () => WebConfigurationManager.ConnectionStrings.Count);
            Case("ConnectionStrings source", () => Path.GetFileName(ConfigurationManager.ConnectionStrings["Main"].ElementInformation.Source ?? "(null)"));

            Section("handlers");
            Case("plainValues", () => Failure(() => { var v = ConfigurationManager.GetSection("plainValues"); return v.GetType().Name + " " + Values((NameValueCollection)v); }));
            Case("Web plainValues", () => Failure(() => Values((NameValueCollection)WebConfigurationManager.GetSection("plainValues"))));
            Case("dictValues", () => { var v = (IDictionary)ConfigurationManager.GetSection("dictValues"); return v.GetType().Name + " " + string.Join(";", v.Keys.Cast<object>().Select(k => k + "=" + v[k]).OrderBy(s => s, StringComparer.Ordinal)); });
            Case("singleTag", () => { var v = (IDictionary)ConfigurationManager.GetSection("singleTag"); return v.GetType().Name + " " + string.Join(";", v.Keys.Cast<object>().Select(k => k + "=" + v[k]).OrderBy(s => s, StringComparer.Ordinal)) + " size=" + v["size"]; });
            Case("legacyHandler", () => string.Join(" / ", (System.Collections.Generic.List<string>)ConfigurationManager.GetSection("legacyHandler")));
            Case("unknown section", () => ConfigurationManager.GetSection("nothing") ?? "(null)");
            Case("system.webServer", () => ConfigurationManager.GetSection("system.webServer")?.GetType().Name ?? "(null)");

            Section("sections");
            Case("probe/shop", () => Failure(() => Shop((ShopSection)ConfigurationManager.GetSection("probe/shop"))));
            Case("Web probe/shop", () => Failure(() => Shop((ShopSection)WebConfigurationManager.GetSection("probe/shop"))));
            Case("Web probe/shop /Sub", () => Failure(() => Shop((ShopSection)WebConfigurationManager.GetSection("probe/shop", "/Sub"))));
            Case("Web probe/shop /Bad", () => Failure(() => Shop((ShopSection)WebConfigurationManager.GetSection("probe/shop", "/Bad"))));
            Case("external", () => Failure(() => Shop((ShopSection)ConfigurationManager.GetSection("external"))));
            Case("Settings.Greeting", () => Properties.Settings.Default.Greeting);
            Case("Settings.Limit", () => Properties.Settings.Default.Limit);
            Case("Settings.Unset", () => Properties.Settings.Default.Unset);
            Case("Settings PropertyValues", () => Properties.Settings.Default.Properties.Count);

            Section("system.web");
            Case("httpRuntime", () => { var s = (HttpRuntimeSection)WebConfigurationManager.GetSection("system.web/httpRuntime"); return "max=" + s.MaxRequestLength + " timeout=" + s.ExecutionTimeout + " invalid=" + s.RequestPathInvalidCharacters + " target=" + s.TargetFramework; });
            Case("httpRuntime app", () => ((HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime")).MaxRequestLength);
            Case("globalization", () => { var s = (GlobalizationSection)WebConfigurationManager.GetSection("system.web/globalization"); return s.Culture + "/" + s.UICulture + " " + s.RequestEncoding.WebName + "/" + s.ResponseEncoding.WebName; });
            Case("Thread culture", () => System.Threading.Thread.CurrentThread.CurrentCulture.Name + "/" + System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
            Case("compilation", () => { var s = (CompilationSection)WebConfigurationManager.GetSection("system.web/compilation"); return "debug=" + s.Debug + " target=" + s.TargetFramework + " lang=" + s.DefaultLanguage + " batch=" + s.Batch; });
            Case("IsDebuggingEnabled", () => HttpContext.Current.IsDebuggingEnabled);
            Case("customErrors", () => ((CustomErrorsSection)WebConfigurationManager.GetSection("system.web/customErrors")).Mode);
            Case("sessionState", () => { var s = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState"); return s.Mode + " " + s.Timeout + " " + s.CookieName + " " + s.Cookieless; });
            Case("Session.Timeout", () => Session.Timeout);
            Case("pages", () => { var s = (PagesSection)WebConfigurationManager.GetSection("system.web/pages"); return s.ClientIDMode + " validate=" + s.ValidateRequest + " scroll=" + s.MaintainScrollPositionOnPostBack + " rendering=" + s.ControlRenderingCompatibilityVersion + " viewStateMac=" + s.EnableViewStateMac; });
            Case("authentication", () => ((AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication")).Mode);
            Case("machineKey", () => { var s = (MachineKeySection)WebConfigurationManager.GetSection("system.web/machineKey"); return s.Validation + " " + s.Decryption + " " + s.CompatibilityMode; });
            Case("trace", () => ((TraceSection)WebConfigurationManager.GetSection("system.web/trace")).Enabled);
            Case("httpCookies", () => { var s = (HttpCookiesSection)WebConfigurationManager.GetSection("system.web/httpCookies"); return "httpOnly=" + s.HttpOnlyCookies + " ssl=" + s.RequireSSL + " sameSite=" + s.SameSite; });

            Section("open");
            Case("OpenWebConfiguration AppSettings", () => { var c = WebConfigurationManager.OpenWebConfiguration(path); return string.Join("; ", c.AppSettings.Settings.AllKeys.Select(k => k + "=" + c.AppSettings.Settings[k].Value)); });
            Case("OpenWebConfiguration file", () => Path.GetFileName(WebConfigurationManager.OpenWebConfiguration(path).FilePath));
            Case("OpenWebConfiguration HasFile", () => WebConfigurationManager.OpenWebConfiguration(path).HasFile);
            Case("OpenWebConfiguration probe/shop", () => Failure(() => Shop((ShopSection)WebConfigurationManager.OpenWebConfiguration(path).GetSection("probe/shop"))));
            Case("OpenWebConfiguration appSettings.File", () => WebConfigurationManager.OpenWebConfiguration(path).AppSettings.File);
            Case("OpenWebConfiguration connectionStrings.ConfigSource", () => WebConfigurationManager.OpenWebConfiguration(path).ConnectionStrings.SectionInformation.ConfigSource);
        }
    }
}
