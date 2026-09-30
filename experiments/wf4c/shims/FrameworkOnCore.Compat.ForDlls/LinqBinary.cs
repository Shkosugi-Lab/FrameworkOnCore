namespace System.Data.Linq
{
    /// <summary>
    /// An immutable block of binary data, as LINQ to SQL's System.Data.Linq.Binary (.NET has no LINQ to SQL). ASP.NET MVC
    /// refers to it wherever a model is bound (its default binders include one for Binary; HiddenFor writes one as
    /// Base64): without the type, every action with parameters fails loading System.Data.Linq.
    /// </summary>
    [Serializable]
    public sealed class Binary : IEquatable<Binary>
    {
        readonly byte[] bytes;
        int? hashCode;

        public Binary(byte[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            bytes = (byte[])value.Clone();
        }

        public int Length => bytes.Length;

        public byte[] ToArray() => (byte[])bytes.Clone();

        public static implicit operator Binary(byte[] value) => value == null ? null : new Binary(value);

        public bool Equals(Binary binary) => EqualsTo(binary);

        public static bool operator ==(Binary binary1, Binary binary2) =>
            ReferenceEquals(binary1, binary2) || (binary1 is not null && binary2 is not null && binary1.EqualsTo(binary2));

        public static bool operator !=(Binary binary1, Binary binary2) => !(binary1 == binary2);

        public override bool Equals(object obj) => EqualsTo(obj as Binary);

        // As LINQ to SQL: a hash of the bytes (computed once, the bytes do not change).
        public override int GetHashCode()
        {
            if (hashCode is { } known) return known;
            var hash = new HashCode();
            hash.AddBytes(bytes);
            return (hashCode = hash.ToHashCode()).Value;
        }

        // As LINQ to SQL: the bytes in Base64 in double quotes.
        public override string ToString() => "\"" + Convert.ToBase64String(bytes, 0, bytes.Length) + "\"";

        bool EqualsTo(Binary binary) =>
            binary is not null && (ReferenceEquals(this, binary) || bytes.AsSpan().SequenceEqual(binary.bytes));
    }
}
