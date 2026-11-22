#if NETCOREAPP

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core = Microsoft.AspNetCore.Http;

namespace System.Web.Hosting
{
	/// <summary>
	/// IIS's output cache's profiles (system.webServer/caching/profiles: a type's policy, duration, location, varyByHeaders,
	/// varyByQueryString; caching's enabled), as IIS 10 answers them (the host's tests' iis-settings.json):
	/// <list type="bullet">
	/// <item>The answers of the profile's type have the location's Cache-Control (Any, Downstream: public; None: no-cache;
	/// Client, Server, ServerAndClient: the answer's own), whatever answered them (a static file, the handler, OPTIONS);
	/// not an error page's (IIS's, ASP.NET's), which have theirs.</item>
	/// <item>CacheUntilChange, CacheForTimePeriod (its duration): the handler's GET answer of 200 kept for the URL (by its
	/// varyByQueryString's parameters, "*" all, and its varyByHeaders' values; without regard to case) once it is asked for
	/// frequentHitThreshold times within frequentHitTimePeriod (serverRuntime's: 2 in 10 seconds), and answered from then
	/// on whatever the query's other parameters are (IIS 10: "?do=throw" after "?big=1" is the kept page). A static file's
	/// are not kept (IIS's file cache answers them as they are).</item>
	/// </list>
	/// kernelCachePolicy (http.sys's cache) answers the same. DontCache, DisableCache: nothing kept, no Cache-Control.
	/// </summary>
	internal sealed class IisOutputCache
	{
		sealed class Profile
		{
			public string Extension;
			public string Policy = "DontCache";
			public TimeSpan Duration = TimeSpan.FromSeconds(30);
			public string Location = "Server";
			public string[] VaryByHeaders = Array.Empty<string>();
			public string[] VaryByQueryString = Array.Empty<string>();
			public bool Keeps => Policy.Equals("CacheUntilChange", StringComparison.OrdinalIgnoreCase) || Policy.Equals("CacheForTimePeriod", StringComparison.OrdinalIgnoreCase);
			public bool Caches => Keeps;
		}

		sealed class Entry
		{
			public DateTime Expires;
			public string ContentType;
			public string CacheControl;   // the answer's own (a location's is put on it again: CacheControl)
			public byte[] Body;
		}

		readonly string contentRoot;
		readonly ConcurrentDictionary<string, (List<Profile> Profiles, int Threshold, TimeSpan Period)> byFolder =
			new ConcurrentDictionary<string, (List<Profile>, int, TimeSpan)>(StringComparer.OrdinalIgnoreCase);
		readonly ConcurrentDictionary<string, Entry> entries = new ConcurrentDictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
		readonly ConcurrentDictionary<string, List<DateTime>> hits = new ConcurrentDictionary<string, List<DateTime>>(StringComparer.OrdinalIgnoreCase);

		IisOutputCache(string contentRoot) { this.contentRoot = contentRoot; }

		/// <summary>The cache of the application in the folder; null when no configuration of it has caching profiles.</summary>
		internal static IisOutputCache Load(string contentRoot)
		{
			if (contentRoot == null) return null;
			var any = IisConfiguration.Sections(contentRoot).Any(s => IisConfiguration.Child(IisConfiguration.Child(s.WebServer, "caching"), "profiles")?.Elements().Any() == true);
			return any ? new IisOutputCache(contentRoot) : null;
		}

		(List<Profile> Profiles, int Threshold, TimeSpan Period) For(string path) => byFolder.GetOrAdd(IisConfiguration.Deepest(contentRoot, path), _ =>
		{
			var profiles = new List<Profile>();
			var enabled = true;
			int threshold = 2;
			var period = TimeSpan.FromSeconds(10);
			foreach (var section in IisConfiguration.For(contentRoot, path))
			{
				var caching = IisConfiguration.Child(section.WebServer, "caching");
				if ((string)caching?.Attribute("enabled") is { } on) enabled = !on.Trim().Equals("false", StringComparison.OrdinalIgnoreCase);
				foreach (var item in IisConfiguration.Child(caching, "profiles")?.Elements() ?? Enumerable.Empty<XElement>())
				{
					var extension = ((string)item.Attribute("extension"))?.Trim();
					switch (item.Name.LocalName)
					{
						case "clear": profiles.Clear(); break;
						case "remove" when !string.IsNullOrEmpty(extension): profiles.RemoveAll(p => p.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase)); break;
						case "add" when !string.IsNullOrEmpty(extension):
							profiles.RemoveAll(p => p.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase));
							var profile = new Profile { Extension = extension };
							profile.Policy = (string)item.Attribute("policy") ?? profile.Policy;
							if (TimeSpan.TryParse((string)item.Attribute("duration"), CultureInfo.InvariantCulture, out var duration)) profile.Duration = duration;
							profile.Location = (string)item.Attribute("location") ?? profile.Location;
							profile.VaryByHeaders = List((string)item.Attribute("varyByHeaders"));
							profile.VaryByQueryString = List((string)item.Attribute("varyByQueryString"));
							profiles.Add(profile);
							break;
					}
				}
				var runtime = IisConfiguration.Child(section.WebServer, "serverRuntime");
				if (int.TryParse((string)runtime?.Attribute("frequentHitThreshold"), out var t)) threshold = t;
				if (TimeSpan.TryParse((string)runtime?.Attribute("frequentHitTimePeriod"), CultureInfo.InvariantCulture, out var p)) period = p;
			}
			return (enabled ? profiles : new List<Profile>(), threshold, period);
		});

		static string[] List(string value) =>
			string.IsNullOrWhiteSpace(value) ? Array.Empty<string>() : value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim()).ToArray();

		static string Path(Core.HttpContext context)
		{
			var path = (context.Request.PathBase.Value ?? "") + (context.Request.Path.Value ?? "");
			return path.Length == 0 ? "/" : path;
		}

		Profile ProfileOf(string path, out int threshold, out TimeSpan period)
		{
			var folder = For(path);
			threshold = folder.Threshold;
			period = folder.Period;
			var extension = IisUrl.Extension(path);
			return extension.Length == 0 ? null : folder.Profiles.FirstOrDefault(p => p.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>The profile's Cache-Control on the request's answer, when it is not an error page.</summary>
		internal void CacheControl(Core.HttpContext context)
		{
			var profile = ProfileOf(Path(context), out _, out _);
			if (profile == null || !profile.Keeps) return;
			var value = profile.Location.ToLowerInvariant() switch { "any" => "public", "downstream" => "public", "none" => "no-cache", _ => null };
			if (value == null) return;
			context.Response.OnStarting(() =>
			{
				var flags = context.Features.Get<IisErrorFlags>();
				if (flags?.DetailedPage == true || flags?.TrySkipIisCustomErrors == true && context.Response.StatusCode >= 500) return Task.CompletedTask;
				context.Response.Headers.CacheControl = value;
				return Task.CompletedTask;
			});
		}

		/// <summary>
		/// The handler's answer from the cache (true when it answered), else the answer the next step makes, kept when it is to
		/// be (keep: the managed pipeline's answer, not a static file's).
		/// </summary>
		internal async Task<bool> RunAsync(Core.HttpContext context, Func<Task> next)
		{
			var path = Path(context);
			var profile = ProfileOf(path, out var threshold, out var period);
			if (profile == null || !profile.Caches || !Core.HttpMethods.IsGet(context.Request.Method)) return false;
			var key = Key(context, path, profile);
			if (entries.TryGetValue(key, out var entry))
			{
				if (entry.Expires > DateTime.UtcNow)
				{
					context.Response.StatusCode = 200;
					context.Response.ContentType = entry.ContentType;
					if (entry.CacheControl != null) context.Response.Headers.CacheControl = entry.CacheControl;
					context.Response.ContentLength = entry.Body.Length;
					IisDecision.Set(context, "HttpCacheModule");
					await context.Response.Body.WriteAsync(entry.Body, 0, entry.Body.Length);
					return true;
				}
				entries.TryRemove(key, out _);
			}
			// (asked for often enough: frequentHitThreshold times in frequentHitTimePeriod)
			var now = DateTime.UtcNow;
			var times = hits.GetOrAdd(key, _ => new List<DateTime>());
			bool frequent;
			lock (times)
			{
				times.Add(now);
				times.RemoveAll(t => now - t > period);
				frequent = times.Count >= threshold;
			}
			if (!frequent)
			{
				await next();
				return true;
			}
			var original = context.Response.Body;
			using var copy = new MemoryStream();
			context.Response.Body = new Tee(original, copy);
			try { await next(); }
			finally { context.Response.Body = original; }
			if (context.Response.StatusCode == 200 && context.Features.Get<IisStaticResponse>() == null)
				entries[key] = new Entry
				{
					Expires = profile.Policy.Equals("CacheForTimePeriod", StringComparison.OrdinalIgnoreCase) ? now + profile.Duration : DateTime.MaxValue,
					ContentType = context.Response.ContentType,
					CacheControl = context.Response.Headers.CacheControl.Count > 0 ? context.Response.Headers.CacheControl.ToString() : null,
					Body = copy.ToArray(),
				};
			return true;
		}

		static string Key(Core.HttpContext context, string path, Profile profile)
		{
			var key = new System.Text.StringBuilder(path);
			var query = context.Request.Query;
			if (profile.VaryByQueryString.Contains("*")) key.Append('?').Append(context.Request.QueryString.Value);
			else foreach (var name in profile.VaryByQueryString) key.Append('|').Append(name).Append('=').Append(query[name].ToString());
			foreach (var header in profile.VaryByHeaders) key.Append('|').Append(header).Append(':').Append(context.Request.Headers[header].ToString());
			return key.ToString();
		}

		// The answer written to the client and to the copy kept.
		sealed class Tee : Stream
		{
			readonly Stream inner, copy;
			public Tee(Stream inner, Stream copy) { this.inner = inner; this.copy = copy; }
			public override bool CanRead => false;
			public override bool CanSeek => false;
			public override bool CanWrite => true;
			public override long Length => throw new NotSupportedException();
			public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
			public override void Flush() => inner.Flush();
			public override Task FlushAsync(Threading.CancellationToken cancellationToken) => inner.FlushAsync(cancellationToken);
			public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
			public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
			public override void SetLength(long value) => throw new NotSupportedException();
			public override void Write(byte[] buffer, int offset, int count) { copy.Write(buffer, offset, count); inner.Write(buffer, offset, count); }
			public override async Task WriteAsync(byte[] buffer, int offset, int count, Threading.CancellationToken cancellationToken)
			{
				copy.Write(buffer, offset, count);
				await inner.WriteAsync(buffer, offset, count, cancellationToken);
			}
			public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, Threading.CancellationToken cancellationToken = default)
			{
				copy.Write(buffer.Span);
				await inner.WriteAsync(buffer, cancellationToken);
			}
		}
	}
}

#endif
