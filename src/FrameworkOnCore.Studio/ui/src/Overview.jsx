// ② The analysis: the numbers, the uses that do not work as they were by status, and the components with their APIs
// (their options are chosen in ③).
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import ColumnLayout from '@cloudscape-design/components/column-layout';
import Box from '@cloudscape-design/components/box';
import BarChart from '@cloudscape-design/components/bar-chart';
import * as tokens from '@cloudscape-design/design-tokens';
import { t, fmt } from './i18n.js';
import { ORDER } from './choices.js';
import { ComponentList } from './Components.jsx';

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

export default function Overview({ ctx }) {
  const r = ctx.result;
  const uses = r.apis.reduce((s, a) => s + a.count, 0);
  const frameworkUses = r.projects.reduce((s, p) => s + p.frameworkCalls, 0);
  const unresolved = r.projects.reduce((s, p) => s + p.unresolved, 0);
  const files = new Set(r.apis.flatMap(a => a.files.map(f => f.file))).size;
  const attention = r.components.filter(c => c.status !== 'Available');
  const count = status => attention.filter(c => c.status === status).length;
  const resolvedRate = frameworkUses + unresolved === 0 ? 100 : (100 * frameworkUses / (frameworkUses + unresolved));

  // Part-to-whole of the uses that do not work as they were, by status.
  const totals = Object.fromEntries(ORDER.map(s => [s, 0]));
  for (const a of r.apis) totals[a.status] += a.count;
  const all = ORDER.reduce((s, k) => s + totals[k], 0);
  const shown = ORDER.slice(0, 5).filter(s => totals[s] > 0);
  const sum = shown.reduce((s, k) => s + totals[k], 0);

  return (
    <SpaceBetween size="l">
      <Box variant="p">{t('step1.intro')}</Box>
      <Container header={<Header variant="h2">{t('step1.numbers')}</Header>}>
        <ColumnLayout columns={4} variant="text-grid">
          <Metric label={t('step1.toDecide')} value={fmt(attention.length)} unit={t('unit.items')}
            foot={t('step1.toDecideFoot', count('Missing'), count('Throws') + count('WindowsOnly'), count('Behavior'))} />
          <Metric label={t('step1.apis')} value={fmt(r.apis.length)} unit={t('unit.kinds')} foot={t('step1.projects', fmt(r.projects.filter(p => !p.skipped).length))} />
          <Metric label={t('step1.uses')} value={fmt(uses)} unit={t('unit.times')} foot={t('step1.files', fmt(files))} />
          <Metric label={t('step1.resolved')} value={resolvedRate.toFixed(1)} unit="%" foot={t('step1.unresolved', fmt(unresolved))} />
        </ColumnLayout>
      </Container>
      <Container header={<Header variant="h2" description={all ? t('step1.breakdownHint', (100 * totals.Available / all).toFixed(1), fmt(sum)) : undefined}>{t('step1.breakdown')}</Header>}>
        {shown.length
          ? <BarChart horizontalBars stackedBars hideFilter height={80} xScaleType="categorical"
              series={shown.map(s => ({ title: t(`status.${s}`), type: 'bar', color: STATUS_COLOR[s], data: [{ x: t('step1.usesAxis'), y: totals[s] }],
                valueFormatter: v => `${fmt(v)} (${(100 * v / sum).toFixed(1)}%)` }))}
              ariaLabel={t('step1.breakdown')} i18nStrings={{ legendAriaLabel: t('chart.legend'), chartAriaRoleDescription: t('chart.bar') }} />
          : <Box color="text-body-secondary">{t('step1.noAttention')}</Box>}
      </Container>
      <Header variant="h2" description={t('step1.componentsHint')}>{t('step1.components')}</Header>
      <ComponentList ctx={ctx} />
    </SpaceBetween>
  );
}
