#if NETCOREAPP

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core = Microsoft.AspNetCore.Http;

namespace System.Web.Hosting
{
	/// <summary>
	/// The URL as http.sys gives it to IIS (its canonical form, observed on IIS 10: tests/RuntimeUnitTests/Security/iis-golden):
	/// every escape decoded ("%2F" and "%5C" too, which Kestrel leaves or keeps as a file name's character), '\' a '/', a run
	/// of '/' one, the "." and ".." segments resolved after the decoding ("/sub/..%2fApp_Data" is /App_Data); a ".." above the
	/// root refused (403), an invalid escape ("%zz") or a control character (%0D%0A, %00) refused (400). Kestrel's own path
	/// kept "/a%2Fb", "//a", "/a\b" as they were: /sub%2fa.txt was 404, //a.txt and /sub\a.txt 400 where IIS serves them, a
	/// folder asked for as //sub was redirected to //sub/ (another host for a browser), URL Rewrite's {R:1} of /go//evil.example
	/// made "//evil.example" (an open redirect), and "/App_Data%2fusers.xml" was not seen as App_Data by request filtering.
	/// An escape sequence that is not UTF-8 is left as Kestrel has it (http.sys reads it in the system's code page: a name no
	/// file has). Once per request (the first of IIS's features or Web Forms), before URL Rewrite changes the path.
	/// </summary>
	internal static class HttpSysUrl
	{
		sealed class Done { }

		/// <summary>The request's path made canonical; false when it is refused (answered: 400 or 403).</summary>
		internal static async Task<bool> Canonicalize(Core.HttpContext context)
		{
			if (context.Features.Get<Done>() != null) return true;
			context.Features.Set(new Done());
			var raw = AspNetCoreWorkerRequest.RawTarget(context);
			var mark = raw.IndexOf('?');
			var status = Canonical(mark < 0 ? raw : raw.Substring(0, mark), out var canonical);
			// http.sys's other checks before IIS (Kestrel lets these through): a Host whose port is not one ("evil.example:99999",
			// 400 Invalid Hostname); a POST or PUT of HTTP/1.x with neither a length nor chunks (411 Length Required: Kestrel
			// takes it as having no body, and the application ran on an empty form).
			if (status == 0 && !ValidHost(context.Request.Host)) status = 400;
			if (status == 0 && (Core.HttpMethods.IsPost(context.Request.Method) || Core.HttpMethods.IsPut(context.Request.Method)) &&
				(context.Request.Protocol == "HTTP/1.1" || context.Request.Protocol == "HTTP/1.0") &&
				!context.Request.Headers.ContainsKey("Content-Length") && !context.Request.Headers.ContainsKey("Transfer-Encoding"))
				status = 411;
			if (status != 0)
			{
				// http.sys's answers: in its HTTPERR log, not in the site's; no IIS feature adds to them.
				IisLogStatus.SetNotLogged(context);
				context.Response.StatusCode = status;
				if (status == 400)
				{
					context.Response.ContentType = "text/html; charset=us-ascii";
					await Core.HttpResponseWritingExtensions.WriteAsync(context.Response, BadRequestPage);
				}
				return false;
			}
			if (canonical == null) return true;
			var request = context.Request;
			var pathBase = request.PathBase.Value ?? "";
			if (pathBase.Length == 0) request.Path = new Core.PathString(canonical);
			else if (canonical.StartsWith(pathBase, StringComparison.OrdinalIgnoreCase) && (canonical.Length == pathBase.Length || canonical[pathBase.Length] == '/'))
				request.Path = new Core.PathString(canonical.Substring(pathBase.Length));
			return true;
		}

		// A Host header's port, where it has one: digits, 0 to 65535.
		static bool ValidHost(Core.HostString host)
		{
			if (!host.HasValue) return true;
			var value = host.Value;
			var colon = value.LastIndexOf(':');
			if (colon < 0 || value.IndexOf(']', colon) >= 0) return true;
			var port = value.Substring(colon + 1);
			return port.Length > 0 && port.Length <= 5 && port.All(char.IsAsciiDigit) && int.Parse(port, System.Globalization.CultureInfo.InvariantCulture) <= 65535;
		}

		/// <summary>http.sys's page of its 400 for a URL it refuses.</summary>
		internal const string BadRequestPage =
			"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\"\"http://www.w3.org/TR/html4/strict.dtd\">\r\n<HTML><HEAD><TITLE>Bad Request</TITLE>\r\n" +
			"<META HTTP-EQUIV=\"Content-Type\" Content=\"text/html; charset=us-ascii\"></HEAD>\r\n<BODY><h2>Bad Request - Invalid URL</h2>\r\n" +
			"<hr><p>HTTP Error 400. The request URL is invalid.</p>\r\n</BODY></HTML>\r\n";

		/// <summary>
		/// The raw path's canonical form (decoded): 0 and the path (null where the path is not to be changed: no escape, no
		/// separator to fold, or an escape that is not UTF-8), or the status http.sys refuses it with.
		/// </summary>
		internal static int Canonical(string rawPath, out string canonical)
		{
			canonical = null;
			if (string.IsNullOrEmpty(rawPath) || rawPath[0] != '/') return 0;
			var simple = true;
			foreach (var c in rawPath)
				if (c == '%' || c == '\\' || c < 0x20 || c == 0x7F) { simple = false; break; }
			if (simple && rawPath.IndexOf("//", StringComparison.Ordinal) < 0 && rawPath.IndexOf("/.", StringComparison.Ordinal) < 0) return 0;

			// The escapes as bytes.
			var bytes = new byte[rawPath.Length];
			var length = 0;
			for (var i = 0; i < rawPath.Length; i++)
			{
				var c = rawPath[i];
				if (c == '%')
				{
					if (i + 2 >= rawPath.Length || !Uri.IsHexDigit(rawPath[i + 1]) || !Uri.IsHexDigit(rawPath[i + 2])) return 400;
					c = (char)Convert.ToByte(rawPath.Substring(i + 1, 2), 16);
					i += 2;
				}
				if (c < 0x20 || c == 0x7F) return 400;
				if (c > 0xFF) return 0;   // (not a request line's: Kestrel refuses them)
				bytes[length++] = (byte)c;
			}
			string decoded;
			try { decoded = new UTF8Encoding(false, true).GetString(bytes, 0, length); }
			catch (DecoderFallbackException) { return 0; }

			// The segments: '\' a separator, empty ones (a run of '/') folded, "." and ".." resolved.
			var segments = decoded.Replace('\\', '/').Split('/');
			var kept = new System.Collections.Generic.List<string>(segments.Length);
			for (var i = 1; i < segments.Length; i++)
			{
				var segment = segments[i];
				var last = i == segments.Length - 1;
				if (segment == ".")
				{
					if (last) kept.Add("");
					continue;
				}
				if (segment == "..")
				{
					if (kept.Count == 0) return 403;
					kept.RemoveAt(kept.Count - 1);
					if (last) kept.Add("");
					continue;
				}
				if (segment.Length == 0 && !last) continue;
				kept.Add(segment);
			}
			canonical = "/" + string.Join("/", kept);
			return 0;
		}
	}
}

#endif
