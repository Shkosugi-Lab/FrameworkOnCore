# .NET Framework's text of doubles and floats (Windows PowerShell 5.1 runs on .NET Framework 4.8): every value of the
# list in every format and culture, as "kind|bits|format|culture|text" lines, the golden data of numbers-parity.ps1.
param([Parameter(Mandatory = $true)][string]$Out)

$values = New-Object System.Collections.Generic.List[double]
foreach ($v in @(0.0, 1.0, -1.0, 0.1, 0.2, 0.3, 0.5, 1.5, 2.5, -2.5, 0.125, 0.375, 2.675, 1.005, 1.115, 22.5, 23.5, -0.004, -0.001,
        -0.0001, 0.004, 0.0049999, 0.005, 0.015, 0.025, 0.045, 1234.5678, -1234567.891, 123456789012345678.0, 1e15, 1e16, 1e17,
        1e21, 1e22, 1e28, 7.9e28, 8e28, 1e29, 1e30, 1.7976931348623157e308, 5e-324, 1e-5, 1e-7, 1e-20, -1e-20, 1e-28, 1e-29, 1e-30,
        9.995, 99.995, 999999999999999.0, 9999999999999999.0, 0.000123456, 12345.6789e10, 33.333333333333336, 66.66666666666667,
        0.7, 0.07, 1.1, 1.21, 110.00000000000001, 0.30000000000000004, (0.1 + 0.7), (3.0 * 1.1), (100.0 / 3), (2.0 / 3), (1.0 / 3), -(1.0 / 3),
        ([Math]::PI), ([Math]::E), ([Math]::Sqrt(2)), 0.585, 4.35, 8.325, 1.45, 2.345, 1234567.125, (1e14 + 0.5), 4503599627370497.0)) {
    $values.Add($v)
}
$random = New-Object System.Random 20261019
$bytes = New-Object byte[] 8
for ($i = 0; $i -lt 500; $i++) {
    $random.NextBytes($bytes)
    $v = [BitConverter]::ToDouble($bytes, 0)
    if (-not [double]::IsNaN($v) -and -not [double]::IsInfinity($v)) { $values.Add($v) }
}
for ($i = 0; $i -lt 1000; $i++) {
    # Prices and quantities: a few decimals, sums of them.
    $values.Add([Math]::Round($random.NextDouble() * [Math]::Pow(10, $random.Next(0, 8)), $random.Next(0, 4)) * ($(if ($random.Next(2) -eq 0) { 1 } else { -1 })))
    $values.Add(($random.Next(0, 100000) / 100.0) + ($random.Next(0, 1000) / 1000.0))
}
$values.Add(-0.0 * 1)   # negative zero
$values.Add([BitConverter]::Int64BitsToDouble([long]::MinValue))

$doubleFormats = @('', 'G', 'g', 'R', 'G1', 'G2', 'G5', 'G10', 'G15', 'G16', 'G17', 'E', 'e', 'E0', 'E2', 'E10', 'E14', 'E15', 'E16',
    'F', 'F0', 'F1', 'F2', 'F3', 'F5', 'F10', 'F20', 'N', 'N0', 'N2', 'N4', 'C', 'C0', 'C2', 'C3', 'P', 'P0', 'P1', 'P4',
    '0', '0.0', '0.00', '0.###', '#,##0.00', '#,#', '00000.000', '0.00;(0.00);zero', '0.0E+0', '0.###e-00', '#%', ('0.00' + [char]0x2030),
    '#.##', '#', '0.0000000000000000000', "'x'0.0", '0,,', '0.00 \#')
$cultures = @('en-US', 'ja-JP', 'de-DE', 'fr-FR', 'en-IN', 'ar-SA', 'sv-SE', 'it-CH', '')

$lines = New-Object System.Collections.Generic.List[string]
foreach ($c in $cultures) {
    $culture = if ($c) { [Globalization.CultureInfo]::GetCultureInfo($c) } else { [Globalization.CultureInfo]::InvariantCulture }
    foreach ($v in $values) {
        $bits = [BitConverter]::DoubleToInt64Bits($v)
        foreach ($f in $doubleFormats) {
            try { $t = $v.ToString($f, $culture) } catch { $t = '!' + $_.Exception.InnerException.GetType().Name }
            $lines.Add("d|$bits|$f|$c|$t")
        }
        $s = [float]$v
        $sbits = [BitConverter]::ToInt32([BitConverter]::GetBytes($s), 0)
        foreach ($f in $doubleFormats) {
            try { $t = $s.ToString($f, $culture) } catch { $t = '!' + $_.Exception.InnerException.GetType().Name }
            $lines.Add("f|$sbits|$f|$c|$t")
        }
    }
}
[IO.File]::WriteAllLines($Out, $lines, (New-Object Text.UTF8Encoding $false))
"$($lines.Count) lines -> $Out"
