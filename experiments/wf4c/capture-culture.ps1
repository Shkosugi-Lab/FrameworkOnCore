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

# A path as given, from PowerShell's location when relative (.NET's is the process's); as it is ([ ] are no wildcards).
# Started by powershell -File in a folder with [ ] in its name, Windows PowerShell cannot make that folder its location
# and is in its own ($PSHOME) instead: the folder it was started in then (the process's).
function Full-Path([string]$Path) {
    $from = (Get-Location).ProviderPath
    if ($from -eq $PSHOME -and [Environment]::CurrentDirectory -ne $from -and [Environment]::CurrentDirectory -match '[\[\]]') { $from = [Environment]::CurrentDirectory }
    [IO.Path]::GetFullPath($(if ([IO.Path]::IsPathRooted($Path)) { $Path } else { Join-Path $from $Path }))
}
# The output: a file's path (told before anything is captured).
$path = Full-Path $Out
if (Test-Path -LiteralPath $path -PathType Container) { throw "-Out $Out is a folder: give the profile's file (culture-profile.json)" }
# -Cultures as powershell -File gives them: "en-US,fr-FR" is one argument there (an array only to a script called from
# PowerShell); each name of each argument.
$Cultures = @($Cultures | ForEach-Object { $_ -split ',' } | ForEach-Object { $_.Trim() } | Where-Object { $_ })
# The cultures named (web.config's, -Cultures): a neutral one of them is captured too.
$named = New-Object Collections.Generic.List[string]
$Cultures | ForEach-Object { $named.Add($_) }

$names = New-Object Collections.Generic.List[string]
$default = [Globalization.CultureInfo]::CurrentCulture.Name
$defaultUI = [Globalization.CultureInfo]::CurrentUICulture.Name
$names.Add($default)
$names.Add($defaultUI)
if ($WebConfig) {
    # In the encoding it says (an application's may be encoding="shift_jis"); a path as it is ([ ] in a folder's name,
    # WingtipToys VB's "[VB]-...", are no wildcards).
    # Read as a file, not as a URL (a '#' or a '%' in a folder's name is a URL's); what it is not is told with its name.
    $full = Full-Path $WebConfig
    if (Test-Path -LiteralPath $full -PathType Container) { throw "-WebConfig $WebConfig is a folder: give the application's web.config" }
    if (-not (Test-Path -LiteralPath $full -PathType Leaf)) { throw "-WebConfig $WebConfig (web.config) is not there" }
    $config = New-Object System.Xml.XmlDocument
    $stream = $null
    try {
        $stream = [IO.File]::OpenRead($full)
        $config.Load($stream)
    }
    catch { throw "-WebConfig $WebConfig (web.config) is no XML: $(if ($_.Exception.InnerException) { $_.Exception.InnerException.Message } else { $_.Exception.Message })" }
    finally { if ($stream) { $stream.Dispose() } }
    # Its globalization elements: the application's (system.web under configuration, or in a <location> for the
    # application itself or a folder), whatever the namespace (an old web.config has .NET 2.0's on configuration).
    $globalizations = @($config.SelectNodes("/*[local-name()='configuration']/*[local-name()='system.web']/*[local-name()='globalization'] | /*[local-name()='configuration']/*[local-name()='location']/*[local-name()='system.web']/*[local-name()='globalization']"))
    foreach ($globalization in $globalizations) {
        foreach ($attribute in 'culture', 'uiCulture') {
            $value = $globalization.GetAttribute($attribute)
            # "auto:ja-JP" falls back to ja-JP; plain "auto" follows the browser (name them in -Cultures).
            if ($value -like 'auto:*') { $value = $value.Substring(5) }
            if ($value -and $value -ne 'auto') { $names.Add($value); $named.Add($value) }
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
    # The server's time zone (Windows' id): IIS ran the application in it; a Linux container runs in UTC (TZ).
    TimeZone         = [TimeZoneInfo]::Local.Id
    # The .NET Framework language packs installed (System.Web's messages and controls' texts in that language when
    # the UI culture is it: Calendar's titles, Login's labels, error pages): the converter gives them to the port.
    LanguagePacks    = @(Get-ChildItem "$env:windir\Microsoft.NET\assembly\GAC_MSIL\System.Web.resources" -Directory -ErrorAction SilentlyContinue |
        ForEach-Object { if ($_.Name -match '^v4\.0_[^_]+_([^_]+)_') { $Matches[1] } } | Sort-Object -Unique)
    Cultures         = [ordered]@{}
}
foreach ($name in ($names | Where-Object { $_ } | Select-Object -Unique)) {
    # (a name that is no culture's, a typing mistake: told with the name; .NET Framework's own word for it is "not supported")
    try { $culture = [Globalization.CultureInfo]::GetCultureInfo($name) }
    catch { throw "'$name' is no culture this machine has (given in -Cultures or named by web.config's globalization): a name as en-US, ja-JP, de" }
    $culture = New-Object Globalization.CultureInfo $culture.Name
    if ($culture.Name -eq '') { continue }   # no formats of its own
    # A neutral culture only when named (a page's UICulture="zh-Hans", Culture="de"; web.config's): .NET Framework gives it formats.
    if ($culture.IsNeutralCulture -and $named -notcontains $name) { continue }
    if ($profile.Cultures.Contains($culture.Name)) { continue }   # (named twice, in another case)
    $date = Get-Settable $culture.DateTimeFormat
    $all = [ordered]@{}
    foreach ($format in 'd', 'D', 't', 'T', 'y', 'Y') { $all[$format] = @($culture.DateTimeFormat.GetAllDateTimePatterns([char]$format)) }
    $date['AllDateTimePatterns'] = $all
    # The calendar's: a two-digit year's century (Windows' setting: 2049 for the Gregorian, 2592 for the Thai Buddhist).
    $date['TwoDigitYearMax'] = $culture.DateTimeFormat.Calendar.TwoDigitYearMax
    $profile.Cultures[$culture.Name] = [ordered]@{
        # Windows' names (ICU's differ: "Chinese (Taiwan)" for "Chinese (Traditional, Taiwan)").
        EnglishName    = $culture.EnglishName
        NativeName     = $culture.NativeName
        NumberFormat   = Get-Settable $culture.NumberFormat
        DateTimeFormat = $date
    }
}

$json = $profile | ConvertTo-Json -Depth 6
[void][IO.Directory]::CreateDirectory((Split-Path $path -Parent))
[IO.File]::WriteAllText($path, $json, (New-Object Text.UTF8Encoding $false))
"captured $($profile.Cultures.Count) culture(s) ($($profile.Cultures.Keys -join ', ')); default $default / $defaultUI -> $Out"
