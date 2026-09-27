# Builds a corpus the way it is built for .NET Framework - the whole solution, post-build events
# (xcopy into the site) and all - in a copy (experiments\wf4c\_original\<name>). The web project's
# folder is then the deployed site: what FrameworkOnCore takes as the application's composition
# (--site). On a real migration, the folder the site is deployed to on the IIS server is that.
#
#   .\experiments\wf4c\build-original-site.ps1 -Name mojo
#
# Tools: Visual Studio (Build Tools) 2022's MSBuild - old-style projects with PackageReference need
# its NuGet targets, which the .NET SDK does not have -, the .NET Framework reference assemblies
# from NuGet (every version the projects target, in one root), nuget.exe for packages.config.
param(
    [Parameter(Mandatory = $true)][string]$Name,
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Continue'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$legacy = Join-Path $repo 'tools\legacy'
$packages = Join-Path $legacy 'packages'
$nuget = Join-Path $legacy 'nuget.exe'

$corpora = @{
    be   = @('BlogEngine.NET-3.3.8.0', 'BlogEngine\BlogEngine.sln', 'BlogEngine\BlogEngine.NET')
    wt   = @('wingtiptoys-master', 'WingtipToys\WingtipToys.sln', 'WingtipToys\WingtipToys')
    mojo = @('mojoportal-3.1.6', 'mojoportal.sln', 'Web')
    dnn  = @('Dnn.Platform-9.13.10', 'DNN_Platform.sln', 'DNN Platform\Website')
    n2   = @('n2cms-master', 'src\N2.Sources.sln', 'src\WebForms\WebFormsTemplates')
}
$root, $solution, $site = $corpora[$Name]
$source = Join-Path $repo "corpora\work\$root"
$work = Join-Path $PSScriptRoot "_original\$Name"

# The reference assemblies of every .NET Framework version the projects may target, in one root
# (a solution mixes them: mojoPortal 4.8.1, DNN 4.7.2, N2 4.5.2).
$refRoot = Join-Path $legacy 'reference-assemblies\.NETFramework'
foreach ($version in 'net45', 'net451', 'net452', 'net46', 'net461', 'net462', 'net47', 'net471', 'net472', 'net48', 'net481') {
    $package = "Microsoft.NETFramework.ReferenceAssemblies.$version"
    $dir = Join-Path $packages "$package.1.0.3"
    if (-not (Test-Path $dir)) { & $nuget install $package -Version 1.0.3 -OutputDirectory $packages -NonInteractive | Out-Null }
    Get-ChildItem (Join-Path $dir 'build\.NETFramework') -Directory | ForEach-Object {
        $target = Join-Path $refRoot $_.Name
        if (-not (Test-Path $target)) { Copy-Item $_.FullName $target -Recurse }
    }
}
$refRootParent = (Split-Path $refRoot -Parent) + '\'

Write-Host "copying $source -> $work"
robocopy $source $work /MIR /XD .git /NFL /NDL /NJH /NJS /NP | Out-Null

# Built through a drive letter mapped to the copy: DNN's deepest paths go past Windows' 260
# characters under this folder (MSB3491), not under a short root, where its authors build it.
$drive = @('W', 'V', 'U', 'T', 'S', 'R', 'Q', 'P') | Where-Object { -not (Test-Path "${_}:\") } | Select-Object -First 1
subst "${drive}:" $work
$buildRoot = "${drive}:\"
Write-Host "building in $buildRoot ($work)"
$solutionPath = Join-Path $buildRoot $solution
$solutionDirectory = Split-Path $solutionPath -Parent
Write-Host 'restoring packages.config'
foreach ($config in Get-ChildItem $work -Recurse -Filter packages.config -File) {
    & $nuget restore $config.FullName -PackagesDirectory (Join-Path $solutionDirectory 'packages') -NonInteractive | Out-Null
}
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (-not $msbuild) { throw 'Visual Studio (Build Tools) MSBuild not found: old-style projects with PackageReference need its NuGet targets' }

# The newest C# compiler (the .NET SDK's): sources are written for the one their authors had, and a
# recent codebase uses C# 14 (mojoPortal 3.1.6 has the "field" keyword, LangVersion latest), which
# Visual Studio 2022's compiler does not know.
$sdk = (& dotnet --list-sdks | Select-Object -Last 1) -replace '^(\S+) \[(.*)\]$', '$2\$1'
$compiler = Join-Path $sdk 'Roslyn\bincore'
# As Visual Studio builds a solution, which is how these are built by their authors: the solution
# builds each project in order and a project reference only names the other's output. mojoPortal's
# web project builds its feature projects after itself (mojoPortal.Web.wpp.targets), and they
# reference it: from the command line, MSBuild would build it again from there (MSB4006, a cycle).

#
# So, as Visual Studio does: the projects the solution configuration builds, in the order their
# project references give, one by one, with BuildingInsideVisualStudio.
$solutionText = Get-Content $solutionPath -Raw
# C# and VB projects (DNN's WebUtility is VB; the C# projects use its output).
$projects = [regex]::Matches($solutionText, 'Project\("\{[^}]+\}"\)\s*=\s*"([^"]*)",\s*"([^"]+\.(?:cs|vb)proj)",\s*"(\{[^}]+\})"') |
    ForEach-Object { [pscustomobject]@{ Name = $_.Groups[1].Value; Path = [IO.Path]::GetFullPath((Join-Path $solutionDirectory $_.Groups[2].Value)); Guid = $_.Groups[3].Value } }
$platforms = [regex]::Matches($solutionText, "(?m)^\s*$([regex]::Escape($Configuration))\|([^=]+?)\s*=") | ForEach-Object { $_.Groups[1].Value }
$platform = @('Any CPU', 'Mixed Platforms') + $platforms | Where-Object { $platforms -contains $_ } | Select-Object -First 1
$built = $projects | Where-Object { $solutionText -match ([regex]::Escape("$($_.Guid).$Configuration|$platform.Build.0")) }
Write-Host "solution configuration ${Configuration}|${platform}: $(@($built).Count) of $(@($projects).Count) projects"

# The solution's own build dependencies too (ProjectSection(ProjectDependencies)): a project may use
# another's output by a reference to its DLL, not a project reference (DNN's Instrumentation and log4net).
$solutionDependencies = @{}
foreach ($block in [regex]::Matches($solutionText, '(?s)Project\("\{[^}]+\}"\)\s*=\s*"[^"]*",\s*"[^"]+",\s*"(\{[^}]+\})"(.*?)EndProject\b')) {
    $section = [regex]::Match($block.Groups[2].Value, '(?s)ProjectSection\(ProjectDependencies\)(.*?)EndProjectSection')
    if ($section.Success) {
        $solutionDependencies[$block.Groups[1].Value] = [regex]::Matches($section.Groups[1].Value, '(\{[^}]+\})\s*=') | ForEach-Object { $_.Groups[1].Value }
    }
}

$order = New-Object Collections.Generic.List[object]
$visiting = @{}
function Visit($project) {
    if ($order.Contains($project) -or $visiting[$project.Path]) { return }
    $visiting[$project.Path] = $true
    [xml]$x = Get-Content $project.Path -Raw
    foreach ($reference in $x.SelectNodes('//*[local-name()="ProjectReference"]')) {
        $path = [IO.Path]::GetFullPath((Join-Path (Split-Path $project.Path -Parent) $reference.GetAttribute('Include')))
        $dependency = $built | Where-Object { $_.Path -eq $path } | Select-Object -First 1
        if ($dependency) { Visit $dependency }
    }
    foreach ($guid in @($solutionDependencies[$project.Guid])) {
        $dependency = $built | Where-Object { $_.Guid -eq $guid } | Select-Object -First 1
        if ($dependency) { Visit $dependency }
    }
    $order.Add($project)
}
foreach ($project in $built) { Visit $project }

Write-Host "building $solution ($Configuration)"
$log = Join-Path $PSScriptRoot "_original\$Name.build.log"
Remove-Item $log -ErrorAction SilentlyContinue
function Build-Projects($projects) {
    $failed = New-Object Collections.Generic.List[object]
    foreach ($project in $projects) {
        & $msbuild $project.Path /restore "/p:Configuration=$Configuration" "/p:Platform=AnyCPU" "/p:SolutionDir=$solutionDirectory\" `
            "/p:TargetFrameworkRootPath=$refRootParent" "/p:CscToolPath=$compiler" /p:CscToolExe=csc.exe `
            /p:BuildingInsideVisualStudio=true /p:ShouldUnsetParentConfigurationAndPlatform=false `
            /m:1 /v:m /nologo "/flp:LogFile=$log;Verbosity=normal;Append" | Out-Null
        if ($LASTEXITCODE -ne 0) { $failed.Add($project) }
    }
    return , $failed
}
$failed = Build-Projects $order
# Once more for the ones that failed, as their authors would: a post-build step of a multi-targeted
# project copies another target's output, built after it the first time (DNN's ModulePipeline
# XCOPYs bin\Release\net472 from its netstandard2.0 build).
if ($failed.Count -gt 0) {
    Write-Host "building again: $(($failed | ForEach-Object { $_.Name }) -join ', ')"
    $failed = Build-Projects $failed
}
$code = if ($failed.Count -gt 0) { 1 } else { 0 }
foreach ($project in $failed) { Write-Host "  failed: $($project.Name)" }
# The repository's own setup steps its documentation has run after the build (not part of the
# solution): N2 links its management pages into the templates site (build.bat
# /t:Source-PrepareDependencies; mklink, or a copy where the link cannot be made).
$setup = @{ n2 = @('build\n2.proj', 'Templates-PrepareDependencies') }
if ($setup.ContainsKey($Name)) {
    $setupProject, $setupTarget = $setup[$Name]
    Write-Host "setup: $setupProject /t:$setupTarget"
    & $msbuild (Join-Path $buildRoot $setupProject) "/t:$setupTarget" "/p:Configuration=$Configuration" `
        "/p:TargetFrameworkRootPath=$refRootParent" "/p:CscToolPath=$compiler" /p:CscToolExe=csc.exe `
        /m:1 /v:m /nologo "/flp:LogFile=$log;Verbosity=normal;Append" | Out-Null
    if ($LASTEXITCODE -ne 0) { $code = $LASTEXITCODE; Write-Host "  setup failed" }
}
subst "${drive}:" /d
$errors = Select-String -Path $log -Pattern ': error ' | ForEach-Object { $_.Line.Trim() } | Sort-Object -Unique
Write-Host "build exit $code, $($errors.Count) error line(s) (both passes)"
$errors | Select-Object -First 15 | ForEach-Object { '  ' + $_.Substring(0, [Math]::Min(220, $_.Length)) }
$siteDirectory = Join-Path $work $site
Write-Host "site: $siteDirectory ($(@(Get-ChildItem (Join-Path $siteDirectory 'bin') -Filter *.dll -ErrorAction SilentlyContinue).Count) DLLs in bin)"
