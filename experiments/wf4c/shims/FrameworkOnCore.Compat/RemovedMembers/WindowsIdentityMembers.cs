namespace System.Security.Principal
{
    /// <summary>WindowsIdentity.Impersonate(), which .NET removed (RemovedTypes/WindowsImpersonationContext.cs).</summary>
    public static class WindowsIdentityMembers
    {
        extension(WindowsIdentity identity)
        {
            public WindowsImpersonationContext Impersonate() => new WindowsImpersonationContext(identity);
        }
    }
}
