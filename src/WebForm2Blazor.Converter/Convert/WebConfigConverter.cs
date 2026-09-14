using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>Converts Web.config into appsettings.json.</summary>
public static class WebConfigConverter
{
    /// <summary>Config sections outside the automatic conversion whose absence could change behavior.</summary>
    private static readonly Dictionary<string, string> NoteworthySections = new(StringComparer.OrdinalIgnoreCase)
    {
        ["authentication"] = "Forms 認証 → ASP.NET Core の Cookie 認証への移行が必要です。",
        ["authorization"] = "認可設定 → ASP.NET Core の認可ポリシーへの移行が必要です。",
        ["sessionState"] = "セッション設定 → Blazor Server ではサーキット単位の Session 互換に置き換わります。",
        ["httpModules"] = "HttpModule → ミドルウェアへの書き換えが必要です。",
        ["httpHandlers"] = "HttpHandler → エンドポイントへの書き換えが必要です。",
        ["customErrors"] = "カスタムエラーページ → UseExceptionHandler への書き換えが必要です。",
        ["globalization"] = "グローバリゼーション設定 → RequestLocalization への書き換えが必要です。",
        ["identity"] = "偽装設定は ASP.NET Core では既定で無効です。",
    };

    private static readonly HashSet<string> EncodingAttributes = new(StringComparer.OrdinalIgnoreCase)
    {
        "fileEncoding", "requestEncoding", "responseEncoding",
    };

    /// <summary>
    /// Writes &lt;globalization culture uiCulture&gt; into the WebFormsGlobalization section that
    /// UseWebFormsGlobalization reads at startup. Returns false when the element names
    /// neither, so the caller still reports it (enableClientBasedCulture and friends are
    /// not carried).
    /// </summary>
    private static bool CarryGlobalization(
        XElement element, JsonObject settings, string sourceName, ConversionReport report)
    {
        var culture = element.Attribute("culture")?.Value;
        var uiCulture = element.Attribute("uiCulture")?.Value;
        if (string.IsNullOrWhiteSpace(culture) && string.IsNullOrWhiteSpace(uiCulture))
        {
            return false;
        }

        // A <location> or a nested Web.config can name the culture twice; the first one
        // wins, matching the way the outermost configuration file is the one applied to
        // the application root.
        if (settings.ContainsKey("WebFormsGlobalization"))
        {
            return true;
        }

        var globalization = new JsonObject();
        if (!string.IsNullOrWhiteSpace(culture))
        {
            globalization["Culture"] = culture;
        }
        if (!string.IsNullOrWhiteSpace(uiCulture))
        {
            globalization["UICulture"] = uiCulture;
        }
        settings["WebFormsGlobalization"] = globalization;

        report.Info(sourceName,
            $"<globalization> のカルチャ(culture={culture ?? "未指定"}, uiCulture={uiCulture ?? "未指定"})を"
            + "引き継ぎました(日付・数値の書式が元アプリと一致します)。");
        return true;
    }

    /// <summary>
    /// Whether the Web.config configures a custom error page at all.
    ///
    /// Asked BEFORE scaffolding, because the answer decides whether the generated
    /// Routes.razor wraps the page in a WebFormsErrorBoundary. Wrapping unconditionally
    /// was wrong: with nothing configured the boundary has to re-throw, and re-throwing
    /// from OnErrorAsync tears the circuit down differently from letting the exception
    /// through - WingtipToys, which ships &lt;customErrors mode="Off"&gt;, lost the page title
    /// on its failing route. A component that must not change anything is better not
    /// emitted.
    /// </summary>
    public static bool HasCustomErrorPage(string? webConfigPath)
    {
        if (string.IsNullOrEmpty(webConfigPath) || !File.Exists(webConfigPath))
        {
            return false;
        }

        try
        {
            return XDocument.Load(webConfigPath).Root?.Element("system.web")?.Elements()
                .Where(element => element.Name.LocalName.Equals("customErrors", StringComparison.OrdinalIgnoreCase))
                .Any(QualifiesAsCustomErrorPage) == true;
        }
        catch (System.Xml.XmlException)
        {
            return false;
        }
    }

    /// <summary>
    /// The one rule both the scaffolder and the settings writer use: a custom error page
    /// exists when the mode is not Off and a defaultRedirect names a page.
    /// </summary>
    private static bool QualifiesAsCustomErrorPage(XElement element)
        => element.Attribute("mode")?.Value is not { } mode
           || !mode.Equals("Off", StringComparison.OrdinalIgnoreCase)
               ? !string.IsNullOrWhiteSpace(element.Attribute("defaultRedirect")?.Value)
               : false;

    /// <summary>
    /// Writes &lt;customErrors mode defaultRedirect&gt; into the WebFormsCustomErrors section
    /// that WebFormsErrorBoundary reads. Returns false when the element names no page to
    /// go to, so the caller still reports it - a mode with no defaultRedirect and no
    /// &lt;error&gt; entries has nothing to carry.
    /// </summary>
    private static bool CarryCustomErrors(
        XElement element, JsonObject settings, string sourceName, ConversionReport report)
    {
        var mode = element.Attribute("mode")?.Value;
        var defaultRedirect = element.Attribute("defaultRedirect")?.Value;

        // mode="Off" means the application deliberately has NO custom error page - the
        // real error is shown. There is nothing to migrate, so reporting it as "not
        // converted, rewrite to UseExceptionHandler" was asking for work that would change
        // the behaviour rather than preserve it. WingtipToys ships exactly this.
        if (mode is not null && mode.Equals("Off", StringComparison.OrdinalIgnoreCase))
        {
            report.Info(sourceName,
                "<customErrors mode=\"Off\"> のためカスタムエラーページはありません(移行不要。例外はそのまま表示されます)。");
            return true;
        }

        if (string.IsNullOrWhiteSpace(defaultRedirect))
        {
            return false;
        }

        // A <location> or a nested Web.config can set it twice; the outermost file is the
        // one applied to the application root, and it is read first.
        if (settings.ContainsKey("WebFormsCustomErrors"))
        {
            return true;
        }

        var customErrors = new JsonObject
        {
            // WebForms' own default when the attribute is absent.
            ["Mode"] = string.IsNullOrWhiteSpace(mode) ? "RemoteOnly" : mode,
            ["DefaultRedirect"] = defaultRedirect,
        };

        var statusPages = new JsonObject();
        foreach (var error in element.Elements().Where(child =>
                     child.Name.LocalName.Equals("error", StringComparison.OrdinalIgnoreCase)))
        {
            var statusCode = error.Attribute("statusCode")?.Value;
            var redirect = error.Attribute("redirect")?.Value;
            if (!string.IsNullOrWhiteSpace(statusCode) && !string.IsNullOrWhiteSpace(redirect))
            {
                statusPages[statusCode] = redirect;
            }
        }
        if (statusPages.Count > 0)
        {
            customErrors["StatusPages"] = statusPages;
        }

        settings["WebFormsCustomErrors"] = customErrors;

        report.Info(sourceName,
            $"<customErrors mode=\"{customErrors["Mode"]}\" defaultRedirect=\"{defaultRedirect}\"> を"
            + "引き継ぎました(未処理例外時に元アプリと同じページへ遷移します)。");
        return true;
    }

    private static bool IsUtf8OnlyGlobalization(XElement element)
        => element.Attributes().All(attribute =>
            EncodingAttributes.Contains(attribute.Name.LocalName)
            && attribute.Value.Replace("-", "").Equals("utf8", StringComparison.OrdinalIgnoreCase));

    public static string Convert(string webConfigPath, string sourceName, ConversionReport report)
    {
        var document = XDocument.Load(webConfigPath);
        var root = document.Root;

        var settings = new JsonObject
        {
            ["Logging"] = new JsonObject
            {
                ["LogLevel"] = new JsonObject
                {
                    ["Default"] = "Information",
                    ["Microsoft.AspNetCore"] = "Warning",
                },
            },
            ["AllowedHosts"] = "*",
        };

        var appSettings = new JsonObject();
        foreach (var entry in root?.Element("appSettings")?.Elements("add") ?? [])
        {
            var key = entry.Attribute("key")?.Value;
            if (string.IsNullOrEmpty(key))
            {
                continue;
            }

            // Settings keys specific to the WebForms runtime are not carried into Blazor
            // (ValidationSettings: = unobtrusive validation, aspnet: = runtime compat switches)
            if (key.StartsWith("ValidationSettings:", StringComparison.OrdinalIgnoreCase)
                || key.StartsWith("aspnet:", StringComparison.OrdinalIgnoreCase)
                || key.StartsWith("vs:", StringComparison.OrdinalIgnoreCase))
            {
                report.Info(sourceName, $"appSettings キー '{key}' は WebForms ランタイム専用のため移行しません。");
                continue;
            }

            appSettings[key] = entry.Attribute("value")?.Value ?? string.Empty;
        }
        if (appSettings.Count > 0)
        {
            settings["AppSettings"] = appSettings;
            report.Info(sourceName, $"appSettings {appSettings.Count} 件を appsettings.json の AppSettings セクションへ変換しました。");
        }

        var connectionStrings = new JsonObject();
        foreach (var entry in root?.Element("connectionStrings")?.Elements("add") ?? [])
        {
            var name = entry.Attribute("name")?.Value;
            if (!string.IsNullOrEmpty(name))
            {
                connectionStrings[name] = entry.Attribute("connectionString")?.Value ?? string.Empty;
            }
        }
        if (connectionStrings.Count > 0)
        {
            settings["ConnectionStrings"] = connectionStrings;
            report.Info(sourceName, $"connectionStrings {connectionStrings.Count} 件を appsettings.json へ変換しました。");
        }

        // The role provider is the application's OWN class and its data is ported with the
        // rest of App_Data, so the store survives the conversion - only the wiring is
        // Framework-specific. Carrying the provider's type across lets the compatibility
        // Roles facade delegate to it instead of answering "no roles", which is the honest
        // answer only while there is genuinely no store.
        var roleManager = root?.Element("system.web")?.Element("roleManager");
        var roleProvider = roleManager?.Element("providers")?.Elements("add").FirstOrDefault(add =>
            roleManager.Attribute("defaultProvider") is null
            || add.Attribute("name")?.Value == roleManager.Attribute("defaultProvider")!.Value);
        if (roleProvider is not null
            && roleProvider.Attribute("type")?.Value is { Length: > 0 } roleProviderType)
        {
            // "name", "type" and "description" are the provider MODEL's own attributes -
            // System.Configuration consumes them before the provider ever sees the
            // collection. A provider's Initialize typically removes the keys it knows and
            // throws on whatever is left, so passing these through makes a correctly
            // written provider reject its own configuration: BlogEngine answers
            // "Unrecognized attribute: description".
            var parameters = new JsonObject();
            foreach (var attribute in roleProvider.Attributes()
                         .Where(a => a.Name.LocalName is not ("name" or "type" or "description")))
            {
                parameters[attribute.Name.LocalName] = attribute.Value;
            }

            settings["WebFormsRoleProvider"] = new JsonObject
            {
                ["Name"] = roleProvider.Attribute("name")?.Value ?? "RoleProvider",
                // Assembly qualifier dropped: the converted application is one assembly and
                // the compatibility layer searches loaded assemblies by full name.
                ["Type"] = roleProviderType.Split(',')[0].Trim(),
                ["Parameters"] = parameters,
            };
            report.Info(sourceName,
                $"<roleManager> のロールプロバイダ {roleProvider.Attribute("name")?.Value} を引き継ぎました"
                + "(ロールデータは移植済みの実装がそのまま読みます)。");
        }

        var systemWeb = root?.Element("system.web");
        if (systemWeb is not null)
        {
            foreach (var element in systemWeb.Elements())
            {
                if (!NoteworthySections.TryGetValue(element.Name.LocalName, out var note))
                {
                    continue;
                }

                // A <globalization> that only specifies encodings (fileEncoding etc.) and
                // is entirely UTF-8 needs no migration - ASP.NET Core is always UTF-8.
                if (element.Name.LocalName.Equals("globalization", StringComparison.OrdinalIgnoreCase)
                    && IsUtf8OnlyGlobalization(element))
                {
                    report.Info(sourceName,
                        "<globalization> は UTF-8 のエンコーディング指定のみのため移行不要です(ASP.NET Core は常に UTF-8)。");
                    continue;
                }

                // The culture decides how every date and every number in the application
                // prints, so it is carried over rather than reported: without it the
                // converted pages render whatever culture the SERVER happens to have.
                if (element.Name.LocalName.Equals("globalization", StringComparison.OrdinalIgnoreCase)
                    && CarryGlobalization(element, settings, sourceName, report))
                {
                    continue;
                }

                // The custom error page decides what a visitor sees when a page throws.
                // Without it the converted application shows Blazor's "An unhandled error
                // has occurred" bar where the original showed the application's own page.
                if (element.Name.LocalName.Equals("customErrors", StringComparison.OrdinalIgnoreCase)
                    && CarryCustomErrors(element, settings, sourceName, report))
                {
                    continue;
                }

                report.Residual(sourceName, ResidualKind.Configuration,
                    $"<{element.Name.LocalName}> は自動変換していません。{note}",
                    disposition: ResidualDisposition.Backlog);
            }
        }

        return settings.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });
    }

    /// <summary>
    /// The &lt;configSections&gt; declarations and the sections they declare, as an App.config.
    /// Null when the Web.config declares no custom sections.
    ///
    /// These cannot travel in appsettings.json: a custom section is materialised by the
    /// application's OWN ConfigurationSection subclass, which is ported like any other
    /// class, and System.Configuration binds one only from a .config file. Once the XML is
    /// there the section resolves exactly as it did on 4.8.
    /// </summary>
    public static string? ExtractCustomSections(string webConfigPath, string assemblyName, ConversionReport report)
    {
        XDocument document;
        try
        {
            document = XDocument.Load(webConfigPath);
        }
        catch (System.Xml.XmlException)
        {
            return null;
        }

        var configSections = document.Root?.Element("configSections");
        if (configSections is null || document.Root is null)
        {
            return null;
        }

        // Sections are declared as <section name="x"> at the top level, or nested one level
        // inside <sectionGroup name="g">, which the config system then addresses as "g/x".
        var names = new List<string>();
        foreach (var declared in configSections.Descendants()
                     .Where(element => element.Name.LocalName == "section"))
        {
            var name = declared.Attribute("name")?.Value;
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }
            var group = declared.Parent?.Name.LocalName == "sectionGroup"
                ? declared.Parent.Attribute("name")?.Value
                : null;
            names.Add(string.IsNullOrEmpty(group) ? name : group + "/" + name);
        }

        if (names.Count == 0)
        {
            return null;
        }

        var carried = new List<XElement>();
        foreach (var name in names)
        {
            XElement? current = document.Root;
            foreach (var segment in name.Split('/'))
            {
                current = current?.Element(segment);
            }
            if (current is not null)
            {
                carried.Add(current);
            }
        }

        if (carried.Count == 0)
        {
            return null;
        }

        var root = new XElement("configuration", new XElement(configSections));
        // A grouped section has to keep its group element, or the name it is addressed by
        // ("BlogEngine/blogProvider") no longer resolves.
        foreach (var group in carried
                     .Select(element => element.Parent)
                     .Where(parent => parent is not null && parent != document.Root)
                     .Distinct())
        {
            root.Add(new XElement(group!));
        }
        foreach (var element in carried.Where(element => element.Parent == document.Root))
        {
            root.Add(new XElement(element));
        }

        // Every "type" in the original names an assembly that no longer exists: the
        // conversion flattens the whole application into one. The assembly is REPLACED
        // rather than dropped - System.Configuration resolves a bare name with
        // Type.GetType, which searches only its own assembly and the core library, so an
        // unqualified handler fails with "Could not resolve type" and the section comes
        // back null. Pointing it at the converted assembly is what makes it load.
        foreach (var typeAttribute in root.Descendants()
                     .Select(element => element.Attribute("type"))
                     .Where(attribute => attribute is not null))
        {
            var typeName = typeAttribute!.Value.Split(',')[0].Trim();
            typeAttribute.Value = $"{typeName}, {assemblyName}";
        }

        report.Info("(project)",
            $"カスタム構成セクション {carried.Count} 件を App.config へ引き継ぎました"
            + "(移植されたセクションハンドラがそのまま読み込みます): " + string.Join(", ", names));

        return new XDocument(new XDeclaration("1.0", "utf-8", null), root).ToString();
    }
}
