# Publishes the WebFormsForCore fork's packages (experiments/wf4c/_feed, made by pack-fork.ps1) as the GitHub Release
# fork-<version> of this repository, the asset fork-feed-<version>.zip: the converter, the analysis and Studio fetch it
# when their feed does not have the version (src/FrameworkOnCore.Converter/RuntimeSetup.cs), so users build nothing.
#
#   .\experiments\wf4c\pack-fork.ps1 -Build All
#   .\experiments\wf4c\publish-fork.ps1                 # the version of rules/packages.json (forkVersion)
#
# GitHub Actions does this on a push that changes the fork (.github/workflows/fork.yml): built from WebFormsForCore/,
# tested on Linux, published when the version has no release yet.
#
# A version is published once: a changed fork is a new version (pack-fork.ps1 -Version, rules/packages.json forkVersion).
param(
    [string]$Version,
    [string]$Repository = 'Shkosugi-Lab/FrameworkOnCore'
)

$ErrorActionPreference = 'Stop'
$repo = Resolve-Path (Join-Path $PSScriptRoot '..\..')
if (-not $Version) {
    # The rules have comments (ConvertFrom-Json of Windows PowerShell does not take them): the one property.
    $rules = Get-Content (Join-Path $repo 'src\FrameworkOnCore.Converter\rules\packages.json') -Raw
    $Version = [regex]::Match($rules, '"forkVersion"\s*:\s*"([^"]+)"').Groups[1].Value
}
$feed = Join-Path $PSScriptRoot '_feed'
$packages = @(Get-ChildItem $feed -File | Where-Object { $_.Name -like "*.$Version.nupkg" -or $_.Name -like "*.$Version.snupkg" })
if (-not ($packages | Where-Object Name -eq "WebFormsForCore.Web.$Version.nupkg")) { throw "WebFormsForCore.Web.$Version.nupkg is not in $feed (pack-fork.ps1)" }

$staging = Join-Path ([IO.Path]::GetTempPath()) "fork-feed-$Version"
if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
New-Item -ItemType Directory $staging | Out-Null
$packages | Copy-Item -Destination $staging
# WebFormsForCore is MIT: its notice goes with the packages.
Copy-Item (Join-Path $repo 'WebFormsForCore\LICENSE') (Join-Path $staging 'LICENSE-WebFormsForCore.txt')
$zip = Join-Path ([IO.Path]::GetTempPath()) "fork-feed-$Version.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }
Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $zip

# Upstream's commit WebFormsForCore/ was last taken from (git subtree add/pull record it).
$upstream = [regex]::Match((git -C $repo log -1 --grep='^git-subtree-dir: WebFormsForCore$' --format=%B), 'git-subtree-split: (\w+)').Groups[1].Value
$notes = @"
The WebFormsForCore fork's packages ($Version), as the converter uses them: WebFormsForCore
(https://github.com/webformsforcore/WebFormsForCore, MIT; LICENSE-WebFormsForCore.txt), taken in at $upstream and
maintained in WebFormsForCore/ of this repository: built from it at this tag. The converter, the analysis and Studio
fetch fork-feed-$Version.zip into experiments/wf4c/_feed when it does not have them.
"@
gh release create "fork-$Version" $zip --repo $Repository --title "WebFormsForCore fork $Version" --notes $notes --target (git -C $repo rev-parse HEAD)
if ($LASTEXITCODE -ne 0) { throw 'gh release create failed' }
Remove-Item $staging -Recurse -Force
"published fork-$Version ($($packages.Count) files) -> $Repository"
