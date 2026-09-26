# Experiment: turn an old-style (.NET Framework) Web Forms project into an SDK-style .NET 10
# project on WebFormsForCore, keeping every source file as it is. A prototype of what the new
# converter's project step has to do - see README.md for what was learned.
#
#   .\experiments\wf4c\convert-project.ps1 -Project <path\to\App.csproj> -Out <dir> [-Name App]
param(
    [Parameter(Mandatory = $true)][string]$Project,
    [Parameter(Mandatory = $true)][string]$Out,
    [string]$Name
)

$ErrorActionPreference = 'Stop'
$source = Split-Path (Resolve-Path $Project) -Parent
if (-not $Name) { $Name = [IO.Path]::GetFileNameWithoutExtension($Project) }

# 1. The application directory, as it is (build output and restored packages excluded).
if (Test-Path $Out) { Remove-Item $Out -Recurse -Force }
New-Item -ItemType Directory $Out | Out-Null
robocopy $source $Out /E /XD bin obj packages .vs /NFL /NDL /NJH /NJS /NP | Out-Null
Get-ChildItem $Out -Filter *.csproj | Remove-Item
Get-ChildItem $Out -Filter *.user | Remove-Item

# 2. What the original compiled, in its order.
[xml]$old = Get-Content (Resolve-Path $Project) -Raw -Encoding UTF8
$ns = @{ m = 'http://schemas.microsoft.com/developer/msbuild/2003' }
$compile = Select-Xml -Xml $old -XPath '//m:Compile/@Include' -Namespace $ns | ForEach-Object { $_.Node.Value }

# 3. Packages: from packages.config, with the .NET 10 answer for each.
$replaced = @{
    # WebFormsForCore ships its own port of these (same assembly names, .NET 10).
    'Microsoft.AspNet.Web.Optimization'          = @('WebFormsForCore.Web.Optimization', '1.6.5-w2l.1')
    'Microsoft.AspNet.Web.Optimization.WebForms' = @('WebFormsForCore.Web.Optimization.WebForms', '1.6.5-w2l.1')
    'WebGrease'                                  = @('WebFormsForCore.WebGrease', '1.6.5-w2l.1')
    'Microsoft.Web.Infrastructure'               = @('WebFormsForCore.Web.Infrastructure', '1.6.5-w2l.1')
    'AjaxControlToolkit'                         = @('WebFormsForCore.AjaxControlToolkit', '1.6.5-w2l.1')
    # First version that runs on .NET (Core).
    'EntityFramework'                            = @('EntityFramework', '6.5.1')
    # Raised to what WebFormsForCore depends on (a downgrade is NU1605); backward compatible.
    'Newtonsoft.Json'                            = @('Newtonsoft.Json', '13.0.4')
}
$dropped = @('NETStandard.Library', 'Microsoft.NETCore.Platforms', 'Microsoft.VisualStudio.Azure.Containers.Tools.Targets',
             'Antlr', 'Microsoft.CodeDom.Providers.DotNetCompilerPlatform', 'Microsoft.Net.Compilers')
$packages = @()
$config = Join-Path $source 'packages.config'
if (Test-Path $config) {
    [xml]$pc = Get-Content $config -Raw -Encoding UTF8
    foreach ($p in $pc.packages.package) {
        $id = $p.id; $version = $p.version
        # In the box on .NET: the 4.x System.* split packages.
        if ($id -like 'System.*' -and $version -match '^4\.') { continue }
        if ($dropped -contains $id) { continue }
        if ($replaced.ContainsKey($id)) { $id, $version = $replaced[$id] }
        $packages += "    <PackageReference Include=""$id"" Version=""$version"" />"
    }
}

# 4. The project.
$template = (Get-Content (Join-Path $PSScriptRoot 'template.csproj.txt') -Raw -Encoding UTF8).Replace('__NAME__', $Name)
$template = $template.Replace('<EnableDefaultContentItems>false</EnableDefaultContentItems>',
    "<EnableDefaultContentItems>false</EnableDefaultContentItems>`r`n    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>`r`n    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>`r`n    <NoWarn>`$(NoWarn);NU1701;NU1603;NU1608</NoWarn>")
$items = "  <ItemGroup>`r`n" + (($compile | ForEach-Object { "    <Compile Include=""$_"" />" }) -join "`r`n") + "`r`n    <Compile Include=""Program.cs"" />`r`n  </ItemGroup>`r`n`r`n" +
         "  <ItemGroup>`r`n" + ($packages -join "`r`n") + "`r`n  </ItemGroup>`r`n`r`n"
$template = $template.Replace('  <Target Name="ChangeAliasesOfNugetRefs"', $items + '  <Target Name="ChangeAliasesOfNugetRefs"')
Set-Content (Join-Path $Out "$Name.csproj") $template -Encoding UTF8

# 5. Program.cs. An app that routes (System.Web.Routing, FriendlyUrls) serves extensionless URLs,
# which only reach Web Forms when every request is handed to it; it also answers "/" itself
# (FriendlyUrls redirects Default.aspx to /Default), so IIS's default document is not emulated.
$program = Get-Content (Join-Path $PSScriptRoot 'Program.cs.txt') -Raw -Encoding UTF8
$routes = ($packages -match 'Microsoft\.AspNet\.FriendlyUrls') -or
          (Get-ChildItem $Out -Filter *.cs -Recurse | Select-String -Pattern 'RouteTable\.Routes|RouteCollection' -List -Quiet)
if ($routes) {
    $program = $program.Replace('app.UseDefaultFiles(defaults);', '// Routed app: Web Forms answers "/" (see convert-project.ps1).')
    $program = $program.Replace('options => options.UseAspNetCoreSessionProvider()', 'options => options.HandleAllRequestsWithWebForms().UseAspNetCoreSessionProvider()')
}
Set-Content (Join-Path $Out 'Program.cs') $program -Encoding UTF8
"converted: $Name ($($compile.Count) compile items, $($packages.Count) packages) -> $Out"
