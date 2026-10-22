# FrameworkOnCore's package version, written in one place: src/FrameworkOnCore.Converter/rules/packages.json
# (frameworkOnCoreVersion). The scripts dot-source this file; the projects import FrameworkOnCoreVersion.props, which
# reads the same place.
#
#   . (Join-Path $PSScriptRoot 'frameworkoncore-version.ps1')
#   $version = Get-FrameworkOnCoreVersion
#   Assert-FrameworkOnCoreVersionUnchanged $version     # before packing it
#
# A version is published once (the GitHub Release frameworkoncore-<version>, its tag at the commit it was built from).
# Packing a published version whose packages' sources changed since gives packages other than the published ones under
# its number: this machine runs the new ones, another machine (the release's) the old ones. That stops the pack.

$FrameworkOnCoreRepository = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path

# What the packages are built from (pack-frameworkoncore.ps1 packs FrameworkOnCore.Runtime's projects; their satellites are
# the translations' folder's).
$FrameworkOnCorePackageSources = @('FrameworkOnCore.Runtime', 'experiments/wf4c/frameworkoncore.slnx', 'translations')

function Get-FrameworkOnCoreVersion {
    # The rules have comments (ConvertFrom-Json of Windows PowerShell does not take them): the one property.
    $rules = Get-Content (Join-Path $FrameworkOnCoreRepository 'src\FrameworkOnCore.Converter\rules\packages.json') -Raw
    $version = [regex]::Match($rules, '"frameworkOnCoreVersion"\s*:\s*"([^"]+)"').Groups[1].Value
    if (-not $version) { throw 'src/FrameworkOnCore.Converter/rules/packages.json に frameworkOnCoreVersion がありません' }
    $version
}

# The commit the version was published from (its release's tag), or $null when it is not published. $false when it
# cannot be told (no network, no GitHub).
function Get-FrameworkOnCorePublishedCommit([string]$Version) {
    $ErrorActionPreference = 'Continue'  # git's messages on stderr are no error here (Windows PowerShell)
    $line = git -C $FrameworkOnCoreRepository ls-remote --tags origin "refs/tags/frameworkoncore-$Version" 2>$null
    if ($LASTEXITCODE -ne 0) { return $false }
    if (-not $line) { return $null }
    $commit = ([string]$line -split '\s+')[0]
    # A shallow clone (GitHub Actions) or a checkout behind it: the commit fetched by its id.
    git -C $FrameworkOnCoreRepository cat-file -e "$commit^{commit}" 2>$null
    if ($LASTEXITCODE -ne 0) {
        git -C $FrameworkOnCoreRepository fetch --quiet --no-tags --depth=1 origin $commit 2>$null
        if ($LASTEXITCODE -ne 0) { return $false }
    }
    $commit
}

# The packages' sources changed since the version was published (the working tree, uncommitted changes and new files
# too): their paths. Empty: not changed, or not published.
function Get-FrameworkOnCoreChangesSincePublished([string]$Version, [string]$Commit) {
    $ErrorActionPreference = 'Continue'
    $changed = @(git -C $FrameworkOnCoreRepository diff --name-only $Commit -- $FrameworkOnCorePackageSources)
    $added = @(git -C $FrameworkOnCoreRepository ls-files --others --exclude-standard -- $FrameworkOnCorePackageSources)
    @($changed + $added | Where-Object { $_ })
}

# Stops when the version is published and its packages' sources changed since: a new version is needed.
function Assert-FrameworkOnCoreVersionUnchanged([string]$Version) {
    $commit = Get-FrameworkOnCorePublishedCommit $Version
    if ($commit -eq $false) {
        Write-Warning "frameworkoncore-$Version が公開済みかどうか確かめられませんでした(GitHub に接続できない)。公開済みの版なら、パッケージの元を変えたときは版番号を上げてください"
        return
    }
    if (-not $commit) { return }
    $changes = Get-FrameworkOnCoreChangesSincePublished $Version $commit
    if ($changes.Count -eq 0) { return }
    $shown = ($changes | Select-Object -First 10) -join "`n  "
    $more = if ($changes.Count -gt 10) { "`n  ほか $($changes.Count - 10) 件" } else { '' }
    throw @"
FrameworkOnCore $Version は公開済み(GitHub Release frameworkoncore-$Version、$($commit.Substring(0, 8)))で、そのあとパッケージの元が変わっています:
  $shown$more
同じ版番号で作り直すと、公開済みのものと中身が違うパッケージになります(ほかのマシンは公開済みの古いものを使う)。
src/FrameworkOnCore.Converter/rules/packages.json の frameworkOnCoreVersion を上げてください(版番号を書くのはそこだけです)。
"@
}
