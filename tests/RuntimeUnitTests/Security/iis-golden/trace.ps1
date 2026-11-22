# Records IIS's own traces of the attack tests' cases (failed request tracing) into iis-trace.json beside this script, and
# IIS's request pipeline as they show it into IIS-PIPELINE.md: for each case the handler IIS mapped, the module that set
# the status and in which notification, the URLs it was changed to, the URL Rewrite rules that matched, the modules
# notified in order. iis.json (record.ps1) says what IIS answers; the traces say which module answers and when.
#
# Record again (Windows with IIS 10, ASP.NET 4.8, URL Rewrite 2.1 and the Windows feature IIS-HttpTracing; an elevated
# PowerShell, from the repository's root):
#   powershell -ExecutionPolicy Bypass -File tests/RuntimeUnitTests/Security/iis-golden/trace.ps1
# It makes a throwaway site (its own application pool) on a free port of 127.0.0.1 over a folder in %TEMP%, with a
# tracing rule for every status, runs the test AttackTests.Trace against it (FOC_IIS_TRACE=port;folder;traces), then
# removes the site, the pool, its tracing rule and the folders. The other sites are not touched. Once for the machine,
# when URL Rewrite was installed before the tracing feature, its trace area is defined (WWW Server: Rewrite, 1024, as
# its installer defines it): the only change that stays.
param([string]$KeepTraces)   # a folder the trace files are copied to (to read one by hand)
$ErrorActionPreference = 'Stop'
$appcmd = Join-Path $env:windir 'system32\inetsrv\appcmd.exe'
if (-not (Test-Path $appcmd)) { throw 'IIS is not installed (no appcmd.exe)' }
if (-not ((& $appcmd list config -section:system.webServer/globalModules) -match 'FailedRequestsTracingModule')) {
    throw 'IIS''s tracing is not installed: Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpTracing -All'
}
if (-not ((& $appcmd list config -section:system.webServer/tracing/traceProviderDefinitions | Out-String) -match 'name="Rewrite"')) {
    & $appcmd set config -section:system.webServer/tracing/traceProviderDefinitions "/+[name='WWW Server'].areas.[name='Rewrite',value='1024']" /commit:apphost | Out-Null
}
$repository = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
$project = Join-Path $repository 'tests\RuntimeUnitTests\RuntimeUnitTests.csproj'

$listener = [System.Net.Sockets.TcpListener]::new([System.Net.IPAddress]::Loopback, 0)
$listener.Start(); $port = $listener.LocalEndpoint.Port; $listener.Stop()

$name = 'foc-iis-trace'
$root = Join-Path $env:TEMP ($name + '-' + [guid]::NewGuid().ToString('N').Substring(0, 8))
$traces = "$root-traces"
New-Item -ItemType Directory -Path $root, $traces | Out-Null
& icacls $root /grant 'IIS_IUSRS:(OI)(CI)RX' 'IUSR:(OI)(CI)RX' | Out-Null
& icacls $traces /grant 'IIS_IUSRS:(OI)(CI)M' | Out-Null
$section = 'system.webServer/tracing/traceFailedRequests'
try {
    & $appcmd add apppool /name:$name /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
    & $appcmd add site /name:$name "/bindings:http/127.0.0.1:${port}:" /physicalPath:$root | Out-Null
    & $appcmd set app "$name/" /applicationPool:$name | Out-Null
    & $appcmd set site $name '-traceFailedRequestsLogging.enabled:true' "-traceFailedRequestsLogging.directory:$traces" '-traceFailedRequestsLogging.maxLogFiles:10000' | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'the site''s tracing could not be set' }
    & $appcmd set config $name "-section:$section" "/+[path='*']" /commit:apphost | Out-Null
    & $appcmd set config $name "-section:$section" "/+[path='*'].traceAreas.[provider='WWW Server',areas='Authentication,Security,Filter,StaticFile,CGI,Compression,Cache,RequestNotifications,Module,Rewrite',verbosity='Verbose']" /commit:apphost | Out-Null
    & $appcmd set config $name "-section:$section" "/+[path='*'].traceAreas.[provider='ASPNET',areas='Infrastructure,Module,Page,AppServices',verbosity='Verbose']" /commit:apphost | Out-Null
    & $appcmd set config $name "-section:$section" "/[path='*'].failureDefinitions.statusCodes:100-999" /commit:apphost | Out-Null
    & $appcmd start site /site.name:$name | Out-Null
    $env:FOC_IIS_TRACE = "$port;$root;$traces"
    $ErrorActionPreference = 'Continue'
    & dotnet test $project -nologo -v q --filter 'FullyQualifiedName~AttackTests.Trace'
    $exit = $LASTEXITCODE
    $ErrorActionPreference = 'Stop'
    if ($exit -ne 0) { throw "the tracing failed ($exit)" }
}
finally {
    $ErrorActionPreference = 'Continue'
    Remove-Item Env:FOC_IIS_TRACE -ErrorAction SilentlyContinue
    & $appcmd stop site /site.name:$name | Out-Null
    & $appcmd delete site $name | Out-Null
    & $appcmd delete apppool $name | Out-Null
    & $appcmd clear config $name "-section:$section" /commit:apphost 2>&1 | Out-Null
    Start-Sleep -Seconds 2
    if ($KeepTraces) { Copy-Item $traces $KeepTraces -Recurse -Force }
    foreach ($folder in @("$root\linked", "$root\datalink")) {
        if (Test-Path $folder) { [System.IO.Directory]::Delete($folder) }
    }
    foreach ($folder in @($root, "$root-outside", $traces)) {
        if (Test-Path $folder) { Remove-Item -Recurse -Force $folder -ErrorAction SilentlyContinue }
    }
    Write-Host "removed: site and application pool $name, $root, $traces"
}
