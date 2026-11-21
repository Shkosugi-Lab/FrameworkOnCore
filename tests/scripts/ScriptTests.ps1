# The scripts a user runs, under Windows PowerShell 5.1 (powershell.exe), as a user has them: its parser (no syntax of
# PowerShell 7), its code page for files read without an encoding, its native commands' stderr under
# $ErrorActionPreference = 'Stop', paths with [ ] and beyond ASCII, a folder that is no git checkout (the repository's ZIP).
# The CI's Windows job runs it (frameworkoncore-scripts.yml); locally:
#   powershell -NoProfile -ExecutionPolicy Bypass -File tests\scripts\ScriptTests.ps1
# Each check prints PASS or FAIL; the exit code is the number of failures.
$ErrorActionPreference = 'Stop'
$repository = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$work = Join-Path ([IO.Path]::GetTempPath()) ('foc-scripts-' + [guid]::NewGuid().ToString('N').Substring(0, 8))
New-Item -ItemType Directory -Path $work | Out-Null
$failures = 0

function Check([string]$Name, [scriptblock]$Body) {
    try {
        $problem = & $Body
        if ($problem) { Write-Host "FAIL $Name : $problem"; $script:failures++ } else { Write-Host "PASS $Name" }
    }
    catch { Write-Host "FAIL $Name : $($_.Exception.Message)"; $script:failures++ }
}

# A script run by Windows PowerShell 5.1 in a process of its own: its output and errors as text, its exit code.
function Invoke-Ps([string]$Command) {
    $file = Join-Path $work ('run-' + [guid]::NewGuid().ToString('N').Substring(0, 6) + '.ps1')
    [IO.File]::WriteAllText($file, $Command, (New-Object Text.UTF8Encoding $true))
    $ErrorActionPreference = 'Continue'
    $output = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $file 2>&1 | Out-String
    return @{ Output = $output; Exit = $LASTEXITCODE }
}

# ---- 1. The scripts a user runs parse in Windows PowerShell 5.1 (no ??, ?:, &&, || of PowerShell 7).
$userScripts = @('converter.ps1', 'studio.ps1', 'tools\foc-tools.ps1', 'experiments\wf4c\capture-iis.ps1', 'experiments\wf4c\capture-culture.ps1',
    'experiments\wf4c\casefs\build.ps1', 'experiments\wf4c\casefs\key.ps1', 'tests\RuntimeUnitTests\Security\iis-golden\record.ps1')
foreach ($relative in $userScripts) {
    Check "parses in Windows PowerShell 5.1: $relative" {
        $tokens = $null; $errors = $null
        [System.Management.Automation.Language.Parser]::ParseFile((Join-Path $repository $relative), [ref]$tokens, [ref]$errors) | Out-Null
        if ($errors.Count -gt 0) { ($errors | ForEach-Object { "line $($_.Extent.StartLineNumber): $($_.Message)" }) -join '; ' }
    }
}

# ---- 1b. A script with characters beyond ASCII has a byte order mark: Windows PowerShell 5.1 reads one without it in the
# system's code page (a Japanese literal garbled: this file's own XML was no XML until it had one).
Check 'every .ps1 beyond ASCII has a byte order mark' {
    $ErrorActionPreference = 'Continue'
    $listed = & git -C $repository ls-files '*.ps1' 2>$null
    $bad = foreach ($relative in $listed) {
        $bytes = [IO.File]::ReadAllBytes((Join-Path $repository $relative))
        $bom = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
        if (-not $bom -and ($bytes | Where-Object { $_ -gt 127 } | Select-Object -First 1)) { $relative }
    }
    if ($bad) { 'without one: ' + ($bad -join ', ') }
}

# ---- 2. studio.ps1 and converter.ps1 in a folder that is no git checkout (the repository's ZIP): they build here, as
# they say; git's "fatal: not a git repository" on stderr stopped them.
Check 'foc-tools.ps1 outside a git checkout goes on to build' {
    $copy = Join-Path $work 'zip-copy'
    New-Item -ItemType Directory -Path (Join-Path $copy 'tools') | Out-Null
    Copy-Item (Join-Path $repository 'tools\foc-tools.ps1') (Join-Path $copy 'tools')
    $run = Invoke-Ps @"
`$ErrorActionPreference = 'Stop'
. '$copy\tools\foc-tools.ps1'
try { Get-FocTools | Out-Null } catch { 'caught: ' + `$_.Exception.Message }
"@
    if ($run.Output -notmatch 'building Studio and the converter here: not a git checkout') { "it did not go on to build: $($run.Output.Trim())" }
}

# ---- 3. capture-iis.ps1 with a site's name and folder in Japanese (applicationHost.config is UTF-8 without a BOM): read in
# the system's code page, the file was no XML and the capture failed; relative paths are PowerShell's location's.
Check 'capture-iis.ps1 reads applicationHost.config as UTF-8' {
    $folder = Join-Path $work 'iis [config]'
    New-Item -ItemType Directory -Path $folder | Out-Null
    $config = @'
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
  <system.applicationHost>
    <sites>
      <site name="社内ポータル" id="2">
        <application path="/" applicationPool="DefaultAppPool"><virtualDirectory path="/" physicalPath="C:\inetpub\社内" /></application>
        <bindings><binding protocol="http" bindingInformation="*:80:" /></bindings>
      </site>
    </sites>
  </system.applicationHost>
  <system.webServer><defaultDocument><files><add value="トップ.aspx" /></files></defaultDocument></system.webServer>
</configuration>
'@
    [IO.File]::WriteAllText((Join-Path $folder 'applicationHost.config'), $config, (New-Object Text.UTF8Encoding $false))
    $run = Invoke-Ps @"
Set-Location -LiteralPath '$work'
& '$repository\experiments\wf4c\capture-iis.ps1' -Site '社内ポータル' -ApplicationHost 'iis [config]\applicationHost.config' -Out 'iis [config]\profile.config'
"@
    $profile = Join-Path $folder 'profile.config'
    if (-not (Test-Path -LiteralPath $profile)) { return "no profile: $($run.Output.Trim())" }
    if ([IO.File]::ReadAllText($profile) -notmatch 'トップ\.aspx') { 'the default document lost its Japanese name' }
}

# ---- 4. capture-culture.ps1 with a web.config in Shift_JIS, in a folder with [ ] (WingtipToys VB's "[VB]-...").
Check 'capture-culture.ps1 reads a Shift_JIS web.config in a folder with [ ]' {
    $folder = Join-Path $work '[VB]-app'
    New-Item -ItemType Directory -Path $folder | Out-Null
    $xml = "<?xml version=`"1.0`" encoding=`"shift_jis`"?>`r`n<configuration><!-- 日本語 --><system.web><globalization culture=`"ja-JP`" uiCulture=`"en-US`" /></system.web></configuration>"
    [IO.File]::WriteAllBytes((Join-Path $folder 'web.config'), [Text.Encoding]::GetEncoding(932).GetBytes($xml))
    $out = Join-Path $work 'culture.json'
    $run = Invoke-Ps "& '$repository\experiments\wf4c\capture-culture.ps1' -WebConfig '$folder\web.config' -Out '$out'"
    if (-not (Test-Path -LiteralPath $out)) { return "no profile: $($run.Output.Trim())" }
    if ([IO.File]::ReadAllText($out) -notmatch '"en-US"') { 'the UI culture of web.config is not in the profile' }
}

# ---- 5. casefs's key: the same as the converter computes it (RuntimeSetup.CaseLibraryKey), whatever the line ends of the
# checkout (CR left out): 12 hexadecimal characters.
Check 'casefs key.ps1 gives a key' {
    $key = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $repository 'experiments\wf4c\casefs\key.ps1')
    if ("$key".Trim() -notmatch '^[0-9a-f]{12}$') { "not a key: $key" }
}

Remove-Item -LiteralPath $work -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "$failures failure(s)"
exit $failures
