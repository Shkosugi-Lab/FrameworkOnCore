#if WebFormsForCore
namespace System.ComponentModel.DataAnnotations {

    /// <summary>
    /// .NET Framework's System.ComponentModel.DataAnnotations.ScaffoldTableAttribute: whether a
    /// table (entity class) is shown by Dynamic Data scaffolding. .NET kept ScaffoldColumnAttribute
    /// but not this one; declared here, in its original namespace, so MetaTable reads it and
    /// application classes that carry it compile unchanged.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ScaffoldTableAttribute : Attribute {
        public ScaffoldTableAttribute(bool scaffold) {
            Scaffold = scaffold;
        }

        public bool Scaffold { get; private set; }
    }
}
#endif
