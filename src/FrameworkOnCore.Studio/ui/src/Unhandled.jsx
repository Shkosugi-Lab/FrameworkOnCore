// What the conversion does not make work (in the analysis step): each component it does nothing for, with what happens,
// where the application uses it, what to do, and a port planned; the APIs of no component together last.
import { useMemo } from 'react';
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import ColumnLayout from '@cloudscape-design/components/column-layout';
import Box from '@cloudscape-design/components/box';
import Badge from '@cloudscape-design/components/badge';
import Alert from '@cloudscape-design/components/alert';
import ExpandableSection from '@cloudscape-design/components/expandable-section';
import { t, fmt } from './i18n.js';
import { leftToDo } from './choices.js';
import { Status } from './Parts.jsx';
import { ApiTable } from './Components.jsx';

const NAMED = 5;
const refs = a => (a.binaries ?? []).reduce((s, b) => s + b.count, 0);

const Number_ = ({ value, label }) => <div><Box variant="awsui-key-label">{label}</Box><Box fontSize="heading-m" fontWeight="bold">{fmt(value)}</Box></div>;

export function UnhandledList({ ctx, groups }) {
  const components = useMemo(() => new Map(ctx.result.components.map(c => [c.id, c])), [ctx.result]);
  if (!groups.length) return <Container><Box textAlign="center" color="text-body-secondary">{t('unhandled.none')}</Box></Container>;
  return (
    <SpaceBetween size="m">
      <Box variant="p">{t('unhandled.intro')}</Box>
      {groups.map(g => <UnhandledCard key={g.component?.id ?? ''} g={g} ctx={ctx} components={components} />)}
    </SpaceBetween>
  );
}

export const useLeftToDo = ctx => useMemo(() => leftToDo(ctx.result, ctx.choices), [ctx.result, ctx.choices]);

function UnhandledCard({ g, ctx, components }) {
  const c = g.component;
  const planned = (c?.options ?? []).filter(o => o.planned);
  const apis = g.apis.slice().sort((x, y) => y.count - x.count || refs(y) - refs(x));
  const dllOnly = g.count === 0 && g.references > 0;
  // What to do: the catalog's for the component; else by what the APIs do (missing: the conversion stubs the methods
  // that call them; throwing: they fail when run).
  const todo = c?.todo ?? t(apis.some(a => a.status === 'Missing') ? 'unhandled.todoMissing' : 'unhandled.todoThrows');
  const named = apis.slice(0, NAMED);
  return (
    <div id={c ? `c-${c.id}` : 'c-other'}>
      <Container
        header={<Header variant="h2"
          description={c ? c.note : t('unhandled.otherNote')}
          info={<SpaceBetween direction="horizontal" size="xs">
            {c && <Status status={c.status} />}
            {dllOnly && <Badge>{t('components.dllOnly')}</Badge>}
            {planned.length > 0 && <Badge color="blue">{t('unhandled.plannedBadge')}</Badge>}
          </SpaceBetween>}>
          {c ? <>{c.title} <Box variant="code" display="inline" fontSize="body-s">{c.id}</Box></> : t('unhandled.other')}
        </Header>}
        footer={
          <ExpandableSection headerText={t('unhandled.where', fmt(apis.length))} variant="footer">
            <ApiTable apis={apis} ctx={ctx} options={[]} components={components} />
          </ExpandableSection>
        }>
        <SpaceBetween size="m">
          <div>
            <Box variant="awsui-key-label">{t('unhandled.apis')}</Box>
            {named.map(a => <div key={a.id}><Box variant="code" fontSize="body-s">{a.name}</Box>
              <Box variant="small" display="inline" color="text-body-secondary"> {a.count ? t('unhandled.uses', fmt(a.count)) : t('unhandled.dllUses', fmt(refs(a)))}</Box></div>)}
            {apis.length > named.length && <Box variant="small" color="text-body-secondary">{t('api.andMore', fmt(apis.length - named.length))}</Box>}
          </div>
          <ColumnLayout columns={3} variant="text-grid">
            <Number_ value={g.count} label={t('components.uses')} />
            <Number_ value={g.files.size} label={t('components.files')} />
            <Number_ value={g.references} label={t('components.dllReferences')} />
          </ColumnLayout>
          <Alert type="warning" header={t('unhandled.todo')}>
            {todo}
            {dllOnly && <Box variant="small" color="text-body-secondary" padding={{ top: 'xs' }}>{t('unhandled.dllOnlyHint')}</Box>}
          </Alert>
          {planned.map(o => (
            <Alert key={o.id} type="info" header={<>{t('unhandled.planned')}: {o.title}</>}>{o.description}</Alert>
          ))}
        </SpaceBetween>
      </Container>
    </div>
  );
}
