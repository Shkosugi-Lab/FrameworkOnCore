# Converts the corpora (corpora\work) with the FrameworkOnCore converter (src\FrameworkOnCore.Converter)
# and prints each one's outcome; the report is <out>\CONVERSION-REPORT.md.
#   .\experiments\wf4c\convert-corpora.ps1 [-Only be,wt] [-UpdateBaseline]
# Each conversion is compared with corpora\regression\conversion-baseline.json (its build, its unresolved, its stubs):
# more unresolved or stubs than there were is a regression (the code the conversion stops to compile is a feature that
# silently does not work), reported, and the script fails at the end. -UpdateBaseline writes this run's counts there
# (after the stubs added were looked at: each a .NET API's absence, StubCauses).
param([string[]]$Only = @('be', 'wt', 'mojo', 'yaf', 'dnn', 'n2', 'imis', 'nop', 'mvcmovie', 'nop390', 'contoso'), [switch]$Rebuild, [switch]$UpdateBaseline)

$repo = Resolve-Path (Join-Path $PSScriptRoot '..\..')
Set-Location $repo
$corpora = [ordered]@{
    be   = @('BlogEngine.NET-3.3.8.0', 'BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj')
    wt   = @('wingtiptoys-master', 'WingtipToys\WingtipToys\WingtipToys.csproj')
    mojo = @('mojoportal-3.1.6', 'Web\mojoPortal.Web.csproj')
    yaf  = @('YAFNET-3.2.15', 'yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj')
    dnn  = @('Dnn.Platform-9.13.10', 'DNN Platform\Website\DotNetNuke.Website.csproj')
    n2   = @('n2cms-master', 'src\WebForms\WebFormsTemplates\N2.Templates.csproj')
    imis = @('web_app_vb-main', 'IMIS\IMIS.vbproj')
    nop  = @('nopCommerce-release-1.90', 'NopCommerceStore\NopCommerceStore.csproj')
    # ASP.NET MVC 5.
    mvcmovie = @('MvcMovie', 'MvcMovie\MvcMovie.csproj')
    nop390 = @('nopCommerce-release-3.90', 'src\Presentation\Nop.Web\Nop.Web.csproj')
    contoso = @('ContosoUniversity', 'ContosoUniversity\ContosoUniversity.csproj')
}
# Their original build, by the converter (--build-original: a Cake build, or else the solution): the
# folder it deploys the site to, relative to the repository, the configuration (--configuration: the solution is built
# in it and the projects are read in it) and the repository's setup steps after it. openIMIS: DemoRelease, the
# configuration whose web.config transform the repository has (Web.Release.config is not in it).
$originals = @{
    dnn  = @{ Site = 'Website' }
    mojo = @{ Site = 'Web' }
    n2   = @{ Site = 'src\WebForms\WebFormsTemplates'; Steps = @('build\n2.proj;Templates-PrepareDependencies') }
    imis = @{ Site = 'IMIS'; Configuration = 'DemoRelease' }
    nop  = @{ Site = 'NopCommerceStore' }
    # The solution's build puts the plugins in the web project's Plugins folder: the site is that folder.
    nop390 = @{ Site = 'src\Presentation\Nop.Web' }
}
dotnet build src\FrameworkOnCore.Converter\FrameworkOnCore.Converter.csproj -v q -nologo | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'converter build failed' }
$converter = 'src\FrameworkOnCore.Converter\bin\Debug\net10.0\FrameworkOnCore.Converter.dll'
$baselinePath = Join-Path $repo 'corpora\regression\conversion-baseline.json'
$baseline = [ordered]@{}
if (Test-Path $baselinePath) { (Get-Content $baselinePath -Raw | ConvertFrom-Json).PSObject.Properties | ForEach-Object { $baseline[$_.Name] = $_.Value } }
$regressions = @()
# A section's count in the report ("## <title>(<n> 件)"), by the start of its title.
function SectionCount([string]$report, [string]$title) {
    $line = Select-String -Path $report -Pattern ('^## ' + [regex]::Escape($title)) | Select-Object -First 1
    if ($line -and $line.Line -match '\((\d+) 件\)$') { [int]$Matches[1] } else { 0 }
}
$logs = Join-Path $PSScriptRoot '_linux'
New-Item -ItemType Directory $logs -Force | Out-Null
foreach ($name in $Only) {
    $root, $project = $corpora[$name]
    $log = Join-Path $logs "foc-$name.log"
    # The deployed site of the original build (in <out>.original-<time>, the converter's newest; <out>.original before
    # it built in a new folder each time): the one built before if there is one (the build takes minutes; -Rebuild runs
    # it again).
    $siteArguments = @()
    if ($originals.ContainsKey($name)) {
        $built = @(Get-ChildItem $PSScriptRoot -Directory | Where-Object { $_.Name -match "^$([regex]::Escape($name))\.original(-\d{8}-\d{6}(-\d+)?)?$" } |
            Sort-Object { if ($_.Name -eq "$name.original") { '' } else { $_.Name } } -Descending)
        $deployed = Join-Path $(if ($built) { $built[0].FullName } else { Join-Path $PSScriptRoot "$name.original" }) $originals[$name].Site
        $siteArguments = @(if ((Test-Path (Join-Path $deployed 'bin')) -and -not $Rebuild) { '--site', $deployed }
                         else { '--build-original'; $originals[$name].Steps | Where-Object { $_ } | ForEach-Object { '--original-step', $_ } })
    }
    if ($originals[$name].Configuration) { $siteArguments += '--configuration', $originals[$name].Configuration }
    dotnet $converter "corpora\work\$root\$project" --out "experiments\wf4c\$name" --root "corpora\work\$root" `
        --culture-profile experiments\wf4c\_culture\culture-profile.json @siteArguments *> $log
    $code = $LASTEXITCODE
    # Deployment settings of this machine (not the application's): mojoPortal's database, the local
    # SQL Server Express (database mojo_w2l, created by the setup page on first run).
    if ($name -eq 'mojo' -and (Test-Path "experiments\wf4c\mojo\site")) {
        $sample = Get-Content "corpora\work\$root\Web\user.config.sample" -Raw
        $config = ($sample -replace '<add key="MSSQLConnectionString" value="[^"]*"', '<add key="MSSQLConnectionString" value="Data Source=.\SQLEXPRESS;Initial Catalog=mojo_w2l;Integrated Security=True;TrustServerCertificate=True"') `
            -replace '</appSettings>', "  <add key=`"DisableSetup`" value=`"false`" />`r`n</appSettings>"
        [IO.File]::WriteAllText((Join-Path $PSScriptRoot 'mojo\site\user.config'), $config, (New-Object Text.UTF8Encoding $false))
    }
    # DNN's: SQL Server Express's database dnn_w2l (the install wizard creates the schema). The
    # release web.config names an attached file (|DataDirectory|Database.mdf, a user instance), which
    # .NET's SqlClient does not open.
    if ($name -eq 'dnn' -and (Test-Path "experiments\wf4c\dnn\site\web.config")) {
        sqlcmd -S .\SQLEXPRESS -E -C -b -Q "IF DB_ID('dnn_w2l') IS NULL CREATE DATABASE dnn_w2l" | Out-Null
        $webConfig = Join-Path $PSScriptRoot 'dnn\site\web.config'
        $config = [IO.File]::ReadAllText($webConfig) -replace '(<add\s+name="SiteSqlServer"\s+connectionString=")[^"]*"', '$1Data Source=.\SQLEXPRESS;Initial Catalog=dnn_w2l;Integrated Security=True"'
        [IO.File]::WriteAllText($webConfig, $config, (New-Object Text.UTF8Encoding $false))
    }
    # openIMIS's: SQL Server Express's database imis_w2l, made from the database repository's scripts (imisdb in
    # corpora\fetch.ps1) as its concatenate_files.sh puts them together for fullDemoDatabase.sql: the schema, the
    # stored procedures, the demo data. The site's web.config has the connection string's placeholders.
    if ($name -eq 'imis' -and (Test-Path "experiments\wf4c\imis\site\web.config")) {
        $exists = sqlcmd -S .\SQLEXPRESS -E -C -h -1 -W -Q "SET NOCOUNT ON; SELECT CASE WHEN DB_ID('imis_w2l') IS NULL THEN 0 ELSE 1 END"
        if ("$exists".Trim() -ne '1') {
            sqlcmd -S .\SQLEXPRESS -E -C -b -Q "CREATE DATABASE imis_w2l" | Out-Null
            $sql = 'corpora\work\database_ms_sqlserver-24.10\sql'
            $scripts = @(Get-ChildItem "$sql\base\*.sql") + @(Get-ChildItem "$sql\stored_procedures\*.sql") + @(Get-ChildItem "$sql\demo\*.sql")
            foreach ($script in $scripts) {
                sqlcmd -S .\SQLEXPRESS -E -C -d imis_w2l -f 65001 -i $script.FullName *> "$logs\imis-db-$($script.BaseName).log"
                if ($LASTEXITCODE -ne 0) { Write-Warning "imis database: $($script.Name) failed (see $logs\imis-db-$($script.BaseName).log)" }
            }
        }
        $webConfig = Join-Path $PSScriptRoot 'imis\site\web.config'
        $config = [IO.File]::ReadAllText($webConfig) -replace '(<add\s+name="IMISConnectionString"\s+connectionString=")[^"]*"', '$1Data Source=.\SQLEXPRESS;Initial Catalog=imis_w2l;Integrated Security=True;TrustServerCertificate=True"'
        [IO.File]::WriteAllText($webConfig, $config, (New-Object Text.UTF8Encoding $false))
    }
    $reportPath = "experiments\wf4c\$name\CONVERSION-REPORT.md"
    $counts = if (Test-Path $reportPath) { (Select-String -Path $reportPath -Pattern '^## .*件' | ForEach-Object { $_.Line -replace '^## ', '' }) -join ' / ' } else { '' }
    $rounds = (Select-String -Path $log -Pattern '^build \d+:').Count
    "=== $name $(if ($code -eq 0) { 'BUILD OK' } else { "FAILED ($code)" }) (rounds: $rounds) $counts"
    if ($code -ne 0) { Select-String -Path $log -Pattern '\[Error\]' | Select-Object -First 8 | ForEach-Object { '  ' + $_.Line.Trim().Substring(0, [Math]::Min(220, $_.Line.Trim().Length)) } }
    if (Test-Path $reportPath) {
        $now = [ordered]@{ build = [int]($code -eq 0); unresolved = SectionCount $reportPath '未解決'; stubs = SectionCount $reportPath 'コンパイルのために変えたソース' }
        $before = $baseline[$name]
        if ($before) {
            $worse = @()
            if ($now.build -lt $before.build) { $worse += 'the build fails' }
            if ($now.unresolved -gt $before.unresolved) { $worse += "unresolved $($before.unresolved) -> $($now.unresolved)" }
            if ($now.stubs -gt $before.stubs) { $worse += "stubs $($before.stubs) -> $($now.stubs)" }
            if ($worse) { "  REGRESSION: $($worse -join ', ') (corpora\regression\conversion-baseline.json)"; $regressions += $name }
            elseif ($now.unresolved -lt $before.unresolved -or $now.stubs -lt $before.stubs) { "  better than the baseline: unresolved $($before.unresolved) -> $($now.unresolved), stubs $($before.stubs) -> $($now.stubs) (-UpdateBaseline records it)" }
        }
        else { "  no baseline yet (-UpdateBaseline records it)" }
        if ($UpdateBaseline) { $baseline[$name] = $now }
    }
}
if ($UpdateBaseline) {
    $sorted = [ordered]@{}
    foreach ($key in ($baseline.Keys | Sort-Object)) { $sorted[$key] = $baseline[$key] }
    [IO.File]::WriteAllText($baselinePath, ($sorted | ConvertTo-Json -Depth 3) + "`n", (New-Object Text.UTF8Encoding $false))
    "baseline written: $baselinePath"
}
if ($regressions) { throw "conversion regressions: $($regressions -join ', ') (more unresolved or stubbed code than corpora\regression\conversion-baseline.json)" }
