using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace WebFormsForCore.CodeDom.Compiler
{
	/// <summary>
	/// The runtime's facades for the assembly names .NET Framework and .NET Standard libraries
	/// reference their types by (mscorlib, netstandard, System, System.Core, System.Data, System.Xml...).
	/// Pages and views are compiled against such libraries, so these belong on every compilation's
	/// reference list: a view calling ASP.NET MVC's HtmlHelper extensions (Expression&lt;&gt; from
	/// System.Core) failed with CS0012, as a page calling ASP.NET Identity's GetUserName(this IIdentity)
	/// (mscorlib) did.
	///
	/// Every assembly of the runtime's directory that only forwards types; not one the application has
	/// itself (System.Web: the fork's, not the runtime's facade for HttpUtility).
	/// </summary>
	internal static class FrameworkFacades
	{
		private static readonly Lazy<string[]> paths = new Lazy<string[]>(() =>
		{
			var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
			string bin = null;
			try { bin = System.Web.Hosting.AppBinDirectory.PhysicalPath; } catch (Exception) { }
			return Directory.EnumerateFiles(runtimeDirectory, "*.dll")
				.Where(IsFacade)
				.Where(file => bin == null || !File.Exists(Path.Combine(bin, Path.GetFileName(file))))
				.Where(file => !string.Equals(Path.GetFileName(file), "System.Web.dll", StringComparison.OrdinalIgnoreCase))
				.OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
				.ToArray();
		});

		// Metadata with no type of its own (<Module> only) and types forwarded elsewhere.
		private static bool IsFacade(string file)
		{
			try
			{
				using var stream = File.OpenRead(file);
				using var pe = new PEReader(stream);
				if (!pe.HasMetadata) return false;
				var reader = pe.GetMetadataReader();
				return reader.IsAssembly && reader.TypeDefinitions.Count <= 1 && reader.ExportedTypes.Count > 0;
			}
			catch (BadImageFormatException) { return false; }
		}

		public static string[] Paths => paths.Value;
	}
}
