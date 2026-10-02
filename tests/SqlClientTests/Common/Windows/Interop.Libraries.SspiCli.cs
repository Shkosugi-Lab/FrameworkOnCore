// The test server's SSPI (TDS.EndPoint's SecurityWrapper, Windows): the one library of Windows' Interop.Libraries it
// takes. TDS.EndPoint was its own assembly, with the whole of that class; here it is compiled with the tests, whose
// Interop.Libraries (maintenance-packages' common one) has the others already.
internal static partial class Interop
{
    internal static partial class Libraries
    {
        internal const string SspiCli = "sspicli.dll";
    }
}
