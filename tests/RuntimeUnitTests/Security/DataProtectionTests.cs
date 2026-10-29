using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using Mono.Security.Cryptography;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// DataProtector (System.Security's, in the port's System.Web: what &lt;machineKey dataProtectorType&gt; makes) and what it
/// protects with: ProtectedDataPortable (DPAPI on Windows, Mono's ManagedProtection elsewhere). DataProtector and DPAPI
/// against .NET Framework 4.8's (golden/net48.json, recorded as the same Windows user); ManagedProtection, which .NET
/// Framework has not, against ProtectedData's contract (its documentation; golden: a round trip, null refused).
/// </summary>
[Collection("Application")]
public class DataProtectionTests
{
    static JsonElement Golden48 => Golden.Root.GetProperty("dataProtector");

    static string Error(Action action)
    {
        try { action(); return null; }
        catch (TargetInvocationException e) { return e.InnerException.GetType().FullName; }
        catch (Exception e) { return e.GetType().FullName; }
    }

    static DataProtector Protector() => DataProtector.Create("DpapiDataProtector", "GoldenApp", "Golden.Primary", "s1", "s2");

    sealed class Purposes : DataProtector
    {
        public Purposes(string application, string primary, params string[] specific) : base(application, primary, specific) { }
        public byte[] Hashed => GetHashedPurpose();
        public string Application => ApplicationName;
        public string Primary => PrimaryPurpose;
        public IEnumerable<string> Specific => SpecificPurposes;
        public bool Prepends => PrependHashedPurposeToPlaintext;
    }

    [Fact] // DataProtector's documentation: the application name, the primary and specific purposes kept as given (none: empty)
    public void The_purposes_are_kept()
    {
        var purposes = new Purposes("GoldenApp", "Golden.Primary", "s1", "s2");
        Assert.Equal("GoldenApp", purposes.Application);
        Assert.Equal("Golden.Primary", purposes.Primary);
        Assert.Equal(new[] { "s1", "s2" }, purposes.Specific);
        Assert.Empty(new Purposes("a", "p", null).Specific);
        // .NET Framework's DataProtector (reference source): the hashed purpose prepended unless a protector says not
        Assert.True(purposes.Prepends);
    }

    [Fact]
    // .NET Framework 4.8 (golden): the purposes' hash (SHA256 of the application name, the primary and specific purposes, as BinaryWriter writes them), kept
    public void The_purposes_are_hashed_as_net_framework_hashes_them()
    {
        var purposes = new Purposes("GoldenApp", "Golden.Primary", "s1", "s2");
        Assert.Equal(Golden48.String("hashedPurpose"), Golden.Hex(purposes.Hashed));
        Assert.Same(purposes.Hashed, purposes.Hashed);
    }

    [Fact] // .NET Framework 4.8 (golden): an empty application name, primary or specific purpose: ArgumentException; null data: ArgumentNullException
    public void Empty_names_and_null_data_are_refused()
    {
        Assert.Equal(Golden48.String("createEmptyApplicationError"), Error(() => DataProtector.Create("DpapiDataProtector", " ", "p")));
        Assert.Equal(Golden48.String("createEmptyPurposeError"), Error(() => DataProtector.Create("DpapiDataProtector", "a", "")));
        Assert.Equal(Golden48.String("createEmptySpecificPurposeError"), Error(() => DataProtector.Create("DpapiDataProtector", "a", "p", "s", " ")));
        Assert.Equal(Golden48.String("protectNullError"), Error(() => Protector().Protect(null)));
        Assert.Equal(Golden48.String("unprotectNullError"), Error(() => Protector().Unprotect(null)));
    }

    [Fact]
    // .NET Framework 4.8 (golden; DataProtector.Create's documentation): providerClass null is ArgumentNullException
    public void A_null_provider_class_is_refused()
    {
        Assert.Equal(Golden48.String("createNullClassError"), Error(() => DataProtector.Create(null, "a", "p")));
    }

    [Fact]
    // .NET Framework 4.8 (golden)
    public void Null_specific_purposes_are_refused()
    {
        Assert.Equal(Golden48.String("createNoSpecificPurposesError"), Error(() => DataProtector.Create("DpapiDataProtector", "a", "p", null)));
    }

    [Fact]
    // .NET Framework 4.8 (golden): DpapiDataProtector.IsReprotectRequired is true
    public void Reprotecting_is_required_as_for_dpapi()
    {
        Assert.True(Golden48.GetProperty("isReprotectRequired").GetBoolean());
        Assert.True(Protector().IsReprotectRequired(Golden.Data));
    }

    [Fact]
    // .NET Framework 4.8 (golden): what DataProtector protected it unprotects; another purpose's or garbage: CryptographicException
    public void What_is_protected_is_unprotected()
    {
        if (!OperatingSystem.IsWindows()) return;
        Assert.Equal(Golden.Data, Protector().Unprotect(Protector().Protect(Golden.Data)));
        Assert.Equal(Golden48.String("unprotectOtherPurposeError"), Error(() => DataProtector.Create("DpapiDataProtector", "GoldenApp", "Golden.Other").Unprotect(Protector().Protect(Golden.Data))));
        Assert.Equal(Golden48.String("unprotectGarbageError"), Error(() => Protector().Unprotect(new byte[] { 1, 2, 3 })));
    }

    [Fact]
    // .NET Framework 4.8 (golden, the same Windows user): DpapiDataProtector's output (DPAPI's of the data, the hashed purpose its entropy) unprotected
    public void What_net_framework_protected_is_unprotected()
    {
        if (!OperatingSystem.IsWindows()) return;
        Assert.Equal(Golden.Hex(Golden.Data), Golden48.String("unprotectWithHashAsEntropy"));
        Assert.Equal(Golden.Data, Protector().Unprotect(Golden.Bytes(Golden48.String("protected"))));
    }

    [Fact]
    // .NET Framework 4.8 (golden): MachineKey.Protect and a forms authentication ticket with dataProtectorType="DpapiDataProtector"
    public void MachineKey_with_a_data_protector_reads_net_framework_s()
    {
        if (!OperatingSystem.IsWindows()) return;
        var golden = Golden.Configuration("F45-dpapi");
        using var keys = new MachineKeys(golden);
        Assert.Equal(Golden.Data, System.Web.Security.MachineKey.Unprotect(Golden.Bytes(golden.String("protect")), Golden.Purposes));
        Assert.Equal("ユーザー\U0001F600name", System.Web.Security.FormsAuthentication.Decrypt(golden.String("ticket")).Name);
    }

    // ProtectedDataPortable

    static readonly FieldInfo Implementation = typeof(ProtectedDataPortable).GetField("impl", BindingFlags.NonPublic | BindingFlags.Static);

    static void Use(string implementation) => Implementation.SetValue(null, Enum.Parse(Implementation.FieldType, implementation));

    [Fact] // .NET Framework 4.8 (golden; ProtectedData's documentation): null data is ArgumentNullException
    public void Null_data_is_refused()
    {
        Assert.Equal(Golden48.String("protectedDataNullError"), Error(() => ProtectedDataPortable.Protect(null, null, DataProtectionScope.CurrentUser)));
        Assert.Equal(typeof(ArgumentNullException).FullName, Error(() => ProtectedDataPortable.Unprotect(null, null, DataProtectionScope.CurrentUser)));
    }

    [Fact] // .NET Framework 4.8 (golden): on Windows DPAPI's output (ProtectedData reads it), of any scope value as DPAPI takes it
    public void On_windows_dpapi_protects()
    {
        if (!OperatingSystem.IsWindows()) return;
        Use("Unknown");
        var entropy = new byte[] { 9, 8, 7 };
        Assert.Equal(Golden.Data, ProtectedData.Unprotect(ProtectedDataPortable.Protect(Golden.Data, entropy, DataProtectionScope.CurrentUser), entropy, DataProtectionScope.CurrentUser));
        Assert.NotNull(Golden48.String("protectedDataBadScope"));
        Assert.Equal(Golden.Data, ProtectedData.Unprotect(ProtectedDataPortable.Protect(Golden.Data, null, (DataProtectionScope)5), null, DataProtectionScope.CurrentUser));
    }

    [Fact]
    // .NET Framework 4.8 (golden: ProtectedData's round trip)
    public void On_windows_what_is_protected_is_unprotected()
    {
        if (!OperatingSystem.IsWindows()) return;
        Use("Unknown");
        Assert.Equal(Golden48.String("protectedDataRoundTrip"),
            Golden.Hex(ProtectedDataPortable.Unprotect(ProtectedDataPortable.Protect(Golden.Data, null, DataProtectionScope.CurrentUser), null, DataProtectionScope.CurrentUser)));
    }

    [Fact] // the port's choice (Mono's ProtectedData): an unsupported platform is PlatformNotSupportedException
    public void An_unsupported_platform_is_not_supported()
    {
        try
        {
            Use("Unsupported");
            Assert.Throws<PlatformNotSupportedException>(() => ProtectedDataPortable.Protect(Golden.Data, null, DataProtectionScope.CurrentUser));
            Assert.Throws<PlatformNotSupportedException>(() => ProtectedDataPortable.Unprotect(Golden.Data, null, DataProtectionScope.CurrentUser));
        }
        finally { Use("Unknown"); }
    }

    // ManagedProtection (Mono's; the platforms other than Windows). Its keys (CurrentUser's, LocalMachine's) RSA keys made
    // here: GetKey makes them in the CSP's "DAPI" container, which would stay in the Windows user's profile.

    static readonly FieldInfo UserKey = typeof(ManagedProtection).GetField("user", BindingFlags.NonPublic | BindingFlags.Static);
    static readonly FieldInfo MachineScopeKey = typeof(ManagedProtection).GetField("machine", BindingFlags.NonPublic | BindingFlags.Static);

    sealed class Managed : IDisposable
    {
        public readonly RSA User = RSA.Create(1536), Machine = RSA.Create(1536);

        public Managed()
        {
            UserKey.SetValue(null, User);
            MachineScopeKey.SetValue(null, Machine);
            Use("ManagedProtection");
        }

        public void Dispose()
        {
            Use("Unknown");
            UserKey.SetValue(null, null);
            MachineScopeKey.SetValue(null, null);
            User.Dispose();
            Machine.Dispose();
        }
    }

    [Fact] // ProtectedData's documentation: what is protected is unprotected with the same entropy and scope (an empty entropy as none)
    public void Managed_protection_round_trips()
    {
        using var keys = new Managed();
        var entropy = new byte[] { 1, 2, 3 };
        foreach (var scope in new[] { DataProtectionScope.CurrentUser, DataProtectionScope.LocalMachine })
        {
            Assert.Equal(Golden.Data, ProtectedDataPortable.Unprotect(ProtectedDataPortable.Protect(Golden.Data, null, scope), null, scope));
            Assert.Equal(Golden.Data, ProtectedDataPortable.Unprotect(ProtectedDataPortable.Protect(Golden.Data, entropy, scope), entropy, scope));
            Assert.Equal(Golden.Data, ProtectedDataPortable.Unprotect(ProtectedDataPortable.Protect(Golden.Data, System.Array.Empty<byte>(), scope), null, scope));
            Assert.Empty(ManagedProtection.Unprotect(ManagedProtection.Protect(System.Array.Empty<byte>(), null, scope), null, scope));
        }
        Assert.Throws<ArgumentNullException>(() => ManagedProtection.Protect(null, null, DataProtectionScope.CurrentUser));
        Assert.Throws<ArgumentNullException>(() => ManagedProtection.Unprotect(null, null, DataProtectionScope.CurrentUser));
    }

    [Fact] // ProtectedData's documentation (CryptographicException when unprotecting fails): another entropy, scope, a changed payload
    public void Managed_protection_refuses_what_it_did_not_protect_so()
    {
        using var keys = new Managed();
        var entropy = new byte[] { 1, 2, 3 };
        var withEntropy = ProtectedDataPortable.Protect(Golden.Data, entropy, DataProtectionScope.CurrentUser);
        var without = ProtectedDataPortable.Protect(Golden.Data, null, DataProtectionScope.CurrentUser);
        var error = Assert.Throws<CryptographicException>(() => ProtectedDataPortable.Unprotect(withEntropy, new byte[] { 1, 2, 4 }, DataProtectionScope.CurrentUser));
        Assert.IsType<CryptographicException>(error.InnerException);
        Assert.Throws<CryptographicException>(() => ProtectedDataPortable.Unprotect(withEntropy, null, DataProtectionScope.CurrentUser));
        Assert.Throws<CryptographicException>(() => ProtectedDataPortable.Unprotect(without, entropy, DataProtectionScope.CurrentUser));
        Assert.Throws<CryptographicException>(() => ProtectedDataPortable.Unprotect(without, null, DataProtectionScope.LocalMachine));
        var changed = (byte[])without.Clone();
        changed[^1] ^= 1;
        Assert.Throws<CryptographicException>(() => ProtectedDataPortable.Unprotect(changed, null, DataProtectionScope.CurrentUser));
        changed = (byte[])without.Clone();
        changed[^17] ^= 1;
        Assert.Throws<CryptographicException>(() => ProtectedDataPortable.Unprotect(changed, null, DataProtectionScope.CurrentUser));
        Assert.Throws<CryptographicException>(() => ProtectedDataPortable.Unprotect(new byte[100], null, DataProtectionScope.CurrentUser));
        Assert.Throws<CryptographicException>(() => ProtectedDataPortable.Unprotect(without.Take(192).ToArray(), null, DataProtectionScope.CurrentUser));
    }

    [Fact] // the port's choice (Mono's ManagedProtection: a header of its own structure, 68 bytes of version, key, IV and digest): another one is refused
    public void Managed_protection_refuses_another_header()
    {
        using var keys = new Managed();
        var formatter = new RSAOAEPKeyExchangeFormatter(keys.User);
        var payload = ManagedProtection.Protect(Golden.Data, null, DataProtectionScope.CurrentUser).Skip(192).ToArray();
        // A secret of another length.
        Assert.Throws<CryptographicException>(() => ManagedProtection.Unprotect(formatter.CreateKeyExchange(new byte[10]).Concat(payload).ToArray(), null, DataProtectionScope.CurrentUser));
        // 68 bytes, the sizes of another structure (key, IV, digest).
        foreach (var at in new[] { 1, 18, 35 })
        {
            var secret = new byte[68];
            secret[0] = 1; secret[1] = 16; secret[18] = 16; secret[35] = 32;
            secret[at] = 8;
            Assert.Throws<CryptographicException>(() => ManagedProtection.Unprotect(formatter.CreateKeyExchange(secret).Concat(payload).ToArray(), null, DataProtectionScope.CurrentUser));
        }
    }

    [Fact] // the port's choice (Mono's ManagedProtection): a scope that is neither CurrentUser nor LocalMachine is CryptographicException (DPAPI takes it: golden)
    public void Managed_protection_refuses_another_scope()
    {
        using var keys = new Managed();
        var error = Assert.Throws<CryptographicException>(() => ProtectedDataPortable.Protect(Golden.Data, null, (DataProtectionScope)5));
        Assert.IsType<CryptographicException>(error.InnerException);
        Assert.Throws<CryptographicException>(() => ManagedProtection.Unprotect(new byte[300], null, (DataProtectionScope)5));
    }

    [Fact] // .NET Framework 4.8 (observed: DataProtector.Create, CryptoConfig's names): DpapiDataProtector's full name; an unknown name is null
    public void The_provider_class_is_named_as_crypto_config_names_it()
    {
        Assert.Equal("System.Security.Cryptography.DpapiDataProtector", DataProtector.Create("System.Security.Cryptography.DpapiDataProtector", "a", "p", "s").GetType().FullName);
        Assert.Null(DataProtector.Create("Nope", "a", "p", "s"));
    }

    [Fact] // the port's choice: without DPAPI (ManagedProtection), DpapiDataProtector prepends the hashed purpose and gives no entropy
    public void Without_dpapi_the_hashed_purpose_is_prepended()
    {
        using var keys = new Managed();
        var hash = Golden48.String("hashedPurpose");
        Assert.Equal(Golden.Data, Protector().Unprotect(Protector().Protect(Golden.Data)));
        Assert.Equal(hash + Golden.Hex(Golden.Data), Golden.Hex(ProtectedDataPortable.Unprotect(Protector().Protect(Golden.Data), null, DataProtectionScope.CurrentUser)));
        Assert.Throws<CryptographicException>(() => DataProtector.Create("DpapiDataProtector", "GoldenApp", "Golden.Other").Unprotect(Protector().Protect(Golden.Data)));
    }

    // DataProtector's own logic, with the purposes' hash .NET Framework computes (golden) and ManagedProtection under it
    // (as ProtectedDataPortable has it elsewhere than Windows).
    sealed class HashedProtector : DataProtector
    {
        readonly byte[] hash;
        readonly bool prepend;
        public HashedProtector(string primary, byte[] hash, bool prepend = true) : base("GoldenApp", primary, null) { this.hash = hash; this.prepend = prepend; }
        protected override byte[] GetHashedPurpose() => hash;
        protected override bool PrependHashedPurposeToPlaintext => prepend;
    }

    [Fact] // .NET Framework 4.8's DataProtector base class (System.Security; reference source): the hashed purpose prepended, checked and removed; another purpose's or none: CryptographicException
    public void The_hashed_purpose_is_prepended_and_checked()
    {
        using var keys = new Managed();
        var hash = Golden.Bytes(Golden48.String("hashedPurpose"));
        var protector = new HashedProtector("Golden.Primary", hash);
        Assert.Equal(Golden.Data, protector.Unprotect(protector.Protect(Golden.Data)));
        Assert.Equal(Golden48.String("hashedPurpose") + Golden.Hex(Golden.Data),
            Golden.Hex(ProtectedDataPortable.Unprotect(protector.Protect(Golden.Data), null, DataProtectionScope.CurrentUser)));
        Assert.Throws<CryptographicException>(() => new HashedProtector("Golden.Other", SHA256.HashData(hash)).Unprotect(protector.Protect(Golden.Data)));
        Assert.Throws<CryptographicException>(() => protector.Unprotect(ProtectedDataPortable.Protect(new byte[5], null, DataProtectionScope.CurrentUser)));
    }

    [Fact] // .NET Framework 4.8's DataProtector base class (reference source): a protector that does not prepend the hash protects the data as it is
    public void Without_the_hash_the_data_is_protected_as_it_is()
    {
        using var keys = new Managed();
        var protector = new HashedProtector("Golden.Primary", Golden.Bytes(Golden48.String("hashedPurpose")), prepend: false);
        Assert.Equal(Golden.Data, ProtectedDataPortable.Unprotect(protector.Protect(Golden.Data), null, DataProtectionScope.CurrentUser));
        Assert.Equal(Golden.Data, protector.Unprotect(ProtectedDataPortable.Protect(Golden.Data, null, DataProtectionScope.CurrentUser)));
    }
}
