# Captures the culture data of the server a Web Forms application runs on (number and date
# formats), for WebFormsForCore.CultureProfile to lay over .NET's ICU data (see FrameworkOnCore's
# CultureProfile.cs). Run it ON THE ORIGINAL SERVER with Windows PowerShell (powershell.exe, which
# runs on .NET Framework and so sees the culture data the application sees).
#
#   powershell -File capture-culture.ps1 -Out culture-profile.json [-WebConfig <app>\web.config] [-Cultures en-US,fr-FR]
#
# Captured: the default culture (IIS gives it to an application whose web.config names none), the
# cultures the web.config's <globalization> names, and -Cultures (e.g. the ones pages set). Each as
# `new CultureInfo(name)` gives it, which is how System.Web creates them. User overrides are the
# ones of the user running this; an application pool's identity may have others.
param(
    [Parameter(Mandatory = $true)][string]$Out,
    [string]$WebConfig,
    [string[]]$Cultures = @()
)

$ErrorActionPreference = 'Stop'
if ($PSVersionTable.PSEdition -eq 'Core') {
    throw 'Run this with Windows PowerShell (powershell.exe): PowerShell 7 runs on .NET, whose culture data are the ones to be replaced.'
}

$names = New-Object Collections.Generic.List[string]
$default = [Globalization.CultureInfo]::CurrentCulture.Name
$defaultUI = [Globalization.CultureInfo]::CurrentUICulture.Name
$names.Add($default)
$names.Add($defaultUI)
if ($WebConfig) {
    [xml]$config = Get-Content $WebConfig -Raw -Encoding UTF8
    $globalization = $config.SelectSingleNode('/configuration/system.web/globalization')
    if ($globalization) {
        foreach ($attribute in 'culture', 'uiCulture') {
            $value = $globalization.GetAttribute($attribute)
            # "auto:ja-JP" falls back to ja-JP; plain "auto" follows the browser (name them in -Cultures).
            if ($value -like 'auto:*') { $value = $value.Substring(5) }
            if ($value -and $value -ne 'auto') { $names.Add($value) }
        }
    }
}
$Cultures | ForEach-Object { $names.Add($_) }

# Public settable properties of plain types: what CultureProfile.Apply sets back.
function Get-Settable($object) {
    $values = [ordered]@{}
    foreach ($property in $object.GetType().GetProperties()) {
        if (-not $property.CanWrite -or $property.GetIndexParameters().Count -gt 0) { continue }
        $type = $property.PropertyType
        if ($type -ne [string] -and $type -ne [int] -and -not $type.IsEnum -and $type -ne [int[]] -and $type -ne [string[]]) { continue }
        $value = $property.GetValue($object, $null)
        if ($type.IsEnum) { $value = [int]$value }
        elseif ($type.IsArray) { $value = @($value) }
        $values[$property.Name] = $value
    }
    return $values
}

$profile = [ordered]@{
    Source           = [ordered]@{
        Machine   = $env:COMPUTERNAME
        User      = "$env:USERDOMAIN\$env:USERNAME"
        Framework = [Environment]::Version.ToString()
        Captured  = (Get-Date).ToString('s')
    }
    DefaultCulture   = $default
    DefaultUICulture = $defaultUI
    Cultures         = [ordered]@{}
}
foreach ($name in ($names | Where-Object { $_ } | Select-Object -Unique)) {
    $culture = New-Object Globalization.CultureInfo $name
    if ($culture.IsNeutralCulture -or $culture.Name -eq '') { continue }   # no formats of their own
    $date = Get-Settable $culture.DateTimeFormat
    $all = [ordered]@{}
    foreach ($format in 'd', 'D', 't', 'T', 'y', 'Y') { $all[$format] = @($culture.DateTimeFormat.GetAllDateTimePatterns([char]$format)) }
    $date['AllDateTimePatterns'] = $all
    $profile.Cultures[$culture.Name] = [ordered]@{
        NumberFormat   = Get-Settable $culture.NumberFormat
        DateTimeFormat = $date
    }
}

$json = $profile | ConvertTo-Json -Depth 6
$path = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Out)
New-Item -ItemType Directory (Split-Path $path -Parent) -Force | Out-Null
[IO.File]::WriteAllText($path, $json, (New-Object Text.UTF8Encoding $false))
"captured $($profile.Cultures.Count) culture(s) ($($profile.Cultures.Keys -join ', ')); default $default / $defaultUI -> $Out"
