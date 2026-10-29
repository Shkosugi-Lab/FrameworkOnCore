using System.Web.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// IIS's URL Rewrite module as the host runs it (IisUrlRewrite), against Microsoft's "URL Rewrite Module Configuration
/// Reference" (learn.microsoft.com/iis/extensions/url-rewrite-module/url-rewrite-module-configuration-reference, "the
/// reference" below) and "URL Rewrite Module 2.0 Configuration Reference" (.../url-rewrite-module-20-configuration-reference,
/// "the 2.0 reference").
/// </summary>
public class IisUrlRewriteTests
{
    static string Rules(string rules) => Site.WebServer("<rewrite><rules>" + rules + "</rules></rewrite>");

    static string Rule(string match, string action, string more = "", string attributes = "") =>
        "<rule name='r' " + attributes + "><match url='" + match + "' />" + more + "<action " + action + " /></rule>";

    /// <summary>Runs the site's rules on a request: whether they answered it, and the request.</summary>
    static async Task<(bool Answered, DefaultHttpContext Context)> Run(Site site, string target, Action<DefaultHttpContext> setup = null,
        string method = "GET", string host = "localhost:8080", bool https = false, string pathBase = "")
    {
        var context = Site.Request(target, method, host, https, pathBase);
        setup?.Invoke(context);
        var answered = await IisUrlRewrite.Load(site.Root).RunAsync(context);
        return (answered, context);
    }

    static async Task<string> PathAfter(Site site, string target)
    {
        var (answered, context) = await Run(site, target);
        Assert.False(answered);
        return context.Request.Path.Value + context.Request.QueryString.Value;
    }

    /// <summary>What an expression gives in a rule's url: a rewrite to /v?expression, without the request's query string.</summary>
    static async Task<string> Value(string expression, string target = "/content/default.aspx?tabid=2&subtabid=3",
        string host = "www.mysite.com", bool https = false, Action<DefaultHttpContext> setup = null, string profile = null)
    {
        using var site = new Site(Rules(Rule(".*", "type='Rewrite' url='/v?" + expression + "' appendQueryString='false'")), profile);
        var (_, context) = await Run(site, target, setup, host: host, https: https);
        return context.Request.QueryString.HasValue ? context.Request.QueryString.Value.Substring(1) : "";
    }

    static IisLogStatus Log(HttpContext context) => context.Features.Get<IisLogStatus>();

    // --- Loading

    [Fact] // no rules: nothing to run
    public void Without_rules_there_is_no_rewrite()
    {
        using var site = new Site(Site.WebServer("<rewrite><rules /></rewrite>"));
        Assert.Null(IisUrlRewrite.Load(site.Root));
    }

    [Fact] // the host's choice: without any web.config nor profile, no rules to run
    public void Without_configuration_there_is_no_rewrite()
    {
        using var site = new Site();
        Assert.Null(IisUrlRewrite.Load(site.Root));
    }

    [Fact] // the host's choice (summary): outboundRules and providers are not done (told on the console); the inbound rules still run
    public async Task Outbound_rules_and_providers_do_not_stop_the_inbound_rules()
    {
        using var site = new Site(Site.WebServer("<rewrite><rules>" + Rule("^a$", "type='Rewrite' url='b'") + "</rules>" +
            "<outboundRules><rule name='o'><match serverVariable='RESPONSE_X' pattern='.*' /><action type='Rewrite' value='y' /></rule></outboundRules>" +
            "<providers><provider name='p' type='T' /></providers></rewrite>"));
        Assert.Equal("/b", await PathAfter(site, "/a"));
    }

    [Fact] // the host's choice: outboundRules and providers without entries are nothing to tell
    public async Task Empty_outbound_rules_and_providers_are_ignored()
    {
        using var site = new Site(Site.WebServer("<rewrite><rules>" + Rule("^a$", "type='Rewrite' url='b'") + "</rules><outboundRules /><providers /></rewrite>"));
        Assert.Equal("/b", await PathAfter(site, "/a"));
    }

    [Fact] // IIS's rewrite schema: a rule's enabled attribute (default true); a disabled rule is not evaluated
    public async Task A_disabled_rule_is_not_run()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b'", attributes: "enabled='false'") +
            "<rule name='s'><match url='^a$' /><action type='Rewrite' url='c' /></rule>"));
        Assert.Equal("/c", await PathAfter(site, "/a"));
    }

    [Fact] // the host's choice (summary: told, not done): a rule whose pattern is not a regular expression is left out, the others run
    public async Task A_rule_with_an_invalid_pattern_is_left_out()
    {
        using var site = new Site(Rules(Rule("(", "type='Rewrite' url='b'") +
            "<rule name='i'><match url='(' ignoreCase='false' /><action type='Rewrite' url='b' /></rule>" +
            "<rule name='s'><match url='^a$' /><action type='Rewrite' url='c' /></rule><unknown />"));
        Assert.Equal("/c", await PathAfter(site, "/a"));
    }

    [Fact] // the host's choice (summary): rules from a configSource file next to the web.config
    public async Task Rules_are_read_from_their_config_source()
    {
        using var site = new Site(Site.WebServer("<rewrite><rules configSource='rewrite.config' /></rewrite>"));
        site.Write("rewrite.config", "<rules>" + Rule("^a$", "type='Rewrite' url='b'") + "</rules>");
        Assert.Equal("/b", await PathAfter(site, "/a"));
    }

    [Fact] // the host's choice (summary: told, not done): a configSource file that is not XML gives the element itself
    public void A_config_source_that_is_not_xml_gives_no_rules()
    {
        using var site = new Site(Site.WebServer("<rewrite><rules configSource='rewrite.config' /></rewrite>"));
        site.Write("rewrite.config", "<rules>");
        Assert.Null(IisUrlRewrite.Load(site.Root));
    }

    [Fact] // the host's choice (summary: told, not done): a configSource that cannot be read gives the element itself
    public void A_missing_config_source_gives_no_rules()
    {
        using var site = new Site(Site.WebServer("<rewrite><rules configSource='missing.config' /></rewrite>"));
        Assert.Null(IisUrlRewrite.Load(site.Root));
    }

    // --- Patterns

    [Fact]
    public async Task A_rewrite_changes_the_path_the_application_sees()
    {
        using var site = new Site(Rules("<rule name='a'><match url='^old/(.*)$' /><action type='Rewrite' url='new/{R:1}' /></rule>"));
        var context = Site.Request("/old/page.aspx?x=1");
        Assert.False(await IisUrlRewrite.Load(site.Root).RunAsync(context));
        Assert.Equal("/new/page.aspx", context.Request.Path.Value);
        Assert.Equal("?x=1", context.Request.QueryString.Value);
        Assert.Equal("/old/page.aspx?x=1", context.Request.Headers["X-Original-URL"].ToString());
    }

    [Fact] // the reference: "By default, case-insensitive pattern matching is used"
    public async Task Patterns_ignore_case_by_default()
    {
        using var site = new Site(Rules(Rule("^OLD/(.*)$", "type='Rewrite' url='/new/{R:1}'")));
        Assert.Equal("/new/Page", await PathAfter(site, "/old/Page"));
    }

    [Fact] // the reference: the ignoreCase attribute of <match> enables case sensitivity
    public async Task IgnoreCase_false_matches_case()
    {
        using var site = new Site(Rules("<rule name='r'><match url='^OLD$' ignoreCase='false' /><action type='Rewrite' url='/new' /></rule>"));
        Assert.Equal("/old", await PathAfter(site, "/old"));
        Assert.Equal("/new", await PathAfter(site, "/OLD"));
    }

    [Fact] // the reference: the rule pattern gets the path without its leading '/' nor the query string
    public async Task The_pattern_gets_the_path_without_its_slash_and_query_string()
    {
        using var site = new Site(Rules(Rule("^content/default\\.aspx$", "type='Rewrite' url='/matched'")));
        Assert.Equal("/matched?tabid=2&subtabid=3", await PathAfter(site, "/content/default.aspx?tabid=2&subtabid=3"));
    }

    [Fact] // the host's choice (summary): the path matched decoded
    public async Task The_pattern_gets_the_decoded_path()
    {
        using var site = new Site(Rules(Rule("^a b/(.*)$", "type='Rewrite' url='/x/{R:1}'")));
        Assert.Equal("/x/c d", await PathAfter(site, "/a%20b/c%20d"));
    }

    [Fact] // the host's choice (not in IIS's docs): a request without a path is the root's ("" to the pattern)
    public async Task An_empty_path_is_the_roots()
    {
        using var site = new Site(Rules(Rule("^$", "type='Rewrite' url='home.aspx'")));
        Assert.Equal("/home.aspx", await PathAfter(site, ""));
    }

    [Fact] // the reference: a back-reference of a group that took no part is empty (ECMAScript's)
    public async Task A_group_that_did_not_match_is_empty()
    {
        using var site = new Site(Rules(Rule("^old(/x)?/(.*)$", "type='Rewrite' url='/new[{R:1}]{R:2}'")));
        Assert.Equal("/new[]y", await PathAfter(site, "/old/y"));
    }

    [Fact] // the host's choice (not in IIS's docs): a lookbehind (not ECMAScript 3's) is taken, as .NET's
    public async Task A_lookbehind_is_taken()
    {
        using var site = new Site(Rules(Rule("^old/(.*)(?&lt;!\\.bak)$", "type='Rewrite' url='/new/{R:1}'")));
        Assert.Equal("/new/a.aspx", await PathAfter(site, "/old/a.aspx"));
        Assert.Equal("/old/a.bak", await PathAfter(site, "/old/a.bak"));
    }

    [Fact] // the host's choice (summary), as above, with ignoreCase='false'
    public async Task A_lookbehind_with_case()
    {
        using var site = new Site(Rules("<rule name='r'><match url='^Old/(.*)(?&lt;!\\.bak)$' ignoreCase='false' /><action type='Rewrite' url='/new/{R:1}' /></rule>"));
        Assert.Equal("/new/a.aspx", await PathAfter(site, "/Old/a.aspx"));
        Assert.Equal("/old/a.aspx", await PathAfter(site, "/old/a.aspx"));
    }

    [Fact] // the host's choice (not in IIS's docs: the schema requires <match url>): a rule without a pattern matches every URL
    public async Task A_rule_without_match_matches_everything()
    {
        using var site = new Site(Rules("<rule name='r'><action type='Rewrite' url='/all' /></rule>"));
        Assert.Equal("/all", await PathAfter(site, "/anything"));
    }

    [Fact] // the reference: Wildcard, '*' matches any characters and captures them
    public async Task Wildcard_stars_capture()
    {
        using var site = new Site(Rules(Rule("*/*.html", "type='Rewrite' url='/{R:1}-{R:2}.aspx'", attributes: "patternSyntax='Wildcard'")));
        Assert.Equal("/contoso-test.aspx", await PathAfter(site, "/contoso/test.html"));
        Assert.Equal("/A-B.aspx", await PathAfter(site, "/A/B.HTML"));   // case ignored by default
    }

    [Fact] // the reference: Wildcard patterns ignore case by default, ignoreCase='false' matches it
    public async Task Wildcard_ignore_case()
    {
        using var site = new Site(Rules("<rule name='r' patternSyntax='Wildcard'><match url='Old/*' ignoreCase='false' /><action type='Rewrite' url='/new/{R:1}' /></rule>"));
        Assert.Equal("/old/x", await PathAfter(site, "/old/x"));
        Assert.Equal("/new/x", await PathAfter(site, "/Old/x"));
        using var site2 = new Site(Rules("<rule name='r' patternSyntax='Wildcard'><match url='Old/*' /><action type='Rewrite' url='/new/{R:1}' /></rule>"));
        Assert.Equal("/new/x", await PathAfter(site2, "/old/x"));
    }

    [Fact] // observed on IIS 10 (URL Rewrite 2.1): a Wildcard "?" matches a '?' only (the reference says "exactly one character":
    // "scripts/*_in.???" did not match /scripts/a_in.asp, it matched /scripts/a_in.%3F%3F%3F; "we/a?c" matched /we/a%3Fc only)
    public async Task Wildcard_question_mark_is_a_question_mark()
    {
        using var site = new Site(Rules(Rule("scripts/*_in.???", "type='Rewrite' url='/s/{R:1}'", attributes: "patternSyntax='Wildcard'")));
        Assert.Equal("/scripts/a_in.asp", await PathAfter(site, "/scripts/a_in.asp"));
        Assert.Equal("/s/a", await PathAfter(site, "/scripts/a_in.%3F%3F%3F"));
    }

    [Fact] // the 2.0 reference: ExactMatch, an exact string comparison (case-insensitive by default)
    public async Task ExactMatch_compares_the_whole_path()
    {
        using var site = new Site(Rules(Rule("old.aspx", "type='Rewrite' url='/new.aspx?was={R:0}'", attributes: "patternSyntax='ExactMatch'")));
        Assert.Equal("/new.aspx?was=OLD.aspx", await PathAfter(site, "/OLD.aspx"));
        Assert.Equal("/old.aspx.bak", await PathAfter(site, "/old.aspx.bak"));
    }

    [Fact] // the reference: ignoreCase='false' makes the comparison case-sensitive
    public async Task ExactMatch_with_case()
    {
        using var site = new Site(Rules("<rule name='r' patternSyntax='ExactMatch'><match url='old.aspx' ignoreCase='false' /><action type='Rewrite' url='/new.aspx' /></rule>"));
        Assert.Equal("/OLD.aspx", await PathAfter(site, "/OLD.aspx"));
        Assert.Equal("/new.aspx", await PathAfter(site, "/old.aspx"));
    }

    [Fact] // the reference: ECMAScript is the default; the host's choice: an unknown patternSyntax is the default
    public async Task An_unknown_pattern_syntax_is_ECMAScript()
    {
        using var site = new Site(Rules(Rule("^o.d$", "type='Rewrite' url='/new'", attributes: "patternSyntax='Other'")));
        Assert.Equal("/new", await PathAfter(site, "/old"));
    }

    [Fact] // the reference: with negate, the action is performed only if the URL does not match; no back-references
    public async Task A_negated_pattern_runs_when_the_path_does_not_match()
    {
        using var site = new Site(Rules("<rule name='r'><match url='^(keep)' negate='true' /><action type='Rewrite' url='/other[{R:1}]' /></rule>"));
        Assert.Equal("/keep", await PathAfter(site, "/keep"));
        Assert.Equal("/other[]", await PathAfter(site, "/x"));
    }

    // --- Rules evaluation

    [Fact] // the reference: the next rule gets the substituted URL as its input
    public async Task The_next_rule_sees_the_rewritten_url()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b'") + "<rule name='s'><match url='^b$' /><action type='Rewrite' url='c' /></rule>"));
        Assert.Equal("/c", await PathAfter(site, "/a"));
    }

    [Fact] // the reference: stopProcessing, no more subsequent rules once the rule matched
    public async Task StopProcessing_stops_the_rules()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b'", attributes: "stopProcessing='true'") +
            "<rule name='s'><match url='^b$' /><action type='Rewrite' url='c' /></rule>"));
        Assert.Equal("/b", await PathAfter(site, "/a"));
    }

    [Fact] // the reference: a None action performs nothing; the rules go on, unless stopProcessing
    public async Task None_does_nothing()
    {
        using var site = new Site(Rules("<rule name='n'><match url='.*' /></rule>" + Rule("^a$", "type='Rewrite' url='b'")));
        Assert.Equal("/b", await PathAfter(site, "/a"));
        using var stop = new Site(Rules("<rule name='n' stopProcessing='true'><match url='.*' /><action type='None' /></rule>" + Rule("^a$", "type='Rewrite' url='b'")));
        var (answered, context) = await Run(stop, "/a");
        Assert.False(answered);
        Assert.Equal("/a", context.Request.Path.Value);
        Assert.False(context.Request.Headers.ContainsKey("X-Original-URL"));
    }

    // --- Conditions

    [Fact] // the reference: MatchAll (the default), every condition must be true
    public async Task All_conditions_must_match_by_default()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b'",
            "<conditions><add input='{QUERY_STRING}' pattern='x=1' /><add input='{QUERY_STRING}' pattern='y=2' /></conditions>")));
        Assert.Equal("/b?x=1&y=2", await PathAfter(site, "/a?x=1&y=2"));
        Assert.Equal("/a?x=1", await PathAfter(site, "/a?x=1"));
    }

    [Fact] // the reference: MatchAny, at least one condition true
    public async Task MatchAny_needs_one_condition()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b'",
            "<conditions logicalGrouping='MatchAny'><add input='{QUERY_STRING}' pattern='x=1' /><add input='{QUERY_STRING}' pattern='y=2' /></conditions>")));
        Assert.Equal("/b?y=2", await PathAfter(site, "/a?y=2"));
        Assert.Equal("/a?z=3", await PathAfter(site, "/a?z=3"));
    }

    [Fact] // the reference: a rule with no conditions runs its action when its pattern matches
    public async Task An_empty_conditions_collection_is_no_condition()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b'", "<conditions logicalGrouping='MatchAny' />")));
        Assert.Equal("/b", await PathAfter(site, "/a"));
    }

    [Fact] // the 2.0 reference: without trackAllCaptures, {C:1} is the last matched condition's (p2, not p1)
    public async Task Condition_back_references_are_the_last_matched_conditions()
    {
        using var site = new Site(Rules(Rule("^article\\.aspx", "type='Rewrite' url='article.aspx/{C:1}' appendQueryString='false'",
            "<conditions><add input='{QUERY_STRING}' pattern='p1=([0-9]+)' /><add input='{QUERY_STRING}' pattern='p2=([a-z]+)' /></conditions>")));
        Assert.Equal("/article.aspx/abc", await PathAfter(site, "/article.aspx?p1=123&p2=abc"));
    }

    [Fact] // the 2.0 reference's example: trackAllCaptures, {C:0} "/article/23/", {C:1} "article", {C:2} "23", {C:3} "abc"
    public async Task TrackAllCaptures_indexes_the_captures_of_every_condition()
    {
        // the doc gives {REQUEST_URI} "/article/23/" with a query string (REQUEST_URI has it): {URL} here
        using var site2 = new Site(Rules(Rule(".*", "type='Rewrite' url='/r?0={C:0}&amp;1={C:1}&amp;2={C:2}&amp;3={C:3}' appendQueryString='false'",
            "<conditions trackAllCaptures='true'><add input='{URL}' pattern='^/([a-zA-Z]+)/([0-9]+)/$' /><add input='{QUERY_STRING}' pattern='p2=([a-z]+)' /></conditions>")));
        Assert.Equal("/r?0=/article/23/&1=article&2=23&3=abc", await PathAfter(site2, "/article/23/?p1=123&p2=abc"));
    }

    [Fact] // the reference: a condition's input can use the rule's back-references and the previously matched condition's
    public async Task Condition_inputs_use_earlier_back_references()
    {
        using var site = new Site(Rules(Rule("^(\\w+)/(\\w+)$", "type='Rewrite' url='/r/{C:1}' appendQueryString='false'",
            "<conditions><add input='{R:1}' pattern='^(\\w)' /><add input='{C:1}{R:2}' pattern='^(.*)$' /></conditions>")));
        Assert.Equal("/r/ab", await PathAfter(site, "/a1/b"));
    }

    [Fact] // the reference: a condition's result can be negated; the host's choice: a negated condition captures nothing
    public async Task A_negated_condition()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='/b[{C:0}]' appendQueryString='false'",
            "<conditions><add input='{QUERY_STRING}' pattern='(debug)' negate='true' /></conditions>")));
        Assert.Equal("/b[]", await PathAfter(site, "/a?x=1"));
        Assert.Equal("/a?debug", await PathAfter(site, "/a?debug"));
    }

    [Fact] // the reference: a condition's ignoreCase attribute (default true)
    public async Task Condition_ignore_case()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b'",
            "<conditions><add input='{QUERY_STRING}' pattern='^X=1$' ignoreCase='false' /></conditions>")));
        Assert.Equal("/a?x=1", await PathAfter(site, "/a?x=1"));
        Assert.Equal("/b?X=1", await PathAfter(site, "/a?X=1"));
    }

    [Fact] // the reference: the conditions' patterns use the rule's patternSyntax
    public async Task Conditions_use_the_rules_pattern_syntax()
    {
        using var site = new Site(Rules("<rule name='r' patternSyntax='Wildcard'><match url='*' /><conditions><add input='{HTTP_HOST}' pattern='*.mysite.com' /></conditions><action type='Rewrite' url='/{C:1}/{R:1}' /></rule>"));
        var (_, context) = await Run(site, "/page", host: "shop.mysite.com");
        Assert.Equal("/shop/page", context.Request.Path.Value);
    }

    [Fact] // the reference: IsFile and IsDirectory on {REQUEST_FILENAME}, negated: the usual "not a file nor a folder" rule
    public async Task IsFile_and_IsDirectory()
    {
        using var site = new Site(Rules(Rule("^(.*)$", "type='Rewrite' url='/index.aspx?p={R:1}'",
            "<conditions><add input='{REQUEST_FILENAME}' matchType='IsFile' negate='true' /><add input='{REQUEST_FILENAME}' matchType='IsDirectory' negate='true' /></conditions>")));
        site.Write("a.css", "x").Write("dir/x.txt", "x");
        Assert.Equal("/a.css", await PathAfter(site, "/a.css"));
        Assert.Equal("/dir", await PathAfter(site, "/dir"));
        Assert.Equal("/dir/", await PathAfter(site, "/dir/"));
        Assert.Equal("/index.aspx?p=missing", await PathAfter(site, "/missing"));
    }

    [Fact] // the reference: IsDirectory is true for a folder (with or without its trailing separator)
    public async Task IsDirectory_without_negate()
    {
        using var site = new Site(Rules(Rule("^(.*)$", "type='Rewrite' url='/folder'",
            "<conditions><add input='{REQUEST_FILENAME}' matchType='IsDirectory' /></conditions>")));
        site.Write("dir/x.txt", "x");
        Assert.Equal("/folder", await PathAfter(site, "/dir/"));
        Assert.Equal("/dir/x.txt", await PathAfter(site, "/dir/x.txt"));
    }

    [Fact]
    // the reference: "If a condition input string is not specified, the URL Rewrite Module uses the physical path of the requested file"
    public async Task IsFile_without_input_is_the_requested_file()
    {
        using var site = new Site(Rules(Rule(".*", "type='Rewrite' url='/file'", "<conditions><add matchType='IsFile' /></conditions>")));
        site.Write("a.css", "x");
        Assert.Equal("/file", await PathAfter(site, "/a.css"));
    }

    [Fact] // the host's choice (not in IIS's docs): a Pattern condition without input matches "", one without pattern matches anything
    public async Task Conditions_without_input_or_pattern()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b'", "<conditions><add pattern='^$' /><add input='{QUERY_STRING}' /></conditions>")));
        Assert.Equal("/b?x", await PathAfter(site, "/a?x"));
    }

    [Fact] // the 2.0 reference: trackAllCaptures with MatchAny, the captures of the condition that matched
    public async Task TrackAllCaptures_with_MatchAny()
    {
        using var site = new Site(Rules(Rule(".*", "type='Rewrite' url='/r/{C:1}' appendQueryString='false'",
            "<conditions logicalGrouping='MatchAny' trackAllCaptures='true'><add input='{QUERY_STRING}' pattern='p1=([0-9]+)' /><add input='{QUERY_STRING}' pattern='p2=([a-z]+)' /></conditions>")));
        Assert.Equal("/r/abc", await PathAfter(site, "/a?p2=abc"));
    }

    [Fact] // the host's choice: an unknown matchType is Pattern
    public async Task An_unknown_match_type_is_a_pattern()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b'", "<conditions><add input='{QUERY_STRING}' matchType='Other' pattern='x' /></conditions>")));
        Assert.Equal("/b?x", await PathAfter(site, "/a?x"));
    }

    // --- Rewrite

    [Fact] // the reference: appendQueryString (default true) appends the original query string to the substituted URL
    public async Task The_request_query_string_is_appended_after_the_rules()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b?r=1'")));
        Assert.Equal("/b?r=1&x=1", await PathAfter(site, "/a?x=1"));
        Assert.Equal("/b?r=1", await PathAfter(site, "/a"));
    }

    [Fact] // the reference: appendQueryString='false' drops the original query string
    public async Task AppendQueryString_false_drops_the_request_query_string()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b' appendQueryString='false'")));
        Assert.Equal("/b", await PathAfter(site, "/a?x=1"));
        using var site2 = new Site(Rules(Rule("^a$", "type='Rewrite' url='b?' ")));
        Assert.Equal("/b?x=1", await PathAfter(site2, "/a?x=1"));
    }

    [Fact] // the host's choice (summary): the rewritten URL's escapes decoded, as the application's path is
    public async Task The_rewritten_path_is_decoded()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='/new%20page.aspx'")));
        Assert.Equal("/new page.aspx", await PathAfter(site, "/a"));
    }

    [Fact] // the reference: UNENCODED_URL keeps the original URL as requested
    public async Task The_original_url_is_kept()
    {
        using var site = new Site(Rules(Rule("^a b$", "type='Rewrite' url='/c?u={UNENCODED_URL}'")));
        Assert.Equal("/c?u=/a%20b", await PathAfter(site, "/a%20b"));
    }

    [Fact] // observed on IIS 10 (URL Rewrite 2.1): X-Original-URL (HTTP_X_ORIGINAL_URL) is the URL as sent, "/a%20b" (the reference
    // says "in decoded format")
    public async Task X_Original_URL_is_the_url_as_sent()
    {
        using var site = new Site(Rules(Rule("^a b$", "type='Rewrite' url='/c'")));
        var (_, context) = await Run(site, "/a%20b");
        Assert.Equal("/a%20b", context.Request.Headers["X-Original-URL"].ToString());
    }

    [Fact] // the host's choice (summary): IIS_WasUrlRewritten set after a rewrite, the server variables rules set in Request.ServerVariables
    public async Task A_rewrite_sets_IIS_WasUrlRewritten()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b'")));
        var (_, context) = await Run(site, "/a");
        Assert.Equal("1", context.Features.Get<IisServerVariables>().Values["IIS_WasUrlRewritten"]);
        var (_, untouched) = await Run(site, "/x");
        Assert.Null(untouched.Features.Get<IisServerVariables>());
    }

    [Fact] // the host's choice (summary): a rewrite to another server is ARR's (not here): 502.3
    public async Task A_rewrite_to_another_server_is_a_502()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='http://backend/{R:0}'")));
        var (answered, context) = await Run(site, "/a");
        Assert.True(answered);
        Assert.Equal(502, context.Response.StatusCode);
        Assert.Equal(3, Log(context).SubStatus);
    }

    [Fact] // the 2.0 reference: logRewrittenUrl logs the rewritten URL instead of the original one
    public async Task LogRewrittenUrl_logs_the_rewritten_url()
    {
        using var site = new Site(Rules(Rule("^article/(\\d+)$", "type='Rewrite' url='article.aspx?id={R:1}' logRewrittenUrl='true'")));
        var (_, context) = await Run(site, "/article/5");
        Assert.Equal("/article.aspx", Log(context).LoggedStem);
        Assert.Equal("?id=5", Log(context).LoggedQuery);
        using var noQuery = new Site(Rules(Rule("^article/(\\d+)$", "type='Rewrite' url='a{R:1}.aspx' logRewrittenUrl='true'")));
        var (_, context2) = await Run(noQuery, "/article/5");
        Assert.Equal("/a5.aspx", Log(context2).LoggedStem);
        Assert.Null(Log(context2).LoggedQuery);
    }

    [Fact] // the 2.0 reference: without logRewrittenUrl the original URL is logged (nothing set)
    public async Task Without_logRewrittenUrl_the_original_url_is_logged()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='b'")));
        var (_, context) = await Run(site, "/a");
        Assert.Null(Log(context));
    }

    [Fact] // the host's choice: the application's path is the rewritten one under its PathBase (a virtual application)
    public async Task A_rewrite_under_a_path_base()
    {
        using var site = new Site(Rules(Rule("^app/a$", "type='Rewrite' url='/app/b'") + "<rule name='s'><match url='^app/c$' /><action type='Rewrite' url='/elsewhere' /></rule>"));
        var (_, context) = await Run(site, "/app/a", pathBase: "/app");
        Assert.Equal("/app", context.Request.PathBase.Value);
        Assert.Equal("/b", context.Request.Path.Value);
        var (_, outside) = await Run(site, "/app/c", pathBase: "/app");
        Assert.Equal("/elsewhere", outside.Request.Path.Value);
    }

    // --- Redirect

    [Fact] // the reference: Redirect's default redirectType is Permanent (301); the host's choice (summary): an absolute Location, IIS's body
    public async Task A_redirect_is_permanent_by_default()
    {
        using var site = new Site(Rules(Rule("^old/(.*)$", "type='Redirect' url='/new/{R:1}'")));
        var (answered, context) = await Run(site, "/old/x?q=1");
        Assert.True(answered);
        Assert.Equal(301, context.Response.StatusCode);
        Assert.Equal("http://localhost:8080/new/x?q=1", context.Response.Headers.Location.ToString());
        Assert.Equal("text/html; charset=UTF-8", context.Response.ContentType);
        Assert.Contains("<a HREF=\"http://localhost:8080/new/x?q=1\">", Site.Body(context));
        Assert.Equal(context.Response.ContentLength, ((MemoryStream)context.Response.Body).Length);
    }

    [Theory] // the reference: redirectType Found 302, SeeOther 303, Temporary 307; the host's choice: IIS's reason phrases
    [InlineData("Found", 302, "Redirect")]
    [InlineData("SeeOther", 303, null)]
    [InlineData("Temporary", 307, "Moved Temporarily")]
    [InlineData("Permanent", 301, null)]
    public async Task Redirect_types(string type, int status, string reason)
    {
        using var site = new Site(Rules(Rule("^a$", "type='Redirect' url='/b' redirectType='" + type + "'")));
        var (_, context) = await Run(site, "/a");
        Assert.Equal(status, context.Response.StatusCode);
        Assert.Equal(reason, context.Features.Get<IHttpResponseFeature>().ReasonPhrase);
    }

    [Fact] // the reference: a redirect to an absolute URI, the original query string appended (appendQueryString default true)
    public async Task A_redirect_to_an_absolute_url()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Redirect' url='https://example.com/b?r=1'") +
            "<rule name='s'><match url='^c$' /><action type='Redirect' url='https://example.com/d' /></rule>" +
            "<rule name='t'><match url='^e$' /><action type='Redirect' url='https://example.com/f?r=1' appendQueryString='false' /></rule>"));
        var (_, a) = await Run(site, "/a?x=1");
        Assert.Equal("https://example.com/b?r=1&x=1", a.Response.Headers.Location.ToString());
        var (_, c) = await Run(site, "/c?x=1");
        Assert.Equal("https://example.com/d?x=1", c.Response.Headers.Location.ToString());
        var (_, c2) = await Run(site, "/c");
        Assert.Equal("https://example.com/d", c2.Response.Headers.Location.ToString());
        var (_, e) = await Run(site, "/e?x=1");
        Assert.Equal("https://example.com/f?r=1", e.Response.Headers.Location.ToString());
    }

    [Fact] // the reference (ToLower's example): the canonical host redirect
    public async Task The_canonical_host_redirect()
    {
        using var site = new Site(Rules(Rule("^(.+)", "type='Redirect' url='http://www.mysite.com/{ToLower:{R:1}}' redirectType='Found'",
            "<conditions><add input='{HTTP_HOST}' pattern='^www\\.mysite\\.com$' negate='true' /></conditions>")));
        var (_, context) = await Run(site, "/Some/Page", host: "mysite.com");
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal("http://www.mysite.com/some/page", context.Response.Headers.Location.ToString());
        var (answered, _) = await Run(site, "/Some/Page", host: "www.mysite.com");
        Assert.False(answered);
    }

    [Fact] // the host's choice: a space in the Location escaped
    public async Task A_redirect_escapes_spaces()
    {
        using var site = new Site(Rules(Rule("^a b$", "type='Redirect' url='/c {R:0}' appendQueryString='false'")));
        var (_, context) = await Run(site, "/a%20b");
        Assert.Equal("http://localhost:8080/c%20a%20b", context.Response.Headers.Location.ToString());
    }

    [Fact] // the reference: no rules after a redirect
    public async Task No_rule_runs_after_a_redirect()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Redirect' url='/b'") + "<rule name='s'><match url='.*' /><action type='CustomResponse' statusCode='403' /></rule>"));
        var (_, context) = await Run(site, "/a");
        Assert.Equal(301, context.Response.StatusCode);
    }

    [Fact] // RFC 9110: a response to HEAD has no body
    public async Task A_redirect_to_HEAD_has_no_body()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Redirect' url='/b'")));
        var (answered, context) = await Run(site, "/a", method: "HEAD");
        Assert.True(answered);
        Assert.Equal("", Site.Body(context));
    }

    [Fact] // the host's choice: without a Host header, the Location's authority is the server's address (with its port: IIS 10, observed, Security/iis-golden "GET /sub HTTP/1.0 without Host"); IIS 10 (observed, "GET /q?to=/evil.example"): "//" a path of the host's too, not another host's
    public void Redirect_without_a_host()
    {
        var context = Site.Request("/a");
        context.Request.Host = new HostString();
        IisUrlRewrite.Redirect(context, 308, "/b");
        Assert.Equal("http://127.0.0.1/b", context.Response.Headers.Location.ToString());
        Assert.Equal("Redirected Permanently", context.Features.Get<IHttpResponseFeature>().ReasonPhrase);
        var other = Site.Request("/a");
        IisUrlRewrite.Redirect(other, 302, "//cdn.example.com/b");
        Assert.Equal("http://localhost:8080//cdn.example.com/b", other.Response.Headers.Location.ToString());
        var port = Site.Request("/a");
        port.Request.Host = new HostString();
        port.Connection.LocalPort = 8081;
        IisUrlRewrite.Redirect(port, 301, "/sub/");
        Assert.Equal("http://127.0.0.1:8081/sub/", port.Response.Headers.Location.ToString());
    }

    // --- CustomResponse, AbortRequest

    [Fact] // the reference: CustomResponse, the given status code, substatus and reason; no rules after
    public async Task A_custom_response()
    {
        using var site = new Site(Rules(Rule("^a$", "type='CustomResponse' statusCode='403' subStatusCode='4' statusReason='Forbidden here'") +
            "<rule name='s'><match url='.*' /><action type='Redirect' url='/x' /></rule>"));
        var (answered, context) = await Run(site, "/a");
        Assert.True(answered);
        Assert.Equal(403, context.Response.StatusCode);
        Assert.Equal("Forbidden here", context.Features.Get<IHttpResponseFeature>().ReasonPhrase);
        Assert.Equal(4, Log(context).SubStatus);
        Assert.Equal("", Site.Body(context));
    }

    [Fact] // the reference: subStatusCode and statusReason are optional (substatus 0, the status's own reason)
    public async Task A_custom_response_without_substatus_nor_reason()
    {
        using var site = new Site(Rules(Rule("^a$", "type='CustomResponse' statusCode='404'")));
        var (_, context) = await Run(site, "/a");
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal(0, Log(context).SubStatus);
        Assert.Null(context.Features.Get<IHttpResponseFeature>().ReasonPhrase);
    }

    [Fact] // the reference: AbortRequest drops the connection; the host's choice (summary): not logged
    public async Task AbortRequest_drops_the_connection()
    {
        using var site = new Site(Rules(Rule("^a$", "type='AbortRequest'")));
        Site.Lifetime lifetime = null;
        var (answered, context) = await Run(site, "/a", c => lifetime = Site.Lifetime.On(c));
        Assert.True(answered);
        Assert.True(lifetime.Aborted);
        Assert.True(Log(context).NotLogged);
    }

    // --- Server variables

    [Fact] // the reference's example: http://www.mysite.com/content/default.aspx?tabid=2&subtabid=3
    public async Task Server_variables_of_the_url_parts()
    {
        Assert.Equal("tabid=2&subtabid=3", await Value("{QUERY_STRING}"));
        Assert.Equal("www.mysite.com", await Value("{HTTP_HOST}"));
        Assert.Equal("80", await Value("{SERVER_PORT}"));
        Assert.Equal("0", await Value("{SERVER_PORT_SECURE}"));
        Assert.Equal("off", (await Value("{HTTPS}")).ToLowerInvariant());   // the doc: "OFF" (IIS's conditions ignore case)
        Assert.Equal("/content/default.aspx?tabid=2&subtabid=3", await Value("{REQUEST_URI}"));
        Assert.Equal("/content/default.aspx", await Value("{PATH_INFO}"));
    }

    [Fact] // IIS Server Variables (learn.microsoft.com/previous-versions/iis/6.0-sdk/ms524602): HTTPS ON, SERVER_PORT_SECURE 1, the port
    public async Task Server_variables_over_https()
    {
        Assert.Equal("on", (await Value("{HTTPS}", https: true)).ToLowerInvariant());
        Assert.Equal("1", await Value("{SERVER_PORT_SECURE}", https: true));
        Assert.Equal("443", await Value("{SERVER_PORT}", https: true));
        Assert.Equal("8443", await Value("{SERVER_PORT}", host: "www.mysite.com:8443", https: true));
    }

    [Fact] // IIS Server Variables: URL and SCRIPT_NAME the path, UNENCODED_URL and HTTP_URL the raw URL, the request's method, protocol, names
    public async Task Server_variables_of_the_request()
    {
        Assert.Equal("/a b", await Value("{URL}", "/a%20b?x=1"));
        Assert.Equal("/a b", await Value("{SCRIPT_NAME}", "/a%20b?x=1"));
        Assert.Equal("/a%20b?x=1", await Value("{UNENCODED_URL}", "/a%20b?x=1"));
        Assert.Equal("/a%20b?x=1", await Value("{HTTP_URL}", "/a%20b?x=1"));
        Assert.Equal("GET", await Value("{REQUEST_METHOD}"));
        Assert.Equal("HTTP/1.1", await Value("{SERVER_PROTOCOL}"));
        Assert.Equal("www.mysite.com", await Value("{SERVER_NAME}", host: "www.mysite.com:8080"));
    }

    [Fact] // IIS Server Variables: REMOTE_ADDR, REMOTE_HOST (the address without a name), REMOTE_PORT, LOCAL_ADDR
    public async Task Server_variables_of_the_connection()
    {
        Assert.Equal("192.168.1.10", await Value("{REMOTE_ADDR}"));   // an IPv4 client on a dual-stack socket
        Assert.Equal("192.168.1.10", await Value("{REMOTE_HOST}"));
        Assert.Equal("50000", await Value("{REMOTE_PORT}"));
        Assert.Equal("127.0.0.1", await Value("{LOCAL_ADDR}"));
        Assert.Equal("::1", await Value("{REMOTE_ADDR}", setup: c => c.Connection.RemoteIpAddress = System.Net.IPAddress.IPv6Loopback));
        // the host's choice: no address known, empty
        Assert.Equal("", await Value("{LOCAL_ADDR}", setup: c => c.Connection.LocalIpAddress = null));
    }

    [Fact] // IIS Server Variables: CONTENT_TYPE and CONTENT_LENGTH as the client gave them; the host's choice: 0 without a length
    public async Task Server_variables_of_the_content()
    {
        Assert.Equal("", await Value("{CONTENT_TYPE}"));
        Assert.Equal("0", await Value("{CONTENT_LENGTH}"));
        Assert.Equal("text/plain", await Value("{CONTENT_TYPE}", setup: c => c.Request.ContentType = "text/plain"));
        Assert.Equal("12", await Value("{CONTENT_LENGTH}", setup: c => c.Request.ContentLength = 12));
    }

    [Fact] // IIS Server Variables: APPL_PHYSICAL_PATH, PATH_TRANSLATED, REQUEST_FILENAME, SCRIPT_FILENAME the physical paths
    public async Task Server_variables_of_the_files()
    {
        using var site = new Site(Rules(Rule(".*", "type='Rewrite' url='/v?{APPL_PHYSICAL_PATH}|{REQUEST_FILENAME}|{SCRIPT_FILENAME}|{PATH_TRANSLATED}' appendQueryString='false'")));
        var (_, context) = await Run(site, "/content/default.aspx");
        var root = site.Root + Path.DirectorySeparatorChar;
        var file = Path.Combine(site.Root, "content", "default.aspx");
        Assert.Equal("?" + root + "|" + file + "|" + file + "|" + file, context.Request.QueryString.Value);
    }

    [Fact] // IIS Server Variables: APPL_MD_PATH /LM/W3SVC/<site>/ROOT, INSTANCE_ID; the host's choice: site 1 without a profile
    public async Task Server_variables_of_the_site()
    {
        Assert.Equal("/LM/W3SVC/1/ROOT", await Value("{APPL_MD_PATH}"));
        Assert.Equal("1", await Value("{INSTANCE_ID}"));
        var profile = "<profile><site id='3' /></profile>";
        Assert.Equal("/LM/W3SVC/3/ROOT", await Value("{APPL_MD_PATH}", profile: profile));
        Assert.Equal("3", await Value("{INSTANCE_ID}", profile: profile));
    }

    [Fact] // IIS Server Variables: APPL_PHYSICAL_PATH with one separator at its end, whichever way the folder was given
    public async Task APPL_PHYSICAL_PATH_of_a_folder_given_with_its_separator()
    {
        using var site = new Site(Rules(Rule(".*", "type='Rewrite' url='/v?{APPL_PHYSICAL_PATH}' appendQueryString='false'")));
        var context = Site.Request("/a");
        await IisUrlRewrite.Load(site.Root + Path.DirectorySeparatorChar).RunAsync(context);
        Assert.Equal("?" + site.Root + Path.DirectorySeparatorChar, context.Request.QueryString.Value);
    }

    [Fact] // the host's choice: CACHE_URL as IIS writes it, scheme://host:port/url
    public async Task Server_variable_CACHE_URL()
    {
        Assert.Equal("http://www.mysite.com:80/content/default.aspx?tabid=2&subtabid=3", await Value("{CACHE_URL}"));
    }

    [Fact] // the reference: the variables after a rewrite are the rewritten URL's (REQUEST_URI, URL, QUERY_STRING, CACHE_URL); UNENCODED_URL stays
    public async Task Server_variables_after_a_rewrite()
    {
        using var site = new Site(Rules(Rule("^a$", "type='Rewrite' url='/b?r=1' appendQueryString='false'") +
            "<rule name='s'><match url='.*' /><action type='Rewrite' url='/v?{REQUEST_URI}|{URL}|{QUERY_STRING}|{UNENCODED_URL}|{CACHE_URL}' appendQueryString='false' /></rule>"));
        var (_, context) = await Run(site, "/a?x=1");
        Assert.Equal("?/b?r=1|/b|r=1|/a?x=1|http://localhost:8080/b?r=1", context.Request.QueryString.Value);
        using var site2 = new Site(Rules(Rule("^a$", "type='Rewrite' url='/b' appendQueryString='false'") +
            "<rule name='s'><match url='.*' /><action type='Rewrite' url='/v?{REQUEST_URI}|{CACHE_URL}' appendQueryString='false' /></rule>"));
        var (_, context2) = await Run(site2, "/a?x=1");
        Assert.Equal("?/b|http://localhost:8080/b", context2.Request.QueryString.Value);
    }

    [Fact] // the reference: a request header is HTTP_ with '-' as '_'; the host's choice: an unknown variable is empty
    public async Task Request_headers_and_unknown_variables()
    {
        Assert.Equal("Agent/1", await Value("{HTTP_USER_AGENT}", setup: c => c.Request.Headers.UserAgent = "Agent/1"));
        Assert.Equal("", await Value("{HTTP_X_NONE}"));
        Assert.Equal("", await Value("{NO_SUCH_VARIABLE}"));
    }

    // --- Functions, back-references, maps

    [Fact] // the reference: ToLower, UrlEncode, UrlDecode, nested
    public async Task String_functions()
    {
        Assert.Equal("default.htm", await Value("{ToLower:DEFAULT.HTM}"));
        Assert.Equal("a%20b%26c", await Value("{UrlEncode:a b&amp;c}"));
        Assert.Equal("résumé", await Value("{UrlDecode:r%C3%A9sum%C3%A9}"));
        Assert.Equal("/content/default.aspx", await Value("{ToLower:{URL}}", "/Content/Default.aspx"));
    }

    [Fact] // the reference: {R:N}, N 0 to 9; the host's choice: a group the pattern has not is empty, an unclosed brace kept as written
    public async Task Back_references_out_of_range_and_unclosed_braces()
    {
        using var site = new Site(Rules(Rule("^(a)$", "type='Rewrite' url='/b{R:0}{R:1}[{R:5}][{C:1}]{R:1' appendQueryString='false'")));
        Assert.Equal("/baa[][]{R:1", await PathAfter(site, "/a"));
    }

    [Fact] // the host's choice (summary: providers not done): an unknown function or map gives ""
    public async Task An_unknown_map_gives_nothing()
    {
        Assert.Equal("[]", await Value("[{NoSuchMap:key}]"));
    }

    static string Maps(string maps, string rules) => Site.WebServer("<rewrite><rewriteMaps>" + maps + "</rewriteMaps><rules>" + rules + "</rules></rewrite>");

    const string StaticRewrites = "<rewriteMap name='StaticRewrites' defaultValue=''>" +
        "<add key='/diagnostics' value='/default.aspx?tabid=2&amp;subtabid=29' /><add key='/php' value='/default.aspx?tabid=7116' /></rewriteMap>";

    const string MapRule = "<rule name='Rewrite Rule'><match url='.*' /><conditions><add input='{StaticRewrites:{REQUEST_URI}}' pattern='(.+)' /></conditions><action type='Rewrite' url='{C:1}' /></rule>";

    [Fact] // the reference's rewrite map example 1
    public async Task A_rewrite_map()
    {
        using var site = new Site(Maps(StaticRewrites, MapRule));
        Assert.Equal("/default.aspx?tabid=2&subtabid=29", await PathAfter(site, "/diagnostics"));
        Assert.Equal("/default.aspx?tabid=7116", await PathAfter(site, "/php"));
        Assert.Equal("/default.aspx", await PathAfter(site, "/default.aspx"));
    }

    [Fact] // IIS's rewrite schema: a map's ignoreCase defaults to true; the reference: defaultValue for a key not found
    public async Task Map_keys_ignore_case_and_the_default_value()
    {
        using var site = new Site(Maps("<rewriteMap name='m' defaultValue='none'><add key='A' value='1' /><add value='no key' /><add key='B' /></rewriteMap>" +
            "<rewriteMap name='cs' ignoreCase='false'><add key='A' value='1' /></rewriteMap><rewriteMap /><other />",
            Rule("^(.*)$", "type='Rewrite' url='/v?{m:{R:1}}|{cs:{R:1}}' appendQueryString='false'")));
        Assert.Equal("/v?1|", await PathAfter(site, "/a"));
        Assert.Equal("/v?1|1", await PathAfter(site, "/A"));
        Assert.Equal("/v?none|", await PathAfter(site, "/z"));
        Assert.Equal("/v?|", await PathAfter(site, "/B"));
    }

    [Fact] // IIS's configuration inheritance: a child level's <remove> and <clear/> of maps; the host's choice (summary): maps from configSource
    public async Task Maps_removed_cleared_and_from_config_source()
    {
        using var site = new Site(Maps("<rewriteMap name='a'><add key='k' value='1' /></rewriteMap><rewriteMap name='b'><add key='k' value='2' /></rewriteMap><remove name='a' /><remove />",
            Rule("^k$", "type='Rewrite' url='/v?[{a:k}][{b:k}]' appendQueryString='false'")));
        Assert.Equal("/v?[][2]", await PathAfter(site, "/k"));
        using var cleared = new Site(Maps("<rewriteMap name='b'><add key='k' value='2' /></rewriteMap><clear />",
            Rule("^k$", "type='Rewrite' url='/v?[{b:k}]' appendQueryString='false'")));
        Assert.Equal("/v?[]", await PathAfter(cleared, "/k"));
        using var external = new Site(Site.WebServer("<rewrite><rewriteMaps configSource='maps.config' /><rules>" +
            Rule("^k$", "type='Rewrite' url='/v?[{b:k}]' appendQueryString='false'") + "</rules></rewrite>"));
        external.Write("maps.config", "<rewriteMaps><rewriteMap name='b'><add key='k' value='3' /></rewriteMap></rewriteMaps>");
        Assert.Equal("/v?[3]", await PathAfter(external, "/k"));
    }

    [Fact]
    // IIS's configuration system: a collection's <clear/> in a folder's web.config applies to that folder and below
    public async Task A_folders_map_clear_does_not_reach_other_folders()
    {
        using var site = new Site(Maps("<rewriteMap name='b'><add key='k' value='2' /></rewriteMap>",
            Rule("^k$", "type='Rewrite' url='/v?[{b:k}]' appendQueryString='false'")));
        site.Write("content/web.config", Site.WebServer("<rewrite><rewriteMaps><clear /></rewriteMaps></rewrite>"));
        Assert.Equal("/v?[2]", await PathAfter(site, "/k"));
    }

    // --- Setting server variables

    const string Allowed = "<rewrite><allowedServerVariables><add name='X_REQUESTED_URL_PATH' /><add name='HTTP_X_ORIGINAL_HOST' /><add name='RESPONSE_X_Custom' /></allowedServerVariables></rewrite>";

    [Fact] // the 2.0 reference's example: a rule sets X_REQUESTED_URL_PATH (allowed), and rewrites
    public async Task A_rule_sets_an_allowed_server_variable()
    {
        using var site = new Site(Rules("<rule name='Rewrite to index.php' stopProcessing='true'><match url='(.*)\\.htm$' />" +
            "<serverVariables><set name='X_REQUESTED_URL_PATH' value='{R:1}' /></serverVariables><action type='Rewrite' url='index.php?path={R:1}' /></rule>"),
            Site.Profile(Allowed));
        var (_, context) = await Run(site, "/page.htm");
        Assert.Equal("/index.php?path=page", context.Request.Path.Value + context.Request.QueryString.Value);
        Assert.Equal("page", context.Features.Get<IisServerVariables>().Values["X_REQUESTED_URL_PATH"]);
    }

    [Fact] // the 2.0 reference: a variable not in allowedServerVariables is a runtime error; the host's choice (summary): IIS's 500.50
    public async Task A_rule_setting_a_variable_not_allowed_is_a_500_50()
    {
        using var site = new Site(Rules(Rule(".*", "type='Rewrite' url='b'", "<serverVariables><set name='X_OTHER' value='1' /></serverVariables>")), Site.Profile(Allowed));
        var (answered, context) = await Run(site, "/a");
        Assert.True(answered);
        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal(50, Log(context).SubStatus);
        Assert.Equal("URL Rewrite Module Error.", context.Features.Get<IHttpResponseFeature>().ReasonPhrase);
    }

    [Fact] // the host's choice (summary): without a profile to say, any variable may be set
    public async Task Without_a_profile_any_variable_may_be_set()
    {
        using var site = new Site(Rules(Rule(".*", "type='Rewrite' url='b'", "<serverVariables><set name='X_OTHER' value='1' /><set value='no name' /><set name='X_EMPTY' /></serverVariables>")));
        var (_, context) = await Run(site, "/a");
        Assert.Equal("1", context.Features.Get<IisServerVariables>().Values["X_OTHER"]);
        Assert.Equal("", context.Features.Get<IisServerVariables>().Values["X_EMPTY"]);   // the host's choice: no value, ""
    }

    [Fact] // IIS's configuration: allowedServerVariables' <clear/>, <remove>, <add> in order (the host's choice: an entry without a name is ignored)
    public async Task Allowed_variables_cleared_and_removed()
    {
        var profile = Site.Profile("<rewrite><allowedServerVariables><add name='A' /><clear /><add name='B' /><add name='C' /><remove name='C' /><add /><remove /></allowedServerVariables></rewrite>");
        using var a = new Site(Rules(Rule(".*", "type='None'", "<serverVariables><set name='A' value='1' /></serverVariables>")), profile);
        Assert.Equal(500, (await Run(a, "/x")).Context.Response.StatusCode);
        using var b = new Site(Rules(Rule(".*", "type='None'", "<serverVariables><set name='b' value='1' /></serverVariables>")), profile);
        Assert.Equal("1", (await Run(b, "/x")).Context.Features.Get<IisServerVariables>().Values["B"]);
        using var c = new Site(Rules(Rule(".*", "type='None'", "<serverVariables><set name='C' value='1' /></serverVariables>")), profile);
        Assert.Equal(500, (await Run(c, "/x")).Context.Response.StatusCode);
    }

    [Fact] // the 2.0 reference: HTTP_X_ORIGINAL_HOST sets the request header x-original-host; replace (default true)
    public async Task A_rule_sets_a_request_header()
    {
        using var site = new Site(Rules(Rule(".*", "type='None'", "<serverVariables><set name='HTTP_X_ORIGINAL_HOST' value='{HTTP_HOST}' /></serverVariables>")), Site.Profile(Allowed));
        var (_, context) = await Run(site, "/a", c => c.Request.Headers["X-Original-Host"] = "old");
        Assert.Equal("localhost:8080", context.Request.Headers["x-original-host"].ToString());
    }

    [Fact] // the 2.0 reference: replace='false' does not overwrite a value that exists
    public async Task Replace_false_keeps_an_existing_value()
    {
        using var site = new Site(Rules(Rule(".*", "type='None'", "<serverVariables><set name='HTTP_X_ORIGINAL_HOST' value='new' replace='false' /></serverVariables>")));
        var (_, kept) = await Run(site, "/a", c => c.Request.Headers["X-Original-Host"] = "old");
        Assert.Equal("old", kept.Request.Headers["X-Original-Host"].ToString());
        var (_, added) = await Run(site, "/a");
        Assert.Equal("new", added.Request.Headers["X-Original-Host"].ToString());
    }

    [Fact] // the 2.0 reference: replace (default true) and replace='false' for a server variable a previous rule set; a later rule reads it
    public async Task Server_variables_set_by_one_rule_seen_by_the_next()
    {
        using var site = new Site(Rules("<rule name='a'><match url='.*' /><serverVariables><set name='X_V' value='1' /></serverVariables></rule>" +
            "<rule name='b'><match url='.*' /><serverVariables><set name='X_V' value='2' replace='false' /><set name='X_W' value='3' /><set name='X_W' value='4' /></serverVariables></rule>" +
            "<rule name='c'><match url='.*' /><action type='Rewrite' url='/v?{X_V}{X_W}' /></rule>"));
        Assert.Equal("/v?14", await PathAfter(site, "/a"));
    }

    [Fact] // the 2.0 reference ("Accessing Response Headers": RESPONSE_ names a response header, '_' as '-'); the host's choice (summary): set when the response starts
    public async Task A_rule_sets_a_response_header()
    {
        using var site = new Site(Rules(Rule(".*", "type='None'", "<serverVariables><set name='RESPONSE_X_Custom' value='v' /><set name='RESPONSE_X_Kept' value='new' replace='false' /></serverVariables>")));
        Site.StartingResponse response = null;
        var (_, context) = await Run(site, "/a", c => response = Site.StartingResponse.On(c));
        context.Response.Headers["X-Kept"] = "old";
        await response.StartAsync();
        Assert.Equal("v", context.Response.Headers["X-Custom"].ToString());
        Assert.Equal("old", context.Response.Headers["X-Kept"].ToString());
        var (_, other) = await Run(site, "/a", c => response = Site.StartingResponse.On(c));
        await response.StartAsync();
        Assert.Equal("new", other.Response.Headers["X-Kept"].ToString());
    }

    // --- Global and distributed rules

    static string GlobalRules(string rules, string more = "") => Site.Profile("<rewrite><globalRules>" + rules + "</globalRules>" + more + "</rewrite>");

    [Fact] // the reference: global rules operate on the absolute URL's path and run before the distributed rules
    public async Task Global_rules_run_first_on_the_absolute_path()
    {
        using var site = new Site(null, GlobalRules(Rule("^content/a\\.aspx$", "type='Rewrite' url='/content/b.aspx'")));
        site.Write("content/web.config", Site.WebServer("<rewrite><rules><rule name='d'><match url='^b\\.aspx$' /><action type='Rewrite' url='c.aspx' /></rule></rules></rewrite>"));
        Assert.Equal("/content/c.aspx", await PathAfter(site, "/content/a.aspx"));
    }

    [Fact] // the host's choice (summary): a global rewrite's URL is the one logged (IIS takes its log's URL after the global rules)
    public async Task A_global_rewrite_is_logged()
    {
        using var site = new Site(null, GlobalRules(Rule("^a$", "type='Rewrite' url='/b?x=1'")));
        var (_, context) = await Run(site, "/a");
        Assert.Equal("/b", Log(context).LoggedStem);
        Assert.Equal("?x=1", Log(context).LoggedQuery);
        var (_, untouched) = await Run(site, "/z");
        Assert.Null(Log(untouched));
    }

    [Fact] // the reference: a global rule that answers ends the request (no distributed rule runs)
    public async Task A_global_redirect_answers()
    {
        using var site = new Site(Rules(Rule(".*", "type='CustomResponse' statusCode='403'")), GlobalRules(Rule("^a$", "type='Redirect' url='/b'")));
        var (answered, context) = await Run(site, "/a");
        Assert.True(answered);
        Assert.Equal(301, context.Response.StatusCode);
    }

    [Fact] // the reference: AbortRequest in a global rule drops the connection, no rule after
    public async Task A_global_abort()
    {
        using var site = new Site(Rules(Rule(".*", "type='CustomResponse' statusCode='403'")), GlobalRules(Rule("^a$", "type='AbortRequest'")));
        Site.Lifetime lifetime = null;
        var (answered, context) = await Run(site, "/a", c => lifetime = Site.Lifetime.On(c));
        Assert.True(answered);
        Assert.True(lifetime.Aborted);
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    // the 2.0 reference: "Global rewrite rules can be used to set any request headers and server variables"; only distributed rules are limited
    public async Task Global_rules_may_set_any_variable()
    {
        using var site = new Site(null, GlobalRules(Rule(".*", "type='None'", "<serverVariables><set name='X_OTHER' value='1' /></serverVariables>"), Allowed.Replace("<rewrite>", "").Replace("</rewrite>", "")));
        var (answered, context) = await Run(site, "/a");
        Assert.False(answered);
        Assert.Equal("1", context.Features.Get<IisServerVariables>().Values["X_OTHER"]);
    }

    [Fact] // the reference: a distributed rule in /content gets "default.aspx" for /content/default.aspx
    public async Task A_folders_rule_gets_the_path_relative_to_its_folder()
    {
        using var site = new Site();
        site.Write("content/web.config", Rules(Rule("^default\\.aspx$", "type='Rewrite' url='/matched'")));
        Assert.Equal("/matched", await PathAfter(site, "/content/default.aspx"));
        Assert.Equal("/default.aspx", await PathAfter(site, "/default.aspx"));
        Assert.Equal("/contents/default.aspx", await PathAfter(site, "/contents/default.aspx"));
    }

    [Fact] // the reference: rules in a <location> operate on the path relative to the location's path
    public async Task A_locations_rule_gets_the_path_relative_to_the_location()
    {
        using var site = new Site("<configuration><location path='content'><system.webServer><rewrite><rules>" +
            Rule("^default\\.aspx$", "type='Rewrite' url='/matched'") + "</rules></rewrite></system.webServer></location></configuration>");
        Assert.Equal("/matched", await PathAfter(site, "/Content/default.aspx"));
        Assert.Equal("/default.aspx", await PathAfter(site, "/default.aspx"));
    }

    [Fact] // the host's choice (summary): a distributed rule's relative URL is its folder's
    public async Task A_folders_relative_url_is_in_its_folder()
    {
        using var site = new Site();
        site.Write("content/web.config", Rules(Rule("^a$", "type='Rewrite' url='b'") + "<rule name='s'><match url='^c$' /><action type='Redirect' url='d' /></rule>"));
        Assert.Equal("/content/b", await PathAfter(site, "/content/a"));
        var (_, context) = await Run(site, "/content/c");
        Assert.Equal("http://localhost:8080/content/d", context.Response.Headers.Location.ToString());
    }

    [Fact] // the reference ("Rules Inheritance"): the parent's rules first, then the child's
    public async Task Parent_rules_run_before_the_folders()
    {
        using var site = new Site(Rules(Rule("^content/(.*)$", "type='Rewrite' url='/content/root-{R:1}'")));
        site.Write("content/web.config", Rules("<rule name='c'><match url='^(.*)$' /><action type='Rewrite' url='/content/child-{R:1}' /></rule>"));
        Assert.Equal("/content/child-root-a.aspx", await PathAfter(site, "/content/a.aspx"));
        Assert.Equal("/other/a", await PathAfter(site, "/other/a"));
    }

    [Fact] // IIS's configuration collections: a folder's <clear/> drops the inherited rules there, not elsewhere
    public async Task A_folders_clear_drops_the_parents_rules()
    {
        using var site = new Site(Rules(Rule("^(.*)/a$", "type='Rewrite' url='/{R:1}/b'")));
        site.Write("content/web.config", Rules("<clear />"));
        Assert.Equal("/content/a", await PathAfter(site, "/content/a"));
        Assert.Equal("/other/b", await PathAfter(site, "/other/a"));
    }

    [Fact] // IIS's configuration collections: a folder's <remove name> drops that inherited rule (name without regard to case)
    public async Task A_folders_remove_drops_one_rule()
    {
        using var site = new Site(Rules("<rule name='One'><match url='^(.*)/a$' /><action type='Rewrite' url='/{R:1}/b' /></rule>" +
            "<rule name='two'><match url='^(.*)/x$' /><action type='Rewrite' url='/{R:1}/y' /></rule>"));
        site.Write("content/web.config", Rules("<remove name='one' />"));
        Assert.Equal("/content/a", await PathAfter(site, "/content/a"));
        Assert.Equal("/content/y", await PathAfter(site, "/content/x"));
    }

    [Fact] // the host's choice (summary): the rules of the profile's site <location> run as the site's, before the web.config's
    public async Task The_profiles_site_rules_are_distributed_rules()
    {
        using var site = new Site(Rules(Rule("^b$", "type='Rewrite' url='c'")),
            "<profile><location path=''><system.webServer><rewrite><rules>" + Rule("^a$", "type='Rewrite' url='b'") + "</rules></rewrite></system.webServer></location></profile>");
        Assert.Equal("/c", await PathAfter(site, "/a"));
    }

    // --- Middleware

    [Fact] // the host's choice: the pipeline goes on unless the rules answered
    public async Task The_middleware_calls_the_next_unless_answered()
    {
        using var site = new Site(Rules(Rule("^a$", "type='CustomResponse' statusCode='403'") + "<rule name='s'><match url='^b$' /><action type='Rewrite' url='c' /></rule>"));
        var rewrite = IisUrlRewrite.Load(site.Root);
        string reached = null;
        var middleware = new IisUrlRewrite.Middleware(c => { reached = c.Request.Path.Value; return Task.CompletedTask; }, rewrite);
        await middleware.Invoke(Site.Request("/a"));
        Assert.Null(reached);
        await middleware.Invoke(Site.Request("/b"));
        Assert.Equal("/c", reached);
    }
}
