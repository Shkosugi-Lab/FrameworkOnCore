# Records dlinq-sql.golden.json (the SQL .NET Framework's System.Data.Linq generates for
# DataLinq\SqlCases.cs) by building and running record.csproj (net48). Windows only; needs a reachable
# SQL Server (the connection picks the SQL generation mode; 2008+ expected).
param([string]$Connection = 'Data Source=.\SQLEXPRESS;Initial Catalog=master;Integrated Security=True')
$ErrorActionPreference = 'Stop'
dotnet build $PSScriptRoot\record.csproj -c Debug -v q -nologo
if ($LASTEXITCODE -ne 0) { throw 'build failed' }
& "$PSScriptRoot\bin\Debug\net48\record.exe" $Connection "$PSScriptRoot\dlinq-sql.golden.json"
