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
    [string]$Version = '1.6.5-w2l.4',
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
    # LINQ to SQL (System.Data.Linq), ported from referencesource (generate-dlinq-resources.ps1 for its resources).
    'WebFormsForCore.Data.Linq\WebFormsForCore.Data.Linq.csproj'
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
    # A submodule (git submodule update --init src/WebFormsForCore.AjaxControlToolkit): openIMIS uses it. Built and packed
    # for net10.0 only (the converted applications'), after the solution: its net8.0 build fails there (CS7069).
    'WebFormsForCore.AjaxControlToolkit\AjaxControlToolkit\AjaxControlToolkit.csproj'
)
$net10Only = @('WebFormsForCore.AjaxControlToolkit\AjaxControlToolkit\AjaxControlToolkit.csproj')

if ($Build -eq 'All') {
    # As a solution (fork.slnx), the way upstream builds: one project at a time, Web.Extensions
    # fails to see IHttpHandlerFactory through Web.Services. After a change in System.Web the first
    # build still fails that way now and then (CS7069) and a later one succeeds; hence up to three retries.
    dotnet build (Join-Path $PSScriptRoot 'fork.slnx') -c Debug -v q -nologo
    foreach ($retry in 1..3) { if ($LASTEXITCODE -eq 0) { break }; dotnet build (Join-Path $PSScriptRoot 'fork.slnx') -c Debug -v q -nologo }
    if ($LASTEXITCODE -ne 0) { throw "build failed: fork.slnx" }
    foreach ($project in $net10Only) {
        # -f, not the TargetFrameworks property: a global property would restore the projects it references for net10.0 only.
        dotnet build (Join-Path $src $project) -c Debug -f net10.0 --no-dependencies -v q -nologo
        if ($LASTEXITCODE -ne 0) { throw "build failed: $project" }
    }
}
elseif ($Build -eq 'Web') {
    dotnet build (Join-Path $src 'WebFormsForCore.Web\WebFormsForCore.Web.csproj') -c Debug -v q -nologo
    if ($LASTEXITCODE -ne 0) { throw "build failed: WebFormsForCore.Web" }
}

New-Item -ItemType Directory $feed -Force | Out-Null
Remove-Item (Join-Path $feed '*') -Force -ErrorAction SilentlyContinue
foreach ($project in $projects) {
    $frameworks = if ($net10Only -contains $project) { @('-p:TargetFrameworks=net10.0', '--no-restore') } else { @() }
    dotnet pack (Join-Path $src $project) --no-build -c Debug -o $feed "-p:Version=$Version" @frameworks -v q -nologo
    if ($LASTEXITCODE -ne 0) { throw "pack failed: $project" }
}

Get-ChildItem (Join-Path $env:USERPROFILE '.nuget\packages') -Directory -Filter 'webformsforcore.*' |
    ForEach-Object { Remove-Item (Join-Path $_.FullName $Version) -Recurse -Force -ErrorAction SilentlyContinue }
"packed $($projects.Count) packages ($Version) -> $feed"
