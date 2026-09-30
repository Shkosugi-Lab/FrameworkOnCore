namespace System.Data
{
    /// <summary>
    /// The state of an entity, as the Entity Framework of .NET Framework (System.Data.Entity.dll, EF 1-4) had it. ASP.NET
    /// MVC's display and editor templates leave properties of this type out (typeof(EntityState)): without the type, every
    /// DisplayFor / EditorFor of an object fails loading System.Data.Entity. EF 6's own is System.Data.Entity.EntityState.
    /// </summary>
    [Flags]
    public enum EntityState
    {
        Detached = 1,
        Unchanged = 2,
        Added = 4,
        Deleted = 8,
        Modified = 16,
    }
}
