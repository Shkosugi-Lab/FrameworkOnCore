using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WebForm2Blazor.Converter.Emit;
using WebForm2Blazor.Converter.Parsing;
using WebForm2Blazor.Converter.Project;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>Converts .master / .aspx / .ascx files into Razor components.</summary>
public static partial class AspxConverters
{
    private static readonly HashSet<string> KnownWebFormsBases = new(StringComparer.Ordinal)
    {
        "Page", "MasterPage", "UserControl",
    };

    public static (ConvertedComponent Component, MasterInfo Info) ConvertMaster(
        string path,
        WebFormsProject project,
        IReadOnlyDictionary<string, UserControlRef> userControlRegistry,
        IReadOnlyDictionary<string, MasterInfo> masters,
        BaseClassRegistry baseRegistry,
        string appNamespace,
        ConversionReport report)
    {
        var sourceName = project.RelativePath(path);
        var source = File.ReadAllText(path);
        var parsed = ParseWithPrefixes(source, path, project);

        var (componentName, sourceClassName) = ResolveNames(parsed, path, baseRegistry);
        var (outputDirectory, targetNamespace) = ResolveTarget(appNamespace, "Components/Layout", sourceName);

        // A master may itself sit under another master (nested master pages). Such a file
        // has no <html>/<head>/<form> at all - its whole content lives inside <asp:Content>
        // elements filling the parent's placeholders, while re-exposing placeholders of its
        // own. Blazor models this directly: a layout component may carry @layout as well.
        var parent = ResolveParentMaster(parsed.MainDirective?.Get("MasterPageFile"), path, project, masters, report, sourceName);

        HashSet<string> headPlaceholders;
        List<AspxNode> bodyNodes;
        List<ElementNode> headControls = [];

        if (parent is not null)
        {
            bodyNodes = parsed.Nodes;
            headPlaceholders = CollectNestedHeadPlaceholders(bodyNodes, parent);
        }
        else
        {
            var headElement = FindServerHtmlElement(parsed, "head");
            headPlaceholders = headElement is null
                ? []
                : headElement.Descendants().Where(IsContentPlaceHolder)
                    .Select(element => element.Id ?? string.Empty)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Server controls the head holds beyond its placeholders (a PlaceHolder the
            // code-behind fills with <link>/<meta>) were dropped with the rest of the head,
            // which left the code-behind referencing a field that was never generated.
            // Blazor renders head markup through <HeadContent>, so they are emitted there.
            headControls = headElement?.Children
                .OfType<ElementNode>()
                .Where(child => !string.IsNullOrEmpty(child.Prefix) && !IsContentPlaceHolder(child))
                .ToList() ?? [];

            var formElement = FindServerHtmlElement(parsed, "form");
            bodyNodes = formElement?.Children ?? parsed.Nodes;
            if (formElement is null)
            {
                report.Residual(sourceName, ResidualKind.Structure,
                    "<form runat=\"server\"> が見つかりません。ファイル全体をレイアウト本文として扱いました。",
                    disposition: ResidualDisposition.Informational);
            }
        }

        var bodyPlaceholders = CollectContentPlaceHolders(bodyNodes)
            .Where(id => !headPlaceholders.Contains(id))
            .ToList();

        var info = new MasterInfo
        {
            ComponentName = componentName,
            Namespace = targetNamespace,
            FilePath = path,
            HeadPlaceholders = headPlaceholders,
            BodyPlaceholders = bodyPlaceholders,
            PlaceholdersWithDefaultContent = CollectPlaceholdersWithDefaultContent(bodyNodes),
            Title = ExtractTitle(source),
        };

        var context = new EmitContext
        {
            Report = report,
            SourceName = sourceName,
            UserControlTags = BuildUserControlTags(parsed, path, project, userControlRegistry, report),
            StubNamespace = $"{appNamespace}.Components.Stubs",
            LocalResources = LoadLocalResources(project, sourceName, report),
        };

        var prefixNamespaces = BuildPrefixNamespaces(parsed, path, project);
        context.LegacyControlResolver = (prefix, name) => ResolveLegacyControl(prefixNamespaces, baseRegistry, prefix, name);

        // The naming-container chain this master's own placeholders sit under. While the
        // emitter is inside an <asp:Content>, it is the parent's prefix for that slot.
        var basePrefix = string.Empty;

        context.SpecialElementHandler = (element, emitter) =>
        {
            // A nested master fills its parent's placeholders the same way a page does
            if (IsContent(element))
            {
                if (parent is null)
                {
                    return null;
                }

                var target = element.Attributes.GetValueOrDefault("ContentPlaceHolderID") ?? string.Empty;
                var previous = basePrefix;
                basePrefix = parent.PlaceholderPrefixes.GetValueOrDefault(target, string.Empty);
                try
                {
                    return EmitContentForMaster(element, emitter, parent, report, sourceName);
                }
                finally
                {
                    basePrefix = previous;
                }
            }

            if (!IsContentPlaceHolder(element))
            {
                return null;
            }

            var placeholderId = element.Id ?? string.Empty;

            // A ContentPlaceHolder is a naming container: its ID joins the ClientID of
            // everything rendered through it, so the position is wrapped on both routes
            var prefix = basePrefix + placeholderId + "_";
            if (!string.IsNullOrEmpty(placeholderId))
            {
                info.PlaceholderPrefixes[placeholderId] = prefix;
            }

            string InNamingContainer(string inner)
                => string.IsNullOrEmpty(placeholderId)
                    ? inner
                    : $"<WebFormsNamingContainer Prefix=\"{prefix}\">{inner}</WebFormsNamingContainer>";

            if (string.Equals(placeholderId, info.PrimaryPlaceholder, StringComparison.OrdinalIgnoreCase))
            {
                report.Info(sourceName, $"<asp:ContentPlaceHolder ID=\"{placeholderId}\"> を @Body に変換しました。");
                return InNamingContainer("@Body");
            }

            // The default content is registered as a <SectionContent> of its own: Blazor
            // lets the last registration win, and the layout renders before the page, so
            // a page that supplies this section overrides the default exactly as WebForms
            // does - and a page that does not supply it still gets the default
            var outlet = $"<SectionOutlet SectionName=\"{placeholderId}\" />";
            if (!info.PlaceholdersWithDefaultContent.Contains(placeholderId))
            {
                report.Info(sourceName,
                    $"<asp:ContentPlaceHolder ID=\"{placeholderId}\"> を <SectionOutlet> に変換しました(@Body 以外の差し込み口)。");
                return InNamingContainer(outlet);
            }

            report.Info(sourceName,
                $"<asp:ContentPlaceHolder ID=\"{placeholderId}\"> を <SectionOutlet> に変換し、"
                + "既定コンテンツを <SectionContent> として登録しました(ページ側が差し込めば上書きされます)。");
            return InNamingContainer(outlet)
                + $"<SectionContent SectionName=\"{placeholderId}\">"
                + InNamingContainer(emitter.EmitNodes(element.Children))
                + "</SectionContent>";
        };

        MarkupEmitter.CollectDeclaredControlIds(parsed.Nodes, context);
        var emitter = new MarkupEmitter(context);
        var markup = NeutralizeUnbalanced(emitter.EmitNodes(bodyNodes), report, sourceName);

        var codeBehindPath = FindCodeBehind(path);
        var inheritsName = ResolveInheritsBase(
            codeBehindPath, sourceClassName, CodeBehindKind.Layout, baseRegistry, report, sourceName,
            parsed.MainDirective?.Get("Inherits"));

        var razor = new StringBuilder();
        if (parent is not null)
        {
            // Blazor nests layouts natively: this layout renders inside its parent's @Body
            razor.AppendLine($"@layout {parent.FullName}");
        }
        razor.AppendLine($"@namespace {targetNamespace}");
        razor.AppendLine($"@inherits {inheritsName}");
        AppendControlUsings(razor, context, parsed, targetNamespace, codeBehindPath, baseRegistry, project);
        razor.AppendLine();
        if (headControls.Count > 0)
        {
            report.Info(sourceName,
                $"<head runat=\"server\"> 内のサーバーコントロール {headControls.Count} 個を <HeadContent> に出力しました。");
            razor.AppendLine("<HeadContent>");
            razor.AppendLine(Trim(emitter.EmitNodes([.. headControls])));
            razor.AppendLine("</HeadContent>");
        }
        razor.AppendLine("<WebFormsScope Owner=\"this\">");
        razor.AppendLine(Trim(markup));
        razor.AppendLine("</WebFormsScope>");

        var component = new ConvertedComponent
        {
            ComponentName = componentName,
            SourceClassName = sourceClassName,
            RazorInheritsBase = inheritsName,
            OutputDirectory = outputDirectory,
            TargetNamespace = targetNamespace,
            RazorContent = razor.ToString(),
            Fields = context.Fields,
            ServerScriptBlocks = context.ServerScriptBlocks,
            Kind = CodeBehindKind.Layout,
            CodeBehindSourcePath = codeBehindPath,
            MarkupSourcePath = path,
            UsedControlNamespaces = [.. context.UsedControlNamespaces],
            StubComponents = context.StubComponents,
        };

        report.ConvertedMasters++;
        report.Info(sourceName, $"マスターページ → {outputDirectory}/{componentName}.razor(レイアウト)");

        return (component, info);
    }

    public static ConvertedComponent ConvertUserControl(
        string path,
        WebFormsProject project,
        IReadOnlyDictionary<string, UserControlRef> userControlRegistry,
        BaseClassRegistry baseRegistry,
        string appNamespace,
        ConversionReport report)
    {
        var sourceName = project.RelativePath(path);
        var source = File.ReadAllText(path);
        var parsed = ParseWithPrefixes(source, path, project);
        var (componentName, sourceClassName) = ResolveNames(parsed, path, baseRegistry);
        var (outputDirectory, targetNamespace) = ResolveTarget(appNamespace, "Components/Controls", sourceName);

        var context = new EmitContext
        {
            Report = report,
            SourceName = sourceName,
            UserControlTags = BuildUserControlTags(parsed, path, project, userControlRegistry, report),
            StubNamespace = $"{appNamespace}.Components.Stubs",
            LocalResources = LoadLocalResources(project, sourceName, report),
        };

        var prefixNamespaces = BuildPrefixNamespaces(parsed, path, project);
        context.LegacyControlResolver = (prefix, name) => ResolveLegacyControl(prefixNamespaces, baseRegistry, prefix, name);

        MarkupEmitter.CollectDeclaredControlIds(parsed.Nodes, context);
        var markup = NeutralizeUnbalanced(
            new MarkupEmitter(context).EmitNodes(parsed.Nodes), report, sourceName);

        var codeBehindPath = FindCodeBehind(path);
        var inheritsName = ResolveInheritsBase(
            codeBehindPath, sourceClassName, CodeBehindKind.UserControl, baseRegistry, report, sourceName,
            parsed.MainDirective?.Get("Inherits"));

        var razor = new StringBuilder();
        razor.AppendLine($"@namespace {targetNamespace}");
        razor.AppendLine($"@inherits {inheritsName}");
        AppendControlUsings(razor, context, parsed, targetNamespace, codeBehindPath, baseRegistry, project);
        razor.AppendLine();
        // A user control with Visible=false renders nothing in WebForms, and code-behind
        // toggles it constantly (pnlX.Visible = ...), so the guard belongs in the markup
        razor.AppendLine("@if (Visible)");
        razor.AppendLine("{");
        razor.AppendLine("<WebFormsScope Owner=\"this\">");
        razor.AppendLine(Trim(markup));
        razor.AppendLine("</WebFormsScope>");
        razor.AppendLine("}");

        report.ConvertedUserControls++;
        report.Info(sourceName, $"ユーザーコントロール → {outputDirectory}/{componentName}.razor");

        return new ConvertedComponent
        {
            ComponentName = componentName,
            SourceClassName = sourceClassName,
            RazorInheritsBase = inheritsName,
            OutputDirectory = outputDirectory,
            TargetNamespace = targetNamespace,
            RazorContent = razor.ToString(),
            Fields = context.Fields,
            ServerScriptBlocks = context.ServerScriptBlocks,
            Kind = CodeBehindKind.UserControl,
            CodeBehindSourcePath = codeBehindPath,
            MarkupSourcePath = path,
            UsedControlNamespaces = [.. context.UsedControlNamespaces],
            StubComponents = context.StubComponents,
        };
    }

    public static ConvertedComponent ConvertPage(
        string path,
        WebFormsProject project,
        IReadOnlyDictionary<string, UserControlRef> userControlRegistry,
        IReadOnlyDictionary<string, MasterInfo> masters,
        BaseClassRegistry baseRegistry,
        string appNamespace,
        ConversionReport report)
    {
        var sourceName = project.RelativePath(path);
        var source = File.ReadAllText(path);
        var parsed = ParseWithPrefixes(source, path, project);
        var (componentName, sourceClassName) = ResolveNames(parsed, path, baseRegistry);
        var (outputDirectory, targetNamespace) = ResolveTarget(appNamespace, "Components/Pages", sourceName);

        var directive = parsed.MainDirective;
        var master = ResolveParentMaster(directive?.Get("MasterPageFile"), path, project, masters, report, sourceName);

        var context = new EmitContext
        {
            Report = report,
            SourceName = sourceName,
            UserControlTags = BuildUserControlTags(parsed, path, project, userControlRegistry, report),
            StubNamespace = $"{appNamespace}.Components.Stubs",
            LocalResources = LoadLocalResources(project, sourceName, report),
        };

        var prefixNamespaces = BuildPrefixNamespaces(parsed, path, project);
        context.LegacyControlResolver = (prefix, name) => ResolveLegacyControl(prefixNamespaces, baseRegistry, prefix, name);

        context.SpecialElementHandler = (element, emitter) =>
        {
            if (!IsContent(element))
            {
                return null;
            }

            return master is null
                ? emitter.EmitNodes(element.Children)
                : EmitContentForMaster(element, emitter, master, report, sourceName);
        };

        MarkupEmitter.CollectDeclaredControlIds(parsed.Nodes, context);
        var emitter = new MarkupEmitter(context);

        List<AspxNode> contentNodes;
        if (master is not null)
        {
            contentNodes = parsed.Nodes;
        }
        else
        {
            var formElement = FindServerHtmlElement(parsed, "form");
            if (formElement is null)
            {
                report.Residual(sourceName, ResidualKind.Structure,
                    "<form runat=\"server\"> もマスターページも見つかりません。ファイル全体を本文として扱いました。",
                    disposition: ResidualDisposition.Informational);
                contentNodes = parsed.Nodes;
            }
            else
            {
                contentNodes = formElement.Children;
            }
        }

        var markup = NeutralizeUnbalanced(emitter.EmitNodes(contentNodes), report, sourceName);

        var codeBehindPath = FindCodeBehind(path);
        var inheritsName = ResolveInheritsBase(
            codeBehindPath, sourceClassName, CodeBehindKind.Page, baseRegistry, report, sourceName,
            directive?.Get("Inherits"));

        var razor = new StringBuilder();
        foreach (var route in BuildRoutes(project.RelativePath(path)))
        {
            razor.AppendLine($"@page \"{route}\"");
        }
        if (master is not null)
        {
            razor.AppendLine($"@layout {master.FullName}");
        }
        razor.AppendLine($"@namespace {targetNamespace}");
        razor.AppendLine($"@inherits {inheritsName}");
        AppendControlUsings(razor, context, parsed, targetNamespace, codeBehindPath, baseRegistry, project);
        razor.AppendLine();

        // Title: the @Page Title attribute wins. A page without a master page has its
        // own <head><title>, so fall back to that as well.
        //
        // A page WITH a master and no Title of its own falls back to the master's, which
        // is what WebForms showed. Emitting nothing was not "no opinion": Blazor's
        // HeadOutlet only changes the title when a PageTitle renders, so such a page KEPT
        // THE PREVIOUS PAGE'S title. WingtipToys' error page read "Welcome" or "" by
        // whichever page the visitor came from.
        var pageTitle = directive?.Get("Title");
        var titleTemplate = master is null ? ExtractTitle(source) : master.Title;

        // The master composes the title around the page's: WingtipToys' is
        // "<%: Page.Title %> - Wingtip Toys", and the original shows
        // "Welcome - Wingtip Toys" on a page whose directive says Title="Welcome".
        // Taking the page's Title alone loses the site name; taking the master's alone
        // loses the page name. The page's value goes where Page.Title stood, and any
        // other expression stays dynamic.
        var title = titleTemplate is not null
                    && titleTemplate.Contains("<%", StringComparison.Ordinal)
            ? PageTitleReferenceRegex().Replace(titleTemplate, pageTitle ?? string.Empty)
            : pageTitle ?? titleTemplate;
        if (!string.IsNullOrWhiteSpace(title))
        {
            if (ConvertTitleExpressions(title) is { } renderable)
            {
                razor.AppendLine($"<PageTitle>{renderable}</PageTitle>");
                razor.AppendLine();
            }
            else
            {
                // A title built by something other than a plain expression - a code block,
                // a call the page has to make - cannot be emitted literally.
                report.Residual(sourceName, ResidualKind.InlineCode,
                    $"動的なページタイトルは変換できません: {title}");
            }
        }

        razor.AppendLine("<WebFormsScope Owner=\"this\">");
        razor.AppendLine(Trim(markup));
        razor.AppendLine("</WebFormsScope>");

        report.ConvertedPages++;
        report.Info(sourceName, $"ページ → {outputDirectory}/{componentName}.razor");

        return new ConvertedComponent
        {
            ComponentName = componentName,
            SourceClassName = sourceClassName,
            RazorInheritsBase = inheritsName,
            OutputDirectory = outputDirectory,
            TargetNamespace = targetNamespace,
            RazorContent = razor.ToString(),
            Fields = context.Fields,
            ServerScriptBlocks = context.ServerScriptBlocks,
            Kind = CodeBehindKind.Page,
            CodeBehindSourcePath = codeBehindPath,
            MarkupSourcePath = path,
            Routes = BuildRoutes(project.RelativePath(path)),
            SmokeControls = context.SmokeControls,
            UsedControlNamespaces = [.. context.UsedControlNamespaces],
            StubComponents = context.StubComponents,
        };
    }

    /// <summary>Makes .ascx files resolvable by "normalized relative path -> component + namespace".</summary>
    public static Dictionary<string, UserControlRef> BuildUserControlRegistry(
        WebFormsProject project, string appNamespace, BaseClassRegistry? baseRegistry = null)
    {
        var registry = new Dictionary<string, UserControlRef>(StringComparer.OrdinalIgnoreCase);
        foreach (var path in project.UserControls)
        {
            var relative = project.RelativePath(path);
            var parsed = ParseWithPrefixes(File.ReadAllText(path), path, project);
            var (_, targetNamespace) = ResolveTarget(appNamespace, "Components/Controls", relative);
            registry[relative] = new UserControlRef(ResolveComponentName(parsed, path, baseRegistry), targetNamespace)
            {
                PropertyTypes = CollectPublicPropertyTypes(FindCodeBehind(path)),
            };
        }
        return registry;
    }

    /// <summary>
    /// Public property name -> declared type from a user control's code-behind. The
    /// emitter uses it to decide how to render markup attribute values (Razor parses the
    /// value of a non-string parameter as a C# expression).
    /// </summary>
    private static IReadOnlyDictionary<string, string> CollectPublicPropertyTypes(string? codeBehindPath)
    {
        var types = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (codeBehindPath is null || !File.Exists(codeBehindPath))
        {
            return types;
        }

        foreach (var property in CodeBehindRewriter.ParseUnit(File.ReadAllText(codeBehindPath))
                     .DescendantNodes().OfType<PropertyDeclarationSyntax>())
        {
            if (property.Modifiers.Any(modifier => modifier.ValueText == "public"))
            {
                types[property.Identifier.Text] = property.Type.ToString();
            }
        }
        return types;
    }

    public static string NormalizeProjectPath(string reference, string referencingFile, WebFormsProject project)
    {
        var trimmed = reference.Trim();
        if (trimmed.StartsWith("~/", StringComparison.Ordinal))
        {
            return trimmed[2..].Replace('\\', '/');
        }
        if (trimmed.StartsWith('/'))
        {
            return trimmed[1..].Replace('\\', '/');
        }

        var directory = Path.GetDirectoryName(referencingFile)!;
        return project.RelativePath(Path.GetFullPath(Path.Combine(directory, trimmed)));
    }

    /// <summary>
    /// Maps the source folder onto the output ("Components/Pages/Administration/Modules")
    /// and onto a matching namespace, so files with the same class name in different
    /// folders no longer collide. The explicit @namespace directive keeps the .razor and
    /// .razor.cs partials in the same namespace regardless of folder-name munging.
    /// </summary>
    private static (string OutputDirectory, string Namespace) ResolveTarget(
        string appNamespace, string baseFolder, string relativePath)
    {
        var relativeDir = Path.GetDirectoryName(relativePath)?.Replace('\\', '/') ?? string.Empty;

        var outputDirectory = string.IsNullOrEmpty(relativeDir) ? baseFolder : $"{baseFolder}/{relativeDir}";

        var ns = new StringBuilder($"{appNamespace}.{baseFolder.Replace('/', '.')}");
        foreach (var segment in relativeDir.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            ns.Append('.').Append(EscapeShadowingSegment(SanitizeIdentifier(segment)));
        }

        return (outputDirectory, ns.ToString());
    }

    /// <summary>
    /// Emits @using lines: the user-control namespaces referenced by this file plus the
    /// file's own &lt;%@ Import Namespace="..." %&gt; directives (markup expressions
    /// depend on those imports, so dropping them breaks the converted @() code).
    /// </summary>
    private static void AppendControlUsings(StringBuilder razor, EmitContext context, ParsedAspx parsed, string ownNamespace,
        string? codeBehindPath = null, BaseClassRegistry? baseRegistry = null, WebFormsProject? project = null)
    {
        var namespaces = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var ns in context.UsedControlNamespaces)
        {
            namespaces.Add(ns);
        }

        // Web.config <pages><namespaces> imports apply to every page / control in
        // WebForms; markup helper types (DateTimeHelper etc.) resolve through them.
        // Only namespaces that survive in ported code are imported.
        foreach (var ns in project?.ConfigNamespaceImports ?? [])
        {
            if (baseRegistry?.HasNamespace(ns) == true)
            {
                namespaces.Add(ns);
            }
        }
        foreach (var directive in parsed.Directives.Where(d => d.Name.Equals("Import", StringComparison.OrdinalIgnoreCase)))
        {
            var ns = directive.Get("Namespace")?.Trim();
            // An import naming a namespace that no ported code declares is dead: the types
            // behind it are not in the output either, and Razor fails on the import itself,
            // taking down markup that would otherwise compile
            if (!string.IsNullOrWhiteSpace(ns) && SurvivesPorting(ns, baseRegistry))
            {
                namespaces.Add(ns);
            }
        }

        // Markup expressions (<%= SomeHelper.X %>) resolve through the code-behind's
        // usings in WebForms; mirror the ones whose namespaces survive in ported code
        if (codeBehindPath is not null && baseRegistry is not null)
        {
            foreach (Match usingMatch in Regex.Matches(File.ReadAllText(codeBehindPath),
                @"^\s*using\s+([A-Za-z_][\w.]*)\s*;", RegexOptions.Multiline))
            {
                var ns = usingMatch.Groups[1].Value;
                if (baseRegistry.HasNamespace(ns))
                {
                    namespaces.Add(ns);
                }
            }
        }

        namespaces.Remove(ownNamespace);

        foreach (var ns in namespaces)
        {
            razor.AppendLine($"@using {ns}");
        }
    }

    /// <summary>
    /// Whether a namespace still exists after porting. Framework namespaces come from
    /// assembly references rather than ported files, so they always do.
    /// </summary>
    private static bool SurvivesPorting(string ns, BaseClassRegistry? baseRegistry)
    {
        if (PortabilityRules.IsFrameworkOnly(ns))
        {
            return false;
        }
        return baseRegistry is null
               || ns.Equals("System", StringComparison.Ordinal)
               || ns.StartsWith("System.", StringComparison.Ordinal)
               || ns.StartsWith("Microsoft.", StringComparison.Ordinal)
               || baseRegistry.HasNamespace(ns);
    }

    /// <summary>
    /// Determines what the razor @inherits (= the partial class's base) should be.
    /// Code-behind classes deriving from a custom base defined in the project (e.g.
    /// BaseNopFrontendPage) keep it; the custom base's own chain root is rewritten to the
    /// compatibility base by CodeBehindRewriter.RewritePlainCodeFile.
    /// </summary>
    private static string ResolveInheritsBase(
        string? codeBehindPath,
        string sourceClassName,
        CodeBehindKind kind,
        BaseClassRegistry baseRegistry,
        ConversionReport report,
        string sourceName,
        string? inheritsDirective = null)
    {
        var fallback = kind switch
        {
            CodeBehindKind.Layout => "WebFormsLayout",
            CodeBehindKind.UserControl => "WebFormsUserControl",
            _ => "WebFormsPage",
        };

        if (codeBehindPath is null)
        {
            // No .aspx.cs beside the markup, but Inherits may still name a class that
            // lives in ordinary source (the App_Code convention). That class IS the
            // code-behind, and ignoring it drops every member the markup uses.
            if (!string.IsNullOrWhiteSpace(inheritsDirective)
                && !KnownWebFormsBases.Contains(LastSegment(inheritsDirective))
                && baseRegistry.TryResolve(inheritsDirective, out var inherited))
            {
                report.Info(sourceName,
                    $"Inherits=\"{inheritsDirective}\" のクラスを基底として使用しました"
                    + "(.ascx.cs ではなく通常のソースに置かれたコードビハインド)。");
                return inherited.FullName;
            }

            return fallback;
        }

        var root = CodeBehindRewriter.ParseUnit(File.ReadAllText(codeBehindPath));
        // Matched against the name the SOURCE declares. Case-insensitively that is the
        // component name for a lowercase WebForms class (class root : Page), but not when
        // the name-collision suffix applied (class post -> component PostComponent) -
        // missing the class there silently emitted the fallback base into @inherits while
        // the code-behind kept its real one, and the partial halves disagreed.
        var classDeclaration = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault(candidate => candidate.Identifier.Text.Equals(
                sourceClassName, StringComparison.OrdinalIgnoreCase));
        var baseName = classDeclaration?.BaseList?.Types.FirstOrDefault()?.Type.ToString();

        if (baseName is null || KnownWebFormsBases.Contains(LastSegment(baseName))
            || baseName.StartsWith("System.Web.", StringComparison.Ordinal)
            || baseName.StartsWith("global::System.Web.", StringComparison.Ordinal))
        {
            return fallback;
        }

        if (baseRegistry.TryResolve(baseName, out var entry))
        {
            report.Info(sourceName,
                $"独自基底クラス {entry.ClassName} を維持しました(基底連鎖の根本は互換基底クラスへ差し替え)。");
            // The registry resolves a NAME; the type arguments live only in the base list as
            // written. Emitting just the name turns "TemplateMasterPage<ContentItem>" into
            // an open generic and the component fails with CS0305. The arguments carry over
            // as written and are qualified where the registry knows them, because the razor
            // file no longer sits in the namespace that made the short name resolve.
            return entry.FullName + QualifyTypeArguments(baseName, baseRegistry);
        }

        report.Residual(sourceName, ResidualKind.CodeBehind,
            $"基底クラス {baseName} がプロジェクト内に見つかりません(外部アセンブリ由来の可能性)。@inherits はそのまま出力しました。",
            disposition: ResidualDisposition.NeedsInput);
        return baseName;
    }

    private static string LastSegment(string typeName)
    {
        var lastDot = typeName.LastIndexOf('.');
        return lastDot >= 0 ? typeName[(lastDot + 1)..] : typeName;
    }

    /// <summary>
    /// The type-argument list of a base type as written ("&lt;A, B&gt;"), with each argument
    /// replaced by its full name where the registry knows it. Empty when non-generic.
    ///
    /// Qualification matters because the argument was written to resolve from the
    /// code-behind's namespace and usings ("Items.Addon" under namespace N2.Templates),
    /// and the generated razor sits in a different namespace entirely.
    /// </summary>
    private static string QualifyTypeArguments(string baseTypeName, BaseClassRegistry baseRegistry)
    {
        var start = baseTypeName.IndexOf('<');
        var end = baseTypeName.LastIndexOf('>');
        if (start < 0 || end < start)
        {
            return string.Empty;
        }

        var arguments = SplitTopLevel(baseTypeName[(start + 1)..end])
            .Select(argument => Qualify(argument.Trim(), baseRegistry));
        return "<" + string.Join(", ", arguments) + ">";
    }

    private static string Qualify(string typeName, BaseClassRegistry baseRegistry)
    {
        if (typeName.Length == 0 || typeName.Contains('<'))
        {
            // Nested generics are left alone: qualifying them needs the same treatment
            // recursively, and no corpus writes one in a page base.
            return typeName;
        }
        var canonical = baseRegistry.ResolveFullTypeName(typeName);
        return canonical ?? typeName;
    }

    /// <summary>Splits a type-argument list on top-level commas only.</summary>
    private static IEnumerable<string> SplitTopLevel(string arguments)
    {
        var depth = 0;
        var start = 0;
        for (var i = 0; i < arguments.Length; i++)
        {
            switch (arguments[i])
            {
                case '<': depth++; break;
                case '>': depth--; break;
                case ',' when depth == 0:
                    yield return arguments[start..i];
                    start = i + 1;
                    break;
            }
        }
        yield return arguments[start..];
    }

    private static Dictionary<string, UserControlRef> BuildUserControlTags(
        ParsedAspx parsed,
        string path,
        WebFormsProject project,
        IReadOnlyDictionary<string, UserControlRef> userControlRegistry,
        ConversionReport report)
    {
        var tags = new Dictionary<string, UserControlRef>(StringComparer.OrdinalIgnoreCase);

        // User controls registered app-wide (or folder-wide) in Web.config <pages><controls>
        foreach (var registration in project.RegistrationsFor(path))
        {
            if (string.IsNullOrEmpty(registration.Src) || string.IsNullOrEmpty(registration.TagName))
            {
                continue;
            }
            var configFile = Path.Combine(project.RootDirectory,
                registration.ScopeDirectory.Replace('/', Path.DirectorySeparatorChar), "Web.config");
            var key = NormalizeProjectPath(registration.Src, configFile, project);
            if (userControlRegistry.TryGetValue(key, out var reference))
            {
                tags[$"{registration.TagPrefix}:{registration.TagName}"] = reference;
            }
        }

        foreach (var directive in parsed.Directives.Where(d => d.Name.Equals("Register", StringComparison.OrdinalIgnoreCase)))
        {
            var tagPrefix = directive.Get("TagPrefix");
            var tagName = directive.Get("TagName");
            var src = directive.Get("Src");

            if (string.IsNullOrEmpty(tagPrefix) || string.IsNullOrEmpty(src))
            {
                // An assembly registration (<%@ Register TagPrefix Assembly Namespace %>)
                // is NOT unsupported: BuildPrefixNamespaces reads its Namespace, and
                // ResolveLegacyControl uses that to resolve the prefix onto ported
                // controls. Reporting it as a residual said the opposite, and said it
                // TWICE - a control that really cannot be resolved is already reported at
                // each use site. DNN's admin/Containers/title.ascx carried one of these
                // plus the five control residuals that describe the same gap.
                //
                // What remains worth saying is which assembly the prefix came from, since
                // that is the thing to name in --package-map when the controls do go
                // missing.
                if (!string.IsNullOrEmpty(tagPrefix))
                {
                    report.Info(project.RelativePath(path),
                        $"<%@ Register TagPrefix=\"{tagPrefix}\" Assembly=\"{directive.Get("Assembly")}\" %> の "
                        + $"Namespace=\"{directive.Get("Namespace")}\" をプレフィックス解決に使用します"
                        + "(移植済みのコントロールは解決され、解決できないものは使用箇所で個別に報告されます)。");
                    continue;
                }

                report.Residual(project.RelativePath(path), ResidualKind.UnmappedControl,
                    $"<%@ Register %> に TagPrefix がありません: Assembly={directive.Get("Assembly")}, Src={src}",
                    disposition: ResidualDisposition.NeedsInput);
                continue;
            }

            var key = NormalizeProjectPath(src, path, project);
            if (!userControlRegistry.TryGetValue(key, out var reference))
            {
                report.Residual(project.RelativePath(path), ResidualKind.UnmappedControl,
                    $"Register ディレクティブの Src=\"{src}\" に対応する .ascx が見つかりません。");
                continue;
            }

            tags[$"{tagPrefix}:{tagName ?? reference.ComponentName}"] = reference;
        }

        return tags;
    }

    /// <summary>
    /// Base-chain roots eligible for LegacyRenderHost - display-oriented control bases.
    /// Interactive families (Button, GridView, TextBox, ...) are excluded: rendering
    /// them statically would look right but be dead, so they stay as honest stubs for
    /// hand-porting via --control-map.
    /// </summary>
    /// <summary>
    /// WebForms base classes whose ported subclasses LegacyRenderHost can render. The name
    /// is the ROOT of the original base chain, so these are System.Web.UI names even though
    /// the ported class has been rewritten onto LegacyWebControl.
    ///
    /// The list was grown one entry at a time and had gaps that cost more than they look:
    /// Button was missing, so mojoPortal's "mojoButton : Button" was reported as an
    /// unmapped control 128 times - 30% of every unmapped control across all six corpora,
    /// for a control whose source was ported and whose tagPrefix was registered.
    /// </summary>
    private static readonly HashSet<string> LegacyRenderableRoots = new(StringComparer.Ordinal)
    {
        "Control", "WebControl", "CompositeControl", "TemplateControl",
        "Panel", "Label", "Literal", "HyperLink", "Image", "PlaceHolder",
        "Button", "LinkButton", "ImageButton", "TextBox", "CheckBox", "RadioButton",
        "DropDownList", "ListBox", "ListControl", "BaseValidator",
        "LegacyWebControl", "LegacyPanel", "LegacyLabel", "LegacyLiteral", "LegacyHyperLink",
    };

    /// <summary>Resolves an unmapped control to a ported legacy class eligible for LegacyRenderHost.</summary>
    private static string? ResolveLegacyControl(
        Dictionary<string, List<string>> prefixNamespaces, BaseClassRegistry baseRegistry, string prefix, string name)
    {
        if (!prefixNamespaces.TryGetValue(prefix, out var namespaces))
        {
            return null;
        }
        string? fullName = null;
        foreach (var ns in namespaces)
        {
            if (baseRegistry.TryGetCanonicalClass($"{ns}.{name}", out var canonical))
            {
                fullName = canonical;
                break;
            }
        }
        if (fullName is null)
        {
            return null;
        }
        var root = baseRegistry.GetRootBaseName(fullName[(fullName.LastIndexOf('.') + 1)..]);
        return root is not null && LegacyRenderableRoots.Contains(root) ? fullName : null;
    }

    /// <summary>
    /// Prefix -> candidate namespaces from assembly/namespace tag registrations
    /// (Web.config &lt;pages&gt;&lt;controls&gt; and per-file &lt;%@ Register Assembly %&gt;).
    /// Used to resolve unmapped custom controls to their ported source classes.
    /// </summary>
    private static Dictionary<string, List<string>> BuildPrefixNamespaces(
        ParsedAspx parsed, string path, WebFormsProject project)
    {
        var map = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        void Add(string? prefix, string? ns)
        {
            if (string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(ns))
            {
                return;
            }
            if (!map.TryGetValue(prefix, out var list))
            {
                list = [];
                map[prefix] = list;
            }
            if (!list.Contains(ns))
            {
                list.Add(ns);
            }
        }

        foreach (var registration in project.RegistrationsFor(path))
        {
            Add(registration.TagPrefix, registration.Namespace);
        }
        foreach (var directive in parsed.Directives.Where(d => d.Name.Equals("Register", StringComparison.OrdinalIgnoreCase)))
        {
            Add(directive.Get("TagPrefix"), directive.Get("Namespace"));
        }
        return map;
    }

    /// <summary>
    /// Collects the prefixes that mark an element as a server control: "asp", the file's
    /// own &lt;%@ Register %&gt; directives, and the Web.config &lt;pages&gt;&lt;controls&gt;
    /// registrations that apply to this file's folder. Without the Web.config prefixes,
    /// controls registered app-wide would silently pass through as plain text.
    /// </summary>
    private static ParsedAspx ParseWithPrefixes(string source, string path, WebFormsProject project)
    {
        var prefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "asp" };

        foreach (var registration in project.RegistrationsFor(path))
        {
            prefixes.Add(registration.TagPrefix);
        }

        foreach (var directive in AspxParser.PreScanDirectives(source))
        {
            if (directive.Name.Equals("Register", StringComparison.OrdinalIgnoreCase))
            {
                var prefix = directive.Get("TagPrefix");
                if (!string.IsNullOrEmpty(prefix))
                {
                    prefixes.Add(prefix);
                }
            }
        }
        return AspxParser.Parse(source, prefixes);
    }

    private static string ResolveComponentName(ParsedAspx parsed, string path, BaseClassRegistry? baseRegistry = null)
        => ResolveNames(parsed, path, baseRegistry).Component;

    /// <summary>
    /// The Razor component name and the class name the code-behind declares. They differ
    /// by more than casing when the collision suffix applies, so both are needed: the
    /// rewriter looks the class up by the source name and renames it to the component name.
    /// </summary>
    private static (string Component, string SourceClass) ResolveNames(
        ParsedAspx parsed, string path, BaseClassRegistry? baseRegistry = null)
    {
        var inherits = parsed.MainDirective?.Get("Inherits");
        var sourceClass = !string.IsNullOrWhiteSpace(inherits)
            ? SanitizeIdentifier(inherits.Split('.').Last())
            // Drop double extensions, e.g. "Site.Master" -> "Site"
            : SanitizeIdentifier(Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path)));

        // Razor component names must start uppercase (the code-behind rewriter renames
        // the partial class to match)
        var name = sourceClass.Length > 0 && char.IsLower(sourceClass[0])
            ? char.ToUpperInvariant(sourceClass[0]) + sourceClass[1..]
            : sourceClass;

        // Uppercasing can collide two ways, and both were legal in WebForms only because
        // C# is case-sensitive:
        //   1. with a member of the same class - "class post { public Post Post; }"
        //   2. with another project type - "class search" next to BlogEngine.Core.Search,
        //      which would otherwise shadow the real type for every page in the generated
        //      namespace, not just this one.
        // Either way the component takes a suffix. Routes come from the file path, so
        // they are unaffected.
        var collides = DeclaresMemberNamed(FindCodeBehind(path), name)
            || (baseRegistry is not null && !string.Equals(name, sourceClass, StringComparison.Ordinal)
                && baseRegistry.DeclaresTypeNamed(name));
        return (collides ? name + "Component" : name, sourceClass);
    }

    /// <summary>
    /// Namespace roots the generated code imports. A folder of the same name (mojoPortal
    /// ships Controls/Microsoft) would turn into a namespace segment that SHADOWS the
    /// root: C# resolves "Microsoft.AspNetCore..." against the enclosing namespace first
    /// and then fails. Suffixing the segment keeps the folder mapping readable and the
    /// import resolvable.
    /// </summary>
    private static readonly HashSet<string> ShadowingSegments = new(StringComparer.Ordinal)
    {
        "System", "Microsoft",
    };

    private static string EscapeShadowingSegment(string segment)
        => ShadowingSegments.Contains(segment) ? segment + "_" : segment;

    private static string SanitizeIdentifier(string value)
    {
        var builder = new StringBuilder();
        foreach (var c in value)
        {
            builder.Append(char.IsLetterOrDigit(c) || c == '_' ? c : '_');
        }
        var result = builder.ToString();
        return result.Length > 0 && char.IsDigit(result[0]) ? "_" + result : result;
    }

    /// <summary>True when the code-behind declares a member with this exact name.</summary>
    private static bool DeclaresMemberNamed(string? codeBehindPath, string name)
    {
        if (codeBehindPath is null || !File.Exists(codeBehindPath))
        {
            return false;
        }

        return CodeBehindRewriter.ParseUnit(File.ReadAllText(codeBehindPath))
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .SelectMany(declaration => declaration.Members)
            .Any(member => member switch
            {
                FieldDeclarationSyntax field =>
                    field.Declaration.Variables.Any(variable => variable.Identifier.Text == name),
                PropertyDeclarationSyntax property => property.Identifier.Text == name,
                MethodDeclarationSyntax method => method.Identifier.Text == name,
                _ => false,
            });
    }

    /// <summary>
    /// The App_LocalResources entries for one page / control, flattened to
    /// "resourceKey.PropertyName" -> value. Empty when the file has no local resources.
    /// </summary>
    private static Dictionary<string, string> LoadLocalResources(
        WebFormsProject project, string sourceName, ConversionReport report)
    {
        var resources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!project.LocalResourceFiles.TryGetValue(sourceName, out var resourcePath))
        {
            return resources;
        }

        try
        {
            foreach (var element in System.Xml.Linq.XDocument.Load(resourcePath).Root?.Elements("data") ?? [])
            {
                var name = element.Attribute("name")?.Value;
                // Implicit localization entries are "key.Property"; typed / binary
                // resources belong to explicit localization and are not applied here
                if (string.IsNullOrEmpty(name) || !name.Contains('.', StringComparison.Ordinal)
                    || element.Attribute("type") is not null
                    || element.Attribute("mimetype") is not null)
                {
                    continue;
                }
                resources[name] = element.Element("value")?.Value ?? string.Empty;
            }
        }
        catch (Exception exception) when (exception is System.Xml.XmlException or IOException)
        {
            report.Residual(sourceName, ResidualKind.Configuration,
                $"App_LocalResources の {Path.GetFileName(resourcePath)} を読み込めませんでした。",
                disposition: ResidualDisposition.NeedsInput);
        }

        return resources;
    }

    private static string? FindCodeBehind(string path)
    {
        var candidate = path + ".cs";
        return File.Exists(candidate) ? candidate : null;
    }

    private static ElementNode? FindServerHtmlElement(ParsedAspx parsed, string tagName)
        => parsed.AllElements.FirstOrDefault(element =>
            string.IsNullOrEmpty(element.Prefix)
            && element.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));

    private static bool IsContentPlaceHolder(ElementNode element)
        => element.Prefix.Equals("asp", StringComparison.OrdinalIgnoreCase)
           && element.Name.Equals("ContentPlaceHolder", StringComparison.OrdinalIgnoreCase);

    private static bool IsContent(ElementNode element)
        => element.Prefix.Equals("asp", StringComparison.OrdinalIgnoreCase)
           && element.Name.Equals("Content", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Resolves the MasterPageFile of a page or of a nested master. Reports a residual
    /// when it cannot be resolved, because the file silently loses its whole shell then.
    /// </summary>
    private static MasterInfo? ResolveParentMaster(
        string? masterPageFile,
        string path,
        WebFormsProject project,
        IReadOnlyDictionary<string, MasterInfo> masters,
        ConversionReport report,
        string sourceName)
    {
        if (string.IsNullOrEmpty(masterPageFile))
        {
            return null;
        }

        var key = NormalizeProjectPath(masterPageFile, path, project);
        if (masters.TryGetValue(key, out var master))
        {
            return master;
        }

        report.Residual(sourceName, ResidualKind.Structure,
            $"MasterPageFile=\"{masterPageFile}\" に対応するマスターページが見つかりません。");
        return null;
    }

    /// <summary>Places an &lt;asp:Content&gt; into the placeholder its master exposes.</summary>
    private static string EmitContentForMaster(
        ElementNode element, MarkupEmitter emitter, MasterInfo master, ConversionReport report, string sourceName)
    {
        var placeholderId = element.Attributes.GetValueOrDefault("ContentPlaceHolderID") ?? string.Empty;
        var inner = emitter.EmitNodes(element.Children);

        if (master.HeadPlaceholders.Contains(placeholderId))
        {
            report.Info(sourceName,
                $"<asp:Content ContentPlaceHolderID=\"{placeholderId}\"> は <head> 用のため <HeadContent> に変換しました。");
            return $"<HeadContent>{inner}</HeadContent>";
        }

        if (string.Equals(placeholderId, master.PrimaryPlaceholder, StringComparison.OrdinalIgnoreCase))
        {
            // Rendered through the master's @Body, which already sits in the container
            return inner;
        }

        // A section is rendered somewhere else entirely, so it carries its naming-container
        // prefix explicitly rather than inheriting one from where it is declared
        if (master.PlaceholderPrefixes.TryGetValue(placeholderId, out var prefix))
        {
            inner = $"<WebFormsNamingContainer Prefix=\"{prefix}\">{inner}</WebFormsNamingContainer>";
        }

        return $"<SectionContent SectionName=\"{placeholderId}\">{inner}</SectionContent>";
    }

    /// <summary>
    /// A nested master has no &lt;head&gt; of its own: the placeholders it re-exposes for
    /// the head are the ones sitting inside an &lt;asp:Content&gt; bound to a head
    /// placeholder of the parent.
    /// </summary>
    private static HashSet<string> CollectNestedHeadPlaceholders(IEnumerable<AspxNode> nodes, MasterInfo parent)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var element in nodes.OfType<ElementNode>())
        {
            if (!IsContent(element))
            {
                continue;
            }

            var target = element.Attributes.GetValueOrDefault("ContentPlaceHolderID") ?? string.Empty;
            if (!parent.HeadPlaceholders.Contains(target))
            {
                continue;
            }

            foreach (var id in CollectContentPlaceHolders(element.Children))
            {
                result.Add(id);
            }
        }

        return result;
    }

    /// <summary>
    /// Placeholders whose body is more than whitespace. WebForms renders that body when
    /// the child page supplies no &lt;asp:Content&gt; for the placeholder, so dropping it
    /// silently removes markup (a whole sidebar, in the corpus that surfaced this).
    /// </summary>
    private static HashSet<string> CollectPlaceholdersWithDefaultContent(IEnumerable<AspxNode> nodes)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void Walk(IEnumerable<AspxNode> current)
        {
            foreach (var element in current.OfType<ElementNode>())
            {
                if (IsContentPlaceHolder(element) && element.Children.Any(HasRenderableContent))
                {
                    result.Add(element.Id ?? string.Empty);
                }
                Walk(element.Children);
            }
        }

        Walk(nodes);
        return result;
    }

    private static bool HasRenderableContent(AspxNode node)
        => node is not TextNode text || !string.IsNullOrWhiteSpace(text.Text);

    /// <summary>
    /// Orders masters so a nested master is converted after its parent: the child needs
    /// the parent's placeholder list to know where each &lt;asp:Content&gt; goes. Masters
    /// whose parent never resolves are emitted last, so they still convert (as standalone
    /// layouts) and report their own residual.
    /// </summary>
    public static List<string> OrderMastersByDependency(WebFormsProject project)
    {
        var parents = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var path in project.MasterPages)
        {
            var directive = ParseWithPrefixes(File.ReadAllText(path), path, project).MainDirective;
            var reference = directive?.Get("MasterPageFile");
            parents[project.RelativePath(path)] = string.IsNullOrEmpty(reference)
                ? null
                : NormalizeProjectPath(reference, path, project);
        }

        var ordered = new List<string>();
        var done = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var pending = new List<string>(project.MasterPages);

        while (pending.Count > 0)
        {
            var ready = pending
                .Where(path =>
                {
                    var parent = parents[project.RelativePath(path)];
                    return parent is null || done.Contains(parent) || !parents.ContainsKey(parent);
                })
                .ToList();

            // A cycle (or a chain we cannot order) must not spin forever
            if (ready.Count == 0)
            {
                ready = pending;
            }

            foreach (var path in ready)
            {
                ordered.Add(path);
                done.Add(project.RelativePath(path));
                pending.Remove(path);
            }
        }

        return ordered;
    }

    private static List<string> CollectContentPlaceHolders(IEnumerable<AspxNode> nodes)
    {
        var result = new List<string>();

        void Walk(IEnumerable<AspxNode> current)
        {
            foreach (var element in current.OfType<ElementNode>())
            {
                if (IsContentPlaceHolder(element))
                {
                    result.Add(element.Id ?? string.Empty);
                }
                Walk(element.Children);
            }
        }

        Walk(nodes);
        return result;
    }

    /// <summary>Builds @page routes from the physical path (Default also gets "/").</summary>
    public static List<string> BuildRoutes(string relativePath)
    {
        var withoutExtension = relativePath[..^Path.GetExtension(relativePath).Length].Replace('\\', '/');
        var routes = new List<string> { "/" + withoutExtension };

        if (Path.GetFileName(withoutExtension).Equals("Default", StringComparison.OrdinalIgnoreCase))
        {
            var directory = Path.GetDirectoryName(withoutExtension)?.Replace('\\', '/');
            routes.Insert(0, string.IsNullOrEmpty(directory) ? "/" : "/" + directory);
        }

        return routes;
    }

    /// <summary>
    /// A &lt;title&gt; with &lt;%: expr %&gt; / &lt;%= expr %&gt; rewritten to Razor, or null
    /// when it holds something that is not a plain expression.
    ///
    /// WingtipToys' master is "&lt;%: Page.Title %&gt; - Wingtip Toys", which is the whole
    /// site's title on every page; treating it as unconvertible left every page with no
    /// PageTitle at all, and Blazor then shows whatever the previous page set.
    ///
    /// Only an expression is accepted - a statement block (&lt;% ... %&gt;) runs somewhere
    /// and cannot be moved into a title - and it must not contain the quote or angle
    /// brackets that would end the attribute it lands in.
    /// </summary>
    private static string? ConvertTitleExpressions(string title)
    {
        if (!title.Contains("<%", StringComparison.Ordinal))
        {
            return title;
        }

        var converted = TitleExpressionRegex().Replace(title, match => "@(" + match.Groups[1].Value.Trim() + ")");
        return converted.Contains("<%", StringComparison.Ordinal) ? null : converted;
    }

    [GeneratedRegex(@"<%[:=]\s*([^<>%""]+?)\s*%>")]
    private static partial Regex TitleExpressionRegex();

    /// <summary>
    /// The master's reference to the page's own title: &lt;%: Page.Title %&gt; and the
    /// spellings around it. Replaced by the page's Title, which is what WebForms put there.
    /// </summary>
    [GeneratedRegex(@"<%[:=]\s*(?:this\.)?(?:Page\.)?Title\s*%>")]
    private static partial Regex PageTitleReferenceRegex();

    private static string? ExtractTitle(string source)
    {
        var match = TitleRegex().Match(source);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    /// <summary>
    /// Last-resort repair for source HTML that never balanced (a &lt;p&gt; left open, a
    /// stray &lt;/div&gt;). WebForms and browsers tolerate it, Razor rejects it outright;
    /// writing only the unmatched tags as raw markup keeps the rendered bytes identical.
    /// Fragments unbalanced for other reasons (a generic inside an @() expression looks
    /// like a tag to the scanner) are left alone rather than reported as a false positive.
    /// </summary>
    private static string NeutralizeUnbalanced(string markup, ConversionReport report, string sourceName)
    {
        if (TagBalance.IsBalanced(markup) || !TagBalance.HasUnbalancedPlainTags(markup))
        {
            return markup;
        }

        report.Info(sourceName,
            "均衡していない HTML タグを raw 出力へ退避しました(元ソースが不正な HTML のため)。");
        return TagBalance.Neutralize(markup);
    }

    private static string Trim(string markup) => markup.Trim('\r', '\n');

    [GeneratedRegex(@"<title>(.*?)</title>", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex TitleRegex();
}
