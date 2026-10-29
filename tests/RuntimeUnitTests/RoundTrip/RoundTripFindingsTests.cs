using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using FrameworkOnCore.Runtime.Tests.Security;
using BinaryFormatter = WebFormsForCore.Serialization.Formatters.Binary.BinaryFormatter;
using FormatterServices = WebFormsForCore.Serialization.Formatters.Binary.FormatterServices;

namespace FrameworkOnCore.Runtime.Tests.RoundTrip;

/// <summary>
/// What the round trips against .NET Framework 4.8 (RoundTripTests) found in the port, each by itself (the golden files'
/// cases show them too), and the round trips no golden file has: a page's view state with its MAC and encryption, DataProtector.
/// </summary>
[Collection("Application")]
public class RoundTripFindingsTests
{
    const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    static byte[] Serialize(object value)
    {
        var s = new MemoryStream();
        new BinaryFormatter().Serialize(s, value);
        return s.ToArray();
    }

    static object Deserialize(byte[] bytes) => new BinaryFormatter().Deserialize(new MemoryStream(bytes));

    // ---- BinaryFormatter: .NET Framework's serializable types that .NET's are not (FrameworkSurrogates) ----

    [Fact] // BinaryFormatter (session state, view state's [Serializable] objects, profiles): a CultureInfo, as .NET Framework writes it (its name, the user's overrides)
    public void A_CultureInfo_is_serialized_as_on_NET_Framework()
    {
        var bytes = Serialize(new CultureInfo("ja-JP", false));
        var text = Encoding.UTF8.GetString(bytes);
        Assert.Contains("System.Globalization.CultureInfo", text);
        var culture = Assert.IsType<CultureInfo>(Deserialize(bytes));
        Assert.Equal("ja-JP", culture.Name);
        Assert.False(culture.UseUserOverride);
        Assert.False(culture.IsReadOnly);
        culture.NumberFormat.CurrencySymbol = "X";   // its own, not a shared instance
        Assert.True(CultureInfo.ReadOnly(culture).IsReadOnly);
        Assert.True(((CultureInfo)Deserialize(Serialize(CultureInfo.GetCultureInfo("fr-FR")))).IsReadOnly);
    }

    [Fact] // BinaryFormatter: a Type, as .NET Framework's UnitySerializationHolder (its full name with mscorlib's names, its assembly's)
    public void A_Type_is_serialized_as_on_NET_Framework()
    {
        foreach (var type in new[] { typeof(int), typeof(List<string>), typeof(Dictionary<string, List<Person>>), typeof(int[,]), typeof(Page), typeof(Box<>), typeof(Person[]) })
        {
            var bytes = Serialize(new object[] { type, type });
            var text = Encoding.UTF8.GetString(bytes);
            Assert.Contains("System.UnitySerializationHolder", text);
            Assert.DoesNotContain("System.Private.CoreLib", text);
            var back = (object[])Deserialize(bytes);
            Assert.Same(type, back[0]);
            Assert.Same(back[0], back[1]);
        }
        Assert.Contains("System.Web, Version=4.0.0.0", Encoding.UTF8.GetString(Serialize(typeof(Page))));
        // DBNull, the other holder's value, as before
        Assert.Same(DBNull.Value, Deserialize(Serialize(DBNull.Value)));
    }

    [Fact] // BinaryFormatter: a NameValueCollection (its keys and values, null ones, the comparer, read-only), as .NET Framework writes it
    public void A_NameValueCollection_is_serialized_as_on_NET_Framework()
    {
        var collection = new NameValueCollection { { "a", "1" }, { "A", "2" }, { "b", null }, { null, "nokey" } };
        var bytes = Serialize(new Pair(collection, collection));
        var text = Encoding.UTF8.GetString(bytes);
        Assert.Contains("System.Collections.CaseInsensitiveHashCodeProvider", text);
        Assert.Contains("System, Version=4.0.0.0", text);
        var pair = (Pair)Deserialize(bytes);
        var back = Assert.IsType<NameValueCollection>(pair.First);
        Assert.Same(back, pair.Second);
        Assert.Equal(new[] { "a", "b", null }, back.AllKeys);
        Assert.Equal(new[] { "1", "2" }, back.GetValues("A"));   // case-insensitive, as before
        Assert.Null(back.GetValues("b"));
        Assert.Equal("nokey", back[null]);
        var ordinal = (NameValueCollection)Deserialize(Serialize(new NameValueCollection(StringComparer.Ordinal) { { "a", "1" }, { "A", "2" } }));
        Assert.Equal(2, ordinal.Count);
        Assert.True(FormatterServices.IsSerializable(typeof(NameValueCollection)) && FormatterServices.IsSerializable(typeof(CultureInfo)) && FormatterServices.IsSerializable(typeof(Type)));
    }

    [Fact] // BinaryFormatter (and what keeps a value with it: session state, an object in view state): shared references and cycles kept
    public void Shared_references_and_cycles_are_kept()
    {
        var person = (Person)Deserialize(Serialize(Values.PersonGraph()));
        Assert.Same(person, person.Friend.Friend);
        Assert.Same(person.Friend.Tag, person.Friend.Friend);
        Assert.Same(person, ((object[])person.Tag)[0]);
        var ring = (Node)Deserialize(Serialize(Values.Ring(3)));
        Assert.Same(ring, ring.Next.Next.Next);
        Assert.Same(ring, ring.Next.Other);
        var items = new SessionStateItemCollection { ["graph"] = Values.PersonGraph() };
        var s = new MemoryStream();
        items.Serialize(new BinaryWriter(s));
        var back = (Person)SessionStateItemCollection.Deserialize(new BinaryReader(new MemoryStream(s.ToArray())))["graph"];
        Assert.Same(back, back.Friend.Friend);
        var viewState = new MemoryStream();
        new ObjectStateFormatter().Serialize(viewState, new Pair(Values.Ring(2), null));
        var node = (Node)((Pair)new ObjectStateFormatter().Deserialize(new MemoryStream(viewState.ToArray()))).First;
        Assert.Same(node, node.Next.Next);
    }

    [Fact] // the type names ObjectStateFormatter, ResX and .resources write: .NET Framework's (mscorlib, System, System.Web 4.0.0.0), a one-dimensional array of a lower bound as T[*]
    public void Types_are_named_as_on_NET_Framework()
    {
        Assert.Equal("System.DayOfWeek, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", FormatterServices.GetFrameworkAssemblyQualifiedName(typeof(DayOfWeek)));
        Assert.Equal("System.Web.UI.Pair, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", FormatterServices.GetFrameworkAssemblyQualifiedName(typeof(Pair)));
        Assert.Equal("System.Collections.Specialized.NameValueCollection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
            FormatterServices.GetFrameworkAssemblyQualifiedName(typeof(NameValueCollection)));
        Assert.StartsWith("System.Int32[*], mscorlib", FormatterServices.GetFrameworkAssemblyQualifiedName(Array.CreateInstance(typeof(int), new[] { 1 }, new[] { 3 }).GetType()));
        Assert.StartsWith("System.RuntimeType, mscorlib", FormatterServices.GetFrameworkAssemblyQualifiedName(typeof(int).GetType()));
        // ObjectStateFormatter: an enum of mscorlib, a CultureInfo by its converter (a new one, as .NET Framework's converter made it)
        var osf = new ObjectStateFormatter();
        var s = new MemoryStream();
        osf.Serialize(s, new object[] { DayOfWeek.Friday, new CultureInfo("en-US") });
        Assert.Contains("System.DayOfWeek, mscorlib, Version=4.0.0.0", Encoding.UTF8.GetString(s.ToArray()));
        var first = (CultureInfo)((object[])new ObjectStateFormatter().Deserialize(new MemoryStream(s.ToArray())))[1];
        var second = (CultureInfo)((object[])new ObjectStateFormatter().Deserialize(new MemoryStream(s.ToArray())))[1];
        Assert.True(first.UseUserOverride);
        Assert.NotSame(first, second);
    }

    // ---- strings .NET Framework's BinaryWriter refused ----

    [Fact] // .NET Framework's BinaryWriter refused a lone surrogate (UTF-8 that throws); .NET's writes U+FFFD: view state and session state refuse it, as before
    public void A_lone_surrogate_is_refused_as_on_NET_Framework()
    {
        Assert.Throws<ArgumentException>(() => new ObjectStateFormatter().Serialize(new MemoryStream(), "a\uD800"));
        var items = new SessionStateItemCollection { ["k"] = "\uDC00" };
        var data = new SessionStateStoreData(items, new HttpStaticObjectsCollection(), 20);
        var serialize = typeof(SessionStateUtility).GetMethod("Serialize", Static);
        var e = Assert.Throws<TargetInvocationException>(() => serialize.Invoke(null, new object[] { data, new MemoryStream() }));
        Assert.IsType<EncoderFallbackException>(e.InnerException);
    }

    // ---- session state ----

    /// <summary>A stream that gives at most a few bytes a read (as .NET's DeflateStream may since .NET 6).</summary>
    sealed class Trickle(Stream inner) : Stream
    {
        public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, Math.Min(count, 7));
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => inner.Position; set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    [Fact] // <sessionState compressionEnabled="true">: the items' bytes read whole from a stream that gives them in parts (.NET's DeflateStream): "invalid session state" before
    public void Session_items_are_read_from_a_stream_that_gives_less_than_asked()
    {
        var items = new SessionStateItemCollection { ["big"] = Enumerable.Range(0, 20000).Select(i => (byte)(i * 7)).ToArray(), ["s"] = "x" };
        var s = new MemoryStream();
        var writer = new BinaryWriter(s);
        items.Serialize(writer);
        writer.Flush();
        var back = SessionStateItemCollection.Deserialize(new BinaryReader(new Trickle(new MemoryStream(s.ToArray()))));
        Assert.Equal((byte[])items["big"], (byte[])back["big"]);
        Assert.Equal("x", back["s"]);
        // through the stores' compressed format
        var data = new SessionStateStoreData(items, Formats.StaticObjects("s", "static"), 20);
        var arguments = new object[] { data, 7000, null, 0, true };
        typeof(SessionStateUtility).GetMethod("SerializeStoreData", Static).Invoke(null, arguments);
        var stored = ((byte[])arguments[2]).Take((int)arguments[3]).ToArray();
        var read = (SessionStateStoreData)typeof(SessionStateUtility).GetMethod("DeserializeStoreData", Static).Invoke(null, new object[] { null, new MemoryStream(stored), true });
        Assert.Equal((byte[])items["big"], (byte[])read.Items["big"]);
    }

    // ---- the legacy forms authentication ticket ----

    [Fact] // the legacy ticket's strings end at their first NUL (webengine4.dll was given C strings): a name with a NUL is cut there, and is no other field
    public void A_legacy_ticket_s_strings_end_at_a_NUL()
    {
        var type = typeof(HttpContext).Assembly.GetType("System.Web.Security.LegacyFormsAuthenticationTicket");
        var construct = type.GetMethod("Construct", Static);
        var parse = type.GetMethod("Parse", Static);
        // a name whose rest, written whole, was read as the dates, the persistence, the user data ("admin") and the path
        var forged = "user\0" + new string('䅁', 4) + new string('䅁', 4) + "\u0001" + new string('䅁', 4) + "admin\0/\0";
        var buffer = new byte[4096];
        int length = (int)construct.Invoke(null, new object[] { buffer, buffer.Length, forged, "role=user", "/app", new byte[] { 2, 0, 0, 0 }, new long[] { 1, 2 } });
        Assert.Equal(8 + 1 + 10 + 8 + 1 + 8 + 20 + 10, length);
        StringBuilder name = new(512), data = new(512), path = new(512);
        var bytes = new byte[4];
        var dates = new long[2];
        Assert.Equal(0, (int)parse.Invoke(null, new object[] { buffer, length, name, 512, data, 512, path, 512, bytes, dates }));
        Assert.Equal("user", name.ToString());
        Assert.Equal("role=user", data.ToString());
        Assert.Equal("/app", path.ToString());
        Assert.Equal(new long[] { 1, 2 }, dates);
        Assert.Equal(0, bytes[1]);
    }

    // ---- ResX, .resources ----

    [Fact] // a .resx of Visual Studio's or .NET Framework's writer: its null resource (ResXNullRef of System.Windows.Forms) read as null
    public void A_null_resource_of_NET_Framework_s_ResX_is_read()
    {
        var body = "<data name=\"n\" type=\"System.Resources.ResXNullRef, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"><value /></data>";
        var entry = Resources.Resx.Read(body).Single();
        Assert.Equal("n", entry.Key);
        Assert.Null(entry.Value);
        Assert.Null(Resources.Resx.Node(body).GetValue((System.ComponentModel.Design.ITypeResolutionService)null));
    }

    [Fact] // ResXResourceWriter: .NET Framework's type names (mscorlib, System.Drawing; ResXNullRef and its own of System.Windows.Forms), which it, Visual Studio and MSBuild read
    public void ResX_names_types_as_NET_Framework()
    {
        var text = Resources.Resx.Write(w => { w.AddResource("b", true); w.AddResource("c", Color.Red); w.AddResource("n", (object)null); w.AddResource("t", typeof(int)); });
        Assert.Contains("type=\"System.Boolean, mscorlib\"", text);
        Assert.Contains("name=\"mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"", text);
        Assert.Contains("System.Drawing, Version=4.0.0.0", text);
        Assert.Contains("type=\"System.Resources.ResXNullRef, System.Windows.Forms\"", text);
        Assert.Contains("name=\"System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"", text);
        Assert.True(text.Contains("System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0"), text);
        Assert.DoesNotContain("System.Private.CoreLib", text);
        var back = Resources.Resx.Entries(ResXResourceReader.FromFileContents(text).GetEnumerator()).ToDictionary(e => e.Key, e => e.Value);
        Assert.Equal(true, back["b"]);
        Assert.Equal(Color.Red, back["c"]);
        Assert.Null(back["n"]);
        Assert.Same(typeof(int), back["t"]);
    }

    [Fact] // App_GlobalResources: a resource of a type other than a string or a primitive (a Color, a Bitmap's...) written as .NET Framework wrote it and read by the ResourceManager (.NET's refuse it)
    public void Resources_of_serialized_objects_are_written_and_read()
    {
        var folder = Path.Combine(Path.GetTempPath(), "foc-resources-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        try
        {
            var web = typeof(HttpContext).Assembly;
            var values = new Hashtable { ["color"] = Color.FromArgb(1, 2, 3, 4), ["person"] = Values.PersonGraph(), ["text"] = "t", ["number"] = 3, ["culture"] = new CultureInfo("ja-JP") };
            using (var file = File.Create(Path.Combine(folder, "Strings.resources")))
                web.GetType("System.Web.Compilation.SerializedResources").GetMethod("Write", Static).Invoke(null, new object[] { file, values.Cast<DictionaryEntry>().OrderBy(e => (string)e.Key).ToArray() });
            var manager = ResourceManager.CreateFileBasedResourceManager("Strings", folder, web.GetType("System.Web.Compilation.SerializedResourceSet"));
            try
            {
                Assert.Equal(Color.FromArgb(1, 2, 3, 4), manager.GetObject("color", CultureInfo.InvariantCulture));
                Assert.Equal(Show.Of(values["person"]), Show.Of(manager.GetObject("person", CultureInfo.InvariantCulture)));
                Assert.Equal("t", manager.GetString("text", CultureInfo.InvariantCulture));
                Assert.Equal(3, manager.GetObject("number", CultureInfo.InvariantCulture));
                Assert.Equal("ja-JP", ((CultureInfo)manager.GetObject("culture", CultureInfo.InvariantCulture)).Name);
            }
            finally
            {
                manager.ReleaseAllResources();
            }
            // the strongly typed class's ResourceManager is given the set (generated code)
            var unit = new System.CodeDom.CodeCompileUnit();
            var ns = new System.CodeDom.CodeNamespace("Resources");
            var type = new System.CodeDom.CodeTypeDeclaration("Strings");
            var property = new System.CodeDom.CodeMemberProperty { Name = "ResourceManager" };
            property.GetStatements.Add(new System.CodeDom.CodeConditionStatement(new System.CodeDom.CodePrimitiveExpression(true),
                new System.CodeDom.CodeVariableDeclarationStatement(typeof(ResourceManager), "temp", new System.CodeDom.CodeObjectCreateExpression(typeof(ResourceManager),
                    new System.CodeDom.CodePrimitiveExpression("Resources.Strings"), new System.CodeDom.CodePrimitiveExpression(null)))));
            type.Members.Add(property);
            ns.Types.Add(type);
            unit.Namespaces.Add(ns);
            web.GetType("System.Web.Compilation.BaseResourcesBuildProvider").GetMethod("WithSerializedResourceSet", Static).Invoke(null, new object[] { unit });
            var create = (System.CodeDom.CodeObjectCreateExpression)((System.CodeDom.CodeVariableDeclarationStatement)((System.CodeDom.CodeConditionStatement)property.GetStatements[0]).TrueStatements[0]).InitExpression;
            var setType = (System.CodeDom.CodeMethodInvokeExpression)create.Parameters[2];
            Assert.Equal("System.Web.Compilation.SerializedResourceSet, System.Web", ((System.CodeDom.CodePrimitiveExpression)setType.Parameters[0]).Value);
            Assert.NotNull(Type.GetType("System.Web.Compilation.SerializedResourceSet, System.Web"));
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }

    // ---- what no golden file has ----

    static string GoldenPath([System.Runtime.CompilerServices.CallerFilePath] string file = null) => Path.Combine(Path.GetDirectoryName(file), "golden", "x");

    public static IEnumerable<object[]> Configurations => MachineKeyCases.Configurations.Select(c => new object[] { c[0] });

    [Theory] // a page's view state (ObjectStateFormatter of the page: its MAC, its encryption with ViewStateEncryptionMode="Always") read back; changed by a bit, refused
    [MemberData(nameof(Configurations))]
    public void A_page_s_view_state_round_trips_with_its_MAC_and_encryption(string configurationName)
    {
        var c = MachineKeyCases.Configurations.Single(x => x[0] == configurationName);
        using var keys = new MachineKeys(c[5], c[6], c[1], c[2], Enum.Parse<MachineKeyCompatibilityMode>(c[3]), c[4] == "true");
        var create = typeof(ObjectStateFormatter).GetConstructor(Instance, null, new[] { typeof(Page), typeof(bool) }, null);
        // not a configuration .NET Framework's MAC'd view state failed with (golden: a validationKey longer than the hash's
        // block, IndexOutOfRangeException in .NET Framework's MachineKeySection, as in the port's)
        if (GoldenFile.Read(Path.Combine(Path.GetDirectoryName(GoldenPath()), "MachineKey.txt.gz"))
            .Any(l => l[0] == configurationName && l[1] == "LosMac" && l[4] == "!" + typeof(IndexOutOfRangeException).FullName))
            return;
        foreach (var encryption in new[] { ViewStateEncryptionMode.Never, ViewStateEncryptionMode.Always })
        {
            var page = new Page { ViewStateEncryptionMode = encryption, ViewStateUserKey = "user ユ" };
            // what a postback of an encrypted view state sets (its __VIEWSTATEENCRYPTED field)
            typeof(Page).GetProperty("ContainsEncryptedViewState", Instance).SetValue(page, encryption == ViewStateEncryptionMode.Always);
            for (int i = 0; i < Values.Count; i += 9)
            {
                object value;
                try { value = Values.Make(i); }
                catch (System.ComponentModel.InvalidEnumArgumentException) { continue; }
                var formatter = (ObjectStateFormatter)create.Invoke(new object[] { page, true });
                string state;
                try { state = formatter.Serialize(value); }
                catch (ArgumentException) { continue; }   // what ObjectStateFormatter refuses (a lone surrogate...), as .NET Framework
                var back = ((ObjectStateFormatter)create.Invoke(new object[] { page, true })).Deserialize(state);
                Assert.True(Show.Of(value) == Show.Of(back), $"{configurationName} {encryption} #{i}: {Show.Of(value)} / {Show.Of(back)}");
                var bytes = Convert.FromBase64String(state);
                bytes[bytes.Length / 2] ^= 0x10;
                var tampered = Convert.ToBase64String(bytes);
                Assert.ThrowsAny<Exception>(() => ((ObjectStateFormatter)create.Invoke(new object[] { page, true })).Deserialize(tampered));
            }
        }
    }

    [Fact] // DataProtector (<machineKey dataProtectorType>): generated data and purposes protected, unprotected; another purpose's refused.
    // On Linux (Mono's ManagedProtection) with its own key pair, made in the user's .config (here a folder of the test's, the
    // key read again from it): .NET has no key containers there (PlatformNotSupportedException before)
    public void DataProtector_round_trips_generated_data()
    {
        var keys = typeof(Mono.Security.Cryptography.ManagedProtection);
        var config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        var folder = Path.Combine(Path.GetTempPath(), "foc-dpapi-" + Guid.NewGuid().ToString("N"));
        void ForgetKeys() { keys.GetField("user", Static).SetValue(null, null); keys.GetField("machine", Static).SetValue(null, null); }
        if (!OperatingSystem.IsWindows())
        {
            Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", folder);
            ForgetKeys();
        }
        try
        {
            for (int i = 0; i < 40; i++)
            {
                var r = new Fuzz.Rng(0x44505254UL + (ulong)i * 0x9E3779B97F4A7C15UL);
                var data = MachineKeyCases.Data(r);
                var purposes = MachineKeyCases.Purposes(r).Where(p => p.Trim().Length > 0).ToArray();
                var protector = DataProtector.Create("DpapiDataProtector", "App " + i, "Primary", purposes);
                var protectedData = protector.Protect(data);
                if (i == 20 && !OperatingSystem.IsWindows()) ForgetKeys();   // the key pair read again from its file
                Assert.Equal(data, DataProtector.Create("DpapiDataProtector", "App " + i, "Primary", purposes).Unprotect(protectedData));
                Assert.ThrowsAny<Exception>(() => DataProtector.Create("DpapiDataProtector", "App " + i, "Other", purposes).Unprotect(protectedData));
            }
            if (!OperatingSystem.IsWindows())
            {
                var file = Path.Combine(folder, ".mono", "keypairs", "[1][DAPI][1].xml");
                Assert.True(File.Exists(file));
                Assert.Equal(UnixFileMode.UserRead | UnixFileMode.UserWrite, File.GetUnixFileMode(file));
            }
        }
        finally
        {
            if (!OperatingSystem.IsWindows())
            {
                Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", config);
                ForgetKeys();
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }
    }
}
