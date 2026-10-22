#if NETCOREAPP

using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace System.Web.Util
{
	/// <summary>
	/// The satellites of .NET's own assemblies in the application's folder: FrameworkOnCore's translations of their texts
	/// (bin\ja\System.ComponentModel.Annotations.resources.dll: DataAnnotations' validation messages, which .NET Framework's
	/// language packs had in the server's language). .NET looks for a satellite next to its assembly only (the shared
	/// framework's folder, which has none); then in the application's folder (bin\&lt;culture&gt;\&lt;name&gt;.resources.dll).
	/// </summary>
	internal static class FrameworkSatellites
	{
		[ModuleInitializer]
		internal static void Initialize() => AssemblyLoadContext.Default.Resolving += Resolve;

		static Assembly Resolve(AssemblyLoadContext context, AssemblyName name)
		{
			if (string.IsNullOrEmpty(name.CultureName) || name.Name == null || !name.Name.EndsWith(".resources", StringComparison.Ordinal)) return null;
			var path = Path.Combine(AppContext.BaseDirectory, name.CultureName, name.Name + ".dll");
			return File.Exists(path) ? context.LoadFromAssemblyPath(path) : null;
		}
	}
}

#endif
