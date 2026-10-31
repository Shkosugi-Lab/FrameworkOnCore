# Requests a page (keeping cookies) and prints its status, title, text and form fields; posts a form back when
# -Submit names a button (the page's hidden fields - ViewState - included, -Fields overriding).
#   page.ps1 -Url http://localhost:5096/ [-Submit <button name>] [-Fields @{ name = 'value' }] [-TextLength 1500]
param(
    [Parameter(Mandatory = $true)][string]$Url,
    [string]$Submit,
    [hashtable]$Fields = @{},
    [int]$TextLength = 1500,
    [string]$Jar = (Join-Path $env:TEMP 'page.cookies'),
    [string]$Last = (Join-Path $env:TEMP 'page.html')
)
Add-Type -AssemblyName System.Web
function Decode([string]$s) { [System.Web.HttpUtility]::HtmlDecode($s) }

$arguments = @('-s', '-L', '-m', '1800', '--retry', '10', '--retry-connrefused', '--retry-delay', '3', '-c', $Jar, '-b', $Jar, '-o', $Last, '-w', '%{http_code} %{url_effective}')
if ($Submit) {
    # The form of the page requested last: its action and inputs.
    $html = Get-Content $Last -Raw -Encoding UTF8
    $form = [regex]::Match($html, '(?s)<form[^>]*action="([^"]*)"[^>]*>(.*?)</form>')
    $action = Decode $form.Groups[1].Value
    $values = [ordered]@{}
    foreach ($input in [regex]::Matches($form.Groups[2].Value, '<input[^>]*>')) {
        $tag = $input.Value
        $name = [regex]::Match($tag, 'name="([^"]*)"').Groups[1].Value
        if (-not $name) { continue }
        $type = [regex]::Match($tag, 'type="([^"]*)"').Groups[1].Value
        if ($type -in 'submit', 'button', 'image') { continue }
        if ($type -in 'checkbox', 'radio' -and $tag -notmatch 'checked') { continue }
        $values[$name] = Decode ([regex]::Match($tag, 'value="([^"]*)"').Groups[1].Value)
    }
    foreach ($select in [regex]::Matches($form.Groups[2].Value, '(?s)<select[^>]*name="([^"]*)"[^>]*>(.*?)</select>')) {
        $option = [regex]::Match($select.Groups[2].Value, '<option[^>]*selected[^>]*value="([^"]*)"')
        if (-not $option.Success) { $option = [regex]::Match($select.Groups[2].Value, '<option[^>]*value="([^"]*)"') }
        $values[$select.Groups[1].Value] = Decode $option.Groups[1].Value
    }
    foreach ($key in $Fields.Keys) { $values[$key] = $Fields[$key] }
    $values[$Submit] = 'x'
    $body = ($values.Keys | ForEach-Object { [uri]::EscapeDataString($_) + '=' + [uri]::EscapeDataString([string]$values[$_]) }) -join '&'
    $bodyFile = Join-Path $env:TEMP 'page.body'
    [IO.File]::WriteAllText($bodyFile, $body)
    $Url = [uri]::new([uri]$Url, $action).AbsoluteUri
    $arguments += @('--data-binary', "@$bodyFile", '-H', 'Content-Type: application/x-www-form-urlencoded')
}
$status = curl.exe @arguments $Url
$html = ''
if (Test-Path $Last) { $html = [string](Get-Content $Last -Raw -Encoding UTF8) }
$title = [regex]::Match($html, '<title[^>]*>\s*([^<]*?)\s*</title>').Groups[1].Value
"$status title='$(Decode $title)'"
$text = $html
foreach ($pattern in '(?s)<script.*?</script>', '(?s)<style.*?</style>', '<[^>]+>') { $text = [regex]::Replace($text, $pattern, ' ') }
$text = [regex]::Replace((Decode $text), '\s+', ' ').Trim()
$text.Substring(0, [Math]::Min($TextLength, $text.Length))
foreach ($input in [regex]::Matches($html, '<(input|select|textarea)[^>]*>')) {
    $tag = $input.Value
    $type = [regex]::Match($tag, 'type="([^"]*)"').Groups[1].Value
    if ($type -eq 'hidden') { continue }
    '  field: ' + [regex]::Match($tag, 'name="([^"]*)"').Groups[1].Value + " ($type) value='" + (Decode ([regex]::Match($tag, 'value="([^"]*)"').Groups[1].Value)) + "'"
}
