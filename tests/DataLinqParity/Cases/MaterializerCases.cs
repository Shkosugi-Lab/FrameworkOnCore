using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq.SqlClient.Implementation;
using System.Linq;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DataLinqParity.Cases
{
    /// <summary>
    /// ObjectMaterializer: public for the materializers LINQ to SQL emits (Reflection.Emit), not for applications. Its
    /// fields, protected constructor and static helpers, through a materializer of one's own; the emitted ones run in
    /// every query case.
    /// </summary>
    static class MaterializerCases
    {
        sealed class OwnMaterializer : ObjectMaterializer<DbDataReader>
        {
            public OwnMaterializer() : base() { }
            public override object InsertLookup(int globalMetaType, object instance) => instance;
            public override void SendEntityMaterialized(int globalMetaType, object instance) { }
            public override System.Collections.IEnumerable ExecuteSubQuery(int iSubQuery, object[] args) => new[] { "sub" + iSubQuery };
            public override IEnumerable<T> GetLinkSource<T>(int globalLink, int localFactory, object[] keyValues) => Enumerable.Empty<T>();
            public override IEnumerable<T> GetNestedLinkSource<T>(int globalLink, int localFactory, object instance) => Enumerable.Empty<T>();
            public override bool Read() => false;
            public override bool CanDeferLoad => false;
        }

        [Case]
        static void Fields_And_Helpers(Probe p)
        {
            var m = new OwnMaterializer();
            p.Is("fields", new object[] { m.Ordinals, m.Globals, m.Locals, m.Arguments, m.DataReader, m.BufferReader });
            m.Ordinals = new[] { 1, 2 };
            m.Globals = new object[] { "g" };
            m.Locals = new object[] { "l" };
            m.Arguments = new object[] { "a" };
            p.Is("set", new object[] { m.Ordinals, m.Globals, m.Locals, m.Arguments });
            p.Is("abstract members", new object[] { m.Read(), m.CanDeferLoad, m.InsertLookup(0, "x"), m.ExecuteSubQuery(0, null), m.GetLinkSource<int>(0, 0, null).Count(), m.GetNestedLinkSource<int>(0, 0, null).Count() });
            m.SendEntityMaterialized(0, "x");

            p.Try("Convert", () => ObjectMaterializer<DbDataReader>.Convert<int>(new object[] { 1, 2 }).ToList());
            p.Try("Convert same type", () => ObjectMaterializer<DbDataReader>.Convert<string>(new[] { "a" }).GetType().IsArray);
            p.Try("Convert wrong", () => ObjectMaterializer<DbDataReader>.Convert<int>(new object[] { "x" }).ToList());
            var group = ObjectMaterializer<DbDataReader>.CreateGroup("key", new[] { 1, 2 });
            p.Is("CreateGroup", new object[] { group.Key, group.ToList() });
            var ordered = ObjectMaterializer<DbDataReader>.CreateOrderedEnumerable(new[] { 3, 1 });
            p.Is("CreateOrderedEnumerable", ordered.ToList());
            p.Try("CreateOrderedEnumerable ThenBy", () => ordered.CreateOrderedEnumerable(x => -x, null, false).ToList());
            p.Is("ErrorAssignmentToNull", Probe.Exception(ObjectMaterializer<DbDataReader>.ErrorAssignmentToNull(typeof(int))));
        }
    }
}
