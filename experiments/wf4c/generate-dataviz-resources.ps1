# Generates Core\SR.resx of the System.Web.DataVisualization port (WebFormsForCore\src\WebFormsForCore.Web.DataVisualization)
# from .NET Framework 4.8's own System.Web.DataVisualization.dll, so that the port's messages and property descriptions
# are exactly .NET Framework's.
#
# referencesource has the generated accessors (Common\SR.cs, from SR.strings) but not SR.strings: the ResourceManager
# of SR.Keys finds nothing, and every message would be null. The real assembly has the resources
# (System.Web.UI.DataVisualization.Charting.SR.resources, 1,290 strings); the port embeds them under the same name.
# Its other resources are not taken: Design.resources are the chart types' bitmaps for the Visual Studio designer
# (serialized with BinaryFormatter, which .NET does not read), ChartControl.ico the toolbox icon.
#
# Windows only (loads the GAC assembly); run under Windows PowerShell 5.1 (.NET Framework). The output is committed
# with the port, so this runs only when regenerating.
$ErrorActionPreference = 'Stop'
if ($PSVersionTable.PSEdition -eq 'Core') { throw 'Run under Windows PowerShell 5.1 (needs .NET Framework to load the GAC assembly).' }
Add-Type -AssemblyName System.Windows.Forms   # ResXResourceWriter

$assembly = [Reflection.Assembly]::Load('System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35')
$out = Join-Path $PSScriptRoot '..\..\WebFormsForCore\src\WebFormsForCore.Web.DataVisualization\Core'
New-Item -ItemType Directory $out -Force | Out-Null

$reader = New-Object Resources.ResourceReader($assembly.GetManifestResourceStream('System.Web.UI.DataVisualization.Charting.SR.resources'))
# In name order: the file does not change from one run to the next.
$entries = @($reader | ForEach-Object { $_ }) | Sort-Object { [string]$_.Key } -Culture ([Globalization.CultureInfo]::InvariantCulture)
$reader.Close()

$path = Join-Path $out 'SR.resx'
$writer = New-Object Resources.ResXResourceWriter($path)
foreach ($entry in $entries) {
    if ($entry.Value -isnot [string]) { throw "$($entry.Key): not a string ($($entry.Value.GetType()))" }
    $writer.AddResource([string]$entry.Key, [string]$entry.Value)
}
$writer.Generate()
$writer.Close()
"generated $($entries.Count) strings -> $path"
