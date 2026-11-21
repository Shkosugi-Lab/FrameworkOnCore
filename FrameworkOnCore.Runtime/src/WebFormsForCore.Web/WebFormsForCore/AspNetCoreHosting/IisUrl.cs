#if NETCOREAPP

using System;

namespace System.Web.Hosting
{
	/// <summary>
	/// A URL's file and its path info as IIS maps them, in one place for every step that asks: request filtering (the file's
	/// type is checked, as the last segment's is), the choice of System.Web or the static files (a handler's file with path
	/// info is System.Web's), the worker request's FilePath and PathInfo. Each step had its own way of finding the file, and
	/// they did not agree: the request filtering looked at the last segment, the worker request ran the file before it
	/// ("/locked/page.aspx/x" in a folder denying .aspx: the page ran; "/appsettings.json/x": the file was served).
	/// </summary>
	internal static class IisUrl
	{
		/// <summary>
		/// The file of a path and its path info: the first segment before the last of a type IIS maps to a handler (its script
		/// maps: .aspx, .ashx ...), or, given isFile, that is a file (the worker request's: a static file with path info, which
		/// IisStaticFile answers 404); the rest is the path info ("/Page.aspx/x/y": "/Page.aspx" and "/x/y"). The path and
		/// no path info when there is none.
		/// </summary>
		internal static (string FilePath, string PathInfo) Split(string path, Func<string, bool> handled, Func<string, bool> isFile = null)
		{
			if (path.Length < 2) return (path, string.Empty);   // "", "/", "*" (OPTIONS *)
			for (var slash = path.IndexOf('/', 1); slash > 0; slash = path.IndexOf('/', slash + 1))
			{
				var candidate = path[..slash];
				var segment = candidate[(candidate.LastIndexOf('/') + 1)..];
				if (segment.IndexOf('.') < 0) continue;
				if (handled(segment) || isFile?.Invoke(candidate) == true) return (candidate, path[slash..]);
			}
			return (path, string.Empty);
		}

		/// <summary>A path's last segment's type ("" when it has none): ".aspx" of "/a/Page.aspx".</summary>
		internal static string Extension(string path)
		{
			var name = path[(path.LastIndexOf('/') + 1)..];
			var dot = name.LastIndexOf('.');
			return dot < 0 ? string.Empty : name[dot..];
		}
	}
}

#endif
