// Records the golden SQL of SqlCases against .NET Framework 4.8's own System.Data.Linq (this project is
// net48): each case's CommandText and parameters into dlinq-sql.golden.json, which
// DataLinqSqlGoldenTests compares the port against. Run by record.ps1; needs a reachable SQL Server (LINQ
// to SQL opens the connection to pick the SQL generation mode from the server's version; SQL Server 2008+
// expected, any newer server generates the same).
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;

namespace FrameworkOnCore.Tests.DataLinq
{
    public static class Record
    {
        public static void Main(string[] args)
        {
            var connection = args.Length > 0 ? args[0] : @"Data Source=.\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";
            var output = args.Length > 1 ? args[1] : "dlinq-sql.golden.json";

            var json = new StringBuilder();
            json.Append("{\n");
            var first = true;
            using (var db = new StoreDb(connection))
            {
                foreach (var item in SqlCases.All(db))
                {
                    var command = db.GetCommand(item.Value);
                    if (!first) json.Append(",\n");
                    first = false;
                    json.Append("  ").Append(Quote(item.Key)).Append(": {\n");
                    json.Append("    \"sql\": ").Append(Quote(command.CommandText)).Append(",\n");
                    json.Append("    \"parameters\": [");
                    var firstParameter = true;
                    foreach (DbParameter parameter in command.Parameters)
                    {
                        if (!firstParameter) json.Append(", ");
                        firstParameter = false;
                        json.Append("{ \"name\": ").Append(Quote(parameter.ParameterName))
                            .Append(", \"value\": ").Append(Quote(Convert.ToString(parameter.Value, System.Globalization.CultureInfo.InvariantCulture))).Append(" }");
                    }
                    json.Append("]\n  }");
                }
            }
            json.Append("\n}\n");
            File.WriteAllText(output, json.ToString(), new UTF8Encoding(false));
            Console.WriteLine("golden -> " + Path.GetFullPath(output));
        }

        static string Quote(string s)
        {
            var sb = new StringBuilder("\"");
            foreach (var c in s)
            {
                if (c == '"') sb.Append("\\\"");
                else if (c == '\\') sb.Append("\\\\");
                else if (c == '\r') sb.Append("\\r");
                else if (c == '\n') sb.Append("\\n");
                else if (c == '\t') sb.Append("\\t");
                else if (c < ' ') sb.Append("\\u").Append(((int)c).ToString("x4"));
                else sb.Append(c);
            }
            return sb.Append('"').ToString();
        }
    }
}
