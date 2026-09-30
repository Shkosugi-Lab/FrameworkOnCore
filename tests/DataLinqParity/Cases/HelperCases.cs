using System;
using System.Data.Linq;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Xml.Linq;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DataLinqParity.Cases
{
    /// <summary>The static helpers: DBConvert (what materialization converts with), SqlHelpers (the LIKE patterns of
    /// string methods), SqlMethods called outside a query.</summary>
    static class HelperCases
    {
        enum Color { Red = 1, Green = 2 }

        [Case]
        static void DBConvert_Generic(Probe p)
        {
            p.Try("int -> long", () => DBConvert.ChangeType<long>(5));
            p.Try("string -> int", () => DBConvert.ChangeType<int>("42"));
            p.Try("decimal -> int", () => DBConvert.ChangeType<int>(3.7m));
            p.Try("double -> decimal", () => DBConvert.ChangeType<decimal>(1.25));
            p.Try("int -> enum", () => DBConvert.ChangeType<Color>(2));
            p.Try("string -> enum", () => DBConvert.ChangeType<Color>("Green"));
            p.Try("undefined int -> enum", () => DBConvert.ChangeType<Color>(7));
            p.Try("enum -> int", () => DBConvert.ChangeType<int>(Color.Red));
            p.Try("null -> int?", () => DBConvert.ChangeType<int?>(null));
            p.Try("null -> int", () => DBConvert.ChangeType<int>(null));
            p.Try("int -> int?", () => DBConvert.ChangeType<int?>(3));
            p.Try("string -> Guid", () => DBConvert.ChangeType<Guid>("7c9e6679-7425-40de-944b-e07fc1f90ae7"));
            p.Try("Guid -> string", () => DBConvert.ChangeType<string>(new Guid("7c9e6679-7425-40de-944b-e07fc1f90ae7")));
            p.Try("byte[] -> Binary", () => DBConvert.ChangeType<Binary>(new byte[] { 1, 2 }));
            p.Try("Binary -> byte[]", () => DBConvert.ChangeType<byte[]>(new Binary(new byte[] { 3 })));
            p.Try("string -> char", () => DBConvert.ChangeType<char>("x"));
            p.Try("char -> string", () => DBConvert.ChangeType<string>('y'));
            p.Try("string -> char[]", () => DBConvert.ChangeType<char[]>("ab"));
            p.Try("char[] -> string", () => DBConvert.ChangeType<string>(new[] { 'c', 'd' }));
            p.Try("string -> XElement", () => DBConvert.ChangeType<XElement>("<a>1</a>"));
            p.Try("XElement -> string", () => DBConvert.ChangeType<string>(new XElement("b", 2)));
            p.Try("string -> XDocument", () => DBConvert.ChangeType<XDocument>("<c/>"));
            p.Try("string -> DateTime", () => DBConvert.ChangeType<DateTime>("2020-01-15 10:30:00"));
            p.Try("DateTime -> DateTimeOffset", () => DBConvert.ChangeType<DateTimeOffset>(new DateTime(2020, 1, 15, 0, 0, 0, DateTimeKind.Utc)));
            p.Try("TimeSpan -> DateTime", () => DBConvert.ChangeType<DateTime>(TimeSpan.FromHours(1)));
            p.Try("string -> TimeSpan", () => DBConvert.ChangeType<TimeSpan>("01:02:03"));
            p.Try("string -> Version (a type's Parse)", () => DBConvert.ChangeType<Version>("1.2.3"));
            p.Try("bool -> int", () => DBConvert.ChangeType<int>(true));
            p.Try("overflow", () => DBConvert.ChangeType<byte>(300));
            p.Try("bad string", () => DBConvert.ChangeType<int>("x"));
            p.Try("unrelated", () => DBConvert.ChangeType<Guid>(5));
        }

        [Case]
        static void DBConvert_ByType(Probe p)
        {
            p.Try("int -> Int64", () => DBConvert.ChangeType(5, typeof(long)));
            p.Try("string -> Int32", () => DBConvert.ChangeType("7", typeof(int)));
            p.Try("int -> Color", () => DBConvert.ChangeType(1, typeof(Color)));
            p.Try("null -> string", () => DBConvert.ChangeType(null, typeof(string)));
            p.Try("null -> Int32", () => DBConvert.ChangeType(null, typeof(int)));
            p.Try("null type", () => DBConvert.ChangeType(1, null));
            p.Try("same type", () => DBConvert.ChangeType("same", typeof(string)));
            p.Try("object", () => DBConvert.ChangeType(5, typeof(object)));
        }

        [Case]
        static void SqlHelpers_Patterns(Probe p)
        {
            foreach (var text in new[] { "abc", "a%b", "a_b", "a[b]", "~x", "", "50%_[off]" })
            {
                p.Try("contains " + text, () => SqlHelpers.GetStringContainsPattern(text, '~'));
                p.Try("starts " + text, () => SqlHelpers.GetStringStartsWithPattern(text, '~'));
                p.Try("ends " + text, () => SqlHelpers.GetStringEndsWithPattern(text, '~'));
            }
            p.Try("contains null", () => SqlHelpers.GetStringContainsPattern(null, '~'));
            p.Try("starts null", () => SqlHelpers.GetStringStartsWithPattern(null, '~'));
            p.Try("ends null", () => SqlHelpers.GetStringEndsWithPattern(null, '~'));
            foreach (var pattern in new[] { "a*b", "a?b", "#", "[a-c]x", "[!a]", "a%b", "a_b", "[*]", "~", "[a", "[a-c-e]", "" })
                p.Try("vb like " + pattern, () => SqlHelpers.TranslateVBLikePattern(pattern, '~'));
            p.Try("vb like null", () => SqlHelpers.TranslateVBLikePattern(null, '~'));
        }

        [Case]
        static void SqlMethods_Outside_A_Query(Probe p)
        {
            var d = new DateTime(2020, 1, 1);
            var o = new DateTimeOffset(d);
            DateTime? n = d;
            DateTimeOffset? no = o;
            // Only for LINQ to SQL's translation: called directly they throw.
            p.Try("DateDiffDay nullable offset", () => SqlMethods.DateDiffDay(no, no));
            p.Try("DateDiffHour nullable offset", () => SqlMethods.DateDiffHour(no, no));
            p.Try("DateDiffMicrosecond nullable offset", () => SqlMethods.DateDiffMicrosecond(no, no));
            p.Try("DateDiffMillisecond nullable offset", () => SqlMethods.DateDiffMillisecond(no, no));
            p.Try("DateDiffMinute nullable offset", () => SqlMethods.DateDiffMinute(no, no));
            p.Try("DateDiffMonth nullable offset", () => SqlMethods.DateDiffMonth(no, no));
            p.Try("DateDiffNanosecond nullable offset", () => SqlMethods.DateDiffNanosecond(no, no));
            p.Try("DateDiffSecond nullable offset", () => SqlMethods.DateDiffSecond(no, no));
            p.Try("DateDiffYear nullable offset", () => SqlMethods.DateDiffYear(no, no));
            p.Try("DateDiffDay", () => SqlMethods.DateDiffDay(d, d));
            p.Try("DateDiffDay offset", () => SqlMethods.DateDiffDay(o, o));
            p.Try("DateDiffDay nullable", () => SqlMethods.DateDiffDay(n, n));
            p.Try("DateDiffHour", () => SqlMethods.DateDiffHour(d, d));
            p.Try("DateDiffHour offset", () => SqlMethods.DateDiffHour(o, o));
            p.Try("DateDiffHour nullable", () => SqlMethods.DateDiffHour(n, n));
            p.Try("DateDiffMicrosecond", () => SqlMethods.DateDiffMicrosecond(d, d));
            p.Try("DateDiffMicrosecond offset", () => SqlMethods.DateDiffMicrosecond(o, o));
            p.Try("DateDiffMicrosecond nullable", () => SqlMethods.DateDiffMicrosecond(n, n));
            p.Try("DateDiffMillisecond", () => SqlMethods.DateDiffMillisecond(d, d));
            p.Try("DateDiffMillisecond offset", () => SqlMethods.DateDiffMillisecond(o, o));
            p.Try("DateDiffMillisecond nullable", () => SqlMethods.DateDiffMillisecond(n, n));
            p.Try("DateDiffMinute", () => SqlMethods.DateDiffMinute(d, d));
            p.Try("DateDiffMinute offset", () => SqlMethods.DateDiffMinute(o, o));
            p.Try("DateDiffMinute nullable", () => SqlMethods.DateDiffMinute(n, n));
            p.Try("DateDiffMonth", () => SqlMethods.DateDiffMonth(d, d));
            p.Try("DateDiffMonth offset", () => SqlMethods.DateDiffMonth(o, o));
            p.Try("DateDiffMonth nullable", () => SqlMethods.DateDiffMonth(n, n));
            p.Try("DateDiffNanosecond", () => SqlMethods.DateDiffNanosecond(d, d));
            p.Try("DateDiffNanosecond offset", () => SqlMethods.DateDiffNanosecond(o, o));
            p.Try("DateDiffNanosecond nullable", () => SqlMethods.DateDiffNanosecond(n, n));
            p.Try("DateDiffSecond", () => SqlMethods.DateDiffSecond(d, d));
            p.Try("DateDiffSecond offset", () => SqlMethods.DateDiffSecond(o, o));
            p.Try("DateDiffSecond nullable", () => SqlMethods.DateDiffSecond(n, n));
            p.Try("DateDiffYear", () => SqlMethods.DateDiffYear(d, d));
            p.Try("DateDiffYear offset", () => SqlMethods.DateDiffYear(o, o));
            p.Try("DateDiffYear nullable", () => SqlMethods.DateDiffYear(n, n));
            p.Try("Like", () => SqlMethods.Like("a", "a"));
            p.Try("Like escape", () => SqlMethods.Like("a", "a", '~'));
        }
    }
}
