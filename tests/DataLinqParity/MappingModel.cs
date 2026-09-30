using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;

namespace FrameworkOnCore.DataLinqParity
{
    /// <summary>The XML-mapped model (XmlMappingSource): plain classes, the mapping in XML.</summary>
    public class PlainDb : DataContext
    {
        public PlainDb(string connection, MappingSource mapping) : base(connection, mapping) { }
        public Table<PlainCustomer> Customers => GetTable<PlainCustomer>();
        public Table<PlainOrder> Orders => GetTable<PlainOrder>();

        public int? OrderCount(string customerId) =>
            (int?)ExecuteMethodCall(this, (MethodInfo)MethodBase.GetCurrentMethod(), customerId).ReturnValue;

        public const string Xml =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<Database Name=\"FocDataLinqParity\" Provider=\"System.Data.Linq.SqlClient.Sql2008Provider\" xmlns=\"http://schemas.microsoft.com/linqtosql/mapping/2007\">" +
            "  <Table Name=\"dbo.Customers\" Member=\"Customers\">" +
            "    <Type Name=\"FrameworkOnCore.DataLinqParity.PlainCustomer\">" +
            "      <Column Name=\"CustomerID\" Member=\"Id\" DbType=\"NChar(5) NOT NULL\" IsPrimaryKey=\"true\" CanBeNull=\"false\" />" +
            "      <Column Name=\"CompanyName\" Member=\"Company\" DbType=\"NVarChar(40) NOT NULL\" CanBeNull=\"false\" />" +
            "      <Column Name=\"City\" Member=\"City\" DbType=\"NVarChar(40)\" UpdateCheck=\"Never\" />" +
            "      <Association Name=\"PC_PO\" Member=\"Orders\" Storage=\"orders\" ThisKey=\"Id\" OtherKey=\"CustomerId\" />" +
            "    </Type>" +
            "  </Table>" +
            "  <Table Name=\"dbo.Orders\" Member=\"Orders\">" +
            "    <Type Name=\"FrameworkOnCore.DataLinqParity.PlainOrder\">" +
            "      <Column Name=\"OrderID\" Member=\"Id\" DbType=\"Int NOT NULL IDENTITY\" IsPrimaryKey=\"true\" IsDbGenerated=\"true\" AutoSync=\"OnInsert\" />" +
            "      <Column Name=\"CustomerID\" Member=\"CustomerId\" DbType=\"NChar(5)\" />" +
            "      <Column Name=\"Amount\" Member=\"Amount\" DbType=\"Decimal(10,2) NOT NULL\" />" +
            "      <Association Name=\"PC_PO\" Member=\"Customer\" Storage=\"customer\" ThisKey=\"CustomerId\" OtherKey=\"Id\" IsForeignKey=\"true\" />" +
            "    </Type>" +
            "  </Table>" +
            "  <Function Name=\"dbo.fn_OrderCount\" Method=\"OrderCount\" IsComposable=\"true\">" +
            "    <Parameter Name=\"customerId\" Parameter=\"customerId\" DbType=\"NChar(5)\" />" +
            "    <Return DbType=\"Int\" />" +
            "  </Function>" +
            "</Database>";
    }

    public class PlainCustomer
    {
        EntitySet<PlainOrder> orders = new EntitySet<PlainOrder>();
        public string Id;
        public string Company;
        public string City;
        public EntitySet<PlainOrder> Orders { get => orders; set => orders.Assign(value); }
        public override string ToString() => "PlainCustomer(" + Id + ", " + Company + ")";
    }

    public class PlainOrder
    {
        EntityRef<PlainCustomer> customer;
        public int Id;
        public string CustomerId;
        public decimal Amount;
        public PlainCustomer Customer { get => customer.Entity; set => customer.Entity = value; }
        public override string ToString() => "PlainOrder(" + Id + ", " + CustomerId + ")";
    }

    // Models a mapping refuses (the errors of the attribute mapping), each on its own context.
    public class Unmapped { public int Id; }

    [Table(Name = "dbo.BadKey")]
    public class BadAssociation
    {
        [Column(IsPrimaryKey = true)] public int Id;
        [Association(ThisKey = "Missing", OtherKey = "Id")] public EntitySet<BadAssociation> Children = new EntitySet<BadAssociation>();
    }

    [Table(Name = "dbo.NoDefault")]
    [InheritanceMapping(Code = "A", Type = typeof(NoDefaultBase))]
    [InheritanceMapping(Code = "B", Type = typeof(NoDefaultDerived))]
    public class NoDefaultBase
    {
        [Column(IsPrimaryKey = true)] public int Id;
        [Column(IsDiscriminator = true)] public string Kind;
    }
    public class NoDefaultDerived : NoDefaultBase { }

    [Table(Name = "dbo.TwoColumns")]
    public class SameColumnTwice
    {
        [Column(Name = "Value", IsPrimaryKey = true)] public int A;
        [Column(Name = "Value")] public int B;
    }

    [Table(Name = "dbo.TwoVersions")]
    public class TwoVersions
    {
        [Column(IsPrimaryKey = true)] public int Id;
        [Column(IsVersion = true)] public Binary V1;
        [Column(IsVersion = true)] public Binary V2;
    }

    [Table(Name = "dbo.NoKey")]
    public class NoKey
    {
        [Column] public int Value;
    }

    public class ErrorDb : DataContext
    {
        public ErrorDb() : base("Data Source=unused") { }
    }

    /// <summary>A mapping source of one's own (MappingSource's protected constructor and CreateModel).</summary>
    public sealed class CountingMappingSource : MappingSource
    {
        readonly AttributeMappingSource inner = new AttributeMappingSource();
        public int Created;

        public CountingMappingSource() : base() { }

        protected override MetaModel CreateModel(Type dataContextType)
        {
            Created++;
            return inner.GetModel(dataContextType);
        }
    }

    /// <summary>An accessor of one's own (MetaAccessor`2's protected constructor; the base class's boxing).</summary>
    public sealed class NameAccessor : MetaAccessor<PlainCustomer, string>
    {
        public NameAccessor() : base() { }
        public override string GetValue(PlainCustomer instance) => instance.Company;
        public override void SetValue(ref PlainCustomer instance, string value) => instance.Company = value;
    }

    /// <summary>A non-generic accessor of one's own (MetaAccessor's protected constructor and its defaults).</summary>
    public sealed class FixedAccessor : MetaAccessor
    {
        public FixedAccessor() : base() { }
        public override Type Type => typeof(int);
        public override object GetBoxedValue(object instance) => 42;
        public override void SetBoxedValue(ref object instance, object value) => instance = value;
    }

    // The abstract metamodel classes can be derived from (their protected constructors): a mapping provider of one's own.
    public sealed class OwnMetaModel : MetaModel
    {
        public OwnMetaModel() : base() { }
        public override MappingSource MappingSource => null;
        public override Type ContextType => typeof(PlainDb);
        public override string DatabaseName => "Own";
        public override Type ProviderType => typeof(System.Data.Linq.SqlClient.Sql2008Provider);
        public override MetaTable GetTable(Type rowType) => null;
        public override MetaFunction GetFunction(MethodInfo method) => null;
        public override System.Collections.Generic.IEnumerable<MetaTable> GetTables() => Enumerable.Empty<MetaTable>();
        public override System.Collections.Generic.IEnumerable<MetaFunction> GetFunctions() => Enumerable.Empty<MetaFunction>();
        public override MetaType GetMetaType(Type type) => null;
    }

    public sealed class OwnMetaTable : MetaTable
    {
        public OwnMetaTable() : base() { }
        public override MetaModel Model => null;
        public override string TableName => "Own";
        public override MetaType RowType => null;
        public override MethodInfo InsertMethod => null;
        public override MethodInfo UpdateMethod => null;
        public override MethodInfo DeleteMethod => null;
    }

    public sealed class OwnMetaFunction : MetaFunction
    {
        public OwnMetaFunction() : base() { }
        public override MetaModel Model => null;
        public override MethodInfo Method => null;
        public override string Name => "Own";
        public override string MappedName => "dbo.Own";
        public override bool IsComposable => false;
        public override System.Collections.ObjectModel.ReadOnlyCollection<MetaParameter> Parameters => null;
        public override MetaParameter ReturnParameter => null;
        public override bool HasMultipleResults => false;
        public override System.Collections.ObjectModel.ReadOnlyCollection<MetaType> ResultRowTypes => null;
    }

    public sealed class OwnMetaParameter : MetaParameter
    {
        public OwnMetaParameter() : base() { }
        public override ParameterInfo Parameter => null;
        public override string Name => "own";
        public override string MappedName => "@own";
        public override Type ParameterType => typeof(int);
        public override string DbType => "Int";
    }

    public sealed class OwnMetaAssociation : MetaAssociation
    {
        public OwnMetaAssociation() : base() { }
        public override MetaDataMember ThisMember => null;
        public override MetaDataMember OtherMember => null;
        public override MetaType OtherType => null;
        public override System.Collections.ObjectModel.ReadOnlyCollection<MetaDataMember> ThisKey => null;
        public override System.Collections.ObjectModel.ReadOnlyCollection<MetaDataMember> OtherKey => null;
        public override bool IsMany => false;
        public override bool IsForeignKey => false;
        public override bool IsUnique => false;
        public override bool IsNullable => true;
        public override bool ThisKeyIsPrimaryKey => false;
        public override bool OtherKeyIsPrimaryKey => false;
        public override string DeleteRule => null;
        public override bool DeleteOnNull => false;
    }

    public sealed class OwnMetaDataMember : MetaDataMember
    {
        public OwnMetaDataMember() : base() { }
        public override MetaType DeclaringType => null;
        public override MemberInfo Member => null;
        public override MemberInfo StorageMember => null;
        public override string Name => "Own";
        public override string MappedName => "Own";
        public override int Ordinal => 0;
        public override Type Type => typeof(int);
        public override MetaAccessor MemberAccessor => null;
        public override MetaAccessor StorageAccessor => null;
        public override MetaAccessor DeferredValueAccessor => null;
        public override MetaAccessor DeferredSourceAccessor => null;
        public override bool IsDeferred => false;
        public override bool IsPersistent => true;
        public override bool IsAssociation => false;
        public override bool IsPrimaryKey => false;
        public override bool IsDbGenerated => false;
        public override bool IsVersion => false;
        public override bool IsDiscriminator => false;
        public override bool CanBeNull => false;
        public override string DbType => "Int";
        public override string Expression => null;
        public override UpdateCheck UpdateCheck => UpdateCheck.Always;
        public override AutoSync AutoSync => AutoSync.Default;
        public override MetaAssociation Association => null;
        public override MethodInfo LoadMethod => null;
        public override bool IsDeclaredBy(MetaType type) => false;
    }

    public sealed class OwnMetaType : MetaType
    {
        public OwnMetaType() : base() { }
        public override MetaModel Model => null;
        public override MetaTable Table => null;
        public override Type Type => typeof(object);
        public override string Name => "Own";
        public override bool IsEntity => false;
        public override bool CanInstantiate => false;
        public override MetaDataMember DBGeneratedIdentityMember => null;
        public override MetaDataMember VersionMember => null;
        public override MetaDataMember Discriminator => null;
        public override bool HasUpdateCheck => false;
        public override bool HasInheritance => false;
        public override bool HasInheritanceCode => false;
        public override object InheritanceCode => null;
        public override bool IsInheritanceDefault => false;
        public override MetaType InheritanceRoot => null;
        public override MetaType InheritanceBase => null;
        public override MetaType InheritanceDefault => null;
        public override MetaType GetInheritanceType(Type type) => null;
        public override MetaType GetTypeForInheritanceCode(object code) => null;
        public override System.Collections.ObjectModel.ReadOnlyCollection<MetaType> InheritanceTypes => null;
        public override bool HasAnyLoadMethod => false;
        public override bool HasAnyValidateMethod => false;
        public override System.Collections.ObjectModel.ReadOnlyCollection<MetaType> DerivedTypes => null;
        public override System.Collections.ObjectModel.ReadOnlyCollection<MetaDataMember> DataMembers => null;
        public override System.Collections.ObjectModel.ReadOnlyCollection<MetaDataMember> PersistentDataMembers => null;
        public override System.Collections.ObjectModel.ReadOnlyCollection<MetaDataMember> IdentityMembers => null;
        public override System.Collections.ObjectModel.ReadOnlyCollection<MetaAssociation> Associations => null;
        public override MetaDataMember GetDataMember(MemberInfo member) => null;
        public override MethodInfo OnLoadedMethod => null;
        public override MethodInfo OnValidateMethod => null;
    }

    /// <summary>A data attribute of one's own (DataAttribute's protected constructor).</summary>
    public sealed class OwnDataAttribute : DataAttribute
    {
        public OwnDataAttribute() : base() { }
    }

    /// <summary>A provider type of one's own (SqlProvider's protected Dispose).</summary>
    public sealed class OwnProvider : System.Data.Linq.SqlClient.SqlProvider
    {
        public OwnProvider() : base() { }
        public void DisposeManaged() => Dispose(true);
    }
}
