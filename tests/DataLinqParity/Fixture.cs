using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FrameworkOnCore.DataLinqParity
{
    /// <summary>
    /// The database the cases run on (FocDataLinqParity on the given server): made once per run with
    /// DataContext.CreateDatabase (the API under test; the database cases show its DDL) and the routines, then reset to
    /// the same rows before each database case, with plain ADO.NET (not the API under test). Identity columns are
    /// reseeded (the next OrderID is 101, the next Note Id 11) so generated keys are the same in every run.
    /// </summary>
    public static class Fixture
    {
        public const string Database = "FocDataLinqParity";
        public static string Connection { get; private set; }

        public static readonly DateTime Day = new DateTime(2020, 1, 15, 10, 30, 0);
        public static readonly DateTimeOffset Stamp = new DateTimeOffset(2020, 1, 15, 10, 30, 0, TimeSpan.FromHours(9));
        public static readonly Guid Hammer = new Guid("11111111-0000-0000-0000-000000000001");
        public static readonly Guid Kite = new Guid("11111111-0000-0000-0000-000000000002");
        public static readonly Guid Atlas = new Guid("11111111-0000-0000-0000-000000000003");

        /// <summary>Creates the database afresh on the server of <paramref name="server"/> (a connection string).</summary>
        public static void Create(string server)
        {
            var builder = new SqlConnectionStringBuilder(server) { InitialCatalog = Database };
            Connection = builder.ConnectionString;
            using (var master = new SqlConnection(new SqlConnectionStringBuilder(server) { InitialCatalog = "master" }.ConnectionString))
            {
                master.Open();
                Execute(master, $"IF DB_ID('{Database}') IS NOT NULL BEGIN ALTER DATABASE [{Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{Database}] END");
            }
            SqlConnection.ClearAllPools();
            using (var db = new ParityDb(Connection)) db.CreateDatabase();
            using (var connection = Open())
                foreach (var routine in ParityDb.Routines) Execute(connection, routine);
        }

        public static SqlConnection Open()
        {
            var connection = new SqlConnection(Connection);
            connection.Open();
            return connection;
        }

        /// <summary>The same rows before every database case.</summary>
        public static void Reset()
        {
            using (var c = Open())
            {
                Execute(c, "DELETE FROM dbo.Orders; DELETE FROM dbo.Customers; DELETE FROM dbo.Products; DELETE FROM dbo.People; DELETE FROM dbo.Notes");
                Execute(c, "INSERT dbo.Customers (CustomerID, CompanyName, City) VALUES " +
                           "(N'ALFKI', N'Alfreds', N'Berlin'), (N'ANTON', N'Antonio', N'Mexico'), (N'BONAP', N'Bon app', N'Marseille'), (N'CHOPS', N'Chop-suey', NULL)");
                Execute(c, "SET IDENTITY_INSERT dbo.Orders ON; INSERT dbo.Orders (OrderID, CustomerID, OrderDate, ShippedDate, Stamp, Amount) VALUES " +
                           "(1, N'ALFKI', '2020-01-15T10:30:00', '2020-01-20T09:00:00', '2020-01-15T10:30:00+09:00', 100.50), " +
                           "(2, N'ALFKI', '2020-02-01T00:00:00', NULL, '2020-02-01T00:00:00+09:00', 20.00), " +
                           "(3, N'ANTON', '2019-12-31T23:59:59', '2020-01-02T00:00:00', '2019-12-31T23:59:59+00:00', 300.00), " +
                           "(4, NULL, '2020-03-01T12:00:00', NULL, '2020-03-01T12:00:00-05:00', 5.25); SET IDENTITY_INSERT dbo.Orders OFF; " +
                           "DBCC CHECKIDENT ('dbo.Orders', RESEED, 100) WITH NO_INFOMSGS");
                Execute(c, "INSERT dbo.Products (ProductID, Name, Kind, Data, Weight, Discontinued, Description) VALUES " +
                           $"('{Hammer}', N'Hammer', 1, 0x010203, 1.5, 0, N'A hammer'), ('{Kite}', N'Kite', 2, NULL, 0.25, 1, N'A kite'), ('{Atlas}', N'Atlas', 3, 0x, 2, 0, NULL)");
                Execute(c, "INSERT dbo.People (PersonID, Kind, Name, Salary, Reports) VALUES " +
                           "(1, N'P', N'Pat', NULL, NULL), (2, N'E', N'Eve', 5000.00, NULL), (3, N'M', N'Max', 9000.00, 2), (4, N'X', N'Xan', NULL, NULL)");
                Execute(c, "SET IDENTITY_INSERT dbo.Notes ON; INSERT dbo.Notes (Id, Text) VALUES (1, N'first'), (2, N'second'); SET IDENTITY_INSERT dbo.Notes OFF; " +
                           "DBCC CHECKIDENT ('dbo.Notes', RESEED, 10) WITH NO_INFOMSGS");
            }
        }

        public static void Execute(SqlConnection connection, string sql)
        {
            using (var command = new SqlCommand(sql, connection)) command.ExecuteNonQuery();
        }

        /// <summary>A query's rows as text (the database's state, read without the API under test).</summary>
        public static List<string> Rows(string sql)
        {
            var rows = new List<string>();
            using (var c = Open())
            using (var command = new SqlCommand(sql, c))
            using (var reader = command.ExecuteReader())
                while (reader.Read())
                    rows.Add(string.Join("|", Enumerable.Range(0, reader.FieldCount).Select(i => reader.IsDBNull(i) ? "null" : Probe.Format(reader.GetValue(i)))));
            return rows;
        }
    }

    /// <summary>Runs the cases: every [Case] method of this assembly, by name, one after another.</summary>
    public static class Runner
    {
        public static IEnumerable<MethodInfo> Cases() =>
            typeof(Runner).Assembly.GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                .Where(m => m.GetCustomAttribute<CaseAttribute>() != null)
                .OrderBy(Name, StringComparer.Ordinal);

        public static string Name(MethodInfo method) => method.DeclaringType.Name + "." + method.Name;

        public static bool NeedsDatabase(MethodInfo method) => method.GetCustomAttribute<CaseAttribute>().Database;

        /// <summary>One case's lines (the database already created by Fixture.Create when it needs one).</summary>
        public static IReadOnlyList<string> Run(MethodInfo method)
        {
            var probe = new Probe();
            var culture = CultureInfo.CurrentCulture;
            var uiCulture = CultureInfo.CurrentUICulture;
            try
            {
                // One culture for both runs: messages and formatting the runtime words by culture.
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
                if (NeedsDatabase(method)) Fixture.Reset();
                method.Invoke(null, new object[] { probe });
            }
            catch (Exception e) { probe.Escaped(e); }
            finally { CultureInfo.CurrentCulture = culture; CultureInfo.CurrentUICulture = uiCulture; }
            return probe.Lines;
        }
    }
}
