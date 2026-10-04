# Each corpus through Studio, as a user takes it (its API): the analysis, the conversion (with the original's build where
# its repository builds the site), the original run (IIS or IIS Express), the native run and the container run. Each run
# counts as working when the site answers with less than 500 at first. And the pages' answers compared: on the original,
# every page reachable from "/" by its links (ParityTest record-links: each URL's status and redirect target), asked
# again on the native and the container run (ParityTest verify). Not that each page works: that each answers as the
# original's does.
#
# The databases: SQL Server Express (.\SQLEXPRESS on TCP 1433, SQL authentication: the login in _verify\sql-login.txt,
# "User ID=...;Password=..."), the same databases for the three runs (the container's through host.docker.internal).
# Studio runs from this checkout's build, with its data in _verify\studio; the results in _verify\results\<name>.json
# (a corpus done is not run again unless -Force), the goldens and reports beside them.
#
#   .\experiments\wf4c\studio-verify.ps1 -Only be,wt [-Force] [-Max 1000]
param(
    [string[]]$Only = @('be', 'wt', 'mvcmovie', 'mojo', 'yaf', 'dnn', 'n2', 'imis', 'nop', 'nop390'),
    [switch]$Force,
    # The crawl's limit (URLs); its depth is not limited.
    [int]$Max = 1000,
    [int]$Port = 5310,
    [string[]]$Steps = @('original', 'native', 'container')
)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$verify = Join-Path $PSScriptRoot '_verify'
$results = Join-Path $verify 'results'
New-Item -ItemType Directory -Force $results | Out-Null
$loginFile = Join-Path $verify 'sql-login.txt'
if (-not (Test-Path $loginFile)) { throw "$loginFile がありません(User ID=...;Password=...)" }
$login = (Get-Content $loginFile -Raw).Trim()

# Each corpus: its project and repository (corpora\work), its configuration, the connection strings its runs get (the
# database's name; {server} is the server as each run reaches it), and app settings.
$corpora = [ordered]@{
    be       = @{ Root = 'BlogEngine.NET-3.3.8.0'; Project = 'BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj' }
    wt       = @{ Root = 'wingtiptoys-master'; Project = 'WingtipToys\WingtipToys\WingtipToys.csproj'
                  Connections = @{ DefaultConnection = 'WingtipToys;MultipleActiveResultSets=True'; WingtipToys = 'WingtipToys;MultipleActiveResultSets=True' } }
    mvcmovie = @{ Root = 'MvcMovie'; Project = 'MvcMovie\MvcMovie.csproj'
                  Connections = @{ MovieDBContext = 'MvcMovie'; DefaultConnection = 'MvcMovieIdentity' } }
    mojo     = @{ Root = 'mojoportal-3.1.6'; Project = 'Web\mojoPortal.Web.csproj'
                  Connections = @{ MSSQLConnectionString = 'mojo_w2l' }; Settings = @{ MSSQLConnectionString = 'mojo_w2l' } }
    yaf      = @{ Root = 'YAFNET-3.2.15'; Project = 'yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj'
                  Connections = @{ yafnet = 'yafnet' } }
    dnn      = @{ Root = 'Dnn.Platform-9.13.10'; Project = 'DNN Platform\Website\DotNetNuke.Website.csproj'
                  Connections = @{ SiteSqlServer = 'dnn_w2l' } }
    n2       = @{ Root = 'n2cms-master'; Project = 'src\WebForms\WebFormsTemplates\N2.Templates.csproj' }
    imis     = @{ Root = 'web_app_vb-main'; Project = 'IMIS\IMIS.vbproj'; Configuration = 'DemoRelease'
                  Connections = @{ IMISConnectionString = 'imis_w2l' } }
    nop      = @{ Root = 'nopCommerce-release-1.90'; Project = 'NopCommerceStore\NopCommerceStore.csproj' }
    nop390   = @{ Root = 'nopCommerce-release-3.90'; Project = 'src\Presentation\Nop.Web\Nop.Web.csproj' }
}

function Environment-Text($corpus, [string]$server) {
    $lines = @()
    if ($corpus.Connections) { foreach ($entry in $corpus.Connections.GetEnumerator()) {
        $database, $more = $entry.Value -split ';', 2
        $lines += "SQLCONNSTR_$($entry.Key)=Data Source=$server;Initial Catalog=$database;$login;TrustServerCertificate=True$(if ($more) { ";$more" })"
    } }
    if ($corpus.Settings) { foreach ($entry in $corpus.Settings.GetEnumerator()) {
        $lines += "APPSETTING_$($entry.Key)=Data Source=$server;Initial Catalog=$($entry.Value);$login;TrustServerCertificate=True"
    } }
    $lines -join "`n"
}

# --- Studio, from this checkout
$studioDll = Join-Path $repo 'src\FrameworkOnCore.Studio\bin\Debug\net10.0\FrameworkOnCore.Studio.dll'
dotnet build (Join-Path $repo 'src\FrameworkOnCore.Studio') --nologo -v q | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'Studio のビルドに失敗しました' }
$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'
dotnet build (Join-Path $repo 'tools\FrameworkOnCore.ParityTest') -o (Split-Path $verifier) --nologo -v q | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'ParityTest のビルドに失敗しました' }

$api = "http://127.0.0.1:$Port/api"
$studioLog = Join-Path $verify 'studio.log'
$studio = Start-Process dotnet -ArgumentList "`"$studioDll`"", '--port', $Port, '--data', "`"$(Join-Path $verify 'studio')`"", '--runtime', "`"$PSScriptRoot`"" `
    -PassThru -WindowStyle Hidden -RedirectStandardOutput $studioLog -RedirectStandardError "$studioLog.err"
function Api([string]$method, [string]$path, $body) {
    $arguments = @{ Method = $method; Uri = "$api$path"; TimeoutSec = 120 }
    if ($null -ne $body) { $arguments.Body = [Text.Encoding]::UTF8.GetBytes(($body | ConvertTo-Json -Depth 5)); $arguments.ContentType = 'application/json; charset=utf-8' }
    Invoke-RestMethod @arguments
}
# Waits until what $get returns has a state not in $busy; the last answer.
function Wait-State([scriptblock]$get, [string[]]$busy, [int]$minutes, [string]$what) {
    $until = (Get-Date).AddMinutes($minutes)
    while ($true) {
        $answer = & $get
        if ($answer.State -notin $busy) { return $answer }
        if ((Get-Date) -gt $until) { throw "$what が $minutes 分で終わりません(状態 $($answer.State))" }
        Start-Sleep 5
    }
}

try {
    foreach ($i in 1..60) { try { Api GET '/analyses' | Out-Null; break } catch { Start-Sleep 1 } }
    $status = if (Test-Path "$studioLog.err") { Get-Content "$studioLog.err" -Raw }
    if ($studio.HasExited) { throw "Studio が起動しません: $status" }

    foreach ($name in $Only) {
        $corpus = $corpora[$name]
        $resultFile = Join-Path $results "$name.json"
        if ((Test-Path $resultFile) -and -not $Force) { "=== ${name}: 済み($resultFile)"; continue }
        $result = [ordered]@{ name = $name; started = (Get-Date).ToString('s') }
        $save = { $result | ConvertTo-Json -Depth 6 | Set-Content $resultFile -Encoding utf8 }
        "=== $name ($(Get-Date -Format HH:mm:ss))"
        try {
            # The analysis (one per corpus: an earlier one is deleted).
            $root = Join-Path $repo "corpora\work\$($corpus.Root)"
            foreach ($old in (Api GET '/analyses')) { if ($old.Name -eq "verify-$name") { Api DELETE "/analyses/$($old.Id)" | Out-Null } }
            $entry = Api POST '/analyses' @{ project = (Join-Path $root $corpus.Project); root = $root; configuration = $corpus.Configuration; name = "verify-$name" }
            $id = $entry.Id
            $entry = Wait-State { (Api GET "/analyses/$id").entry } @('queued', 'running') 30 '解析'
            $result.analysis = $entry.State
            "  解析: $($entry.State)$(if ($entry.Error) { " - $($entry.Error)" })"
            if ($entry.State -ne 'done') { throw "解析: $($entry.Error)" }

            # The conversion, with the original's build where Studio advises it (the repository builds the site).
            $advice = Api GET "/analyses/$id/original-build-advice"
            $buildOriginal = [bool]$advice.needed
            Api POST "/analyses/$id/conversion" @{ buildOriginal = $buildOriginal } | Out-Null
            $conversion = Wait-State { (Api GET "/analyses/$id/conversion").conversion } @('queued', 'running') 120 '変換'
            $result.conversion = [ordered]@{ state = $conversion.State; buildOriginal = $buildOriginal; error = $conversion.Error }
            "  変換(元のアプリのビルド: $buildOriginal): $($conversion.State)$(if ($conversion.Error) { " - $($conversion.Error)" })"
            & $save
            if ($conversion.State -ne 'done') { throw "変換: $($conversion.Error)" }

            $golden = Join-Path $results "$name.golden.json"
            Remove-Item $golden -ErrorAction SilentlyContinue
            $scenario = Join-Path $results "$name.scenario.json"
            @{ steps = @(@{ action = 'goto'; path = '/' }); crawl = @{ max = $Max; depth = 1000; exclude = @('log ?(off|out)|sign ?out|logoff', 'delete|remove', '/setup/', '/install') } } |
                ConvertTo-Json -Depth 5 | Set-Content $scenario -Encoding utf8
            foreach ($run in $Steps) {
                # Docker's engine for the container run: Docker Desktop started if it is not (Studio's button).
                if ($run -eq 'container' -and -not (Api GET '/docker').available) {
                    Api POST '/docker/start' | Out-Null
                    $until = (Get-Date).AddMinutes(5)
                    while (-not (Api GET '/docker').available -and (Get-Date) -lt $until) { Start-Sleep 5 }
                }
                $server = if ($run -eq 'container') { 'host.docker.internal,1433' } else { '127.0.0.1,1433' }
                $path = "/analyses/$id/$run"
                $started = Api POST $path @{ environment = (Environment-Text $corpus $server); rebuild = $false }
                $entry = Wait-State { (Api GET $path).$run } @('building', 'starting') 120 $run
                $outcome = [ordered]@{ state = $entry.State; firstStatus = $entry.FirstStatus; url = $entry.Url; error = $entry.Error }
                $outcome.ok = $entry.State -eq 'running' -and $entry.FirstStatus -lt 500
                "  ${run}: $($entry.State) 最初の応答 $($entry.FirstStatus)$(if ($entry.Error) { " - $($entry.Error)" })"
                if ($entry.State -eq 'running' -and $entry.FirstStatus -ge 500) {
                    # The error page, to see why.
                    try { Invoke-WebRequest $entry.Url -UseBasicParsing -TimeoutSec 120 | Out-Null }
                    catch { if ($_.Exception.Response) { $reader = New-Object IO.StreamReader($_.Exception.Response.GetResponseStream()); Set-Content (Join-Path $results "$name.$run.error.html") -Value $reader.ReadToEnd() -Encoding utf8 } }
                }
                if ($entry.State -eq 'running' -and ($run -eq 'original' -or (Test-Path $golden))) {
                    $report = Join-Path $results "$name.$run.md"
                    $output = if ($run -eq 'original') { & dotnet $verifier record-links --url $entry.Url --scenario $scenario --golden $golden 2>&1 }
                              else { & dotnet $verifier verify --url $entry.Url --scenario $scenario --golden $golden --report $report 2>&1 }
                    $outcome.parity = ($output | Where-Object { "$_" -match '^(RESULT|リンク|  (OK|NG)   リンク)' } | ForEach-Object { "$_".Trim() }) -join ' / '
                    "    $($outcome.parity)"
                }
                else {
                    $log = (Api GET $path).log
                    Set-Content (Join-Path $results "$name.$run.log") -Value $log -Encoding utf8
                }
                Api DELETE $path | Out-Null
                $result[$run] = $outcome
                & $save
            }
        }
        catch {
            $result.error = $_.Exception.Message
            "  失敗: $($_.Exception.Message)"
        }
        $result.finished = (Get-Date).ToString('s')
        & $save
    }
}
finally {
    if (-not $studio.HasExited) { Stop-Process -Id $studio.Id -Force }
}
