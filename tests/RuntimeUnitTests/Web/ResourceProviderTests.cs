using System.Globalization;
using System.Resources;
using System.Web.Compilation;

namespace FrameworkOnCore.Runtime.Tests.Web;

/// <summary>
/// An application's resources (App_GlobalResources, App_LocalResources: BaseResXResourceProvider) looked for culture by
/// culture as on .NET Framework, whose Chinese cultures have the old names as parents (its documentation, CultureInfo.Parent:
/// zh-CN, zh-SG -> zh-CHS -> zh-Hans; zh-TW, zh-HK, zh-MO -> zh-CHT -> zh-Hant), which .NET skips.
/// </summary>
public class ResourceProviderTests
{
    // Resources of some cultures by name ("" the neutral ones); a culture named "broken" fails as a missing manifest does.
    sealed class Resources : ResourceManager
    {
        readonly Dictionary<string, Dictionary<string, object>> sets;
        public Resources(Dictionary<string, Dictionary<string, object>> sets) => this.sets = sets;

        // .NET's fallback (the culture's parents), over these sets.
        public override object GetObject(string name, CultureInfo culture) => InternalGetResourceSet(culture, true, true)?.GetObject(name);

        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            for (var c = culture; ; c = c.Parent)
            {
                if (sets.TryGetValue(c.Name, out var values)) return new Set(values);
                if (sets.ContainsKey("broken") && c.Name.StartsWith("zh-CH", StringComparison.OrdinalIgnoreCase)) throw new MissingManifestResourceException();
                if (!tryParents || c.Name.Length == 0) return null;
            }
        }

        sealed class Set : ResourceSet
        {
            readonly Dictionary<string, object> values;
            public Set(Dictionary<string, object> values) => this.values = values;
            public override object GetObject(string name) => values.TryGetValue(name, out var value) ? value : null;
        }
    }

    sealed class Provider : BaseResXResourceProvider
    {
        readonly ResourceManager manager;
        public Provider(ResourceManager manager) => this.manager = manager;
        protected override ResourceManager CreateResourceManager() => manager;
    }

    static Provider Of(params (string Culture, string Value)[] sets) =>
        new(new Resources(sets.ToDictionary(s => s.Culture, s => new Dictionary<string, object> { ["key"] = s.Value })));

    static object Get(Provider provider, string culture) => provider.GetObject("key", CultureInfo.GetCultureInfo(culture));

    [Fact] // .NET Framework's parents: zh-CN and zh-SG find zh-CHS's resources, zh-TW, zh-HK and zh-MO zh-CHT's
    public void The_old_chinese_names_are_parents()
    {
        var provider = Of(("", "neutral"), ("zh-CHS", "chs"), ("zh-CHT", "cht"));
        Assert.Equal("chs", Get(provider, "zh-CN"));
        Assert.Equal("chs", Get(provider, "zh-SG"));
        Assert.Equal("cht", Get(provider, "zh-TW"));
        Assert.Equal("cht", Get(provider, "zh-HK"));
        Assert.Equal("cht", Get(provider, "zh-MO"));
    }

    [Fact] // .NET Framework's chain: the culture's own first, then zh-CHS, zh-Hans, the neutral resources; a key in none: null
    public void The_chain_is_walked_culture_by_culture()
    {
        Assert.Equal("cn", Get(Of(("zh-CN", "cn"), ("zh-CHS", "chs")), "zh-CN"));
        Assert.Equal("hans", Get(Of(("", "neutral"), ("zh-Hans", "hans")), "zh-CN"));
        Assert.Equal("hant", Get(Of(("", "neutral"), ("zh-Hant", "hant")), "zh-TW"));
        Assert.Equal("neutral", Get(Of(("", "neutral")), "zh-CN"));
        Assert.Null(Get(Of(("fr", "fr")), "zh-CN"));
        Assert.Null(Of(("", "neutral")).GetObject("other", CultureInfo.GetCultureInfo("zh-CN")));
    }

    [Fact] // the port's comment: a culture whose set fails to load (MissingManifestResourceException) is skipped
    public void A_culture_that_fails_is_skipped()
    {
        Assert.Equal("neutral", Get(Of(("", "neutral"), ("broken", "x")), "zh-CN"));
    }

    [Fact] // ResourceManager's fallback for the other cultures; no culture: the current UI culture; no resources: null
    public void Other_cultures_fall_back_as_dotnet_s()
    {
        var provider = Of(("", "neutral"), ("fr", "fr"));
        Assert.Equal("fr", Get(provider, "fr-CA"));
        Assert.Equal("neutral", Get(provider, "de-DE"));
        var current = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
            Assert.Equal("fr", provider.GetObject("key", null));
        }
        finally { CultureInfo.CurrentUICulture = current; }
        Assert.Null(new Provider(null).GetObject("key", CultureInfo.GetCultureInfo("zh-CN")));
        Assert.Null(new Provider(null).ResourceReader);
    }
}
