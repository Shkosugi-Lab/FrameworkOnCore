using System;
using System.IO;
using System.Runtime.InteropServices;

namespace WebFormsForCore;

/// <summary>
/// File names on IIS are case-insensitive, and Web Forms applications are written that way
/// ("Web.Config", "~/default.aspx" for Default.aspx, "App_data"). On a case-sensitive file system,
/// resolves a physical path to the file or directory that exists under a different casing.
/// </summary>
public static class PhysicalPathCasing
{
	/// <summary>
	/// WEBFORMSFORCORE_PATH_CASING=0: paths are used as asked. For a process whose file system calls already find names
	/// without regard to case (a preloaded library, a case-insensitive file system), where resolving them here is only
	/// work done twice.
	/// </summary>
	public static readonly bool Enabled = Environment.GetEnvironmentVariable("WEBFORMSFORCORE_PATH_CASING") != "0";

	/// <summary>
	/// The path as it exists on disk, segment by segment. Where a segment exists under no casing (or
	/// under several), the rest of the path is kept as asked (a file about to be created).
	/// Unchanged on Windows and when the path exists as it is.
	/// </summary>
	public static string Resolve(string path)
	{
		if (!Enabled || string.IsNullOrEmpty(path) || OSInfo.IsWindows || File.Exists(path) || Directory.Exists(path)) return path;

		try
		{
			var root = Path.GetPathRoot(path);
			if (string.IsNullOrEmpty(root)) return path;

			var segments = path.Substring(root.Length).Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
			var current = root;
			for (int i = 0; i < segments.Length; i++)
			{
				var next = Path.Combine(current, segments[i]);
				if (!Directory.Exists(next) && !File.Exists(next))
				{
					var match = FindEntry(current, segments[i]);
					if (match == null)
					{
						for (int j = i; j < segments.Length; j++) current = Path.Combine(current, segments[j]);
						break;
					}
					next = match;
				}
				current = next;
			}
			if (path.EndsWith(Path.DirectorySeparatorChar) && !current.EndsWith(Path.DirectorySeparatorChar)) current += Path.DirectorySeparatorChar;
			return current;
		}
		catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException)
		{
			return path;
		}
	}

	/// <summary>
	/// The entry of <paramref name="directory"/> named <paramref name="name"/> in any casing; null
	/// when there is none, or more than one (ambiguous).
	/// </summary>
	public static string FindEntry(string directory, string name)
	{
		if (!Directory.Exists(directory)) return null;
		if (!Enabled)
		{
			var exact = Path.Combine(directory, name);
			return File.Exists(exact) || Directory.Exists(exact) ? exact : null;
		}

		string match = null;
		foreach (var entry in Directory.EnumerateFileSystemEntries(directory))
		{
			if (!string.Equals(Path.GetFileName(entry), name, StringComparison.OrdinalIgnoreCase)) continue;
			if (match != null) return null;
			match = entry;
		}
		return match;
	}
}
