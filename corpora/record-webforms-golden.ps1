# 変換前の WebForms アプリを IIS で動かし、その実描画をゴールデンマスターとして採る。
#
# なぜ要るか
# ----------
# 変換後のアプリが元と同じに動くかは、ParityTest で変換前アプリの実際の描画と照合して
# 確かめる(experiments\wf4c\run-linux.ps1 -Golden)。「ビルドエラー 0」は「コンパイルが
# 通る」であって「元と同じ挙動」ではなく、描画の違いは旧アプリの実描画と突き合わせる
# 以外に検出手段がない。その正解(corpora\parity\*.golden-webforms.json)をここで採る。
#
# なぜ IIS Express ではないか
# ---------------------------
# 単体インストーラは配布が Visual Studio 同梱に寄っていて、この環境にはありません。
# Windows 同梱の IIS は機能として入っており、同じ System.Web パイプラインを動かします。
# 手製のホストで採る道は採りません —— 手製ホストで採った正解は「WebForms の描画」
# ではなく「そのホストの癖」を記録するためです。
#
# 使い方
#   .\corpora\record-webforms-golden.ps1 -Only be
#   .\corpora\record-webforms-golden.ps1 -Only be -KeepSite   # 調査用に残す
param(
    [string[]]$Only,
    [switch]$KeepSite
)

$ErrorActionPreference = 'Continue'
$repo = Split-Path $PSScriptRoot -Parent
$parityDir = Join-Path $PSScriptRoot 'parity'

# 旧アプリを動かせると実測できたものだけ。足すときは実際に採ってから。
#
# mojo / yaf / dnn はデータベースが要るのでまだ入っていません。
#
# wt の ConnectionString について
# ------------------------------
# 元の Web.config が指すのは実在しない Azure SQL です
# (ms.database.windows.net、資格情報も uid1 / P@123 と伏せ字)。
# 匿名化されたサンプルで、そのままでは Application_Start の
# roleActions.AddUserAndRole() が到達しないホストを待ち続け、アプリが起動しません。
#
# ここで与えているのは「元アプリが本来つながるはずだったデータベース」です。
# WingtipToys は EF Code First なので、接続さえできればスキーマもシードも自分で作ります。
#
# 「起動させるために接続文字列を書き換えて、つながらないことにする」のとは別物です。
# そちらをやると、採れたものはもう変換前アプリの実描画ではなくなります。
#
# LocalDB ではなく SQL Server Express を使う理由
# ----------------------------------------------
# LocalDB のインスタンスは【ユーザーごと】です。変換前アプリは IIS のアプリプール ID で
# 動き、変換後アプリは dotnet run、つまり別のユーザーで動きます。LocalDB だと同じ接続
# 文字列を書いても【見ているデータベースが違い】、照合が成立しません
# (実測: 元アプリが作った WingtipToys が admin 側の LocalDB には無かった)。
#
# Express はサービスとして動くのでユーザーに依存しません。両方が同じデータベースを見ます。
$targets = @(
    @{ Name = 'be'
       Path = 'BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET'
       Port = 8091 },

    @{ Name = 'wt'
       Path = 'wingtiptoys-master\WingtipToys\WingtipToys'
       Port = 8092
       ConnectionString = 'Data Source=.\SQLEXPRESS;Initial Catalog=WingtipToys;Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=30'
       SqlLogin = $true }
)

if ($Only) {
    $targets = $targets | Where-Object { $Only -contains $_.Name }
    if (-not $targets) { Write-Error "-Only に一致する対象がありません。指定可能: be, wt"; exit 1 }
}

$appcmd = Join-Path $env:SystemRoot 'System32\inetsrv\appcmd.exe'
if (-not (Test-Path $appcmd)) {
    Write-Error @"
IIS がありません($appcmd)。
有効化:
  Enable-WindowsOptionalFeature -Online -All -NoRestart -FeatureName `
    IIS-WebServerRole,IIS-ASPNET45,IIS-NetFxExtensibility45,IIS-ISAPIExtensions,IIS-ISAPIFilter
"@
    exit 1
}

$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'
if (-not (Test-Path $verifier)) {
    Write-Host '=== ParityTest のビルド ==='
    dotnet build (Join-Path $repo 'tools\FrameworkOnCore.ParityTest') `
        -o (Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt') --nologo -v q | Out-Null
}

New-Item -ItemType Directory -Force $parityDir | Out-Null
$failures = @()

foreach ($target in $targets) {
    $name = $target.Name
    $siteName = "frameworkoncore-$name"
    $poolName = "frameworkoncore-$name"
    $physical = Join-Path $PSScriptRoot ("work\" + $target.Path)
    $scenario = Join-Path $PSScriptRoot "regression\$name.scenario.json"
    $golden = Join-Path $parityDir "$name.golden-webforms.json"

    Write-Host ""
    Write-Host ("=== {0} ===" -f $name)

    if (-not (Test-Path $scenario)) {
        Write-Warning "  シナリオがありません: $scenario"
        $failures += "${name}: シナリオなし"
        continue
    }

    Write-Host '  変換前アプリをビルド'
    & (Join-Path $PSScriptRoot 'build-original.ps1') -Only $name | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "  ビルドに失敗しました"
        $failures += "${name}: ビルド失敗"
        continue
    }

    # 前回の残骸があると、古いパスのサイトに対して採ってしまう
    & $appcmd delete site $siteName 2>&1 | Out-Null
    & $appcmd delete apppool $poolName 2>&1 | Out-Null

    # 元アプリが本来つながるはずだったデータベースを与えます(上の targets のコメント)。
    if ($target.ConnectionString) {
        $webConfig = Join-Path $physical 'Web.config'
        $text = Get-Content $webConfig -Raw
        $replaced = [regex]::Replace($text,
            'connectionString="Server=tcp:ms\.database\.windows\.net[^"]*"',
            'connectionString="' + $target.ConnectionString.Replace('$', '$$') + '"')
        if ($replaced -ne $text) {
            Set-Content $webConfig -Value $replaced -Encoding utf8
            Write-Host '  接続文字列を SQL Server Express に向けました'
        }
    }

    Write-Host '  IIS サイトを作成'
    & $appcmd add apppool /name:$poolName /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null

    # LocalDB はユーザー単位のインスタンスです。アプリプール ID に自分のプロファイルを
    # 持たせないと、(localdb)\MSSQLLocalDB を自動生成できず接続できません。
    & $appcmd set apppool $poolName `
        /processModel.loadUserProfile:true /processModel.setProfileEnvironment:true | Out-Null

    & $appcmd add site /name:$siteName /physicalPath:$physical `
        ("/bindings:http/*:{0}:localhost" -f $target.Port) | Out-Null
    & $appcmd set app "$siteName/" /applicationPool:$poolName | Out-Null

    # 匿名要求は既定で IUSR として動きます。下で権限を与えるのはアプリプール ID の
    # ほうなので、そのままだと読み取れず 401 になります(wt で実測)。空文字を指定すると
    # 匿名要求もアプリプール ID で動き、権限が 1 か所で揃います。
    & $appcmd set config "$siteName/" /section:anonymousAuthentication `
        /userName:"" /commit:apphost | Out-Null

    # アプリプール ID で SQL Server に入れるようにします。dbcreator なのは、
    # EF Code First の初期化子がデータベース自体を作るためです。
    # 仮想アカウント "IIS APPPOOL\<pool>" はアプリプールを作った後でなければ解決できません。
    if ($target.SqlLogin) {
        $login = "IIS APPPOOL\$poolName"
        $sql = @"
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'$login')
    CREATE LOGIN [$login] FROM WINDOWS;
ALTER SERVER ROLE [dbcreator] ADD MEMBER [$login];
ALTER SERVER ROLE [sysadmin] ADD MEMBER [$login];
"@
        try {
            $connection = New-Object System.Data.SqlClient.SqlConnection `
                'Server=.\SQLEXPRESS;Integrated Security=true;Connect Timeout=30'
            $connection.Open()
            $command = $connection.CreateCommand()
            $command.CommandText = $sql
            $command.ExecuteNonQuery() | Out-Null
            $connection.Close()
            Write-Host "  SQL ログインを用意しました: $login"
        }
        catch {
            Write-Warning "  SQL ログインを作成できません: $($_.Exception.Message)"
            $failures += "${name}: SQL ログイン作成失敗"
            continue
        }
    }

    # BlogEngine は App_Data に書きます(設定・キャッシュ)。読み取り専用だと
    # 初回要求で落ち、そのエラーページがゴールデンマスターとして記録されます。
    $identity = "IIS AppPool\$poolName"
    & icacls $physical /grant "${identity}:(OI)(CI)(M)" /T /Q 2>&1 | Out-Null

    try {
        $url = "http://localhost:$($target.Port)/"
        $ready = $false
        foreach ($attempt in 1..30) {
            try {
                $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 10
                if ($response.StatusCode -eq 200) { $ready = $true; break }
            } catch { Start-Sleep -Seconds 2 }
        }
        if (-not $ready) {
            Write-Warning "  変換前アプリが応答しません: $url"
            $failures += "${name}: 起動失敗"
            continue
        }

        Write-Host "  採取: $golden"
        & dotnet $verifier record --url $url --scenario $scenario --out $golden
        if ($LASTEXITCODE -ne 0) {
            $failures += "${name}: 採取に失敗"
        }
    }
    finally {
        if (-not $KeepSite) {
            & $appcmd delete site $siteName 2>&1 | Out-Null
            & $appcmd delete apppool $poolName 2>&1 | Out-Null
        }
    }
}

Write-Host ""
if ($failures.Count -gt 0) {
    Write-Host '要確認:' -ForegroundColor Red
    foreach ($failure in $failures) { Write-Host "  - $failure" -ForegroundColor Red }
    exit 1
}

Write-Host '採取しました。' -ForegroundColor Green
Write-Host '中身を必ず目で確認してください。エラーページを 200 で記録していても' -ForegroundColor Yellow
Write-Host 'ここは成功と言います。以後すべての照合がそれを正解として測ります。' -ForegroundColor Yellow
exit 0
