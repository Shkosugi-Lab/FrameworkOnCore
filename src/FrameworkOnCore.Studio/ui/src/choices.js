// The user's choices (foc-choices.json: components, APIs, settings) as the page compares and saves them.

export const DLL_COMPONENT = 'dll-references';
export const ORDER = ['Missing', 'Throws', 'WindowsOnly', 'Behavior', 'Obsolete', 'Available'];

export const defaultOf = options => (options.find(o => o.default) ?? options[0])?.id;

/** A component with options to choose from (one only is not a decision; a planned one cannot be chosen yet). */
export function choosable(c) {
  return (c.options ?? []).filter(o => !o.planned).length > 1;
}

/** The option the conversion takes for a component (an API's own first), when it is one that is there. */
export function optionOf(c, choices, api) {
  const id = (api && choices.apis[api]) ?? choices.components[c.id] ?? defaultOf(c.options ?? []);
  return (c.options ?? []).find(o => o.id === id && !o.planned);
}

export const normalize = c => ({ schema: c?.schema ?? 1, components: { ...(c?.components ?? {}) }, apis: { ...(c?.apis ?? {}) }, settings: { ...(c?.settings ?? {}) } });
export const clone = o => JSON.parse(JSON.stringify(o));
export const empty = () => ({ schema: 1, components: {}, apis: {}, settings: {} });

/** The choices as the conversion reads them: an option at its default is the same as none. */
export function effective(result, c) {
  const out = { components: {}, apis: { ...c.apis }, settings: {} };
  for (const comp of result?.components ?? []) {
    const option = c.components[comp.id];
    if (option && option !== defaultOf(comp.options ?? [])) out.components[comp.id] = option;
  }
  for (const s of result?.settings ?? []) {
    const option = c.settings[s.id];
    if (option && option !== defaultOf(s.options)) out.settings[s.id] = option;
  }
  return out;
}

/** How many choices differ between two (the parts given: components and APIs, settings). */
export function changes(result, a, b, parts = ['components', 'apis', 'settings']) {
  if (!a || !b) return 0;
  const x = effective(result, a), y = effective(result, b);
  let n = 0;
  for (const part of parts)
    for (const k of new Set([...Object.keys(x[part]), ...Object.keys(y[part])])) if (x[part][k] !== y[part][k]) n++;
  return n;
}

/**
 * The analysis' result as the page shows it: the catalog's titles and options now (in the page's language: an analysis
 * keeps those of its time), and the DLLs' references the converter retargets (the APIs' retargetedTo) as the component
 * their choice is made for (dll-references: retargeted, or left). Their APIs are their own components' too; without the
 * choice they fail where they are used (the type is not in the assembly the DLL names): Missing.
 */
export function prepare(result, catalog) {
  const byId = new Map((catalog.components ?? []).map(c => [c.id, c]));
  result.settings = catalog.settings ?? result.settings;
  for (const c of result.components) {
    const known = byId.get(c.id);
    if (known) Object.assign(c, { title: known.title, note: known.note, todo: known.todo, options: known.options });
  }
  const byComponent = new Map();
  for (const a of result.apis) {
    if (!byComponent.has(a.component)) byComponent.set(a.component, []);
    byComponent.get(a.component).push(a);
  }
  const dll = byId.get(DLL_COMPONENT);
  const retargeted = result.apis.filter(a => a.retargetedTo);
  if (dll && retargeted.length && !result.components.some(c => c.id === DLL_COMPONENT)) {
    const count = retargeted.reduce((s, a) => s + a.count, 0);
    const references = retargeted.reduce((s, a) => s + (a.binaries ?? []).reduce((t, b) => t + b.count, 0), 0);
    result.components.push({ id: dll.id, title: dll.title, note: dll.note, status: 'Missing', options: dll.options, apis: retargeted.length,
      count, files: 0, attentionApis: retargeted.length, attentionCount: count, binaryReferences: references, projects: [], retargetedApis: 0 });
    byComponent.set(DLL_COMPONENT, retargeted);
  }
  return byComponent;
}

/** A component's APIs that do not work as they were (all of them for one that does), the most used first. */
export function apisOf(byComponent, c) {
  const all = byComponent.get(c.id) ?? [];
  if (c.id === DLL_COMPONENT) return all;
  const attention = all.filter(a => a.status !== 'Available');
  return (attention.length ? attention : all).slice().sort((a, b) => ORDER.indexOf(a.status) - ORDER.indexOf(b.status) || b.count - a.count);
}

/**
 * What the converter does for each API, with the choices as they are: its DLLs' references retargeted (or their calls
 * replaced: dll-references), its component's option (a component with options to choose, the option not none: the API's
 * own if it has one), else what its status means: it works as it is, it works differently (or is obsolete), or nothing
 * is done (missing: a stub, throws when it runs; throws). The worst first.
 */
export const HANDLINGS = ['none', 'option', 'callReplaced', 'retarget', 'differs', 'works'];
export const HANDLING_COLOR = { none: 'red', option: 'blue', callReplaced: 'blue', retarget: 'blue', differs: 'grey', works: 'green' };

export function handlingOf(a, components, choices) {
  if (a.retargetedTo) {
    const dll = components.get(DLL_COMPONENT);
    const option = choices.components[DLL_COMPONENT] ?? defaultOf(dll?.options ?? []);
    if (option !== 'none') return { kind: a.callReplaced ? 'callReplaced' : 'retarget', component: DLL_COMPONENT };
    return { kind: 'none', component: DLL_COMPONENT };
  }
  // The component's option, chosen or its default, when it does something (the port of System.Drawing, the only one, too).
  const c = components.get(a.component);
  if (c && (a.status !== 'Available' || c.status !== 'Available')) {
    const option = optionOf(c, choices, a.id);
    if (option && option.id !== 'none') return { kind: 'option', component: c.id, option };
  }
  if (a.status === 'Available') return { kind: 'works' };
  if (a.status === 'Behavior' || a.status === 'Obsolete') return { kind: 'differs' };
  return { kind: 'none' };
}

/** The APIs with their handling (and their DLLs' references), the worst first, then the most used. */
export function withHandling(result, choices) {
  const components = new Map(result.components.map(c => [c.id, c]));
  return result.apis.map(a => ({
    ...a, handling: handlingOf(a, components, choices),
    references: (a.binaries ?? []).reduce((s, b) => s + b.count, 0),
  })).sort((x, y) => HANDLINGS.indexOf(x.handling.kind) - HANDLINGS.indexOf(y.handling.kind)
    || ORDER.indexOf(x.status) - ORDER.indexOf(y.status) || y.count - x.count || y.references - x.references);
}

/**
 * What the conversion leaves to the application's developers: the components it does nothing for (none to choose, or
 * none chosen) with the APIs left as they were that do not work (handling none), the most used first; the APIs of no
 * catalog component (an assembly's) together last (component null).
 */
export function leftToDo(result, choices) {
  const components = new Map(result.components.map(c => [c.id, c]));
  const groups = new Map();
  for (const a of result.apis) {
    const h = handlingOf(a, components, choices);
    if (a.retargetedTo) continue;
    const c = components.get(a.component);
    const key = c && !a.component.startsWith('fw:') ? c.id : '';
    // An option that works on Windows only (the ACLs' compat): on Linux it fails all the same.
    const windowsOnly = h.kind === 'option' && h.option?.windowsOnly && a.status !== 'Available';
    if (h.kind !== 'none' && !windowsOnly) continue;
    if (c && choosable(c) && !windowsOnly) continue;   // chosen "none" there: its card says so
    if (!groups.has(key)) groups.set(key, { component: key ? c : null, apis: [], count: 0, references: 0, files: new Set() });
    const g = groups.get(key);
    g.apis.push({ ...a, handling: h });
    g.count += a.count;
    g.references += (a.binaries ?? []).reduce((s, b) => s + b.count, 0);
    for (const f of a.files ?? []) g.files.add(f.file);
  }
  const rank = g => [g.component ? 0 : 1, g.count > 0 ? 0 : 1, -g.count, -g.references];
  return [...groups.values()].sort((x, y) => {
    const a = rank(x), b = rank(y);
    for (let i = 0; i < a.length; i++) if (a[i] !== b[i]) return a[i] - b[i];
    return 0;
  });
}

/** The repository's setup steps after its build (--original-step), one a line. */
export const parseSteps = text => String(text ?? '').split('\n').map(s => s.trim()).filter(s => s);

/** The conversion as a command line: the server's (with the choices file), and the original build with its steps. */
export const commandLine = (command, buildOriginal, steps) =>
  command + (buildOriginal ? ' --build-original' : '') + (buildOriginal ? steps : []).map(s => ` --original-step "${s}"`).join('');
