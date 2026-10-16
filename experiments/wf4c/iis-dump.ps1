# The answers of a sample on IIS as sent (headers and body), for a look at IIS's own pages: the recorded site
# (sample-parity.ps1 -Record) on its port while this runs.
#   .\experiments\wf4c\iis-dump.ps1 -Name ErrorsProbe -Paths '/App.aspx?status=400','/nothing.htm'
param(
    [Parameter(Mandatory = $true)][string]$Name,
    [Parameter(Mandatory = $true)][string[]]$Paths,
    [int]$Port = 5096
)

$ErrorActionPreference = 'Stop'
$site = Join-Path $PSScriptRoot "_samples\$Name\iis"
$base = "http://localhost:$Port"
$appcmd = Join-Path $env:SystemRoot 'System32\inetsrv\appcmd.exe'
$pool = "frameworkoncore-$($Name.ToLowerInvariant())"
& $appcmd delete site $pool 2>&1 | Out-Null
& $appcmd delete apppool $pool 2>&1 | Out-Null
& $appcmd add apppool /name:$pool /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
& $appcmd add site /name:$pool /physicalPath:$site ("/bindings:http/*:{0}:" -f $Port) | Out-Null
& $appcmd set app "$pool/" /applicationPool:$pool | Out-Null
& $appcmd set config "$pool/" /section:anonymousAuthentication /userName:"" /commit:apphost | Out-Null
& icacls $site /grant "IIS AppPool\${pool}:(OI)(CI)(M)" /T /Q | Out-Null
try {
    foreach ($i in 1..300) { if ((curl.exe -s -o NUL -w "%{http_code}" "$base/") -ne '000') { break }; Start-Sleep -Milliseconds 200 }
    foreach ($path in $Paths) {
        "===== $path"
        $bytes = [IO.Path]::GetTempFileName()
        curl.exe -s -D - -o $bytes "$base$path"
        [IO.File]::ReadAllText($bytes, [Text.Encoding]::UTF8)
        [IO.File]::Delete($bytes)
        ''
    }
}
finally {
    & $appcmd delete site $pool 2>&1 | Out-Null
    & $appcmd delete apppool $pool 2>&1 | Out-Null
}
