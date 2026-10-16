#if NETCOREAPP

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace System.Web.Hosting
{
	/// <summary>
	/// The system.webServer sections IIS applies to a URL, in IIS's order: the original server's (IisProfile: the server's,
	/// the site's &lt;location&gt;s), then the application's web.config files from the root down (each file's own section,
	/// then its &lt;location&gt;s, a folder's web.config after its parent's). Each section with the URL folder it is for
	/// ("" the site's root, "/admin", "/old/page.aspx" a file's location) and the folder of its file (configSource files).
	/// Read once (a change of web.config restarts the application).
	/// </summary>
	internal static class IisConfiguration
	{
		internal sealed class Section
		{
			public string Base;
			public XElement WebServer;
			public string FileFolder;   // null: the profile's
			public bool Server;         // the profile's (applicationHost.config's)
		}

		static readonly ConcurrentDictionary<string, List<Section>> loaded = new ConcurrentDictionary<string, List<Section>>(StringComparer.OrdinalIgnoreCase);

		internal static IEnumerable<XElement> Children(XElement element, string name) =>
			element == null ? Enumerable.Empty<XElement>() : element.Elements().Where(e => e.Name.LocalName == name);

		internal static XElement Child(XElement element, string name) => Children(element, name).FirstOrDefault();

		/// <summary>A folder's URL as a base: "" for the root, "/a/b" otherwise.</summary>
		internal static string Folder(string parent, string path)
		{
			path = (path ?? "").Replace('\\', '/').Trim('/');
			if (path.Length == 0 || path == ".") return parent;
			return parent + "/" + path;
		}

		/// <summary>Whether the folder's settings apply to the URL's path (IIS: without regard to case).</summary>
		internal static bool Covers(string folder, string path) =>
			folder.Length == 0 || (path.StartsWith(folder, StringComparison.OrdinalIgnoreCase) && (path.Length == folder.Length || path[folder.Length] == '/'));

		/// <summary>All the sections of the application in the folder, in IIS's order.</summary>
		internal static List<Section> Sections(string contentRoot) =>
			loaded.GetOrAdd(contentRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), Load);

		/// <summary>The running application's folder.</summary>
		internal static string Root => HttpRuntime.AppDomainAppPathInternal ?? HostingEnvironment.MapPath("~/");

		/// <summary>The sections that apply to the URL's path (decoded, from '/'), in IIS's order.</summary>
		internal static IEnumerable<Section> For(string contentRoot, string path) => Sections(contentRoot).Where(s => Covers(s.Base, path));

		/// <summary>
		/// The deepest folder with sections of its own that applies to the path: the sections that apply to the path are the
		/// ones that apply to it (a key for what is made of them).
		/// </summary>
		internal static string Deepest(string contentRoot, string path)
		{
			string deepest = "";
			foreach (var section in Sections(contentRoot))
				if (section.Base.Length > deepest.Length && Covers(section.Base, path)) deepest = section.Base;
			return deepest;
		}

		static List<Section> Load(string contentRoot)
		{
			var sections = new List<(int Rank, int FileDepth, int Order, Section Section)>();
			var profile = IisProfile.Of(contentRoot);
			if (profile != null)
			{
				foreach (var webServer in Children(profile, "system.webServer"))
					sections.Add((0, 0, sections.Count, new Section { Base = "", WebServer = webServer, Server = true }));
				foreach (var location in Children(profile, "location"))
					foreach (var webServer in Children(location, "system.webServer"))
						sections.Add((0, 0, sections.Count, new Section { Base = Folder("", (string)location.Attribute("path")), WebServer = webServer, Server = true }));
			}
			foreach (var file in WebConfigs(contentRoot))
			{
				var folder = Path.GetDirectoryName(file);
				var relative = Path.GetRelativePath(contentRoot, folder);
				var fileBase = relative == "." ? "" : Folder("", relative);
				XElement root;
				try { root = XDocument.Load(file).Root; }
				catch (Exception e) when (e is System.Xml.XmlException || e is IOException || e is UnauthorizedAccessException) { continue; }
				var depth = fileBase.Count(c => c == '/');
				foreach (var webServer in Children(root, "system.webServer"))
					sections.Add((1, depth, sections.Count, new Section { Base = fileBase, WebServer = webServer, FileFolder = folder }));
				foreach (var location in Children(root, "location"))
					foreach (var webServer in Children(location, "system.webServer"))
						sections.Add((1, depth, sections.Count, new Section { Base = Folder(fileBase, (string)location.Attribute("path")), WebServer = webServer, FileFolder = folder }));
			}
			// The server's first; then by file (the root's, a folder's after); in a file, its own section, then its locations
			// (a deeper one after); the order written otherwise.
			return sections.OrderBy(s => s.Rank).ThenBy(s => s.FileDepth).ThenBy(s => s.Section.Base.Count(c => c == '/')).ThenBy(s => s.Order)
				.Select(s => s.Section).ToList();
		}

		// The application's web.config files: its root's first, then its folders' (not bin's).
		static IEnumerable<string> WebConfigs(string contentRoot)
		{
			var options = new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = true, IgnoreInaccessible = true };
			string[] files;
			try { files = Directory.GetFiles(contentRoot, "web.config", options); }
			catch (Exception e) when (e is IOException || e is UnauthorizedAccessException) { return Enumerable.Empty<string>(); }
			var bin = Path.Combine(contentRoot, "bin") + Path.DirectorySeparatorChar;
			return files.Where(f => !f.StartsWith(bin, StringComparison.OrdinalIgnoreCase))
				.OrderBy(f => f.Count(c => c == Path.DirectorySeparatorChar)).ThenBy(f => f, StringComparer.OrdinalIgnoreCase);
		}
	}
}

#endif
