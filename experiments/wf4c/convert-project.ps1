# Experiment: turn an old-style (.NET Framework) web project - and the projects it references -
# into SDK-style .NET 10 projects on WebFormsForCore, keeping every source file as it is. A
# prototype of what the new converter's project step has to do; see README.md for what was learned.
#
#   .\experiments\wf4c\convert-project.ps1 -Project <path\to\App.csproj> -Out <dir>
#
# Each project lands in <Out>\<ProjectName>\; the web project gets Program.cs.
param(
    [Parameter(Mandatory = $true)][string]$Project,
    [Parameter(Mandatory = $true)][string]$Out,
    # Source files (as the original project names them) left out of the build: code on a .NET
    # Framework API .NET has no counterpart for. The new converter will exclude and stub these
    # itself (as the old one did); here they are named by hand.
    [string[]]$ExcludeFiles = @()
)

$ErrorActionPreference = 'Stop'
$ForkVersion = '1.6.5-w2l.1'
$msbuildNs = @{ m = 'http://schemas.microsoft.com/developer/msbuild/2003' }

# packages.config ids with a .NET 10 answer: a replacement package, or nothing (dropped).
$replacedPackages = @{
    # WebFormsForCore ships its own port of these (same assembly names, .NET 10).
    'Microsoft.AspNet.Web.Optimization'          = @('WebFormsForCore.Web.Optimization', $ForkVersion)
    'Microsoft.AspNet.Web.Optimization.WebForms' = @('WebFormsForCore.Web.Optimization.WebForms', $ForkVersion)
    'WebGrease'                                  = @('WebFormsForCore.WebGrease', $ForkVersion)
    'Microsoft.Web.Infrastructure'               = @('WebFormsForCore.Web.Infrastructure', $ForkVersion)
    'AjaxControlToolkit'                         = @('WebFormsForCore.AjaxControlToolkit', $ForkVersion)
    # First version that runs on .NET (Core).
    'EntityFramework'                            = @('EntityFramework', '6.5.1')
    # Raised to what WebFormsForCore depends on (a downgrade is NU1605); backward compatible.
    'Newtonsoft.Json'                            = @('Newtonsoft.Json', '13.0.4')
}
$droppedPackages = @('NETStandard.Library', 'Microsoft.NETCore.Platforms', 'Microsoft.VisualStudio.Azure.Containers.Tools.Targets',
                     'Antlr', 'Microsoft.CodeDom.Providers.DotNetCompilerPlatform', 'Microsoft.Net.Compilers')

# .NET Framework assembly references (no HintPath) with a .NET 10 answer. Anything not listed is
# in the box on .NET (System.Core, System.Xml, ...) or has no answer (reported).
$frameworkReferences = @{
    'System.Web'                     = @('WebFormsForCore.Web', $ForkVersion)
    'System.Web.Extensions'          = @('WebFormsForCore.Web.Extensions', $ForkVersion)
    'System.Web.ApplicationServices' = @('WebFormsForCore.Web.ApplicationServices', $ForkVersion)
    'System.Web.Services'            = @('WebFormsForCore.Web.Services', $ForkVersion)
    'System.Web.DynamicData'         = @('WebFormsForCore.Web.DynamicData', $ForkVersion)
    'System.Configuration'           = @('WebFormsForCore.Configuration', $ForkVersion)
    'System.Drawing'                 = @('System.Drawing.Common', '10.0.0')
    'System.Runtime.Caching'         = @('System.Runtime.Caching', '10.0.0')
    'System.DirectoryServices'       = @('System.DirectoryServices', '10.0.0')
    'System.ServiceModel'            = @('System.ServiceModel.Http', '10.0.652802')
    'System.ServiceModel.Web'        = @('System.ServiceModel.Http', '10.0.652802')
}
$noAnswer = @('System.Data.Linq', 'System.Data.Services.Client', 'System.Design', 'System.Web.Mobile')

$converted = @{}

# A path relative to a directory (Windows PowerShell 5.1 has no Path.GetRelativePath). The output
# is built elsewhere too (a Linux container), so no absolute path goes into a project file.
function Get-RelativePath([string]$fromDirectory, [string]$to) {
    $from = New-Object Uri ($fromDirectory.TrimEnd('\') + '\')
    [Uri]::UnescapeDataString($from.MakeRelativeUri((New-Object Uri $to)).ToString()).Replace('/', '\')
}

function Convert-One([string]$projectPath, [bool]$isWeb) {
    $projectPath = (Resolve-Path $projectPath).Path
    if ($converted.ContainsKey($projectPath)) { return $converted[$projectPath] }

    $source = Split-Path $projectPath -Parent
    $name = [IO.Path]::GetFileNameWithoutExtension($projectPath)
    $target = Join-Path $Out $name
    $converted[$projectPath] = $target

    # 1. The project directory, as it is (build output and restored packages excluded).
    if (Test-Path $target) { Remove-Item $target -Recurse -Force }
    New-Item -ItemType Directory $target -Force | Out-Null
    robocopy $source $target /E /XD bin obj packages .vs /NFL /NDL /NJH /NJS /NP | Out-Null
    Get-ChildItem $target -Filter *.csproj | Remove-Item
    Get-ChildItem $target -Filter *.user | Remove-Item

    [xml]$old = Get-Content $projectPath -Raw -Encoding UTF8
    $nodes = { param($xpath) Select-Xml -Xml $old -XPath $xpath -Namespace $msbuildNs | ForEach-Object { $_.Node } }

    # 2. What the original compiled and embedded, in its order.
    $compile = & $nodes '//m:Compile' | ForEach-Object { $_.Include } | Where-Object { $ExcludeFiles -notcontains $_ }
    $embedded = & $nodes '//m:EmbeddedResource' | ForEach-Object { $_.Include }

    # 3. Packages (packages.config) and references.
    $packages = [ordered]@{}
    $config = Join-Path $source 'packages.config'
    if (Test-Path $config) {
        [xml]$pc = Get-Content $config -Raw -Encoding UTF8
        foreach ($p in $pc.packages.package) {
            $id = $p.id; $version = $p.version
            if ($id -like 'System.*' -and $version -match '^4\.') { continue }   # in the box on .NET
            if ($droppedPackages -contains $id) { continue }
            if ($replacedPackages.ContainsKey($id)) { $id, $version = $replacedPackages[$id] }
            $packages[$id] = $version
        }
    }
    foreach ($id in 'WebFormsForCore.Web', 'WebFormsForCore.Web.Extensions', 'WebFormsForCore.Configuration', 'WebFormsForCore.Web.DynamicData') {
        if (-not $packages.Contains($id)) { $packages[$id] = $ForkVersion }
    }

    $binaryReferences = @()
    $unanswered = @()
    foreach ($reference in & $nodes '//m:Reference') {
        $assembly = ($reference.Include -split ',')[0].Trim()
        $hint = $reference.HintPath
        if ($hint) {
            # A package's DLL is covered by the PackageReference above; a DLL checked into the
            # repository is copied along and referenced where it now is.
            if ($hint -match '(^|\\)packages\\') { continue }
            $dll = [IO.Path]::GetFullPath((Join-Path $source $hint))
            if (-not (Test-Path $dll)) { $unanswered += "$assembly (HintPath not found)"; continue }
            $lib = Join-Path $target '_lib'
            New-Item -ItemType Directory $lib -Force | Out-Null
            Copy-Item $dll $lib
            $binaryReferences += "    <Reference Include=""$assembly"" HintPath=""_lib\$([IO.Path]::GetFileName($dll))"" />"
        }
        elseif ($frameworkReferences.ContainsKey($assembly)) {
            $id, $version = $frameworkReferences[$assembly]
            if (-not $packages.Contains($id)) { $packages[$id] = $version }
            # SyndicationItem and friends were in System.ServiceModel(.Web); on .NET they are a package of their own.
            if ($assembly -like 'System.ServiceModel*' -and -not $packages.Contains('System.ServiceModel.Syndication')) {
                $packages['System.ServiceModel.Syndication'] = '10.0.0'
            }
        }
        elseif ($noAnswer -contains $assembly) {
            $unanswered += $assembly
        }
    }

    # The assemblies web.config names for page compilation (<compilation><assemblies>) are loaded
    # by name at run time; the ones .NET does not ship need their package, or the configuration
    # itself fails ("Could not load file or assembly 'System.Management'").
    $webConfig = Get-ChildItem $source -Filter 'web.config' | Select-Object -First 1
    if ($isWeb -and $webConfig) {
        [xml]$wc = Get-Content $webConfig.FullName -Raw -Encoding UTF8
        foreach ($add in $wc.SelectNodes('/configuration/system.web/compilation/assemblies/add')) {
            $assembly = ($add.assembly -split ',')[0].Trim()
            if ($assembly -eq 'System.Management' -and -not $packages.Contains('System.Management')) {
                $packages['System.Management'] = '10.0.0'
            }
            elseif ($frameworkReferences.ContainsKey($assembly)) {
                $id, $version = $frameworkReferences[$assembly]
                if (-not $packages.Contains($id)) { $packages[$id] = $version }
            }
        }
    }

    $projectReferences = @()
    foreach ($reference in & $nodes '//m:ProjectReference') {
        $referenced = [IO.Path]::GetFullPath((Join-Path $source $reference.Include))
        $referencedTarget = Convert-One $referenced $false
        $projectReferences += "    <ProjectReference Include=""`$(MSBuildThisFileDirectory)$(Get-RelativePath $target (Join-Path $referencedTarget ([IO.Path]::GetFileName($referenced))))"" />"
    }

    # 4. The project file.
    if ($isWeb) {
        $text = (Get-Content (Join-Path $PSScriptRoot 'template.csproj.txt') -Raw -Encoding UTF8).Replace('__NAME__', $name)
        # The template's own four package references are covered by the list below.
        $text = [regex]::Replace($text, '(?s)  <ItemGroup>\s*<PackageReference Include="WebFormsForCore\.Web".*?</ItemGroup>\s*', '')
        $text = $text.Replace('<EnableDefaultContentItems>false</EnableDefaultContentItems>',
            "<EnableDefaultContentItems>false</EnableDefaultContentItems>`r`n    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>`r`n    <EnableDefaultEmbeddedResourceItems>false</EnableDefaultEmbeddedResourceItems>`r`n    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>`r`n    <NoWarn>`$(NoWarn);NU1701;NU1603;NU1608;SYSLIB0011</NoWarn>")
    }
    else {
        $text = @"
<Project Sdk="Microsoft.NET.Sdk">
  <!-- Converted library (experiments/wf4c/convert-project.ps1); sources unchanged. -->
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <AssemblyName>$name</AssemblyName>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <EnableDefaultEmbeddedResourceItems>false</EnableDefaultEmbeddedResourceItems>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <NoWarn>`$(NoWarn);NU1701;NU1603;NU1608;SYSLIB0011</NoWarn>
    <RestoreAdditionalProjectSources>`$(MSBuildThisFileDirectory)..\..\_feed</RestoreAdditionalProjectSources>
  </PropertyGroup>

  <Target Name="ChangeAliasesOfNugetRefs" BeforeTargets="FindReferenceAssembliesForReferences;ResolveReferences">
    <ItemGroup>
      <ReferencePath Remove="%(Identity)" Condition="'%(FileName)' == 'System.Web' AND `$([System.Text.RegularExpressions.Regex]::IsMatch(%(Identity),'[/\x5C]dotnet[/\x5C]'))" />
    </ItemGroup>
  </Target>
</Project>
"@
    }
    $items = "  <ItemGroup>`r`n" + (($compile | ForEach-Object { "    <Compile Include=""$_"" />" }) -join "`r`n")
    if ($isWeb) { $items += "`r`n    <Compile Include=""Program.cs"" />" }
    $items += "`r`n" + (($embedded | ForEach-Object { "    <EmbeddedResource Include=""$_"" />" }) -join "`r`n")
    $items += "`r`n  </ItemGroup>`r`n`r`n  <ItemGroup>`r`n" +
              (($packages.GetEnumerator() | ForEach-Object { "    <PackageReference Include=""$($_.Key)"" Version=""$($_.Value)"" />" }) -join "`r`n") +
              "`r`n" + ($binaryReferences -join "`r`n") + "`r`n" + ($projectReferences -join "`r`n") + "`r`n  </ItemGroup>`r`n`r`n"
    # The templates' paths into experiments\wf4c (the local feed, the shims), made relative to where
    # the project now is. Before the items go in: their ProjectReferences are relative already.
    $toScripts = '$(MSBuildThisFileDirectory)' + (Get-RelativePath $target ($PSScriptRoot + '\'))
    $text = $text.Replace('$(MSBuildThisFileDirectory)..\..\', '__SCRIPTS__').Replace('$(MSBuildThisFileDirectory)..\', '__SCRIPTS__').Replace('__SCRIPTS__', $toScripts)
    $text = $text.Replace('  <Target Name="ChangeAliasesOfNugetRefs"', $items + '  <Target Name="ChangeAliasesOfNugetRefs"')
    Set-Content (Join-Path $target "$name.csproj") $text -Encoding UTF8

    # 5. Program.cs for the web project. An app that routes (System.Web.Routing, FriendlyUrls)
    # serves extensionless URLs, which only reach Web Forms when every request is handed to it; it
    # also answers "/" itself (FriendlyUrls redirects Default.aspx to /Default), so IIS's default
    # document is not emulated.
    if ($isWeb) {
        $program = Get-Content (Join-Path $PSScriptRoot 'Program.cs.txt') -Raw -Encoding UTF8
        $routes = $packages.Contains('Microsoft.AspNet.FriendlyUrls') -or
                  (Get-ChildItem $target -Filter *.cs -Recurse | Select-String -Pattern 'RouteTable\.Routes|RouteCollection' -List -Quiet)
        if ($routes) {
            $program = $program.Replace('app.UseDefaultFiles(defaults);', '// Routed app: Web Forms answers "/" (see convert-project.ps1).')
            $program = $program.Replace('options => options.UseAspNetCoreSessionProvider()', 'options => options.HandleAllRequestsWithWebForms().UseAspNetCoreSessionProvider()')
        }
        Set-Content (Join-Path $target 'Program.cs') $program -Encoding UTF8
    }

    Write-Host "converted: $name ($($compile.Count) compile, $($packages.Count) packages, $($binaryReferences.Count) DLLs, $($projectReferences.Count) project refs) -> $target"
    if ($unanswered) { Write-Host "  no .NET answer: $($unanswered -join ', ')" }
    return $target
}

New-Item -ItemType Directory $Out -Force | Out-Null
# Absolute while converting (relative paths in the output are computed from it).
$Out = (Resolve-Path $Out).Path
Convert-One $Project $true | Out-Null
