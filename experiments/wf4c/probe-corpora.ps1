# Starts each converted corpus (after convert-corpora.ps1) on Windows and requests "/" (following
# redirects); prints the status, the final URL and the page title or the error.
#   .\experiments\wf4c\probe-corpora.ps1 [-Only mojo,yaf]
param([string[]]$Only = @('be', 'wt', 'mojo', 'yaf', 'dnn', 'n2'), [int]$Port = 5096)

$webProjects = @{
    be   = 'be\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj'
    wt   = 'wt\WingtipToys\WingtipToys\WingtipToys.csproj'
    mojo = 'mojo\Web\mojoPortal.Web.csproj'
    yaf  = 'yaf\yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj'
    dnn  = 'dnn\DNN Platform\Website\DotNetNuke.Website.csproj'
    n2   = 'n2\src\WebForms\WebFormsTemplates\N2.Templates.csproj'
}
foreach ($name in $Only) {
    $project = Join-Path $PSScriptRoot $webProjects[$name]
    $directory = Split-Path $project -Parent
    [xml]$x = Get-Content $project -Raw
    $assembly = ($x.Project.PropertyGroup | ForEach-Object { $_.AssemblyName } | Where-Object { $_ } | Select-Object -First 1)
    $dll = Join-Path $directory "bin\$assembly.dll"
    if (-not (Test-Path $dll)) { "=== ${name}: $dll not built"; continue }
    $log = Join-Path $directory 'probe.log'
    $process = Start-Process -FilePath dotnet -ArgumentList "`"$dll`"", '--urls', "http://localhost:$Port" -WorkingDirectory $directory -PassThru -WindowStyle Hidden -RedirectStandardOutput $log -RedirectStandardError "$log.err"
    try {
        foreach ($i in 1..60) { if ((Test-Path $log) -and (Select-String -Path $log -Pattern 'Now listening' -Quiet)) { break }; if ($process.HasExited) { break }; Start-Sleep 1 }
        if ($process.HasExited) { "=== ${name}: exited ($($process.ExitCode))"; Get-Content "$log.err", $log -ErrorAction SilentlyContinue | Select-Object -Last 8; continue }
        $out = curl.exe -s -L --max-redirs 5 -m 180 -o "$directory\probe.html" -w "%{http_code} %{url_effective} %{time_total}s" "http://localhost:$Port/"
        $html = if (Test-Path "$directory\probe.html") { Get-Content "$directory\probe.html" -Raw -Encoding UTF8 } else { '' }
        $title = if ($html -match '<title>\s*([^<]*?)\s*</title>') { $Matches[1] } else { '' }
        $detail = if ($html -match '<b>\s*Exception Details:\s*</b>\s*([^<]{0,200})') { $Matches[1] } elseif ($html -match '(?s)<h2>\s*<i>(.{0,200}?)</i>') { $Matches[1] } else { '' }
        "=== ${name}: $out title='$title' $detail"
    }
    finally { Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue }
}
