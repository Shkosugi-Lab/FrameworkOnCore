using System.Data.Common;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace System.Web
{
	/// <summary>
	/// The ADO.NET providers by invariant name (DbProviderFactories.GetFactory: SqlDataSource's ProviderName, an application's
	/// own code), as .NET Framework has them: its configuration's (System.Data.SqlClient, System.Data.OleDb and System.Data.Odbc,
	/// built into its system.data section's handler) and the application's web.config's (system.data/DbProviderFactories:
	/// add, remove, clear). .NET reads no configuration for them; they are registered here when the application starts
	/// (HttpRuntime's first request), each by its type's name: resolved when asked for, as .NET Framework did (a provider
	/// whose assembly the application has not fails then, as there).
	/// </summary>
	internal static class DbProviderFactoriesConfiguration
	{
		static readonly (string Invariant, string Type)[] Defaults =
		{
			("System.Data.SqlClient", "System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient"),
			("System.Data.OleDb", "System.Data.OleDb.OleDbFactory, System.Data.OleDb"),
			("System.Data.Odbc", "System.Data.Odbc.OdbcFactory, System.Data.Odbc"),
		};

		static int defaultsRegistered;

		/// <summary>
		/// .NET Framework's built-in providers, once: also where the application has not started (a SqlDataSource outside a
		/// request, a page's code before the first request's initialization).
		/// </summary>
		internal static void EnsureDefaults()
		{
			if (System.Threading.Interlocked.Exchange(ref defaultsRegistered, 1) == 1) return;
			foreach (var (invariant, type) in Defaults)
				if (!DbProviderFactories.TryGetFactory(invariant, out _)) DbProviderFactories.RegisterFactory(invariant, type);
		}

		internal static void Register(string applicationPath)
		{
			EnsureDefaults();
			if (string.IsNullOrEmpty(applicationPath) || !Directory.Exists(applicationPath)) return;
			var file = Directory.EnumerateFiles(applicationPath, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();
			if (file == null) return;
			XElement factories;
			try
			{
				factories = XDocument.Load(file).Root?.Elements().FirstOrDefault(e => e.Name.LocalName == "system.data")?
					.Elements().FirstOrDefault(e => e.Name.LocalName == "DbProviderFactories");
			}
			// A web.config that is no XML fails the application's configuration elsewhere, with its own error.
			catch (XmlException) { return; }
			if (factories == null) return;
			foreach (var entry in factories.Elements())
			{
				var invariant = (string)entry.Attribute("invariant");
				switch (entry.Name.LocalName)
				{
					case "clear":
						foreach (var name in DbProviderFactories.GetProviderInvariantNames().ToList()) DbProviderFactories.UnregisterFactory(name);
						break;
					case "remove":
						if (!string.IsNullOrEmpty(invariant)) DbProviderFactories.UnregisterFactory(invariant);
						break;
					case "add":
						var type = (string)entry.Attribute("type");
						if (!string.IsNullOrEmpty(invariant) && !string.IsNullOrEmpty(type)) DbProviderFactories.RegisterFactory(invariant, type);
						break;
				}
			}
		}
	}
}
