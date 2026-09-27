namespace System.Reflection.Emit
{
    /// <summary>
    /// The DefineDynamicModule overloads .NET removed with the saving of dynamic assemblies (a file name to
    /// save the module in, symbols to write with it): the module, in memory, as .NET defines it. Proxy
    /// generators written for .NET Framework call them (N2's copy of Castle DynamicProxy). Saving
    /// (AssemblyBuilder.Save) is not here: nothing on .NET saves an AssemblyBuilder, and the code calling
    /// it stays reported.
    /// </summary>
    public static class AssemblyBuilderMembers
    {
        extension(AssemblyBuilder builder)
        {
            public ModuleBuilder DefineDynamicModule(string name, bool emitSymbolInfo) => builder.DefineDynamicModule(name);

            public ModuleBuilder DefineDynamicModule(string name, string fileName) => builder.DefineDynamicModule(name);

            public ModuleBuilder DefineDynamicModule(string name, string fileName, bool emitSymbolInfo) => builder.DefineDynamicModule(name);
        }
    }
}
