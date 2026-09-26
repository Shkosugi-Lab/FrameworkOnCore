using System;
using System.IO;
using System.Linq;

namespace WebFormsForCore.CodeDom.Compiler
{
	/// <summary>
	/// The runtime's facades for the assembly names .NET Framework and .NET Standard libraries
	/// reference their base types by (mscorlib, netstandard). Pages are compiled against such
	/// libraries, so these belong on every compilation's reference list.
	/// </summary>
	internal static class FrameworkFacades
	{
		private static readonly Lazy<string[]> paths = new Lazy<string[]>(() =>
		{
			var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
			return new[] { "mscorlib.dll", "netstandard.dll" }
				.Select(name => Path.Combine(runtimeDirectory, name))
				.Where(File.Exists)
				.ToArray();
		});

		public static string[] Paths => paths.Value;
	}
}
