using System.Globalization;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// One setting of IIS's configuration schema tried in a folder of its own (its web.config): a value of an attribute other than
/// its default, a value IIS's schema refuses (invalid), a required attribute left out (missing), or a change to a collection
/// (add, remove, clear, an entry the server's list has already: duplicate). Its answers are the probes' (SettingsProbes) that
/// differ from the same probes' in a folder without a web.config (the baseline).
/// </summary>
sealed class SettingsVariant
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("section")] public string Section { get; set; }
    /// <summary>The attribute's path in the section ("/requestLimits@maxUrl"), or the collection's ("/fileExtensions{clear}").</summary>
    [JsonPropertyName("setting")] public string Setting { get; set; }
    [JsonPropertyName("kind")] public string Kind { get; set; }
    [JsonPropertyName("value")] public string Value { get; set; }
    [JsonPropertyName("config")] public string Config { get; set; }
    /// <summary>The probes' answers that are not the baseline's (null: all of them are).</summary>
    [JsonPropertyName("answers")] public Dictionary<string, string> Answers { get; set; }

    [JsonIgnore] public string Folder => "/s/" + Id;
    /// <summary>An invalid value is IIS's 500.19 for any request of the folder: one probe tells it.</summary>
    [JsonIgnore] public bool OneProbe => Kind == "invalid";

    public override string ToString() => Section + Setting + " " + Kind + (Value == null ? "" : " " + Value);
}

/// <summary>A setting of the schema that is not tried, and why.</summary>
sealed record SettingsNotTried([property: JsonPropertyName("section")] string Section, [property: JsonPropertyName("setting")] string Setting,
    [property: JsonPropertyName("why")] string Why);

/// <summary>
/// The variants of IIS's configuration schema (%windir%\system32\inetsrv\config\schema: IIS_schema.xml, rewrite_schema.xml):
/// for each section of system.webServer a site's web.config may have, every attribute (its type's values other than the
/// default: a bool's other one, each value of an enum and flag, numbers at their range's bounds and beyond, time spans; the
/// samples' for texts), every collection (add, remove, clear, a duplicate entry). What the schema does not tell is in
/// SettingsSamples: a text's values, a collection's entry, the context a setting needs to show (an error page shows for a
/// remote client or errorMode Custom), why a setting is not tried.
/// </summary>
static class SettingsSchema
{
    static readonly string[] Files = { "IIS_schema.xml", "rewrite_schema.xml" };

    public static List<SettingsVariant> Variants(string schemaFolder, List<SettingsNotTried> notTried)
    {
        var variants = new List<SettingsVariant>();
        foreach (var file in Files)
        {
            var document = XDocument.Load(Path.Combine(schemaFolder, file));
            foreach (var schema in document.Descendants("sectionSchema"))
            {
                var name = (string)schema.Attribute("name");
                if (!name.StartsWith("system.webServer/", StringComparison.Ordinal)) continue;
                var section = name["system.webServer/".Length..];
                if (SettingsSamples.SectionsNotTried.TryGetValue(section, out var why))
                {
                    notTried.Add(new SettingsNotTried(section, "", why));
                    continue;
                }
                Walk(section, schema, "", new List<string>(), variants, notTried);
            }
        }
        for (var i = 0; i < variants.Count; i++)
        {
            variants[i].Id = "v" + (i + 1).ToString("D4", CultureInfo.InvariantCulture);
            variants[i].Config = variants[i].Config.Replace("{F}", variants[i].Folder);
        }
        return variants;
    }

    static void Walk(string section, XElement schema, string path, List<string> segments, List<SettingsVariant> variants, List<SettingsNotTried> notTried)
    {
        foreach (var child in schema.Elements())
            switch (child.Name.LocalName)
            {
                case "attribute":
                    Attribute(section, path, segments, child, variants, notTried);
                    break;
                case "element":
                    var element = (string)child.Attribute("name");
                    Walk(section, child, path + "/" + element, segments.Append(element).ToList(), variants, notTried);
                    break;
                case "collection":
                    var item = (string)child.Attribute("addElement") ?? "add";
                    if (!segments.Any(s => s.StartsWith('['))) Collection(section, path, segments, child, variants, notTried);
                    Walk(section, child, path + "/[" + item + "]", segments.Append("[" + item + "]").ToList(), variants, notTried);
                    break;
            }
    }

    static void Attribute(string section, string path, List<string> segments, XElement attribute, List<SettingsVariant> variants, List<SettingsNotTried> notTried)
    {
        var name = (string)attribute.Attribute("name");
        var setting = path + "@" + name;
        var key = section + setting;
        var sample = SettingsSamples.Of(key);
        if (sample?.NoEffect != null)
        {
            notTried.Add(new SettingsNotTried(section, setting, sample.NoEffect));
            return;
        }
        var type = (string)attribute.Attribute("type");
        var values = sample?.Values ?? Generated(attribute);
        if (values == null)
        {
            notTried.Add(new SettingsNotTried(section, setting, "a text: no sample value (SettingsSamples)"));
            return;
        }
        if (!Buildable(section, segments, key, notTried, setting)) return;
        foreach (var value in values)
            variants.Add(Make(section, setting, "value", value, segments, key, target => target.SetAttributeValue(name, value)));
        var invalid = type switch
        {
            "bool" => "maybe", "uint" => "-1", "int" or "int64" => "x", "enum" or "flags" => "Bogus", "timeSpan" => "x", _ => null,
        };
        if (invalid != null) variants.Add(Make(section, setting, "invalid", invalid, segments, key, target => target.SetAttributeValue(name, invalid)));
        // A required attribute of an entry left out: its default, or IIS's 500.19.
        if ((string)attribute.Attribute("required") == "true" && segments.Any(s => s.StartsWith('[')))
        {
            var missing = Make(section, setting, "missing", null, segments, key, target => target.Attribute(name)?.Remove());
            variants.Add(missing);
        }
    }

    /// <summary>The values of the attribute's type other than its default; null for a text (the samples' then).</summary>
    static string[] Generated(XElement attribute)
    {
        var type = (string)attribute.Attribute("type");
        var @default = (string)attribute.Attribute("defaultValue");
        var validation = (string)attribute.Attribute("validationType");
        var parameter = ((string)attribute.Attribute("validationParameter") ?? "").Split(',');
        bool NotDefault(string v) => @default == null || !string.Equals(v, @default.Trim(), StringComparison.OrdinalIgnoreCase);
        switch (type)
        {
            case "bool":
                return new[] { "true", "false" }.Where(NotDefault).ToArray();
            case "enum":
                return attribute.Elements("enum").Select(e => (string)e.Attribute("name")).Where(NotDefault).ToArray();
            case "flags":
                return attribute.Elements("flag").Select(e => (string)e.Attribute("name")).Where(NotDefault).ToArray();
            case "uint" or "int" or "int64":
            {
                var candidates = new List<long> { 0, 10, 100000 };
                if (validation == "integerRange" && long.TryParse(parameter[0], out var low) && long.TryParse(parameter[1], out var high))
                {
                    var exclude = parameter.Length > 2 && parameter[2] == "exclude";
                    if (!exclude) candidates.Add(low);
                    candidates = candidates.Where(v => exclude ? v < low || v > high : v >= low && v <= high).ToList();
                    if (!exclude && high < 100000) candidates.Add(high);
                }
                return candidates.Distinct().Select(v => v.ToString(CultureInfo.InvariantCulture)).Where(NotDefault).ToArray();
            }
            case "timeSpan":
            {
                var candidates = new List<TimeSpan> { TimeSpan.Zero, TimeSpan.FromSeconds(100) };
                if (validation == "timeSpanRange" && long.TryParse(parameter[0], out var low) && long.TryParse(parameter[1], out var high))
                {
                    var unit = parameter.Length > 2 && long.TryParse(parameter[2], out var u) ? u : 1;
                    candidates.Add(TimeSpan.FromSeconds(low * unit));
                    candidates = candidates.Where(v => v.TotalSeconds >= low * unit && v.TotalSeconds <= high * unit).ToList();
                }
                return candidates.Distinct().Select(v => v.ToString("c", CultureInfo.InvariantCulture)).Where(NotDefault).ToArray();
            }
            default:
                return null;
        }
    }

    static void Collection(string section, string path, List<string> segments, XElement collection, List<SettingsVariant> variants, List<SettingsNotTried> notTried)
    {
        var item = (string)collection.Attribute("addElement") ?? "add";
        var setting = path + "/[" + item + "]";
        var key = section + setting;
        var sample = SettingsSamples.Of(key);
        if (sample?.NoEffect != null) { notTried.Add(new SettingsNotTried(section, setting, sample.NoEffect)); return; }
        if (sample?.Item == null) { notTried.Add(new SettingsNotTried(section, setting, "a collection: no sample entry (SettingsSamples)")); return; }
        var clear = (string)collection.Attribute("clearElement");
        if (clear != null)
            variants.Add(Make(section, path + "{" + clear + "}", "clear", null, segments, key, target => target.Add(new XElement(clear)), collectionKey: key));
        foreach (var remove in sample.Remove ?? Array.Empty<string>())
            variants.Add(Make(section, path + "{remove}", "remove", remove, segments, key, target => target.Add(Parse(remove)), collectionKey: key));
        variants.Add(Make(section, path + "{" + item + "}", "add", sample.Item, segments, key,
            target => target.Add(Parse(sample.Prefix).Concat(Parse(sample.Item))), collectionKey: key));
        if (sample.Duplicate != null)
            variants.Add(Make(section, path + "{" + item + "}", "duplicate", sample.Duplicate, segments, key, target => target.Add(Parse(sample.Duplicate)), collectionKey: key));
    }

    // Whether the entries the attribute's path goes through have their samples.
    static bool Buildable(string section, List<string> segments, string key, List<SettingsNotTried> notTried, string setting)
    {
        var itemKey = section;
        foreach (var segment in segments)
        {
            itemKey += "/" + segment;
            if (segment.StartsWith('[') && SettingsSamples.Of(itemKey)?.Item == null)
            {
                notTried.Add(new SettingsNotTried(section, setting, "an entry of " + itemKey + ": no sample entry (SettingsSamples)"));
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// The folder's web.config: the section (its context from the samples), the entries down to the attribute's (the samples'),
    /// what the variant does to the element it is about (an attribute set or taken out; a collection's add, remove, clear).
    /// </summary>
    static SettingsVariant Make(string section, string setting, string kind, string value, List<string> segments, string key,
        Action<XElement> change, string collectionKey = null)
    {
        var webServer = new XElement("system.webServer");
        var sectionElement = webServer;
        foreach (var part in section.Split('/'))
        {
            var next = new XElement(part);
            sectionElement.Add(next);
            sectionElement = next;
        }
        var attributeSample = collectionKey == null ? SettingsSamples.Of(key) : null;
        // The context: the section's, the outermost collection's, the attribute's.
        var outermost = section;
        string outermostKey = collectionKey;
        foreach (var segment in segments)
        {
            outermost += "/" + segment;
            if (segment.StartsWith('[')) { outermostKey ??= outermost; break; }
        }
        Context(webServer, sectionElement, SettingsSamples.Of(section));
        if (outermostKey != null) Context(webServer, sectionElement, SettingsSamples.Of(outermostKey));
        Context(webServer, sectionElement, attributeSample);

        var deepest = segments.FindLastIndex(s => s.StartsWith('['));
        var current = sectionElement;
        var itemKey = section;
        var inEntry = false;
        for (var i = 0; i < segments.Count; i++)
        {
            var segment = segments[i];
            itemKey += "/" + segment;
            if (!segment.StartsWith('['))
            {
                var element = current.Element(segment);
                if (element == null) current.Add(element = new XElement(segment));
                current = element;
                continue;
            }
            var name = segment[1..^1];
            var sample = i == deepest && attributeSample?.Item != null ? attributeSample : SettingsSamples.Of(itemKey);
            if (!inEntry)
            {
                current.Add(Parse(sample.Prefix));
                var entry = Parse(sample.Item).Single();
                current.Add(entry);
                current = entry;
                inEntry = true;
            }
            else
            {
                var existing = current.Elements(name).FirstOrDefault();
                if (existing != null && sample == attributeSample) { current.Elements(name).Remove(); existing = null; }
                if (existing == null) current.Add(existing = Parse(sample.Item).Single());
                current = existing;
            }
        }
        change(current);
        var configuration = new XElement("configuration", webServer);
        foreach (var web in Parse(attributeSample?.SystemWeb ?? SettingsSamples.Of(section)?.SystemWeb)) configuration.AddFirst(web);
        return new SettingsVariant
        {
            Section = section, Setting = setting, Kind = kind, Value = value,
            Config = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + configuration.ToString() + "\n",
        };
    }

    static void Context(XElement webServer, XElement sectionElement, SettingsSamples.Sample sample)
    {
        if (sample == null) return;
        if (sample.WithAttributes != null)
            foreach (var attribute in XElement.Parse("<x " + sample.WithAttributes + " />").Attributes()) sectionElement.SetAttributeValue(attribute.Name, attribute.Value);
        Merge(sectionElement, Parse(sample.With));
        Merge(webServer, Parse(sample.Beside));
    }

    // Elements put into the target: one with children and no attributes into the same-named element there (the sections'
    // groups: security, authentication, rewrite), the others added.
    static void Merge(XElement target, IEnumerable<XElement> elements)
    {
        foreach (var element in elements)
        {
            var same = !element.HasAttributes && element.HasElements ? target.Element(element.Name) : null;
            if (same != null) Merge(same, element.Elements().ToList());
            else target.Add(new XElement(element));
        }
    }

    static List<XElement> Parse(string xml) => xml == null ? new List<XElement>() : XElement.Parse("<x>" + xml + "</x>").Elements().Select(e => new XElement(e)).ToList();
}
