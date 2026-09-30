using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DataLinqParity.Cases
{
    /// <summary>EntitySet without a context: the list operations, the attach / detach callbacks, deferred sources,
    /// ListChanged and the binding list.</summary>
    static class EntitySetCases
    {
        static Order O(int id) => new Order { OrderID = id };

        [Case]
        static void List_Operations(Probe p)
        {
            var log = new List<string>();
            var set = new EntitySet<Order>(o => log.Add("attach " + o.OrderID), o => log.Add("detach " + o.OrderID));
            p.Is("empty Count", set.Count);
            p.Is("empty HasLoadedOrAssignedValues", set.HasLoadedOrAssignedValues);
            p.Is("empty IsDeferred", set.IsDeferred);

            var o1 = O(1); var o2 = O(2); var o3 = O(3);
            set.Add(o1);
            set.Add(o1);   // already in: no second attach
            set.AddRange(new[] { o2, o3 });
            p.Is("after add", set);
            p.Is("Count", set.Count);
            p.Is("Contains", set.Contains(o2));
            p.Is("Contains other", set.Contains(O(9)));
            p.Is("IndexOf", set.IndexOf(o3));
            p.Is("IndexOf missing", set.IndexOf(O(9)));
            p.Is("Item[1]", set[1]);

            var o4 = O(4);
            set.Insert(0, o4);
            p.Is("after insert", set);
            set[0] = O(5);
            p.Is("after set item", set);
            p.Is("Remove", set.Remove(o2));
            p.Is("Remove missing", set.Remove(O(9)));
            set.RemoveAt(0);
            p.Is("after removes", set);

            var array = new Order[4];
            set.CopyTo(array, 1);
            p.Is("CopyTo", array);
            var enumerated = new List<Order>();
            using (var e = set.GetEnumerator()) while (e.MoveNext()) enumerated.Add(e.Current);
            p.Is("GetEnumerator", enumerated);

            set.Clear();
            p.Is("after clear", set);
            p.Is("HasLoadedOrAssignedValues", set.HasLoadedOrAssignedValues);
            p.Is("callbacks", log);
        }

        [Case]
        static void Misuse(Probe p)
        {
            var set = new EntitySet<Order>();
            p.Does("Add null", () => set.Add(null));
            p.Does("AddRange null", () => set.AddRange(null));
            p.Does("Insert out of range", () => set.Insert(5, O(1)));
            p.Does("RemoveAt out of range", () => set.RemoveAt(0));
            p.Try("Item out of range", () => set[3]);
            p.Does("Insert null", () => set.Insert(0, null));
            set.Add(O(1));
            p.Does("Insert existing", () => set.Insert(0, set[0]));
            p.Does("modify while enumerating", () => { foreach (var o in set) set.Add(O(2)); });
            p.Does("Load without source", () => set.Load());
        }

        [Case]
        static void Deferred_Source(Probe p)
        {
            var loads = 0;
            IEnumerable<Order> Source() { loads++; yield return O(1); yield return O(2); }

            var set = new EntitySet<Order>();
            set.SetSource(Source());
            p.Is("IsDeferred", set.IsDeferred);
            p.Is("HasLoadedOrAssignedValues", set.HasLoadedOrAssignedValues);
            p.Is("loads before", loads);
            set.Add(O(3));   // an addition is kept apart until the source loads
            p.Is("loads after add", loads);
            p.Is("Count", set.Count);
            p.Is("items", set);
            p.Is("loads", loads);
            p.Is("IsDeferred after", set.IsDeferred);
            p.Does("SetSource again", () => set.SetSource(new[] { O(9) }));

            var explicitly = new EntitySet<Order>();
            explicitly.SetSource(new[] { O(7) });
            explicitly.Load();
            p.Is("Load", explicitly);
            p.Is("Load HasLoadedOrAssignedValues", explicitly.HasLoadedOrAssignedValues);
        }

        [Case]
        static void Assign(Probe p)
        {
            var log = new List<string>();
            var set = new EntitySet<Order>(o => log.Add("attach " + o.OrderID), o => log.Add("detach " + o.OrderID));
            set.Add(O(1)); set.Add(O(2));
            set.Assign(new[] { O(2), O(3) });
            p.Is("after assign", set);
            p.Is("callbacks", log);
            set.Assign(null);
            p.Is("assign null", set);
            var other = new EntitySet<Order>();
            other.Add(O(8));
            set.Assign(other);
            p.Is("assign set", set);
            set.Assign(set);
            p.Is("assign itself", set);
        }

        [Case]
        static void ListChanged_And_BindingList(Probe p)
        {
            var events = new List<string>();
            var set = new EntitySet<Order>();
            set.ListChanged += (s, e) => events.Add(e.ListChangedType + " " + e.NewIndex + " " + e.OldIndex);
            set.Add(O(1)); set.Add(O(2)); set.Remove(set[0]); set.Insert(0, O(3)); set.Clear();
            p.Is("ListChanged", events);

            set.Add(O(1));
            var list = set.GetNewBindingList();
            p.Is("binding Count", list.Count);
            p.Is("binding type", list.GetType());
            p.Is("AllowNew", list.AllowNew);
            p.Is("AllowEdit", list.AllowEdit);
            p.Is("AllowRemove", list.AllowRemove);
            p.Is("SupportsSorting", list.SupportsSorting);
            var added = list.AddNew();
            p.Is("AddNew", added);
            p.Is("binding Count after AddNew", list.Count);
            p.Is("set Count after AddNew", set.Count);
            list.RemoveAt(0);
            p.Is("set after binding remove", set);
            var sortable = (IBindingList)list;
            p.Does("ApplySort", () => sortable.ApplySort(TypeDescriptor.GetProperties(typeof(Order))["CustomerID"], ListSortDirection.Descending));
            p.Is("IsSorted", sortable.IsSorted);
        }
    }
}
