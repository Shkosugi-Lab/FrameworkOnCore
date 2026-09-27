# Logs in to a running openIMIS site (the demo database's Admin, whose password the demo build's login page fills in)
# and requests its main pages as a logged-in user; prints each one's status and title. The pages refuse a request
# without a Referer ("You can't copy and paste or modify URL"): one is sent, as a browser following the menu does.
#   .\experiments\wf4c\imis-login.ps1 -Base http://localhost:5097
#   .\experiments\wf4c\imis-login.ps1 -Base http://localhost:5098     # run-linux-site.ps1 -Keep
param(
    [string]$Base = 'http://localhost:5097',
    [string]$User = 'Admin',
    [string]$Password = 'admin123',
    [string[]]$Paths = @('/Home.aspx', '/FindFamily.aspx', '/FindInsuree.aspx', '/FindPolicy.aspx', '/FindClaims.aspx', '/FindUser.aspx', '/Reports.aspx')
)

$work = Join-Path $PSScriptRoot '_linux\imis-login'
New-Item -ItemType Directory $work -Force | Out-Null
$cookies = Join-Path $work 'cookies.txt'
$page = Join-Path $work 'page.html'
Remove-Item $cookies -ErrorAction SilentlyContinue

function Describe([string]$status) {
    $html = if (Test-Path $page) { Get-Content $page -Raw -Encoding UTF8 } else { '' }
    $title = ([regex]::Match($html, '<title>\s*(.*?)\s*</title>', 'Singleline')).Groups[1].Value
    $problem = ([regex]::Match($html, '(Parser Error Message|Exception Details|Compiler Error Message):(.*?)<br', 'Singleline')).Groups[2].Value -replace '<[^>]+>', ''
    "$status title='$title'$(if ($problem) { " error='$($problem.Trim())'" })"
}

# The first request waits for the site to start (and compile the page).
curl.exe -s -c $cookies -o $page -m 600 --retry 30 --retry-connrefused --retry-delay 2 "$Base/Default.aspx" | Out-Null
$form = Get-Content $page -Raw -Encoding UTF8
function Field([string]$name) { [regex]::Match($form, "name=""$name""[^>]*value=""([^""]*)""").Groups[1].Value }
$fields = [ordered]@{
    '__VIEWSTATE' = (Field '__VIEWSTATE'); '__VIEWSTATEGENERATOR' = (Field '__VIEWSTATEGENERATOR'); '__EVENTVALIDATION' = (Field '__EVENTVALIDATION')
    hfOfflineHFIDFlag = '0'; txtOfflineHF = ''; txtUserName = $User; txtPassword = $Password; btnLogin = 'Login'
}
$body = Join-Path $work 'login.txt'
[IO.File]::WriteAllText($body, (($fields.GetEnumerator() | ForEach-Object { "$($_.Key)=$([uri]::EscapeDataString($_.Value))" }) -join '&'))
Remove-Item $page -ErrorAction SilentlyContinue
$status = curl.exe -s -b $cookies -c $cookies -o $page -m 600 -w '%{http_code} %{redirect_url}' -d "@$body" "$Base/Default.aspx"
"=== login -> $(Describe $status)"
foreach ($path in $Paths) {
    Remove-Item $page -ErrorAction SilentlyContinue
    $status = curl.exe -s -b $cookies -c $cookies -e "$Base/Home.aspx" -o $page -m 600 -w '%{http_code}' "$Base$path"
    "=== $path -> $(Describe $status)"
}
