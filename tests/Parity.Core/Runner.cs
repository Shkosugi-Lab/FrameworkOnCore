using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FrameworkOnCore.Parity
{
    /// <summary>A case: a static method taking a Probe. Database: it needs the database (the runner's reset runs first).</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CaseAttribute : Attribute
    {
        public bool Database { get; set; }
    }

    /// <summary>
    /// Cases made by a method rather than written one by one (the cases of an API list, member by member): a static
    /// method returning them, each with its name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CaseSourceAttribute : Attribute
    {
    }

    /// <summary>A case to run: its name ("Class.Method", or its source's), whether it needs the database, what it does.</summary>
    public sealed class ParityCase
    {
        public ParityCase(string name, bool database, Action<Probe> run)
        {
            Name = name;
            Database = database;
            Run = run;
        }

        public string Name { get; }
        public bool Database { get; }
        public Action<Probe> Run { get; }
    }

    /// <summary>
    /// Runs the cases of an assembly: every [Case] method and every case of its [CaseSource] methods, by name, one after
    /// another, in the invariant culture (what the runtime words by culture, the same in both runs).
    /// </summary>
    public sealed class Runner
    {
        readonly Assembly assembly;
        readonly Action beforeDatabaseCase;
        List<ParityCase> cases;

        public Runner(Assembly assembly, Action beforeDatabaseCase = null)
        {
            this.assembly = assembly;
            this.beforeDatabaseCase = beforeDatabaseCase;
        }

        public IReadOnlyList<ParityCase> Cases()
        {
            if (cases != null) return cases;
            var methods = assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)).ToList();
            var found = new List<ParityCase>();
            foreach (var method in methods.Where(m => m.GetCustomAttribute<CaseAttribute>() != null))
            {
                var run = (Action<Probe>)Delegate.CreateDelegate(typeof(Action<Probe>), method);
                found.Add(new ParityCase(method.DeclaringType.Name + "." + method.Name, method.GetCustomAttribute<CaseAttribute>().Database, run));
            }
            foreach (var method in methods.Where(m => m.GetCustomAttribute<CaseSourceAttribute>() != null))
                found.AddRange((IEnumerable<ParityCase>)method.Invoke(null, null));
            var duplicates = found.GroupBy(c => c.Name).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicates.Count > 0) throw new InvalidOperationException("cases named twice: " + string.Join(", ", duplicates));
            return cases = found.OrderBy(c => c.Name, StringComparer.Ordinal).ToList();
        }

        public ParityCase Case(string name) => Cases().Single(c => c.Name == name);

        /// <summary>One case's lines.</summary>
        public IReadOnlyList<string> Run(ParityCase parityCase)
        {
            var probe = new Probe();
            var culture = CultureInfo.CurrentCulture;
            var uiCulture = CultureInfo.CurrentUICulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
                if (parityCase.Database) beforeDatabaseCase?.Invoke();
                parityCase.Run(probe);
            }
            catch (Exception e) { probe.Escaped(e); }
            finally { CultureInfo.CurrentCulture = culture; CultureInfo.CurrentUICulture = uiCulture; }
            return probe.Lines;
        }
    }
}
