// The LINQ to SQL test model: a small file store shaped like BlogEngine.NET's (corpora/work/...:
// FileStoreDb.cs, generated from a .dbml), the mapping the port must serve. Attribute mapping, Guid keys, a
// nullable Guid, associations both ways (EntitySet / EntityRef), Binary content, INotifyPropertyChanging /
// INotifyPropertyChanged as the designer generates them; Note adds what BlogEngine does not use but LINQ to
// SQL applications commonly do: an identity key and a rowversion (synchronized back on insert and update).
//
// Compiled twice: into the tests (against the port) and into golden\record.csproj (net48, against .NET
// Framework's System.Data.Linq): keep to what both compile, in the .dbml generator's (nullable-oblivious) style.
#nullable disable
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace FrameworkOnCore.Tests.DataLinq
{
    [Database(Name = "FocDataLinqTest")]
    public class StoreDb : DataContext
    {
        static readonly MappingSource mapping = new AttributeMappingSource();

        public StoreDb(string connection) : base(connection, mapping) { }
        public StoreDb(System.Data.IDbConnection connection) : base(connection, mapping) { }

        public Table<StoreDirectory> Directories { get { return GetTable<StoreDirectory>(); } }
        public Table<StoreFile> Files { get { return GetTable<StoreFile>(); } }
        public Table<Note> Notes { get { return GetTable<Note>(); } }
    }

    [Table(Name = "dbo.foc_Directory")]
    public class StoreDirectory : INotifyPropertyChanging, INotifyPropertyChanged
    {
        static readonly PropertyChangingEventArgs emptyChanging = new PropertyChangingEventArgs(string.Empty);

        Guid _Id;
        Guid? _ParentId;
        string _FullPath;
        DateTime _CreateDate;
        EntitySet<StoreFile> _Files;

        public StoreDirectory()
        {
            _Files = new EntitySet<StoreFile>(attach_Files, detach_Files);
        }

        [Column(Storage = "_Id", DbType = "UniqueIdentifier NOT NULL", IsPrimaryKey = true)]
        public Guid Id
        {
            get { return _Id; }
            set { if (_Id != value) { SendPropertyChanging(); _Id = value; SendPropertyChanged("Id"); } }
        }

        [Column(Storage = "_ParentId", DbType = "UniqueIdentifier")]
        public Guid? ParentId
        {
            get { return _ParentId; }
            set { if (_ParentId != value) { SendPropertyChanging(); _ParentId = value; SendPropertyChanged("ParentId"); } }
        }

        [Column(Storage = "_FullPath", DbType = "NVarChar(255) NOT NULL", CanBeNull = false)]
        public string FullPath
        {
            get { return _FullPath; }
            set { if (_FullPath != value) { SendPropertyChanging(); _FullPath = value; SendPropertyChanged("FullPath"); } }
        }

        [Column(Storage = "_CreateDate", DbType = "DateTime NOT NULL")]
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { if (_CreateDate != value) { SendPropertyChanging(); _CreateDate = value; SendPropertyChanged("CreateDate"); } }
        }

        [Association(Storage = "_Files", ThisKey = "Id", OtherKey = "DirectoryId")]
        public EntitySet<StoreFile> Files
        {
            get { return _Files; }
            set { _Files.Assign(value); }
        }

        void attach_Files(StoreFile entity) { SendPropertyChanging(); entity.Directory = this; }
        void detach_Files(StoreFile entity) { SendPropertyChanging(); entity.Directory = null; }

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging() { var handler = PropertyChanging; if (handler != null) handler(this, emptyChanging); }
        protected virtual void SendPropertyChanged(string propertyName) { var handler = PropertyChanged; if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName)); }
    }

    [Table(Name = "dbo.foc_File")]
    public class StoreFile : INotifyPropertyChanging, INotifyPropertyChanged
    {
        static readonly PropertyChangingEventArgs emptyChanging = new PropertyChangingEventArgs(string.Empty);

        Guid _FileId;
        Guid _DirectoryId;
        string _FullPath;
        Binary _Contents;
        EntityRef<StoreDirectory> _Directory;

        [Column(Storage = "_FileId", DbType = "UniqueIdentifier NOT NULL", IsPrimaryKey = true)]
        public Guid FileId
        {
            get { return _FileId; }
            set { if (_FileId != value) { SendPropertyChanging(); _FileId = value; SendPropertyChanged("FileId"); } }
        }

        [Column(Storage = "_DirectoryId", DbType = "UniqueIdentifier NOT NULL")]
        public Guid DirectoryId
        {
            get { return _DirectoryId; }
            set
            {
                if (_DirectoryId != value)
                {
                    if (_Directory.HasLoadedOrAssignedValue) throw new ForeignKeyReferenceAlreadyHasValueException();
                    SendPropertyChanging(); _DirectoryId = value; SendPropertyChanged("DirectoryId");
                }
            }
        }

        [Column(Storage = "_FullPath", DbType = "NVarChar(255) NOT NULL", CanBeNull = false)]
        public string FullPath
        {
            get { return _FullPath; }
            set { if (_FullPath != value) { SendPropertyChanging(); _FullPath = value; SendPropertyChanged("FullPath"); } }
        }

        [Column(Storage = "_Contents", DbType = "VarBinary(MAX)", UpdateCheck = UpdateCheck.Never)]
        public Binary Contents
        {
            get { return _Contents; }
            set { if (_Contents != value) { SendPropertyChanging(); _Contents = value; SendPropertyChanged("Contents"); } }
        }

        [Association(Storage = "_Directory", ThisKey = "DirectoryId", OtherKey = "Id", IsForeignKey = true)]
        public StoreDirectory Directory
        {
            get { return _Directory.Entity; }
            set
            {
                var previous = _Directory.Entity;
                if (previous != value || _Directory.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();
                    if (previous != null) { _Directory.Entity = null; previous.Files.Remove(this); }
                    _Directory.Entity = value;
                    if (value != null) { value.Files.Add(this); _DirectoryId = value.Id; }
                    else _DirectoryId = default(Guid);
                    SendPropertyChanged("Directory");
                }
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging() { var handler = PropertyChanging; if (handler != null) handler(this, emptyChanging); }
        protected virtual void SendPropertyChanged(string propertyName) { var handler = PropertyChanged; if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName)); }
    }

    [Table(Name = "dbo.foc_Note")]
    public class Note
    {
        [Column(DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id;

        [Column(DbType = "NVarChar(100) NOT NULL", CanBeNull = false)]
        public string Text;

        [Column(DbType = "rowversion NOT NULL", IsVersion = true, IsDbGenerated = true, CanBeNull = false)]
        public Binary Version;
    }
}
