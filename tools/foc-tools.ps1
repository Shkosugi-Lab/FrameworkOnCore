# The folder of Studio and the converter for this checkout (dot-sourced by studio.ps1 and converter.ps1): the build of
# its src/ that GitHub Actions published (frameworkoncore-tools.yml: the release frameworkoncore-tools, the asset of the last
# commit that changed src/), fetched once into %LOCALAPPDATA%\FrameworkOnCore\tools\<commit>. Built here instead
# (dotnet build, as before) when src/ has changes not committed, when the commit has no published build (not pushed, or
# the workflow has not finished), or with -Build.
#
# The repository is public: the asset is fetched as it is. A private one's through the API, with the token of GH_TOKEN,
# GITHUB_TOKEN or the GitHub CLI (gh auth token), as the converter fetches FrameworkOnCore's packages.

function Get-FocTools([switch]$Build) {
    $repo = Split-Path $PSScriptRoot -Parent
    $reason = $null
    $commit = $null
    if ($Build) { $reason = '-Build' }
    elseif (-not (Get-Command git -ErrorAction SilentlyContinue)) { $reason = 'git is not there' }
    else {
        $commit = git -C $repo log -1 --format=%H -- src 2>$null
        if (-not $commit) { $reason = 'not a git checkout' }
        elseif (git -C $repo status --porcelain -- src) { $reason = 'src/ has changes not committed' }
    }

    if (-not $reason) {
        $tools = Join-Path $env:LOCALAPPDATA "FrameworkOnCore\tools\$($commit.Substring(0, 12))"
        if (Test-Path (Join-Path $tools 'FrameworkOnCore.Studio.dll')) { return $tools }
        $asset = "frameworkoncore-tools-$($commit.Substring(0, 12)).zip"
        $slug = 'Shkosugi-Lab/FrameworkOnCore'
        if ((git -C $repo remote get-url origin 2>$null) -match 'github\.com[/:]([^/]+/[^/.]+)') { $slug = $Matches[1] }
        $zip = Join-Path ([IO.Path]::GetTempPath()) "$asset.$PID"
        try {
            Write-Host "fetching Studio and the converter (src/ at $($commit.Substring(0, 12))): $asset"
            Save-FocAsset $slug 'frameworkoncore-tools' $asset $zip
            # Extracted beside, then moved in: a fetch cut short leaves no half a folder.
            $staging = "$tools.download"
            if (Test-Path $staging) { [IO.Directory]::Delete($staging, $true) }
            Add-Type -AssemblyName System.IO.Compression.FileSystem
            [IO.Compression.ZipFile]::ExtractToDirectory($zip, $staging)
            [IO.Directory]::Move($staging, $tools)
            # The older builds go (the five latest are kept).
            Get-ChildItem (Split-Path $tools -Parent) -Directory | Where-Object { $_.Name -notlike '*.download' } |
                Sort-Object LastWriteTime -Descending | Select-Object -Skip 5 | ForEach-Object { [IO.Directory]::Delete($_.FullName, $true) }
            return $tools
        }
        catch { $reason = "no published build of it ($($_.Exception.Message))" }
        finally { if (Test-Path $zip) { [IO.File]::Delete($zip) } }
    }

    Write-Host "building Studio and the converter here: $reason"
    dotnet build (Join-Path $repo 'src\FrameworkOnCore.Studio') -nologo -v q | Out-Host
    if ($LASTEXITCODE -ne 0) { throw 'the build of Studio failed' }
    return (Join-Path $repo 'src\FrameworkOnCore.Studio\bin\Debug\net10.0')
}

# A release asset: as it is (a public repository), or through the API with a token (a private one).
function Save-FocAsset([string]$slug, [string]$tag, [string]$name, [string]$path) {
    $headers = @{ 'User-Agent' = 'FrameworkOnCore' }
    try {
        Invoke-WebRequest "https://github.com/$slug/releases/download/$tag/$name" -OutFile $path -UseBasicParsing -Headers $headers
        return
    }
    catch {
        if ($_.Exception.Response -and [int]$_.Exception.Response.StatusCode -ne 404) { throw }
    }
    $token = @($env:GH_TOKEN, $env:GITHUB_TOKEN) | Where-Object { $_ } | Select-Object -First 1
    if (-not $token -and (Get-Command gh -ErrorAction SilentlyContinue)) { $token = gh auth token 2>$null }
    if (-not $token) { throw "$name is not there (or the repository is private: gh auth login, or GH_TOKEN)" }
    $headers['Authorization'] = "Bearer $token"
    $release = Invoke-RestMethod "https://api.github.com/repos/$slug/releases/tags/$tag" -Headers $headers
    $found = $release.assets | Where-Object { $_.name -eq $name } | Select-Object -First 1
    if (-not $found) { throw "$name is not there" }
    $headers['Accept'] = 'application/octet-stream'
    Invoke-WebRequest "https://api.github.com/repos/$slug/releases/assets/$($found.id)" -OutFile $path -UseBasicParsing -Headers $headers
}
