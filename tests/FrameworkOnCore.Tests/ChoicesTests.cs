using FrameworkOnCore.Analysis;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The user's choices (foc-choices.json): checked against the catalog, and the converter's rules narrowed to them
/// (Rules.Choose). No choice: the catalog's defaults, the rules as they are.
/// </summary>
public sealed class ChoicesTests
{
    static readonly Catalog catalog = Catalog.Default();

    static Choices Of(string json) => System.Text.Json.JsonSerializer.Deserialize<Choices>(json, Choices.Json)!;

    [Fact]
    public void What_the_catalog_does_not_have_is_an_error()
    {
        var choices = Of("""
            {
              "components": { "binary-formatter": "none", "no-such-part": "none", "wcf-server": "corewcf", "thread-abort": "maybe" },
              "apis": { "P:System.Text.Encoding.Default": "none", "Encoding.Default": "none" },
              "settings": { "file-name-case": "sensitive", "colour": "blue" }
            }
            """);

        var errors = choices.Validate(catalog);

        Assert.Equal(5, errors.Count);
        Assert.Contains(errors, e => e.StartsWith("component no-such-part: not in the catalog"));
        Assert.Contains(errors, e => e.StartsWith("component wcf-server: option corewcf is not there yet"));
        Assert.Contains(errors, e => e.StartsWith("component thread-abort: no option maybe (compat, none)"));
        Assert.Contains(errors, e => e.StartsWith("api Encoding.Default: not a documentation id"));
        Assert.Contains(errors, e => e.StartsWith("setting colour: not in the catalog"));
    }

    [Fact]
    public void No_choice_is_the_default_and_the_rules_are_as_they_are()
    {
        var rules = RewriteHarness.Rules.Choose(new Choices(), catalog);

        Assert.Equal(RewriteHarness.Rules.PlatformReplacements.Count, rules.PlatformReplacements.Count);
        Assert.Equal(RewriteHarness.Rules.SourcePackages.Count, rules.SourcePackages.Count);
        Assert.Equal(RewriteHarness.Rules.MemberReplacements.Count, rules.MemberReplacements.Count);
        Assert.NotEmpty(rules.NamespaceMoves);
        Assert.True(rules.IsChosen("code-pages:register"));
        Assert.True(rules.IsChosen("file-name-case:insensitive"));
        // Without Choose, the same (the catalog's defaults).
        Assert.True(RewriteHarness.Rules.IsChosen("async-delegates:compat"));
        Assert.False(RewriteHarness.Rules.IsChosen("async-delegates:none"));
    }

    [Fact]
    public void An_option_not_chosen_leaves_its_rules_out()
    {
        var rules = RewriteHarness.Rules.Choose(Of("""
            { "components": { "binary-formatter": "none", "entity-framework-4": "none", "file-access-control": "none", "encoding-default": "none" } }
            """), catalog);

        Assert.DoesNotContain(rules.SourcePackages, p => p.Package.Id == "System.Runtime.Serialization.Formatters");
        Assert.DoesNotContain(rules.SourcePackages, p => p.Package.Id == "EntityFramework");
        Assert.Empty(rules.NamespaceMoves);
        Assert.Empty(rules.TypeMoves);
        Assert.DoesNotContain(rules.MemberReplacements, r => r.Member == "GetAccessControl");
        Assert.DoesNotContain(rules.PlatformReplacements, r => r.Member == "System.Text.Encoding.Default");
        Assert.True(rules.IsChosen("encoding-default:none"));
        Assert.False(rules.IsChosen("encoding-default:framework"));
    }

    [Fact] // Encoding.Default left as .NET has it, the rest of the rules as they are
    public void An_api_chosen_on_its_own_is_rewritten_so_and_the_others_by_their_component()
    {
        var rules = RewriteHarness.Rules.Choose(Of("""
            { "apis": { "P:System.Text.Encoding.Default": "none" }, "components": { "thread-abort": "none", "async-delegates": "none" } }
            """), catalog);

        var written = RewriteHarness.CSharp("""
            using System;
            using System.Threading;
            class C
            {
                delegate int Work(int x);
                byte[] F(string s) => System.Text.Encoding.Default.GetBytes(s);
                void G() { try { } catch (ThreadAbortException) { Thread.ResetAbort(); } }
                int H(Work w) => w.EndInvoke(w.BeginInvoke(1, null, null));
                string I() => AppDomain.CurrentDomain.RelativeSearchPath;
            }
            """, rules);

        Assert.Contains("System.Text.Encoding.Default.GetBytes(s)", written);
        Assert.Contains("Thread.ResetAbort();", written);
        Assert.Contains("w.EndInvoke(w.BeginInvoke(1, null, null))", written);
        Assert.Contains("global::FrameworkOnCore.Platform.RelativeSearchPath", written);
    }

    [Fact] // the LINQ to SQL port is a framework reference by choice: not chosen, the assembly has no answer
    public void A_framework_reference_by_choice_moves_to_no_answer_when_not_chosen()
    {
        Assert.True(RewriteHarness.Rules.FrameworkReferences.ContainsKey("System.Data.Linq"));

        var port = RewriteHarness.Rules.Choose(new Choices(), catalog);
        Assert.Equal("FrameworkOnCore.Data.Linq", port.FrameworkReferences["System.Data.Linq"].Id);
        Assert.DoesNotContain("System.Data.Linq", port.NoAnswer);

        var none = RewriteHarness.Rules.Choose(Of("""{ "components": { "linq-to-sql": "none" } }"""), catalog);
        Assert.False(none.FrameworkReferences.ContainsKey("System.Data.Linq"));
        Assert.Contains("System.Data.Linq", none.NoAnswer);
    }

    [Fact] // what analyze writes: the components to decide on at their defaults, the settings
    public void The_defaults_of_an_analysis_are_the_choices_to_make()
    {
        var result = new AnalysisResult
        {
            Tool = "t", Repository = "r", Entry = "e", Configuration = "Debug", Analyzed = DateTimeOffset.UnixEpoch, CatalogVersion = catalog.Version,
            Projects = [], Apis = [], Libraries = [], Binaries = [], Settings = catalog.Settings,
            Components =
            [
                new ComponentUsage("binary-formatter", "BinaryFormatter", ApiStatus.Throws, 1, 1, 1, 1, 1, 0, [], null, catalog.OptionsOf("binary-formatter")),
                new ComponentUsage("fw:System.Xml", "System.Xml", ApiStatus.Obsolete, 1, 1, 1, 1, 1, 0, [], null, catalog.OptionsOf("fw:System.Xml")),
                new ComponentUsage("web-forms", "ASP.NET", ApiStatus.Available, 1, 1, 1, 0, 0, 0, [], null, catalog.OptionsOf("web-forms")),
            ],
        };

        var defaults = Choices.Defaults(result, catalog);

        Assert.Equal(new Dictionary<string, string> { ["binary-formatter"] = "compat-package" }, defaults.Components);
        Assert.Equal("insensitive", defaults.Settings["file-name-case"]);
        Assert.Empty(defaults.Validate(catalog));
    }
}
