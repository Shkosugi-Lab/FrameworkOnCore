using System.Globalization;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Analysis;

/// <summary>
/// An MSBuild condition evaluated as MSBuild evaluates it (learn.microsoft.com/visualstudio/msbuild/msbuild-conditions):
/// the properties of the project put in ($(Name), without regard to case; one not defined is empty; $(OS) is Windows_NT,
/// the original's build's), comparisons ==, != (text, without regard to case) and &lt;, &gt;, &lt;=, &gt;= (numbers),
/// "and" before "or", parentheses, "!", the functions Exists (relative to the project's folder) and HasTrailingSlash, a
/// bare true or false. What it cannot read (a property function, another operator) holds, and is told (unknown).
/// </summary>
public static class MSBuildCondition
{
    /// <summary>
    /// The properties a project's build has for a configuration, in order (a PropertyGroup's and a property's condition with
    /// the ones before: "$(DefineConstants);TRACE" builds on the one before), as the analysis and the converter read them.
    /// A condition it cannot read is reported (unknown) and holds.
    /// </summary>
    public static Dictionary<string, string> Properties(System.Xml.Linq.XElement project, string projectPath, string configuration, string platform,
        string solutionDirectory, Action<string>? unknown = null)
    {
        var directory = Path.GetDirectoryName(projectPath)!;
        var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Configuration"] = configuration, ["Platform"] = platform,
            ["SolutionDir"] = solutionDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar,
        };
        bool Holds(System.Xml.Linq.XElement element)
        {
            var condition = (string?)element.Attribute("Condition");
            if (string.IsNullOrWhiteSpace(condition)) return true;
            var holds = MSBuildCondition.Holds(condition, properties, directory, out var notRead);
            if (notRead) unknown?.Invoke(condition);
            return holds;
        }
        foreach (var group in project.Elements().Where(e => e.Name.LocalName == "PropertyGroup").Where(Holds))
            foreach (var property in group.Elements().Where(Holds))
                properties[property.Name.LocalName] = Expand(property.Value.Trim(), properties, directory, keepUnknown: true);
        return properties;
    }

    /// <summary>A value's $(Property) put in (one not defined: kept as it is, or empty, as a condition has it).</summary>
    public static string Expand(string value, IReadOnlyDictionary<string, string> properties, string directory, bool keepUnknown) =>
        Regex.Replace(value, @"\$\((?<name>[\w.]+)\)", m =>
        {
            var name = m.Groups["name"].Value;
            if (properties.TryGetValue(name, out var v)) return v;
            return name.ToLowerInvariant() switch
            {
                "msbuildprojectdirectory" or "msbuildthisfiledirectory" or "projectdir" => directory + Path.DirectorySeparatorChar,
                "os" => "Windows_NT",
                _ => keepUnknown ? m.Value : "",
            };
        });

    public static bool Holds(string condition, IReadOnlyDictionary<string, string> properties, string directory, out bool unknown)
    {
        var parser = new Parser(condition, properties, directory);
        try
        {
            var value = parser.Or();
            parser.SkipSpace();
            if (!parser.AtEnd) throw new FormatException("more after the condition");
            unknown = false;
            return value;
        }
        catch (FormatException)
        {
            unknown = true;
            return true;
        }
    }

    sealed class Parser(string text, IReadOnlyDictionary<string, string> properties, string directory)
    {
        int at;
        public bool AtEnd => at >= text.Length;

        public void SkipSpace() { while (at < text.Length && char.IsWhiteSpace(text[at])) at++; }

        bool Keyword(string word)
        {
            SkipSpace();
            if (string.Compare(text, at, word, 0, word.Length, StringComparison.OrdinalIgnoreCase) != 0) return false;
            var end = at + word.Length;
            if (end < text.Length && (char.IsLetterOrDigit(text[end]) || text[end] == '_')) return false;
            at = end;
            return true;
        }

        bool Symbol(string symbol)
        {
            SkipSpace();
            if (string.CompareOrdinal(text, at, symbol, 0, symbol.Length) != 0) return false;
            at += symbol.Length;
            return true;
        }

        public bool Or()
        {
            var value = And();
            while (Keyword("or")) value = And() | value;
            return value;
        }

        bool And()
        {
            var value = Not();
            while (Keyword("and")) value = Not() & value;
            return value;
        }

        bool Not()
        {
            SkipSpace();
            if (at < text.Length && text[at] == '!' && (at + 1 >= text.Length || text[at + 1] != '=')) { at++; return !Not(); }
            return Comparison();
        }

        bool Comparison()
        {
            if (Symbol("("))
            {
                var inner = Or();
                if (!Symbol(")")) throw new FormatException("')' missing");
                return inner;
            }
            var left = Operand(out var function);
            if (function is { } f) return f;
            foreach (var op in new[] { "==", "!=", "<=", ">=", "<", ">" })
            {
                if (!Symbol(op)) continue;
                var right = Operand(out var rightFunction);
                if (rightFunction != null) throw new FormatException("a function compared");
                return Compare(left, op, right);
            }
            SkipSpace();
            if (at < text.Length && text[at] is '=' or '~') throw new FormatException("an operator MSBuild has not");
            // A bare value: true or false (a property holding one).
            if (bool.TryParse(left.Trim(), out var boolean)) return boolean;
            if (left.Trim().Equals("on", StringComparison.OrdinalIgnoreCase) || left.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase)) return true;
            if (left.Trim().Equals("off", StringComparison.OrdinalIgnoreCase) || left.Trim().Equals("no", StringComparison.OrdinalIgnoreCase)) return false;
            throw new FormatException("not a boolean: " + left);
        }

        static bool Compare(string left, string op, string right)
        {
            if (op is "==" or "!=")
            {
                var equal = string.Equals(left.Trim(), right.Trim(), StringComparison.OrdinalIgnoreCase);
                return op == "==" ? equal : !equal;
            }
            if (!double.TryParse(left.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var a) ||
                !double.TryParse(right.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var b))
            {
                if (Version.TryParse(left.Trim(), out var va) && Version.TryParse(right.Trim(), out var vb)) return Ordered(va.CompareTo(vb), op);
                throw new FormatException("not numbers: " + left + " " + op + " " + right);
            }
            return Ordered(a.CompareTo(b), op);
        }

        static bool Ordered(int order, string op) => op switch { "<" => order < 0, ">" => order > 0, "<=" => order <= 0, _ => order >= 0 };

        // A quoted text, a bare word or $(Property), or a function (its value: function).
        string Operand(out bool? function)
        {
            function = null;
            SkipSpace();
            if (at >= text.Length) throw new FormatException("an operand missing");
            if (text[at] == '\'')
            {
                var end = text.IndexOf('\'', at + 1);
                if (end < 0) throw new FormatException("' missing");
                var quoted = text[(at + 1)..end];
                at = end + 1;
                return Expand(quoted);
            }
            if (text[at] == '$' && at + 1 < text.Length && text[at + 1] == '(')
            {
                var end = text.IndexOf(')', at);
                if (end < 0) throw new FormatException(") missing");
                var whole = text[at..(end + 1)];
                at = end + 1;
                return Expand(whole);
            }
            var start = at;
            while (at < text.Length && (char.IsLetterOrDigit(text[at]) || text[at] is '_' or '.' or '-')) at++;
            var word = text[start..at];
            if (word.Length == 0) throw new FormatException("an operand missing at " + start);
            if (Symbol("("))
            {
                var argument = Operand(out var nested);
                if (nested != null || !Symbol(")")) throw new FormatException("a function's argument");
                function = word.ToLowerInvariant() switch
                {
                    "exists" => argument.Trim().Length > 0 && (File.Exists(Path.Combine(directory, Native(argument.Trim()))) || Directory.Exists(Path.Combine(directory, Native(argument.Trim())))),
                    "hastrailingslash" => argument.EndsWith('\\') || argument.EndsWith('/'),
                    _ => throw new FormatException("a function MSBuild's conditions have not: " + word),
                };
                return "";
            }
            return word;
        }

        static string Native(string path) => path.Replace('\\', Path.DirectorySeparatorChar);

        string Expand(string value)
        {
            if (value.Contains("$([", StringComparison.Ordinal)) throw new FormatException("a property function");
            return Regex.Replace(value, @"\$\((?<name>[\w.]+)\)", m =>
            {
                var name = m.Groups["name"].Value;
                if (properties.TryGetValue(name, out var v)) return v;
                return name.ToLowerInvariant() switch
                {
                    "os" => "Windows_NT",
                    "msbuildprojectdirectory" or "msbuildthisfiledirectory" => directory,
                    "projectdir" => directory + Path.DirectorySeparatorChar,
                    _ => "",
                };
            });
        }
    }
}
