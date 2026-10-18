// The components (① to look at them, ② to choose): a search and the statuses to show, then a container for each
// component with its numbers, its options (② tiles to choose; ① what is chosen) and its APIs (a table, opened on demand).
import { useMemo, useState } from 'react';
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import TextFilter from '@cloudscape-design/components/text-filter';
import Multiselect from '@cloudscape-design/components/multiselect';
import ColumnLayout from '@cloudscape-design/components/column-layout';
import Box from '@cloudscape-design/components/box';
import Badge from '@cloudscape-design/components/badge';
import Tiles from '@cloudscape-design/components/tiles';
import ExpandableSection from '@cloudscape-design/components/expandable-section';
import Table from '@cloudscape-design/components/table';
import Link from '@cloudscape-design/components/link';
import Select from '@cloudscape-design/components/select';
import Button from '@cloudscape-design/components/button';
import { api } from './api.js';
import { t, fmt, known } from './i18n.js';
import { DLL_COMPONENT, ORDER, apisOf, choosable, defaultOf } from './choices.js';
import { Code, Muted, Status } from './Parts.jsx';

const ROWS = 60;

const Number_ = ({ value, label }) => <div><Box variant="awsui-key-label">{label}</Box><Box fontSize="heading-m" fontWeight="bold">{fmt(value)}</Box></div>;

export function ComponentList({ ctx, deciding = false }) {
  const { result } = ctx;
  const [q, setQ] = useState('');
  const pool = useMemo(() => result.components.filter(c => !deciding || choosable(c)), [result, deciding]);
  const counts = Object.fromEntries(ORDER.map(s => [s, pool.filter(c => c.status === s).length]));
  const statusOptions = ORDER.filter(s => counts[s] > 0).map(s => ({ value: s, label: t(`status.${s}`), tags: [fmt(counts[s])] }));
  const [statuses, setStatuses] = useState(() => statusOptions.filter(o => o.value !== 'Available'));

  // Those the sources use first (a component only DLLs reference after them), the worst status first, the most used first.
  const shown = useMemo(() => {
    const query = q.trim().toLowerCase();
    const on = new Set(statuses.map(s => s.value));
    const rank = c => [c.attentionCount > 0 || c.status === 'Available' ? 0 : 1, ORDER.indexOf(c.status), -c.attentionCount, -c.binaryReferences];
    const compare = (a, b) => { const x = rank(a), y = rank(b); for (let i = 0; i < x.length; i++) if (x[i] !== y[i]) return x[i] - y[i]; return 0; };
    return pool.slice().sort(compare).filter(c => {
      if (!on.has(c.status)) return false;
      if (!query) return true;
      if (`${c.title} ${c.id} ${c.note ?? ''}`.toLowerCase().includes(query)) return true;
      return (ctx.byComponent.get(c.id) ?? []).some(a => a.name.toLowerCase().includes(query) || a.id.toLowerCase().includes(query));
    });
  }, [pool, q, statuses, ctx.byComponent]);

  return (
    <SpaceBetween size="m">
      <ColumnLayout columns={2}>
        <TextFilter filteringText={q} onChange={ev => setQ(ev.detail.filteringText)} filteringPlaceholder={t('components.search')}
          filteringAriaLabel={t('components.search')} countText={t('components.count', shown.length)} />
        <Multiselect selectedOptions={statuses} onChange={ev => setStatuses(ev.detail.selectedOptions)} options={statusOptions}
          placeholder={t('components.statuses')} hideTokens={false} tokenLimit={5} />
      </ColumnLayout>
      {shown.length === 0
        ? <Container><Box textAlign="center" color="text-body-secondary">{t('components.none')}</Box></Container>
        : shown.map(c => <ComponentCard key={c.id} c={c} ctx={ctx} deciding={deciding} />)}
    </SpaceBetween>
  );
}

function ComponentCard({ c, ctx, deciding }) {
  const { choices, setChoices } = ctx;
  const options = c.options ?? [];
  const chosen = choices.components[c.id] ?? defaultOf(options);
  const apis = apisOf(ctx.byComponent, c);
  const changed = chosen !== defaultOf(options) || apis.some(a => choices.apis[a.id]);
  const choose = value => setChoices({ ...choices, components: { ...choices.components, [c.id]: value } });
  const picked = options.find(o => o.id === chosen);

  return (
    <div id={`c-${c.id}`}>
      <Container
        header={<Header variant="h2"
          description={c.note}
          info={<SpaceBetween direction="horizontal" size="xs">
            <Status status={c.status} />
            {c.attentionCount === 0 && c.binaryReferences > 0 && <Badge>{t('components.dllOnly')}</Badge>}
            {c.retargetedApis > 0 && <Badge color="blue">{t('components.retargeted', fmt(c.retargetedApis))}</Badge>}
            {changed && <Badge color="severity-medium">{t('common.changed')}</Badge>}
          </SpaceBetween>}>
          {c.title} <Box variant="code" display="inline" fontSize="body-s">{c.id}</Box>
        </Header>}
        footer={
          <ExpandableSection headerText={t('components.apis', fmt(apis.length))} defaultExpanded={ctx.linked === c.id} variant="footer">
            {c.id === DLL_COMPONENT ? <RetargetTable apis={apis} /> : <ApiTable c={c} apis={apis} ctx={ctx} options={deciding ? options : []} />}
          </ExpandableSection>
        }>
        <SpaceBetween size="m">
          <ColumnLayout columns={c.binaryReferences ? 4 : 3} variant="text-grid">
            <Number_ value={c.attentionApis} label={t('components.attentionApis')} />
            <Number_ value={c.attentionCount} label={t('components.uses')} />
            <Number_ value={c.files} label={t('components.files')} />
            {c.binaryReferences ? <Number_ value={c.binaryReferences} label={t('components.dllReferences')} /> : null}
          </ColumnLayout>
          {deciding && choosable(c)
            ? <Tiles value={chosen} onChange={ev => choose(ev.detail.value)} columns={Math.min(options.length, 3)}
                items={options.map(o => ({ value: o.id, label: <>{o.title} {o.default && <Badge>{t('common.default')}</Badge>}</>, description: o.description, disabled: o.planned }))} />
            : choosable(c)
              ? <Box>{t('components.chosen')}: <b>{picked?.title}</b> {chosen === defaultOf(options) && <Muted>{t('components.chosenDefault')}</Muted>}</Box>
              : <Muted>{t('components.noChoice', options[0]?.title ?? '')}</Muted>}
        </SpaceBetween>
      </Container>
    </div>
  );
}

function ApiTable({ c, apis, ctx, options }) {
  const { choices, setChoices, setSplit } = ctx;
  const [limit, setLimit] = useState(ROWS);
  const overridable = options.filter(o => !o.planned).length > 1;
  const choose = (api_, value) => {
    const next = { ...choices.apis };
    if (value) next[api_] = value; else delete next[api_];
    setChoices({ ...choices, apis: next });
  };
  const openSource = async (place, name) => {
    setSplit({ header: `${place.file}:${place.line}`, description: name, content: <Muted>{t('common.loading')}</Muted> });
    try {
      const src = await api(`/analyses/${ctx.id}/source?file=${encodeURIComponent(place.file)}&line=${place.line}`);
      setSplit({ header: `${place.file}:${place.line}`, description: name, content: <Code lines={src.lines} start={src.start} hit={place.line} /> });
    } catch {
      setSplit({ header: `${place.file}:${place.line}`, description: name, content: <Muted>{t('source.unreadable')}</Muted> });
    }
  };
  const columns = [
    {
      id: 'api', header: 'API', cell: a => <>
        <Box variant="code" fontSize="body-s">{a.name}</Box>
        {(a.obsolete || a.note) && <Muted>{known(a.obsolete ?? a.note)}</Muted>}
        {a.retargetedTo && <Muted>↪ {a.callReplaced ? t('api.callReplaced') : t('api.retargeted')}: {a.assembly} → {a.retargetedTo}</Muted>}
      </>,
    },
    { id: 'status', header: t('api.status'), cell: a => <Status status={a.status} /> },
    { id: 'count', header: t('api.count'), cell: a => fmt(a.count) },
    {
      id: 'place', header: t('api.place'), cell: a => {
        const place = a.places?.[0];
        const dlls = (a.binaries ?? []).map(b => b.file.split('/').pop());
        return <>
          {place ? <Link onFollow={() => openSource(place, a.name)}>{place.file}:{place.line}</Link> : '—'}
          {a.files.length > 1 && <Muted>{t('api.otherFiles', a.files.length - 1)}</Muted>}
          {dlls.length > 0 && <Muted>DLL: {dlls.slice(0, 3).join(', ')}{dlls.length > 3 ? ` ${t('api.andMore', dlls.length - 3)}` : ''}</Muted>}
        </>;
      },
    },
    ...(overridable ? [{
      id: 'override', header: t('api.override'), cell: a => {
        const list = [{ value: '', label: t('api.followComponent') }, ...options.filter(o => !o.planned).map(o => ({ value: o.id, label: o.title }))];
        return <Select selectedOption={list.find(o => o.value === (choices.apis[a.id] ?? '')) ?? list[0]} options={list}
          onChange={ev => choose(a.id, ev.detail.selectedOption.value)} ariaLabel={t('api.override')} expandToViewport />;
      },
    }] : []),
  ];
  return (
    <Table variant="embedded" wrapLines items={apis.slice(0, limit)} columnDefinitions={columns} trackBy="id"
      footer={apis.length > limit && <Box textAlign="center"><Button variant="link" onClick={() => setLimit(limit + ROWS * 3)}>{t('api.more', fmt(apis.length - limit))}</Button></Box>} />
  );
}

// The DLLs' references the converter retargets: .NET has the type, not in the assembly the DLL names (FrameworkOnCore's
// CallContext is in its System.Web, the DLL looks in mscorlib). Grouped by from -> to.
function RetargetTable({ apis }) {
  const groups = new Map();
  for (const a of apis) {
    const key = `${a.assembly} → ${a.retargetedTo}${a.callReplaced ? ' call' : ''}`;
    if (!groups.has(key)) groups.set(key, { key, from: a.assembly, to: a.retargetedTo, call: a.callReplaced, apis: [], dlls: new Set() });
    const g = groups.get(key);
    g.apis.push(a);
    for (const b of a.binaries ?? []) g.dlls.add(b.file.split('/').pop());
  }
  const items = [...groups.values()].map(g => {
    // A call replaced: the members themselves (the type is .NET's); retargeted: the types (and how many members).
    const types = g.apis.filter(a => g.call || a.kind === 'Type').map(a => a.name);
    return { ...g, types, members: g.apis.length - types.length };
  });
  return (
    <SpaceBetween size="s">
      <Muted>{t('retarget.hint')}</Muted>
      <Table variant="embedded" wrapLines items={items} trackBy="key" columnDefinitions={[
        { id: 'flow', header: t('retarget.flow'), cell: g => <><Box variant="code" fontSize="body-s">{g.from}</Box> → <Box variant="code" fontSize="body-s">{g.to}</Box>{g.call && <Muted>{t('api.callReplaced')}</Muted>}</> },
        { id: 'types', header: t('retarget.types'), cell: g => <>{g.types.map(n => <div key={n}><Box variant="code" fontSize="body-s">{n}</Box></div>)}{g.members > 0 && <Muted>{t('retarget.members', fmt(g.members))}</Muted>}</> },
        { id: 'dlls', header: 'DLL', cell: g => [...g.dlls].map(d => <div key={d}>{d}</div>) },
      ]} />
    </SpaceBetween>
  );
}
