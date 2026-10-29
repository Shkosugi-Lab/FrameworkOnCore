// The round-trip tests' values: compiled into the .NET Framework 4.8 recorder (golden/Recorder.csproj, an assembly of the
// tests' name and key: the user types below have the same identity in both) and into the port's tests (RoundTripTests),
// which make the same values from the same seeds (Fuzz's Rng). Show writes a value as text the same way on both runtimes
// (types by their full names without assemblies, numbers by their bits, unordered collections sorted): two values are the
// same when their Show is.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using FrameworkOnCore.Runtime.Tests.Fuzz;

namespace FrameworkOnCore.Runtime.Tests.RoundTrip
{
    // ---- user types (an application's: [Serializable], ISerializable, callbacks, inheritance, generics, a TypeConverter) ----

    public enum Shade { Red, Green, Blue = 10 }

    [Flags]
    public enum Access : byte { None = 0, Read = 1, Write = 2, Execute = 4, All = 7 }

    public enum Wide : long { Min = long.MinValue, Zero = 0, Max = long.MaxValue }

    public enum Tiny : sbyte { Low = -128, High = 127 }

    [Serializable]
    public class Person
    {
        public string Name;
        public int Age;
        public DateTime Born;
        public object Tag;
        public Person Friend;
        public List<int> Numbers;
        public int? Maybe;
        public Shade Shade;
        private decimal id;

        public decimal Id { get { return id; } set { id = value; } }
    }

    [Serializable]
    public class Employee : Person
    {
        private string id = "employee";   // the same name as the base class's private field
        public string Title;
        public Access Access;

        public string EmployeeId { get { return id; } set { id = value; } }
    }

    [Serializable]
    public struct Point3
    {
        public int X, Y, Z;
        public Point3(int x, int y, int z) { X = x; Y = y; Z = z; }
    }

    [Serializable]
    public class Node
    {
        public object Value;
        public Node Next;
        public Node Other;
    }

    [Serializable]
    public sealed class Custom : ISerializable
    {
        public string A;
        public int B;
        public object Inner;

        public Custom() { }

        Custom(SerializationInfo info, StreamingContext context)
        {
            A = info.GetString("a");
            B = info.GetInt32("b");
            Inner = info.GetValue("inner", typeof(object));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("a", A);
            info.AddValue("b", B);
            info.AddValue("inner", Inner);
        }
    }

    [Serializable]
    public class WithCallbacks
    {
        public int X;
        [NonSerialized] public int Transient;
        [NonSerialized] public string Computed;

        [OnDeserialized]
        void Done(StreamingContext context) { Computed = "done:" + X.ToString(CultureInfo.InvariantCulture); }
    }

    [Serializable]
    public class Box<T>
    {
        public T Value;
    }

    /// <summary>A type converted to and from a string by its TypeConverter (ObjectStateFormatter, ResX and profiles take that way).</summary>
    [Serializable]
    [TypeConverter(typeof(MoneyConverter))]
    public class Money
    {
        public decimal Amount;
        public string Currency;
    }

    public class MoneyConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = value as string;
            if (s == null) return base.ConvertFrom(context, culture, value);
            int space = s.IndexOf(' ');
            return new Money { Amount = decimal.Parse(s.Substring(0, space), CultureInfo.InvariantCulture), Currency = s.Substring(space + 1) };
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Money m) return m.Amount.ToString(CultureInfo.InvariantCulture) + " " + m.Currency;
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>The values: the edge values first (Fixed), then values made from the seed (nested, of every kind).</summary>
    public static class Values
    {
        /// <summary>The cases each format is given.</summary>
        public const int Count = 420;

        /// <summary>The value of a case.</summary>
        public static object Make(int index)
        {
            var fixedValues = Fixed();
            if (index < fixedValues.Count) return fixedValues[index]();
            var r = new Rng(0x526F756E64547269UL ^ ((ulong)index * 0x9E3779B97F4A7C15UL));
            return Any(r, 0);
        }

        static List<Func<object>> fixedCache;

        /// <summary>The edge values, each made anew when asked (a case's value is not shared with another's).</summary>
        public static List<Func<object>> Fixed()
        {
            if (fixedCache != null) return fixedCache;
            var list = new List<Func<object>>();
            void Add(params Func<object>[] makes) => list.AddRange(makes);
            // primitives
            Add(() => null, () => true, () => false, () => (byte)0, () => byte.MaxValue, () => sbyte.MinValue, () => sbyte.MaxValue,
                () => short.MinValue, () => short.MaxValue, () => ushort.MaxValue, () => 0, () => -1, () => int.MinValue, () => int.MaxValue,
                () => uint.MaxValue, () => long.MinValue, () => long.MaxValue, () => ulong.MaxValue, () => (ulong)0,
                () => 'a', () => '\0', () => '\uD800', () => '\uDFFF', () => '\uFFFF', () => '\uFFFE', () => 'あ', () => '\u0085');
            // floating point: zeros, NaNs (with payloads), infinities, subnormals, ones that print shorter than they are
            Add(() => 0.0, () => -0.0, () => double.NaN, () => BitConverter.Int64BitsToDouble(unchecked((long)0xFFF8000000000000UL)),
                () => BitConverter.Int64BitsToDouble(0x7FF0000000000001L), () => double.PositiveInfinity, () => double.NegativeInfinity,
                () => double.Epsilon, () => double.MaxValue, () => double.MinValue, () => 0.1, () => 0.1 + 0.2, () => 1.0 / 3, () => 1e-310,
                () => 123456789.123456789, () => -1.5e300, () => 0.0f, () => -0.0f, () => float.NaN, () => float.Epsilon, () => float.MaxValue,
                () => float.MinValue, () => 0.1f, () => 16777217f, () => float.NegativeInfinity);
            // decimal: scales (trailing zeros), negative zero, the extremes
            Add(() => 0m, () => 0.0m, () => 0.00m, () => new decimal(0, 0, 0, true, 0), () => new decimal(0, 0, 0, true, 3), () => 1m, () => 1.000m,
                () => decimal.MaxValue, () => decimal.MinValue, () => 0.0000000000000000000000000001m, () => 7.9228162514264337593543950335m,
                () => 123.4500m, () => -98765.4321000m);
            // dates and times: the extremes, kinds, ticks below a millisecond
            Add(() => DateTime.MinValue, () => DateTime.MaxValue, () => new DateTime(2026, 10, 30, 1, 2, 3, 456, DateTimeKind.Utc),
                () => new DateTime(2026, 10, 30, 1, 2, 3, 456, DateTimeKind.Local), () => new DateTime(2026, 10, 30, 1, 2, 3, 456, DateTimeKind.Unspecified),
                () => new DateTime(636000000000000001L, DateTimeKind.Utc), () => new DateTime(0, DateTimeKind.Utc), () => new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc),
                () => new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local).AddTicks(1),
                () => DateTimeOffset.MinValue, () => DateTimeOffset.MaxValue, () => new DateTimeOffset(2026, 10, 30, 1, 2, 3, 456, TimeSpan.FromHours(14)),
                () => new DateTimeOffset(2026, 10, 30, 1, 2, 3, TimeSpan.FromHours(-14)), () => new DateTimeOffset(636000000000000001L, new TimeSpan(5, 45, 0)),
                () => TimeSpan.Zero, () => TimeSpan.MinValue, () => TimeSpan.MaxValue, () => new TimeSpan(1), () => new TimeSpan(-1),
                () => new TimeSpan(1, 2, 3, 4, 567).Add(new TimeSpan(89)));
            // Guid
            Add(() => Guid.Empty, () => new Guid("01234567-89ab-cdef-0123-456789abcdef"), () => new Guid(Enumerable.Repeat((byte)0xFF, 16).ToArray()));
            // strings: empty, white space, line breaks, XML's specials, NUL, lone surrogates, non-characters, long
            Add(() => "", () => " ", () => "  lead and trail  ", () => "\t", () => "\r\n", () => "\n", () => "\r", () => "a\r\nb\nc\rd",
                () => "<data name=\"x\"><value>y</value></data>", () => "]]>", () => "&amp;&lt;", () => "\0", () => "a\0b", () => "\uD800",
                () => "\uDC00x", () => "\U0001F600", () => "\uFFFE\uFFFF", () => "\u0001\u001F\u007F\u0085\u2028", () => "ユーザー名 漢字 ｶﾀｶﾅ",
                () => new string('a', 70000), () => AllChars(0, 0x2FF) + AllChars(0xD7F0, 0xE010) + AllChars(0xFFF0, 0xFFFF));
            // enums (of mscorlib, of System.Web, of the application; undefined values, flags)
            Add(() => DayOfWeek.Friday, () => (DayOfWeek)99, () => Shade.Blue, () => (Shade)(-5), () => Access.Read | Access.Write, () => (Access)0x80,
                () => Wide.Min, () => Wide.Max, () => Tiny.Low, () => BorderStyle.Dashed, () => FontSize.XLarge,
                () => CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth, () => UnitType.Percentage);
            // arrays: empty, of primitives, multi-dimensional, jagged, with a lower bound, of structs and objects
            Add(() => new int[0], () => new[] { 1, -2, int.MaxValue }, () => new byte[0], () => Enumerable.Range(0, 256).Select(i => (byte)i).ToArray(),
                () => Bytes(new Rng(7), 20000), () => new string[] { null, "", "a", "\uD800" }, () => new object[] { null, 1, "s", 2.5, new object[] { 'c' } },
                () => new[,] { { 1, 2, 3 }, { 4, 5, 6 } }, () => new int[0, 3], () => new string[,] { { "a", null }, { "", "b" } },
                () => new int[][] { new[] { 1 }, null, new int[0], new[] { 2, 3 } }, () => new object[][] { new object[] { "x" }, null },
                () => { var a = Array.CreateInstance(typeof(int), new[] { 3 }, new[] { -2 }); a.SetValue(7, -2); a.SetValue(9, 0); return a; },
                () => new[] { double.NaN, -0.0, 1e308 }, () => new[] { DateTime.MinValue, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                () => new[] { 'a', '\uD800', '\0' }, () => new[] { true, false }, () => new[] { 1.5m, -0.00m }, () => new[] { Guid.Empty },
                () => new[] { TimeSpan.MaxValue }, () => new[] { Color.Red, Color.FromArgb(1, 2, 3, 4) }, () => new[] { Unit.Pixel(3), Unit.Empty },
                () => new int?[] { 1, null, 3 }, () => new long[] { long.MinValue }, () => new float[] { float.NaN }, () => new IndexedString[] { new IndexedString("i") },
                () => new Shade[] { Shade.Green, (Shade)7 });
            // System.Web's and the collections ObjectStateFormatter writes itself
            Add(() => new ArrayList(), () => new ArrayList { null, 1, "a", new ArrayList { 2 } }, () => new Hashtable(),
                () => new Hashtable { { "a", 1 }, { "b", null }, { "c", "x" } }, () => new Hashtable { { 1, "one" }, { 2, "two" }, { 300, new ArrayList() } },
                () => new Hashtable { { "k", 1 }, { 2, "v" }, { Shade.Red, 3.5 } }, () => Hybrid(3, false), () => Hybrid(12, false), () => Hybrid(4, true),
                () => new Pair(), () => new Pair("a", new Pair(1, null)), () => new Triplet("x", null, new Triplet(1, 2, 3)), () => new IndexedString("idx"),
                () => new ArrayList { new IndexedString("r"), new IndexedString("r"), new IndexedString("s"), new IndexedString("r") },
                () => new Pair(new IndexedString("p"), new IndexedString("p")));
            // colors, units, font units
            Add(() => Color.Red, () => Color.FromArgb(1, 2, 3, 4), () => Color.FromArgb(0), () => Color.Empty, () => Color.Transparent,
                () => Color.FromKnownColor(KnownColor.ControlText), () => Color.FromName("NoSuchColor"), () => Color.FromArgb(255, 255, 0, 0),
                () => Unit.Empty, () => Unit.Pixel(10), () => Unit.Percentage(12.5), () => Unit.Point(3), () => new Unit(1.5, UnitType.Em), () => new Unit(2, UnitType.Ex),
                () => new Unit(7.25, UnitType.Mm), () => new Unit(0.5, UnitType.Cm), () => new Unit(1, UnitType.Inch), () => new Unit(3, UnitType.Pica),
                () => Unit.Pixel(-5), () => new Unit(0.001, UnitType.Cm), () => FontUnit.Empty, () => FontUnit.Small, () => FontUnit.XXLarge, () => FontUnit.Smaller,
                () => FontUnit.Larger, () => FontUnit.Point(12), () => new FontUnit(Unit.Percentage(150)), () => new FontUnit(1.25, UnitType.Em));
            // types, cultures, URIs, System.Drawing's structs, Version, DBNull
            Add(() => typeof(int), () => typeof(string), () => typeof(int[]), () => typeof(int[,]), () => typeof(List<string>), () => typeof(Dictionary<string, List<int>>),
                () => typeof(Person), () => typeof(Page), () => typeof(Color), () => typeof(int?), () => typeof(Box<Person>), () => typeof(Box<>),
                () => CultureInfo.InvariantCulture, () => new CultureInfo("en-US"), () => new CultureInfo("ja-JP"), () => new CultureInfo("fr-FR"),
                () => new CultureInfo("tr-TR"), () => new CultureInfo("ar-SA"), () => new CultureInfo("zh-CN"), () => new CultureInfo("de-DE", false),
                () => CultureInfo.GetCultureInfo("ja-JP"),
                () => new Uri("http://example.com/a%20b?q=1#f"), () => new Uri("http://例え.jp/パス?キー=値"), () => new Uri("a/b?c=d", UriKind.Relative),
                () => new Uri("file:///C:/x/y.txt"), () => new Uri("mailto:a@b.c"), () => new Uri("http://u:p@h:8080/p/../q/%2F/"),
                () => new Point(1, -2), () => new Size(3, 4), () => new Rectangle(1, 2, 3, 4), () => new Version(1, 2, 3, 4), () => new Version(1, 0),
                () => DBNull.Value);
            // the application's types: graphs with shared references and cycles
            Add(() => new Money { Amount = 12.50m, Currency = "JPY" }, () => new Point3(1, 2, 3), () => PersonGraph(), () => Ring(3), () => Ring(1),
                () => new Employee { Name = "e", EmployeeId = "E-1", Id = 5m, Title = "t", Access = Access.All }, () => new Custom { A = "a", B = 2, Inner = new Point3(4, 5, 6) },
                () => new WithCallbacks { X = 3, Transient = 9, Computed = "before" }, () => new Box<Person> { Value = new Person { Name = "boxed" } },
                () => new Box<int?> { Value = null }, () => new Box<int?> { Value = 4 }, () => new List<string> { "a", null, "" },
                () => new List<object> { 1, "x", null, new Person { Name = "in list" } }, () => new Dictionary<string, object> { { "a", 1 }, { "b", new[] { 1, 2 } }, { "c", null } },
                () => new Dictionary<int, Person> { { 1, new Person { Name = "one" } } }, () => SharedArray(), () => Names(), () => DataTable(false),
                () => DataTable(true), () => DataSet());
            fixedCache = list;
            return list;
        }

        static string AllChars(int from, int to)
        {
            var sb = new StringBuilder();
            for (int c = from; c <= to; c++) sb.Append((char)c);
            return sb.ToString();
        }

        static byte[] Bytes(Rng r, int length)
        {
            var bytes = new byte[length];
            for (int i = 0; i < length; i++) bytes[i] = (byte)r.Next();
            return bytes;
        }

        static HybridDictionary Hybrid(int count, bool caseInsensitive)
        {
            var d = new HybridDictionary(caseInsensitive);
            for (int i = 0; i < count; i++) d.Add("key" + i.ToString(CultureInfo.InvariantCulture), i % 3 == 0 ? null : (object)i);
            return d;
        }

        public static Person PersonGraph()
        {
            var a = new Person { Name = "a", Age = 30, Born = new DateTime(1990, 5, 6, 0, 0, 0, DateTimeKind.Utc), Numbers = new List<int> { 1, 2 }, Maybe = null, Shade = Shade.Green, Id = 1.50m };
            var b = new Person { Name = "b", Friend = a, Maybe = 7, Tag = a };   // b.Tag and b.Friend: the same object
            a.Friend = b;                                                        // a cycle
            a.Tag = new object[] { a, b, "t" };
            return a;
        }

        public static Node Ring(int count)
        {
            var first = new Node { Value = 0 };
            var node = first;
            for (int i = 1; i < count; i++) node = node.Next = new Node { Value = "n" + i.ToString(CultureInfo.InvariantCulture), Other = first };
            node.Next = first;
            return first;
        }

        static Person[] SharedArray()
        {
            var p = new Person { Name = "shared" };
            return new[] { p, p, null, new Person { Name = "other", Friend = p } };
        }

        static NameValueCollection Names() => new NameValueCollection { { "a", "1" }, { "a", "2" }, { "b", null }, { null, "nokey" }, { "ユ", "\uD800" } };

        static DataTable DataTable(bool binary)
        {
            var t = new DataTable("Orders");
            t.Columns.Add("Id", typeof(int));
            t.Columns.Add("Name", typeof(string));
            t.Columns.Add("When", typeof(DateTime));
            t.Columns.Add("Price", typeof(decimal));
            t.Columns.Add("Data", typeof(byte[]));
            t.Columns.Add("Key", typeof(Guid));
            t.Rows.Add(1, "first", new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc), 1.50m, new byte[] { 1, 2 }, Guid.Empty);
            t.Rows.Add(2, DBNull.Value, DBNull.Value, -0.001m, DBNull.Value, new Guid("01234567-89ab-cdef-0123-456789abcdef"));
            t.Rows.Add(3, "ユーザー", new DateTime(2000, 1, 1), 0m, new byte[0], Guid.Empty);
            t.AcceptChanges();
            t.Rows[0]["Name"] = "changed";   // a modified row (its original and current versions)
            if (binary) t.RemotingFormat = SerializationFormat.Binary;
            return t;
        }

        static DataSet DataSet()
        {
            var s = new DataSet("Shop");
            var orders = DataTable(false);
            var lines = new DataTable("Lines");
            lines.Columns.Add("OrderId", typeof(int));
            lines.Columns.Add("Qty", typeof(short));
            lines.Rows.Add(1, (short)3);
            lines.Rows.Add(2, (short)-1);
            s.Tables.Add(orders);
            s.Tables.Add(lines);
            s.Relations.Add("OrderLines", orders.Columns["Id"], lines.Columns["OrderId"]);
            return s;
        }

        // ---- values made from a seed ----

        /// <summary>A value of any kind; containers hold values of any kind (up to a depth).</summary>
        internal static object Any(Rng r, int depth)
        {
            int kind = r.Below(depth >= 3 ? 14 : 22);
            switch (kind)
            {
                case 0: return null;
                case 1: return Int(r);
                case 2: return Text(r);
                case 3: return Floating(r);
                case 4: return DecimalValue(r);
                case 5: return Date(r);
                case 6: return r.Chance(50) ? (object)new TimeSpan((long)r.Next()) : new DateTimeOffset(Date(r).Ticks % (DateTime.MaxValue.Ticks - 2 * TimeSpan.TicksPerDay) + TimeSpan.TicksPerDay, TimeSpan.FromMinutes(r.Below(28 * 60 + 1) - 14 * 60));
                case 7: return r.Chance(50) ? (object)new Guid(Bytes(r, 16)) : (char)r.Below(0x10000);
                case 8: return r.Chance(50) ? (object)(r.Chance(50)) : (byte)r.Below(256);
                case 9: return r.Pick(new object[] { Shade.Green, (Shade)r.Below(20), (Access)r.Below(256), DayOfWeek.Monday, BorderStyle.Inset, (Wide)(long)r.Next() });
                case 10: return Bytes(r, r.Small(300));
                case 11: return r.Chance(50) ? (object)Color.FromArgb((int)r.Next()) : Color.FromKnownColor((KnownColor)(1 + r.Below(174)));
                case 12: return r.Chance(60) ? (object)new Unit(Math.Round((r.Below(20000) - 10000) / 100.0, 2), (UnitType)(1 + r.Below(9))) : new FontUnit((FontSize)r.Below(11));
                case 13: return new IndexedString(Text(r) + "i");
                case 14: { var list = new ArrayList(); int n = r.Small(5); for (int i = 0; i < n; i++) list.Add(Any(r, depth + 1)); return list; }
                case 15:
                    {
                        var table = new Hashtable();
                        int n = r.Small(5);
                        for (int i = 0; i < n; i++)
                        {
                            object key = r.Chance(60) ? (object)("k" + r.Below(1000).ToString(CultureInfo.InvariantCulture) + Text(r)) : r.Below(1000);
                            table[key] = Any(r, depth + 1);
                        }
                        return table;
                    }
                case 16: return r.Chance(50) ? (object)new Pair(Any(r, depth + 1), Any(r, depth + 1)) : new Triplet(Any(r, depth + 1), Any(r, depth + 1), Any(r, depth + 1));
                case 17: { int n = r.Small(4); var a = new object[n]; for (int i = 0; i < n; i++) a[i] = Any(r, depth + 1); return a; }
                case 18:
                    {
                        var p = new Person { Name = Text(r), Age = r.Below(120), Born = Date(r), Tag = Any(r, depth + 1), Maybe = r.Chance(50) ? (int?)null : r.Below(10), Shade = (Shade)r.Below(3), Id = DecimalValue(r) };
                        if (r.Chance(40)) p.Friend = p;
                        if (r.Chance(50)) p.Numbers = Enumerable.Range(0, r.Small(4)).Select(i => (int)r.Next()).ToList();
                        return p;
                    }
                case 19: { var d = new Dictionary<string, object>(); int n = r.Small(4); for (int i = 0; i < n; i++) d["k" + i.ToString(CultureInfo.InvariantCulture)] = Any(r, depth + 1); return d; }
                case 20: { var n = new Node { Value = Any(r, depth + 1) }; n.Next = new Node { Value = Any(r, depth + 1), Next = n, Other = n }; return n; }
                default: return Hybrid(r.Small(12), r.Chance(30));
            }
        }

        static object Int(Rng r)
        {
            ulong v = r.Next();
            switch (r.Below(8))
            {
                case 0: return (int)v;
                case 1: return (long)v;
                case 2: return (short)v;
                case 3: return (ushort)v;
                case 4: return (uint)v;
                case 5: return v;
                case 6: return (sbyte)v;
                default: return (int)(v % 100);
            }
        }

        internal static string Text(Rng r)
        {
            switch (r.Below(10))
            {
                case 0: return "";
                case 1: return new string((char)('a' + r.Below(26)), 1 + r.Small(2000));
                case 2: return Gen.Text(r, 3) + "\r\n" + Gen.Text(r, 3);
                default: return Gen.Text(r, 6);
            }
        }

        static object Floating(Rng r)
        {
            long bits = (long)r.Next();
            switch (r.Below(4))
            {
                case 0: return BitConverter.Int64BitsToDouble(bits);
                case 1: return (float)BitConverter.Int64BitsToDouble(bits);
                case 2: return (r.Below(2000001) - 1000000) / 1000.0;
                default: return BitConverter.ToSingle(BitConverter.GetBytes((int)bits), 0);
            }
        }

        static decimal DecimalValue(Rng r) => new decimal((int)r.Next(), r.Chance(50) ? (int)r.Next() : 0, r.Chance(30) ? (int)r.Next() : 0, r.Chance(50), (byte)r.Below(29));

        internal static DateTime Date(Rng r)
        {
            long ticks = (long)(r.Next() % (ulong)DateTime.MaxValue.Ticks);
            if (r.Chance(30)) ticks -= ticks % TimeSpan.TicksPerMillisecond;
            return new DateTime(ticks, (DateTimeKind)r.Below(3));
        }
    }

    /// <summary>A value as text, the same on .NET Framework and .NET for the same value.</summary>
    public static class Show
    {
        /// <summary>
        /// The ticks of a local DateTime shown (or only its kind): false where the time zone is not the recorder's (a formatter
        /// that keeps the instant gives other ticks in another zone, as .NET Framework did).
        /// </summary>
        public static bool LocalTicks = true;

        public static string Of(object value)
        {
            var sb = new StringBuilder();
            Write(sb, value, new List<object>());
            return sb.ToString();
        }

        public static string Text(string s)
        {
            if (s == null) return "\\N";
            var sb = new StringBuilder(s.Length + 2);
            sb.Append('"');
            foreach (char c in s)
            {
                if (c == '\\' || c == '"') sb.Append('\\').Append(c);
                else if (c >= 0x20 && c < 0x7F) sb.Append(c);
                else sb.Append("\\u").Append(((int)c).ToString("X4", CultureInfo.InvariantCulture));
            }
            return sb.Append('"').ToString();
        }

        public static string TypeName(Type t)
        {
            if (t == null) return "null";
            if (t.IsArray) return TypeName(t.GetElementType()) + "[" + new string(',', t.GetArrayRank() - 1) + "]";
            if (t.IsGenericParameter) return t.Name;
            if (t.IsGenericType)
            {
                var definition = t.GetGenericTypeDefinition();
                return definition.FullName + (t.IsGenericTypeDefinition ? "" : "[" + string.Join(",", t.GetGenericArguments().Select(TypeName)) + "]");
            }
            return t.FullName;
        }

        static string Bits(double d) => BitConverter.DoubleToInt64Bits(d).ToString("X16", CultureInfo.InvariantCulture);

        static string Bits(float f) => BitConverter.ToInt32(BitConverter.GetBytes(f), 0).ToString("X8", CultureInfo.InvariantCulture);

        static string I(IFormattable f) => f.ToString(null, CultureInfo.InvariantCulture);

        public static string Date(DateTime d) => d.Kind == DateTimeKind.Local && !LocalTicks ? "Local" : d.Kind + ":" + I(d.Ticks);

        static void Write(StringBuilder sb, object value, List<object> ancestors)
        {
            switch (value)
            {
                case null: sb.Append("null"); return;
                case string s: sb.Append(Text(s)); return;
                case bool b: sb.Append(b ? "true" : "false"); return;
                case char c: sb.Append("char:").Append(((int)c).ToString("X4", CultureInfo.InvariantCulture)); return;
                case double d: sb.Append("double:").Append(Bits(d)); return;
                case float f: sb.Append("float:").Append(Bits(f)); return;
                case decimal m: sb.Append("decimal:").Append(string.Join(",", decimal.GetBits(m).Select(x => I(x)))); return;
                case DateTime d: sb.Append("DateTime:").Append(Date(d)); return;
                case DateTimeOffset o: sb.Append("DateTimeOffset:").Append(I(o.Ticks)).Append(o.Offset < TimeSpan.Zero ? "" : "+").Append(I(o.Offset.TotalMinutes)); return;
                case TimeSpan t: sb.Append("TimeSpan:").Append(I(t.Ticks)); return;
                case Guid g: sb.Append("Guid:").Append(g.ToString("N")); return;
                case DBNull _: sb.Append("DBNull"); return;
                case Type t: sb.Append("Type:").Append(TypeName(t)); return;
                case CultureInfo c: sb.Append("CultureInfo:").Append(c.Name).Append(c.UseUserOverride ? "" : "/noOverride"); return;
                case Uri u: sb.Append("Uri:").Append(Text(u.OriginalString)).Append(u.IsAbsoluteUri ? "/abs" : "/rel"); return;
                case Color c:
                    // a system color's value: the desktop's (Windows' settings; other systems' defaults)
                    sb.Append("Color:").Append(c.IsEmpty ? "Empty" : (c.IsKnownColor ? "known " : "") + (c.IsNamedColor ? "named " + c.Name : "") +
                        (c.IsSystemColor ? "" : (c.IsNamedColor ? " " : "") + c.ToArgb().ToString("X8", CultureInfo.InvariantCulture)));
                    return;
                case Unit u: sb.Append("Unit:").Append(u.IsEmpty ? "Empty" : u.Type + " " + Bits(u.Value)); return;
                case FontUnit f: sb.Append("FontUnit:").Append(f.Type).Append(' '); Write(sb, f.Unit, ancestors); return;
                case IndexedString i: sb.Append("IndexedString:").Append(Text(i.Value)); return;
                case Point p: sb.Append("Point:").Append(I(p.X)).Append(',').Append(I(p.Y)); return;
                case Size z: sb.Append("Size:").Append(I(z.Width)).Append(',').Append(I(z.Height)); return;
                case Rectangle q: sb.Append("Rectangle:").Append(I(q.X)).Append(',').Append(I(q.Y)).Append(',').Append(I(q.Width)).Append(',').Append(I(q.Height)); return;
                case Version v: sb.Append("Version:").Append(v.ToString()); return;
                case Control c: sb.Append(ControlTree.Show(c)); return;
            }
            var type = value.GetType();
            if (type.IsEnum) { sb.Append(TypeName(type)).Append(':').Append(Convert.ToString(Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture), CultureInfo.InvariantCulture)); return; }
            if (type.IsPrimitive) { sb.Append(TypeName(type)).Append(':').Append(Convert.ToString(value, CultureInfo.InvariantCulture)); return; }
            // a reference back to an object being shown (a cycle): its depth
            int back = ancestors.FindIndex(a => ReferenceEquals(a, value));
            if (back >= 0) { sb.Append("^").Append(I(ancestors.Count - back)); return; }
            ancestors.Add(value);
            try
            {
                Container(sb, value, type, ancestors);
            }
            finally
            {
                ancestors.RemoveAt(ancestors.Count - 1);
            }
        }

        static void Items(StringBuilder sb, IEnumerable items, List<object> ancestors)
        {
            sb.Append('[');
            bool first = true;
            foreach (var item in items)
            {
                if (!first) sb.Append(", ");
                first = false;
                Write(sb, item, ancestors);
            }
            sb.Append(']');
        }

        static void Entries(StringBuilder sb, IDictionary dictionary, bool sorted, List<object> ancestors)
        {
            var entries = new List<KeyValuePair<string, string>>();
            foreach (DictionaryEntry e in dictionary)
            {
                var key = new StringBuilder();
                Write(key, e.Key, ancestors);
                var item = new StringBuilder();
                Write(item, e.Value, ancestors);
                entries.Add(new KeyValuePair<string, string>(key.ToString(), item.ToString()));
            }
            if (sorted) entries.Sort((a, b) => string.CompareOrdinal(a.Key, b.Key));
            sb.Append('{').Append(string.Join(", ", entries.Select(e => e.Key + ": " + e.Value))).Append('}');
        }

        static void Container(StringBuilder sb, object value, Type type, List<object> ancestors)
        {
            switch (value)
            {
                case Array a:
                    sb.Append(TypeName(type));
                    if (a.Rank > 1 || a.GetLowerBound(0) != 0)
                        sb.Append('<').Append(string.Join(",", Enumerable.Range(0, a.Rank).Select(d => I(a.GetLowerBound(d)) + ":" + I(a.GetLength(d))))).Append('>');
                    Items(sb, a, ancestors);
                    return;
                case ArrayList l: sb.Append("ArrayList"); Items(sb, l, ancestors); return;
                case Hashtable h: sb.Append("Hashtable"); Entries(sb, h, true, ancestors); return;
                case HybridDictionary h: sb.Append("HybridDictionary"); Entries(sb, h, true, ancestors); return;
                case Pair p: sb.Append("Pair("); Write(sb, p.First, ancestors); sb.Append(", "); Write(sb, p.Second, ancestors); sb.Append(')'); return;
                case Triplet t: sb.Append("Triplet("); Write(sb, t.First, ancestors); sb.Append(", "); Write(sb, t.Second, ancestors); sb.Append(", "); Write(sb, t.Third, ancestors); sb.Append(')'); return;
                case NameValueCollection n:
                    sb.Append("NameValueCollection{").Append(string.Join(", ", Enumerable.Range(0, n.Count).Select(i => Text(n.GetKey(i)) + ": " +
                        (n.GetValues(i) == null ? "null" : "[" + string.Join(",", n.GetValues(i).Select(Text)) + "]")))).Append('}');
                    return;
                case DataSet s:
                    sb.Append("DataSet ").Append(Text(s.DataSetName)).Append('{');
                    foreach (DataTable t in s.Tables) Table(sb, t, ancestors);
                    sb.Append(" relations:").Append(string.Join(",", s.Relations.Cast<DataRelation>().Select(r => r.RelationName + "=" + r.ParentTable.TableName + "." + r.ParentColumns[0].ColumnName +
                        ">" + r.ChildTable.TableName + "." + r.ChildColumns[0].ColumnName))).Append('}');
                    return;
                case DataTable t: Table(sb, t, ancestors); return;
            }
            if (value is IDictionary d && type.IsGenericType) { sb.Append(TypeName(type)); Entries(sb, d, false, ancestors); return; }
            if (value is IList list && type.IsGenericType) { sb.Append(TypeName(type)); Items(sb, list, ancestors); return; }
            if (type.Assembly != typeof(Show).Assembly)
                throw new NotSupportedException("Show: " + type.AssemblyQualifiedName);
            // the application's types: their fields (the base classes' too), as BinaryFormatter has them
            sb.Append(TypeName(type)).Append('{');
            bool first = true;
            for (var t = type; t != null && t != typeof(object) && t != typeof(ValueType); t = t.BaseType)
            {
                foreach (var field in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).OrderBy(f => f.Name, StringComparer.Ordinal))
                {
                    if (!first) sb.Append(", ");
                    first = false;
                    sb.Append(t == type ? "" : t.Name + ".").Append(field.Name).Append('=');
                    Write(sb, field.GetValue(value), ancestors);
                }
            }
            sb.Append('}');
        }

        static void Table(StringBuilder sb, DataTable t, List<object> ancestors)
        {
            sb.Append(" DataTable ").Append(Text(t.TableName)).Append(" (").Append(string.Join(",", t.Columns.Cast<DataColumn>().Select(c => c.ColumnName + ":" + TypeName(c.DataType)))).Append(')');
            foreach (DataRow row in t.Rows)
            {
                sb.Append(' ').Append(row.RowState).Append('[');
                for (int i = 0; i < t.Columns.Count; i++)
                {
                    if (i > 0) sb.Append(", ");
                    Write(sb, row.RowState == DataRowState.Deleted ? null : row[i], ancestors);
                    if (row.HasVersion(DataRowVersion.Original) && row.RowState == DataRowState.Modified) { sb.Append(" was "); Write(sb, row[i, DataRowVersion.Original], ancestors); }
                }
                sb.Append(']');
            }
        }
    }
}
