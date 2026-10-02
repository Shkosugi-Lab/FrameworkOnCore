# Converts the corpora (corpora\work) with the FrameworkOnCore converter (src\FrameworkOnCore.Converter)
# and prints each one's outcome; the report is <out>\CONVERSION-REPORT.md.
#   .\experiments\wf4c\convert-corpora.ps1 [-Only be,wt]
param([string[]]$Only = @('be', 'wt', 'mojo', 'yaf', 'dnn', 'n2', 'imis', 'nop', 'mvcmovie', 'nop390'), [switch]$Rebuild)

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
        $config = [IO.File]::ReadAllText($webConfig) -replace '(<add name="SiteSqlServer" connectionString=")[^"]*"', '$1Data Source=.\SQLEXPRESS;Initial Catalog=dnn_w2l;Integrated Security=True"'
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
        $config = [IO.File]::ReadAllText($webConfig) -replace '(<add name="IMISConnectionString" connectionString=")[^"]*"', '$1Data Source=.\SQLEXPRESS;Initial Catalog=imis_w2l;Integrated Security=True;TrustServerCertificate=True"'
        [IO.File]::WriteAllText($webConfig, $config, (New-Object Text.UTF8Encoding $false))
    }
    $reportPath = "experiments\wf4c\$name\CONVERSION-REPORT.md"
    $counts = if (Test-Path $reportPath) { (Select-String -Path $reportPath -Pattern '^## .*件' | ForEach-Object { $_.Line -replace '^## ', '' }) -join ' / ' } else { '' }
    $rounds = (Select-String -Path $log -Pattern '^build \d+:').Count
    "=== $name $(if ($code -eq 0) { 'BUILD OK' } else { "FAILED ($code)" }) (rounds: $rounds) $counts"
    if ($code -ne 0) { Select-String -Path $log -Pattern '\[Error\]' | Select-Object -First 8 | ForEach-Object { '  ' + $_.Line.Trim().Substring(0, [Math]::Min(220, $_.Line.Trim().Length)) } }
}
