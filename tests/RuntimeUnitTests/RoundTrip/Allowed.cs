using System.Globalization;

namespace FrameworkOnCore.Runtime.Tests.RoundTrip;

/// <summary>
/// The differences from .NET Framework 4.8's round trips the tests allow, each with why: inherent to .NET (its collections,
/// number formatting, XmlSerializer, System.Data, compression; not the port's code) and harmless for a migrated site (what
/// 4.8 wrote is read as 4.8 read it). Kept small: a difference of the port's own code is fixed, not listed.
/// </summary>
static class Allowed
{
    /// <summary>Target: a format's name (a machine-key case's: its configuration and kind), a prefix of it ending with '*', or null.</summary>
    sealed record Rule(string Target, string Kind, Func<Difference, bool> Applies, string Why);

    static readonly string[] TextFormats = { "ResX", "ResXDataNodes", "ProfileString", "ProfileXml" };

    static readonly Rule[] rules =
    {
        new(null, "bytes", d => HasUnordered(Value(d)) || d.Target == "ViewStateControls",
            "a Hashtable's (HybridDictionary's, a StateBag's) entries in the order of their keys' hash codes: .NET's string hash codes are " +
            "randomized (each process its own), .NET Framework's were not; the same entries"),
        new(null, "bytes", d => Value(d).Contains("CultureInfo:", StringComparison.Ordinal) || Value(d).Contains("NameValueCollection{", StringComparison.Ordinal),
            "a CultureInfo (a NameValueCollection's comparer): .NET Framework wrote its CompareInfo and SortVersion (the machine's NLS " +
            "version); the port writes what .NET Framework makes it again from, its name (FrameworkSurrogates)"),
        new(null, "bytes", d => Value(d).Contains("DataTable ", StringComparison.Ordinal),
            "a DataTable or DataSet: System.Data (.NET's) writes its columns' types by .NET's assembly names in its XML schema " +
            "(msdata:DataType=\"System.Guid, System.Private.CoreLib\"), and an empty byte[] as <Data></Data>"),
        new(null, "bytes", d => Value(d).Contains("System.Collections.Generic.List`1[", StringComparison.Ordinal),
            "a List<T>: BinaryFormatter writes its array and version, whose capacity .NET's List grows otherwise (the same items)"),
        new(null, "bytes", d => Value(d).Contains("decimal:", StringComparison.Ordinal) &&
                                (Value(d).Contains("Pair(", StringComparison.Ordinal) || Value(d).Contains("Triplet(", StringComparison.Ordinal) || Value(d).Contains(".RoundTrip.", StringComparison.Ordinal)),
            "a decimal in a field of type object: .NET's System.Decimal is ISerializable, BinaryFormatter writes it as an object of " +
            "flags, hi, lo and mid (.NET Framework's fields, which it reads), .NET Framework wrote it as a primitive"),
        new("SessionStoreDeflate", "bytes", d => true,
            "<sessionState compressionEnabled>: .NET's DeflateStream (zlib-ng) compresses otherwise than .NET Framework's; either reads the other's"),
        new(null, "bytes", d => TextFormats.Contains(d.Target) && (Value(d).Contains("double:", StringComparison.Ordinal) || Value(d).Contains("float:", StringComparison.Ordinal)),
            "a double or float as text: .NET writes the shortest text that reads back the same value (\"0.30000000000000004\", \"-0\", " +
            "\"5E-324\"), .NET Framework 15 or 17 digits and lost some values (0.1 + 0.2 as \"0.3\", -0 as \"0\"); either reads the other's"),
        new(null, "round trip", d => TextFormats.Contains(d.Target) && (d.Input == "double:8000000000000000" || d.Input == "float:80000000") && d.Actual == d.Input,
            "-0.0 as text: .NET writes \"-0\" and reads it back, .NET Framework wrote \"0\" (the port keeps the value)"),
        new("ProfileXml", "bytes", d => true,
            "a profile property of serializeAs=\"Xml\": .NET's XmlSerializer declares xmlns:xsi before xmlns:xsd, serializes DateTimeOffset " +
            "and TimeSpan (.NET Framework's wrote an empty element), formats numbers as above and refuses the characters XML cannot hold"),
        new("ProfileXml", "round trip", d => d.Actual == d.Input,
            "a DateTimeOffset or TimeSpan profile property of serializeAs=\"Xml\": .NET's XmlSerializer keeps it, .NET Framework's lost it (default)"),
        // in a time zone other than the recorder's (Linux's containers: UTC)
        new(null, "bytes", d => !RoundTripTests.InRecorderZone && Value(d).Contains("DateTime:Local", StringComparison.Ordinal),
            "a local DateTime in another time zone than the recorder's: written as its instant (DateTime.ToBinary: ObjectStateFormatter, " +
            ".resources), which is another one here; read back the same local time, as .NET Framework in that zone"),
        new(null, null, d => !RoundTripTests.InRecorderZone && (d.Kind == "read" || d.Kind == "round trip") && Value(d).Contains("DataTable ", StringComparison.Ordinal),
            "a DataTable in another time zone than the recorder's: System.Data writes a DateTime column (DateTimeMode UnspecifiedLocal) " +
            "with the writer's offset and reads it as that instant's local time, here another one (as .NET Framework in another zone)"),
        new(null, null, d => (d.Kind == "value" || d.Kind == "read") && MadeError(d.Index) == typeof(System.ComponentModel.InvalidEnumArgumentException),
            "a DataTable of RemotingFormat = SerializationFormat.Binary: .NET refuses the binary remoting format (its setter and its " +
            "deserialization: InvalidEnumArgumentException), by design since .NET 5; the XML format (the default) is read"),
    };

    public static string Why(Difference d) =>
        rules.FirstOrDefault(r => (r.Target == null || r.Target == d.Target || (r.Target.EndsWith('*') && d.Target.StartsWith(r.Target[..^1], StringComparison.Ordinal)))
            && (r.Kind == null || r.Kind == d.Kind) && r.Applies(d))?.Why;

    // The value a difference is of, shown (a MAC'd LosFormatter case's: "value N", the Values' one).
    static string Value(Difference d)
    {
        if (d.Input.StartsWith("value ", StringComparison.Ordinal) && int.TryParse(d.Input[6..], NumberStyles.None, CultureInfo.InvariantCulture, out int index))
        {
            try { return Formats.Shown(Values.Make(index)); }
            catch (Exception) { return ""; }
        }
        return d.Input;
    }

    static Type MadeError(int index)
    {
        try { Values.Make(index); return null; }
        catch (Exception e) { return e.GetType(); }
    }

    /// <summary>Whether a shown value has a Hashtable or HybridDictionary of two entries or more.</summary>
    static bool HasUnordered(string shown)
    {
        foreach (var start in new[] { "Hashtable{", "HybridDictionary{" })
        {
            for (int at = shown.IndexOf(start, StringComparison.Ordinal); at >= 0; at = shown.IndexOf(start, at + 1, StringComparison.Ordinal))
            {
                int depth = 0;
                bool quoted = false;
                for (int i = at + start.Length - 1; i < shown.Length; i++)
                {
                    char c = shown[i];
                    if (quoted)
                    {
                        if (c == '\\') i++;
                        else if (c == '"') quoted = false;
                        continue;
                    }
                    if (c == '"') quoted = true;
                    else if (c == '{' || c == '[' || c == '(') depth++;
                    else if (c == '}' || c == ']' || c == ')') { if (--depth == 0) break; }
                    else if (c == ',' && depth == 1 && i + 1 < shown.Length && shown[i + 1] == ' ') return true;
                }
            }
        }
        return false;
    }
}
