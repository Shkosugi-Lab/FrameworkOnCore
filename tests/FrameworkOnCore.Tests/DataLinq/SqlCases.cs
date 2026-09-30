// The query shapes whose SQL the port must translate as .NET Framework does. Taken from BlogEngine.NET's
// DbFileSystemProvider (equality on a Guid, ToLower with &&, a nullable Guid against null and a value, an
// association member in the predicate, FirstOrDefault as Take(1)) plus the common ones (Contains over a
// local list, a projection, OrderBy). golden\record.csproj (net48) writes each case's CommandText and
// parameters to dlinq-sql.golden.json; DataLinqSqlGoldenTests compares the port against it.
//
// Compiled into the tests and into golden\record.csproj (net48): keep to what both compile.
#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;

namespace FrameworkOnCore.Tests.DataLinq
{
    public static class SqlCases
    {
        public static readonly Guid KnownId = new Guid("7c9e6679-7425-40de-944b-e07fc1f90ae7");

        public static IReadOnlyDictionary<string, IQueryable> All(StoreDb db)
        {
            var ids = new List<Guid> { KnownId, new Guid("11111111-2222-3333-4444-555555555555") };
            return new Dictionary<string, IQueryable>
            {
                { "where-equals-guid", db.Directories.Where(x => x.Id == KnownId) },
                { "where-tolower-and", db.Directories.Where(x => x.FullPath.ToLower() == "/docs" && x.Id == KnownId) },
                { "where-nullable-null", db.Directories.Where(x => x.ParentId == null) },
                { "where-nullable-value", db.Directories.Where(x => x.ParentId == KnownId) },
                { "where-association", db.Files.Where(f => f.Directory.Id == KnownId) },
                { "first-or-default-shape", db.Directories.Where(x => x.ParentId == null).Take(1) },
                { "contains-local-list", db.Directories.Where(x => ids.Contains(x.Id)) },
                { "projection", db.Files.Select(f => new { f.FileId, f.FullPath }) },
                { "orderby-date", db.Directories.OrderByDescending(x => x.CreateDate).ThenBy(x => x.FullPath) },
            };
        }
    }
}
