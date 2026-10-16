#if NETCOREAPP

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
	/// <summary>
	/// The requests' log IIS writes (its HTTP logging: W3C extended log files), in one of two ways (W3CLogging:Mode):
	/// <list type="bullet">
	/// <item>Iis (the default): IIS's own files (IisLogWriter): u_exYYMMDD.log, IIS's header, fields, order and escaping,
	/// sc-substatus and sc-win32-status as IIS has them, written in batches (no limit but the disk's: requests wait when the
	/// writer falls behind). W3CLogging:Fields (IIS's logExtFileFlags: "Date, Time, ClientIP, ..."; IIS 10's default),
	/// W3CLogging:Period (Daily, Hourly, Monthly), W3CLogging:LocalTimeRollover, W3CLogging:SiteName (s-sitename,
	/// W3SVC1), W3CLogging:Software (the #Software line: IIS's, for the tools that read IIS's logs).</item>
	/// <item>AspNetCore: ASP.NET Core's W3C logging, which an application rewritten for ASP.NET Core keeps: its format
	/// (below), its writer's limit.</item>
	/// </list>
	/// ASP.NET Core's W3C logging (Mode AspNetCore): IIS's
	/// default fields (date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent)
	/// cs(Referer) sc-status time-taken; no sc-substatus and sc-win32-status, which ASP.NET Core does not have), a file a
	/// day kept as IIS keeps them (no size limit, none deleted), u_ex as the files' prefix. Off where web.config's
	/// system.webServer/httpLogging has dontLog="true". Its configuration (appsettings.json, environment variables):
	/// W3CLogging:Enabled (true / false, over web.config), W3CLogging:LogDirectory (outside the application's folder: its
	/// files are watched, FileCache); by default %ProgramData%\FrameworkOnCore\LogFiles\&lt;folder&gt; on Windows,
	/// /var/log/frameworkoncore/&lt;folder&gt; elsewhere (the temporary folder's FrameworkOnCore/LogFiles/&lt;folder&gt; where it
	/// cannot be made: a container's user). W3CLogging:FlushInterval: how long its writer waits once it has written all
	/// (20 ms here; ASP.NET Core's 1 second let at most about 1,000 requests a second through). Its writer keeps up with
	/// about 10,000 requests a second (measured: the converted PipelineProbe on Windows); above, requests wait for it.
	/// </summary>
	public static class WebFormsW3CLoggingExtensions
	{
		// Registered when the log is on: UseWebFormsW3CLogging adds the middleware then.
		sealed class W3CLoggingOn
		{
			public string Directory;
			public System.Web.Hosting.IisLogWriter Iis;   // null: ASP.NET Core's W3C logging
		}

		/// <summary>The W3C log's services, as IIS logs the application's requests (unless web.config or the configuration says not).</summary>
		public static WebApplicationBuilder AddWebFormsW3CLogging(this WebApplicationBuilder builder)
		{
			var contentRoot = builder.Environment.ContentRootPath;
			if (!Enabled(contentRoot, builder.Configuration)) return builder;
			var directory = Directory(contentRoot, builder.Configuration);
			if (directory == null) return builder;
			// Should the directory be in the application's folder after all (the configuration's): its writes are not the
			// application's files' changes.
			System.Web.Hosting.FileCache.IgnoreChangesUnder(directory);
			var mode = builder.Configuration["W3CLogging:Mode"];
			if (string.IsNullOrEmpty(mode) || string.Equals(mode, "Iis", StringComparison.OrdinalIgnoreCase))
			{
				builder.Services.AddSingleton(new W3CLoggingOn { Directory = directory, Iis = new System.Web.Hosting.IisLogWriter(IisOptions(directory, builder.Configuration, System.Web.Hosting.IisProfile.Of(contentRoot))) });
				return builder;
			}
			if (!string.Equals(mode, "AspNetCore", StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException("W3CLogging:Mode: Iis or AspNetCore, not " + mode);
			builder.Services.AddSingleton(new W3CLoggingOn { Directory = directory });
			builder.Services.AddW3CLogging(options =>
			{
				options.LoggingFields = W3CLoggingFields.Date | W3CLoggingFields.Time | W3CLoggingFields.ServerIpAddress |
					W3CLoggingFields.Method | W3CLoggingFields.UriStem | W3CLoggingFields.UriQuery | W3CLoggingFields.ServerPort |
					W3CLoggingFields.UserName | W3CLoggingFields.ClientIpAddress | W3CLoggingFields.UserAgent |
					W3CLoggingFields.Referer | W3CLoggingFields.ProtocolStatus | W3CLoggingFields.TimeTaken;
				options.LogDirectory = directory;
				options.FileName = "u_ex";
				options.FileSizeLimit = null;
				options.RetainedFileCountLimit = 10000;
				// The writer, once its queue is empty, waits FlushInterval (1 second by default) while at most 1,024 lines
				// queue; more requests wait for it: about 1,000 requests a second at most, latencies up to the second.
				options.FlushInterval = FlushInterval(builder.Configuration);
			});
			return builder;
		}

		// The configuration's, else the original server's (IisProfile: the site's logFile, its id), else IIS's defaults.
		static System.Web.Hosting.IisLogOptions IisOptions(string directory, IConfiguration configuration, XElement profile)
		{
			var options = new System.Web.Hosting.IisLogOptions { Directory = directory };
			var logFile = System.Web.Hosting.IisProfile.LogFile(profile);
			string setting;
			if (logFile.TryGetValue("logExtFileFlags", out setting) && !string.IsNullOrEmpty(setting))
			{
				var flags = System.Web.Hosting.IisLogWriter.ParseFields(setting);
				if (flags != null) options.Fields = flags.Value;
				else Console.Error.WriteLine("W3C log: the site's logExtFileFlags (" + setting + ") has a field this log does not write; IIS's defaults written");
			}
			if (logFile.TryGetValue("period", out setting) && !string.IsNullOrEmpty(setting))
			{
				System.Web.Hosting.IisLogPeriod sitePeriod;
				if (Enum.TryParse(setting, true, out sitePeriod)) options.Period = sitePeriod;
				else Console.Error.WriteLine("W3C log: the site's log period " + setting + " is not written (Weekly, MaxSize); a file a day");
			}
			bool rollover;
			if (logFile.TryGetValue("localTimeRollover", out setting) && bool.TryParse(setting, out rollover)) options.LocalTimeRollover = rollover;
			if (logFile.TryGetValue("logFormat", out setting) && !string.IsNullOrEmpty(setting) && !string.Equals(setting, "W3C", StringComparison.OrdinalIgnoreCase))
				Console.Error.WriteLine("W3C log: the site's log format " + setting + " is not written; W3C");
			var siteId = System.Web.Hosting.IisProfile.SiteId(profile);
			if (!string.IsNullOrEmpty(siteId)) options.SiteName = "W3SVC" + siteId;
			var fields = configuration["W3CLogging:Fields"];
			if (!string.IsNullOrEmpty(fields))
				options.Fields = System.Web.Hosting.IisLogWriter.ParseFields(fields) ?? throw new InvalidOperationException("W3CLogging:Fields: IIS's logExtFileFlags (Date, Time, ClientIP, UserName, SiteName, ComputerName, ServerIP, Method, UriStem, UriQuery, HttpStatus, HttpSubStatus, Win32Status, BytesSent, BytesRecv, TimeTaken, ServerPort, UserAgent, Cookie, Referer, ProtocolVersion, Host), not " + fields);
			var period = configuration["W3CLogging:Period"];
			if (!string.IsNullOrEmpty(period))
			{
				System.Web.Hosting.IisLogPeriod value;
				if (!Enum.TryParse(period, true, out value)) throw new InvalidOperationException("W3CLogging:Period: Daily, Hourly or Monthly, not " + period);
				options.Period = value;
			}
			bool local;
			if (bool.TryParse(configuration["W3CLogging:LocalTimeRollover"], out local)) options.LocalTimeRollover = local;
			if (!string.IsNullOrEmpty(configuration["W3CLogging:SiteName"])) options.SiteName = configuration["W3CLogging:SiteName"];
			if (!string.IsNullOrEmpty(configuration["W3CLogging:Software"])) options.Software = configuration["W3CLogging:Software"];
			return options;
		}

		/// <summary>The W3C log's middleware (first: every request logged, the ones answered before System.Web too).</summary>
		public static IApplicationBuilder UseWebFormsW3CLogging(this IApplicationBuilder app)
		{
			var on = app.ApplicationServices.GetService(typeof(W3CLoggingOn)) as W3CLoggingOn;
			if (on == null) return app;
			// The addresses as IIS logs them: an IPv4 client of Kestrel's dual-mode socket is ::ffff:a.b.c.d (c-ip, s-ip),
			// IIS's a.b.c.d (as the host gives System.Web its REMOTE_ADDR).
			app.Use((context, next) =>
			{
				var connection = context.Connection;
				if (connection.RemoteIpAddress != null && connection.RemoteIpAddress.IsIPv4MappedToIPv6)
					connection.RemoteIpAddress = connection.RemoteIpAddress.MapToIPv4();
				if (connection.LocalIpAddress != null && connection.LocalIpAddress.IsIPv4MappedToIPv6)
					connection.LocalIpAddress = connection.LocalIpAddress.MapToIPv4();
				return next(context);
			});
			return on.Iis != null ? app.UseMiddleware<System.Web.Hosting.IisLogMiddleware>(on.Iis) : app.UseW3CLogging();
		}

		static TimeSpan FlushInterval(IConfiguration configuration)
		{
			TimeSpan configured;
			if (TimeSpan.TryParse(configuration["W3CLogging:FlushInterval"], System.Globalization.CultureInfo.InvariantCulture, out configured) && configured > TimeSpan.Zero)
				return configured;
			return TimeSpan.FromMilliseconds(20);
		}

		static bool Enabled(string contentRoot, IConfiguration configuration)
		{
			bool configured;
			if (bool.TryParse(configuration["W3CLogging:Enabled"], out configured)) return configured;
			// The original server's (IisProfile): the site's log file off; its httpLogging's dontLog, which web.config's may change.
			var profile = System.Web.Hosting.IisProfile.Of(contentRoot);
			string logFileEnabled;
			if (System.Web.Hosting.IisProfile.LogFile(profile).TryGetValue("enabled", out logFileEnabled) && string.Equals(logFileEnabled, "false", StringComparison.OrdinalIgnoreCase))
				return false;
			var dontLog = DontLog(System.Web.Hosting.IisProfile.WebServer(profile)) ?? false;
			var file = Path.Combine(contentRoot, "web.config");
			if (!File.Exists(file)) file = System.IO.Directory.EnumerateFiles(contentRoot, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();
			if (file == null) return !dontLog;
			try
			{
				var root = XDocument.Load(file).Root;
				var sections = root.Elements("system.webServer")
					.Concat(root.Elements("location")
						.Where(l => { var p = (string)l.Attribute("path"); return string.IsNullOrEmpty(p) || p == "."; })
						.Elements("system.webServer"));
				return !(DontLog(sections) ?? dontLog);
			}
			catch (Exception e) when (e is System.Xml.XmlException || e is IOException)
			{
				return !dontLog;
			}
		}

		// The last httpLogging's dontLog among the sections; null where none says.
		static bool? DontLog(System.Collections.Generic.IEnumerable<XElement> webServers)
		{
			bool? dontLog = null;
			foreach (var logging in webServers.Elements("httpLogging"))
			{
				bool value;
				if (bool.TryParse((string)logging.Attribute("dontLog"), out value)) dontLog = value;
			}
			return dontLog;
		}

		static string Directory(string contentRoot, IConfiguration configuration)
		{
			var site = Path.GetFileName(contentRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			if (string.IsNullOrEmpty(site)) site = "site";
			var configured = configuration["W3CLogging:LogDirectory"];
			var candidates = !string.IsNullOrEmpty(configured)
				? new[] { Path.GetFullPath(configured, contentRoot) }
				: OperatingSystem.IsWindows()
					? new[] { Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "FrameworkOnCore", "LogFiles", site), Path.Combine(Path.GetTempPath(), "FrameworkOnCore", "LogFiles", site) }
					: new[] { Path.Combine("/var/log/frameworkoncore", site), Path.Combine(Path.GetTempPath(), "FrameworkOnCore", "LogFiles", site) };
			foreach (var candidate in candidates)
			{
				try
				{
					System.IO.Directory.CreateDirectory(candidate);
					return candidate;
				}
				catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
				{
				}
			}
			Console.Error.WriteLine("W3C log: no folder to write it to (" + string.Join(", ", candidates) + "); not written");
			return null;
		}
	}
}

#endif
