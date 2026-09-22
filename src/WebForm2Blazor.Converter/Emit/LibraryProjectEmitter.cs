using System.Text;

namespace WebForm2Blazor.Converter.Emit;

/// <summary>
/// One referenced class library, as it will be emitted: a directory under the output that
/// already holds that library's ported sources, plus what its .csproj has to say.
/// </summary>
public sealed record LibraryProject(string Name, string SourceDirectory)
{
    /// <summary>
    /// The assembly the original project produced, which is NOT always the directory name
    /// (n2cms builds N2.Extensions out of a directory called "Extensions"). Null when the
    /// source project could not say, in which case <see cref="Name"/> stands in.
    /// </summary>
    public string? AssemblyName { get; init; }

    /// <summary>The original project's RootNamespace, on the same terms as <see cref="AssemblyName"/>.</summary>
    public string? RootNamespace { get; init; }

    /// <summary>The <see cref="Name"/> of every other emitted library this one references.</summary>
    public List<string> References { get; init; } = [];

    /// <summary>Its ported sources contain the "unsafe" keyword.</summary>
    public bool AllowUnsafeBlocks { get; init; }

    /// <summary>Its ported sources declare assembly-level attributes the SDK also generates.</summary>
    public bool PortedAssemblyAttributes { get; init; }
}

/// <summary>
/// Writes one .csproj per referenced library, so the output reproduces the ASSEMBLY
/// BOUNDARIES of the source solution instead of merging everything into the web project.
///
/// The merge is not a neutral simplification. Three measured consequences of it:
///
/// - Mutually exclusive data providers (mojoPortal ships MSSQL / MySql / pgsql / SQLite,
///   each declaring the whole provider surface) cannot all be taken: together they are
///   2,052 CS0111 and 171 CS0101. The converter therefore takes NONE of them and asks the
///   user to choose, which is a decision separate projects never force.
/// - A "global using" covers a COMPILATION. YAF.Web's "global using System.IO;" reached
///   the vendored Lucene.Net sources once they shared a project, and "Directory" meant two
///   types - the whole of YAF.NET's CS0104 count. The converter works around it by turning
///   library global usings back into file-level usings.
/// - Namespaces that were never adjacent become adjacent, so a partially qualified name
///   can resolve somewhere the original could not reach. Nothing works around that one.
///
/// Splitting removes the first two problems by construction and makes the third
/// impossible. It is behind --split-projects rather than being the default because the
/// six-corpus baseline is the only thing that says whether a change helped, and a new
/// output shape has to be measured against that baseline, not fused with it.
/// </summary>
public static class LibraryProjectEmitter
{
    /// <summary>
    /// One XML comment line, with the two things XML forbids inside one taken out: a double
    /// hyphen anywhere, and a hyphen last.
    ///
    /// Not a hypothetical. The first version of this file wrote the flag's own name into
    /// the application's .csproj ("...(--split-projects)"), and MSBuild refused to LOAD the
    /// project: MSB4025, zero compiler diagnostics, and a build-error count of 1 that the
    /// gate could only read as a floor. A comment took the whole measurement down. The
    /// other half is the library's source path, which is not this converter's to choose.
    /// </summary>
    private static string Comment(string text)
    {
        // Looped, because one pass over "---" leaves "--" behind.
        while (text.Contains("--", StringComparison.Ordinal))
        {
            text = text.Replace("--", "-", StringComparison.Ordinal);
        }
        return "  <!-- " + text.TrimEnd('-') + " -->";
    }

    /// <summary>
    /// Emits <c>&lt;Name&gt;/&lt;Name&gt;.csproj</c> for each library.
    ///
    /// Plain Microsoft.NET.Sdk, deliberately: every converted .razor is written at the
    /// application level and the library directories hold nothing but .cs, so the Razor SDK
    /// would only add compilation passes with nothing to compile.
    /// </summary>
    /// <param name="packages">
    /// NuGet references. Given to every project rather than apportioned by use: a project
    /// missing a package it needs is a build error, a project carrying one it does not need
    /// is nothing at all. Apportioning would have to infer use from the ported code's
    /// usings, and being wrong in that direction costs errors that look like conversion
    /// defects.
    /// </param>
    /// <param name="analyzers">
    /// Source generators the ORIGINAL build ran, as assembly paths. Given to the libraries
    /// as well as to the application, because that is where the generated declarations are
    /// missing: DNN Platform's [DnnDeprecated] generator writes the defining half of every
    /// partial method in DotNetNuke.Library, not in the website. Attaching it only to the
    /// web project - which is all the merged output needed, there being one project - would
    /// hand the split output back the 230 CS0759 the analyzer was wired up to remove.
    /// </param>
    public static void Emit(
        string outputDirectory,
        IReadOnlyList<LibraryProject> libraries,
        string componentsProjectReference,
        IReadOnlyList<(string Id, string Version)> packages,
        IReadOnlyList<string> analyzers)
    {
        foreach (var library in libraries)
        {
            var directory = Path.Combine(outputDirectory, library.Name);
            Directory.CreateDirectory(directory);
            File.WriteAllText(
                Path.Combine(directory, library.Name + ".csproj"),
                Render(library, componentsProjectReference, packages, analyzers));

            var friends = FriendAssembliesOf(library.SourceDirectory);
            if (friends.Count > 0)
            {
                File.WriteAllText(Path.Combine(directory, FriendAssemblyFile), RenderFriends(friends));
            }
        }
    }

    /// <summary>The generated file carrying the friend declarations recovered below.</summary>
    public const string FriendAssemblyFile = "InternalsVisibleTo.g.cs";

    /// <summary>
    /// Friend assemblies the original project declared.
    ///
    /// Declaring one is how a solution lets sibling assemblies share "internal", and the
    /// port loses it twice over: an SDK project writes it as an
    /// &lt;InternalsVisibleTo&gt; ITEM in the .csproj, which the converter does not carry,
    /// and an older project writes it in AssemblyInfo.cs, which the converter deliberately
    /// does not port (the SDK generates its own assembly attributes and two copies of
    /// [assembly: AssemblyVersion] is CS0579).
    ///
    /// Merged into one assembly that cost nothing, because there was no "other assembly"
    /// left. Split apart it costs a great deal: YAF.NET's vendored Lucene declares
    /// Analyzer.CreateComponents "protected internal" in YAF.Lucene.Net and overrides it in
    /// YAF.Lucene.Net.Analysis.Common, which is legal only BETWEEN FRIENDS - elsewhere
    /// "protected internal" is merely "protected" and the override may not keep its
    /// modifier. 66 of YAF's 70 split-mode errors were this one lost declaration
    /// (47 CS0507 + 19 CS0122).
    ///
    /// Both sources are read. Of the .cs files only AssemblyInfo.cs, because that is
    /// precisely the file the port drops: taking the attribute out of a file that IS ported
    /// would declare it twice.
    ///
    /// The public key is cut off the name. It identifies a strong-named friend, the
    /// generated projects are not signed, and a key that cannot match makes the declaration
    /// silently grant nothing.
    /// </summary>
    public static List<string> FriendAssembliesOf(string sourceDirectory)
    {
        var friends = new List<string>();
        if (!Directory.Exists(sourceDirectory))
        {
            return friends;
        }

        void Add(string? value)
        {
            var name = value?.Split(',')[0].Trim();
            if (!string.IsNullOrEmpty(name)
                && !name.Contains('$')
                && !friends.Contains(name, StringComparer.Ordinal))
            {
                friends.Add(name);
            }
        }

        foreach (var projectPath in SafeFiles(sourceDirectory, "*.csproj", SearchOption.TopDirectoryOnly))
        {
            try
            {
                foreach (var item in System.Xml.Linq.XDocument.Load(projectPath).Descendants()
                             .Where(node => node.Name.LocalName == "InternalsVisibleTo"))
                {
                    Add(item.Attribute("Include")?.Value);
                }
            }
            catch (Exception exception) when (exception is System.Xml.XmlException or IOException)
            {
                // Unreadable project file: the .cs scan below may still find the friends.
            }
        }

        foreach (var file in SafeFiles(sourceDirectory, "AssemblyInfo.cs", SearchOption.AllDirectories))
        {
            string text;
            try
            {
                text = File.ReadAllText(file);
            }
            catch (IOException)
            {
                continue;
            }

            foreach (System.Text.RegularExpressions.Match match in
                     System.Text.RegularExpressions.Regex.Matches(
                         text,
                         @"\[\s*assembly\s*:\s*(?:System\.Runtime\.CompilerServices\.)?InternalsVisibleTo(?:Attribute)?\s*\(\s*""(?<name>[^""]+)"""))
            {
                Add(match.Groups["name"].Value);
            }
        }

        friends.Sort(StringComparer.Ordinal);
        return friends;
    }

    private static IEnumerable<string> SafeFiles(string directory, string pattern, SearchOption option)
    {
        try
        {
            return Directory.EnumerateFiles(directory, pattern, option).ToList();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return [];
        }
    }

    private static string RenderFriends(IReadOnlyList<string> friends)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// Generated by WebForm2Blazor.");
        builder.AppendLine("// 元のプロジェクトの AssemblyInfo.cs が宣言していた InternalsVisibleTo です。");
        builder.AppendLine("// AssemblyInfo.cs 自体は移植しません(SDK が生成する属性と重複するため)が、");
        builder.AppendLine("// この属性だけは移植しないとアセンブリ間の internal / protected internal が");
        builder.AppendLine("// 通らなくなるので、ここに再宣言します。");
        builder.AppendLine("// 厳密名の公開キーは落としています(生成プロジェクトは署名しないため)。");
        builder.AppendLine();
        foreach (var friend in friends)
        {
            builder.AppendLine(
                $"[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(\"{friend}\")]");
        }
        return builder.ToString();
    }

    private static string Render(
        LibraryProject library,
        string componentsProjectReference,
        IReadOnlyList<(string Id, string Version)> packages,
        IReadOnlyList<string> analyzers)
    {
        var builder = new StringBuilder();
        builder.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        builder.AppendLine();
        builder.AppendLine(Comment("変換元の " + library.SourceDirectory.Replace('\\', '/')
            + " に対応するクラスライブラリです。"));
        builder.AppendLine(Comment("元のソリューションで別アセンブリだったものを、別アセンブリのまま出力しています。"));
        builder.AppendLine("  <PropertyGroup>");
        builder.AppendLine("    <TargetFramework>net10.0</TargetFramework>");
        builder.AppendLine("    <Nullable>disable</Nullable>");
        builder.AppendLine("    <ImplicitUsings>disable</ImplicitUsings>");
        // The assembly keeps the name the original had, because ported code can name it:
        // Assembly.Load("YAF.Core") and [assembly: InternalsVisibleTo] both resolve by
        // string, and a renamed assembly fails at run time with nothing at build time.
        // The project FILE is named after the directory instead - that is the name the
        // rest of the converter already uses to say which project a file belongs to.
        builder.AppendLine($"    <AssemblyName>{library.AssemblyName ?? library.Name}</AssemblyName>");
        builder.AppendLine($"    <RootNamespace>{library.RootNamespace ?? library.Name}</RootNamespace>");
        builder.AppendLine("    <NoWarn>$(NoWarn);SYSLIB0011</NoWarn>");
        if (library.AllowUnsafeBlocks)
        {
            builder.AppendLine("    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>");
        }
        if (library.PortedAssemblyAttributes)
        {
            builder.AppendLine("    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>");
        }
        builder.AppendLine("  </PropertyGroup>");
        builder.AppendLine();

        builder.AppendLine("  <ItemGroup>");
        // Every library gets the compatibility layer. Its page base classes and controls
        // are exactly what these libraries declare - YAF.Core's ForumPage, DNN's
        // PortalModuleBase - so the reference is not incidental to a few of them.
        builder.AppendLine($"    <ProjectReference Include=\"{componentsProjectReference}\" />");
        foreach (var reference in library.References.OrderBy(name => name, StringComparer.Ordinal))
        {
            builder.AppendLine($"    <ProjectReference Include=\"..\\{reference}\\{reference}.csproj\" />");
        }
        builder.AppendLine("  </ItemGroup>");

        if (packages.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("  <ItemGroup>");
            foreach (var (id, version) in packages)
            {
                builder.AppendLine($"    <PackageReference Include=\"{id}\" Version=\"{version}\" />");
            }
            builder.AppendLine("  </ItemGroup>");
        }

        if (analyzers.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("  <ItemGroup>");
            foreach (var analyzer in analyzers)
            {
                builder.AppendLine($"    <Analyzer Include=\"{Path.GetFullPath(analyzer)}\" />");
            }
            builder.AppendLine("  </ItemGroup>");
        }

        builder.AppendLine();
        builder.AppendLine("</Project>");
        return builder.ToString();
    }

    /// <summary>
    /// Points the application's .csproj at the emitted libraries and stops it compiling
    /// their sources itself.
    ///
    /// The Remove items are not tidiness. An SDK project globs <c>**/*.cs</c> from its own
    /// directory DOWNWARD and knows nothing about a .csproj sitting in a subdirectory, so
    /// without them every library file would be compiled twice - once into its own assembly
    /// and once into the application's - and every type in them would be ambiguous. The
    /// Content / None removals cover the library's own bin and obj once it has been built,
    /// which the application's default globs would otherwise claim as duplicate items
    /// (NETSDK1022, an error that aborts the build before the compiler ever runs).
    /// </summary>
    public static void PointApplicationAtLibraries(
        string outputDirectory, string appName, IReadOnlyList<LibraryProject> libraries)
    {
        if (libraries.Count == 0)
        {
            return;
        }

        var csprojPath = Path.Combine(outputDirectory, appName + ".csproj");
        var builder = new StringBuilder();
        builder.AppendLine(Comment("参照ライブラリは独立したプロジェクトとして出力しています(split-projects)。"));
        builder.AppendLine(Comment("既定の glob は子ディレクトリの .csproj を除外しないので、明示的に外します。"));
        builder.AppendLine("  <ItemGroup>");
        foreach (var library in libraries)
        {
            builder.AppendLine($"    <Compile Remove=\"{library.Name}\\**\" />");
            builder.AppendLine($"    <Content Remove=\"{library.Name}\\**\" />");
            builder.AppendLine($"    <None Remove=\"{library.Name}\\**\" />");
            builder.AppendLine($"    <EmbeddedResource Remove=\"{library.Name}\\**\" />");
        }
        builder.AppendLine("  </ItemGroup>");
        builder.AppendLine();
        builder.AppendLine("  <ItemGroup>");
        foreach (var library in libraries)
        {
            builder.AppendLine($"    <ProjectReference Include=\"{library.Name}\\{library.Name}.csproj\" />");
        }
        builder.AppendLine("  </ItemGroup>");
        builder.AppendLine();

        File.WriteAllText(csprojPath, File.ReadAllText(csprojPath)
            .Replace("</Project>", builder.ToString() + "</Project>"));
    }

    /// <summary>
    /// Orders the libraries so that no project references one declared after it, and
    /// reports any edge that had to be dropped to make that possible.
    ///
    /// MSBuild refuses a circular ProjectReference outright - the build fails before
    /// compiling anything, which would turn a measurable result into no result at all. The
    /// source solution cannot contain a cycle either (MSBuild built it), so a cycle here
    /// means the directory-level collapse invented one: two projects in the same directory
    /// referencing each other become one node with a self-loop, and that is the converter's
    /// doing, not the application's.
    ///
    /// The closing edge is dropped rather than the split abandoned, and the drop is
    /// returned so the caller can say which reference is now missing. A missing reference
    /// shows up as CS0246 in a known place; a refused build shows up as nothing.
    /// </summary>
    public static List<(string From, string To)> BreakCycles(IReadOnlyList<LibraryProject> libraries)
    {
        var dropped = new List<(string From, string To)>();
        var byName = libraries.ToDictionary(library => library.Name, StringComparer.Ordinal);
        var state = new Dictionary<string, int>(StringComparer.Ordinal);

        void Visit(string name)
        {
            state[name] = 1;
            if (byName.TryGetValue(name, out var library))
            {
                foreach (var reference in library.References.ToList())
                {
                    var status = state.GetValueOrDefault(reference);
                    if (status == 1)
                    {
                        library.References.Remove(reference);
                        dropped.Add((name, reference));
                    }
                    else if (status == 0 && byName.ContainsKey(reference))
                    {
                        Visit(reference);
                    }
                }
            }
            state[name] = 2;
        }

        foreach (var library in libraries.OrderBy(library => library.Name, StringComparer.Ordinal))
        {
            if (state.GetValueOrDefault(library.Name) == 0)
            {
                Visit(library.Name);
            }
        }

        return dropped;
    }
}
