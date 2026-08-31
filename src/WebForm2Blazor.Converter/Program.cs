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
string? packageMapPath = null;
string? propertyCatalogPath = null;
string? webConfigOverride = null;
var includeDirectories = new List<string>();
var analyzerAssemblies = new List<string>();
string? entryProjectPath = null;
var deriveIncludes = true;
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
        case "--package-map": packageMapPath = args[++i]; break;
        case "--catalog": propertyCatalogPath = args[++i]; break;
        case "--include": includeDirectories.Add(args[++i]); break;
        case "--project": entryProjectPath = args[++i]; break;
        case "--analyzer": analyzerAssemblies.Add(args[++i]); break;
        case "--no-derive-includes": deriveIncludes = false; break;
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

// Which library projects to port alongside the app is stated by the app's .csproj, so it
// is read rather than guessed. Getting this set wrong is the most expensive mistake in a
// conversion and it does not announce itself: the missing types surface as hundreds of
// CS0246 that read like converter or compatibility-layer failures.
if (deriveIncludes)
{
    var derived = WebForm2Blazor.Converter.Project.ProjectReferenceGraph.Derive(input, entryProjectPath);

    if (derived.AmbiguousProjects.Count > 0)
    {
        report.Residual("(project)", ResidualKind.Configuration,
            $"{input} に .csproj が {derived.AmbiguousProjects.Count} 個あり、参照プロジェクトを自動導出できません"
            + "(相互排他のビルド構成である場合が多く、選ぶとデータベース等を暗黙に決めてしまいます)。"
            + "--project でどれを使うか指定してください: "
            + string.Join(", ", derived.AmbiguousProjects.Select(Path.GetFileName)),
            disposition: ResidualDisposition.ManualMigration);
    }

    var added = derived.Directories
        .Where(directory => !includeDirectories.Any(existing =>
            string.Equals(Path.GetFullPath(existing).TrimEnd(Path.DirectorySeparatorChar),
                directory.TrimEnd(Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase)))
        .ToList();
    includeDirectories.AddRange(added);

    if (added.Count > 0)
    {
        report.Info("(project)",
            $"{Path.GetFileName(derived.EntryProject)} の ProjectReference から参照プロジェクト {added.Count} 件を自動で移植対象にしました: "
            + string.Join(", ", added.Select(Path.GetFileName)));
    }

    foreach (var group in derived.ExclusiveGroups)
    {
        report.Residual("(project)", ResidualKind.Configuration,
            "同じ型を宣言する参照プロジェクトが複数あります(データプロバイダ等、実行時に 1 つだけ使う排他構成)。"
            + "どれを使うかは配置の判断のため自動選択せず、いずれも移植対象から外しました。"
            + "--include で 1 つ指定してください: "
            + string.Join(" / ", group.Select(Path.GetFileName)),
            disposition: ResidualDisposition.ManualMigration);
    }

    if (derived.Analyzers.Count > 0)
    {
        // Not portable and not reproducible: whatever the generator emitted at build time
        // is simply absent, and the code that used it will not compile. Saying so here
        // saves the reader from hunting for a converter bug that does not exist.
        var unwired = derived.Analyzers
            .Where(path => !analyzerAssemblies.Any(dll =>
                Path.GetFileNameWithoutExtension(dll)
                    .Equals(Path.GetFileNameWithoutExtension(path), StringComparison.OrdinalIgnoreCase)))
            .ToList();
        if (unwired.Count > 0)
        {
            report.Residual("(project)", ResidualKind.Configuration,
                "ビルド時にコードを生成するアナライザ参照があります。生成される宣言は変換元に存在しないため、"
                + "それに依存するコードはこのままではビルドできません。"
                + "アナライザをビルドして --analyzer <dll> で渡すと、生成プロジェクトに組み込まれ"
                + "ビルド時に元と同じ宣言が生成されます: "
                + string.Join(", ", unwired.Select(Path.GetFileName)),
                disposition: ResidualDisposition.ManualMigration);
        }
    }

    if (derived.ForeignLanguage.Count > 0)
    {
        report.Residual("(project)", ResidualKind.Configuration,
            "C# 以外のプロジェクトが参照されています。変換対象外です: "
            + string.Join(", ", derived.ForeignLanguage.Select(Path.GetFileName)),
            disposition: ResidualDisposition.ManualMigration);
    }

    if (derived.Missing.Count > 0)
    {
        report.Residual("(project)", ResidualKind.Configuration,
            "ProjectReference の参照先が見つかりません(取得漏れの可能性があります): "
            + string.Join(", ", derived.Missing.Select(Path.GetFileName)),
            disposition: ResidualDisposition.ManualMigration);
    }
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

var userControlRegistry = AspxConverters.BuildUserControlRegistry(project, appName, baseRegistry);
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

// Half of the "Framework-only namespace" residuals are not WebForms at all: DNN hosts
// WebForms, MVC and Web API in one application, and 232 of its 321 are System.Web.Mvc,
// System.Web.Http or ASP.NET Identity. Saying "manual migration required" of those is
// misleading - nothing was lost in conversion, that code was never in scope. The reader
// needs to tell "this converter dropped something you must rebuild" apart from "this part
// of your app is a different framework".
static string OutOfScopeFrameworkNote(string unportable)
{
    var outOfScope = unportable switch
    {
        var ns when ns == "System.Web.Mvc" || ns.StartsWith("System.Web.Mvc.", StringComparison.Ordinal)
            => "ASP.NET MVC",
        var ns when ns == "System.Web.Http" || ns.StartsWith("System.Web.Http.", StringComparison.Ordinal)
            => "ASP.NET Web API",
        var ns when ns.StartsWith("Microsoft.AspNet.Identity", StringComparison.Ordinal)
            => "ASP.NET Identity",
        var ns when ns == "System.Web.Optimization" => "ASP.NET バンドル",
        _ => null,
    };

    return outOfScope is not null
        ? $"{outOfScope}({unportable})のコードです。**この変換器は WebForms のみを対象とする**ため"
          + "移植していません。変換で失われたものはなく、対応する ASP.NET Core の仕組みへ"
          + "別途移行してください。"
        : $".NET Framework 専用の名前空間 {unportable} を使用しているため移植から除外しました"
          + "(認証/ルーティング等の基盤コードは手動移行が必要)。";
}

var excludedCandidates = new HashSet<int>();
for (var i = 0; i < candidateNamespaces.Count; i++)
{
    if (FindUnportableNamespace(candidateNamespaces[i].candidate.Source) is { } unportable)
    {
        excludedCandidates.Add(i);
        report.Residual(candidateNamespaces[i].candidate.ReportName, ResidualKind.CodeBehind,
            OutOfScopeFrameworkNote(unportable), disposition: ResidualDisposition.ManualMigration);
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
        var codeBehindSource = File.ReadAllText(component.CodeBehindSourcePath);
        // Page code-behind holds fields of user controls, so add the namespaces of the
        // user controls this component actually references to the usings. Markup is only
        // half the story: code-behind also names a user control's type without placing it
        // in markup - (UserControlSettings)Page.LoadControl("Settings.ascx") - so the
        // namespaces of controls NAMED IN THE CODE are added too. Only names the file
        // actually mentions, to avoid dragging in same-named controls from other folders.
        var controlUsings = component.UsedControlNamespaces
            .Concat(UserControlNamespacesNamedIn(codeBehindSource, userControlRegistry, baseRegistry))
            .Distinct(StringComparer.Ordinal)
            .ToList();
        var rewritten = CodeBehindRewriter.Rewrite(
            codeBehindSource, component, sourceName, report,
            controlUsings, baseRegistry);
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

    // BinaryFormatter still compiles (the generated project suppresses SYSLIB0011) but the
    // runtime removed it, so the call throws the first time it runs. Report it rather than
    // let a build that succeeds imply the code works.
    if (candidate.Source.Contains("BinaryFormatter", StringComparison.Ordinal))
    {
        report.Residual(candidate.ReportName, ResidualKind.CodeBehind,
            "BinaryFormatter は .NET から削除されています。ビルドは通りますが実行時に "
            + "PlatformNotSupportedException になります。別のシリアライザへの移行が必要です。",
            disposition: ResidualDisposition.ManualMigration);
    }
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
var packageMap = packageMapPath is not null ? LoadPackageMap(packageMapPath) : null;
if (packageMap is not null)
{
    report.Info("(project)", $"--package-map から {packageMap.Count} 件のパッケージ指定を読み込みました。");
}

var packageReferences = CollectDeclaredPackages(
        [input, .. includeDirectories], report, packageMap,
        Path.Combine(output, "package-map.template.json"))
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

// Source generators the ORIGINAL build ran. Their output is not in the source - that is
// the whole point of a generator - so ported code that depends on it cannot compile:
// DNN Platform's [DnnDeprecated] generator writes the defining half of every partial
// method in its Obsolete/ files, and without it the build reports 230 CS0759.
//
// Rather than re-implement generator hosting, the generator is handed to the compiler the
// same way the original project did, as an Analyzer on the generated project. MSBuild then
// runs it during the build with a real compilation behind it, which is what a generator
// needs and what the converter has no way to assemble on its own.
//
// Measured on DNN Platform: 941 build errors -> 726, with all 230 CS0759 gone.
if (analyzerAssemblies.Count > 0)
{
    var csprojPath = Path.Combine(output, appName + ".csproj");
    var csprojText = File.ReadAllText(csprojPath);
    var analyzerItems = string.Join(Environment.NewLine, analyzerAssemblies.Select(path =>
        $"    <Analyzer Include=\"{Path.GetFullPath(path)}\" />"));
    csprojText = csprojText.Replace("</Project>",
        $"  <ItemGroup>{Environment.NewLine}{analyzerItems}{Environment.NewLine}  </ItemGroup>{Environment.NewLine}{Environment.NewLine}</Project>");
    File.WriteAllText(csprojPath, csprojText);
    report.Info("(project)",
        "ソースジェネレータを生成プロジェクトに組み込みました(ビルド時に元と同じ宣言が生成されます): "
        + string.Join(", ", analyzerAssemblies.Select(Path.GetFileName)));
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

// Custom configuration sections travel as App.config, not appsettings.json. Their handler
// is the application's own ConfigurationSection subclass - ordinary code, ported with
// everything else - and System.Configuration on .NET can still bind one, but only from a
// .config file. appsettings.json cannot carry them: it has no schema for a typed section.
//
// Only <configSections> and the sections it declares come across. The rest of Web.config
// (<system.web>, <system.webServer>) is Framework-only and is converted or reported
// separately; copying it would put settings into App.config that nothing reads.
// App_Data is the application's data, not its code: a file-backed provider keeps its
// store there, and without it the provider initialises and then fails on first read.
// BlogEngine ships its entire blog as XML under App_Data, so the converted site cannot
// serve a single post without it. Copied verbatim and marked to travel to the output
// directory, because that is where the running app resolves the path from.
var appDataSource = Path.Combine(input, "App_Data");
if (Directory.Exists(appDataSource))
{
    var copied = 0;
    foreach (var file in Directory.EnumerateFiles(appDataSource, "*", SearchOption.AllDirectories))
    {
        var relative = Path.GetRelativePath(appDataSource, file);
        var destination = Path.Combine(output, "App_Data", relative);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        File.Copy(file, destination, overwrite: true);
        copied++;
    }

    if (copied > 0)
    {
        var csprojPath = Path.Combine(output, appName + ".csproj");
        var csprojText = File.ReadAllText(csprojPath);
        csprojText = csprojText.Replace("</Project>",
            $"  <ItemGroup>{Environment.NewLine}"
            // The SDK's own default globs already claim some of these files by extension
            // (**/*.config, **/*.json ...). A plain Include re-adds them and the build dies
            // at NETSDK1022 BEFORE compiling anything - which also silently floors the
            // build-error count at 1. Exclude what is already a Content item and set the
            // copy metadata on those separately with Update.
            + $"    <Content Include=\"App_Data\\**\" Exclude=\"@(Content)\" CopyToOutputDirectory=\"PreserveNewest\" />{Environment.NewLine}"
            + $"    <Content Update=\"App_Data\\**\" CopyToOutputDirectory=\"PreserveNewest\" />{Environment.NewLine}"
            + $"  </ItemGroup>{Environment.NewLine}{Environment.NewLine}</Project>");
        File.WriteAllText(csprojPath, csprojText);
        report.Info("(project)", $"App_Data の {copied} ファイルを出力にコピーしました(ファイルベースのプロバイダのデータ)。");
    }
}

if (project.WebConfigPath is not null)
{
    var appConfig = WebConfigConverter.ExtractCustomSections(project.WebConfigPath, appName, report);
    if (appConfig is not null)
    {
        File.WriteAllText(Path.Combine(output, "App.config"), appConfig);
    }
}

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
/// <summary>
/// Assembly name -> the NuGet package to use for it, from --package-map:
/// [{ "assembly": "Lucene.Net", "package": "Lucene.Net", "version": "4.8.0-beta00017" }]
///
/// An entry with no "package" means "deliberately not carried over", which suppresses the
/// residual for that assembly without adding a reference - the difference between a
/// decision made and a decision missing.
/// </summary>
static Dictionary<string, (string? Package, string? Version)> LoadPackageMap(string path)
{
    var map = new Dictionary<string, (string?, string?)>(StringComparer.OrdinalIgnoreCase);
    using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(path));
    foreach (var entry in document.RootElement.EnumerateArray())
    {
        if (!entry.TryGetProperty("assembly", out var assembly)
            || assembly.GetString() is not { Length: > 0 } assemblyName)
        {
            continue;
        }

        map[assemblyName] = (
            entry.TryGetProperty("package", out var package) ? package.GetString() : null,
            entry.TryGetProperty("version", out var version) ? version.GetString() : null);
    }
    return map;
}

static List<(string Id, string Version)> CollectDeclaredPackages(
    IEnumerable<string> projectDirectories,
    ConversionReport report,
    Dictionary<string, (string? Package, string? Version)>? packageMap = null,
    string? templatePath = null)
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

    // Packages that exist only to backport BCL types to .NET Framework. The types are in
    // the box on modern .NET, so carrying the reference over is at best redundant (NU1510)
    // and at worst harmful: an old out-of-band assembly can win binding over the in-box one.
    //
    // Matched by identity, not by version. The previous rule keyed off a 4.x/5.x version
    // number, but these packages now ship on the .NET release train - mojoPortal declares
    // System.Text.Json 10.0.2 and System.Runtime.CompilerServices.Unsafe 6.1.2 - so every
    // one of them slipped through.
    var inBoxOnModernDotNet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Microsoft.Bcl.AsyncInterfaces", "Microsoft.Bcl.HashCode", "Microsoft.Bcl.TimeProvider",
        "Microsoft.CSharp",
        "System.Buffers", "System.Collections.Immutable", "System.ComponentModel.Annotations",
        "System.Diagnostics.DiagnosticSource", "System.IO.Pipelines", "System.Memory",
        "System.Net.Http", "System.Numerics.Vectors", "System.Runtime.CompilerServices.Unsafe",
        "System.Security.AccessControl", "System.Security.Principal.Windows",
        "System.Text.Encoding.CodePages", "System.Text.Encodings.Web", "System.Text.Json",
        "System.Threading.Tasks.Extensions", "System.ValueTuple",
    };

    // Kept alongside the identity list: a 4.x/5.x System.* reference is a Framework-era
    // split package regardless of whether it is named above, and carrying it downgrades the
    // modern framework reference. Packages that genuinely still ship out of band
    // (System.Configuration.ConfigurationManager, System.Drawing.Common) are re-added from
    // the ported code's usings by ResolvePackageReferences.
    static bool IsSdkProvidedSystemPackage(string id, string version)
        => id.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
           && System.Text.RegularExpressions.Regex.IsMatch(version, @"^[45]\.");
    var versionUplifts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["EntityFramework"] = "6.5.1",
    };

    var carried = new List<(string Id, string Version)>();
    var skipped = new List<string>();
    var inBox = new List<string>();
    var binaryReferences = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    void Add(string id, string version)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(version))
        {
            return;
        }
        // Reported apart from the skipped list: dropping these needs no decision from
        // anyone, whereas a skipped Framework-only package is a migration the reader has to
        // plan for. Mixing the two buries the second in the first.
        if (inBoxOnModernDotNet.Contains(id))
        {
            inBox.Add(id);
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

                // A "<Reference>" pointing at a DLL checked into the repository (_libs\,
                // packages\, lib\) is a dependency with NO NuGet identity to carry over, so
                // it vanishes silently and its types come back as CS0246 - mojoPortal's
                // Lucene.Net is 115 errors of exactly this. The converter cannot pick the
                // replacement: the modern package may be a different major with a different
                // API (Lucene.Net 3.0.3 -> 4.8 is a rewrite, not an upgrade). Name them so
                // the choice is visible instead of silent.
                foreach (var reference in document.Descendants()
                             .Where(node => node.Name.LocalName == "Reference"))
                {
                    var hintPath = reference.Elements()
                        .FirstOrDefault(child => child.Name.LocalName == "HintPath")?.Value;
                    if (string.IsNullOrEmpty(hintPath))
                    {
                        continue;
                    }

                    var assembly = reference.Attribute("Include")?.Value?.Split(',')[0].Trim();
                    if (!string.IsNullOrEmpty(assembly))
                    {
                        binaryReferences.Add(assembly);
                    }
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

    if (inBox.Count > 0)
    {
        report.Info("(project)",
            ".NET に同梱済みのため NuGet 参照を削除しました(対応不要): "
            + string.Join(", ", inBox.Distinct(StringComparer.OrdinalIgnoreCase)));
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

    // Reported apart from skipped packages: those had a NuGet identity and were rejected,
    // these never had one. The reader has to find the modern package themselves, and for
    // some there is not one at the same API level.
    // A --package-map answer is applied here rather than earlier so it is visible as an
    // explicit decision: the assembly had no NuGet identity and someone supplied one.
    var mapped = new List<string>();
    var declined = new List<string>();
    foreach (var assembly in binaryReferences)
    {
        if (packageMap is null || !packageMap.TryGetValue(assembly, out var choice))
        {
            continue;
        }

        if (string.IsNullOrEmpty(choice.Package))
        {
            declined.Add(assembly);
        }
        else
        {
            carried.Add((choice.Package!, choice.Version ?? "*"));
            mapped.Add($"{assembly} -> {choice.Package} {choice.Version}");
        }
    }
    if (mapped.Count > 0)
    {
        report.Info("(project)", "--package-map で指定された置き換えを適用しました: " + string.Join(", ", mapped));
    }
    if (declined.Count > 0)
    {
        report.Info("(project)",
            "--package-map で「引き継がない」と指定された依存です(判断済みのため残差にしません): "
            + string.Join(", ", declined));
    }

    var undecided = binaryReferences
        .Where(assembly => packageMap is null || !packageMap.ContainsKey(assembly))
        .Where(assembly => !carried.Any(package =>
            package.Id.Equals(assembly, StringComparison.OrdinalIgnoreCase)))
        .OrderBy(assembly => assembly, StringComparer.OrdinalIgnoreCase)
        .ToList();

    // A list in a report is something to read; a file with the assembly names already in
    // it is something to answer. The template is the --package-map format with the
    // packages left blank, so deciding is filling in blanks rather than looking up a
    // schema.
    if (undecided.Count > 0 && templatePath is not null)
    {
        // Valid JSON as written, so it can be passed straight back with --package-map
        // without editing anything out first. The instructions ride in an entry with no
        // "assembly", which LoadPackageMap skips.
        var entries = new List<string>
        {
            "  { \"_readme\": \""
            + "リポジトリ同梱の DLL を直接参照していた依存です。NuGet の識別子が無いため"
            + "自動では引き継げません。package と version を埋めて "
            + "--package-map <このファイル> で再変換してください。"
            + "引き継がないと決めたものは package を空のままにすると残差から消えます。\" }",
        };
        entries.AddRange(undecided.Select(assembly =>
            $"  {{ \"assembly\": \"{assembly}\", \"package\": \"\", \"version\": \"\" }}"));

        // BOM: these outputs are read by PowerShell 5.1 tooling, which treats a BOM-less
        // file as ANSI and mangles the Japanese (see corpora/README.md).
        File.WriteAllText(
            templatePath,
            "[" + Environment.NewLine + string.Join("," + Environment.NewLine, entries)
            + Environment.NewLine + "]" + Environment.NewLine,
            new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }

    if (undecided.Count > 0)
    {
        report.Residual("(project)", ResidualKind.Configuration,
            $"リポジトリ同梱の DLL を直接参照していた依存が {undecided.Count} 件あります。"
            + "NuGet の識別子が無いため引き継げません。対応する .NET 版パッケージを "
            + "PackageReference として追加してください(メジャーバージョンが変わり API 移行が"
            + "必要なものもあります): " + string.Join(", ", undecided),
            disposition: ResidualDisposition.ManualMigration);
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
        // MEF. Referenced on 4.8 as a GAC assembly ("<Reference Include=" with no HintPath),
        // which leaves no NuGet trace to carry over, so ImportMany and friends came out as
        // CS0246. The package has the same API as the Framework assembly.
        ("System.ComponentModel.Composition", "System.ComponentModel.Composition", RuntimeLibraryVersion),
        ("System.Runtime.Caching", "System.Runtime.Caching", RuntimeLibraryVersion),
        ("System.Management", "System.Management", RuntimeLibraryVersion),
        ("System.Security.Cryptography.Xml", "System.Security.Cryptography.Xml", RuntimeLibraryVersion),
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
    var survivingNames = new HashSet<string>(StringComparer.Ordinal);

    // Simple name -> namespace-qualified name, for inheritable classes only. A stub is
    // rendered with almost no usings and its base often lives in another namespace, so a
    // base class has to be written out fully qualified. A name that is not unique maps to
    // null and is then dropped rather than guessed.
    var classesBySimpleName = new Dictionary<string, string?>(StringComparer.Ordinal);
    void RecordInheritableClass(StubType declaration)
    {
        if (declaration.Declaration is not Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax classDeclaration)
        {
            return;
        }
        // sealed/static cannot be derived from at all. abstract is excluded for a subtler
        // reason: the stub would inherit abstract members it has no way to implement, and
        // CS0534 replaces the error we were trying to remove. A concrete class cannot carry
        // unimplemented abstract members, so restricting to concrete keeps this sound
        // without having to walk the inheritance chain.
        var unusable = classDeclaration.Modifiers.Any(modifier =>
            Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.SealedKeyword)
            || Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.StaticKeyword)
            || Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.AbstractKeyword));
        if (unusable)
        {
            return;
        }

        var qualified = QualifiedName(declaration.Namespace, declaration.Name);
        if (classesBySimpleName.TryGetValue(declaration.Name, out var existing) && existing != qualified)
        {
            classesBySimpleName[declaration.Name] = null;
            return;
        }
        classesBySimpleName[declaration.Name] = qualified;
    }

    foreach (var source in survivingSources)
    {
        foreach (var declaration in EnumerateTopLevelTypes(source))
        {
            surviving.Add(declaration.Key);
            survivingNames.Add(QualifiedName(declaration.Namespace, declaration.Name));
            RecordInheritableClass(declaration);
        }
    }

    // Pass 1: which types need a stub. Their names are needed before rendering, because a
    // member is only emitted when every type in its signature can actually be resolved.
    var pending = new SortedDictionary<string, List<StubType>>(StringComparer.Ordinal);
    var stubNames = new HashSet<string>(StringComparer.Ordinal);
    var seen = new HashSet<string>(StringComparer.Ordinal);
    foreach (var source in excludedSources)
    {
        foreach (var declaration in EnumerateTopLevelTypes(source))
        {
            if (surviving.Contains(declaration.Key) || !seen.Add(declaration.Key))
            {
                continue;
            }
            if (!pending.TryGetValue(declaration.Namespace, out var members))
            {
                pending[declaration.Namespace] = members = [];
            }
            members.Add(declaration);
            stubNames.Add(QualifiedName(declaration.Namespace, declaration.Name));
            RecordInheritableClass(declaration);
        }
    }

    var known = new HashSet<string>(survivingNames, StringComparer.Ordinal);
    known.UnionWith(stubNames);

    var builder = new StringBuilder();
    builder.AppendLine("// Generated by WebForm2Blazor.");
    builder.AppendLine("// Declarations of types whose only source files were excluded from the port");
    builder.AppendLine("// (see the residual report). They exist so the rest of the application still");
    builder.AppendLine("// compiles.");
    builder.AppendLine("//");
    builder.AppendLine("// Members are reproduced only where every type in the signature still resolves;");
    builder.AppendLine("// the rest are dropped, because a stub that does not compile helps no one.");
    builder.AppendLine("// Constants keep their original literal value - code branches on them. Methods");
    builder.AppendLine("// throw: the implementation did not come across, and failing loudly at the call");
    builder.AppendLine("// site beats returning a plausible default.");
    builder.AppendLine();
    builder.AppendLine("using System;");
    builder.AppendLine("using System.Collections.Generic;");
    builder.AppendLine();

    foreach (var (declaredNamespace, members) in pending)
    {
        var indent = string.IsNullOrEmpty(declaredNamespace) ? string.Empty : "    ";
        if (!string.IsNullOrEmpty(declaredNamespace))
        {
            builder.AppendLine($"namespace {declaredNamespace}");
            builder.AppendLine("{");
        }
        foreach (var stub in members.OrderBy(member => member.Name, StringComparer.Ordinal))
        {
            builder.AppendLine(RenderStubType(stub, known, classesBySimpleName, indent, declaredNamespace));
            typeCount++;
        }
        if (!string.IsNullOrEmpty(declaredNamespace))
        {
            builder.AppendLine("}");
        }
        builder.AppendLine();
    }

    return builder.ToString();
}

/// <summary>Renders one stubbed type, keeping the members whose signatures still resolve.</summary>
static string RenderStubType(
    StubType stub,
    HashSet<string> known,
    IReadOnlyDictionary<string, string?> classesBySimpleName,
    string indent,
    string declaredNamespace)
{
    var declaration = stub.Declaration;

    if (declaration is Microsoft.CodeAnalysis.CSharp.Syntax.EnumDeclarationSyntax enumDeclaration)
    {
        var names = string.Join(", ", enumDeclaration.Members.Select(member => member.Identifier.Text));
        return $"{indent}public enum {stub.Name} {{ {names} }}";
    }

    var typeDeclaration = declaration as Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax;
    var typeParameters = typeDeclaration?.TypeParameterList?.ToString() ?? string.Empty;
    var keyword = declaration switch
    {
        Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax => "interface",
        Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax => "struct",
        Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax => "record",
        _ => "class",
    };

    // A static class cannot hold instance members; keeping the modifier also keeps the
    // call syntax at the use site identical (RazorHelpers.ParseRazor(...)).
    var isStatic = typeDeclaration is not null
                   && typeDeclaration.Modifiers.Any(modifier => Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.StaticKeyword));
    var modifiers = isStatic ? "public static" : "public";
    var isInterface = keyword == "interface";

    // A sealed stub cannot carry virtual members. The stub itself is not emitted sealed,
    // but the source declaration is what decides whether subclasses could exist at all.
    var isSealed = typeDeclaration is not null
                   && typeDeclaration.Modifiers.Any(modifier => Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.SealedKeyword));

    // Dropping the base class silently changes what the type IS, and the damage lands
    // somewhere else entirely: a .razor that says "@inherits ThatType" stops being a Blazor
    // component and fails with CS0115 on BuildRenderTree. 33 of DNN Platform's 39
    // generated-Razor errors traced back to this one omission, through chains like
    // PortalModuleBase (stubbed) -> UserControlBase (ported) -> the compat UserControl.
    //
    // Only a project CLASS is carried. An interface in the base list would oblige the stub
    // to implement its members, which is exactly what a stub cannot do, and a base outside
    // the port (System.Web, a NuGet package) cannot be named from here.
    var baseClause = string.Empty;
    var ownQualifiedName = QualifiedName(declaredNamespace, stub.Name);
    if (!isStatic && keyword == "class" && typeDeclaration?.BaseList is { } baseList)
    {
        foreach (var candidate in baseList.Types)
        {
            var baseName = candidate.Type switch
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax identifier
                    => identifier.Identifier.Text,
                Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax qualified
                    => qualified.Right.Identifier.Text,
                _ => null,
            };

            if (baseName is null)
            {
                continue;
            }

            // A project class first; failing that, a WebForms base the compat layer supplies.
            // The stub file carries almost no usings, so either way the name is written out
            // fully qualified.
            string? resolvedBase = null;
            if (classesBySimpleName.TryGetValue(baseName, out var projectClass)
                && projectClass is not null
                && projectClass != ownQualifiedName)
            {
                resolvedBase = projectClass;
            }
            else if (CodeBehindRewriter.ResolveComponentBase(baseName) is { } compatBase)
            {
                resolvedBase = "WebForm2Blazor.Components." + compatBase;
            }
            else if (CodeBehindRewriter.ResolveControlBase(baseName) is { } controlBase)
            {
                resolvedBase = "WebForm2Blazor.Components." + controlBase;
            }

            if (resolvedBase is not null)
            {
                baseClause = $" : {resolvedBase}";
                break;
            }
        }
    }

    var lines = new List<string>();
    if (typeDeclaration is not null)
    {
        foreach (var member in typeDeclaration.Members)
        {
            var text = RenderStubMember(
                member, known, isStatic, isInterface, declaredNamespace, containerIsSealed: isSealed);
            if (text is not null)
            {
                lines.Add($"{indent}    {text}");
            }
        }
    }

    if (lines.Count == 0)
    {
        return $"{indent}{modifiers} {keyword} {stub.Name}{typeParameters}{baseClause} {{ }}";
    }

    var body = new StringBuilder();
    body.AppendLine($"{indent}{modifiers} {keyword} {stub.Name}{typeParameters}{baseClause}");
    body.AppendLine($"{indent}{{");
    foreach (var line in lines)
    {
        body.AppendLine(line);
    }
    body.Append($"{indent}}}");
    return body.ToString();
}

/// <summary>
/// One stubbed member, or null when it cannot be reproduced faithfully enough to compile
/// (a type in the signature was itself excluded, or the shape needs a real body).
/// </summary>
static string? RenderStubMember(
    Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member,
    HashSet<string> known,
    bool containerIsStatic,
    bool containerIsInterface,
    string declaredNamespace,
    bool containerIsSealed = false)
{
    bool IsPublic(Microsoft.CodeAnalysis.SyntaxTokenList modifiers)
        => modifiers.Any(modifier => Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword))
           || modifiers.Any(modifier => Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.InternalKeyword));

    string Prefix(Microsoft.CodeAnalysis.SyntaxTokenList modifiers)
    {
        if (containerIsInterface)
        {
            return string.Empty;
        }
        var isStatic = containerIsStatic
                       || modifiers.Any(modifier => Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.StaticKeyword));
        if (isStatic)
        {
            return "public static ";
        }

        // A member the original declared virtual/abstract/override is one subclasses
        // override, and those subclasses are being ported even though this type was not.
        // Emitting it without the modifier turns every one of those overrides into CS0506
        // ("no suitable method found to override... not marked virtual"), reported against
        // the subclass. DNN's FileInstaller, PermissionsGrid and AuthorizeAttributeBase
        // are stubs whose subclasses failed this way.
        //
        // Only mirrors what the source said - a member that was not overridable stays that
        // way, so a stub never invites an override the original did not allow.
        var wasOverridable = !containerIsSealed
            && modifiers.Any(modifier =>
                Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.VirtualKeyword)
                || Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.AbstractKeyword)
                || Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.OverrideKeyword));

        return wasOverridable ? "public virtual " : "public ";
    }

    switch (member)
    {
        case Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax method:
        {
            if (!IsPublic(method.Modifiers)
                || method.TypeParameterList is not null
                || !Resolves(method.ReturnType, known, declaredNamespace)
                || method.ParameterList.Parameters.Any(parameter =>
                    parameter.Modifiers.Count > 0 || !Resolves(parameter.Type, known, declaredNamespace)))
            {
                return null;
            }
            var parameters = string.Join(", ", method.ParameterList.Parameters
                .Select(parameter => $"{parameter.Type} {parameter.Identifier.Text}"));
            var message = $"{method.Identifier.Text} は変換対象外です(元の実装は移植されていません)。";
            return $"{Prefix(method.Modifiers)}{method.ReturnType} {method.Identifier.Text}({parameters})"
                   + $" => throw new global::System.NotSupportedException(\"{message}\");";
        }

        case Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax property:
        {
            if (!IsPublic(property.Modifiers) || !Resolves(property.Type, known, declaredNamespace))
            {
                return null;
            }
            // An auto-property rather than a throwing accessor: properties read as data,
            // and a control-tree walk that touches one should not bring the page down.
            return $"{Prefix(property.Modifiers)}{property.Type} {property.Identifier.Text} {{ get; set; }}";
        }

        case Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax field:
        {
            if (!IsPublic(field.Modifiers) || !Resolves(field.Declaration.Type, known, declaredNamespace))
            {
                return null;
            }
            var isConst = field.Modifiers.Any(modifier => Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.ConstKeyword));
            var variable = field.Declaration.Variables.FirstOrDefault();
            if (variable is null)
            {
                return null;
            }
            // Constants keep their literal: callers compare and measure against them
            // (RazorHelpers.PAGE_BODY_MARKER.Length), so an empty value changes behaviour.
            if (isConst)
            {
                return variable.Initializer?.Value is Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax literal
                    ? $"public const {field.Declaration.Type} {variable.Identifier.Text} = {literal};"
                    : null;
            }
            return $"{Prefix(field.Modifiers)}{field.Declaration.Type} {variable.Identifier.Text};";
        }

        default:
            return null;
    }
}

/// <summary>
/// True when every name in a type reference resolves after the port: a predefined C# type,
/// a common BCL type, or a project type that survived or is itself stubbed.
/// Generic arguments are checked too, so List&lt;ExcludedThing&gt; is rejected.
/// </summary>
static bool Resolves(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? type, HashSet<string> known, string declaredNamespace)
{
    switch (type)
    {
        case null:
            return false;
        case Microsoft.CodeAnalysis.CSharp.Syntax.PredefinedTypeSyntax:
            return true;
        case Microsoft.CodeAnalysis.CSharp.Syntax.NullableTypeSyntax nullable:
            return Resolves(nullable.ElementType, known, declaredNamespace);
        case Microsoft.CodeAnalysis.CSharp.Syntax.ArrayTypeSyntax array:
            return Resolves(array.ElementType, known, declaredNamespace);
        case Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax generic:
            return IsKnownSimpleType(generic.Identifier.Text)
                   && generic.TypeArgumentList.Arguments.All(argument => Resolves(argument, known, declaredNamespace));
        case Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax identifier:
            return IsKnownSimpleType(identifier.Identifier.Text)
                   || known.Contains(QualifiedName(declaredNamespace, identifier.Identifier.Text));
        default:
            // Qualified names (System.Web.X) would need real resolution; leave them out.
            return false;
    }
}

/// <summary>Namespace-qualified type key ("Ns.Name", or just "Name" in the global namespace).</summary>
static string QualifiedName(string declaredNamespace, string name)
    => string.IsNullOrEmpty(declaredNamespace) ? name : declaredNamespace + "." + name;

/// <summary>BCL type names that are safe to name in a stub signature without resolving anything.</summary>
static bool IsKnownSimpleType(string name) => name switch
{
    "String" or "Object" or "Boolean" or "Int32" or "Int64" or "Double" or "Decimal"
        or "DateTime" or "Guid" or "TimeSpan" or "Uri" or "Exception" or "Type" or "Stream"
        or "List" or "IList" or "IEnumerable" or "ICollection" or "Dictionary" or "IDictionary"
        or "KeyValuePair" or "Nullable" or "IReadOnlyList" or "IReadOnlyCollection" => true,
    _ => false,
};

/// <summary>Top-level type declarations of a file.</summary>
static List<StubType> EnumerateTopLevelTypes(string source)
{
    var result = new List<StubType>();

    foreach (var declaration in CodeBehindRewriter.ParseUnit(source)
                 .DescendantNodes()
                 .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax>())
    {
        // Nested types would need their container; the container itself is stubbed
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

        result.Add(new StubType(
            $"{declaredNamespace}.{name}`{typeParameters.Length}", declaredNamespace, name, declaration));
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


/// <summary>
/// Namespaces of the user controls a code-behind file names directly. WebForms code
/// reaches a user control's type without it being in markup - the LoadControl cast is the
/// common shape - and the generated component lives in a different namespace, so the
/// using has to be added or the cast will not resolve.
/// Matching is on identifiers actually present in the source, so unrelated controls that
/// merely share a short name elsewhere in the app are not imported.
/// </summary>
static IEnumerable<string> UserControlNamespacesNamedIn(
    string source,
    IReadOnlyDictionary<string, UserControlRef> registry,
    BaseClassRegistry baseRegistry)
{
    if (registry.Count == 0)
    {
        yield break;
    }

    var identifiers = CodeBehindRewriter.ParseUnit(source)
        .DescendantNodes()
        .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax>()
        .Select(name => name.Identifier.Text)
        .ToHashSet(StringComparer.Ordinal);

    // A short name is only safe to import when exactly one user control answers to it and
    // no ported class shares it. BlogEngine has both a PostViewBase user control and a
    // PostViewBase class in BlogEngine.Core; importing the control's namespace there makes
    // every mention of the name ambiguous instead of resolving it.
    foreach (var group in registry.Values.GroupBy(control => control.ComponentName, StringComparer.Ordinal))
    {
        if (!identifiers.Contains(group.Key)
            || group.Select(control => control.Namespace).Distinct(StringComparer.Ordinal).Count() != 1
            || baseRegistry.DeclaresTypeNamed(group.Key))
        {
            continue;
        }
        yield return group.First().Namespace;
    }
}

/// <summary>A top-level type declaration that needs a stub.</summary>
sealed record StubType(
    string Key,
    string Namespace,
    string Name,
    Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax Declaration);
