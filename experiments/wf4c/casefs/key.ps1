# The key of libfoccase.so's build: what it is built from (its sources, its build script and image), 12 hex characters of
# their SHA-256. The GitHub Release frameworkoncore-casefs has a build per key (libfoccase-<key>.zip,
# frameworkoncore-casefs.yml), which the converter fetches into out/ (RuntimeSetup.EnsureCaseLibraries computes the same
# key: each file's name, a line feed, its bytes without carriage returns, a line feed; the files in this order).
#   .\experiments\wf4c\casefs\key.ps1
$bytes = New-Object System.Collections.Generic.List[byte]
foreach ($name in 'Dockerfile.build', 'build.sh', 'casemap.h', 'foccase.c') {
    $bytes.AddRange([Text.Encoding]::UTF8.GetBytes("$name`n"))
    $bytes.AddRange([byte[]]@([IO.File]::ReadAllBytes((Join-Path $PSScriptRoot $name)) | Where-Object { $_ -ne 13 }))
    $bytes.Add(10)
}
$hash = [Security.Cryptography.SHA256]::Create().ComputeHash($bytes.ToArray())
(-join ($hash | ForEach-Object { $_.ToString('x2') })).Substring(0, 12)
