using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Linq.SqlClient;
using System.Globalization;
using System.Linq;

namespace FrameworkOnCore.DataLinqParity.Cases
{
    /// <summary>Queries: the SQL each one translates to (GetCommand) and its rows, over the query operators, the
    /// expressions LINQ to SQL translates (string, date, math, conversion, null, local collections), SqlMethods,
    /// inheritance, composable functions, and the three providers' SQL.</summary>
    static class QueryCases
    {
        /// <summary>A query's SQL, parameters and rows (or the exception translating or running it).</summary>
        static void Q(Probe p, string label, DataContext db, IQueryable query, bool rows = true)
        {
            DbCommand command;
            try { command = db.GetCommand(query); }
            catch (Exception e) { p.Is(label + " translate", "-> " + Probe.Exception(e)); return; }
            p.Text(label + " sql", command.CommandText);
            p.Is(label + " parameters", command.Parameters.Cast<DbParameter>().Select(x => x.ParameterName + "=" + Probe.Format(x.Value) + " " + x.DbType));
            if (rows) p.Try(label + " rows", () => query.Cast<object>().ToList());
        }

        static readonly DateTime Day = new DateTime(2020, 1, 1);

        [Case(Database = true)]
        static void Operators(Probe p)
        {
            using (var db = new ParityDb(Fixture.Connection))
            {
                Q(p, "where", db, db.Customers.Where(c => c.City == "Berlin"));
                Q(p, "where null", db, db.Customers.Where(c => c.City == null));
                Q(p, "where not null", db, db.Customers.Where(c => c.City != null));
                string city = null;
                Q(p, "where variable null", db, db.Customers.Where(c => c.City == city));
                Q(p, "order", db, db.Customers.OrderByDescending(c => c.City).ThenBy(c => c.CustomerID));
                Q(p, "select", db, db.Customers.Select(c => new { c.CustomerID, Upper = c.CompanyName.ToUpper() }));
                Q(p, "take", db, db.Customers.OrderBy(c => c.CustomerID).Take(2));
                Q(p, "skip", db, db.Customers.OrderBy(c => c.CustomerID).Skip(1));
                Q(p, "skip take", db, db.Customers.OrderBy(c => c.CustomerID).Skip(1).Take(2));
                Q(p, "distinct", db, db.Orders.Select(o => o.CustomerID).Distinct());
                Q(p, "group", db, db.Orders.GroupBy(o => o.CustomerID).Select(g => new { g.Key, Count = g.Count(), Sum = g.Sum(o => o.Amount), Max = g.Max(o => o.OrderDate) }));
                Q(p, "join", db, from o in db.Orders join c in db.Customers on o.CustomerID equals c.CustomerID select new { o.OrderID, c.CompanyName });
                Q(p, "group join", db, from c in db.Customers join o in db.Orders on c.CustomerID equals o.CustomerID into os select new { c.CustomerID, Count = os.Count() });
                Q(p, "left join", db, from c in db.Customers from o in c.Orders.DefaultIfEmpty() select new { c.CustomerID, Order = (int?)o.OrderID });
                Q(p, "association", db, db.Orders.Where(o => o.Customer.City == "Berlin"));
                Q(p, "any", db, db.Customers.Where(c => c.Orders.Any(o => o.Amount > 50)));
                Q(p, "all", db, db.Customers.Where(c => c.Orders.All(o => o.Amount > 50)));
                Q(p, "count subquery", db, db.Customers.Select(c => new { c.CustomerID, N = c.Orders.Count }));
                Q(p, "union", db, db.Customers.Select(c => c.City).Union(db.Customers.Select(c => c.CompanyName)));
                Q(p, "concat", db, db.Customers.Select(c => c.CustomerID).Concat(db.Orders.Select(o => o.CustomerID)));
                Q(p, "intersect", db, db.Customers.Select(c => c.CustomerID).Intersect(db.Orders.Select(o => o.CustomerID)));
                Q(p, "except", db, db.Customers.Select(c => c.CustomerID).Except(db.Orders.Select(o => o.CustomerID)));
                Q(p, "local contains", db, db.Customers.Where(c => new[] { "ALFKI", "BONAP" }.Contains(c.CustomerID)));
                Q(p, "local list contains", db, db.Customers.Where(c => new List<string> { "ANTON" }.Contains(c.CustomerID)));
                Q(p, "conditional", db, db.Customers.Select(c => c.City == null ? "(none)" : c.City));
                Q(p, "coalesce", db, db.Customers.Select(c => c.City ?? "(none)"));
                Q(p, "type test", db, db.People.Where(x => x is Employee));
                Q(p, "OfType", db, db.People.OfType<Manager>());
                Q(p, "cast in select", db, db.People.Select(x => x as Employee).Where(e => e != null));
                Q(p, "enum", db, db.Products.Where(x => x.Kind == ProductKind.Toy));
                Q(p, "binary compare", db, db.Products.Where(x => x.Data == new Binary(new byte[] { 1, 2, 3 })));
                Q(p, "bool", db, db.Products.Where(x => !x.Discontinued));

                p.Try("First", () => db.Customers.OrderBy(c => c.CustomerID).First());
                p.Try("First empty", () => db.Customers.First(c => c.CustomerID == "NONE"));
                p.Try("FirstOrDefault empty", () => db.Customers.FirstOrDefault(c => c.CustomerID == "NONE"));
                p.Try("Single", () => db.Customers.Single(c => c.CustomerID == "ALFKI"));
                p.Try("Single many", () => db.Customers.Single());
                p.Try("SingleOrDefault many", () => db.Customers.SingleOrDefault());
                p.Try("Count", () => db.Orders.Count());
                p.Try("LongCount", () => db.Orders.LongCount());
                p.Try("Sum", () => db.Orders.Sum(o => o.Amount));
                p.Try("Average", () => db.Orders.Average(o => o.Amount));
                p.Try("Min Max", () => new object[] { db.Orders.Min(o => o.OrderDate), db.Orders.Max(o => o.Amount) });
                p.Try("Sum empty", () => db.Orders.Where(o => o.OrderID < 0).Sum(o => o.Amount));
                p.Try("Max empty", () => db.Orders.Where(o => o.OrderID < 0).Max(o => o.Amount));
                p.Try("Any", () => db.Customers.Any(c => c.City == "Berlin"));
                p.Try("Contains entity", () => db.Customers.Contains(db.Customers.First()));
                p.Try("ElementAt", () => db.Customers.OrderBy(c => c.CustomerID).ElementAt(1));
                p.Try("Last", () => db.Customers.Last());
                p.Try("Reverse", () => db.Customers.Reverse().ToList());
                p.Try("TakeWhile", () => db.Customers.TakeWhile(c => true).ToList());
                p.Try("SkipWhile", () => db.Customers.SkipWhile(c => true).ToList());
                p.Try("Aggregate", () => db.Customers.Aggregate((a, b) => a));
                p.Try("enumerate twice", () => { var q = db.ExecuteQuery<Customer>("SELECT * FROM dbo.Customers"); q.ToList(); return q.ToList(); });
            }
        }

        [Case(Database = true)]
        static void Expressions(Probe p)
        {
            using (var db = new ParityDb(Fixture.Connection))
            {
                Q(p, "string", db, db.Customers.Select(c => new
                {
                    c.CustomerID,
                    Len = c.CompanyName.Length,
                    Sub = c.CompanyName.Substring(1, 3),
                    Idx = c.CompanyName.IndexOf("o"),
                    Trim = (" " + c.CompanyName + " ").Trim(),
                    Rep = c.CompanyName.Replace("a", "4"),
                    Pad = c.CustomerID.PadLeft(8, '*'),
                    Lower = c.CompanyName.ToLower(),
                    Concat = string.Concat(c.CustomerID, "-", c.CompanyName),
                    Cmp = string.Compare(c.CompanyName, "B"),
                    Empty = string.IsNullOrEmpty(c.City),
                    Insert = c.CompanyName.Insert(1, "_"),
                    Remove = c.CompanyName.Remove(1, 2),
                }));
                Q(p, "contains", db, db.Customers.Where(c => c.CompanyName.Contains("ap")));
                Q(p, "contains wildcard", db, db.Customers.Where(c => c.CompanyName.Contains("%_[")));
                Q(p, "starts", db, db.Customers.Where(c => c.CompanyName.StartsWith("A")));
                Q(p, "ends", db, db.Customers.Where(c => c.CompanyName.EndsWith("s")));
                Q(p, "date", db, db.Orders.Select(o => new { o.OrderID, o.OrderDate.Year, o.OrderDate.Month, o.OrderDate.Day, o.OrderDate.Hour, o.OrderDate.DayOfWeek, o.OrderDate.DayOfYear, Date = o.OrderDate.Date, Added = o.OrderDate.AddDays(1).AddMonths(1).AddHours(2), Span = (o.OrderDate - Day).Days }));
                Q(p, "offset", db, db.Orders.Select(o => new { o.OrderID, o.Stamp.Offset, Utc = o.Stamp.UtcDateTime, Plus = o.Stamp.AddMinutes(30) }));
                Q(p, "nullable date", db, db.Orders.Where(o => o.ShippedDate.HasValue).Select(o => o.ShippedDate.Value.Day));
                Q(p, "math", db, db.Orders.Select(o => new { o.OrderID, Round = Math.Round(o.Amount, 1), Floor = Math.Floor(o.Amount), Ceil = Math.Ceiling(o.Amount), Abs = Math.Abs(-o.Amount), Pow = Math.Pow((double)o.Amount, 2), Sqrt = Math.Sqrt((double)o.Amount), Mod = o.OrderID % 3, Sign = Math.Sign(o.Amount - 50) }));
                Q(p, "round to even", db, db.Orders.Select(o => Math.Round(o.Amount, MidpointRounding.AwayFromZero)));
                Q(p, "convert", db, db.Orders.Select(o => new { S = o.OrderID.ToString(), D = Convert.ToDouble(o.Amount), I = (int)o.Amount, Dec = (decimal)o.OrderID }));
                // A date as text: SQL Server's CONVERT, which writes the month in the server's language (the SQL only).
                Q(p, "date to string", db, db.Orders.Select(o => Convert.ToString(o.OrderDate)), rows: false);
                Q(p, "guid", db, db.Products.Where(x => x.ProductID == Fixture.Kite));
                Q(p, "double", db, db.Products.Where(x => x.Weight > 1.0).Select(x => x.Weight / 3));
                Q(p, "computed column", db, db.Products.Select(x => x.DoubleWeight));
                Q(p, "untranslatable", db, db.Customers.Where(c => c.CompanyName.GetHashCode() == 1));
                Q(p, "local method", db, db.Customers.Where(c => Local(c.CustomerID)));
                Q(p, "local method in select", db, db.Customers.Select(c => Local(c.CustomerID)));
                Q(p, "string format in select", db, db.Customers.Select(c => string.Format("{0}!", c.CustomerID)));
                Q(p, "Like", db, db.Customers.Where(c => SqlMethods.Like(c.CompanyName, "%o%")));
                Q(p, "Like escape", db, db.Customers.Where(c => SqlMethods.Like(c.CompanyName, "C%~-%", '~')));
            }
        }

        static bool Local(string id) => id.StartsWith("A", StringComparison.Ordinal);

        [Case(Database = true)]
        static void SqlMethods_In_A_Query(Probe p)
        {
            using (var db = new ParityDb(Fixture.Connection))
            {
                var d = new DateTime(2020, 1, 15, 10, 30, 0, 500);
                var o = new DateTimeOffset(2020, 1, 15, 10, 30, 0, 500, TimeSpan.FromHours(9));
                DateTime? n = d;
                Q(p, "DateTime", db, db.Orders.Where(x => x.OrderID == 1).Select(x => new
                {
                    Year = SqlMethods.DateDiffYear(x.OrderDate, d), Month = SqlMethods.DateDiffMonth(x.OrderDate, d), Day = SqlMethods.DateDiffDay(x.OrderDate, d),
                    Hour = SqlMethods.DateDiffHour(x.OrderDate, d), Minute = SqlMethods.DateDiffMinute(x.OrderDate, d), Second = SqlMethods.DateDiffSecond(x.OrderDate, d),
                    Milli = SqlMethods.DateDiffMillisecond(x.OrderDate, d), Micro = SqlMethods.DateDiffMicrosecond(x.OrderDate, d), Nano = SqlMethods.DateDiffNanosecond(x.OrderDate, d),
                }));
                Q(p, "DateTimeOffset", db, db.Orders.Where(x => x.OrderID == 1).Select(x => new
                {
                    Year = SqlMethods.DateDiffYear(x.Stamp, o), Month = SqlMethods.DateDiffMonth(x.Stamp, o), Day = SqlMethods.DateDiffDay(x.Stamp, o),
                    Hour = SqlMethods.DateDiffHour(x.Stamp, o), Minute = SqlMethods.DateDiffMinute(x.Stamp, o), Second = SqlMethods.DateDiffSecond(x.Stamp, o),
                    Milli = SqlMethods.DateDiffMillisecond(x.Stamp, o), Micro = SqlMethods.DateDiffMicrosecond(x.Stamp, o), Nano = SqlMethods.DateDiffNanosecond(x.Stamp, o),
                }));
                Q(p, "nullable", db, db.Orders.OrderBy(x => x.OrderID).Select(x => new
                {
                    Year = SqlMethods.DateDiffYear(x.ShippedDate, n), Month = SqlMethods.DateDiffMonth(x.ShippedDate, n), Day = SqlMethods.DateDiffDay(x.ShippedDate, n),
                    Hour = SqlMethods.DateDiffHour(x.ShippedDate, n), Minute = SqlMethods.DateDiffMinute(x.ShippedDate, n), Second = SqlMethods.DateDiffSecond(x.ShippedDate, n),
                    Milli = SqlMethods.DateDiffMillisecond(x.ShippedDate, n),
                }));
                // Micro- and nanoseconds overflow DATEDIFF's int beyond seconds / minutes: rows near the value (and a null).
                DateTime? near = new DateTime(2020, 1, 20, 9, 0, 0, 500);
                Q(p, "nullable fine", db, db.Orders.Where(x => x.OrderID <= 2).OrderBy(x => x.OrderID).Select(x => new
                {
                    Micro = SqlMethods.DateDiffMicrosecond(x.ShippedDate, near), Nano = SqlMethods.DateDiffNanosecond(x.ShippedDate, near),
                }));
                Q(p, "overflow", db, db.Orders.Select(x => SqlMethods.DateDiffNanosecond(x.OrderDate, d)));
                DateTimeOffset? no = o;
                Q(p, "nullable offset", db, db.Orders.Where(x => x.OrderID == 1).Select(x => new
                {
                    Year = SqlMethods.DateDiffYear((DateTimeOffset?)x.Stamp, no), Month = SqlMethods.DateDiffMonth((DateTimeOffset?)x.Stamp, no), Day = SqlMethods.DateDiffDay((DateTimeOffset?)x.Stamp, no),
                    Hour = SqlMethods.DateDiffHour((DateTimeOffset?)x.Stamp, no), Minute = SqlMethods.DateDiffMinute((DateTimeOffset?)x.Stamp, no), Second = SqlMethods.DateDiffSecond((DateTimeOffset?)x.Stamp, no),
                    Milli = SqlMethods.DateDiffMillisecond((DateTimeOffset?)x.Stamp, no), Micro = SqlMethods.DateDiffMicrosecond((DateTimeOffset?)x.Stamp, no), Nano = SqlMethods.DateDiffNanosecond((DateTimeOffset?)x.Stamp, no),
                    Null = SqlMethods.DateDiffDay((DateTimeOffset?)null, no),
                }));
                Q(p, "in a predicate", db, db.Orders.Where(x => SqlMethods.DateDiffDay(x.OrderDate, d) > 10));
            }
        }

        [Case(Database = true)]
        static void Providers_Sql(Probe p)
        {
            using (var db = new ParityDb2000(Fixture.Connection))
            {
                Q(p, "2000 take", db, db.Customers.OrderBy(c => c.CustomerID).Take(2));
                Q(p, "2000 skip", db, db.Customers.OrderBy(c => c.CustomerID).Skip(1));
                Q(p, "2000 skip take", db, db.Customers.OrderBy(c => c.CustomerID).Skip(1).Take(2));
                Q(p, "2000 max text", db, db.Products.Select(x => x.Description));
                Q(p, "2000 datetimeoffset", db, db.Orders.Select(x => x.Stamp));
                Q(p, "2000 apply", db, from c in db.Customers from o in c.Orders.OrderBy(x => x.OrderID).Take(1) select o.OrderID);
            }
            using (var db = new ParityDb2005(Fixture.Connection))
            {
                Q(p, "2005 skip take", db, db.Customers.OrderBy(c => c.CustomerID).Skip(1).Take(2));
                Q(p, "2005 datetimeoffset", db, db.Orders.Select(x => x.Stamp));
                Q(p, "2005 apply", db, from c in db.Customers from o in c.Orders.OrderBy(x => x.OrderID).Take(1) select o.OrderID);
            }
            using (var db = new ParityDb(Fixture.Connection))
            {
                Q(p, "2008 skip take", db, db.Customers.OrderBy(c => c.CustomerID).Skip(1).Take(2));
                Q(p, "2008 apply", db, from c in db.Customers from o in c.Orders.OrderBy(x => x.OrderID).Take(1) select o.OrderID);
            }
        }

        [Case(Database = true)]
        static void Inheritance(Probe p)
        {
            using (var db = new ParityDb(Fixture.Connection))
            {
                // Kind X has no mapping: the default type (Person).
                Q(p, "all", db, db.People.OrderBy(x => x.PersonID));
                Q(p, "employees", db, db.People.OfType<Employee>().Select(e => new { e.Name, e.Salary }));
                Q(p, "discriminator", db, db.People.Where(x => x.Kind == "M"));
                p.Try("types", () => db.People.OrderBy(x => x.PersonID).AsEnumerable().Select(x => x.GetType().Name).ToList());
                db.People.InsertOnSubmit(new Manager { PersonID = 10, Kind = "M", Name = "New", Salary = 1, Reports = 0 });
                db.SubmitChanges();
                p.Is("inserted", Fixture.Rows("SELECT PersonID, Kind, Name, Salary, Reports FROM dbo.People WHERE PersonID = 10"));
                var eve = db.People.OfType<Employee>().Single(e => e.PersonID == 2);
                p.Does("change discriminator", () => { eve.Kind = "P"; db.SubmitChanges(); });
            }
        }

        [Case(Database = true)]
        static void Composable_Functions(Probe p)
        {
            using (var db = new ParityDb(Fixture.Connection))
            {
                Q(p, "scalar in query", db, db.Customers.Select(c => new { c.CustomerID, N = db.OrderCount(c.CustomerID) }));
                Q(p, "table-valued", db, db.OrdersAbove(50m));
                Q(p, "table-valued composed", db, db.OrdersAbove(10m).Where(o => o.CustomerID == "ALFKI").OrderBy(o => o.OrderID));
                p.Try("scalar direct", () => db.OrderCount("ALFKI"));
                p.Try("scalar null", () => db.OrderCount(null));
            }
        }
    }
}
