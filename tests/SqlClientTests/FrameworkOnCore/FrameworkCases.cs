// FrameworkOnCore: what the port gives back of .NET Framework 4.8's System.Data.SqlClient (the Network Library, Asynchronous
// Processing, Connection Reset, Context Connection and TransparentNetworkIPResolution keywords and their builder
// properties, |DataDirectory| in AttachDBFilename, the factory's data source enumerator, SqlParameterCollection's
// Add(string, object)), as cases whose outcome is a string: the value, or the exception's type and message.
//
// Compiled twice: into the tests (FrameworkGoldenTests compares the port's outcomes with the golden) and into
// golden\record.csproj (net48), which records the golden from .NET Framework's own System.Data: see golden\Record.cs.
// Only what a connection string, a builder or a collection gives without a server: nothing here connects.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;

namespace System.Data.SqlClient.Tests
{
    public static class FrameworkCases
    {
        static readonly string[] s_libraries = { "dbmssocn", "DBNMPNTW", " dbmslpcn ", "dbmsrpcn", "dbmsvinn", "dbmsadsn", "dbmsspxn", "dbmsgnet" };

        /// <summary>The folder the |DataDirectory| cases set as the AppDomain's DataDirectory.</summary>
        public static readonly string Folder = Path.Combine(Path.GetTempPath(), "site", "App_Data");

        public static IEnumerable<KeyValuePair<string, Func<string>>> All()
        {
            var cases = new List<KeyValuePair<string, Func<string>>>();
            void Add(string name, Func<string> run) => cases.Add(new KeyValuePair<string, Func<string>>(name, run));

            // --- the builder's properties
            foreach (var library in s_libraries)
            {
                Add("builder.NetworkLibrary=" + library.Trim(), () =>
                {
                    var b = new SqlConnectionStringBuilder("Data Source=srv") { NetworkLibrary = library };
                    return b.NetworkLibrary + " | " + b.ConnectionString;
                });
            }
            Add("builder.NetworkLibrary=tcp", () => { new SqlConnectionStringBuilder { NetworkLibrary = "tcp" }; return "set"; });
            Add("builder.NetworkLibrary=empty", () => { new SqlConnectionStringBuilder { NetworkLibrary = "" }; return "set"; });
            Add("builder.NetworkLibrary=null", () => { new SqlConnectionStringBuilder { NetworkLibrary = null }; return "set"; });
            Add("builder(net=DBMSSOCN)", () => Show(new SqlConnectionStringBuilder("Data Source=srv;net=DBMSSOCN")));
            Add("builder(network= dbnmpntw )", () => Show(new SqlConnectionStringBuilder("network= dbnmpntw ;Data Source=srv")));
            Add("builder(Network Library=dbmsfoo)", () => Show(new SqlConnectionStringBuilder("Data Source=srv;Network Library=dbmsfoo")));
            Add("builder[net]=DBMSLPCN", () => { var b = new SqlConnectionStringBuilder(); b["net"] = "DBMSLPCN"; return Show(b); });
            Add("builder.defaults", () => Show(new SqlConnectionStringBuilder()));
            Add("builder.indexer defaults", () =>
            {
                var b = new SqlConnectionStringBuilder();
                return string.Join(" | ", new[] { "async", "Asynchronous Processing", "Connection Reset", "context connection", "net", "network", "Network Library", "TransparentNetworkIPResolution" }
                    .Select(k => k + "=" + Convert.ToString(b[k], CultureInfo.InvariantCulture)));
            });
            Add("builder.set all", () =>
            {
#pragma warning disable 618 // ConnectionReset: obsolete, as on .NET Framework
                var b = new SqlConnectionStringBuilder { AsynchronousProcessing = true, ConnectionReset = false, ContextConnection = true, NetworkLibrary = "dbmssocn", TransparentNetworkIPResolution = false };
#pragma warning restore 618
                return Show(b);
            });
            Add("builder(yes/no values)", () => Show(new SqlConnectionStringBuilder("async=yes;connection reset=no;context connection=yes;transparentnetworkipresolution=no")));
            Add("builder(Asynchronous Processing=maybe)", () => Show(new SqlConnectionStringBuilder("Asynchronous Processing=maybe")));
            Add("builder(Connection Reset=maybe)", () => Show(new SqlConnectionStringBuilder("Connection Reset=maybe")));
            Add("builder(Context Connection=maybe)", () => Show(new SqlConnectionStringBuilder("Context Connection=maybe")));
            Add("builder(TransparentNetworkIPResolution=maybe)", () => Show(new SqlConnectionStringBuilder("TransparentNetworkIPResolution=maybe")));
            Add("builder.Remove each", () =>
            {
                var b = new SqlConnectionStringBuilder("Data Source=srv;async=true;Connection Reset=false;Context Connection=true;net=dbmssocn;TransparentNetworkIPResolution=false");
                var removed = string.Join(",", new[] { "async", "connection reset", "Context Connection", "network", "transparentnetworkipresolution", "async" }.Select(k => b.Remove(k).ToString()));
                return removed + " | " + Show(b);
            });
            Add("builder.Clear", () =>
            {
                var b = new SqlConnectionStringBuilder("Data Source=srv;async=true;Connection Reset=false;Context Connection=true;net=dbmssocn;TransparentNetworkIPResolution=false");
                b.Clear();
                return Show(b);
            });
            Add("builder.Keys and synonyms", () =>
            {
                var b = new SqlConnectionStringBuilder();
                var keys = b.Keys.Cast<string>().ToList();
                return string.Join(" | ", new[] { "Asynchronous Processing", "Connection Reset", "Context Connection", "Network Library", "TransparentNetworkIPResolution" }.Select(k => k + "=" + keys.Contains(k)))
                    + " | " + string.Join(" | ", new[] { "async", "net", "network" }.Select(k => k + "=" + b.ContainsKey(k)));
            });
            Add("builder.ShouldSerialize", () =>
            {
                var b = new SqlConnectionStringBuilder("net=dbmssocn");
                return b.ShouldSerialize("network") + "," + b.ShouldSerialize("async");
            });
            Add("builder.TryGetValue", () =>
            {
                var b = new SqlConnectionStringBuilder("net=dbmssocn");
                return b.TryGetValue("network", out object value) + "=" + value + "," + b.TryGetValue("async", out value) + "=" + value;
            });

            // --- the connection string
            foreach (var library in s_libraries.Concat(new[] { "dbmsfoo", "tcp" }))
            {
                Add("connection Network Library=" + library.Trim(), () => Connect("Network Library=" + library));
            }
            Add("connection Network Library=(empty)", () => Connect("Network Library="));
            Add("connection net=DBMSSOCN", () => Connect("net=DBMSSOCN"));
            Add("connection network=dbmsfoo", () => Connect("network=dbmsfoo"));
            Add("connection async=yes", () => Connect("async=yes"));
            Add("connection Asynchronous Processing=maybe", () => Connect("Asynchronous Processing=maybe"));
            Add("connection Connection Reset=no", () => Connect("Connection Reset=no"));
            Add("connection Connection Reset=maybe", () => Connect("Connection Reset=maybe"));
            Add("connection Context Connection=false", () => Connect("Context Connection=false"));
            Add("connection Context Connection=maybe", () => Connect("Context Connection=maybe"));
            Add("connection Context Connection=true", () => Connect("Context Connection=true"));
            Add("connection TransparentNetworkIPResolution=no", () => Connect("TransparentNetworkIPResolution=no"));
            Add("connection TransparentNetworkIPResolution=maybe", () => Connect("TransparentNetworkIPResolution=maybe"));
            Add("connection ConnectionString kept", () => new SqlConnection("Data Source=srv;async=true;Network=DBMSSOCN").ConnectionString);

            // --- |DataDirectory| in AttachDBFilename (the AppDomain's DataDirectory set to Folder unless the case says)
            char sep = Path.DirectorySeparatorChar;
            Add("datadirectory Data Source=.", () => Attach(".", "|DataDirectory|Database.mdf"));
            Add("datadirectory Data Source=(local)", () => Attach("(local)", "|DataDirectory|Database.mdf"));
            Add("datadirectory Data Source=machine", () => Attach(Environment.MachineName, "|DataDirectory|Database.mdf"));
            Add("datadirectory Data Source=.\\SQLEXPRESS", () => Attach(".\\SQLEXPRESS", "|DataDirectory|Database.mdf"));
            Add("datadirectory Data Source=(localdb)", () => Attach("(localdb)\\MSSQLLocalDB", "|DataDirectory|Database.mdf"));
            Add("datadirectory Data Source=localhost", () => Attach("localhost", "|DataDirectory|Database.mdf"));
            Add("datadirectory Data Source=srv", () => Attach("srv", "|DataDirectory|Database.mdf"));
            Add("datadirectory lower case token", () => Attach(".", "|datadirectory|Database.mdf"));
            Add("datadirectory token only", () => Attach(".", "|DataDirectory|"));
            Add("datadirectory separators both", () => Attach(".", "|DataDirectory|" + sep + "Database.mdf", Folder + sep));
            Add("datadirectory parent", () => Attach(".", "|DataDirectory|.." + sep + "Database.mdf"));
            Add("datadirectory sibling folder", () => Attach(".", "|DataDirectory|.." + sep + "App_Data2" + sep + "Database.mdf"));
            Add("datadirectory inner parent", () => Attach(".", "|DataDirectory|sub" + sep + ".." + sep + "Database.mdf"));
            Add("datadirectory token elsewhere", () => Attach(".", "x|DataDirectory|Database.mdf"));
            Add("datadirectory token twice", () => Attach(".", "|DataDirectory||DataDirectory|Database.mdf"));
            Add("datadirectory too long", () => Attach(".", "|DataDirectory|" + new string('a', 300) + ".mdf"));
            Add("datadirectory not set", () => Attach(".", "|DataDirectory|Database.mdf", null));
            Add("datadirectory empty", () => Attach(".", "|DataDirectory|Database.mdf", ""));
            Add("datadirectory not a string", () => Attach(".", "|DataDirectory|Database.mdf", 42));
            Add("datadirectory relative", () => Attach(".", "|DataDirectory|Database.mdf", "App_Data"));
            Add("datadirectory path as it is", () => Attach("srv", Path.Combine(Folder, "Database.mdf")));

            // --- the factory's enumerator (the table's schema: its rows are the network's)
            Add("factory.CanCreateDataSourceEnumerator", () => SqlClientFactory.Instance.CanCreateDataSourceEnumerator.ToString());
            Add("factory.CreateDataSourceEnumerator", () =>
            {
                var e = SqlClientFactory.Instance.CreateDataSourceEnumerator();
                return e.GetType().FullName + "," + ReferenceEquals(e, SqlDataSourceEnumerator.Instance);
            });
            Add("enumerator.GetDataSources schema", () =>
            {
                DataTable table = SqlDataSourceEnumerator.Instance.GetDataSources();
                return table.TableName + "," + table.Locale.Name + ": " + string.Join(", ", table.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName + " " + c.DataType.Name + (c.ReadOnly ? " ro" : "") + (c.AllowDBNull ? " null" : "")));
            });

            // --- SqlParameterCollection.Add(string, object)
#pragma warning disable 618 // obsolete, as on .NET Framework
            Add("parameters.Add(string)", () => ShowParameters(c => c.Add("@name", (object)"value")));
            Add("parameters.Add(int)", () => ShowParameters(c => c.Add("@n", (object)5)));
            Add("parameters.Add(null)", () => ShowParameters(c => c.Add("@n", (object)null)));
            Add("parameters.Add(DBNull)", () => ShowParameters(c => c.Add("@n", DBNull.Value)));
            Add("parameters.Add(no name)", () => ShowParameters(c => c.Add(null, (object)1.5m)));
            Add("parameters.Add twice", () => ShowParameters(c => { c.Add("@n", (object)1); return c.Add("@n", (object)2); }));
#pragma warning restore 618
            return cases;
        }

        /// <summary>The outcome of a case: "=" and the value, or "!" and the exception (its parameter apart from the message,
        /// whose suffix for it differs between .NET Framework and .NET).</summary>
        public static string Run(Func<string> run)
        {
            try
            {
                return "=" + run();
            }
            catch (Exception e)
            {
                string message = e.Message;
                string parameter = (e as ArgumentException)?.ParamName;
                if (parameter != null)
                {
                    foreach (var suffix in new[] { Environment.NewLine + "Parameter name: " + parameter, " (Parameter '" + parameter + "')" })
                    {
                        if (message.EndsWith(suffix, StringComparison.Ordinal))
                        {
                            message = message.Substring(0, message.Length - suffix.Length);
                        }
                    }
                    message += " [parameter: " + parameter + "]";
                }
                return "!" + e.GetType().FullName + ": " + message;
            }
        }

        static string Show(SqlConnectionStringBuilder b)
        {
#pragma warning disable 618
            return string.Format(CultureInfo.InvariantCulture, "AsynchronousProcessing={0} ConnectionReset={1} ContextConnection={2} NetworkLibrary='{3}' TransparentNetworkIPResolution={4} | {5}",
                b.AsynchronousProcessing, b.ConnectionReset, b.ContextConnection, b.NetworkLibrary, b.TransparentNetworkIPResolution, b.ConnectionString);
#pragma warning restore 618
        }

        static string Connect(string keywords)
        {
            using (var connection = new SqlConnection("Data Source=srv;" + keywords))
            {
                return "taken";
            }
        }

        /// <summary>A connection to the data source with the AttachDBFilename, the AppDomain's DataDirectory set to Folder or
        /// to the value given (null: none), restored after.</summary>
        static string Attach(string dataSource, string attachDbFilename) => Attach(dataSource, attachDbFilename, Folder);

        static string Attach(string dataSource, string attachDbFilename, object dataDirectory)
        {
            var before = AppDomain.CurrentDomain.GetData("DataDirectory");
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
            try
            {
                using (var connection = new SqlConnection("Data Source=" + dataSource + ";AttachDBFilename=" + attachDbFilename))
                {
                    return "taken";
                }
            }
            finally
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", before);
            }
        }

        static string ShowParameters(Func<SqlParameterCollection, SqlParameter> add)
        {
            var command = new SqlCommand();
            SqlParameter p = add(command.Parameters);
            return string.Format(CultureInfo.InvariantCulture, "{0}: '{1}' {2} {3} {4} {5} {6} | count {7}, index {8}",
                p.GetType().Name, p.ParameterName, p.Value == null ? "<null>" : p.Value is DBNull ? "<DBNull>" : p.Value.GetType().Name + " " + Convert.ToString(p.Value, CultureInfo.InvariantCulture),
                p.SqlDbType, p.DbType, p.Direction, p.Size, command.Parameters.Count, command.Parameters.IndexOf(p));
        }
    }
}
