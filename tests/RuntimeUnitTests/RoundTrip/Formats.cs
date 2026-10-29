// The round-trip tests' formats: how a value is written (to bytes) and read back, the same calls on .NET Framework 4.8
// (golden/Recorder.csproj: its System.Web, System.Windows.Forms' ResX, its BinaryFormatter) and on the port (RoundTripTests:
// FrameworkOnCore's). System.Web's internals are reached by reflection, by the names both have.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
#if NETFRAMEWORK
using System.Runtime.Serialization.Formatters.Binary;
#else
using WebFormsForCore.Serialization.Formatters.Binary;
#endif

namespace FrameworkOnCore.Runtime.Tests.RoundTrip
{
    /// <summary>A way a value is kept: written to bytes, read back from them.</summary>
    public sealed class Format
    {
        public string Name;
        /// <summary>What is written for a case (its index, its value): the value itself but for the controls' tree.</summary>
        public Func<int, object, object> Prepare = (index, value) => value;
        public Func<object, byte[]> Write;
        public Func<byte[], object> Read;
    }

    public static class Formats
    {
        const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        static readonly Assembly web = typeof(HttpContext).Assembly;

        /// <summary>The formats without a machine key.</summary>
        public static readonly Format[] All =
        {
            new Format { Name = "ObjectStateFormatter", Write = OsfWrite, Read = OsfRead },
            new Format { Name = "LosFormatter", Write = v => { var w = new StringWriter(); new LosFormatter().Serialize(w, v); return Encoding.UTF8.GetBytes(w.ToString()); },
                         Read = b => new LosFormatter().Deserialize(Encoding.UTF8.GetString(b)) },
            new Format { Name = "SessionItems", Write = SessionItemsWrite, Read = SessionItemsRead },
            new Format { Name = "SessionStore", Write = v => SessionStoreWrite(v, false), Read = b => SessionStoreRead(b, false) },
            new Format { Name = "SessionStoreDeflate", Write = v => SessionStoreWrite(v, true), Read = b => SessionStoreRead(b, true) },
            new Format { Name = "StaticObjects", Write = StaticObjectsWrite, Read = StaticObjectsRead },
            new Format { Name = "BinaryFormatter", Write = v => { var s = new MemoryStream(); new BinaryFormatter().Serialize(s, v); return s.ToArray(); },
                         Read = b => new BinaryFormatter().Deserialize(new MemoryStream(b)) },
            new Format { Name = "ResX", Write = v => ResXWrite(v, false), Read = b => ResXRead(b, false) },
            new Format { Name = "ResXDataNodes", Write = v => ResXWrite(v, true), Read = b => ResXRead(b, true) },
            new Format { Name = "Resources", Write = ResourcesWrite, Read = ResourcesRead },
            new Format { Name = "ViewStateControls", Prepare = (i, v) => ControlTree.Build(i, v, true), Write = root => OsfWrite(ControlTree.Save((Control)root)),
                         Read = b => ControlTree.Load(OsfRead(b)) },
            new Format { Name = "ProfileString", Write = v => ProfileWrite(v, "String"), Read = b => ProfileRead(b, "String") },
            new Format { Name = "ProfileXml", Write = v => ProfileWrite(v, "Xml"), Read = b => ProfileRead(b, "Xml") },
            new Format { Name = "ProfileBinary", Write = v => ProfileWrite(v, "Binary"), Read = b => ProfileRead(b, "Binary") },
        };

        static byte[] OsfWrite(object v)
        {
            var s = new MemoryStream();
            new ObjectStateFormatter().Serialize(s, v);
            return s.ToArray();
        }

        static object OsfRead(byte[] b) => new ObjectStateFormatter().Deserialize(new MemoryStream(b));

        // ---- session state ----

        // Items of other names around the value's (the collection's table of offsets), a name beyond ASCII.
        static byte[] SessionItemsWrite(object v)
        {
            var items = new SessionStateItemCollection();
            items["first"] = "x";
            items["値"] = v;
            items["last"] = 2;
            var s = new MemoryStream();
            var writer = Writer(s);
            items.Serialize(writer);
            writer.Flush();
            return s.ToArray();
        }

        // The writer SessionStateUtility gives them (.NET Framework's default: UTF-8 that refuses a lone surrogate; .NET's
        // default BinaryWriter replaces it)
        static BinaryWriter Writer(Stream s) => new BinaryWriter(s, new UTF8Encoding(false, true));

        static object SessionItemsRead(byte[] b)
        {
            var items = SessionStateItemCollection.Deserialize(new BinaryReader(new MemoryStream(b)));
            if (items.Count != 3 || (string)items["first"] != "x" || !(items["last"] is int i && i == 2)) throw new InvalidDataException("the other items");
            return items["値"];
        }

        internal static HttpStaticObjectsCollection StaticObjects(string name, object instance)
        {
            var objects = new HttpStaticObjectsCollection();
            var entryType = web.GetType("System.Web.HttpStaticObjectsEntry");
            var entry = entryType.GetConstructor(Instance, null, new[] { typeof(string), typeof(object), typeof(int) }, null).Invoke(new[] { name, instance, (object)0 });
            ((Hashtable)typeof(HttpStaticObjectsCollection).GetField("_objects", Instance).GetValue(objects)).Add(name, entry);
            return objects;
        }

        // SessionStateUtility's format (StateServer's and SQL Server's): the timeout, the items, the static objects (an instance),
        // compressed or not (<sessionState compressionEnabled>).
        static byte[] SessionStoreWrite(object v, bool compressed)
        {
            var items = new SessionStateItemCollection();
            items["v"] = v;
            var data = new SessionStateStoreData(items, StaticObjects("s", "static"), 20);
            var arguments = new object[] { data, 7000, null, 0, compressed };
            typeof(SessionStateUtility).GetMethod("SerializeStoreData", Static).Invoke(null, arguments);
            return ((byte[])arguments[2]).Take((int)arguments[3]).ToArray();
        }

        static object SessionStoreRead(byte[] b, bool compressed)
        {
            var data = (SessionStateStoreData)Invoke(typeof(SessionStateUtility).GetMethod("DeserializeStoreData", Static), null, new object[] { null, new MemoryStream(b), compressed });
            if (data.Timeout != 20 || data.Items.Count != 1 || (string)data.StaticObjects["s"] != "static") throw new InvalidDataException("the store data");
            return data.Items["v"];
        }

        static byte[] StaticObjectsWrite(object v)
        {
            var s = new MemoryStream();
            var writer = Writer(s);
            StaticObjects("v", v).Serialize(writer);
            writer.Flush();
            return s.ToArray();
        }

        static object StaticObjectsRead(byte[] b) => HttpStaticObjectsCollection.Deserialize(new BinaryReader(new MemoryStream(b)))["v"];

        // ---- ResX, .resources ----

        static byte[] ResXWrite(object v, bool node)
        {
            var s = new MemoryStream();
            using (var writer = new ResXResourceWriter(s))
            {
                if (node) writer.AddResource(new ResXDataNode("v", v) { Comment = "comment ユ" });
                else writer.AddResource("v", v);
                writer.Generate();
                return s.ToArray();
            }
        }

        static object ResXRead(byte[] b, bool node)
        {
            using (var reader = new ResXResourceReader(new MemoryStream(b)))
            {
                reader.UseResXDataNodes = node;
                var e = reader.GetEnumerator();
                if (!e.MoveNext() || (string)e.Key != "v") throw new InvalidDataException("no resource");
                var value = e.Value;
                if (e.MoveNext()) throw new InvalidDataException("more than one resource");
                if (!node) return value;
                var dataNode = (ResXDataNode)value;
                if (dataNode.Name != "v" || dataNode.Comment != "comment ユ") throw new InvalidDataException("the node's name or comment");
                return dataNode.GetValue((System.ComponentModel.Design.ITypeResolutionService)null);
            }
        }

        // What App_GlobalResources' compilation writes (BaseResourcesBuildProvider: a ResourceWriter; the port's objects as
        // .NET Framework wrote them, SerializedResources) and its ResourceManager reads (a ResourceSet of the .resources
        // stream: the port's SerializedResourceSet).
        static byte[] ResourcesWrite(object v)
        {
            var s = new MemoryStream();
#if NETFRAMEWORK
            using (var writer = new ResourceWriter(s))
            {
                writer.AddResource("v", v);
                writer.Generate();
            }
#else
            Invoke(web.GetType("System.Web.Compilation.SerializedResources").GetMethod("Write", Static), null, new object[] { s, new[] { new DictionaryEntry("v", v) } });
#endif
            return s.ToArray();
        }

        static object ResourcesRead(byte[] b)
        {
#if NETFRAMEWORK
            using (var set = new ResourceSet(new MemoryStream(b)))
#else
            using (var set = (ResourceSet)Activator.CreateInstance(web.GetType("System.Web.Compilation.SerializedResourceSet"), new object[] { new MemoryStream(b) }))
#endif
                return set.GetObject("v");
        }

        // ---- profiles (SqlProfileProvider's columns: names, the string values, the binary ones) ----

        // System.Configuration's settings types as System.Web has them (the port's own System.Configuration.ConfigurationManager,
        // not .NET's of the same name: by reflection).
        static readonly Assembly settings = web.GetType("System.Web.Profile.ProfileBase").BaseType.Assembly;

        static Type Settings(string name) => settings.GetType("System.Configuration." + name, true);

        static object Profile(Type type, string serializeAs)
        {
            var property = Activator.CreateInstance(Settings("SettingsProperty"), "v");
            Settings("SettingsProperty").GetProperty("PropertyType").SetValue(property, type, null);
            Settings("SettingsProperty").GetProperty("SerializeAs").SetValue(property, Enum.Parse(Settings("SettingsSerializeAs"), serializeAs), null);
            ((Hashtable)Settings("SettingsProperty").GetProperty("Attributes").GetValue(property, null)).Add("AllowAnonymous", true);
            var values = Activator.CreateInstance(Settings("SettingsPropertyValueCollection"));
            Settings("SettingsPropertyValueCollection").GetMethod("Add").Invoke(values, new[] { Activator.CreateInstance(Settings("SettingsPropertyValue"), property) });
            return values;
        }

        static PropertyInfo PropertyValue => Settings("SettingsPropertyValue").GetProperty("PropertyValue");

        static object Value(object values) => Settings("SettingsPropertyValueCollection").GetProperty("Item", new[] { typeof(string) }).GetValue(values, new object[] { "v" });

        static byte[] ProfileWrite(object v, string serializeAs)
        {
            var values = Profile(v == null ? typeof(object) : v.GetType(), serializeAs);
            PropertyValue.SetValue(Value(values), v, null);
            var arguments = new object[] { null, null, null, true, values, true };
            Invoke(web.GetType("System.Web.Profile.ProfileModule").GetMethod("PrepareDataForSaving", Static), null, arguments);
            // the type, then the three columns
            var s = new MemoryStream();
            var writer = new BinaryWriter(s);
            // the strings as UTF-16 (BinaryWriter's UTF-8 refuses a lone surrogate on .NET Framework)
            writer.Write(v == null ? "" : Show.TypeName(v.GetType()));
            writer.Write(Convert.ToBase64String(Encoding.Unicode.GetBytes((string)arguments[0])));
            writer.Write(Convert.ToBase64String(Encoding.Unicode.GetBytes((string)arguments[1])));
            var buf = (byte[])arguments[2] ?? new byte[0];
            writer.Write(buf.Length);
            writer.Write(buf);
            writer.Flush();
            return s.ToArray();
        }

        static object ProfileRead(byte[] b, string serializeAs)
        {
            var reader = new BinaryReader(new MemoryStream(b));
            var typeName = reader.ReadString();
            var type = typeName.Length == 0 ? typeof(object) : Named(typeName);
            var names = Encoding.Unicode.GetString(Convert.FromBase64String(reader.ReadString()));
            var values = Encoding.Unicode.GetString(Convert.FromBase64String(reader.ReadString()));
            var buf = reader.ReadBytes(reader.ReadInt32());
            var properties = Profile(type, serializeAs);
            Invoke(web.GetType("System.Web.Profile.ProfileModule").GetMethod("ParseDataFromDB", Static), null, new object[] { names.Split(':'), values, buf, properties });
            return Invoke(PropertyValue.GetGetMethod(), Value(properties), null);
        }

        /// <summary>A type by Show.TypeName's name (the values' types: the runtime's, System.Web's, System.Drawing's, the tests').</summary>
        public static Type Named(string name)
        {
            var assemblies = new[] { typeof(object).Assembly, typeof(Uri).Assembly, web, typeof(System.Drawing.Color).Assembly, typeof(System.Drawing.Point).Assembly,
                typeof(Values).Assembly, typeof(System.Collections.Specialized.NameValueCollection).Assembly, typeof(System.Data.DataTable).Assembly, typeof(List<>).Assembly };
            if (name.EndsWith("]", StringComparison.Ordinal))
            {
                if (name.EndsWith("[]", StringComparison.Ordinal) || name.EndsWith(",]", StringComparison.Ordinal))
                {
                    int open = name.LastIndexOf('[');
                    var element = Named(name.Substring(0, open));
                    int rank = name.Length - open - 1;
                    return rank == 1 ? element.MakeArrayType() : element.MakeArrayType(rank);
                }
                int at = name.IndexOf('[');
                var definition = Named(name.Substring(0, at));
                var arguments = Split(name.Substring(at + 1, name.Length - at - 2)).Select(Named).ToArray();
                return definition.MakeGenericType(arguments);
            }
            foreach (var assembly in assemblies.Distinct())
            {
                var type = assembly.GetType(name);
                if (type != null) return type;
            }
            return Type.GetType(name, true);
        }

        static IEnumerable<string> Split(string arguments)
        {
            int depth = 0, start = 0;
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == '[') depth++;
                else if (arguments[i] == ']') depth--;
                else if (arguments[i] == ',' && depth == 0) { yield return arguments.Substring(start, i - start); start = i + 1; }
            }
            yield return arguments.Substring(start);
        }

        public static object Invoke(MethodInfo method, object target, object[] arguments)
        {
            try { return method.Invoke(target, arguments); }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                throw;
            }
        }

        /// <summary>An exception as the golden files have it: its type (the innermost's of TargetInvocationExceptions).</summary>
        public static string Error(Exception e)
        {
            while (e is TargetInvocationException && e.InnerException != null) e = e.InnerException;
            // ROUNDTRIP_STACK (triage): the message and where it was thrown too
            if (Environment.GetEnvironmentVariable("ROUNDTRIP_STACK") != null)
            {
                var text = "";
                for (var x = e; x != null; x = x.InnerException) text += " >> " + x.GetType().FullName + ": " + x.Message + " @ " + (x.StackTrace ?? "").Replace("\r", "").Replace("\n", " | ");
                return "!" + e.GetType().FullName + text;
            }
            return "!" + e.GetType().FullName;
        }

        /// <summary>A value shown, or the exception showing it threw.</summary>
        public static string Shown(object value)
        {
            try { return Show.Of(value); }
            catch (Exception e) { return "!Show " + e.Message; }
        }
    }

    /// <summary>
    /// The golden files (golden/*.txt.gz): one line a case, its fields tab-separated. A format's: index, the input's hash,
    /// what was written (base64; "!" and an exception's type if writing threw), what was read back (shown; "=" if the input;
    /// "!" and an exception's type), whether writing again gave the same bytes ("same", "differs").
    /// </summary>
    public static class GoldenFile
    {
        public static void Write(string path, IEnumerable<string[]> lines)
        {
            using (var stream = new System.IO.Compression.GZipStream(File.Create(path), System.IO.Compression.CompressionLevel.Optimal))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)) { NewLine = "\n" })
                foreach (var line in lines) writer.WriteLine(string.Join("\t", line));
        }

        public static List<string[]> Read(string path)
        {
            var lines = new List<string[]>();
            using (var reader = new StreamReader(new System.IO.Compression.GZipStream(File.OpenRead(path), System.IO.Compression.CompressionMode.Decompress), Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null) lines.Add(line.Split('\t'));
            }
            return lines;
        }

        /// <summary>A value's show as the golden files' hash has it (local times' ticks shown: the same in every zone).</summary>
        public static string Hash(string shown)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(shown));
                return BitConverter.ToString(hash, 0, 8).Replace("-", "");
            }
        }

        public static string WithLocalTicks(Func<string> show)
        {
            bool previous = Show.LocalTicks;
            Show.LocalTicks = true;
            try { return show(); }
            finally { Show.LocalTicks = previous; }
        }

        /// <summary>A format's case: the value written, read back; its line.</summary>
        public static string[] Case(Format format, int index, Func<object> make, bool again = true)
        {
            var input = format.Prepare(index, make());
            var shown = WithLocalTicks(() => Formats.Shown(input));
            string written, read;
            byte[] bytes = null;
            try { bytes = format.Write(input); written = Convert.ToBase64String(bytes); }
            catch (Exception e) { written = Formats.Error(e); }
            if (bytes == null) read = "";
            else
            {
                try
                {
                    var value = format.Read(bytes);
                    read = WithLocalTicks(() => Formats.Shown(value));
                    if (read == shown) read = "=";
                }
                catch (Exception e) { read = Formats.Error(e); }
            }
            string same = "";
            if (again && bytes != null)
            {
                try { same = Convert.ToBase64String(format.Write(format.Prepare(index, make()))) == written ? "same" : "differs"; }
                catch (Exception e) { same = Formats.Error(e); }
            }
            return new[] { index.ToString(CultureInfo.InvariantCulture), Hash(shown), written, read, same };
        }
    }
}
