# The Chart control end to end (samples\ChartProbe): its pages requested as a browser would, the images they name
# fetched (ChartImg.axd: the handler's file storage; ImageLocation: files the page writes into the application's
# folder; a page writing a PNG as its response), and what was seen written as lines: status, content type, the
# picture's size, digest and 8x8 grid, the image map's areas. -Record takes the lines from the original on IIS (.NET
# Framework 4.8) into samples\ChartProbe\golden-chart-probe.txt; -Windows and -Linux convert the sample
# (FrameworkOnCore.Converter, its default choices: the charts' port) and compare the lines with those: on Windows
# exactly (the same gdiplus.dll draws; pictures by their grid, see Compare-Lines), on Linux as the System.Drawing suite does (libgdiplus and other fonts: the grid
# within 12 on average, the areas' coordinates within 6 pixels).
#
#   .\experiments\wf4c\chart-probe.ps1 -Record      # IIS (an administrator's shell)
#   .\experiments\wf4c\chart-probe.ps1 -Windows
#   .\experiments\wf4c\chart-probe.ps1 -Linux       # Docker: the converter's Dockerfile, built and run
param(
    [switch]$Record,
    [switch]$Windows,
    [switch]$Linux,
    [int]$Port = 5097
)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$sample = Join-Path $repo 'samples\ChartProbe'
$golden = Join-Path $sample 'golden-chart-probe.txt'
$work = Join-Path $PSScriptRoot '_chartprobe'
New-Item -ItemType Directory $work -Force | Out-Null

Add-Type -AssemblyName System.Drawing
Add-Type -ReferencedAssemblies System.Drawing -TypeDefinition @'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

public static class ChartProbePicture
{
    // "<width>x<height> sha:<16 hex of the file's SHA-256> grid:<8x8 cells' average R, G, B as hex>"
    public static string Describe(byte[] file)
    {
        string sha;
        using (var hash = SHA256.Create()) sha = BitConverter.ToString(hash.ComputeHash(file)).Replace("-", "").Substring(0, 16).ToLowerInvariant();
        using (var bitmap = new Bitmap(new MemoryStream(file)))
        {
            int width = bitmap.Width, height = bitmap.Height;
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var pixels = new byte[data.Stride * height];
            Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
            bitmap.UnlockBits(data);
            var sums = new long[8, 8, 3];
            var counts = new long[8, 8];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    int cx = x * 8 / width, cy = y * 8 / height, at = y * data.Stride + x * 4;
                    for (int c = 0; c < 3; c++) sums[cy, cx, c] += pixels[at + 2 - c];
                    counts[cy, cx]++;
                }
            var grid = new StringBuilder();
            for (int cy = 0; cy < 8; cy++)
                for (int cx = 0; cx < 8; cx++)
                    for (int c = 0; c < 3; c++)
                        grid.Append((counts[cy, cx] == 0 ? 0 : sums[cy, cx, c] / counts[cy, cx]).ToString("x2"));
            return width + "x" + height + " sha:" + sha + " grid:" + grid;
        }
    }
}
'@

# As a browser: the cookies kept (the handler's images are private to the session that made them: privateImages).
$cookies = New-Object Net.CookieContainer
function Get-Url([string]$url) {
    $request = [Net.HttpWebRequest]::Create($url)
    $request.AllowAutoRedirect = $false
    $request.CookieContainer = $cookies
    $request.Timeout = 120000
    try { $response = $request.GetResponse() } catch [Net.WebException] { $response = $_.Exception.Response; if (-not $response) { throw } }
    $buffer = New-Object IO.MemoryStream
    $response.GetResponseStream().CopyTo($buffer)
    $result = @{ Status = [int]$response.StatusCode; Type = ($response.ContentType -split ';')[0]; Bytes = $buffer.ToArray() }
    $response.Close()
    $result
}

# What the site does, as lines.
function Probe([string]$base) {
    $lines = New-Object Collections.Generic.List[string]
    function Picture($label, $page) {
        if ($page.Type -like 'image/*') { [IO.File]::WriteAllBytes((Join-Path $pictures (($label.Trim() -replace '\W+', '-') + "-$($lines.Count).png")), $page.Bytes) }
        if ($page.Type -like 'image/*') { $lines.Add("$label -> $($page.Status) $($page.Type) ~px $([ChartProbePicture]::Describe($page.Bytes))") }
        else {
            $text = [Text.Encoding]::UTF8.GetString($page.Bytes) -replace '\s+', ' '
            $lines.Add("$label -> $($page.Status) $($page.Type) " + $text.Substring(0, [Math]::Min(300, $text.Length)))
        }
    }
    $page = Get-Url "$base/Default.aspx"
    $html = [Text.Encoding]::UTF8.GetString($page.Bytes)
    $lines.Add("Default.aspx -> $($page.Status) $($page.Type)")
    if ($page.Status -ne 200) { $lines.Add(($html -replace '\s+', ' ').Substring(0, [Math]::Min(2000, $html.Length))) }
    foreach ($image in [regex]::Matches($html, '<img\b[^>]*>')) {
        $src = [Net.WebUtility]::HtmlDecode([regex]::Match($image.Value, '\bsrc="([^"]*)"').Groups[1].Value)
        $map = [regex]::Match($image.Value, '\busemap="([^"]*)"').Groups[1].Value
        # The names made per request: the handler's key and guid, the location's sequence number.
        $shown = $src -replace '[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}', '#' -replace '[0-9a-fA-F]{16,}', '#' -replace '_\d+', '_#'
        $lines.Add("img $shown map=$map")
        $url = if ($src -match '^https?:') { $src } elseif ($src.StartsWith('/')) { "$base$src" } else { "$base/$src" }
        Picture '  fetched' (Get-Url $url)
        # Its status only when not found: the body and its type are the server's error page (IIS writes one).
        $again = Get-Url $url
        $lines.Add("  fetched again -> $($again.Status)" + $(if ($again.Status -eq 200) { " $($again.Type)" }))
    }
    foreach ($area in [regex]::Matches($html, '<area\b[^>]*>')) {
        $attribute = { param($name) [Net.WebUtility]::HtmlDecode([regex]::Match($area.Value, "\b$name=""([^""]*)""").Groups[1].Value) }
        $lines.Add("area $(& $attribute 'shape') coords=$(& $attribute 'coords') title=$(& $attribute 'title')")
    }
    Picture 'Stream.aspx' (Get-Url "$base/Stream.aspx")
    $missing = Get-Url "$base/ChartImg.axd?i=chart_0123456789abcdef0123456789abcdef_0.png&g=0123456789abcdef0123456789abcdef"
    $lines.Add("ChartImg.axd (no such image) -> $($missing.Status)")
    $lines
}

function Wait-Site([string]$base) {
    foreach ($attempt in 1..90) {
        try { Invoke-WebRequest "$base/Stream.aspx" -UseBasicParsing -TimeoutSec 60 | Out-Null; return }
        catch { if ($_.Exception.Response) { return }; Start-Sleep -Seconds 2 }
    }
    throw "$base did not answer"
}

# Golden and actual, as the parity suites compare them (ParityComparison): on Windows exactly, pictures by size and
# grid; on Linux pictures by size and grid within 12 on average, the areas' coordinates within 6 pixels.
function Compare-Lines([string[]]$expected, [string[]]$actual, [bool]$loose) {
    $report = @()
    for ($i = 0; $i -lt [Math]::Max($expected.Count, $actual.Count); $i++) {
        $e = if ($i -lt $expected.Count) { $expected[$i] } else { '<none>' }
        $a = if ($i -lt $actual.Count) { $actual[$i] } else { '<none>' }
        if ($e -ceq $a) { continue }
        $same = $false
        $pixels = '^(?<label>.* ~px )(?<size>\d+x\d+) sha:[0-9a-f]+ grid:(?<grid>[0-9a-f]+)$'
        $areas = '^area (?<shape>\w+) coords=(?<coords>[\d,]+) (?<rest>.*)$'
        if (-not $loose) {
            # The file may differ, its grid not: GDI+ draws a pixel of the legend's marker one way or another from run
            # to run (measured on .NET Framework: two digests in turn, one pixel apart).
            if ($e -match $pixels) {
                $em = $Matches.Clone()
                $same = $a -match $pixels -and $Matches.label -eq $em.label -and $Matches.size -eq $em.size -and $Matches.grid -eq $em.grid
            }
        }
        else {
            if ($e -match $pixels) {
                $em = $Matches.Clone()
                if ($a -match $pixels -and $Matches.label -eq $em.label -and $Matches.size -eq $em.size) {
                    $sum = 0
                    for ($j = 0; $j -lt $em.grid.Length; $j += 2) { $sum += [Math]::Abs([Convert]::ToInt32($em.grid.Substring($j, 2), 16) - [Convert]::ToInt32($Matches.grid.Substring($j, 2), 16)) }
                    $same = ($sum / ($em.grid.Length / 2)) -le 12
                }
            }
            elseif ($e -match $areas) {
                $em = $Matches.Clone()
                if ($a -match $areas -and $Matches.shape -eq $em.shape -and $Matches.rest -ceq $em.rest) {
                    $ec = $em.coords -split ','; $ac = $Matches.coords -split ','
                    $same = $ec.Count -eq $ac.Count
                    for ($j = 0; $same -and $j -lt $ec.Count; $j++) { $same = [Math]::Abs([int]$ec[$j] - [int]$ac[$j]) -le 6 }
                }
            }
        }
        if (-not $same) { $report += "line $($i + 1)`n  .NET Framework: $e`n  port:           $a" }
    }
    $report
}

# The pictures seen, for a look at them (_chartprobe\pictures-<where>).
function Set-Pictures([string]$where) {
    $script:pictures = Join-Path $work "pictures-$where"
    if (Test-Path $pictures) { [IO.Directory]::Delete($pictures, $true) }
    New-Item -ItemType Directory $pictures -Force | Out-Null
}

function Check([string[]]$lines, [bool]$loose, [string]$where) {
    $lines | Set-Content (Join-Path $work "$where.txt") -Encoding UTF8
    $expected = Get-Content $golden -Encoding UTF8
    $differences = Compare-Lines $expected $lines $loose
    if ($differences) {
        Write-Host "$where`: $(@($differences).Count) line(s) differ from .NET Framework" -ForegroundColor Red
        $differences | ForEach-Object { Write-Host $_ }
        exit 1
    }
    Write-Host "$where`: the same as .NET Framework ($($lines.Count) lines)" -ForegroundColor Green
}

if ($Record) {
    $appcmd = Join-Path $env:SystemRoot 'System32\inetsrv\appcmd.exe'
    $site = Join-Path $work 'iis'
    if (Test-Path $site) { [IO.Directory]::Delete($site, $true) }
    Copy-Item $sample $site -Recurse
    & 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe' (Join-Path $site 'ChartProbe.csproj') /nologo /v:q /p:Configuration=Debug
    if ($LASTEXITCODE -ne 0) { throw 'the sample did not build for .NET Framework' }
    $name = 'frameworkoncore-chartprobe'
    & $appcmd delete site $name 2>&1 | Out-Null
    & $appcmd delete apppool $name 2>&1 | Out-Null
    & $appcmd add apppool /name:$name /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
    & $appcmd add site /name:$name /physicalPath:$site ("/bindings:http/*:{0}:localhost" -f $Port) | Out-Null
    & $appcmd set app "$name/" /applicationPool:$name | Out-Null
    & $appcmd set config "$name/" /section:anonymousAuthentication /userName:"" /commit:apphost | Out-Null
    # The pool's identity reads the site (IIS answers 500.19 otherwise) and writes the handler's and the page's images
    # to ChartImages.
    & icacls $site /grant "IIS AppPool\${name}:(OI)(CI)(RX)" /T /Q | Out-Null
    & icacls (Join-Path $site 'ChartImages') /grant "IIS AppPool\${name}:(OI)(CI)(M)" /T /Q | Out-Null
    try {
        Wait-Site "http://localhost:$Port"
        Set-Pictures 'iis'
        $lines = Probe "http://localhost:$Port"
        $lines | Set-Content $golden -Encoding UTF8
        $lines | ForEach-Object { Write-Host $_ }
        Write-Host "-> $golden (look at it: an error page recorded is what every run is compared with)" -ForegroundColor Yellow
    }
    finally {
        & $appcmd delete site $name 2>&1 | Out-Null
        & $appcmd delete apppool $name 2>&1 | Out-Null
    }
    exit 0
}

# Converted with the converter's default choices, built, with a Dockerfile (for -Linux).
$converted = Join-Path $work 'converted'
# A file just closed may still be held a moment (the virus scanner): tried again.
foreach ($attempt in 1..10) {
    if (-not (Test-Path $converted)) { break }
    try { [IO.Directory]::Delete($converted, $true) } catch { if ($attempt -eq 10) { throw }; Start-Sleep -Seconds 3 }
}
$converter = Join-Path $repo 'src\FrameworkOnCore.Converter\bin\Debug\net10.0\FrameworkOnCore.Converter.dll'
dotnet build (Join-Path $repo 'src\FrameworkOnCore.Converter') --nologo -v q | Out-Null
& dotnet $converter (Join-Path $sample 'ChartProbe.csproj') --out $converted --root $sample --deploy container
if ($LASTEXITCODE -ne 0) { throw 'the conversion failed' }

if ($Windows) {
    $env:ASPNETCORE_URLS = "http://localhost:$Port"
    $process = Start-Process dotnet -ArgumentList 'bin\ChartProbe.dll' -WorkingDirectory $converted -PassThru -WindowStyle Hidden `
        -RedirectStandardOutput (Join-Path $work 'windows-out.log') -RedirectStandardError (Join-Path $work 'windows-err.log')
    try {
        Wait-Site "http://localhost:$Port"
        Set-Pictures 'windows'
        $lines = Probe "http://localhost:$Port"
    }
    # The tree: the application runs in a worker process its process starts (FrameworkOnCore's System.Web).
    finally { taskkill /PID $process.Id /T /F 2>&1 | Out-Null }
    Check $lines $false 'windows'
}

if ($Linux) {
    $ErrorActionPreference = 'Continue'   # docker's stderr is not an error; exit codes are checked
    $image = 'w2l-chartprobe'
    docker build -t $image $converted *> (Join-Path $work 'docker-build.log')
    if ($LASTEXITCODE -ne 0) { throw "docker build failed (see $work\docker-build.log)" }
    cmd /c "docker rm -f $image" 2>&1 | Out-Null
    docker run -d --name $image -p "${Port}:8080" $image | Out-Null
    try {
        Wait-Site "http://localhost:$Port"
        Set-Pictures 'linux'
        $lines = Probe "http://localhost:$Port"
    }
    finally {
        docker logs $image *> (Join-Path $work 'linux-server.log')
        cmd /c "docker rm -f $image" 2>&1 | Out-Null
    }
    Check $lines $true 'linux'
}
