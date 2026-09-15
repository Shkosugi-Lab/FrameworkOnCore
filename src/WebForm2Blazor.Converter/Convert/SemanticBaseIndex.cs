using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Answers "does this class's base chain declare a member of this name?" by RESOLVING the
/// base, not by matching its written name.
///
/// Every other base lookup in this converter works on the name as written, which is all a
/// syntax tree offers. That is correct for the compatibility layer, where the converter
/// itself chose the base, and it has been wrong every time it was pointed at anything else:
/// DNN's own MembershipProvider read as ASP.NET's, n2's Page&lt;TPage&gt; read as the compat
/// Page, two different Tree classes read as one another. Trying it on FRAMEWORK bases took
/// the corpora from 46 build errors to 65 and 507 residuals to 580 in one run.
///
/// A real compilation has no such ambiguity. The application's own sources are compiled
/// against the framework this converter runs on, and C#'s own name resolution decides what
/// a base means - enclosing namespaces first, then usings, exactly as the original compiler
/// did.
///
/// The sources are the ORIGINAL ones, before rewriting. That is deliberate: the question is
/// what the base was on 4.8, and a rewritten base has already been redirected to the compat
/// layer, which the other lookups handle.
/// </summary>
internal sealed class SemanticBaseIndex
{
    private readonly CSharpCompilation? _compilation;

    private SemanticBaseIndex(CSharpCompilation? compilation) => _compilation = compilation;

    public static SemanticBaseIndex Build(IEnumerable<string> portedSources)
    {
        try
        {
            var trees = portedSources
                .Select(source => CodeBehindRewriter.ParseUnit(source).SyntaxTree)
                .ToList();

            // The same assembly set FrameworkTypeIndex reads: what this process runs on.
            // System.Web is not among them, which is the point - a base that only existed
            // on 4.8 does not resolve, and an unresolved base answers "cannot tell".
            var platform = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? string.Empty;
            var references = platform
                .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Where(path => path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) && File.Exists(path))
                .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
                .ToList();

            return new SemanticBaseIndex(CSharpCompilation.Create(
                "W2BSemanticProbe",
                trees,
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)));
        }
        catch (Exception exception) when (exception is IOException or BadImageFormatException)
        {
            // No compilation means no answers, which the callers already handle.
            return new SemanticBaseIndex(null);
        }
    }

    /// <summary>
    /// True / false when the base chain could be resolved all the way, null when it could
    /// not.
    ///
    /// Null is the important answer and the common one: most of these applications derive
    /// from System.Web types that are not here. Only a chain that resolves completely -
    /// every link a real type, ending at object - can support "the member is not there",
    /// and that is the only case where an override is dropped.
    /// </summary>
    public bool? BaseDeclaresMember(string metadataName, string memberName)
    {
        if (_compilation?.GetTypeByMetadataName(metadataName) is not { } type)
        {
            return null;
        }

        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (current.TypeKind == TypeKind.Error)
            {
                return null;
            }

            if (current.GetMembers(memberName).Length > 0)
            {
                return true;
            }
        }

        // Walked to object without finding it, and nothing on the way was unresolved.
        return type.BaseType is null ? null : false;
    }
}
