# Checks FrameworkOnCore's own translations (translations\<culture>\<assembly>.<culture>.resx) against the English
# resources they translate: every key is one the assembly has, the placeholders ({0}, {1}...) are the same, and the
# English in each entry's comment is still the assembly's (else the English changed since: the translation to review).
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

function Placeholders($text) { ([regex]::Matches($text, '\{\d+(,[^}]*)?(:[^}]*)?\}') | ForEach-Object Value | Sort-Object -Unique) -join ' ' }

$errors = 0; $warnings = 0
foreach ($file in Get-ChildItem (Join-Path $repo 'translations') -Recurse -Filter *.resx) {
    $culture = $file.Directory.Name
    $assembly = $file.Name -replace "\.$([regex]::Escape($culture))\.resx$", ''
    if (-not $sources.ContainsKey($assembly)) { Write-Host "ERROR $($file.Name): no English resources known for $assembly (check.ps1's sources)"; $errors++; continue }
    $english = Read-Resx (Join-Path $repo $sources[$assembly])
    $translated = Read-Resx $file.FullName
    foreach ($key in $translated.Keys) {
        $entry = $translated[$key]
        if (-not $english.Contains($key)) { Write-Host "ERROR $culture $key`: $assembly has no such resource"; $errors++; continue }
        $source = $english[$key].Value
        if ((Placeholders $source) -ne (Placeholders $entry.Value)) { Write-Host "ERROR $culture $key`: placeholders [$(Placeholders $source)] translated as [$(Placeholders $entry.Value)]"; $errors++ }
        if ([string]::IsNullOrWhiteSpace($entry.Value)) { Write-Host "ERROR $culture $key`: empty"; $errors++ }
        if ($entry.Comment -ne $source) { Write-Host "WARN  $culture $key`: the English changed since the translation (comment: '$($entry.Comment)'; now: '$source')"; $warnings++ }
    }
    if (-not $Quiet) { Write-Host "$culture $assembly`: $($translated.Count) of $($english.Count) translated" }
}
Write-Host "$errors error(s), $warnings warning(s)"
exit ([int]($errors -gt 0))