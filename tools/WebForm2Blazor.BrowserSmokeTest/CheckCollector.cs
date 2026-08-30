namespace WebForm2Blazor.BrowserSmokeTest;

/// <summary>Collects verification results and produces the final report.</summary>
public sealed class CheckCollector
{
    private readonly List<string> _failures = [];
    private int _passed;

    public bool HasFailures => _failures.Count > 0;

    public void Fail(string message) => _failures.Add(message);

    public void True(string label, bool condition, string detail = "")
    {
        if (condition)
        {
            _passed++;
            Console.WriteLine($"  OK   {label}");
        }
        else
        {
            var message = string.IsNullOrEmpty(detail) ? label : $"{label}: {detail}";
            _failures.Add(message);
            Console.WriteLine($"  NG   {message}");
        }
    }

    public void Equal(string label, string actual, string expected)
        => True(label, actual.Trim() == expected, $"期待 '{expected}' / 実際 '{actual.Trim()}'");

    public void Contains(string label, string actual, string expectedSubstring)
        => True(label, actual.Contains(expectedSubstring, StringComparison.Ordinal),
            $"'{expectedSubstring}' を含まない / 実際 '{actual.Trim()}'");

    public string Report()
    {
        if (!HasFailures)
        {
            return $"RESULT: OK ({_passed} 項目すべて実ブラウザで確認)";
        }

        var lines = new List<string> { $"RESULT: FAIL (成功 {_passed} / 失敗 {_failures.Count})" };
        lines.AddRange(_failures.Select(failure => "  - " + failure));
        return string.Join(Environment.NewLine, lines);
    }
}
