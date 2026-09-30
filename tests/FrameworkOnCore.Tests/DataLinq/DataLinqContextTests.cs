using System.Data.Linq;

namespace FrameworkOnCore.Tests.DataLinq;

/// <summary>
/// DataContext behavior that needs no database: change tracking (InsertOnSubmit / DeleteOnSubmit /
/// Attach as GetChangeSet sees them), the guards and their .NET Framework 4.8 messages (the port
/// regenerates them from the original's resources: generate-dlinq-resources.ps1), Dispose.
/// </summary>
public class DataLinqContextTests
{
    static StoreDb Db() => new("Data Source=nowhere;Initial Catalog=X;Integrated Security=True");

    [Fact]
    public void An_insert_is_pending_in_the_change_set()
    {
        using var db = Db();
        var directory = new StoreDirectory { Id = Guid.NewGuid(), FullPath = "/docs", CreateDate = DateTime.UtcNow };

        db.Directories.InsertOnSubmit(directory);

        var changes = db.GetChangeSet();
        Assert.Same(directory, Assert.Single(changes.Inserts));
        Assert.Empty(changes.Updates);
        Assert.Empty(changes.Deletes);
    }

    [Fact]
    public void Inserting_an_associated_graph_is_pending_for_both()
    {
        using var db = Db();
        var directory = new StoreDirectory { Id = Guid.NewGuid(), FullPath = "/docs" };
        directory.Files.Add(new StoreFile { FileId = Guid.NewGuid(), FullPath = "/docs/a.txt" });

        db.Directories.InsertOnSubmit(directory);

        Assert.Equal(2, db.GetChangeSet().Inserts.Count);
    }

    [Fact]
    public void Deleting_what_was_never_attached_is_refused_with_the_original_message()
    {
        using var db = Db();
        var e = Assert.Throws<InvalidOperationException>(() => db.Directories.DeleteOnSubmit(new StoreDirectory { Id = Guid.NewGuid(), FullPath = "/x" }));
        Assert.Equal("Cannot remove an entity that has not been attached.", e.Message);
    }

    [Fact]
    public void An_attached_entity_can_be_deleted()
    {
        using var db = Db();
        var directory = new StoreDirectory { Id = Guid.NewGuid(), FullPath = "/docs" };
        db.Directories.Attach(directory);

        db.Directories.DeleteOnSubmit(directory);

        Assert.Same(directory, Assert.Single(db.GetChangeSet().Deletes));
    }

    [Fact]
    public void An_attached_entitys_change_becomes_an_update()
    {
        using var db = Db();
        var directory = new StoreDirectory { Id = Guid.NewGuid(), FullPath = "/docs" };
        db.Directories.Attach(directory);

        directory.FullPath = "/docs2";

        Assert.Same(directory, Assert.Single(db.GetChangeSet().Updates));
    }

    [Fact]
    public void Without_object_tracking_an_insert_is_refused_with_the_original_message()
    {
        using var db = Db();
        db.ObjectTrackingEnabled = false;
        var e = Assert.Throws<InvalidOperationException>(() => db.Directories.InsertOnSubmit(new StoreDirectory { Id = Guid.NewGuid(), FullPath = "/x" }));
        Assert.Equal("Object tracking is not enabled for the current data context instance.", e.Message);
    }

    [Fact]
    public void A_disposed_context_refuses_with_the_original_object_name()
    {
        var db = Db();
        db.Dispose();
        var e = Assert.Throws<ObjectDisposedException>(() => db.Directories);
        Assert.Equal("DataContext accessed after Dispose.", e.ObjectName);
    }

    [Fact]
    public void The_table_of_a_type_is_the_same_instance()
    {
        using var db = Db();
        Assert.Same(db.Directories, db.GetTable<StoreDirectory>());
    }
}
