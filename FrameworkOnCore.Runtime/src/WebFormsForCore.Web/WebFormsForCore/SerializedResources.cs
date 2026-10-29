// WebFormsForCore: an application's resources (App_GlobalResources, App_LocalResources: .resx and .resources files) that
// hold objects other than strings and primitives (a Color, a Bitmap, an application's [Serializable] type...). .NET Framework's
// ResourceWriter wrote them with BinaryFormatter and its ResourceReader read them; .NET's refuse them (ResourceWriter:
// PlatformNotSupportedException, ResourceReader: NotSupportedException "Cannot read resources that depend on
// serialization"), which failed the compilation of such a folder. Here they are written in .NET Framework's format (the
// type's .NET Framework name, BinaryFormatter's bytes: the port's formatter) and read by the port's formatter, through a
// ResourceSet the application's ResourceManagers are given (BuildManager's providers, the strongly typed classes).
// Found by the round-trip tests (tests/RuntimeUnitTests/RoundTrip).

using System.Collections;
using System.IO;
using System.Resources;
using WebFormsForCore.Serialization.Formatters.Binary;

namespace System.Web.Compilation
{
	internal static class SerializedResources
	{
		/// <summary>Whether .NET's ResourceWriter writes the value itself (its ResourceTypeCodes: no serialization).</summary>
		internal static bool IsNative(object value) =>
			value == null || value is string || value is bool || value is char || value is byte || value is sbyte || value is short || value is ushort ||
			value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal ||
			value is DateTime || value is TimeSpan || value is byte[] || value is Stream;

		/// <summary>A resource added to a writer: the value, or what .NET Framework's writer wrote of it (its type's name, its bytes).</summary>
		internal static void Add(ResourceWriter writer, string name, object value)
		{
			if (IsNative(value))
			{
				writer.AddResource(name, value);
				return;
			}
			var stream = new MemoryStream();
			new BinaryFormatter().Serialize(stream, value);
			writer.AddResourceData(name, FormatterServices.GetFrameworkAssemblyQualifiedName(value.GetType()), stream.ToArray());
		}

		/// <summary>Resources written (as the build providers write them).</summary>
		internal static void Write(Stream stream, IEnumerable entries)
		{
			using (var writer = new ResourceWriter(stream))
			{
				foreach (DictionaryEntry entry in entries)
					Add(writer, (string)entry.Key, entry.Value);
				writer.Generate();
			}
		}

		/// <summary>The ResourceSet a ResourceManager of the application's resources is given (its objects read by the port's formatter).</summary>
		internal static Type SetType => typeof(SerializedResourceSet);
	}

	/// <summary>A ResourceSet whose serialized objects are read by the port's BinaryFormatter.</summary>
	internal sealed class SerializedResourceSet : ResourceSet
	{
		public SerializedResourceSet(Stream stream) : base(new SerializedResourceReader(new ResourceReader(stream))) { }

		// (a ResourceManager of .resources files: CreateFileBasedResourceManager)
		public SerializedResourceSet(string fileName) : base(new SerializedResourceReader(new ResourceReader(fileName))) { }

		// What a ResourceManager makes of a .resources stream (the reader its header names: .NET's ResourceReader).
		public SerializedResourceSet(IResourceReader reader) : base(reader is ResourceReader resources ? new SerializedResourceReader(resources) : reader) { }

		public override Type GetDefaultReader() => typeof(ResourceReader);
	}

	/// <summary>.NET's ResourceReader, the objects it does not deserialize read by the port's BinaryFormatter.</summary>
	internal sealed class SerializedResourceReader : IResourceReader
	{
		private readonly ResourceReader _reader;

		public SerializedResourceReader(ResourceReader reader) => _reader = reader;

		public SerializedResourceReader(Stream stream) : this(new ResourceReader(stream)) { }

		public void Close() => _reader.Close();

		public void Dispose() => _reader.Dispose();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IDictionaryEnumerator GetEnumerator()
		{
			// in the file's order
			var entries = new ArrayList();
			var e = _reader.GetEnumerator();
			while (e.MoveNext())
			{
				var name = (string)e.Key;
				object value;
				try
				{
					value = e.Value;
				}
				catch (NotSupportedException)
				{
					_reader.GetResourceData(name, out _, out byte[] data);
					value = new BinaryFormatter().Deserialize(new MemoryStream(data));
				}
				entries.Add(new DictionaryEntry(name, value));
			}
			return new Enumerator(entries);
		}

		private sealed class Enumerator : IDictionaryEnumerator
		{
			private readonly ArrayList _entries;
			private int _index = -1;

			public Enumerator(ArrayList entries) => _entries = entries;

			public DictionaryEntry Entry => (DictionaryEntry)_entries[_index];
			public object Key => Entry.Key;
			public object Value => Entry.Value;
			public object Current => Entry;
			public bool MoveNext() => ++_index < _entries.Count;
			public void Reset() => _index = -1;
		}
	}
}
