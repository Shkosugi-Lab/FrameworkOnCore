# Captures what the original IIS server sets for a site outside the application's web.config: the server's settings
# (applicationHost.config's system.webServer, the ones the application inherits), the site's own (its <location>s), its log
# settings, the modules IIS has. FrameworkOnCore's host reads it (iis-profile.config in the converted application: the
# converter's --iis-profile) as the server's settings under web.config's, and the converter reports the IIS features in
# use that the host does not have. Run it ON THE ORIGINAL SERVER, as an administrator (applicationHost.config is the
# administrators' only).
#
#   powershell -File capture-iis.ps1 -Site "Default Web Site" -Out iis-profile.config
#
# Not captured: secrets (attributes named password, and the ones IIS keeps encrypted: [enc:...]), the sites' and
# application pools' other settings. A shared configuration (redirection.config) is read where it points.
param(
    [Parameter(Mandatory = $true)][string]$Site,
    [Parameter(Mandatory = $true)][string]$Out,
    [string]$ApplicationHost = (Join-Path $env:SystemRoot 'System32\inetsrv\config\applicationHost.config')
)

$ErrorActionPreference = 'Stop'

# A path as given, from PowerShell's location when relative (.NET's is the process's); as it is ([ ] are no wildcards).
# Started by powershell -File in a folder with [ ] in its name, Windows PowerShell cannot make that folder its location
# and is in its own ($PSHOME) instead: the folder it was started in then (the process's).
function Full-Path([string]$Path) {
    $from = (Get-Location).ProviderPath
    if ($from -eq $PSHOME -and [Environment]::CurrentDirectory -ne $from -and [Environment]::CurrentDirectory -match '[\[\]]') { $from = [Environment]::CurrentDirectory }
    [IO.Path]::GetFullPath($(if ([IO.Path]::IsPathRooted($Path)) { $Path } else { Join-Path $from $Path }))
}

# An XML file in the encoding it says (its declaration, UTF-8 without one), as IIS reads it. Get-Content read it in the
# system's code page on Windows PowerShell: a Japanese site's name or folder garbled, the file no XML (the capture failed).
# Read as a file, not as a URL (XmlDocument.Load of a name: a '#' or a '%' in a folder's name is a URL's). What it is
# not is told with its name: not there, a folder, no XML.
function Read-Xml([string]$Path, [string]$What) {
    $full = Full-Path $Path
    if (Test-Path -LiteralPath $full -PathType Container) { throw "${What}: $full is a folder, not a file" }
    if (-not (Test-Path -LiteralPath $full -PathType Leaf)) { throw "${What}: $full is not there" }
    $document = New-Object System.Xml.XmlDocument
    $stream = $null
    try {
        $stream = [IO.File]::OpenRead($full)
        $document.Load($stream)
    }
    catch [System.Xml.XmlException] { throw "${What}: $full is no XML ($($_.Exception.Message))" }
    catch [System.Management.Automation.MethodInvocationException] {
        if ($_.Exception.InnerException -is [System.Xml.XmlException]) { throw "${What}: $full is no XML ($($_.Exception.InnerException.Message))" }
        throw "${What}: $full cannot be read ($($_.Exception.InnerException.Message))"
    }
    finally { if ($stream) { $stream.Dispose() } }
    $document
}

# A shared configuration: applicationHost.config is where redirection.config says.
$ApplicationHost = Full-Path $ApplicationHost
$redirection = Join-Path (Split-Path $ApplicationHost) 'redirection.config'
if (Test-Path -LiteralPath $redirection -PathType Leaf) {
    $redirect = (Read-Xml $redirection 'redirection.config').SelectSingleNode('/configuration/configurationRedirection')
    if ($redirect -and $redirect.GetAttribute('enabled') -eq 'true' -and $redirect.GetAttribute('path')) { $ApplicationHost = Join-Path $redirect.GetAttribute('path') 'applicationHost.config' }
}
$config = Read-Xml $ApplicationHost 'applicationHost.config'
$root = $config.SelectSingleNode('/configuration')
if (-not $root) { throw "applicationHost.config: $ApplicationHost is no IIS configuration (its root is <$($config.DocumentElement.Name)>, not <configuration>)" }
$sitesNode = $root.SelectSingleNode('system.applicationHost/sites')
if (-not $sitesNode) { throw "applicationHost.config: $ApplicationHost has no sites (system.applicationHost/sites): not a server's applicationHost.config" }
# The output: a file's path (told before anything is read into it).
$target = Full-Path $Out
if (Test-Path -LiteralPath $target -PathType Container) { throw "-Out $Out is a folder: give the profile's file (iis-profile.config)" }

# The system.webServer sections IIS applies to a site's requests (the server's and the site's); not the ones about the
# server itself.
$sections = @('defaultDocument', 'directoryBrowse', 'httpErrors', 'httpRedirect', 'httpProtocol', 'httpCompression',
    'urlCompression', 'staticContent', 'caching', 'serverRuntime', 'httpLogging', 'security', 'rewrite', 'modules', 'handlers',
    'isapiFilters', 'cgi', 'asp', 'serverSideInclude', 'webdav', 'applicationInitialization', 'tracing', 'httpTracing',
    'validation', 'odbcLogging')

$document = New-Object Xml.XmlDocument
$iisProfile = $document.AppendChild($document.CreateElement('iisProfile'))
$iisProfile.SetAttribute('version', '1')
$iisProfile.SetAttribute('site', $Site)
$iisProfile.SetAttribute('captured', [DateTime]::UtcNow.ToString('yyyy-MM-ddTHH:mm:ssZ'))
$iisProfile.SetAttribute('computer', $env:COMPUTERNAME)
$inetstp = Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\InetStp' -ErrorAction SilentlyContinue
if ($inetstp) { $iisProfile.SetAttribute('iisVersion', "$($inetstp.MajorVersion).$($inetstp.MinorVersion)") }

# Secrets out: password attributes, encrypted values; and the secrets inside a value: a connection string's password
# ("...;Password=x;", "pwd=x"), a URL's ("https://user:x@host"), the value of an entry named as a secret (a custom header
# X-Api-Password, a setting ApiKey: its name, key or header says password, secret, token or key).
$secretName = 'password|passwd|pwd|secret|token|api[-_]?key|credential'
function Copy-Clean([Xml.XmlNode]$node) {
    $copy = $document.ImportNode($node, $true)
    foreach ($element in @($copy.SelectNodes('descendant-or-self::*'))) {
        $named = @('name', 'key', 'header') | ForEach-Object { $element.GetAttribute($_) } | Where-Object { $_ -match $secretName }
        foreach ($attribute in @($element.Attributes)) {
            if ($attribute.Name -match 'password' -or $attribute.Value -match '^\[enc:') { [void]$element.Attributes.Remove($attribute); continue }
            if ($named -and $attribute.Name -eq 'value') { [void]$element.Attributes.Remove($attribute); continue }
            $value = $attribute.Value
            $value = [regex]::Replace($value, "(?i)\b($secretName)\s*=\s*[^;]*", '$1=')
            $value = [regex]::Replace($value, '(?i)\b([a-z][a-z0-9+.-]*://[^/:@\s]*):[^@/\s]*@', '$1@')
            if ($value -cne $attribute.Value) { $attribute.Value = $value }
        }
    }
    $copy
}

function Copy-Sections([Xml.XmlNode]$from, [Xml.XmlNode]$to) {
    $webServer = $from.SelectSingleNode('system.webServer')
    if (-not $webServer) { return }
    $target = $to.SelectSingleNode('system.webServer')
    if (-not $target) { $target = $to.AppendChild($document.CreateElement('system.webServer')) }
    foreach ($child in $webServer.ChildNodes) {
        if ($child.NodeType -eq 'Element' -and $sections -contains $child.Name) { [void]$target.AppendChild((Copy-Clean $child)) }
    }
}

# The server's: applicationHost.config's own system.webServer, and its <location path=""> (every site's: the modules,
# the handlers...).
Copy-Sections $root $iisProfile
foreach ($location in @($root.SelectNodes('location'))) {
    if ([string]$location.path -eq '' -or [string]$location.path -eq '.') { Copy-Sections $location $iisProfile }
}

# The modules IIS has (installed features: compression, Windows authentication, URL Rewrite...).
$globalModules = $root.SelectSingleNode('system.webServer/globalModules')
if ($globalModules) { [void]$iisProfile.AppendChild((Copy-Clean $globalModules)) }

# The site: its id (W3SVC<id>), its log file, its limits; the sites' defaults; the server's log.
$siteNode = @($sitesNode.SelectNodes('site')) | Where-Object { $_.name -eq $Site } | Select-Object -First 1
if (-not $siteNode) { throw "no site '$Site' in $ApplicationHost (sites: $((@($sitesNode.SelectNodes('site')) | ForEach-Object { $_.name }) -join ', '))" }
$siteCopy = $iisProfile.AppendChild($document.CreateElement('site'))
$siteCopy.SetAttribute('id', $siteNode.id)
foreach ($name in 'logFile', 'limits', 'traceFailedRequestsLogging') {
    $node = $siteNode.SelectSingleNode($name)
    if ($node) { [void]$siteCopy.AppendChild((Copy-Clean $node)) }
}
$defaults = $sitesNode.SelectSingleNode('siteDefaults')
if ($defaults) {
    $defaultsCopy = $iisProfile.AppendChild($document.CreateElement('siteDefaults'))
    foreach ($name in 'logFile', 'limits') {
        $node = $defaults.SelectSingleNode($name)
        if ($node) { [void]$defaultsCopy.AppendChild((Copy-Clean $node)) }
    }
}
$log = $root.SelectSingleNode('system.applicationHost/log')
if ($log) { [void]$iisProfile.AppendChild((Copy-Clean $log)) }

# The site's <location>s (the site and its folders): their paths relative to the site.
foreach ($location in @($root.SelectNodes('location'))) {
    # (without regard to case, as IIS takes a location's path: "app/Admin" is the site App's)
    $path = [string]$location.path
    if ($path -ne $Site -and -not $path.StartsWith("$Site/", [StringComparison]::OrdinalIgnoreCase)) { continue }
    $copy = $iisProfile.AppendChild($document.CreateElement('location'))
    $copy.SetAttribute('path', $path.Substring($Site.Length).TrimStart('/'))
    Copy-Sections $location $copy
}

$settings = New-Object Xml.XmlWriterSettings
$settings.Indent = $true
$settings.Encoding = New-Object Text.UTF8Encoding($false)
# (its folder made when it is not there, as capture-culture.ps1 does)
[void][IO.Directory]::CreateDirectory((Split-Path $target -Parent))
$writer = [Xml.XmlWriter]::Create($target, $settings)
$document.Save($writer)
$writer.Dispose()
"captured: site '$Site' (W3SVC$($siteNode.id)) -> $Out"
