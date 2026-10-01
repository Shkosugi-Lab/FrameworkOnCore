# Publishes FrameworkOnCore's packages (experiments/wf4c/_feed, made by pack-frameworkoncore.ps1) as the GitHub Release
# frameworkoncore-<version> of this repository, the asset frameworkoncore-feed-<version>.zip: the converter, the analysis and Studio fetch it
# when their feed does not have the version (src/FrameworkOnCore.Converter/RuntimeSetup.cs), so users build nothing.
#
#   .\experiments\wf4c\pack-frameworkoncore.ps1 -Build All
#   .\experiments\wf4c\publish-frameworkoncore.ps1                 # the version of rules/packages.json (frameworkOnCoreVersion)
#
# GitHub Actions does this on a push that changes FrameworkOnCore's WebFormsForCore (.github/workflows/frameworkoncore-packages.yml): built from FrameworkOnCore.Runtime/,
# tested on Linux, published when the version has no release yet.
#
# A version is published once: changed packages is a new version (pack-frameworkoncore.ps1 -Version, rules/packages.json frameworkOnCoreVersion).
param(
    [string]$Version,
    [string]$Repository = 'Shkosugi-Lab/FrameworkOnCore'
)

$ErrorActionPreference = 'Stop'
$repo = Resolve-Path (Join-Path $PSScriptRoot '..\..')
if (-not $Version) {
    # The rules have comments (ConvertFrom-Json of Windows PowerShell does not take them): the one property.
    $rules = Get-Content (Join-Path $repo 'src\FrameworkOnCore.Converter\rules\packages.json') -Raw
    $Version = [regex]::Match($rules, '"frameworkOnCoreVersion"\s*:\s*"([^"]+)"').Groups[1].Value
}
$feed = Join-Path $PSScriptRoot '_feed'
$packages = @(Get-ChildItem $feed -File | Where-Object { $_.Name -like "*.$Version.nupkg" -or $_.Name -like "*.$Version.snupkg" })
if (-not ($packages | Where-Object Name -eq "FrameworkOnCore.Web.$Version.nupkg")) { throw "FrameworkOnCore.Web.$Version.nupkg is not in $feed (pack-frameworkoncore.ps1)" }

$staging = Join-Path ([IO.Path]::GetTempPath()) "frameworkoncore-feed-$Version"
if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
New-Item -ItemType Directory $staging | Out-Null
$packages | Copy-Item -Destination $staging
# WebFormsForCore is MIT: its notice goes with the packages.
Copy-Item (Join-Path $repo 'FrameworkOnCore.Runtime\LICENSE') (Join-Path $staging 'LICENSE-WebFormsForCore.txt')
$zip = Join-Path ([IO.Path]::GetTempPath()) "frameworkoncore-feed-$Version.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }
Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $zip

# Upstream's commit FrameworkOnCore.Runtime/ was last taken from (git subtree add/pull record it; WebFormsForCore/ was its
# first name, as subtree add recorded it).
$upstream = [regex]::Match((git -C $repo log -1 -E --grep='^git-subtree-dir: (WebFormsForCore|FrameworkOnCore\.Runtime)$' --format=%B), 'git-subtree-split: (\w+)').Groups[1].Value
$notes = @"
FrameworkOnCore's packages ($Version), as the converter uses them: WebFormsForCore
(https://github.com/webformsforcore/WebFormsForCore, MIT; LICENSE-WebFormsForCore.txt), taken in at $upstream and
maintained in FrameworkOnCore.Runtime/ of this repository: built from it at this tag. The converter, the analysis and Studio
fetch frameworkoncore-feed-$Version.zip into experiments/wf4c/_feed when it does not have them.
"@
gh release create "frameworkoncore-$Version" $zip --repo $Repository --title "FrameworkOnCore packages $Version" --notes $notes --target (git -C $repo rev-parse HEAD)
if ($LASTEXITCODE -ne 0) { throw 'gh release create failed' }
Remove-Item $staging -Recurse -Force
"published frameworkoncore-$Version ($($packages.Count) files) -> $Repository"
