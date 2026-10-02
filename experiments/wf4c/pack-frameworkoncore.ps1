# Packs FrameworkOnCore's WebFormsForCore (FrameworkOnCore.Runtime/ of this repository: upstream taken in by git subtree, maintained here) into
# experiments/wf4c/_feed as version $Version, and drops that version from the NuGet cache so the
# next restore picks the new build up (a package version is cached once and never re-read).
#
#   .\experiments\wf4c\pack-frameworkoncore.ps1                  # rebuild System.Web only, pack everything
#   .\experiments\wf4c\pack-frameworkoncore.ps1 -Build All       # rebuild every packed project first
#
# Build order matters upstream (Web.Extensions fails when built before its references), so -Build
# All builds in dependency order, after src/WebFormsForCore.Build when it was not built yet (it emits the
# FakeStrongName targets every project imports).
param(
    [string]$Version = '1.6.5-w2l.12',
    [ValidateSet('Web', 'All', 'None')][string]$Build = 'Web'
)

$ErrorActionPreference = 'Stop'
# Release, as Microsoft shipped the originals and upstream ships its packages: a Debug build has referencesource's
# Debug.Assert (about 630 in the packed assemblies), and on .NET a failed Debug.Assert ends the process (found by the
# System.Data.Linq parity cases: SingleResult's assert, not true for Translate(DbDataReader)).
$Configuration = 'Release'
$webFormsForCore = Join-Path $PSScriptRoot '..\..\FrameworkOnCore.Runtime'
$src = Join-Path $webFormsForCore 'src'
$feed = Join-Path $PSScriptRoot '_feed'

$projects = @(
    'WebFormsForCore.Compilers\WebFormsForCore.Compilers.csproj'
    'WebFormsForCore.Configuration\WebFormsForCore.Configuration.csproj'
    'WebFormsForCore.Drawing\WebFormsForCore.Drawing.csproj'
    # System.Drawing.Common with its Linux implementation (dotnet/runtime release/6.0, libgdiplus): the Windows build and,
    # built apart below, the Unix one, in one package (runtimes/win, runtimes/unix).
    'WebFormsForCore.Drawing.Common\WebFormsForCore.Drawing.Common.csproj'
    # System.Data.SqlClient expanding |DataDirectory| as .NET Framework's (dotnet/maintenance-packages' 4.9.0 source): the
    # Windows build (native SNI) and, built apart below, the Unix one (managed SNI), in one package.
    'WebFormsForCore.Data.SqlClient\WebFormsForCore.Data.SqlClient.csproj'
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
    # The Chart control (System.Web.DataVisualization), ported from referencesource (generate-dataviz-resources.ps1 for its
    # resources); net10.0 only, as the System.Drawing.Common port it draws with.
    'WebFormsForCore.Web.DataVisualization\WebFormsForCore.Web.DataVisualization.csproj'
    # The mobile controls (System.Web.Mobile), from referencesource (upstream's adaptation to .NET; resources as .NET
    # Framework's assembly has them).
    'WebFormsForCore.Web.Mobile\WebFormsForCore.Web.Mobile.csproj'
    # A submodule (git submodule update --init src/WebFormsForCore.AjaxControlToolkit): openIMIS uses it. Built and packed
    # for net10.0 only (the converted applications'), after the solution: its net8.0 build fails there (CS7069).
    'WebFormsForCore.AjaxControlToolkit\AjaxControlToolkit\AjaxControlToolkit.csproj'
)
$net10Only = @('WebFormsForCore.AjaxControlToolkit\AjaxControlToolkit\AjaxControlToolkit.csproj')

if ($Build -eq 'All') {
    # The build tasks every project imports (lib/WebFormsForCore.Build: FakeStrongName and the rest), once: a fresh
    # checkout has none (build output, not in Git). Not in frameworkoncore.slnx (built there, its loaded task DLL is copied over: the build fails).
    # A framework at a time, each with its intermediate folder of its own: the project has none per framework (net8.0
    # and net10.0 write the same obj\...\WebFormsForCore.Build.NetCore.dll: together, CS2012; one after the other, the
    # second one's compilation is skipped as up to date).
    if (-not (Test-Path (Join-Path $webFormsForCore 'lib\WebFormsForCore.Build\net10.0\FakeStrongName.targets'))) {
        foreach ($framework in 'net48', 'net8.0', 'net10.0', 'netstandard2.0') {
            dotnet build (Join-Path $src 'WebFormsForCore.Build\WebFormsForCore.Build.csproj') -c $Configuration -f $framework "-p:IntermediateOutputPath=obj\$Configuration\$framework\" -v q -nologo
            if ($LASTEXITCODE -ne 0) { throw "build failed: WebFormsForCore.Build ($framework)" }
        }
    }
    # As a solution (frameworkoncore.slnx), the way upstream builds: one project at a time, Web.Extensions
    # fails to see IHttpHandlerFactory through Web.Services. After a change in System.Web the first
    # build still fails that way now and then (CS7069) and a later one succeeds; hence up to three retries.
    dotnet build (Join-Path $PSScriptRoot 'frameworkoncore.slnx') -c $Configuration -v q -nologo
    foreach ($retry in 1..3) { if ($LASTEXITCODE -eq 0) { break }; dotnet build (Join-Path $PSScriptRoot 'frameworkoncore.slnx') -c $Configuration -v q -nologo }
    if ($LASTEXITCODE -ne 0) { throw "build failed: frameworkoncore.slnx" }
    # The System.Drawing.Common port's Unix build (the solution builds its Windows one; the package takes both).
    dotnet build (Join-Path $src 'WebFormsForCore.Drawing.Common\WebFormsForCore.Drawing.Common.csproj') -c $Configuration -p:FocTargetOS=unix -v q -nologo
    if ($LASTEXITCODE -ne 0) { throw "build failed: FrameworkOnCore.Drawing.Common (unix)" }
    # The System.Data.SqlClient port's Unix build, likewise.
    dotnet build (Join-Path $src 'WebFormsForCore.Data.SqlClient\WebFormsForCore.Data.SqlClient.csproj') -c $Configuration -p:FocTargetOS=unix -v q -nologo
    if ($LASTEXITCODE -ne 0) { throw "build failed: FrameworkOnCore.Data.SqlClient (unix)" }
    foreach ($project in $net10Only) {
        # -f, not the TargetFrameworks property: a global property would restore the projects it references for net10.0 only.
        dotnet build (Join-Path $src $project) -c $Configuration -f net10.0 --no-dependencies -v q -nologo
        if ($LASTEXITCODE -ne 0) { throw "build failed: $project" }
    }
}
elseif ($Build -eq 'Web') {
    dotnet build (Join-Path $src 'WebFormsForCore.Web\WebFormsForCore.Web.csproj') -c $Configuration -v q -nologo
    if ($LASTEXITCODE -ne 0) { throw "build failed: FrameworkOnCore.Web" }
}

New-Item -ItemType Directory $feed -Force | Out-Null
Remove-Item (Join-Path $feed '*') -Force -ErrorAction SilentlyContinue
foreach ($project in $projects) {
    $frameworks = if ($net10Only -contains $project) { @('-p:TargetFrameworks=net10.0', '--no-restore') } else { @() }
    dotnet pack (Join-Path $src $project) --no-build -c $Configuration -o $feed "-p:Version=$Version" @frameworks -v q -nologo
    if ($LASTEXITCODE -ne 0) { throw "pack failed: $project" }
}

Get-ChildItem (Join-Path $env:USERPROFILE '.nuget\packages') -Directory -Filter 'frameworkoncore.*' |
    ForEach-Object { Remove-Item (Join-Path $_.FullName $Version) -Recurse -Force -ErrorAction SilentlyContinue }
"packed $($projects.Count) packages ($Version) -> $feed"
