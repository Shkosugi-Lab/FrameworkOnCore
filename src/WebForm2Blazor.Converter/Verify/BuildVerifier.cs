using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace WebForm2Blazor.Converter.Verify;

/// <summary>
/// Builds a converted project and classifies the compiler diagnostics.
///
/// Residual count alone does not tell whether the output actually compiles: whole
/// defect classes (parser mistakes, missing compatibility APIs, unresolvable
/// dependencies) only surface at build time. This turns that build into a repeatable
/// metric alongside the residual report.
/// </summary>
public static partial class BuildVerifier
{
    private sealed record Diagnostic(string File, int Line, string Code, string Message)
    {
        /// <summary>
        /// The .csproj MSBuild attributes this diagnostic to, which it prints in brackets at
        /// the end of every line. Null when the line did not carry one.
        /// </summary>
        public string? Project
        {
            get
            {
                var match = ProjectSuffix().Match(Message);
                return match.Success ? match.Groups[1].Value : null;
            }
        }

        /// <summary>Which layer the diagnostic belongs to, derived from the file it lands in.</summary>
        public FileKind Kind => File.EndsWith(".razor", StringComparison.OrdinalIgnoreCase)
                                || File.EndsWith("_razor.g.cs", StringComparison.OrdinalIgnoreCase)
            ? FileKind.GeneratedMarkup
            : File.EndsWith(".razor.cs", StringComparison.OrdinalIgnoreCase)
                ? FileKind.PortedCodeBehind
                : FileKind.PortedLibrary;
    }

    private enum FileKind
    {
        /// <summary>Emitted .razor - the converter wrote it, so an error here is the converter's.</summary>
        GeneratedMarkup,

        /// <summary>Ported code-behind - usually a missing compatibility API.</summary>
        PortedCodeBehind,

        /// <summary>Plain ported .cs - usually an unportable dependency.</summary>
        PortedLibrary,
    }

    /// <summary>
    /// Error codes that always mean the same thing regardless of locale. Message text is
    /// localized by the SDK, so classification keys off codes only.
    /// </summary>
    private static readonly Dictionary<string, string> KnownCauses = new(StringComparer.Ordinal)
    {
        // Razor could not parse / accept what the converter emitted
        ["RZ1005"] = "変換器の不具合(生成 Razor の構文)",
        ["RZ2008"] = "変換器の不具合(パラメータ値の型)",
        ["RZ9980"] = "変換器の不具合(タグの非対応)",
        ["RZ9981"] = "変換器の不具合(タグの非対応)",
        ["RZ9983"] = "変換器の不具合(void 要素の閉じタグ)",
        ["RZ9985"] = "変換器の不具合(コンポーネント名の衝突)",
        ["RZ9996"] = "変換器の不具合(未対応の子要素)",
        ["RZ9999"] = "変換器の不具合(テンプレートの Context 名衝突)",
        ["RZ10009"] = "変換器の不具合(パラメータの重複)",
        ["RZ10011"] = "変換器の不具合(小文字のコンポーネント名)",
        ["CS1003"] = "変換器の不具合(生成コードの構文)",
        ["CS1525"] = "変換器の不具合(生成コードの構文)",
        ["CS8124"] = "変換器の不具合(生成コードの構文)",
        ["CS0102"] = "変換器の不具合(メンバーの重複生成)",
        ["CS0111"] = "変換器の不具合(メンバーの重複生成)",
        ["CS0542"] = "変換器の不具合(型名と同名のメンバー)",

        // The compatibility layer does not expose an API the ported code uses
        ["CS0246"] = "互換シム不足(型が見つからない)",
        ["CS0234"] = "互換シム不足(名前空間が見つからない)",
        ["CS0117"] = "互換シム不足(メンバーが無い)",
        ["CS1061"] = "互換シム不足(メンバーが無い)",
        ["CS0103"] = "互換シム不足(名前が解決できない)",
        ["CS1069"] = "互換シム不足(アセンブリ参照)",
        ["CS0115"] = "互換シム不足(オーバーライド先が無い)",
        ["CS0534"] = "互換シム不足(抽象メンバー未実装)",
        ["CS0506"] = "互換シム不足(virtual でない)",
        ["CS0538"] = "互換シム不足(インターフェイスでない)",
        ["CS1729"] = "互換シム不足(コンストラクタ)",

        // Semantics that have no Blazor equivalent
        ["CS0021"] = "手動移行領域(動的コントロール操作)",

        // Not an independent failure: a Razor template compiles to a lambda, so any
        // broken expression inside it also reports as a lambda conversion failure
        ["CS1662"] = "連鎖(テンプレート内の式エラーの二次症状)",
    };

    /// <summary>
    /// Diagnostics produced by the PARSER. C# never runs semantic analysis over a
    /// compilation it could not parse, so the moment one of these appears the error count
    /// stops being a total and becomes a FLOOR: every type-resolution error behind it goes
    /// unreported. This is not a small effect - injecting a single syntax error into the
    /// converted YAF.NET output took the reported count from 1,630 down to 2.
    ///
    /// The list is deliberately over-inclusive. Treating a semantic error as a syntax one
    /// only costs a rejected candidate, while the reverse lets syntactically broken code
    /// walk through a gate that compares counts.
    /// </summary>
    private static readonly HashSet<string> ParseErrorCodes = new(StringComparer.Ordinal)
    {
        "CS1001", "CS1002", "CS1003", "CS1004", "CS1010", "CS1012", "CS1022", "CS1026",
        "CS1027", "CS1031", "CS1035", "CS1039", "CS1041", "CS1056", "CS1513", "CS1514",
        "CS1519", "CS1520", "CS1525", "CS1526", "CS1528", "CS1547", "CS1553", "CS1733",
        "CS8124", "CS8641",
    };

    /// <summary>
    /// The result of a gate build.
    /// </summary>
    /// <param name="ErrorCount">
    /// Number of distinct error diagnostics. Only a total when
    /// <paramref name="StoppedAtParse"/> is false; otherwise a lower bound.
    /// </param>
    /// <param name="StoppedAtParse">
    /// The sources would not parse, so semantic analysis never ran and
    /// <paramref name="ErrorCount"/> may hide an arbitrary number of further errors.
    /// Counts from such a build must never be compared against one from a build that
    /// completed - the broken one looks better.
    /// </param>
    public readonly record struct BuildOutcome(int ErrorCount, bool StoppedAtParse);

    /// <summary>
    /// Builds for a gate check (no report written). Null when the build could not be run or
    /// failed without parseable diagnostics - the caller must treat that as a failure rather
    /// than as success.
    /// </summary>
    public static BuildOutcome? Measure(string outputDirectory)
    {
        var projectPath = Directory.EnumerateFiles(outputDirectory, "*.csproj").FirstOrDefault();
        if (projectPath is null)
        {
            return null;
        }

        var (output, exitCode) = RunBuildPastUndecided(projectPath, outputDirectory, out _);
        var diagnostics = Parse(output);
        if (exitCode != 0 && diagnostics.Count == 0)
        {
            return null;
        }

        // StoppedAtParse is judged on ALL diagnostics: a syntax error hides real errors
        // whether or not an undecided dependency is also in the file.
        var stoppedAtParse = StoppedAtParse(diagnostics, outputDirectory);
        var undecidedTypes = ReadUndecidedDependencyTypes(outputDirectory);
        var restored = ReadRestoredOutOfScopeFiles(outputDirectory);
        var undecidedByFile = UndecidedAssemblyByFile(diagnostics, undecidedTypes);
        var counted = diagnostics.Count(d =>
            UndecidedDependency(d, undecidedTypes) is null
            && UndecidedBaseClass(d, undecidedByFile) is null
            && !InRestoredOutOfScopeFile(d, restored));
        return new BuildOutcome(counted, stoppedAtParse);
    }

    /// <summary>
    /// True when the count cannot be read as a total. Two ways that happens:
    ///
    /// 1. The sources did not parse, so semantic analysis never ran (see
    ///    <see cref="ParseErrorCodes"/>).
    /// 2. The build failed OUTSIDE the compiler and so never reached it. An SDK or MSBuild
    ///    error (NETSDK1022 from a duplicate item, a missing reference, an analyzer that
    ///    would not load) aborts the build while the compiler still has zero diagnostics,
    ///    and the run reports a handful of errors where a real compile would report
    ///    thousands. A duplicate App_Data Content item put n2cms in exactly this state and
    ///    the AI gate accepted a deliberately broken answer because "1 error" had not moved.
    ///
    /// Recognising (2) as "no CS diagnostic was produced at all" rather than by listing
    /// tool codes keeps it closed against MSB*, NETSDK*, RZ* and anything else that fails
    /// ahead of the compiler.
    /// </summary>
    private static bool StoppedAtParse(List<Diagnostic> diagnostics, string? outputDirectory = null)
        => DependencyProjectFailed(diagnostics, outputDirectory)
            || SourcesFailToParse(outputDirectory)
            || diagnostics.Any(diagnostic => ParseErrorCodes.Contains(diagnostic.Code))
            // ANY Razor error. RZ means the Razor compiler refused a .razor, so the C#
            // it would have generated for that file never existed and nothing in it was
            // ever bound - the same stop as a syntax error, one stage earlier.
            //
            // The "no CS diagnostic at all" rule below does not catch it: a project can
            // have an RZ error in one file and ordinary CS errors in another, which is
            // exactly what happened - mojoPortal reported "390 -> 2, improvement" with no
            // floor warning while two RZ9996 were stopping the build.
            || diagnostics.Any(diagnostic =>
                diagnostic.Code.StartsWith("RZ", StringComparison.Ordinal))
            || (diagnostics.Count > 0
                && !diagnostics.Any(diagnostic =>
                    diagnostic.Code.StartsWith("CS", StringComparison.Ordinal)))
            || StoppedAtDeclarations(diagnostics);

    /// <summary>
    /// The application project never compiled, because a project it REFERENCES failed
    /// first.
    ///
    /// MSBuild builds in dependency order and stops a branch at the first failure, so one
    /// error in a library means the application - and everything else downstream of that
    /// library - is never handed to the compiler at all. The count is then a floor, and an
    /// unusually convincing one: with --split-projects, n2cms reported "1 error" against a
    /// merged baseline of 4 and mojoPortal reported "1" against 312, and in every case the
    /// application assembly had not been produced. Nothing else here catches it. A single
    /// CS1501 in a library is not a parse error, not a declaration-stage error, and not the
    /// absence of CS diagnostics, so all three existing backstops read the build as complete.
    ///
    /// Judged on the .csproj MSBuild attributes each diagnostic to. With one project -
    /// every merged conversion - no diagnostic can name another project, so this never
    /// fires and the merged numbers are untouched.
    /// </summary>
    private static bool DependencyProjectFailed(List<Diagnostic> diagnostics, string? outputDirectory)
    {
        if (outputDirectory is null || diagnostics.Count == 0)
        {
            return false;
        }

        var applicationProject = Directory.EnumerateFiles(outputDirectory, "*.csproj").FirstOrDefault();
        if (applicationProject is null)
        {
            return false;
        }

        return diagnostics.Any(diagnostic =>
        {
            if (diagnostic.Project is not { } project)
            {
                return false;
            }
            try
            {
                return !string.Equals(
                    Path.GetFullPath(project),
                    Path.GetFullPath(applicationProject),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch (ArgumentException)
            {
                return false;
            }
        });
    }

    /// <summary>The "[C:\...\Thing.csproj]" MSBuild appends to every diagnostic line.</summary>
    [GeneratedRegex(@"\[([^\[\]]+\.csproj)\]\s*$")]
    private static partial Regex ProjectSuffix();

    /// <summary>
    /// Whether the emitted C# actually parses - asked of Roslyn, not of a code list.
    ///
    /// <see cref="ParseErrorCodes"/> is 26 hand-picked codes out of the couple of hundred
    /// the C# parser can produce: CS1002 is there and CS1005 is not, CS1513/1514 are there
    /// and CS1515/1517/1518 are not. A build with one unlisted syntax code AND ordinary
    /// semantic errors slips past every backstop - the "no CS diagnostic at all" rule
    /// fails the moment any CS error exists, and StoppedAtDeclarations fails the moment
    /// any non-declaration code does. It reports a floor as a total, silently. That shape
    /// has now cost three wrong readings in one session (CS0506, RZ*, and the codes this
    /// replaces).
    ///
    /// "Do these sources parse" is a question the converter can put to the parser directly,
    /// in milliseconds, with no list to keep current and no dependence on the SDK's
    /// localized message text.
    ///
    /// Only the files the converter WROTE. bin/ and obj/ hold generated and copied code
    /// that is not the output's to answer for, and the Razor-generated .cs does not exist
    /// until the build runs.
    /// </summary>
    private static bool SourcesFailToParse(string? outputDirectory)
    {
        if (outputDirectory is null || !Directory.Exists(outputDirectory))
        {
            return false;
        }

        foreach (var file in Directory.EnumerateFiles(outputDirectory, "*.cs", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(outputDirectory, file).Replace('\\', '/');
            // Any bin/ or obj/ SEGMENT, not just a leading one. With --split-projects the
            // build output of a referenced library lands at "<library>/obj/..." and a
            // leading-prefix test does not see it - the generated code in there is not the
            // converter's to answer for, and reading it as "the sources do not parse" would
            // mark every count in a split conversion as a floor.
            if (relative.Split('/').Any(segment =>
                    segment.Equals("bin", StringComparison.OrdinalIgnoreCase)
                    || segment.Equals("obj", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            try
            {
                var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(
                    File.ReadAllText(file),
                    Microsoft.CodeAnalysis.CSharp.CSharpParseOptions.Default
                        .WithPreprocessorSymbols("DEBUG"));
                if (tree.GetDiagnostics().Any(diagnostic =>
                        diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                // Unreadable is not "does not parse" - say nothing rather than guess.
            }
        }

        return false;
    }

    /// <summary>
    /// Errors about an INHERITANCE CONTRACT - "no suitable method to override", "does not
    /// implement inherited abstract member". Every one of them is a statement about a base
    /// class, so when the base class is the thing that went missing, they are the same
    /// dependency reported a second time.
    ///
    /// DNN's DnnBodyProvider derives from ClientDependency.Core's
    /// WebFormsFileRegistrationProvider, which is a vendored DLL nobody has chosen a
    /// package for. The CS0246 for the base is attributed; the four CS0534 / CS0115 the
    /// missing base then produces name only DNN's own types, so they read as conversion
    /// defects. Sixteen of DNN's nineteen counted errors were that.
    ///
    /// Declared HERE, above the two sets that use it, because both of them need it and
    /// they had drifted apart: CS0535 / CS0537 / CS0540 were in this set and missing from
    /// <see cref="DeclarationErrorCodes"/>, which is the same incomplete-list shape that
    /// let CS0506 floor mojoPortal's count at 1 while 1043 errors waited behind it.
    /// One set, used twice, so they cannot disagree again.
    /// </summary>
    private static readonly HashSet<string> InheritanceContractCodes = new(StringComparer.Ordinal)
    {
        "CS0115", "CS0506", "CS0507", "CS0533", "CS0534", "CS0535", "CS0537", "CS0540",
    };

    /// <summary>
    /// Errors a SIGNATURE can have, which is all the Razor SDK's first pass checks.
    ///
    /// A Blazor project compiles twice: a declaration-only pass, then the real one with the
    /// generated .razor code. An error in the first pass stops the build before the second,
    /// and only signature-level diagnostics are reported - method BODIES were never bound.
    /// </summary>
    private static readonly HashSet<string> DeclarationErrorCodes = new(InheritanceContractCodes, StringComparer.Ordinal)
    {
        "CS0106", "CS0111", "CS0101", "CS0509",
        "CS0549", "CS0238", "CS0539", "CS0736", "CS0738",
        // Type resolution belongs here too. A signature names types, so an unresolved one
        // fails the declaration pass just as a bad override does - and these codes also
        // occur in method bodies, which is why they were left out at first. Four times in
        // one session a small count turned out to be a floor hiding hundreds (YAF: 1, then
        // 2, then 10), and every one of those was CS0234/CS0246. Saying "this may be a
        // floor" when it is not costs a sentence; not saying it cost four wrong readings.
        "CS0234", "CS0246", "CS0012", "CS1069",
        // CS0400 is the same failure written differently: "global::X not found in the
        // global namespace". It hid yaf's count at 1 while 337 errors waited behind it.
        "CS0400",
        // The rest of the "this override does not fit its base" family, beyond the shared
        // set above. Those are follow-on-able (a missing base explains them); these are
        // only ever a declaration-stage stop.
        //
        // An override error is a declaration error by construction - the compiler is
        // matching a signature against a base, which is the only thing the first pass does.
        // Adding a non-virtual Text to the TextBox base produced CS0506 on the one ported
        // editor that overrides it, the build stopped at declarations, and mojoPortal
        // reported "1159 -> 1" as an IMPROVEMENT with no floor warning at all.
        "CS0505", "CS0508", "CS0239", "CS0546", "CS0545", "CS0550",
    };

    /// <summary>
    /// True when every error is one the declaration pass could have raised, which means the
    /// build may never have reached the pass that binds method bodies.
    ///
    /// YAF reported ONE build error for a long time. Removing that one error - an override
    /// of DbProviderFactory.CreatePermission, a member .NET deleted - took the count to
    /// 2135, because the declaration pass had been failing on it and the real compile had
    /// never run. The number was a floor and nothing said so. It says so now.
    /// </summary>
    private static bool StoppedAtDeclarations(List<Diagnostic> diagnostics)
        => diagnostics.Count > 0
           && diagnostics.All(diagnostic => DeclarationErrorCodes.Contains(diagnostic.Code));

    /// <summary>
    /// Diagnostics caused by a vendored DLL whose replacement package the user has not
    /// chosen yet.
    ///
    /// These are NOT conversion defects. The converter cannot pick the package (Lucene.Net
    /// 3.0.3 -> 4.8 is a rewrite, not an upgrade), so nothing in this tool can remove them;
    /// only a --package-map answer can. Counting them together with the rest was actively
    /// misleading, because they are numerous enough to dominate: of DNN Platform's 203
    /// missing-type errors, 154 were Lucene.Net and DotNetNuke.WebControls.
    ///
    /// Membership is decided by the ASSEMBLY'S OWN METADATA - the converter writes
    /// unresolved-dependency-types.txt next to the package-map template while it still has
    /// the DLL in hand. A prefix rule would have to guess; this reads the answer.
    /// </summary>
    /// <summary>
    /// The file name of the scaffold the gate writes, builds against, and deletes.
    /// </summary>
    private const string ScaffoldFile = "_UndecidedDependencyScaffold.g.cs";

    /// <summary>
    /// Re-declared names read from unresolved-dependency-shapes.txt.
    /// </summary>
    /// <summary>
    /// Every diagnostic, one per line, next to the report.
    ///
    /// BUILD-REPORT.md groups by code and shows ONE representative message per group. That
    /// is right for reading, and wrong for deciding what to fix: the representative is not
    /// the most common message, and reading it as one sends you after a shim gap that
    /// accounts for 6 of the 86 errors under its heading. The grouped view cannot answer
    /// "which single message repeats most" at all - only the full list can, and
    /// "dotnet build" cannot produce it either, because a declaration-pass error stops the
    /// build long before the rest are reported.
    /// </summary>
    private static void WriteFullErrorList(string? outputDirectory, List<Diagnostic> diagnostics)
    {
        if (outputDirectory is null)
        {
            return;
        }

        var lines = diagnostics
            .OrderBy(d => d.File, StringComparer.OrdinalIgnoreCase)
            .ThenBy(d => d.Line)
            .Select(d => $"{d.File}({d.Line}): {d.Code}: {d.Message}");

        File.WriteAllLines(Path.Combine(outputDirectory, "build-errors.txt"), lines);
    }

    private sealed record UndecidedType(string Namespace, string Name, string Shape)
    {
        public string FullName => Namespace + "." + Name;

        /// <summary>The first segment, which is what a "namespace not found" error names.</summary>
        public string RootNamespace => Namespace.Split('.')[0];
    }

    private static List<UndecidedType> ReadUndecidedDependencyShapes(string outputDirectory)
    {
        var types = new List<UndecidedType>();
        var path = Path.Combine(outputDirectory, "unresolved-dependency-shapes.txt");
        if (!File.Exists(path))
        {
            return types;
        }

        foreach (var line in File.ReadLines(path))
        {
            var parts = line.Split('\t');
            if (parts.Length == 3 && parts[0].Length > 0 && parts[1].Length > 0)
            {
                types.Add(new UndecidedType(parts[0], parts[1], parts[2]));
            }
        }
        return types;
    }

    /// <summary>
    /// Every single-quoted token in a "name not found" diagnostic.
    ///
    /// Read this way rather than by matching the sentence, because the SDK localizes
    /// message text and this file already learned once to key off codes only. Extra tokens
    /// are harmless: nothing is done with a token unless the shapes file also has it, so a
    /// name the compiler was merely mentioning cannot cause anything to be declared.
    /// </summary>
    private static HashSet<string> UnresolvedNames(List<Diagnostic> diagnostics)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var diagnostic in diagnostics)
        {
            if (diagnostic.Code is not ("CS0246" or "CS0234" or "CS0400" or "CS0103"))
            {
                continue;
            }
            foreach (Match match in QuotedToken().Matches(diagnostic.Message))
            {
                names.Add(match.Groups[1].Value);
            }
        }
        return names;
    }

    [GeneratedRegex("'([^']+)'")]
    private static partial Regex QuotedToken();

    /// <summary>
    /// Builds, and if the build stopped before binding method bodies BECAUSE of a vendored
    /// DLL whose replacement nobody has chosen, re-declares exactly the missing names and
    /// builds again.
    ///
    /// Why this exists: the gate does not COUNT those errors - the decision is the reader's,
    /// not the converter's - but an uncounted error still stops the compiler. A Blazor
    /// project compiles twice, and an unresolved name fails the first pass, so the real
    /// compile never runs and every other error goes unseen. mojoPortal reported "6 build
    /// errors" for that reason while the figure behind two uncounted HtmlDiff errors was
    /// 2236. Excluding an error from the count and letting it decide the entire measurement
    /// are not consistent positions.
    ///
    /// Only names the compiler ITSELF reported as missing are declared. That distinction is
    /// the whole design: declaring the vendored DLL's types up front - which was tried -
    /// shadows the real library where the project also references a package that provides
    /// it, and took BlogEngine from 0 errors to 23 against a SharpZipLib that had resolved
    /// perfectly well. A name the compiler says is missing is shadowing nothing.
    ///
    /// The scaffold is deleted again afterwards. It is measurement apparatus, not output:
    /// left in place it would keep resolving those names after the user fills in
    /// --package-map and rebuilds, which is the same shadowing by a slower route.
    /// </summary>
    private static (string Output, int ExitCode) RunBuildPastUndecided(
        string projectPath, string outputDirectory, out int scaffoldedTypeCount)
    {
        scaffoldedTypeCount = 0;
        var scaffoldPath = Path.Combine(outputDirectory, ScaffoldFile);
        // A scaffold left behind by an interrupted run would resolve names silently.
        File.Delete(scaffoldPath);

        var (output, exitCode) = RunBuild(projectPath);

        var shapes = ReadUndecidedDependencyShapes(outputDirectory);
        if (shapes.Count == 0)
        {
            return (output, exitCode);
        }

        var declared = new List<UndecidedType>();
        var declaredNames = new HashSet<string>(StringComparer.Ordinal);

        // Bounded: a round that declares nothing new stops the loop anyway, and the cap
        // keeps a pathological project from rebuilding indefinitely.
        for (var round = 0; round < 3; round++)
        {
            var diagnostics = Parse(output);
            if (!StoppedAtParse(diagnostics))
            {
                break;
            }

            var missing = UnresolvedNames(diagnostics);
            if (missing.Count == 0)
            {
                break;
            }

            // A missing NAMESPACE brings its whole namespace in - a namespace exists only
            // by way of the types in it, and the compiler saying the namespace is absent
            // is the evidence that none of them resolve.
            var additions = shapes
                .Where(type => !declaredNames.Contains(type.FullName))
                .Where(type => missing.Contains(type.RootNamespace)
                               || missing.Contains(type.Namespace)
                               || missing.Contains(type.Name))
                .ToList();

            if (additions.Count == 0)
            {
                break;
            }

            declared.AddRange(additions);
            foreach (var type in additions)
            {
                declaredNames.Add(type.FullName);
            }

            File.WriteAllText(scaffoldPath, RenderScaffold(declared));
            (output, exitCode) = RunBuild(projectPath);
        }

        scaffoldedTypeCount = declared.Count;
        File.Delete(scaffoldPath);
        return (output, exitCode);
    }

    private static string RenderScaffold(List<UndecidedType> types)
    {
        var writer = new StringBuilder();
        writer.AppendLine("// <auto-generated />");
        writer.AppendLine("// ビルド検証のための一時ファイルです(検証後に削除されます)。");
        writer.AppendLine("// 置き換え先が未決の同梱 DLL の型のうち、コンパイラが「見つからない」と");
        writer.AppendLine("// 報告した名前だけを再宣言します。実装ではありません。");
        writer.AppendLine();

        foreach (var group in types
            .GroupBy(type => type.Namespace, StringComparer.Ordinal)
            .OrderBy(group => group.Key, StringComparer.Ordinal))
        {
            writer.AppendLine($"namespace {group.Key}");
            writer.AppendLine("{");
            foreach (var type in group
                .DistinctBy(type => type.Name, StringComparer.Ordinal)
                .OrderBy(type => type.Name, StringComparer.Ordinal))
            {
                var keyword = type.Shape switch
                {
                    "Interface" => "interface",
                    "Enum" => "enum",
                    "Struct" => "struct",
                    "AbstractClass" => "abstract class",
                    _ => "class",
                };
                writer.AppendLine($"    public {keyword} {type.Name}");
                writer.AppendLine("    {");
                writer.AppendLine("    }");
            }
            writer.AppendLine("}");
            writer.AppendLine();
        }

        return writer.ToString();
    }

    /// <summary>
    /// Files the converter excluded as ANOTHER FRAMEWORK'S code and then handed back.
    ///
    /// The restore pass does that when a file that IS being ported names one of their
    /// types: excluded, the exclusion cascades through everything downstream, which costs
    /// far more than keeping them. The converter records the trade at the moment it makes
    /// it ("このファイル内には未解決の型が残ります") - so the unresolved types inside are
    /// not a defect being discovered here, they are the price that was already reported.
    ///
    /// DNN is the case: 50 of its 67 counted errors were MVC types inside six restored
    /// MVC files. Counting them as conversion defects is the same mistake as counting a
    /// vendored DLL nobody has chosen a package for - numerous enough to dominate, and
    /// nothing in the converter can remove them while the file has to stay.
    /// </summary>
    private static HashSet<string> ReadRestoredOutOfScopeFiles(string outputDirectory)
    {
        var files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var path = Path.Combine(outputDirectory, "restored-out-of-scope-files.txt");
        if (!File.Exists(path))
        {
            return files;
        }

        foreach (var line in File.ReadLines(path))
        {
            if (line.Length > 0)
            {
                files.Add(Path.GetFullPath(Path.Combine(
                    outputDirectory, line.Replace('/', Path.DirectorySeparatorChar))));
            }
        }
        return files;
    }

    /// <summary>Whether this diagnostic landed in one of those files.</summary>
    private static bool InRestoredOutOfScopeFile(Diagnostic diagnostic, HashSet<string> restored)
    {
        if (restored.Count == 0 || diagnostic.File.Length == 0)
        {
            return false;
        }

        try
        {
            return restored.Contains(Path.GetFullPath(diagnostic.File));
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static Dictionary<string, string> ReadUndecidedDependencyTypes(string outputDirectory)
    {
        var byName = new Dictionary<string, string>(StringComparer.Ordinal);
        var path = Path.Combine(outputDirectory, "unresolved-dependency-types.txt");
        if (!File.Exists(path))
        {
            return byName;
        }

        foreach (var line in File.ReadLines(path))
        {
            var tab = line.IndexOf('\t');
            if (tab > 0)
            {
                // First assembly wins; the name is only used to say which one to decide on.
                byName.TryAdd(line[(tab + 1)..], line[..tab]);
            }
        }
        return byName;
    }

    /// <summary>
    /// The undecided assembly this diagnostic is about, or null when it is not one.
    ///
    /// The names in the message are matched rather than the message parsed: compiler text is
    /// localized by the SDK, so anything that reads it in one language breaks in another.
    /// Only the codes that mean "this name does not exist" are considered - a missing type
    /// elsewhere in the file is a real error even if a dropped assembly also has that name.
    /// </summary>
    private static string? UndecidedDependency(
        Diagnostic diagnostic, IReadOnlyDictionary<string, string> undecidedTypes)
    {
        if (undecidedTypes.Count == 0
            || diagnostic.Code is not ("CS0246" or "CS0234" or "CS0012" or "CS1069" or "CS7069"))
        {
            return null;
        }

        foreach (Match quoted in QuotedName().Matches(diagnostic.Message))
        {
            if (undecidedTypes.TryGetValue(quoted.Groups[1].Value, out var assembly))
            {
                return assembly;
            }
        }
        return null;
    }

    /// <summary>
    /// The undecided assembly a follow-on error belongs to, judged by its FILE.
    ///
    /// Deliberately file-scoped and deliberately narrow: only the inheritance codes, and
    /// only in a file that already has an error naming a type from that assembly. A base
    /// class lives in one file with its subclass, so "this file could not find a type from
    /// X, and this file cannot satisfy a base contract" is one fact, not two. Any other
    /// error in the same file is still counted - a missing dependency does not excuse
    /// whatever else is wrong there.
    /// </summary>
    private static string? UndecidedBaseClass(
        Diagnostic diagnostic, IReadOnlyDictionary<string, string> undecidedByFile)
        => InheritanceContractCodes.Contains(diagnostic.Code)
           && undecidedByFile.TryGetValue(diagnostic.File, out var assembly)
            ? assembly
            : null;

    /// <summary>
    /// File -> the dependency a diagnostic in it already named.
    ///
    /// Both kinds count: a vendored DLL with no package chosen, AND a library whose public
    /// API still needs System.Web. DNN's DnnBodyProvider is the second - its base comes
    /// from ClientDependency.Core, which is built for .NET Framework, and the compiler
    /// says so as CS7069 ("'Control' is defined in System.Web"). The base is unusable
    /// either way, and the CS0534 / CS0115 that follow say nothing new.
    /// </summary>
    private static Dictionary<string, string> UndecidedAssemblyByFile(
        List<Diagnostic> diagnostics, IReadOnlyDictionary<string, string> undecidedTypes)
    {
        var byFile = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var diagnostic in diagnostics)
        {
            if ((UndecidedDependency(diagnostic, undecidedTypes) ?? NeedsSystemWeb(diagnostic))
                is { } assembly)
            {
                byFile.TryAdd(diagnostic.File, assembly);
            }
        }
        return byFile;
    }

    /// <summary>
    /// The System.Web assembly a REFERENCED LIBRARY still needs, or null.
    ///
    /// CS7069 and CS0012 are the compiler saying "an assembly you reference declares this
    /// member in terms of a type from an assembly that is not here". The converter never
    /// emits a reference to System.Web - removing it is the whole job - so when that
    /// assembly is System.Web, the dependency is a library the application brought with
    /// it. DNN's Dnn.ClientDependency package is built for .NET Framework and its
    /// WebFormsFileRegistrationProvider takes a System.Web.UI.Control.
    ///
    /// Nothing the converter does can fix that; a .NET build of the library has to be
    /// supplied. Counted separately for the same reason a vendored DLL with no
    /// replacement is, and reported so it cannot be mistaken for progress.
    /// </summary>
    private static string? NeedsSystemWeb(Diagnostic diagnostic)
    {
        if (diagnostic.Code is not ("CS7069" or "CS0012"))
        {
            return null;
        }

        foreach (Match quoted in QuotedName().Matches(diagnostic.Message))
        {
            var name = quoted.Groups[1].Value;
            if (name == "System.Web"
                || name.StartsWith("System.Web,", StringComparison.Ordinal)
                || name.StartsWith("System.Web.Services,", StringComparison.Ordinal)
                || name.StartsWith("System.Web.Extensions,", StringComparison.Ordinal))
            {
                return name.Split(',')[0];
            }
        }

        return null;
    }

    /// <summary>A name in single quotes, as every compiler locale writes identifiers.</summary>
    [GeneratedRegex(@"'([^']+)'")]
    private static partial Regex QuotedName();

    private const string ParseStopWarning = """
        > **この件数は下限です。**
        > 構文エラーがあるため C# コンパイラは意味解析を実行しておらず、型解決のエラーは
        > 1 件も報告されていません。構文エラーを解消すると件数は大幅に増える可能性があります。
        > 前後比較や合否判定にこの数値をそのまま使わないでください。
        """;

    private const string DependencyStopWarning = """
        > **この件数は下限です。参照しているプロジェクトが先に失敗しました。**
        > MSBuild は依存順にビルドし、失敗した枝から先へは進みません。つまり
        > **アプリケーション本体はコンパイルされておらず**、その中のエラーは 1 件も
        > 報告されていません。ライブラリ側のエラーを直すと件数は大幅に増えます。
        > 下の表はライブラリ側だけを見た数字です。
        """;

    public static int Run(string outputDirectory, string? reportPath)
    {
        var projectPath = Directory.EnumerateFiles(outputDirectory, "*.csproj").FirstOrDefault();
        if (projectPath is null)
        {
            Console.Error.WriteLine($"{outputDirectory} に .csproj が見つかりません。");
            return 1;
        }

        Console.WriteLine($"ビルド検証: {projectPath}");
        var (output, exitCode) = RunBuildPastUndecided(projectPath, outputDirectory, out var scaffolded);
        var diagnostics = Parse(output);

        reportPath ??= Path.Combine(outputDirectory, "BUILD-REPORT.md");

        // A failing build with nothing parsed means the verifier itself is broken (or the
        // build never ran). Reporting "success" there would be the worst possible outcome,
        // so surface the raw output instead.
        if (exitCode != 0 && diagnostics.Count == 0)
        {
            File.WriteAllText(reportPath, UnparsedReport(projectPath, exitCode, output));
            Console.Error.WriteLine(
                $"ビルドは失敗(終了コード {exitCode})しましたが、診断を解析できませんでした。レポート: {reportPath}");
            return 1;
        }

        var stoppedAtParse = StoppedAtParse(diagnostics, outputDirectory);
        var undecidedTypes = ReadUndecidedDependencyTypes(outputDirectory);
        var restored = ReadRestoredOutOfScopeFiles(outputDirectory);
        var undecidedByFile = UndecidedAssemblyByFile(diagnostics, undecidedTypes);
        var undecided = diagnostics
            .Select(diagnostic => (
                diagnostic,
                assembly: UndecidedDependency(diagnostic, undecidedTypes)
                          ?? UndecidedBaseClass(diagnostic, undecidedByFile)
                          ?? NeedsSystemWeb(diagnostic)))
            .Where(pair => pair.assembly is not null)
            .ToList();
        diagnostics = diagnostics
            .Where(diagnostic => UndecidedDependency(diagnostic, undecidedTypes) is null
                                 && UndecidedBaseClass(diagnostic, undecidedByFile) is null
                                 && NeedsSystemWeb(diagnostic) is null)
            .ToList();

        var outOfScope = diagnostics.Where(d => InRestoredOutOfScopeFile(d, restored)).ToList();
        diagnostics = diagnostics.Where(d => !InRestoredOutOfScopeFile(d, restored)).ToList();

        var report = BuildReport(projectPath, diagnostics, undecided!, outOfScope);
        if (scaffolded > 0)
        {
            // In the report as well as on the console, because the number below came from a
            // build the reader cannot reproduce with "dotnet build" here - the scaffold that
            // produced it has been deleted again.
            report =
                $"""
                > **この件数は、一時的な足場を置いて計測したものです。**
                > 置き換え先が未決の同梱 DLL の型のうち、コンパイラが「見つからない」と報告した
                > {scaffolded} 件を空の宣言で補ってからビルドしています。その名前は declaration パスで
                > ビルドを止めてしまい、**残りのエラーを全部隠していました**(mojoPortal はこれで
                > 「6 件」と報告し続けていました。実際は 2200 件超です)。
                > 足場は計測後に削除済みで、出力には含まれていません。`--package-map` を埋めれば
                > 本物のライブラリが入り、足場は不要になります。

                """ + Environment.NewLine + report;
        }
        // Which of the two ways the count became a floor, because the answer decides where
        // to look. "Fix the syntax error" is useless advice when the application simply was
        // not built.
        var dependencyFailed = DependencyProjectFailed(diagnostics, outputDirectory);
        if (stoppedAtParse)
        {
            // Blank line between: a block quote running straight into the "#" heading would
            // swallow it into the quote.
            report = (dependencyFailed ? DependencyStopWarning : ParseStopWarning)
                + Environment.NewLine + Environment.NewLine + report;
        }
        File.WriteAllText(reportPath, report);
        WriteFullErrorList(outputDirectory, diagnostics);

        Console.WriteLine($"エラー {diagnostics.Count} 件(うち連鎖 {diagnostics.Count(d => IsCascade(d))} 件)");
        if (undecided.Count > 0)
        {
            Console.WriteLine(
                $"別に、未決の依存(package-map 未指定)によるエラーが {undecided.Count} 件あります"
                + "(変換の欠陥ではないため件数に含めていません)。");
        }
        if (outOfScope.Count > 0)
        {
            Console.WriteLine(
                $"別に、スコープ外(MVC 等)のまま復元したファイル内のエラーが {outOfScope.Count} 件あります"
                + "(除外の連鎖を避けるために残したファイルで、件数に含めていません)。");
        }
        if (scaffolded > 0)
        {
            // Said out loud because the count came from a build the user cannot reproduce
            // by running dotnet build on this directory - the scaffold is gone again.
            Console.WriteLine(
                $"注記: 置き換え先が未決の同梱 DLL の型 {scaffolded} 件を一時的に宣言して計測しました"
                + "(その名前が declaration パスでビルドを止め、残りのエラーを隠していたため)。"
                + "一時ファイルは削除済みです。");
        }
        if (dependencyFailed)
        {
            // The most convincing wrong number this tool can produce: a small count from a
            // build in which the application was never compiled at all.
            Console.WriteLine(
                "警告: 参照しているプロジェクトが先に失敗したため、アプリケーション本体は"
                + "コンパイルされていません。上の件数はライブラリ側だけの下限であり、総数ではありません。");
        }
        else if (stoppedAtParse)
        {
            // Without this the number reads as "almost building" when the truth is the
            // opposite: the compiler gave up before it ever looked at any type.
            Console.WriteLine(
                "警告: 構文エラーがあるため意味解析が実行されていません。上の件数は下限であり、"
                + "総数ではありません。構文エラーを直すと件数は大幅に増える可能性があります。");
        }
        Console.WriteLine($"レポート: {reportPath}");
        return diagnostics.Count == 0 ? 0 : 2;
    }

    private static (string Output, int ExitCode) RunBuild(string projectPath)
    {
        // --no-incremental is mandatory, not an optimisation trade-off. A stale obj/ (left by a
        // killed or interrupted build) makes an incremental build report far fewer errors than a
        // clean one - this project once reported "391 -> 39 -> 3 -> 1" only for a clean build to
        // produce 705. The AI residual layer accepts or rolls back a candidate answer purely on
        // whether this count went up, so an under-count here silently admits broken code.
        var process = new Process
        {
            StartInfo = new ProcessStartInfo("dotnet", $"build \"{projectPath}\" --no-incremental --nologo -v q")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            },
        };

        // Async reads: a large build easily fills a pipe buffer, and reading one stream to
        // the end while the other fills up deadlocks the child
        var captured = new StringBuilder();
        process.OutputDataReceived += (_, e) => AppendLine(captured, e.Data);
        process.ErrorDataReceived += (_, e) => AppendLine(captured, e.Data);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        return (captured.ToString(), process.ExitCode);
    }

    private static void AppendLine(StringBuilder builder, string? line)
    {
        if (line is not null)
        {
            lock (builder)
            {
                builder.AppendLine(line);
            }
        }
    }

    /// <summary>
    /// Lists the errors that are waiting on a package choice, kept out of the totals above.
    /// Reported rather than hidden: they disappear the moment --package-map names a
    /// replacement, and the reader is the one who has to name it.
    /// </summary>
    private static void AppendUndecidedDependencies(
        StringBuilder builder, List<(Diagnostic Diagnostic, string Assembly)> undecided)
    {
        if (undecided.Count == 0)
        {
            return;
        }

        builder.AppendLine("## 未決の依存によるエラー(件数に含めていません)");
        builder.AppendLine();
        builder.AppendLine($"**{undecided.Count} 件**は、参照ライブラリ側の都合で型が見つからないものです。");
        builder.AppendLine("変換の欠陥ではありません。");
        builder.AppendLine();
        builder.AppendLine("- リポジトリ同梱 DLL の置き換え先が未決定: `package-map.template.json` に");
        builder.AppendLine("  パッケージを書いて `--package-map` で再変換すれば解消します。");
        builder.AppendLine("- `System.Web` を必要とするライブラリ: **公開 API が System.Web の型を");
        builder.AppendLine("  含んでいます。** 変換器にできることはなく、.NET 向けにビルドされた");
        builder.AppendLine("  そのライブラリを用意してもらう必要があります。");
        builder.AppendLine();
        builder.AppendLine("| アセンブリ | 件数 |");
        builder.AppendLine("| --- | ---: |");
        foreach (var group in undecided
                     .GroupBy(pair => pair.Assembly, StringComparer.Ordinal)
                     .OrderByDescending(group => group.Count()))
        {
            builder.AppendLine($"| `{group.Key}` | {group.Count()} |");
        }
        builder.AppendLine();
    }

    /// <summary>
    /// Errors inside files the converter restored from an out-of-scope exclusion. Same
    /// treatment as an undecided dependency, for the same reason: reported in full, and
    /// not counted as conversion defects, because the trade that produced them was made
    /// and reported deliberately.
    /// </summary>
    private static void AppendRestoredOutOfScope(StringBuilder builder, List<Diagnostic> outOfScope)
    {
        if (outOfScope.Count == 0)
        {
            return;
        }

        builder.AppendLine("## スコープ外のまま復元したファイル内のエラー(件数に含めていません)");
        builder.AppendLine();
        builder.AppendLine($"**{outOfScope.Count} 件**は、別フレームワーク(MVC / Web API / Identity 等)の");
        builder.AppendLine("コードとして移植対象外にしたものの、**移植されるファイルがその型を使っているため**");
        builder.AppendLine("残したファイルの中にあります。");
        builder.AppendLine();
        builder.AppendLine("除外すると連鎖でそちら側が落ちるので残す、という判断の代償で、");
        builder.AppendLine("**そのファイル内に未解決の型が残ることは変換時点で報告済み**です");
        builder.AppendLine("(各ファイルの残差を参照)。変換器がこれを消す方法はありません。");
        builder.AppendLine();
        builder.AppendLine("| ファイル | 件数 |");
        builder.AppendLine("| --- | ---: |");
        foreach (var group in outOfScope
                     .GroupBy(diagnostic => Path.GetFileName(diagnostic.File), StringComparer.Ordinal)
                     .OrderByDescending(group => group.Count()))
        {
            builder.AppendLine($"| `{group.Key}` | {group.Count()} |");
        }
        builder.AppendLine();
    }

    private static string UnparsedReport(string projectPath, int exitCode, string output)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# ビルド検証レポート");
        builder.AppendLine();
        builder.AppendLine($"対象: `{projectPath}`");
        builder.AppendLine();
        builder.AppendLine($"**ビルドは失敗しました(終了コード {exitCode})が、診断を1件も解析できませんでした。**");
        builder.AppendLine();
        builder.AppendLine("検証ツール側の問題の可能性があります。以下は生の出力(末尾 100 行)です。");
        builder.AppendLine();
        builder.AppendLine("```");
        foreach (var line in output.Split('\n').TakeLast(100))
        {
            builder.AppendLine(line.TrimEnd('\r'));
        }
        builder.AppendLine("```");
        return builder.ToString();
    }

    private static List<Diagnostic> Parse(string buildOutput)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var diagnostics = new List<Diagnostic>();

        foreach (Match match in DiagnosticRegex().Matches(buildOutput))
        {
            var file = match.Groups["file"].Value;
            var line = int.Parse(match.Groups["line"].Value);
            var code = match.Groups["code"].Value;
            var message = match.Groups["message"].Value.Trim();

            // MSBuild repeats diagnostics once per target framework / project pass
            if (seen.Add($"{file}|{line}|{code}|{message}"))
            {
                diagnostics.Add(new Diagnostic(file, line, code, message));
            }
        }
        return diagnostics;
    }

    private static bool IsCascade(Diagnostic diagnostic) => diagnostic.Code == "CS1662";

    private static string BuildReport(
        string projectPath,
        List<Diagnostic> diagnostics,
        List<(Diagnostic Diagnostic, string Assembly)> undecidedDependencies,
        List<Diagnostic> restoredOutOfScope)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# ビルド検証レポート");
        builder.AppendLine();
        builder.AppendLine($"対象: `{projectPath}`");
        builder.AppendLine();

        if (diagnostics.Count == 0)
        {
            builder.AppendLine(undecidedDependencies.Count == 0 && restoredOutOfScope.Count == 0
                ? "**ビルド成功(エラー 0 件)**。"
                : "**変換側のエラーは 0 件です。**");
            builder.AppendLine();
            AppendUndecidedDependencies(builder, undecidedDependencies);
            AppendRestoredOutOfScope(builder, restoredOutOfScope);
            builder.AppendLine("注意: ビルドが通ることと動作が一致することは別です。");
            builder.AppendLine("実際の描画一致は ParityTest(旧アプリのゴールデンマスター照合)で確認してください。");
            return builder.ToString();
        }

        AppendUndecidedDependencies(builder, undecidedDependencies);
        AppendRestoredOutOfScope(builder, restoredOutOfScope);

        var primary = diagnostics.Where(diagnostic => !IsCascade(diagnostic)).ToList();
        builder.AppendLine("## サマリー");
        builder.AppendLine();
        builder.AppendLine("| 項目 | 件数 |");
        builder.AppendLine("| --- | ---: |");
        builder.AppendLine($"| エラー合計 | {diagnostics.Count} |");
        builder.AppendLine($"| 一次エラー | {primary.Count} |");
        builder.AppendLine($"| 連鎖(二次症状) | {diagnostics.Count - primary.Count} |");
        builder.AppendLine($"| 影響ファイル数 | {diagnostics.Select(d => d.File).Distinct().Count()} |");
        builder.AppendLine();

        builder.AppendLine("## 層別(エラーが出たファイルの種類)");
        builder.AppendLine();
        builder.AppendLine("| 層 | 件数 | 意味 |");
        builder.AppendLine("| --- | ---: | --- |");
        foreach (var group in primary.GroupBy(d => d.Kind).OrderByDescending(g => g.Count()))
        {
            builder.AppendLine($"| {DescribeKind(group.Key)} | {group.Count()} | {ExplainKind(group.Key)} |");
        }
        builder.AppendLine();

        builder.AppendLine("## 推定原因別");
        builder.AppendLine();
        builder.AppendLine("| 推定原因 | 件数 | 主なコード |");
        builder.AppendLine("| --- | ---: | --- |");
        foreach (var group in diagnostics
                     .GroupBy(diagnostic => KnownCauses.GetValueOrDefault(diagnostic.Code, "未分類"))
                     .OrderByDescending(group => group.Count()))
        {
            var codes = string.Join(", ", group.Select(d => d.Code).Distinct().Take(6));
            builder.AppendLine($"| {group.Key} | {group.Count()} | {codes} |");
        }
        builder.AppendLine();

        builder.AppendLine("## エラーコード別(上位20)");
        builder.AppendLine();
        builder.AppendLine("| コード | 件数 | 代表メッセージ |");
        builder.AppendLine("| --- | ---: | --- |");
        foreach (var group in diagnostics.GroupBy(d => d.Code).OrderByDescending(g => g.Count()).Take(20))
        {
            builder.AppendLine($"| {group.Key} | {group.Count()} | {Truncate(group.First().Message)} |");
        }
        builder.AppendLine();

        builder.AppendLine("## エラーの多いファイル(上位20)");
        builder.AppendLine();
        builder.AppendLine("| ファイル | 件数 | 先頭のエラー |");
        builder.AppendLine("| --- | ---: | --- |");
        foreach (var group in diagnostics.GroupBy(d => d.File).OrderByDescending(g => g.Count()).Take(20))
        {
            var first = group.OrderBy(d => d.Line).First();
            builder.AppendLine(
                $"| `{Path.GetFileName(group.Key)}` | {group.Count()} | {first.Code} ({first.Line}行) |");
        }
        builder.AppendLine();

        builder.AppendLine("## 読み方");
        builder.AppendLine();
        builder.AppendLine("- **連鎖**: Razor のテンプレートはラムダにコンパイルされるため、本体の式が壊れると");
        builder.AppendLine("  `CS1662 ラムダ変換不可` としても報告されます。単独では発生せず、原因を直すと連動して消えます。");
        builder.AppendLine("- **生成 Razor** で出るエラーは変換器自身の出力の誤りで、決定的変換で直せる可能性が高い箇所です。");
        builder.AppendLine("- **移植ライブラリ** で出るエラーは .NET に存在しない依存が主因で、除外や手動移行の判断が要ります。");
        builder.AppendLine("- ビルドが通っても描画が一致するとは限りません。合否判定は ParityTest を最終ゲートにしてください。");

        return builder.ToString();
    }

    private static string DescribeKind(FileKind kind) => kind switch
    {
        FileKind.GeneratedMarkup => "生成 Razor",
        FileKind.PortedCodeBehind => "移植コードビハインド",
        _ => "移植ライブラリ",
    };

    private static string ExplainKind(FileKind kind) => kind switch
    {
        FileKind.GeneratedMarkup => "変換器が書いた出力 = 変換器側で直せる",
        FileKind.PortedCodeBehind => "互換シムの不足が主因",
        _ => ".NET に無い依存が主因(除外/手動移行の判断)",
    };

    private static string Truncate(string value)
        => value.Length <= 70 ? value : value[..70] + "…";

    // The trailing "\r?" matters: with CRLF output, "$" in multiline mode only matches
    // before "\n", so anchoring straight to "$" never matches a single line
    [GeneratedRegex(@"^(?<file>[^\r\n(]+)\((?<line>\d+),\d+\):\s*error\s+(?<code>\w+):\s*(?<message>[^\r\n]*)\r?$",
        RegexOptions.Multiline)]
    private static partial Regex DiagnosticRegex();
}
