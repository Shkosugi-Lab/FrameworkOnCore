namespace System.Data.Objects
{
    /// <summary>
    /// The Entity Framework 4 (System.Data.Entity.dll) class of the canonical functions for LINQ to Entities
    /// (EntityFunctions.TruncateTime...), its name only: Dynamic LINQ (System.Linq.Dynamic) lists it among the types an
    /// expression may use (typeof, in its ExpressionParser's static constructor), and without the type every dynamic
    /// expression failed loading System.Data.Entity (nopCommerce 3.90's validators). Its functions are not given: EF 6's
    /// are System.Data.Entity.DbFunctions, which EF 6 translates.
    /// </summary>
    public static class EntityFunctions
    {
    }
}
