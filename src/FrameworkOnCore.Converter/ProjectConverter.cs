using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FrameworkOnCore.Converter;

/// <summary>A project as converted: its file in the output tree and its conditional compilation symbols.</summary>
public sealed record ConvertedProject(string Name, string SourcePath, string TargetPath, bool IsWeb, IReadOnlyList<string> Defines);

/// <summary>Where the runtime pieces the converted projects refer to are (the fork's feed, the shims).</summary>
public sealed record RuntimeLayout(string Feed, IReadOnlyList<string> ShimProjects);

/// <summary>
/// Turns a .NET Framework web project - and the projects it references - into .NET 10 projects on
/// WebFormsForCore, keeping every source file as it is. The repository is copied as a whole and the
/// project files are rewritten where they are.
/// </summary>
public sealed class ProjectConverter(Rules rules, Report report, Conditions conditions, string sourceRoot, string outRoot, RuntimeLayout runtime)
{
    static readonly XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";

    // Every converted project: new vulnerability advisories on old package versions are warnings, not
    // the errors TreatWarningsAsErrors would make them (the original built before they were published).
    const string NoWarn = "$(NoWarn);NU1701;NU1603;NU1608;SYSLIB0011";
    const string WarningsNotAsErrors = "$(WarningsNotAsErrors);NU1901;NU1902;NU1903;NU1904";

    // WebFormsForCore's packages carry System.Web; the one in .NET's own reference set is removed.
    // CoreWCF (under WebFormsForCore.Web.Extensions) brings System.Web.Services.Description, whose WSDL
    // types (WsiProfiles, ServiceDescription, ...) System.Web.Services has too (CS0433).
    const string AliasTarget = """
          <Target Name="ChangeAliasesOfNugetRefs" BeforeTargets="FindReferenceAssembliesForReferences;ResolveReferences">
            <ItemGroup>
              <ReferencePath Remove="%(Identity)" Condition="'%(FileName)' == 'System.Web' AND $([System.Text.RegularExpressions.Regex]::IsMatch(%(Identity),'[/\x5C]dotnet[/\x5C]'))" />
              <ReferencePath Remove="%(Identity)" Condition="'%(FileName)' == 'System.Web.Services.Description'" />
            </ItemGroup>
          </Target>
    """;

    // The symbols the .NET SDK defines for net10.0 (and the Debug configuration), for reading sources
    // the way the compiler does.
    public static readonly IReadOnlyList<string> SdkSymbols = new[] { "NET", "NETCOREAPP", "NET10_0", "DEBUG", "TRACE" }
        .Concat(new[] { "5_0", "6_0", "7_0", "8_0", "9_0", "10_0" }.Select(v => $"NET{v}_OR_GREATER"))
        .Concat(new[] { "1_0", "1_1", "2_0", "2_1", "2_2", "3_0", "3_1" }.Select(v => $"NETCOREAPP{v}_OR_GREATER")).ToList();

    readonly Dictionary<string, ConvertedProject> converted = new(StringComparer.OrdinalIgnoreCase);
    Dictionary<string, string>? assemblyProjects;

    public IReadOnlyCollection<ConvertedProject> Converted => converted.Values;

    public string TargetOf(string sourcePath) => Path.Combine(outRoot, Path.GetRelativePath(sourceRoot, sourcePath));

    public ConvertedProject Convert(string projectPath, bool isWeb)
    {
        projectPath = Path.GetFullPath(projectPath);
        if (converted.TryGetValue(projectPath, out var done)) return done;
        var name = Path.GetFileNameWithoutExtension(projectPath);
        // Registered before its references are converted: a cycle stops here.
        converted[projectPath] = new ConvertedProject(name, projectPath, TargetOf(projectPath), isWeb, Array.Empty<string>());

        var document = XDocument.Load(projectPath, LoadOptions.PreserveWhitespace);
        var isSdk = document.Root!.Attribute("Sdk") != null;

        var references = new List<string>();
        foreach (var reference in Elements(document, "ProjectReference"))
        {
            if (!isSdk && !ItemHolds(reference, name)) continue;
            var referenced = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectPath)!, (string)reference.Attribute("Include")!));
            if (!referenced.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)) { report.Add(Report.Kind.Unsupported, name, $"project reference not converted (not C#): {referenced}"); continue; }
            if (!File.Exists(referenced)) { report.Add(Report.Kind.Error, name, $"project reference not found: {referenced}"); continue; }
            Convert(referenced, false);
            references.Add(referenced);
        }

        var result = isSdk ? ConvertSdk(projectPath, name, document) : ConvertOld(projectPath, name, document, isWeb, references);
        converted[projectPath] = result;
        return result;
    }

    static IEnumerable<XElement> Elements(XDocument document, string localName) =>
        document.Descendants().Where(e => e.Name.LocalName == localName).ToList();

    bool ItemHolds(XElement item, string subject) =>
        conditions.Holds((string?)item.Attribute("Condition"), subject) &&
        conditions.Holds((string?)item.Parent?.Attribute("Condition"), subject);

    // ------------------------------------------------------------------------------------------
    // Old-style (.NET Framework) projects: a new SDK-style project file with the original's items.

    ConvertedProject ConvertOld(string projectPath, string name, XDocument old, bool isWeb, List<string> projectReferences)
    {
        var source = Path.GetDirectoryName(projectPath)!;
        var targetPath = TargetOf(projectPath);
        var target = Path.GetDirectoryName(targetPath)!;
        string? Property(string property) => old.Descendants(msbuild + property).FirstOrDefault()?.Value;

        // What the original compiled and embedded, in its order (linked files keep their paths: the
        // repository is copied as a whole).
        var compile = old.Descendants(msbuild + "Compile").Where(e => ItemHolds(e, name)).Select(e => (string)e.Attribute("Include")!).ToList();
        var embedded = old.Descendants(msbuild + "EmbeddedResource").Where(e => ItemHolds(e, name)).Select(e => (string)e.Attribute("Include")!).ToList();
        var assemblyName = Property("AssemblyName") ?? name;
        var rootNamespace = Property("RootNamespace") ?? assemblyName;
        // Conditional compilation symbols of the configuration built (NET_4_0 and the like select code).
        var defines = old.Descendants(msbuild + "DefineConstants")
            .Where(d => conditions.Holds((string?)d.Parent?.Attribute("Condition"), name))
            .SelectMany(d => d.Value.Split(';', ','))
            .Select(s => s.Trim()).Where(s => s.Length > 0 && !s.Contains('$')).Distinct().ToList();

        var packages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        void AddPackage(Package p) => packages.TryAdd(p.Id, p.Version);
        void AddListed(string id, string version)
        {
            if (rules.IsDropped(id, version)) return;
            var package = rules.ReplacedPackages.TryGetValue(id, out var replaced) ? replaced : new Package(id, version);
            packages[package.Id] = package.Version;
        }
        var config = Path.Combine(source, "packages.config");
        if (File.Exists(config))
        {
            foreach (var p in XDocument.Load(config).Descendants("package")) AddListed((string)p.Attribute("id")!, (string)p.Attribute("version")!);
        }
        foreach (var p in old.Descendants(msbuild + "PackageReference").Where(e => ItemHolds(e, name)))
        {
            AddListed((string)p.Attribute("Include")!, (string?)p.Attribute("Version") ?? p.Element(msbuild + "Version")?.Value ?? "*");
        }
        if (isWeb) foreach (var id in rules.WebPackages) AddPackage(new Package(id, rules.ForkVersion));

        var binaryReferences = new List<(string Assembly, string HintPath, string? Aliases)>();
        var builtReferences = new List<string>();
        foreach (var reference in old.Descendants(msbuild + "Reference").Where(e => ItemHolds(e, name)))
        {
            var assembly = ((string)reference.Attribute("Include")!).Split(',')[0].Trim();
            var hint = reference.Element(msbuild + "HintPath")?.Value;
            if (hint != null)
            {
                // A package's DLL is covered by the PackageReference.
                if (Regex.IsMatch(hint, @"(^|\\)packages\\", RegexOptions.IgnoreCase)) continue;
                var dll = Path.GetFullPath(Path.Combine(source, hint));
                // Another project's build output: that project, by reference.
                var producer = ProjectOfAssembly(assembly);
                if (producer != null && (Regex.IsMatch(hint, @"(^|\\)bin\\", RegexOptions.IgnoreCase) || !File.Exists(dll)))
                {
                    if (!producer.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                    {
                        report.Add(Report.Kind.Unsupported, name, $"{assembly} is built by {Path.GetFileName(producer)}: not converted (only C# projects so far); the code using it is stubbed");
                        continue;
                    }
                    Convert(producer, false);
                    builtReferences.Add(producer);
                    continue;
                }
                // As MSBuild does with a HintPath to nothing (warning MSB3245): the reference is left out.
                if (!File.Exists(dll)) { report.Add(Report.Kind.Project, name, $"{assembly}: HintPath to a file that is not there ({hint}), left out as MSBuild does"); continue; }
                // A DLL checked into the repository: referenced where it is.
                binaryReferences.Add((assembly, hint, reference.Element(msbuild + "Aliases")?.Value));
            }
            else if (rules.FrameworkReferences.TryGetValue(assembly, out var package))
            {
                AddPackage(package);
                if (rules.FrameworkCompanions.TryGetValue(assembly, out var companions)) foreach (var c in companions) AddPackage(c);
            }
            else if (rules.NoAnswer.Contains(assembly))
            {
                report.Add(Report.Kind.Unsupported, name, $"{assembly} has no .NET counterpart");
            }
        }

        // The assemblies web.config names for page compilation are loaded by name at run time; the ones
        // .NET does not ship need their package, or the configuration itself fails.
        var webConfig = isWeb ? Directory.EnumerateFiles(source, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault() : null;
        if (webConfig != null)
        {
            try
            {
                var configDocument = XDocument.Load(webConfig, LoadOptions.PreserveWhitespace);
                var removed = false;
                // Wherever system.web is (<location path="."> too, as mojoPortal has it).
                foreach (var add in configDocument.Descendants().Where(e => e.Name.LocalName == "add" &&
                             e.Parent?.Name.LocalName == "assemblies" && e.Parent.Parent?.Name.LocalName == "compilation").ToList())
                {
                    var assembly = ((string?)add.Attribute("assembly") ?? "").Split(',')[0].Trim();
                    if (rules.FrameworkReferences.TryGetValue(assembly, out var package)) AddPackage(package);
                    // An assembly .NET has no counterpart for fails the whole configuration (every page) when
                    // it is loaded for page compilation: left out of the converted web.config (the original
                    // is unchanged). Pages using its types do not compile. A compatibility assembly will
                    // take its place (LINUX-CONVERTER-DESIGN.md §8).
                    if (rules.NoAnswer.Contains(assembly))
                    {
                        add.Remove();
                        removed = true;
                        report.Add(Report.Kind.Unsupported, name, $"web.config <compilation><assemblies>: {assembly} left out (no .NET counterpart; pages using it do not compile)");
                    }
                }
                if (removed) configDocument.Save(TargetOf(webConfig), SaveOptions.DisableFormatting);
            }
            catch (System.Xml.XmlException e) { report.Add(Report.Kind.Error, name, $"web.config not read: {e.Message}"); }
        }

        AddSourcePackages(name, compile.Select(c => Path.Combine(source, c)), AddPackage);
        var usesWebFormsForCore = isWeb || packages.Keys.Any(k => k.StartsWith("WebFormsForCore.", StringComparison.OrdinalIgnoreCase));

        // The project file.
        var text = new StringBuilder();
        text.Append($"<Project Sdk=\"{(isWeb ? "Microsoft.NET.Sdk.Web" : "Microsoft.NET.Sdk")}\">\n");
        text.Append($"  <!-- Converted by FrameworkOnCore from the .NET Framework project; the sources are the original's. -->\n");
        text.Append("  <PropertyGroup>\n");
        void Prop(string p, string v) => text.Append($"    <{p}>{SecurityElement.Escape(v)}</{p}>\n");
        Prop("TargetFramework", "net10.0");
        Prop("AssemblyName", assemblyName);
        Prop("RootNamespace", rootNamespace);
        Prop("Nullable", "disable");
        Prop("ImplicitUsings", "disable");
        Prop("EnableDefaultCompileItems", "false");
        Prop("EnableDefaultEmbeddedResourceItems", "false");
        Prop("GenerateAssemblyInfo", "false");
        Prop("NoWarn", NoWarn);
        Prop("WarningsNotAsErrors", WarningsNotAsErrors);
        if (defines.Count > 0) Prop("DefineConstants", "$(DefineConstants);" + string.Join(';', defines));
        Prop("RestoreAdditionalProjectSources", Paths.FromProject(target, runtime.Feed));
        if (isWeb)
        {
            Prop("OutputType", "Exe");
            // bin, as on .NET Framework: applications look for their assemblies in ~/bin.
            Prop("OutputPath", "bin");
            Prop("AppendTargetFrameworkToOutputPath", "false");
            Prop("AppendRuntimeIdentifierToOutputPath", "false");
            Prop("IntermediateOutputPath", "$(BaseIntermediateOutputPath)$(Configuration)\\$(TargetFramework.ToLowerInvariant())\\");
            Prop("StartupObject", "Program");
            Prop("EnableDefaultContentItems", "false");
        }
        text.Append("  </PropertyGroup>\n\n");

        if (isWeb)
        {
            text.Append("""
                  <ItemGroup>
                    <Content Remove="bin\**\*.*" />
                    <None Remove="bin\**\*.*" />
                    <Compile Remove="bin\**\*.*" />
                    <Content Include="**\*.aspx;**\*.ascx;**\*.master;**\*.Master;**\*.ashx;**\*.asmx;Web.config" Exclude="bin\**" CopyToOutputDirectory="Never" />
                  </ItemGroup>


                """);
        }

        text.Append("  <ItemGroup>\n");
        foreach (var c in compile) text.Append($"    <Compile Include=\"{SecurityElement.Escape(c)}\" />\n");
        if (isWeb) text.Append("    <Compile Include=\"Program.cs\" />\n");
        foreach (var e in embedded) text.Append($"    <EmbeddedResource Include=\"{SecurityElement.Escape(e)}\" />\n");
        text.Append("  </ItemGroup>\n\n  <ItemGroup>\n");
        foreach (var (id, version) in packages) text.Append($"    <PackageReference Include=\"{id}\" Version=\"{version}\" />\n");
        foreach (var (assembly, hint, referenceAliases) in binaryReferences)
        {
            var aliasAttribute = string.IsNullOrWhiteSpace(referenceAliases) ? "" : $" Aliases=\"{SecurityElement.Escape(referenceAliases)}\"";
            text.Append($"    <Reference Include=\"{SecurityElement.Escape(assembly)}\" HintPath=\"{SecurityElement.Escape(hint)}\"{aliasAttribute} />\n");
        }
        // Analyzers and source generators referenced as projects (DNN generates the defining parts of
        // partial methods, CS0759 without it): with their metadata, as the original has them.
        var analyzers = old.Descendants(msbuild + "ProjectReference")
            .Where(r => string.Equals((string?)r.Attribute("OutputItemType"), "Analyzer", StringComparison.OrdinalIgnoreCase))
            .Select(r => Path.GetFullPath(Path.Combine(source, (string)r.Attribute("Include")!)))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        // extern alias names on project references (Aliases), as the original has them.
        var aliases = old.Descendants(msbuild + "ProjectReference")
            .Select(r => (Path: Path.GetFullPath(Path.Combine(source, (string)r.Attribute("Include")!)), Aliases: r.Element(msbuild + "Aliases")?.Value ?? (string?)r.Attribute("Aliases")))
            .Where(r => !string.IsNullOrWhiteSpace(r.Aliases)).ToDictionary(r => r.Path, r => r.Aliases!, StringComparer.OrdinalIgnoreCase);
        foreach (var referenced in projectReferences.Concat(builtReferences).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var metadata = analyzers.Contains(referenced) ? " ReferenceOutputAssembly=\"false\" OutputItemType=\"Analyzer\"" : "";
            if (aliases.TryGetValue(referenced, out var alias)) metadata += $" Aliases=\"{SecurityElement.Escape(alias)}\"";
            text.Append($"    <ProjectReference Include=\"{SecurityElement.Escape(Paths.FromProject(source, referenced))}\"{metadata} />\n");
        }
        // Analyzers checked into the repository (not the ones of packages: the PackageReference brings them).
        foreach (var analyzer in old.Descendants(msbuild + "Analyzer").Where(e => ItemHolds(e, name)).Select(e => (string)e.Attribute("Include")!))
        {
            if (Regex.IsMatch(analyzer, @"(^|\\)packages\\", RegexOptions.IgnoreCase) || !File.Exists(Path.Combine(source, analyzer))) continue;
            text.Append($"    <Analyzer Include=\"{SecurityElement.Escape(analyzer)}\" />\n");
        }
        text.Append("  </ItemGroup>\n");

        if (isWeb)
        {
            text.Append("\n  <!-- Assemblies .NET Framework had and .NET does not, referenced by .NET Framework packages. -->\n  <ItemGroup>\n");
            foreach (var shim in runtime.ShimProjects) text.Append($"    <ProjectReference Include=\"{SecurityElement.Escape(Paths.FromProject(target, shim))}\" />\n");
            text.Append("  </ItemGroup>\n");
            text.Append("""

                  <!-- Culture data from Windows (NLS), as on .NET Framework; on Linux the ICU data are made the
                       original server's instead (culture-profile.json). -->
                  <ItemGroup>
                    <RuntimeHostConfigurationOption Include="System.Globalization.UseNls" Value="true" />
                  </ItemGroup>

                """);
        }
        if (usesWebFormsForCore) text.Append('\n').Append(AliasTarget).Append('\n');
        text.Append("</Project>\n");
        File.WriteAllText(targetPath, text.ToString(), new UTF8Encoding(false));

        report.Add(Report.Kind.Project, name, $"{compile.Count} files, {packages.Count} packages, {binaryReferences.Count} DLLs -> {Path.GetRelativePath(outRoot, targetPath)}");
        return new ConvertedProject(name, projectPath, targetPath, isWeb, defines);
    }

    // ------------------------------------------------------------------------------------------
    // SDK-style projects: one target framework, net10.0 - the application runs on .NET 10 only, and a
    // netstandard target next to it could not reference the projects that became net10.0 (NU1201).
    // Conditions on the .NET Framework target are rewritten to it. Framework references become packages.

    ConvertedProject ConvertSdk(string projectPath, string name, XDocument project)
    {
        var targetPath = TargetOf(projectPath);
        var target = Path.GetDirectoryName(targetPath)!;
        var root = project.Root!;
        XName N(string local) => root.Name.Namespace + local;

        // Analyzers and source generators run in the compiler: netstandard2.0, as they are (RS1041).
        if (Elements(project, "PackageReference").Any(p => ((string?)p.Attribute("Include") ?? "").StartsWith("Microsoft.CodeAnalysis", StringComparison.OrdinalIgnoreCase)))
        {
            report.Add(Report.Kind.Project, name, "analyzer / source generator: kept as it is");
            return new ConvertedProject(name, projectPath, targetPath, false, Array.Empty<string>());
        }

        var frameworks = new List<string>();
        foreach (var node in Elements(project, "TargetFramework").Concat(Elements(project, "TargetFrameworks")))
        {
            frameworks.AddRange(node.Value.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
            node.Value = "net10.0";
        }
        var netfx = frameworks.Where(f => Regex.IsMatch(f, @"^net4\d*$")).Distinct().ToList();
        foreach (var element in project.Descendants().Where(e => e.Attribute("Condition") != null))
        {
            var condition = (string)element.Attribute("Condition")!;
            var rewritten = netfx.Aggregate(condition, (c, f) => c.Replace($"'{f}'", "'net10.0'")).Replace("'.NETFramework'", "'.NETCoreApp'");
            if (rewritten != condition) element.SetAttributeValue("Condition", rewritten);
        }
        var dropped = frameworks.Where(f => !netfx.Contains(f) && f != "net10.0").Distinct().ToList();
        if (dropped.Count > 0) report.Add(Report.Kind.Project, name, $"target frameworks {string.Join(';', frameworks)} -> net10.0 (items conditioned on {string.Join(", ", dropped)} no longer apply)");

        bool HasPackage(string id) => Elements(project, "PackageReference").Any(p => string.Equals((string?)p.Attribute("Include"), id, StringComparison.OrdinalIgnoreCase));
        XElement PackageElement(Package p) => new(N("PackageReference"), new XAttribute("Include", p.Id), new XAttribute("Version", p.Version));

        foreach (var reference in Elements(project, "Reference"))
        {
            if (reference.Elements().Any(e => e.Name.LocalName == "HintPath")) continue;
            var assembly = ((string?)reference.Attribute("Include") ?? "").Split(',')[0].Trim();
            // In the reference's place: its ItemGroup's condition stays with it.
            if (rules.FrameworkReferences.TryGetValue(assembly, out var package) && !HasPackage(package.Id)) reference.AddBeforeSelf(PackageElement(package));
            else if (rules.NoAnswer.Contains(assembly)) report.Add(Report.Kind.Unsupported, name, $"{assembly} has no .NET counterpart");
            reference.Remove();
        }
        foreach (var reference in Elements(project, "PackageReference"))
        {
            var id = (string?)reference.Attribute("Include") ?? "";
            if (rules.DroppedPackages.Contains(id)) { reference.Remove(); continue; }
            if (rules.ReplacedPackages.TryGetValue(id, out var replaced))
            {
                reference.SetAttributeValue("Include", replaced.Id);
                if (reference.Attribute("Version") != null) reference.SetAttributeValue("Version", replaced.Version);
            }
        }
        // Deployment steps copying build output into the site (XCOPY ...): Windows commands, and the
        // converted projects reach each other by ProjectReference.
        foreach (var exec in Elements(project, "Target").Where(t => t.Elements().Any(e => e.Name.LocalName == "Exec")))
        {
            report.Add(Report.Kind.Project, name, $"target '{(string?)exec.Attribute("Name")}' (Exec) removed");
            exec.Remove();
        }

        var added = new ItemsToAdd();
        var directory = Path.GetDirectoryName(projectPath)!;
        var sources = Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
            .Where(f => !Regex.IsMatch(Path.GetRelativePath(directory, f), @"(^|[\\/])(bin|obj)[\\/]", RegexOptions.IgnoreCase));
        AddSourcePackages(name, sources, p => { if (!HasPackage(p.Id)) added.Add(p); });
        var itemGroup = new XElement(N("ItemGroup"), added.Items.Select(PackageElement));
        if (itemGroup.HasElements) root.Add(itemGroup);
        root.Add(new XElement(N("PropertyGroup"),
            new XElement(N("NoWarn"), NoWarn),
            new XElement(N("WarningsNotAsErrors"), WarningsNotAsErrors),
            // The converted code is not the original: the build's edits and .NET's obsoletions (SYSLIB) stay
            // warnings (the SYSLIB ones are reported).
            new XElement(N("TreatWarningsAsErrors"), "false"),
            new XElement(N("RestoreAdditionalProjectSources"), Paths.FromProject(target, runtime.Feed))));
        if (Elements(project, "PackageReference").Any(p => ((string?)p.Attribute("Include") ?? "").StartsWith("WebFormsForCore.", StringComparison.OrdinalIgnoreCase)))
        {
            root.Add(XElement.Parse(AliasTarget.Trim()));
        }
        project.Save(targetPath);

        var defines = Elements(project, "DefineConstants").SelectMany(d => d.Value.Split(';')).Select(s => s.Trim())
            .Where(s => s.Length > 0 && !s.Contains('$')).Distinct().ToList();
        report.Add(Report.Kind.Project, name, $"SDK-style: {string.Join(';', frameworks)} -> net10.0");
        return new ConvertedProject(name, projectPath, targetPath, false, defines);
    }

    sealed class ItemsToAdd
    {
        public List<Package> Items { get; } = new();
        public void Add(Package p) { if (!Items.Any(i => i.Id.Equals(p.Id, StringComparison.OrdinalIgnoreCase))) Items.Add(p); }
    }

    // What .NET Framework had in its own assemblies and .NET ships as packages: added when used.
    void AddSourcePackages(string name, IEnumerable<string> files, Action<Package> add)
    {
        var text = string.Join('\n', files.Where(File.Exists).Select(File.ReadAllText));
        foreach (var rule in rules.SourcePackages)
        {
            if (!rule.Pattern.IsMatch(text)) continue;
            add(rule.Package);
            if (rule.Note != null) report.Add(Report.Kind.Unsupported, name, $"uses {rule.Package.Id}: {rule.Note}");
        }
    }

    // Assembly name -> project file, for references to another project's build output (a HintPath
    // into its bin folder, which does not exist in a clean checkout).
    string? ProjectOfAssembly(string assembly)
    {
        if (assemblyProjects == null)
        {
            assemblyProjects = new(StringComparer.OrdinalIgnoreCase);
            foreach (var file in Directory.EnumerateFiles(sourceRoot, "*.*proj", SearchOption.AllDirectories)
                         .Where(f => f.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase)))
            {
                string? name = null;
                try { name = XDocument.Load(file).Descendants().FirstOrDefault(e => e.Name.LocalName == "AssemblyName")?.Value; }
                catch (System.Xml.XmlException) { continue; }
                assemblyProjects.TryAdd(string.IsNullOrWhiteSpace(name) ? Path.GetFileNameWithoutExtension(file) : name, file);
            }
        }
        return assemblyProjects.GetValueOrDefault(assembly);
    }
}

static class XPathExtensions
{
    /// <summary>Elements at a slash-separated path of local names, from the root.</summary>
    public static IEnumerable<XElement> XPathSelect(this XDocument document, string path)
    {
        IEnumerable<XElement> current = new[] { document.Root! }.Where(r => r.Name.LocalName == path.Split('/')[0]);
        foreach (var segment in path.Split('/').Skip(1)) current = current.Elements().Where(e => e.Name.LocalName == segment);
        return current;
    }
}
