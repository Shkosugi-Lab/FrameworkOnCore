// ② The analysis and what to do with it: the numbers, the APIs by what the converter does for them (or by status), then
// three tabs: what the conversion leaves to the developers (Unhandled.jsx), the components with options to choose
// (Components.jsx; the choices' bar above), and every API (one by one, not by component: an assembly's APIs no component
// gathers are handled each its own way).
import { useEffect, useMemo, useState } from 'react';
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import ColumnLayout from '@cloudscape-design/components/column-layout';
import Box from '@cloudscape-design/components/box';
import BarChart from '@cloudscape-design/components/bar-chart';
import SegmentedControl from '@cloudscape-design/components/segmented-control';
import Tabs from '@cloudscape-design/components/tabs';
import Link from '@cloudscape-design/components/link';
import * as tokens from '@cloudscape-design/design-tokens';
import { t, fmt } from './i18n.js';
import { HANDLINGS, ORDER, choosable, withHandling } from './choices.js';
import { ApiList } from './Apis.jsx';
import { ComponentList } from './Components.jsx';
import { UnhandledList, useLeftToDo } from './Unhandled.jsx';

const HANDLING_CHART = {
  none: tokens.colorChartsStatusCritical, option: tokens.colorChartsPaletteCategorical1, callReplaced: tokens.colorChartsPaletteCategorical4,
  retarget: tokens.colorChartsPaletteCategorical2, differs: tokens.colorChartsStatusNeutral, works: tokens.colorChartsStatusPositive,
};
const STATUS_COLOR = {
  Missing: tokens.colorChartsStatusCritical, Throws: tokens.colorChartsStatusHigh, WindowsOnly: tokens.colorChartsStatusMedium,
  Behavior: tokens.colorChartsStatusLow, Obsolete: tokens.colorChartsStatusNeutral, Available: tokens.colorChartsStatusPositive,
};

const Metric = ({ label, value, unit, foot }) => (
  <div>
    <Box variant="awsui-key-label">{label}</Box>
    <Box fontSize="display-l" fontWeight="bold">{value}<Box variant="span" fontSize="body-m" padding={{ left: 'xxs' }}>{unit}</Box></Box>
    <Box variant="small" color="text-body-secondary">{foot}</Box>
  </div>
);

export default function Overview({ ctx, actions, bar }) {
  const r = ctx.result;
  const components = useMemo(() => new Map(r.components.map(c => [c.id, c])), [r]);
  const apis = useMemo(() => withHandling(r, ctx.choices), [r, ctx.choices]);
  const left = useLeftToDo(ctx);
  const [by, setBy] = useState('handling');
  const [measure, setMeasure] = useState('apis');
  // The tab: a component's link opens the one it is in (to choose for, or left to do).
  const linkedTab = id => !id ? null : left.some(g => g.component?.id === id) ? 'todo' : 'components';
  const [tab, setTab] = useState(() => linkedTab(ctx.linked) ?? (left.length ? 'todo' : 'components'));
  useEffect(() => { const t_ = linkedTab(ctx.linked); if (t_) setTab(t_); }, [ctx.linked]);  // eslint-disable-line react-hooks/exhaustive-deps

  const uses = r.apis.reduce((s, a) => s + a.count, 0);
  const frameworkUses = r.projects.reduce((s, p) => s + p.frameworkCalls, 0);
  const unresolved = r.projects.reduce((s, p) => s + p.unresolved, 0);
  const files = new Set(r.apis.flatMap(a => a.files.map(f => f.file))).size;
  const deciding = r.components.filter(c => choosable(c) && c.status !== 'Available');
  const handled = kind => apis.filter(a => a.handling.kind === kind).length;
  const resolvedRate = frameworkUses + unresolved === 0 ? 100 : (100 * frameworkUses / (frameworkUses + unresolved));
  const inSources = left.filter(g => g.count > 0).length;

  // Part-to-whole of the APIs (or their uses) that do not work as they were: by handling, or by status.
  const keys = by === 'handling' ? HANDLINGS : ORDER;
  const fine = by === 'handling' ? 'works' : 'Available';
  const value = a => measure === 'apis' ? 1 : a.count + a.references;
  const totals = Object.fromEntries(keys.map(k => [k, 0]));
  for (const a of apis) totals[by === 'handling' ? a.handling.kind : a.status] += value(a);
  const all = keys.reduce((s, k) => s + totals[k], 0);
  const shown = keys.filter(k => k !== fine && totals[k] > 0);
  const sum = shown.reduce((s, k) => s + totals[k], 0);
  const label = k => by === 'handling' ? t(`handling.${k}`) : t(`status.${k}`);
  const color = k => by === 'handling' ? HANDLING_CHART[k] : STATUS_COLOR[k];
  const axis = measure === 'apis' ? t('step1.apisAxis') : t('step1.usesAxis');
  const goTab = id => ev => { ev.preventDefault(); setTab(id); document.getElementById('analysis-tabs')?.scrollIntoView({ block: 'start' }); };

  return (
    <SpaceBetween size="l">
      <Box variant="p">{t('step1.intro')}</Box>
      <Container header={<Header variant="h2" actions={actions}>{t('step1.numbers')}</Header>}>
        <ColumnLayout columns={4} variant="text-grid">
          <Metric label={t('step1.apis')} value={fmt(r.apis.length)} unit={t('unit.kinds')}
            foot={<>{t('step1.projects', fmt(r.projects.filter(p => !p.skipped).length))} ・ {t('step1.usesShort', fmt(uses))} ・ {t('step1.files', fmt(files))}</>} />
          <Metric label={t('step1.handled')} value={fmt(handled('option') + handled('retarget') + handled('callReplaced'))} unit={t('unit.kinds')}
            foot={<>{t('step1.handledFoot', fmt(handled('none')), fmt(handled('differs')))} ・ {t('step1.resolvedShort', resolvedRate.toFixed(1))}{unresolved > 0 && <> ・ {t('step1.unresolved', fmt(unresolved))}</>}</>} />
          <Metric label={t('step1.toDo')} value={fmt(left.length)} unit={t('unit.items')}
            foot={<Link href="#" onFollow={goTab('todo')} fontSize="body-s">{t('step1.toDoFoot', fmt(inSources))}</Link>} />
          <Metric label={t('step1.toDecide')} value={fmt(deciding.length)} unit={t('unit.items')}
            foot={<Link href="#" onFollow={goTab('components')} fontSize="body-s">{t('step1.toDecideFoot')}</Link>} />
        </ColumnLayout>
      </Container>
      <Container header={<Header variant="h2"
        description={all ? t(measure === 'apis' ? 'step1.breakdownApisHint' : 'step1.breakdownHint', (100 * totals[fine] / all).toFixed(1), fmt(sum)) : undefined}
        actions={<SpaceBetween direction="horizontal" size="xs">
          <SegmentedControl selectedId={by} onChange={ev => setBy(ev.detail.selectedId)} label={t('step1.by')}
            options={[{ id: 'handling', text: t('step1.byHandling') }, { id: 'status', text: t('step1.byStatus') }]} />
          <SegmentedControl selectedId={measure} onChange={ev => setMeasure(ev.detail.selectedId)} label={t('step1.measure')}
            options={[{ id: 'apis', text: t('step1.apisAxis') }, { id: 'uses', text: t('step1.usesAxis') }]} />
        </SpaceBetween>}>{t('step1.breakdown')}</Header>}>
        {shown.length
          ? <BarChart horizontalBars stackedBars hideFilter height={80} xScaleType="categorical"
              series={shown.map(k => ({ title: label(k), type: 'bar', color: color(k), data: [{ x: axis, y: totals[k] }],
                valueFormatter: v => `${fmt(v)} (${(100 * v / sum).toFixed(1)}%)` }))}
              ariaLabel={t('step1.breakdown')} i18nStrings={{ legendAriaLabel: t('chart.legend'), chartAriaRoleDescription: t('chart.bar') }} />
          : <Box color="text-body-secondary">{t('step1.noAttention')}</Box>}
      </Container>
      {bar}
      <div id="analysis-tabs">
        <Tabs activeTabId={tab} onChange={ev => setTab(ev.detail.activeTabId)} ariaLabel={t('step1.tabs')} tabs={[
          { id: 'todo', label: t('tab.todo', fmt(left.length)), content: <UnhandledList ctx={ctx} groups={left} /> },
          { id: 'components', label: t('tab.components', fmt(deciding.length)), content: <ComponentList ctx={ctx} /> },
          { id: 'apis', label: t('tab.apis', fmt(r.apis.length)), content: <ApiList ctx={ctx} apis={apis} components={components} /> },
        ]} />
      </div>
    </SpaceBetween>
  );
}
