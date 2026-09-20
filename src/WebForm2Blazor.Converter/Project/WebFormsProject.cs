using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WebForm2Blazor.Converter.Project;

/// <summary>
/// A tag prefix registered in a Web.config &lt;pages&gt;&lt;controls&gt; section.
/// Applies to all files under the config's directory (WebForms config inheritance).
/// </summary>
public sealed record ConfigTagRegistration(
    string ScopeDirectory,
    string TagPrefix,
    string? TagName,
    string? Src,
    string? Namespace,
    string? Assembly);

/// <summary>A .cs file from an --include library directory, ported with its output path.</summary>
public sealed record IncludedCodeFile(string AbsolutePath, string OutputRelativePath);

/// <summary>Inventory of the source WebForms project.</summary>
public sealed class WebFormsProject
{
    public required string RootDirectory { get; init; }
    public List<string> MasterPages { get; } = [];
    public List<string> Pages { get; } = [];
    public List<string> UserControls { get; } = [];

    /// <summary>.cs files that are neither code-behind nor designer files (business logic etc.; ported as-is).</summary>
    public List<string> PlainCodeFiles { get; } = [];

    /// <summary>
    /// Files on disk that the app's own .csproj does not compile, so they are not part of
    /// the application and were not converted. Reported, never silent: "the tool dropped a
    /// file" and "the tool decided the file was never yours" have to be told apart by the
    /// reader, and only the project file can settle which one it is.
    /// </summary>
    public List<string> NotCompiledFiles { get; } = [];

    /// <summary>
    /// .cs files from referenced library projects (--include). Real apps keep their page
    /// base classes and business logic in separate assemblies (YAF.Core,
    /// DotNetNuke.Library, ...); including them feeds the base-class registry and ports
    /// the sources alongside the app.
    /// </summary>
    public List<IncludedCodeFile> IncludedCodeFiles { get; } = [];

    public string? WebConfigPath { get; set; }

    /// <summary>Tag prefixes registered in any Web.config (root or subfolder).</summary>
    public List<ConfigTagRegistration> ConfigTagRegistrations { get; } = [];

    /// <summary>
    /// Namespaces imported for every page / control by Web.config
    /// &lt;pages&gt;&lt;namespaces&gt;. WebForms markup resolves helper types through
    /// these without any @Import in the file itself.
    /// </summary>
    public List<string> ConfigNamespaceImports { get; } = [];

    /// <summary>
    /// True when Web.config says &lt;pages clientIDMode="AutoID"&gt;.
    ///
    /// This decides whether the MASTER PAGE contributes to every ClientID under it. A
    /// master is a control on the page with an auto-generated ID, and the two modes
    /// disagree about what that means:
    ///
    ///   AutoID      - every naming container counts, generated ID or not, so the master's
    ///                 own "ctl00" leads: "ctl00_cphBody_divError"
    ///   Predictable - a naming container whose ID was generated is skipped, so the
    ///                 chain starts at the placeholder: "cphMain_cphBody_pClickResult"
    ///
    /// .NET 4.0 made Predictable the default, and both spellings are live in the corpora:
    /// BlogEngine / mojoPortal / YAF / n2 set AutoID explicitly, DNN and WingtipToys take
    /// the default. Both ID shapes above are quoted from golden data recorded off the
    /// ORIGINAL apps, which is what settled this - the converter had been emitting
    /// Predictable for everything, so the samples matched exactly and BlogEngine could not.
    /// </summary>
    public bool UsesAutoIdClientIds { get; set; }

    /// <summary>
    /// App_GlobalResources .resx files, which back &lt;%$ Resources: Class, Key %&gt;.
    /// Culture-specific variants (labels.ja.resx) are listed separately: embedding them
    /// next to the neutral file would collide, and satellite assemblies are a manual step.
    /// </summary>
    public List<string> GlobalResourceFiles { get; } = [];

    public List<string> CultureResourceFiles { get; } = [];

    /// <summary>
    /// App_LocalResources .resx files, keyed by the page / control they localize
    /// ("Admin/Default.aspx" -> "Admin/App_LocalResources/Default.aspx.resx").
    ///
    /// These back WebForms IMPLICIT localization: a control carrying
    /// meta:resourcekey="X" takes its Text / ToolTip / ... from the "X.Text" entries.
    /// The mapping is resolved at conversion time, so no runtime lookup is needed.
    /// </summary>
    public Dictionary<string, string> LocalResourceFiles { get; } = new(StringComparer.OrdinalIgnoreCase);

    public string RelativePath(string absolutePath)
        => Path.GetRelativePath(RootDirectory, absolutePath).Replace('\\', '/');

    /// <summary>The registrations whose Web.config directory contains the given file (config inheritance).</summary>
    public IEnumerable<ConfigTagRegistration> RegistrationsFor(string filePath)
    {
        var directory = Path.GetDirectoryName(RelativePath(filePath))?.Replace('\\', '/') ?? string.Empty;
        foreach (var registration in ConfigTagRegistrations)
        {
            if (registration.ScopeDirectory.Length == 0
                || directory.Equals(registration.ScopeDirectory, StringComparison.OrdinalIgnoreCase)
                || directory.StartsWith(registration.ScopeDirectory + "/", StringComparison.OrdinalIgnoreCase))
            {
                yield return registration;
            }
        }
    }

    /// <summary>
    /// The .cs files a library directory's project actually compiles, as directory-relative
    /// forward-slash paths. Returns null whenever the project does not enumerate its
    /// sources exhaustively - no single unambiguous .csproj, an SDK-style project (those
    /// glob **/*.cs by default and list only extras), or a wildcard include. In all of
    /// those the directory scan is the right answer and the caller keeps every file.
    /// </summary>
    private static HashSet<string>? ReadCompiledFiles(string directory, string? entryProjectPath = null)
    {
        string project;

        // The app root is where several .csproj commonly sit side by side (one per database
        // for YAF, the app plus its addons for n2cms). That is exactly the case --project
        // already answers, so the caller's answer is used rather than giving up.
        if (entryProjectPath is not null
            && Path.GetDirectoryName(Path.GetFullPath(entryProjectPath)) == directory.TrimEnd(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
        {
            project = Path.GetFullPath(entryProjectPath);
        }
        else
        {
            var projects = Directory.GetFiles(directory, "*.csproj", SearchOption.TopDirectoryOnly);
            if (projects.Length != 1)
            {
                return null;
            }
            project = projects[0];
        }

        var text = File.ReadAllText(project);
        if (Regex.IsMatch(text, @"<Project[^>]*\sSdk\s*=", RegexOptions.IgnoreCase))
        {
            return null;
        }

        var compiled = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match match in Regex.Matches(text, @"<Compile\s+Include\s*=\s*""(?<path>[^""]+)"""))
        {
            var include = match.Groups["path"].Value.Replace('\\', '/');
            if (include.Contains('*'))
            {
                return null;
            }
            compiled.Add(include);
        }

        return compiled.Count == 0 ? null : compiled;
    }

    /// <summary>
    /// True when the project file proves this file is not part of the application.
    ///
    /// Only two kinds of file are judged, and only on POSITIVE evidence:
    ///
    /// - a plain .cs the project does not compile;
    /// - a markup file (.aspx / .ascx / .master) whose code-behind EXISTS on disk and is
    ///   not compiled. Markup with no code-behind at all is kept, because inline-code pages
    ///   are legitimate and the project file says nothing that could settle it.
    ///
    /// Everything else - images, scripts, config, resources - is kept regardless. Content
    /// items are not a reliable inventory (files get added to a site and deployed without
    /// ever entering the .csproj), so absence there is not evidence of anything.
    /// </summary>
    private static bool IsNotPartOfProject(
        WebFormsProject project, HashSet<string> compiled, string path, string relative, string fileName)
    {
        if (fileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
        {
            // Designer and AssemblyInfo files are filtered by name downstream; judging them
            // here would add them to the "not compiled" report as if they were dropped code.
            return !IsGeneratedOrCodeBehind(fileName) && !compiled.Contains(relative);
        }

        if (fileName.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".master", StringComparison.OrdinalIgnoreCase))
        {
            // Converting the markup while its code-behind is gone is worse than dropping
            // both: the component would keep the @using and control declarations and lose
            // every handler, so it compiles against types that are no longer there.
            return File.Exists(path + ".cs") && !compiled.Contains(relative + ".cs");
        }

        return false;
    }

    public static WebFormsProject Scan(
        string rootDirectory, IEnumerable<string>? includeDirectories = null, string? webConfigOverride = null,
        string? entryProjectPath = null)
    {
        var project = new WebFormsProject { RootDirectory = Path.GetFullPath(rootDirectory) };

        // Repositories often ship the config under another name (recommended.web.config
        // etc.) and rename it at deploy time; --web-config points the converter at it
        if (webConfigOverride is not null && File.Exists(webConfigOverride))
        {
            project.WebConfigPath = Path.GetFullPath(webConfigOverride);
            CollectTagRegistrations(project, project.WebConfigPath, Path.GetFileName(webConfigOverride));
        }

        foreach (var directory in includeDirectories ?? [])
        {
            var fullDirectory = Path.GetFullPath(directory);
            var directoryName = Path.GetFileName(fullDirectory.TrimEnd(Path.DirectorySeparatorChar, '/'));

            // The .csproj, not the directory, decides what the library compiles. Real
            // repositories keep orphan .cs files in the tree (BlogEngine.Core/Profile.cs
            // is not in its csproj) that declare types shadowing the real ones - porting
            // them by directory scan produces ambiguity errors the original never had.
            var compiled = ReadCompiledFiles(fullDirectory);

            foreach (var path in Directory.EnumerateFiles(fullDirectory, "*.cs", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(fullDirectory, path).Replace('\\', '/');
                if (relative.StartsWith("bin/", StringComparison.OrdinalIgnoreCase)
                    || relative.StartsWith("obj/", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("AssemblyInfo.cs", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if (compiled is not null && !compiled.Contains(relative))
                {
                    continue;
                }
                project.IncludedCodeFiles.Add(new IncludedCodeFile(path, $"{directoryName}/{relative}"));
            }
        }
        project.IncludedCodeFiles.Sort((a, b) =>
            string.Compare(a.OutputRelativePath, b.OutputRelativePath, StringComparison.OrdinalIgnoreCase));

        // The same rule the --include walk above has always applied, now applied to the app
        // itself. It was only ever missing here because the app root often has more than one
        // .csproj, which ReadCompiledFiles used to answer "null" to.
        //
        // n2cms ships 190 .cs files its own project does not compile - a whole addon tree
        // (AddonCatalog, Demo, Wiki) left in the repository. Porting them by directory scan
        // pulled in code referencing types that exist nowhere in the source at all
        // (N2.Addons.Wiki.Fragmenters.RegexFragmenter), which cannot be anything but an
        // error: the application never contained that code.
        var rootCompiled = ReadCompiledFiles(project.RootDirectory, entryProjectPath);

        foreach (var path in Directory.EnumerateFiles(project.RootDirectory, "*.*", SearchOption.AllDirectories))
        {
            var relative = project.RelativePath(path);
            if (relative.StartsWith("bin/", StringComparison.OrdinalIgnoreCase)
                || relative.StartsWith("obj/", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var fileName = Path.GetFileName(path);

            if (rootCompiled is not null && IsNotPartOfProject(project, rootCompiled, path, relative, fileName))
            {
                project.NotCompiledFiles.Add(relative);
                continue;
            }

            if (fileName.EndsWith(".master", StringComparison.OrdinalIgnoreCase))
            {
                project.MasterPages.Add(path);
            }
            else if (fileName.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
            {
                project.Pages.Add(path);
            }
            else if (fileName.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase))
            {
                project.UserControls.Add(path);
            }
            else if (fileName.Equals("Web.config", StringComparison.OrdinalIgnoreCase))
            {
                // Only the root Web.config feeds appsettings.json; every Web.config may
                // contribute tag-prefix registrations for its subtree
                if (!relative.Contains('/') && project.WebConfigPath is null)
                {
                    project.WebConfigPath = path;
                }
                CollectTagRegistrations(project, path, relative);
            }
            else if (fileName.EndsWith(".resx", StringComparison.OrdinalIgnoreCase)
                     && relative.Contains("App_GlobalResources/", StringComparison.OrdinalIgnoreCase))
            {
                if (IsCultureSpecificResource(fileName))
                {
                    project.CultureResourceFiles.Add(path);
                }
                else
                {
                    project.GlobalResourceFiles.Add(path);
                }
            }
            else if (fileName.EndsWith(".resx", StringComparison.OrdinalIgnoreCase)
                     && relative.Contains("App_LocalResources/", StringComparison.OrdinalIgnoreCase))
            {
                if (IsCultureSpecificResource(fileName))
                {
                    project.CultureResourceFiles.Add(path);
                }
                else
                {
                    // "Admin/App_LocalResources/Default.aspx.resx" localizes "Admin/Default.aspx"
                    var owner = relative.Replace("App_LocalResources/", string.Empty, StringComparison.OrdinalIgnoreCase);
                    owner = owner[..^".resx".Length];
                    project.LocalResourceFiles[owner] = path;
                }
            }
            else if (fileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) && !IsGeneratedOrCodeBehind(fileName))
            {
                project.PlainCodeFiles.Add(path);
            }
        }

        project.MasterPages.Sort(StringComparer.OrdinalIgnoreCase);
        project.Pages.Sort(StringComparer.OrdinalIgnoreCase);
        project.UserControls.Sort(StringComparer.OrdinalIgnoreCase);
        project.PlainCodeFiles.Sort(StringComparer.OrdinalIgnoreCase);

        return project;
    }

    private static void CollectTagRegistrations(WebFormsProject project, string path, string relative)
    {
        var scope = Path.GetDirectoryName(relative)?.Replace('\\', '/') ?? string.Empty;
        XDocument document;
        try
        {
            document = XDocument.Load(path);
        }
        catch (Exception exception) when (exception is System.Xml.XmlException or IOException)
        {
            return;
        }

        // Descendant search: <system.web> may sit inside a <location> wrapper
        // (mojoPortal-style configs), so fixed-path navigation misses it
        foreach (var add in document.Descendants("pages").Elements("controls").Elements("add"))
        {
            var tagPrefix = add.Attribute("tagPrefix")?.Value;
            if (string.IsNullOrEmpty(tagPrefix))
            {
                continue;
            }
            project.ConfigTagRegistrations.Add(new ConfigTagRegistration(
                scope,
                tagPrefix,
                add.Attribute("tagName")?.Value,
                add.Attribute("src")?.Value,
                add.Attribute("namespace")?.Value,
                add.Attribute("assembly")?.Value));
        }

        foreach (var add in document.Descendants("pages").Elements("namespaces").Elements("add"))
        {
            var ns = add.Attribute("namespace")?.Value;
            if (!string.IsNullOrWhiteSpace(ns) && !project.ConfigNamespaceImports.Contains(ns))
            {
                project.ConfigNamespaceImports.Add(ns.Trim());
            }
        }

        // Only the root config decides this. A subfolder Web.config may narrow the mode for
        // its own folder, but the master pages whose IDs this changes are shared by the
        // whole app - taking a nested value would apply one folder's rule to every page.
        if (scope.Length == 0)
        {
            var mode = document.Descendants("pages")
                .Select(pages => pages.Attribute("clientIDMode")?.Value)
                .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
            if (string.Equals(mode, "AutoID", StringComparison.OrdinalIgnoreCase))
            {
                project.UsesAutoIdClientIds = true;
            }
        }
    }

    /// <summary>"labels.ja.resx" / "labels.pt-BR.resx" carry a culture; "labels.resx" does not.</summary>
    private static bool IsCultureSpecificResource(string fileName)
    {
        var withoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var lastDot = withoutExtension.LastIndexOf('.');
        return lastDot > 0
               && System.Text.RegularExpressions.Regex.IsMatch(
                   withoutExtension[(lastDot + 1)..], @"^[a-z]{2}(-[A-Za-z]{2,4})?$");
    }

    private static bool IsGeneratedOrCodeBehind(string fileName)
        => fileName.EndsWith(".designer.cs", StringComparison.OrdinalIgnoreCase)
           || fileName.EndsWith(".aspx.cs", StringComparison.OrdinalIgnoreCase)
           || fileName.EndsWith(".ascx.cs", StringComparison.OrdinalIgnoreCase)
           || fileName.EndsWith(".master.cs", StringComparison.OrdinalIgnoreCase)
           || fileName.Equals("AssemblyInfo.cs", StringComparison.OrdinalIgnoreCase);
}

/// <summary>A user-control reference resolvable from markup (component name + namespace).</summary>
public sealed record UserControlRef(string ComponentName, string Namespace)
{
    /// <summary>
    /// Public property name -> declared type of the control's code-behind. Razor parses
    /// attribute values of non-string parameters as C# expressions, so the emitter needs
    /// the type to decide how to render a markup value.
    /// </summary>
    public IReadOnlyDictionary<string, string> PropertyTypes { get; init; }
        = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// The class the .ascx inherits, as written in its Control directive
    /// ("mojoPortal.Web.UI.AllowedRolesSetting"), or null when it declares none.
    ///
    /// A user control is usually reached by a src registration, but not always: mojoPortal
    /// registers only "tagPrefix=portal namespace=mojoPortal.Web.UI", writes
    /// &lt;portal:TimeZoneIdSetting&gt; with no Register directive anywhere, and relies on the
    /// class being in that namespace. Without this the tag resolved to nothing even though
    /// the control had been converted.
    /// </summary>
    public string? OriginalTypeName { get; init; }
}

/// <summary>One converted component (.razor + .razor.cs).</summary>
public sealed class ConvertedComponent
{
    public required string ComponentName { get; init; }

    /// <summary>
    /// The class name the code-behind actually declares. Usually differs from
    /// <see cref="ComponentName"/> only in casing (class post -> PostComponent), but the
    /// name-collision suffix makes the two genuinely different, and the rewriter has to
    /// find the class by its ORIGINAL name before it can rename it.
    /// </summary>
    public required string SourceClassName { get; init; }

    public required string OutputDirectory { get; init; }
    public required string TargetNamespace { get; init; }
    public required string RazorContent { get; init; }
    public required List<Emit.ControlField> Fields { get; init; }
    public required CodeBehindKind Kind { get; init; }
    public string? CodeBehindSourcePath { get; init; }

    /// <summary>
    /// The .aspx / .ascx / .master this component came from.
    ///
    /// Needed because a file using &lt;script runat="server"&gt; has NO separate
    /// code-behind, so anything keyed on <see cref="CodeBehindSourcePath"/> silently
    /// misses it - which is how the AI layer came to discard residuals for files it had
    /// converted perfectly well.
    /// </summary>
    public string? MarkupSourcePath { get; init; }

    /// <summary>
    /// The directive's AutoEventWireup, which decides whether Page_Load is wired to the
    /// Load event AT ALL.
    ///
    /// Default true, as WebForms defaults it. When the directive says false the page hooks
    /// its own lifecycle by hand ("this.Load += new EventHandler(Page_Load);" in OnInit)
    /// and the runtime wires nothing - so calling Page_Load from the generated driver AND
    /// raising the Load event the page subscribed to would run the handler twice.
    /// mojoPortal writes AutoEventWireup="false" on essentially every page.
    /// </summary>
    public bool AutoEventWireup { get; init; } = true;

    /// <summary>
    /// Bodies of &lt;script runat="server"&gt; blocks from the markup. Appended to the
    /// generated partial class - it is code-behind, only written inline.
    /// </summary>
    public List<string> ServerScriptBlocks { get; init; } = [];

    /// <summary>
    /// Exactly what the razor emitted as @inherits, so the code-behind half of the partial
    /// class can be made to say the same thing.
    ///
    /// C# requires every partial declaration to name the SAME base, textually resolvable to
    /// the same type. The two halves are written from different places - the razor resolves
    /// and fully qualifies the base, the code-behind keeps it as the source wrote it - and
    /// a generic base makes them disagree: "ContentUserControl&lt;ContentItem, RegisterItem&gt;"
    /// on one side, "...&lt;N2.ContentItem, Demo.Items.RegisterItem&gt;" on the other. Same type,
    /// different text, CS0263.
    /// </summary>
    public string? RazorInheritsBase { get; set; }

    public string FullName => $"{TargetNamespace}.{ComponentName}";

    /// <summary>@page routes when this is a page (the first one is the primary route).</summary>
    public List<string> Routes { get; init; } = [];

    /// <summary>Control list for smoke-scenario auto-generation.</summary>
    public List<Emit.SmokeControl> SmokeControls { get; init; } = [];

    /// <summary>Namespaces of the user controls this component references (for code-behind usings).</summary>
    public List<string> UsedControlNamespaces { get; init; } = [];

    /// <summary>Placeholder components generated for unmapped controls (tag -> stub name).</summary>
    public Dictionary<string, string> StubComponents { get; init; } = new(StringComparer.OrdinalIgnoreCase);

}

public enum CodeBehindKind
{
    Page,
    UserControl,
    Layout,
}

/// <summary>Master page analysis result (needed to convert the page-side &lt;asp:Content&gt;).</summary>
public sealed class MasterInfo
{
    public required string ComponentName { get; init; }
    public required string Namespace { get; init; }
    public required string FilePath { get; init; }
    public required HashSet<string> HeadPlaceholders { get; init; }
    public required List<string> BodyPlaceholders { get; init; }

    /// <summary>
    /// Placeholders that carry default content (rendered by WebForms when the child
    /// supplies no &lt;asp:Content&gt; for them). Those must become sections rather than
    /// @Body, because only a section can be given a default that the child overrides.
    /// </summary>
    public HashSet<string> PlaceholdersWithDefaultContent { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Placeholder ID -> the ClientID prefix controls rendered through it receive
    /// ("cphMain_cphSide_"). A ContentPlaceHolder is a naming container, and with nested
    /// masters the chain spans files, so it is resolved once here and used by both the
    /// master that owns the placeholder and every page that fills it.
    /// </summary>
    public Dictionary<string, string> PlaceholderPrefixes { get; } = new(StringComparer.OrdinalIgnoreCase);

    public string? Title { get; init; }

    public string FullName => $"{Namespace}.{ComponentName}";

    /// <summary>
    /// The placeholder assigned to @Body. Placeholders carrying default content are
    /// passed over: @Body has no way to express "render this unless the page overrides
    /// it", while a section does (the layout registers the default, the page wins).
    /// </summary>
    public string? PrimaryPlaceholder
        => BodyPlaceholders.FirstOrDefault(id => !PlaceholdersWithDefaultContent.Contains(id))
           ?? BodyPlaceholders.FirstOrDefault();
}
