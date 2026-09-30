# Analyzes the corpora (corpora\work) with the converter's analyze: the .NET Framework APIs each uses, their counts, what
# .NET 10 has of them. Writes _analysis\<name>\api-analysis.json and API-ANALYSIS.md; prints each one's summary.
#   .\experiments\wf4c\analyze-corpora.ps1 [-Only be,wt]
param([string[]]$Only = @('be', 'wt', 'mojo', 'yaf', 'dnn', 'n2', 'imis', 'nop', 'mvcmovie', 'nop390'))

$repo = Resolve-Path (Join-Path $PSScriptRoot '..\..')
Set-Location $repo
# The corpora as convert-corpora.ps1 has them: repository, web project, configuration.
$corpora = [ordered]@{
    be   = @('BlogEngine.NET-3.3.8.0', 'BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj', 'Debug')
    wt   = @('wingtiptoys-master', 'WingtipToys\WingtipToys\WingtipToys.csproj', 'Debug')
    mojo = @('mojoportal-3.1.6', 'Web\mojoPortal.Web.csproj', 'Debug')
    yaf  = @('YAFNET-3.2.15', 'yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj', 'Debug')
    dnn  = @('Dnn.Platform-9.13.10', 'DNN Platform\Website\DotNetNuke.Website.csproj', 'Debug')
    n2   = @('n2cms-master', 'src\WebForms\WebFormsTemplates\N2.Templates.csproj', 'Debug')
    imis = @('web_app_vb-main', 'IMIS\IMIS.vbproj', 'DemoRelease')
    nop  = @('nopCommerce-release-1.90', 'NopCommerceStore\NopCommerceStore.csproj', 'Debug')
    mvcmovie = @('MvcMovie', 'MvcMovie\MvcMovie.csproj', 'Debug')
    nop390 = @('nopCommerce-release-3.90', 'src\Presentation\Nop.Web\Nop.Web.csproj', 'Debug')
}
dotnet build src\FrameworkOnCore.Converter\FrameworkOnCore.Converter.csproj -v q -nologo | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'converter build failed' }
$converter = 'src\FrameworkOnCore.Converter\bin\Debug\net10.0\FrameworkOnCore.Converter.dll'
foreach ($name in $Only) {
    $root, $project, $configuration = $corpora[$name]
    $out = Join-Path $PSScriptRoot "_analysis\$name"
    $watch = [Diagnostics.Stopwatch]::StartNew()
    $output = dotnet $converter analyze "corpora\work\$root\$project" --out $out --root "corpora\work\$root" --configuration $configuration 2>&1
    $output | Set-Content (Join-Path $PSScriptRoot "_analysis\$name.log")
    "=== $name ($([int]$watch.Elapsed.TotalSeconds)s, exit $LASTEXITCODE): $($output | Select-Object -Last 1)"
}
