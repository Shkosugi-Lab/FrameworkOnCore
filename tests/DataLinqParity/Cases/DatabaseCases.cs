using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace FrameworkOnCore.DataLinqParity.Cases
{
    /// <summary>What reaches the database: its creation, SubmitChanges (the commands, what is synchronized back,
    /// custom methods, validation), concurrency conflicts and their resolution, Refresh, transactions, raw SQL and
    /// Translate, stored procedures, deferred loading and load options, binding lists.</summary>
    static class DatabaseCases
    {
        static ParityDb Db(StringWriter log = null)
        {
            var db = new ParityDb(Fixture.Connection);
            if (log != null) db.Log = log;
            return db;
        }

        static List<string> Customers() => Fixture.Rows("SELECT CustomerID, CompanyName, City FROM dbo.Customers ORDER BY CustomerID");

        [Case(Database = true)]
        static void Create_Exists_Delete(Probe p)
        {
            var other = new SqlConnectionStringBuilder(Fixture.Connection) { InitialCatalog = "FocDataLinqParity_Other" }.ConnectionString;
            using (var db = new ParityDb(other))
            {
                var log = new StringWriter();
                db.Log = log;
                if (db.DatabaseExists()) db.DeleteDatabase();
                p.Is("exists before", db.DatabaseExists());
                db.CreateDatabase();
                p.Text("create log", log.ToString().Replace("FocDataLinqParity_Other", "<db>"));
                p.Is("exists after", db.DatabaseExists());
                p.Does("create again", () => db.CreateDatabase());
                p.Is("tables", Fixture.Rows("SELECT TABLE_NAME FROM FocDataLinqParity_Other.INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME"));
                p.Is("columns", Fixture.Rows("SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE FROM FocDataLinqParity_Other.INFORMATION_SCHEMA.COLUMNS ORDER BY TABLE_NAME, ORDINAL_POSITION"));
                p.Is("keys", Fixture.Rows("SELECT tc.TABLE_NAME, tc.CONSTRAINT_TYPE, k.COLUMN_NAME FROM FocDataLinqParity_Other.INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc JOIN FocDataLinqParity_Other.INFORMATION_SCHEMA.KEY_COLUMN_USAGE k ON k.CONSTRAINT_NAME = tc.CONSTRAINT_NAME ORDER BY tc.TABLE_NAME, tc.CONSTRAINT_TYPE, k.COLUMN_NAME"));
                SqlConnection.ClearAllPools();
                db.DeleteDatabase();
                p.Is("exists after delete", db.DatabaseExists());
                p.Does("delete again", () => db.DeleteDatabase());
            }
            using (var empty = new DataContext(other))
                p.Does("create without tables", () => empty.CreateDatabase());
        }

        [Case(Database = true)]
        static void Submit_Insert_Update_Delete(Probe p)
        {
            var log = new StringWriter();
            using (var db = Db(log))
            {
                var customer = new Customer { CustomerID = "NEWCO", CompanyName = "New Co", City = "Kyoto" };
                customer.Orders.Add(new Order { OrderDate = Fixture.Day, Stamp = Fixture.Stamp, Amount = 1.5m });
                db.Customers.InsertOnSubmit(customer);
                var alfki = db.Customers.Single(c => c.CustomerID == "ALFKI");
                alfki.City = "Hamburg";
                var chops = db.Customers.Single(c => c.CustomerID == "CHOPS");
                db.Customers.DeleteOnSubmit(chops);
                p.Is("change set", db.GetChangeSet().ToString());
                db.SubmitChanges();
                p.Text("log", log.ToString());
                p.Is("identity synced", customer.Orders.Single().OrderID);
                p.Is("foreign key synced", customer.Orders.Single().CustomerID);
                p.Is("customers", Customers());
                p.Is("orders", Fixture.Rows("SELECT OrderID, CustomerID, Amount FROM dbo.Orders ORDER BY OrderID"));
                p.Is("change set after", db.GetChangeSet().ToString());
                p.Is("original state after", db.Customers.GetOriginalEntityState(alfki));
            }
        }

        [Case(Database = true)]
        static void Submit_Failures(Probe p)
        {
            using (var db = Db())
            {
                db.Customers.InsertOnSubmit(new Customer { CustomerID = "ALFKI", CompanyName = "Dup" });
                p.Does("duplicate key (identity cache)", () => db.SubmitChanges());
            }
            using (var db = Db())
            {
                db.Customers.InsertOnSubmit(new Customer { CustomerID = "ALFKI", CompanyName = "Dup" });
                db.Customers.InsertOnSubmit(new Customer { CustomerID = "ALFKI", CompanyName = "Dup2" });
                p.Does("same key twice", () => db.SubmitChanges());
            }
            using (var db = Db())
            {
                db.Customers.InsertOnSubmit(new Customer { CustomerID = "LONGER THAN FIVE", CompanyName = "x" });
                p.Does("server error", () => db.SubmitChanges());
                p.Is("nothing written", Customers().Count);
            }
            using (var db = Db())
            {
                db.Notes.InsertOnSubmit(new Note { Text = "invalid" });
                p.Does("OnValidate throws", () => db.SubmitChanges());
            }
            using (var db = Db())
            {
                db.Customers.InsertOnSubmit(new Customer { CustomerID = "X1" });   // CompanyName NOT NULL
                p.Does("null into not null", () => db.SubmitChanges());
            }
            using (var db = Db())
            {
                var order = db.Orders.First(o => o.OrderID == 1);
                p.Does("foreign key already has a value", () => order.CustomerID = "ANTON");
            }
        }

        [Case(Database = true)]
        static void Generated_Values_And_Hooks(Probe p)
        {
            var log = new StringWriter();
            using (var db = Db(log))
            {
                var note = new Note { Text = "third" };
                db.Notes.InsertOnSubmit(note);
                db.SubmitChanges();
                p.Is("identity", note.Id);
                p.Is("version set", note.Version != null && note.Version.Length == 8);
                var inserted = note.Version;
                p.Is("events on insert", note.Events);
                note.Text = "changed";
                db.SubmitChanges();
                p.Is("version changed", note.Version != inserted);
                p.Is("events on update", note.Events);
                var loaded = db.Notes.Single(n => n.Id == 1);
                p.Is("OnLoaded", loaded.Events);
                db.Notes.DeleteOnSubmit(loaded);
                db.SubmitChanges();
                p.Is("OnValidate on delete", loaded.Events);

                var product = new Product { ProductID = Guid.Empty, Name = "Computed", Kind = ProductKind.Book, Weight = 4, Description = "d" };
                db.Products.InsertOnSubmit(product);
                db.SubmitChanges();
                p.Is("computed synced (AutoSync Always)", product.DoubleWeight);
                product.Weight = 5;
                db.SubmitChanges();
                p.Is("computed after update", product.DoubleWeight);
                p.Text("log", log.ToString());
            }
        }

        [Case(Database = true)]
        static void Custom_Methods(Probe p)
        {
            using (var db = Db())
            {
                var note = new Note { Text = "custom" };
                db.Notes.InsertOnSubmit(note);
                db.SubmitChanges();
                note.Text = "custom2";
                db.SubmitChanges();
                db.Notes.DeleteOnSubmit(note);
                db.SubmitChanges();
                p.Is("calls", db.CustomCalls);
                p.Is("notes", Fixture.Rows("SELECT Id, Text FROM dbo.Notes ORDER BY Id"));
            }
        }

        [Case(Database = true)]
        static void Conflicts(Probe p)
        {
            using (var first = Db())
            using (var second = Db())
            {
                var a1 = first.Customers.Single(c => c.CustomerID == "ALFKI");
                var b1 = first.Customers.Single(c => c.CustomerID == "ANTON");
                var a2 = second.Customers.Single(c => c.CustomerID == "ALFKI");
                var b2 = second.Customers.Single(c => c.CustomerID == "ANTON");
                var d2 = second.Customers.Single(c => c.CustomerID == "BONAP");
                a1.City = "First"; b1.CompanyName = "First Co";
                first.SubmitChanges();
                using (var c = Fixture.Open()) Fixture.Execute(c, "DELETE FROM dbo.Customers WHERE CustomerID = N'BONAP'");

                a2.City = "Second"; b2.City = "Second"; d2.City = "Gone";
                p.Does("fail on first", () => second.SubmitChanges(ConflictMode.FailOnFirstConflict));
                p.Is("conflicts (first)", second.ChangeConflicts.Count);
                p.Does("continue on conflict", () => second.SubmitChanges(ConflictMode.ContinueOnConflict));
                var conflicts = second.ChangeConflicts;
                p.Is("conflicts", conflicts.Count);
                foreach (ObjectChangeConflict conflict in conflicts)
                {
                    var prefix = "conflict " + conflict.Object + ": ";
                    p.Is(prefix + "IsDeleted", conflict.IsDeleted);
                    p.Is(prefix + "IsResolved", conflict.IsResolved);
                    foreach (var member in conflict.MemberConflicts)
                        p.Is(prefix + member.Member.Name, new object[] { member.OriginalValue, member.CurrentValue, member.DatabaseValue, member.IsModified, member.IsResolved });
                }
                var list = conflicts.ToList();
                p.Is("Item[0]", ReferenceEquals(conflicts[0], list[0]));
                p.Is("Contains", conflicts.Contains(list[0]));
                var array = new ObjectChangeConflict[conflicts.Count + 1];
                conflicts.CopyTo(array, 1);
                p.Is("CopyTo", array.Select(x => x?.Object));
                p.Does("Remove", () => conflicts.Remove(list[0]));
                p.Does("Clear", () => conflicts.Clear());
                var enumerated = 0;
                using (var e = conflicts.GetEnumerator()) while (e.MoveNext()) enumerated++;
                p.Is("enumerated", enumerated);

                // One resolution per way: the member, the object, all.
                var alfki = list.Single(x => ((Customer)x.Object).CustomerID == "ALFKI");
                var city = alfki.MemberConflicts.Single(m => m.Member.Name == "City");
                city.Resolve("Resolved");
                p.Is("member resolved to value", new object[] { city.IsResolved, a2.City });
                var anton = list.Single(x => ((Customer)x.Object).CustomerID == "ANTON");
                anton.MemberConflicts.First().Resolve(RefreshMode.KeepChanges);
                anton.Resolve(RefreshMode.KeepChanges);
                p.Is("object resolved KeepChanges", new object[] { anton.IsResolved, b2.CompanyName, b2.City });
                var bonap = list.Single(x => ((Customer)x.Object).CustomerID == "BONAP");
                p.Does("deleted: resolve keeping", () => bonap.Resolve(RefreshMode.KeepCurrentValues));
                p.Does("deleted: resolve auto-resolving", () => bonap.Resolve(RefreshMode.KeepCurrentValues, true));
                p.Does("resolve default", () => alfki.Resolve());
                p.Does("ResolveAll", () => conflicts.ResolveAll(RefreshMode.OverwriteCurrentValues));
                p.Does("ResolveAll deleted", () => conflicts.ResolveAll(RefreshMode.OverwriteCurrentValues, true));
                p.Does("submit after resolving", () => second.SubmitChanges());
                p.Is("customers", Customers());
            }
        }

        [Case(Database = true)]
        static void Refresh(Probe p)
        {
            using (var db = Db())
            {
                var alfki = db.Customers.Single(c => c.CustomerID == "ALFKI");
                var anton = db.Customers.Single(c => c.CustomerID == "ANTON");
                var bonap = db.Customers.Single(c => c.CustomerID == "BONAP");
                alfki.CompanyName = "Mine";
                using (var c = Fixture.Open()) Fixture.Execute(c, "UPDATE dbo.Customers SET City = N'Theirs', CompanyName = N'Theirs Co'");
                db.Refresh(RefreshMode.KeepChanges, alfki);
                p.Is("KeepChanges", alfki);
                db.Refresh(RefreshMode.KeepCurrentValues, new object[] { anton });
                p.Is("KeepCurrentValues", anton);
                db.Refresh(RefreshMode.OverwriteCurrentValues, (IEnumerable)new[] { bonap });
                p.Is("OverwriteCurrentValues", bonap);
                p.Does("refresh untracked", () => db.Refresh(RefreshMode.OverwriteCurrentValues, new Customer { CustomerID = "ZZZZZ" }));
                p.Does("refresh null", () => db.Refresh(RefreshMode.OverwriteCurrentValues, (object)null));
                using (var c = Fixture.Open()) Fixture.Execute(c, "DELETE FROM dbo.Orders; DELETE FROM dbo.Customers WHERE CustomerID = N'ALFKI'");
                p.Does("refresh deleted", () => db.Refresh(RefreshMode.OverwriteCurrentValues, alfki));
                var created = new Customer { CustomerID = "NEW", CompanyName = "x" };
                db.Customers.InsertOnSubmit(created);
                p.Does("refresh pending insert", () => db.Refresh(RefreshMode.OverwriteCurrentValues, created));
            }
        }

        [Case(Database = true)]
        static void Transactions(Probe p)
        {
            using (var db = Db())
            {
                db.Connection.Open();
                using (var transaction = db.Connection.BeginTransaction())
                {
                    db.Transaction = transaction;
                    p.Is("Transaction", ReferenceEquals(db.Transaction, transaction));
                    db.Customers.InsertOnSubmit(new Customer { CustomerID = "TX", CompanyName = "Tx" });
                    db.SubmitChanges();
                    p.Is("inside", db.Customers.Count(c => c.CustomerID == "TX"));
                    transaction.Rollback();
                }
                db.Transaction = null;
                p.Is("after rollback", Fixture.Rows("SELECT COUNT(*) FROM dbo.Customers WHERE CustomerID = N'TX'"));
                using (var other = new SqlConnection(Fixture.Connection))
                {
                    other.Open();
                    using (var foreign = other.BeginTransaction())
                        p.Does("transaction of another connection", () => db.Transaction = foreign);
                }
                db.Connection.Close();
            }
            using (var db = Db())
            {
                db.Customers.InsertOnSubmit(new Customer { CustomerID = "OK", CompanyName = "ok" });
                db.Customers.InsertOnSubmit(new Customer { CustomerID = "TOO LONG ID", CompanyName = "x" });
                p.Does("submit is one transaction", () => db.SubmitChanges());
                p.Is("nothing written", Fixture.Rows("SELECT COUNT(*) FROM dbo.Customers WHERE CustomerID = N'OK'"));
            }
        }

        [Case(Database = true)]
        static void Raw_Sql_And_Translate(Probe p)
        {
            using (var db = Db())
            {
                p.Try("ExecuteQuery<T>", () => db.ExecuteQuery<Customer>("SELECT * FROM dbo.Customers WHERE City = {0} ORDER BY CustomerID", "Berlin").ToList());
                p.Try("ExecuteQuery<T> tracked", () => ReferenceEquals(db.ExecuteQuery<Customer>("SELECT * FROM dbo.Customers WHERE CustomerID = 'ALFKI'").Single(), db.Customers.Single(c => c.CustomerID == "ALFKI")));
                p.Try("ExecuteQuery<T> scalar type", () => db.ExecuteQuery<int>("SELECT OrderID FROM dbo.Orders ORDER BY OrderID").ToList());
                p.Try("ExecuteQuery<T> projection", () => db.ExecuteQuery<Plain>("SELECT CustomerID AS Id, City FROM dbo.Customers ORDER BY CustomerID").Select(x => x.Id + "/" + x.City).ToList());
                p.Try("ExecuteQuery<T> null argument", () => db.ExecuteQuery<Customer>("SELECT * FROM dbo.Customers WHERE City IS NULL OR City = {0}", (object)null).Count());
                p.Try("ExecuteQuery(Type)", () => db.ExecuteQuery(typeof(Customer), "SELECT * FROM dbo.Customers WHERE CustomerID = {0}", "ANTON").Cast<object>().ToList());
                p.Try("ExecuteQuery bad sql", () => db.ExecuteQuery<Customer>("SELECT * FROM dbo.Nothing").ToList());
                p.Try("ExecuteQuery null", () => db.ExecuteQuery<Customer>(null));
                p.Try("ExecuteQuery missing column", () => db.ExecuteQuery<Customer>("SELECT CustomerID FROM dbo.Customers").ToList());
                p.Try("ExecuteCommand", () => db.ExecuteCommand("UPDATE dbo.Customers SET City = {0} WHERE CustomerID = {1}", "Paris", "BONAP"));
                p.Try("ExecuteCommand none", () => db.ExecuteCommand("UPDATE dbo.Customers SET City = City WHERE 1 = 0"));
                p.Try("ExecuteCommand bad", () => db.ExecuteCommand("UPDATE dbo.Nothing SET x = 1"));
                p.Is("after command", Customers());

                db.Connection.Open();
                using (var command = new SqlCommand("SELECT * FROM dbo.Orders ORDER BY OrderID", (SqlConnection)db.Connection))
                using (var reader = command.ExecuteReader())
                    p.Try("Translate<T>", () => db.Translate<Order>(reader).ToList());
                using (var command = new SqlCommand("SELECT * FROM dbo.Customers ORDER BY CustomerID", (SqlConnection)db.Connection))
                using (var reader = command.ExecuteReader())
                    p.Try("Translate(Type)", () => db.Translate(typeof(Customer), reader).Cast<object>().ToList());
                p.Try("Translate null", () => db.Translate<Order>(null));
                // Translate(DbDataReader): several result sets, each read by GetResult as the caller asks.
                using (var command = new SqlCommand("SELECT * FROM dbo.Customers ORDER BY CustomerID; SELECT * FROM dbo.Orders ORDER BY OrderID", (SqlConnection)db.Connection))
                using (var reader = command.ExecuteReader())
                using (var results = db.Translate(reader))
                {
                    p.Is("Translate multiple: first", results.GetResult<Customer>().ToList());
                    p.Is("Translate multiple: second", results.GetResult<Order>().ToList());
                    p.Try("Translate multiple: ReturnValue", () => results.ReturnValue);
                }
                p.Try("Translate multiple null", () => db.Translate((System.Data.Common.DbDataReader)null));
                db.Connection.Close();
            }
        }

        public class Plain { public string Id; public string City; }

        [Case(Database = true)]
        static void Stored_Procedures(Probe p)
        {
            var log = new StringWriter();
            using (var db = Db(log))
            {
                var single = db.CustomersIn("Berlin");
                p.Is("ISingleResult", single.ToList());
                p.Is("ISingleResult ReturnValue", single.ReturnValue);
                p.Try("ISingleResult twice", () => single.ToList());
                using (var multiple = db.CustomersAndOrders())
                {
                    p.Is("IMultipleResults first", multiple.GetResult<Customer>().ToList());
                    p.Is("IMultipleResults second", multiple.GetResult<Order>().ToList());
                    p.Try("IMultipleResults beyond", () => multiple.GetResult<Order>());
                    p.Is("IMultipleResults ReturnValue", multiple.ReturnValue);
                }
                int? output = null;
                p.Is("return value", db.Double(21, ref output));
                p.Is("output parameter", output);
                p.Text("log", log.ToString());
            }
        }

        [Case(Database = true)]
        static void Compiled_Queries(Probe p)
        {
            using (var db = Db())
            {
                var q0 = CompiledQuery.Compile((ParityDb d) => d.Customers.OrderBy(c => c.CustomerID));
                var q1 = CompiledQuery.Compile((ParityDb d, string a) => d.Customers.Where(c => c.City == a));
                var q2 = CompiledQuery.Compile((ParityDb d, int a, int b) => d.Orders.Where(o => o.OrderID >= a && o.OrderID <= b).Count());
                var q3 = CompiledQuery.Compile((ParityDb d, int a, int b, int c) => a + b + c + d.Orders.Count());
                var q4 = CompiledQuery.Compile((ParityDb d, int a, int b, int c, int e) => a + b + c + e + d.Orders.Count());
                var q5 = CompiledQuery.Compile((ParityDb d, int a, int b, int c, int e, int f) => a + b + c + e + f + d.Orders.Count());
                var q6 = CompiledQuery.Compile((ParityDb d, int a, int b, int c, int e, int f, int g) => a + b + c + e + f + g + d.Orders.Count());
                var q7 = CompiledQuery.Compile((ParityDb d, int a, int b, int c, int e, int f, int g, int h) => a + b + c + e + f + g + h + d.Orders.Count());
                var q8 = CompiledQuery.Compile((ParityDb d, int a, int b, int c, int e, int f, int g, int h, int i) => a + b + c + e + f + g + h + i + d.Orders.Count());
                var q9 = CompiledQuery.Compile((ParityDb d, int a, int b, int c, int e, int f, int g, int h, int i, int j) => a + b + c + e + f + g + h + i + j + d.Orders.Count());
                var q10 = CompiledQuery.Compile((ParityDb d, int a, int b, int c, int e, int f, int g, int h, int i, int j, int k) => a + b + c + e + f + g + h + i + j + k + d.Orders.Count());
                var q11 = CompiledQuery.Compile((ParityDb d, int a, int b, int c, int e, int f, int g, int h, int i, int j, int k, int l) => a + b + c + e + f + g + h + i + j + k + l + d.Orders.Count());
                var q12 = CompiledQuery.Compile((ParityDb d, int a, int b, int c, int e, int f, int g, int h, int i, int j, int k, int l, int m) => a + b + c + e + f + g + h + i + j + k + l + m + d.Orders.Count());
                var q13 = CompiledQuery.Compile((ParityDb d, int a, int b, int c, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n) => a + b + c + e + f + g + h + i + j + k + l + m + n + d.Orders.Count());
                var q14 = CompiledQuery.Compile((ParityDb d, int a, int b, int c, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o) => a + b + c + e + f + g + h + i + j + k + l + m + n + o + d.Orders.Count());
                var q15 = CompiledQuery.Compile((ParityDb d, int a, int b, int c, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int q) => a + b + c + e + f + g + h + i + j + k + l + m + n + o + q + d.Orders.Count());
                p.Is("0", q0(db).ToList());
                p.Is("1", q1(db, "Berlin").ToList());
                p.Is("1 again", q1(db, "Mexico").ToList());
                p.Is("2", q2(db, 1, 3));
                p.Is("3..15", new object[] { q3(db, 1, 1, 1), q4(db, 1, 1, 1, 1), q5(db, 1, 1, 1, 1, 1), q6(db, 1, 1, 1, 1, 1, 1), q7(db, 1, 1, 1, 1, 1, 1, 1), q8(db, 1, 1, 1, 1, 1, 1, 1, 1),
                    q9(db, 1, 1, 1, 1, 1, 1, 1, 1, 1), q10(db, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), q11(db, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), q12(db, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1),
                    q13(db, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), q14(db, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), q15(db, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1) });
                var compiled = new CompiledQueryHolder(q1);
                p.Is("Expression", compiled.Expression);
                p.Try("null context", () => q0(null).ToList());
                p.Try("Compile null", () => CompiledQuery.Compile<ParityDb, int>(null));
                p.Try("other mapping source", () => q0(new ParityDb(Fixture.Connection, new System.Data.Linq.Mapping.AttributeMappingSource())).Count());
            }
        }

        /// <summary>A compiled query's Expression: the delegate's target is the CompiledQuery.</summary>
        sealed class CompiledQueryHolder
        {
            public readonly string Expression;
            public CompiledQueryHolder(Delegate compiled) => Expression = ((CompiledQuery)compiled.Target).Expression.ToString();
        }

        [Case(Database = true)]
        static void Loading(Probe p)
        {
            var log = new StringWriter();
            using (var db = Db(log))
            {
                var customer = db.Customers.Single(c => c.CustomerID == "ALFKI");
                p.Is("orders deferred", customer.Orders.IsDeferred);
                p.Is("orders", customer.Orders.ToList());
                p.Is("orders loaded", customer.Orders.HasLoadedOrAssignedValues);
                var product = db.Products.Single(x => x.ProductID == Fixture.Hammer);
                p.Is("deferred column", product.Description);
                p.Is("order's customer", db.Orders.Single(o => o.OrderID == 3).Customer);
                p.Text("log", log.ToString());
            }
            log = new StringWriter();
            using (var db = Db(log))
            {
                var options = new DataLoadOptions();
                options.LoadWith<Customer>(c => c.Orders);
                options.AssociateWith<Customer>(c => c.Orders.Where(o => o.Amount > 50));
                db.LoadOptions = options;
                p.Is("LoadWith", db.Customers.OrderBy(c => c.CustomerID).ToList().Select(c => c.CustomerID + ":" + string.Join(",", c.Orders.Select(o => o.OrderID))));
                p.Text("log", log.ToString());
                p.Does("options after query", () => db.LoadOptions = new DataLoadOptions());
            }
            using (var db = Db())
            {
                var options = new DataLoadOptions();
                options.LoadWith((System.Linq.Expressions.LambdaExpression)(System.Linq.Expressions.Expression<Func<Order, Customer>>)(o => o.Customer));
                options.AssociateWith((System.Linq.Expressions.LambdaExpression)(System.Linq.Expressions.Expression<Func<Customer, IEnumerable<Order>>>)(c => c.Orders.OrderByDescending(o => o.OrderID)));
                db.LoadOptions = options;
                p.Is("untyped LoadWith", db.Orders.OrderBy(o => o.OrderID).ToList().Select(o => o.Customer?.CustomerID));
                p.Does("frozen after set", () => options.LoadWith<Customer>(c => c.Orders));
            }
            var misuse = new DataLoadOptions();
            p.Does("LoadWith not a member", () => misuse.LoadWith<Customer>(c => c.CompanyName.Length));
            p.Does("LoadWith cycle", () => { var o = new DataLoadOptions(); o.LoadWith<Customer>(c => c.Orders); o.LoadWith<Order>(x => x.Customer); });
            p.Does("AssociateWith not a subquery", () => misuse.AssociateWith<Customer>(c => c.CompanyName));
            p.Does("LoadWith null", () => misuse.LoadWith((System.Linq.Expressions.LambdaExpression)null));
            p.Does("AssociateWith null", () => misuse.AssociateWith((System.Linq.Expressions.LambdaExpression)null));
            using (var db = Db())
            {
                db.DeferredLoadingEnabled = false;
                var customer = db.Customers.Single(c => c.CustomerID == "ALFKI");
                p.Is("deferred loading off", customer.Orders.Count);
            }
        }

        [Case(Database = true)]
        static void Enumeration_And_Binding(Probe p)
        {
            using (var db = Db())
            {
                var enumerated = new List<string>();
                using (var e = db.Customers.GetEnumerator()) while (e.MoveNext()) enumerated.Add(e.Current.CustomerID);
                p.Is("Table.GetEnumerator", enumerated.OrderBy(x => x));
                IBindingList list = db.Customers.GetNewBindingList();
                p.Is("binding list", new object[] { list.GetType(), list.Count, list.AllowNew, list.AllowRemove, list.SupportsSorting });
                var added = (Customer)list.AddNew();
                added.CustomerID = "BIND"; added.CompanyName = "Bound";
                p.Is("AddNew inserts", db.GetChangeSet().Inserts.Count);
                list.Remove(list.Cast<Customer>().Single(c => c.CustomerID == "CHOPS"));
                p.Is("Remove deletes", db.GetChangeSet().Deletes.Count);
                db.SubmitChanges();
                p.Is("customers", Customers());
                var orders = db.Customers.Single(c => c.CustomerID == "ALFKI").Orders;
                var ordersList = orders.GetNewBindingList();
                p.Is("EntitySet binding list", new object[] { ordersList.GetType(), ordersList.Count });
            }
            using (var db = Db() )
            {
                db.ObjectTrackingEnabled = false;
                p.Try("read-only table", () => db.Customers.IsReadOnly);
                p.Try("untracked enumerate", () => db.Customers.OrderBy(c => c.CustomerID).ToList());
            }
        }
    }
}
