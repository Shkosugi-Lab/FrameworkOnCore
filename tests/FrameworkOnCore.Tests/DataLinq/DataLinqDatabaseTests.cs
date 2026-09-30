using System.Data.Linq;
using System.Transactions;

namespace FrameworkOnCore.Tests.DataLinq;

/// <summary>
/// The port against a real SQL Server (skipped when none is reachable: SqlServer): CreateDatabase from
/// the mapping, the CRUD BlogEngine.NET's DbFileSystemProvider does (insert a graph, query by predicate
/// and association, update, DeleteAllOnSubmit), what the database generates coming back (identity,
/// rowversion), deferred loading, optimistic concurrency, a transaction rolling back.
/// </summary>
public class DataLinqDatabaseTests : IClassFixture<DataLinqDatabaseTests.DatabaseFixture>
{
    /// <summary>One throwaway database for the class: DataContext.CreateDatabase builds it from the mapping.</summary>
    public sealed class DatabaseFixture : IDisposable
    {
        public string? Connection { get; }

        public DatabaseFixture()
        {
            if (!SqlServer.Available) return;
            Connection = SqlServer.For("FocDataLinqTest_" + Guid.NewGuid().ToString("N"));
            using var db = new StoreDb(Connection);
            Assert.False(db.DatabaseExists());
            db.CreateDatabase();
            Assert.True(db.DatabaseExists());
        }

        public void Dispose()
        {
            if (Connection == null) return;
            using var db = new StoreDb(Connection);
            db.DeleteDatabase();
        }
    }

    readonly string? connection;

    public DataLinqDatabaseTests(DatabaseFixture fixture) => connection = fixture.Connection;

    StoreDb Db() => new(connection!);

    [Fact]
    public void A_graph_is_inserted_queried_updated_and_deleted()
    {
        if (connection == null) return;
        var directoryId = Guid.NewGuid();
        using (var db = Db())
        {
            var directory = new StoreDirectory { Id = directoryId, ParentId = null, FullPath = "/docs", CreateDate = DateTime.UtcNow };
            directory.Files.Add(new StoreFile { FileId = Guid.NewGuid(), FullPath = "/docs/a.txt", Contents = new byte[] { 1, 2, 3 } });
            directory.Files.Add(new StoreFile { FileId = Guid.NewGuid(), FullPath = "/docs/b.txt", Contents = new byte[] { 4, 5 } });
            db.Directories.InsertOnSubmit(directory);
            db.SubmitChanges();
        }
        using (var db = Db())
        {
            // The provider's query shapes (DbFileSystemProvider): FirstOrDefault by predicate, ToLower, the association.
            var directory = db.Directories.FirstOrDefault(x => x.FullPath.ToLower() == "/docs" && x.ParentId == null);
            Assert.NotNull(directory);
            Assert.Equal(2, directory!.Files.Count);
            var file = db.Files.FirstOrDefault(f => f.FullPath == "/docs/a.txt" && f.Directory.Id == directoryId);
            Assert.Equal(new byte[] { 1, 2, 3 }, file!.Contents.ToArray());

            file.FullPath = "/docs/renamed.txt";
            db.SubmitChanges();
        }
        using (var db = Db())
        {
            Assert.Equal(1, db.Files.Count(f => f.FullPath == "/docs/renamed.txt"));
            db.Files.DeleteAllOnSubmit(db.Files.Where(f => f.Directory.Id == directoryId));
            db.SubmitChanges();
            // Only this test's rows: the class' tests share the fixture's database.
            Assert.Equal(0, db.Files.Count(f => f.DirectoryId == directoryId));
        }
    }

    [Fact]
    public void What_the_database_generates_comes_back_on_submit()
    {
        if (connection == null) return;
        using var db = Db();
        var note = new Note { Text = "first" };
        db.Notes.InsertOnSubmit(note);
        db.SubmitChanges();

        Assert.True(note.Id > 0);
        Assert.NotNull(note.Version);
        var inserted = note.Version;

        note.Text = "second";
        db.SubmitChanges();
        Assert.NotEqual(inserted, note.Version);
    }

    [Fact]
    public void Deferred_loading_follows_the_association()
    {
        if (connection == null) return;
        var id = Guid.NewGuid();
        using (var db = Db())
        {
            var directory = new StoreDirectory { Id = id, FullPath = "/deferred", CreateDate = DateTime.UtcNow };
            directory.Files.Add(new StoreFile { FileId = Guid.NewGuid(), FullPath = "/deferred/x.txt", Contents = new byte[] { 7 } });
            db.Directories.InsertOnSubmit(directory);
            db.SubmitChanges();
        }
        using (var db = Db())
        {
            var file = db.Files.First(f => f.FullPath == "/deferred/x.txt");
            Assert.Equal("/deferred", file.Directory.FullPath);
        }
    }

    [Fact]
    public void A_conflicting_update_raises_a_change_conflict()
    {
        if (connection == null) return;
        int id;
        using (var db = Db())
        {
            var note = new Note { Text = "original" };
            db.Notes.InsertOnSubmit(note);
            db.SubmitChanges();
            id = note.Id;
        }
        using var first = Db();
        using var second = Db();
        var inFirst = first.Notes.Single(n => n.Id == id);
        var inSecond = second.Notes.Single(n => n.Id == id);
        inFirst.Text = "first writer";
        first.SubmitChanges();

        inSecond.Text = "second writer";
        Assert.Throws<ChangeConflictException>(() => second.SubmitChanges());
        second.ChangeConflicts.ResolveAll(RefreshMode.KeepChanges);
        second.SubmitChanges();

        using var check = Db();
        Assert.Equal("second writer", check.Notes.Single(n => n.Id == id).Text);
    }

    [Fact]
    public void An_uncompleted_transaction_scope_rolls_back()
    {
        if (connection == null) return;
        var id = Guid.NewGuid();
        using (var scope = new TransactionScope())
        using (var db = Db())
        {
            db.Directories.InsertOnSubmit(new StoreDirectory { Id = id, FullPath = "/rollback", CreateDate = DateTime.UtcNow });
            db.SubmitChanges();
            // No scope.Complete(): rolled back.
        }
        using (var db = Db())
            Assert.Equal(0, db.Directories.Count(x => x.Id == id));
    }
}
