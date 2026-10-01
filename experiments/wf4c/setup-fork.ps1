# Makes the WebFormsForCore fork from nothing: upstream cloned at the commit the patches are made on, the patches of
# experiments/wf4c/patches applied (git am), and the one submodule the packages need (the Ajax Control Toolkit, with its
# patches/AjaxControlToolkit). Then pack-fork.ps1 -Build All builds and packs it. The GitHub Actions workflow
# (.github/workflows/fork.yml) does this; a developer's _upstream is the same, with its history.
#
#   .\experiments\wf4c\setup-fork.ps1                  # into experiments\wf4c\_upstream (must not exist)
param(
    [string]$Target = (Join-Path $PSScriptRoot '_upstream'),
    # Upstream's commit the patches apply to (git merge-base of the fork's branch and upstream).
    [string]$Base = '22c7d354b59995616c8e8983ed73766bef7c805a',
    [string]$Repository = 'https://github.com/webformsforcore/WebFormsForCore.git'
)

$ErrorActionPreference = 'Stop'
if (Test-Path $Target) { throw "$Target exists: setup-fork.ps1 makes the fork from nothing" }
$patches = Join-Path $PSScriptRoot 'patches'
# git am records a committer: the patches' authors stay theirs.
$identity = @('-c', 'user.name=FrameworkOnCore', '-c', 'user.email=frameworkoncore@users.noreply.github.com')

function Invoke-Git {
    & git @identity @args
    if ($LASTEXITCODE -ne 0) { throw "git $args failed" }
}

Invoke-Git clone --no-checkout $Repository $Target
Invoke-Git -C $Target checkout -b w2l/fork $Base
# The Ajax Control Toolkit at the commit upstream has (its patch changes the gitlink to a commit only the fork has).
Invoke-Git -C $Target submodule update --init src/WebFormsForCore.AjaxControlToolkit
Invoke-Git -C $Target am (Get-ChildItem $patches -Filter '*.patch' | Sort-Object Name | ForEach-Object FullName)
$toolkit = Join-Path $Target 'src/WebFormsForCore.AjaxControlToolkit'
Invoke-Git -C $toolkit checkout -B w2l/fork
Invoke-Git -C $toolkit am (Get-ChildItem (Join-Path $patches 'AjaxControlToolkit') -Filter '*.patch' | Sort-Object Name | ForEach-Object FullName)
"fork made in $Target ($((Get-ChildItem $patches -Filter '*.patch').Count) patches on $($Base.Substring(0, 8)))"
