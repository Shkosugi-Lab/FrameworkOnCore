namespace FrameworkOnCore.Runtime.Tests;

/// <summary>A test of what only Windows has (COM, IIS's listener adapter): skipped elsewhere, with the reason.</summary>
public sealed class WindowsFactAttribute : FactAttribute
{
    public WindowsFactAttribute(string why)
    {
        if (!OperatingSystem.IsWindows()) Skip = "Windows only: " + why;
    }
}
