using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace FrameworkOnCore.DataLinqParity.Cases
{
    /// <summary>The mapping: the attributes, the metamodel both mapping sources build (every member of every Meta*
    /// class), the accessors, a mapping source and metamodel classes of one's own, and the models a mapping refuses.</summary>
    static class MappingCases
    {
        const string NoServer = "Data Source=unused";

        [Case]
        static void Attribute_Defaults_And_Values(Probe p)
        {
            var association = new AssociationAttribute();
            p.Is("Association defaults", new object[] { association.Name, association.Storage, association.ThisKey, association.OtherKey, association.IsForeignKey, association.IsUnique, association.DeleteRule, association.DeleteOnNull });
            association = new AssociationAttribute { Name = "n", Storage = "s", ThisKey = "a", OtherKey = "b", IsForeignKey = true, IsUnique = true, DeleteRule = "CASCADE", DeleteOnNull = true };
            p.Is("Association set", new object[] { association.Name, association.Storage, association.ThisKey, association.OtherKey, association.IsForeignKey, association.IsUnique, association.DeleteRule, association.DeleteOnNull });

            var column = new ColumnAttribute();
            p.Is("Column defaults", new object[] { column.Name, column.Storage, column.DbType, column.Expression, column.IsPrimaryKey, column.IsDbGenerated, column.IsVersion, column.IsDiscriminator, column.CanBeNull, column.UpdateCheck, column.AutoSync });
            column = new ColumnAttribute { Name = "c", DbType = "Int", Expression = "1", IsPrimaryKey = true, IsDbGenerated = true, IsVersion = true, IsDiscriminator = true, CanBeNull = false, UpdateCheck = UpdateCheck.Never, AutoSync = AutoSync.OnUpdate };
            p.Is("Column set", new object[] { column.Name, column.DbType, column.Expression, column.IsPrimaryKey, column.IsDbGenerated, column.IsVersion, column.IsDiscriminator, column.CanBeNull, column.UpdateCheck, column.AutoSync });

            p.Is("Database", new object[] { new DatabaseAttribute().Name, new DatabaseAttribute { Name = "d" }.Name });
            p.Is("Table", new object[] { new TableAttribute().Name, new TableAttribute { Name = "t" }.Name });
            var function = new FunctionAttribute { Name = "f", IsComposable = true };
            p.Is("Function", new object[] { new FunctionAttribute().Name, new FunctionAttribute().IsComposable, function.Name, function.IsComposable });
            var inheritance = new InheritanceMappingAttribute { Code = "X", Type = typeof(Person), IsDefault = true };
            p.Is("InheritanceMapping", new object[] { new InheritanceMappingAttribute().Code, new InheritanceMappingAttribute().Type, new InheritanceMappingAttribute().IsDefault, inheritance.Code, inheritance.Type, inheritance.IsDefault });
            var parameter = new ParameterAttribute { Name = "p", DbType = "Int" };
            p.Is("Parameter", new object[] { new ParameterAttribute().Name, new ParameterAttribute().DbType, parameter.Name, parameter.DbType });
            var provider = new ProviderAttribute(typeof(Sql2005Provider));
            p.Is("Provider", new object[] { new ProviderAttribute().Type, provider.Type });
            p.Is("ResultType", new ResultTypeAttribute(typeof(Customer)).Type);
            p.Try("ResultType null", () => new ResultTypeAttribute(null).Type);
            var own = new OwnDataAttribute { Name = "o", Storage = "s" };
            p.Is("DataAttribute", new object[] { own.Name, own.Storage });

            // Where each may be put, and more than once or not.
            foreach (var type in new[] { typeof(AssociationAttribute), typeof(ColumnAttribute), typeof(DatabaseAttribute), typeof(TableAttribute), typeof(FunctionAttribute), typeof(InheritanceMappingAttribute), typeof(ParameterAttribute), typeof(ProviderAttribute), typeof(ResultTypeAttribute), typeof(DataAttribute) })
            {
                var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(type, typeof(AttributeUsageAttribute));
                p.Is("usage " + type.Name, usage == null ? "none" : usage.ValidOn + " multiple=" + usage.AllowMultiple + " inherited=" + usage.Inherited);
            }
        }

        [Case]
        static void Attributed_Model(Probe p)
        {
            var model = new AttributeMappingSource().GetModel(typeof(ParityDb));
            DumpModel(p, model, new[] { typeof(Customer), typeof(Order), typeof(Product), typeof(Person), typeof(Employee), typeof(Manager), typeof(Note) });
            p.Is("GetFunction", model.GetFunction(typeof(ParityDb).GetMethod("OrderCount"))?.Name);
            p.Is("GetFunction unmapped", model.GetFunction(typeof(ParityDb).GetMethod("GetChangeSet")));
            p.Is("GetTable of derived", model.GetTable(typeof(Manager))?.TableName);
            p.Is("GetTable unmapped", model.GetTable(typeof(Unmapped)));
            p.Is("GetMetaType unmapped IsEntity", model.GetMetaType(typeof(Unmapped)).IsEntity);
            var person = model.GetMetaType(typeof(Person));
            p.Is("GetInheritanceType", person.GetInheritanceType(typeof(Employee))?.Name);
            p.Is("GetInheritanceType other", person.GetInheritanceType(typeof(Customer)));
            p.Is("GetTypeForInheritanceCode E", person.GetTypeForInheritanceCode("E")?.Name);
            p.Is("GetTypeForInheritanceCode unknown", person.GetTypeForInheritanceCode("Z")?.Name);
            var field = typeof(Order).GetField("Amount");
            p.Is("GetDataMember", model.GetMetaType(typeof(Order)).GetDataMember(field)?.Name);
            p.Try("GetDataMember of other type", () => model.GetMetaType(typeof(Customer)).GetDataMember(field)?.Name);
            p.Try("GetDataMember null", () => model.GetMetaType(typeof(Customer)).GetDataMember(null));
        }

        [Case]
        static void Xml_Model(Probe p)
        {
            var model = XmlMappingSource.FromXml(PlainDb.Xml).GetModel(typeof(PlainDb));
            DumpModel(p, model, new[] { typeof(PlainCustomer), typeof(PlainOrder) });
        }

        [Case]
        static void Xml_Sources(Probe p)
        {
            string Tables(MappingSource source) => string.Join(",", source.GetModel(typeof(PlainDb)).GetTables().Select(t => t.TableName).OrderBy(n => n));
            p.Try("FromXml", () => Tables(XmlMappingSource.FromXml(PlainDb.Xml)));
            p.Try("FromReader", () => { using (var reader = XmlReader.Create(new StringReader(PlainDb.Xml))) return Tables(XmlMappingSource.FromReader(reader)); });
            p.Try("FromStream", () => Tables(XmlMappingSource.FromStream(new MemoryStream(Encoding.UTF8.GetBytes(PlainDb.Xml)))));
            var file = Path.Combine(Path.GetTempPath(), "foc-dlinq-mapping.xml");
            File.WriteAllText(file, PlainDb.Xml);
            try { p.Try("FromUrl", () => Tables(XmlMappingSource.FromUrl(file))); }
            finally { File.Delete(file); }

            p.Try("FromXml null", () => XmlMappingSource.FromXml(null));
            p.Try("FromReader null", () => XmlMappingSource.FromReader(null));
            p.Try("FromStream null", () => XmlMappingSource.FromStream(null));
            p.Try("FromUrl null", () => XmlMappingSource.FromUrl(null));
            p.Try("not XML", () => XmlMappingSource.FromXml("<Database"));
            p.Try("wrong root", () => XmlMappingSource.FromXml("<Other xmlns=\"http://schemas.microsoft.com/linqtosql/mapping/2007\" />"));
            p.Try("unknown element", () => XmlMappingSource.FromXml("<Database Name=\"d\" xmlns=\"http://schemas.microsoft.com/linqtosql/mapping/2007\"><Tabel /></Database>"));
            p.Try("unknown type", () => XmlMappingSource.FromXml(PlainDb.Xml.Replace("PlainOrder\"", "NoSuchOrder\"")).GetModel(typeof(PlainDb)).GetTables().Count());
            p.Try("unknown member", () => XmlMappingSource.FromXml(PlainDb.Xml.Replace("Member=\"Amount\"", "Member=\"Total\"")).GetModel(typeof(PlainDb)).GetTables().Count());
            p.Try("GetModel null", () => XmlMappingSource.FromXml(PlainDb.Xml).GetModel(null));
        }

        [Case]
        static void Mapping_Source_Of_Ones_Own(Probe p)
        {
            var source = new CountingMappingSource();
            var first = source.GetModel(typeof(ParityDb));
            var second = source.GetModel(typeof(ParityDb));
            p.Is("created once", source.Created);
            p.Is("same model", ReferenceEquals(first, second));
            p.Is("model's source is the inner", first.MappingSource.GetType());
            p.Try("GetModel null", () => source.GetModel(null));
            var attributed = new AttributeMappingSource();
            p.Is("attribute source caches", ReferenceEquals(attributed.GetModel(typeof(ParityDb)), attributed.GetModel(typeof(ParityDb))));
            p.Is("model's MappingSource", ReferenceEquals(attributed.GetModel(typeof(ParityDb)).MappingSource, attributed));
        }

        [Case]
        static void Meta_Classes_Of_Ones_Own(Probe p)
        {
            // Derivable, with what their subclasses give; the classes are the provider model's extension points.
            MetaModel model = new OwnMetaModel();
            p.Is("MetaModel", new object[] { model.DatabaseName, model.ContextType, model.ProviderType, model.GetTables().Count(), model.GetFunctions().Count() });
            MetaTable table = new OwnMetaTable();
            p.Is("MetaTable", table.TableName);
            MetaFunction function = new OwnMetaFunction();
            p.Is("MetaFunction", new object[] { function.Name, function.MappedName });
            MetaParameter parameter = new OwnMetaParameter();
            p.Is("MetaParameter", new object[] { parameter.Name, parameter.MappedName, parameter.ParameterType, parameter.DbType });
            MetaAssociation association = new OwnMetaAssociation();
            p.Is("MetaAssociation", association.IsNullable);
            MetaDataMember member = new OwnMetaDataMember();
            p.Is("MetaDataMember", new object[] { member.Name, member.DbType, member.UpdateCheck, member.AutoSync });
            MetaType type = new OwnMetaType();
            p.Is("MetaType", type.Name);

            MetaAccessor fixedAccessor = new FixedAccessor();
            object instance = "x";
            p.Is("MetaAccessor GetBoxedValue", fixedAccessor.GetBoxedValue(instance));
            // The base class's defaults: a value is there, assigned and loaded.
            p.Is("MetaAccessor HasValue", fixedAccessor.HasValue(instance));
            p.Is("MetaAccessor HasAssignedValue", fixedAccessor.HasAssignedValue(instance));
            p.Is("MetaAccessor HasLoadedValue", fixedAccessor.HasLoadedValue(instance));
            fixedAccessor.SetBoxedValue(ref instance, 7);
            p.Is("MetaAccessor SetBoxedValue", instance);

            var name = new NameAccessor();
            var customer = new PlainCustomer { Company = "Old" };
            p.Is("MetaAccessor`2 Type", name.Type);
            p.Is("MetaAccessor`2 GetBoxedValue", name.GetBoxedValue(customer));
            object boxed = customer;
            name.SetBoxedValue(ref boxed, "New");
            p.Is("MetaAccessor`2 SetBoxedValue", customer.Company);
            p.Try("MetaAccessor`2 wrong instance", () => name.GetBoxedValue("not a customer"));
            p.Is("MetaAccessor`2 GetValue", name.GetValue(customer));
            name.SetValue(ref customer, "Set");
            p.Is("MetaAccessor`2 SetValue", customer.Company);
        }

        [Case]
        static void Accessors_Of_A_Model(Probe p)
        {
            var model = new AttributeMappingSource().GetModel(typeof(ParityDb));
            var customerType = model.GetMetaType(typeof(Customer));
            var company = customerType.DataMembers.Single(m => m.Name == "CompanyName");
            var customer = new Customer { CustomerID = "A", CompanyName = "Acme" };
            p.Is("member GetBoxedValue", company.MemberAccessor.GetBoxedValue(customer));
            object boxed = customer;
            company.MemberAccessor.SetBoxedValue(ref boxed, "Changed");
            p.Is("member SetBoxedValue", customer.CompanyName);
            p.Is("storage GetBoxedValue", company.StorageAccessor.GetBoxedValue(customer));
            p.Is("storage accessor is generic", company.StorageAccessor.GetType().BaseType?.IsGenericType);
            var typed = (MetaAccessor<Customer, string>)company.StorageAccessor;
            p.Is("typed GetValue", typed.GetValue(customer));
            typed.SetValue(ref customer, "Typed");
            p.Is("typed SetValue", customer.CompanyName);

            var orderType = model.GetMetaType(typeof(Order));
            var customerRef = orderType.DataMembers.Single(m => m.Name == "Customer");
            var order = new Order();
            p.Is("EntityRef HasAssignedValue empty", customerRef.StorageAccessor.HasAssignedValue(order));
            p.Is("EntityRef HasLoadedValue empty", customerRef.StorageAccessor.HasLoadedValue(order));
            p.Is("EntityRef HasValue empty", customerRef.StorageAccessor.HasValue(order));
            order.Customer = customer;
            p.Is("EntityRef HasAssignedValue", customerRef.StorageAccessor.HasAssignedValue(order));
            p.Is("EntityRef HasValue", customerRef.StorageAccessor.HasValue(order));
            p.Is("deferred value accessor", customerRef.DeferredValueAccessor?.GetBoxedValue(order));
            p.Is("deferred source accessor type", customerRef.DeferredSourceAccessor?.Type);

            var description = model.GetMetaType(typeof(Product)).DataMembers.Single(m => m.Name == "Description");
            var product = new Product();
            p.Is("Link HasLoadedValue", description.StorageAccessor.HasLoadedValue(product));
            p.Is("Link HasAssignedValue", description.StorageAccessor.HasAssignedValue(product));
            product.Description = "d";
            p.Is("Link HasAssignedValue after set", description.StorageAccessor.HasAssignedValue(product));
            p.Is("Link DeferredValueAccessor", description.DeferredValueAccessor?.GetBoxedValue(product));
        }

        [Case]
        static void Models_A_Mapping_Refuses(Probe p)
        {
            var model = new AttributeMappingSource().GetModel(typeof(ErrorDb));
            p.Try("unmapped", () => model.GetTable(typeof(Unmapped)));
            p.Try("bad association key", () => model.GetMetaType(typeof(BadAssociation)).Associations.Count);
            p.Try("inheritance without default", () => model.GetTable(typeof(NoDefaultBase)).RowType.Name);
            p.Try("column mapped twice", () => model.GetTable(typeof(SameColumnTwice)).RowType.DataMembers.Count);
            p.Try("two versions", () => model.GetTable(typeof(TwoVersions)).RowType.VersionMember?.Name);
            p.Try("no key", () => model.GetTable(typeof(NoKey)).RowType.IdentityMembers.Count);
            using (var db = new ErrorDb())
            {
                p.Try("GetTable unmapped", () => db.GetTable<Unmapped>());
                p.Try("GetTable(Type) unmapped", () => db.GetTable(typeof(Unmapped)));
                p.Try("GetTable(Type) null", () => db.GetTable(null));
                p.Try("insert without key", () => { db.GetTable<NoKey>().InsertOnSubmit(new NoKey()); return db.GetChangeSet().Inserts.Count; });
            }
        }

        /// <summary>Everything a metamodel says about the given types, table by table, member by member.</summary>
        static void DumpModel(Probe p, MetaModel model, Type[] types)
        {
            p.Is("model type", model.GetType());
            p.Is("ContextType", model.ContextType);
            p.Is("DatabaseName", model.DatabaseName);
            p.Is("ProviderType", model.ProviderType);
            p.Is("MappingSource", model.MappingSource.GetType());
            p.Is("tables", model.GetTables().Select(t => t.TableName).OrderBy(n => n, StringComparer.Ordinal));
            foreach (var table in model.GetTables().OrderBy(t => t.TableName, StringComparer.Ordinal))
            {
                var prefix = "table " + table.TableName + ": ";
                p.Is(prefix + "type", table.GetType());
                p.Is(prefix + "Model", ReferenceEquals(table.Model, model));
                p.Is(prefix + "RowType", table.RowType.Name);
                p.Is(prefix + "InsertMethod", table.InsertMethod);
                p.Is(prefix + "UpdateMethod", table.UpdateMethod);
                p.Is(prefix + "DeleteMethod", table.DeleteMethod);
            }
            foreach (var type in types) DumpType(p, model, model.GetMetaType(type));
            foreach (var function in model.GetFunctions().OrderBy(f => f.Name, StringComparer.Ordinal))
            {
                var prefix = "function " + function.Name + ": ";
                p.Is(prefix + "type", function.GetType());
                p.Is(prefix + "MappedName", function.MappedName);
                p.Is(prefix + "Method", function.Method);
                p.Is(prefix + "Model", ReferenceEquals(function.Model, model));
                p.Is(prefix + "IsComposable", function.IsComposable);
                p.Is(prefix + "HasMultipleResults", function.HasMultipleResults);
                p.Is(prefix + "ResultRowTypes", function.ResultRowTypes.Select(t => t.Name));
                foreach (var parameter in function.Parameters) DumpParameter(p, prefix + "parameter ", parameter);
                if (function.ReturnParameter != null) DumpParameter(p, prefix + "return ", function.ReturnParameter);
            }
        }

        static void DumpParameter(Probe p, string prefix, MetaParameter parameter) =>
            p.Is(prefix + parameter.Name, new object[] { parameter.GetType().Name, parameter.MappedName, parameter.DbType, parameter.ParameterType, parameter.Parameter?.Name });

        static void DumpType(Probe p, MetaModel model, MetaType type)
        {
            var prefix = "type " + type.Name + ": ";
            p.Is(prefix + "type", type.GetType());
            p.Is(prefix + "Type", type.Type);
            p.Is(prefix + "Model", ReferenceEquals(type.Model, model));
            p.Is(prefix + "Table", type.Table?.TableName);
            p.Is(prefix + "flags", new object[] { type.IsEntity, type.CanInstantiate, type.HasUpdateCheck, type.HasAnyLoadMethod, type.HasAnyValidateMethod });
            p.Is(prefix + "inheritance", new object[] { type.HasInheritance, type.HasInheritanceCode, type.InheritanceCode, type.IsInheritanceDefault, type.InheritanceRoot?.Name, type.InheritanceBase?.Name, type.InheritanceDefault?.Name });
            p.Is(prefix + "InheritanceTypes", type.InheritanceTypes.Select(t => t.Name));
            p.Is(prefix + "DerivedTypes", type.DerivedTypes.Select(t => t.Name));
            p.Is(prefix + "Discriminator", type.Discriminator?.Name);
            p.Is(prefix + "DBGeneratedIdentityMember", type.DBGeneratedIdentityMember?.Name);
            p.Is(prefix + "VersionMember", type.VersionMember?.Name);
            p.Is(prefix + "IdentityMembers", type.IdentityMembers.Select(m => m.Name));
            p.Is(prefix + "PersistentDataMembers", type.PersistentDataMembers.Select(m => m.Name));
            p.Is(prefix + "OnLoadedMethod", type.OnLoadedMethod);
            p.Is(prefix + "OnValidateMethod", type.OnValidateMethod);
            p.Is(prefix + "DataMembers", type.DataMembers.Select(m => m.Name));
            foreach (var member in type.DataMembers)
            {
                var m = prefix + member.Name + ": ";
                p.Is(m + "type", member.GetType());
                p.Is(m + "members", new object[] { member.Member, member.StorageMember, member.MappedName, member.Ordinal, member.Type, member.DeclaringType.Name, member.IsDeclaredBy(type) });
                p.Is(m + "column", new object[] { member.DbType, member.Expression, member.CanBeNull, member.UpdateCheck, member.AutoSync });
                p.Is(m + "flags", new object[] { member.IsPersistent, member.IsPrimaryKey, member.IsDbGenerated, member.IsVersion, member.IsDiscriminator, member.IsAssociation, member.IsDeferred });
                p.Is(m + "accessors", new object[] { member.MemberAccessor?.Type, member.StorageAccessor?.Type, member.DeferredValueAccessor?.Type, member.DeferredSourceAccessor?.Type });
                p.Is(m + "LoadMethod", member.LoadMethod);
                var association = member.Association;
                if (association == null) continue;
                p.Is(m + "association type", association.GetType());
                p.Is(m + "association", new object[] { association.IsMany, association.IsForeignKey, association.IsUnique, association.IsNullable, association.DeleteRule, association.DeleteOnNull });
                p.Is(m + "keys", new object[] { association.ThisKey.Select(k => k.Name), association.OtherKey.Select(k => k.Name), association.ThisKeyIsPrimaryKey, association.OtherKeyIsPrimaryKey });
                p.Is(m + "other", new object[] { association.OtherType.Name, association.OtherMember?.Name, association.ThisMember.Name });
            }
            p.Is(prefix + "Associations", type.Associations.Select(a => a.ThisMember.Name));
        }
    }
}
