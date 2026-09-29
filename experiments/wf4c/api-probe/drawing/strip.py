import json
# Leaves System.Drawing.Common 10 (and its private GDI+ assemblies) out of the application's assemblies (deps.json):
# the runtime does not find it, and AssemblyLoadContext.Resolving is asked.
path = 'out/DrawingProbe.deps.json'
deps = json.load(open(path))
for target in deps['targets'].values():
    for library in target.values():
        for kind in ('runtime', 'runtimeTargets'):
            assets = library.get(kind)
            if assets:
                library[kind] = {a: v for a, v in assets.items() if 'System.Drawing.Common' not in a and 'System.Private.Windows' not in a}
json.dump(deps, open(path, 'w'), indent=2)
