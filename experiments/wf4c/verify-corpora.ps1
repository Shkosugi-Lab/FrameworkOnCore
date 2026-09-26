# Converts the corpora (corpora\work) with convert-project.ps1 and builds them; prints the errors.
#   .\experiments\wf4c\verify-corpora.ps1 [-Only be,wt] [-Exclude @{ yaf = @('Base\...\File.cs') }]
param([string[]]$Only = @('be','wt','mojo','yaf','dnn','n2'), [hashtable]$Exclude = @{})
Set-Location C:\wcc\data\sessions\-rETZO4pVnOb
$env:DOTNET_CLI_UI_LANGUAGE = 'en'
$cp = 'experiments\wf4c\_culture\culture-profile.json'
$ex = @('Services\Compilation\Design\CodeExpressionEditor.cs','Services\Compilation\Design\QueryStringExpressionEditor.cs','Services\Compilation\Design\ServerVariableExpressionEditor.cs','Services\Compilation\Design\SessionExpressionEditor.cs','Services\Compilation\LinqLengthExpressionBuilder.cs','Services\FileSystem\FileStoreDb.cs','Providers\FileSystemProviders\DbFileSystemProvider.cs','Service References\GalleryServer\Reference.cs')
$corpora = @{
  be   = @('BlogEngine.NET-3.3.8.0', 'BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj')
  wt   = @('wingtiptoys-master', 'WingtipToys\WingtipToys\WingtipToys.csproj')
  mojo = @('mojoportal-3.1.6', 'Web\mojoPortal.Web.csproj')
  yaf  = @('YAFNET-3.2.15', 'yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj')
  dnn  = @('Dnn.Platform-9.13.10', 'DNN Platform\Website\DotNetNuke.Website.csproj')
  n2   = @('n2cms-master', 'src\WebForms\WebFormsTemplates\N2.Templates.csproj')
}
New-Item -ItemType Directory experiments\wf4c\_linux -Force | Out-Null
foreach ($k in $Only) {
  $root, $rel = $corpora[$k]
  $a = @{ Project = "corpora\work\$root\$rel"; Out = "experiments\wf4c\$k"; Root = "corpora\work\$root"; CultureProfile = $cp }
  if ($k -eq 'be') { $a.ExcludeFiles = $ex }
  if ($Exclude.ContainsKey($k)) { $a.ExcludeFiles = $Exclude[$k] }
  & .\experiments\wf4c\convert-project.ps1 @a *> "experiments\wf4c\_linux\convert-$k.log"
  if (-not $?) { "=== $k CONVERT FAILED"; Get-Content "experiments\wf4c\_linux\convert-$k.log" -Tail 5; continue }
  $log = "experiments\wf4c\_linux\build-$k.log"
  dotnet build "experiments\wf4c\$k\$rel" -v q -nologo > $log 2>&1
  $code = $LASTEXITCODE
  $lines = Select-String -Path $log -Pattern ': error ' | ForEach-Object { $_.Line }
  "=== $k exit=$code error-lines=$($lines.Count)"
  $lines | ForEach-Object { $_ -replace '^.*[\/]([^\/(]+)\([0-9,]+\): error ', '$1: ' -replace '^.*: error (NU|MSB)', '$1' -replace ' \[[^\]]*\]$', '' } |
    ForEach-Object { $_.Substring(0, [Math]::Min(200, $_.Length)) } | Group-Object | Sort-Object Count -Descending | Select-Object -First 8 | ForEach-Object { "  $($_.Count)x $($_.Name)" }
}
