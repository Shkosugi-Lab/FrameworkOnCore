// ③ The components with options to choose: a search and the statuses to show, then a container for each component with
// its numbers, what the converter does for its APIs, its options (tiles) and its APIs (a table, opened on demand).
import { useEffect, useMemo, useRef, useState } from 'react';
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
import Select from '@cloudscape-design/components/select';
import Button from '@cloudscape-design/components/button';
import { t, fmt } from './i18n.js';
import { DLL_COMPONENT, HANDLINGS, HANDLING_COLOR, ORDER, apisOf, choosable, defaultOf, handlingOf } from './choices.js';
import { Muted, Status } from './Parts.jsx';
import { ApiName, Handling, Place } from './Apis.jsx';

const ROWS = 60;

const Number_ = ({ value, label }) => <div><Box variant="awsui-key-label">{label}</Box><Box fontSize="heading-m" fontWeight="bold">{fmt(value)}</Box></div>;

export function ComponentList({ ctx }) {
  const { result } = ctx;
  const [q, setQ] = useState('');
  const pool = useMemo(() => result.components.filter(choosable), [result]);
  const components = useMemo(() => new Map(result.components.map(c => [c.id, c])), [result]);
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
        : shown.map(c => <ComponentCard key={c.id} c={c} ctx={ctx} components={components} />)}
    </SpaceBetween>
  );
}

function ComponentCard({ c, ctx, components }) {
  const { choices, setChoices } = ctx;
  const options = c.options ?? [];
  const chosen = choices.components[c.id] ?? defaultOf(options);
  const apis = useMemo(() => apisOf(ctx.byComponent, c).map(a => ({ ...a, handling: handlingOf(a, components, choices) })), [ctx.byComponent, c, components, choices]);
  const handlings = HANDLINGS.map(k => [k, apis.filter(a => a.handling.kind === k).length]).filter(([, n]) => n > 0);
  const ref = useRef(null);
  useEffect(() => { if (ctx.linked === c.id) ref.current?.scrollIntoView({ block: 'start' }); }, [ctx.linked, c.id]);
  const changed = chosen !== defaultOf(options) || apis.some(a => choices.apis[a.id]);
  const choose = value => setChoices({ ...choices, components: { ...choices.components, [c.id]: value } });

  return (
    <div id={`c-${c.id}`} ref={ref}>
      <Container
        header={<Header variant="h2"
          description={c.note}
          info={<SpaceBetween direction="horizontal" size="xs">
            <Status status={c.status} />
            {c.attentionCount === 0 && c.binaryReferences > 0 && <Badge>{t('components.dllOnly')}</Badge>}
            {changed && <Badge color="severity-medium">{t('common.changed')}</Badge>}
          </SpaceBetween>}>
          {c.title}
        </Header>}
        footer={
          <ExpandableSection headerText={t('components.apis', fmt(apis.length))} defaultExpanded={ctx.linked === c.id} variant="footer">
            {c.id === DLL_COMPONENT ? <RetargetTable apis={apis} /> : <ApiTable apis={apis} ctx={ctx} options={options} components={components} />}
          </ExpandableSection>
        }>
        <SpaceBetween size="m">
          <ColumnLayout columns={c.binaryReferences ? 4 : 3} variant="text-grid">
            <Number_ value={c.attentionApis} label={t('components.attentionApis')} />
            <Number_ value={c.attentionCount} label={t('components.uses')} />
            <Number_ value={c.files} label={t('components.files')} />
            {c.binaryReferences ? <Number_ value={c.binaryReferences} label={t('components.dllReferences')} /> : null}
          </ColumnLayout>
          <SpaceBetween direction="horizontal" size="xs">
            <Box variant="awsui-key-label">{t('components.handlings')}</Box>
            {handlings.map(([k, n]) => <Badge key={k} color={HANDLING_COLOR[k]}>{t(`handling.${k}`)} {fmt(n)}</Badge>)}
          </SpaceBetween>
          <Tiles value={chosen} onChange={ev => choose(ev.detail.value)} columns={Math.min(options.length, 3)}
            items={options.map(o => ({ value: o.id, label: <>{o.title} {o.default && <Badge>{t('common.default')}</Badge>}</>, description: o.description, disabled: o.planned }))} />
        </SpaceBetween>
      </Container>
    </div>
  );
}

function ApiTable({ apis, ctx, options, components }) {
  const { choices, setChoices } = ctx;
  const [limit, setLimit] = useState(ROWS);
  const overridable = options.filter(o => !o.planned).length > 1;
  const choose = (api_, value) => {
    const next = { ...choices.apis };
    if (value) next[api_] = value; else delete next[api_];
    setChoices({ ...choices, apis: next });
  };
  const columns = [
    { id: 'api', header: 'API', cell: a => <ApiName a={a} /> },
    { id: 'handling', header: t('api.handling'), cell: a => <Handling a={a} ctx={ctx} components={components} /> },
    { id: 'status', header: t('api.status'), cell: a => <Status status={a.status} /> },
    { id: 'count', header: t('api.count'), cell: a => fmt(a.count) },
    { id: 'place', header: t('api.place'), cell: a => <Place a={a} ctx={ctx} /> },
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
