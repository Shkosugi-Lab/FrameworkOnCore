// Records .NET Framework 4.8's results the Security tests compare the port's with (net48.json beside this file): its
// webengine4.dll's functions the port replaced in managed code (the legacy forms authentication ticket, the hashes
// MachineKeySection asked it for), and what System.Web makes with a given <machineKey> (forms authentication tickets,
// MachineKey.Protect/Encode, Page.EncryptString's protected URLs) for each configuration in Configurations, each in its
// own AppDomain with that configuration; DataProtector (DPAPI, the current user's: the tests read it as the same user).
//
// Re-record (Windows, .NET Framework 4.8; as the user who runs the tests, for the DPAPI values):
//   dotnet build tests/RuntimeUnitTests/Security/golden/Recorder.csproj -c Release
//   tests/RuntimeUnitTests/Security/golden/bin/Release/net48/Recorder.exe tests/RuntimeUnitTests/Security/golden/net48.json
//   then delete the folder's bin and obj.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

static class Program
{
    // name, validation, decryption, compatibilityMode, the legacy ticket format (appSettings), validationKey, decryptionKey,
    // dataProtectorType
    public static readonly string[][] Configurations =
    {
        new[] { "F45-HMACSHA256-AES", "HMACSHA256", "AES", "Framework45", "false", Keys.Validation64, Keys.Aes24, "" },
        new[] { "F45-SHA1-3DES-legacy", "SHA1", "3DES", "Framework45", "true", Keys.Validation64, Keys.Aes24, "" },
        new[] { "F45-HMACSHA512-AES128", "HMACSHA512", "AES", "Framework45", "false", Keys.Validation64, Keys.Aes16, "" },
        new[] { "F45-HMACSHA384-DES", "HMACSHA384", "DES", "Framework45", "false", Keys.Validation64, Keys.Des8, "" },
        new[] { "F20SP1-SHA1-AES-legacy", "SHA1", "AES", "Framework20SP1", "true", Keys.Validation64, Keys.Aes24, "" },
        new[] { "F20SP1-HMACSHA256-3DES", "HMACSHA256", "3DES", "Framework20SP1", "false", Keys.Validation64, Keys.Aes24, "" },
        new[] { "F20SP1-HMACSHA384-AES-legacy", "HMACSHA384", "AES", "Framework20SP1", "true", Keys.Validation64, Keys.Aes24, "" },
        new[] { "F20SP1-HMACSHA512-AES", "HMACSHA512", "AES", "Framework20SP1", "false", Keys.Validation64, Keys.Aes32, "" },
        new[] { "F20SP1-MD5-AES", "MD5", "AES", "Framework20SP1", "false", Keys.Validation64, Keys.Aes24, "" },
        new[] { "F20SP2-SHA1-AES-legacy", "SHA1", "AES", "Framework20SP2", "true", Keys.Validation64, Keys.Aes24, "" },
        new[] { "F20SP1-AES-AES", "AES", "AES", "Framework20SP1", "false", Keys.Validation64, Keys.Aes24, "" },
        // validationKey longer than the algorithm's key (SHA1's and HMACSHA256's 64 bytes)
        new[] { "F20SP1-SHA1-longkey", "SHA1", "AES", "Framework20SP1", "false", Keys.Validation80, Keys.Aes24, "" },
        new[] { "F45-HMACSHA256-longkey", "HMACSHA256", "AES", "Framework45", "false", Keys.Validation65, Keys.Aes24, "" },
        // <machineKey>'s custom algorithms ("alg:", CryptoConfig's names)
        new[] { "F45-alg", "alg:HMACSHA256", "alg:AesManaged", "Framework45", "false", Keys.Validation64, Keys.Aes24, "" },
        new[] { "F45-dpapi", "HMACSHA256", "AES", "Framework45", "false", Keys.Validation64, Keys.Aes24, "DpapiDataProtector" },
    };

    static class Keys
    {
        public const string Validation64 = "0123456789ABCDEFFEDCBA98765432100F1E2D3C4B5A69788796A5B4C3D2E1F00112233445566778899AABBCCDDEEFF00102030405060708090A0B0C0D0E0F10";
        public const string Validation65 = Validation64 + "11";
        public const string Validation80 = Validation64 + "A1A2A3A4A5A6A7A8A9AAABACADAEAFB0";
        public const string Aes16 = "00112233445566778899AABBCCDDEEFF";
        public const string Aes24 = "00112233445566778899AABBCCDDEEFF0011223344556677";
        public const string Aes32 = "00112233445566778899AABBCCDDEEFF00112233445566778899AABBCCDDEEFF";
        public const string Des8 = "0123456789ABCDEF";
    }

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern IntPtr LoadLibrary(string path);

    [DllImport("webengine4.dll", CharSet = CharSet.Unicode)]
    static extern int CookieAuthConstructTicket(byte[] pData, int iDataLen, string szName, string szData, string szPath, byte[] pBytes, long[] pDates);

    [DllImport("webengine4.dll", CharSet = CharSet.Unicode)]
    static extern int CookieAuthParseTicket(byte[] pData, int iDataLen, StringBuilder szName, int iNameLen, StringBuilder szData, int iUserDataLen,
                                            StringBuilder szPath, int iPathLen, byte[] pBytes, long[] pDates);

    [DllImport("webengine4.dll")]
    static extern int GetSHA1Hash(byte[] data, int dataSize, byte[] hash, int hashSize);

    [DllImport("webengine4.dll")]
    static extern int GetHMACSHA1Hash(byte[] data1, int dataOffset1, int dataSize1, byte[] data2, int dataSize2,
                                      byte[] innerKey, int innerKeySize, byte[] outerKey, int outerKeySize, byte[] hash, int hashSize);

    static int Main(string[] args)
    {
        var output = args.Length > 0 ? args[0] : "net48.json";
        if (LoadLibrary(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "webengine4.dll")) == IntPtr.Zero)
            throw new InvalidOperationException("webengine4.dll: " + Marshal.GetLastWin32Error());
        var json = new StringBuilder("{\n");
        json.Append("\"construct\": [\n").Append(string.Join(",\n", Construct())).Append("\n],\n");
        json.Append("\"parse\": [\n").Append(string.Join(",\n", Parse())).Append("\n],\n");
        json.Append("\"hash\": [\n").Append(string.Join(",\n", Hashes())).Append("\n],\n");
        json.Append("\"configurations\": [\n");
        var first = true;
        foreach (var configuration in Configurations)
        {
            if (!first) json.Append(",\n");
            first = false;
            json.Append(Object(InDomain(configuration)));
        }
        json.Append("\n],\n");
        json.Append("\"dataProtector\": ").Append(Object(DataProtection())).Append("\n}\n");
        File.WriteAllText(output, json.ToString(), new UTF8Encoding(false));
        Console.WriteLine("written " + output);
        return 0;
    }

    // The legacy ticket's fields the cases use.
    public static readonly string[][] TicketStrings =
    {
        new[] { "user", "data", "/" },
        new[] { "", "", "" },
        new[] { "ユーザー\U0001F600", "role=a;b", "/app/x" },
        new[] { new string('n', 300), "", "/" },
    };

    public const long IssueTime = 0x01DC7B1E2F3A4B5CL;
    public const long ExpireTime = -2;

    static IEnumerable<string> Construct()
    {
        foreach (var strings in TicketStrings)
        {
            int exact = 8 + 1 + (strings[0].Length + 1) * 2 + 8 + 1 + 8 + (strings[1].Length + 1) * 2 + (strings[2].Length + 1) * 2;
            foreach (var length in new[] { 4096, 0, 17, 18, exact, exact - 1 }.Distinct())
            {
                var buffer = new byte[Math.Max(length, 1)];
                for (int i = 0; i < 8 && i < buffer.Length; i++) buffer[i] = (byte)(0xA0 + i);
                int hr = CookieAuthConstructTicket(buffer, length, strings[0], strings[1], strings[2], new byte[] { 2, 1, 0, 0 }, new[] { IssueTime, ExpireTime });
                yield return Object(new Dictionary<string, object>
                {
                    ["name"] = strings[0], ["data"] = strings[1], ["path"] = strings[2], ["length"] = length,
                    ["result"] = hr, ["bytes"] = hr > 0 ? Hex(buffer.Take(hr).ToArray()) : "",
                });
            }
        }
        foreach (var nulls in new[] { 0, 1, 2 })
        {
            var buffer = new byte[100];
            int hr = CookieAuthConstructTicket(buffer, 100, nulls == 0 ? null : "n", nulls == 1 ? null : "d", nulls == 2 ? null : "p", new byte[] { 1, 0, 0, 0 }, new long[] { 1, 2 });
            yield return Object(new Dictionary<string, object> { ["null"] = nulls, ["length"] = 100, ["result"] = hr });
        }
    }

    static IEnumerable<string> Parse()
    {
        // A ticket of each case's strings, read whole and cut at each length, and into buffers of each size around the
        // strings' lengths.
        foreach (var strings in TicketStrings.Take(3))
        {
            var buffer = new byte[4096];
            for (int i = 0; i < 8; i++) buffer[i] = (byte)(0xA0 + i);
            int length = CookieAuthConstructTicket(buffer, buffer.Length, strings[0], strings[1], strings[2], new byte[] { 7, 1, 0, 0 }, new[] { IssueTime, ExpireTime });
            var ticket = buffer.Take(length).ToArray();
            for (int cut = length; cut >= 0; cut--)
                yield return ParseOne(ticket, cut, 512, 512, 512);
            int name = strings[0].Length, data = strings[1].Length, path = strings[2].Length;
            foreach (var sizes in new[] { new[] { name, 512, 512 }, new[] { name + 1, 512, 512 }, new[] { 512, data, 512 }, new[] { 512, data + 1, 512 },
                                          new[] { 512, 512, path }, new[] { 512, 512, path + 1 }, new[] { 0, 512, 512 }, new[] { 512, 0, 512 }, new[] { 512, 512, 0 }, new[] { 512, 512, -1 } })
                yield return ParseOne(ticket, length, sizes[0], sizes[1], sizes[2]);
        }
        // A string without its NUL before the end; null data.
        var unterminated = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, (byte)'a', 0, (byte)'b', 0 };
        yield return ParseOne(unterminated, unterminated.Length, 512, 512, 512);
        yield return ParseOne(null, 0, 512, 512, 512);
    }

    static string ParseOne(byte[] ticket, int length, int nameLength, int dataLength, int pathLength)
    {
        StringBuilder name = new StringBuilder(Math.Max(nameLength, 1)), data = new StringBuilder(Math.Max(dataLength, 1)), path = new StringBuilder(Math.Max(pathLength, 1));
        var bytes = new byte[4];
        var dates = new long[2];
        int hr = CookieAuthParseTicket(ticket, length, name, nameLength, data, dataLength, path, pathLength, bytes, dates);
        return Object(new Dictionary<string, object>
        {
            ["ticket"] = ticket == null ? null : Hex(ticket), ["length"] = length, ["nameLength"] = nameLength, ["dataLength"] = dataLength, ["pathLength"] = pathLength,
            ["result"] = hr, ["name"] = hr == 0 ? name.ToString() : null, ["data"] = hr == 0 ? data.ToString() : null, ["path"] = hr == 0 ? path.ToString() : null,
            ["version"] = hr == 0 ? bytes[0] : -1, ["persistent"] = hr == 0 ? bytes[1] : -1,
            ["issued"] = hr == 0 ? dates[0].ToString(CultureInfo.InvariantCulture) : null, ["expires"] = hr == 0 ? dates[1].ToString(CultureInfo.InvariantCulture) : null,
        });
    }

    static IEnumerable<string> Hashes()
    {
        var data = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
        var modifier = new byte[] { 1, 2, 3 };
        foreach (var size in new[] { 16, 20, 24, 32, 48, 64 })
        {
            var hash = new byte[size];
            int hr = GetSHA1Hash(data, data.Length, hash, size);
            // The keys padded to the hash's block (as MachineKeySection pads them: 64 bytes, SHA384's and SHA512's 128).
            int block = size > 32 ? 128 : 64;
            var inner = Enumerable.Range(0, block).Select(i => (byte)(0x36 ^ i)).ToArray();
            var outer = Enumerable.Range(0, block).Select(i => (byte)(0x5C ^ i)).ToArray();
            var mac = new byte[size];
            int macHr = GetHMACSHA1Hash(data, 4, 20, modifier, modifier.Length, inner, inner.Length, outer, outer.Length, mac, size);
            yield return Object(new Dictionary<string, object>
            {
                ["size"] = size, ["result"] = hr, ["hash"] = hr == 0 ? Hex(hash) : "", ["hmacResult"] = macHr, ["hmac"] = macHr == 0 ? Hex(mac) : "",
            });
        }
    }

    static Dictionary<string, object> InDomain(string[] configuration)
    {
        var file = Path.Combine(Path.GetTempPath(), "foc-golden-" + configuration[0] + ".config");
        File.WriteAllText(file,
            "<configuration><appSettings><add key=\"aspnet:UseLegacyFormsAuthenticationTicketCompatibility\" value=\"" + configuration[4] + "\" /></appSettings>" +
            "<system.web><machineKey validationKey=\"" + configuration[5] + "\" decryptionKey=\"" + configuration[6] + "\" validation=\"" + configuration[1] +
            "\" decryption=\"" + configuration[2] + "\" compatibilityMode=\"" + configuration[3] + "\"" +
            (configuration[7].Length > 0 ? " dataProtectorType=\"" + configuration[7] + "\" applicationName=\"GoldenApp\"" : "") + " /></system.web></configuration>");
        var setup = new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory, ConfigurationFile = file };
        var domain = AppDomain.CreateDomain(configuration[0], null, setup);
        try
        {
            var worker = (Worker)domain.CreateInstanceAndUnwrap(typeof(Worker).Assembly.FullName, typeof(Worker).FullName);
            var values = worker.Run();
            var result = new Dictionary<string, object>
            {
                ["name"] = configuration[0], ["validation"] = configuration[1], ["decryption"] = configuration[2], ["compatibilityMode"] = configuration[3],
                ["legacyTicket"] = configuration[4], ["validationKey"] = configuration[5], ["decryptionKey"] = configuration[6], ["dataProtectorType"] = configuration[7],
            };
            foreach (var pair in values) result[pair.Key] = pair.Value;
            return result;
        }
        finally
        {
            AppDomain.Unload(domain);
            File.Delete(file);
        }
    }

    // The values the tests' cases protect.
    public static readonly byte[] Data = Encoding.UTF8.GetBytes("golden data: ゴールデン");
    public static readonly string[] Purposes = { "Golden.Purpose", "user 1" };
    public const string Url = "d=Golden/resource.js|ゴ";

    public sealed class Worker : MarshalByRefObject
    {
        public Dictionary<string, string> Run()
        {
            var values = new Dictionary<string, string>();
            void Record(string key, Func<string> value)
            {
                try { values[key] = value(); }
                catch (Exception e) { values[key + "Error"] = (e is TargetInvocationException ? e.InnerException : e).GetType().FullName; }
            }
            var issued = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var ticket = new FormsAuthenticationTicket(2, "ユーザー\U0001F600name", issued, issued.AddMinutes(30), true, "role=admin;x", "/app");
            Record("ticket", () => FormsAuthentication.Encrypt(ticket));
            Record("protect", () => Hex(MachineKey.Protect(Data, Purposes)));
            Record("unprotectShort", () => Hex(MachineKey.Unprotect(new byte[40], Purposes)));
            Record("unprotectTampered", () => { var p = MachineKey.Protect(Data, Purposes); p[p.Length - 1] ^= 1; return Hex(MachineKey.Unprotect(p, Purposes)); });
            Record("unprotectOtherPurpose", () => Hex(MachineKey.Unprotect(MachineKey.Protect(Data, Purposes), "Other")));
            Record("encode", () => MachineKey.Encode(Data, MachineKeyProtection.All));
            Record("encodeValidation", () => MachineKey.Encode(Data, MachineKeyProtection.Validation));
            Record("encodeEncryption", () => MachineKey.Encode(Data, MachineKeyProtection.Encryption));
            var web = typeof(HttpContext).Assembly;
            var purpose = web.GetType("System.Web.Security.Cryptography.Purpose").GetField("AssemblyResourceLoader_WebResourceUrl", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            var encryptString = typeof(System.Web.UI.Page).GetMethod("EncryptString", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(string), purpose.GetType() }, null);
            Record("encryptString", () => (string)encryptString.Invoke(null, new[] { Url, purpose }));
            Record("los", () => { var w = new StringWriter(); new System.Web.UI.LosFormatter(true, "golden modifier").Serialize(w, Url); return w.ToString(); });
            if (values.ContainsKey("encryptString"))
                Record("encryptStringAgain", () => (string)encryptString.Invoke(null, new[] { Url, purpose }));
            return values;
        }
    }

    static Dictionary<string, object> DataProtection()
    {
        var result = new Dictionary<string, object>();
        void Record(string key, Func<object> value)
        {
            try { result[key] = value(); }
            catch (Exception e) { result[key + "Error"] = (e is TargetInvocationException ? e.InnerException : e).GetType().FullName; }
        }
        var data = Data;
        var protector = DataProtector.Create("DpapiDataProtector", "GoldenApp", "Golden.Primary", "s1", "s2");
        var protectedData = protector.Protect(data);
        result["protected"] = Hex(protectedData);
        var hashed = (byte[])typeof(DataProtector).GetMethod("GetHashedPurpose", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(protector, null);
        result["hashedPurpose"] = Hex(hashed);
        // What DpapiDataProtector's output is: DPAPI's of the data with the hashed purpose as its entropy, or of the hashed
        // purpose and the data without one.
        Record("unprotectWithHashAsEntropy", () => Hex(ProtectedData.Unprotect(protectedData, hashed, DataProtectionScope.CurrentUser)));
        Record("unprotectWithoutEntropy", () => Hex(ProtectedData.Unprotect(protectedData, null, DataProtectionScope.CurrentUser)));
        Record("isReprotectRequired", () => protector.IsReprotectRequired(protectedData));
        Record("unprotectOtherPurpose", () => Hex(DataProtector.Create("DpapiDataProtector", "GoldenApp", "Golden.Other").Unprotect(protectedData)));
        Record("unprotectGarbage", () => Hex(protector.Unprotect(new byte[] { 1, 2, 3 })));
        Record("protectNull", () => Hex(protector.Protect(null)));
        Record("unprotectNull", () => Hex(protector.Unprotect(null)));
        Record("createNullClass", () => DataProtector.Create(null, "a", "p").GetType().FullName);
        Record("createEmptyApplication", () => DataProtector.Create("DpapiDataProtector", " ", "p").GetType().FullName);
        Record("createEmptyPurpose", () => DataProtector.Create("DpapiDataProtector", "a", "").GetType().FullName);
        Record("createEmptySpecificPurpose", () => DataProtector.Create("DpapiDataProtector", "a", "p", "s", " ").GetType().FullName);
        Record("createNoSpecificPurposes", () => Hex(DataProtector.Create("DpapiDataProtector", "a", "p", null).Unprotect(DataProtector.Create("DpapiDataProtector", "a", "p").Protect(data))));
        Record("protectedDataRoundTrip", () => Hex(ProtectedData.Unprotect(ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser), null, DataProtectionScope.CurrentUser)));
        Record("protectedDataNull", () => Hex(ProtectedData.Protect(null, null, DataProtectionScope.CurrentUser)));
        Record("protectedDataBadScope", () => Hex(ProtectedData.Protect(data, null, (DataProtectionScope)5)));
        return result;
    }

    static string Hex(byte[] bytes) => bytes == null ? null : BitConverter.ToString(bytes).Replace("-", "");

    static string Object(Dictionary<string, object> values) =>
        "{" + string.Join(", ", values.Select(pair => Quote(pair.Key) + ": " + Value(pair.Value))) + "}";

    static string Value(object value)
    {
        switch (value)
        {
            case null: return "null";
            case bool b: return b ? "true" : "false";
            case int i: return i.ToString(CultureInfo.InvariantCulture);
            case byte b: return b.ToString(CultureInfo.InvariantCulture);
            default: return Quote(Convert.ToString(value, CultureInfo.InvariantCulture));
        }
    }

    static string Quote(string s)
    {
        var text = new StringBuilder("\"");
        foreach (char c in s)
        {
            if (c == '"' || c == '\\') text.Append('\\').Append(c);
            else if (c < 0x20 || c > 0x7E) text.Append("\\u").Append(((int)c).ToString("x4"));
            else text.Append(c);
        }
        return text.Append('"').ToString();
    }
}
