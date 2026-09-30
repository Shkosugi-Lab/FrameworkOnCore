using System.Data.Linq;

namespace FrameworkOnCore.Tests.DataLinq;

/// <summary>
/// The entity types of System.Data.Linq itself: Binary (what ASP.NET MVC's model binder and .dbml models
/// hold varbinary in; its literals are .NET Framework 4.8's, checked there), EntitySet / EntityRef and the
/// .dbml-generated synchronization of the association's two sides.
/// </summary>
public class DataLinqTypesTests
{
    [Fact]
    public void A_binary_wraps_its_bytes_immutably()
    {
        Binary binary = new byte[] { 1, 2, 3 };
        Assert.Equal(3, binary.Length);
        Assert.Equal(new byte[] { 1, 2, 3 }, binary.ToArray());
        // ToArray gives a copy: writing to it does not change the Binary.
        binary.ToArray()[0] = 9;
        Assert.Equal(new byte[] { 1, 2, 3 }, binary.ToArray());
    }

    [Fact]
    public void Binaries_of_the_same_bytes_are_equal()
    {
        var left = new Binary(new byte[] { 1, 2, 3 });
        var right = new Binary(new byte[] { 1, 2, 3 });
        Assert.True(left == right);
        Assert.True(left.Equals(right));
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
        Assert.False(left == new Binary(new byte[] { 1, 2 }));
    }

    [Fact]
    public void A_binary_prints_as_quoted_Base64_as_NET_Framework_did() =>
        Assert.Equal("\"AQID\"", new Binary(new byte[] { 1, 2, 3 }).ToString());

    [Fact] // as .NET Framework: null is an empty Binary, not an exception
    public void A_null_binary_is_empty()
    {
        var binary = new Binary(null!);
        Assert.Equal(0, binary.Length);
        Assert.Equal("\"\"", binary.ToString());
    }

    [Fact]
    public void Adding_to_the_entity_set_sets_the_back_reference()
    {
        var directory = new StoreDirectory { Id = Guid.NewGuid(), FullPath = "/docs" };
        var file = new StoreFile { FileId = Guid.NewGuid(), FullPath = "/docs/a.txt" };

        directory.Files.Add(file);

        Assert.Same(directory, file.Directory);
        Assert.Equal(directory.Id, file.DirectoryId);
        Assert.Single(directory.Files);
    }

    [Fact]
    public void Removing_from_the_entity_set_clears_the_back_reference()
    {
        var directory = new StoreDirectory { Id = Guid.NewGuid(), FullPath = "/docs" };
        var file = new StoreFile { FileId = Guid.NewGuid(), FullPath = "/docs/a.txt" };
        directory.Files.Add(file);

        directory.Files.Remove(file);

        Assert.Null(file.Directory);
        Assert.Empty(directory.Files);
    }

    [Fact]
    public void Setting_the_reference_moves_the_entity_between_sets()
    {
        var first = new StoreDirectory { Id = Guid.NewGuid(), FullPath = "/a" };
        var second = new StoreDirectory { Id = Guid.NewGuid(), FullPath = "/b" };
        var file = new StoreFile { FileId = Guid.NewGuid(), FullPath = "/a/x.txt", Directory = first };

        file.Directory = second;

        Assert.Empty(first.Files);
        Assert.Single(second.Files);
        Assert.Equal(second.Id, file.DirectoryId);
    }

    [Fact]
    public void The_foreign_key_cannot_change_under_a_loaded_reference()
    {
        // The .dbml-generated setter: once the association is set, the raw foreign key refuses to move.
        var file = new StoreFile { FileId = Guid.NewGuid(), FullPath = "/a/x.txt", Directory = new StoreDirectory { Id = Guid.NewGuid(), FullPath = "/a" } };

        Assert.Throws<ForeignKeyReferenceAlreadyHasValueException>(() => file.DirectoryId = Guid.NewGuid());
    }
}
