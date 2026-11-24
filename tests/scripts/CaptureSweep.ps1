# The capture scripts a user runs on the original server (experiments\wf4c\capture-iis.ps1, capture-culture.ps1), under
# Windows PowerShell 5.1 as a user starts them (powershell.exe -File, a process of its own), on inputs made here in every
# shape of one thing at a time: the site's name, applicationHost.config's form (its encoding, what it has and has not,
# its <location>s, its secrets, a shared configuration), web.config's (its encoding, its namespace, where its
# globalization is, the cultures it names), the cultures given, where the output goes and how its path is given.
# There is no original to compare with: what is held is what must be so whatever the input:
#   - the script ends with exit code 0 and its output is there and well formed (XML, JSON), or it ends with another exit
#     code, tells what is wrong in words of its own (not a .NET exception's "You cannot call a method on a null-valued
#     expression"), and leaves no output;
#   - a profile has what was asked (the site's settings and no other site's; the cultures named) and no secret;
#   - the converter takes the output (--iis-profile, --culture-profile: its own check of them).
#   powershell -NoProfile -ExecutionPolicy Bypass -File tests\scripts\CaptureSweep.ps1 [-Converter <FrameworkOnCore.Converter.dll>]
# Each check prints PASS or FAIL; the exit code is the number of failures.
param([string]$Converter)

$ErrorActionPreference = 'Stop'
$repository = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$work = Join-Path ([IO.Path]::GetTempPath()) ('foc-capture-' + [guid]::NewGuid().ToString('N').Substring(0, 8))
New-Item -ItemType Directory -Path $work | Out-Null
$failures = 0
$checks = 0
$captureIis = Join-Path $repository 'experiments\wf4c\capture-iis.ps1'
$captureCulture = Join-Path $repository 'experiments\wf4c\capture-culture.ps1'
if (-not $Converter) { $Converter = Join-Path $repository 'src\FrameworkOnCore.Converter\bin\Debug\net10.0\FrameworkOnCore.Converter.dll' }
$utf8 = New-Object Text.UTF8Encoding $false

function Check([string]$Name, [scriptblock]$Body) {
    $script:checks++
    try {
        $problem = & $Body
        if ($problem) { Write-Host "FAIL $Name : $problem"; $script:failures++ } else { Write-Host "PASS $Name" }
    }
    catch { Write-Host "FAIL $Name : $($_.Exception.Message)"; $script:failures++ }
}

# A script as a user starts it: powershell.exe -File <script> <arguments>, from a folder; its output and exit code.
function Run-Script([string]$Script, [string[]]$Arguments, [string]$From = $work) {
    $start = New-Object Diagnostics.ProcessStartInfo 'powershell.exe'
    $quoted = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $Script) + $Arguments | ForEach-Object { '"' + ($_ -replace '"', '\"') + '"' }
    $start.Arguments = $quoted -join ' '
    $start.WorkingDirectory = $From
    $start.UseShellExecute = $false
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    $process = [Diagnostics.Process]::Start($start)
    $error2 = $process.StandardError.ReadToEndAsync()
    $output = $process.StandardOutput.ReadToEnd()
    $process.WaitForExit()
    return @{ Exit = $process.ExitCode; Output = ($output + $error2.Result) }
}

# What a script's own message is not: PowerShell's or .NET's words for a mistake in the script.
$notItsOwn = 'null-valued expression|Cannot index into a null array|Cannot bind argument|InvokeMethodOnNull|Exception calling ".*" with|MethodInvocationException|PropertyNotFound|is not recognized as the name'

function New-Folder([string]$name) {
    $folder = Join-Path $work $name
    New-Item -ItemType Directory -Path $folder -Force | Out-Null
    $folder
}

# ======================================================================================================================
# capture-iis.ps1

# applicationHost.config: the sites, the server's sections, the <location>s, as given.
function New-ApplicationHost([string[]]$Sites = @('App'), [string]$Locations = '', [string]$Server = '<defaultDocument><files><add value="Default.aspx" /></files></defaultDocument>',
    [string]$Extra = '', [switch]$NoSites, [switch]$NoApplicationHost) {
    $siteXml = ''
    $id = 1
    foreach ($site in $Sites) {
        $name = [Security.SecurityElement]::Escape($site)
        $siteXml += "<site name=`"$name`" id=`"$id`"><application path=`"/`"><virtualDirectory path=`"/`" physicalPath=`"C:\inetpub\$id`" /></application><logFile directory=`"C:\logs\$id`" /></site>"
        $id++
    }
    $host2 = if ($NoApplicationHost) { '' } elseif ($NoSites) { '<system.applicationHost><log /></system.applicationHost>' }
        else { "<system.applicationHost><sites>$siteXml<siteDefaults><logFile logFormat=`"W3C`" /></siteDefaults></sites><log><centralW3CLogFile enabled=`"false`" /></log></system.applicationHost>" }
    "<?xml version=`"1.0`" encoding=`"UTF-8`"?>`r`n<configuration>$host2<system.webServer>$Server<globalModules><add name=`"StaticFileModule`" image=`"static.dll`" /></globalModules></system.webServer>$Locations$Extra</configuration>"
}

# The profile as XML, or what it is not.
function Read-Profile([string]$path) {
    if (-not (Test-Path -LiteralPath $path)) { throw "no profile at $path" }
    $document = New-Object Xml.XmlDocument
    $document.Load($path)
    if ($document.DocumentElement.Name -ne 'iisProfile') { throw "the root is $($document.DocumentElement.Name)" }
    $document
}

# The converter's own check of a profile (its option's file: refused with exit code 2 when it is none).
function Converter-Takes([string]$option, [string]$file) {
    if (-not (Test-Path -LiteralPath $Converter)) { return $null }
    $app = New-Folder ('conv-' + [guid]::NewGuid().ToString('N').Substring(0, 6))
    [IO.File]::WriteAllText((Join-Path $app 'App.csproj'), '<Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003"><PropertyGroup><AssemblyName>App</AssemblyName></PropertyGroup></Project>')
    [IO.File]::WriteAllText((Join-Path $app 'Web.config'), '<configuration />')
    $ErrorActionPreference = 'Continue'
    $said = & dotnet $Converter (Join-Path $app 'App.csproj') --out (Join-Path $work ('out-' + [guid]::NewGuid().ToString('N').Substring(0, 6))) --no-build --deploy none $option $file 2>&1 | Out-String
    if ($LASTEXITCODE -eq 2 -and $said -match [regex]::Escape($option)) { return "the converter refuses it: $($said.Trim())" }
    $null
}

# ---- the site's name
$siteNames = @('App', 'Default Web Site', '社内ポータル', 'a&b', 'site[1]', "it's", 'dollar$x', 'semi;colon', 'x (1)', 'UPPER lower', 'ａｂｃ', 'tête', '-dash', 'a,b', 'a=b', 'percent%20', 'back`tick', 'brace{x}', 'at@x', 'hash#1')
foreach ($site in $siteNames) {
    Check "capture-iis: the site '$site'" {
        $folder = New-Folder ('site-' + [guid]::NewGuid().ToString('N').Substring(0, 6))
        $locations = "<location path=`"$([Security.SecurityElement]::Escape($site))`"><system.webServer><httpErrors errorMode=`"Custom`" /></system.webServer></location>" +
            "<location path=`"$([Security.SecurityElement]::Escape($site))/admin`"><system.webServer><directoryBrowse enabled=`"true`" /></system.webServer></location>" +
            "<location path=`"Other`"><system.webServer><directoryBrowse enabled=`"false`" /></system.webServer></location>"
        [IO.File]::WriteAllText((Join-Path $folder 'applicationHost.config'), (New-ApplicationHost -Sites 'Other', $site, ($site + '2') -Locations $locations), $utf8)
        $out = Join-Path $folder 'profile.config'
        $run = Run-Script $captureIis @('-Site', $site, '-Out', $out, '-ApplicationHost', (Join-Path $folder 'applicationHost.config'))
        if ($run.Exit -ne 0) { return "exit $($run.Exit): $($run.Output.Trim())" }
        $profile = Read-Profile $out
        if ($profile.DocumentElement.GetAttribute('site') -ne $site) { return "site is $($profile.DocumentElement.GetAttribute('site'))" }
        if ($profile.DocumentElement.SelectSingleNode('site').GetAttribute('id') -ne '2') { return 'not the site''s id' }
        $paths = @($profile.DocumentElement.SelectNodes('location') | ForEach-Object { $_.GetAttribute('path') }) -join '|'
        if ($paths -ne '|admin') { return "the site's locations are '$paths', not the site's own and admin" }
        Converter-Takes '--iis-profile' $out
    }
}

# ---- the <location>s: the site's own, in another case (IIS takes a location's path without regard to case), of a folder
# below, of a site whose name begins with the site's; none of another site's
$locationCases = @(
    @{ Name = 'the site and two folders'; Paths = @('App', 'App/admin', 'App/admin/users'); Expected = '|admin|admin/users' },
    @{ Name = 'in another case'; Paths = @('app', 'APP/Admin'); Expected = '|Admin' },
    @{ Name = 'a site whose name begins with it'; Paths = @('App2', 'App2/admin', 'Apple', 'App'); Expected = '' },
    @{ Name = 'none of the site'; Paths = @('Other', 'Other/App'); Expected = $null },
    @{ Name = 'a path with a final slash'; Paths = @('App/', 'App/admin/'); Expected = '|admin/' },
    @{ Name = 'a folder with a space and Japanese'; Paths = @('App/my folder/資料'); Expected = 'my folder/資料' },
    @{ Name = 'the server''s (an empty path, a dot)'; Paths = @('', '.'); Expected = $null }
)
foreach ($case in $locationCases) {
    Check "capture-iis: locations, $($case.Name)" {
        $folder = New-Folder ('loc-' + [guid]::NewGuid().ToString('N').Substring(0, 6))
        $locations = ($case.Paths | ForEach-Object { "<location path=`"$_`"><system.webServer><httpErrors errorMode=`"Custom`" /></system.webServer></location>" }) -join ''
        [IO.File]::WriteAllText((Join-Path $folder 'applicationHost.config'), (New-ApplicationHost -Sites 'App', 'App2', 'Apple', 'Other' -Locations $locations), $utf8)
        $out = Join-Path $folder 'profile.config'
        $run = Run-Script $captureIis @('-Site', 'App', '-Out', $out, '-ApplicationHost', (Join-Path $folder 'applicationHost.config'))
        if ($run.Exit -ne 0) { return "exit $($run.Exit): $($run.Output.Trim())" }
        $profile = Read-Profile $out
        $paths = @($profile.DocumentElement.SelectNodes('location') | ForEach-Object { $_.GetAttribute('path') })
        $expected = if ($null -eq $case.Expected) { @() } else { @($case.Expected -split '\|') }
        if (($paths -join '|') -ne ($expected -join '|')) { return "locations '$($paths -join '|')', expected '$($expected -join '|')'" }
    }
}

# ---- secrets: none in the profile, wherever the configuration has them
$secretCases = @(
    @{ Name = 'a password attribute'; Xml = '<security><authentication><anonymousAuthentication enabled="true" userName="svc" password="SECRET-1" /></authentication></security>' },
    @{ Name = 'a Password attribute in another case'; Xml = '<odbcLogging dataSource="x" userName="u" Password="SECRET-1" />' },
    @{ Name = 'an encrypted value'; Xml = '<security><authentication><anonymousAuthentication userName="svc" other="[enc:AesProvider:SECRET-1:enc]" /></authentication></security>' },
    @{ Name = 'a connection string in a value'; Xml = '<rewrite><providers><provider name="Db" type="x"><settings><add key="ConnectionString" value="Server=db;User ID=sa;Password=SECRET-1;" /></settings></provider></providers></rewrite>' },
    @{ Name = 'a connection string with pwd'; Xml = '<rewrite><providers><provider name="Db" type="x"><settings><add key="ConnectionString" value="Server=db;uid=sa;pwd=SECRET-1" /></settings></provider></providers></rewrite>' },
    @{ Name = 'a password in a custom header'; Xml = '<httpProtocol><customHeaders><add name="X-Api-Password" value="SECRET-1" /><add name="X-Powered-By" value="ASP.NET" /></customHeaders></httpProtocol>' },
    @{ Name = 'a URL with credentials'; Xml = '<httpRedirect enabled="true" destination="https://user:SECRET-1@example.invalid/" />' }
)
foreach ($case in $secretCases) {
    foreach ($where in 'the server''s', 'the site''s location') {
        Check "capture-iis: secrets, $($case.Name), $where" {
            $folder = New-Folder ('sec-' + [guid]::NewGuid().ToString('N').Substring(0, 6))
            $config = if ($where -eq 'the server''s') { New-ApplicationHost -Server $case.Xml } else { New-ApplicationHost -Locations "<location path=`"App`"><system.webServer>$($case.Xml)</system.webServer></location>" }
            [IO.File]::WriteAllText((Join-Path $folder 'applicationHost.config'), $config, $utf8)
            $out = Join-Path $folder 'profile.config'
            $run = Run-Script $captureIis @('-Site', 'App', '-Out', $out, '-ApplicationHost', (Join-Path $folder 'applicationHost.config'))
            if ($run.Exit -ne 0) { return "exit $($run.Exit): $($run.Output.Trim())" }
            [void](Read-Profile $out)
            if ([IO.File]::ReadAllText($out) -match 'SECRET-1') { return 'the secret is in the profile' }
            if ($run.Output -match 'SECRET-1') { return 'the secret is in what the script says' }
        }
    }
}

# ---- applicationHost.config's form
$formCases = @(
    @{ Name = 'with a byte order mark'; Write = { param($f) [IO.File]::WriteAllText($f, (New-ApplicationHost), (New-Object Text.UTF8Encoding $true)) }; Ok = $true },
    @{ Name = 'in UTF-16'; Write = { param($f) [IO.File]::WriteAllText($f, ((New-ApplicationHost) -replace 'encoding="UTF-8"', 'encoding="utf-16"'), [Text.Encoding]::Unicode) }; Ok = $true },
    @{ Name = 'without a declaration'; Write = { param($f) [IO.File]::WriteAllText($f, ((New-ApplicationHost) -replace '<\?xml[^>]*\?>\r\n', ''), $utf8) }; Ok = $true },
    @{ Name = 'with comments and a DOCTYPE-free prolog'; Write = { param($f) [IO.File]::WriteAllText($f, ((New-ApplicationHost) -replace '<configuration>', "<!-- 設定 -->`r`n<configuration><!-- sites -->"), $utf8) }; Ok = $true },
    @{ Name = 'without the server''s system.webServer'; Write = { param($f) [IO.File]::WriteAllText($f, ((New-ApplicationHost) -replace '<system.webServer>.*</system.webServer>', ''), $utf8) }; Ok = $true },
    @{ Name = 'without siteDefaults and log'; Write = { param($f) [IO.File]::WriteAllText($f, ((New-ApplicationHost) -replace '<siteDefaults>.*</siteDefaults>', '' -replace '<log>.*</log>', ''), $utf8) }; Ok = $true },
    @{ Name = 'without sites'; Write = { param($f) [IO.File]::WriteAllText($f, (New-ApplicationHost -NoSites), $utf8) }; Ok = $false },
    @{ Name = 'without system.applicationHost'; Write = { param($f) [IO.File]::WriteAllText($f, (New-ApplicationHost -NoApplicationHost), $utf8) }; Ok = $false },
    @{ Name = 'another root'; Write = { param($f) [IO.File]::WriteAllText($f, '<other />', $utf8) }; Ok = $false },
    @{ Name = 'empty'; Write = { param($f) [IO.File]::WriteAllText($f, '', $utf8) }; Ok = $false },
    @{ Name = 'no XML'; Write = { param($f) [IO.File]::WriteAllText($f, 'not xml <', $utf8) }; Ok = $false },
    @{ Name = 'not there'; Write = { param($f) }; Ok = $false },
    @{ Name = 'a folder'; Write = { param($f) New-Item -ItemType Directory -Path $f | Out-Null }; Ok = $false }
)
foreach ($case in $formCases) {
    Check "capture-iis: applicationHost.config $($case.Name)" {
        $folder = New-Folder ('form-' + [guid]::NewGuid().ToString('N').Substring(0, 6))
        $config = Join-Path $folder 'applicationHost.config'
        & $case.Write $config
        $out = Join-Path $folder 'profile.config'
        $run = Run-Script $captureIis @('-Site', 'App', '-Out', $out, '-ApplicationHost', $config)
        if ($case.Ok) {
            if ($run.Exit -ne 0) { return "exit $($run.Exit): $($run.Output.Trim())" }
            [void](Read-Profile $out)
            return (Converter-Takes '--iis-profile' $out)
        }
        if ($run.Exit -eq 0) { return "exit 0: $($run.Output.Trim())" }
        if (Test-Path -LiteralPath $out) { return 'a profile was written all the same' }
        if ($run.Output -match $notItsOwn) { return "not told in its own words: $($run.Output.Trim())" }
        if ($run.Output -notmatch 'applicationHost') { return "the file is not named: $($run.Output.Trim())" }
    }
}

# ---- a site that is not there: told, with the sites there are
Check 'capture-iis: a site that is not there' {
    $folder = New-Folder 'nosite'
    [IO.File]::WriteAllText((Join-Path $folder 'applicationHost.config'), (New-ApplicationHost -Sites 'One', 'Two'), $utf8)
    $out = Join-Path $folder 'profile.config'
    $run = Run-Script $captureIis @('-Site', 'Three', '-Out', $out, '-ApplicationHost', (Join-Path $folder 'applicationHost.config'))
    if ($run.Exit -eq 0) { return 'exit 0' }
    if ($run.Output -notmatch 'Three' -or $run.Output -notmatch 'One, Two') { return "not told with the sites: $($run.Output.Trim())" }
    if (Test-Path -LiteralPath $out) { return 'a profile was written' }
}

# ---- a shared configuration (redirection.config)
foreach ($case in @(@{ Name = 'enabled'; Enabled = 'true'; Expected = 'Shared' }, @{ Name = 'not enabled'; Enabled = 'false'; Expected = 'Local' })) {
    Check "capture-iis: a shared configuration $($case.Name)" {
        $folder = New-Folder ('shared-' + $case.Name)
        $shared = New-Folder ('shared-' + $case.Name + '\共有 [1]')
        [IO.File]::WriteAllText((Join-Path $folder 'applicationHost.config'), (New-ApplicationHost -Sites 'Local'), $utf8)
        [IO.File]::WriteAllText((Join-Path $shared 'applicationHost.config'), (New-ApplicationHost -Sites 'Shared'), $utf8)
        [IO.File]::WriteAllText((Join-Path $folder 'redirection.config'), "<configuration><configurationRedirection enabled=`"$($case.Enabled)`" path=`"$shared`" userName=`"u`" password=`"SECRET-1`" /></configuration>", $utf8)
        $out = Join-Path $folder 'profile.config'
        $run = Run-Script $captureIis @('-Site', $case.Expected, '-Out', $out, '-ApplicationHost', (Join-Path $folder 'applicationHost.config'))
        if ($run.Exit -ne 0) { return "exit $($run.Exit): $($run.Output.Trim())" }
        [void](Read-Profile $out)
        if ([IO.File]::ReadAllText($out) -match 'SECRET-1') { return 'the secret is in the profile' }
    }
}

# ---- where the output goes, and how the paths are given (from a folder with [ ] too)
$outCases = @(
    @{ Name = 'relative'; Out = 'profile.config'; From = 'plain' },
    @{ Name = 'relative with .\'; Out = '.\profile.config'; From = 'plain' },
    @{ Name = 'relative, from a folder with [ ]'; Out = 'profile.config'; From = 'from [1]' },
    @{ Name = 'relative, from a folder with Japanese and a space'; Out = 'profile.config'; From = '日本語 のフォルダー' },
    @{ Name = 'in a folder that is not there'; Out = 'new\deeper\profile.config'; From = 'plain2' },
    @{ Name = 'a name with [ ] and a space'; Out = 'my [iis] profile.config'; From = 'plain3' },
    @{ Name = 'a name with Japanese'; Out = 'プロファイル.config'; From = 'plain4' },
    @{ Name = 'forward slashes'; Out = 'sub/profile.config'; From = 'plain5' },
    @{ Name = 'over a file that is there'; Out = 'profile.config'; From = 'plain6'; Before = 'old' }
)
foreach ($case in $outCases) {
    Check "capture-iis: the output $($case.Name)" {
        $from = New-Folder ('out-' + $case.From)
        [IO.File]::WriteAllText((Join-Path $from 'applicationHost.config'), (New-ApplicationHost), $utf8)
        if ($case.Before) { [IO.File]::WriteAllText((Join-Path $from $case.Out), $case.Before, $utf8) }
        # (the configuration given relative too: PowerShell's location's)
        $run = Run-Script $captureIis @('-Site', 'App', '-Out', $case.Out, '-ApplicationHost', 'applicationHost.config') -From $from
        if ($run.Exit -ne 0) { return "exit $($run.Exit): $($run.Output.Trim())" }
        [void](Read-Profile (Join-Path $from ($case.Out -replace '/', '\')))
    }
}
Check 'capture-iis: the output a folder that is there' {
    $from = New-Folder 'out-folder'
    [IO.File]::WriteAllText((Join-Path $from 'applicationHost.config'), (New-ApplicationHost), $utf8)
    New-Item -ItemType Directory -Path (Join-Path $from 'profile') | Out-Null
    $run = Run-Script $captureIis @('-Site', 'App', '-Out', 'profile', '-ApplicationHost', 'applicationHost.config') -From $from
    if ($run.Exit -eq 0) { return 'exit 0' }
    if ($run.Output -match $notItsOwn) { return "not told in its own words: $($run.Output.Trim())" }
}

# ======================================================================================================================
# capture-culture.ps1

function Read-Culture([string]$path) {
    if (-not (Test-Path -LiteralPath $path)) { throw "no profile at $path" }
    $profile = [IO.File]::ReadAllText($path) | ConvertFrom-Json
    if (-not $profile.Cultures) { throw 'no Cultures in it' }
    $profile
}
function Culture-Names($profile) { @($profile.Cultures.PSObject.Properties | ForEach-Object { $_.Name }) }

# ---- the cultures given, as a user gives them to -File ("-Cultures en-US,fr-FR" is one argument there)
$cultureCases = @(
    @{ Name = 'none'; Arguments = @(); Has = @() },
    @{ Name = 'one'; Arguments = @('-Cultures', 'fr-FR'); Has = @('fr-FR') },
    @{ Name = 'two, with a comma (as the script''s own usage line has it)'; Arguments = @('-Cultures', 'en-US,fr-FR'); Has = @('en-US', 'fr-FR') },
    @{ Name = 'two, with a comma and a space'; Arguments = @('-Cultures', 'en-US, fr-FR'); Has = @('en-US', 'fr-FR') },
    @{ Name = 'a neutral one'; Arguments = @('-Cultures', 'de'); Has = @('de') },
    @{ Name = 'neutral ones of Chinese'; Arguments = @('-Cultures', 'zh-Hans,zh-CHS,zh-TW'); Has = @('zh-Hans', 'zh-CHS', 'zh-TW') },
    @{ Name = 'in another case'; Arguments = @('-Cultures', 'JA-jp'); Has = @('ja-JP') },
    @{ Name = 'the same twice'; Arguments = @('-Cultures', 'fr-FR,fr-FR,FR-fr'); Has = @('fr-FR') },
    @{ Name = 'cultures of other calendars and directions'; Arguments = @('-Cultures', 'th-TH,ar-SA,fa-IR,he-IL,hi-IN'); Has = @('th-TH', 'ar-SA', 'fa-IR', 'he-IL', 'hi-IN') },
    @{ Name = 'an empty one among them'; Arguments = @('-Cultures', 'fr-FR,,de-DE'); Has = @('fr-FR', 'de-DE') }
)
foreach ($case in $cultureCases) {
    Check "capture-culture: -Cultures $($case.Name)" {
        $out = Join-Path (New-Folder ('cul-' + [guid]::NewGuid().ToString('N').Substring(0, 6))) 'culture.json'
        $run = Run-Script $captureCulture (@('-Out', $out) + $case.Arguments)
        if ($run.Exit -ne 0) { return "exit $($run.Exit): $($run.Output.Trim())" }
        $profile = Read-Culture $out
        $names = Culture-Names $profile
        foreach ($name in $case.Has) { if ($names -notcontains $name) { return "$name is not in the profile ($($names -join ', '))" } }
        if ($names -notcontains $profile.DefaultCulture) { return "the default culture $($profile.DefaultCulture) is not in the profile" }
        foreach ($name in $names) {
            $culture = $profile.Cultures.$name
            if (-not $culture.NumberFormat.NumberDecimalSeparator -or -not $culture.DateTimeFormat.ShortDatePattern -or -not $culture.EnglishName) { return "$name lacks its formats" }
        }
        Converter-Takes '--culture-profile' $out
    }
}
# A name that is no culture's: told with the name (a typing mistake), no profile.
foreach ($name in 'xx-INVALID-NAME-TOO-LONG', 'fr_FR', 'not a culture', '!!', 'fr-FR;de-DE') {
    Check "capture-culture: -Cultures '$name' is no culture" {
        $out = Join-Path (New-Folder ('bad-' + [guid]::NewGuid().ToString('N').Substring(0, 6))) 'culture.json'
        $run = Run-Script $captureCulture @('-Out', $out, '-Cultures', $name)
        if ($run.Exit -eq 0) { return "exit 0: $($run.Output.Trim())" }
        if (Test-Path -LiteralPath $out) { return 'a profile was written all the same' }
        if ($run.Output -match $notItsOwn) { return "not told in its own words: $($run.Output.Trim())" }
        if ($run.Output -notmatch [regex]::Escape(($name -split '[,;]')[0])) { return "the name is not told: $($run.Output.Trim())" }
    }
}

# ---- web.config: where its globalization is and what it names
$webCases = @(
    @{ Name = 'culture and uiCulture'; Xml = '<configuration><system.web><globalization culture="fr-FR" uiCulture="de-DE" /></system.web></configuration>'; Has = @('fr-FR', 'de-DE') },
    @{ Name = 'auto with a fallback'; Xml = '<configuration><system.web><globalization culture="auto:it-IT" uiCulture="auto" /></system.web></configuration>'; Has = @('it-IT') },
    @{ Name = 'auto'; Xml = '<configuration><system.web><globalization culture="auto" uiCulture="auto" /></system.web></configuration>'; Has = @() },
    @{ Name = 'empty attributes'; Xml = '<configuration><system.web><globalization culture="" uiCulture="" /></system.web></configuration>'; Has = @() },
    @{ Name = 'no globalization'; Xml = '<configuration><system.web><compilation debug="true" /></system.web></configuration>'; Has = @() },
    @{ Name = 'no system.web'; Xml = '<configuration><appSettings /></configuration>'; Has = @() },
    @{ Name = 'the .NET 2.0 namespace'; Xml = '<configuration xmlns="http://schemas.microsoft.com/.NetConfiguration/v2.0"><system.web><globalization culture="fr-FR" uiCulture="de-DE" /></system.web></configuration>'; Has = @('fr-FR', 'de-DE') },
    @{ Name = 'in a location for the application'; Xml = '<configuration><location path="."><system.web><globalization culture="es-ES" /></system.web></location></configuration>'; Has = @('es-ES') },
    @{ Name = 'in a location without a path'; Xml = '<configuration><location><system.web><globalization culture="es-ES" /></system.web></location></configuration>'; Has = @('es-ES') },
    @{ Name = 'in a location for a folder too'; Xml = '<configuration><system.web><globalization culture="fr-FR" /></system.web><location path="Admin"><system.web><globalization culture="pt-BR" /></system.web></location></configuration>'; Has = @('fr-FR', 'pt-BR') },
    @{ Name = 'attributes in another case of value'; Xml = '<configuration><system.web><globalization culture="FR-fr" uiCulture="AUTO:DE-de" /></system.web></configuration>'; Has = @('fr-FR', 'de-DE') },
    @{ Name = 'a neutral culture'; Xml = '<configuration><system.web><globalization culture="de" uiCulture="ja" /></system.web></configuration>'; Has = @('de', 'ja') },
    @{ Name = 'with a byte order mark and comments'; Xml = "<?xml version=`"1.0`" encoding=`"utf-8`"?><!-- 設定 --><configuration><system.web><!-- c --><globalization culture=`"fr-FR`" /></system.web></configuration>"; Bom = $true; Has = @('fr-FR') }
)
foreach ($case in $webCases) {
    Check "capture-culture: web.config, $($case.Name)" {
        $folder = New-Folder ('web-' + [guid]::NewGuid().ToString('N').Substring(0, 6))
        $encoding = if ($case.Bom) { New-Object Text.UTF8Encoding $true } else { $utf8 }
        [IO.File]::WriteAllText((Join-Path $folder 'web.config'), $case.Xml, $encoding)
        $out = Join-Path $folder 'culture.json'
        $run = Run-Script $captureCulture @('-Out', $out, '-WebConfig', (Join-Path $folder 'web.config'))
        if ($run.Exit -ne 0) { return "exit $($run.Exit): $($run.Output.Trim())" }
        $names = Culture-Names (Read-Culture $out)
        foreach ($name in $case.Has) { if ($names -notcontains $name) { return "$name is not in the profile ($($names -join ', '))" } }
        Converter-Takes '--culture-profile' $out
    }
}
# A web.config that is not one, or names what is no culture: told, no profile.
$badWeb = @(
    @{ Name = 'not there'; Write = { param($f) }; Tells = 'web.config' },
    @{ Name = 'no XML'; Write = { param($f) [IO.File]::WriteAllText($f, 'not xml <', $utf8) }; Tells = 'web.config' },
    @{ Name = 'empty'; Write = { param($f) [IO.File]::WriteAllText($f, '', $utf8) }; Tells = 'web.config' },
    @{ Name = 'a folder'; Write = { param($f) New-Item -ItemType Directory -Path $f | Out-Null }; Tells = 'web.config' },
    @{ Name = 'a culture that is none'; Write = { param($f) [IO.File]::WriteAllText($f, '<configuration><system.web><globalization culture="xx-NOT-A-CULTURE-NAME" /></system.web></configuration>', $utf8) }; Tells = 'xx-NOT-A-CULTURE-NAME' }
)
foreach ($case in $badWeb) {
    Check "capture-culture: a web.config $($case.Name)" {
        $folder = New-Folder ('badweb-' + [guid]::NewGuid().ToString('N').Substring(0, 6))
        & $case.Write (Join-Path $folder 'web.config')
        $out = Join-Path $folder 'culture.json'
        $run = Run-Script $captureCulture @('-Out', $out, '-WebConfig', (Join-Path $folder 'web.config'))
        if ($run.Exit -eq 0) { return "exit 0: $($run.Output.Trim())" }
        if (Test-Path -LiteralPath $out) { return 'a profile was written all the same' }
        if ($run.Output -match $notItsOwn) { return "not told in its own words: $($run.Output.Trim())" }
        if ($run.Output -notmatch [regex]::Escape($case.Tells)) { return "$($case.Tells) is not told: $($run.Output.Trim())" }
    }
}

# ---- where the output goes
foreach ($case in $outCases) {
    Check "capture-culture: the output $($case.Name)" {
        $from = New-Folder ('cout-' + $case.From)
        $name = $case.Out -replace '\.config$', '.json'
        if ($case.Before) { [IO.File]::WriteAllText((Join-Path $from $name), $case.Before, $utf8) }
        [IO.File]::WriteAllText((Join-Path $from 'web.config'), '<configuration><system.web><globalization culture="fr-FR" /></system.web></configuration>', $utf8)
        $run = Run-Script $captureCulture @('-Out', $name, '-WebConfig', 'web.config') -From $from
        if ($run.Exit -ne 0) { return "exit $($run.Exit): $($run.Output.Trim())" }
        if ((Culture-Names (Read-Culture (Join-Path $from ($name -replace '/', '\')))) -notcontains 'fr-FR') { return 'fr-FR is not in the profile' }
    }
}
Check 'capture-culture: the output a folder that is there' {
    $from = New-Folder 'cout-folder'
    New-Item -ItemType Directory -Path (Join-Path $from 'profile') | Out-Null
    $run = Run-Script $captureCulture @('-Out', 'profile') -From $from
    if ($run.Exit -eq 0) { return 'exit 0' }
    if ($run.Output -match $notItsOwn) { return "not told in its own words: $($run.Output.Trim())" }
}

# ---- the profile's secrets: the user's name is the machine's own, told; nothing of the environment
Check 'capture-culture: nothing of the environment but what it says it has' {
    $out = Join-Path (New-Folder 'env') 'culture.json'
    $env:FOC_SWEEP_SECRET = 'SECRET-1'
    $run = Run-Script $captureCulture @('-Out', $out)
    Remove-Item Env:FOC_SWEEP_SECRET
    if ($run.Exit -ne 0) { return "exit $($run.Exit): $($run.Output.Trim())" }
    if ([IO.File]::ReadAllText($out) -match 'SECRET-1') { return 'the environment is in the profile' }
}

# ======================================================================================================================
# converter.ps1: its arguments go to the converter as they are (the converter's "<argument>: no such option" tells what
# it was given). In a copy of the checkout's layout, with the tools' folder the built converter's (no fetch, no build).
if (Test-Path -LiteralPath $Converter) {
    $checkout = New-Folder 'checkout [1] 日本語'
    New-Item -ItemType Directory -Path (Join-Path $checkout 'tools'), (Join-Path $checkout 'experiments\wf4c') -Force | Out-Null
    Copy-Item (Join-Path $repository 'converter.ps1') $checkout
    [IO.File]::WriteAllText((Join-Path $checkout 'tools\foc-tools.ps1'), (([IO.File]::ReadAllText((Join-Path $repository 'tools\foc-tools.ps1')) -replace 'function Get-FocTools', 'function Get-FocToolsOfTheCheckout') + "`r`nfunction Get-FocTools([switch]`$Build) { '$(Split-Path $Converter -Parent)' }"), (New-Object Text.UTF8Encoding $true))
    $payloads = @('--plain', '--with space', '--two  spaces', '--日本語', '--semi;colon', '--amp&ersand', '--pipe|x', '--percent%PATH%', '--dollar$x', '--comma,x', "--quote'x", '--paren(x)',
        '--brace{x}', '--at@x', '--hash#x', '--back`tick', '--caret^x', '--less<more>', '--equals=x', '--bang!x', '--tilde~x', '--final backslash', '--back\slash', '--double"quote', '--star*x', '--question?x')
    foreach ($payload in $payloads) {
        Check "converter.ps1: the argument $payload as it is" {
            $run = Run-Script (Join-Path $checkout 'converter.ps1') @($payload)
            if ($run.Exit -ne 2) { return "exit $($run.Exit): $($run.Output.Trim())" }
            if ($run.Output -notmatch [regex]::Escape("${payload}: no such option")) { return "the converter was given another: $($run.Output.Trim())" }
        }
    }
    # A project's path and an output's with a space, [ ] and Japanese: the converter names the project as given.
    foreach ($name in 'my app', 'app [1]', 'アプリ', "it's", 'a&b', 'final space ') {
        Check "converter.ps1: a project in '$name'" {
            $project = Join-Path (Join-Path $work $name.TrimEnd()) 'None.csproj'
            $run = Run-Script (Join-Path $checkout 'converter.ps1') @($project, '--out', (Join-Path $work "$($name.TrimEnd()) out"), '--no-build')
            if ($run.Exit -ne 2) { return "exit $($run.Exit): $($run.Output.Trim())" }
            if ($run.Output -notmatch [regex]::Escape("${project}: no such project file")) { return "the converter was given another: $($run.Output.Trim())" }
        }
    }
    # The checkout's runtime is given unless one is, and not to the commands that take none.
    Check 'converter.ps1: --runtime of the checkout, unless given' {
        $none = Join-Path $work 'no-runtime'
        $run = Run-Script (Join-Path $checkout 'converter.ps1') @((Join-Path $checkout 'converter.ps1'), '--out', (Join-Path $work 'rt-out'), '--no-build', '--runtime', $none)
        if ($run.Output -notmatch [regex]::Escape("--runtime ${none}: no such folder")) { return "the runtime given was not the one taken: $($run.Output.Trim())" }
        $run = Run-Script (Join-Path $checkout 'converter.ps1') @('snapshot')
        if ($run.Exit -ne 2 -or $run.Output -notmatch 'usage: FrameworkOnCore.Converter snapshot') { return "snapshot was given more: $($run.Output.Trim())" }
        $run = Run-Script (Join-Path $checkout 'converter.ps1') @('build-original-advice')
        if ($run.Exit -ne 2 -or $run.Output -notmatch 'usage: FrameworkOnCore.Converter build-original-advice') { return "build-original-advice was given more: $($run.Output.Trim())" }
    }
    Check 'converter.ps1: the converter''s exit code is the script''s' {
        $run = Run-Script (Join-Path $checkout 'converter.ps1') @()
        if ($run.Exit -ne 2) { return "exit $($run.Exit)" }
    }
}

Remove-Item -LiteralPath $work -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "$failures failure(s) of $checks"
exit $failures
