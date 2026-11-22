# Records IIS's answers to its settings one at a time (SettingsTests) into iis-settings.json beside this script, and what is
# tried and what shows into IIS-SETTINGS.md: the variants are made from this machine's IIS configuration schema
# (%windir%\system32\inetsrv\config\schema), each a folder's web.config of a throwaway site, the same probes sent to each.
#
# Record again (Windows with IIS 10, ASP.NET 4.8, URL Rewrite 2.1, the Windows features IIS-URLAuthorization, IIS-IPSecurity,
# IIS-BasicAuthentication and IIS-WebSockets; an elevated PowerShell, from the repository's root):
#   powershell -ExecutionPolicy Bypass -File tests/RuntimeUnitTests/Security/iis-golden/settings.ps1
# It makes a site (its own application pool) on a free port of 127.0.0.1 over a folder in %TEMP%, unlocks for that site only
# the sections IIS locks by default that are tried (security/access, anonymous, basic and Windows authentication, IP and
# dynamic IP restrictions, serverRuntime, webSocket:
# an application that has them in its web.config ran on a server that had them unlocked), runs the test
# SettingsTests.Record against it (FOC_IIS_SETTINGS=port;folder;schema), then removes the site, the pool, the site's
# location in applicationHost.config and the folder. The other sites are not touched.
$ErrorActionPreference = 'Stop'
$appcmd = Join-Path $env:windir 'system32\inetsrv\appcmd.exe'
if (-not (Test-Path $appcmd)) { throw 'IIS is not installed (no appcmd.exe)' }
$schema = Join-Path $env:windir 'system32\inetsrv\config\schema'
$repository = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
$project = Join-Path $repository 'tests\RuntimeUnitTests\RuntimeUnitTests.csproj'

$listener = [System.Net.Sockets.TcpListener]::new([System.Net.IPAddress]::Loopback, 0)
$listener.Start(); $port = $listener.LocalEndpoint.Port; $listener.Stop()

$name = 'foc-iis-settings'
$root = Join-Path $env:TEMP ($name + '-' + [guid]::NewGuid().ToString('N').Substring(0, 8))
New-Item -ItemType Directory -Path $root | Out-Null
& icacls $root /grant 'IIS_IUSRS:(OI)(CI)RX' 'IUSR:(OI)(CI)RX' | Out-Null
$unlocked = @('security/access', 'security/authentication/anonymousAuthentication', 'security/authentication/windowsAuthentication',
    'security/authentication/basicAuthentication', 'security/ipSecurity', 'security/dynamicIpSecurity', 'serverRuntime', 'webSocket')
try {
    & $appcmd add apppool /name:$name /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
    & $appcmd add site /name:$name "/bindings:http/127.0.0.1:${port}:" /physicalPath:$root | Out-Null
    & $appcmd set app "$name/" /applicationPool:$name | Out-Null
    foreach ($section in $unlocked) {
        & $appcmd unlock config $name "-section:system.webServer/$section" /commit:apphost | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "system.webServer/$section could not be unlocked for the site" }
    }
    & $appcmd start site /site.name:$name | Out-Null
    $env:FOC_IIS_SETTINGS = "$port;$root;$schema"
    $ErrorActionPreference = 'Continue'
    & dotnet test $project -nologo -v q --filter 'FullyQualifiedName~SettingsTests.Record'
    $exit = $LASTEXITCODE
    $ErrorActionPreference = 'Stop'
    if ($exit -ne 0) { throw "the recording failed ($exit)" }
    # The site's server settings, as the converter's --iis-profile gives them to a converted application (capture-iis.ps1):
    # the port runs the settings' site with them (iis-settings-profile.config; the site's folder written {root}).
    $serverSettings = Join-Path $PSScriptRoot 'iis-settings-profile.config'
    & powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $repository 'experiments\wf4c\capture-iis.ps1') -Site $name -Out $serverSettings
    if ($LASTEXITCODE -ne 0) { throw 'the site''s settings could not be captured' }
    $text = [IO.File]::ReadAllText($serverSettings).Replace($root, '{root}').Replace("127.0.0.1:${port}:", '127.0.0.1:{port}:')
    [IO.File]::WriteAllText($serverSettings, $text.Replace("`r`n", "`n"), (New-Object System.Text.UTF8Encoding $false))
}
finally {
    $ErrorActionPreference = 'Continue'
    Remove-Item Env:FOC_IIS_SETTINGS -ErrorAction SilentlyContinue
    & $appcmd stop site /site.name:$name | Out-Null
    & $appcmd delete site $name | Out-Null
    & $appcmd delete apppool $name | Out-Null
    # The site's location (the unlocked sections) out of applicationHost.config.
    Add-Type -Path (Join-Path $env:windir 'system32\inetsrv\Microsoft.Web.Administration.dll')
    $manager = New-Object Microsoft.Web.Administration.ServerManager
    $manager.GetApplicationHostConfiguration().RemoveLocationPath($name)
    $manager.CommitChanges()
    Start-Sleep -Seconds 2
    if (Test-Path $root) { Remove-Item -Recurse -Force $root -ErrorAction SilentlyContinue }
    Write-Host "removed: site and application pool $name, its location in applicationHost.config, $root"
}
