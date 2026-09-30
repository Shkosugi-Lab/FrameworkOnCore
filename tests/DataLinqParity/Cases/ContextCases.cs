using System;
using System.Collections;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DataLinqParity.Cases
{
    /// <summary>DataContext and Table without a database: construction, settings, change tracking (insert, attach,
    /// delete, the change set, original state and modified members), the ITable interfaces, disposal.</summary>
    static class ContextCases
    {
        const string NoServer = "Data Source=unused;Initial Catalog=FocDataLinqParity;Integrated Security=True";

        static Customer C(string id, string name = "N", string city = null) => new Customer { CustomerID = id, CompanyName = name, City = city };

        [Case]
        static void Construction(Probe p)
        {
            using (var db = new DataContext(NoServer))
            {
                p.Is("connection type", db.Connection.GetType());
                p.Is("connection string", db.Connection.ConnectionString);
                p.Is("connection state", db.Connection.State);
                p.Is("Mapping type", db.Mapping.GetType());
                p.Is("Mapping context", db.Mapping.ContextType);
            }
            using (var connection = new SqlConnection(NoServer))
            using (var db = new DataContext(connection))
                p.Is("given connection kept", ReferenceEquals(db.Connection, connection));
            var source = new AttributeMappingSource();
            using (var db = new DataContext(NoServer, source))
                p.Is("mapping source kept", ReferenceEquals(db.Mapping.MappingSource, source));
            using (var connection = new SqlConnection(NoServer))
            using (var db = new DataContext(connection, source))
                p.Is("connection and mapping", new object[] { ReferenceEquals(db.Connection, connection), ReferenceEquals(db.Mapping.MappingSource, source) });
            // A file name: SQL Server Express's attached file (the connection string made from it).
            using (var db = new DataContext("Northwind.mdf"))
                p.Is("file name", db.Connection.ConnectionString.Contains("AttachDBFilename"));

            p.Try("null string", () => new DataContext((string)null));
            p.Try("null connection", () => new DataContext((System.Data.IDbConnection)null));
            p.Try("null mapping", () => new DataContext(NoServer, null));
            p.Try("null connection and mapping", () => new DataContext((System.Data.IDbConnection)null, source));
            p.Try("bad connection string", () => new DataContext("not=a;valid string"));
        }

        [Case]
        static void Settings(Probe p)
        {
            using (var db = new ParityDb(NoServer))
            {
                p.Is("defaults", new object[] { db.CommandTimeout, db.DeferredLoadingEnabled, db.ObjectTrackingEnabled, db.LoadOptions, db.Log, db.Transaction });
                db.CommandTimeout = 5;
                db.DeferredLoadingEnabled = false;
                var writer = new StringWriter();
                db.Log = writer;
                var options = new DataLoadOptions();
                db.LoadOptions = options;
                p.Is("set", new object[] { db.CommandTimeout, db.DeferredLoadingEnabled, ReferenceEquals(db.Log, writer), ReferenceEquals(db.LoadOptions, options) });
                db.ObjectTrackingEnabled = false;
                p.Is("tracking off", db.ObjectTrackingEnabled);
                p.Is("tracking off forces deferred loading off", db.DeferredLoadingEnabled);
                p.Does("deferred loading on without tracking", () => db.DeferredLoadingEnabled = true);
                p.Does("negative timeout", () => db.CommandTimeout = -1);
                p.Does("options changed after set", () => options.LoadWith<Customer>(c => c.Orders));
            }
        }

        [Case]
        static void Tables(Probe p)
        {
            using (var db = new ParityDb(NoServer))
            {
                var table = db.Customers;
                p.Is("same instance", ReferenceEquals(table, db.GetTable<Customer>()));
                p.Is("ToString", table.ToString());
                p.Is("Context", ReferenceEquals(table.Context, db));
                p.Is("IsReadOnly", table.IsReadOnly);
                var untyped = db.GetTable(typeof(Customer));
                p.Is("GetTable(Type) same", ReferenceEquals(untyped, table));
                p.Is("ITable.Context", ReferenceEquals(untyped.Context, db));
                p.Is("ITable.IsReadOnly", untyped.IsReadOnly);
                p.Try("derived type's table", () => db.GetTable(typeof(Employee)).ToString());
                p.Try("GetTable<Employee>", () => db.GetTable<Employee>().ToString());
                p.Is("IQueryable", new object[] { ((IQueryable)table).ElementType, ((IQueryable)table).Expression.NodeType, ((IQueryable)table).Provider.GetType() });
            }
        }

        [Case]
        static void Insert_Delete_ChangeSet(Probe p)
        {
            using (var db = new ParityDb(NoServer))
            {
                var a = C("A"); var b = C("B"); var c = C("C");
                db.Customers.InsertOnSubmit(a);
                db.Customers.InsertAllOnSubmit(new[] { b, c });
                var set = db.GetChangeSet();
                p.Is("inserts", set.Inserts);
                p.Is("ToString", set.ToString());
                p.Does("insert again", () => db.Customers.InsertOnSubmit(a));
                db.Customers.DeleteOnSubmit(b);   // an insert deleted: neither
                p.Is("after delete of an insert", db.GetChangeSet().ToString());
                db.Customers.DeleteAllOnSubmit(new[] { c });
                p.Is("after DeleteAllOnSubmit", db.GetChangeSet().Inserts);
                p.Does("InsertOnSubmit null", () => db.Customers.InsertOnSubmit(null));
                p.Does("InsertAllOnSubmit null", () => db.Customers.InsertAllOnSubmit<Customer>(null));
                p.Does("DeleteOnSubmit null", () => db.Customers.DeleteOnSubmit(null));
                p.Does("DeleteAllOnSubmit null", () => db.Customers.DeleteAllOnSubmit<Customer>(null));
                p.Does("delete unattached", () => db.Customers.DeleteOnSubmit(C("Z")));
                p.Does("insert derived into its table", () => db.People.InsertOnSubmit(new Employee { PersonID = 9, Kind = "E", Name = "e" }));
                p.Is("derived insert", db.GetChangeSet().Inserts.Count);
                p.Does("insert to other table", () => db.GetTable(typeof(Order)).InsertOnSubmit(C("W")));
            }
        }

        [Case]
        static void Attach(Probe p)
        {
            using (var db = new ParityDb(NoServer))
            {
                var a = C("A", "Alpha");
                db.Customers.Attach(a);
                p.Is("attached is unchanged", db.GetChangeSet().ToString());
                a.CompanyName = "Beta";
                p.Is("changed", db.GetChangeSet().Updates);
                p.Is("original state", db.Customers.GetOriginalEntityState(a));
                p.Is("modified members", db.Customers.GetModifiedMembers(a).Select(m => m.Member.Name + ": " + m.OriginalValue + " -> " + m.CurrentValue));
                p.Does("attach again", () => db.Customers.Attach(a));
                p.Does("attach same key", () => db.Customers.Attach(C("A")));
                p.Does("attach null", () => db.Customers.Attach(null));

                // As modified: needs a version member or no update checks (Customer has checks: refused).
                p.Does("attach as modified with checks", () => db.Customers.Attach(C("M"), true));
                p.Does("attach as modified with version", () => db.Notes.Attach(new Note { Id = 1, Text = "t", Version = new byte[] { 1 } }, true));
                p.Is("modified note", db.GetChangeSet().Updates.Count);
                p.Does("attach as unmodified", () => db.Customers.Attach(C("U"), false));

                var current = C("O", "Now");
                var original = C("O", "Was");
                db.Customers.Attach(current, original);
                p.Is("attach with original: modified", db.Customers.GetModifiedMembers(current).Select(m => m.Member.Name + ": " + m.OriginalValue + " -> " + m.CurrentValue));
                p.Is("attach with original: original state", db.Customers.GetOriginalEntityState(current));
                p.Does("attach with original of other type", () => db.GetTable(typeof(Customer)).Attach(C("Q"), new Order()));

                db.Customers.AttachAll(new[] { C("L1"), C("L2") });
                db.Notes.AttachAll(new[] { new Note { Id = 5, Text = "a", Version = new byte[] { 1 } } }, true);
                p.Is("after AttachAll", db.GetChangeSet().ToString());
                p.Does("AttachAll null", () => db.Customers.AttachAll<Customer>(null));
                p.Is("GetOriginalEntityState untracked", db.Customers.GetOriginalEntityState(C("X")));
                p.Is("GetModifiedMembers untracked", db.Customers.GetModifiedMembers(C("X")).Length);
                p.Does("GetOriginalEntityState null", () => db.Customers.GetOriginalEntityState(null));
                p.Does("GetModifiedMembers null", () => db.Customers.GetModifiedMembers(null));
            }
        }

        [Case]
        static void Untyped_Table_Interfaces(Probe p)
        {
            using (var db = new ParityDb(NoServer))
            {
                ITable table = db.GetTable(typeof(Customer));
                var a = C("A", "Alpha"); var b = C("B"); var c = C("C");
                table.Attach(a);
                table.Attach(C("M"), false);
                table.Attach(C("O", "Now"), C("O", "Was"));
                table.AttachAll(new[] { C("L1") });
                table.AttachAll(new[] { C("L2") }, false);
                table.InsertOnSubmit(b);
                table.InsertAllOnSubmit(new[] { c });
                table.DeleteOnSubmit(a);
                table.DeleteAllOnSubmit(new[] { c });
                p.Is("change set", db.GetChangeSet().ToString());
                var o = (Customer)db.GetChangeSet().Updates.Concat(db.GetChangeSet().Deletes).FirstOrDefault();
                p.Is("GetOriginalEntityState", table.GetOriginalEntityState(a));
                p.Is("GetModifiedMembers", table.GetModifiedMembers(a).Length);
                p.Does("wrong type", () => table.InsertOnSubmit(new Order()));
                p.Does("Attach wrong type", () => table.Attach(new Order()));
                p.Does("AttachAll null", () => table.AttachAll(null));
                p.Does("InsertAllOnSubmit null", () => table.InsertAllOnSubmit(null));
                p.Does("DeleteAllOnSubmit null", () => table.DeleteAllOnSubmit(null));

                ITable<Note> notes = db.Notes;
                var note = new Note { Id = 3, Text = "n" };
                notes.Attach(note);
                notes.DeleteOnSubmit(note);
                notes.InsertOnSubmit(new Note { Text = "new" });
                p.Is("ITable<T>", db.GetChangeSet().ToString());
            }
        }

        [Case]
        static void Tracking_Off(Probe p)
        {
            using (var db = new ParityDb(NoServer) { ObjectTrackingEnabled = false })
            {
                p.Does("InsertOnSubmit", () => db.Customers.InsertOnSubmit(C("A")));
                p.Does("Attach", () => db.Customers.Attach(C("A")));
                p.Does("GetChangeSet", () => db.GetChangeSet());
                p.Does("SubmitChanges", () => db.SubmitChanges());
            }
        }

        [Case]
        static void Disposal(Probe p)
        {
            var db = new ParityDb(NoServer);
            var table = db.Customers;
            db.Dispose();
            p.Is("Dispose(bool) called", db.Disposed);
            p.Does("dispose again", () => db.Dispose());
            p.Try("Connection", () => db.Connection);
            p.Try("Mapping", () => db.Mapping);
            p.Try("GetTable", () => db.GetTable<Customer>());
            p.Try("Log", () => db.Log);
            p.Try("GetChangeSet", () => db.GetChangeSet());
            p.Does("SubmitChanges", () => db.SubmitChanges());
            p.Does("table InsertOnSubmit", () => table.InsertOnSubmit(C("A")));
            p.Try("table enumerate", () => table.ToList());
            p.Try("CommandTimeout", () => db.CommandTimeout);
            p.Try("ChangeConflicts", () => db.ChangeConflicts.Count);
            p.Try("Transaction", () => db.Transaction);
            p.Try("ObjectTrackingEnabled", () => db.ObjectTrackingEnabled);
            p.Try("DeferredLoadingEnabled", () => db.DeferredLoadingEnabled);
            p.Try("LoadOptions", () => db.LoadOptions);
            p.Try("DatabaseExists", () => db.DatabaseExists());
        }

        [Case]
        static void Providers(Probe p)
        {
            foreach (var provider in new System.Data.Linq.SqlClient.SqlProvider[] { new System.Data.Linq.SqlClient.SqlProvider(), new System.Data.Linq.SqlClient.Sql2000Provider(), new System.Data.Linq.SqlClient.Sql2005Provider(), new System.Data.Linq.SqlClient.Sql2008Provider() })
            {
                p.Is("provider", provider.GetType());
                p.Does("Dispose", () => provider.Dispose());
            }
            var own = new OwnProvider();
            p.Does("protected Dispose(bool)", () => own.DisposeManaged());
            p.Is("model provider types", new[] { typeof(ParityDb), typeof(ParityDb2000), typeof(ParityDb2005), typeof(DataContext) }.Select(t => new AttributeMappingSource().GetModel(t).ProviderType.Name));
        }
    }
}
