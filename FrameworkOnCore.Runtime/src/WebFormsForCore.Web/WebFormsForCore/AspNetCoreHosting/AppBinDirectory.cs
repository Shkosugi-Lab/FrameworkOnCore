namespace System.Web.Hosting
{
	// The folder the application's assemblies are in (bin): AppDomain.BaseDirectory as .NET starts the
	// process. The host then sets AppDomain.BaseDirectory to the application's root, as it is on .NET
	// Framework (AspNetCoreHost.Configure): this is read before, and kept. A class of its own: read
	// before the application's HttpRuntime is (WebFormsMiddleware), which must not be initialized then.
	internal static class AppBinDirectory
	{
		static string physicalPath;
		internal static string PhysicalPath => physicalPath ??= AppContext.BaseDirectory;
	}
}
