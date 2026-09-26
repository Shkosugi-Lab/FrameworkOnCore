# Packs the WebFormsForCore fork (experiments/wf4c/_upstream, local branch w2l/*) into
# experiments/wf4c/_feed as version $Version, and drops that version from the NuGet cache so the
# next restore picks the new build up (a package version is cached once and never re-read).
#
#   .\experiments\wf4c\pack-fork.ps1                  # rebuild System.Web only, pack everything
#   .\experiments\wf4c\pack-fork.ps1 -Build All       # rebuild every packed project first
#
# Build order matters upstream (Web.Extensions fails when built before its references), so -Build
# All builds in dependency order. Assumes src/WebFormsForCore.Build was built once (it emits the
# FakeStrongName targets every project imports).
param(
    [string]$Version = '1.6.5-w2l.1',
    [ValidateSet('Web', 'All', 'None')][string]$Build = 'Web'
)

$ErrorActionPreference = 'Stop'
$src = Join-Path $PSScriptRoot '_upstream\src'
$feed = Join-Path $PSScriptRoot '_feed'

$projects = @(
    'WebFormsForCore.Compilers\WebFormsForCore.Compilers.csproj'
    'WebFormsForCore.Configuration\WebFormsForCore.Configuration.csproj'
    'WebFormsForCore.Drawing\WebFormsForCore.Drawing.csproj'
    'WebFormsForCore.Serialization.Formatters\WebFormsForCore.Serialization.Formatters.csproj'
    'WebFormsForCore.Web\WebFormsForCore.Web.csproj'
    'WebFormsForCore.Web.ApplicationServices\WebFormsForCore.Web.ApplicationServices.csproj'
    'WebFormsForCore.Web.RegularExpressions\WebFormsForCore.Web.RegularExpressions.csproj'
    'WebFormsForCore.Web.Services\WebFormsForCore.Web.Services.csproj'
    'WebFormsForCore.Web.Extensions\WebFormsForCore.Web.Extensions.csproj'
    'WebFormsForCore.Web.Infrastructure\WebFormsForCore.Web.Infrastructure.csproj'
    'WebFormsForCore.Web.Optimization\WebFormsForCore.Web.Optimization.csproj'
    'WebFormsForCore.Web.Optimization.WebForms\WebFormsForCore.Web.Optimization.WebForms.csproj'
    'WebFormsForCore.WebGrease\WebFormsForCore.WebGrease.csproj'
    'WebFormsForCore.Web.DynamicData\WebFormsForCore.Web.DynamicData.csproj'
)

$toBuild = switch ($Build) {
    'All' { $projects }
    'Web' { @('WebFormsForCore.Web\WebFormsForCore.Web.csproj') }
    default { @() }
}
foreach ($project in $toBuild) {
    dotnet build (Join-Path $src $project) -c Debug -v q -nologo
    if ($LASTEXITCODE -ne 0) { throw "build failed: $project" }
}

New-Item -ItemType Directory $feed -Force | Out-Null
Remove-Item (Join-Path $feed '*') -Force -ErrorAction SilentlyContinue
foreach ($project in $projects) {
    dotnet pack (Join-Path $src $project) --no-build -c Debug -o $feed "-p:Version=$Version" -v q -nologo
    if ($LASTEXITCODE -ne 0) { throw "pack failed: $project" }
}

Get-ChildItem (Join-Path $env:USERPROFILE '.nuget\packages') -Directory -Filter 'webformsforcore.*' |
    ForEach-Object { Remove-Item (Join-Path $_.FullName $Version) -Recurse -Force -ErrorAction SilentlyContinue }
"packed $($projects.Count) packages ($Version) -> $feed"
