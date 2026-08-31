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

                report.Residual(sourceName, ResidualKind.Configuration,
                    $"<{element.Name.LocalName}> は自動変換していません。{note}",
                    disposition: ResidualDisposition.ManualMigration);
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
