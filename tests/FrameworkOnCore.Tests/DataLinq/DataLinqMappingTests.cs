using System.Data.Linq.Mapping;

namespace FrameworkOnCore.Tests.DataLinq;

/// <summary>
/// The attribute mapping (AttributeMappingSource -> MetaModel), the way .dbml-generated models declare it
/// (BlogEngine.NET's FileStoreDb): the database's name, the tables', the columns' storage and facets, the
/// association's two sides. No database is needed to read the model.
/// </summary>
public class DataLinqMappingTests
{
    static MetaModel Model => new StoreDb("Data Source=nowhere;Initial Catalog=X;Integrated Security=True").Mapping;

    [Fact]
    public void The_database_and_table_names_come_from_the_attributes()
    {
        var model = Model;
        Assert.Equal("FocDataLinqTest", model.DatabaseName);
        Assert.Equal("dbo.foc_Directory", model.GetTable(typeof(StoreDirectory)).TableName);
        Assert.Equal("dbo.foc_File", model.GetTable(typeof(StoreFile)).TableName);
    }

    [Fact]
    public void Columns_keep_their_key_nullability_and_type()
    {
        var members = Model.GetTable(typeof(StoreDirectory)).RowType.DataMembers.Where(m => m.IsPersistent).ToDictionary(m => m.Name);
        Assert.True(members["Id"].IsPrimaryKey);
        Assert.True(members["ParentId"].CanBeNull);
        Assert.False(members["FullPath"].CanBeNull);
        Assert.Equal("NVarChar(255) NOT NULL", members["FullPath"].DbType);
        Assert.Equal("_Id", members["Id"].StorageMember.Name);
    }

    [Fact]
    public void The_association_pairs_its_two_sides()
    {
        var files = Model.GetTable(typeof(StoreDirectory)).RowType.DataMembers.Single(m => m.Name == "Files");
        Assert.True(files.IsAssociation);
        Assert.False(files.Association.IsForeignKey);
        Assert.True(files.Association.IsMany);

        var directory = Model.GetTable(typeof(StoreFile)).RowType.DataMembers.Single(m => m.Name == "Directory");
        Assert.True(directory.Association.IsForeignKey);
        Assert.Equal("DirectoryId", directory.Association.ThisKey.Single().Name);
        Assert.Equal("Id", directory.Association.OtherKey.Single().Name);
        Assert.Same(files.Association.OtherMember, directory);
    }

    [Fact]
    public void Generated_members_are_marked_so()
    {
        var members = Model.GetTable(typeof(Note)).RowType.DataMembers.Where(m => m.IsPersistent).ToDictionary(m => m.Name);
        Assert.True(members["Id"].IsDbGenerated);
        Assert.True(members["Version"].IsVersion);
    }
}
