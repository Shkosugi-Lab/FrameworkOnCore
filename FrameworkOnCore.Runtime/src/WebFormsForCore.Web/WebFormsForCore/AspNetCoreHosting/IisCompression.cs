#if NETCOREAPP

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Pipelines;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core = Microsoft.AspNetCore.Http;

namespace System.Web.Hosting
{
	/// <summary>
	/// IIS's compression (Compression:Mode Iis, the default): its static and dynamic compression modules as the original
	/// server has them (IisProfile's globalModules; without it, Windows' default: static only), httpCompression (the
	/// server's: its schemes, staticTypes, dynamicTypes, minFileSizeForComp, noCompressionForHttp10, noCompressionForProxies,
	/// noCompressionForRange, staticCompressionIgnoreHitFrequency), urlCompression (doStaticCompression,
	/// doDynamicCompression: each folder's), serverRuntime's frequentHitThreshold and frequentHitTimePeriod. As IIS 10
	/// answers (its documentation; checked on IIS, samples/HeadersProbe, experiments/wf4c/compression-check.ps1):
	/// <list type="bullet">
	/// <item>200 answers only (not the errors, not the redirects); a client that takes the scheme (Accept-Encoding, q above
	/// 0; the schemes in the server's order: gzip, deflate, br); not HTTP/1.0, not through a proxy (Via), not a range.</item>
	/// <item>A static file (IisStaticFile, a kept response: StaticResponseCache): static compression once the file is
	/// asked for frequentHitThreshold times within frequentHitTimePeriod (2 in 10 seconds), of minFileSizeForComp bytes
	/// (2,700) or more, its type in staticTypes; before that, dynamic compression if the server has it.</item>
	/// <item>The rest (pages, handlers, IIS's listing): dynamic compression, its type in dynamicTypes.</item>
	/// <item>Content-Encoding and Vary: Accept-Encoding, the ETag as it is; the whole answer compressed (Content-Length),
	/// or, once the application flushes, compressed as it goes (chunked).</item>
	/// </list>
	/// </summary>
	internal sealed class IisCompression
	{
		sealed class Server
		{
			public bool StaticModule = true;
			public bool DynamicModule;
			public List<string> Schemes = new List<string> { "gzip" };
			public List<(Regex Type, bool Enabled)> StaticTypes = Types("text/*", "message/*", "application/javascript", "application/atom+xml", "application/xaml+xml", "image/svg+xml");
			public List<(Regex Type, bool Enabled)> DynamicTypes = Types("text/*", "message/*", "application/x-javascript", "application/javascript");
			public long MinFileSize = 2700;
			public bool NoHttp10 = true;
			public bool NoProxies = true;
			public bool NoRange = true;
			public bool IgnoreHitFrequency;
		}

		sealed class Folder
		{
			public bool Static = true;
			public bool Dynamic = true;
			public int FrequentHitThreshold = 2;
			public TimeSpan FrequentHitTimePeriod = TimeSpan.FromSeconds(10);
		}

		sealed class Hits
		{
			public int Count;
			public long Since;
		}

		readonly string contentRoot;
		readonly Server server = new Server();
		readonly ConcurrentDictionary<string, Folder> byFolder = new ConcurrentDictionary<string, Folder>(StringComparer.OrdinalIgnoreCase);
		readonly ConcurrentDictionary<string, Hits> hits = new ConcurrentDictionary<string, Hits>(StringComparer.OrdinalIgnoreCase);
		// IIS's compressed static files (its "IIS Temporary Compressed Files"): in memory here, by file, time and scheme.
		readonly ConcurrentDictionary<string, byte[]> compressed = new ConcurrentDictionary<string, byte[]>(StringComparer.Ordinal);
		long compressedBytes;
		const long MaxCompressedBytes = 64L * 1024 * 1024;

		static List<(Regex, bool)> Types(params string[] enabled)
		{
			var types = enabled.Select(t => (TypePattern(t), true)).ToList();
			types.Add((TypePattern("*/*"), false));
			return types;
		}

		static Regex TypePattern(string type) =>
			new Regex("^" + Regex.Escape(type.Trim()).Replace(@"\*", "[^;]*") + "$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

		internal IisCompression(string contentRoot)
		{
			this.contentRoot = contentRoot;
			var profile = IisProfile.Of(contentRoot);
			if (profile == null) return;
			var modules = IisConfiguration.Children(IisConfiguration.Child(profile, "globalModules"), "add").Select(m => (string)m.Attribute("name")).ToList();
			server.StaticModule = modules.Contains("StaticCompressionModule", StringComparer.OrdinalIgnoreCase);
			server.DynamicModule = modules.Contains("DynamicCompressionModule", StringComparer.OrdinalIgnoreCase);
			// httpCompression: the server's (applicationHost.config's) only.
			foreach (var compression in IisConfiguration.Children(profile, "system.webServer").Select(w => IisConfiguration.Child(w, "httpCompression")).Where(c => c != null))
			{
				var schemes = IisConfiguration.Children(compression, "scheme").Select(s => ((string)s.Attribute("name") ?? "").ToLowerInvariant())
					.Where(s => s == "gzip" || s == "deflate" || s == "br").ToList();
				if (schemes.Count > 0) server.Schemes = schemes;
				var staticTypes = TypesOf(IisConfiguration.Child(compression, "staticTypes"));
				if (staticTypes != null) server.StaticTypes = staticTypes;
				var dynamicTypes = TypesOf(IisConfiguration.Child(compression, "dynamicTypes"));
				if (dynamicTypes != null) server.DynamicTypes = dynamicTypes;
				long size;
				if (long.TryParse((string)compression.Attribute("minFileSizeForComp"), out size)) server.MinFileSize = size;
				bool flag;
				if (bool.TryParse((string)compression.Attribute("noCompressionForHttp10"), out flag)) server.NoHttp10 = flag;
				if (bool.TryParse((string)compression.Attribute("noCompressionForProxies"), out flag)) server.NoProxies = flag;
				if (bool.TryParse((string)compression.Attribute("noCompressionForRange"), out flag)) server.NoRange = flag;
				if (bool.TryParse((string)compression.Attribute("staticCompressionIgnoreHitFrequency"), out flag)) server.IgnoreHitFrequency = flag;
			}
		}

		static List<(Regex, bool)> TypesOf(XElement types)
		{
			if (types == null) return null;
			var list = new List<(Regex, bool)>();
			foreach (var item in types.Elements())
			{
				var type = (string)item.Attribute("mimeType");
				switch (item.Name.LocalName)
				{
					case "clear": list.Clear(); break;
					case "add" when !string.IsNullOrEmpty(type):
						bool enabled;
						list.Add((TypePattern(type), !bool.TryParse((string)item.Attribute("enabled"), out enabled) || enabled));
						break;
				}
			}
			return list;
		}

		// A type's entry: the first that matches (IIS's order), its enabled.
		static bool Takes(List<(Regex Type, bool Enabled)> types, string contentType)
		{
			var type = (contentType ?? "").Split(';')[0].Trim();
			if (type.Length == 0) return false;
			foreach (var (pattern, enabled) in types)
				if (pattern.IsMatch(type)) return enabled;
			return false;
		}

		Folder For(string path) => byFolder.GetOrAdd(IisConfiguration.Deepest(contentRoot, path), _ =>
		{
			var folder = new Folder();
			foreach (var section in IisConfiguration.For(contentRoot, path))
			{
				bool flag;
				var url = IisConfiguration.Child(section.WebServer, "urlCompression");
				if (bool.TryParse((string)url?.Attribute("doStaticCompression"), out flag)) folder.Static = flag;
				if (bool.TryParse((string)url?.Attribute("doDynamicCompression"), out flag)) folder.Dynamic = flag;
				var runtime = IisConfiguration.Child(section.WebServer, "serverRuntime");
				int threshold;
				if (int.TryParse((string)runtime?.Attribute("frequentHitThreshold"), out threshold)) folder.FrequentHitThreshold = threshold;
				TimeSpan period;
				if (TimeSpan.TryParse((string)runtime?.Attribute("frequentHitTimePeriod"), global::System.Globalization.CultureInfo.InvariantCulture, out period)) folder.FrequentHitTimePeriod = period;
			}
			return folder;
		});

		/// <summary>Whether the server compresses at all (one of its modules): the middleware is not needed otherwise.</summary>
		internal bool Any => server.StaticModule || server.DynamicModule;

		// The scheme the client takes (the server's order), null for none; what the request is (HTTP/1.0, a proxy's, a range).
		string Scheme(Core.HttpRequest request)
		{
			if (server.NoHttp10 && request.Protocol == "HTTP/1.0") return null;
			if (server.NoProxies && request.Headers.ContainsKey("Via")) return null;
			if (server.NoRange && request.Headers.ContainsKey("Range")) return null;
			var accepted = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
			foreach (var item in request.Headers.AcceptEncoding.ToString().Split(','))
			{
				var parts = item.Split(';');
				var name = parts[0].Trim();
				if (name.Length == 0) continue;
				double quality = 1;
				foreach (var parameter in parts.Skip(1))
				{
					var pair = parameter.Split('=');
					if (pair.Length == 2 && pair[0].Trim().Equals("q", StringComparison.OrdinalIgnoreCase))
						double.TryParse(pair[1].Trim(), global::System.Globalization.NumberStyles.Float, global::System.Globalization.CultureInfo.InvariantCulture, out quality);
				}
				accepted[name] = quality;
			}
			return server.Schemes.FirstOrDefault(s => accepted.TryGetValue(s, out var q) && q > 0);
		}

		/// <summary>How the answer is compressed (its scheme, static or not), null when it is not: decided when it starts.</summary>
		(string Scheme, bool Static)? Decide(Core.HttpContext context)
		{
			var response = context.Response;
			if (response.StatusCode != 200 || response.Headers.ContainsKey("Content-Encoding")) return null;
			var request = context.Request;
			var scheme = Scheme(request);
			var path = (request.PathBase.Value ?? "") + (request.Path.Value ?? "/");
			var folder = For(path);
			var file = context.Features.Get<IisStaticResponse>();
			if (file != null)
			{
				// A static file's hits counted whatever the client takes (IIS's frequent hits: its file cache's).
				var frequent = server.IgnoreHitFrequency || Frequent(file.PhysicalPath, folder);
				if (scheme == null) return null;
				if (server.StaticModule && folder.Static && frequent && file.Length >= server.MinFileSize && Takes(server.StaticTypes, response.ContentType))
					return (scheme, true);
			}
			if (scheme == null) return null;
			if (server.DynamicModule && folder.Dynamic && Takes(server.DynamicTypes, response.ContentType)) return (scheme, false);
			return null;
		}

		// A static file answered uncompressed whatever it was (the client takes no scheme): its hit counted all the same.
		void Count(Core.HttpContext context)
		{
			var file = context.Features.Get<IisStaticResponse>();
			if (file == null || context.Response.StatusCode != 200 || server.IgnoreHitFrequency) return;
			var request = context.Request;
			Frequent(file.PhysicalPath, For((request.PathBase.Value ?? "") + (request.Path.Value ?? "/")));
		}

		bool Frequent(string file, Folder folder)
		{
			var now = Environment.TickCount64;
			if (hits.Count >= 10000) hits.Clear();
			var record = hits.GetOrAdd(file, _ => new Hits { Since = now });
			lock (record)
			{
				if (now - record.Since > (long)folder.FrequentHitTimePeriod.TotalMilliseconds)
				{
					record.Since = now;
					record.Count = 0;
				}
				record.Count++;
				return record.Count >= folder.FrequentHitThreshold;
			}
		}

		static Stream Compressor(string scheme, Stream output, bool forStatic)
		{
			// IIS's levels: static 7 (its best: kept), dynamic 0 (its fastest: each answer's).
			var level = forStatic ? CompressionLevel.Optimal : CompressionLevel.Fastest;
			switch (scheme)
			{
				case "deflate": return new ZLibStream(output, level, leaveOpen: true);
				case "br": return new BrotliStream(output, level, leaveOpen: true);
				default: return new GZipStream(output, level, leaveOpen: true);
			}
		}

		byte[] Compress(string scheme, bool forStatic, byte[] content, int length, IisStaticResponse file)
		{
			var key = file != null && forStatic ? file.PhysicalPath + "|" + file.LastWriteTicks + "|" + file.Length + "|" + scheme : null;
			byte[] kept;
			if (key != null && compressed.TryGetValue(key, out kept)) return kept;
			using var output = new MemoryStream();
			using (var compressor = Compressor(scheme, output, forStatic)) compressor.Write(content, 0, length);
			var result = output.ToArray();
			if (key != null && Interlocked.Add(ref compressedBytes, result.Length) <= MaxCompressedBytes) compressed[key] = result;
			return result;
		}

		/// <summary>The response's body: compressed once it is known to be one IIS compresses.</summary>
		sealed class Body : Stream, Core.Features.IHttpResponseBodyFeature
		{
			readonly IisCompression compression;
			readonly Core.HttpContext context;
			internal readonly Core.Features.IHttpResponseBodyFeature Inner;
			PipeWriter writer;
			bool decided;
			string scheme;
			bool forStatic;
			MemoryStream buffer;     // the answer so far, until it ends or is flushed
			Stream streaming;        // the compressor once flushed
			internal bool CompletePending;

			public Body(IisCompression compression, Core.HttpContext context, Core.Features.IHttpResponseBodyFeature inner)
			{
				this.compression = compression;
				this.context = context;
				Inner = inner;
			}

			bool Compressing
			{
				get
				{
					if (!decided)
					{
						decided = true;
						var decision = compression.Decide(context);
						if (decision != null)
						{
							(scheme, forStatic) = decision.Value;
							var headers = context.Response.Headers;
							headers.ContentLength = null;
							headers["Content-Encoding"] = scheme;
							Core.HeaderDictionaryExtensions.Append(headers, "Vary", "Accept-Encoding");
							IisAddedHeaders.Add(context, "Content-Encoding");
							IisAddedHeaders.Add(context, "Vary");
							buffer = new MemoryStream();
						}
					}
					return scheme != null;
				}
			}

			internal Task FlushWriterAsync() => writer == null ? Task.CompletedTask : writer.FlushAsync().AsTask();

			/// <summary>The end: the whole answer compressed (its length known), or the compressor's end.</summary>
			internal async Task FinishAsync()
			{
				if (scheme == null) return;
				if (streaming != null)
				{
					await streaming.DisposeAsync();
					return;
				}
				var content = compression.Compress(scheme, forStatic, buffer.GetBuffer(), (int)buffer.Length, context.Features.Get<IisStaticResponse>());
				context.Response.ContentLength = content.Length;
				if (!Core.HttpMethods.IsHead(context.Request.Method)) await Inner.Stream.WriteAsync(content, 0, content.Length);
			}

			// Flushed by the application: what is written from now on goes as it comes, compressed (chunked).
			async Task StreamAsync(CancellationToken cancellationToken)
			{
				if (streaming != null) return;
				streaming = Compressor(scheme, Inner.Stream, forStatic);
				if (buffer.Length > 0) await streaming.WriteAsync(buffer.GetBuffer(), 0, (int)buffer.Length, cancellationToken);
				buffer = null;
			}

			public Stream Stream => this;
			public PipeWriter Writer => writer ??= PipeWriter.Create(this, new StreamPipeWriterOptions(leaveOpen: true));
			public void DisableBuffering() => Inner.DisableBuffering();
			public Task StartAsync(CancellationToken cancellationToken = default) =>
				Compressing ? Task.CompletedTask : Inner.StartAsync(cancellationToken);
			public async Task SendFileAsync(string path, long offset, long? count, CancellationToken cancellationToken = default)
			{
				if (!Compressing)
				{
					await Inner.SendFileAsync(path, offset, count, cancellationToken);
					return;
				}
				using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 1, true);
				file.Seek(offset, SeekOrigin.Begin);
				var target = (Stream)streaming ?? buffer;
				if (count == null) await file.CopyToAsync(target, cancellationToken);
				else
				{
					var chunk = new byte[64 * 1024];
					for (var left = count.Value; left > 0;)
					{
						var read = await file.ReadAsync(chunk, 0, (int)Math.Min(chunk.Length, left), cancellationToken);
						if (read == 0) break;
						await target.WriteAsync(chunk, 0, read, cancellationToken);
						left -= read;
					}
				}
			}
			public Task CompleteAsync()
			{
				if (decided && scheme == null) return Inner.CompleteAsync();
				CompletePending = true;   // the middleware ends it (compressed, or with nothing written yet)
				return Task.CompletedTask;
			}

			public override bool CanRead => false;
			public override bool CanSeek => false;
			public override bool CanWrite => true;
			public override long Length => throw new NotSupportedException();
			public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
			public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
			public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
			public override void SetLength(long value) => throw new NotSupportedException();
			public override void Flush() => FlushAsync(CancellationToken.None).GetAwaiter().GetResult();
			public override async Task FlushAsync(CancellationToken cancellationToken)
			{
				if (!decided) return;   // nothing written yet: nothing to send
				if (scheme == null) { await Inner.Stream.FlushAsync(cancellationToken); return; }
				await StreamAsync(cancellationToken);
				await streaming.FlushAsync(cancellationToken);
				await Inner.Stream.FlushAsync(cancellationToken);
			}
			public override void Write(byte[] data, int offset, int count)
			{
				if (count == 0) return;
				if (!Compressing) { Inner.Stream.Write(data, offset, count); return; }
				((Stream)streaming ?? buffer).Write(data, offset, count);
			}
			public override Task WriteAsync(byte[] data, int offset, int count, CancellationToken cancellationToken) =>
				WriteAsync(new ReadOnlyMemory<byte>(data, offset, count), cancellationToken).AsTask();
			public override ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
			{
				if (data.Length == 0) return default;
				if (!Compressing) return Inner.Stream.WriteAsync(data, cancellationToken);
				return ((Stream)streaming ?? buffer).WriteAsync(data, cancellationToken);
			}
		}

		internal sealed class Middleware
		{
			readonly Core.RequestDelegate next;
			readonly IisCompression compression;

			public Middleware(Core.RequestDelegate next, IisCompression compression)
			{
				this.next = next;
				this.compression = compression;
			}

			public Task Invoke(Core.HttpContext context)
			{
				// A request whose answer is not compressed whatever it is (no scheme the client takes, HTTP/1.0, a proxy's, a
				// range): nothing in front of its answer. A static file's hit is counted then too (IIS's file cache counts them all).
				if (compression.Scheme(context.Request) == null)
				{
					var task = next(context);
					if (!task.IsCompletedSuccessfully) return CountAfterAsync(task, context);
					compression.Count(context);
					return Task.CompletedTask;
				}
				return CompressAsync(context);
			}

			async Task CountAfterAsync(Task task, Core.HttpContext context)
			{
				await task;
				compression.Count(context);
			}

			async Task CompressAsync(Core.HttpContext context)
			{
				var inner = context.Features.Get<Core.Features.IHttpResponseBodyFeature>();
				var body = new Body(compression, context, inner);
				context.Features.Set<Core.Features.IHttpResponseBodyFeature>(body);
				try
				{
					await next(context);
					await body.FlushWriterAsync();
				}
				finally
				{
					context.Features.Set(inner);
				}
				await body.FinishAsync();
				if (body.CompletePending) await inner.CompleteAsync();
			}
		}
	}

	/// <summary>A static file's whole answer (IisStaticFile, StaticResponseCache): what IIS's static compression looks at.</summary>
	internal sealed class IisStaticResponse
	{
		public string PhysicalPath;
		public long Length;
		public long LastWriteTicks;
	}
}

#endif
