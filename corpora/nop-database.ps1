# Puts nopCommerce 1.90's database back to a known state: the one every run of nop's operations suite starts from
# (record-nop-operations.ps1 on IIS, verify-windows.ps1 -Name nop -Suite operations, run-linux.ps1 ... -Suite operations).
# nopCommerce keeps everything in SQL Server (the catalogue, the customers, the carts, the orders, the settings, the
# pictures, the log): a run's operations change it, and the next run would start from what the last one left.
#
# The known state is what the application's installer (install/install.aspx: "create a new database", "create sample
# data") makes, made here without the installer: the same three scripts of the original (install/Scripts/
# nopCommerce_createDatabase.sql, nopCommerce_createData.sql, nopCommerce_createSampleData.sql), split at their GO lines
# and run one statement at a time as the installer does (InstallPage.proceedSQLScripts), then the version the installer
# sets (Common.CurrentVersion = 1.90). Then what the runs need to be repeatable, each one an update of the sample data:
#   - the e-mail accounts' SMTP server 127.0.0.1:9 (a closed port: the queued e-mails' sending - the Emails task, every
#     minute - fails at once on every machine, not on the network of the day; the sample's smtp.mail.com is a real host);
#   - the store's URL (Common.StoreURL) http://127.0.0.1:9/: the KeepAlive task asks it every minute (the sample's
#     www.youstore.com is on the Internet), and it is written in the e-mails' bodies (the message queue is compared).
# The database is then backed up once on that server (a file in its default backup folder, its name with a hash of this
# script's setup: a new setup makes a new one), and every later call restores the backup.
#
#   .\corpora\nop-database.ps1                                   # .\SQLEXPRESS, Windows authentication
#   .\corpora\nop-database.ps1 -Server 127.0.0.1,14333 -User sa -Password ...   # the Linux runs' SQL Server container
#   .\corpora\nop-database.ps1 -Rebuild                          # create it from the scripts again (a new backup)
#   .\corpora\nop-database.ps1 -Drop                             # remove the database (the backup stays)
param([string]$Server = '.\SQLEXPRESS', [string]$User, [string]$Password, [string]$Database = 'nopCommerceOps',
    [switch]$Rebuild, [switch]$Drop)

$ErrorActionPreference = 'Stop'
$scripts = Join-Path $PSScriptRoot 'work\nopCommerce-release-1.90\NopCommerceStore\install\Scripts'
$setup = @(
    "UPDATE [Nop_Setting] SET [Value] = '1.90' WHERE [Name] = 'Common.CurrentVersion'",
    "UPDATE [Nop_EmailAccount] SET [Host] = '127.0.0.1', [Port] = 9",
    "UPDATE [Nop_Setting] SET [Value] = 'http://127.0.0.1:9/' WHERE [Name] = 'Common.StoreURL'"
)

function Connection([string]$catalog) {
    $builder = New-Object Data.SqlClient.SqlConnectionStringBuilder
    $builder['Data Source'] = $Server
    $builder['Initial Catalog'] = $catalog
    if ($User) { $builder['User ID'] = $User; $builder['Password'] = $Password } else { $builder['Integrated Security'] = $true }
    $builder['Connect Timeout'] = 30
    $connection = New-Object Data.SqlClient.SqlConnection $builder.ConnectionString
    $connection.Open()
    return $connection
}

function Invoke-Sql([string]$sql, [string]$catalog = 'master', [switch]$Scalar) {
    $connection = Connection $catalog
    try {
        $command = $connection.CreateCommand()
        $command.CommandText = $sql
        $command.CommandTimeout = 600
        if ($Scalar) { return $command.ExecuteScalar() }
        $command.ExecuteNonQuery() | Out-Null
    }
    finally { $connection.Dispose() }
}

# The installer's reading of a script: the statements between the lines that are GO.
function Invoke-Script([string]$file) {
    $connection = Connection $Database
    try {
        $reader = New-Object IO.StreamReader $file
        $statement = New-Object Text.StringBuilder
        $count = 0
        while ($true) {
            $line = $reader.ReadLine()
            if ($line -eq $null -or $line.TrimEnd().ToUpperInvariant() -eq 'GO') {
                if ($statement.Length -gt 0 -and $statement.ToString().Trim()) {
                    $command = $connection.CreateCommand()
                    $command.CommandText = $statement.ToString()
                    $command.CommandTimeout = 600
                    $command.ExecuteNonQuery() | Out-Null
                    $count++
                }
                $statement.Length = 0
                if ($line -eq $null) { break }
                continue
            }
            [void]$statement.Append($line).Append("`r`n")
        }
        $reader.Dispose()
        Write-Host "  $(Split-Path $file -Leaf): $count statements"
    }
    finally { $connection.Dispose() }
}

$dropSql = "IF DB_ID(N'$Database') IS NOT NULL BEGIN ALTER DATABASE [$Database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$Database]; END"
if ($Drop) { Invoke-Sql $dropSql; Write-Host "dropped: $Database ($Server)"; exit 0 }

# The backup's name: the scripts' and the setup's hash.
$sha = [Security.Cryptography.SHA256]::Create()
$material = ($setup -join "`n") + "`n" + (@('nopCommerce_createDatabase.sql', 'nopCommerce_createData.sql', 'nopCommerce_createSampleData.sql') | ForEach-Object { (Get-Item (Join-Path $scripts $_)).Length }) -join ','
$hash = -join ($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($material))[0..3] | ForEach-Object { $_.ToString('x2') })
$folder = [string](Invoke-Sql "SELECT CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS nvarchar(4000))" -Scalar)
$separator = if ($folder.StartsWith('/')) { '/' } else { '\' }
$backup = $folder.TrimEnd('/', '\') + $separator + "$Database-$hash.bak"
$exists = [int](Invoke-Sql "DECLARE @r TABLE (a int, b int, c int); INSERT @r EXEC master.dbo.xp_fileexist N'$backup'; SELECT a FROM @r" -Scalar)

if ($Rebuild -or $exists -ne 1) {
    Write-Host "creating $Database on $Server from the installer's scripts (once; a few minutes)"
    Invoke-Sql $dropSql
    Invoke-Sql "CREATE DATABASE [$Database]"
    foreach ($file in 'nopCommerce_createDatabase.sql', 'nopCommerce_createData.sql', 'nopCommerce_createSampleData.sql') {
        Invoke-Script (Join-Path $scripts $file)
    }
    foreach ($statement in $setup) { Invoke-Sql $statement $Database }
    Invoke-Sql "BACKUP DATABASE [$Database] TO DISK = N'$backup' WITH INIT, FORMAT"
    Write-Host "backup: $backup"
}
# The known state: the backup restored (whatever a run left is replaced).
Invoke-Sql "IF DB_ID(N'$Database') IS NOT NULL ALTER DATABASE [$Database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; RESTORE DATABASE [$Database] FROM DISK = N'$backup' WITH REPLACE; ALTER DATABASE [$Database] SET MULTI_USER;"
Write-Host "restored: $Database ($Server) from $backup"
exit 0
