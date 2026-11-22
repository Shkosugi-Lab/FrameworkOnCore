namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>The variants made from this machine's IIS schema are well-formed (each folder's web.config is XML) and named once each.</summary>
public sealed class SettingsGenerateCheck
{
    static readonly string Schema = Path.Combine(Environment.GetEnvironmentVariable("windir") ?? "", "system32", "inetsrv", "config", "schema");

    [Fact]
    public void The_variants_are_made_from_the_schema()
    {
        if (!File.Exists(Path.Combine(Schema, "IIS_schema.xml"))) return;
        var notTried = new List<SettingsNotTried>();
        var variants = SettingsSchema.Variants(Schema, notTried);
        foreach (var variant in variants) System.Xml.Linq.XDocument.Parse(variant.Config);
        Assert.Equal(variants.Count, variants.Select(v => v.Id).Distinct().Count());
        if (Environment.GetEnvironmentVariable("FOC_SETTINGS_DUMP") is { } dump)
            File.WriteAllLines(dump, variants.Select(v => v.Id + "\t" + v + "\n" + v.Config).Concat(notTried.Select(n => "NOT " + n.Section + n.Setting + ": " + n.Why)));
    }
}
