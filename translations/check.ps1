# Checks FrameworkOnCore's own translations (translations\<culture>\<assembly>.<culture>.resx) against the English
# resources they translate: every key is one the assembly has, the placeholders ({0}, {1}...) are the same, and the
# English in each entry's comment is still the assembly's (else the English changed since: the translation to review), and,
# where this machine has .NET Framework's language pack of the language, no translation is wider than its text there.
#
#   .\translations\check.ps1            # errors fail (exit 1); English changed since is a warning
param([switch]$Quiet)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
# The assemblies translated and where their English resources are.
$sources = @{
    'System.Web' = 'FrameworkOnCore.Runtime\src\WebFormsForCore.Web\System.Web.resx'
}

function Read-Resx($path) {
    $doc = New-Object Xml.XmlDocument
    $doc.PreserveWhitespace = $true
    $doc.Load($path)
    $entries = [ordered]@{}
    foreach ($data in $doc.SelectNodes('/root/data')) {
        $entries[$data.GetAttribute('name')] = @{ Value = $data.SelectSingleNode('value').InnerText; Comment = $(if ($c = $data.SelectSingleNode('comment')) { $c.InnerText } else { $null }) }
    }
    return $entries
}

# Display width: full-width characters 2, others 1.
function Width([string]$s) {
    $w = 0
    foreach ($c in $s.ToCharArray()) {
        $i = [int]$c
        if (($i -ge 0x1100 -and $i -le 0x115F) -or ($i -ge 0x2E80 -and $i -le 0xA4CF) -or ($i -ge 0xAC00 -and $i -le 0xD7A3) -or ($i -ge 0xF900 -and $i -le 0xFAFF) -or ($i -ge 0xFE30 -and $i -le 0xFE4F) -or ($i -ge 0xFF00 -and $i -le 0xFF60) -or ($i -ge 0xFFE0 -and $i -le 0xFFE6) -or ($i -ge 0x3000 -and $i -le 0x303F)) { $w += 2 } else { $w += 1 }
    }
    $w
}

# The widths of .NET Framework's own texts in a language, where this machine has its language pack (only their widths
# are used: a translation is not to be longer, so that pages keep their layout; the texts themselves are not read
# into the translations, see README.md).
function Get-FrameworkWidths([string]$assembly, [string]$culture) {
    $widths = @{}
    $main = Join-Path $env:windir "Microsoft.NET\Framework64\v4.0.30319\$assembly.dll"
    if (-not (Test-Path $main)) { return $widths }
    try {
        $manager = New-Object Resources.ResourceManager($assembly, [Reflection.Assembly]::LoadFile($main))
        $set = $manager.GetResourceSet([Globalization.CultureInfo]$culture, $true, $false)
        if ($set) { foreach ($entry in $set) { $widths[[string]$entry.Key] = Width ([string]$entry.Value) } }
    }
    catch { }
    return $widths
}

function Placeholders($text) { ([regex]::Matches($text, '\{\d+(,[^}]*)?(:[^}]*)?\}') | ForEach-Object Value | Sort-Object -Unique) -join ' ' }

$errors = 0; $warnings = 0
foreach ($file in Get-ChildItem (Join-Path $repo 'translations') -Recurse -Filter *.resx) {
    $culture = $file.Directory.Name
    $assembly = $file.Name -replace "\.$([regex]::Escape($culture))\.resx$", ''
    if (-not $sources.ContainsKey($assembly)) { Write-Host "ERROR $($file.Name): no English resources known for $assembly (check.ps1's sources)"; $errors++; continue }
    $english = Read-Resx (Join-Path $repo $sources[$assembly])
    $translated = Read-Resx $file.FullName
    $widths = Get-FrameworkWidths $assembly $culture
    foreach ($key in $translated.Keys) {
        $entry = $translated[$key]
        if (-not $english.Contains($key)) { Write-Host "ERROR $culture $key`: $assembly has no such resource"; $errors++; continue }
        $source = $english[$key].Value
        if ((Placeholders $source) -ne (Placeholders $entry.Value)) { Write-Host "ERROR $culture $key`: placeholders [$(Placeholders $source)] translated as [$(Placeholders $entry.Value)]"; $errors++ }
        if ($widths.ContainsKey($key) -and (Width $entry.Value) -gt $widths[$key]) { Write-Host "ERROR $culture $key`: wider than .NET Framework's text ($(Width $entry.Value) > $($widths[$key])): '$($entry.Value)'"; $errors++ }
        if ([string]::IsNullOrWhiteSpace($entry.Value)) { Write-Host "ERROR $culture $key`: empty"; $errors++ }
        if ($entry.Comment -ne $source) { Write-Host "WARN  $culture $key`: the English changed since the translation (comment: '$($entry.Comment)'; now: '$source')"; $warnings++ }
    }
    if (-not $Quiet) { Write-Host "$culture $assembly`: $($translated.Count) of $($english.Count) translated$(if ($widths.Count -eq 0) { ' (widths not checked: no .NET Framework language pack here)' })" }
}
Write-Host "$errors error(s), $warnings warning(s)"
exit ([int]($errors -gt 0))