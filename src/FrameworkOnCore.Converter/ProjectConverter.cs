using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FrameworkOnCore.Converter;

/// <summary>A project as converted: its file in the output tree and its conditional compilation symbols.</summary>
/// <param name="Defines">The symbols its sources are compiled with (the configuration's, the project's, the .NET Framework
/// target's), but the SDK's own of net10.0 (NET, NETCOREAPP...), which only a project with ImplicitFrameworkDefines has: one
/// built for .NET only (netstandard, net6.0), whose code is meant for it.</param>
/// <param name="CodePage">The code page its sources without a byte order mark that are not UTF-8 are in (its CodePage, else
/// the machine's: SourceEncoding.MachineCodePage); null: none known.</param>
/// <param name="Packages">The packages it references as converted (their ids).</param>
public sealed record ConvertedProject(string Name, string SourcePath, string TargetPath, bool IsWeb, IReadOnlyList<string> Defines, string? AssemblyName = null, bool CrossPlatform = false, bool ImplicitFrameworkDefines = false, bool Kept = false, int? CodePage = null, IReadOnlyCollection<string>? Packages = null)
{
    /// <summary>Whether it serves WCF's services over CoreWCF (FrameworkOnCore.ServiceModel): the host maps them before Web Forms.</summary>
    public bool ServesWcf => Packages?.Contains(ProjectConverter.ServiceModelPackage, StringComparer.OrdinalIgnoreCase) == true;
}

/// <summary>Where the runtime pieces the converted projects refer to are (FrameworkOnCore's feed, the shims).</summary>
public sealed record RuntimeLayout(string Feed, IReadOnlyList<string> ShimProjects);

/// <summary>
/// Turns a .NET Framework web project - and the projects it references - into .NET 10 projects on
/// WebFormsForCore, keeping every source file as it is. The repository is copied as a whole and the
/// project files are rewritten where they are.
/// </summary>
/// <param name="deployedBin">The deployed site's bin (--site): a project this converter does not convert is referenced by the
/// assembly its build put there, as it is (a .NET Framework assembly, which .NET loads: its System.Web is the runtime's).</param>
public sealed partial class ProjectConverter(Rules rules, Report report, Conditions conditions, string sourceRoot, string outRoot, RuntimeLayout runtime, string? deployedBin = null)
{
    static readonly XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";

    // Every converted project: new vulnerability advisories on old package versions are warnings, not
    // the errors TreatWarningsAsErrors would make them (the original built before they were published).
    const string NoWarn = "$(NoWarn);NU1701;NU1603;NU1608;SYSLIB0011";
    const string WarningsNotAsErrors = "$(WarningsNotAsErrors);NU1901;NU1902;NU1903;NU1904";

    // WebFormsForCore's packages carry System.Web; the one in .NET's own reference set is removed.
    // CoreWCF (under FrameworkOnCore.Web.Extensions) brings System.Web.Services.Description, whose WSDL
    // types (WsiProfiles, ServiceDescription, ...) System.Web.Services has too (CS0433).
    const string AliasTarget = """
          <Target Name="ChangeAliasesOfNugetRefs" BeforeTargets="FindReferenceAssembliesForReferences;ResolveReferences">
            <ItemGroup>
              <ReferencePath Remove="%(Identity)" Condition="'%(FileName)' == 'System.Web' AND '%(ReferencePath.FrameworkReferenceName)' != ''" />
              <ReferencePath Remove="%(Identity)" Condition="'%(FileName)' == 'System.Web.Services.Description'" />
            </ItemGroup>
          </Target>
    """;

    // A shim only DLLs refer to (FrameworkOnCore.Compat.ForDlls): not compiled against, its assembly copied to the output.
    const string CopyOnly = " ReferenceOutputAssembly=\"false\" OutputItemType=\"Content\" CopyToOutputDirectory=\"PreserveNewest\"";

    /// <summary>The folder of the DLLs retargeted after the build (AssemblyRetargeter), under the output.</summary>
    public const string RetargetedFolder = "foc-retargeted";

    // DLLs without source whose type references were retargeted (AssemblyRetargeter): copied to the output in place of
    // the package's or the repository's (every build, in any place: a container's too).
    const string RetargetTarget = """
          <Target Name="FocUseRetargetedAssemblies" BeforeTargets="_CopyFilesMarkedCopyLocal" Condition="Exists('$(FocRetargetedAssemblies)')">
            <ItemGroup>
              <_FocRetargeted Include="@(ReferenceCopyLocalPaths)" Condition="'%(Extension)' == '.dll' AND Exists('$(FocRetargetedAssemblies)%(Filename)%(Extension)')" />
              <ReferenceCopyLocalPaths Remove="@(_FocRetargeted)" />
              <ReferenceCopyLocalPaths Include="@(_FocRetargeted->'$(FocRetargetedAssemblies)%(Filename)%(Extension)')" />
            </ItemGroup>
          </Target>
    """;

    // The symbols the .NET SDK defines for net10.0, for reading sources the way the compiler does: a project built for
    // .NET only keeps them (ConvertedProject.ImplicitFrameworkDefines); one built for .NET Framework is compiled without
    // them, as it was (DisableImplicitFrameworkDefines: its #if NETCOREAPP, NET6_0_OR_GREATER code is not the build that
    // ran; YAF's ServiceStack took its .NET 6 branch, whose class its sources have not).
    public static readonly IReadOnlyList<string> SdkSymbols = new[] { "NET", "NETCOREAPP", "NET10_0" }
        .Concat(new[] { "5_0", "6_0", "7_0", "8_0", "9_0", "10_0" }.Select(v => $"NET{v}_OR_GREATER"))
        .Concat(new[] { "1_0", "1_1", "2_0", "2_1", "2_2", "3_0", "3_1" }.Select(v => $"NETCOREAPP{v}_OR_GREATER")).ToList();

    readonly Dictionary<string, ConvertedProject> converted = new(StringComparer.OrdinalIgnoreCase);
    Dictionary<string, string>? assemblyProjects;

    public IReadOnlyCollection<ConvertedProject> Converted => converted.Values;

    /// <summary>The cultures of Microsoft's language packages the projects had (Microsoft.AspNet.Mvc.ja: ja), each with the
    /// assemblies their satellites were of: FrameworkOnCore's ports' translations kept for them (LanguagePacks).</summary>
    public Dictionary<string, SortedSet<string>> LanguagePackageCultures { get; } = new(StringComparer.OrdinalIgnoreCase);

    // A language package (Microsoft.AspNet.Mvc.ja) the project had: not referenced (it depends on Microsoft's package, whose
    // DLLs the port replaces); the port's own translation in its culture instead.
    readonly HashSet<string> languagePackagesReported = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>A package the project lists (packages.config, PackageReference; version null: none given): what becomes of
    /// it (PackageDecisions), reported, a language package's culture noted.</summary>
    PackageDecision Listed(string name, string id, string? version)
    {
        var decision = rules.Listed(id, version ?? "*");
        var named = version == null ? id : $"{id} {version}";
        switch (decision.Fate)
        {
            case PackageFate.LanguagePackage:
                var language = rules.LanguagePackageOf(id)!.Value;
                if (!LanguagePackageCultures.TryGetValue(language.Culture, out var assemblies)) LanguagePackageCultures[language.Culture] = assemblies = new(StringComparer.OrdinalIgnoreCase);
                assemblies.UnionWith(language.Assemblies);
                if (languagePackagesReported.Add(name + "|" + id)) report.Add(Report.Kind.Project, name, $"{id}: Microsoft's language package removed; FrameworkOnCore's own translations of {string.Join(", ", language.Assemblies)} in {language.Culture} instead (the wording is FrameworkOnCore's)");
                break;
            case PackageFate.Shim:
                report.Add(Report.Kind.Project, name, $"{named}: replaced by its shim ({decision.Shim})");
                break;
            case PackageFate.Dropped:
                report.Add(Report.Kind.Project, name, $"{named}: dropped (in .NET, or listed in droppedPackages)");
                break;
            case PackageFate.Replaced when decision.RaisedNote != null:
                report.Add(Report.Kind.Raised, name, decision.RaisedNote + $" (replaced by {decision.Package!.Id} {decision.Package.Version}: replacedPackages)");
                break;
        }
        return decision;
    }

    // A package referenced by its .NET Framework asset (frameworkAssets): reported where it is.
    void ReportFrameworkAsset(string name, string id)
    {
        var asset = rules.FrameworkAssets[id];
        report.Add(Report.Kind.Project, name, $"{id}: its .NET Framework asset ({asset.Asset}), as the site ran it: {asset.Note}");
    }

    public string TargetOf(string sourcePath) => Path.Combine(outRoot, Path.GetRelativePath(sourceRoot, sourcePath));

    /// <param name="copyLocal">A project built on its own (not referenced by the web project): its packages' assemblies are
    /// copied to its output, as .NET Framework did (an old-style project's CopyLocalLockFileAssemblies).</param>
    public ConvertedProject Convert(string projectPath, bool isWeb, bool copyLocal = false)
    {
        projectPath = Path.GetFullPath(projectPath);
        if (converted.TryGetValue(projectPath, out var done)) return done;
        var name = Path.GetFileNameWithoutExtension(projectPath);
        // Registered before its references are converted: a cycle stops here.
        converted[projectPath] = new ConvertedProject(name, projectPath, TargetOf(projectPath), isWeb, Array.Empty<string>());

        var document = XDocument.Load(projectPath, LoadOptions.PreserveWhitespace);
        var isSdk = document.Root!.Attribute("Sdk") != null;

        var references = new List<string>();
        var deployed = new List<(string Assembly, string Dll)>();
        foreach (var reference in Elements(document, "ProjectReference").ToList())
        {
            if (reference.Attribute("Include") == null || !isSdk && !ItemHolds(reference, name)) continue;
            var referenced = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectPath)!, Native((string)reference.Attribute("Include")!)));
            // A path that is not there (a project moved in the repository: nopCommerce's promotion providers name
            // ..\Nop.BusinessLogic, which is in Libraries): the repository's project of its GUID, as Visual Studio finds it.
            if (!File.Exists(referenced) && reference.Elements().FirstOrDefault(e => e.Name.LocalName == "Project")?.Value is { } guid &&
                ProjectOfGuid(guid) is { } byGuid)
            {
                report.Add(Report.Kind.Project, name, $"project reference {(string)reference.Attribute("Include")!} is not there: the project of its GUID, {Path.GetRelativePath(sourceRoot, byGuid)} (as Visual Studio finds it)");
                referenced = byGuid;
            }
            if (!IsConvertible(referenced))
            {
                if (DeployedAssembly(referenced) is { } dll)
                {
                    deployed.Add(dll);
                    report.Add(Report.Kind.Unsupported, name, $"{Path.GetFileName(referenced)} not converted (only C# and Visual Basic projects): its .NET Framework assembly in the deployed site is referenced as it is");
                    // In an SDK-style project, the reference is rewritten where it is (its conditions stay).
                    if (isSdk) reference.ReplaceWith(DeployedReference(reference.Name.Namespace, dll, Path.GetDirectoryName(TargetOf(projectPath))!));
                }
                else report.Add(Report.Kind.Unsupported, name, $"project reference not converted (not C# or Visual Basic): {referenced}");
                continue;
            }
            if (!File.Exists(referenced)) { report.Add(Report.Kind.Error, name, $"project reference not found: {referenced}"); continue; }
            Convert(referenced, false);
            references.Add(referenced);
        }

        var result = isSdk ? ConvertSdk(projectPath, name, document, isWeb) : ConvertOld(projectPath, name, document, isWeb, copyLocal, references, deployed);
        converted[projectPath] = result;
        return result;
    }

    /// <summary>
    /// The web.config rules: the assemblies it names for page compilation get their packages, and the
    /// ones .NET has no counterpart for are left out - loaded for page compilation, they fail the whole
    /// configuration (every page). Pages using their types do not compile; a compatibility assembly
    /// will take their place (LINUX-CONVERTER-DESIGN.md §8). Written to <paramref name="target"/> when
    /// changed (the original is unchanged).
    /// </summary>
    public void TransformWebConfig(string source, string target, string subject, Action<Package>? addPackage)
    {
        try
        {
            var configDocument = XmlFiles.Load(source, LoadOptions.PreserveWhitespace, out var original);
            var removed = false;
            // Wherever system.web is (<location path="."> too, as mojoPortal has it).
            foreach (var add in configDocument.Descendants().Where(e => e.Name.LocalName == "add" &&
                         e.Parent?.Name.LocalName == "assemblies" && e.Parent.Parent?.Name.LocalName == "compilation").ToList())
            {
                var assembly = ((string?)add.Attribute("assembly") ?? "").Split(',')[0].Trim();
                var decision = rules.Reference(assembly);
                if (addPackage != null && decision.Package != null) foreach (var p in decision.Companions.Prepend(decision.Package)) addPackage(p);
                if (decision.NoAnswer)
                {
                    add.Remove();
                    removed = true;
                    report.Add(Report.Kind.Unsupported, subject, $"web.config <compilation><assemblies>: {assembly} left out (no .NET counterpart; pages using it do not compile)");
                }
            }
            // What else names such an assembly: controls registered from it (<pages><controls>: the tag prefix's lookup
            // loads it for every page with that prefix; nopCommerce registers asp: from System.Web.DataVisualization), and
            // handlers and modules of its types (type="..., Assembly").
            foreach (var add in configDocument.Descendants().Where(e => e.Name.LocalName == "add").ToList())
            {
                var parent = add.Parent?.Name.LocalName;
                var assembly = parent == "controls" ? (string?)add.Attribute("assembly")
                    : parent is "httpHandlers" or "handlers" or "httpModules" or "modules" && ((string?)add.Attribute("type"))?.Split(',') is { Length: > 1 } parts ? parts[1] : null;
                var assemblyName = assembly?.Split(',')[0].Trim();
                if (assemblyName == null || !rules.NoAnswer.Contains(assemblyName)) continue;
                add.Remove();
                removed = true;
                report.Add(Report.Kind.Unsupported, subject, $"web.config <{parent}>: {(string?)add.Attribute("tagPrefix") ?? (string?)add.Attribute("name") ?? (string?)add.Attribute("path")} of {assemblyName} left out (no .NET counterpart)");
            }
            // The Chart control's image files (ChartImg.axd, storage=file): a folder on a Windows drive is not on Linux,
            // where the handler fails every chart. Left as it is (right on Windows); the deployment's setting replaces it.
            foreach (var add in configDocument.Descendants().Where(e => e.Name.LocalName == "add" && e.Parent?.Name.LocalName == "appSettings" &&
                         (string?)e.Attribute("key") == "ChartImageHandler"))
                if (System.Text.RegularExpressions.Regex.Match((string?)add.Attribute("value") ?? "", @"(?i)(^|;)\s*dir\s*=\s*(?<dir>[a-z]:[^;]*)") is { Success: true } dir)
                    report.Add(Report.Kind.Platform, subject, $"web.config <appSettings> ChartImageHandler: the chart images' folder {dir.Groups["dir"].Value.Trim()} is a Windows path; on Linux set APPSETTING_ChartImageHandler (dir=a Linux folder the application may write, or storage=memory)");
            // A connection string's providerName as .NET Framework's DbProviderFactories found it: its provider table's
            // lookup (a DataTable's) ignored trailing spaces (U+0020, U+3000) and case ("System.Data.SqlClient ",
            // "system.data.sqlclient"; leading spaces it did not); .NET's matches the name exactly (ArgumentException
            // "wasn't found in the list of registered .NET Data Providers": every query of WingtipToys VB's EF context).
            // The name the provider is registered by: .NET Framework's built-in ones, the web.config's own (DbProviderFactories).
            var invariants = new[] { "System.Data.SqlClient", "System.Data.OleDb", "System.Data.Odbc" }
                .Concat(configDocument.Descendants().Where(e => e.Name.LocalName == "add" && e.Parent?.Name.LocalName == "DbProviderFactories")
                    .Select(e => ((string?)e.Attribute("invariant"))?.TrimEnd(' ', '　')).OfType<string>())
                .ToList();
            foreach (var add in configDocument.Descendants().Where(e => e.Name.LocalName == "add" && e.Parent?.Name.LocalName == "connectionStrings"))
                if (add.Attribute("providerName") is { } provider)
                {
                    var name = provider.Value.TrimEnd(' ', '　');
                    name = invariants.FirstOrDefault(i => string.Equals(i, name, StringComparison.OrdinalIgnoreCase)) ?? name;
                    if (name == provider.Value) continue;
                    report.Add(Report.Kind.Project, subject, $"web.config <connectionStrings> {(string?)add.Attribute("name")}: providerName \"{provider.Value}\" as \"{name}\" (.NET's DbProviderFactories matches the name exactly; .NET Framework's ignored trailing spaces and case)");
                    provider.Value = name;
                    removed = true;
                }
            // In its encoding (shift_jis: its declaration's), its byte order mark as it was.
            if (removed) XmlFiles.Save(configDocument, target, SaveOptions.DisableFormatting, original);
        }
        catch (System.Xml.XmlException e) { report.Add(Report.Kind.Error, subject, $"web.config not read: {e.Message}"); }
    }

    // web.config's WCF endpoints CoreWCF does not serve in the site (FrameworkOnCore.ServiceModel's ServicePlan leaves them
    // out): bindings other than basicHttp(s), wsHttp and webHttp, and wsHttpBinding's message security (its default mode).
    public static IEnumerable<(string Endpoint, string Why)> WcfEndpointsNotServed(string source)
    {
        var config = Directory.EnumerateFiles(source, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();
        if (config == null) yield break;
        XDocument document;
        try { document = XDocument.Load(config); } catch (System.Xml.XmlException) { yield break; }
        var model = document.Root?.Element("system.serviceModel");
        if (model == null) yield break;
        foreach (var service in model.Element("services")?.Elements("service") ?? [])
            foreach (var endpoint in service.Elements("endpoint"))
            {
                var kind = (string?)endpoint.Attribute("binding") ?? "";
                var contract = (string?)endpoint.Attribute("contract") ?? "";
                if (kind is "mexHttpBinding" or "mexHttpsBinding" || contract == "IMetadataExchange") continue;
                var where = $"{(string?)service.Attribute("name")} \"{(string?)endpoint.Attribute("address")}\" ({kind})";
                if (kind is not ("basicHttpBinding" or "basicHttpsBinding" or "wsHttpBinding" or "webHttpBinding"))
                    yield return (where, $"{kind} is not one it serves in the site");
                else if (kind == "wsHttpBinding")
                {
                    var name = (string?)endpoint.Attribute("bindingConfiguration") ?? "";
                    var mode = (string?)model.Element("bindings")?.Element("wsHttpBinding")?.Elements("binding")
                        .FirstOrDefault(b => ((string?)b.Attribute("name") ?? "") == name)?.Element("security")?.Attribute("mode") ?? "Message";
                    if (mode.Equals("Message", StringComparison.OrdinalIgnoreCase))
                        yield return (where, $"security mode {mode}{(name == "" ? ", the binding's default" : "")}: message security");
                }
            }
    }

    // The web project's, old-style or SDK-style: FrameworkOnCore's web packages, and for WCF's services the site has (.svc
    // files, web.config's serviceActivations) CoreWCF's, which serves them in the application's host (FrameworkOnCore.ServiceModel;
    // Program.cs calls it) as IIS's WCF activation served them, when that is the choice.
    void AddWebPackages(string name, string source, Action<Package> add)
    {
        foreach (var id in rules.WebPackages) add(new Package(id, rules.FrameworkOnCoreVersion));
        if (WcfServices(source) is not { Count: > 0 } services) return;
        if (rules.IsChosen("wcf-server:corewcf"))
        {
            add(new Package(ServiceModelPackage, rules.FrameworkOnCoreVersion));
            report.Add(Report.Kind.Project, name, $"WCF services ({string.Join(", ", services.Take(6))}{(services.Count > 6 ? ", ..." : "")}): served by CoreWCF (FrameworkOnCore.ServiceModel), as web.config configures them");
            foreach (var (endpoint, why) in WcfEndpointsNotServed(source))
                report.Add(Report.Kind.Unsupported, name, $"WCF endpoint {endpoint}: not served by CoreWCF ({why}); the service's other endpoints are");
        }
        else report.Add(Report.Kind.Unsupported, name, $"WCF services ({string.Join(", ", services.Take(6))}{(services.Count > 6 ? ", ..." : "")}): not served (wcf-server: none)");
    }

    /// <summary>FrameworkOnCore's WCF services over CoreWCF: the host serves them (HostProgram) where the web project has it.</summary>
    public const string ServiceModelPackage = "FrameworkOnCore.ServiceModel";

    // The web project's web.config and its folders' (nopCommerce's Administration registers controls of an assembly .NET
    // does not have): the assemblies web.config names for page compilation are loaded by name at run time; the ones .NET
    // does not ship need their package, or the configuration itself fails.
    void TransformWebConfigs(string name, string source, Action<Package> add)
    {
        if (Paths.WebConfig(source) is not { } webConfig) return;
        TransformWebConfig(webConfig, TargetOf(webConfig), name, add);
        foreach (var folderConfig in Paths.WebConfigs(source).Where(c => !string.Equals(c, webConfig, StringComparison.OrdinalIgnoreCase)))
            TransformWebConfig(folderConfig, TargetOf(folderConfig), $"{name} {Path.GetRelativePath(source, folderConfig)}", null);
    }

    // A .NET Framework assembly referenced by name (no HintPath): its package and what .NET Framework's assembly also had,
    // or reported where .NET has no answer.
    void AddFrameworkReference(string name, string assembly, Action<Package> add)
    {
        var decision = rules.Reference(assembly);
        if (decision.Package != null) foreach (var p in decision.Companions.Prepend(decision.Package)) add(p);
        else if (decision.NoAnswer) report.Add(Report.Kind.Unsupported, name, $"{assembly} has no .NET counterpart");
    }

    // A PackageReference's attributes for how the package is referenced (PackageDecisions.AssetsOf): its .NET Framework
    // asset's (its Reference next to it), or, for .NET Framework build tooling another package depends on (Microsoft.Bcl.Build:
    // Microsoft.Bcl's), nothing of it used, so that neither this reference nor the dependency imports its targets.
    IEnumerable<(string Attribute, string Value)> AssetAttributes(string id) => rules.AssetsOf(id) switch
    {
        PackageAssets.FrameworkAsset => [("ExcludeAssets", "compile;runtime"), ("GeneratePathProperty", "true")],
        PackageAssets.Inert => [("ExcludeAssets", "all"), ("PrivateAssets", "all")],
        _ => [],
    };

    // In an SDK-style project: a PackageReference referenced as AssetAttributes says, the Reference to its .NET Framework
    // asset after it.
    void ApplyAssets(XElement reference, string id, string name)
    {
        foreach (var (attribute, value) in AssetAttributes(id)) reference.SetAttributeValue(attribute, value);
        if (rules.AssetsOf(id) != PackageAssets.FrameworkAsset) return;
        var asset = rules.FrameworkAssets[id].Asset;
        reference.AddAfterSelf(new XElement(reference.Name.Namespace + "Reference", new XAttribute("Include", Path.GetFileNameWithoutExtension(asset)),
            new XElement(reference.Name.Namespace + "HintPath", FrameworkAssetPath(id, asset))));
        ReportFrameworkAsset(name, id);
    }

    /// <summary>The projects converted: C# and Visual Basic (an SDK-style project of the same language).</summary>
    public static bool IsConvertible(string project) =>
        project.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || project.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase);

    /// <summary>The WCF services of a web project's folder: its .svc files and web.config's serviceActivations (their addresses).</summary>
    public static List<string> WcfServices(string directory)
    {
        var services = Directory.EnumerateFiles(directory, "*.svc", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = true })
            .Where(f => !Paths.InBuildOutput(directory, f))
            .Select(f => Path.GetRelativePath(directory, f).Replace('\\', '/')).ToList();
        var config = Paths.WebConfig(directory);
        if (config != null)
        {
            try
            {
                services.AddRange(XDocument.Load(config).Root?.Element("system.serviceModel")?.Element("serviceHostingEnvironment")?.Element("serviceActivations")
                    ?.Elements("add").Select(a => ((string?)a.Attribute("relativeAddress") ?? "").TrimStart('~', '/')).Where(a => a.Length > 0) ?? []);
            }
            catch (System.Xml.XmlException) { }
        }
        return services.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    public static bool IsVisualBasic(string project) => project.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase);

    /// <summary>The web project's entry point (the host Program writes): Program.cs, Program.vb.</summary>
    public static string ProgramFile(string project) => IsVisualBasic(project) ? "Program.vb" : "Program.cs";

    static IEnumerable<XElement> Elements(XDocument document, string localName) =>
        document.Descendants().Where(e => e.Name.LocalName == localName).ToList();

    // An item the project has in the configuration built: its Include (an element with a Remove or an Update only is no
    // item of its own: ItemsOf).
    bool ItemHolds(XElement item, string subject) => item.Attribute("Include") != null && ConditionsHold(item, subject);

    bool ConditionsHold(XElement item, string subject) =>
        conditions.Holds((string?)item.Attribute("Condition"), subject) &&
        conditions.Holds((string?)item.Parent?.Attribute("Condition"), subject);

    /// <summary>
    /// The project's items of a kind as MSBuild evaluated them, in their order, each once: as the project writes it where it
    /// names one file ("Copy %281%29.cs", a linked "..\Shared\X.cs": the converted project's item is the original's), else
    /// the files MSBuild made of it (a wildcard's with its Exclude and the Removes after it, a property's list, another
    /// item's, an imported file's, Choose's), escaped as MSBuild writes names.
    /// </summary>
    static (List<string> Includes, Dictionary<string, string> Excludes, List<string> Removes) EvaluatedItems(FrameworkOnCore.Analysis.EvaluatedProject evaluated, string kind, string directory)
    {
        static string Escape(string name) => Regex.Replace(name, @"[%$@;?*'()]", m => "%" + ((int)m.Value[0]).ToString("X2"));
        var includes = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in evaluated.ItemsOf(kind))
        {
            if (!seen.Add(item.FullPath)) continue;
            var literal = !item.Imported && item.Unevaluated.IndexOfAny(['*', '?', ';', '$', '@']) < 0;
            // (a full path, "$(MSBuildProjectDirectory)\A.cs" evaluated: from the project's folder, as the converted project has it: the original's is not its)
            var include = Path.IsPathRooted(item.Include) ? Path.GetRelativePath(directory, item.FullPath).Replace('/', '\\') : item.Include;
            includes.Add(literal ? item.Unevaluated.Trim() : Escape(include));
        }
        return (includes, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase), new List<string>());
    }

    /// <summary>
    /// The project's items of a kind as MSBuild takes them, in their order: an Include of several (';' between them, spaces
    /// and line ends around), each once (a file listed twice is compiled once, as Visual Studio does: openIMIS's
    /// Resource1.designer.vb); an Exclude (of a wildcard: kept with it; of a file: the file left out); a Remove after them
    /// (the files of its name taken out, and kept for the wildcards before it).
    /// </summary>
    (List<string> Includes, Dictionary<string, string> Excludes, List<string> Removes) ItemsOf(XDocument old, string kind, string subject)
    {
        static IEnumerable<string> Parts(string? list) => (list ?? "").Split(';').Select(p => p.Trim()).Where(p => p.Length > 0);
        static string Key(string spec) => Regex.Replace(spec.Replace('/', '\\'), @"(^|\\)\.(?=\\)", "").TrimStart('\\');
        static bool Same(string a, string b) => string.Equals(Key(a), Key(b), StringComparison.OrdinalIgnoreCase);
        var includes = new List<string>();
        var excludes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var removes = new List<string>();
        foreach (var item in old.Descendants(msbuild + kind).Where(e => ConditionsHold(e, subject)))
        {
            if (item.Attribute("Include") is { } include)
            {
                var excluded = Parts((string?)item.Attribute("Exclude")).ToList();
                foreach (var part in Parts(include.Value))
                {
                    if (excluded.Any(x => Same(x, part)) || includes.Contains(part, StringComparer.OrdinalIgnoreCase)) continue;
                    includes.Add(part);
                    if (excluded.Count > 0 && part.Contains('*')) excludes[part] = string.Join(";", excluded);
                }
            }
            else if (item.Attribute("Remove") is { } remove)
                foreach (var part in Parts(remove.Value))
                {
                    includes.RemoveAll(i => Same(i, part));
                    removes.Add(part);
                }
        }
        if (!includes.Any(i => i.Contains('*'))) removes.Clear();
        return (includes, excludes, removes);
    }

    // ------------------------------------------------------------------------------------------
    // Old-style (.NET Framework) projects: a new SDK-style project file with the original's items.

    ConvertedProject ConvertOld(string projectPath, string name, XDocument old, bool isWeb, bool copyLocal, List<string> projectReferences, List<(string Assembly, string Dll)> deployed)
    {
        var source = Path.GetDirectoryName(projectPath)!;
        conditions.Project(old, projectPath, sourceRoot, name);
        var targetPath = TargetOf(projectPath);
        var target = Path.GetDirectoryName(targetPath)!;
        // A property the project has: its value as MSBuild evaluated it for the configuration (the last definition that
        // holds, its $(...) put in: "$(Product).Core", an AssemblyName by the configuration); as its first element writes it
        // where MSBuild could not read the project. One the project does not define is none (MSBuild's own defaults are not its).
        var evaluated = conditions.Evaluated;
        string? Property(string property) =>
            old.Descendants(msbuild + property).FirstOrDefault() is { } written ? evaluated != null ? evaluated.Property(property) ?? "" : written.Value : null;

        // What the original compiled and embedded, in its order (linked files keep their paths: the
        // repository is copied as a whole). A file listed twice is compiled once, as Visual Studio does (openIMIS's
        // Resource1.designer.vb; the Visual Basic compiler would define its types twice).
        var visualBasic = IsVisualBasic(projectPath);
        var (compile, compileExcludes, compileRemoves) = evaluated != null ? EvaluatedItems(evaluated, "Compile", source) : ItemsOf(old, "Compile", name);
        // A file the project lists that the repository does not have (the original does not build either: CS2001): left
        // out and reported (MvcMovie of AspNetDocs has no Properties\AssemblyInfo.cs).
        foreach (var missing in compile.Where(c => !c.Contains('*') && !c.Contains("$(") && !File.Exists(ItemFile(source, c))).ToList())
        {
            compile.Remove(missing);
            report.Add(Report.Kind.Project, name, $"{missing}: listed by the project, not in the repository (the original build fails on it: CS2001); left out");
        }
        var (embedded, embeddedExcludes, embeddedRemoves) = evaluated != null ? EvaluatedItems(evaluated, "EmbeddedResource", source) : ItemsOf(old, "EmbeddedResource", name);
        foreach (var missing in embedded.Where(c => !c.Contains('*') && !c.Contains("$(") && !File.Exists(ItemFile(source, c))).ToList())
        {
            embedded.Remove(missing);
            report.Add(Report.Kind.Project, name, $"{missing}: an embedded resource the project lists, not in the repository (the original build fails on it); left out");
        }
        var assemblyName = Property("AssemblyName") ?? name;
        // Visual Basic's puts every type in it: as written, empty too (DNN's DotNetNuke.WebUtility; the SDK would make it
        // the project's name).
        var rootNamespace = visualBasic ? Property("RootNamespace")?.Trim() ?? "" : Property("RootNamespace") ?? assemblyName;
        // Conditional compilation symbols of the configuration built (NET_4_0 and the like select code).
        // (MSBuild's final DefineConstants: a later definition replaces the one before unless it builds on it; by hand, every one that holds)
        var defines = (evaluated != null
                ? (evaluated.Property("DefineConstants") ?? "").Split(';', ',')
                : old.Descendants(msbuild + "DefineConstants")
                    .Where(d => conditions.Holds((string?)d.Parent?.Attribute("Condition"), name))
                    .SelectMany(d => d.Value.Split(';', ',')))
            .Select(s => s.Trim()).Where(s => s.Length > 0 && !s.Contains('$')).Distinct().ToList();
        // The code page its sources without a byte order mark that are not UTF-8 are in: the project's (csc /codepage), else
        // the machine's (ProjectConverter.Encodings.cs).
        var projectCodePage = old.Descendants(msbuild + "CodePage").Where(d => conditions.Holds((string?)d.Parent?.Attribute("Condition"), name))
            .Select(d => ParseCodePage(d.Value)).LastOrDefault(p => p != null);
        var codePage = SourcesCodePage(name, compile.Select(c => ItemFile(source, c)), projectCodePage) ?? projectCodePage;

        var packages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        // packages.config's packages that another package replaces (Microsoft.AspNet.Mvc: FrameworkOnCore.Web.Mvc), kept at
        // their version with nothing of them used. packages.config lists every package, the ones others depend on too:
        // without them NuGet resolves those dependencies itself (eShopLegacyMVC's Autofac.Mvc5 on Microsoft.AspNet.Mvc
        // 5.1.0), and the .NET Framework DLLs it brings (System.Web.WebPages 3.1) take the ports' place in the output.
        var pinned = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        void AddPackage(Package p) => packages.TryAdd(p.Id, p.Version);
        void AddListed(string id, string version, bool pin = false)
        {
            if (Listed(name, id, version) is not { Package: { } package } decision) return;
            packages[package.Id] = package.Version;
            foreach (var c in decision.Companions) AddPackage(c);
            if (pin && decision.Fate == PackageFate.Replaced && !package.Id.Equals(id, StringComparison.OrdinalIgnoreCase) && packages.TryAdd(id, version)) pinned.Add(id);
        }
        var config = Path.Combine(source, "packages.config");
        if (File.Exists(config))
        {
            foreach (var p in XDocument.Load(config).Descendants("package")) AddListed((string)p.Attribute("id")!, (string)p.Attribute("version")!, pin: true);
            if (pinned.Count > 0)
                report.Add(Report.Kind.Project, name, $"replaced packages kept at their version with nothing of them used (ExcludeAssets all), for the packages that depend on them: {string.Join(", ", pinned)}");
        }
        foreach (var p in old.Descendants(msbuild + "PackageReference").Where(e => ItemHolds(e, name)))
        {
            AddListed((string)p.Attribute("Include")!, (string?)p.Attribute("Version") ?? p.Element(msbuild + "Version")?.Value ?? "*");
        }
        if (isWeb) AddWebPackages(name, source, AddPackage);

        var binaryReferences = deployed.Select(d => (d.Assembly, HintPath: Paths.FromProject(target, d.Dll), Aliases: (string?)null)).ToList();
        var builtReferences = new List<string>();
        foreach (var reference in old.Descendants(msbuild + "Reference").Where(e => ItemHolds(e, name)))
        {
            var assembly = ((string)reference.Attribute("Include")!).Split(',')[0].Trim();
            var hint = reference.Element(msbuild + "HintPath")?.Value;
            if (hint != null)
            {
                // A package's DLL is covered by the PackageReference.
                if (Regex.IsMatch(hint, @"(^|\\)packages\\", RegexOptions.IgnoreCase)) continue;
                var dll = Path.GetFullPath(Path.Combine(source, Native(hint)));
                // Another project's build output: that project, by reference.
                var producer = ProjectOfAssembly(assembly);
                if (producer != null && (Regex.IsMatch(hint, @"(^|\\)bin\\", RegexOptions.IgnoreCase) || !File.Exists(dll)))
                {
                    if (!IsConvertible(producer))
                    {
                        if (DeployedAssembly(producer) is { } deployedDll)
                        {
                            binaryReferences.Add((deployedDll.Assembly, Paths.FromProject(target, deployedDll.Dll), reference.Element(msbuild + "Aliases")?.Value));
                            report.Add(Report.Kind.Unsupported, name, $"{assembly} is built by {Path.GetFileName(producer)}, not converted (only C# and Visual Basic projects): its .NET Framework assembly in the deployed site is referenced as it is");
                        }
                        else report.Add(Report.Kind.Unsupported, name, $"{assembly} is built by {Path.GetFileName(producer)}: not converted (only C# and Visual Basic projects); the code using it is stubbed");
                        continue;
                    }
                    Convert(producer, false);
                    builtReferences.Add(producer);
                    continue;
                }
                // As MSBuild does with a HintPath to nothing (warning MSB3245): the reference is left out.
                if (!File.Exists(dll)) { report.Add(Report.Kind.Project, name, $"{assembly}: HintPath to a file that is not there ({hint}), left out as MSBuild does"); continue; }
                // A DLL checked into the repository of a package that has a .NET answer (nopCommerce's AjaxControlToolkit 4.1,
                // which .NET's ASP.NET AJAX refuses): that package, as for a package reference.
                if (rules.InRepository(assembly) is { Package: { } replacement } inRepository)
                {
                    packages[replacement.Id] = replacement.Version;
                    foreach (var c in inRepository.Companions) AddPackage(c);
                    report.Add(Report.Kind.Project, name, $"{assembly}: a DLL in the repository ({hint}), replaced by the package {replacement.Id} {replacement.Version} (replacedPackages)");
                    continue;
                }
                // A DLL checked into the repository: referenced where it is.
                binaryReferences.Add((assembly, hint, reference.Element(msbuild + "Aliases")?.Value));
            }
            else AddFrameworkReference(name, assembly, AddPackage);
        }

        if (isWeb) TransformWebConfigs(name, source, AddPackage);

        AddSourcePackages(name, compile.Select(c => ItemFile(source, c)), AddPackage);
        var preserialized = HasNonStringResources(embedded.Select(e => ItemFile(source, e)));
        if (preserialized) AddPackage(rules.ResourcesPackage);
        var usesWebFormsForCore = isWeb || packages.Keys.Any(k => k.StartsWith("FrameworkOnCore.", StringComparison.OrdinalIgnoreCase));

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
        // Visual Basic's are separated by commas, and the SDK's own are not in DefineConstants (FinalDefineConstants).
        if (defines.Count > 0) Prop("DefineConstants", visualBasic ? string.Join(',', defines) : "$(DefineConstants);" + string.Join(';', defines));
        // The .NET Framework build had no NET, NETCOREAPP, NET5_0_OR_GREATER... (nor NETFRAMEWORK: an old-style project
        // defines none): code under them is not the code that ran.
        Prop("DisableImplicitFrameworkDefines", "true");
        // An old-style project compiled against its own references only, not theirs (mojoPortal's Features.UI did not see
        // Features.Data.MSSQL's namespace mojoPortal.MediaPlayer, which hides its class MediaPlayer: CS0118). Their
        // assemblies still go to the output, found next to the references (as they did).
        Prop("DisableTransitiveProjectReferences", "true");
        // Its sources' code page: the project's, or the machine's the original build read them in (on Linux there is none).
        if (codePage != null) Prop("CodePage", codePage.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        if (visualBasic)
        {
            // The language options the sources were written for (Option Strict Off: late binding), and the project's
            // imports as they are (the SDK's own set differs: System.Threading.Tasks, no System.Data).
            string? Option(string option) => old.Descendants(msbuild + option).FirstOrDefault(o => conditions.Holds((string?)o.Parent?.Attribute("Condition"), name))?.Value.Trim();
            foreach (var option in new[] { "OptionExplicit", "OptionCompare", "OptionStrict", "OptionInfer" })
            {
                if (Option(option) is { Length: > 0 } value) Prop(option, value);
            }
            if (Option("MyType") is { Length: > 0 } myType) Prop("MyType", ServerMyType(name, myType, compile.Select(c => ItemFile(source, c))));
            Prop("DisableImplicitNamespaceImports", "true");
        }
        Prop("RestoreAdditionalProjectSources", Paths.FromProject(target, runtime.Feed));
        if (preserialized) Prop("GenerateResourceUsePreserializedResources", "true");
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
        else if (copyLocal)
        {
            Prop("CopyLocalLockFileAssemblies", "true");
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
        foreach (var c in compile) text.Append($"    <Compile Include=\"{SecurityElement.Escape(c)}\"{(compileExcludes.TryGetValue(c, out var without) ? $" Exclude=\"{SecurityElement.Escape(without)}\"" : "")} />\n");
        foreach (var c in compileRemoves) text.Append($"    <Compile Remove=\"{SecurityElement.Escape(c)}\" />\n");
        if (isWeb) text.Append($"    <Compile Include=\"{ProgramFile(projectPath)}\" />\n");
        foreach (var import in old.Descendants(msbuild + "Import").Where(e => e.Attribute("Include") != null && ItemHolds(e, name)))
            text.Append($"    <Import Include=\"{SecurityElement.Escape((string)import.Attribute("Include")!)}\" />\n");
        foreach (var e in embedded) text.Append($"    <EmbeddedResource Include=\"{SecurityElement.Escape(e)}\"{(embeddedExcludes.TryGetValue(e, out var without) ? $" Exclude=\"{SecurityElement.Escape(without)}\"" : "")} />\n");
        foreach (var e in embeddedRemoves) text.Append($"    <EmbeddedResource Remove=\"{SecurityElement.Escape(e)}\" />\n");
        // The models embedded for EF6 (entity-framework-4: ef6, the default).
        foreach (var (file, logicalName) in rules.IsChosen("entity-framework-4:ef6") ? EntityDeploy(name, old, source, target) : [])
            text.Append($"    <EmbeddedResource Include=\"{SecurityElement.Escape(file)}\" LogicalName=\"{SecurityElement.Escape(logicalName)}\" />\n");
        text.Append("  </ItemGroup>\n\n  <ItemGroup>\n");
        foreach (var (id, version) in packages)
        {
            text.Append($"    <PackageReference Include=\"{id}\" Version=\"{version}\"");
            foreach (var (attribute, value) in pinned.Contains(id) ? [("ExcludeAssets", "all"), ("PrivateAssets", "all")] : AssetAttributes(id)) text.Append($" {attribute}=\"{value}\"");
            text.Append(" />\n");
            if (rules.AssetsOf(id) == PackageAssets.FrameworkAsset)
            {
                var asset = rules.FrameworkAssets[id].Asset;
                text.Append($"    <Reference Include=\"{Path.GetFileNameWithoutExtension(asset)}\" HintPath=\"{FrameworkAssetPath(id, asset)}\" />\n");
                ReportFrameworkAsset(name, id);
            }
        }
        foreach (var (assembly, hint, referenceAliases) in binaryReferences)
        {
            var aliasAttribute = string.IsNullOrWhiteSpace(referenceAliases) ? "" : $" Aliases=\"{SecurityElement.Escape(referenceAliases)}\"";
            text.Append($"    <Reference Include=\"{SecurityElement.Escape(assembly)}\" HintPath=\"{SecurityElement.Escape(hint)}\"{aliasAttribute} />\n");
        }
        // Analyzers and source generators referenced as projects (DNN generates the defining parts of
        // partial methods, CS0759 without it): with their metadata, as the original has them.
        var analyzers = old.Descendants(msbuild + "ProjectReference")
            .Where(r => r.Attribute("Include") != null && string.Equals((string?)r.Attribute("OutputItemType"), "Analyzer", StringComparison.OrdinalIgnoreCase))
            .Select(r => Path.GetFullPath(Path.Combine(source, Native((string)r.Attribute("Include")!))))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        // extern alias names on project references (Aliases), as the original has them.
        var aliases = old.Descendants(msbuild + "ProjectReference").Where(r => r.Attribute("Include") != null)
            .Select(r => (Path: Path.GetFullPath(Path.Combine(source, Native((string)r.Attribute("Include")!))), Aliases: r.Element(msbuild + "Aliases")?.Value ?? (string?)r.Attribute("Aliases")))
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
            if (Regex.IsMatch(analyzer, @"(^|\\)packages\\", RegexOptions.IgnoreCase) || !File.Exists(ItemFile(source, analyzer))) continue;
            text.Append($"    <Analyzer Include=\"{SecurityElement.Escape(analyzer)}\" />\n");
        }
        text.Append("  </ItemGroup>\n");

        // Every project: what .NET Framework had and .NET does not - assemblies .NET Framework packages
        // reference (System.Web.Abstractions: a library using WebFormsMvp needs it to compile), types and
        // members the sources use (FrameworkOnCore.Compat).
        text.Append("\n  <!-- Assemblies, types and members .NET Framework had and .NET does not (shims). -->\n  <ItemGroup>\n");
        foreach (var shim in runtime.ShimProjects)
            text.Append($"    <ProjectReference Include=\"{SecurityElement.Escape(Paths.FromProject(target, shim))}\"{(RuntimeSetup.ForDllsOnly(shim) ? CopyOnly : "")} />\n");
        text.Append("  </ItemGroup>\n");
        if (isWeb)
        {
            text.Append("""

                  <!-- Culture data from Windows (NLS), as on .NET Framework; on Linux the ICU data are made the
                       original server's instead (culture-profile.json). -->
                  <ItemGroup>
                    <RuntimeHostConfigurationOption Include="System.Globalization.UseNls" Value="true" />
                  </ItemGroup>

                """);
            // The original server's ANSI code page, where the pages (what the server compiles) are in it: ASP.NET read them in it
            // (web.config without fileEncoding), and it was Encoding.Default; Linux has none.
            if (ServerCodePage(name, source, compile.Select(c => ItemFile(source, c))) is { } serverCodePage)
                text.Append($"""

                      <!-- The ANSI code page of the original server (its pages are in it): the pages read in it where
                           web.config has no fileEncoding, as ASP.NET read them; Encoding.Default's. -->
                      <ItemGroup>
                        <RuntimeHostConfigurationOption Include="{AnsiCodePageOption}" Value="{serverCodePage}" />
                      </ItemGroup>

                    """);
            // The choice of .NET's text of doubles and floats: FrameworkOnCore's System.Web writes bound values and rewrites
            // what the server compiles as .NET Framework did unless switched off.
            if (!rules.IsChosen("floating-point:framework"))
                text.Append("""
                      <!-- Doubles and floats as .NET writes them (the choice floating-point: none). -->
                      <ItemGroup>
                        <RuntimeHostConfigurationOption Include="FrameworkOnCore.FloatingPointText" Value="false" />
                      </ItemGroup>

                    """);
        }
        // UTF-7 named in the sources (Encoding.GetEncoding("utf-7"), 65000, Encoding.UTF7): .NET Framework gave it, .NET
        // throws NotSupportedException for GetEncoding unless switched on (SYSLIB0001; disabled for its XSS risk).
        if (NamesUtf7(compile.Select(c => ItemFile(source, c)).Concat(isWeb ? PageFiles(source) : [])))
        {
            text.Append("""

                  <!-- UTF-7, which the sources name, as .NET Framework gave it (.NET: switched off, SYSLIB0001). -->
                  <ItemGroup>
                    <RuntimeHostConfigurationOption Include="System.Text.Encoding.EnableUnsafeUTF7Encoding" Value="true" />
                  </ItemGroup>

                """);
            report.Add(Report.Kind.Project, name, "UTF-7 named in the sources: switched on (System.Text.Encoding.EnableUnsafeUTF7Encoding), as .NET Framework had it; .NET switches it off for its XSS risk");
        }
        if (usesWebFormsForCore) text.Append('\n').Append(AliasTarget).Append('\n');
        text.Append($"\n  <PropertyGroup>\n    <FocRetargetedAssemblies>{SecurityElement.Escape(Paths.FromProject(target, Path.Combine(outRoot, RetargetedFolder)))}\\</FocRetargetedAssemblies>\n  </PropertyGroup>\n");
        text.Append(RetargetTarget).Append('\n');
        text.Append("</Project>\n");
        File.WriteAllText(targetPath, text.ToString(), new UTF8Encoding(false));

        report.Add(Report.Kind.Project, name, $"{compile.Count} files, {packages.Count} packages, {binaryReferences.Count} DLLs -> {Path.GetRelativePath(outRoot, targetPath)}");
        return new ConvertedProject(name, projectPath, targetPath, isWeb, defines.Concat(ConfigurationSymbols(visualBasic)).Distinct().ToList(), assemblyName,
            CodePage: codePage ?? SourceEncodingFallback, Packages: packages.Keys.ToList());
    }

    // ------------------------------------------------------------------------------------------
    // SDK-style projects: one target framework, net10.0 - the application runs on .NET 10 only, and a
    // netstandard target next to it could not reference the projects that became net10.0 (NU1201).
    // Conditions on the .NET Framework target are rewritten to it. Framework references become packages.

    ConvertedProject ConvertSdk(string projectPath, string name, XDocument project, bool isWeb)
    {
        var targetPath = TargetOf(projectPath);
        var target = Path.GetDirectoryName(targetPath)!;
        var root = project.Root!;
        XName N(string local) => root.Name.Namespace + local;

        // Analyzers and source generators run in the compiler: netstandard2.0, as they are (RS1041).
        if (Elements(project, "PackageReference").Any(p => ((string?)p.Attribute("Include") ?? "").StartsWith("Microsoft.CodeAnalysis", StringComparison.OrdinalIgnoreCase)))
        {
            report.Add(Report.Kind.Project, name, "analyzer / source generator: kept as it is");
            return new ConvertedProject(name, projectPath, targetPath, false, Array.Empty<string>(), Kept: true);
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
        // The .NET Framework target is the build the site ran: its code too (DNN's ModulePipeline registers
        // its services under #if NET472). The symbols the SDK defines for it.
        var frameworkSymbols = new List<string>();
        if (netfx.Count > 0)
        {
            var framework = NewestFramework(netfx);
            frameworkSymbols = FrameworkSymbols(framework);
            // Not the SDK's own of net10.0 too (NET, NET6_0_OR_GREATER: contradicting branches, YAF's ServiceStack).
            root.Add(new XElement(N("PropertyGroup"), new XElement(N("DefineConstants"), "$(DefineConstants);" + string.Join(';', frameworkSymbols)),
                new XElement(N("DisableImplicitFrameworkDefines"), "true")));
            report.Add(Report.Kind.Project, name, $"{framework}'s symbols defined ({string.Join(", ", frameworkSymbols.Take(2))} ...): the site ran that build");
        }

        bool HasPackage(string id) => Elements(project, "PackageReference").Any(p => string.Equals((string?)p.Attribute("Include"), id, StringComparison.OrdinalIgnoreCase));
        XElement PackageElement(Package p) => new(N("PackageReference"), new XAttribute("Include", p.Id), new XAttribute("Version", p.Version));

        foreach (var reference in Elements(project, "Reference"))
        {
            if (reference.Elements().Any(e => e.Name.LocalName == "HintPath")) continue;
            var assembly = ((string?)reference.Attribute("Include") ?? "").Split(',')[0].Trim();
            // In the reference's place: its ItemGroup's condition stays with it.
            AddFrameworkReference(name, assembly, p => { if (!HasPackage(p.Id)) reference.AddBeforeSelf(PackageElement(p)); });
            reference.Remove();
        }
        foreach (var reference in Elements(project, "PackageReference"))
        {
            var id = (string?)reference.Attribute("Include") ?? "";
            var version = ProjectFile.VersionOf(reference);
            var decision = Listed(name, id, version);
            if (decision.Package is not { } package)
            {
                reference.Remove();
                continue;
            }
            if (decision.Fate == PackageFate.Replaced)
            {
                // Two packages replaced by the same one (Microsoft.AspNet.WebApi and .WebApi.WebHost): referenced once. Not
                // this one itself, replaced by its own id at another version (Newtonsoft.Json 13.0.4: YAF.Types lost it).
                if (Elements(project, "PackageReference").Any(p => p != reference && string.Equals((string?)p.Attribute("Include"), package.Id, StringComparison.OrdinalIgnoreCase)))
                {
                    reference.Remove();
                    continue;
                }
                reference.SetAttributeValue("Include", package.Id);
                if (version != null) ProjectFile.SetVersion(reference, package.Version);
            }
            foreach (var c in decision.Companions.Reverse().Where(c => !HasPackage(c.Id))) reference.AddAfterSelf(PackageElement(c));
            ApplyAssets(reference, package.Id, name);
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
        var sources = Paths.SourceFiles(directory, IsVisualBasic(projectPath) ? "*.vb" : "*.cs");
        void Add(Package p) { if (!HasPackage(p.Id)) added.Add(p); }
        // A web project, as an old-style one: FrameworkOnCore's web packages, CoreWCF's for its services, web.config's assemblies.
        if (isWeb)
        {
            AddWebPackages(name, directory, Add);
            TransformWebConfigs(name, directory, Add);
        }
        AddSourcePackages(name, sources, Add);
        var preserialized = HasNonStringResources(Paths.SourceFiles(directory, "*.resx"));
        if (preserialized) Add(rules.ResourcesPackage);
        // Its sources' code page: the project's, or the machine's the original build read them in (on Linux there is none).
        var projectCodePage = ParseCodePage(Elements(project, "CodePage").LastOrDefault()?.Value);
        var codePage = SourcesCodePage(name, sources, projectCodePage);
        if (codePage != null) root.Add(new XElement(N("PropertyGroup"), new XElement(N("CodePage"), codePage.Value.ToString(System.Globalization.CultureInfo.InvariantCulture))));
        var itemGroup = new XElement(N("ItemGroup"), added.Items.Select(PackageElement));
        if (itemGroup.HasElements) root.Add(itemGroup);
        foreach (var reference in itemGroup.Elements().Where(e => e.Attribute("Include") != null).ToList()) ApplyAssets(reference, (string)reference.Attribute("Include")!, name);
        // As the old-style projects: the shims (types and members .NET Framework had).
        root.Add(new XElement(N("ItemGroup"), runtime.ShimProjects.Select(s => new XElement(N("ProjectReference"), new XAttribute("Include", Paths.FromProject(target, s)),
            RuntimeSetup.ForDllsOnly(s)
                ? new object[] { new XAttribute("ReferenceOutputAssembly", "false"), new XAttribute("OutputItemType", "Content"), new XAttribute("CopyToOutputDirectory", "PreserveNewest") }
                : Array.Empty<object>()))));
        if (preserialized) root.Add(new XElement(N("PropertyGroup"), new XElement(N("GenerateResourceUsePreserializedResources"), "true")));
        root.Add(new XElement(N("PropertyGroup"),
            new XElement(N("NoWarn"), NoWarn),
            new XElement(N("WarningsNotAsErrors"), WarningsNotAsErrors),
            // The converted code is not the original: the build's edits and .NET's obsoletions (SYSLIB) stay
            // warnings (the SYSLIB ones are reported).
            new XElement(N("TreatWarningsAsErrors"), "false"),
            new XElement(N("RestoreAdditionalProjectSources"), Paths.FromProject(target, runtime.Feed))));
        if (Elements(project, "PackageReference").Any(p => ((string?)p.Attribute("Include") ?? "").StartsWith("FrameworkOnCore.", StringComparison.OrdinalIgnoreCase)))
        {
            root.Add(XElement.Parse(AliasTarget.Trim()));
        }
        root.Add(new XElement(N("PropertyGroup"), new XElement(N("FocRetargetedAssemblies"), Paths.FromProject(target, Path.Combine(outRoot, RetargetedFolder)) + "\\")));
        root.Add(XElement.Parse(RetargetTarget.Trim()));
        project.Save(targetPath);

        var defines = Elements(project, "DefineConstants").SelectMany(d => d.Value.Split(';')).Select(s => s.Trim())
            .Where(s => s.Length > 0 && !s.Contains('$')).Distinct().ToList();
        report.Add(Report.Kind.Project, name, $"SDK-style: {string.Join(';', frameworks)} -> net10.0");
        // Built for .NET too (netstandard, netcoreapp, net5+): its code runs on Linux as it is, and what it does with
        // Windows' separators is meant (Lucene.Net: a backslash is refused in a name on every platform).
        var crossPlatform = frameworks.Any(f => Regex.IsMatch(f, @"^(netstandard|netcoreapp|net[5-9]\.|net\d\d\.)"));
        return new ConvertedProject(name, projectPath, targetPath, isWeb, defines.Concat(frameworkSymbols).Concat(ConfigurationSymbols(false)).Distinct().ToList(),
            Elements(project, "AssemblyName").FirstOrDefault()?.Value ?? name, crossPlatform, ImplicitFrameworkDefines: netfx.Count == 0,
            CodePage: projectCodePage ?? codePage ?? SourceEncodingFallback,
            Packages: Elements(project, "PackageReference").Select(p => (string?)p.Attribute("Include")).OfType<string>().ToList());
    }

    // What the SDK defines for the configuration (C#: TRACE, and DEBUG for Debug; Visual Basic's are its own: DEBUG=-1,
    // TRACE=-1, read as defined).
    IEnumerable<string> ConfigurationSymbols(bool visualBasic) =>
        conditions.Configuration.Equals("Debug", StringComparison.OrdinalIgnoreCase) ? new[] { "DEBUG", "TRACE" } : new[] { "TRACE" };

    // A path an MSBuild item names (Windows' separators) as this system's: the converter's tests run on Linux too.
    internal static string Native(string path) => path.Replace('\\', Path.DirectorySeparatorChar);

    /// <summary>
    /// The newest of .NET Framework's target framework monikers, by version ("net48" is 4.8, "net462" 4.6.2): the longest
    /// was taken, net462 of "net462;net48" (the code under NET48 and NET47_OR_GREATER compiled out).
    /// </summary>
    internal static string NewestFramework(IEnumerable<string> monikers) =>
        monikers.OrderBy(f => new Version(string.Join('.', f[3..].Where(char.IsAsciiDigit).Select(c => c.ToString()).Concat(["0"]).Take(Math.Max(2, f[3..].Count(char.IsAsciiDigit))))))
            .ThenBy(f => f, StringComparer.Ordinal).Last();

    /// <summary>
    /// An item's file as MSBuild finds it: its Include unescaped (Visual Studio writes "%28" for '(', "%3b" for ';' and the
    /// like: MSBuild's special characters) and with the system's separator ('\' in the project, on Linux too). The Include
    /// itself is kept as it is where it is written again (the converted project's: still MSBuild's). The file was looked
    /// for by the Include as it is: an escaped name, and every item in a folder on Linux, was "not in the repository".
    /// </summary>
    internal static string ItemFile(string directory, string include) =>
        Path.Combine(directory, Native(Regex.Replace(include, "%([0-9A-Fa-f]{2})", m => ((char)System.Convert.ToInt32(m.Groups[1].Value, 16)).ToString())));

    // The DLL of a package's .NET Framework asset, by the package's path property (GeneratePathProperty).
    internal static string FrameworkAssetPath(string id, string asset) => $"$(Pkg{id.Replace('.', '_').Replace('-', '_')})\\{asset.Replace('/', '\\')}";

    // What the .NET SDK defines for a .NET Framework target (net472: NETFRAMEWORK, NET472, NET20_OR_GREATER
    // ... NET472_OR_GREATER).
    static List<string> FrameworkSymbols(string framework)
    {
        string[] versions = ["20", "30", "35", "40", "45", "451", "452", "46", "461", "462", "47", "471", "472", "48", "481"];
        var own = framework.Substring(3);
        var index = Array.IndexOf(versions, own);
        var symbols = new List<string> { "NETFRAMEWORK", "NET" + own.ToUpperInvariant() };
        if (index >= 0) symbols.AddRange(versions.Take(index + 1).Select(v => $"NET{v}_OR_GREATER"));
        return symbols;
    }

    // Entity Framework's models (EntityDeploy: an .edmx): the .NET Framework build split each into its conceptual, storage
    // and mapping models and embedded them (Data\NopModel.edmx: Data.NopModel.csdl, .ssdl, .msl; the connection string names
    // them, res://*/Data.NopModel.csdl). The .NET SDK has no EntityDeploy: split here, embedded by the same names (nopCommerce's
    // EF4, openIMIS's EF6 model).
    List<(string File, string LogicalName)> EntityDeploy(string name, XDocument old, string source, string target)
    {
        var resources = new List<(string, string)>();
        foreach (var model in old.Descendants(msbuild + "EntityDeploy").Where(e => ItemHolds(e, name)).Select(e => (string)e.Attribute("Include")!))
        {
            var path = ItemFile(source, model);
            if (!File.Exists(path)) { report.Add(Report.Kind.Error, name, $"EntityDeploy {model}: not there"); continue; }
            var baseName = Path.ChangeExtension(model, null).Replace('\\', '.').Replace('/', '.');
            XDocument edmx;
            try { edmx = XDocument.Load(path); }
            catch (System.Xml.XmlException e) { report.Add(Report.Kind.Error, name, $"EntityDeploy {model}: {e.Message}"); continue; }
            var runtime = edmx.Descendants().FirstOrDefault(e => e.Name.LocalName == "Runtime");
            var parts = new[] { ("ConceptualModels", ".csdl"), ("StorageModels", ".ssdl"), ("Mappings", ".msl") };
            foreach (var (section, extension) in parts)
            {
                var content = runtime?.Elements().FirstOrDefault(e => e.Name.LocalName == section)?.Elements().FirstOrDefault();
                if (content == null) { report.Add(Report.Kind.Error, name, $"EntityDeploy {model}: no {section}"); continue; }
                var file = Path.Combine("FrameworkOnCore.EntityDeploy", baseName + extension);
                var output = Path.Combine(target, file);
                Directory.CreateDirectory(Path.GetDirectoryName(output)!);
                new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement(content)).Save(output);
                resources.Add((file, baseName + extension));
            }
            report.Add(Report.Kind.Project, name, $"EntityDeploy {model}: its conceptual, storage and mapping models embedded ({baseName}.csdl, .ssdl, .msl), as the .NET Framework build did");
        }
        return resources;
    }

    // A UTF-7 encoding named: its names ("utf-7", "unicode-1-1-utf-7"), its code page, Encoding.UTF7, UTF7Encoding.
    static bool NamesUtf7(IEnumerable<string> files) =>
        files.Where(File.Exists).Any(f => Regex.IsMatch(FrameworkOnCore.Analysis.SourceEncoding.ReadText(f), @"[""']utf-7[""']|unicode-1-1-utf-7|GetEncoding\(\s*65000\s*\)|\bUTF7(Encoding)?\b", RegexOptions.IgnoreCase));

    // The pages, user controls, handlers and App_Code of a web project: the code the server compiles.
    static IEnumerable<string> PageFiles(string directory) =>
        Paths.SourceFiles(directory, "*.*")
            .Where(f => Path.GetExtension(f).ToLowerInvariant() is ".aspx" or ".ascx" or ".master" or ".ashx" or ".asmx" or ".asax" or ".cs" or ".vb");

    // Visual Basic's My: a desktop application's (MyType Windows, WindowsForms, Console: My.Application, My.Computer, My.User)
    // is in .NET's Windows desktop only (Microsoft.VisualBasic.Forms); the compiler's My template would name its types, in no
    // source file the build could fix. A server's (Web: My.Computer, My.User, My.Log, My.Request, My.Response) is in
    // FrameworkOnCore.Compat: a library of a web application that uses My gets that one, one that does not none (Empty).
    string ServerMyType(string name, string myType, IEnumerable<string> sources)
    {
        if (myType is not ("Windows" or "WindowsForms" or "WindowsFormsWithCustomSubMain" or "Console")) return myType;
        var usesMy = sources.Where(File.Exists).Any(f => Regex.IsMatch(FrameworkOnCore.Analysis.SourceEncoding.ReadText(f), @"\bMy\.(Application|Computer|User|Log)\b"));
        var server = usesMy ? "Web" : "Empty";
        report.Add(Report.Kind.Project, name, $"MyType {myType} -> {server}: a desktop application's My is not in .NET outside the Windows desktop" +
            (usesMy ? "; the sources use My: a server's (My.Computer, My.User, My.Log; not My.Application)" : "; the sources do not use it"));
        return server;
    }

    // Resources that are not strings (images, icons: <data type="..."> or mimetype) are embedded
    // serialized; .NET's build does that with System.Resources.Extensions (preserializedResourcesPackage), which reads
    // them at run time (MSB3822 without it).
    static bool HasNonStringResources(IEnumerable<string> files) =>
        files.Where(f => f.EndsWith(".resx", StringComparison.OrdinalIgnoreCase) && File.Exists(f)).Any(f =>
        {
            try
            {
                return XDocument.Load(f).Root!.Elements("data").Any(d =>
                    d.Attribute("mimetype") != null ||
                    (d.Attribute("type") is { } type && !type.Value.StartsWith("System.String", StringComparison.Ordinal)));
            }
            catch (System.Xml.XmlException) { return false; }
        });

    string? DeployedBin => deployedBin;

    // Copied into the output (.deployed), referenced from there: null without a deployed site, or
    // without that assembly in it.
    (string Assembly, string Dll)? DeployedAssembly(string producer)
    {
        if (DeployedBin == null || !File.Exists(producer)) return null;
        string? assembly = null;
        try { assembly = XDocument.Load(producer).Descendants().FirstOrDefault(e => e.Name.LocalName == "AssemblyName")?.Value; }
        catch (System.Xml.XmlException) { }
        if (string.IsNullOrWhiteSpace(assembly)) assembly = Path.GetFileNameWithoutExtension(producer);
        var dll = Path.Combine(DeployedBin, assembly + ".dll");
        if (!File.Exists(dll)) return null;
        var copy = Path.Combine(outRoot, ".deployed", assembly + ".dll");
        Directory.CreateDirectory(Path.GetDirectoryName(copy)!);
        File.Copy(dll, copy, overwrite: true);
        return (assembly, copy);
    }

    static XElement DeployedReference(XNamespace ns, (string Assembly, string Dll) dll, string projectDirectory) =>
        new(ns + "Reference", new XAttribute("Include", dll.Assembly), new XElement(ns + "HintPath", Paths.FromProject(projectDirectory, dll.Dll)));

    sealed class ItemsToAdd
    {
        public List<Package> Items { get; } = new();
        public void Add(Package p) { if (!Items.Any(i => i.Id.Equals(p.Id, StringComparison.OrdinalIgnoreCase))) Items.Add(p); }
    }

    // The web project's properties a used package needs (sourcePackages' appProperties), whichever project uses it: written
    // once every project is converted (WriteApplicationProperties).
    readonly SortedDictionary<string, string> appProperties = new(StringComparer.Ordinal);

    /// <summary>The properties packages the application uses need (sourcePackages' appProperties: a runtime setting, set on
    /// the application the runtime runs), into the web project: those of every converted project, the ones built on their
    /// own too. Once every project is converted.</summary>
    public void WriteApplicationProperties(ConvertedProject web)
    {
        if (appProperties.Count == 0 || web.Kept) return;
        var file = ProjectFile.Load(web.TargetPath);
        file.AddProperties(appProperties.Select(p => (p.Key, p.Value)));
        file.Save();
        foreach (var (property, value) in appProperties)
            report.Add(Report.Kind.Project, web.Name, $"{property}={value} (a package the application uses needs it: sourcePackages)");
    }

    // What .NET Framework had in its own assemblies and .NET ships as packages: added when used.
    void AddSourcePackages(string name, IEnumerable<string> files, Action<Package> add)
    {
        var text = string.Join('\n', files.Where(File.Exists).Select(f => FrameworkOnCore.Analysis.SourceEncoding.ReadText(f)));
        foreach (var rule in rules.SourcePackages)
        {
            if (!rule.Pattern.IsMatch(text)) continue;
            add(rule.Package);
            if (rule.Note != null) report.Add(Report.Kind.Unsupported, name, $"uses {rule.Package.Id}: {rule.Note}");
            foreach (var (property, value) in rule.AppProperties ?? new Dictionary<string, string>()) appProperties[property] = value;
        }
        foreach (var note in rules.SourceNotes)
        {
            if (note.Pattern.IsMatch(text)) report.Add(Report.Kind.Unsupported, name, note.Note);
        }
    }

    Dictionary<string, string>? guidProjects;

    // Project GUID -> project file (the repository's projects).
    string? ProjectOfGuid(string guid)
    {
        if (guidProjects == null)
        {
            guidProjects = new(StringComparer.OrdinalIgnoreCase);
            foreach (var file in Directory.EnumerateFiles(sourceRoot, "*.*proj", SearchOption.AllDirectories).Where(IsConvertible))
            {
                string? projectGuid = null;
                try { projectGuid = XDocument.Load(file).Descendants().FirstOrDefault(e => e.Name.LocalName == "ProjectGuid")?.Value; }
                catch (System.Xml.XmlException) { continue; }
                if (!string.IsNullOrWhiteSpace(projectGuid)) guidProjects.TryAdd(projectGuid.Trim(), file);
            }
        }
        return guidProjects.GetValueOrDefault(guid.Trim());
    }

    // Assembly name -> project file, for references to another project's build output (a HintPath
    // into its bin folder, which does not exist in a clean checkout).
    public string? ProjectOfAssembly(string assembly)
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
