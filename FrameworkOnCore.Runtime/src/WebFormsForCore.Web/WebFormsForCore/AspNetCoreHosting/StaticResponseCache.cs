#if NETCOREAPP

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Primitives;
using Core = Microsoft.AspNetCore.Http;

namespace System.Web.Hosting
{
	/// <summary>
	/// IIS's kernel cache (http.sys's response cache) for static files, as the host can have it: a static file's response,
	/// once its URL is asked for often (serverRuntime's frequentHitThreshold, 2 times within frequentHitTimePeriod, 10
	/// seconds), is answered again as it was, before System.Web (no module, no global.asax event: as a response from the
	/// kernel cache reaches none on IIS), until a file of the application changes (FileCache). The responses kept are the
	/// ones http.sys keeps: IIS's static file handler's 200 with the whole file (IisStaticFile, at most 256 KB here) to an
	/// anonymous GET without a query string, a range, a condition, credentials or a body; not one with Set-Cookie, Vary,
	/// Content-Encoding or a private / no-store Cache-Control, nor one System.Web disabled the kernel cache for
	/// (HttpResponse.DisableKernelCache). system.webServer/caching's enabled and enableKernelCache false: none kept.
	/// A static file went through System.Web's pipeline each time (its modules' events, a hop to a worker thread):
	/// about the time of an empty page.
	/// </summary>
	internal static class StaticResponseCache
	{
		internal const int MaxBodyLength = 256 * 1024;
		const int MaxEntries = 10000;
		const long MaxCachedBytes = 64L * 1024 * 1024;

		sealed class Response
		{
			public long Generation;
			public KeyValuePair<string, StringValues>[] Headers;
			public byte[] Body;
			public IisStaticResponse File;   // the file (IIS's static compression looks at it when it is answered again)
		}

		sealed class Hits
		{
			public int Count;
			public long Since;
		}

		sealed class Settings
		{
			public bool Enabled = true;
			public int FrequentHitThreshold = 2;
			public TimeSpan FrequentHitTimePeriod = TimeSpan.FromSeconds(10);
		}

		static readonly HashSet<string> NotKept = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Date", "Content-Length", "Connection", "Keep-Alive", "Transfer-Encoding" };
		static readonly ConcurrentDictionary<string, Response> responses = new ConcurrentDictionary<string, Response>(StringComparer.Ordinal);
		static readonly ConcurrentDictionary<string, Hits> hits = new ConcurrentDictionary<string, Hits>(StringComparer.Ordinal);
		static long cachedBytes;
		static long cachedGeneration = -1;

		/// <summary>What a request's response is captured with, to be kept (AspNetCoreWorkerRequest).</summary>
		internal sealed class Capture
		{
			public readonly long Generation;
			public readonly string Key;
			readonly MemoryStream body = new MemoryStream();
			public bool Abandoned;

			public Capture(string key, long generation)
			{
				Key = key;
				Generation = generation;
			}

			public void Append(byte[] data, int offset, int length)
			{
				if (Abandoned) return;
				if (body.Length + length > MaxBodyLength) { Abandon(); return; }
				body.Write(data, offset, length);
			}

			public void Abandon()
			{
				Abandoned = true;
				body.SetLength(0);
			}

			public byte[] Body() => body.ToArray();
		}

		/// <summary>A request whose response may be kept or answered from what was kept (null key otherwise).</summary>
		static string KeyOf(Core.HttpContext context)
		{
			var request = context.Request;
			if (!string.Equals(request.Method, "GET", StringComparison.OrdinalIgnoreCase) || request.QueryString.HasValue || request.ContentLength > 0) return null;
			var headers = request.Headers;
			if (headers.ContainsKey("Range") || headers.ContainsKey("If-Range") || headers.ContainsKey("If-None-Match") ||
				headers.ContainsKey("If-Modified-Since") || headers.ContainsKey("If-Match") || headers.ContainsKey("If-Unmodified-Since") ||
				headers.ContainsKey("Authorization") || headers.ContainsKey("Transfer-Encoding")) return null;
			return request.Host.Value + "|" + request.PathBase.Value + request.Path.Value;
		}

		static Settings Read()
		{
			return FileCache.Memo("staticResponseCache:settings", () =>
			{
				var settings = new Settings();
				var file = HostingEnvironment.MapPath("~/web.config");
				if (file == null || !FileCache.FileExists(file)) return settings;
				XElement root;
				try { root = XDocument.Load(file).Root; }
				catch (Exception e) when (e is System.Xml.XmlException || e is IOException) { return settings; }
				if (root == null) return settings;
				var sections = root.Elements("system.webServer")
					.Concat(root.Elements("location")
						.Where(l => { var p = (string)l.Attribute("path"); return string.IsNullOrEmpty(p) || p == "."; })
						.Elements("system.webServer"));
				foreach (var section in sections)
				{
					foreach (var caching in section.Elements("caching"))
					{
						if (string.Equals((string)caching.Attribute("enabled"), "false", StringComparison.OrdinalIgnoreCase) ||
							string.Equals((string)caching.Attribute("enableKernelCache"), "false", StringComparison.OrdinalIgnoreCase))
							settings.Enabled = false;
					}
					foreach (var runtime in section.Elements("serverRuntime"))
					{
						int threshold;
						if (int.TryParse((string)runtime.Attribute("frequentHitThreshold"), out threshold) && threshold > 0) settings.FrequentHitThreshold = threshold;
						TimeSpan period;
						if (TimeSpan.TryParse((string)runtime.Attribute("frequentHitTimePeriod"), System.Globalization.CultureInfo.InvariantCulture, out period)) settings.FrequentHitTimePeriod = period;
					}
				}
				// A folder's own caching section (a <location> for a path, a folder's web.config) is not read: none kept then.
				if (root.Elements("location").Any(l =>
					{
						var p = (string)l.Attribute("path");
						return !string.IsNullOrEmpty(p) && p != "." && l.Elements("system.webServer").Elements("caching").Any();
					}))
					settings.Enabled = false;
				return settings;
			});
		}

		static bool Current(long generation)
		{
			var now = FileCache.Generation;
			if (Interlocked.Exchange(ref cachedGeneration, now) != now)
			{
				// Files changed since: nothing kept is the response any more.
				responses.Clear();
				hits.Clear();
				Interlocked.Exchange(ref cachedBytes, 0);
			}
			return generation == now;
		}

		/// <summary>The response kept for the request, answered; false if there is none.</summary>
		internal static async Task<bool> TryServe(Core.HttpContext context)
		{
			if (!FileCache.Watching || responses.IsEmpty) return false;
			var key = KeyOf(context);
			Response kept;
			if (key == null || !responses.TryGetValue(key, out kept) || !Current(kept.Generation)) return false;
			var response = context.Response;
			response.StatusCode = 200;
			if (kept.File != null) context.Features.Set(kept.File);
			foreach (var header in kept.Headers) response.Headers[header.Key] = header.Value;
			response.ContentLength = kept.Body.Length;
			await response.Body.WriteAsync(kept.Body, 0, kept.Body.Length);
			return true;
		}

		/// <summary>
		/// A capture for the request's response (IIS's static file handler's whole file to an anonymous user: IisStaticFile),
		/// if it is a request whose response may be kept; generation: the files' state when the request came.
		/// </summary>
		internal static Capture Start(Core.HttpContext context, long generation)
		{
			if (!FileCache.Watching) return null;
			var key = KeyOf(context);
			return key == null ? null : new Capture(key, generation);
		}

		/// <summary>The request's response kept, if it is one to keep and its URL is asked for often.</summary>
		internal static void Store(Core.HttpContext context, Capture capture)
		{
			if (capture == null || capture.Abandoned) return;
			var response = context.Response;
			if (response.StatusCode != 200) return;
			var headers = response.Headers;
			// IIS's features' own (customHeaders, compression's Content-Encoding and Vary) are added again when it is answered.
			var added = context.Features.Get<IisAddedHeaders>();
			bool Own(string name) => headers.ContainsKey(name) && added?.Has(name) != true;
			if (Own("Set-Cookie") || Own("Vary") || Own("Content-Encoding")) return;
			var cacheControl = headers.CacheControl.ToString();
			if (cacheControl.IndexOf("private", StringComparison.OrdinalIgnoreCase) >= 0 || cacheControl.IndexOf("no-store", StringComparison.OrdinalIgnoreCase) >= 0) return;
			var settings = Read();
			if (!settings.Enabled || !Current(capture.Generation)) return;

			// IIS's frequent hits: kept once asked for frequentHitThreshold times within frequentHitTimePeriod.
			var now = Environment.TickCount64;
			if (hits.Count >= MaxEntries) hits.Clear();
			var record = hits.GetOrAdd(capture.Key, _ => new Hits { Since = now });
			bool frequent;
			lock (record)
			{
				if (now - record.Since > (long)settings.FrequentHitTimePeriod.TotalMilliseconds)
				{
					record.Since = now;
					record.Count = 0;
				}
				record.Count++;
				frequent = record.Count >= settings.FrequentHitThreshold;
			}
			if (!frequent) return;

			var body = capture.Body();
			if (response.ContentLength != null && response.ContentLength != body.Length) return;
			if (responses.Count >= MaxEntries || Interlocked.Add(ref cachedBytes, body.Length) > MaxCachedBytes) return;
			var kept = new Response
			{
				Generation = capture.Generation,
				// Not the connection's (Kestrel's own: the date of the response kept, its length set from the body, the
				// connection's).
				// Nor IIS's customHeaders (IisHttpProtocol adds them to every response, this one's again).
				Headers = headers.Where(h => !NotKept.Contains(h.Key) && added?.Has(h.Key) != true).ToArray(),
				File = context.Features.Get<IisStaticResponse>(),
				Body = body,
			};
			responses[capture.Key] = kept;
		}
	}
}

#endif
