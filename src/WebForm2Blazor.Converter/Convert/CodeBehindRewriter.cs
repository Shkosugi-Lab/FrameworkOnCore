using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WebForm2Blazor.Converter.Project;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Converts code-behind (.aspx.cs) into a Blazor component partial class (.razor.cs).
/// Working on the Roslyn syntax tree, only usings / namespace / base class / generated
/// members are replaced - method bodies are never touched.
/// </summary>
public static class CodeBehindRewriter
{
    private static readonly HashSet<string> WebFormsBaseTypes = new(StringComparer.Ordinal)
    {
        "Page", "System.Web.UI.Page", "global::System.Web.UI.Page",
        "MasterPage", "System.Web.UI.MasterPage", "global::System.Web.UI.MasterPage",
        "UserControl", "System.Web.UI.UserControl", "global::System.Web.UI.UserControl",
    };

    /// <summary>Lifecycle events that cannot be converted automatically (Init / Load / PreRender are supported).</summary>
    private static readonly HashSet<string> UnsupportedLifecycleMethods = new(StringComparer.Ordinal)
    {
        "Page_PreInit", "Page_InitComplete", "Page_PreLoad", "Page_LoadComplete",
        "Page_PreRenderComplete", "Page_SaveStateComplete", "Page_Unload",
        "Page_Error", "Page_AbortTransaction",
    };

    private static readonly string[] RequiredUsings =
    [
        "System",
        "Microsoft.AspNetCore.Components",
        "WebForm2Blazor.Components",
    ];

    // WebForms projects are conventionally built as Debug during development; parsing
    // without the symbol makes #if DEBUG classes invisible (found via YAF's TestData page)
    private static readonly CSharpParseOptions ParseOptions =
        CSharpParseOptions.Default.WithPreprocessorSymbols("DEBUG");

    /// <summary>Parses a source file with the converter's standard parse options.</summary>
    internal static CompilationUnitSyntax ParseUnit(string source)
        => (CompilationUnitSyntax)CSharpSyntaxTree.ParseText(source, ParseOptions).GetRoot();

    public static string Rewrite(
        string source,
        ConvertedComponent component,
        string sourceName,
        ConversionReport report,
        IEnumerable<string>? additionalUsings = null,
        BaseClassRegistry? baseRegistry = null,
        PortedTypeIndex? portedTypes = null)
        => DeepSyntaxWork.Run(() =>
            RewriteCore(source, component, sourceName, report, additionalUsings, baseRegistry, portedTypes));

    private static string RewriteCore(
        string source,
        ConvertedComponent component,
        string sourceName,
        ConversionReport report,
        IEnumerable<string>? additionalUsings,
        BaseClassRegistry? baseRegistry,
        PortedTypeIndex? portedTypes)
    {
        var root = ParseUnit(source);

        // The namespace rewrite severs same-namespace references to types that stay in
        // the original namespace (business classes in plain code files). C# also resolves
        // short names through the ENCLOSING namespace chain (a class in ...Web.Admin.Modules
        // sees types in ...Web), so every ancestor namespace that still holds ported types
        // is bridged with a using.
        var originalNamespace = root.DescendantNodes()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault()?.Name.ToString();
        var namespaceBridge = new List<string>();
        if (originalNamespace is not null)
        {
            for (var ns = originalNamespace; !string.IsNullOrEmpty(ns);)
            {
                if (ns != component.TargetNamespace && baseRegistry?.HasNamespace(ns) == true)
                {
                    namespaceBridge.Add(ns);
                }
                var lastDot = ns.LastIndexOf('.');
                ns = lastDot < 0 ? null : ns[..lastDot];
            }
        }

        root = RewriteUsings(root,
            [.. RequiredUsings, .. additionalUsings ?? [], .. namespaceBridge,
             .. NestedNamespaceAliases(source, originalNamespace, baseRegistry)]);
        root = RewriteNamespace(root, component.TargetNamespace);

        // The class is looked up by the name the SOURCE declares, not by the component
        // name: they differ by more than casing when the name-collision suffix applied
        // (class post -> component PostComponent), and matching only on the component
        // name silently left the two partial halves in different classes.
        var candidates = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
        var classDeclaration =
            candidates.FirstOrDefault(candidate => candidate.Identifier.Text == component.SourceClassName)
            ?? candidates.FirstOrDefault(candidate => candidate.Identifier.Text == component.ComponentName)
            ?? candidates.FirstOrDefault(candidate => candidate.Identifier.Text.Equals(
                component.SourceClassName, StringComparison.OrdinalIgnoreCase))
            ?? candidates.FirstOrDefault(candidate => candidate.Identifier.Text.Equals(
                component.ComponentName, StringComparison.OrdinalIgnoreCase));

        if (classDeclaration is null)
        {
            report.Error(sourceName, $"コードビハインドに partial class {component.ComponentName} が見つかりません。");
            return RewriteQualifiedFrameworkTypes(RewriteSyntax(root).ToFullString());
        }

        // A .razor always generates "partial class", so the code-behind half has to be
        // partial too. WebForms did not require it - a .master or .aspx with no designer
        // file is an ordinary class, and mojoPortal's Web/App_MasterPages/layout.Master.cs
        // is one - and without this the two halves are two declarations of one name
        // (CS0260).
        if (!classDeclaration.Modifiers.Any(modifier =>
                modifier.RawKind == (int)SyntaxKind.PartialKeyword))
        {
            // The trailing space matters: AddModifiers appends the token with no trivia,
            // and the "class" keyword carries its own leading space only when it follows
            // the modifier list as parsed - so without this it renders "partialclass".
            var madePartial = classDeclaration.AddModifiers(
                SyntaxFactory.Token(SyntaxKind.PartialKeyword)
                    .WithTrailingTrivia(SyntaxFactory.Space));
            root = root.ReplaceNode(classDeclaration, madePartial);
            classDeclaration = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .First(candidate => candidate.Identifier.Text == madePartial.Identifier.Text);
            report.Info(sourceName,
                $"{classDeclaration.Identifier.Text} に partial を付けました"
                + "(.razor 側が partial class を生成するため)。");
        }

        // Component names must start uppercase in Razor; a lowercase WebForms class
        // (class root : Page) is renamed so the partial halves line up
        if (classDeclaration.Identifier.Text != component.ComponentName)
        {
            var renamed = classDeclaration.WithIdentifier(
                SyntaxFactory.Identifier(component.ComponentName)
                    .WithTriviaFrom(classDeclaration.Identifier));
            var renamedConstructors = renamed.Members.OfType<ConstructorDeclarationSyntax>()
                .Where(ctor => ctor.Identifier.Text == classDeclaration.Identifier.Text)
                .ToList();
            renamed = renamed.ReplaceNodes(renamedConstructors, (ctor, _) =>
                ctor.WithIdentifier(SyntaxFactory.Identifier(component.ComponentName)
                    .WithTriviaFrom(ctor.Identifier)));
            root = root.ReplaceNode(classDeclaration, renamed);
            classDeclaration = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .First(candidate => candidate.Identifier.Text == component.ComponentName);
            report.Info(sourceName,
                $"クラス名を {component.ComponentName} に変更しました(Razor コンポーネント名は大文字始まりが必須)。");
        }

        ReportUnsupportedLifecycle(classDeclaration, sourceName, report);
        CodeBehindUsageAnalyzer.Analyze(classDeclaration, component.Fields, sourceName, report);

        // Classes deriving from a custom base defined in the project (BaseNopPage etc.)
        // keep it - the razor side emits the same base in @inherits, and the custom
        // base's own chain root is rewritten to the compatibility base
        var keepsCustomBase = HasCustomProjectBase(classDeclaration, baseRegistry);

        var updated = keepsCustomBase ? classDeclaration : RemoveWebFormsBaseType(classDeclaration);
        if (keepsCustomBase)
        {
            // Both halves of the partial class must name the base with the SAME text. The
            // razor resolves and fully qualifies it; the source wrote it short. Restating
            // the razor's version here keeps them textually identical, which is what C#
            // compares - a generic base otherwise reads as two different types and fails
            // with CS0263 even though both spellings resolve to one.
            updated = WithBaseType(updated, component.RazorInheritsBase);
        }
        // BEFORE InsertGeneratedMembers. That step adds the converter's own
        // OnAfterRender(bool) - the lifecycle driver - and running the override check
        // afterwards demoted it: the base is whatever the .razor inherits, which the check
        // resolves through the ported chain, and a miss there silently turns the page
        // lifecycle off while the build still passes.
        updated = DropOverridesTheCompatBaseDoesNotHave(updated, sourceName, report, portedTypes);
        updated = AddParameterAttributes(updated, component, sourceName, report);
        updated = InsertGeneratedMembers(updated, component, sourceName, report);

        root = root.ReplaceNode(classDeclaration, updated);
        return RewriteQualifiedFrameworkTypes(RewriteSyntax(root).ToFullString());
    }

    /// <summary>
    /// Aliases that keep partially qualified references working after the file moves
    /// namespace. Code inside N2.Addons.AddonCatalog.UI writes "Items.Addon" and resolves it
    /// through the ENCLOSING namespace chain, which the conversion replaces - and C# does
    /// not import nested namespaces through a using, so no plain import restores it. An
    /// alias does: "using Items = N2.Addons.AddonCatalog.Items;".
    ///
    /// Only segments the file actually uses as a qualifier are emitted, and only when no
    /// type of that name exists - aliasing over a type name would change what the file
    /// means rather than preserve it.
    /// </summary>
    private static List<string> NestedNamespaceAliases(
        string source, string? originalNamespace, BaseClassRegistry? registry)
    {
        var aliases = new List<string>();
        if (registry is null || string.IsNullOrEmpty(originalNamespace))
        {
            return aliases;
        }

        var taken = new HashSet<string>(StringComparer.Ordinal);
        for (var ancestor = originalNamespace; !string.IsNullOrEmpty(ancestor);)
        {
            foreach (var segment in registry.ChildNamespaceSegments(ancestor))
            {
                if (!taken.Add(segment)
                    || registry.DeclaresTypeNamed(segment)
                    || !source.Contains(segment + ".", StringComparison.Ordinal))
                {
                    continue;
                }
                aliases.Add($"{segment} = {ancestor}.{segment}");
            }

            var lastDot = ancestor.LastIndexOf('.');
            ancestor = lastDot < 0 ? null : ancestor[..lastDot];
        }

        return aliases;
    }

    /// <summary>
    /// Replaces the first entry of the base list with <paramref name="baseTypeName"/>,
    /// leaving any interfaces after it alone. A no-op when the razor did not record a base.
    /// </summary>
    private static ClassDeclarationSyntax WithBaseType(
        ClassDeclarationSyntax classDeclaration, string? baseTypeName)
    {
        if (string.IsNullOrWhiteSpace(baseTypeName)
            || classDeclaration.BaseList is not { Types.Count: > 0 } baseList)
        {
            return classDeclaration;
        }

        var existing = baseList.Types[0];
        var replacement = SyntaxFactory.SimpleBaseType(
            SyntaxFactory.ParseTypeName(baseTypeName).WithTriviaFrom(existing.Type));
        return classDeclaration.WithBaseList(
            baseList.WithTypes(baseList.Types.Replace(existing, replacement)));
    }

    private static bool HasCustomProjectBase(ClassDeclarationSyntax classDeclaration, BaseClassRegistry? registry)
    {
        if (registry is null)
        {
            return false;
        }
        var baseName = classDeclaration.BaseList?.Types.FirstOrDefault()?.Type.ToString();
        return baseName is not null && registry.TryResolve(baseName, out _);
    }

    /// <summary>
    /// The WebForms bases that make a type a Blazor component. Only these three matter to
    /// <see cref="ResolveComponentBase"/>: the render-based Legacy* replacements are not
    /// components, and some of them declare abstract members a stub could not implement.
    /// </summary>
    private static readonly HashSet<string> ComponentBaseNames = new(StringComparer.Ordinal)
    {
        "Page", "MasterPage", "UserControl",
    };

    /// <summary>
    /// The compat component base for a WebForms base name, or null when there is none.
    ///
    /// Exposed for the excluded-type stubs. A stub that loses a WebForms base stops being a
    /// component, and then every .razor with "@inherits ThatStub" fails on BuildRenderTree -
    /// the error surfaces in generated markup while the cause sits in the stub.
    /// </summary>
    internal static string? ResolveComponentBase(string baseName)
        => ComponentBaseNames.Contains(baseName)
           && CompatBaseReplacements.TryGetValue(baseName, out var compat)
            ? compat
            : null;

    /// <summary>
    /// WebForms control bases that the compatibility layer models as Blazor COMPONENTS.
    /// A plain class - an excluded-type stub - cannot usefully derive from one of those,
    /// so it derives from LegacyWebControl instead, which is the same surface as a plain
    /// class.
    ///
    /// Without this the stub gets no base at all, and then every class deriving FROM the
    /// stub fails on each lifecycle override it declares. DNN's DnnDropDownList is a
    /// Panel; losing that took out OnInit, OnPreRender and CreateChildControls across all
    /// of its subclasses, and the errors pointed at the subclasses.
    /// </summary>
    private static readonly HashSet<string> ControlBaseNames = new(StringComparer.Ordinal)
    {
        "WebControl", "CompositeControl", "Panel", "PlaceHolder", "Literal", "Label",
        "TextBox", "Button", "LinkButton", "ImageButton", "HyperLink", "Image",
        "CheckBox", "RadioButton", "ListControl", "DropDownList", "ListBox",
        "CheckBoxList", "RadioButtonList", "Repeater", "DataList", "GridView",
        "DetailsView", "FormView", "BaseValidator", "HtmlGenericControl",
        "DataBoundControl", "CompositeDataBoundControl", "BaseDataBoundControl", "Calendar", "TreeView",
        "HierarchicalDataBoundControl", "TemplateControl", "WebPart",
    };

    /// <summary>
    /// The compat class an excluded stub should derive from so that lifecycle overrides in
    /// its subclasses resolve, or null when the base is not a WebForms control.
    /// </summary>
    /// <summary>
    /// Every control type the compatibility layer declares, by simple name.
    ///
    /// The list above was hand-written and had gaps, and so did the SECOND hand-written
    /// list beside it (AspxConverters.LegacyRenderableRoots) - the two disagreed, which is
    /// what two hand-written lists of the same thing always end up doing. Measured across
    /// the corpora: 69 of the 113 unmapped-control residuals were a control whose source
    /// WAS ported and whose base chain ended at a name one list had and the other did not
    /// (RequiredFieldValidator 10, RegularExpressionValidator 6, DataSourceControl 15,
    /// FileUpload 2, HtmlForm 2, Login 1, ...).
    ///
    /// So the assembly is asked instead. A name is a control base when the compat layer
    /// declares it as one - a Blazor component (WebFormsControlBase) or a plain render
    /// class (LegacyWebControl). Page / MasterPage / UserControl are excluded: those are
    /// not child controls and have their own mapping.
    /// </summary>
    private static readonly Lazy<HashSet<string>> CompatControlNames = new(() =>
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        var assembly = typeof(WebForm2Blazor.Components.WebFormsControlBase).Assembly;
        var component = typeof(WebForm2Blazor.Components.WebFormsControlBase);
        var legacy = assembly.GetType("WebForm2Blazor.Components.LegacyWebControl");

        foreach (var type in assembly.GetExportedTypes())
        {
            if (type.Namespace != "WebForm2Blazor.Components"
                || type.IsInterface
                || ComponentBaseNames.Contains(type.Name))
            {
                continue;
            }

            if (component.IsAssignableFrom(type) || (legacy is not null && legacy.IsAssignableFrom(type)))
            {
                names.Add(type.Name);
            }
        }

        return names;
    });

    internal static string? ResolveControlBase(string baseName)
        => ControlBaseNames.Contains(baseName) || CompatControlNames.Value.Contains(baseName)
            ? "LegacyWebControl"
            : null;

    private static readonly Dictionary<string, string> CompatBaseReplacements = new(StringComparer.Ordinal)
    {
        ["Page"] = "WebFormsPage",
        ["MasterPage"] = "WebFormsLayout",
        ["UserControl"] = "WebFormsUserControl",
        // Render-based custom controls run under LegacyRenderHost
        ["WebControl"] = "LegacyWebControl",
        ["Control"] = "LegacyWebControl",
        ["Panel"] = "LegacyPanel",
        ["Label"] = "LegacyLabel",
        ["Literal"] = "LegacyLiteral",
        ["HyperLink"] = "LegacyHyperLink",
        // Calendar renders now, so a ported control deriving from it gets the base that
        // carries the members rather than the component - the LegacyPanel arrangement.
        ["Calendar"] = "LegacyCalendar",
        ["TreeView"] = "LegacyTreeView",
        // Interactive-control bases: the derived custom controls are stubbed in markup
        // (interactivity is manual-migration territory), but their ported source must
        // still compile - LegacyWebControl carries the lifecycle/render virtuals
        ["CheckBox"] = "LegacyWebControl",
        ["RadioButton"] = "LegacyWebControl",
        ["TextBox"] = "LegacyWebControl",
        ["Button"] = "LegacyWebControl",
        ["LinkButton"] = "LegacyWebControl",
        ["ImageButton"] = "LegacyWebControl",
        ["DropDownList"] = "LegacyWebControl",
        ["ListBox"] = "LegacyWebControl",
        ["GridView"] = "LegacyWebControl",
        ["DataGrid"] = "LegacyWebControl",
        ["Repeater"] = "LegacyWebControl",
        ["DataList"] = "LegacyWebControl",
        ["CompositeControl"] = "LegacyWebControl",
        ["PlaceHolder"] = "LegacyWebControl",
        ["Image"] = "LegacyWebControl",
        // Same reason as the block above, found by re-measuring CS0115: the compat
        // counterpart of each of these is a Blazor COMPONENT, which a ported plain class
        // cannot derive from, so its Render / OnPreRender overrides had nothing to bind to
        // (mojoPortal's "jQueryFileUpload : FileUpload").
        //
        // Only names whose compat form is a component belong here. TreeView, Menu,
        // MultiView and Calendar are declaration shims that ALREADY derive from
        // LegacyWebControl, and redirecting those would throw away the members the shim
        // carries - a regression, not a fix.
        ["FileUpload"] = "LegacyWebControl",
        ["CheckBoxList"] = "LegacyWebControl",
        ["RadioButtonList"] = "LegacyWebControl",
        ["DetailsView"] = "LegacyWebControl",
        ["FormView"] = "LegacyWebControl",
        ["HiddenField"] = "LegacyWebControl",
        ["Table"] = "LegacyWebControl",
    };

    /// <summary>
    /// Removes "override" where the compat base has no such member.
    ///
    /// WebForms had a control class per widget, each with its own virtuals: DataGridColumn
    /// had Initialize, BaseValidator had EvaluateIsValid, ListControl had
    /// PerformDataBinding. The compat layer models the ones that render and collapses the
    /// rest onto LegacyWebControl, so a ported subclass's override has nothing to bind to
    /// and the file stops compiling on a method whose body is perfectly good.
    ///
    /// Dropping the keyword keeps the body. Nothing in the compat layer would have called
    /// these anyway - it does not implement DataGrid or validator behaviour - so this
    /// removes an error rather than a call. Each one is reported, because a control whose
    /// base is not modelled does not behave like the original and that has to stay visible.
    ///
    /// The compat ASSEMBLY is asked, never a list: which members exist changes as the
    /// layer grows, and a list would go stale in the direction that silently drops
    /// overrides that had become valid.
    /// </summary>
    private static CompilationUnitSyntax DropOverridesTheCompatBaseDoesNotHave(
        CompilationUnitSyntax root,
        string? sourceName,
        ConversionReport? report,
        PortedTypeIndex? portedTypes)
    {
        var rewritten = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
            .ToDictionary(
                declaration => declaration,
                declaration => (SyntaxNode)DropOverridesTheCompatBaseDoesNotHave(
                    declaration, sourceName, report, portedTypes))
            .Where(pair => pair.Key != pair.Value)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        return rewritten.Count == 0
            ? root
            : root.ReplaceNodes(rewritten.Keys, (original, _) => rewritten[original]);
    }

    /// <inheritdoc cref="DropOverridesTheCompatBaseDoesNotHave(CompilationUnitSyntax, string, ConversionReport, PortedTypeIndex)"/>
    private static ClassDeclarationSyntax DropOverridesTheCompatBaseDoesNotHave(
        ClassDeclarationSyntax classDeclaration,
        string? sourceName,
        ConversionReport? report,
        PortedTypeIndex? portedTypes)
    {
        var edits = new Dictionary<MemberDeclarationSyntax, MemberDeclarationSyntax>();

        {
            if (classDeclaration.BaseList?.Types.FirstOrDefault()?.Type.ToString().Trim() is not { } baseName)
            {
                return classDeclaration;
            }

            // Name`arity, so a ported generic of the same name is not mistaken for the
            // compat type (n2 declares Page<TPage>).
            // The first entry of a base list is only the base CLASS when there is one. A
            // converted code-behind has its base moved to the .razor's @inherits, so what
            // is left first is an interface - BlogEngine's CommentList starts
            // ": ICallbackEventHandler" - and judging overrides against an interface
            // demoted two perfectly good ones.
            if (baseName.StartsWith('I') && baseName.Length > 1 && char.IsUpper(baseName[1])
                && CompatType(baseName) is { IsInterface: true })
            {
                return classDeclaration;
            }

            var baseKey = PortedTypeIndex.KeyOfWrittenType(baseName);
            var simpleName = baseKey.Contains('`', StringComparison.Ordinal)
                ? baseKey[..baseKey.IndexOf('`')]
                : baseKey;

            // The application's own type of the same name wins over the compat layer's.
            // DNN declares DotNetNuke.Security.Membership.MembershipProvider, and reading
            // that as ASP.NET's - a completely different set of members - stripped the
            // override off 30 implementations of DNN's own abstract members and turned 13
            // errors into 52.
            var portedBase = portedTypes?.Declares(baseKey) == true;
            var baseType = portedBase
                ? null
                : typeof(WebForm2Blazor.Components.WebFormsControlBase).Assembly
                    .GetType("WebForm2Blazor.Components." + simpleName);

            // Neither the compat layer's nor the application's. It may still be a type a
            // real compilation can resolve - the framework's - and SemanticBaseIndex asks
            // that question properly. Name-matching it was tried and reverted (residuals
            // 507 -> 580, build errors 46 -> 65): a base is written unqualified, and
            // matching a base by simple name is the trap this file has been burned by
            // repeatedly. The semantic answer is null unless the whole chain resolved.
            var metadataName = MetadataNameOf(classDeclaration);
            if (baseType is null && !portedBase
                && (metadataName is null || Semantics is null))
            {
                // Third-party, and nothing can resolve it. Its members are not ours to judge.
                return classDeclaration;
            }

            foreach (var member in classDeclaration.Members)
            {
                var (name, modifiers) = member switch
                {
                    MethodDeclarationSyntax method => (method.Identifier.Text, method.Modifiers),
                    PropertyDeclarationSyntax property => (property.Identifier.Text, property.Modifiers),
                    _ => (null, default),
                };

                if (name is null
                    || !modifiers.Any(modifier =>
                        modifier.RawKind == (int)SyntaxKind.OverrideKeyword))
                {
                    continue;
                }

                bool? found = portedBase
                    ? portedTypes!.AnyBaseDeclares(baseKey, name, CompatDeclares)
                    : baseType is not null
                        ? DeclaresMember(baseType, name)
                        : Semantics!.BaseDeclaresMember(metadataName!, name);

                if (found is null)
                {
                    // The base chain could not be resolved all the way. Nothing is known,
                    // so nothing is changed - the same answer as a third-party base.
                    continue;
                }

                if (found == true)
                {
                    if (baseType is null)
                    {
                        continue;
                    }

                    // The member is there; the only thing that can still be wrong is how
                    // visible it is. WebForms declared the same method at different
                    // accessibilities on different bases - WebControl.RenderBeginTag is
                    // public, HtmlControl's is protected - and the compat layer collapses
                    // both onto one, so half the ported overrides disagree with it (CS0507).
                    if (AlignAccessibility(member, modifiers, baseType, name) is { } aligned)
                    {
                        edits[member] = aligned;
                        report?.Info(sourceName ?? string.Empty,
                            $"{classDeclaration.Identifier.Text}.{name} を public override にしました"
                            + $"(互換層の {simpleName} が public で宣言しているため)。");
                    }
                    continue;
                }

                // virtual, not nothing: subclasses in the same application override this
                // member too, and a plain method cannot be overridden (CS0506). The chain
                // below this class keeps working; only the link above it is gone, and
                // that link was to a base the compat layer does not model.
                //
                // Unless the class is sealed, where a virtual member is CS0549 and there
                // can be no subclass to keep working anyway.
                var sealedClass = classDeclaration.Modifiers.Any(modifier =>
                    modifier.RawKind == (int)SyntaxKind.SealedKeyword);

                var kept = SyntaxFactory.TokenList(modifiers
                    .Select(modifier => modifier.RawKind == (int)SyntaxKind.OverrideKeyword && !sealedClass
                        ? SyntaxFactory.Token(SyntaxKind.VirtualKeyword).WithTriviaFrom(modifier)
                        : modifier)
                    // "sealed override" seals the override; with no override left it is
                    // CS0238 ("cannot be sealed because it is not an override").
                    .Where(modifier => modifier.RawKind != (int)SyntaxKind.SealedKeyword)
                    .Where(modifier => !(sealedClass && modifier.RawKind == (int)SyntaxKind.OverrideKeyword)));

                edits[member] = member switch
                {
                    MethodDeclarationSyntax method => method.WithModifiers(kept),
                    PropertyDeclarationSyntax property => property.WithModifiers(kept),
                    _ => member,
                };

                report?.Residual(sourceName ?? string.Empty, ResidualKind.CodeBehind,
                    $"{classDeclaration.Identifier.Text}.{name} の override を外しました。"
                    + $"{simpleName} から上に、元の基底が持っていたこのメンバがありません"
                    + "(互換層がこのコントロールを描画対象としてモデル化していないため、"
                    + "このメソッドは基底から呼ばれません)。",
                    disposition: ResidualDisposition.Backlog);
            }
        }

        return edits.Count == 0
            ? classDeclaration
            : classDeclaration.ReplaceNodes(edits.Keys, (original, _) => edits[original]);
    }

    /// <summary>
    /// Makes a protected override public when the compat base declares that member public,
    /// or null when nothing needs changing. C# requires an override to match the base
    /// exactly, and only widening is ever needed here: the compat layer never narrows a
    /// member the original had public.
    /// </summary>
    private static MemberDeclarationSyntax? AlignAccessibility(
        MemberDeclarationSyntax member, SyntaxTokenList modifiers, Type baseType, string name)
    {
        var isPublicOnBase = baseType.GetMember(
                name,
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.FlattenHierarchy)
            .Length > 0;

        var isProtectedHere = modifiers.Any(modifier =>
            modifier.RawKind == (int)SyntaxKind.ProtectedKeyword);

        if (!isPublicOnBase || !isProtectedHere)
        {
            return null;
        }

        var widened = SyntaxFactory.TokenList(modifiers.Select(modifier =>
            modifier.RawKind == (int)SyntaxKind.ProtectedKeyword
                ? SyntaxFactory.Token(SyntaxKind.PublicKeyword).WithTriviaFrom(modifier)
                : modifier));

        return member switch
        {
            MethodDeclarationSyntax method => method.WithModifiers(widened),
            PropertyDeclarationSyntax property => property.WithModifiers(widened),
            _ => null,
        };
    }

    /// <summary>
    /// Whether the compat type of this name declares the member, or null when the compat
    /// layer has no type of that name.
    /// </summary>
    /// <summary>The compat type of this simple name, or null.</summary>
    private static Type? CompatType(string simpleName)
        => typeof(WebForm2Blazor.Components.WebFormsControlBase).Assembly
            .GetType("WebForm2Blazor.Components." + simpleName);

    private static bool? CompatDeclares(string simpleName, string member)
    {
        // The index is built from the sources BEFORE the base rewrite, so a ported class
        // still says "UserControl" where the output says WebFormsUserControl. Asking the
        // compat layer for "UserControl" finds the empty marker type instead of the base
        // that has the lifecycle on it, and everything below it lost its OnInit.
        var mapped = ResolveComponentBase(simpleName) ?? ResolveControlBase(simpleName) ?? simpleName;

        return typeof(WebForm2Blazor.Components.WebFormsControlBase).Assembly
                   .GetType("WebForm2Blazor.Components." + mapped) is { } type
            ? DeclaresMember(type, member)
            : null;
    }

    /// <summary>
    /// Whether the type or any base declares a member of this name.
    ///
    /// Walks BaseType by hand. Type.GetMember does not return a base class's non-public
    /// members, and FlattenHierarchy only widens that for STATIC members - so asking
    /// Panel for "OnLoad", which LegacyWebControl declares protected, answers no. That
    /// made this strip the override off valid code: n2's Edit.aspx.cs overrides OnLoad
    /// through four ported bases down to the compat page, and it was being demoted.
    /// </summary>
    private static bool DeclaresMember(Type type, string name)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (current.GetMember(
                    name,
                    System.Reflection.BindingFlags.Instance
                    | System.Reflection.BindingFlags.Public
                    | System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.DeclaredOnly).Length > 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Non-code-behind .cs files (business logic etc.). Usings are replaced; namespace and
    /// classes are ported as-is - except that classes deriving directly from
    /// System.Web.UI.Page / MasterPage / UserControl (= custom base classes such as
    /// BaseNopPage) get that base swapped for the compatibility base, so the whole
    /// inheritance chain of converted pages lands on the compat runtime.
    /// </summary>
    public static string RewritePlainCodeFile(
        string source,
        string? sourceName = null,
        ConversionReport? report = null,
        PortedTypeIndex? portedTypes = null)
        => DeepSyntaxWork.Run(() =>
            RewritePlainCodeFileCore(source, sourceName, report, portedTypes));

    private static string RewritePlainCodeFileCore(
        string source, string? sourceName, ConversionReport? report, PortedTypeIndex? portedTypes)
    {
        var root = ParseUnit(source);

        var replacedAny = false;
        var targets = new Dictionary<BaseTypeSyntax, BaseTypeSyntax>();
        foreach (var classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
        {
            var baseType = classDeclaration.BaseList?.Types.FirstOrDefault();
            if (baseType is null)
            {
                continue;
            }
            var baseName = baseType.Type.ToString().Trim();
            var lastDot = baseName.LastIndexOf('.');
            var shortName = lastDot >= 0 ? baseName[(lastDot + 1)..] : baseName;

            // A bare short name (WebControl, Control, Page, ...) in a WebForms project
            // refers to System.Web.UI; qualified names must actually point there
            var isSystemWebBase = baseName == shortName
                                  || baseName.Contains("System.Web.UI", StringComparison.Ordinal);
            if (!isSystemWebBase || !CompatBaseReplacements.TryGetValue(shortName, out var replacement))
            {
                continue;
            }

            targets[baseType] = SyntaxFactory.SimpleBaseType(
                SyntaxFactory.ParseTypeName(replacement).WithTriviaFrom(baseType.Type));
            replacedAny = true;
            report?.Info(sourceName ?? string.Empty,
                $"独自基底クラス {classDeclaration.Identifier.Text} の基底 {baseName} を {replacement} に差し替えました。");
        }

        if (replacedAny)
        {
            root = root.ReplaceNodes(targets.Keys, (original, _) => targets[original]);
        }

        // The body rewrite runs FIRST so the import decision can see its result. Ordered
        // the other way, the decision was made from the file's usings alone - and a file
        // whose body gets compat substitutions may have no usings to judge by. n2's
        // MembershipToolbarPluginAttribute has none at all: the rewrite turned Control into
        // IWebFormsControl and then nothing imported it. Same shape in a global-usings
        // project, where the per-file import list is empty by construction.
        var bodyRewritten = DropOverridesTheCompatBaseDoesNotHave(
            RewriteSyntax(root), sourceName, report, portedTypes);

        // Dropped System.Web usings mean the file references that API surface
        // (HttpContext, HttpUtility, ...) - the compatibility namespace supplies it
        var needsCompatNamespace = replacedAny
            || !ReferenceEquals(bodyRewritten, root)
            || AllUsings(root).Any(directive =>
            {
                var usingName = directive.Name?.ToString() ?? string.Empty;
                return usingName == "System.Web"
                       || usingName.StartsWith("System.Web.", StringComparison.Ordinal)
                       // Dropped above, so the compat FileIOPermission must be importable
                       || usingName == "System.Security.Permissions";
            });

        var rewritten = RewriteUsings(
            bodyRewritten, needsCompatNamespace ? ["WebForm2Blazor.Components"] : []).ToFullString();
        return RewriteQualifiedFrameworkTypes(rewritten);
    }

    /// <summary>
    /// The syntax-level passes, applied to the parsed tree before the text pass in
    /// <see cref="RewriteQualifiedFrameworkTypes"/>. A rewrite belongs here whenever it has
    /// to reason about the SHAPE of the code rather than the spelling of a name.
    /// </summary>
    private static CompilationUnitSyntax RewriteSyntax(CompilationUnitSyntax root)
        => (CompilationUnitSyntax)new HtmlGenericControlRewriter()
            .Visit(RewriteControlReferences(root));

    /// <summary>
    /// Maps System.Web.UI.Control REFERENCES onto IWebFormsControl.
    ///
    /// WebForms had one universal control base, so code writes
    /// "foreach (Control c in panel.Controls)" and then casts c down to TextBox. Here the
    /// compat components and the legacy render-based controls are two sibling families,
    /// so a Control-typed value cannot be cast to either - the C# compiler rejects a cast
    /// between unrelated classes outright. Typing those references as the interface both
    /// families implement restores the downcast, which is what the original code means.
    ///
    /// Declaration positions are left alone: "class MyControl : Control" still needs a
    /// class to derive from, "new Control()" still needs something instantiable, and
    /// typeof(Control) must keep denoting the same thing it compares against.
    /// </summary>
    private static CompilationUnitSyntax RewriteControlReferences(CompilationUnitSyntax root)
        => (CompilationUnitSyntax)new ControlReferenceRewriter().Visit(root);

    private sealed class ControlReferenceRewriter : CSharpSyntaxRewriter
    {
        private static readonly SyntaxAnnotation Rewritten = new();

        public override SyntaxNode? VisitQualifiedName(QualifiedNameSyntax node)
        {
            // System.Web.UI.Control -> IWebFormsControl (the qualifier proves the origin)
            if (node.Right.Identifier.Text == "Control"
                && node.Left.ToString() == "System.Web.UI"
                && IsReferencePosition(node))
            {
                return Replacement(node);
            }
            return base.VisitQualifiedName(node);
        }

        public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (node.Identifier.Text == "Control" && IsReferencePosition(node))
            {
                return Replacement(node);
            }
            return base.VisitIdentifierName(node);
        }

        private static SyntaxNode Replacement(SyntaxNode node)
            => SyntaxFactory.IdentifierName("IWebFormsControl")
                .WithTriviaFrom(node)
                .WithAdditionalAnnotations(Rewritten);

        /// <summary>
        /// True when the node names a type in a position that only needs the common
        /// surface - a variable, parameter, return type, cast or generic argument.
        /// </summary>
        private static bool IsReferencePosition(SyntaxNode node)
        {
            // "class X : Control" - a base must stay a class. The base list can nest the
            // name (generic arguments), so this one is checked across ancestors.
            if (node.Ancestors().OfType<BaseListSyntax>().Any())
            {
                return false;
            }

            var parent = node.Parent;
            return parent switch
            {
                // "new Control()" - an interface cannot be instantiated
                ObjectCreationExpressionSyntax creation => creation.Type != node,
                // "typeof(Control)" - compared against other typeof values
                TypeOfExpressionSyntax typeOf => typeOf.Type != node,
                // "Control.StaticMember": only the LEFT side is a static type reference.
                // A type ARGUMENT is not - dcf.Controls.Cast<Control>() sits under the
                // same member access and does need rewriting.
                MemberAccessExpressionSyntax access => access.Expression != node,
                _ => true,
            };
        }
    }

    /// <summary>
    /// new HtmlGenericControl("div")
    ///   -> new WebForm2Blazor.Components.HtmlGenericControl { TagName = "div" }
    ///
    /// The compat control is a Blazor component and a component must have exactly one
    /// (parameterless) constructor, so the tag has to travel as a property instead.
    ///
    /// This has to run on syntax rather than text. WebForms code routinely writes the
    /// constructor WITH an object initializer already attached:
    ///
    ///     new HtmlGenericControl("style") { InnerHtml = ... }
    ///
    /// and a regex that simply appends its own initializer emits two initializer blocks
    /// back to back, which is not valid C#. That was not a theoretical case: it was the
    /// only thing keeping mojoPortal, YAF.NET and DNN Platform from building. The existing
    /// initializer is merged instead, with TagName placed first.
    /// </summary>
    private sealed class HtmlGenericControlRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            // Children first: the tag argument or the initializer may contain further
            // constructions of their own.
            var creation = (ObjectCreationExpressionSyntax)base.VisitObjectCreationExpression(node)!;

            // Only the (tag) overload moves. A bare new HtmlGenericControl() already binds
            // to the component's own constructor and TagName keeps its default.
            if (!DenotesHtmlGenericControl(creation.Type)
                || creation.ArgumentList is not { Arguments.Count: 1 } argumentList)
            {
                return creation;
            }

            var tagName = SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName("TagName").WithTrailingTrivia(SyntaxFactory.Space),
                SyntaxFactory.Token(SyntaxKind.EqualsToken).WithTrailingTrivia(SyntaxFactory.Space),
                argumentList.Arguments[0].Expression.WithoutTrivia());

            return creation
                // The argument list disappears, so whatever followed its ")" - typically the
                // line break in front of a multi-line initializer - moves onto the type, or
                // the initializer would be dragged up onto the type's line.
                .WithType(SyntaxFactory
                    .ParseTypeName("WebForm2Blazor.Components.HtmlGenericControl")
                    .WithLeadingTrivia(creation.Type.GetLeadingTrivia())
                    .WithTrailingTrivia(argumentList.CloseParenToken.TrailingTrivia))
                .WithArgumentList(null)
                .WithInitializer(Merge(creation.Initializer, tagName));
        }

        /// <summary>
        /// Puts TagName at the head of the initializer, keeping the existing entries and
        /// their separators (a trailing comma is legal and some corpora write one). The
        /// layout of the original block is preserved: the new entry adopts the indentation
        /// of the entry it displaces, and the comma introduced after it carries a line
        /// break only when the block was already spread over several lines.
        /// </summary>
        private static InitializerExpressionSyntax Merge(
            InitializerExpressionSyntax? existing,
            AssignmentExpressionSyntax tagName)
        {
            if (existing is null)
            {
                return SyntaxFactory.InitializerExpression(
                        SyntaxKind.ObjectInitializerExpression,
                        SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(tagName))
                    .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken)
                        .WithLeadingTrivia(SyntaxFactory.Space)
                        .WithTrailingTrivia(SyntaxFactory.Space))
                    .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken)
                        .WithLeadingTrivia(SyntaxFactory.Space));
            }

            var expressions = existing.Expressions;
            if (expressions.Count == 0)
            {
                return existing.WithExpressions(
                    SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(tagName));
            }

            var multiLine = existing.OpenBraceToken.TrailingTrivia
                .Any(trivia => trivia.IsKind(SyntaxKind.EndOfLineTrivia));

            var parts = new List<SyntaxNodeOrToken>
            {
                tagName.WithLeadingTrivia(expressions[0].GetLeadingTrivia()),
                SyntaxFactory.Token(SyntaxKind.CommaToken).WithTrailingTrivia(
                    multiLine ? SyntaxFactory.EndOfLine(Environment.NewLine) : SyntaxFactory.Space),
            };

            for (var index = 0; index < expressions.Count; index++)
            {
                parts.Add(expressions[index]);
                if (index < expressions.SeparatorCount)
                {
                    parts.Add(expressions.GetSeparator(index));
                }
            }

            return existing.WithExpressions(SyntaxFactory.SeparatedList<ExpressionSyntax>(parts));
        }

        /// <summary>
        /// Matches on the final identifier, so the bare name, the original
        /// System.Web.UI.HtmlControls qualification and an already-mapped compat
        /// qualification are all recognised.
        /// </summary>
        private static bool DenotesHtmlGenericControl(TypeSyntax type) => type switch
        {
            IdentifierNameSyntax identifier => identifier.Identifier.Text == "HtmlGenericControl",
            QualifiedNameSyntax qualified => qualified.Right.Identifier.Text == "HtmlGenericControl",
            AliasQualifiedNameSyntax aliased => aliased.Name.Identifier.Text == "HtmlGenericControl",
            _ => false,
        };
    }

    /// <summary>
    /// Fully qualified Framework type references written inline (System.Web.UI.HtmlTextWriter
    /// in a method signature, EF4 ObjectContext namespaces in EDMX designer code) are not
    /// touched by the using rewrite; a text pass maps them onto the compat / EF6 types.
    /// </summary>
    private static string RewriteQualifiedFrameworkTypes(string code)
        => RewriteRootSystemWebTypes(code
            .Replace("System.Data.Objects", "System.Data.Entity.Core.Objects")
            .Replace("System.Data.EntityClient", "System.Data.Entity.Core.EntityClient")
            .Replace("System.Data.Metadata.Edm", "System.Data.Entity.Core.Metadata.Edm")
            // Targeted, not blanket: only types with a compat counterpart of the same name.
            // Helper libraries write these fully qualified in signatures
            // (Utils.AddJavaScriptInclude(System.Web.UI.Page page, ...)), where a using
            // rewrite never reaches them.
            .Replace("System.Web.UI.HtmlTextWriter", "WebForm2Blazor.Components.HtmlTextWriter")
            .Replace("System.Web.UI.AttributeCollection", "WebForm2Blazor.Components.AttributeCollection")
            .Replace("System.Configuration.ConnectionStringSettings", "WebForm2Blazor.Components.Compat.ConnectionStringSettings")
            // The REAL System.Configuration.ConfigurationManager reads App.config, and the
            // converted application's settings are in appsettings.json - so
            // ConnectionStrings["WingtipToys"] came back null and the next dereference was
            // a NullReferenceException that took the circuit down with it. The compat one
            // reads what the converter actually wrote.
            .Replace("System.Configuration.ConfigurationManager", "WebForm2Blazor.Components.Compat.ConfigurationManager")
            .Replace("System.Web.HttpBrowserCapabilities", "WebForm2Blazor.Components.HttpBrowserCapabilitiesShim")
            .Replace("HttpCapabilitiesBase", "HttpBrowserCapabilitiesShim"));

    /// <summary>
    /// The WebForms namespaces the compat layer stands in for, longest first so that
    /// "System.Web.UI.WebControls.Adapters.X" is not read as the shorter prefix plus
    /// "Adapters.X" - the compat layer is flat and has no Adapters namespace in it.
    /// </summary>
    private static readonly string[] CompatSourceNamespaces =
    [
        "System.Web.UI.WebControls.Adapters",
        "System.Web.UI.HtmlControls",
        "System.Web.UI.WebControls",
        "System.Web.UI.Adapters",
        "System.Web.Script.Serialization",
        "System.Web.Script.Services",
        "System.Web.Services.Protocols",
        "System.Web.Services",
        "System.Web.Routing",
        "System.Security.Permissions",
        "System.Web.Configuration",
        "System.Web.Security",
        "System.Web.Caching",
        "System.Web.Hosting",
        "System.Web.Profile",
        "System.Web.UI",
        "System.Web",
    ];

    /// <summary>
    /// "System.Web.Foo" -> "WebForm2Blazor.Components.Foo", but only where the compat layer
    /// really declares a Foo. Anything else is left alone so the error stays visible, and
    /// pointing at a namespace member that does not exist is a worse error than the one it
    /// replaces: rewriting System.Security.Permissions wholesale turned
    /// System.Security.Permissions.PermissionState, which the compat layer does not have,
    /// into WebForm2Blazor.Components.PermissionState, which does not exist at all.
    ///
    /// The list above is of SOURCE namespaces, which is fixed by WebForms. What the compat
    /// layer contains is asked of the assembly, never listed - that is what went wrong
    /// before, when HttpContext and HttpUtility were listed while HttpApplication,
    /// SiteMapNode, SiteMapProvider and HttpRequestBase were not.
    /// </summary>
    private static string RewriteRootSystemWebTypes(string code)
    {
        foreach (var sourceNamespace in CompatSourceNamespaces)
        {
            code = System.Text.RegularExpressions.Regex.Replace(
                code,
                @"\b" + System.Text.RegularExpressions.Regex.Escape(sourceNamespace) + @"\.([A-Z]\w*)\b",
                match => CompatTypeNameFor(match.Groups[1].Value) is { } compatName
                    ? "WebForm2Blazor.Components." + compatName
                    : match.Value);
        }

        return code;
    }

    /// <summary>
    /// The compatibility layer's name for a type written as "Namespace.Name", or null when
    /// it has none.
    ///
    /// An attribute may be written without its "Attribute" suffix, and fully qualified at
    /// that: DNN's InstallWizard writes "[System.Web.Services.WebMethod]". Matching the
    /// written name alone left that one qualified reference pointing at a namespace that no
    /// longer exists, so the compat layer's WebMethodAttribute - which was right there -
    /// was never reached.
    /// </summary>
    private static string? CompatTypeNameFor(string writtenName)
    {
        if (CompatTypeNames.Contains(writtenName))
        {
            return writtenName;
        }

        // Only for the shorthand form: a name already ending in "Attribute" was looked up
        // above, and appending a second one would invent a type.
        return !writtenName.EndsWith("Attribute", StringComparison.Ordinal)
               && CompatTypeNames.Contains(writtenName + "Attribute")
            ? writtenName + "Attribute"
            : null;
    }

    /// <summary>
    /// Whether the compatibility layer declares a type of this name. Used by the
    /// excluded-type stubs: a name that came from System.Web in the source resolves to the
    /// compat layer after the port, and the stub has to write THAT name in its signatures.
    /// </summary>
    internal static bool DeclaresCompatType(string name) => CompatTypeNames.Contains(name);

    /// <summary>The same set, for CompatImportDisambiguator to enumerate.</summary>
    internal static IReadOnlySet<string> CompatTypeNamesForDisambiguation => CompatTypeNames;

    /// <summary>Every public type the compat layer declares directly in its namespace.</summary>
    private static readonly HashSet<string> CompatTypeNames =
        typeof(WebForm2Blazor.Components.WebFormsControlBase).Assembly
            .GetExportedTypes()
            .Where(type => type.Namespace == "WebForm2Blazor.Components")
            .Select(type => type.Name)
            .ToHashSet(StringComparer.Ordinal);
    // new HtmlGenericControl("div") is NOT handled here: rewriting a constructor call
    // needs to see whether an object initializer already follows it, which a text pass
    // cannot. See HtmlGenericControlRewriter.

    /// <summary>
    /// Every using in the file, including the ones written INSIDE the namespace. That
    /// layout is a StyleCop convention and a whole corpus (BlogEngine) is written that
    /// way; looking only at the file-level list left every System.Web import in place
    /// and the compatibility namespace unimported, so nothing in those files resolved.
    /// </summary>
    /// <summary>
    /// The semantic index for this conversion, or null outside one. Ambient rather than
    /// threaded through eight signatures; the converter builds one per run and the rewrite
    /// runs on a single thread.
    /// </summary>
    internal static SemanticBaseIndex? Semantics { get; set; }

    /// <summary>
    /// A class's metadata name ("YAF.Data.ProfiledProviderFactory", "N2.Web.Page`1"), which
    /// is how a compilation is asked for a type. Null for a nested class, where the
    /// metadata name uses "+" and the extra cases are not worth guessing at.
    /// </summary>
    private static string? MetadataNameOf(ClassDeclarationSyntax declaration)
    {
        if (declaration.Parent is TypeDeclarationSyntax)
        {
            return null;
        }

        var namespaceName = declaration.Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault()?.Name.ToString();
        var arity = declaration.TypeParameterList?.Parameters.Count ?? 0;

        var name = declaration.Identifier.Text + (arity == 0 ? string.Empty : "`" + arity);
        return string.IsNullOrEmpty(namespaceName) ? name : namespaceName + "." + name;
    }

    private static List<UsingDirectiveSyntax> AllUsings(CompilationUnitSyntax root)
        => [.. root.Usings,
            .. root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>().SelectMany(ns => ns.Usings)];

    /// <summary>
    /// Namespaces with no .NET counterpart. System.Web is supplied by the compatibility
    /// library instead; AjaxControlToolkit / FCKeditor have no .NET build at all and are
    /// usually only imported, not used.
    /// </summary>
    private static bool IsDroppedNamespace(string name)
        => name == "System.Web" || name.StartsWith("System.Web.", StringComparison.Ordinal)
           // Code Access Security was removed in .NET Core: the namespace still exists, so
           // the using resolves and then the TYPE does not. Dropping it lets the inert
           // FileIOPermission in the compatibility library bind instead.
           || name == "System.Security.Permissions"
           // The CodeDom shapes an expression builder returns are declared in the compat
           // layer rather than referenced from the System.CodeDom package: nothing here
           // compiles a CodeDom graph, so the types only need to exist. Dropping the
           // import lets them bind to the compat ones.
           || name == "System.CodeDom" || name.StartsWith("System.CodeDom.", StringComparison.Ordinal)
           // .NET Remoting was removed outright. The namespace does not exist, so the
           // IMPORT itself is the error (CS0234) - YAF's HelpMenu.cs carries a
           // "using System.Runtime.Remoting.Contexts;" it never uses. Dropping it removes
           // the error; a file that really used a remoting type gets a CS0246 naming that
           // type, which says far more than "the namespace does not exist".
           // System.Runtime.Remoting.Messaging is excepted: the compat layer declares
           // CallContext there, and ported code does use it.
           || (name.StartsWith("System.Runtime.Remoting", StringComparison.Ordinal)
               && !name.StartsWith("System.Runtime.Remoting.Messaging", StringComparison.Ordinal))
           || name == "AjaxControlToolkit" || name.StartsWith("AjaxControlToolkit.", StringComparison.Ordinal)
           || name == "FredCK" || name.StartsWith("FredCK.", StringComparison.Ordinal);

    private static CompilationUnitSyntax RewriteUsings(CompilationUnitSyntax root, string[] requiredUsings)
    {
        var aliasUsings = new List<string>();
        var removals = new List<UsingDirectiveSyntax>();
        var configurationAliasScope = ConfigurationAliasScope.None;

        // Duplicates are tracked per CONTAINER: one file may hold several namespaces, each
        // with its own import list, and a file-wide set would strip the second "using
        // System;" and leave that namespace unable to resolve anything.
        var duplicates = new Dictionary<SyntaxNode, HashSet<string>>();

        foreach (var directive in AllUsings(root))
        {
            var name = directive.Name?.ToString() ?? string.Empty;

            // An alias ("using Page = System.Web.UI.Page;") names ONE type and outranks
            // every namespace import, so dropping it silently changes what that name means
            // in the file. It is kept; the qualified-type pass retargets it to the compat
            // type, and an alias with no counterpart fails loudly instead.
            if (directive.Alias is not null)
            {
                continue;
            }

            if (IsDroppedNamespace(name))
            {
                // The compat shims keep the "Shim" suffix so they never shadow a real
                // .NET type, so code declaring System.Web parameter types needs an alias
                if (name == "System.Web")
                {
                    aliasUsings.Add("using HttpRequest = WebForm2Blazor.Components.HttpRequestShim;");
                    aliasUsings.Add("using HttpResponse = WebForm2Blazor.Components.HttpResponseShim;");

                    // The *Base abstractions (System.Web.Abstractions) and HttpSessionState
                    // / HttpServerUtility used to be supplied here as aliases too. They are
                    // real types in the compatibility layer now - base classes of the shims -
                    // because an alias only reaches the file that dropped a "using
                    // System.Web;" of its own. An application whose imports are all "global
                    // using" (YAF.NET) has no such file, so the names were supplied nowhere.
                    // HttpRequest / HttpResponse stay aliases: those names DO exist in
                    // Microsoft.AspNetCore.Http, so the compat types keep the Shim suffix.
                }
                removals.Add(directive);
                continue;
            }

            // ConfigurationManager is replaced by the compatibility shim, but the rest of
            // the namespace (ConfigurationPropertyAttribute, StringValidatorAttribute, ...)
            // is genuinely used by ported provider code, so the import stays and only the
            // one type is redirected - an alias outranks a namespace import.
            if (name == "System.Configuration")
            {
                // An alias only outranks a namespace import in the SAME scope. Ported
                // provider code puts its imports INSIDE the namespace, so a file-level
                // alias loses to the nearer "using System.Configuration;" and the name
                // silently binds to the framework type again. The aliases therefore go
                // wherever this import lives.
                var target = directive.Parent is BaseNamespaceDeclarationSyntax ? ConfigurationAliasScope.Namespace
                    : ConfigurationAliasScope.File;
                configurationAliasScope = target;
                continue;
            }

            if (directive.Alias is not null || directive.Parent is null)
            {
                continue;
            }

            if (!duplicates.TryGetValue(directive.Parent, out var namesInScope))
            {
                duplicates[directive.Parent] = namesInScope = new HashSet<string>(StringComparer.Ordinal);
            }
            if (!namesInScope.Add(name))
            {
                removals.Add(directive);
            }
        }

        if (removals.Count > 0)
        {
            root = root.RemoveNodes(removals, SyntaxRemoveOptions.KeepUnbalancedDirectives)!;
        }

        // Added at file level: these are fully qualified, so they mean the same wherever
        // the file's own usings happen to live
        var kept = new List<UsingDirectiveSyntax>(root.Usings);
        var seen = new HashSet<string>(
            AllUsings(root).Select(directive => directive.Name?.ToString() ?? string.Empty),
            StringComparer.Ordinal);

        // A GlobalUsings.cs holds "global using" directives that serve the WHOLE project.
        // Injecting the compat imports there as plain file-scoped usings makes them apply
        // to that one (otherwise empty) file and to nothing else, so every file that was
        // relying on "global using System.Web.UI;" - which this pass just removed - loses
        // the type with no import of its own to fall back on. YAF.NET is built this way and
        // that is where its HtmlTextWriter and HttpContext failures came from.
        var isGlobal = root.Usings.Any(directive => directive.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword));

        foreach (var name in requiredUsings)
        {
            // Also accepts an alias written out in full ("Items = N2.Addons.X.Items"),
            // which "using {name};" renders correctly either way.
            if (seen.Add(name))
            {
                kept.Add(MakeUsing($"using {name};", isGlobal));
            }
        }

        // Aliases stay FILE-scoped even here. A "global using X = ..." and a file-level
        // "using X = ..." for the same name is CS1537 ("the alias appeared previously"),
        // and every other file in the project adds its own alias when it drops its
        // System.Web import - so emitting them globally collides with all of them at once
        // (120 errors in YAF.NET). The namespace import above is what has to be global;
        // an alias is only needed where the name is actually written.
        if (!isGlobal)
        {
            foreach (var aliasUsing in aliasUsings.Distinct())
            {
                if (seen.Add(aliasUsing))
                {
                    kept.Add(MakeUsing(aliasUsing));
                }
            }
        }

        if (configurationAliasScope == ConfigurationAliasScope.File)
        {
            kept.AddRange(ConfigurationAliases.Select(alias => MakeUsing(alias, isGlobal)));
        }

        root = root.WithUsings(SyntaxFactory.List(kept));
        return configurationAliasScope == ConfigurationAliasScope.Namespace
            ? AddNamespaceUsings(root, ConfigurationAliases)
            : root;
    }

    /// <summary>Where the System.Configuration aliases have to go to win name lookup.</summary>
    private enum ConfigurationAliasScope
    {
        None,
        File,
        Namespace,
    }

    /// <summary>
    /// ConfigurationManager and ConnectionStringSettings come from the compatibility layer;
    /// the rest of System.Configuration (ConfigurationPropertyAttribute, validators) is
    /// genuinely used by ported provider code, so the import stays and only these two
    /// names are redirected.
    /// </summary>
    private static readonly string[] ConfigurationAliases =
    [
        "using ConfigurationManager = WebForm2Blazor.Components.Compat.ConfigurationManager;",
        "using ConnectionStringSettings = WebForm2Blazor.Components.Compat.ConnectionStringSettings;",
    ];

    /// <summary>Adds usings inside every namespace declaration of the file.</summary>
    private static CompilationUnitSyntax AddNamespaceUsings(CompilationUnitSyntax root, string[] usings)
    {
        var namespaces = root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>().ToList();
        if (namespaces.Count == 0)
        {
            return root;
        }
        return root.ReplaceNodes(namespaces, (original, _) =>
        {
            var present = original.Usings
                .Select(directive => directive.ToString().Trim())
                .ToHashSet(StringComparer.Ordinal);
            // Namespace-level usings are never global (C# forbids it), so no flag here.
            var added = usings.Where(text => present.Add(text.Trim())).Select(text => MakeUsing(text));
            return original.WithUsings(original.Usings.AddRange(added));
        });
    }

    /// <summary>
    /// Builds a using directive. <paramref name="global"/> emits "global using", which is
    /// required when the directive is being added to a file whose imports are global -
    /// mixing the two there would scope the new import to that file alone.
    ///
    /// The leading newline matters only for reading the output: without it the directive is
    /// appended to the last existing line ("global using X;using WebForm2Blazor.Components;").
    /// </summary>
    private static UsingDirectiveSyntax MakeUsing(string usingStatement, bool global = false)
    {
        var text = global && !usingStatement.StartsWith("global ", StringComparison.Ordinal)
            ? "global " + usingStatement
            : usingStatement;

        return SyntaxFactory.ParseCompilationUnit(text + Environment.NewLine).Usings[0]
            .WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed);
    }

    private static CompilationUnitSyntax RewriteNamespace(CompilationUnitSyntax root, string targetNamespace)
    {
        var declaration = root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
        if (declaration is not null)
        {
            var newName = SyntaxFactory.ParseName(targetNamespace).WithTriviaFrom(declaration.Name);
            return root.ReplaceNode(declaration.Name, newName);
        }

        // Code-behind written in the global namespace (legal in WebForms) would leave its
        // partial half outside the component's namespace - the .razor always declares
        // @namespace - so the two halves never join and every base member goes missing
        if (root.Members.Count == 0)
        {
            return root;
        }

        // Tokens built by the factory carry no trivia, so the separators are explicit -
        // otherwise the output reads "namespaceX{" and no longer parses
        var wrapper = SyntaxFactory.NamespaceDeclaration(
                SyntaxFactory.ParseName(targetNamespace).WithTrailingTrivia(SyntaxFactory.LineFeed))
            .WithNamespaceKeyword(
                SyntaxFactory.Token(SyntaxKind.NamespaceKeyword).WithTrailingTrivia(SyntaxFactory.Space))
            .WithOpenBraceToken(
                SyntaxFactory.Token(SyntaxKind.OpenBraceToken).WithTrailingTrivia(SyntaxFactory.LineFeed))
            .WithCloseBraceToken(
                SyntaxFactory.Token(SyntaxKind.CloseBraceToken).WithTrailingTrivia(SyntaxFactory.LineFeed))
            .WithMembers(root.Members);
        return root.WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(wrapper));
    }

    private static ClassDeclarationSyntax RemoveWebFormsBaseType(ClassDeclarationSyntax classDeclaration)
    {
        if (classDeclaration.BaseList is null)
        {
            return classDeclaration;
        }

        var remaining = classDeclaration.BaseList.Types
            .Where(baseType => !WebFormsBaseTypes.Contains(baseType.Type.ToString().Trim()))
            .ToList();

        if (remaining.Count == classDeclaration.BaseList.Types.Count)
        {
            return classDeclaration;
        }

        if (remaining.Count == 0)
        {
            // Remove ": System.Web.UI.Page" entirely and move its trailing newline to the identifier
            return classDeclaration
                .WithBaseList(null)
                .WithIdentifier(classDeclaration.Identifier
                    .WithTrailingTrivia(classDeclaration.BaseList.GetTrailingTrivia()));
        }

        return classDeclaration.WithBaseList(
            classDeclaration.BaseList.WithTypes(SyntaxFactory.SeparatedList(remaining)));
    }

    /// <summary>
    /// Whether Blazor could actually assign this property, which is what [Parameter]
    /// promises. The renderer rejects a parameter it cannot set - "declares a parameter
    /// matching the name 'X' that is not public" - and the failure is at RUNTIME, when the
    /// component is first rendered, not at build time.
    ///
    /// Excluded: get-only and expression-bodied properties (no accessor list at all), and
    /// "private/protected/internal set". Those are computed or internally-owned values, not
    /// something markup passes in. BlogEngine's CommentList.NestingSupported is a get-only
    /// property that probes the theme directory - it was being advertised as a parameter
    /// and took /post down with it.
    /// </summary>
    private static bool IsPubliclySettable(PropertyDeclarationSyntax property)
    {
        var setter = property.AccessorList?.Accessors
            .FirstOrDefault(accessor => accessor.IsKind(SyntaxKind.SetAccessorDeclaration));

        return setter is not null
            && !setter.Modifiers.Any(SyntaxKind.PrivateKeyword)
            && !setter.Modifiers.Any(SyntaxKind.ProtectedKeyword)
            && !setter.Modifiers.Any(SyntaxKind.InternalKeyword);
    }

    /// <summary>Public properties of a user control become Blazor [Parameter]s.</summary>
    private static ClassDeclarationSyntax AddParameterAttributes(
        ClassDeclarationSyntax classDeclaration,
        ConvertedComponent component,
        string sourceName,
        ConversionReport report)
    {
        if (component.Kind != CodeBehindKind.UserControl)
        {
            return classDeclaration;
        }

        var properties = classDeclaration.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(property => property.Modifiers.Any(SyntaxKind.PublicKeyword))
            .Where(property => !property.Modifiers.Any(SyntaxKind.StaticKeyword))
            .Where(IsPubliclySettable)
            .Where(property => !property.AttributeLists
                .SelectMany(list => list.Attributes)
                .Any(attribute => attribute.Name.ToString().Contains("Parameter", StringComparison.Ordinal)))
            .ToList();

        if (properties.Count == 0)
        {
            return classDeclaration;
        }

        foreach (var property in properties)
        {
            report.Info(sourceName,
                $"公開プロパティ {property.Identifier.Text} に [Parameter] を付与しました(ユーザーコントロールのプロパティ → コンポーネントパラメータ)。");
        }

        return classDeclaration.ReplaceNodes(properties, (original, _) =>
        {
            var rewritten = (PropertyDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                "[Parameter] " + original.ToString())!;
            return rewritten.WithTriviaFrom(original);
        });
    }

    /// <summary>
    /// One control field. A plain field for most, a stand-in backed property for the ones a
    /// page can touch before they exist.
    ///
    /// WebForms built the control tree and THEN called OnInit, so "ucCommentList.Visible =
    /// x" in OnInit is ordinary code. Here the field is @ref, which Blazor assigns after
    /// the first render, so it is null during OnInit and the page dies -
    /// BlogEngine's /post did exactly that.
    ///
    /// The property hands out a stand-in until @ref delivers the real control, and the
    /// setter replays onto it whatever the page assigned in the meantime (see
    /// IDeferredControlState). Only assignments actually made are replayed.
    ///
    /// Restricted to types that can stand in: a compat control or a converted user control,
    /// both of which have a parameterless constructor and record their own assignments.
    /// "dynamic" (a stub placeholder) and anything else keeps the plain field - a stand-in
    /// that cannot record would swallow the assignment instead of deferring it, which is
    /// worse than the null it replaces.
    /// </summary>
    private static string EmitControlField(Emit.ControlField field, string indent)
    {
        if (field.LegacyHost)
        {
            return EmitLegacyHostField(field, indent);
        }

        if (!CanStandIn(field.Type))
        {
            return $"{indent}protected {field.Type} {field.Name};\r\n";
        }

        var pending = $"__{field.Name}_pending";
        var reference = $"__{field.Name}_ref";
        return $"{indent}private {field.Type} {pending};\r\n"
             + $"{indent}private {field.Type} {reference};\r\n"
             + $"{indent}protected {field.Type} {field.Name}\r\n"
             + $"{indent}{{\r\n"
             + $"{indent}    get => {reference} ?? ({pending} ??= new {field.Type}());\r\n"
             + $"{indent}    set\r\n"
             + $"{indent}    {{\r\n"
             + $"{indent}        {reference} = value;\r\n"
             + $"{indent}        {pending}?.ReplayPendingStateOnto(value);\r\n"
             + $"{indent}    }}\r\n"
             + $"{indent}}}\r\n";
    }

    /// <summary>
    /// The field for a control the markup renders through a LegacyRenderHost wrapper.
    ///
    /// @ref can only capture the component, and the component is the wrapper - so the
    /// wrapper is captured into its own field and the name the code-behind uses reaches
    /// through it to the control. Pointing the named field at the wrapper is what
    /// BlogEngine hit: "recaptcha.UserUniqueIdentifier" and "pager1.Posts" both resolved
    /// against LegacyRenderHost and threw RuntimeBinderException.
    ///
    /// Still null before the first render - the wrapper builds the instance in
    /// OnParametersSet, which is before @ref is assigned but after OnInit.
    ///
    /// Shared with the generated partial for markup that has no code-behind file
    /// (a theme's Site.master), which emits plain fields and would otherwise leave the
    /// @ref target undeclared.
    /// </summary>
    /// <summary>
    /// The field a code-behind uses for a legacy-hosted control: the host is captured by
    /// @ref and the named field reaches through it to the control instance.
    ///
    /// The field is typed with the PORTED CONTROL'S OWN TYPE where one is known, not
    /// dynamic. Dynamic compiles anything, which is why it was used - but it spreads: an
    /// expression with a dynamic operand is dynamically dispatched, and extension methods
    /// cannot be dispatched that way. YAF writes
    ///
    ///     this.GetRepository&lt;UserAlbumImage&gt;().ListPaged(..., this.PagerTop.PageSize)
    ///
    /// and one dynamic argument turned the whole call into CS1973, 44 times. A cast is
    /// what the type actually is, so nothing is lost by writing it down.
    /// </summary>
    internal static string EmitLegacyHostField(Emit.ControlField field, string indent)
        => $"{indent}private global::WebForm2Blazor.Components.LegacyRenderHost __{field.Name}_host;\r\n"
         + (field.Type == "dynamic"
             ? $"{indent}protected dynamic {field.Name} => __{field.Name}_host?.ControlInstance;\r\n"
             : $"{indent}protected {field.Type} {field.Name} => __{field.Name}_host?.ControlInstance as {field.Type};\r\n");

    /// <summary>
    /// Whether a stand-in of this type can be constructed and can record assignments.
    ///
    /// Asked of the compat assembly rather than assumed from the name: not every compat
    /// control records. ObjectDataSource derives from ComponentBase directly, so a
    /// stand-in of it would swallow assignments silently - and emitting the replay call
    /// against it does not even compile (CS1929, which is how this was caught).
    /// </summary>
    private static bool CanStandIn(string type)
    {
        if (type is "dynamic" || type.Contains('<', StringComparison.Ordinal))
        {
            return false;
        }

        var simpleName = type[(type.LastIndexOf('.') + 1)..];
        if (CompatTypeNames.Contains(simpleName))
        {
            return RecordsPendingState(simpleName);
        }

        // Anything else with a namespace is a converted user control: those are always
        // WebFormsUserControl, which records, and their type does not exist here to check.
        return type.Contains('.', StringComparison.Ordinal);
    }

    /// <summary>Whether a compat control implements IDeferredControlState.</summary>
    private static bool RecordsPendingState(string simpleName)
        => typeof(WebForm2Blazor.Components.WebFormsControlBase).Assembly
            .GetType("WebForm2Blazor.Components." + simpleName) is { } type
           && typeof(WebForm2Blazor.Components.IDeferredControlState).IsAssignableFrom(type);

    private static ClassDeclarationSyntax InsertGeneratedMembers(
        ClassDeclarationSyntax classDeclaration,
        ConvertedComponent component,
        string sourceName,
        ConversionReport report)
    {
        var indent = GetMemberIndent(classDeclaration);
        var generated = new StringBuilder();

        // Not every project keeps its control fields in a designer file. Where the
        // code-behind declares them itself, generating them again is a duplicate member,
        // and the declaration that loses is the hand-written one carrying the real type -
        // "protected dynamic rc" beside "protected Repeater rc". The source wins.
        var declaredMembers = classDeclaration.Members
            .SelectMany(member => member switch
            {
                FieldDeclarationSyntax field => field.Declaration.Variables.Select(v => v.Identifier.Text),
                PropertyDeclarationSyntax property => [property.Identifier.Text],
                _ => Enumerable.Empty<string>(),
            })
            .ToHashSet(StringComparer.Ordinal);

        // The same ID can appear more than once in markup (mutually exclusive branches,
        // tab panels); the field is declared once, as the designer did.
        var emittedFields = component.Fields
            .DistinctBy(field => field.Name)
            .Where(field => !declaredMembers.Contains(field.Name))
            .ToList();

        if (emittedFields.Count > 0)
        {
            generated.Append($"{indent}// Server controls from the .aspx (the WebForms designer.cs equivalent).\r\n");
            generated.Append($"{indent}// Instances are assigned via @ref on the .razor side.\r\n");
            foreach (var field in emittedFields)
            {
                generated.Append(EmitControlField(field, indent));
            }
            generated.Append("\r\n");
        }

        var methodNames = classDeclaration.Members
            .OfType<MethodDeclarationSyntax>()
            .Select(method => method.Identifier.Text)
            .ToHashSet(StringComparer.Ordinal);

        var hasInit = methodNames.Contains("Page_Init");
        var hasLoad = methodNames.Contains("Page_Load");
        var hasPreRender = methodNames.Contains("Page_PreRender");

        // AutoEventWireup is one way to hook the lifecycle; overriding OnLoad / OnPreRender
        // is the other, and a control library usually takes the second. Both are driven.
        //
        // Only an override counts. A class that merely INHERITS OnLoad must not have it
        // called here - the base's OnLoad is the compat layer's own, and calling it from
        // OnAfterRender would run the base lifecycle twice.
        var overridesLoad = OverridesLifecycleMethod(classDeclaration, "OnLoad");
        var overridesPreRender = OverridesLifecycleMethod(classDeclaration, "OnPreRender");

        if (hasInit || hasLoad || hasPreRender || overridesLoad || overridesPreRender)
        {
            generated.Append($"{indent}// Equivalent of the WebForms page lifecycle (Init -> Load -> PreRender).\r\n");
            generated.Append($"{indent}// In Blazor, child-component @ref values are assigned only after the first\r\n");
            generated.Append($"{indent}// render, so this is called from OnAfterRender(firstRender), not OnInitialized.\r\n");
            generated.Append($"{indent}protected override void OnAfterRender(bool firstRender)\r\n");
            generated.Append($"{indent}{{\r\n");
            generated.Append($"{indent}    if (!firstRender)\r\n");
            generated.Append($"{indent}    {{\r\n");
            generated.Append($"{indent}        return;\r\n");
            generated.Append($"{indent}    }}\r\n");
            generated.Append("\r\n");
            if (hasInit)
            {
                generated.Append($"{indent}    Page_Init(this, EventArgs.Empty);\r\n");
            }
            if (overridesLoad)
            {
                generated.Append($"{indent}    OnLoad(EventArgs.Empty);\r\n");
            }
            if (hasLoad)
            {
                generated.Append($"{indent}    Page_Load(this, EventArgs.Empty);\r\n");
            }
            generated.Append($"{indent}    MarkPageLoaded();\r\n");
            if (overridesPreRender)
            {
                generated.Append($"{indent}    OnPreRender(EventArgs.Empty);\r\n");
            }
            if (hasPreRender)
            {
                generated.Append($"{indent}    Page_PreRender(this, EventArgs.Empty);\r\n");
            }
            generated.Append($"{indent}    StateHasChanged();\r\n");
            generated.Append($"{indent}}}\r\n");
            generated.Append("\r\n");

            report.Info(sourceName,
                "ライフサイクル("
                + string.Join(" → ", new[]
                {
                    hasInit ? "Page_Init" : null,
                    overridesLoad ? "OnLoad" : null,
                    hasLoad ? "Page_Load" : null,
                    overridesPreRender ? "OnPreRender" : null,
                    hasPreRender ? "Page_PreRender" : null,
                }.Where(name => name is not null))
                + ")を OnAfterRender(firstRender) から呼び出すよう生成しました(メソッド本体は無変更)。");
        }

        if (hasPreRender)
        {
            // WebForms runs Page_PreRender after event processing, before rendering, on
            // every request. The compatibility base class calls OnPreRenderCompat when an
            // event completes.
            generated.Append($"{indent}protected override void OnPreRenderCompat()\r\n");
            generated.Append($"{indent}{{\r\n");
            generated.Append($"{indent}    Page_PreRender(this, EventArgs.Empty);\r\n");
            generated.Append($"{indent}}}\r\n");
            generated.Append("\r\n");

            report.Info(sourceName,
                "Page_PreRender をイベント処理後にも実行するよう OnPreRenderCompat を生成しました。");
        }

        if (generated.Length == 0)
        {
            return classDeclaration;
        }

        // C# 12 body-less form ("class X : Base;") cannot receive members; give it braces
        if (classDeclaration.OpenBraceToken.IsKind(SyntaxKind.None))
        {
            classDeclaration = classDeclaration
                .WithSemicolonToken(default)
                .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken)
                    .WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed))
                .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken)
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed));
        }

        var wrapper = SyntaxFactory.ParseCompilationUnit(
            $"class __GeneratedMembers\r\n{{\r\n{generated}}}\r\n");
        var generatedMembers = ((ClassDeclarationSyntax)wrapper.Members[0]).Members.ToList();

        var existingMembers = classDeclaration.Members.ToList();
        if (existingMembers.Count > 0)
        {
            // Insert a blank line between generated members and the original code for readability
            existingMembers[0] = existingMembers[0].WithLeadingTrivia(
                existingMembers[0].GetLeadingTrivia().Insert(0, SyntaxFactory.CarriageReturnLineFeed));
        }

        return classDeclaration.WithMembers(SyntaxFactory.List(generatedMembers.Concat(existingMembers)));
    }

    /// <summary>
    /// True when this class DECLARES an override of the named lifecycle method.
    ///
    /// The declaration has to be here, with the "override" keyword, and take one argument -
    /// an inherited OnLoad belongs to the compat base, which drives itself, and a same-named
    /// helper that is not an override is not a lifecycle hook at all.
    /// </summary>
    private static bool OverridesLifecycleMethod(ClassDeclarationSyntax classDeclaration, string name) =>
        classDeclaration.Members
            .OfType<MethodDeclarationSyntax>()
            .Any(method =>
                method.Identifier.Text == name
                && method.ParameterList.Parameters.Count == 1
                && method.Modifiers.Any(modifier => modifier.RawKind == (int)SyntaxKind.OverrideKeyword));

    private static void ReportUnsupportedLifecycle(
        ClassDeclarationSyntax classDeclaration,
        string sourceName,
        ConversionReport report)
    {
        foreach (var method in classDeclaration.Members.OfType<MethodDeclarationSyntax>())
        {
            if (UnsupportedLifecycleMethods.Contains(method.Identifier.Text))
            {
                report.Residual(sourceName, ResidualKind.PageLifecycle,
                    $"{method.Identifier.Text} は自動変換の対象外です(呼び出し元が生成されないため、そのまま残しました)。");
            }
        }
    }

    private static string GetMemberIndent(ClassDeclarationSyntax classDeclaration)
    {
        var classIndent = classDeclaration.GetLeadingTrivia()
            .Reverse()
            .TakeWhile(trivia => trivia.IsKind(SyntaxKind.WhitespaceTrivia))
            .Select(trivia => trivia.ToString())
            .FirstOrDefault() ?? string.Empty;

        return classIndent + "    ";
    }
}
