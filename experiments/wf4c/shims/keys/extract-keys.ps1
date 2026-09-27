# Writes the public keys of .NET Framework's assemblies the shims stand in for, as <token>.snk (public key
# only): the shims are public-signed with them (PublicSign), so that they have the identity the assemblies
# built against .NET Framework reference (a compiler resolves a reference by name and public key token:
# WebFormsMvp's System.Web.Abstractions, 3.5, 31bf3856ad364e35). .NET does not verify the signature.
# Run in Windows PowerShell (.NET Framework):
#   powershell -File experiments\wf4c\shims\keys\extract-keys.ps1
foreach ($name in 'System.Web.Abstractions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35',
                  'System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a') {
    $assembly = [Reflection.Assembly]::Load($name)
    $key = $assembly.GetName().GetPublicKey()
    $token = ($assembly.GetName().GetPublicKeyToken() | ForEach-Object { $_.ToString('x2') }) -join ''
    [IO.File]::WriteAllBytes((Join-Path $PSScriptRoot "$token.snk"), $key)
    "$token : $($key.Length) bytes (from $($assembly.GetName().Name))"
}
