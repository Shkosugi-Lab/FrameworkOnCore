# 変換前の WebForms アプリを、Visual Studio 無しでビルドする。
#
# なぜ要るか
# ----------
# このプロジェクトの最終ゲートは ParityTest — 変換前アプリの実際の描画と照合する —
# で、その正解は samples\ の 4 本にしかありません。6 本のコーパスには 1 本も無く、
# つまり「BlogEngine はビルドエラー 0」は「コンパイルが通る」であって
# 「元と同じ挙動」ではありません。0 になっても挙動については何も言えません。
#
# 採取するには旧アプリを動かす必要があり、その前に旧アプリをビルドする必要があります。
# 「Visual Studio Build Tools が要る」と思われていましたが、要りません。必要なものは
# すべて NuGet と Windows 同梱の .NET Framework から揃います。
#
#   .NET Framework の MSBuild   C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe
#   v4.8 参照アセンブリ          NuGet: Microsoft.NETFramework.ReferenceAssemblies.net48
#   C# 6 以降のコンパイラ        NuGet: Microsoft.Net.Compilers
#                               (同梱の csc は C# 5 までで、$"..." が CS1056 になります)
#   packages.config の復元       nuget.exe(dotnet restore は packages.config を扱いません)
#
# 使い方
#   .\corpora\build-original.ps1 -Only be
#
# 実測: BlogEngine.NET 3.3.8 はこの手順で bin\BlogEngine.NET.dll までビルドできます。
param(
    [string[]]$Only,
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Continue'
$repo = Split-Path $PSScriptRoot -Parent
$legacy = Join-Path $repo 'tools\legacy'
$packages = Join-Path $legacy 'packages'

# 対象は「元がビルドできることを実測したもの」だけ。足すときは実際に通してから。
$targets = @(
    @{ Name = 'be'
       Solution = 'BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.sln'
       Project = 'BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj' }
)

if ($Only) {
    $targets = $targets | Where-Object { $Only -contains $_.Name }
    if (-not $targets) {
        Write-Error ("-Only に一致する対象がありません。指定可能: " + (($targets | ForEach-Object { $_.Name }) -join ', '))
    }
}

$msbuild = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe'
if (-not (Test-Path $msbuild)) {
    Write-Error ".NET Framework の MSBuild がありません: $msbuild"
    exit 1
}

New-Item -ItemType Directory -Force $legacy | Out-Null

$nuget = Join-Path $legacy 'nuget.exe'
if (-not (Test-Path $nuget)) {
    Write-Host '=== nuget.exe を取得 ==='
    Invoke-WebRequest -Uri 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe' `
        -OutFile $nuget -UseBasicParsing
}

# バージョンは固定。ビルドに使う道具が黙って変わると、採取した正解が何で作られたのか
# 後から言えなくなります。
$referenceAssemblies = 'Microsoft.NETFramework.ReferenceAssemblies.net48'
$referenceVersion = '1.0.3'
$compilers = 'Microsoft.Net.Compilers'
$compilersVersion = '3.11.0'

foreach ($pair in @(@($referenceAssemblies, $referenceVersion), @($compilers, $compilersVersion))) {
    $dir = Join-Path $packages ("{0}.{1}" -f $pair[0], $pair[1])
    if (-not (Test-Path $dir)) {
        Write-Host ("=== {0} {1} を取得 ===" -f $pair[0], $pair[1])
        & $nuget install $pair[0] -Version $pair[1] -OutputDirectory $packages -NonInteractive | Out-Null
    }
}

$refRoot = Join-Path $packages ("{0}.{1}\build\" -f $referenceAssemblies, $referenceVersion)
$cscPath = Join-Path $packages ("{0}.{1}\tools" -f $compilers, $compilersVersion)

$failures = @()

foreach ($target in $targets) {
    $name = $target.Name
    Write-Host ""
    Write-Host ("=== {0} ===" -f $name)

    $solution = Join-Path $PSScriptRoot ("work\" + $target.Solution)
    $project = Join-Path $PSScriptRoot ("work\" + $target.Project)
    if (-not (Test-Path $project)) {
        Write-Warning "  取得されていません: $project(先に fetch.ps1 を実行してください)"
        $failures += "${name}: ソースなし"
        continue
    }

    # packages.config を 1 つずつ。ソリューションを渡すと、PackageReference 形式の
    # プロジェクト(BlogEngine なら Tests)を同梱 MSBuild 4.0 が読めず、復元自体は
    # 成功しているのに MSB4066 を吐きます。読む人には失敗に見えるので、そこは通さない。
    Write-Host '  パッケージを復元'
    $solutionDirectory = Split-Path $solution -Parent
    $packagesDirectory = Join-Path $solutionDirectory 'packages'
    foreach ($config in Get-ChildItem $solutionDirectory -Recurse -Filter packages.config -File) {
        & $nuget restore $config.FullName -PackagesDirectory $packagesDirectory -NonInteractive | Out-Null
    }

    # TargetFrameworkVersion を v4.8 に寄せています。元は v4.5 で、その参照アセンブリは
    # この環境にありません。v4.8 は上位互換なので、旧アプリのコードはそのまま通ります。
    #
    # VSToolsPath を空にすると Microsoft.WebApplication.targets のインポートが条件で
    # 飛びます。あれが要るのは発行のときで、ビルドには要りません。
    Write-Host '  ビルド'
    & $msbuild $project `
        "/p:Configuration=$Configuration" `
        /p:TargetFrameworkVersion=v4.8 `
        "/p:TargetFrameworkRootPath=$refRoot" `
        "/p:CscToolPath=$cscPath" `
        /p:CscToolExe=csc.exe `
        /p:OutputPath=bin\ `
        /p:VSToolsPath= `
        /v:m /nologo | Out-Null

    if ($LASTEXITCODE -ne 0) {
        Write-Warning "  ビルドに失敗しました"
        $failures += "${name}: ビルド失敗"
        continue
    }

    $assembly = Join-Path (Split-Path $project -Parent) 'bin'
    $built = @(Get-ChildItem $assembly -Filter *.dll -ErrorAction SilentlyContinue).Count
    Write-Host ("  成功: {0} に {1} 個のアセンブリ" -f $assembly, $built) -ForegroundColor Green
}

Write-Host ""
if ($failures.Count -gt 0) {
    Write-Host '要確認:' -ForegroundColor Red
    foreach ($failure in $failures) { Write-Host "  - $failure" -ForegroundColor Red }
    exit 1
}

Write-Host 'ビルドできました。' -ForegroundColor Green
Write-Host ''
Write-Host '次は採取ですが、そこには WebForms を動かすホストが要ります。' -ForegroundColor Yellow
Write-Host 'IIS Express がある環境なら README の samples の手順がそのまま使えます。' -ForegroundColor Yellow
Write-Host '無い環境での試みと、どこで止まったかは corpora\README.md に記録してあります。' -ForegroundColor Yellow
exit 0
