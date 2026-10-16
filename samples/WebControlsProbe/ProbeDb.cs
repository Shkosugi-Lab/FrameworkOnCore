using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;

namespace WebControlsProbe
{
    /// <summary>
    /// An ADO.NET provider of the probe site's products, in memory (web.config's DbProviderFactories, invariant name
    /// "WebControlsProbe.Data": SqlDataSource's ProviderName), so that a SqlDataSource runs without a database server, the
    /// same on IIS and on the port. Its commands are names, not SQL: "select" (every product, ordered by Code), "select one"
    /// (@Code), "update" (@Name, @Price by @Code), "insert" (@Code, @Name, @Price), "delete" (@Code), "count". Reset() puts
    /// the products back as they were (ProbeData.Many()).
    /// </summary>
    public sealed class ProbeDbFactory : DbProviderFactory
    {
        public static readonly ProbeDbFactory Instance = new ProbeDbFactory();

        static readonly object gate = new object();
        static DataTable products = Initial();

        static DataTable Initial()
        {
            var table = new DataTable("Products") { Locale = CultureInfo.InvariantCulture };
            table.Columns.Add("Code", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Price", typeof(decimal));
            table.Columns.Add("Active", typeof(bool));
            foreach (var product in ProbeData.Many()) table.Rows.Add(product.Code, product.Name, product.Price, product.Active);
            return table;
        }

        public static void Reset()
        {
            lock (gate) products = Initial();
        }

        public override DbConnection CreateConnection() => new ProbeDbConnection();
        public override DbCommand CreateCommand() => new ProbeDbCommand();
        public override DbParameter CreateParameter() => new ProbeDbParameter();
        public override DbDataAdapter CreateDataAdapter() => new ProbeDbDataAdapter();
        public override DbCommandBuilder CreateCommandBuilder() => null;

        sealed class ProbeDbDataAdapter : DbDataAdapter
        {
        }

        /// <summary>A command run against the products: its rows (a select), or the rows it changed.</summary>
        internal static object Run(string text, DbParameterCollection parameters, out DataTable rows)
        {
            object Value(string name)
            {
                foreach (DbParameter parameter in parameters)
                    if (string.Equals(parameter.ParameterName.TrimStart('@'), name, StringComparison.OrdinalIgnoreCase)) return parameter.Value;
                throw new InvalidOperationException("The command '" + text + "' needs the parameter @" + name + ".");
            }
            lock (gate)
            {
                rows = null;
                var all = products.Rows.Cast<DataRow>();
                switch ((text ?? "").Trim().ToLowerInvariant())
                {
                    case "select":
                        rows = products.Clone();
                        foreach (var row in all.OrderBy(r => (string)r["Code"], StringComparer.Ordinal)) rows.ImportRow(row);
                        return rows.Rows.Count;
                    case "select one":
                        rows = products.Clone();
                        foreach (var row in all.Where(r => (string)r["Code"] == Convert.ToString(Value("Code"), CultureInfo.InvariantCulture))) rows.ImportRow(row);
                        return rows.Rows.Count;
                    case "count":
                        return products.Rows.Count;
                    case "update":
                    {
                        var changed = 0;
                        foreach (var row in all.Where(r => (string)r["Code"] == Convert.ToString(Value("Code"), CultureInfo.InvariantCulture)).ToList())
                        {
                            row["Name"] = Value("Name");
                            row["Price"] = Convert.ToDecimal(Value("Price"), CultureInfo.InvariantCulture);
                            changed++;
                        }
                        return changed;
                    }
                    case "insert":
                        products.Rows.Add(Value("Code"), Value("Name"), Convert.ToDecimal(Value("Price"), CultureInfo.InvariantCulture), false);
                        return 1;
                    case "delete":
                    {
                        var removed = all.Where(r => (string)r["Code"] == Convert.ToString(Value("Code"), CultureInfo.InvariantCulture)).ToList();
                        foreach (var row in removed) products.Rows.Remove(row);
                        return removed.Count;
                    }
                    default:
                        throw new InvalidOperationException("Unknown command '" + text + "' (select, select one, update, insert, delete, count).");
                }
            }
        }
    }

    public sealed class ProbeDbConnection : DbConnection
    {
        ConnectionState state = ConnectionState.Closed;

        public override string ConnectionString { get; set; } = "";
        public override string Database => "Products";
        public override string DataSource => "memory";
        public override string ServerVersion => "1.0";
        public override ConnectionState State => state;
        public override void ChangeDatabase(string databaseName) { }
        public override void Open() => state = ConnectionState.Open;
        public override void Close() => state = ConnectionState.Closed;
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
        protected override DbCommand CreateDbCommand() => new ProbeDbCommand { Connection = this };
    }

    public sealed class ProbeDbCommand : DbCommand
    {
        readonly ProbeDbParameterCollection parameters = new ProbeDbParameterCollection();

        public override string CommandText { get; set; } = "";
        public override int CommandTimeout { get; set; } = 30;
        public override CommandType CommandType { get; set; } = CommandType.Text;
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection => parameters;
        protected override DbTransaction DbTransaction { get; set; }
        public override void Cancel() { }
        public override void Prepare() { }
        protected override DbParameter CreateDbParameter() => new ProbeDbParameter();

        public override int ExecuteNonQuery() => Convert.ToInt32(ProbeDbFactory.Run(CommandText, parameters, out _), CultureInfo.InvariantCulture);

        public override object ExecuteScalar() => ProbeDbFactory.Run(CommandText, parameters, out _);

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            ProbeDbFactory.Run(CommandText, parameters, out var rows);
            return (rows ?? new DataTable()).CreateDataReader();
        }
    }

    public sealed class ProbeDbParameter : DbParameter
    {
        public override DbType DbType { get; set; } = DbType.String;
        public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;
        public override bool IsNullable { get; set; } = true;
        public override string ParameterName { get; set; } = "";
        public override string SourceColumn { get; set; } = "";
        public override object Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override int Size { get; set; }
        public override void ResetDbType() => DbType = DbType.String;
    }

    public sealed class ProbeDbParameterCollection : DbParameterCollection
    {
        readonly List<DbParameter> items = new List<DbParameter>();

        public override int Count => items.Count;
        public override object SyncRoot => items;
        public override int Add(object value) { items.Add((DbParameter)value); return items.Count - 1; }
        public override void AddRange(Array values) { foreach (var value in values) Add(value); }
        public override void Clear() => items.Clear();
        public override bool Contains(object value) => items.Contains((DbParameter)value);
        public override bool Contains(string value) => IndexOf(value) >= 0;
        public override void CopyTo(Array array, int index) => ((ICollection)items).CopyTo(array, index);
        public override IEnumerator GetEnumerator() => items.GetEnumerator();
        public override int IndexOf(object value) => items.IndexOf((DbParameter)value);
        public override int IndexOf(string parameterName) => items.FindIndex(p => string.Equals(p.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase));
        public override void Insert(int index, object value) => items.Insert(index, (DbParameter)value);
        public override void Remove(object value) => items.Remove((DbParameter)value);
        public override void RemoveAt(int index) => items.RemoveAt(index);
        public override void RemoveAt(string parameterName) => items.RemoveAt(IndexOf(parameterName));
        protected override DbParameter GetParameter(int index) => items[index];
        protected override DbParameter GetParameter(string parameterName) => items[IndexOf(parameterName)];
        protected override void SetParameter(int index, DbParameter value) => items[index] = value;
        protected override void SetParameter(string parameterName, DbParameter value) => items[IndexOf(parameterName)] = value;
    }

    /// <summary>A LinqDataSource's context (ContextTypeName): the products as a list (LINQ to Objects).</summary>
    public sealed class ProbeLinqContext
    {
        public List<ProbeData.Product> Products => ProbeData.Many();
    }
}
