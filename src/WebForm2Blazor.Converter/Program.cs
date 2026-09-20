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

// Named once because the residual that tells the reader how to supply the catalog used to
// spell it "--property-catalog", which is not a flag this converter has - the switch below
// falls through to "不明な引数" and exits 1. The message was advice that could only fail.
// A const cannot drift from the case label that uses it.
const string PropertyCatalogFlag = "--catalog";

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
        case PropertyCatalogFlag: propertyCatalogPath = args[++i]; break;
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
// (by property existence).
//
// Looked for BESIDE THE CONVERTER, not in the current directory. It used to be the
// current directory alone, which made the conversion depend on where the caller happened
// to stand: running corpora\convert-all.ps1 from the repository root loaded the catalog
// and running it from corpora\ did not, and the residual counts differed by 80 across the
// six corpora with no change to the converter at all. A measurement that moves with the
// shell's working directory is not a measurement.
//
// Silence was the worse half of that. Not finding the catalog is now said out loud, so the
// numbers can never quietly mean something else again.
propertyCatalogPath ??= FindPropertyCatalog();
if (propertyCatalogPath is not null && File.Exists(propertyCatalogPath))
{
    WebForm2Blazor.Converter.Mapping.PropertyKnowledge.Load(propertyCatalogPath);
    report.Info("(project)", $"プロパティカタログを読み込みました: {propertyCatalogPath}(expando 属性判定を実プロパティ基準で行います)。");
}
else
{
    report.Residual("(project)", ResidualKind.Configuration,
        "プロパティカタログ webforms-property-catalog.json が見つかりません。expando 属性の判定を"
        + "実プロパティではなく既定のマッピング表だけで行うため、残差が増えます"
        + $"({PropertyCatalogFlag} で明示するか、変換器の出力先に配置してください)。",
        disposition: ResidualDisposition.NeedsInput);
}

static string? FindPropertyCatalog()
{
    const string fileName = "webforms-property-catalog.json";

    // Beside the converter first: that copy travels with the build and is the one the
    // project file guarantees. Then the current directory, which is what callers used to
    // rely on. Then up from the assembly, for a run straight out of bin\Debug in a
    // working tree.
    var candidates = new List<string> { Path.Combine(AppContext.BaseDirectory, fileName), fileName };
    for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
         directory is not null;
         directory = directory.Parent)
    {
        candidates.Add(Path.Combine(directory.FullName, fileName));
    }

    return candidates.FirstOrDefault(File.Exists);
}

// Read here, ahead of everything: the porting pass below decides what to exclude, and a
// library the user has declared unreplaceable has to be known by then. The library
// MIGRATION built further down uses the same map.
var packageMap = packageMapPath is not null ? LoadPackageMap(packageMapPath) : null;

// Which library projects to port alongside the app is stated by the app's .csproj, so it
// is read rather than guessed. Getting this set wrong is the most expensive mistake in a
// conversion and it does not announce itself: the missing types surface as hundreds of
// CS0246 that read like converter or compatibility-layer failures.
// Referenced .vbproj / .fsproj: this converter cannot port them, and the build gate needs
// their names later to keep their types out of the counted errors.
var foreignLanguageProjects = new List<string>();

if (deriveIncludes)
{
    var derived = WebForm2Blazor.Converter.Project.ProjectReferenceGraph.Derive(input, entryProjectPath);
    foreignLanguageProjects.AddRange(derived.ForeignLanguage);

    if (derived.AmbiguousProjects.Count > 0)
    {
        report.Residual("(project)", ResidualKind.Configuration,
            $"{input} に .csproj が {derived.AmbiguousProjects.Count} 個あり、参照プロジェクトを自動導出できません"
            + "(相互排他のビルド構成である場合が多く、選ぶとデータベース等を暗黙に決めてしまいます)。"
            + "--project でどれを使うか指定してください: "
            + string.Join(", ", derived.AmbiguousProjects.Select(Path.GetFileName)),
            disposition: ResidualDisposition.NeedsInput);
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
            disposition: ResidualDisposition.NeedsInput);
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
                disposition: ResidualDisposition.NeedsInput);
        }
    }

    if (derived.ForeignLanguage.Count > 0)
    {
        report.Residual("(project)", ResidualKind.Configuration,
            "C# 以外のプロジェクトが参照されています。変換対象外です: "
            + string.Join(", ", derived.ForeignLanguage.Select(Path.GetFileName)),
            disposition: ResidualDisposition.OutOfScope);
    }

    if (derived.Missing.Count > 0)
    {
        report.Residual("(project)", ResidualKind.Configuration,
            "ProjectReference の参照先が見つかりません(取得漏れの可能性があります): "
            + string.Join(", ", derived.Missing.Select(Path.GetFileName)),
            disposition: ResidualDisposition.NeedsInput);
    }
}

var project = WebFormsProject.Scan(input, includeDirectories, webConfigOverride, entryProjectPath);

report.Info("(project)",
    $"棚卸し: マスターページ {project.MasterPages.Count} / ページ {project.Pages.Count} / "
    + $"ユーザーコントロール {project.UserControls.Count} / その他 .cs {project.PlainCodeFiles.Count}");

if (project.NotCompiledFiles.Count > 0)
{
    // Informational, not a residual to act on: there is nothing for anyone to migrate.
    // But it is never left unsaid - a reader comparing file counts between the input tree
    // and the output has to be able to find out where the difference went.
    const int shown = 12;
    var listed = string.Join(", ", project.NotCompiledFiles.Take(shown));
    var more = project.NotCompiledFiles.Count > shown
        ? $" ほか {project.NotCompiledFiles.Count - shown} 件"
        : string.Empty;
    report.Info("(project)",
        $"アプリの .csproj がコンパイルしないファイル {project.NotCompiledFiles.Count} 件は"
        + $"変換対象外にしました(ディスクに残っているだけで、アプリケーションの一部ではありません): {listed}{more}");
}

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
    HasCustomErrorPage = WebConfigConverter.HasCustomErrorPage(project.WebConfigPath),
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
var componentMoves =
    new List<(string OriginalNamespace, string ClassName, string TargetFullName)>();
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
    if (originalNamespace == component.TargetNamespace)
    {
        continue;
    }

    // Recorded whether or not the namespace as a whole can be mapped. When it can, the
    // rewrite below moves every reference and this list goes unused; when it cannot -
    // because ordinary ported code stays in that namespace - this is the only record that
    // the type went anywhere, and the code left behind still names it.
    componentMoves.Add((originalNamespace, component.SourceClassName, component.FullName));

    if (baseRegistry.HasNamespace(originalNamespace))
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

// Only the moves the namespace map will NOT make. Doing both would be the same rewrite
// twice, and the second one would be looking at a name that is already gone.
var unmappedNamespaces = new HashSet<string>(
    componentMoves.Select(move => move.OriginalNamespace)
        .Where(ns => !namespaceMap.TryGetValue(ns, out var targets)
                     || targets.Distinct(StringComparer.Ordinal).Count() != 1),
    StringComparer.Ordinal);

var relocatedTypes = WebForm2Blazor.Converter.Convert.RelocatedTypeIndex.Build(
    componentMoves.Where(move => unmappedNamespaces.Contains(move.OriginalNamespace)));

if (!relocatedTypes.IsEmpty)
{
    report.Info("(project)",
        $"コンポーネントになって名前空間が変わった型 {relocatedTypes.Count} 件について、"
        + "元の名前で参照している移植コードに別名(using X = ...)を補いました"
        + "(その名前空間には移動しない移植コードが残るため、名前空間ごとの付け替えはできません)。");
}

/// <summary>
/// Turns a reference written RELATIVE to the file's own namespace into the full name, so
/// the rewrites below - which all match on the full name - can see it.
///
/// mojoBasePage.cs declares "namespace mojoPortal.Web" and writes
///
///     if (this is UI.Pages.LoginPage)
///
/// C# resolves that by walking outward from the declaration, so it names
/// mojoPortal.Web.UI.Pages.LoginPage without the characters ever appearing. The namespace
/// map moved that namespace and matched nothing, and the reference was left pointing at a
/// namespace this converter had emptied.
///
/// Only spellings of two segments or more are expanded. A single segment ("Pages.Login")
/// is equally likely to be a local variable or a property, and turning one of those into a
/// namespace would be a much worse error than the one being fixed.
/// </summary>
string ExpandRelativeNamespaceReferences(string code)
{
    var enclosing = new HashSet<string>(StringComparer.Ordinal);
    foreach (System.Text.RegularExpressions.Match match in
             System.Text.RegularExpressions.Regex.Matches(code, @"namespace\s+([A-Za-z_][\w.]*)"))
    {
        var name = match.Groups[1].Value;
        while (name.Length > 0)
        {
            enclosing.Add(name);
            var cut = name.LastIndexOf('.');
            name = cut < 0 ? string.Empty : name[..cut];
        }
    }

    if (enclosing.Count == 0)
    {
        return code;
    }

    // Both rewriters that follow match on the full name, so both need the normalization:
    // the map's namespaces AND the ones RelocatedTypeIndex moved types out of.
    foreach (var original in namespaceRewrites.Select(pair => pair.Key)
                 .Concat(relocatedTypes.OriginalNamespaces))
    {
        foreach (var scope in enclosing)
        {
            if (!original.StartsWith(scope + ".", StringComparison.Ordinal))
            {
                continue;
            }

            var relative = original[(scope.Length + 1)..];
            if (!relative.Contains('.'))
            {
                continue;
            }

            code = System.Text.RegularExpressions.Regex.Replace(
                code,
                @"(?<![\w.])" + System.Text.RegularExpressions.Regex.Escape(relative) + @"\.(?=[A-Za-z_])",
                _ => original + ".");
        }
    }

    return code;
}

string ApplyNamespaceMap(string code, bool razorContent = false)
{
    // A .razor file has no namespace declaration to be relative to.
    if (!razorContent)
    {
        code = ExpandRelativeNamespaceReferences(code);
    }

    // Before the namespace rewrite below, which deletes the imports this reads.
    code = razorContent ? relocatedTypes.RewriteQualified(code) : relocatedTypes.Apply(code);

    foreach (var (originalNamespace, targets) in namespaceRewrites)
    {
        var distinct = targets.Distinct(StringComparer.Ordinal).ToList();
        if (distinct.Count == 1)
        {
            // Qualified type references follow only when the mapping is unambiguous.
            //
            // Anchored at a name boundary, not a plain substring replace. The target
            // CONTAINS the original ("DesktopModules.Admin.Security" ->
            // "dnn.Components.Controls.DesktopModules.Admin.Security"), so a substring
            // replace matches inside its own output and prefixes it twice:
            // "dnn.Components.Controls.dnn.Components.Controls.DesktopModules.Admin.
            // Security.DNNProfile". That reached both the .razor tag and the field it
            // generates, so DNN lost four user-control references to it.
            code = System.Text.RegularExpressions.Regex.Replace(
                code,
                @"(?<![\w.])" + System.Text.RegularExpressions.Regex.Escape(originalNamespace) + @"\.",
                // A literal replacement: "$" in a namespace would otherwise be read as a
                // substitution.
                _ => distinct[0] + ".");
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

// Libraries declined in the package map (an entry naming the assembly and no package).
// Their namespaces come from the assembly, so the exclusion covers exactly what the
// library declared - nothing guessed from the name.
//
// EXCEPT a namespace the application itself declares. HtmlDiff.dll puts its three types in
// a namespace called "Helpers", and mojoPortal has its own Helpers; declining the library
// therefore condemned 42 of the application's own files, none of which had ever heard of
// HtmlDiff. A namespace that still has code in it after the library is gone is not the
// library's to take away.
if (packageMap is not null)
{
    var portDeclares = new HashSet<string>(
        candidateNamespaces.SelectMany(entry => entry.Declared), StringComparer.Ordinal);

    var declined = new List<string>();
    var kept = new List<string>();
    foreach (var (assemblyName, choices) in packageMap)
    {
        if (choices.Any(choice => !string.IsNullOrEmpty(choice.Package)))
        {
            continue;
        }

        var assemblyPath = FindBuiltAssembly(input, assemblyName)
            ?? includeDirectories.Select(directory => FindBuiltAssembly(directory, assemblyName))
                .FirstOrDefault(found => found is not null);
        if (assemblyPath is null)
        {
            continue;
        }

        var namespaces = new HashSet<string>(StringComparer.Ordinal);
        WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.ReadPublicTypes(
            assemblyPath, (typeNamespace, _) => namespaces.Add(typeNamespace));

        var shared = namespaces.Where(portDeclares.Contains).ToList();
        namespaces.ExceptWith(shared);
        if (shared.Count > 0)
        {
            kept.Add($"{assemblyName}({string.Join("/", shared)})");
        }

        if (namespaces.Count == 0)
        {
            continue;
        }

        WebForm2Blazor.Converter.Convert.PortabilityRules.Decline(namespaces);
        declined.Add($"{assemblyName}({namespaces.Count} 名前空間)");
    }

    if (declined.Count > 0)
    {
        report.Info("(project)",
            "--package-map で「置き換え先なし」と指定されたライブラリを移植対象外にしました: "
            + string.Join(", ", declined)
            + "。これらに依存するファイルは除外し、宣言していた型はスタブにします"
            + "(参照側はコンパイルできます)。");
    }

    if (kept.Count > 0)
    {
        report.Residual("(project)", ResidualKind.Configuration,
            "置き換え先なしと指定されたライブラリが、アプリ自身も宣言している名前空間を使っています: "
            + string.Join(", ", kept)
            + "。その名前空間は除外対象にしていません(アプリのコードまで巻き込むため)。"
            + "このライブラリの型を参照している箇所は CS0246 として残ります。",
            disposition: ResidualDisposition.NeedsInput);
    }
}

var excludedCandidates = new HashSet<int>();

// Files excluded because they belong to a framework this converter does not convert.
// Kept apart from the rest because that exclusion is reversible: see the restore pass
// below the loop.
var outOfScopeExclusions = new HashSet<int>();

for (var i = 0; i < candidateNamespaces.Count; i++)
{
    if (FindUnportableNamespace(candidateNamespaces[i].candidate.Source) is { } unportable)
    {
        excludedCandidates.Add(i);

        // Only a file belonging to ANOTHER FRAMEWORK is a candidate for restoring. A file
        // built on a namespace whose API is simply gone (System.Data.Linq, LINQ to SQL) is
        // not: restoring one of those cost BlogEngine 55 errors, because every line of it
        // needs types that do not exist. The other framework's file needs a handful.
        if (OutOfScopeFrameworkNote(unportable).Contains("この変換器は WebForms のみ", StringComparison.Ordinal))
        {
            outOfScopeExclusions.Add(i);
        }
        report.Residual(candidateNamespaces[i].candidate.ReportName, ResidualKind.CodeBehind,
            OutOfScopeFrameworkNote(unportable), disposition: ResidualDisposition.OutOfScope);
        continue;
    }

    if (PortabilityRules.FindFrameworkIntegrationNamespace(candidateNamespaces[i].Declared) is { } integration)
    {
        excludedCandidates.Add(i);
        report.Residual(candidateNamespaces[i].candidate.ReportName, ResidualKind.CodeBehind,
            $"名前空間 {integration} は .NET に存在しないライブラリの統合コードのため移植から除外しました。",
            disposition: ResidualDisposition.OutOfScope);
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
/// <summary>
/// Whether the code writes this type's simple name as an identifier.
///
/// Only used to decide that an excluded file should be RESTORED, never to exclude one -
/// see the note above <see cref="UsesGoneType"/> for why the other direction is closed.
/// </summary>
static bool MentionsTypeName(string code, string typeName)
    => !string.IsNullOrEmpty(typeName)
       && System.Text.RegularExpressions.Regex.IsMatch(
           code, @"(?<![\w.])" + System.Text.RegularExpressions.Regex.Escape(typeName) + @"\b");

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

/// <summary>
/// Whether the source names any type declared in <paramref name="ns"/> - qualified or not.
///
/// Used to decide whether an import of an emptied namespace is a real dependency. A bare
/// identifier is weak evidence, which is why it is only ever used to keep a file excluded,
/// never to exclude one (see the note above UsesGoneType).
/// </summary>
bool MentionsAnyTypeOf(string source, string ns)
{
    var identifiers = new HashSet<string>(StringComparer.Ordinal);
    foreach (var identifier in CodeBehindRewriter.ParseUnit(source)
                 .DescendantNodes()
                 .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax>())
    {
        identifiers.Add(identifier.Identifier.Text);
    }

    for (var index = 0; index < declaredTypes.Count; index++)
    {
        foreach (var declared in declaredTypes[index])
        {
            if (string.Equals(declared.Namespace, ns, StringComparison.Ordinal)
                && identifiers.Contains(declared.Type))
            {
                return true;
            }
        }
    }

    return false;
}

HashSet<string> ComputeFullyExcludedNamespaces()
    => candidateNamespaces
        .SelectMany((entry, index) => entry.Declared.Select(ns => (ns, index)))
        .GroupBy(pair => pair.ns, StringComparer.Ordinal)
        .Where(group => group.All(pair => excludedCandidates.Contains(pair.index)))
        .Select(group => group.Key)
        .ToHashSet(StringComparer.Ordinal);

// An exclusion exists to avoid local errors. When the file it removes is one that
// SURVIVING files use, it stops doing that and starts a cascade instead - and
// PortabilityRules' own caveat is that "an exclusion cascade produces far more damage than
// the local errors do".
//
// YAF's AspNetUsers is the case that made this visible. It is a data model - a table POCO -
// that happens to implement Microsoft.AspNet.Identity's IUser<TKey>, so it was excluded as
// Identity code. That took BoardContext with it, and BoardContext is the thing half the
// application reads its settings from: 1614 of YAF's 2135 build errors were
// "BoardContext has no definition for ...".
//
// So an out-of-scope exclusion is undone when a surviving file names one of its types.
// Ported, the file costs a handful of local errors for the interface that is not here.
// Excluded, it costs everything that depends on it. The evidence is the same qualified
// reference the cascade uses, so a file nobody names stays excluded.
// The files this pass hands back. They are known to carry unresolved types - that is the
// deal being struck - so the build gate counts their errors separately, the way it already
// counts the ones from a dependency nobody has chosen a package for. Written down rather
// than inferred later: the moment of the decision is the only place that knows.
var restoredOutOfScopeFiles = new List<string>();

bool restoredAny;
do
{
    restoredAny = false;
    foreach (var index in outOfScopeExclusions.ToList())
    {
        var offered = declaredTypes[index];
        if (offered.Count == 0)
        {
            continue;
        }

        // Two kinds of evidence, with different bars.
        //
        // A QUALIFIED reference is unambiguous, so one is enough - and a threshold of two
        // was measured and discarded: YAF's AspNetUsers is named by exactly one surviving
        // file, BoardContext, and it is BoardContext that half the application reads from.
        // The size of the cascade is what matters, and a direct reference is the only part
        // of that this pass can see cheaply.
        //
        // The SIMPLE name is weak evidence - the same identifier can be anything - so it
        // takes several. Accepting one measured badly: yaf fell 598 -> 46 but DNN rose
        // 19 -> 169, n2 32 -> 96 and mojoPortal 13 -> 40, because a single incidental
        // mention dragged back genuinely-MVC files. Several call sites is what a piece of
        // wrongly-classified infrastructure looks like: YAF's IAspNetUsersHelper has 235.
        var qualified = 0;
        var mentions = 0;
        for (var other = 0; other < candidateNamespaces.Count && qualified < 1 && mentions < 5; other++)
        {
            if (other == index || excludedCandidates.Contains(other))
            {
                continue;
            }

            var code = PortabilityRules.WithoutStringsAndComments(
                candidateNamespaces[other].candidate.Source);

            // The SIMPLE name counts here, not only the qualified one. Almost nothing is
            // written qualified - YAF asks for its user helper as
            // "this.Get<IAspNetUsersHelper>()" - so a qualified-only test saw none of the
            // 346 errors that interface was responsible for.
            //
            // Weak evidence is safe in this direction and only in this direction. Used to
            // EXCLUDE a file it has run away every time it was tried (see the note above
            // UsesGoneType: 1 error became 28, then 705); used to KEEP one, the worst case
            // is a file that ports and contributes a few local errors.
            if (offered.Any(declared => UsesGoneType(code, (declared.Type, declared.Namespace))))
            {
                qualified++;
            }
            else if (offered.Any(declared => MentionsTypeName(code, declared.Type)))
            {
                mentions++;
            }
        }

        if (qualified < 1 && mentions < 5)
        {
            continue;
        }

        excludedCandidates.Remove(index);
        outOfScopeExclusions.Remove(index);
        restoredAny = true;
        restoredOutOfScopeFiles.Add(candidateNamespaces[index].candidate.OutputRelative);
        report.Residual(candidateNamespaces[index].candidate.ReportName, ResidualKind.CodeBehind,
            "別フレームワークのコードとして除外しましたが、移植されるファイルがこのファイルの型を使っているため移植します"
            + "(除外すると連鎖でそちら側が落ちます。このファイル内には未解決の型が残ります)。",
            disposition: ResidualDisposition.Backlog);
    }
}
while (restoredAny);

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
        // Importing a namespace that lost every file is only fatal if the file actually
        // uses something from it. An import alone is not a dependency, and treating it as
        // one cascades hard: DNN's HtmlUtils.cs imports DotNetNuke.Services.Upgrade
        // without naming a type from it, which took Globals.cs with it and 22 more after
        // that - from two files excluded for System.Web.Compilation and System.Data.Linq.
        //
        // NAME-BASED, not qualified-name-based, and that direction is deliberate. Chasing
        // unqualified uses to EXCLUDE more was measured three times and ran away every
        // time (see UsesGoneType). Here the same evidence is used to KEEP a file only when
        // no name from the namespace appears at all, so an uncertain case stays excluded.
        var dependency = EnumerateUsingNamespaces(source)
            .Where(ns => emptyNamespaces.Contains(ns))
            .FirstOrDefault(ns => MentionsAnyTypeOf(source, ns));
        if (dependency is not null)
        {
            excludedCandidates.Add(i);
            cascadeChanged = true;
            report.Residual(candidateNamespaces[i].candidate.ReportName, ResidualKind.CodeBehind,
                $"移植から除外済みの名前空間 {dependency} に依存するため、連鎖して除外しました(根が移植されれば追随できます)。", disposition: ResidualDisposition.Backlog);
            continue;
        }

        // Blanked FIRST, so a type named in a STRING is not read as a dependency. That is
        // the same mistake PortabilityRules made and the same fix: BlogEngine's
        // CodeExpressionBuilder carries
        //     [ExpressionEditor("BlogEngine.Core.Compilation.Design.CodeExpressionEditor, BlogEngine.Core")]
        // and four of its expression builders were excluded for a string literal - the
        // attribute names the editor by TEXT precisely because it does not reference it.
        var code = WebForm2Blazor.Converter.Convert.PortabilityRules.WithoutStringsAndComments(source);
        if (goneTypes.FirstOrDefault(gone => UsesGoneType(code, gone)) is { Type.Length: > 0 } goneType)
        {
            excludedCandidates.Add(i);
            cascadeChanged = true;
            report.Residual(candidateNamespaces[i].candidate.ReportName, ResidualKind.CodeBehind,
                $"移植から除外済みの型 {goneType.Namespace}.{goneType.Type} に依存するため、連鎖して除外しました(根が移植されれば追随できます)。", disposition: ResidualDisposition.Backlog);
        }
    }
} while (cascadeChanged);

var fullyExcludedNamespaces = ComputeFullyExcludedNamespaces();

// Namespaces ExcludedTypeStubs.g.cs re-creates.
//
// A fully-excluded namespace is NOT empty after the port - the stub file declares its
// types again, which is the whole point of that file. Dropping an "@using" that names one
// was right before stubs existed and has been wrong since: YAF's AlbumImageList writes
// &lt;%@ Import Namespace="YAF.Core.Context.Start" %&gt; and reads WebApiConfig.UrlPrefix
// from it, and the import was removed under the reason "移植後に型が残らない" while the
// stub for WebApiConfig sat in the generated file.
//
// Computed from the excluded sources rather than assumed equal to the fully-excluded set,
// so that a namespace the generator cannot stub still counts as gone.
var stubbedNamespaces = Enumerable.Range(0, candidateNamespaces.Count)
    .Where(excludedCandidates.Contains)
    .SelectMany(index => EnumerateTopLevelTypes(candidateNamespaces[index].candidate.Source))
    .Select(type => type.Namespace)
    .Where(ns => !string.IsNullOrEmpty(ns))
    .ToHashSet(StringComparer.Ordinal);

var deadNamespacesForMarkup = fullyExcludedNamespaces
    .Where(ns => !stubbedNamespaces.Contains(ns))
    .ToHashSet(StringComparer.Ordinal);

var unsafeCodePorted = false;
var assemblyAttributesPorted = false;

var portedSources = Enumerable.Range(0, candidateNamespaces.Count)
    .Where(index => !excludedCandidates.Contains(index))
    .Select(index => candidateNamespaces[index].candidate.Source)
    .ToList();

// The compat layer is one namespace where WebForms had a dozen, so importing it brings in
// names the original import never had - see CompatImportDisambiguator.
var compatImports = WebForm2Blazor.Converter.Convert.CompatImportDisambiguator.Build(portedSources);

// What the ported classes declare and derive from, so an override can be checked against
// the whole chain rather than only a direct compat base.
var portedTypes = WebForm2Blazor.Converter.Convert.PortedTypeIndex.Build(portedSources);

// Libraries the user pointed at a .NET replacement AND gave its assembly for. Both sides
// are read and the names that match are rewritten; what does not match is reported, not
// guessed at. Built once, applied to every generated file.
var libraryMigrations = new List<(string Assembly, WebForm2Blazor.Converter.Convert.AssemblyTypeMigration Migration)>();

// Types the replacement library has no name for AND the ported code still mentions.
//
// These are why the numbers below can be read at all. Six unresolvable using-aliases in
// mojoPortal (com.drew.metadata.Metadata and AbstractDirectory) failed the Razor SDK's
// DECLARATION pass, which stops the build before the real compile ever runs - so the
// corpus reported "6 build errors" for as long as they were there, while the actual
// figure behind them was 2202. A floor that low reads as "almost done" and makes every
// before/after comparison on that corpus meaningless.
//
// They get exactly the treatment the excluded-type stubs get, for exactly the same reason
// (see GenerateExcludedTypeStubs): re-declare the name and nothing else. Code that only
// MENTIONS the type compiles; code that used its members fails at the member, which is
// where the migration work actually is. Nothing is hidden - each one is already a
// NeedsInput residual above, and stays one.
var migratedAwayTypes = new List<(string FullName, WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.TypeShape Shape)>();
foreach (var (assemblyName, replacementPaths) in
         WebForm2Blazor.Converter.Convert.AssemblyTypeMigration.ReplacementAssemblies)
{
    // The ORIGINAL assembly, found in the repository the same way the undecided-dependency
    // scan finds it. Both sides are needed: the migration is a comparison, and one list of
    // names on its own says nothing about what moved where.
    var originalPath = FindBuiltAssembly(input, assemblyName)
        ?? includeDirectories.Select(directory => FindBuiltAssembly(directory, assemblyName))
            .FirstOrDefault(found => found is not null);

    if (originalPath is null)
    {
        report.Residual("(project)", ResidualKind.Configuration,
            $"--package-map の {assemblyName} に dll を指定しましたが、元のアセンブリが見つからないため"
            + "名前の対応を取れません(元と新の両方を読んで初めて突き合わせられます)。",
            disposition: ResidualDisposition.NeedsInput);
        continue;
    }

    if (WebForm2Blazor.Converter.Convert.AssemblyTypeMigration.Build(originalPath, replacementPaths)
        is not { } migration || migration.IsEmpty)
    {
        report.Residual("(project)", ResidualKind.Configuration,
            $"{assemblyName} と置き換え先のあいだに、名前が一致する型が見つかりませんでした"
            + "(別物のライブラリか、dll の指定先が違います)。",
            disposition: ResidualDisposition.NeedsInput);
        continue;
    }

    libraryMigrations.Add((assemblyName, migration));
    report.Info("(project)",
        $"{assemblyName} を置き換え先へ移行します: 型 {migration.RenamedTypeCount} 件を対応付け、"
        + $"名前空間 {migration.RenamedNamespaces.Count} 件を書き換えます。");

    // What is left over, reported ONE TYPE AT A TIME and only for the types the code
    // actually uses.
    //
    // It used to be a single residual listing ten names and "ほか". That is a fact, not a
    // task: MetaDataExtractor alone leaves 50 unmatched names and mojoPortal references
    // two of them. Forty-eight of those names describe a library the application never
    // touched, and burying the two that matter among them is how a report stops being
    // read.
    //
    // Each one that IS used becomes its own entry, with the files that name it and the
    // types the replacement actually declares in the namespace the rest of its neighbours
    // went to. That is a question with a short answer list, which is what the AI layer can
    // work from - as opposed to "what did com.drew.metadata.AbstractDirectory become",
    // which asks it to remember.
    var usedUnmatched = migration.Unmatched
        .Select(name => (Name: name, Users: FilesReferencing(name)))
        .Where(entry => entry.Users.Count > 0)
        .OrderBy(entry => entry.Name, StringComparer.Ordinal)
        .ToList();

    foreach (var (unmatchedName, users) in usedUnmatched)
    {
        var candidates = migration.CandidatesFor(unmatchedName);
        report.Residual("(project)", ResidualKind.CodeBehind,
            $"{assemblyName} の {unmatchedName} は置き換え先に同名の型がありません"
            + "(改名されたか、なくなったかのどちらかです)。"
            + $"参照しているファイル: {string.Join(", ", users.Take(5))}"
            + (users.Count > 5 ? $" ほか {users.Count - 5} 件" : string.Empty)
            + (candidates.Count > 0
                ? $"。同じ名前空間の移行先が宣言している型: {string.Join(", ", candidates)}"
                : "。置き換え先に対応する名前空間がありません"),
            disposition: ResidualDisposition.NeedsInput);

        migratedAwayTypes.Add((unmatchedName, migration.ShapeOf(unmatchedName)));
    }

    if (migration.Unmatched.Count > usedUnmatched.Count)
    {
        report.Info("(project)",
            $"{assemblyName} の型 {migration.Unmatched.Count - usedUnmatched.Count} 件も置き換え先に"
            + "同名のものがありませんが、移植コードは参照していないため残差にしていません。");
    }
}

/// <summary>
/// Ported files that name this type - fully qualified, or by its simple name with the
/// namespace imported. Both spellings count: a using alias
/// ("using MetadataDirectory = com.drew.metadata.AbstractDirectory;") is the qualified
/// form, and the code around it uses the alias.
/// </summary>
List<string> FilesReferencing(string fullName)
{
    var simpleName = fullName[(fullName.LastIndexOf('.') + 1)..];
    var namespaceName = fullName[..Math.Max(0, fullName.LastIndexOf('.'))];

    var qualified = new System.Text.RegularExpressions.Regex(
        @"(?<![\w.])" + System.Text.RegularExpressions.Regex.Escape(fullName) + @"(?![\w])");
    var bare = new System.Text.RegularExpressions.Regex(
        @"(?<![\w.])" + System.Text.RegularExpressions.Regex.Escape(simpleName) + @"(?![\w])");
    var imports = new System.Text.RegularExpressions.Regex(
        @"(?m)^\s*using\s+" + System.Text.RegularExpressions.Regex.Escape(namespaceName) + @"\s*;");

    var users = new List<string>();
    for (var index = 0; index < candidateNamespaces.Count; index++)
    {
        if (excludedCandidates.Contains(index))
        {
            continue;
        }

        var code = PortabilityRules.WithoutStringsAndComments(
            candidateNamespaces[index].candidate.Source);
        if (qualified.IsMatch(code) || (imports.IsMatch(code) && bare.IsMatch(code)))
        {
            users.Add(candidateNamespaces[index].candidate.ReportName);
        }
    }
    return users;
}

string ApplyLibraryMigrations(string code)
{
    foreach (var (_, migration) in libraryMigrations)
    {
        code = migration.Apply(code);
    }
    return code;
}

// The same question asked of a real compilation: what a base actually IS, rather than what
// its written name looks like. Only used where the name-based lookups have nothing to say.
// NOT WIRED YET - see corpora/README.md. Building it and letting the override-drop pass
// use it made YAF drop exactly one override (the right one, DbProviderFactory.CreatePermission,
// removed from .NET with Code Access Security) and took that corpus from 1 build error to
// 2135, in files that have nothing to do with it. The output for the dropped member is
// correct, so the damage is somewhere else in the pipeline and is not understood yet.
// Shipping it in that state would trade one honest error for two thousand.
WebForm2Blazor.Converter.Convert.CodeBehindRewriter.Semantics = WebForm2Blazor.Converter.Convert.SemanticBaseIndex.Build(portedSources);

// A global using does not appear in the file it reaches, so the disambiguator has to be
// told about them. The web project's stay global (its pages' generated halves rely on
// them) and so does the compat namespace the generated project imports for everyone.
compatImports.WithAmbientImports(
    Enumerable.Range(0, candidateNamespaces.Count)
        .Where(index => !excludedCandidates.Contains(index))
        .Where(index => !candidateNamespaces[index].candidate.Included)
        .SelectMany(index => CodeBehindRewriter.ParseUnit(
                candidateNamespaces[index].candidate.Source).Usings
            .Where(directive => directive.GlobalKeyword.RawKind
                                == (int)Microsoft.CodeAnalysis.CSharp.SyntaxKind.GlobalKeyword)
            .Where(directive => directive.Alias is null && directive.StaticKeyword.RawKind == 0)
            .Select(directive => directive.Name?.ToString())
            .OfType<string>())
        .Append("WebForm2Blazor.Components"));

// The WEB project's global usings, as plain namespace names.
//
// They are about to stop being global - left that way they are in force for the whole
// merged compilation, and YAF's "global using YAF.Types.Constants;" reached the vendored
// Lucene.Net sources, where Constants then meant two different types. The generated half
// of each page is where they belong instead: those files ARE the web project, and the
// imports a file sees have to be the imports it had.
var webProjectGlobalUsings = Enumerable.Range(0, candidateNamespaces.Count)
    .Where(index => !excludedCandidates.Contains(index))
    .Where(index => !candidateNamespaces[index].candidate.Included)
    .SelectMany(index => CodeBehindRewriter.ParseUnit(
            candidateNamespaces[index].candidate.Source).Usings
        .Where(directive => directive.GlobalKeyword.RawKind
                            == (int)Microsoft.CodeAnalysis.CSharp.SyntaxKind.GlobalKeyword)
        .Where(directive => directive.Alias is null)
        // "global using static System.FormattableString;" carried MEMBERS, not a
        // namespace, and dropping it left every call to Invariant($"...") as "the name
        // Invariant does not exist in the current context" - 19 of them across mojoPortal's
        // code-behinds, which is the whole point of a project-wide static import.
        //
        // The "static" is kept beside the name rather than glued in front of it, because
        // the System.Web filter below reads the NAME. Prefixing first was tried and hid
        // "global using static System.Web.Security.AntiXss.AntiXssEncoder;" from that
        // filter, which put an import of a namespace that does not exist into 385 files -
        // and, being a declaration-stage error, floored the whole count at 385.
        .Select(directive => (
            Name: directive.Name?.ToString(),
            IsStatic: directive.StaticKeyword.RawKind
                      == (int)Microsoft.CodeAnalysis.CSharp.SyntaxKind.StaticKeyword)))
    .Where(entry => entry.Name is not null)
    // Not the ones the conversion removes. A "global using System.Web.UI;" is exactly what
    // the rewriter strips from every file it touches - writing it back into the generated
    // half re-imports a namespace that does not exist, which is 171 CS0234 in YAF.
    .Where(entry => entry.Name != "System.Web"
                   && !entry.Name!.StartsWith("System.Web.", StringComparison.Ordinal))
    .Select(entry => entry.IsStatic ? "static " + entry.Name : entry.Name!)
    .Distinct(StringComparer.Ordinal)
    .ToList();

// The same, over EVERY ported project rather than only the web one.
//
// Used where the question is "what could this file's names have resolved to", not "what
// does the generated web project import". Each project has its own GlobalUsings.cs -
// YAF has nine - so a type declared in YAF.Types resolves its own neighbours through
// YAF.Types' list, which the web project's does not contain.
//
// A union is wider than any single project's list, and that is a real cost: a simple name
// declared in two ported namespaces could resolve to the wrong one. It is bounded by
// resolution only accepting names that actually exist, and it is only used for stub
// signatures, where the alternative measured worse - the member is dropped entirely and
// the error moves to whoever called it.
var portedProjectGlobalUsings = Enumerable.Range(0, candidateNamespaces.Count)
    .Where(index => !excludedCandidates.Contains(index))
    .SelectMany(index => CodeBehindRewriter.ParseUnit(
            candidateNamespaces[index].candidate.Source).Usings
        .Where(directive => directive.GlobalKeyword.RawKind
                            == (int)Microsoft.CodeAnalysis.CSharp.SyntaxKind.GlobalKeyword)
        .Where(directive => directive.Alias is null && directive.StaticKeyword.RawKind == 0)
        .Select(directive => directive.Name?.ToString()))
    .OfType<string>()
    .Where(name => name != "System.Web"
                   && !name.StartsWith("System.Web.", StringComparison.Ordinal))
    .Distinct(StringComparer.Ordinal)
    .ToList();

foreach (var component in components)
{
    var directory = Path.Combine(output, component.OutputDirectory.Replace('/', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(directory);

    // A page / user control whose code-behind depends on Framework-only infrastructure
    // (OWIN auth etc.) or on a namespace whose files were all excluded cannot work by
    // porting - emit an honest placeholder that keeps the route and the component name
    string? unportablePage = null;
    IReadOnlyDictionary<string, string> codeBehindProperties =
        new Dictionary<string, string>(StringComparer.Ordinal);
    if (component.Kind is not CodeBehindKind.Layout && component.CodeBehindSourcePath is not null)
    {
        var codeBehindSource = File.ReadAllText(component.CodeBehindSourcePath);
        unportablePage = FindUnportableNamespace(codeBehindSource)
            ?? EnumerateUsingNamespaces(codeBehindSource)
                .FirstOrDefault(ns => fullyExcludedNamespaces.Contains(ns));

        // The placeholder keeps the SURFACE as well as the name. A user control that
        // cannot be ported is still set up by whoever hosts it, and dropping its
        // properties moved the failure to the caller: mojoPortal's ImageCropper depends
        // on Microsoft.Ajax.Utilities, and the dialog that opens it sets 14 properties on
        // it - 14 errors in a file that converted perfectly well.
        //
        // Same rule as the excluded-type stubs: re-declare what the type offered, so the
        // code that only MENTIONS it compiles, and the residual above still says the
        // component itself needs hand-migration.
        if (unportablePage is not null)
        {
            codeBehindProperties = AspxConverters.PublicPropertyTypes(codeBehindSource);
        }
    }
    if (unportablePage is not null)
    {
        report.Residual(project.RelativePath(component.CodeBehindSourcePath), ResidualKind.CodeBehind,
            $".NET Framework 専用の名前空間 {unportablePage} に依存するため、ページ全体をプレースホルダー化しました(認証/OWIN は別フレームワーク)。", disposition: ResidualDisposition.OutOfScope);
        File.WriteAllText(Path.Combine(directory, component.ComponentName + ".razor"),
            GenerateUnportablePlaceholder(component, unportablePage, codeBehindProperties));
        continue;
    }

    LintGeneratedRazor(component, report);
    File.WriteAllText(Path.Combine(directory, component.ComponentName + ".razor"),
        ApplyNamespaceMap(
            StripDeadUsings(
                WithGlobalUsings(component.RazorContent, webProjectGlobalUsings, component.TargetNamespace),
                deadNamespacesForMarkup, report, component.ComponentName),
            razorContent: true));

    if (component.CodeBehindSourcePath is not null)
    {
        var sourceName = project.RelativePath(component.CodeBehindSourcePath);
        var codeBehindSource = File.ReadAllText(component.CodeBehindSourcePath);

        // A PAGE's imports imply packages too. Only the plain library files were scanned,
        // so a library used exclusively from a page went unreferenced: mojoPortal charts
        // from SiteStatisticsModule.razor.cs and SalesByItemPage.razor.cs, and nothing
        // added ZedGraph because no library file mentions it.
        CollectUsingNamespaces(codeBehindSource, portedNamespaces);
        CollectQualifiedPackageNamespaces(codeBehindSource, portedNamespaces);
        // Page code-behind holds fields of user controls, so add the namespaces of the
        // user controls this component actually references to the usings. Markup is only
        // half the story: code-behind also names a user control's type without placing it
        // in markup - (UserControlSettings)Page.LoadControl("Settings.ascx") - so the
        // namespaces of controls NAMED IN THE CODE are added too. Only names the file
        // actually mentions, to avoid dragging in same-named controls from other folders.
        var controlUsings = component.UsedControlNamespaces
            .Concat(UserControlNamespacesNamedIn(codeBehindSource, userControlRegistry, baseRegistry))
            .Concat(webProjectGlobalUsings)
            .Distinct(StringComparer.Ordinal)
            .ToList();
        var rewritten = CodeBehindRewriter.Rewrite(
            codeBehindSource, component, sourceName, report,
            controlUsings, baseRegistry, portedTypes);
        File.WriteAllText(Path.Combine(directory, component.ComponentName + ".razor.cs"),
            ApplyLibraryMigrations(ApplyNamespaceMap(compatImports.Apply(rewritten))));
    }
    else if (component.Fields.Count > 0)
    {
        // Even a markup-only page (no code-behind) needs a receptacle for @ref.
        //
        // Through ApplyNamespaceMap like every other generated file. It was the one output
        // that skipped it, and the fields name PORTED control types - which are
        // re-namespaced - so YAF's ForumPageBase ended up with
        // "global::YAF.Web.Controls.Form" beside a .razor that correctly said
        // "yaf.Components.Pages.Web.Controls.Form". One file, one error, and the cause was
        // simply that this branch wrote straight to disk.
        File.WriteAllText(
            Path.Combine(directory, component.ComponentName + ".razor.cs"),
            ApplyLibraryMigrations(ApplyNamespaceMap(GenerateFieldOnlyCodeBehind(component))));
        report.Info(component.ComponentName, "コードビハインドがないため、コントロールのフィールドのみを生成しました。");
    }
}

// A "global using" covers the whole COMPILATION. The original solution compiled each
// library as its own assembly, so YAF.Web's "global using System.IO;" reached YAF.Web and
// nothing else; here everything is merged into one project and it reaches the vendored
// Lucene.Net sources too, where "Directory" then means both System.IO.Directory and
// Lucene's own. That is the whole of YAF.NET's CS0104 count and the original had none of
// it - the imports a file sees have to be the imports it had.
//
// So a library's global usings are turned back into file-level usings on that library's
// own files. The web project's stay global: its files are the pages, whose generated
// .razor / .razor.cs halves are written elsewhere and would otherwise lose them.
var libraryGlobalUsings = new Dictionary<string, List<string>>(StringComparer.Ordinal);

static string OwningProjectOf(string outputRelative, bool included)
    => included ? outputRelative.Split('/')[0] : string.Empty;

// Drops the "global" keyword from this file's own global usings and gives the file every
// global using its library declared, so the file ends up with exactly the imports the
// original compilation gave it - no more (the leak this fixes) and no less.
static string UsingText(Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax directive)
    => System.Text.RegularExpressions.Regex.Replace(
        directive.WithGlobalKeyword(default).ToString(), @"\s+", " ").Trim();

static string ScopeGlobalUsings(string source, IReadOnlyList<string> libraryUsings)
{
    var unit = CodeBehindRewriter.ParseUnit(source);

    var localised = unit.Usings
        .Select(directive => directive.GlobalKeyword.RawKind
                             == (int)Microsoft.CodeAnalysis.CSharp.SyntaxKind.GlobalKeyword
            ? ParseUsing(UsingText(directive))
            : directive)
        .ToList();

    var present = new HashSet<string>(localised.Select(UsingText), StringComparer.Ordinal);

    foreach (var text in libraryUsings)
    {
        if (present.Add(text))
        {
            localised.Add(ParseUsing(text));
        }
    }

    return WebForm2Blazor.Converter.Convert.SyntaxUsings.Replace(unit, localised).ToFullString();
}

static Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax ParseUsing(string text)
    => Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseCompilationUnit(text + "\r\n").Usings[0];

for (var i = 0; i < candidateNamespaces.Count; i++)
{
    if (excludedCandidates.Contains(i))
    {
        continue;
    }
    var (candidate, _) = candidateNamespaces[i];

    // The WEB project's global usings are collected too, under the "" owner. They used to
    // be left global on the grounds that the pages' generated halves would lose them -
    // which is true, and handled by giving those halves the same usings explicitly. What
    // leaving them global actually did was put them in force for the WHOLE merged
    // compilation: YAF's "global using YAF.Types.Constants;" reached the vendored
    // Lucene.Net sources, where Constants then meant two different types (24 CS0104).
    var owner = OwningProjectOf(candidate.OutputRelative, candidate.Included);
    foreach (var directive in CodeBehindRewriter.ParseUnit(candidate.Source).Usings)
    {
        if (directive.GlobalKeyword.RawKind
            != (int)Microsoft.CodeAnalysis.CSharp.SyntaxKind.GlobalKeyword)
        {
            continue;
        }

        if (!libraryGlobalUsings.TryGetValue(owner, out var list))
        {
            libraryGlobalUsings[owner] = list = [];
        }
        var text = UsingText(directive);
        if (!list.Contains(text, StringComparer.Ordinal))
        {
            list.Add(text);
        }
    }
}

if (libraryGlobalUsings.Count > 0)
{
    report.Info("(project)",
        $"ライブラリ {libraryGlobalUsings.Count} 件の global using を、"
        + "そのライブラリのファイル内に閉じ込めました(統合後の他プロジェクトへ漏れないようにするため)。");
}

for (var i = 0; i < candidateNamespaces.Count; i++)
{
    if (excludedCandidates.Contains(i))
    {
        continue;
    }
    var (candidate, _) = candidateNamespaces[i];
    var candidateSource = libraryGlobalUsings.TryGetValue(
            OwningProjectOf(candidate.OutputRelative, candidate.Included), out var scoped)
        ? ScopeGlobalUsings(candidate.Source, scoped)
        : candidate.Source;
    CollectUsingNamespaces(candidateSource, portedNamespaces);
    CollectQualifiedPackageNamespaces(candidateSource, portedNamespaces);
    unsafeCodePorted |= System.Text.RegularExpressions.Regex.IsMatch(
        candidateSource, @"(?<![\w.])unsafe(?![\w])");
    // The namespace may be written out in full - log4net, which DNN vendors, says
    // [assembly: System.Reflection.AssemblyCompany(...)] - so the prefix is optional.
    assemblyAttributesPorted |= System.Text.RegularExpressions.Regex.IsMatch(
        candidateSource,
        @"\[\s*assembly\s*:\s*(System\.Reflection\.)?Assembly(Version|FileVersion|Company|Product|Title|Configuration|Trademark|Culture|InformationalVersion)\s*\(");
    var destination = Path.Combine(output, candidate.OutputRelative.Replace('/', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
    // A file kept despite importing an emptied namespace still has the import, and the
    // namespace is not there any more - so the using itself has to go, exactly as
    // StripDeadUsings does for @using in .razor.
    File.WriteAllText(destination,
        ApplyLibraryMigrations(ApplyNamespaceMap(StripDeadCodeUsings(
            compatImports.Apply(
                CodeBehindRewriter.RewritePlainCodeFile(
                    candidateSource, candidate.ReportName, report, portedTypes)),
            fullyExcludedNamespaces, report, candidate.ReportName))));
    report.CopiedCodeFiles++;

    // BinaryFormatter still compiles (the generated project suppresses SYSLIB0011) but the
    // runtime removed it, so the call throws the first time it runs. Report it rather than
    // let a build that succeeds imply the code works.
    if (candidate.Source.Contains("BinaryFormatter", StringComparison.Ordinal))
    {
        report.Residual(candidate.ReportName, ResidualKind.CodeBehind,
            "BinaryFormatter は .NET から削除されています。ビルドは通りますが実行時に "
            + "PlatformNotSupportedException になります。別のシリアライザへの移行が必要です。",
            disposition: ResidualDisposition.Backlog);
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
    portedProjectGlobalUsings,
    out var stubbedTypeCount);

if (stubbedTypeCount > 0)
{
    File.WriteAllText(Path.Combine(output, "ExcludedTypeStubs.g.cs"), ApplyLibraryMigrations(ApplyNamespaceMap(excludedTypeStubs)));
    report.Residual("(project)", ResidualKind.CodeBehind,
        $"移植から除外したファイルが宣言していた型 {stubbedTypeCount} 個を空のスタブとして生成しました"
        + "(ExcludedTypeStubs.g.cs)。参照側はコンパイルできます(除外の理由は各ファイルの残差を参照)。", disposition: ResidualDisposition.Backlog);
}

// NOT passed through ApplyLibraryMigrations/ApplyNamespaceMap, unlike the stubs above.
// The whole point is to declare the OLD name: that is the name the ported code still
// spells, precisely because the migration found nothing to rewrite it to.
var migratedAwayStubs = GenerateMigratedAwayTypeStubs(migratedAwayTypes, out var migratedAwayCount);
if (migratedAwayCount > 0)
{
    File.WriteAllText(Path.Combine(output, "MigratedAwayTypeStubs.g.cs"), migratedAwayStubs);
    report.Info("(project)",
        $"置き換え先に同名が無い型 {migratedAwayCount} 個を空のスタブとして生成しました"
        + "(MigratedAwayTypeStubs.g.cs)。型名だけを再宣言するので、メンバーを使っている箇所は"
        + "ビルドエラーとして残ります — 移行先の判断はそこで必要です(型ごとの残差は上記)。");
}

// What the build gate needs in order to read the restored out-of-scope files correctly.
// See the restore pass: these were excluded as another framework's code and handed back
// only so the ported code naming their types keeps compiling. They carry that framework's
// unresolved types by construction.
if (restoredOutOfScopeFiles.Count > 0)
{
    File.WriteAllLines(
        Path.Combine(output, "restored-out-of-scope-files.txt"),
        restoredOutOfScopeFiles.Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase));
}

// Virtual path -> converted component type, so LoadControl can resolve at runtime.
//
// WebForms compiled a .ascx on demand; LoadControl("~/Custom/PostView.ascx") returned an
// instance. Here the .ascx is already a component, and the only thing missing was the map
// from the path the code-behind still names to the type it became. The converter is the
// one place that knows both, so it writes them out.
//
// Without this LoadControl returned null and the call site silently produced nothing -
// BlogEngine's post list builds every post that way, which is why its home page rendered
// no articles at all.
WriteUserControlCatalog(output, appName, userControlRegistry, project, report);

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
        File.WriteAllText(
            Path.Combine(stubDirectory, stub.Name + ".razor"),
            GenerateStubComponent(appName, stub.Name, stub.Tag));
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
        + "サテライトアセンブリ化(または IStringLocalizer への移行)が必要です。", disposition: ResidualDisposition.Backlog);
}

// Unsafe code. The original project must have allowed it or it would not have compiled,
// and the ported source is the evidence: "unsafe" is a keyword, so a declaration of it
// cannot be anything else. YAF.NET carries Lucene.Net, whose EncodingExtensions has an
// unsafe block, and without this the port stops at CS0227.
if (unsafeCodePorted)
{
    var csprojPath = Path.Combine(output, appName + ".csproj");
    File.WriteAllText(csprojPath, File.ReadAllText(csprojPath).Replace(
        "<Nullable>disable</Nullable>",
        "<Nullable>disable</Nullable>" + Environment.NewLine
        + "    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>"));
    report.Info("(project)", "移植コードに unsafe があるため AllowUnsafeBlocks を有効にしました。");
}

// The SDK writes AssemblyVersion / AssemblyCompany / AssemblyFileVersion into a generated
// AssemblyInfo.cs. A ported file that declares them too is CS0579 ("duplicate attribute"),
// and the application's own values are the ones to keep - DNN carries a shared assembly
// info file that every project of the solution included.
if (assemblyAttributesPorted)
{
    var csprojPath = Path.Combine(output, appName + ".csproj");
    File.WriteAllText(csprojPath, File.ReadAllText(csprojPath).Replace(
        "<Nullable>disable</Nullable>",
        "<Nullable>disable</Nullable>" + Environment.NewLine
        + "    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>"));
    report.Info("(project)",
        "移植コードがアセンブリ属性を宣言しているため GenerateAssemblyInfo を無効にしました"
        + "(SDK 生成分と重複するため)。");
}

// NuGet references: what the original projects declared (csproj PackageReference /
// packages.config) carries over first, then references implied by the ported code's
// usings (EF6, JSON.NET etc.). Framework-only web packages are skipped with a report.
if (packageMap is not null)
{
    report.Info("(project)", $"--package-map から {packageMap.Count} 件のパッケージ指定を読み込みました。");
}

var declared = CollectDeclaredPackages(
    [input, .. includeDirectories], report, packageMap,
    Path.Combine(output, "package-map.template.json"),
    foreignLanguageProjects);

// A package the code's usings imply is only needed when the original did not already
// bring that library in under another name. mojoPortal declares DotNetZip.Original and
// the Ionic.Zip import adds DotNetZip: two packages, one Ionic.Zip.ZipFile, and every use
// of it is CS0433 ("exists in both"). Deduplicating by Id alone does not see it, because
// the ids differ - a fork or a rename keeps the old id as a prefix.
var impliedPackages = ResolvePackageReferences(portedNamespaces)
    .Where(implied => !declared.Any(existing =>
        existing.Id.StartsWith(implied.Id, StringComparison.OrdinalIgnoreCase)))
    .ToList();

var packageReferences = declared
    .Concat(impliedPackages)
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
// Keyed on the MARKUP path. Deriving it from the code-behind path was the earlier
// approach and it missed every file using <script runat="server">, which has no separate
// code-behind at all - the AI layer then dropped those files as "not converted" when they
// had converted fine, just under a component name taken from the inherits base
// (CommentForm.ascx -> CommentFormBase.razor). The code-behind path is still used as a
// fallback for anything that somehow lacks the markup path.
var componentBySource = components
    .Select(component => (
        Key: component.MarkupSourcePath is not null
            ? project.RelativePath(component.MarkupSourcePath)
            : component.CodeBehindSourcePath is not null
                ? project.RelativePath(component.CodeBehindSourcePath)
                    .Replace(".aspx.cs", ".aspx", StringComparison.OrdinalIgnoreCase)
                    .Replace(".ascx.cs", ".ascx", StringComparison.OrdinalIgnoreCase)
                    .Replace(".master.cs", ".master", StringComparison.OrdinalIgnoreCase)
                : null,
        Component: component))
    .Where(entry => entry.Key is not null)
    .GroupBy(entry => entry.Key!, StringComparer.OrdinalIgnoreCase)
    .ToDictionary(group => group.Key, group => group.First().Component, StringComparer.OrdinalIgnoreCase);

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
/// <summary>
/// The property types an unportable placeholder may re-declare: the ones that resolve
/// with no import at all. See the loop below for why the list is this short.
/// </summary>
static bool IsBuiltInParameterType(string type)
    => type is "string" or "bool" or "int" or "long" or "short" or "byte" or "sbyte"
        or "uint" or "ulong" or "ushort" or "double" or "float" or "decimal" or "char"
        or "object"
        or "string?" or "bool?" or "int?" or "long?" or "short?" or "byte?" or "double?"
        or "float?" or "decimal?" or "char?"
        or "String" or "Boolean" or "Int32" or "Int64" or "Double" or "Decimal" or "Object";

static string GenerateUnportablePlaceholder(
    ConvertedComponent component,
    string unportableNamespace,
    IReadOnlyDictionary<string, string> properties)
{
    var builder = new System.Text.StringBuilder();
    foreach (var route in component.Routes ?? [])
    {
        builder.AppendLine($"@page \"{route}\"");
    }
    builder.AppendLine($"@namespace {component.TargetNamespace}");
    builder.AppendLine();

    // A PageTitle even here. The host page carries no static <title>, so a routable
    // component without one leaves the document titleless.
    if (component.Routes is { Count: > 0 })
    {
        builder.AppendLine($"<PageTitle>{component.ComponentName}</PageTitle>");
        builder.AppendLine();
    }

    builder.AppendLine("<div class=\"w2b-unported\" style=\"border:2px dashed #cc0000; padding:1em; margin:0.5em 0;\">");
    builder.AppendLine($"    <strong>{component.ComponentName}</strong>: このコンポーネントは .NET Framework 専用 API({unportableNamespace})に依存しているため自動変換できません。手動移行が必要です。");
    builder.AppendLine("</div>");
    builder.AppendLine();
    builder.AppendLine("@code {");
    builder.AppendLine("    [Parameter(CaptureUnmatchedValues = true)]");
    builder.AppendLine("    public Dictionary<string, object> AdditionalAttributes { get; set; }");
    builder.AppendLine();
    builder.AppendLine("    [Parameter] public RenderFragment ChildContent { get; set; }");

    // The properties the original code-behind declared. Nothing reads them - this
    // component renders its "needs hand-migration" panel and nothing else - but the host
    // still sets them, and without the declarations that host stops compiling for a
    // decision taken about a DIFFERENT file.
    //
    // Only types that resolve HERE. This file is a bare .razor with an @namespace and no
    // imports of its own, so a property typed with one of the application's own types
    // does not compile - which is how this moved YAF from 16 errors to 2 and a floor,
    // its DisplayPost placeholder asking for a PagedMessage nothing had imported.
    //
    // A built-in type is the one thing knowable without resolving anything, and it covers
    // what a host actually sets across a control boundary. Anything else is left out; the
    // caller still fails on it, but on the line that names the real type rather than on a
    // declaration this converter invented.
    foreach (var (name, type) in properties.OrderBy(pair => pair.Key, StringComparer.Ordinal))
    {
        if (name != "ChildContent" && IsBuiltInParameterType(type))
        {
            builder.AppendLine($"    [Parameter] public {type} {name} {{ get; set; }}");
        }
    }

    builder.AppendLine("}");
    return builder.ToString();
}

/// <summary>
/// Detects .NET Framework-only infrastructure namespaces (OWIN auth, bundling,
/// Web API/MVC, routing config) that cannot be ported as-is. Files using them are
/// excluded with a residual instead of breaking the whole build.
/// </summary>
/// <summary>The three members every type inherits from object, matched by signature.</summary>
static bool IsObjectMember(string name, int parameterCount)
    => (name is "ToString" or "GetHashCode" && parameterCount == 0)
       || (name == "Equals" && parameterCount == 1);

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
    return PortabilityRules.FindQualifiedFrameworkReference(source)
           ?? PortabilityRules.FindServiceHostBase(source);
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
/// <summary>
/// Records which type names disappear because a vendored DLL has no replacement chosen yet.
///
/// Those types come back as CS0246 in the ported code, and counting them as conversion
/// defects is wrong twice over: nothing can fix them but the user naming a package, and
/// their sheer number (Lucene.Net alone is ~86 in DNN Platform) hides the errors that ARE
/// the converter's. The build gate reads this file and reports them separately.
///
/// The names come from the assembly's own metadata rather than a guess at what a namespace
/// prefix implies - the DLL is right there in the input tree, and it is the only thing that
/// actually knows.
/// </summary>
static void WriteUndecidedDependencyTypes(
    IEnumerable<(string Assembly, string? DllPath)> undecided, string path)
{
    var lines = new SortedSet<string>(StringComparer.Ordinal);

    foreach (var (assembly, dllPath) in undecided)
    {
        if (dllPath is null || !File.Exists(dllPath))
        {
            continue;
        }

        WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.ReadPublicTypes(dllPath, (typeNamespace, typeName) =>
        {
            // The simple name is what a CS0246 reports ("DNNNode"), and the root namespace
            // segment is what a CS0234 reports ("'Lucene' does not exist in ...").
            lines.Add($"{assembly}\t{typeName}");
            if (typeNamespace.Length > 0)
            {
                lines.Add($"{assembly}\t{typeNamespace.Split('.')[0]}");
            }
        });
    }

    if (lines.Count == 0)
    {
        // Left behind from an earlier run it would be read as still current.
        File.Delete(path);
        return;
    }

    File.WriteAllLines(path, lines);
}

/// <summary>
/// What each undecided vendored DLL declares, as "namespace TAB name TAB shape".
///
/// Written for the build gate, which uses it to get PAST these names rather than to count
/// them. The gate already refuses to count them - the decision is the reader's, not the
/// converter's - but an uncounted error still stops the compiler, and the Razor SDK
/// compiles twice: an unresolved name fails the declaration pass, so the real compile never
/// runs and every other error in the project goes unseen. mojoPortal reported "6 build
/// errors" that way while the true figure behind two uncounted HtmlDiff errors was 2236.
/// Excluding an error from the count and letting it decide the whole measurement are not
/// consistent positions.
///
/// The stubs are NOT written here, and deliberately so. Writing them into the conversion
/// output declares a type inside the project, and a type declared in the project BEATS the
/// same name from a referenced package - which is how this was first tried and how
/// BlogEngine went from 0 errors to 23, its real SharpZipLib shadowed by a stub for the
/// vendored copy. The gate stubs only the names the compiler has actually reported as
/// missing, where by definition nothing is being shadowed.
/// </summary>
static void WriteUndecidedDependencyShapes(
    IEnumerable<(string Assembly, string? DllPath)> undecided, string path)
{
    var lines = new SortedSet<string>(StringComparer.Ordinal);

    foreach (var (_, dllPath) in undecided)
    {
        if (dllPath is null || !File.Exists(dllPath))
        {
            continue;
        }

        WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.ReadPublicTypeShapes(
            dllPath,
            (typeNamespace, typeName, shape) =>
            {
                if (shape == WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.TypeShape.Unsupported
                    || typeNamespace.Length == 0
                    // Metadata spells generic arity with a backtick, which is not an
                    // identifier. A stub written from one would be a SYNTAX error - the one
                    // failure that costs more than the problem this file solves, because it
                    // stops the compiler earlier still.
                    || typeName.IndexOfAny(['`', '<', '>']) >= 0)
                {
                    return;
                }

                lines.Add($"{typeNamespace}\t{typeName}\t{shape}");
            });
    }

    if (lines.Count == 0)
    {
        // Left behind from an earlier run it would be read as still current.
        File.Delete(path);
        return;
    }

    File.WriteAllLines(path, lines);
}

/// <summary>
/// Emits UserControlCatalog.g.cs: the virtual path of every converted .ascx mapped to the
/// component type it became, registered with the compatibility layer at startup.
///
/// Both spellings a code-behind uses are registered - "~/Custom/PostView.ascx" and
/// "/Custom/PostView.ascx" - because LoadControl is called with whichever the original
/// author wrote, and the lookup is not in a position to guess.
/// </summary>
static void WriteUserControlCatalog(
    string output,
    string appName,
    IReadOnlyDictionary<string, UserControlRef> registry,
    WebFormsProject project,
    ConversionReport report)
{
    if (registry.Count == 0)
    {
        return;
    }

    var entries = new List<string>();
    foreach (var (relative, reference) in registry.OrderBy(pair => pair.Key, StringComparer.Ordinal))
    {
        var normalized = relative.Replace('\\', '/');
        entries.Add(
            $"        Register(\"~/{normalized}\", typeof(global::{reference.Namespace}.{reference.ComponentName}));");
        entries.Add(
            $"        Register(\"/{normalized}\", typeof(global::{reference.Namespace}.{reference.ComponentName}));");
    }

    var source = $$"""
        // Generated by WebForm2Blazor.
        // Virtual path -> converted component, for Page.LoadControl / UserControl.LoadControl.
        //
        // WebForms compiled a .ascx on demand and handed back an instance. The .ascx is a
        // component here, so what LoadControl needs is only this map - which the converter
        // is the one place that knows.

        namespace {{appName}};

        internal static class UserControlCatalog
        {
            [global::System.Runtime.CompilerServices.ModuleInitializer]
            internal static void Register()
            {
        {{string.Join(Environment.NewLine, entries)}}
            }

            private static void Register(string virtualPath, global::System.Type componentType)
                => global::WebForm2Blazor.Components.UserControlCatalog.Register(virtualPath, componentType);
        }
        """;

    File.WriteAllText(Path.Combine(output, "UserControlCatalog.g.cs"), source);
    report.Info("(project)",
        $"ユーザーコントロール {registry.Count} 件の仮想パスと変換後コンポーネントの対応表を生成しました"
        + "(UserControlCatalog.g.cs)。LoadControl(\"~/…​.ascx\") が実体を返します。");
    _ = project;
}

/// <summary>
/// Reads --package-map. One old assembly may appear on SEVERAL entries: a replacement is
/// free to split one Framework assembly across several packages (Lucene.Net 3.x had its
/// query parsers in Lucene.Net.dll; 4.8 has them in Lucene.Net.QueryParser), and the answer
/// to "what replaces Lucene.Net" is then more than one package. Entries accumulate instead
/// of overwriting, so writing the split down is enough to express it.
/// </summary>
static Dictionary<string, List<(string? Package, string? Version)>> LoadPackageMap(string path)
{
    var map = new Dictionary<string, List<(string?, string?)>>(StringComparer.OrdinalIgnoreCase);
    using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(path));
    foreach (var entry in document.RootElement.EnumerateArray())
    {
        if (!entry.TryGetProperty("assembly", out var assembly)
            || assembly.GetString() is not { Length: > 0 } assemblyName)
        {
            continue;
        }

        if (!map.TryGetValue(assemblyName, out var choices))
        {
            map[assemblyName] = choices = new List<(string?, string?)>();
        }
        choices.Add((
            entry.TryGetProperty("package", out var package) ? package.GetString() : null,
            entry.TryGetProperty("version", out var version) ? version.GetString() : null));

        // Optional: a path to the replacement's own assembly, or several. Naming it turns
        // the entry from "add this PackageReference" into "move the code onto this library"
        // - the converter reads both sides and rewrites the names that match. See
        // AssemblyTypeMigration.
        if (entry.TryGetProperty("dll", out var dll))
        {
            var paths = dll.ValueKind == System.Text.Json.JsonValueKind.Array
                ? dll.EnumerateArray().Select(element => element.GetString())
                : new[] { dll.GetString() };

            foreach (var dllPath in paths.OfType<string>().Where(value => value.Length > 0))
            {
                if (!WebForm2Blazor.Converter.Convert.AssemblyTypeMigration.ReplacementAssemblies
                        .TryGetValue(assemblyName, out var replacements))
                {
                    WebForm2Blazor.Converter.Convert.AssemblyTypeMigration
                        .ReplacementAssemblies[assemblyName] = replacements = new List<string>();
                }
                replacements.Add(dllPath);
            }
        }
    }
    return map;
}

/// <summary>
/// The built assembly of a project the converter cannot port, so its TYPE NAMES can be
/// read from metadata.
///
/// Its own bin\ is checked first, then the directory above it. A repository does not
/// always build in place: DNN keeps DotNetNuke.WebUtility's sources in
/// "DNN Platform\DotNetNuke.WebUtility" and its output in
/// "DNN Platform\Controls\DotNetNuke.WebUtility\bin", so looking only beside the project
/// finds nothing. Any copy will do - what is wanted is the list of types that go missing,
/// and every build of the assembly declares the same ones.
/// </summary>
static string? FindBuiltAssembly(string? projectDirectory, string assembly)
{
    if (projectDirectory is null)
    {
        return null;
    }

    var fileName = assembly + ".dll";
    var beside = new[] { "bin", Path.Combine("bin", "Release"), Path.Combine("bin", "Debug") }
        .Select(folder => Path.Combine(projectDirectory, folder, fileName))
        .FirstOrDefault(File.Exists);
    if (beside is not null)
    {
        return beside;
    }

    try
    {
        return Directory.GetParent(projectDirectory) is { } parent
            ? Directory.EnumerateFiles(parent.FullName, fileName, SearchOption.AllDirectories)
                .FirstOrDefault()
            : null;
    }
    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
    {
        return null;
    }
}

/// <summary>
/// Package IDs the target framework supersedes, read from the SDK's own data.
///
/// This was a hand-written list of 20 names, and a name missing from it meant the package
/// reference was carried into the generated project - where an old out-of-band assembly
/// can win binding over the in-box one. Nothing reports that; it shows up as behaviour.
///
/// The artifact that knows is <c>PackageOverrides.txt</c>, shipped inside each targeting
/// pack. It is the file NuGet itself reads to decide a PackageReference is redundant
/// (NU1510) - the authoritative answer, versioned with the SDK in use. For .NET 10 the two
/// framework references a converted Blazor app has list 412 IDs between them. The hand
/// list named 17 of those and missed 395, among them Microsoft.Win32.Registry,
/// System.Reflection.Emit and the whole runtime.* family that WebForms-era projects carry.
///
/// Only the two packs a web project actually references are read. Microsoft.WindowsDesktop
/// .App.Ref supersedes System.Drawing.Common and System.Windows.Extensions, which a Blazor
/// app does NOT get from its framework reference - reading it would drop packages the
/// ported code needs.
/// </summary>
static HashSet<string> InBoxPackageIds()
{
    // Facades that are no-ops on modern .NET but are NOT in PackageOverrides, because the
    // framework does not supersede the package - it type-forwards the types out of it.
    var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Microsoft.Bcl.AsyncInterfaces", "Microsoft.Bcl.HashCode", "Microsoft.Bcl.TimeProvider",
    };

    string[] webProjectPacks = ["Microsoft.NETCore.App.Ref", "Microsoft.AspNetCore.App.Ref"];
    var packsRoot = DotNetPacksRoot();

    if (packsRoot is not null)
    {
        foreach (var pack in webProjectPacks)
        {
            var packDirectory = Path.Combine(packsRoot, pack);
            if (!Directory.Exists(packDirectory))
            {
                continue;
            }
            // Every installed version is unioned. These lists only grow, and an ID listed
            // by any of them is a Framework-era split package either way.
            foreach (var file in Directory.EnumerateFiles(
                packDirectory, "PackageOverrides.txt", SearchOption.AllDirectories))
            {
                foreach (var line in File.ReadLines(file))
                {
                    // "Microsoft.CSharp|4.7.0" - identity before the version
                    var id = line.Split('|')[0].Trim();
                    if (id.Length > 0)
                    {
                        ids.Add(id);
                    }
                }
            }
        }
    }

    // Reading nothing would carry EVERY split package over - the exact hazard this guards
    // against, arrived at silently. If the packs cannot be found, fall back to the names
    // the corpora proved matter rather than to an empty set.
    if (ids.Count > 3)
    {
        return ids;
    }

    ids.UnionWith([
        "Microsoft.CSharp",
        "System.Buffers", "System.Collections.Immutable", "System.ComponentModel.Annotations",
        "System.Diagnostics.DiagnosticSource", "System.IO.Pipelines", "System.Memory",
        "System.Net.Http", "System.Numerics.Vectors", "System.Runtime.CompilerServices.Unsafe",
        "System.Security.AccessControl", "System.Security.Principal.Windows",
        "System.Text.Encoding.CodePages", "System.Text.Encodings.Web", "System.Text.Json",
        "System.Threading.Tasks.Extensions", "System.ValueTuple",
    ]);
    return ids;
}

/// <summary>
/// The "packs" directory of the .NET install this converter is running on, derived from the
/// runtime directory (.../shared/Microsoft.NETCore.App/&lt;version&gt;) rather than guessed
/// from Program Files, so a side-by-side or non-default install resolves correctly.
/// </summary>
static string? DotNetPacksRoot()
{
    var runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
    if (string.IsNullOrEmpty(runtimeDirectory))
    {
        return null;
    }

    var dotnetRoot = Directory.GetParent(runtimeDirectory.TrimEnd(
        Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))?.Parent?.Parent?.FullName;

    if (dotnetRoot is null)
    {
        return null;
    }

    var packs = Path.Combine(dotnetRoot, "packs");
    return Directory.Exists(packs) ? packs : null;
}

static List<(string Id, string Version)> CollectDeclaredPackages(
    IEnumerable<string> projectDirectories,
    ConversionReport report,
    Dictionary<string, List<(string? Package, string? Version)>>? packageMap = null,
    string? templatePath = null,
    IEnumerable<string>? foreignLanguageProjects = null)
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
    var inBoxOnModernDotNet = InBoxPackageIds();

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
    // The DLL path is kept, not just the name: the assembly is the only place that says
    // which TYPES go missing when it is dropped, and the build gate needs that to tell
    // "the user has not chosen a package yet" apart from a defect in the conversion.
    var binaryReferences = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

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

    // A ProjectReference to a .vbproj / .fsproj is a dependency this converter cannot port
    // and has no NuGet identity either, so its types came back as plain CS0246 and counted
    // against the conversion. DNN Platform references DotNetNuke.WebUtility.vbproj, which
    // declares DotNetNuke.UI.Utilities - IClientAPICallbackEventHandler and DataCache -
    // and 16 errors in Default.aspx.cs and InstallWizard.aspx.cs were nothing but that.
    //
    // It is exactly the vendored-DLL situation: someone has to supply a .NET build, and
    // until they do the converter can neither fix it nor take credit for it. Registering
    // the project's BUILT assembly (its own metadata lists the types, same as a vendored
    // DLL) puts it in the package-map template and out of the counted errors.
    foreach (var foreignProject in foreignLanguageProjects ?? [])
    {
        var assembly = Path.GetFileNameWithoutExtension(foreignProject);
        if (string.IsNullOrEmpty(assembly) || binaryReferences.ContainsKey(assembly))
        {
            continue;
        }

        binaryReferences[assembly] = FindBuiltAssembly(Path.GetDirectoryName(foreignProject), assembly);
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
                    if (string.IsNullOrEmpty(assembly))
                    {
                        continue;
                    }

                    // HintPath is relative to the project file.
                    var dllPath = Path.GetFullPath(Path.Combine(
                        Path.GetDirectoryName(csprojPath) ?? directory,
                        hintPath.Replace('\\', Path.DirectorySeparatorChar)));
                    if (!binaryReferences.TryGetValue(assembly, out var known) || known is null)
                    {
                        binaryReferences[assembly] = File.Exists(dllPath) ? dllPath : null;
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
            ".NET へそのまま持ち込めない NuGet 参照をスキップしました(認証/バンドルは別フレームワーク側の基盤): "
            + string.Join(", ", skipped.Distinct(StringComparer.OrdinalIgnoreCase)), disposition: ResidualDisposition.OutOfScope);
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
    foreach (var assembly in binaryReferences.Keys)
    {
        if (packageMap is null || !packageMap.TryGetValue(assembly, out var choices))
        {
            continue;
        }

        foreach (var choice in choices)
        {
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

    var undecided = binaryReferences.Keys
        .Where(assembly => packageMap is null || !packageMap.ContainsKey(assembly))
        .Where(assembly => !carried.Any(package =>
            package.Id.Equals(assembly, StringComparison.OrdinalIgnoreCase)))
        .OrderBy(assembly => assembly, StringComparer.OrdinalIgnoreCase)
        .ToList();

    // What the build gate needs in order to not count these against the conversion, and -
    // from the shapes file - in order to get past them and see what they were hiding.
    if (templatePath is not null)
    {
        var outputDirectory = Path.GetDirectoryName(templatePath)!;
        var undecidedWithPaths = undecided
            .Select(assembly => (assembly, binaryReferences[assembly]))
            .ToList();

        WriteUndecidedDependencyTypes(
            undecidedWithPaths,
            Path.Combine(outputDirectory, "unresolved-dependency-types.txt"));

        WriteUndecidedDependencyShapes(
            undecidedWithPaths,
            Path.Combine(outputDirectory, "unresolved-dependency-shapes.txt"));
    }

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
            disposition: ResidualDisposition.NeedsInput);
    }

    return carried.DistinctBy(package => package.Id, StringComparer.OrdinalIgnoreCase).ToList();
}

/// <summary>Well-known WebForms-era libraries that have .NET-compatible NuGet packages.</summary>
static List<(string Id, string Version)> ResolvePackageReferences(HashSet<string> namespaces)
    => KnownPackages()
        .Where(package => namespaces.Any(ns =>
            ns.Equals(package.Prefix, StringComparison.Ordinal)
            || ns.StartsWith(package.Prefix + ".", StringComparison.Ordinal)))
        .Select(package => (package.Id, package.Version))
        .DistinctBy(package => package.Id)
        .ToList();

/// <summary>
/// Records the packages a file needs because it writes the namespace out in full.
///
/// Reading the using list alone misses them, and a fully qualified reference is the normal
/// way to name a type used once: ServiceStack.OrmLite's ProfiledProviderFactory writes
/// System.Security.Permissions.PermissionState in a signature and imports nothing.
/// Only the known prefixes are looked for, so the set of packages stays closed.
/// </summary>
static void CollectQualifiedPackageNamespaces(string source, HashSet<string> namespaces)
{
    foreach (var package in KnownPackages())
    {
        if (source.Contains(package.Prefix + ".", StringComparison.Ordinal))
        {
            namespaces.Add(package.Prefix);
        }
    }
}

static (string Prefix, string Id, string Version)[] KnownPackages()
{
    // Microsoft ships these alongside the runtime, so their version must track the target
    // framework. Pinning an older major downgrades what a carried-over package already
    // depends on (NU1605) and the restore fails.
    const string RuntimeLibraryVersion = "10.0.*";

    return
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
        // SharpZipLib. Referenced on 4.8 as a checked-in DLL, which leaves no NuGet trace
        // to carry over. The modern package is the SAME library - it has shipped
        // netstandard2.0 since 1.0 - so the ported code resolves against it.
        ("ICSharpCode.SharpZipLib", "SharpZipLib", "1.4.2"),
        // Libraries whose .NET build keeps the SAME namespace and API, so the ported code
        // binds to it unchanged. Only these are mapped automatically; a library whose .NET
        // successor renamed its namespace or changed its API is a migration decision and
        // stays in package-map.template.json for the user to answer (Lucene.Net 3 -> 4.8,
        // MetaDataExtractor -> MetadataExtractor, ClientDependency -> Smidge, AppFabric ->
        // Redis). See corpora/README.md for the survey behind this list.
        //
        // Novell's LDAP client: the .NET Standard fork keeps namespace
        // Novell.Directory.Ldap and targets .NET 6/8/9.
        ("Novell.Directory.Ldap", "Novell.Directory.Ldap.NETStandard", "4.0.0"),
        // ZedGraph's charting core still ships, targeting .NET 6 (so .NET 10 resolves it),
        // with namespace ZedGraph unchanged. Its ASP.NET WebForms half - the separate
        // ZedGraph.Web package - stopped at .NET Framework in 2011 and is NOT mapped: a
        // WebForms chart control has no .NET build to bind to.
        ("ZedGraph", "ZedGraph", "5.2.1"),
        ("System.ServiceModel.Syndication", "System.ServiceModel.Syndication", RuntimeLibraryVersion),
        ("System.DirectoryServices", "System.DirectoryServices", RuntimeLibraryVersion),
        // MEF. Referenced on 4.8 as a GAC assembly ("<Reference Include=" with no HintPath),
        // which leaves no NuGet trace to carry over, so ImportMany and friends came out as
        // CS0246. The package has the same API as the Framework assembly.
        ("System.ComponentModel.Composition", "System.ComponentModel.Composition", RuntimeLibraryVersion),
        ("System.Runtime.Caching", "System.Runtime.Caching", RuntimeLibraryVersion),
        ("System.Management", "System.Management", RuntimeLibraryVersion),
        ("System.Security.Cryptography.Xml", "System.Security.Cryptography.Xml", RuntimeLibraryVersion),
        // CodeAccessPermission and PermissionState live here on .NET. They are still in the
        // framework index because the shim assembly forwards them, which is why the failure
        // reads CS1069 ("forwarded to System.Security.Permissions, consider adding a
        // reference") rather than "type not found".
        ("System.Security.Permissions", "System.Security.Permissions", RuntimeLibraryVersion),
    ];
}

/// <summary>
/// Drops @using lines naming a namespace that no ported file declares any more.
/// The import is emitted from the source file's own usings, which are checked against
/// the SOURCE tree; the porting exclusion cascade can then remove every file of that
/// namespace, and Razor fails to compile on the now-dead import.
/// </summary>
/// <summary>
/// The .cs counterpart of <see cref="StripDeadUsings"/>: removes "using X;" for a namespace
/// that no longer has any type in it. A file only reaches here when it names nothing from
/// that namespace, so removing the import cannot break anything it does use.
/// </summary>
static string StripDeadCodeUsings(
    string code, HashSet<string> deadNamespaces, ConversionReport report, string reportName)
{
    if (deadNamespaces.Count == 0)
    {
        return code;
    }

    var lines = code.Split('\n');
    var kept = new List<string>(lines.Length);

    foreach (var line in lines)
    {
        var match = System.Text.RegularExpressions.Regex.Match(
            line, @"^\s*(global\s+)?using\s+([A-Za-z_][\w.]*)\s*;\s*$");
        if (match.Success && deadNamespaces.Contains(match.Groups[2].Value))
        {
            report.Info(reportName,
                $"using {match.Groups[2].Value}; は移植後に型が残らないため除去しました"
                + "(このファイルはその名前空間の型を使っていません)。");
            continue;
        }
        kept.Add(line);
    }

    return string.Join("\n", kept);
}

/// <summary>
/// Gives the .razor the web project's GLOBAL usings, which the markup had and the
/// generated component did not.
///
/// A markup expression resolves through the imports the page was compiled with. The
/// code-behind half already gets these (see webProjectGlobalUsings); the markup half was
/// left with only the imports written in the .ascx and in the code-behind FILE, and a
/// global using is in neither. YAF writes
/// "&lt;%# ... WebApiConfig.UrlPrefix %&gt;" in two of its controls and that name comes
/// from a global using, so the markup could not compile - and the error lands on the
/// generated Razor, where nothing is wrong.
///
/// The lines go in as the ORIGINAL namespace names, so the namespace map rewrites them and
/// StripDeadUsings drops the ones whose types did not survive - the same two passes every
/// other import in the file goes through.
/// </summary>
static string WithGlobalUsings(
    string razorContent, IReadOnlyList<string> globalUsings, string ownNamespace)
{
    if (globalUsings.Count == 0)
    {
        return razorContent;
    }

    var lines = razorContent.Replace("\r\n", "\n").Split('\n').ToList();

    var present = new HashSet<string>(
        lines.Where(line => line.StartsWith("@using ", StringComparison.Ordinal))
            .Select(line => line["@using ".Length..].Trim()),
        StringComparer.Ordinal);

    var additions = globalUsings
        .Where(ns => ns != ownNamespace && !present.Contains(ns))
        .Distinct(StringComparer.Ordinal)
        .OrderBy(ns => ns, StringComparer.Ordinal)
        .Select(ns => "@using " + ns)
        .ToList();

    if (additions.Count == 0)
    {
        return razorContent;
    }

    // After the last directive at the top of the file. A @using has to precede the markup,
    // and @namespace / @inherits / @layout have to stay where they are.
    var insertAt = 0;
    for (var index = 0; index < lines.Count; index++)
    {
        if (lines[index].StartsWith('@') && !lines[index].StartsWith("@using ", StringComparison.Ordinal)
            && !lines[index].StartsWith("@page", StringComparison.Ordinal)
            && !lines[index].StartsWith("@namespace", StringComparison.Ordinal)
            && !lines[index].StartsWith("@inherits", StringComparison.Ordinal)
            && !lines[index].StartsWith("@layout", StringComparison.Ordinal)
            && !lines[index].StartsWith("@implements", StringComparison.Ordinal)
            && !lines[index].StartsWith("@attribute", StringComparison.Ordinal))
        {
            break;
        }
        if (lines[index].StartsWith('@') || lines[index].Trim().Length == 0)
        {
            insertAt = index + 1;
            continue;
        }
        break;
    }

    lines.InsertRange(insertAt, additions);
    return string.Join("\r\n", lines);
}

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
/// Empty declarations for types the replacement library has no name for.
///
/// The shape (class / interface / enum / struct, and whether it was abstract) is read out
/// of the ORIGINAL assembly, never inferred: an interface re-declared as a class breaks
/// every implementer, and a type the original made abstract must not become newable here.
/// A type whose shape the old assembly does not state - a delegate, or a name it merely
/// forwarded - gets no stub, because the only alternative is to invent one.
/// </summary>
static string GenerateMigratedAwayTypeStubs(
    IReadOnlyList<(string FullName, WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.TypeShape Shape)> types,
    out int typeCount)
{
    using var writer = new StringWriter();
    writer.WriteLine("// <auto-generated />");
    writer.WriteLine("// 置き換え先のライブラリに同名の型が無く、移植コードがまだその名前を書いている型です。");
    writer.WriteLine("// 名前だけを再宣言します。メンバーは意図的に空で、使っている箇所はビルドエラーになります。");
    writer.WriteLine("// 移行先の判断はそこで必要です(CONVERSION-REPORT.md の該当残差を参照)。");
    writer.WriteLine();

    typeCount = 0;
    var seen = new HashSet<string>(StringComparer.Ordinal);

    foreach (var group in types
        .Where(type => type.Shape != WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.TypeShape.Unsupported)
        // A generic arity survives in metadata as a backtick ("Foo`1"), which is not a C#
        // identifier. Emitting one would be a SYNTAX error - the single failure mode that
        // would cost more than the problem this file solves, because it stops the compiler
        // before declarations and hides everything, which is exactly what we are undoing.
        .Where(type => type.FullName.IndexOfAny(['`', '<', '>']) < 0)
        .Where(type => type.FullName.LastIndexOf('.') > 0)
        .Where(type => seen.Add(type.FullName))
        .GroupBy(type => type.FullName[..type.FullName.LastIndexOf('.')], StringComparer.Ordinal)
        .OrderBy(group => group.Key, StringComparer.Ordinal))
    {
        writer.WriteLine($"namespace {group.Key}");
        writer.WriteLine("{");
        foreach (var (fullName, shape) in group.OrderBy(type => type.FullName, StringComparer.Ordinal))
        {
            var name = fullName[(fullName.LastIndexOf('.') + 1)..];
            var keyword = shape switch
            {
                WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.TypeShape.Interface => "interface",
                WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.TypeShape.Enum => "enum",
                WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.TypeShape.Struct => "struct",
                WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.TypeShape.AbstractClass => "abstract class",
                _ => "class",
            };
            writer.WriteLine($"    public {keyword} {name}");
            writer.WriteLine("    {");
            writer.WriteLine("    }");
            typeCount++;
        }
        writer.WriteLine("}");
        writer.WriteLine();
    }

    return writer.ToString();
}

/// <summary>
/// Empty declarations for the top-level types that only excluded files declared.
/// Enum members are carried over (they are plain identifiers with no dependencies of
/// their own); everything else is deliberately left empty, so a consumer that really
/// used the type fails at the member rather than silently getting a working-looking stub.
/// </summary>
static string GenerateExcludedTypeStubs(
    IReadOnlyList<string> excludedSources,
    IReadOnlyList<string> survivingSources,
    IReadOnlyList<string> globalUsings,
    out int typeCount)
{
    typeCount = 0;

    var surviving = new HashSet<string>(StringComparer.Ordinal);
    var survivingNames = new HashSet<string>(StringComparer.Ordinal);

    // Simple name -> namespace-qualified name, for inheritable classes only. A stub is
    // rendered with almost no usings and its base often lives in another namespace, so a
    // base class has to be written out fully qualified. A name that is not unique maps to
    // null and is then dropped rather than guessed.
    var classesBySimpleName = new Dictionary<string, string?>(StringComparer.Ordinal);

    // Qualified names of the ported classes that are abstract. A stub deriving from one
    // is emitted abstract too, so the abstract members it cannot implement are simply
    // passed down to the real subclasses - which do implement them, since they compiled
    // in the original. That is what makes carrying an abstract base safe.
    var abstractClasses = new HashSet<string>(StringComparer.Ordinal);

    // Qualified name -> the simple name of its base class, for the ported classes. Used to
    // walk the chain and find the attribute classes: an attribute can never be abstract
    // ("cannot apply attribute class X because it is abstract"), so those keep their base
    // and stay concrete. Read off the base lists rather than guessed from the "Attribute"
    // suffix, which is a convention and not the rule the compiler applies.
    var baseNameByQualifiedName = new Dictionary<string, string>(StringComparer.Ordinal);
    void RecordInheritableClass(StubType declaration)
    {
        if (declaration.Declaration is not Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax classDeclaration)
        {
            return;
        }
        // sealed/static cannot be derived from at all.
        //
        // abstract WAS excluded, because a concrete stub inherits abstract members it has
        // no way to implement and CS0534 replaces the CS0115 we were removing - measured
        // at DNN -14 / YAF.NET +138. The answer is not to drop the base but to stop making
        // the stub concrete: see abstractClasses below.
        var unusable = classDeclaration.Modifiers.Any(modifier =>
            Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.SealedKeyword)
            || Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.StaticKeyword));
        if (unusable)
        {
            return;
        }

        var qualified = QualifiedName(declaration.Namespace, declaration.Name);

        if (classDeclaration.BaseList?.Types.Count > 0)
        {
            var firstBase = classDeclaration.BaseList.Types[0].Type switch
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax identifier
                    => identifier.Identifier.Text,
                Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax qualifiedBase
                    => qualifiedBase.Right.Identifier.Text,
                _ => null,
            };
            if (firstBase is not null)
            {
                baseNameByQualifiedName[qualified] = firstBase;
            }
        }

        if (classDeclaration.Modifiers.Any(modifier =>
                Microsoft.CodeAnalysis.CSharpExtensions.IsKind(
                    modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.AbstractKeyword)))
        {
            abstractClasses.Add(qualified);
        }

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

    // An attribute class cannot be abstract, so anything whose base chain reaches
    // System.Attribute is taken back out of the abstract set - it keeps its base and stays
    // concrete, which is what [DisplayableImage] on N2's ContentItem needs.
    foreach (var qualified in abstractClasses.ToList())
    {
        var current = qualified;
        for (var depth = 0; depth < 16; depth++)
        {
            if (!baseNameByQualifiedName.TryGetValue(current, out var baseName))
            {
                break;
            }
            if (baseName is "Attribute" or "System.Attribute")
            {
                abstractClasses.Remove(qualified);
                break;
            }
            if (!classesBySimpleName.TryGetValue(baseName, out var next) || next is null)
            {
                break;
            }
            current = next;
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
            builder.AppendLine(RenderStubType(
                stub, known, classesBySimpleName, abstractClasses, indent, declaredNamespace,
                globalUsings));
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
    IReadOnlySet<string> abstractClasses,
    string indent,
    string declaredNamespace,
    IReadOnlyList<string> globalUsings)
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
    var baseIsAbstract = false;
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
            else if (baseName is "Attribute" or "System.Attribute")
            {
                // An attribute class that loses its base is no longer an attribute, and the
                // error lands on every USE of it ("StringFormatMethodAttribute is not an
                // attribute class") rather than on the stub. This is the language's own
                // rule, not a list: [X] requires X : Attribute.
                //
                // Only Attribute is carried over from outside the port. A framework base in
                // general may declare abstract members the stub cannot implement, which
                // trades CS0115 for CS0534 - see RecordInheritableClass for the measurement.
                resolvedBase = "global::System.Attribute";
            }

            if (resolvedBase is not null)
            {
                baseClause = $" : {resolvedBase}";

                // An abstract base declares members this stub has no implementation for -
                // that is exactly why the base used to be dropped. Marking the stub
                // abstract passes the obligation down to the real subclasses, which do
                // implement them (the original compiled), instead of failing here.
                if (abstractClasses.Contains(resolvedBase))
                {
                    baseIsAbstract = true;
                }
                break;
            }
        }
    }

    if (baseIsAbstract)
    {
        modifiers = "public abstract";
    }

    var lines = new List<string>();
    if (typeDeclaration is not null)
    {
        var lookupNamespaces = LookupNamespacesFor(typeDeclaration, declaredNamespace, globalUsings);
        foreach (var member in typeDeclaration.Members)
        {
            var text = RenderStubMember(
                member, known, isStatic, isInterface, lookupNamespaces, stub.Name,
                containerIsSealed: isSealed, containerHasBase: baseClause.Length > 0);
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
    IReadOnlyList<string> lookupNamespaces,
    string containerName,
    bool containerIsSealed = false,
    bool containerHasBase = false)
{
    bool Has(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind kind)
        => modifiers.Any(modifier => Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, kind));

    // "protected" belongs in a stub even though nothing outside can call it: the members a
    // subclass OVERRIDES are usually protected, and the subclasses are being ported. Left
    // out, DNN's EditControl stub loses RenderEditMode / RenderViewMode / StringValue and
    // every control deriving from it fails on CS0115 - the single biggest cluster of them.
    //
    // An INTERFACE member carries no modifier at all and is public by definition. Reading
    // "no public keyword" as "not public" emptied every stubbed interface: YAF's
    // IAspNetRoleManager became "public interface IAspNetRoleManager { }" and its four
    // members vanished, so AspNetRolesHelper - which is ported and calls all four - could
    // not compile. The error then names the caller, which is the one file that was right.
    bool IsPublic(Microsoft.CodeAnalysis.SyntaxTokenList modifiers)
        => containerIsInterface
           || Has(modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword)
           || Has(modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind.InternalKeyword)
           || Has(modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind.ProtectedKeyword);

    string Prefix(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, string memberName = "", int parameterCount = -1)
    {
        if (containerIsInterface)
        {
            return string.Empty;
        }
        var isStatic = containerIsStatic
                       || modifiers.Any(modifier => Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.StaticKeyword));
        // A protected member stays protected: making it public would change the type's
        // surface, and the only reason it is here is so a subclass can override it.
        //
        // An internal one is widened to public, which is safe on its own - except on an
        // override, where the accessibility has to match the base exactly or it is CS0507
        // ("cannot change access modifiers when overriding"). BlogEngine's
        // DbFileSystemProvider.GetFileContents is internal in its base.
        var access = !Has(modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword)
                     && Has(modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind.ProtectedKeyword)
            ? "protected "
            : !Has(modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword)
              && Has(modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind.InternalKeyword)
              && Has(modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind.OverrideKeyword)
                ? "internal "
                : "public ";

        if (isStatic)
        {
            return access + "static ";
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
        var wasOverride = Has(modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind.OverrideKeyword);
        var wasOverridable = !containerIsSealed
            && (wasOverride
                || Has(modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind.VirtualKeyword)
                || Has(modifiers, Microsoft.CodeAnalysis.CSharp.SyntaxKind.AbstractKeyword));

        // An override stays an override only when the stub kept the base that declares the
        // member; without one there is nothing to override and it becomes virtual instead.
        // Getting this wrong the other way is what an abstract base makes visible: the base
        // declares the member abstract, so a virtual re-declaration leaves it unimplemented.
        if (wasOverride && containerHasBase && !containerIsSealed)
        {
            return access + "override ";
        }

        // object's own members always have a base, whether or not the stub kept one, and a
        // record declares ToString itself - so re-declaring it virtual is CS8869 ("does not
        // override an expected method from object"). mojoPortal's Author / Content / Date /
        // Title records are stubbed and each overrides ToString.
        //
        // By SIGNATURE, not by name: Equals(string, string) is IEqualityComparer<string>,
        // not object.Equals, and forcing an override onto it is CS0115 with nothing to
        // bind to. mojoPortal's UserProfileKeyComparer is one.
        if (wasOverride && !containerIsSealed && IsObjectMember(memberName, parameterCount))
        {
            return access + "override ";
        }

        return wasOverridable ? access + "virtual " : access;
    }

    switch (member)
    {
        case Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax method:
        {
            if (!IsPublic(method.Modifiers) || method.TypeParameterList is not null)
            {
                return null;
            }
            var returnType = ResolveType(method.ReturnType, known, lookupNamespaces);
            if (returnType is null)
            {
                return null;
            }
            var parameters = new List<string>();
            foreach (var parameter in method.ParameterList.Parameters)
            {
                // ref / out / in / params are part of the signature, so dropping the member
                // over them makes an abstract base impossible to satisfy - DNN's
                // MembershipProvider declares eight abstract members taking "ref UserInfo".
                // "this" is not reproduced: an extension method needs a static container
                // this stub may not have.
                var passing = string.Concat(parameter.Modifiers.Select(modifier => modifier.Text + " "));
                var parameterType = parameter.Modifiers.Any(modifier =>
                        Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.ThisKeyword))
                    ? null
                    : ResolveType(parameter.Type, known, lookupNamespaces);
                if (parameterType is null)
                {
                    return null;
                }
                // An OPTIONAL parameter stays optional. The stub exists so that calls to
                // the excluded member still compile, and a call is written against the
                // signature the author saw: YAF's SelectForumsLoadJs has six parameters,
                // the last two optional, and every one of its six call sites passes five.
                // Dropping "= null" turned each of them into CS7036 - an error about the
                // caller, produced by the stub.
                //
                // Only a default that is a LITERAL is carried. Anything else can name a
                // type that did not port, which would put an unresolvable expression in a
                // file whose whole purpose is to compile; "= default" keeps the parameter
                // optional and is honest about the value being gone.
                var defaultValue = parameter.Default is null
                    ? string.Empty
                    : parameter.Default.Value is Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax
                      or Microsoft.CodeAnalysis.CSharp.Syntax.DefaultExpressionSyntax
                        ? " = " + parameter.Default.Value.ToString()
                        : " = default";

                parameters.Add($"{passing}{parameterType} {parameter.Identifier.Text}{defaultValue}");
            }
            var message = $"{method.Identifier.Text} は変換対象外です(元の実装は移植されていません)。";
            return $"{Prefix(method.Modifiers, method.Identifier.Text, method.ParameterList.Parameters.Count)}{returnType} {method.Identifier.Text}({string.Join(", ", parameters)})"
                   + $" => throw new global::System.NotSupportedException(\"{message}\");";
        }

        case Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax property:
        {
            if (!IsPublic(property.Modifiers))
            {
                return null;
            }
            var propertyType = ResolveType(property.Type, known, lookupNamespaces);
            if (propertyType is null)
            {
                return null;
            }
            // An auto-property rather than a throwing accessor: properties read as data,
            // and a control-tree walk that touches one should not bring the page down.
            //
            // The ACCESSORS are copied, not assumed to be both. A read-only property in the
            // source stayed "{ get; set; }" here, and an override of one is CS0546 ("no
            // overridable set accessor") - which only shows up once the stub keeps its base,
            // so it was invisible while bases were being dropped.
            var hasGetter = property.AccessorList is null // "=> expression" is a getter
                            || property.AccessorList.Accessors.Any(accessor =>
                                Microsoft.CodeAnalysis.CSharpExtensions.IsKind(accessor, Microsoft.CodeAnalysis.CSharp.SyntaxKind.GetAccessorDeclaration));
            var hasSetter = property.AccessorList is not null
                            && property.AccessorList.Accessors.Any(accessor =>
                                Microsoft.CodeAnalysis.CSharpExtensions.IsKind(accessor, Microsoft.CodeAnalysis.CSharp.SyntaxKind.SetAccessorDeclaration)
                                || Microsoft.CodeAnalysis.CSharpExtensions.IsKind(accessor, Microsoft.CodeAnalysis.CSharp.SyntaxKind.InitAccessorDeclaration));
            var accessors = (hasGetter, hasSetter) switch
            {
                (true, true) => "{ get; set; }",
                // A getter-only auto-property cannot be assigned, so it reads as default -
                // which is what a stub has to offer anyway.
                (true, false) => "{ get; }",
                // An auto-property must have a getter (CS8051), so a setter-only property
                // gets one it did not have. Reading it is not something the original allowed,
                // so nothing can be relying on the value.
                (false, true) => "{ get; set; }",
                _ => null,
            };
            if (accessors is null)
            {
                return null;
            }
            return $"{Prefix(property.Modifiers)}{propertyType} {property.Identifier.Text} {accessors}";
        }

        case Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax field:
        {
            if (!IsPublic(field.Modifiers))
            {
                return null;
            }
            var fieldType = ResolveType(field.Declaration.Type, known, lookupNamespaces);
            if (fieldType is null)
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
                    ? $"public const {fieldType} {variable.Identifier.Text} = {literal};"
                    : null;
            }
            return $"{Prefix(field.Modifiers)}{fieldType} {variable.Identifier.Text};";
        }

        case Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax constructor:
        {
            // A stub declaring no constructor gets the implicit parameterless one, and every
            // "new Stub(a, b)" then fails with CS1729 against a type that otherwise looks
            // fine. An attribute is the same case: [Stub("x")] IS a constructor call, which
            // is why giving those stubs their Attribute base only moved the error.
            if (containerIsStatic || containerIsInterface || !IsPublic(constructor.Modifiers))
            {
                return null;
            }
            var constructorParameters = new List<string>();
            foreach (var parameter in constructor.ParameterList.Parameters)
            {
                var passing = string.Concat(parameter.Modifiers.Select(modifier => modifier.Text + " "));
                var parameterType = parameter.Modifiers.Any(modifier =>
                        Microsoft.CodeAnalysis.CSharpExtensions.IsKind(modifier, Microsoft.CodeAnalysis.CSharp.SyntaxKind.ThisKeyword))
                    ? null
                    : ResolveType(parameter.Type, known, lookupNamespaces);
                if (parameterType is null)
                {
                    return null;
                }
                constructorParameters.Add($"{passing}{parameterType} {parameter.Identifier.Text}");
            }
            // An empty body, not a throw: constructing one of these is how ported code
            // reaches the members the stub does carry, and an attribute is only ever
            // constructed by the runtime reading metadata.
            return $"public {containerName}({string.Join(", ", constructorParameters)}) {{ }}";
        }

        default:
            return null;
    }
}

/// <summary>
/// The type as it can be written in the stub file, fully qualified, or null when nothing
/// it names can be resolved from here.
///
/// The stub file carries two usings (System, System.Collections.Generic) and cannot carry
/// the source file's, which would drag in the very namespaces that were excluded. So the
/// name lookup happens HERE and the result is written out qualified.
///
/// This used to be a bool plus a hand-written list of "simple" BCL names, and a member was
/// kept only when its signature named a type in the SAME namespace. That drops most of what
/// a real base class declares: DNN's PermissionsGrid lost AddPermission / GetPermissions /
/// SupportsDenyPermissions to ArrayList, RoleInfo, UserInfo and PermissionInfo - one BCL
/// type not on the list and three ported types one namespace over - and every grid deriving
/// from it then failed CS0115 on all of them.
/// </summary>
static string? ResolveType(
    Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? type,
    HashSet<string> known,
    IReadOnlyList<string> lookupNamespaces)
{
    switch (type)
    {
        case null:
            return null;
        case Microsoft.CodeAnalysis.CSharp.Syntax.PredefinedTypeSyntax predefined:
            return predefined.ToString();
        case Microsoft.CodeAnalysis.CSharp.Syntax.NullableTypeSyntax nullable:
            return ResolveType(nullable.ElementType, known, lookupNamespaces) is { } element
                ? element + "?"
                : null;
        case Microsoft.CodeAnalysis.CSharp.Syntax.ArrayTypeSyntax array:
            return ResolveType(array.ElementType, known, lookupNamespaces) is { } item
                ? item + string.Concat(array.RankSpecifiers.Select(rank => rank.ToString()))
                : null;
        case Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax generic:
        {
            var definition = ResolveTypeName(
                generic.Identifier.Text, generic.TypeArgumentList.Arguments.Count, known, lookupNamespaces);
            if (definition is null)
            {
                return null;
            }
            var arguments = new List<string>();
            foreach (var argument in generic.TypeArgumentList.Arguments)
            {
                var resolved = ResolveType(argument, known, lookupNamespaces);
                if (resolved is null)
                {
                    return null;
                }
                arguments.Add(resolved);
            }
            return $"{definition}<{string.Join(", ", arguments)}>";
        }
        case Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax identifier:
            return ResolveTypeName(identifier.Identifier.Text, 0, known, lookupNamespaces);
        case Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax qualified:
            // Written out in the source ("System.Data.IDataReader", "Collections.ArrayList").
            // Treated as a name to look up like any other, so a partial qualification still
            // resolves through the file's imports.
            return ResolveTypeName(qualified.ToString(), 0, known, lookupNamespaces);
        default:
            return null;
    }
}

/// <summary>
/// One type name against the namespaces in scope, project types first. Returns the
/// global:: qualified name, or null when no namespace in scope declares it.
/// </summary>
static string? ResolveTypeName(
    string name, int arity, HashSet<string> known, IReadOnlyList<string> lookupNamespaces)
{
    // System.Web.UI.Control becomes IWebFormsControl in ported code, so a stub has to say
    // the same thing. It used to resolve to the compat layer's Control CLASS, which is a
    // different type: N2's AbstractDisplayableAttribute declares
    // "AddTo(ContentItem, string, IWebFormsControl)" after porting, and the stub for a
    // subclass wrote "AddTo(ContentItem, string, Control)" - an override of a method that
    // does not exist, and the error names the stub rather than the divergence.
    //
    // The port's own Control wins, as everywhere else: a project that declares one means
    // that one, and the check below runs first.
    if (arity == 0
        && (name == "Control" || name == "System.Web.UI.Control")
        && !known.Contains("Control")
        && !lookupNamespaces.Any(ns => known.Contains(QualifiedName(ns, "Control"))))
    {
        return "global::WebForm2Blazor.Components.IWebFormsControl";
    }

    // A name already written in full needs no prefix.
    if (known.Contains(name) || WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.Contains(MetadataName(name, arity)))
    {
        return "global::" + name;
    }

    foreach (var candidateNamespace in lookupNamespaces)
    {
        var candidate = QualifiedName(candidateNamespace, name);
        if (known.Contains(candidate) || WebForm2Blazor.Converter.Convert.FrameworkTypeIndex.Contains(MetadataName(candidate, arity)))
        {
            return "global::" + candidate;
        }

        // A name imported from System.Web resolves to the compat layer after the port, so
        // that is the name the stub has to write. Missing this dropped every member taking
        // an HtmlTextWriter - RenderViewMode / RenderEditMode / Render on DNN's EditControl -
        // and the CS0115 came out on the controls deriving from it.
        if (IsSystemWebNamespace(candidateNamespace)
            && CodeBehindRewriter.DeclaresCompatType(name))
        {
            return "global::WebForm2Blazor.Components." + name;
        }
    }

    return null;
}

/// <summary>System.Web and everything under it - the namespaces the compat layer flattens.</summary>
static bool IsSystemWebNamespace(string name)
    => name == "System.Web" || name.StartsWith("System.Web.", StringComparison.Ordinal);

/// <summary>Metadata spells a generic type "Ns.List`1"; the source spells it "List&lt;T&gt;".</summary>
static string MetadataName(string qualifiedName, int arity)
    => arity == 0 ? qualifiedName : qualifiedName + "`" + arity;

/// <summary>
/// The namespaces a name written in this file could bind to: its own and each enclosing
/// one, then its imports - the order the compiler would try.
/// </summary>
static List<string> LookupNamespacesFor(
    Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax declaration,
    string declaredNamespace,
    IReadOnlyList<string> globalUsings)
{
    var namespaces = new List<string>();

    if (!string.IsNullOrEmpty(declaredNamespace))
    {
        var parts = declaredNamespace.Split('.');
        for (var count = parts.Length; count > 0; count--)
        {
            namespaces.Add(string.Join(".", parts.Take(count)));
        }
    }

    foreach (var directive in declaration.SyntaxTree.GetRoot()
                 .DescendantNodes()
                 .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax>())
    {
        // An alias names one type under a different name; reproducing that in the stub
        // would need the alias too, so those are left to fail the resolve.
        if (directive.Alias is null && directive.Name is { } name)
        {
            namespaces.Add(name.ToString());
        }
    }

    // The web project's GLOBAL usings. A stub member is only emitted when every type in
    // its signature resolves, and a file that relies on a global using names its types
    // with nothing in the file to resolve them by: YAF's IAspNetRoleManager returns
    // AspNetRoles, which lives two namespaces away and is reached globally. Without these
    // the interface stubbed out to one member and three dropped, and the error landed on
    // the file that CALLS all four.
    namespaces.AddRange(globalUsings);

    namespaces.Add("System");
    return namespaces;
}

/// <summary>Namespace-qualified type key ("Ns.Name", or just "Name" in the global namespace).</summary>
static string QualifiedName(string declaredNamespace, string name)
    => string.IsNullOrEmpty(declaredNamespace) ? name : declaredNamespace + "." + name;


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
    // An HTML COMMENT, not a visible span. The stub used to render
    // "<span class="w2b-stub">[asp:LoginView]</span>", which put text on the page that the
    // original never had - running WingtipToys showed "[asp:LoginView]" and
    // "[webopt:bundlereference]" sitting in the header of every page. Against a goal of
    // rendering what the original rendered, a visible marker IS a difference.
    //
    // The gap still has to be visible SOMEWHERE, and it is: every one of these is already
    // a residual in the conversion report, and the comment names the tag in the page
    // source. What changes is that it no longer shows up to a user of the site.
    => $$"""
        @namespace {{appName}}.Components.Stubs

        @* Auto-generated placeholder for the unconverted control <{{originalTag}}>.
           Map the tag to a real component via --control-map, or replace this stub with a
           hand-ported component. Original markup attributes arrive in UnmatchedParameters;
           child template markup arrives as ChildContent and is intentionally not rendered. *@
        @((MarkupString)$"<!-- W2B: unconverted control {{originalTag}}{(string.IsNullOrEmpty(ID) ? "" : $" id={ID}")} -->")

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
    // System / collections: the field list rarely needs them, but a moved
    // <script runat="server"> body is ordinary code-behind and uses EventArgs, List<T>
    // and LINQ the way any handler does.
    builder.AppendLine("using System;");
    builder.AppendLine("using System.Collections.Generic;");
    builder.AppendLine("using System.Linq;");
    builder.AppendLine("using Microsoft.AspNetCore.Components;");
    builder.AppendLine("using WebForm2Blazor.Components;");
    var usings = component.UsedControlNamespaces.ToHashSet(StringComparer.Ordinal);

    // The .razor and this file are two halves of ONE partial class, so whatever the markup
    // imports applies here too. It matters once a <script runat="server"> body lands here:
    // that code was written against the page's own <%@ Import %> directives, which the
    // razor carries as @using and this half did not.
    if (component.ServerScriptBlocks.Count > 0)
    {
        foreach (var line in component.RazorContent.Split('\n'))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("@using ", StringComparison.Ordinal) && trimmed.EndsWith(';') is false)
            {
                usings.Add(trimmed["@using ".Length..].Trim());
            }
        }
    }

    foreach (var ns in usings.Distinct().Order(StringComparer.Ordinal))
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
        builder.Append(field.LegacyHost
            ? WebForm2Blazor.Converter.Convert.CodeBehindRewriter.EmitLegacyHostField(field, "        ")
            : $"        protected {field.Type} {field.Name};\r\n");
    }

    // <script runat="server"> is code-behind written inside the markup, and this generated
    // partial is where code-behind goes. Emitted verbatim: it is the application's own C#
    // and rewriting it here would duplicate what the code-behind rewriter already does for
    // a real .aspx.cs.
    foreach (var script in component.ServerScriptBlocks)
    {
        builder.AppendLine();
        builder.AppendLine("        // Moved from a <script runat=\"server\"> block in the markup.");
        builder.AppendLine(script.TrimEnd());
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
