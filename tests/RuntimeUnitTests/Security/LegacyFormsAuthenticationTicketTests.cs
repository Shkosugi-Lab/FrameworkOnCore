using System.Text;
using System.Text.Json;
using System.Web.Security;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// The legacy forms authentication ticket (LegacyFormsAuthenticationTicket: webengine4.dll's CookieAuthConstructTicket and
/// CookieAuthParseTicket, managed): the native functions' results on .NET Framework 4.8 (golden/net48.json) for the same
/// arguments.
/// </summary>
public class LegacyFormsAuthenticationTicketTests
{
    const int E_UNEXPECTED = unchecked((int)0x8000FFFF);
    const int E_INVALIDARG = unchecked((int)0x80070057);
    const long IssueTime = 0x01DC7B1E2F3A4B5CL;
    const long ExpireTime = -2;

    public static IEnumerable<object[]> Constructs() => Enumerable.Range(0, Golden.Array("construct").Length).Select(i => new object[] { i });

    // The records of a ticket of 18 bytes or more (the shorter ones: Below_18_bytes_a_ticket_is_an_invalid_argument).
    public static IEnumerable<object[]> Parses() => Enumerable.Range(0, Golden.Array("parse").Length)
        .Where(i => Golden.Array("parse")[i].Int("length") >= 18 || Golden.Array("parse")[i].String("ticket") == null).Select(i => new object[] { i });

    [Theory, MemberData(nameof(Constructs))] // .NET Framework 4.8 (golden): CookieAuthConstructTicket's length and bytes, or its error
    public void A_ticket_is_written_as_webengine_writes_it(int index)
    {
        var record = Golden.Array("construct")[index];
        int length = record.Int("length");
        var buffer = new byte[Math.Max(length, 1)];
        for (int i = 0; i < 8 && i < buffer.Length; i++) buffer[i] = (byte)(0xA0 + i);
        int result;
        if (record.TryGetProperty("null", out var nulls))
        {
            int which = nulls.GetInt32();
            result = LegacyFormsAuthenticationTicket.Construct(buffer, length, which == 0 ? null : "n", which == 1 ? null : "d", which == 2 ? null : "p", new byte[] { 1, 0, 0, 0 }, new long[] { 1, 2 });
        }
        else
        {
            result = LegacyFormsAuthenticationTicket.Construct(buffer, length, record.String("name"), record.String("data"), record.String("path"), new byte[] { 2, 1, 0, 0 }, new[] { IssueTime, ExpireTime });
            if (result > 0) Assert.Equal(record.String("bytes"), Golden.Hex(buffer.Take(result).ToArray()));
        }
        Assert.Equal(record.Int("result"), result);
    }

    [Theory, MemberData(nameof(Parses))] // .NET Framework 4.8 (golden): CookieAuthParseTicket's fields, or its error, for a ticket whole and cut, into buffers of each size
    public void A_ticket_is_read_as_webengine_reads_it(int index)
    {
        var record = Golden.Array("parse")[index];
        var ticket = Golden.Bytes(record.String("ticket"));
        StringBuilder name = new(), data = new(), path = new();
        var bytes = new byte[4];
        var dates = new long[2];
        int result = LegacyFormsAuthenticationTicket.Parse(ticket, record.Int("length"), name, record.Int("nameLength"), data, record.Int("dataLength"),
            path, record.Int("pathLength"), bytes, dates);
        Assert.Equal(record.Int("result"), result);
        if (result != 0) return;
        Assert.Equal(record.String("name"), name.ToString());
        Assert.Equal(record.String("data"), data.ToString());
        Assert.Equal(record.String("path"), path.ToString());
        Assert.Equal(record.Int("version"), bytes[0]);
        Assert.Equal(record.Int("persistent"), bytes[1]);
        Assert.Equal(long.Parse(record.String("issued")), dates[0]);
        Assert.Equal(long.Parse(record.String("expires")), dates[1]);
    }

    [Fact]
    // .NET Framework 4.8 (golden): below 18 bytes (the prefix, the version, the persistence and a date) the native function
    // rejects the ticket as an argument, as CookieAuthConstructTicket rejects such a buffer (FormsAuthentication.Decrypt:
    // null either way)
    public void Below_18_bytes_a_ticket_is_an_invalid_argument()
    {
        var records = Golden.Array("parse").Where(r => r.Int("length") < 18 && r.String("ticket") != null).ToArray();
        Assert.Equal(55, records.Length);
        foreach (var record in records)
        {
            Assert.Equal(E_INVALIDARG, record.Int("result"));
            Assert.Equal(E_INVALIDARG, LegacyFormsAuthenticationTicket.Parse(Golden.Bytes(record.String("ticket")), record.Int("length"), new StringBuilder(), 512,
                new StringBuilder(), 512, new StringBuilder(), 512, new byte[4], new long[2]));
        }
    }

    [Fact] // the port's choice (the class's summary: too little room is E_UNEXPECTED): a length past the array (the native function read or wrote past it)
    public void A_length_past_the_array_is_unexpected()
    {
        Assert.Equal(E_UNEXPECTED, LegacyFormsAuthenticationTicket.Construct(new byte[20], 100, "n", "d", "p", new byte[4], new long[2]));
        Assert.Equal(E_UNEXPECTED, LegacyFormsAuthenticationTicket.Parse(new byte[20], 21, new StringBuilder(), 10, new StringBuilder(), 10, new StringBuilder(), 10, new byte[4], new long[2]));
        Assert.Equal(E_INVALIDARG, LegacyFormsAuthenticationTicket.Parse(null, 0, new StringBuilder(), 10, new StringBuilder(), 10, new StringBuilder(), 10, new byte[4], new long[2]));
    }
}
