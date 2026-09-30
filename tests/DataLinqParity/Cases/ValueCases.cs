using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace FrameworkOnCore.DataLinqParity.Cases
{
    /// <summary>Binary, Link, EntityRef, the enums, the exceptions: the types that are values, without a context.</summary>
    static class ValueCases
    {
        [Case]
        static void Binary_Construction(Probe p)
        {
            var bytes = new byte[] { 1, 2, 3 };
            var binary = new Binary(bytes);
            bytes[0] = 9;   // the constructor copies
            p.Is("copied", binary.ToArray());
            p.Is("length", binary.Length);
            p.Is("null is empty", new Binary(null).Length);
            p.Is("empty", new Binary(new byte[0]).ToArray());
            var array = binary.ToArray();
            array[1] = 9;   // ToArray copies
            p.Is("toarray copies", binary.ToArray());
        }

        [Case]
        static void Binary_Equality(Probe p)
        {
            var a = new Binary(new byte[] { 1, 2, 3 });
            var b = new Binary(new byte[] { 1, 2, 3 });
            var c = new Binary(new byte[] { 1, 2 });
            Binary none = null;
            p.Is("a == b", a == b);
            p.Is("a != b", a != b);
            p.Is("a == c", a == c);
            p.Is("a != c", a != c);
            p.Is("a == null", a == none);
            p.Is("null == null", none == (Binary)null);
            p.Is("null != a", none != a);
            p.Is("Equals(Binary)", a.Equals(b));
            p.Is("Equals(Binary null)", a.Equals((Binary)null));
            p.Is("Equals(object)", a.Equals((object)b));
            p.Is("Equals(object other type)", a.Equals((object)new byte[] { 1, 2, 3 }));
            p.Is("Equals(object null)", a.Equals((object)null));
            p.Is("hash equal", a.GetHashCode() == b.GetHashCode());
            // The hash itself: the original's algorithm, whatever the runtime (a hash stored or compared somewhere).
            p.Is("hash 123", a.GetHashCode());
            p.Is("hash empty", new Binary(new byte[0]).GetHashCode());
            p.Is("hash long", new Binary(Enumerable.Range(0, 300).Select(i => (byte)i).ToArray()).GetHashCode());
        }

        [Case]
        static void Binary_Conversion_And_Text(Probe p)
        {
            Binary converted = new byte[] { 255, 0 };
            p.Is("implicit", converted.ToArray());
            Binary fromNull = (byte[])null;
            p.Is("implicit null", fromNull);
            p.Is("ToString", new Binary(new byte[] { 1, 2, 3 }).ToString());
            p.Is("ToString empty", new Binary(new byte[0]).ToString());
            p.Is("ToString long", new Binary(Enumerable.Range(0, 60).Select(i => (byte)i).ToArray()).ToString());
        }

        [Case]
        static void Link_States(Probe p)
        {
            var empty = new Link<string>();
            p.Is("default HasValue", empty.HasValue);
            p.Is("default HasLoadedOrAssignedValue", empty.HasLoadedOrAssignedValue);
            p.Is("default Value", empty.Value);

            var assigned = new Link<string>("x");
            p.Is("value HasValue", assigned.HasValue);
            p.Is("value HasLoadedOrAssignedValue", assigned.HasLoadedOrAssignedValue);
            p.Is("value Value", assigned.Value);

            var loads = 0;
            var deferred = new Link<string>(Source(() => loads++, "loaded"));
            p.Is("deferred HasValue", deferred.HasValue);
            p.Is("deferred HasLoadedOrAssignedValue before", deferred.HasLoadedOrAssignedValue);
            p.Is("deferred Value", deferred.Value);
            p.Is("deferred HasLoadedOrAssignedValue after", deferred.HasLoadedOrAssignedValue);
            p.Is("deferred Value again", deferred.Value);
            p.Is("loads", loads);

            var copy = new Link<string>(deferred);
            p.Is("copy Value", copy.Value);
            deferred.Value = "set";
            p.Is("set Value", deferred.Value);
            p.Is("set HasLoadedOrAssignedValue", deferred.HasLoadedOrAssignedValue);

            var emptySource = new Link<string>(new string[0]);
            p.Try("empty source Value", () => emptySource.Value);
            var twoSource = new Link<string>(new[] { "a", "b" });
            p.Try("two source Value", () => twoSource.Value);
        }

        [Case]
        static void EntityRef_States(Probe p)
        {
            var empty = new EntityRef<Customer>();
            p.Is("default Entity", empty.Entity);
            p.Is("default HasLoadedOrAssignedValue", empty.HasLoadedOrAssignedValue);

            var customer = new Customer { CustomerID = "C1" };
            var assigned = new EntityRef<Customer>(customer);
            p.Is("entity", assigned.Entity);
            p.Is("entity HasLoadedOrAssignedValue", assigned.HasLoadedOrAssignedValue);

            var loads = 0;
            var deferred = new EntityRef<Customer>(Source(() => loads++, customer));
            p.Is("deferred before", deferred.HasLoadedOrAssignedValue);
            p.Is("deferred Entity", deferred.Entity);
            p.Is("deferred after", deferred.HasLoadedOrAssignedValue);
            p.Is("deferred again", deferred.Entity);
            p.Is("loads", loads);

            var copy = new EntityRef<Customer>(deferred);
            p.Is("copy", copy.Entity);
            deferred.Entity = null;
            p.Is("set null Entity", deferred.Entity);
            p.Is("set null HasLoadedOrAssignedValue", deferred.HasLoadedOrAssignedValue);

            var none = new EntityRef<Customer>(new Customer[0]);
            p.Try("empty source", () => none.Entity);
            var two = new EntityRef<Customer>(new[] { customer, customer });
            p.Try("two source", () => two.Entity);
        }

        static IEnumerable<T> Source<T>(Action counted, T value)
        {
            counted();
            yield return value;
        }

        [Case]
        static void Enums(Probe p)
        {
            p.Is("ChangeAction", new object[] { ChangeAction.None, ChangeAction.Delete, ChangeAction.Insert, ChangeAction.Update }.Select(v => v + "=" + (int)v));
            p.Is("ConflictMode", new object[] { ConflictMode.FailOnFirstConflict, ConflictMode.ContinueOnConflict }.Select(v => v + "=" + (int)v));
            p.Is("RefreshMode", new object[] { RefreshMode.KeepCurrentValues, RefreshMode.KeepChanges, RefreshMode.OverwriteCurrentValues }.Select(v => v + "=" + (int)v));
            p.Is("AutoSync", new object[] { System.Data.Linq.Mapping.AutoSync.Default, System.Data.Linq.Mapping.AutoSync.Always, System.Data.Linq.Mapping.AutoSync.Never, System.Data.Linq.Mapping.AutoSync.OnInsert, System.Data.Linq.Mapping.AutoSync.OnUpdate }.Select(v => v + "=" + (int)v));
            p.Is("UpdateCheck", new object[] { System.Data.Linq.Mapping.UpdateCheck.Always, System.Data.Linq.Mapping.UpdateCheck.Never, System.Data.Linq.Mapping.UpdateCheck.WhenChanged }.Select(v => v + "=" + (int)v));
            p.Is("names", new[] { typeof(ChangeAction), typeof(ConflictMode), typeof(RefreshMode), typeof(System.Data.Linq.Mapping.AutoSync), typeof(System.Data.Linq.Mapping.UpdateCheck) }.Select(t => string.Join(",", Enum.GetNames(t))));
        }

        [Case]
        static void Exceptions(Probe p)
        {
            var inner = new InvalidOperationException("inner");
            p.Is("ChangeConflictException()", Probe.Exception(new ChangeConflictException()));
            p.Is("ChangeConflictException(m)", Probe.Exception(new ChangeConflictException("m")));
            p.Is("ChangeConflictException(m, e)", Probe.Exception(new ChangeConflictException("m", inner)));
            var key = new Customer { CustomerID = "K" };
            var duplicate = new DuplicateKeyException(key);
            p.Is("DuplicateKeyException(o)", Probe.Exception(duplicate));
            p.Is("DuplicateKeyException.Object", duplicate.Object);
            p.Is("DuplicateKeyException(o, m)", Probe.Exception(new DuplicateKeyException(key, "m")));
            var withInner = new DuplicateKeyException(key, "m", inner);
            p.Is("DuplicateKeyException(o, m, e)", Probe.Exception(withInner));
            p.Is("DuplicateKeyException(o, m, e).Object", withInner.Object);
            p.Is("ForeignKeyReferenceAlreadyHasValueException()", Probe.Exception(new ForeignKeyReferenceAlreadyHasValueException()));
            p.Is("ForeignKeyReferenceAlreadyHasValueException(m)", Probe.Exception(new ForeignKeyReferenceAlreadyHasValueException("m")));
            p.Is("ForeignKeyReferenceAlreadyHasValueException(m, e)", Probe.Exception(new ForeignKeyReferenceAlreadyHasValueException("m", inner)));
            p.Is("bases", new[] { typeof(ChangeConflictException).BaseType, typeof(DuplicateKeyException).BaseType, typeof(ForeignKeyReferenceAlreadyHasValueException).BaseType });
        }
    }
}
