# Checks FrameworkOnCore's own translations (translations\<culture>\<assembly>\<resources>.<culture>.resx) against the
# English resources they translate (sources.json: the assemblies, their resources and where their English is): every key
# is one the resources have, the placeholders ({0}, {1}...) are the same, and the English in each entry's comment is
# still the resources' (else the English changed since: the translation to review), and, where this machine has .NET
# Framework's language pack of the language, no translation is wider than its text there (an entry it has no text for,
# or an empty one: than the English, a short one's (under 12) plus 4).
#
#   .\translations\check.ps1            # errors fail (exit 1); English changed since is a warning
param([switch]$Quiet)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$sources = Get-Content (Join-Path $PSScriptRoot 'sources.json') -Raw -Encoding UTF8 | ConvertFrom-Json

function Read-Resx($path) {
    $doc = New-Object Xml.XmlDocument
    $doc.PreserveWhitespace = $true
    $doc.Load($path)
    $entries = [ordered]@{}
    foreach ($data in $doc.SelectNodes('/root/data')) {
        if ($data.GetAttribute('type') -or $data.GetAttribute('mimetype')) { continue }
        # Line ends as LF (a resx's CRLF is a character reference in some).
        $entries[$data.GetAttribute('name')] = @{ Value = $data.SelectSingleNode('value').InnerText.Replace("`r`n", "`n"); Comment = $(if ($c = $data.SelectSingleNode('comment')) { $c.InnerText.Replace("`r`n", "`n") } else { $null }) }
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

# The widths of .NET Framework's own texts in a language (its language pack's satellite of the assembly in the GAC), by
# name, where this machine has the pack (only their widths are used: a translation is not to be longer, so that pages
# keep their layout; the texts themselves are not read into the translations, see README.md): by resources ('' : the
# first of any). $null: no pack here. MVC, Web Pages and Web API (sources.json's package): Microsoft's language package's
# (Microsoft.AspNet.Mvc.ja: its satellite of the assembly) where the NuGet cache has it (README.md: how to get them).
function Get-FrameworkWidths($source, [string]$assemblyName, [string]$culture) {
    if ($source.package) {
        $packages = Join-Path $env:USERPROFILE ".nuget\packages\$($source.package.ToLowerInvariant()).$($culture.ToLowerInvariant())"
        $dll = Get-ChildItem $packages -Recurse -Filter "$assemblyName.resources.dll" -ErrorAction SilentlyContinue |
            Where-Object { $_.FullName -match '\\lib\\net4' } | Sort-Object FullName | Select-Object -Last 1
        if (-not $dll) { return $null }
    }
    else {
        # Not one of .NET Framework's (a package's: System.Web.Optimization, WebGrease): the English widths.
        if (-not $source.framework) { return @{ '' = @{} } }
        $folder = Join-Path $env:windir "Microsoft.NET\assembly\GAC_MSIL\$($source.framework).resources"
        $satellite = Get-ChildItem $folder -Directory -Filter "*_$($culture)_*" -ErrorAction SilentlyContinue | Select-Object -First 1
        if (-not $satellite) { return $null }
        $dll = Get-ChildItem $satellite.FullName -Filter *.dll | Select-Object -First 1
    }
    $widths = @{ '' = @{} }
    $assembly = [Reflection.Assembly]::ReflectionOnlyLoadFrom($dll.FullName)
    foreach ($name in $assembly.GetManifestResourceNames()) {
        if ($name -notlike '*.resources') { continue }
        try {
            $own = $widths[$name -replace "\.$([regex]::Escape($culture))\.resources$", ''] = @{}
            $reader = New-Object Resources.ResourceReader($assembly.GetManifestResourceStream($name))
            $e = $reader.GetEnumerator()
            while ($e.MoveNext()) {
                try {
                    if ($e.Value -isnot [string]) { continue }
                    $own[[string]$e.Key] = Width $e.Value
                    if (-not $widths[''].ContainsKey([string]$e.Key)) { $widths[''][[string]$e.Key] = $own[[string]$e.Key] }
                }
                catch { }
            }
            $reader.Close()
        }
        catch { }
    }
    return $widths
}

function Placeholders($text) { ([regex]::Matches($text, '\{\d+(,[^}]*)?(:[^}]*)?\}') | ForEach-Object Value | Sort-Object -Unique) -join ' ' }

$errors = 0; $warnings = 0
$frameworkWidths = @{}
foreach ($culture in Get-ChildItem (Join-Path $repo 'translations') -Directory | Where-Object { -not $_.Name.StartsWith('_') }) {
    foreach ($folder in Get-ChildItem $culture.FullName -Directory) {
        $assembly = $folder.Name
        $source = $sources.$assembly
        if (-not $source) { Write-Host "ERROR $($culture.Name)\$assembly`: not an assembly sources.json lists"; $errors++; continue }
        $key = "$($source.framework)|$($source.package)|$assembly|$($culture.Name)"
        if (-not $frameworkWidths.ContainsKey($key)) { $frameworkWidths[$key] = Get-FrameworkWidths $source $assembly $culture.Name }
        $widths = $frameworkWidths[$key]
        foreach ($file in Get-ChildItem $folder.FullName -Filter *.resx) {
            $resources = $file.Name -replace "\.$([regex]::Escape($culture.Name))\.resx$", ''
            $path = $source.resources.$resources
            if (-not $path) { Write-Host "ERROR $($file.Name): $assembly has no resources $resources (sources.json)"; $errors++; continue }
            $english = Read-Resx (Join-Path $repo $path)
            $translated = Read-Resx $file.FullName
            foreach ($name in $translated.Keys) {
                $entry = $translated[$name]
                if (-not $english.Contains($name)) { Write-Host "ERROR $($culture.Name) $resources $name`: no such resource"; $errors++; continue }
                $text = $english[$name].Value
                if ((Placeholders $text) -ne (Placeholders $entry.Value)) { Write-Host "ERROR $($culture.Name) $resources $name`: placeholders [$(Placeholders $text)] translated as [$(Placeholders $entry.Value)]"; $errors++ }
                if ($null -ne $widths) {
                    $own = if ($widths.ContainsKey($resources)) { $widths[$resources] } else { $widths[''] }
                    $limit = if ($own.ContainsKey($name) -and $own[$name] -gt 0) { $own[$name] } else { $w = Width $text; if ($w -lt 12) { $w + 4 } else { $w } }
                    if ((Width $entry.Value) -gt $limit) { Write-Host "ERROR $($culture.Name) $resources $name`: wider than .NET Framework's text ($(Width $entry.Value) > $limit): '$($entry.Value)'"; $errors++ }
                }
                if ([string]::IsNullOrWhiteSpace($entry.Value) -and -not [string]::IsNullOrWhiteSpace($text)) { Write-Host "ERROR $($culture.Name) $resources $name`: empty"; $errors++ }
                if ($entry.Comment -ne $text) { Write-Host "WARN  $($culture.Name) $resources $name`: the English changed since the translation (comment: '$($entry.Comment)'; now: '$text')"; $warnings++ }
            }
            if (-not $Quiet) { Write-Host "$($culture.Name) $resources`: $($translated.Count) of $($english.Count) translated$(if ($null -eq $widths) { ' (widths not checked: no .NET Framework language pack or language package here)' })" }
        }
    }
}
Write-Host "$errors error(s), $warnings warning(s)"
exit ([int]($errors -gt 0))
