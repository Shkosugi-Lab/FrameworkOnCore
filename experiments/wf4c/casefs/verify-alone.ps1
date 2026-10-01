# Whether libfoccase.so alone finds the application's names without regard to case: be and wt on Linux (run-linux.ps1,
# compared with their golden data), with FrameworkOnCore's and the compatibility assembly's own case matching turned off
# (WEBFORMSFORCORE_PATH_CASING=0), with and without the library, and as they run by default. wt's protected folders
# are requested under other cases too (their web.config: authorization).
#   .\experiments\wf4c\casefs\verify-alone.ps1 [-Only be,wt]
param([string[]]$Only = @('be', 'wt'))
$wf4c = Split-Path $PSScriptRoot
$configurations = [ordered]@{
    'default (FrameworkOnCore's System.Web matches case, no library)' = @{ Case = $false; Environment = @{} }
    'no case matching at all'                     = @{ Case = $false; Environment = @{ WEBFORMSFORCORE_PATH_CASING = '0' } }
    'the library alone'                           = @{ Case = $true; Environment = @{ WEBFORMSFORCORE_PATH_CASING = '0' } }
}
$apps = @{
    be = @{ App = 'be\BlogEngine\BlogEngine.NET'; Scenario = 'corpora\regression\be.scenario.json'; Golden = 'corpora\parity\be.golden-webforms.json'; Sql = $false; Container = 'w2l-blogengine-net'; Paths = @('/', '/SCRIPTS/syntaxhighlighter/scripts/SHCORE.js') }
    wt = @{ App = 'wt\WingtipToys\WingtipToys'; Scenario = 'corpora\regression\wt.scenario.json'; Golden = 'corpora\parity\wt.golden-webforms.json'; Sql = $true; Container = 'w2l-wingtiptoys'; Paths = @('/Admin/AdminPage', '/admin/adminpage', '/checkout/checkoutreview', '/content/SITE.css') }
}
foreach ($name in $Only) {
    $a = $apps[$name]
    foreach ($label in $configurations.Keys) {
        $c = $configurations[$label]
        "===== $name, $label"
        $arguments = @{ App = $a.App; Scenario = $a.Scenario; Golden = $a.Golden; Environment = $c.Environment; Keep = $true }
        if ($a.Sql) { $arguments.SqlServer = $true } else { docker stop w2l-sql 2>$null | Out-Null }
        if ($c.Case) { $arguments.CaseInsensitive = $true }
        & (Join-Path $wf4c 'run-linux.ps1') @arguments 2>&1 | ForEach-Object { "$_" } | Where-Object { $_ -match '^\s+(OK|NG)\s|RESULT|did not start' }
        foreach ($path in $a.Paths) {
            curl.exe -s -o NUL -w "  $path -> %{http_code} %{redirect_url}`n" -m 300 "http://localhost:5095$path"
        }
        "  names found under another case (libfoccase.so's log): $((docker logs $a.Container 2>&1 | Select-String '^foccase:').Count)"
        docker rm -f $a.Container 2>$null | Out-Null
    }
}
