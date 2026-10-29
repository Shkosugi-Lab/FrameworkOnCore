// WebFormsForCore: the types .NET Framework serialized that .NET does not (they are not [Serializable] any more, or
// refuse their serialization constructor): a Type (UnitySerializationHolder), CultureInfo, NameValueCollection and what
// their graphs hold (TextInfo, CaseInsensitiveComparer, CaseInsensitiveHashCodeProvider). What .NET Framework wrote of
// them (a migrated site's session state, view state, resources) is read, and they are written as .NET Framework wrote
// them (its type names and members), which it reads. Found by the round-trip tests (tests/RuntimeUnitTests/RoundTrip).

using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace WebFormsForCore.Serialization.Formatters.Binary
{
    /// <summary>A surrogate whose SetObjectData returns an object of its own, made whole (not the one it was given).</summary>
    internal interface IReplacingSurrogate : ISerializationSurrogate
    {
    }

    /// <summary>A surrogate that completes its object once the whole graph is (after the fixups: OnDeserialization's time).</summary>
    internal interface IDeferredSurrogate : ISerializationSurrogate
    {
        void Complete(object obj, SerializationInfo info);
    }

    /// <summary>The surrogates of .NET Framework's serializable types that .NET's are not; after the application's own selector.</summary>
    internal sealed class FrameworkSurrogates : ISurrogateSelector
    {
        private const string Mscorlib = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        private const string SystemAssembly = "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        private static readonly Type s_runtimeType = typeof(Type).GetType();
        private static readonly Type? s_unityHolder = typeof(object).Assembly.GetType("System.UnitySerializationHolder");

        private ISurrogateSelector? _next;

        /// <summary>The selector a formatter uses: the application's (BinaryFormatter.SurrogateSelector) first, then these.</summary>
        internal static ISurrogateSelector With(ISurrogateSelector? selector) => new FrameworkSurrogates { _next = selector };

        public void ChainSelector(ISurrogateSelector selector) => _next = selector;

        public ISurrogateSelector? GetNextSelector() => _next;

        public ISerializationSurrogate? GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if (_next != null)
            {
                var surrogate = _next.GetSurrogate(type, context, out selector);
                if (surrogate != null) return surrogate;
            }
            selector = this;
            return For(type);
        }

        internal static ISerializationSurrogate? For(Type type)
        {
            if (type == s_runtimeType || (s_unityHolder != null && type == s_unityHolder)) return TypeSurrogate.Instance;
            if (type == typeof(CultureInfo)) return CultureInfoSurrogate.Instance;
            if (type == typeof(NameValueCollection)) return NameValueCollectionSurrogate.Instance;
            if (type == typeof(TextInfo)) return TextInfoSurrogate.Instance;
            if (type == typeof(CaseInsensitiveComparer) || type == typeof(CaseInsensitiveHashCodeProvider)) return CaseInsensitiveSurrogate.Instance;
            if ((type == typeof(NumberFormatInfo) || type == typeof(DateTimeFormatInfo) || typeof(Calendar).IsAssignableFrom(type)) && !type.IsSerializable)
                return FieldsSurrogate.Instance;
            return null;
        }

        private static T? Get<T>(SerializationInfo info, string name, T? fallback = default)
        {
            foreach (SerializationEntry entry in info)
                if (entry.Name == name)
                    return entry.Value is T value ? value : fallback;
            return fallback;
        }

        // ---- Type: .NET Framework's UnitySerializationHolder (RuntimeTypeUnity: the type's full name and assembly) ----

        private sealed class TypeSurrogate : IReplacingSurrogate
        {
            internal static readonly TypeSurrogate Instance = new TypeSurrogate();

            private const int RuntimeTypeUnity = 4, ModuleUnity = 5, AssemblyUnity = 6;

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                var type = (Type)obj;
                var root = type;
                while (root.HasElementType) root = root.GetElementType()!;
                if (root.IsGenericParameter || (!type.IsGenericTypeDefinition && type.ContainsGenericParameters))
                    throw new SerializationException(SR.Format(SR.Serialization_NonSerType, type.FullName, type.Assembly.FullName));
                info.FullTypeName = "System.UnitySerializationHolder";
                info.AssemblyName = Mscorlib;
                info.AddValue("Data", type.IsGenericTypeDefinition ? type.FullName : FormatterServices.GetClrTypeFullName(type), typeof(string));
                info.AddValue("UnityType", RuntimeTypeUnity);
                info.AddValue("AssemblyName", FormatterServices.GetClrAssemblyName(type, out _));
            }

            [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2057", Justification = "The serialized type's name.")]
            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector? selector)
            {
                int unity = Get(info, "UnityType", 0);
                string? data = Get<string>(info, "Data");
                string? assembly = Get<string>(info, "AssemblyName");
                switch (unity)
                {
                    case RuntimeTypeUnity:
                        if (data == null) break;
                        var type = Type.GetType(string.IsNullOrEmpty(assembly) ? data : data + ", " + assembly, ObjectReader.ResolveSimpleAssemblyName,
                            (a, name, ignoreCase) => a != null ? a.GetType(name, false, ignoreCase) : Type.GetType(name, false, ignoreCase), throwOnError: false);
                        return type ?? throw new SerializationException("The type '" + data + ", " + assembly + "' was not found.");
                    case AssemblyUnity:
                        if (data == null) break;
                        return ObjectReader.ResolveSimpleAssemblyName(new AssemblyName(data)) ?? throw new SerializationException(data);
                    case ModuleUnity:
                        if (data == null || assembly == null) break;
                        return ObjectReader.ResolveSimpleAssemblyName(new AssemblyName(assembly))?.GetModule(data) ?? throw new SerializationException(data);
                    default:
                        // DBNull, Missing...: .NET's own holder
                        if (s_unityHolder != null)
                            return ((IObjectReference)Activator.CreateInstance(s_unityHolder, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                                new object[] { info, context }, null)!).GetRealObject(context);
                        break;
                }
                throw new SerializationException(SR.Serialization_InsufficientState);
            }
        }

        // ---- CultureInfo: .NET Framework's fields; its name and whether the user's overrides apply make it again ----

        private sealed class CultureInfoSurrogate : IReplacingSurrogate
        {
            internal static readonly CultureInfoSurrogate Instance = new CultureInfoSurrogate();

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                var culture = (CultureInfo)obj;
                info.FullTypeName = "System.Globalization.CultureInfo";
                info.AssemblyName = Mscorlib;
                // .NET Framework's fields (its CompareInfo, TextInfo... made again from the name when it reads them)
                info.AddValue("m_isReadOnly", culture.IsReadOnly);
                // (null, of the type object: .NET's NumberFormatInfo, DateTimeFormatInfo, Calendar are not serializable)
                info.AddValue("compareInfo", null, typeof(object));
                info.AddValue("textInfo", null, typeof(object));
                info.AddValue("numInfo", null, typeof(object));
                info.AddValue("dateTimeInfo", null, typeof(object));
                info.AddValue("calendar", null, typeof(object));
                info.AddValue("m_dataItem", 0);
                info.AddValue("cultureID", culture.LCID);
                info.AddValue("m_name", culture.Name, typeof(string));
                info.AddValue("m_useUserOverride", culture.UseUserOverride);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector? selector)
            {
                string? name = Get<string>(info, "m_name");
                bool useUserOverride = Get(info, "m_useUserOverride", true);
                var culture = name != null ? new CultureInfo(name, useUserOverride) : new CultureInfo(Get(info, "cultureID", 0x7F), useUserOverride);
                return Get(info, "m_isReadOnly", false) ? CultureInfo.ReadOnly(culture) : culture;
            }
        }

        // ---- NumberFormatInfo, DateTimeFormatInfo, a Calendar (in a CultureInfo .NET Framework wrote, if it had made them):
        // read, their fields of the same names (.NET's: '_' and the name) copied; not written (not serializable, as before) ----

        private sealed class FieldsSurrogate : IReplacingSurrogate, IDeferredSurrogate
        {
            internal static readonly FieldsSurrogate Instance = new FieldsSurrogate();

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) =>
                throw new SerializationException(SR.Format(SR.Serialization_NonSerType, obj.GetType().FullName, obj.GetType().Assembly.FullName));

            [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2072", Justification = "The types of FrameworkSurrogates.For.")]
            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector? selector) =>
                Activator.CreateInstance(obj.GetType())!;

            [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075", Justification = "The types of FrameworkSurrogates.For.")]
            public void Complete(object obj, SerializationInfo info)
            {
                foreach (SerializationEntry entry in info)
                {
                    if (entry.Value == null || entry.Name == "isReadOnly" || entry.Name == "_isReadOnly") continue;
                    for (var type = obj.GetType(); type != null && type != typeof(object); type = type.BaseType)
                    {
                        var field = type.GetField("_" + entry.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                            ?? type.GetField(entry.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                        if (field == null || field.IsInitOnly || !field.FieldType.IsInstanceOfType(entry.Value)) continue;
                        try { field.SetValue(obj, entry.Value); } catch (ArgumentException) { }
                        break;
                    }
                }
            }
        }

        // ---- TextInfo, CaseInsensitiveComparer, CaseInsensitiveHashCodeProvider (a NameValueCollection's comparer) ----

        private sealed class TextInfoSurrogate : IReplacingSurrogate
        {
            internal static readonly TextInfoSurrogate Instance = new TextInfoSurrogate();

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                var text = (TextInfo)obj;
                var culture = CultureInfo.GetCultureInfo(text.CultureName);
                info.FullTypeName = "System.Globalization.TextInfo";
                info.AssemblyName = Mscorlib;
                info.AddValue("m_listSeparator", null, typeof(string));
                info.AddValue("m_isReadOnly", text.IsReadOnly);
                info.AddValue("m_cultureName", text.CultureName, typeof(string));
                info.AddValue("customCultureName", null, typeof(string));
                info.AddValue("m_nDataItem", 0);
                info.AddValue("m_useUserOverride", false);
                info.AddValue("m_win32LangID", culture.LCID);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector? selector) =>
                Culture(Get<string>(info, "m_cultureName") ?? Get<string>(info, "customCultureName")).TextInfo;
        }

        private static CultureInfo Culture(string? name)
        {
            try { return string.IsNullOrEmpty(name) ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(name); }
            catch (CultureNotFoundException) { return CultureInfo.InvariantCulture; }
        }

        private sealed class CaseInsensitiveSurrogate : IReplacingSurrogate
        {
            internal static readonly CaseInsensitiveSurrogate Instance = new CaseInsensitiveSurrogate();

            private static readonly FieldInfo? s_comparerInfo = typeof(CaseInsensitiveComparer).GetField("_compareInfo", BindingFlags.Instance | BindingFlags.NonPublic);
            private static readonly FieldInfo? s_providerText = typeof(CaseInsensitiveHashCodeProvider).GetField("_compareInfo", BindingFlags.Instance | BindingFlags.NonPublic)
                ?? typeof(CaseInsensitiveHashCodeProvider).GetField("_text", BindingFlags.Instance | BindingFlags.NonPublic);

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                info.AssemblyName = Mscorlib;
                if (obj is CaseInsensitiveComparer)
                {
                    info.FullTypeName = "System.Collections.CaseInsensitiveComparer";
                    info.AddValue("m_compareInfo", s_comparerInfo?.GetValue(obj) as CompareInfo ?? CultureInfo.InvariantCulture.CompareInfo, typeof(CompareInfo));
                }
                else
                {
                    info.FullTypeName = "System.Collections.CaseInsensitiveHashCodeProvider";
                    var name = s_providerText?.GetValue(obj) switch { CompareInfo c => c.Name, TextInfo t => t.CultureName, _ => "" };
                    info.AddValue("m_text", Culture(name).TextInfo, typeof(TextInfo));
                }
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector? selector)
            {
                if (obj is CaseInsensitiveComparer)
                {
                    var compareInfo = Get<CompareInfo>(info, "m_compareInfo");
                    return compareInfo == null || compareInfo.Name.Length == 0 ? CaseInsensitiveComparer.DefaultInvariant : new CaseInsensitiveComparer(Culture(compareInfo.Name));
                }
                var text = Get<TextInfo>(info, "m_text");
                return text == null || text.CultureName.Length == 0 ? CaseInsensitiveHashCodeProvider.DefaultInvariant : new CaseInsensitiveHashCodeProvider(Culture(text.CultureName));
            }
        }

        // ---- NameValueCollection: NameObjectCollectionBase's members (its keys, its values: a list of strings a key) ----

        private sealed class NameValueCollectionSurrogate : IReplacingSurrogate, IDeferredSurrogate
        {
            internal static readonly NameValueCollectionSurrogate Instance = new NameValueCollectionSurrogate();

            private static readonly FieldInfo? s_keyComparer = typeof(NameObjectCollectionBase).GetField("_keyComparer", BindingFlags.Instance | BindingFlags.NonPublic);
            private static readonly FieldInfo? s_version = typeof(NameObjectCollectionBase).GetField("_version", BindingFlags.Instance | BindingFlags.NonPublic);
            private static readonly PropertyInfo s_readOnly = typeof(NameObjectCollectionBase).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic)!;

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                var collection = (NameValueCollection)obj;
                info.FullTypeName = "System.Collections.Specialized.NameValueCollection";
                info.AssemblyName = SystemAssembly;
                info.AddValue("ReadOnly", (bool)s_readOnly.GetValue(collection)!);
                var comparer = s_keyComparer?.GetValue(collection);
                var hashProvider = comparer?.GetType().GetField("_hcp", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(comparer) as IHashCodeProvider;
                var keyComparer = comparer?.GetType().GetField("_comparer", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(comparer) as IComparer;
                // (.NET's default comparer: StringComparer.InvariantCultureIgnoreCase; .NET Framework's: CaseInsensitive's of the
                // invariant culture, a hash code provider and a comparer)
                if (comparer == null || hashProvider != null || keyComparer != null || comparer.GetType().Name == "CompatibleComparer" ||
                    comparer.Equals(StringComparer.InvariantCultureIgnoreCase))
                {
                    // the default (CaseInsensitive's of the invariant culture), as .NET Framework writes it
                    info.AddValue("HashProvider", comparer == null ? null : hashProvider ?? CaseInsensitiveHashCodeProvider.DefaultInvariant, typeof(IHashCodeProvider));
                    info.AddValue("Comparer", comparer == null ? null : keyComparer ?? CaseInsensitiveComparer.DefaultInvariant, typeof(IComparer));
                }
                else
                {
                    info.AddValue("KeyComparer", comparer, typeof(IEqualityComparer));
                }
                int count = collection.Count;
                var keys = new string?[count];
                var values = new object?[count];
                for (int i = 0; i < count; i++)
                {
                    keys[i] = collection.GetKey(i);
                    var items = collection.GetValues(i);
                    values[i] = items == null ? null : new ArrayList(items);
                }
                info.AddValue("Count", count);
                info.AddValue("Keys", keys, typeof(string[]));
                info.AddValue("Values", values, typeof(object[]));
                info.AddValue("Version", s_version?.GetValue(collection) is int version ? version : 0);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector? selector)
            {
                NameValueCollection collection;
                var keyComparer = Get<IEqualityComparer>(info, "KeyComparer");
                bool hasProvider = false;
                foreach (SerializationEntry entry in info) hasProvider |= entry.Name == "HashProvider";
                if (keyComparer != null) collection = new NameValueCollection(keyComparer);
                else if (hasProvider)
                {
                    var provider = Get<IHashCodeProvider>(info, "HashProvider");
                    var comparer = Get<IComparer>(info, "Comparer");
                    if (provider == null && comparer == null) collection = new NameValueCollection(StringComparer.Ordinal);
                    else if ((provider == null || provider is CaseInsensitiveHashCodeProvider) && (comparer == null || comparer is CaseInsensitiveComparer)) collection = new NameValueCollection();
#pragma warning disable CS0618 // the obsolete constructor: the comparer .NET Framework kept
                    else collection = new NameValueCollection(provider, comparer);
#pragma warning restore CS0618
                }
                else collection = new NameValueCollection();
                return collection;
            }

            // the keys and values (arrays and lists the graph's fixups complete), the read-only flag
            public void Complete(object obj, SerializationInfo info)
            {
                var collection = (NameValueCollection)obj;
                var keys = Get<string?[]>(info, "Keys") ?? Array.Empty<string?>();
                var values = Get<object?[]>(info, "Values") ?? Array.Empty<object?>();
                for (int i = 0; i < keys.Length; i++)
                {
                    var items = i < values.Length ? values[i] as ArrayList : null;
                    if (items == null || items.Count == 0) collection.Add(keys[i], null);
                    else foreach (var item in items) collection.Add(keys[i], (string?)item);
                }
                if (Get(info, "ReadOnly", false)) s_readOnly.SetValue(collection, true);
            }
        }
    }
}
