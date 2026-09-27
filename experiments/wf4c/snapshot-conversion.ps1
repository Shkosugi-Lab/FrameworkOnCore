# The converted sources of a corpus (convert-corpora.ps1's output), as hashes: taken before a change of the converter
# that should not change what it writes, and compared after (a refactoring).
#   .\experiments\wf4c\snapshot-conversion.ps1 -Only be,wt,n2,dnn -Save       # _linux\snapshot-<name>.json
#   .\experiments\wf4c\snapshot-conversion.ps1 -Only be,wt,n2,dnn            # compare: the files that differ
param([string[]]$Only = @('be', 'wt', 'mojo', 'yaf', 'dnn', 'n2'), [switch]$Save)
$sha = [Security.Cryptography.SHA256]::Create()
foreach ($name in $Only) {
    $root = Join-Path $PSScriptRoot $name
    if (-not (Test-Path $root)) { "=== ${name}: not converted"; continue }
    $hashes = [ordered]@{}
    Get-ChildItem $root -Recurse -File -Include *.cs, *.vb, *.csproj, *.vbproj, *.config, Program.cs |
        # App_Data\machine.config: written by the runtime when the application runs.
        Where-Object { $_.FullName -notmatch '\\(obj|bin|site|node_modules|\.git)\\' -and $_.FullName -notmatch '\\App_Data\\machine\.config$' } |
        Sort-Object FullName |
        ForEach-Object { $hashes[$_.FullName.Substring($root.Length + 1)] = [BitConverter]::ToString($sha.ComputeHash([IO.File]::ReadAllBytes($_.FullName))) }
    $file = Join-Path $PSScriptRoot "_linux\snapshot-$name.json"
    if ($Save) {
        $hashes | ConvertTo-Json | Set-Content $file -Encoding UTF8
        "=== ${name}: $($hashes.Count) files saved"
        continue
    }
    if (-not (Test-Path $file)) { "=== ${name}: no snapshot"; continue }
    $before = @{}
    (Get-Content $file -Raw -Encoding UTF8 | ConvertFrom-Json).PSObject.Properties | ForEach-Object { $before[$_.Name] = $_.Value }
    $changed = @($hashes.Keys | Where-Object { $before.ContainsKey($_) -and $before[$_] -ne $hashes[$_] })
    $added = @($hashes.Keys | Where-Object { -not $before.ContainsKey($_) })
    $removed = @($before.Keys | Where-Object { -not $hashes.Contains($_) })
    "=== ${name}: $($hashes.Count) files, changed $($changed.Count), added $($added.Count), removed $($removed.Count)"
    $changed + ($added | ForEach-Object { "+ $_" }) + ($removed | ForEach-Object { "- $_" }) | Select-Object -First 20 | ForEach-Object { "  $_" }
}
