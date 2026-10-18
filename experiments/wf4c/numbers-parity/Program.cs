using System.Globalization;
using FrameworkOnCore;

// FrameworkNumbers against .NET Framework's golden lines (framework-golden.ps1): each value formatted here, the lines
// that differ written out. With UseNls (as the converted applications on Windows), .NET's culture data is .NET Framework's.
var golden = File.ReadAllLines(args[0]);
var differ = 0;
var dotnetDiffer = 0;
var shown = 0;
foreach (var line in golden)
{
    var parts = line.Split('|', 5);
    var culture = parts[3].Length == 0 ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(parts[3]);
    var format = parts[2];
    string ours, dotnet;
    if (parts[0] == "d")
    {
        var v = BitConverter.Int64BitsToDouble(long.Parse(parts[1]));
        ours = Run(() => FrameworkNumbers.ToString(v, format, culture));
        dotnet = Run(() => v.ToString(format, culture));
    }
    else
    {
        var v = BitConverter.Int32BitsToSingle(int.Parse(parts[1]));
        ours = Run(() => FrameworkNumbers.ToString(v, format, culture));
        dotnet = Run(() => v.ToString(format, culture));
    }
    if (dotnet != parts[4]) dotnetDiffer++;
    if (ours == parts[4]) continue;
    differ++;
    if (shown++ < 60) Console.WriteLine($"{parts[0]} {(parts[0] == "d" ? BitConverter.Int64BitsToDouble(long.Parse(parts[1])).ToString("R") : BitConverter.Int32BitsToSingle(int.Parse(parts[1])).ToString("R"))} '{format}' {parts[3]}: framework [{parts[4]}] ours [{ours}] dotnet [{dotnet}]");
}
Console.WriteLine($"{golden.Length} cases: {differ} differ from .NET Framework (.NET itself: {dotnetDiffer})");
return differ == 0 ? 0 : 1;

static string Run(Func<string> f)
{
    try { return f(); }
    catch (Exception ex) { return "!" + ex.GetType().Name; }
}
