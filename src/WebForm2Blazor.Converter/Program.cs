using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using WebForm2Blazor.Converter;
using WebForm2Blazor.Converter.Convert;
using WebForm2Blazor.Converter.Emit;
using WebForm2Blazor.Converter.Project;

// --- Property coverage audit mode ---
// Cross-references the defaults catalog the PropertyCatalog tool sampled from the real
// runtime against this tool's coverage and enumerates unsupported properties.
// --- Build verification mode ---
// Builds a converted project and classifies the diagnostics. Residual count says
// nothing about whether the output compiles; this makes that a repeatable metric.
if (args.Length >= 2 && args[0] == "--verify-build")
{
    var verifyTarget = args[1];
    var verifyReport = args.Length >= 4 && args[2] == "--output" ? args[3] : null;
    return WebForm2Blazor.Converter.Verify.BuildVerifier.Run(verifyTarget, verifyReport);
}

// --- AI residual layer (layer 3) ---
// Generates one task per residual-carrying file, then applies the answers behind the
// build + parity gates. The model call happens outside this tool on purpose: the gate,
// not the generator, is what makes the layer trustworthy.
if (args.Length >= 2 && args[0] == "--ai-tasks")
{
    var aiOutput = args[1];
    var aiLimit = 20;
    string? aiKind = null;
    for (var i = 2; i + 1 < args.Length; i += 2)
    {
        if (args[i] == "--limit") aiLimit = int.Parse(args[i + 1]);
        else if (args[i] == "--kind") aiKind = args[i + 1];
    }
    return WebForm2Blazor.Converter.Ai.AiResidualLayer.GeneratePrompts(aiOutput, aiLimit, aiKind);
}

if (args.Length >= 2 && args[0] == "--ai-apply")
{
    var aiOutput = args[1];
    var parity = args.Length >= 4 && args[2] == "--parity" ? args[3] : null;
    return WebForm2Blazor.Converter.Ai.AiResidualLayer.ApplyAnswers(aiOutput, parity);
}

if (args.Length >= 2 && args[0] == "--coverage")
{
    var catalogPath = args[1];
    var coverageOutput = args.Length >= 4 && args[2] == "--output" ? args[3] : "PROPERTY-COVERAGE.md";
    var markdown = WebForm2Blazor.Converter.Coverage.CoverageAuditor.BuildReport(catalogPath);
    File.WriteAllText(coverageOutput, markdown);
    Console.WriteLine($"カバレッジ監査レポート: {coverageOutput}");
    return 0;
}

string? input = null;
string? output = null;
string? appName = null;
string? componentsReference = null;
string? controlMapPath = null;
string? expressionMapPath = null;
string? propertyCatalogPath = null;
string? webConfigOverride = null;
var includeDirectories = new List<string>();
var port = 5080;

for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--input": input = args[++i]; break;
        case "--output": output = args[++i]; break;
        case "--name": appName = args[++i]; break;
        case "--components-ref": componentsReference = args[++i]; break;
        case "--port": port = int.Parse(args[++i]); break;
        case "--control-map": controlMapPath = args[++i]; break;
        case "--expression-map": expressionMapPath = args[++i]; break;
        case "--catalog": propertyCatalogPath = args[++i]; break;
        case "--include": includeDirectories.Add(args[++i]); break;
        case "--web-config": webConfigOverride = args[++i]; break;
        default:
            Console.Error.WriteLine($"不明な引数: {args[i]}");
            return 1;
    }
}

if (input is null || output is null || appName is null || componentsReference is null)
{
    Console.Error.WriteLine("""
        使い方:
          WebForm2Blazor.Converter
            --input <WebFormsプロジェクトのディレクトリ>
            --output <出力ディレクトリ>
            --name <生成する Blazor プロジェクト名>
            --components-ref <出力csprojから互換コンポーネントcsprojへの相対パス>
            [--port <開発サーバーのポート。既定 5080>]
        """);
    return 1;
}

var report = new ConversionReport();

// User-supplied mappings for custom / third-party controls (hand-ported components)
if (controlMapPath is not null)
{
    var loaded = WebForm2Blazor.Converter.Mapping.ControlMappings.LoadExternal(controlMapPath);
    report.Info("(project)", $"--control-map から {loaded} 件のカスタムコントロールマッピングを読み込みました。");
}

// User-supplied expression-builder templates (<%$ Prefix:Value %> for custom builders)
if (expressionMapPath is not null)
{
    var loaded = WebForm2Blazor.Converter.Emit.ExpressionBuilders.LoadExternal(expressionMapPath);
    report.Info("(project)", $"--expression-map から {loaded} 件の式ビルダー変換を読み込みました。");
}

// The property catalog makes expando-attribute decisions exactly like WebForms
// (by property existence). Defaults to the catalog next to the current directory.
propertyCatalogPath ??= File.Exists("webforms-property-catalog.json") ? "webforms-property-catalog.json" : null;
if (propertyCatalogPath is not null && File.Exists(propertyCatalogPath))
{
    WebForm2Blazor.Converter.Mapping.PropertyKnowledge.Load(propertyCatalogPath);
    report.Info("(project)", $"プロパティカタログを読み込みました: {propertyCatalogPath}(expando 属性判定を実プロパティ基準で行います)。");
}

var project = WebFormsProject.Scan(input, includeDirectories, webConfigOverride);

report.Info("(project)",
    $"棚卸し: マスターページ {project.MasterPages.Count} / ページ {project.Pages.Count} / "
    + $"ユーザーコントロール {project.UserControls.Count} / その他 .cs {project.PlainCodeFiles.Count}");

if (project.Pages.Count == 0 && project.MasterPages.Count == 0 && project.UserControls.Count == 0)
{
    report.Error("(project)", $"{input} に変換対象 (.aspx / .ascx / .master) が見つかりません。");
}

// --- Conversion (no files are written at this point) ---

// Custom page/control base classes (BaseNopPage etc.) defined in plain code files
// and in --include library projects (ForumPage / PortalModuleBase live there)
var baseRegistry = BaseClassRegistry.Build(
    project.PlainCodeFiles.Concat(project.IncludedCodeFiles.Select(file => file.AbsolutePath)));
if (baseRegistry.Count > 0)
{
    report.Info("(project)", $"独自基底クラスを {baseRegistry.Count} 個検出しました(基底連鎖を互換基底クラスへ組み替えます)。");
}

var userControlRegistry = AspxConverters.BuildUserControlRegistry(project, appName);
var components = new List<ConvertedComponent>();
var masters = new Dictionary<string, MasterInfo>(StringComparer.OrdinalIgnoreCase);

// Nested masters must follow their parent: the child reads the parent's placeholders
foreach (var masterPath in AspxConverters.OrderMastersByDependency(project))
{
    var (component, info) = AspxConverters.ConvertMaster(
        masterPath, project, userControlRegistry, masters, baseRegistry, appName, report);
    components.Add(component);
    masters[project.RelativePath(masterPath)] = info;
}

foreach (var userControlPath in project.UserControls)
{
    components.Add(AspxConverters.ConvertUserControl(userControlPath, project, userControlRegistry, baseRegistry, appName, report));
}

foreach (var pagePath in project.Pages)
{
    components.Add(AspxConverters.ConvertPage(pagePath, project, userControlRegistry, masters, baseRegistry, appName, report));
}

// --- Output ---

CleanGeneratedOutput(output);

var singleMaster = masters.Count == 1 ? masters.Values.First() : null;

// The <title> lives in the outermost master, so it must not be tied to there being
// exactly one: with nested masters the root holds it and every other master has none.
// Masters are inserted in dependency order, so the first one carrying a title is the
// outermost that declares one.
// A title holding inline code (<%: ... %>) is computed per request; the scaffolded
// site title is a static string, so such a master is passed over rather than leaked raw.
var siteTitle = masters.Values
    .Select(master => master.Title)
    .FirstOrDefault(title => !string.IsNullOrWhiteSpace(title) && !title.Contains("<%", StringComparison.Ordinal));

new BlazorScaffolder(new ScaffoldOptions
{
    ProjectName = appName,
    ComponentsProjectReference = componentsReference,
    DefaultLayoutComponent = singleMaster?.FullName ?? $"{appName}.Components.Layout.MainLayout",
    EmitFallbackLayout = singleMaster is null,
    SiteTitle = siteTitle ?? string.Empty,
    Port = port,
}).Scaffold(output);

if (masters.Count > 1)
{
    report.Residual("(project)", ResidualKind.Structure,
        $"マスターページが {masters.Count} 個あります。既定レイアウトは MainLayout(素通し)にし、"
        + "各ページには @layout を個別に付けました。", disposition: ResidualDisposition.Informational);
}

// Qualified references written against a component's ORIGINAL namespace
// (Old.Web.Admin.Modules.SomeControl) must follow the re-namespacing. Map original ->
// target namespace where the mapping is unambiguous and no ported plain code stays there.
var namespaceMap = new Dictionary<string, List<string>>(StringComparer.Ordinal);
foreach (var component in components)
{
    if (component.CodeBehindSourcePath is null)
    {
        continue;
    }
    var namespaceMatch = System.Text.RegularExpressions.Regex.Match(
        File.ReadAllText(component.CodeBehindSourcePath), @"namespace\s+([A-Za-z_][\w.]*)");
    if (!namespaceMatch.Success)
    {
        continue;
    }
    var originalNamespace = namespaceMatch.Groups[1].Value;
    if (originalNamespace == component.TargetNamespace || baseRegistry.HasNamespace(originalNamespace))
    {
        continue;
    }
    if (!namespaceMap.TryGetValue(originalNamespace, out var targets))
    {
        namespaceMap[originalNamespace] = targets = [];
    }
    targets.Add(component.TargetNamespace);
}
var namespaceRewrites = namespaceMap.OrderByDescending(pair => pair.Key.Length).ToList();

string ApplyNamespaceMap(string code, bool razorContent = false)
{
    _ = razorContent;
    foreach (var (originalNamespace, targets) in namespaceRewrites)
    {
        var distinct = targets.Distinct(StringComparer.Ordinal).ToList();
        if (distinct.Count == 1)
        {
            // Qualified type references follow only when the mapping is unambiguous
            code = code.Replace(originalNamespace + ".", distinct[0] + ".");
            code = code.Replace($"using {originalNamespace};", $"using {distinct[0]};");
        }
        else
        {
            // Conflicting targets (pages and user controls shared the namespace):
            // importing the component namespaces invites short-name ambiguities, and
            // the original namespace no longer exists - drop the dead using
            code = code.Replace($"using {originalNamespace};", string.Empty);
        }
    }
    return code;
}

var portedNamespaces = new HashSet<string>(StringComparer.Ordinal);

// Two-phase porting with exclusion cascade: a file excluded for a Framework-only
// namespace (a web-service proxy etc.) can leave a namespace with no remaining types;
// files importing such a namespace cannot compile either and are excluded in rounds.
var portCandidates = project.PlainCodeFiles
    .Select(codeFile => (
        ReportName: project.RelativePath(codeFile),
        OutputRelative: project.RelativePath(codeFile),
        Source: File.ReadAllText(codeFile),
        Included: false))
    .Concat(project.IncludedCodeFiles.Select(included => (
        ReportName: included.OutputRelativePath,
        OutputRelative: included.OutputRelativePath,
        Source: File.ReadAllText(included.AbsolutePath),
        Included: true)))
    .ToList();

var candidateNamespaces = portCandidates
    .Select(candidate => (candidate,
        Declared: System.Text.RegularExpressions.Regex.Matches(candidate.Source, @"namespace\s+([A-Za-z_][\w.]*)")
            .Select(match => match.Groups[1].Value).Distinct(StringComparer.Ordinal).ToList()))
    .ToList();

var excludedCandidates = new HashSet<int>();
for (var i = 0; i < candidateNamespaces.Count; i++)
{
    if (FindUnportableNamespace(candidateNamespaces[i].candidate.Source) is { } unportable)
    {
        excludedCandidates.Add(i);
        report.Residual(candidateNamespaces[i].candidate.ReportName, ResidualKind.CodeBehind,
            $".NET Framework 専用の名前空間 {unportable} を使用しているため移植から除外しました(認証/ルーティング等の基盤コードは手動移行が必要)。", disposition: ResidualDisposition.ManualMigration);
        continue;
    }

    if (PortabilityRules.FindFrameworkIntegrationNamespace(candidateNamespaces[i].Declared) is { } integration)
    {
        excludedCandidates.Add(i);
        report.Residual(candidateNamespaces[i].candidate.ReportName, ResidualKind.CodeBehind,
            $"名前空間 {integration} は .NET に存在しないライブラリの統合コードのため移植から除外しました(手動移行が必要)。",
            disposition: ResidualDisposition.ManualMigration);
    }
}

// Type name -> the namespaces declaring it, per candidate. A namespace usually survives
// its excluded files, so the cascade has to follow individual TYPES as well: a file using
// a type whose only declaration was excluded cannot compile either.
var declaredTypes = candidateNamespaces
    .Select(entry => System.Text.RegularExpressions.Regex
        .Matches(entry.candidate.Source, @"\b(?:class|struct|interface|enum|record)\s+([A-Za-z_]\w*)")
        .Select(match => match.Groups[1].Value)
        .Distinct(StringComparer.Ordinal)
        .SelectMany(type => entry.Declared.Select(ns => (Type: type, Namespace: ns)))
        .ToList())
    .ToList();

/// <summary>Types every declaring file of which has been excluded, with their namespace.</summary>
List<(string Type, string Namespace)> ComputeGoneTypes()
{
    var surviving = new HashSet<string>(StringComparer.Ordinal);
    for (var index = 0; index < declaredTypes.Count; index++)
    {
        if (!excludedCandidates.Contains(index))
        {
            foreach (var declared in declaredTypes[index])
            {
                surviving.Add(declared.Namespace + "." + declared.Type);
            }
        }
    }

    var gone = new List<(string, string)>();
    for (var index = 0; index < declaredTypes.Count; index++)
    {
        if (!excludedCandidates.Contains(index))
        {
            continue;
        }
        foreach (var declared in declaredTypes[index])
        {
            if (!surviving.Contains(declared.Namespace + "." + declared.Type))
            {
                gone.Add((declared.Type, declared.Namespace));
            }
        }
    }
    return gone;
}

/// <summary>
/// Whether the source names a type that is gone, QUALIFIED by its namespace. Matching a
/// bare type name as well was tried and cascades out of control: one gone type in a
/// widely imported namespace then drags out every file that happens to use the word.
/// </summary>
static bool UsesGoneType(string source, (string Type, string Namespace) gone)
{
    // Qualified, in full or partially: C# lets "BlogEngine.Core.FileSystem.FileStoreFile"
    // be written as "FileSystem.FileStoreFile" from inside BlogEngine.Core, so every
    // suffix of the namespace is a possible spelling
    var segments = gone.Namespace.Split('.');
    for (var start = 0; start < segments.Length; start++)
    {
        var qualified = string.Join('.', segments[start..]) + "." + gone.Type;
        if (source.Contains(qualified, StringComparison.Ordinal))
        {
            return true;
        }
    }

    return false;
}

// Unqualified uses of a gone type are deliberately NOT followed. Three variants were
// measured on this corpus - any text, any parsed identifier, identifiers in type position,
// each further limited to files of the same namespace - and every one cascaded out of
// control (1 remaining error became 28, then 705): a single gone type in a shared
// namespace drags out base classes whole page trees are built on. A qualified name is
// unambiguous evidence of a dependency; a bare one is not worth the blast radius.

HashSet<string> ComputeFullyExcludedNamespaces()
    => candidateNamespaces
        .SelectMany((entry, index) => entry.Declared.Select(ns => (ns, index)))
        .GroupBy(pair => pair.ns, StringComparer.Ordinal)
        .Where(group => group.All(pair => excludedCandidates.Contains(pair.index)))
        .Select(group => group.Key)
        .ToHashSet(StringComparer.Ordinal);

// Cascade: a namespace counts as gone only when EVERY file declaring it is excluded
// (shared namespaces with surviving files must keep their importers)
bool cascadeChanged;
do
{
    cascadeChanged = false;
    var emptyNamespaces = ComputeFullyExcludedNamespaces();
    var goneTypes = ComputeGoneTypes();
    if (emptyNamespaces.Count == 0 && goneTypes.Count == 0)
    {
        break;
    }

    for (var i = 0; i < candidateNamespaces.Count; i++)
    {
        if (excludedCandidates.Contains(i))
        {
            continue;
        }

        var source = candidateNamespaces[i].candidate.Source;
        var dependency = EnumerateUsingNamespaces(source)
            .FirstOrDefault(ns => emptyNamespaces.Contains(ns));
        if (dependency is not null)
        {
            excludedCandidates.Add(i);
            cascadeChanged = true;
            report.Residual(candidateNamespaces[i].candidate.ReportName, ResidualKind.CodeBehind,
                $"移植から除外済みの名前空間 {dependency} に依存するため、連鎖して除外しました。", disposition: ResidualDisposition.ManualMigration);
            continue;
        }

        if (goneTypes.FirstOrDefault(gone => UsesGoneType(source, gone)) is { Type.Length: > 0 } goneType)
        {
            excludedCandidates.Add(i);
            cascadeChanged = true;
            report.Residual(candidateNamespaces[i].candidate.ReportName, ResidualKind.CodeBehind,
                $"移植から除外済みの型 {goneType.Namespace}.{goneType.Type} に依存するため、連鎖して除外しました。", disposition: ResidualDisposition.ManualMigration);
        }
    }
} while (cascadeChanged);

var fullyExcludedNamespaces = ComputeFullyExcludedNamespaces();

foreach (var component in components)
{
    var directory = Path.Combine(output, component.OutputDirectory.Replace('/', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(directory);

    // A page / user control whose code-behind depends on Framework-only infrastructure
    // (OWIN auth etc.) or on a namespace whose files were all excluded cannot work by
    // porting - emit an honest placeholder that keeps the route and the component name
    string? unportablePage = null;
    if (component.Kind is not CodeBehindKind.Layout && component.CodeBehindSourcePath is not null)
    {
        var codeBehindSource = File.ReadAllText(component.CodeBehindSourcePath);
        unportablePage = FindUnportableNamespace(codeBehindSource)
            ?? EnumerateUsingNamespaces(codeBehindSource)
                .FirstOrDefault(ns => fullyExcludedNamespaces.Contains(ns));
    }
    if (unportablePage is not null)
    {
        report.Residual(project.RelativePath(component.CodeBehindSourcePath), ResidualKind.CodeBehind,
            $".NET Framework 専用の名前空間 {unportablePage} に依存するため、ページ全体をプレースホルダー化しました(認証/OWIN 等は手動移行が必要)。", disposition: ResidualDisposition.ManualMigration);
        File.WriteAllText(Path.Combine(directory, component.ComponentName + ".razor"),
            GenerateUnportablePlaceholder(component, unportablePage));
        continue;
    }

    LintGeneratedRazor(component, report);
    File.WriteAllText(Path.Combine(directory, component.ComponentName + ".razor"),
        ApplyNamespaceMap(
            StripDeadUsings(component.RazorContent, fullyExcludedNamespaces, report, component.ComponentName),
            razorContent: true));

    if (component.CodeBehindSourcePath is not null)
    {
        var sourceName = project.RelativePath(component.CodeBehindSourcePath);
        // Page code-behind holds fields of user controls, so add the namespaces of the
        // user controls this component actually references to the usings
        var rewritten = CodeBehindRewriter.Rewrite(
            File.ReadAllText(component.CodeBehindSourcePath), component, sourceName, report,
            component.UsedControlNamespaces, baseRegistry);
        File.WriteAllText(Path.Combine(directory, component.ComponentName + ".razor.cs"),
            ApplyNamespaceMap(rewritten));
    }
    else if (component.Fields.Count > 0)
    {
        // Even a markup-only page (no code-behind) needs a receptacle for @ref
        File.WriteAllText(
            Path.Combine(directory, component.ComponentName + ".razor.cs"),
            GenerateFieldOnlyCodeBehind(component));
        report.Info(component.ComponentName, "コードビハインドがないため、コントロールのフィールドのみを生成しました。");
    }
}

for (var i = 0; i < candidateNamespaces.Count; i++)
{
    if (excludedCandidates.Contains(i))
    {
        continue;
    }
    var (candidate, _) = candidateNamespaces[i];
    CollectUsingNamespaces(candidate.Source, portedNamespaces);
    var destination = Path.Combine(output, candidate.OutputRelative.Replace('/', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
    File.WriteAllText(destination,
        ApplyNamespaceMap(CodeBehindRewriter.RewritePlainCodeFile(candidate.Source, candidate.ReportName, report)));
    report.CopiedCodeFiles++;
    if (!candidate.Included)
    {
        report.Info(candidate.ReportName, "業務ロジックとしてそのまま移植しました(using のみ差し替え)。");
    }
}

// Excluding a file takes its TYPES with it, and every surviving file naming one of them
// stops compiling. Chasing those consumers out of the port too was measured and cascades
// out of control, so the declarations are re-created empty instead: the code that only
// mentions the type compiles, and what actually used its members fails at exactly the
// places that need hand-migration.
var excludedTypeStubs = GenerateExcludedTypeStubs(
    [.. Enumerable.Range(0, candidateNamespaces.Count)
        .Where(excludedCandidates.Contains)
        .Select(index => candidateNamespaces[index].candidate.Source)],
    [.. Enumerable.Range(0, candidateNamespaces.Count)
        .Where(index => !excludedCandidates.Contains(index))
        .Select(index => candidateNamespaces[index].candidate.Source)],
    out var stubbedTypeCount);

if (stubbedTypeCount > 0)
{
    File.WriteAllText(Path.Combine(output, "ExcludedTypeStubs.g.cs"), ApplyNamespaceMap(excludedTypeStubs));
    report.Residual("(project)", ResidualKind.CodeBehind,
        $"移植から除外したファイルが宣言していた型 {stubbedTypeCount} 個を空のスタブとして生成しました"
        + "(ExcludedTypeStubs.g.cs)。参照側はコンパイルできますが、メンバーの実装は手動移行が必要です。", disposition: ResidualDisposition.ManualMigration);
}

// Placeholder components for unmapped controls: the output compiles and the missing
// pieces are visible on the page (each is also recorded as a residual)
var stubComponents = components
    .SelectMany(component => component.StubComponents)
    .GroupBy(pair => pair.Value, StringComparer.OrdinalIgnoreCase)
    .Select(group => (Name: group.Key, Tag: group.First().Key))
    .ToList();
if (stubComponents.Count > 0)
{
    var stubDirectory = Path.Combine(output, "Components", "Stubs");
    Directory.CreateDirectory(stubDirectory);
    foreach (var stub in stubComponents)
    {
        File.WriteAllText(Path.Combine(stubDirectory, stub.Name + ".razor"), GenerateStubComponent(appName, stub.Name, stub.Tag));
    }
    report.Info("(project)", $"未対応コントロール {stubComponents.Count} 種のプレースホルダを Components/Stubs に生成しました。");
}

// Library projects supplied via --include: ported with the same using/base rewriting,
// preserving each library's folder under the output root
if (project.IncludedCodeFiles.Count > 0)
{
    report.Info("(project)", $"--include のライブラリソース {project.IncludedCodeFiles.Count} ファイルを移植対象にしました(除外分は残差参照)。");
}

// App_GlobalResources: the .resx files back <%$ Resources: Class, Key %>. Copying them
// into a Resources folder is enough - the SDK embeds **/*.resx automatically, and the
// runtime lookup finds them by manifest name.
if (project.GlobalResourceFiles.Count > 0)
{
    var resourceDirectory = Path.Combine(output, "Resources");
    Directory.CreateDirectory(resourceDirectory);
    foreach (var resourceFile in project.GlobalResourceFiles)
    {
        File.Copy(resourceFile, Path.Combine(resourceDirectory, Path.GetFileName(resourceFile)), overwrite: true);
    }
    // WebForms also compiles each .resx into a strongly-typed class ("Resources.labels.Home")
    // that code-behind uses directly, so copying the .resx alone leaves every such
    // reference unresolved. The generated class forwards to the same runtime lookup.
    var generated = 0;
    foreach (var resourceFile in project.GlobalResourceFiles)
    {
        var className = Path.GetFileNameWithoutExtension(resourceFile);
        var keys = ReadResourceKeys(resourceFile);
        if (keys.Count == 0)
        {
            continue;
        }

        var builder = new StringBuilder();
        builder.AppendLine("// Generated from App_GlobalResources: the WebForms equivalent of the");
        builder.AppendLine("// strongly-typed class the ASP.NET build produced for this .resx.");
        builder.AppendLine("namespace Resources");
        builder.AppendLine("{");
        builder.AppendLine($"    public static class {SanitizeTypeName(className)}");
        builder.AppendLine("    {");
        foreach (var key in keys)
        {
            builder.AppendLine($"        public static string {SanitizeTypeName(key)}");
            builder.AppendLine("            => global::WebForm2Blazor.Components.GlobalResources.GetString("
                + $"\"{className}\", \"{key}\");");
        }
        builder.AppendLine("    }");
        builder.AppendLine("}");

        File.WriteAllText(Path.Combine(resourceDirectory, className + ".Generated.cs"), builder.ToString());
        generated++;
    }

    report.Info("(project)",
        $"App_GlobalResources の .resx を {project.GlobalResourceFiles.Count} 件移植し、"
        + $"強く型付けされたクラスを {generated} 件生成しました"
        + "(<%$ Resources: Class, Key %> とコードビハインドの Resources.Class.Key の両方が解決されます)。");
}

if (project.CultureResourceFiles.Count > 0)
{
    report.Residual("(project)", ResidualKind.Configuration,
        $"言語別リソース {project.CultureResourceFiles.Count} 件は移植していません。"
        + "サテライトアセンブリ化(または IStringLocalizer への移行)が必要です。", disposition: ResidualDisposition.ManualMigration);
}

// NuGet references: what the original projects declared (csproj PackageReference /
// packages.config) carries over first, then references implied by the ported code's
// usings (EF6, JSON.NET etc.). Framework-only web packages are skipped with a report.
var packageReferences = CollectDeclaredPackages([input, .. includeDirectories], report)
    .Concat(ResolvePackageReferences(portedNamespaces))
    .DistinctBy(package => package.Id, StringComparer.OrdinalIgnoreCase)
    .ToList();
if (packageReferences.Count > 0)
{
    var csprojPath = Path.Combine(output, appName + ".csproj");
    var csprojText = File.ReadAllText(csprojPath);
    var packageItems = string.Join(Environment.NewLine, packageReferences.Select(package =>
        $"    <PackageReference Include=\"{package.Id}\" Version=\"{package.Version}\" />"));
    csprojText = csprojText.Replace("</Project>",
        $"  <ItemGroup>{Environment.NewLine}{packageItems}{Environment.NewLine}  </ItemGroup>{Environment.NewLine}{Environment.NewLine}</Project>");
    File.WriteAllText(csprojPath, csprojText);
    report.Info("(project)",
        "移植コードの using から NuGet 参照を追加しました: " + string.Join(", ", packageReferences.Select(package => package.Id)));
}

var appSettingsJson = project.WebConfigPath is not null
    ? WebConfigConverter.Convert(project.WebConfigPath, project.RelativePath(project.WebConfigPath), report)
    : """
      {
        "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
        "AllowedHosts": "*"
      }
      """;
File.WriteAllText(Path.Combine(output, "appsettings.json"), appSettingsJson);

// Smoke-scenario auto-generation (verification layer 2).
// The converter knows every route, control ID, and event presence from the syntax
// tree, so it can deterministically emit a scenario that "opens every page, asserts
// every control's presence, and fires every event". Executed by
// tools/WebForm2Blazor.BrowserSmokeTest --auto.
var smokeScenario = new
{
    pages = components
        .Where(component => component.Kind == CodeBehindKind.Page && component.Routes.Count > 0)
        .Select(component => new
        {
            name = component.ComponentName,
            route = component.Routes[0],
            controls = component.SmokeControls.Select(control => new
            {
                id = control.Id,
                type = control.Type,
                assertPresence = control.AssertPresence,
                click = control.Click,
                change = control.Change,
                fill = control.Fill,
            }),
        }),
};
var smokeScenarioPath = Path.Combine(output, "smoke-scenario.json");
File.WriteAllText(smokeScenarioPath, JsonSerializer.Serialize(smokeScenario, new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
}));
report.Info("(project)", $"スモークシナリオを自動生成しました: smoke-scenario.json");

// Task bundle for the AI residual layer: one task per SOURCE FILE that still has
// residuals, carrying the paths of the original markup, the generated component and its
// code-behind. The AI layer works one residual at a time and its output is accepted only
// when the parity harness still matches - build-green alone is not a sufficient gate
// (a change can compile and still break the page at runtime).
var componentBySource = components
    .Where(component => component.CodeBehindSourcePath is not null)
    .GroupBy(component => project.RelativePath(component.CodeBehindSourcePath!)
        .Replace(".aspx.cs", ".aspx", StringComparison.OrdinalIgnoreCase)
        .Replace(".ascx.cs", ".ascx", StringComparison.OrdinalIgnoreCase)
        .Replace(".master.cs", ".master", StringComparison.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase)
    .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

var aiTasks = report.Residuals
    .GroupBy(residual => residual.Source, StringComparer.OrdinalIgnoreCase)
    .Select(group =>
    {
        var component = componentBySource.GetValueOrDefault(group.Key);
        var sourcePath = Path.Combine(project.RootDirectory,
            group.Key.Replace('/', Path.DirectorySeparatorChar));
        return new
        {
            source = group.Key,
            sourcePath = File.Exists(sourcePath) ? sourcePath : null,
            componentName = component?.ComponentName,
            generatedRazor = component is null
                ? null
                : Path.Combine(output, component.OutputDirectory.Replace('/', Path.DirectorySeparatorChar),
                    component.ComponentName + ".razor"),
            generatedCodeBehind = component is null
                ? null
                : Path.Combine(output, component.OutputDirectory.Replace('/', Path.DirectorySeparatorChar),
                    component.ComponentName + ".razor.cs"),
            originalCodeBehind = component?.CodeBehindSourcePath,
            residuals = group.Select(residual => new
            {
                kind = residual.Kind.ToString(),
                kindDescription = ConversionReport.KindDescription(residual.Kind),
                disposition = residual.Disposition.ToString(),
                dispositionDescription = ConversionReport.DescribeDisposition(residual.Disposition),
                line = residual.Line,
                message = residual.Message,
            }).ToList(),
        };
    })
    .OrderByDescending(task => task.residuals.Count)
    .ToList();

var aiTasksPath = Path.Combine(output, "AI-TASKS.json");
File.WriteAllText(aiTasksPath, JsonSerializer.Serialize(new
{
    note = "AI 残差変換層への入力。1 残差 = 1 タスクとして扱い、"
           + "受け入れ判定はビルド成功ではなく ParityTest の一致をゲートにすること。",
    taskCount = aiTasks.Sum(task => task.residuals.Count),
    files = aiTasks,
}, new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
}));
report.Info("(project)",
    $"AI 残差層向けタスク束を生成しました: AI-TASKS.json({aiTasks.Count} ファイル)");

var reportPath = Path.Combine(output, "CONVERSION-REPORT.md");
File.WriteAllText(reportPath, report.ToMarkdown());

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine(report.ToConsoleSummary());
Console.WriteLine($"レポート: {reportPath}");

return report.HasErrors ? 2 : 0;

/// <summary>
/// Removes only generated artifacts (bin / obj are build caches and are kept).
/// Under OneDrive the sync process can transiently hold a handle and make deletion
/// fail, so files are deleted one by one with retries.
/// </summary>
static void CleanGeneratedOutput(string outputDirectory)
{
    if (!Directory.Exists(outputDirectory))
    {
        return;
    }

    var targets = Directory.GetDirectories(outputDirectory)
        .Where(directory =>
        {
            var name = Path.GetFileName(directory);
            return !name.Equals("bin", StringComparison.OrdinalIgnoreCase)
                   && !name.Equals("obj", StringComparison.OrdinalIgnoreCase);
        })
        .ToList();

    // Delete files reliably (with retries). Deleting the directories themselves can
    // fail when OneDrive holds a handle, but they are overwritten at generation time,
    // so it is fine to continue on failure.
    for (var attempt = 1; attempt <= 5; attempt++)
    {
        try
        {
            foreach (var directory in targets.Where(Directory.Exists))
            {
                foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
            }

            foreach (var file in Directory.GetFiles(outputDirectory))
            {
                File.Delete(file);
            }
            break;
        }
        catch (IOException) when (attempt < 5)
        {
            Thread.Sleep(500 * attempt);
        }
        catch (UnauthorizedAccessException) when (attempt < 5)
        {
            Thread.Sleep(500 * attempt);
        }
    }

    foreach (var directory in targets.Where(Directory.Exists))
    {
        try
        {
            Directory.Delete(directory, recursive: true);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"警告: {directory} を削除できませんでした(中身は消去済みのため続行します)。");
        }
    }
}

/// <summary>
/// Last line of defense against silent leak-through: converted Razor must never contain
/// raw ASPX remnants. Anything the pipeline failed to recognize (comment-truncation
/// bugs, unknown expression forms, ...) surfaces here as a residual instead of ending
/// up as literal text on the page.
/// </summary>
/// <summary>
/// Placeholder for a page / user control whose code-behind cannot be ported. Keeps the
/// routes and the component name (parent markup and menus keep working) and absorbs any
/// attributes / child content the parent passes.
/// </summary>
static string GenerateUnportablePlaceholder(ConvertedComponent component, string unportableNamespace)
{
    var builder = new System.Text.StringBuilder();
    foreach (var route in component.Routes ?? [])
    {
        builder.AppendLine($"@page \"{route}\"");
    }
    builder.AppendLine($"@namespace {component.TargetNamespace}");
    builder.AppendLine();
    builder.AppendLine("<div class=\"w2b-unported\" style=\"border:2px dashed #cc0000; padding:1em; margin:0.5em 0;\">");
    builder.AppendLine($"    <strong>{component.ComponentName}</strong>: このコンポーネントは .NET Framework 専用 API({unportableNamespace})に依存しているため自動変換できません。手動移行が必要です。");
    builder.AppendLine("</div>");
    builder.AppendLine();
    builder.AppendLine("@code {");
    builder.AppendLine("    [Parameter(CaptureUnmatchedValues = true)]");
    builder.AppendLine("    public Dictionary<string, object> AdditionalAttributes { get; set; }");
    builder.AppendLine();
    builder.AppendLine("    [Parameter] public RenderFragment ChildContent { get; set; }");
    builder.AppendLine("}");
    return builder.ToString();
}

/// <summary>
/// Detects .NET Framework-only infrastructure namespaces (OWIN auth, bundling,
/// Web API/MVC, routing config) that cannot be ported as-is. Files using them are
/// excluded with a residual instead of breaking the whole build.
/// </summary>
static string? FindUnportableNamespace(string source)
{
    foreach (var ns in EnumerateUsingNamespaces(source))
    {
        if (PortabilityRules.IsFrameworkOnly(ns))
        {
            return ns;
        }
    }

    // Generated proxies (WCF "Service References") write every type fully qualified and
    // import nothing, so a using-only scan sees a portable file and lets it through
    return PortabilityRules.FindQualifiedFrameworkReference(source);
}

static void CollectUsingNamespaces(string source, HashSet<string> namespaces)
{
    foreach (var ns in EnumerateUsingNamespaces(source))
    {
        namespaces.Add(ns);
    }
}

static IEnumerable<string> EnumerateUsingNamespaces(string source)
{
    // Also matches the alias form (using PA = Microsoft.Extensions.PlatformAbstractions;)
    foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(
        source, @"^\s*using\s+(?:static\s+)?(?:[A-Za-z_]\w*\s*=\s*)?([A-Za-z_][A-Za-z0-9_.]*)\s*;",
        System.Text.RegularExpressions.RegexOptions.Multiline))
    {
        yield return match.Groups[1].Value;
    }
}

/// <summary>
/// Reads the NuGet dependencies the ORIGINAL projects declared (SDK-style
/// PackageReference and old-style packages.config) so the generated project keeps them.
/// Framework-only web packages (OWIN, System.Web.*, script bundles) are skipped;
/// packages with a known .NET-compatible newer version are uplifted.
/// </summary>
static List<(string Id, string Version)> CollectDeclaredPackages(
    IEnumerable<string> projectDirectories, ConversionReport report)
{
    string[] skipPrefixes =
    [
        // Framework-only web stack / client-side asset packages
        "Microsoft.Owin", "Owin", "Microsoft.AspNet.", "Microsoft.AspNet.Identity",
        "Microsoft.Web.Infrastructure", "Antlr", "WebGrease", "Microsoft.CodeDom.Providers",
        "AspNet.ScriptManager", "AjaxControlToolkit", "SonarAnalyzer", "StyleCop.Analyzers",
        "Roslynator", "System.Web", "Microsoft.jQuery", "jQuery", "bootstrap", "Modernizr",
        "Respond", "WebForms.", "elmah", "Microsoft.VisualStudio.",
        // Shipped by the .NET SDK now; carrying the old split packages over breaks
        // or downgrades the modern framework references
        "NETStandard.Library", "Microsoft.NETCore.", "Microsoft.Extensions.",
        "Microsoft.AspNetCore.",
    ];

    // The .NET SDK provides these; a carried-over 4.x/5.x reference downgrades them
    static bool IsSdkProvidedSystemPackage(string id, string version)
        => id.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
           && System.Text.RegularExpressions.Regex.IsMatch(version, @"^[45]\.");
    var versionUplifts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["EntityFramework"] = "6.5.1",
    };

    var carried = new List<(string Id, string Version)>();
    var skipped = new List<string>();

    void Add(string id, string version)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(version))
        {
            return;
        }
        if (skipPrefixes.Any(prefix => id.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            || IsSdkProvidedSystemPackage(id, version))
        {
            skipped.Add(id);
            return;
        }
        carried.Add((id, versionUplifts.GetValueOrDefault(id, version)));
    }

    foreach (var directory in projectDirectories.Where(Directory.Exists))
    {
        foreach (var csprojPath in Directory.GetFiles(directory, "*.csproj", SearchOption.TopDirectoryOnly))
        {
            try
            {
                var document = System.Xml.Linq.XDocument.Load(csprojPath);
                foreach (var reference in document.Descendants()
                             .Where(node => node.Name.LocalName == "PackageReference"))
                {
                    Add(reference.Attribute("Include")?.Value,
                        reference.Attribute("Version")?.Value
                        ?? reference.Elements().FirstOrDefault(child => child.Name.LocalName == "Version")?.Value);
                }
            }
            catch (System.Xml.XmlException)
            {
                // Malformed project file: nothing to carry over
            }
        }

        var packagesConfigPath = Path.Combine(directory, "packages.config");
        if (File.Exists(packagesConfigPath))
        {
            try
            {
                foreach (var package in System.Xml.Linq.XDocument.Load(packagesConfigPath)
                             .Descendants().Where(node => node.Name.LocalName == "package"))
                {
                    Add(package.Attribute("id")?.Value, package.Attribute("version")?.Value);
                }
            }
            catch (System.Xml.XmlException)
            {
            }
        }
    }

    if (skipped.Count > 0)
    {
        report.Residual("(project)", ResidualKind.Configuration,
            ".NET へそのまま持ち込めない NuGet 参照をスキップしました(認証/バンドル等は手動移行): "
            + string.Join(", ", skipped.Distinct(StringComparer.OrdinalIgnoreCase)), disposition: ResidualDisposition.ManualMigration);
    }
    if (carried.Count > 0)
    {
        report.Info("(project)",
            "元プロジェクト宣言の NuGet 参照を引き継ぎました: "
            + string.Join(", ", carried.Select(package => package.Id).Distinct(StringComparer.OrdinalIgnoreCase)));
    }

    return carried.DistinctBy(package => package.Id, StringComparer.OrdinalIgnoreCase).ToList();
}

/// <summary>Well-known WebForms-era libraries that have .NET-compatible NuGet packages.</summary>
static List<(string Id, string Version)> ResolvePackageReferences(HashSet<string> namespaces)
{
    // Microsoft ships these alongside the runtime, so their version must track the target
    // framework. Pinning an older major downgrades what a carried-over package already
    // depends on (NU1605) and the restore fails.
    const string RuntimeLibraryVersion = "10.0.*";

    (string Prefix, string Id, string Version)[] knownPackages =
    [
        ("System.Data.Entity", "EntityFramework", "6.5.1"),
        ("System.Data.Objects", "EntityFramework", "6.5.1"),
        ("System.Data.EntityClient", "EntityFramework", "6.5.1"),
        ("System.Data.SqlClient", "System.Data.SqlClient", "4.9.0"),
        ("System.Data.OleDb", "System.Data.OleDb", RuntimeLibraryVersion),
        ("System.CodeDom", "System.CodeDom", RuntimeLibraryVersion),
        ("Newtonsoft.Json", "Newtonsoft.Json", "13.0.3"),
        ("Steeltoe.Extensions.Configuration", "Steeltoe.Extensions.Configuration.CloudFoundry", "1.1.0"),
        ("Microsoft.Extensions.PlatformAbstractions", "Microsoft.Extensions.PlatformAbstractions", "1.1.0"),
        ("System.Configuration", "System.Configuration.ConfigurationManager", RuntimeLibraryVersion),
        ("System.Drawing", "System.Drawing.Common", RuntimeLibraryVersion),
        ("Ionic.Zip", "DotNetZip", "1.16.0"),
        ("System.ServiceModel.Syndication", "System.ServiceModel.Syndication", RuntimeLibraryVersion),
        ("System.DirectoryServices", "System.DirectoryServices", RuntimeLibraryVersion),
    ];

    return knownPackages
        .Where(package => namespaces.Any(ns =>
            ns.Equals(package.Prefix, StringComparison.Ordinal)
            || ns.StartsWith(package.Prefix + ".", StringComparison.Ordinal)))
        .Select(package => (package.Id, package.Version))
        .DistinctBy(package => package.Id)
        .ToList();
}

/// <summary>
/// Drops @using lines naming a namespace that no ported file declares any more.
/// The import is emitted from the source file's own usings, which are checked against
/// the SOURCE tree; the porting exclusion cascade can then remove every file of that
/// namespace, and Razor fails to compile on the now-dead import.
/// </summary>
static string StripDeadUsings(
    string razor, HashSet<string> deadNamespaces, ConversionReport report, string componentName)
{
    if (deadNamespaces.Count == 0)
    {
        return razor;
    }

    var lines = razor.Split('\n');
    var kept = new List<string>(lines.Length);

    foreach (var line in lines)
    {
        var match = System.Text.RegularExpressions.Regex.Match(line, @"^@using\s+([A-Za-z_][\w.]*)\s*$");
        if (match.Success && deadNamespaces.Contains(match.Groups[1].Value))
        {
            report.Info(componentName,
                $"@using {match.Groups[1].Value} は移植後に型が残らないため除去しました。");
            continue;
        }
        kept.Add(line);
    }

    return string.Join("\n", kept);
}

/// <summary>
/// Empty declarations for the top-level types that only excluded files declared.
/// Enum members are carried over (they are plain identifiers with no dependencies of
/// their own); everything else is deliberately left empty, so a consumer that really
/// used the type fails at the member rather than silently getting a working-looking stub.
/// </summary>
static string GenerateExcludedTypeStubs(
    IReadOnlyList<string> excludedSources, IReadOnlyList<string> survivingSources, out int typeCount)
{
    typeCount = 0;

    var surviving = new HashSet<string>(StringComparer.Ordinal);
    foreach (var source in survivingSources)
    {
        foreach (var declaration in EnumerateTopLevelTypes(source))
        {
            surviving.Add(declaration.Key);
        }
    }

    var stubs = new SortedDictionary<string, SortedDictionary<string, string>>(StringComparer.Ordinal);
    foreach (var source in excludedSources)
    {
        foreach (var declaration in EnumerateTopLevelTypes(source))
        {
            if (surviving.Contains(declaration.Key) || string.IsNullOrEmpty(declaration.Namespace))
            {
                continue;
            }
            if (!stubs.TryGetValue(declaration.Namespace, out var members))
            {
                stubs[declaration.Namespace] = members = new SortedDictionary<string, string>(StringComparer.Ordinal);
            }
            members.TryAdd(declaration.Key, declaration.Text);
        }
    }

    var builder = new StringBuilder();
    builder.AppendLine("// Generated by WebForm2Blazor.");
    builder.AppendLine("// Declarations of types whose only source files were excluded from the port");
    builder.AppendLine("// (see the residual report). They exist so the rest of the application still");
    builder.AppendLine("// compiles; every member is missing on purpose.");
    builder.AppendLine();

    foreach (var (declaredNamespace, members) in stubs)
    {
        builder.AppendLine($"namespace {declaredNamespace}");
        builder.AppendLine("{");
        foreach (var text in members.Values)
        {
            builder.AppendLine(text);
            typeCount++;
        }
        builder.AppendLine("}");
        builder.AppendLine();
    }

    return builder.ToString();
}

/// <summary>Top-level type declarations of a file, with the text of an empty stub for each.</summary>
static List<(string Key, string Namespace, string Text)> EnumerateTopLevelTypes(string source)
{
    var result = new List<(string, string, string)>();

    foreach (var declaration in CodeBehindRewriter.ParseUnit(source)
                 .DescendantNodes()
                 .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax>())
    {
        // Nested types would need their container; the container itself is stubbed empty
        if (declaration.Parent is Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax)
        {
            continue;
        }

        var declaredNamespace = declaration.Ancestors()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault()?.Name.ToString() ?? string.Empty;

        var name = declaration.Identifier.Text;
        var typeParameters = (declaration as Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax)
            ?.TypeParameterList?.ToString() ?? string.Empty;

        var keyword = declaration switch
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.EnumDeclarationSyntax => "enum",
            Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax => "interface",
            Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax => "struct",
            Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax => "record",
            _ => "class",
        };

        var body = declaration is Microsoft.CodeAnalysis.CSharp.Syntax.EnumDeclarationSyntax enumDeclaration
            ? string.Join(", ", enumDeclaration.Members.Select(member => member.Identifier.Text))
            : string.Empty;

        var text = $"    public {keyword} {name}{typeParameters} {{ {body} }}";
        result.Add(($"{declaredNamespace}.{name}`{typeParameters.Length}", declaredNamespace, text));
    }

    return result;
}

/// <summary>Resource names of a .resx, in file order (duplicates and invalid names skipped).</summary>
static List<string> ReadResourceKeys(string path)
{
    var keys = new List<string>();
    var seen = new HashSet<string>(StringComparer.Ordinal);

    try
    {
        foreach (var element in System.Xml.Linq.XDocument.Load(path).Root?.Elements("data") ?? [])
        {
            var name = element.Attribute("name")?.Value;
            // Only plain string resources: a typed entry points at an embedded object
            if (string.IsNullOrEmpty(name)
                || element.Attribute("type") is not null
                || element.Attribute("mimetype") is not null)
            {
                continue;
            }
            if (seen.Add(name))
            {
                keys.Add(name);
            }
        }
    }
    catch (Exception exception) when (exception is System.Xml.XmlException or IOException)
    {
        return [];
    }

    return keys;
}

/// <summary>Turns a resource name into a valid C# identifier (WebForms did the same).</summary>
static string SanitizeTypeName(string value)
{
    var builder = new StringBuilder();
    foreach (var c in value)
    {
        builder.Append(char.IsLetterOrDigit(c) || c == '_' ? c : '_');
    }
    var result = builder.ToString();
    if (result.Length > 0 && char.IsDigit(result[0]))
    {
        result = "_" + result;
    }

    // Resource names are free text, so "default" / "class" and friends do occur;
    // a verbatim identifier keeps the member callable under its original name
    return Microsoft.CodeAnalysis.CSharp.SyntaxFacts.GetKeywordKind(result)
           == Microsoft.CodeAnalysis.CSharp.SyntaxKind.None
        ? result
        : "@" + result;
}

static void LintGeneratedRazor(ConvertedComponent component, ConversionReport report)
{
    // Razor comments never render, so the deliberate @* TODO(W2B): <% ... %> *@ markers
    // are not leaks - strip them before scanning
    var visible = System.Text.RegularExpressions.Regex.Replace(
        component.RazorContent, @"@\*.*?\*@", string.Empty,
        System.Text.RegularExpressions.RegexOptions.Singleline);

    foreach (var marker in new[] { "<%", "%>", "runat=" })
    {
        var index = visible.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            continue;
        }
        var start = Math.Max(0, index - 30);
        var excerpt = visible
            .Substring(start, Math.Min(90, visible.Length - start))
            .Replace("\r", " ").Replace("\n", " ");
        report.Residual($"{component.OutputDirectory}/{component.ComponentName}.razor", ResidualKind.Structure,
            $"変換漏れの疑い: 生成 Razor に '{marker}' が残っています(…{excerpt}…)。");
    }
}

static string GenerateStubComponent(string appName, string stubName, string originalTag)
    => $$"""
        @namespace {{appName}}.Components.Stubs

        @* Auto-generated placeholder for the unconverted control <{{originalTag}}>.
           Map the tag to a real component via --control-map, or replace this stub with a
           hand-ported component. Original markup attributes arrive in UnmatchedParameters;
           child template markup arrives as ChildContent and is intentionally not rendered. *@
        <span id="@ID" class="w2b-stub" title="unconverted control: {{originalTag}}">[{{originalTag}}]</span>

        @code {
            [Parameter] public string ID { get; set; }
            [Parameter] public RenderFragment ChildContent { get; set; }
            [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> UnmatchedParameters { get; set; }
        }
        """;

static string GenerateFieldOnlyCodeBehind(ConvertedComponent component)
{
    var baseClass = component.Kind switch
    {
        CodeBehindKind.Layout => "WebFormsLayout",
        CodeBehindKind.UserControl => "WebFormsUserControl",
        _ => "WebFormsPage",
    };

    var builder = new StringBuilder();
    builder.AppendLine("using Microsoft.AspNetCore.Components;");
    builder.AppendLine("using WebForm2Blazor.Components;");
    foreach (var ns in component.UsedControlNamespaces.Distinct().Order(StringComparer.Ordinal))
    {
        builder.AppendLine($"using {ns};");
    }
    builder.AppendLine();
    builder.AppendLine($"namespace {component.TargetNamespace}");
    builder.AppendLine("{");
    builder.AppendLine($"    public partial class {component.ComponentName}");
    builder.AppendLine("    {");
    foreach (var field in component.Fields.DistinctBy(f => f.Name))
    {
        builder.AppendLine($"        protected {field.Type} {field.Name};");
    }
    builder.AppendLine("    }");
    builder.AppendLine("}");
    _ = baseClass;
    return builder.ToString();
}
