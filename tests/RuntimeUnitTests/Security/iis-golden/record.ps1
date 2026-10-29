# Records IIS's answers to the attack tests' cases (tests/RuntimeUnitTests/Security/Attack: AttackCases, the probe site
# ProbeSite) into iis.json beside this script: the oracle AttackTests compares the port with.
#
# Re-record (Windows with IIS 10, ASP.NET 4.8 and URL Rewrite 2.1; an elevated PowerShell, from the repository's root):
#   powershell -ExecutionPolicy Bypass -File tests/RuntimeUnitTests/Security/iis-golden/record.ps1
# It makes a throwaway site (its own application pool, integrated mode, .NET 4.0) on a free port of 127.0.0.1 over a folder
# in %TEMP% (IIS's users given its reading), runs the test AttackTests.Record against it (FOC_IIS_RECORD=port;folder: it
# writes the probe site into the folder and sends every case), then removes the site, the pool and the folder. The
# Default Web Site and the other sites are not touched. Review the diff of iis.json before committing it.
param(
    [string]$Filter = "FullyQualifiedName~AttackTests.Record"
)
$ErrorActionPreference = 'Stop'
$appcmd = Join-Path $env:windir 'system32\inetsrv\appcmd.exe'
if (-not (Test-Path $appcmd)) { throw 'IIS is not installed (no appcmd.exe)' }
$repository = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
$project = Join-Path $repository 'tests\RuntimeUnitTests\RuntimeUnitTests.csproj'

# A free port of 127.0.0.1.
$listener = [System.Net.Sockets.TcpListener]::new([System.Net.IPAddress]::Loopback, 0)
$listener.Start(); $port = $listener.LocalEndpoint.Port; $listener.Stop()

$name = 'foc-iis-probe'
$root = Join-Path $env:TEMP ($name + '-' + [guid]::NewGuid().ToString('N').Substring(0, 8))
New-Item -ItemType Directory -Path $root | Out-Null
& icacls $root /grant 'IIS_IUSRS:(OI)(CI)RX' 'IUSR:(OI)(CI)RX' | Out-Null
try {
    & $appcmd add apppool /name:$name /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
    & $appcmd add site /name:$name "/bindings:http/127.0.0.1:${port}:" /physicalPath:$root | Out-Null
    & $appcmd set app "$name/" /applicationPool:$name | Out-Null
    & $appcmd start site /site.name:$name | Out-Null
    $env:FOC_IIS_RECORD = "$port;$root"
    # (dotnet's build warnings on its error stream are not this script's errors)
    $ErrorActionPreference = 'Continue'
    & dotnet test $project -nologo -v q --filter $Filter
    $exit = $LASTEXITCODE
    $ErrorActionPreference = 'Stop'
    if ($exit -ne 0) { throw "the recording failed ($exit)" }
}
finally {
    $ErrorActionPreference = 'Continue'
    Remove-Item Env:FOC_IIS_RECORD -ErrorAction SilentlyContinue
    & $appcmd stop site /site.name:$name | Out-Null
    & $appcmd delete site $name | Out-Null
    & $appcmd delete apppool $name | Out-Null
    Start-Sleep -Seconds 2
    foreach ($folder in @("$root\linked", "$root\datalink")) {
        if (Test-Path $folder) { [System.IO.Directory]::Delete($folder) }
    }
    foreach ($folder in @($root, "$root-outside")) {
        if (Test-Path $folder) { Remove-Item -Recurse -Force $folder -ErrorAction SilentlyContinue }
    }
    Write-Host "removed: site and application pool $name, $root"
}
