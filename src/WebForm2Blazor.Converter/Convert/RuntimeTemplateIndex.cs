using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// The .cshtml templates an application compiles and renders ITSELF, found from the paths
/// its code hands to BuildManager.GetCompiledType.
///
/// BlogEngine renders every widget that way: WidgetZone builds
/// string.Format("{0}Custom/Widgets/{1}/widget.cshtml", ...) and passes it to its own
/// RazorHelpers.ParseRazor, which calls BuildManager.GetCompiledType. The converter copies
/// .cshtml as content (the Razor SDK cannot compile the rest of them - they are Web Pages
/// pages, not views), so those templates had nothing to compile them and every widget
/// rendered "Widget X not found".
///
/// Which .cshtml are templates is not a naming convention; it is what the code does with
/// them. So the answer is read from the code: the methods that call GetCompiledType, the
/// calls to those methods (and to GetCompiledType itself), and the path each call's first
/// argument evaluates to - a literal, a string.Format / interpolation of one, a local
/// initialized from one, or a constant. A placeholder matches one path segment. A path
/// built any other way is not followed; its template stays content, as before.
///
/// A page the application links to or serves ("~/admin/editors/tinymce/editor.cshtml")
/// never reaches GetCompiledType, so it is not taken.
/// </summary>
internal static class RuntimeTemplateIndex
{
    /// <summary>Patterns matching the app-relative path ("Custom/Widgets/Search/widget.cshtml") of a template.</summary>
    public static List<Regex> Find(IEnumerable<string> sources)
    {
        var units = sources
            .Where(source => source.Contains(".cshtml", StringComparison.OrdinalIgnoreCase)
                             || source.Contains("GetCompiledType", StringComparison.Ordinal))
            .Select(source => CodeBehindRewriter.ParseUnit(source))
            .ToList();

        // Methods that compile a path: GetCompiledType itself, and every method whose body
        // calls it with one of its own parameters (RazorHelpers.ParseRazor).
        var compilers = new HashSet<string>(StringComparer.Ordinal) { "GetCompiledType" };
        foreach (var method in units.SelectMany(unit => unit.DescendantNodes().OfType<MethodDeclarationSyntax>()))
        {
            var parameters = method.ParameterList.Parameters.Select(parameter => parameter.Identifier.Text).ToHashSet(StringComparer.Ordinal);
            if (method.DescendantNodes().OfType<InvocationExpressionSyntax>()
                .Any(call => CalledName(call) == "GetCompiledType"
                             && call.ArgumentList.Arguments.FirstOrDefault()?.Expression is IdentifierNameSyntax argument
                             && (parameters.Contains(argument.Identifier.Text) || LocalFromParameter(method, argument.Identifier.Text, parameters))))
            {
                compilers.Add(method.Identifier.Text);
            }
        }

        // Constants anywhere in the code ("RAZOR_HOST_PAGE_VPATH"), by simple name.
        var constants = new Dictionary<string, ExpressionSyntax>(StringComparer.Ordinal);
        foreach (var field in units.SelectMany(unit => unit.DescendantNodes().OfType<FieldDeclarationSyntax>())
                     .Where(field => field.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ConstKeyword))))
        {
            foreach (var variable in field.Declaration.Variables.Where(variable => variable.Initializer is not null))
            {
                constants.TryAdd(variable.Identifier.Text, variable.Initializer!.Value);
            }
        }

        var patterns = new Dictionary<string, Regex>(StringComparer.Ordinal);
        foreach (var call in units.SelectMany(unit => unit.DescendantNodes().OfType<InvocationExpressionSyntax>()))
        {
            if (CalledName(call) is not { } name || !compilers.Contains(name)
                || call.ArgumentList.Arguments.FirstOrDefault()?.Expression is not { } argument)
            {
                continue;
            }

            if (PathTemplate(argument, call, constants) is { } template
                && template.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
            {
                patterns.TryAdd(template, ToRegex(template));
            }
        }

        return [.. patterns.Values];
    }

    /// <summary>
    /// ASP.NET Core Razor directive names that were plain identifiers in Web Pages.
    /// BlogEngine's PageList widget loops "foreach (var page in ...)" and writes
    /// "@page.Title" - to ASP.NET Core Razor that is the @page directive (RZ3906, RZ2005).
    /// </summary>
    private static readonly Regex CoreDirectiveName = new(
        @"(?<!@)@(page|model|inject|namespace|attribute|implements|addTagHelper|removeTagHelper|tagHelperPrefix|preservewhitespace|rendermode|typeparam)\b",
        RegexOptions.CultureInvariant);

    /// <summary>
    /// Rewrites an implicit expression that starts with one of those names as an explicit
    /// one, so it means what it meant: "@page.Title" -> "@(page.Title)". The implicit
    /// expression is read the way Razor reads one - identifier, then ".name", "(...)" or
    /// "[...]" - and nothing after it is touched.
    /// </summary>
    public static string EscapeCoreDirectiveNames(string template)
    {
        var builder = new System.Text.StringBuilder(template.Length);
        var position = 0;
        foreach (Match match in CoreDirectiveName.Matches(template))
        {
            if (match.Index < position)
            {
                continue;
            }

            var end = match.Index + match.Length;
            while (end < template.Length)
            {
                if (template[end] == '.' && end + 1 < template.Length
                    && (char.IsLetter(template[end + 1]) || template[end + 1] == '_'))
                {
                    end++;
                    while (end < template.Length && (char.IsLetterOrDigit(template[end]) || template[end] == '_'))
                    {
                        end++;
                    }
                }
                else if (template[end] is '(' or '[')
                {
                    var close = template[end] == '(' ? ')' : ']';
                    var open = template[end];
                    var depth = 0;
                    var scan = end;
                    for (; scan < template.Length; scan++)
                    {
                        if (template[scan] == open)
                        {
                            depth++;
                        }
                        else if (template[scan] == close && --depth == 0)
                        {
                            break;
                        }
                    }
                    if (scan >= template.Length)
                    {
                        break;
                    }
                    end = scan + 1;
                }
                else
                {
                    break;
                }
            }

            builder.Append(template, position, match.Index - position);
            builder.Append("@(").Append(template, match.Index + 1, end - match.Index - 1).Append(')');
            position = end;
        }
        builder.Append(template, position, template.Length - position);
        return builder.ToString();
    }

    private static string? CalledName(InvocationExpressionSyntax call) => call.Expression switch
    {
        MemberAccessExpressionSyntax access => access.Name.Identifier.Text,
        IdentifierNameSyntax identifier => identifier.Identifier.Text,
        _ => null,
    };

    private static bool LocalFromParameter(MethodDeclarationSyntax method, string local, HashSet<string> parameters)
        => method.DescendantNodes().OfType<VariableDeclaratorSyntax>()
            .Any(variable => variable.Identifier.Text == local
                             && variable.Initializer?.Value is IdentifierNameSyntax source
                             && parameters.Contains(source.Identifier.Text));

    /// <summary>
    /// The path an expression evaluates to, with every part not known at conversion time
    /// written as "{}". Null when the expression is not one of the shapes followed.
    /// </summary>
    private static string? PathTemplate(ExpressionSyntax expression, SyntaxNode site, Dictionary<string, ExpressionSyntax> constants, int depth = 0)
    {
        if (depth > 4)
        {
            return null;
        }

        switch (expression)
        {
            case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression):
                return literal.Token.ValueText;

            case InterpolatedStringExpressionSyntax interpolated:
                return string.Concat(interpolated.Contents.Select(content => content switch
                {
                    InterpolatedStringTextSyntax text => text.TextToken.ValueText,
                    _ => "{}",
                }));

            // string.Format("...{0}...", ...): the placeholders are the unknown parts.
            case InvocationExpressionSyntax format
                when format.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "Format" }
                     && format.ArgumentList.Arguments.FirstOrDefault()?.Expression is LiteralExpressionSyntax formatLiteral
                     && formatLiteral.IsKind(SyntaxKind.StringLiteralExpression):
                return Regex.Replace(formatLiteral.Token.ValueText, @"\{\d+(,[^}]*)?(:[^}]*)?\}", "{}");

            // A constant, by name or qualified (RazorHelpers.RAZOR_HOST_PAGE_VPATH).
            case IdentifierNameSyntax or MemberAccessExpressionSyntax
                when (expression is MemberAccessExpressionSyntax access ? access.Name.Identifier.Text : ((IdentifierNameSyntax)expression).Identifier.Text) is { } name
                     && constants.TryGetValue(name, out var constant):
                return PathTemplate(constant, site, constants, depth + 1);

            // A local declared in the same method: "string vPath = string.Format(...)".
            case IdentifierNameSyntax local
                when site.Ancestors().OfType<BaseMethodDeclarationSyntax>().FirstOrDefault() is { } method
                     && method.DescendantNodes().OfType<VariableDeclaratorSyntax>()
                         .FirstOrDefault(variable => variable.Identifier.Text == local.Identifier.Text) is { Initializer.Value: { } initial }:
                return PathTemplate(initial, site, constants, depth + 1);

            default:
                return null;
        }
    }

    /// <summary>
    /// "{}Custom/Widgets/{}/widget.cshtml" -> matches "Custom/Widgets/Search/widget.cshtml".
    /// A leading "~/" or "/" is the application root; an unknown part is one path segment,
    /// except a LEADING one, which is the root itself (Utils.ApplicationRelativeWebRoot).
    /// </summary>
    private static Regex ToRegex(string template)
    {
        var path = template.Replace('\\', '/');
        path = path.StartsWith("~/", StringComparison.Ordinal) ? path[2..] : path.TrimStart('/');
        path = path.StartsWith("{}", StringComparison.Ordinal) ? path[2..].TrimStart('/') : path;

        var pattern = string.Join("[^/]*", path.Split("{}").Select(Regex.Escape));
        return new Regex("^" + pattern + "$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }
}
