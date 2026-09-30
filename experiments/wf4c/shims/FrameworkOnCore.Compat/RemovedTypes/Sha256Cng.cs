namespace System.Security.Cryptography
{
    /// <summary>
    /// SHA-256 over Windows' CNG, as .NET Framework had it (.NET has SHA256 only): the same hash, from .NET's SHA256.
    /// ASP.NET Web Pages' antiforgery (CryptoUtil.ComputeSHA256) and MVC's child action output cache create one.
    /// </summary>
    public sealed class SHA256Cng : SHA256
    {
        readonly IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

        public override void Initialize()
        {
            // IncrementalHash starts over after GetHashAndReset; a hash begun and not finished is dropped.
            hash.GetHashAndReset();
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize) => hash.AppendData(array, ibStart, cbSize);

        protected override byte[] HashFinal() => hash.GetHashAndReset();

        protected override void Dispose(bool disposing)
        {
            if (disposing) hash.Dispose();
            base.Dispose(disposing);
        }
    }
}
