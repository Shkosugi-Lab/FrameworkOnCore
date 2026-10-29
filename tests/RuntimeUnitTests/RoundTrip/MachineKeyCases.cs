// The round-trip tests' cases with a <machineKey>: what each configuration protects (MachineKey.Protect, MachineKey.Encode,
// forms authentication tickets, Page.EncryptString, LosFormatter with its MAC) for generated data. Compiled into the .NET
// Framework 4.8 recorder (each configuration in its own AppDomain) and into the port's tests (Security's MachineKeys).
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using FrameworkOnCore.Runtime.Tests.Fuzz;

namespace FrameworkOnCore.Runtime.Tests.RoundTrip
{
    public static class MachineKeyCases
    {
        /// <summary>
        /// Security/golden's configurations (name, validation, decryption, compatibilityMode, the legacy ticket, validationKey,
        /// decryptionKey) but DPAPI's (the user's key: Security's tests have it).
        /// </summary>
        public static readonly string[][] Configurations =
        {
            new[] { "F45-HMACSHA256-AES", "HMACSHA256", "AES", "Framework45", "false", Keys.Validation64, Keys.Aes24 },
            new[] { "F45-SHA1-3DES-legacy", "SHA1", "3DES", "Framework45", "true", Keys.Validation64, Keys.Aes24 },
            new[] { "F45-HMACSHA512-AES128", "HMACSHA512", "AES", "Framework45", "false", Keys.Validation64, Keys.Aes16 },
            new[] { "F45-HMACSHA384-DES", "HMACSHA384", "DES", "Framework45", "false", Keys.Validation64, Keys.Des8 },
            new[] { "F20SP1-SHA1-AES-legacy", "SHA1", "AES", "Framework20SP1", "true", Keys.Validation64, Keys.Aes24 },
            new[] { "F20SP1-HMACSHA256-3DES", "HMACSHA256", "3DES", "Framework20SP1", "false", Keys.Validation64, Keys.Aes24 },
            new[] { "F20SP1-HMACSHA384-AES-legacy", "HMACSHA384", "AES", "Framework20SP1", "true", Keys.Validation64, Keys.Aes24 },
            new[] { "F20SP1-HMACSHA512-AES", "HMACSHA512", "AES", "Framework20SP1", "false", Keys.Validation64, Keys.Aes32 },
            new[] { "F20SP1-MD5-AES", "MD5", "AES", "Framework20SP1", "false", Keys.Validation64, Keys.Aes24 },
            new[] { "F20SP2-SHA1-AES-legacy", "SHA1", "AES", "Framework20SP2", "true", Keys.Validation64, Keys.Aes24 },
            new[] { "F20SP1-AES-AES", "AES", "AES", "Framework20SP1", "false", Keys.Validation64, Keys.Aes24 },
            new[] { "F20SP1-SHA1-longkey", "SHA1", "AES", "Framework20SP1", "false", Keys.Validation80, Keys.Aes24 },
            new[] { "F45-HMACSHA256-longkey", "HMACSHA256", "AES", "Framework45", "false", Keys.Validation65, Keys.Aes24 },
            new[] { "F45-alg", "alg:HMACSHA256", "alg:AesManaged", "Framework45", "false", Keys.Validation64, Keys.Aes24 },
        };

        public static class Keys
        {
            public const string Validation64 = "0123456789ABCDEFFEDCBA98765432100F1E2D3C4B5A69788796A5B4C3D2E1F00112233445566778899AABBCCDDEEFF00102030405060708090A0B0C0D0E0F10";
            public const string Validation65 = Validation64 + "11";
            public const string Validation80 = Validation64 + "A1A2A3A4A5A6A7A8A9AAABACADAEAFB0";
            public const string Aes16 = "00112233445566778899AABBCCDDEEFF";
            public const string Aes24 = "00112233445566778899AABBCCDDEEFF0011223344556677";
            public const string Aes32 = "00112233445566778899AABBCCDDEEFF00112233445566778899AABBCCDDEEFF";
            public const string Des8 = "0123456789ABCDEF";
        }

        /// <summary>The cases a configuration is given (each kind of case's own values).</summary>
        public const int Count = 40;

        /// <summary>The kinds of case: what is protected, how it is read back.</summary>
        public static readonly string[] Kinds = { "Protect", "EncodeAll", "EncodeValidation", "EncodeEncryption", "Ticket", "EncryptString", "LosMac" };

        static Rng Seed(string kind, int index) => new Rng(0x4D4B4559UL * 31 + (ulong)kind.Length * 1000003UL + (ulong)index * 0x9E3779B97F4A7C15UL + (ulong)kind[0]);

        internal static byte[] Data(Rng r)
        {
            int length = r.Pick(new[] { 0, 1, 15, 16, 17, 31, 32, 33, 64, 1000, r.Below(300) });
            var data = new byte[length];
            for (int i = 0; i < length; i++) data[i] = (byte)r.Next();
            return data;
        }

        internal static string[] Purposes(Rng r)
        {
            switch (r.Below(4))
            {
                case 0: return new string[0];
                case 1: return new[] { "Purpose" };
                case 2: return new[] { "目的 \U0001F600", "user", "" .PadLeft(r.Below(3), 'x') + "z" };
                default: return Enumerable.Range(0, 1 + r.Small(6)).Select(i => Gen.Text(r, 2) + "p").ToArray();
            }
        }

        static string Name(Rng r) => r.Pick(new[] { "user", "", "ユーザー\U0001F600", "a;b|c:d", "\0x", "\uD800" , new string('n', 500) }) + (r.Chance(50) ? Gen.Text(r, 3) : "");

        static DateTime TicketDate(Rng r)
        {
            long ticks = new DateTime(1990, 1, 1).Ticks + (long)(r.Next() % (ulong)(new DateTime(2090, 1, 1).Ticks - new DateTime(1990, 1, 1).Ticks));
            if (r.Chance(40)) ticks -= ticks % TimeSpan.TicksPerSecond;
            return new DateTime(ticks, r.Chance(50) ? DateTimeKind.Utc : (r.Chance(50) ? DateTimeKind.Local : DateTimeKind.Unspecified));
        }

        internal static FormsAuthenticationTicket Ticket(Rng r) => Ticket(r, out _);

        /// <summary>A ticket; zoneDependent: of local dates (Local, Unspecified: their instant is the time zone's).</summary>
        internal static FormsAuthenticationTicket Ticket(Rng r, out bool zoneDependent)
        {
            var issued = TicketDate(r);
            zoneDependent = issued.Kind != DateTimeKind.Utc;
            return new FormsAuthenticationTicket(r.Below(256), Name(r), issued, issued.AddMinutes(r.Below(100000)), r.Chance(50),
                r.Chance(30) ? "" : Gen.Text(r, 8) + (r.Chance(10) ? new string('d', 2000) : ""), r.Pick(new[] { "/", "/app", "", "/パス", "/a b" }));
        }

        /// <summary>A ticket's fields (its dates as UTC: Decrypt gives them local).</summary>
        public static string ShowTicket(FormsAuthenticationTicket t)
        {
            if (t == null) return "null";
            DateTime Utc(string name, DateTime fallback)
            {
                var p = typeof(FormsAuthenticationTicket).GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);
                return p != null ? (DateTime)p.GetValue(t, null) : fallback.ToUniversalTime();
            }
            return "Ticket{" + t.Version.ToString(CultureInfo.InvariantCulture) + ", " + Show.Text(t.Name) + ", " + Utc("IssueDateUtc", t.IssueDate).Ticks.ToString(CultureInfo.InvariantCulture) +
                ", " + Utc("ExpirationUtc", t.Expiration).Ticks.ToString(CultureInfo.InvariantCulture) + ", " + t.IsPersistent + ", " + Show.Text(t.UserData) + ", " + Show.Text(t.CookiePath) + "}";
        }

        /// <summary>A case: what is protected (shown), how it is written, how read back (shown).</summary>
        public sealed class Case
        {
            public string Kind;
            public int Index;
            public string Input;
            public Func<string> Write;
            public Func<string, string> Read;
            /// <summary>Whether the case is one of the time zone (a ticket of local dates: their instant is the zone's).</summary>
            public bool ZoneDependent;
        }

        public static Case Make(string kind, int index)
        {
            var r = Seed(kind, index);
            switch (kind)
            {
                case "Protect":
                    {
                        var data = Data(r);
                        var purposes = Purposes(r);
                        return new Case { Kind = kind, Index = index, Input = Show.Of(data),
                            Write = () => Hex(MachineKey.Protect(data, purposes)), Read = s => Show.Of(MachineKey.Unprotect(FromHex(s), purposes)) };
                    }
                case "EncodeAll":
                case "EncodeValidation":
                case "EncodeEncryption":
                    {
                        var data = Data(r);
                        var protection = kind == "EncodeAll" ? MachineKeyProtection.All : kind == "EncodeValidation" ? MachineKeyProtection.Validation : MachineKeyProtection.Encryption;
                        return new Case { Kind = kind, Index = index, Input = Show.Of(data), Write = () => MachineKey.Encode(data, protection), Read = s => Show.Of(MachineKey.Decode(s, protection)) };
                    }
                case "Ticket":
                    {
                        var ticket = Ticket(r, out bool zoneDependent);
                        return new Case { Kind = kind, Index = index, Input = ShowTicket(ticket), ZoneDependent = zoneDependent, Write = () => FormsAuthentication.Encrypt(ticket), Read = s => ShowTicket(FormsAuthentication.Decrypt(s)) };
                    }
                case "EncryptString":
                    {
                        var text = r.Chance(30) ? "d=" + Gen.Path(r) + "|" + Gen.Text(r, 2) : Gen.Text(r, 6);
                        return new Case { Kind = kind, Index = index, Input = Show.Text(text), Write = () => PageString("EncryptString", text), Read = s => Show.Text(PageString("DecryptString", s)) };
                    }
                case "LosMac":
                    {
                        int valueIndex = index * 7 % Values.Count;
                        var value = Values.Make(valueIndex);
                        return new Case { Kind = kind, Index = index, Input = "value " + valueIndex.ToString(CultureInfo.InvariantCulture),
                            Write = () => { var w = new StringWriter(); new LosFormatter(true, "modifier ユ").Serialize(w, value); return w.ToString(); },
                            Read = s => Formats.Shown(new LosFormatter(true, "modifier ユ").Deserialize(s)) };
                    }
            }
            throw new ArgumentException(kind);
        }

        static string PageString(string method, string s)
        {
            var web = typeof(HttpContext).Assembly;
            var purpose = web.GetType("System.Web.Security.Cryptography.Purpose").GetField("AssemblyResourceLoader_WebResourceUrl", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            var m = typeof(Page).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(string), purpose.GetType() }, null);
            return (string)Formats.Invoke(m, null, new[] { s, purpose });
        }

        public static string Hex(byte[] bytes)
        {
            if (bytes == null) return null;
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
            return sb.ToString();
        }

        public static byte[] FromHex(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++) bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            return bytes;
        }
    }
}
