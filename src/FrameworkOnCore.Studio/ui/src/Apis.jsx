// The APIs one by one (② all of them; ③ a component's): what the converter does for each (its handling, with the
// choices as they are), its status on .NET 10, its uses and where (a source file opens in the split panel).
import { useMemo, useState } from 'react';
import { useCollection } from '@cloudscape-design/collection-hooks';
import Table from '@cloudscape-design/components/table';
import Header from '@cloudscape-design/components/header';
import TextFilter from '@cloudscape-design/components/text-filter';
import Multiselect from '@cloudscape-design/components/multiselect';
import Pagination from '@cloudscape-design/components/pagination';
import SpaceBetween from '@cloudscape-design/components/space-between';
import Box from '@cloudscape-design/components/box';
import Badge from '@cloudscape-design/components/badge';
import Link from '@cloudscape-design/components/link';
import { api } from './api.js';
import { t, fmt, known } from './i18n.js';
import { HANDLINGS, HANDLING_COLOR, ORDER } from './choices.js';
import { Code, Muted, Status } from './Parts.jsx';

/** A place of the sources, opened in the split panel. */
export async function openSource(ctx, place, name) {
  const header = `${place.file}:${place.line}`;
  ctx.setSplit({ header, description: name, content: <Muted>{t('common.loading')}</Muted> });
  try {
    const src = await api(`/analyses/${ctx.id}/source?file=${encodeURIComponent(place.file)}&line=${place.line}`);
    ctx.setSplit({ header, description: name, content: <Code lines={src.lines} start={src.start} hit={place.line} /> });
  } catch {
    ctx.setSplit({ header, description: name, content: <Muted>{t('source.unreadable')}</Muted> });
  }
}

const componentLink = (ctx, id) => `#/a/${encodeURIComponent(ctx.id)}/c/${encodeURIComponent(id)}`;

/** What the converter does for an API: the handling, and its detail (the option chosen, from -> to). */
export function Handling({ a, ctx, components }) {
  const h = a.handling;
  const c = h.component && components.get(h.component);
  // The catalog's component it is in (none to choose: its title tells what it is); an assembly's (fw:) tells nothing.
  const own = !a.component.startsWith('fw:') && components.get(a.component);
  return <>
    <Badge color={HANDLING_COLOR[h.kind]}>{t(`handling.${h.kind}`)}</Badge>
    {h.kind === 'option' && <Muted>{h.option?.title}{ctx.choices.apis[a.id] ? ` (${t('api.override')})` : ''}</Muted>}
    {(h.kind === 'retarget' || h.kind === 'callReplaced') && <Muted>{a.assembly} → {a.retargetedTo}</Muted>}
    {h.kind === 'none' && h.component && <Muted>{t('handling.dllNotRetargeted')}</Muted>}
    {c && <Muted><Link href={componentLink(ctx, c.id)} fontSize="body-s">{c.title}</Link></Muted>}
    {!c && own && <Muted>{own.title}</Muted>}
  </>;
}

/** Where an API is used: its first place in the sources (and how many files), the DLLs referencing it. */
export function Place({ a, ctx }) {
  const place = a.places?.[0];
  const dlls = (a.binaries ?? []).map(b => b.file.split('/').pop());
  return <>
    {place ? <Link onFollow={() => openSource(ctx, place, a.name)}>{place.file}:{place.line}</Link> : (dlls.length ? null : '—')}
    {a.files.length > 1 && <Muted>{t('api.otherFiles', a.files.length - 1)}</Muted>}
    {dlls.length > 0 && <Muted>DLL: {dlls.slice(0, 3).join(', ')}{dlls.length > 3 ? ` ${t('api.andMore', dlls.length - 3)}` : ''}</Muted>}
  </>;
}

export const ApiName = ({ a }) => <>
  <Box variant="code" fontSize="body-s">{a.name}</Box>
  {(a.obsolete || a.note) && <Muted>{known(a.obsolete ?? a.note)}</Muted>}
</>;

const options = (keys, counts, label) => keys.filter(k => counts.get(k)).map(k => ({ value: k, label: label(k), tags: [fmt(counts.get(k))] }));
const countBy = (items, key) => { const m = new Map(); for (const a of items) m.set(key(a), (m.get(key(a)) ?? 0) + 1); return m; };

/** ② Every API the application uses: found by name, shown by handling, status and assembly; the worst first. */
export function ApiList({ ctx, apis, components }) {
  const handlingCounts = useMemo(() => countBy(apis, a => a.handling.kind), [apis]);
  const statusCounts = useMemo(() => countBy(apis, a => a.status), [apis]);
  const assemblyCounts = useMemo(() => countBy(apis, a => a.assembly), [apis]);
  const assemblies = useMemo(() => [...assemblyCounts.keys()].sort((x, y) => assemblyCounts.get(y) - assemblyCounts.get(x) || x.localeCompare(y)), [assemblyCounts]);
  const handlingOptions = options(HANDLINGS, handlingCounts, k => t(`handling.${k}`));
  const statusOptions = options(ORDER, statusCounts, k => t(`status.${k}`));
  const assemblyOptions = options(assemblies, assemblyCounts, k => k);
  const [handlings, setHandlings] = useState(() => handlingOptions.filter(o => o.value !== 'works'));
  const [statuses, setStatuses] = useState([]);
  const [assemblyFilter, setAssemblyFilter] = useState([]);

  const on = list => list.length ? new Set(list.map(o => o.value)) : null;
  const h = on(handlings), s = on(statuses), asm = on(assemblyFilter);
  const filtered = useMemo(() => apis.filter(a => (!h || h.has(a.handling.kind)) && (!s || s.has(a.status)) && (!asm || asm.has(a.assembly))),
    [apis, handlings, statuses, assemblyFilter]); // eslint-disable-line react-hooks/exhaustive-deps

  const { items, collectionProps, filterProps, paginationProps, filteredItemsCount } = useCollection(filtered, {
    filtering: {
      filteringFunction: (a, text) => {
        const q = text.trim().toLowerCase();
        return !q || a.name.toLowerCase().includes(q) || a.id.toLowerCase().includes(q) || a.namespace.toLowerCase().includes(q);
      },
    },
    pagination: { pageSize: 50 },
    sorting: {},
  });

  const columns = [
    { id: 'api', header: 'API', cell: a => <ApiName a={a} />, sortingField: 'name', minWidth: 320 },
    { id: 'handling', header: t('api.handling'), cell: a => <Handling a={a} ctx={ctx} components={components} />, minWidth: 170, sortingComparator: (x, y) => HANDLINGS.indexOf(x.handling.kind) - HANDLINGS.indexOf(y.handling.kind) },
    { id: 'status', header: t('api.status'), cell: a => <Status status={a.status} />, minWidth: 140, sortingComparator: (x, y) => ORDER.indexOf(x.status) - ORDER.indexOf(y.status) },
    { id: 'assembly', header: t('api.assembly'), cell: a => <Box variant="code" fontSize="body-s">{a.assembly}</Box>, sortingField: 'assembly', minWidth: 170 },
    { id: 'count', header: t('api.count'), cell: a => fmt(a.count), sortingField: 'count', minWidth: 90 },
    { id: 'references', header: t('api.references'), cell: a => a.references ? fmt(a.references) : '—', sortingField: 'references', minWidth: 110 },
    { id: 'place', header: t('api.place'), cell: a => <Place a={a} ctx={ctx} />, minWidth: 240 },
  ];

  return (
    <Table {...collectionProps} resizableColumns items={items} columnDefinitions={columns} trackBy="id" wrapLines variant="container"
      header={<Header variant="h2" counter={filteredItemsCount === apis.length ? `(${fmt(apis.length)})` : `(${fmt(filteredItemsCount)}/${fmt(apis.length)})`}
        description={t('apis.hint')}>{t('apis.title')}</Header>}
      filter={<SpaceBetween direction="horizontal" size="xs">
        <div style={{ minWidth: 280 }}>
          <TextFilter {...filterProps} filteringPlaceholder={t('apis.search')} filteringAriaLabel={t('apis.search')}
            countText={t('components.count', fmt(filteredItemsCount ?? 0))} />
        </div>
        <Multiselect selectedOptions={handlings} onChange={ev => setHandlings(ev.detail.selectedOptions)} options={handlingOptions}
          placeholder={t('apis.handlings')} inlineTokens />
        <Multiselect selectedOptions={statuses} onChange={ev => setStatuses(ev.detail.selectedOptions)} options={statusOptions}
          placeholder={t('apis.statuses')} inlineTokens />
        <Multiselect selectedOptions={assemblyFilter} onChange={ev => setAssemblyFilter(ev.detail.selectedOptions)} options={assemblyOptions}
          placeholder={t('apis.assemblies')} inlineTokens filteringType="auto" />
      </SpaceBetween>}
      pagination={<Pagination {...paginationProps} ariaLabels={{ nextPageLabel: t('pagination.next'), previousPageLabel: t('pagination.previous'), pageLabel: n => t('pagination.page', n) }} />}
      empty={<Box textAlign="center" color="text-body-secondary">{t('apis.none')}</Box>} />
  );
}
