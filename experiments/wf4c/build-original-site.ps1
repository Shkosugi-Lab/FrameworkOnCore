# Builds a corpus the way it is built for .NET Framework - the whole solution, post-build events
# (xcopy into the site) and all - in a copy (experiments\wf4c\_original\<name>), without Visual
# Studio. The web project's folder is then the deployed site: what FrameworkOnCore takes as the
# application's composition (--site). On a real migration, the folder the site is deployed to on
# the IIS server is that.
#
#   .\experiments\wf4c\build-original-site.ps1 -Name mojo
#
# Tools: the .NET SDK's MSBuild (reads old-style and SDK-style projects), the .NET Framework
# reference assemblies from NuGet (every version the projects target, in one root), nuget.exe for
# packages.config.
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

$solutionPath = Join-Path $work $solution
$solutionDirectory = Split-Path $solutionPath -Parent
Write-Host 'restoring packages.config'
foreach ($config in Get-ChildItem $work -Recurse -Filter packages.config -File) {
    & $nuget restore $config.FullName -PackagesDirectory (Join-Path $solutionDirectory 'packages') -NonInteractive | Out-Null
}
Write-Host 'restoring PackageReference'
& dotnet restore $solutionPath "/p:TargetFrameworkRootPath=$refRootParent" --nologo -v q | Out-Null

Write-Host "building $solution ($Configuration)"
$log = Join-Path $PSScriptRoot "_original\$Name.build.log"
& dotnet msbuild $solutionPath "/p:Configuration=$Configuration" "/p:TargetFrameworkRootPath=$refRootParent" `
    /p:VSToolsPath= /p:BuildInParallel=false /m:1 /v:m /nologo "/flp:LogFile=$log;Verbosity=normal" | Out-Null
$code = $LASTEXITCODE
$errors = Select-String -Path $log -Pattern ': error ' | ForEach-Object { $_.Line.Trim() } | Sort-Object -Unique
Write-Host "build exit $code, $($errors.Count) error line(s)"
$errors | Select-Object -First 15 | ForEach-Object { '  ' + $_.Substring(0, [Math]::Min(220, $_.Length)) }
$siteDirectory = Join-Path $work $site
Write-Host "site: $siteDirectory ($(@(Get-ChildItem (Join-Path $siteDirectory 'bin') -Filter *.dll -ErrorAction SilentlyContinue).Count) DLLs in bin)"
