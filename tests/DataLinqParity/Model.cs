using System;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Reflection;

namespace FrameworkOnCore.DataLinqParity
{
    /// <summary>
    /// The model the cases run on, the way the O/R designer (.dbml) and hand-written LINQ to SQL models declare one:
    /// every mapping feature the API has - associations both ways (EntitySet / EntityRef, delete rules), inheritance
    /// (discriminator, default), identity / rowversion / computed columns (IsDbGenerated, IsVersion, Expression,
    /// AutoSync), a deferred column (Link), enums and Binary, functions and stored procedures (FunctionAttribute,
    /// ExecuteMethodCall / CreateMethodCallQuery), custom insert / update / delete (ExecuteDynamic*), OnLoaded /
    /// OnValidate. Deterministic values only (keys and dates are fixed), so both runs see the same.
    /// </summary>
    [Database(Name = "FocDataLinqParity")]
    [Provider(typeof(Sql2008Provider))]
    public class ParityDb : DataContext
    {
        public static readonly MappingSource Mapping_ = new AttributeMappingSource();

        public ParityDb(string connection) : base(connection, Mapping_) { }
        public ParityDb(IDbConnection connection) : base(connection, Mapping_) { }
        public ParityDb(string connection, MappingSource mapping) : base(connection, mapping) { }

        public Table<Customer> Customers => GetTable<Customer>();
        public Table<Order> Orders => GetTable<Order>();
        public Table<Product> Products => GetTable<Product>();
        public Table<Person> People => GetTable<Person>();
        public Table<Note> Notes => GetTable<Note>();

        /// <summary>Set by the cases that look at custom insert / update / delete (the context's Insert{T} etc.).</summary>
        public System.Collections.Generic.List<string> CustomCalls { get; } = new System.Collections.Generic.List<string>();
        public bool UseCustomNoteMethods { get; set; }
        public bool Disposed { get; private set; }

        // Custom insert / update / delete: found by name (Insert{Entity}); they may call the default (ExecuteDynamic*).
        void InsertNote(Note note)
        {
            CustomCalls.Add("InsertNote " + note.Text);
            ExecuteDynamicInsert(note);
        }

        void UpdateNote(Note note)
        {
            CustomCalls.Add("UpdateNote " + note.Text);
            ExecuteDynamicUpdate(note);
        }

        void DeleteNote(Note note)
        {
            CustomCalls.Add("DeleteNote " + note.Id);
            ExecuteDynamicDelete(note);
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }

        [Function(Name = "dbo.fn_OrderCount", IsComposable = true)]
        [return: Parameter(DbType = "Int")]
        public int? OrderCount([Parameter(Name = "customerId", DbType = "NChar(5)")] string customerId) =>
            (int?)ExecuteMethodCall(this, (MethodInfo)MethodBase.GetCurrentMethod(), customerId).ReturnValue;

        [Function(Name = "dbo.fn_OrdersAbove", IsComposable = true)]
        public IQueryable<Order> OrdersAbove([Parameter(DbType = "Decimal(10,2)")] decimal? amount) =>
            CreateMethodCallQuery<Order>(this, (MethodInfo)MethodBase.GetCurrentMethod(), amount);

        [Function(Name = "dbo.sp_CustomersIn")]
        public ISingleResult<Customer> CustomersIn([Parameter(DbType = "NVarChar(40)")] string city) =>
            (ISingleResult<Customer>)ExecuteMethodCall(this, (MethodInfo)MethodBase.GetCurrentMethod(), city).ReturnValue;

        [Function(Name = "dbo.sp_CustomersAndOrders")]
        [ResultType(typeof(Customer))]
        [ResultType(typeof(Order))]
        public IMultipleResults CustomersAndOrders() =>
            (IMultipleResults)ExecuteMethodCall(this, (MethodInfo)MethodBase.GetCurrentMethod()).ReturnValue;

        [Function(Name = "dbo.sp_Double")]
        public int Double([Parameter(DbType = "Int")] int? input, [Parameter(DbType = "Int")] ref int? output)
        {
            var result = ExecuteMethodCall(this, (MethodInfo)MethodBase.GetCurrentMethod(), input, output);
            output = (int?)result.GetParameterValue(1);
            return (int)result.ReturnValue;
        }

        /// <summary>The functions and procedures the model maps, created after CreateDatabase (which makes tables only).</summary>
        public static readonly string[] Routines =
        {
            "CREATE FUNCTION dbo.fn_OrderCount(@customerId NCHAR(5)) RETURNS INT AS BEGIN RETURN (SELECT COUNT(*) FROM dbo.Orders WHERE CustomerID = @customerId) END",
            "CREATE FUNCTION dbo.fn_OrdersAbove(@amount DECIMAL(10,2)) RETURNS TABLE AS RETURN (SELECT * FROM dbo.Orders WHERE Amount > @amount)",
            "CREATE PROCEDURE dbo.sp_CustomersIn(@city NVARCHAR(40)) AS SELECT * FROM dbo.Customers WHERE City = @city ORDER BY CustomerID",
            "CREATE PROCEDURE dbo.sp_CustomersAndOrders AS BEGIN SELECT * FROM dbo.Customers ORDER BY CustomerID; SELECT * FROM dbo.Orders ORDER BY OrderID END",
            "CREATE PROCEDURE dbo.sp_Double(@input INT, @output INT OUTPUT) AS BEGIN SET @output = @input * 2; RETURN @input + 1 END",
        };
    }

    /// <summary>The same database under the SQL Server 2000 / 2005 providers (their SQL generation differs).</summary>
    [Database(Name = "FocDataLinqParity")]
    [Provider(typeof(Sql2000Provider))]
    public class ParityDb2000 : DataContext
    {
        public ParityDb2000(string connection) : base(connection) { }
        public Table<Customer> Customers => GetTable<Customer>();
        public Table<Order> Orders => GetTable<Order>();
        public Table<Product> Products => GetTable<Product>();
    }

    [Database(Name = "FocDataLinqParity")]
    [Provider(typeof(Sql2005Provider))]
    public class ParityDb2005 : DataContext
    {
        public ParityDb2005(string connection) : base(connection) { }
        public Table<Customer> Customers => GetTable<Customer>();
        public Table<Order> Orders => GetTable<Order>();
        public Table<Product> Products => GetTable<Product>();
    }

    [Table(Name = "dbo.Customers")]
    public class Customer : INotifyPropertyChanging, INotifyPropertyChanged
    {
        static readonly PropertyChangingEventArgs emptyChanging = new PropertyChangingEventArgs("");
        string _CustomerID;
        string _CompanyName;
        string _City;
        EntitySet<Order> _Orders;

        public Customer() { _Orders = new EntitySet<Order>(o => { SendChanging(); o.Customer = this; }, o => { SendChanging(); o.Customer = null; }); }

        [Column(Storage = "_CustomerID", DbType = "NChar(5) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string CustomerID { get => _CustomerID; set { if (_CustomerID != value) { SendChanging(); _CustomerID = value; SendChanged("CustomerID"); } } }

        [Column(Storage = "_CompanyName", DbType = "NVarChar(40) NOT NULL", CanBeNull = false)]
        public string CompanyName { get => _CompanyName; set { if (_CompanyName != value) { SendChanging(); _CompanyName = value; SendChanged("CompanyName"); } } }

        [Column(Storage = "_City", DbType = "NVarChar(40)")]
        public string City { get => _City; set { if (_City != value) { SendChanging(); _City = value; SendChanged("City"); } } }

        [Association(Name = "Customer_Order", Storage = "_Orders", ThisKey = "CustomerID", OtherKey = "CustomerID")]
        public EntitySet<Order> Orders { get => _Orders; set => _Orders.Assign(value); }

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
        void SendChanging() => PropertyChanging?.Invoke(this, emptyChanging);
        void SendChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public override string ToString() => "Customer(" + _CustomerID + ", " + _CompanyName + ", " + (_City ?? "null") + ")";
    }

    [Table(Name = "dbo.Orders")]
    public class Order
    {
        EntityRef<Customer> _Customer;
        string _CustomerID;

        [Column(DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int OrderID;

        [Column(Storage = "_CustomerID", DbType = "NChar(5)")]
        public string CustomerID
        {
            get => _CustomerID;
            set
            {
                if (_CustomerID == value) return;
                if (_Customer.HasLoadedOrAssignedValue) throw new ForeignKeyReferenceAlreadyHasValueException();
                _CustomerID = value;
            }
        }

        [Column(DbType = "DateTime NOT NULL")]
        public DateTime OrderDate;

        [Column(DbType = "DateTime")]
        public DateTime? ShippedDate;

        [Column(DbType = "DateTimeOffset NOT NULL")]
        public DateTimeOffset Stamp;

        [Column(DbType = "Decimal(10,2) NOT NULL")]
        public decimal Amount;

        [Association(Name = "Customer_Order", Storage = "_Customer", ThisKey = "CustomerID", OtherKey = "CustomerID", IsForeignKey = true, DeleteRule = "CASCADE")]
        public Customer Customer
        {
            get => _Customer.Entity;
            set
            {
                var previous = _Customer.Entity;
                if (previous == value && _Customer.HasLoadedOrAssignedValue) return;
                if (previous != null) { _Customer.Entity = null; previous.Orders.Remove(this); }
                _Customer.Entity = value;
                if (value != null) { value.Orders.Add(this); _CustomerID = value.CustomerID; }
                else _CustomerID = null;
            }
        }

        public override string ToString() => "Order(" + OrderID + ", " + (_CustomerID ?? "null") + ", " + Amount.ToString(System.Globalization.CultureInfo.InvariantCulture) + ")";
    }

    public enum ProductKind { Tool = 1, Toy = 2, Book = 3 }

    [Table(Name = "dbo.Products")]
    public class Product
    {
        [Column(DbType = "UniqueIdentifier NOT NULL", IsPrimaryKey = true)]
        public Guid ProductID;

        [Column(DbType = "NVarChar(40) NOT NULL", CanBeNull = false, UpdateCheck = UpdateCheck.WhenChanged)]
        public string Name;

        [Column(DbType = "Int NOT NULL")]
        public ProductKind Kind;

        [Column(DbType = "VarBinary(100)", UpdateCheck = UpdateCheck.Never)]
        public Binary Data;

        [Column(DbType = "Float NOT NULL")]
        public double Weight;

        [Column(DbType = "Bit NOT NULL")]
        public bool Discontinued;

        [Column(DbType = "Float NOT NULL", Expression = "[Weight] * 2", IsDbGenerated = true, AutoSync = AutoSync.Always, UpdateCheck = UpdateCheck.Never)]
        public double DoubleWeight;

        // A deferred column: loaded when first read (Link), not with the row.
        [Column(Storage = "_Description", DbType = "NVarChar(MAX)", UpdateCheck = UpdateCheck.Never)]
        public string Description { get => _Description.Value; set => _Description.Value = value; }
        Link<string> _Description;

        public override string ToString() => "Product(" + Name + ", " + Kind + ")";
    }

    [Table(Name = "dbo.People")]
    [InheritanceMapping(Code = "P", Type = typeof(Person), IsDefault = true)]
    [InheritanceMapping(Code = "E", Type = typeof(Employee))]
    [InheritanceMapping(Code = "M", Type = typeof(Manager))]
    public class Person
    {
        [Column(DbType = "Int NOT NULL", IsPrimaryKey = true)]
        public int PersonID;

        [Column(DbType = "NChar(1) NOT NULL", IsDiscriminator = true, CanBeNull = false)]
        public string Kind;

        [Column(DbType = "NVarChar(40) NOT NULL", CanBeNull = false)]
        public string Name;

        public override string ToString() => GetType().Name + "(" + PersonID + ", " + Name + ")";
    }

    public class Employee : Person
    {
        [Column(DbType = "Decimal(10,2)")]
        public decimal? Salary;
    }

    public class Manager : Employee
    {
        [Column(DbType = "Int")]
        public int? Reports;
    }

    [Table(Name = "dbo.Notes")]
    public class Note
    {
        [Column(DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id;

        [Column(DbType = "NVarChar(100) NOT NULL", CanBeNull = false)]
        public string Text;

        [Column(DbType = "rowversion NOT NULL", IsVersion = true, IsDbGenerated = true, CanBeNull = false)]
        public Binary Version;

        public System.Collections.Generic.List<string> Events = new System.Collections.Generic.List<string>();

        // Found by name: after a row is materialized, before SubmitChanges writes it.
        void OnLoaded() => Events.Add("OnLoaded");
        void OnValidate(ChangeAction action)
        {
            Events.Add("OnValidate " + action);
            if (Text == "invalid") throw new InvalidOperationException("OnValidate refused");
        }

        public override string ToString() => "Note(" + Id + ", " + Text + ")";
    }
}
