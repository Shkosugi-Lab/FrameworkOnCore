#if NETCOREAPP

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core = Microsoft.AspNetCore.Http;

namespace System.Web.Hosting
{
	/// <summary>
	/// Who IIS lets in, after its request filtering and before the handler (IIS 10's answers to each setting: the host's tests'
	/// iis-settings.json), each the URL's folder's (the original server's settings, web.config's, its locations' and folders'):
	/// <list type="bullet">
	/// <item>IP restrictions (security/ipSecurity): the client's address against the entries (ipAddress and subnetMask, the
	/// most specific one's allowed), else allowUnlisted; enableProxyMode: X-Forwarded-For's first address. Refused as
	/// denyAction says: 403.503 (Forbidden), 401.503, 404.503, the connection closed (AbortRequest). domainName entries
	/// need reverse DNS (enableReverseDns), which the host does not do: not matched (IIS 10 the same without it).</item>
	/// <item>Authentication: anonymous authentication off (anonymousAuthentication enabled false) is the anonymous
	/// request's 401.2, with the challenges of the schemes on (basicAuthentication's Basic realm, the realm or the host;
	/// windowsAuthentication's providers, Negotiate and NTLM by default). A request with credentials (Authorization) is not
	/// one the host can check (Windows' accounts): 401.1 with the same challenges.</item>
	/// <item>URL authorization (security/authorization, IIS's own module): for the anonymous user, a Deny rule of "?" or "*"
	/// (of the request's verb, if the rule has verbs) refuses, whatever the order; else an Allow rule of "?" or "*" lets in;
	/// else 401.2. A request with a user of its own (an Authorization header, a cookie) is left to the rules only when they
	/// name no user or role (who it is, is System.Web's to say).</item>
	/// </list>
	/// </summary>
	internal static class IisAccess
	{
		// security/access sslFlags (IIS's schema: Ssl 8, SslNegotiateCert 32, SslRequireCert 64, Ssl128 256)
		const int Ssl = 8, SslNegotiateCert = 32, SslRequireCert = 64, Ssl128 = 256;

		/// <summary>
		/// The client certificates the host trusts besides the machine's trusted roots (null: the machine's only, as IIS: its
		/// tests' own).
		/// </summary>
		internal static X509Certificate2Collection TrustedClientCertificates;

		sealed class Settings
		{
			public int SslFlags;
			public List<(IPAddress Address, IPAddress Mask, bool Allowed)> Addresses = new List<(IPAddress, IPAddress, bool)>();
			public bool AllowUnlisted = true;
			public bool ProxyMode;
			public string DenyAction = "Forbidden";
			public bool Anonymous = true;
			public bool Basic;
			public string Realm;
			public bool Windows;
			public List<string> Providers = new List<string> { "Negotiate", "NTLM" };
			public List<(bool Allow, string[] Users, string[] Roles, string[] Verbs)> Rules = new List<(bool, string[], string[], string[])> { (true, new[] { "*" }, Array.Empty<string>(), Array.Empty<string>()) };
		}

		static readonly ConcurrentDictionary<string, Settings> byFolder = new ConcurrentDictionary<string, Settings>(StringComparer.OrdinalIgnoreCase);

		static Settings For(string root, string path) => byFolder.GetOrAdd(root + "|" + IisConfiguration.Deepest(root, path), _ =>
		{
			var settings = new Settings();
			foreach (var section in IisConfiguration.For(root, path))
			{
				var security = IisConfiguration.Child(section.WebServer, "security");
				if (security == null) continue;
				if (IisConfiguration.Child(security, "ipSecurity") is { } ip)
				{
					settings.AllowUnlisted = Flag(ip, "allowUnlisted", settings.AllowUnlisted);
					settings.ProxyMode = Flag(ip, "enableProxyMode", settings.ProxyMode);
					settings.DenyAction = (string)ip.Attribute("denyAction") ?? settings.DenyAction;
					foreach (var item in ip.Elements())
					{
						var address = Parse((string)item.Attribute("ipAddress"));
						var mask = Parse((string)item.Attribute("subnetMask") ?? "255.255.255.255");
						switch (item.Name.LocalName)
						{
							case "clear": settings.Addresses.Clear(); break;
							case "remove" when address != null: settings.Addresses.RemoveAll(a => a.Address.Equals(address)); break;
							case "add" when address != null && mask != null:
								settings.Addresses.RemoveAll(a => a.Address.Equals(address) && a.Mask.Equals(mask));
								settings.Addresses.Add((address, mask, Flag(item, "allowed", false)));
								break;
						}
					}
				}
				if (IisConfiguration.Child(security, "access") is { } access && (string)access.Attribute("sslFlags") is { } sslFlags)
					settings.SslFlags = SslFlagsOf(sslFlags);
				var authentication = IisConfiguration.Child(security, "authentication");
				if (IisConfiguration.Child(authentication, "anonymousAuthentication") is { } anonymous) settings.Anonymous = Flag(anonymous, "enabled", settings.Anonymous);
				if (IisConfiguration.Child(authentication, "basicAuthentication") is { } basic)
				{
					settings.Basic = Flag(basic, "enabled", settings.Basic);
					settings.Realm = (string)basic.Attribute("realm") ?? settings.Realm;
				}
				if (IisConfiguration.Child(authentication, "windowsAuthentication") is { } windows)
				{
					settings.Windows = Flag(windows, "enabled", settings.Windows);
					foreach (var item in IisConfiguration.Child(windows, "providers")?.Elements() ?? Enumerable.Empty<XElement>())
					{
						var value = ((string)item.Attribute("value"))?.Trim();
						switch (item.Name.LocalName)
						{
							case "clear": settings.Providers.Clear(); break;
							case "remove" when value != null: settings.Providers.RemoveAll(p => p.Equals(value, StringComparison.OrdinalIgnoreCase)); break;
							case "add" when !string.IsNullOrEmpty(value):
								if (!settings.Providers.Contains(value, StringComparer.OrdinalIgnoreCase)) settings.Providers.Add(value);
								break;
						}
					}
				}
				if (IisConfiguration.Child(security, "authorization") is { } authorization)
					foreach (var item in authorization.Elements())
					{
						string[] List(string name) => ((string)item.Attribute(name) ?? "").Split(',').Select(v => v.Trim()).Where(v => v.Length > 0).ToArray();
						var users = List("users");
						var roles = List("roles");
						var verbs = List("verbs");
						bool Same((bool Allow, string[] Users, string[] Roles, string[] Verbs) rule) =>
							rule.Users.SequenceEqual(users, StringComparer.OrdinalIgnoreCase) && rule.Roles.SequenceEqual(roles, StringComparer.OrdinalIgnoreCase) && rule.Verbs.SequenceEqual(verbs, StringComparer.OrdinalIgnoreCase);
						switch (item.Name.LocalName)
						{
							case "clear": settings.Rules.Clear(); break;
							case "remove": settings.Rules.RemoveAll(Same); break;
							case "add":
								settings.Rules.RemoveAll(Same);
								settings.Rules.Add((!string.Equals((string)item.Attribute("accessType"), "Deny", StringComparison.OrdinalIgnoreCase), users, roles, verbs));
								break;
						}
					}
			}
			return settings;
		});

		static int SslFlagsOf(string value)
		{
			var flags = 0;
			foreach (var name in value.Split(','))
				switch (name.Trim().ToLowerInvariant())
				{
					case "ssl": flags |= Ssl; break;
					case "sslnegotiatecert": flags |= SslNegotiateCert; break;
					case "sslrequirecert": flags |= SslRequireCert; break;
					case "ssl128": flags |= Ssl128; break;
				}
			return flags;
		}

		/// <summary>
		/// TLS as security/access sslFlags asks for it (IIS 10's answers: iis-settings.json): without TLS, Ssl is 403.4, Ssl128
		/// 403.5, SslRequireCert 403.7; with TLS, SslRequireCert without a client certificate 403.7, a certificate (asked for
		/// with SslNegotiateCert or SslRequireCert) that is not trusted 403.16, out of its dates 403.17, revoked 403.13.
		/// </summary>
		static int SslRefusal(Core.HttpContext context, int flags)
		{
			if (flags == 0) return 0;
			if (!context.Request.IsHttps)
				return (flags & Ssl) != 0 ? 4 : (flags & Ssl128) != 0 ? 5 : (flags & SslRequireCert) != 0 ? 7 : 0;
			var certificate = context.Connection.ClientCertificate;
			if ((flags & (SslNegotiateCert | SslRequireCert)) != 0 && certificate != null)
			{
				var problem = CertificateProblem(certificate);
				if (problem != 0) return problem;
			}
			return (flags & SslRequireCert) != 0 && certificate == null ? 7 : 0;
		}

		static readonly ConcurrentDictionary<string, int> checkedCertificates = new ConcurrentDictionary<string, int>(StringComparer.Ordinal);

		/// <summary>
		/// What IIS finds wrong with a client's certificate (0: nothing): its chain to a trusted root (16), its dates (17), its
		/// revocation (13; one that cannot be checked is let in).
		/// </summary>
		internal static int CertificateProblem(X509Certificate2 certificate) => checkedCertificates.GetOrAdd(certificate.Thumbprint, _ =>
		{
			using var chain = new X509Chain();
			chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
			chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
			chain.ChainPolicy.UrlRetrievalTimeout = TimeSpan.FromSeconds(15);
			if (TrustedClientCertificates != null)
			{
				chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
				chain.ChainPolicy.CustomTrustStore.AddRange(TrustedClientCertificates);
			}
			chain.Build(certificate);
			var status = chain.ChainStatus.Aggregate(X509ChainStatusFlags.NoError, (all, s) => all | s.Status);
			if ((status & X509ChainStatusFlags.Revoked) != 0) return 13;
			if ((status & X509ChainStatusFlags.NotTimeValid) != 0) return 17;
			status &= ~(X509ChainStatusFlags.RevocationStatusUnknown | X509ChainStatusFlags.OfflineRevocation);
			return status == X509ChainStatusFlags.NoError ? 0 : 16;
		});

		static bool Flag(XElement element, string name, bool otherwise) =>
			(string)element.Attribute(name) is { } value ? value.Trim().Equals("true", StringComparison.OrdinalIgnoreCase) : otherwise;

		static IPAddress Parse(string value) => IPAddress.TryParse((value ?? "").Trim(), out var address) ? Normal(address) : null;

		static IPAddress Normal(IPAddress address) => address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;

		/// <summary>True when the URL's folder's sslFlags refuse the request (its 403 set): IIS's first check of a request.</summary>
		internal static bool RefusesTls(Core.HttpContext context, string root)
		{
			if (root == null) return false;
			var path = (context.Request.PathBase.Value ?? "") + (context.Request.Path.Value ?? "");
			var ssl = SslRefusal(context, For(root, path.Length == 0 ? "/" : path).SslFlags);
			if (ssl == 0) return false;
			IisDecision.Set(context, "SslAccess");
			return Refuse(context, 403, ssl);
		}

		/// <summary>True when IIS refuses the request here (its status set, or the connection closed).</summary>
		internal static bool Refuses(Core.HttpContext context, string root)
		{
			if (root == null) return false;
			var path = (context.Request.PathBase.Value ?? "") + (context.Request.Path.Value ?? "");
			if (path.Length == 0) path = "/";
			var settings = For(root, path);

			// TLS (sslFlags), for the URL as rewritten (RefusesTls: as sent)
			var ssl = SslRefusal(context, settings.SslFlags);
			if (ssl != 0)
			{
				IisDecision.Set(context, "SslAccess");
				return Refuse(context, 403, ssl);
			}

			// IP restrictions
			var client = context.Connection.RemoteIpAddress;
			if (settings.ProxyMode && context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded) &&
				IPAddress.TryParse(forwarded.ToString().Split(',')[0].Trim(), out var forwardedAddress)) client = forwardedAddress;
			if (client != null && (settings.Addresses.Count > 0 || !settings.AllowUnlisted))
			{
				client = Normal(client);
				// (IIS 10: an entry of a loopback address, 127.0.0.2, 127.1.0.0, is the loopback's, 127.0.0.1 too; 0.0.0.0 every address)
				var match = settings.Addresses.Where(a => a.Address.Equals(IPAddress.Any) || IPAddress.IsLoopback(a.Address) && IPAddress.IsLoopback(client) ||
						a.Address.AddressFamily == client.AddressFamily && Masked(client, a.Mask).Equals(Masked(a.Address, a.Mask)))
					.OrderByDescending(a => Bits(a.Mask)).Select(a => (bool?)a.Allowed).FirstOrDefault();
				if (!(match ?? settings.AllowUnlisted))
				{
					IisDecision.Set(context, "IpRestrictionModule");
					switch (settings.DenyAction.Trim().ToLowerInvariant())
					{
						case "abortrequest": context.Abort(); return true;
						case "unauthorized": return Refuse(context, 401, 503);
						case "notfound": return Refuse(context, 404, 503);
						default: return Refuse(context, 403, 503);
					}
				}
			}

			// Authentication
			var credentials = context.Request.Headers.ContainsKey("Authorization");
			if (!settings.Anonymous || credentials && (settings.Basic || settings.Windows))
			{
				IisDecision.Set(context, settings.Basic ? "BasicAuthenticationModule" : settings.Windows ? "WindowsAuthenticationModule" : "AnonymousAuthenticationModule");
				if (settings.Basic) Core.HeaderDictionaryExtensions.Append(context.Response.Headers, "WWW-Authenticate", "Basic realm=\"" + (settings.Realm ?? (string.IsNullOrEmpty(context.Request.Host.Host) ? context.Connection.LocalIpAddress?.ToString() : context.Request.Host.Host)) + "\"");
				if (settings.Windows) foreach (var provider in settings.Providers) Core.HeaderDictionaryExtensions.Append(context.Response.Headers, "WWW-Authenticate", provider.Split(':')[0]);
				return Refuse(context, 401, credentials ? 1 : 2);
			}

			// URL authorization
			var named = settings.Rules.Any(r => r.Roles.Length > 0 || r.Users.Any(u => u != "*" && u != "?"));
			var ownUser = credentials || context.Request.Headers.ContainsKey("Cookie");
			if (named && ownUser) return false;
			var method = context.Request.Method;
			bool Applies((bool Allow, string[] Users, string[] Roles, string[] Verbs) rule) =>
				rule.Users.Any(u => u == "*" || u == "?") && (rule.Verbs.Length == 0 || rule.Verbs.Contains(method, StringComparer.OrdinalIgnoreCase));
			if (settings.Rules.Any(r => !r.Allow && Applies(r)) || !settings.Rules.Any(r => r.Allow && Applies(r)))
			{
				IisDecision.Set(context, "UrlAuthorizationModule");
				return Refuse(context, 401, 2);
			}
			return false;
		}

		static bool Refuse(Core.HttpContext context, int status, int subStatus)
		{
			IisLogStatus.Set(context, subStatus);
			context.Response.StatusCode = status;
			return true;
		}

		static IPAddress Masked(IPAddress address, IPAddress mask)
		{
			var bytes = address.GetAddressBytes();
			var maskBytes = mask.GetAddressBytes();
			if (maskBytes.Length != bytes.Length) return address;
			for (var i = 0; i < bytes.Length; i++) bytes[i] &= maskBytes[i];
			return new IPAddress(bytes);
		}

		static int Bits(IPAddress mask) => mask.GetAddressBytes().Sum(b => Convert.ToString(b, 2).Count(c => c == '1'));
	}
}

#endif
