# Converts the corpora (corpora\work) with the FrameworkOnCore converter (src\FrameworkOnCore.Converter)
# and prints each one's outcome; the report is <out>\CONVERSION-REPORT.md.
#   .\experiments\wf4c\convert-corpora.ps1 [-Only be,wt]
param([string[]]$Only = @('be', 'wt', 'mojo', 'yaf', 'dnn', 'n2'))

$repo = Resolve-Path (Join-Path $PSScriptRoot '..\..')
Set-Location $repo
$corpora = [ordered]@{
    be   = @('BlogEngine.NET-3.3.8.0', 'BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj')
    wt   = @('wingtiptoys-master', 'WingtipToys\WingtipToys\WingtipToys.csproj')
    mojo = @('mojoportal-3.1.6', 'Web\mojoPortal.Web.csproj')
    yaf  = @('YAFNET-3.2.15', 'yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj')
    dnn  = @('Dnn.Platform-9.13.10', 'DNN Platform\Website\DotNetNuke.Website.csproj')
    n2   = @('n2cms-master', 'src\WebForms\WebFormsTemplates\N2.Templates.csproj')
}
dotnet build src\FrameworkOnCore.Converter\FrameworkOnCore.Converter.csproj -v q -nologo | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'converter build failed' }
$converter = 'src\FrameworkOnCore.Converter\bin\Debug\net10.0\FrameworkOnCore.Converter.dll'
$logs = Join-Path $PSScriptRoot '_linux'
New-Item -ItemType Directory $logs -Force | Out-Null
foreach ($name in $Only) {
    $root, $project = $corpora[$name]
    $log = Join-Path $logs "foc-$name.log"
    dotnet $converter "corpora\work\$root\$project" --out "experiments\wf4c\$name" --root "corpora\work\$root" `
        --culture-profile experiments\wf4c\_culture\culture-profile.json *> $log
    $code = $LASTEXITCODE
    $reportPath = "experiments\wf4c\$name\CONVERSION-REPORT.md"
    $counts = if (Test-Path $reportPath) { (Select-String -Path $reportPath -Pattern '^## .*件' | ForEach-Object { $_.Line -replace '^## ', '' }) -join ' / ' } else { '' }
    $rounds = (Select-String -Path $log -Pattern '^build \d+:').Count
    "=== $name $(if ($code -eq 0) { 'BUILD OK' } else { "FAILED ($code)" }) (rounds: $rounds) $counts"
    if ($code -ne 0) { Select-String -Path $log -Pattern '\[Error\]' | Select-Object -First 8 | ForEach-Object { '  ' + $_.Line.Trim().Substring(0, [Math]::Min(220, $_.Line.Trim().Length)) } }
}
