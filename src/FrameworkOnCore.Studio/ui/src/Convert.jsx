// ④ Converting and building with the saved choices (unsaved ones are saved first); the original build (--build-original)
// and the repository's setup steps after it; the conversion's report (CONVERSION-REPORT.md), its sections opened on demand.
import { useEffect, useState } from 'react';
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import Button from '@cloudscape-design/components/button';
import Checkbox from '@cloudscape-design/components/checkbox';
import FormField from '@cloudscape-design/components/form-field';
import Textarea from '@cloudscape-design/components/textarea';
import Alert from '@cloudscape-design/components/alert';
import Box from '@cloudscape-design/components/box';
import ProgressBar from '@cloudscape-design/components/progress-bar';
import ExpandableSection from '@cloudscape-design/components/expandable-section';
import Badge from '@cloudscape-design/components/badge';
import { post, remove, text } from './api.js';
import { t, fmt, when, duration, reportTitle } from './i18n.js';
import { parseSteps } from './choices.js';
import { Code, LogSection, Muted, RunState } from './Parts.jsx';

export default function Convert({ ctx, onNext }) {
  const body = ctx.conversion;
  const c = body?.conversion;
  const active = c && (c.state === 'queued' || c.state === 'running');
  const done = c?.state === 'done';

  async function start() {
    // What is converted is what is saved: unsaved changes are saved first.
    if (ctx.unsaved > 0 && !(await ctx.save())) return;
    await post(`/analyses/${ctx.id}/conversion`, { buildOriginal: ctx.buildOriginal, originalSteps: parseSteps(ctx.stepsText) });
    ctx.setStepsTyped(null);
    await ctx.loadConversion();
    await ctx.loadAnalyses();
  }
  async function cancel() {
    if (!(await ctx.confirm(t('conversion.cancelQuestion')))) return;
    await remove(`/analyses/${ctx.id}/conversion`);
    await ctx.loadConversion();
  }

  const advice = ctx.advice;
  const reasons = advice?.reasons ?? [];
  const options = (
    <SpaceBetween size="m">
      <Checkbox checked={ctx.buildOriginal} onChange={ev => ctx.setBuildOriginalChoice(ev.detail.checked)}
        description={t('conversion.buildOriginalHint')}>{t('conversion.buildOriginal')}</Checkbox>
      {advice && (advice.needed
        ? <Alert type="info" header={t('conversion.adviceNeeded')}>{reasons.slice(0, 3).join(' ・ ')}{reasons.length > 3 ? ` ・ ${t('conversion.andMore', reasons.length - 3)}` : ''}</Alert>
        : <Muted>{t('conversion.adviceNotNeeded')}</Muted>)}
      <FormField label={t('steps.label')} description={t('conversion.stepsHint')} stretch>
        <Textarea value={ctx.stepsText} onChange={ev => ctx.setStepsTyped(ev.detail.value)} rows={2} spellcheck={false} disabled={!ctx.buildOriginal} />
      </FormField>
    </SpaceBetween>
  );

  return (
    <SpaceBetween size="l">
      <Box variant="p">{t('step4.intro')}</Box>
      <Container header={<Header variant="h2" info={c && <RunState state={c.state} prefix="conversion" />}
        description={!c ? t('conversion.description')
          : active ? (c.state === 'queued' ? t('conversion.queued.hint') : t('conversion.running.hint', duration(c.started)))
            : <>{when(c.finished ?? c.started)} ・ {duration(c.started, c.finished)}{c.buildOriginal ? ` ・ ${t('conversion.fromOriginal')}` : ''}{c.buildOriginal && c.originalSteps?.length ? ` (${t('conversion.withSteps', c.originalSteps.length)})` : ''}</>}
        actions={<SpaceBetween direction="horizontal" size="xs">
          {active && <Button onClick={() => cancel().catch(e => ctx.notify('error', e.message))}>{t('common.cancel')}</Button>}
          {!active && <Button variant={done ? 'normal' : 'primary'} iconName={c ? 'refresh' : undefined}
            onClick={() => start().catch(e => ctx.notify('error', e.message))}>{c ? t('conversion.again') : t('conversion.start')}</Button>}
          {done && <Button variant="primary" onClick={onNext}>{t('conversion.next')}</Button>}
        </SpaceBetween>}>
        {t('conversion.title')}
      </Header>}>
        <SpaceBetween size="m">
          {c?.error && <Alert type="error">{c.error}</Alert>}
          {body?.stale && <Alert type="warning">{t('conversion.stale')}</Alert>}
          {active ? <ProgressBar status="in-progress" /> : options}
          <LogSection lines={body?.log} open={active || (c && !done && !c.sections)} />
        </SpaceBetween>
      </Container>
      <Report ctx={ctx} />
    </SpaceBetween>
  );
}

// The report's sections ("## <title>(<n> 件)" then "- **<subject>**: <text>" lines), each opened on demand.
function Report({ ctx }) {
  const c = ctx.conversion?.conversion;
  const key = c?.sections ? `${ctx.id}:${c.finished ?? c.started}` : null;
  const [report, setReport] = useState(null);
  useEffect(() => {
    if (!key) { setReport(null); return; }
    text(`/analyses/${ctx.id}/conversion/report`).then(content => setReport(content)).catch(() => setReport(null));
  }, [key, ctx.id]);
  if (!report) return null;

  const sections = [];
  for (const line of report.split('\n')) {
    const heading = line.match(/^## (.+?)\((\d+) 件\)\s*$/);
    if (heading) sections.push({ title: reportTitle(heading[1]), count: +heading[2], items: [] });
    else if (line.startsWith('- ') && sections.length) sections.at(-1).items.push(line.slice(2));
  }
  const LIMIT = 300;
  const item = (s, i) => {
    const m = s.match(/^\*\*(.+?)\*\*: ?(.*)$/);
    return <li key={i}>{m ? <><b>{m[1]}</b>: {m[2]}</> : s}</li>;
  };
  const full = () => ctx.setSplit({
    header: 'CONVERSION-REPORT.md', description: t('report.of', ctx.entry.name),
    content: <Code lines={report.split('\n')} heading />,
  });

  return (
    <Container header={<Header variant="h2" description={t('report.hint')}
      actions={<Button onClick={full}>{t('report.full')}</Button>}>{t('report.title')}</Header>}>
      <SpaceBetween size="xs">
        {sections.map((s, i) => (
          <ExpandableSection key={s.title} variant="container" defaultExpanded={i === 0 && s.count > 0}
            headerText={<>{s.title} <Badge color={i === 0 && s.count > 0 ? 'red' : 'grey'}>{fmt(s.count)}</Badge></>}>
            {s.items.length
              ? <><ul style={{ margin: 0, paddingInlineStart: 20 }}>{s.items.slice(0, LIMIT).map(item)}</ul>
                  {s.items.length > LIMIT && <Muted>{t('report.more', fmt(s.items.length - LIMIT))}</Muted>}</>
              : <Muted>{t('report.none')}</Muted>}
          </ExpandableSection>
        ))}
      </SpaceBetween>
    </Container>
  );
}
