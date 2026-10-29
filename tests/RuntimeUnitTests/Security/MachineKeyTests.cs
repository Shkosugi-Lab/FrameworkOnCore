using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.Security.Cryptography;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// What System.Web protects with a &lt;machineKey&gt; (MachineKeySection's legacy routines, the port's managed webengine4.dll
/// hashes in them; NetFXCryptoService's kept algorithms; the legacy ticket format) read and written as .NET Framework 4.8
/// does with the same configuration: its outputs (golden/net48.json) read by the port, the port's deterministic outputs
/// byte for byte the same, its errors the same.
/// </summary>
[Collection("Application")]
public class MachineKeyTests
{
    public static IEnumerable<object[]> Configurations() => Golden.ConfigurationNames(c => c.String("dataProtectorType") == "");

    static string Error(Action action)
    {
        try { action(); return null; }
        catch (System.Reflection.TargetInvocationException e) { return e.InnerException.GetType().FullName; }
        catch (Exception e) { return e.GetType().FullName; }
    }

    [Theory, MemberData(nameof(Configurations))] // .NET Framework 4.8 (golden): its forms authentication ticket read; the port's read back
    public void A_forms_authentication_ticket_of_net_framework_is_read(string name)
    {
        var golden = Golden.Configuration(name);
        using var keys = new MachineKeys(golden);
        var expected = Golden.Ticket();
        if (golden.String("ticketError") is string error)
        {
            Assert.Equal(error, Error(() => FormsAuthentication.Encrypt(expected)));
            return;
        }
        foreach (var encrypted in new[] { golden.String("ticket"), FormsAuthentication.Encrypt(expected) })
        {
            var ticket = FormsAuthentication.Decrypt(encrypted);
            Assert.NotNull(ticket);
            Assert.Equal(expected.Version, ticket.Version);
            Assert.Equal(expected.Name, ticket.Name);
            Assert.Equal(expected.IssueDate, ticket.IssueDate.ToUniversalTime());
            Assert.Equal(expected.Expiration, ticket.Expiration.ToUniversalTime());
            Assert.True(ticket.IsPersistent);
            Assert.Equal(expected.UserData, ticket.UserData);
            Assert.Equal(expected.CookiePath, ticket.CookiePath);
        }
    }

    [Theory, MemberData(nameof(Configurations))] // .NET Framework 4.8 (golden): a ticket changed by a bit is no ticket (Decrypt: null, or an exception)
    public void A_changed_ticket_is_refused(string name)
    {
        var golden = Golden.Configuration(name);
        if (golden.String("ticket") == null) return;
        using var keys = new MachineKeys(golden);
        var bytes = Golden.Bytes(golden.String("ticket"));
        bytes[bytes.Length / 2] ^= 1;
        FormsAuthenticationTicket ticket = null;
        var error = Error(() => ticket = FormsAuthentication.Decrypt(Golden.Hex(bytes)));
        Assert.Null(ticket);
        Assert.True(error == null || error == typeof(HttpException).FullName || error == typeof(CryptographicException).FullName, error);
    }

    [Theory, MemberData(nameof(Configurations))] // .NET Framework 4.8 (golden): MachineKey.Protect's output unprotected; its errors the same
    public void MachineKey_Protect_of_net_framework_is_unprotected(string name)
    {
        var golden = Golden.Configuration(name);
        using var keys = new MachineKeys(golden);
        if (golden.String("protectError") is string error)
        {
            Assert.Equal(error, Error(() => MachineKey.Protect(Golden.Data, Golden.Purposes)));
            return;
        }
        Assert.Equal(Golden.Data, MachineKey.Unprotect(Golden.Bytes(golden.String("protect")), Golden.Purposes));
        Assert.Equal(Golden.Data, MachineKey.Unprotect(MachineKey.Protect(Golden.Data, Golden.Purposes), Golden.Purposes));
        Assert.Equal(golden.String("unprotectShortError"), Error(() => MachineKey.Unprotect(new byte[40], Golden.Purposes)));
        var tampered = MachineKey.Protect(Golden.Data, Golden.Purposes);
        tampered[^1] ^= 1;
        Assert.Equal(golden.String("unprotectTamperedError"), Error(() => MachineKey.Unprotect(tampered, Golden.Purposes)));
        Assert.Equal(golden.String("unprotectOtherPurposeError"), Error(() => MachineKey.Unprotect(MachineKey.Protect(Golden.Data, Golden.Purposes), "Other")));
    }

    [Theory, MemberData(nameof(Configurations))] // .NET Framework 4.8 (golden): MachineKey.Encode's hash the same bytes; its encryption decoded; its errors the same
    public void MachineKey_Encode_is_net_framework_s(string name)
    {
        var golden = Golden.Configuration(name);
        using var keys = new MachineKeys(golden);
        if (golden.String("encodeValidationError") is string error)
        {
            Assert.Equal(error, Error(() => MachineKey.Encode(Golden.Data, MachineKeyProtection.Validation)));
            Assert.Equal(golden.String("encodeError"), Error(() => MachineKey.Encode(Golden.Data, MachineKeyProtection.All)));
            Assert.Equal(golden.String("encodeEncryptionError"), Error(() => MachineKey.Encode(Golden.Data, MachineKeyProtection.Encryption)));
            return;
        }
        Assert.Equal(golden.String("encodeValidation"), MachineKey.Encode(Golden.Data, MachineKeyProtection.Validation));
        Assert.Equal(Golden.Data, MachineKey.Decode(golden.String("encodeValidation"), MachineKeyProtection.Validation));
        Assert.Equal(Golden.Data, MachineKey.Decode(golden.String("encode"), MachineKeyProtection.All));
        Assert.Equal(Golden.Data, MachineKey.Decode(golden.String("encodeEncryption"), MachineKeyProtection.Encryption));
        Assert.Equal(Golden.Data, MachineKey.Decode(MachineKey.Encode(Golden.Data, MachineKeyProtection.All), MachineKeyProtection.All));
    }

    [Theory, MemberData(nameof(Configurations))] // .NET Framework 4.8 (golden): a WebResource URL's protected part (Page.EncryptString: a predictable IV) the same bytes
    public void A_protected_resource_url_is_net_framework_s(string name)
    {
        var golden = Golden.Configuration(name);
        using var keys = new MachineKeys(golden);
        if (golden.String("encryptStringError") is string error)
        {
            Assert.Equal(error, Error(() => System.Web.UI.Page.EncryptString(Golden.Url, Purpose.AssemblyResourceLoader_WebResourceUrl)));
            return;
        }
        Assert.Equal(golden.String("encryptString"), System.Web.UI.Page.EncryptString(Golden.Url, Purpose.AssemblyResourceLoader_WebResourceUrl));
        Assert.Equal(golden.String("encryptStringAgain"), System.Web.UI.Page.EncryptString(Golden.Url, Purpose.AssemblyResourceLoader_WebResourceUrl));
        Assert.Equal(Golden.Url, System.Web.UI.Page.DecryptString(golden.String("encryptString"), Purpose.AssemblyResourceLoader_WebResourceUrl));
    }

    [Theory, MemberData(nameof(Configurations))] // .NET Framework 4.8 (golden): LosFormatter's MAC (MachineKeySection's legacy HMAC, a modifier appended) the same bytes
    public void A_los_formatter_s_mac_is_net_framework_s(string name)
    {
        var golden = Golden.Configuration(name);
        using var keys = new MachineKeys(golden);
        var formatter = new System.Web.UI.LosFormatter(true, "golden modifier");
        var writer = new StringWriter();
        if (golden.String("losError") is string error)
        {
            Assert.Equal(error, Error(() => formatter.Serialize(writer, Golden.Url)));
            return;
        }
        Assert.Equal(Golden.Url, formatter.Deserialize(golden.String("los")));
        formatter.Serialize(writer, Golden.Url);
        if (name.EndsWith("AES-AES")) Assert.Equal(Golden.Url, formatter.Deserialize(writer.ToString())); // encrypted: a random IV
        else Assert.Equal(golden.String("los"), writer.ToString());
        Assert.Equal(typeof(HttpException).FullName, Error(() => new System.Web.UI.LosFormatter(true, "another modifier").Deserialize(golden.String("los"))));
    }

    [Fact] // .NET Framework 4.8 (golden): webengine4.dll's GetHMACSHA1Hash (padded keys, data, modifier) by the hash's size; another size: no hash, the managed algorithm then
    public void The_hmac_of_webengine_is_of_the_size_asked_for()
    {
        using var keys = new MachineKeys(Golden.Configuration("F45-HMACSHA256-AES"));
        var type = typeof(MachineKeySection);
        const System.Reflection.BindingFlags Static = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;
        var hmac = type.GetMethod("GetHMACSHA1Hash", Static);
        var data = System.Text.Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
        foreach (var record in Golden.Array("hash"))
        {
            int size = record.Int("size"), block = size > 32 ? 128 : 64;
            type.GetField("s_inner", Static).SetValue(null, Enumerable.Range(0, block).Select(i => (byte)(0x36 ^ i)).ToArray());
            type.GetField("s_outer", Static).SetValue(null, Enumerable.Range(0, block).Select(i => (byte)(0x5C ^ i)).ToArray());
            type.GetField("_HashSize", Static).SetValue(null, size);
            type.GetField("_UseHMACSHA", Static).SetValue(null, true);
            var hash = (byte[])hmac.Invoke(null, new object[] { data, new byte[] { 1, 2, 3 }, 4, 20 });
            if (record.Int("hmacResult") == 0)
                Assert.Equal(record.String("hmac"), Golden.Hex(hash));
            else
            {
                Assert.Null(hash);
                Assert.False((bool)type.GetField("_UseHMACSHA", Static).GetValue(null));
            }
        }
    }

    [Fact] // the port's choice (NetFXCryptoService's summary): the kept algorithms, dropped when too many, are made again; what they protected is read
    public void Values_protected_under_many_purposes_are_read()
    {
        using var keys = new MachineKeys(Golden.Configuration("F45-HMACSHA256-AES"));
        var purposes = Enumerable.Range(0, 100).Select(i => "purpose " + i).ToArray();
        var protectedValues = purposes.Select(p => MachineKey.Protect(Golden.Data, p)).ToArray();
        for (int i = 0; i < purposes.Length; i++)
            Assert.Equal(Golden.Data, MachineKey.Unprotect(protectedValues[i], purposes[i]));
    }

    [Fact] // the port's choice (NetFXCryptoService's summary): a custom algorithm ("alg:" not of .NET's) goes the original way: the same bytes as the kept one
    public void A_custom_algorithm_protects_as_the_built_in_one()
    {
        var golden = Golden.Configuration("F45-HMACSHA256-AES");
        using var keys = new MachineKeys(golden);
        // A purpose with a specific purpose: its keys derived for each provider (a built-in purpose keeps the first ones).
        var purpose = Purpose.User_MachineKey_Protect.AppendSpecificPurposes(new[] { "custom" });
        var builtIn = new AspNetCryptoServiceProvider(keys.Section, new MachineKeyCryptoAlgorithmFactory(keys.Section),
            new MachineKeyMasterKeyProvider(keys.Section), new MachineKeyDataProtectorFactory(keys.Section), SP800_108.DeriveKey);
        var custom = new AspNetCryptoServiceProvider(keys.Section, new CustomAlgorithms(),
            new MachineKeyMasterKeyProvider(keys.Section), new MachineKeyDataProtectorFactory(keys.Section), SP800_108.DeriveKey);
        var clear = System.Text.Encoding.UTF8.GetBytes(Golden.Url);
        var expected = builtIn.GetCryptoService(purpose, CryptoServiceOptions.CacheableOutput).Protect(clear);
        Assert.Equal(expected, builtIn.GetCryptoService(purpose, CryptoServiceOptions.CacheableOutput).Protect(clear));
        var service = custom.GetCryptoService(purpose, CryptoServiceOptions.CacheableOutput);
        Assert.Equal(expected, service.Protect(clear));
        Assert.Equal(clear, service.Unprotect(expected));
        Assert.Equal(clear, custom.GetCryptoService(purpose).Unprotect(builtIn.GetCryptoService(purpose).Protect(clear)));
        Assert.Throws<CryptographicException>(() => service.Unprotect(new byte[40]));
    }

    // AES and HMACSHA256 as a custom <machineKey> algorithm gives them: classes not .NET's own.
    sealed class CustomAlgorithms : ICryptoAlgorithmFactory
    {
        public SymmetricAlgorithm GetEncryptionAlgorithm() => new CustomAes();
        public KeyedHashAlgorithm GetValidationAlgorithm() => new CustomHmac();
    }

    sealed class CustomAes : Aes
    {
        readonly Aes aes = Aes.Create();
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV) => aes.CreateDecryptor(rgbKey, rgbIV);
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV) => aes.CreateEncryptor(rgbKey, rgbIV);
        public override void GenerateIV() => IV = RandomNumberGenerator.GetBytes(16);
        public override void GenerateKey() => Key = RandomNumberGenerator.GetBytes(32);
    }

    sealed class CustomHmac : KeyedHashAlgorithm
    {
        HMACSHA256 hmac;
        public CustomHmac() => HashSizeValue = 256;
        public override void Initialize() => hmac = null;
        protected override void HashCore(byte[] array, int ibStart, int cbSize) => (hmac ??= new HMACSHA256(Key)).TransformBlock(array, ibStart, cbSize, null, 0);
        protected override byte[] HashFinal()
        {
            (hmac ??= new HMACSHA256(Key)).TransformFinalBlock(System.Array.Empty<byte>(), 0, 0);
            var hash = hmac.Hash;
            hmac = null;
            return hash;
        }
    }

    [Fact] // .NET Framework 4.8 (golden): webengine4.dll's GetSHA1Hash, the hash of the size asked for (MD5, SHA1, SHA256/384/512); another size E_INVALIDARG (ArgumentException)
    public void The_hash_of_webengine_is_of_the_size_asked_for()
    {
        var method = typeof(MachineKeySection).GetMethod("WebEngineHash", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var data = System.Text.Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
        foreach (var record in Golden.Array("hash"))
        {
            int size = record.Int("size");
            if (record.Int("result") == 0)
                Assert.Equal(record.String("hash"), Golden.Hex((byte[])method.Invoke(null, new object[] { data, size })));
            else
            {
                Assert.Equal(unchecked((int)0x80070057), record.Int("result"));
                Assert.Equal(typeof(ArgumentException).FullName, Error(() => method.Invoke(null, new object[] { data, size })));
                Assert.IsType<ArgumentException>(Marshal.GetExceptionForHR(record.Int("result")));
            }
        }
    }
}
