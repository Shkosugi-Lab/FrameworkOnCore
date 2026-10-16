#if NETCOREAPP

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace System.Web.Hosting
{
	/// <summary>
	/// The original IIS server's settings for the site, the ones outside the application's web.config (capture-iis.ps1 of the
	/// FrameworkOnCore repository, on that server; the converter's --iis-profile puts it in the application's folder as
	/// iis-profile.config, a name IIS's request filtering does not serve): the server's system.webServer
	/// (applicationHost.config's and its &lt;location path=""&gt;), the site's own &lt;location&gt;s, its log file, the modules
	/// IIS has. Read as IIS reads them: under the application's web.config (the server's first, then the site's, then
	/// web.config's). Without the file, IIS's defaults (as the host had them).
	/// </summary>
	internal static class IisProfile
	{
		internal const string FileName = "iis-profile.config";

		static readonly ConcurrentDictionary<string, XElement> loaded = new ConcurrentDictionary<string, XElement>(StringComparer.OrdinalIgnoreCase);

		/// <summary>The profile of the application in the folder; null without one (or one that is not XML: told once).</summary>
		internal static XElement Of(string applicationFolder)
		{
			if (string.IsNullOrEmpty(applicationFolder)) return null;
			return loaded.GetOrAdd(applicationFolder, folder =>
			{
				var file = Path.Combine(folder, FileName);
				if (!File.Exists(file)) return null;
				try
				{
					return XDocument.Load(file).Root;
				}
				catch (Exception e) when (e is System.Xml.XmlException || e is IOException)
				{
					Console.Error.WriteLine("IIS profile " + file + ": " + e.Message + " (not read)");
					return null;
				}
			});
		}

		/// <summary>The running application's profile.</summary>
		internal static XElement Current => Of(HttpRuntime.AppDomainAppPathInternal ?? HostingEnvironment.MapPath("~/"));

		/// <summary>
		/// The system.webServer elements the profile gives the site's root, in IIS's order: the server's, then the site's
		/// own (its &lt;location&gt; for the site); none without a profile.
		/// </summary>
		internal static IEnumerable<XElement> WebServer(XElement profile)
		{
			if (profile == null) return Enumerable.Empty<XElement>();
			return profile.Elements("system.webServer")
				.Concat(profile.Elements("location").Where(l => { var p = (string)l.Attribute("path"); return string.IsNullOrEmpty(p) || p == "."; })
					.Elements("system.webServer"));
		}

		/// <summary>The site's log file settings: the sites' defaults, the site's own over them (attribute by attribute).</summary>
		internal static IDictionary<string, string> LogFile(XElement profile)
		{
			var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			if (profile == null) return settings;
			foreach (var logFile in new[] { profile.Element("siteDefaults")?.Element("logFile"), profile.Element("site")?.Element("logFile") })
				if (logFile != null)
					foreach (var attribute in logFile.Attributes()) settings[attribute.Name.LocalName] = attribute.Value;
			return settings;
		}

		/// <summary>The site's id (W3SVC&lt;id&gt;, s-sitename); null without a profile.</summary>
		internal static string SiteId(XElement profile) => (string)profile?.Element("site")?.Attribute("id");
	}
}

#endif
