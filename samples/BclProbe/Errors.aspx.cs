using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace BclProbe
{
    /// <summary>
    /// The messages of the exceptions applications catch and show (lblError.Text = ex.Message): parsing, collections,
    /// arguments, casts, LINQ, files (paths relative to the application, so the text has no machine's folder), DataTable,
    /// and what ToString() of an exception starts with. The message is what the user saw.
    /// </summary>
    public partial class Errors : ProbePage
    {
        enum Color { Red, Green }

        protected override void Run()
        {
            Section("parsing");
            Case("int.Parse abc", () => int.Parse("abc"));
            Case("int.Parse empty", () => int.Parse(""));
            Case("int.Parse null", () => int.Parse(null));
            Case("int.Parse overflow", () => int.Parse("99999999999"));
            Case("byte.Parse 300", () => byte.Parse("300"));
            Case("double.Parse x", () => double.Parse("1.2.3"));
            Case("decimal.Parse x", () => decimal.Parse("12a"));
            Case("Convert.ToInt32 x", () => Convert.ToInt32("x1"));
            Case("Convert.ToInt32 obj", () => Convert.ToInt32(new object()));
            Case("Convert.ToBoolean", () => Convert.ToBoolean("yes"));
            Case("bool.Parse", () => bool.Parse("1"));
            Case("DateTime.Parse", () => DateTime.Parse("31/31/2026"));
            Case("DateTime.ParseExact", () => DateTime.ParseExact("2026-13-01", "yyyy-MM-dd", CultureInfo.InvariantCulture));
            Case("DateTime.Parse empty", () => DateTime.Parse(""));
            Case("TimeSpan.Parse", () => TimeSpan.Parse("25:00:00"));
            Case("Guid.Parse", () => Guid.Parse("not-a-guid"));
            Case("Guid ctor", () => new Guid("1234"));
            Case("Enum.Parse", () => Enum.Parse(typeof(Color), "Blue"));
            Case("Enum.Parse empty", () => Enum.Parse(typeof(Color), ""));
            Case("Uri", () => new Uri("not a uri"));
            Case("Uri relative", () => new Uri("/a/b"));
            Case("Version.Parse", () => Version.Parse("1.x"));
            Case("char.Parse", () => char.Parse("ab"));

            Section("collections");
            var dictionary = new Dictionary<string, int> { { "a", 1 } };
            Case("Dictionary missing", () => dictionary["missing"]);
            Case("Dictionary duplicate", () => { var d = new Dictionary<string, int> { { "k", 1 } }; d.Add("k", 2); return d.Count; });
            Case("Dictionary null key", () => dictionary[null]);
            Case("Hashtable duplicate", () => { var h = new Hashtable { { "k", 1 } }; h.Add("k", 2); return h.Count; });
            Case("SortedList duplicate", () => { var s = new SortedList<int, int> { { 1, 1 } }; s.Add(1, 2); return s.Count; });
            Case("HashSet ok", () => new HashSet<int> { 1, 1 }.Count);
            Case("List index", () => new List<int> { 1 }[3]);
            Case("List RemoveAt", () => { new List<int>().RemoveAt(0); return 0; });
            Case("List Insert", () => { new List<int>().Insert(2, 1); return 0; });
            Case("List GetRange", () => new List<int> { 1, 2 }.GetRange(1, 5));
            Case("array index", () => { var a = new int[2]; var i = 5; return a[i]; });
            Case("array negative", () => { var n = -1; return new int[n]; });
            Case("ArrayList index", () => new ArrayList()[0]);
            Case("modified in foreach", () => { var l = new List<int> { 1, 2 }; foreach (var i in l) l.Add(i); return 0; });
            Case("Dictionary modified", () => { var d = new Dictionary<int, int> { { 1, 1 }, { 2, 2 } }; foreach (var k in d.Keys) d.Remove(k); return d.Count; });
            Case("Dictionary modified add", () => { var d = new Dictionary<int, int> { { 1, 1 }, { 2, 2 } }; foreach (var k in d.Keys) d[k + 10] = 0; return d.Count; });
            Case("Queue empty", () => new Queue<int>().Dequeue());
            Case("Stack empty", () => new Stack<int>().Pop());
            Case("ReadOnly", () => { IList<int> l = new List<int> { 1 }.AsReadOnly(); l.Add(2); return 0; });
            Case("array as IList Add", () => { IList<int> l = new int[1]; l.Add(1); return 0; });
            Case("Array.Copy", () => { Array.Copy(new int[2], new int[1], 2); return 0; });
            Case("CopyTo", () => { new List<int> { 1, 2 }.CopyTo(new int[1]); return 0; });

            Section("LINQ");
            var empty = new List<int>();
            Case("First empty", () => empty.First());
            Case("First predicate", () => new[] { 1 }.First(i => i > 5));
            Case("Single many", () => new[] { 1, 2 }.Single());
            Case("Single empty", () => empty.Single());
            Case("Max empty", () => empty.Max());
            Case("Average empty", () => empty.Average());
            Case("ElementAt", () => new[] { 1 }.ElementAt(3));
            Case("Cast", () => new object[] { "a" }.Cast<int>().ToList());
            Case("ToDictionary duplicate", () => new[] { "a", "a" }.ToDictionary(s => s));
            Case("Sum overflow", () => new[] { int.MaxValue, 1 }.Sum());
            Case("null source", () => ((IEnumerable<int>)null).Count());
            Case("Max nullable empty", () => new List<int?>().Max());

            Section("arguments and casts");
            Case("Substring", () => "abc".Substring(5));
            Case("Substring length", () => "abc".Substring(1, 9));
            Case("PadLeft", () => "a".PadLeft(-1));
            Case("new string", () => new string('a', -1));
            Case("string.Join null", () => string.Join(",", (string[])null));
            Case("Replace empty", () => "abc".Replace("", "x"));
            Case("ArgumentNull", () => { throw new ArgumentNullException("customer"); });
            Case("ArgumentNull message", () => { throw new ArgumentNullException("customer", "A customer is required."); });
            Case("ArgumentOutOfRange", () => { throw new ArgumentOutOfRangeException("age", 200, "Too old."); });
            Case("ArgumentException param", () => { throw new ArgumentException("Bad value.", "code"); });
            Case("ArgumentException no message", () => { throw new ArgumentException(); });
            Case("InvalidCast", () => (int)(object)"a");
            Case("InvalidCast long", () => (int)(object)5L);
            Case("InvalidCast null unbox", () => (int)(object)null);
            Case("NullReference", () => ((string)null).Length);
            Case("DivideByZero", () => { int z = 0; return 1 / z; });
            Case("decimal DivideByZero", () => { decimal z = 0; return 1m / z; });
            Case("Overflow checked", () => { int m = int.MaxValue; return checked(m + 1); });
            Case("Overflow decimal", () => { var m = decimal.MaxValue; return m * 2; });
            Case("Overflow Convert", () => Convert.ToByte(256));
            Case("Overflow array", () => new byte[long.MaxValue]);
            Case("Nullable no value", () => ((int?)null).Value);
            Case("InvalidOperation", () => { throw new InvalidOperationException(); });
            Case("NotSupported", () => { throw new NotSupportedException(); });
            Case("NotImplemented", () => { throw new NotImplementedException(); });
            Case("Exception no message", () => { throw new Exception(); });
            Case("ApplicationException", () => { throw new ApplicationException(); });
            Case("Format of string.Format", () => string.Format("{x}", 1));
            Case("KeyNotFound plain", () => { throw new KeyNotFoundException(); });
            Case("IndexOutOfRange plain", () => { throw new IndexOutOfRangeException(); });
            Case("ObjectDisposed", () => { var s = new MemoryStream(); s.Dispose(); return s.ReadByte(); });
            Case("Timeout", () => { throw new TimeoutException(); });
            Case("UnauthorizedAccess", () => { throw new UnauthorizedAccessException(); });
            Case("HttpException", () => { throw new HttpException(404, "Not here"); });
            Case("HttpException plain", () => { throw new HttpException(); });
            Case("Activator", () => Activator.CreateInstance(typeof(Errors).Assembly.FullName, "BclProbe.Nothing"));
            Case("Type.GetType throw", () => Type.GetType("No.Such.Type, NoAssembly", true));

            Section("files");
            var root = HttpRuntime.AppDomainAppPath;
            Case("File missing", () => Hide(() => File.ReadAllText(Path.Combine(root, "App_Data", "missing.txt")), root));
            Case("Directory missing", () => Hide(() => Directory.GetFiles(Path.Combine(root, "App_Data", "nowhere")), root));
            Case("Path illegal", () => Path.GetFileName("a|b"));
            Case("Path.Combine null", () => Path.Combine("a", null));
            Case("Path.GetExtension", () => Path.GetExtension("a.b/c"));
            Case("Path.GetFullPath empty", () => Path.GetFullPath(""));
            Case("Path.ChangeExtension", () => Path.ChangeExtension("a.txt", ".bak"));
            Case("Path.GetInvalidFileNameChars", () => Path.GetInvalidFileNameChars().Length);
            Case("Path.HasExtension", () => Path.HasExtension("a."));
            Case("MapPath outside", () => Hide(() => HttpContext.Current.Server.MapPath("../../x"), root));
            Case("MapPath ok", () => Hide(() => HttpContext.Current.Server.MapPath("~/App_Data/x.txt"), root));
            Case("StreamReader missing", () => Hide(() => new StreamReader(Path.Combine(root, "nope.txt")), root));

            Section("DataTable");
            var table = new DataTable("t");
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.PrimaryKey = new[] { table.Columns["Id"] };
            table.Rows.Add(1, "a");
            Case("column missing", () => table.Rows[0]["Nope"]);
            Case("row index", () => table.Rows[5]);
            Case("duplicate key", () => table.Rows.Add(1, "b"));
            Case("wrong type", () => table.Rows.Add("x", "b"));
            Case("null in key", () => table.Rows.Add(null, "c"));
            Case("Select syntax", () => table.Select("Name = "));
            Case("Compute", () => table.Compute("Sum(Id)", ""));
            Case("Compute bad", () => table.Compute("Sum(Name)", ""));
            Case("DBNull cast", () => { var t = new DataTable(); t.Columns.Add("x", typeof(int)); t.Rows.Add(DBNull.Value); return (int)t.Rows[0]["x"]; });
            Case("Field<int> DBNull", () => { var t = new DataTable(); t.Columns.Add("x", typeof(int)); t.Rows.Add(DBNull.Value); return t.Rows[0].Field<int>("x"); });
            Case("DataView RowFilter", () => new DataView(table, "Id > 0", "Name DESC", DataViewRowState.CurrentRows).Count);
            Case("Constraint", () => { var t = new DataTable(); var c = t.Columns.Add("x", typeof(string)); c.AllowDBNull = false; t.Rows.Add((object)null); return 0; });
            Case("MaxLength", () => { var t = new DataTable(); var c = t.Columns.Add("x", typeof(string)); c.MaxLength = 2; t.Rows.Add("abc"); return 0; });
            Case("Expression column", () => { var t = new DataTable(); t.Columns.Add("a", typeof(double)); t.Columns.Add("b", typeof(double), "a * 1.1"); t.Rows.Add(0.1 + 0.2); return t.Rows[0]["b"]; });
            Case("Convert in Select", () => table.Select("Convert(Id, 'System.String') = '1'").Length);

            Section("ToString of exceptions (first line)");
            Case("ToString", () => FirstLine(new InvalidOperationException("outer", new FormatException("inner"))));
            Case("ToString aggregate", () => FirstLine(new AggregateException(new Exception("one"), new Exception("two"))));
            Case("Aggregate message", () => new AggregateException(new Exception("one"), new Exception("two")).Message);
            Case("Aggregate empty", () => new AggregateException().Message);
            Case("TargetInvocation", () => { try { typeof(Errors).GetMethod("Boom", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(null, null); return "no"; } catch (Exception ex) { return ex.GetType().Name + ": " + ex.Message + " / " + ex.InnerException?.Message; } });
            Case("TypeInitialization", () => { try { return Broken.Value; } catch (Exception ex) { return ex.GetType().Name + ": " + ex.Message.Replace(typeof(Broken).FullName, "{type}") + " / " + ex.InnerException?.Message; } });
            Case("Source", () => { try { int.Parse("x"); return ""; } catch (Exception ex) { return ex.Source; } });
            Case("HResult", () => { try { int.Parse("x"); return 0; } catch (Exception ex) { return ex.HResult; } });
            Case("Data", () => new Exception().Data.Count);
        }

        static void Boom() { throw new InvalidOperationException("boom"); }

        static class Broken
        {
            public static readonly int Value = int.Parse("broken");
        }

        static string FirstLine(Exception ex) => ex.ToString().Split('\n')[0].TrimEnd('\r');

        static object Hide(Func<object> run, string root)
        {
            try
            {
                return Show(run()).Replace(root.TrimEnd('\\', '/'), "{app}").Replace('\\', '/');
            }
            catch (Exception ex)
            {
                return "!" + ex.GetType().FullName + ": " + ex.Message.Replace(root.TrimEnd('\\', '/'), "{app}").Replace('\\', '/');
            }
        }
    }
}
