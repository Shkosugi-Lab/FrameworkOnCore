# Experiment: turn a .NET Framework web project - and the projects it references - into .NET 10
# projects on WebFormsForCore, keeping every source file as it is. A prototype of what the new
# converter's project step has to do; see README.md for what was learned.
#
#   .\experiments\wf4c\convert-project.ps1 -Project <path\to\App.csproj> -Out <dir> [-Root <dir>]
#
# The repository the project is in (-Root; default: the topmost folder above the project holding a
# .sln, or the .git folder) is copied to <Out> as it is, and the project files are rewritten where
# they are. Repositories share files across projects (..\SolutionInfo.cs, ..\..\DNN_Platform.build,
# build output one project references from another), so a project does not stand on its own.
param(
    [Parameter(Mandatory = $true)][string]$Project,
    [Parameter(Mandatory = $true)][string]$Out,
    [string]$Root,
    # Source files (as the original project names them) left out of the build: code on a .NET
    # Framework API .NET has no counterpart for. The new converter will exclude and stub these
    # itself (as the old one did); here they are named by hand.
    [string[]]$ExcludeFiles = @(),
    # The original server's culture data (capture-culture.ps1, run there), placed where the
    # runtime looks for it (App_Data/culture-profile.json).
    [string]$CultureProfile
)

$ErrorActionPreference = 'Stop'
$FrameworkOnCoreVersion = '1.6.5-w2l.6'
$msbuildNs = @{ m = 'http://schemas.microsoft.com/developer/msbuild/2003' }
$msbuildUri = 'http://schemas.microsoft.com/developer/msbuild/2003'

# packages.config ids with a .NET 10 answer: a replacement package, or nothing (dropped).
$replacedPackages = @{
    # WebFormsForCore ships its own port of these (same assembly names, .NET 10).
    'Microsoft.AspNet.Web.Optimization'          = @('FrameworkOnCore.Web.Optimization', $FrameworkOnCoreVersion)
    'Microsoft.AspNet.Web.Optimization.WebForms' = @('FrameworkOnCore.Web.Optimization.WebForms', $FrameworkOnCoreVersion)
    'WebGrease'                                  = @('FrameworkOnCore.WebGrease', $FrameworkOnCoreVersion)
    'Microsoft.Web.Infrastructure'               = @('FrameworkOnCore.Web.Infrastructure', $FrameworkOnCoreVersion)
    'AjaxControlToolkit'                         = @('FrameworkOnCore.AjaxControlToolkit', $FrameworkOnCoreVersion)
    # System.Drawing.Common from NuGet is Windows only since .NET 7: FrameworkOnCore's has the Linux implementation.
    'System.Drawing.Common'                      = @('FrameworkOnCore.Drawing.Common', $FrameworkOnCoreVersion)
    # First version that runs on .NET (Core).
    'EntityFramework'                            = @('EntityFramework', '6.5.1')
    # Raised to what WebFormsForCore depends on (a downgrade is NU1605); backward compatible.
    'Newtonsoft.Json'                            = @('Newtonsoft.Json', '13.0.4')
}
$droppedPackages = @('NETStandard.Library', 'Microsoft.NETCore.Platforms', 'Microsoft.VisualStudio.Azure.Containers.Tools.Targets',
                     'Antlr', 'Microsoft.CodeDom.Providers.DotNetCompilerPlatform', 'Microsoft.Net.Compilers',
                     'Microsoft.Net.Compilers.Toolset', 'Microsoft.NETFramework.ReferenceAssemblies')

# .NET Framework assembly references (no HintPath) with a .NET 10 answer. Anything not listed is
# in the box on .NET (System.Core, System.Xml, ...) or has no answer (reported).
$frameworkReferences = @{
    'System.Web'                     = @('FrameworkOnCore.Web', $FrameworkOnCoreVersion)
    'System.Web.Extensions'          = @('FrameworkOnCore.Web.Extensions', $FrameworkOnCoreVersion)
    'System.Web.ApplicationServices' = @('FrameworkOnCore.Web.ApplicationServices', $FrameworkOnCoreVersion)
    'System.Web.Services'            = @('FrameworkOnCore.Web.Services', $FrameworkOnCoreVersion)
    'System.Web.DynamicData'         = @('FrameworkOnCore.Web.DynamicData', $FrameworkOnCoreVersion)
    'System.Web.Abstractions'        = @('FrameworkOnCore.Web', $FrameworkOnCoreVersion)
    'System.Web.Routing'             = @('FrameworkOnCore.Web', $FrameworkOnCoreVersion)
    'System.Configuration'           = @('FrameworkOnCore.Configuration', $FrameworkOnCoreVersion)
    'System.Drawing'                 = @('FrameworkOnCore.Drawing.Common', $FrameworkOnCoreVersion)
    'System.Data.Linq'               = @('FrameworkOnCore.Data.Linq', $FrameworkOnCoreVersion)
    'System.Runtime.Caching'         = @('System.Runtime.Caching', '10.0.0')
    'System.DirectoryServices'       = @('System.DirectoryServices', '10.0.0')
    'System.DirectoryServices.AccountManagement' = @('System.DirectoryServices.AccountManagement', '10.0.0')
    'System.Management'              = @('System.Management', '10.0.0')
    'System.ServiceModel'            = @('System.ServiceModel.Http', '10.0.652802')
    'System.ServiceModel.Web'        = @('System.ServiceModel.Http', '10.0.652802')
}
$noAnswer = @('System.Data.Services.Client', 'System.Design', 'System.Web.Mobile')

# What .NET Framework had in its own assemblies (System.Data, System, System.Security, ...) and .NET
# ships as packages: added when the project's sources use it (a namespace or a type name).
$sourcePackages = @(
    @{ Pattern = '\bSystem\.Data\.SqlClient\b|\bMicrosoft\.SqlServer\.Server\b'; Id = 'System.Data.SqlClient'; Version = '4.9.0' }
    @{ Pattern = '\bSystem\.Data\.Odbc\b'; Id = 'System.Data.Odbc'; Version = '10.0.0' }
    @{ Pattern = '\bSystem\.Data\.OleDb\b'; Id = 'System.Data.OleDb'; Version = '10.0.0'; Note = 'OLE DB is Windows only' }
    @{ Pattern = '\bSystem\.Security\.Cryptography\.Xml\b'; Id = 'System.Security.Cryptography.Xml'; Version = '10.0.0' }
    @{ Pattern = '\bSystem\.Security\.Cryptography\.Pkcs\b'; Id = 'System.Security.Cryptography.Pkcs'; Version = '10.0.0' }
    @{ Pattern = '\bSystem\.Security\.Permissions\b|\bSecurityPermission(Attribute)?\b|\bReflectionPermission\b|\bFileIOPermission\b'; Id = 'System.Security.Permissions'; Version = '10.0.0' }
    @{ Pattern = '\bEventLog(Entry|EntryType)?\b'; Id = 'System.Diagnostics.EventLog'; Version = '10.0.0'; Note = 'the event log is Windows only' }
    @{ Pattern = '\bPerformanceCounter(Category|Type)?\b'; Id = 'System.Diagnostics.PerformanceCounter'; Version = '10.0.0'; Note = 'performance counters are Windows only' }
    @{ Pattern = '\bSystem\.ServiceProcess\b'; Id = 'System.ServiceProcess.ServiceController'; Version = '10.0.0'; Note = 'Windows services' }
    @{ Pattern = '\bSystem\.IO\.Ports\b'; Id = 'System.IO.Ports'; Version = '10.0.0' }
    @{ Pattern = '\bSystem\.ComponentModel\.Composition\b'; Id = 'System.ComponentModel.Composition'; Version = '10.0.0' }
    @{ Pattern = '\bSystem\.Runtime\.Caching\b'; Id = 'System.Runtime.Caching'; Version = '10.0.0' }
    @{ Pattern = '\bSystem\.DirectoryServices\b'; Id = 'System.DirectoryServices'; Version = '10.0.0'; Note = 'Active Directory (Windows; LDAP via System.DirectoryServices.Protocols)' }
    @{ Pattern = '\bSystem\.Management\b'; Id = 'System.Management'; Version = '10.0.0'; Note = 'WMI is Windows only' }
    @{ Pattern = '\bSystem\.Drawing\b'; Id = 'FrameworkOnCore.Drawing.Common'; Version = $FrameworkOnCoreVersion; Note = 'System.Drawing: FrameworkOnCore''s System.Drawing.Common (with its Linux implementation over libgdiplus)' }
    @{ Pattern = '\bSystem\.ServiceModel\.Syndication\b'; Id = 'System.ServiceModel.Syndication'; Version = '10.0.0' }
    @{ Pattern = '\bSystem\.Configuration\.ConfigurationManager\b|\bConfigurationManager\b'; Id = 'FrameworkOnCore.Configuration'; Version = $FrameworkOnCoreVersion }
)
$notesSeen = @{}
function Add-SourcePackages($packages, [string[]]$files, [string]$name) {
    $text = ($files | Where-Object { Test-Path $_ } | ForEach-Object { [IO.File]::ReadAllText($_) }) -join "`n"
    foreach ($rule in $sourcePackages) {
        if ($text -match $rule.Pattern) {
            Add-Package $packages $rule.Id $rule.Version
            if ($rule.Note -and -not $notesSeen.ContainsKey("$name|$($rule.Id)")) {
                $notesSeen["$name|$($rule.Id)"] = $true
                $report.Add("${name}: uses $($rule.Id) - $($rule.Note)")
            }
        }
    }
}

# Every converted project: new vulnerability advisories on old package versions are warnings, not
# the errors TreatWarningsAsErrors would make them (the original built before they were published).
$commonProperties = '<NoWarn>$(NoWarn);NU1701;NU1603;NU1608;SYSLIB0011</NoWarn><WarningsNotAsErrors>$(WarningsNotAsErrors);NU1901;NU1902;NU1903;NU1904</WarningsNotAsErrors>'
# The web project always gets these (Program.cs uses them; pages reference them at run time).
$webPackages = @('FrameworkOnCore.Web', 'FrameworkOnCore.Web.Extensions', 'FrameworkOnCore.Configuration', 'FrameworkOnCore.Web.DynamicData')

# A path relative to a directory (Windows PowerShell 5.1 has no Path.GetRelativePath). The output
# is built elsewhere too (a Linux container), so no absolute path goes into a project file.
function Get-RelativePath([string]$fromDirectory, [string]$to) {
    $from = New-Object Uri ($fromDirectory.TrimEnd('\') + '\')
    [Uri]::UnescapeDataString($from.MakeRelativeUri((New-Object Uri $to)).ToString()).Replace('/', '\')
}

# The topmost folder above the project holding a solution (.sln), below the repository's root.
function Find-Root([string]$projectPath) {
    $top = Split-Path $projectPath -Parent
    $directory = $top
    while ($directory) {
        # "*.sln" also matches ".slnx" on Windows (8.3 matching): the extension is compared.
        if (Get-ChildItem $directory -File -ErrorAction SilentlyContinue | Where-Object { $_.Extension -eq '.sln' }) { $top = $directory }
        if (Test-Path (Join-Path $directory '.git')) { break }
        $parent = Split-Path $directory -Parent
        if (-not $parent -or $parent -eq $directory) { break }
        $directory = $parent
    }
    return $top
}

# Assembly name -> project file, for references to another project's build output (HintPath into
# its bin folder, which does not exist in a clean checkout).
$assemblyProjects = $null
function Find-ProjectOfAssembly([string]$assembly) {
    if ($null -eq $script:assemblyProjects) {
        $script:assemblyProjects = @{}
        foreach ($file in Get-ChildItem $sourceRoot -Filter *.csproj -Recurse -File -ErrorAction SilentlyContinue) {
            try { [xml]$x = Get-Content $file.FullName -Raw -Encoding UTF8 } catch { continue }
            $name = ($x.SelectNodes('//*[local-name()="AssemblyName"]') | Select-Object -First 1).InnerText
            if (-not $name) { $name = [IO.Path]::GetFileNameWithoutExtension($file.Name) }
            if (-not $script:assemblyProjects.ContainsKey($name)) { $script:assemblyProjects[$name] = $file.FullName }
        }
    }
    return $script:assemblyProjects[$assembly]
}

# Old-style projects decide by configuration what they reference and compile (mojoPortal picks its
# database provider by Configuration, log4net its code by DefineConstants): the conditions are
# evaluated for the configuration built, Debug|AnyCPU. Comparisons joined by and/or; a condition
# this does not understand holds (and is reported).
$Configuration = 'Debug'
$Platform = 'AnyCPU'
function Test-Condition([string]$condition, [string]$name) {
    if (-not $condition) { return $true }
    $expanded = $condition.Replace('$(Configuration)', $Configuration).Replace('$(Platform)', $Platform)
    $parts = [regex]::Split($expanded, '\s+(and|or)\s+', 'IgnoreCase')
    $result = $null; $operator = 'and'
    foreach ($part in $parts) {
        if ($part -match '^(?i)(and|or)$') { $operator = $part.ToLowerInvariant(); continue }
        $m = [regex]::Match($part.Trim(), "^\(?\s*'([^']*)'\s*(==|!=)\s*'([^']*)'\s*\)?$")
        if (-not $m.Success) {
            if ($part -notmatch '\$\(') { $value = $true }
            else { $report.Add("${name}: condition not evaluated (taken as true): $condition"); return $true }
        }
        else {
            $equal = [string]::Equals($m.Groups[1].Value.Trim(), $m.Groups[3].Value.Trim(), 'OrdinalIgnoreCase')
            $value = if ($m.Groups[2].Value -eq '==') { $equal } else { -not $equal }
        }
        $result = if ($null -eq $result) { $value } elseif ($operator -eq 'and') { $result -and $value } else { $result -or $value }
    }
    return [bool]$result
}
# Whether an item holds: its own condition and its ItemGroup's.
function Test-Item($node, [string]$name) {
    (Test-Condition $node.GetAttribute('Condition') $name) -and (Test-Condition $node.ParentNode.GetAttribute('Condition') $name)
}

$converted = @{}
$report = New-Object Collections.Generic.List[string]

# The project file in the output tree for a project file in the source tree.
function Get-TargetPath([string]$sourcePath) { Join-Path $Out (Get-RelativePath $sourceRoot $sourcePath) }

function Add-Package($packages, [string]$id, [string]$version) {
    if (-not $packages.Contains($id)) { $packages[$id] = $version }
}

function Convert-One([string]$projectPath, [bool]$isWeb) {
    $projectPath = (Resolve-Path $projectPath).Path
    if ($converted.ContainsKey($projectPath)) { return }
    $converted[$projectPath] = $true

    $targetPath = Get-TargetPath $projectPath
    $target = Split-Path $targetPath -Parent
    $name = [IO.Path]::GetFileNameWithoutExtension($projectPath)
    $text = Get-Content $projectPath -Raw -Encoding UTF8
    [xml]$old = $text
    $isSdk = [bool]$old.Project.GetAttribute('Sdk')

    # The referenced projects first (their files are rewritten the same way).
    $projectReferences = @()
    foreach ($reference in $old.SelectNodes('//*[local-name()="ProjectReference"]')) {
        if (-not $isSdk -and -not (Test-Item $reference $name)) { continue }
        $referenced = [IO.Path]::GetFullPath((Join-Path (Split-Path $projectPath -Parent) $reference.GetAttribute('Include')))
        if ($referenced -notlike '*.csproj') { $report.Add("${name}: project reference not converted: $referenced"); continue }
        if (-not (Test-Path $referenced)) { $report.Add("${name}: project reference not found: $referenced"); continue }
        Convert-One $referenced $false
        $projectReferences += $referenced
    }

    if ($isSdk) { Convert-SdkProject $projectPath $targetPath $name $old }
    else { Convert-OldProject $projectPath $targetPath $name $old $isWeb $projectReferences }
}

# An SDK-style project: one target framework, net10.0 - the application runs on .NET 10 only, and a
# netstandard target next to it could not reference the projects that became net10.0 (NU1201).
# Conditions on the .NET Framework target (" '$(TargetFramework)' == 'net472' ") are rewritten to
# it; the netstandard ones no longer hold. Framework assembly references become packages.
function Convert-SdkProject([string]$projectPath, [string]$targetPath, [string]$name, [xml]$project) {
    # Analyzers and source generators run in the compiler: netstandard2.0, as they are (RS1041).
    if ($project.SelectSingleNode("//PackageReference[starts-with(@Include, 'Microsoft.CodeAnalysis')]")) {
        $report.Add("${name}: analyzer / source generator, kept as it is")
        return
    }
    $frameworks = @()
    foreach ($node in @($project.SelectNodes('//TargetFramework | //TargetFrameworks'))) {
        $frameworks += $node.InnerText -split ';' | ForEach-Object { $_.Trim() } | Where-Object { $_ }
        $node.InnerText = 'net10.0'
    }
    $netfx = $frameworks | Where-Object { $_ -match '^net4\d*$' }
    $dropped = $frameworks | Where-Object { $_ -notmatch '^net4\d*$' -and $_ -ne 'net10.0' }
    foreach ($element in @($project.SelectNodes('//*[@Condition]'))) {
        $condition = $element.GetAttribute('Condition')
        $new = $condition
        foreach ($framework in $netfx) { $new = $new -replace "'$([regex]::Escape($framework))'", "'net10.0'" }
        $new = $new -replace "'\.NETFramework'", "'.NETCoreApp'"
        if ($new -ne $condition) { $element.SetAttribute('Condition', $new) }
    }
    if ($dropped) { $report.Add("${name}: target frameworks $($frameworks -join ';') -> net10.0 (items conditioned on $($dropped -join ', ') no longer apply)") }
    $packages = [ordered]@{}
    $itemGroup = $project.CreateElement('ItemGroup')
    foreach ($reference in @($project.SelectNodes('//Reference'))) {
        $assembly = ($reference.GetAttribute('Include') -split ',')[0].Trim()
        $hint = $reference.SelectSingleNode('HintPath')
        if ($hint) { continue }
        if ($frameworkReferences.ContainsKey($assembly)) {
            # In the reference's place: its ItemGroup's condition stays with it.
            $id, $version = $frameworkReferences[$assembly]
            if (-not $project.SelectSingleNode("//PackageReference[@Include='$id']")) {
                $package = $project.CreateElement('PackageReference')
                $package.SetAttribute('Include', $id)
                $package.SetAttribute('Version', $version)
                [void]$reference.ParentNode.InsertBefore($package, $reference)
                $packages[$id] = $version
            }
        }
        elseif ($noAnswer -contains $assembly) { $report.Add("${name}: no .NET answer: $assembly") }
        [void]$reference.ParentNode.RemoveChild($reference)
    }
    $sources = Get-ChildItem (Split-Path $projectPath -Parent) -Filter *.cs -Recurse -File | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } | ForEach-Object { $_.FullName }
    $fromSources = [ordered]@{}
    Add-SourcePackages $fromSources $sources $name
    foreach ($id in @($fromSources.Keys)) {
        if ($project.SelectSingleNode("//PackageReference[@Include='$id']")) { continue }
        Add-Package $packages $id $fromSources[$id]
        $item = $project.CreateElement('PackageReference')
        $item.SetAttribute('Include', $id)
        $item.SetAttribute('Version', $fromSources[$id])
        [void]$itemGroup.AppendChild($item)
    }
    foreach ($reference in @($project.SelectNodes('//PackageReference'))) {
        $id = $reference.GetAttribute('Include')
        if ($droppedPackages -contains $id) { [void]$reference.ParentNode.RemoveChild($reference); continue }
        if ($replacedPackages.ContainsKey($id)) {
            $newId, $version = $replacedPackages[$id]
            $reference.SetAttribute('Include', $newId)
            if ($reference.GetAttribute('Version')) { $reference.SetAttribute('Version', $version) }
        }
    }
    # Deployment steps copying build output into the site (XCOPY ...): Windows commands, and the
    # converted projects reach each other by ProjectReference.
    foreach ($exec in @($project.SelectNodes('//Target[Exec]'))) {
        $report.Add("${name}: target '$($exec.GetAttribute('Name'))' (Exec) removed: $((($exec.SelectNodes('Exec') | ForEach-Object { $_.GetAttribute('Command') }) -join ' / ').Substring(0, [Math]::Min(120, (($exec.SelectNodes('Exec') | ForEach-Object { $_.GetAttribute('Command') }) -join ' / ').Length)))")
        [void]$exec.ParentNode.RemoveChild($exec)
    }
    # Excluded files (as paths relative to the project) that are this project's.
    foreach ($file in $ExcludeFiles) {
        if (-not (Test-Path (Join-Path (Split-Path $projectPath -Parent) $file))) { continue }
        $remove = $project.CreateElement('Compile')
        $remove.SetAttribute('Remove', $file)
        [void]$itemGroup.AppendChild($remove)
    }
    if ($itemGroup.HasChildNodes) { [void]$project.Project.AppendChild($itemGroup) }
    $properties = $project.CreateElement('PropertyGroup')
    $properties.InnerXml = $commonProperties +
        "<RestoreAdditionalProjectSources>`$(MSBuildThisFileDirectory)$(Get-RelativePath (Split-Path $targetPath -Parent) ($PSScriptRoot + '\'))_feed</RestoreAdditionalProjectSources>"
    [void]$project.Project.AppendChild($properties)
    if ($project.SelectSingleNode("//PackageReference[starts-with(@Include, 'FrameworkOnCore.')]")) { Add-AliasTarget $project }
    $project.Save($targetPath)
    Write-Host "converted: $name (SDK; $($frameworks -join ';') -> net10.0, $($packages.Count) package(s) added)"
}

# WebFormsForCore's packages carry System.Web; the one in .NET's own reference set is removed.
function Add-AliasTarget([xml]$project) {
    $fragment = $project.CreateDocumentFragment()
    $xmlns = if ($project.DocumentElement.NamespaceURI) { ' xmlns="' + $project.DocumentElement.NamespaceURI + '"' } else { '' }
    $fragment.InnerXml = '<Target Name="ChangeAliasesOfNugetRefs" BeforeTargets="FindReferenceAssembliesForReferences;ResolveReferences"' + $xmlns + '><ItemGroup><ReferencePath Remove="%(Identity)" Condition="''%(FileName)'' == ''System.Web'' AND $([System.Text.RegularExpressions.Regex]::IsMatch(%(Identity),''[/\x5C]dotnet[/\x5C]''))" /><ReferencePath Remove="%(Identity)" Condition="''%(FileName)'' == ''System.Web.Services.Description''" /></ItemGroup></Target>'
    [void]$project.DocumentElement.AppendChild($fragment)
}

# An old-style (.NET Framework) project: a new SDK-style project file with the original's items.
function Convert-OldProject([string]$projectPath, [string]$targetPath, [string]$name, [xml]$old, [bool]$isWeb, $projectReferences) {
    $source = Split-Path $projectPath -Parent
    $target = Split-Path $targetPath -Parent
    $nodes = { param($xpath) Select-Xml -Xml $old -XPath $xpath -Namespace $msbuildNs | ForEach-Object { $_.Node } }

    # What the original compiled and embedded, in its order (linked files keep their paths: the
    # repository is copied as a whole).
    $compile = & $nodes '//m:Compile' | Where-Object { Test-Item $_ $name } | ForEach-Object { $_.Include } | Where-Object { $ExcludeFiles -notcontains $_ }
    $embedded = & $nodes '//m:EmbeddedResource' | Where-Object { Test-Item $_ $name } | ForEach-Object { $_.Include }
    $assemblyName = (& $nodes '//m:AssemblyName' | Select-Object -First 1).InnerText
    if (-not $assemblyName) { $assemblyName = $name }
    $rootNamespace = (& $nodes '//m:RootNamespace' | Select-Object -First 1).InnerText
    # Conditional compilation symbols of the configuration built (NET_4_0 and the like select code).
    $defines = & $nodes '//m:DefineConstants' | Where-Object { Test-Condition $_.ParentNode.GetAttribute('Condition') $name } |
        ForEach-Object { $_.InnerText -split '[;,]' } | ForEach-Object { $_.Trim() } | Where-Object { $_ -and $_ -notmatch '\$' } | Select-Object -Unique
    $defineProperty = if ($defines) { "<DefineConstants>`$(DefineConstants);$($defines -join ';')</DefineConstants>" } else { '' }

    # Packages (packages.config) and references.
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
    foreach ($reference in & $nodes '//m:PackageReference') {
        $id = $reference.Include
        $version = if ($reference.Version) { $reference.Version } else { ($reference.SelectSingleNode('*[local-name()="Version"]')).InnerText }
        if ($droppedPackages -contains $id) { continue }
        if ($replacedPackages.ContainsKey($id)) { $id, $version = $replacedPackages[$id] }
        $packages[$id] = $version
    }
    if ($isWeb) { foreach ($id in $webPackages) { Add-Package $packages $id $FrameworkOnCoreVersion } }

    $binaryReferences = @()
    $builtReferences = @()
    foreach ($reference in & $nodes '//m:Reference') {
        if (-not (Test-Item $reference $name)) { continue }
        $assembly = ($reference.Include -split ',')[0].Trim()
        $hint = $reference.HintPath
        if ($hint) {
            # A package's DLL is covered by the PackageReference above.
            if ($hint -match '(^|\\)packages\\') { continue }
            $dll = [IO.Path]::GetFullPath((Join-Path $source $hint))
            # Another project's build output: that project, by reference.
            $producer = Find-ProjectOfAssembly $assembly
            if ($producer -and ($hint -match '(^|\\)bin\\' -or -not (Test-Path $dll))) {
                Convert-One $producer $false
                $builtReferences += $producer
                continue
            }
            if (-not (Test-Path $dll)) { $report.Add("${name}: no .NET answer: $assembly (HintPath not found)"); continue }
            # A DLL checked into the repository: referenced where it is (the repository is copied).
            $binaryReferences += "    <Reference Include=""$assembly"" HintPath=""$hint"" />"
        }
        elseif ($frameworkReferences.ContainsKey($assembly)) {
            Add-Package $packages @($frameworkReferences[$assembly])[0] @($frameworkReferences[$assembly])[1]
            # SyndicationItem and friends were in System.ServiceModel(.Web); on .NET they are a package of their own.
            if ($assembly -like 'System.ServiceModel*') { Add-Package $packages 'System.ServiceModel.Syndication' '10.0.0' }
        }
        elseif ($noAnswer -contains $assembly) {
            $report.Add("${name}: no .NET answer: $assembly")
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
            if ($frameworkReferences.ContainsKey($assembly)) {
                Add-Package $packages @($frameworkReferences[$assembly])[0] @($frameworkReferences[$assembly])[1]
            }
        }
    }

    Add-SourcePackages $packages ($compile | ForEach-Object { Join-Path $source $_ }) $name
    $usesWebFormsForCore = $isWeb -or ($packages.Keys | Where-Object { $_ -like 'FrameworkOnCore.*' })
    $toScripts = '$(MSBuildThisFileDirectory)' + (Get-RelativePath $target ($PSScriptRoot + '\'))

    # The project file.
    if ($isWeb) {
        $text = (Get-Content (Join-Path $PSScriptRoot 'template.csproj.txt') -Raw -Encoding UTF8).Replace('__NAME__', $assemblyName)
        if ($rootNamespace) { $text = $text.Replace("<RootNamespace>$assemblyName</RootNamespace>", "<RootNamespace>$rootNamespace</RootNamespace>") }
        # The template's own four package references are covered by the list below.
        $text = [regex]::Replace($text, '(?s)  <ItemGroup>\s*<PackageReference Include="FrameworkOnCore.Runtime\.Web".*?</ItemGroup>\s*', '')
        $text = $text.Replace('<EnableDefaultContentItems>false</EnableDefaultContentItems>',
            "<EnableDefaultContentItems>false</EnableDefaultContentItems>`r`n    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>`r`n    <EnableDefaultEmbeddedResourceItems>false</EnableDefaultEmbeddedResourceItems>`r`n    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>`r`n    $commonProperties$defineProperty")
    }
    else {
        $text = @"
<Project Sdk="Microsoft.NET.Sdk">
  <!-- Converted library (experiments/wf4c/convert-project.ps1); sources unchanged. -->
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <AssemblyName>$assemblyName</AssemblyName>
    <RootNamespace>$rootNamespace</RootNamespace>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <EnableDefaultEmbeddedResourceItems>false</EnableDefaultEmbeddedResourceItems>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    $commonProperties$defineProperty
    <RestoreAdditionalProjectSources>`$(MSBuildThisFileDirectory)..\_feed</RestoreAdditionalProjectSources>
  </PropertyGroup>
__ALIAS__
</Project>
"@
        $alias = if ($usesWebFormsForCore) { @"

  <Target Name="ChangeAliasesOfNugetRefs" BeforeTargets="FindReferenceAssembliesForReferences;ResolveReferences">
    <ItemGroup>
      <ReferencePath Remove="%(Identity)" Condition="'%(FileName)' == 'System.Web' AND `$([System.Text.RegularExpressions.Regex]::IsMatch(%(Identity),'[/\x5C]dotnet[/\x5C]'))" />
      <ReferencePath Remove="%(Identity)" Condition="'%(FileName)' == 'System.Web.Services.Description'" />
    </ItemGroup>
  </Target>
"@ } else { '' }
        $text = $text.Replace('__ALIAS__', $alias)
    }
    $references = @($projectReferences) + @($builtReferences) | Select-Object -Unique |
        ForEach-Object { "    <ProjectReference Include=""`$(MSBuildThisFileDirectory)$(Get-RelativePath (Split-Path $projectPath -Parent) $_)"" />" }
    $items = "  <ItemGroup>`r`n" + (($compile | ForEach-Object { "    <Compile Include=""$_"" />" }) -join "`r`n")
    if ($isWeb) { $items += "`r`n    <Compile Include=""Program.cs"" />" }
    $items += "`r`n" + (($embedded | ForEach-Object { "    <EmbeddedResource Include=""$_"" />" }) -join "`r`n")
    $items += "`r`n  </ItemGroup>`r`n`r`n  <ItemGroup>`r`n" +
              (($packages.GetEnumerator() | ForEach-Object { "    <PackageReference Include=""$($_.Key)"" Version=""$($_.Value)"" />" }) -join "`r`n") +
              "`r`n" + ($binaryReferences -join "`r`n") + "`r`n" + ($references -join "`r`n") + "`r`n  </ItemGroup>`r`n`r`n"
    # The templates' paths into experiments\wf4c (the local feed, the shims), made relative to where
    # the project now is. Before the items go in: their paths are relative already.
    $text = $text.Replace('$(MSBuildThisFileDirectory)..\..\', '__SCRIPTS__').Replace('$(MSBuildThisFileDirectory)..\', '__SCRIPTS__').Replace('__SCRIPTS__', $toScripts)
    $anchor = if ($text.Contains('  <Target Name="ChangeAliasesOfNugetRefs"')) { '  <Target Name="ChangeAliasesOfNugetRefs"' } else { '</Project>' }
    $text = $text.Replace($anchor, $items + $anchor)
    Get-ChildItem $target -Filter *.csproj | Where-Object { $_.FullName -eq $targetPath } | Remove-Item
    Set-Content $targetPath $text -Encoding UTF8

    # Program.cs for the web project. An app that routes (System.Web.Routing, FriendlyUrls)
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

        if ($CultureProfile) {
            $appData = Join-Path $target 'App_Data'
            New-Item -ItemType Directory $appData -Force | Out-Null
            Copy-Item $CultureProfile (Join-Path $appData 'culture-profile.json')
        }
    }

    Write-Host "converted: $name ($($compile.Count) compile, $($packages.Count) packages, $($binaryReferences.Count) DLLs, $(@($references).Count) project refs)"
}

# Package versions below what another package needs (NU1605, an error): raised to that, as
# restore reports it, until restore is clean.
function Resolve-Downgrades([string]$webProject) {
    $env:DOTNET_CLI_UI_LANGUAGE = 'en'
    for ($round = 1; $round -le 10; $round++) {
        $output = dotnet restore $webProject -nologo 2>&1 | Out-String
        $downgrades = [regex]::Matches($output, 'Detected package downgrade: (\S+) from (\S+?)\.? to (\S+?)\.?\s') |
            ForEach-Object { [pscustomobject]@{ Id = $_.Groups[1].Value; From = $_.Groups[2].Value; To = $_.Groups[3].Value } } |
            Sort-Object Id, From -Unique
        if (-not $downgrades) { return }
        foreach ($d in $downgrades) {
            foreach ($file in $converted.Keys | ForEach-Object { Get-TargetPath $_ }) {
                $content = Get-Content $file -Raw -Encoding UTF8
                $pattern = '(<PackageReference Include="' + [regex]::Escape($d.Id) + '" Version=")' + [regex]::Escape($d.To) + '"'
                if ($content -match $pattern) {
                    Set-Content $file ([regex]::Replace($content, $pattern, '${1}' + $d.From + '"')) -Encoding UTF8
                    $report.Add("$([IO.Path]::GetFileNameWithoutExtension($file)): $($d.Id) raised $($d.To) -> $($d.From) (NU1605)")
                }
            }
        }
    }
}

$Project = (Resolve-Path $Project).Path
$sourceRoot = if ($Root) { (Resolve-Path $Root).Path } else { Find-Root $Project }
New-Item -ItemType Directory $Out -Force | Out-Null
$Out = (Resolve-Path $Out).Path
Write-Host "copying $sourceRoot -> $Out"
robocopy $sourceRoot $Out /MIR /XD bin obj packages .vs node_modules .git /NFL /NDL /NJH /NJS /NP | Out-Null
if ($LASTEXITCODE -ge 8) { throw "robocopy failed ($LASTEXITCODE)" }

Convert-One $Project $true
Resolve-Downgrades (Get-TargetPath $Project)
if ($report.Count) { Write-Host ($report -join "`r`n") }
Write-Host "web project: $(Get-TargetPath $Project)"
