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

# A shared configuration: applicationHost.config is where redirection.config says.
$redirection = Join-Path (Split-Path $ApplicationHost) 'redirection.config'
if (Test-Path $redirection) {
    $redirect = ([xml](Get-Content $redirection -Raw)).configuration.'configurationRedirection'
    if ($redirect -and $redirect.enabled -eq 'true' -and $redirect.path) { $ApplicationHost = Join-Path $redirect.path 'applicationHost.config' }
}
[xml]$config = Get-Content $ApplicationHost -Raw
$root = $config.configuration

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

# Secrets out: password attributes, encrypted values.
function Copy-Clean([Xml.XmlNode]$node) {
    $copy = $document.ImportNode($node, $true)
    foreach ($element in @($copy.SelectNodes('descendant-or-self::*'))) {
        foreach ($attribute in @($element.Attributes)) {
            if ($attribute.Name -match 'password' -or $attribute.Value -match '^\[enc:') { [void]$element.Attributes.Remove($attribute) }
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
$sitesNode = $root.SelectSingleNode('system.applicationHost/sites')
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
    $path = [string]$location.path
    if ($path -ne $Site -and -not $path.StartsWith("$Site/")) { continue }
    $copy = $iisProfile.AppendChild($document.CreateElement('location'))
    $copy.SetAttribute('path', $path.Substring($Site.Length).TrimStart('/'))
    Copy-Sections $location $copy
}

$settings = New-Object Xml.XmlWriterSettings
$settings.Indent = $true
$settings.Encoding = New-Object Text.UTF8Encoding($false)
$target = if ([IO.Path]::IsPathRooted($Out)) { $Out } else { Join-Path (Get-Location) $Out }
$writer = [Xml.XmlWriter]::Create($target, $settings)
$document.Save($writer)
$writer.Dispose()
"captured: site '$Site' (W3SVC$($siteNode.id)) -> $Out"
