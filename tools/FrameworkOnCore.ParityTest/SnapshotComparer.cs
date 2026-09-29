namespace FrameworkOnCore.ParityTest;

/// <summary>Compares golden and measured snapshots.</summary>
public static class SnapshotComparer
{
    public static List<string> Compare(Snapshot golden, Snapshot actual)
    {
        var problems = new List<string>();

        if (golden.Path != actual.Path)
        {
            problems.Add($"URL パス: 期待 '{golden.Path}' / 実際 '{actual.Path}'");
        }

        if (golden.Title != actual.Title)
        {
            problems.Add($"ページタイトル: 期待 '{golden.Title}' / 実際 '{actual.Title}'");
        }

        CompareTextLines(golden.Text, actual.Text, problems);
        CompareTables(golden.Tables, actual.Tables, problems);
        CompareInputs(golden.Inputs, actual.Inputs, problems);
        CompareElements(golden.Elements, actual.Elements, problems);

        return problems;
    }

    /// <summary>
    /// For elements present on both sides (matched by normalized ID), compares tag,
    /// presentation attributes, and style. This layer detects unreproduced "default
    /// rendering not written in markup" (GridView borders etc.).
    ///
    /// Elements only the CONVERTED side renders are reported: emitting markup the legacy
    /// app never produced is always a conversion defect (a tag-crossing &lt;% if %&gt;
    /// rendered unconditionally, for instance).
    /// The opposite direction stays silent on purpose - the conversion legitimately drops
    /// wrappers such as the UpdatePanel div.
    /// </summary>
    private static void CompareElements(
        Dictionary<string, ElementInfo> golden, Dictionary<string, ElementInfo> actual, List<string> problems)
    {
        if (golden is null || actual is null)
        {
            return;
        }

        foreach (var id in actual.Keys.Except(golden.Keys).OrderBy(key => key, StringComparer.Ordinal))
        {
            problems.Add($"要素 '{id}' (<{actual[id].Tag}>): 変換前には無い要素が描画されています");
        }

        foreach (var (id, expected) in golden.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            if (!actual.TryGetValue(id, out var got))
            {
                continue;
            }

            if (!string.Equals(expected.Tag, got.Tag, StringComparison.OrdinalIgnoreCase))
            {
                problems.Add($"要素 '{id}' のタグ: 期待 <{expected.Tag}> / 実際 <{got.Tag}>");
                continue;
            }

            CompareValueMap(id, "属性", expected.Attributes, got.Attributes, problems);
            CompareValueMap(id, "スタイル", expected.Style, got.Style, problems);
        }
    }

    private static void CompareValueMap(
        string id, string kind,
        Dictionary<string, string> expected, Dictionary<string, string> actual, List<string> problems)
    {
        foreach (var (name, expectedValue) in expected.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            if (!actual.TryGetValue(name, out var actualValue))
            {
                problems.Add($"要素 '{id}' の{kind} {name}: 期待 '{expectedValue}' / 実際 (なし)");
            }
            else if (!string.Equals(expectedValue, actualValue, StringComparison.Ordinal))
            {
                problems.Add($"要素 '{id}' の{kind} {name}: 期待 '{expectedValue}' / 実際 '{actualValue}'");
            }
        }

        foreach (var name in actual.Keys.Except(expected.Keys).OrderBy(key => key, StringComparer.Ordinal))
        {
            problems.Add($"要素 '{id}' の{kind} {name}: 期待 (なし) / 実際 '{actual[name]}'");
        }
    }

    private static void CompareTextLines(List<string> golden, List<string> actual, List<string> problems)
    {
        if (golden.SequenceEqual(actual))
        {
            return;
        }

        var index = 0;
        while (index < golden.Count && index < actual.Count && golden[index] == actual[index])
        {
            index++;
        }

        var expected = index < golden.Count ? golden[index] : "(行なし)";
        var got = index < actual.Count ? actual[index] : "(行なし)";
        problems.Add($"本文テキスト {index + 1} 行目から相違: 期待 '{expected}' / 実際 '{got}'"
                     + $"(期待 {golden.Count} 行 / 実際 {actual.Count} 行)");

        // Also summarize lines unique to either side (helps distinguish reordering from omission)
        var onlyInGolden = golden.Except(actual).Take(5).ToList();
        var onlyInActual = actual.Except(golden).Take(5).ToList();
        if (onlyInGolden.Count > 0)
        {
            problems.Add("  正解側にのみある行: " + string.Join(" / ", onlyInGolden.Select(line => $"'{line}'")));
        }
        if (onlyInActual.Count > 0)
        {
            problems.Add("  実測側にのみある行: " + string.Join(" / ", onlyInActual.Select(line => $"'{line}'")));
        }
    }

    private static void CompareTables(
        List<List<List<string>>> golden, List<List<List<string>>> actual, List<string> problems)
    {
        if (golden.Count != actual.Count)
        {
            problems.Add($"テーブル数: 期待 {golden.Count} / 実際 {actual.Count}");
            return;
        }

        for (var tableIndex = 0; tableIndex < golden.Count; tableIndex++)
        {
            var goldenTable = golden[tableIndex];
            var actualTable = actual[tableIndex];

            if (goldenTable.Count != actualTable.Count)
            {
                problems.Add($"テーブル{tableIndex + 1} の行数: 期待 {goldenTable.Count} / 実際 {actualTable.Count}");
                continue;
            }

            for (var rowIndex = 0; rowIndex < goldenTable.Count; rowIndex++)
            {
                var goldenRow = goldenTable[rowIndex];
                var actualRow = actualTable[rowIndex];

                if (!goldenRow.SequenceEqual(actualRow))
                {
                    problems.Add($"テーブル{tableIndex + 1} {rowIndex + 1} 行目: "
                                 + $"期待 [{string.Join(" | ", goldenRow)}] / 実際 [{string.Join(" | ", actualRow)}]");
                }
            }
        }
    }

    private static void CompareInputs(
        Dictionary<string, string> golden, Dictionary<string, string> actual, List<string> problems)
    {
        foreach (var (id, expected) in golden.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            if (!actual.TryGetValue(id, out var got))
            {
                problems.Add($"入力欄 '{id}': 実測側に存在しない");
            }
            else if (expected != got)
            {
                problems.Add($"入力欄 '{id}' の値: 期待 '{expected}' / 実際 '{got}'");
            }
        }

        foreach (var id in actual.Keys.Except(golden.Keys).OrderBy(key => key, StringComparer.Ordinal))
        {
            problems.Add($"入力欄 '{id}': 正解側に存在しない");
        }
    }
}
