// One analysis: its header, and a wizard of six steps: ① loading (what the project's files say, the original application
// run here), ② the analysis (its progress; done, the numbers, the components and their APIs), ③ each component's option
// (the DLLs' references without their types among them), ④ the application's settings (③ and ④ saved through PUT
// /api/analyses/{id}/choices), ⑤ the conversion and its build (its report), ⑥ the deployment (the zip, the converted
// application run here or in a container). ③ to ⑥ open once the analysis is done.
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import Wizard from '@cloudscape-design/components/wizard';
import Header from '@cloudscape-design/components/header';
import Container from '@cloudscape-design/components/container';
import SpaceBetween from '@cloudscape-design/components/space-between';
import Button from '@cloudscape-design/components/button';
import Box from '@cloudscape-design/components/box';
import Alert from '@cloudscape-design/components/alert';
import ProgressBar from '@cloudscape-design/components/progress-bar';
import { api, post, remove } from './api.js';
import { t, when } from './i18n.js';
import { changes, clone, commandLine, empty, normalize, parseSteps, prepare } from './choices.js';
import Overview from './Overview.jsx';
import Load from './Load.jsx';
import { ComponentList } from './Components.jsx';
import Settings from './Settings.jsx';
import ChoicesBar from './ChoicesBar.jsx';
import Convert from './Convert.jsx';
import Deploy from './Deploy.jsx';
import { Log } from './Parts.jsx';

export default function Analysis({ listed, route, notify, confirm, open, setSplit, dirty, loadAnalyses }) {
  const id = listed.id;
  const [entry, setEntry] = useState(null);
  const [log, setLog] = useState([]);
  const [data, setData] = useState(null);           // { result, byComponent }
  const [choices, setChoices] = useState(null);
  const [saved, setSaved] = useState(null);
  const [errors, setErrors] = useState([]);
  const [command, setCommand] = useState(null);
  const [conversion, setConversion] = useState(null);
  const [advice, setAdvice] = useState(null);
  const [buildOriginalChoice, setBuildOriginalChoice] = useState(null);
  const [stepsTyped, setStepsTyped] = useState(null);  // the setup steps being written (shared by ① and ④)
  const [savedSteps, setSavedSteps] = useState([]);
  const [saving, setSaving] = useState(false);
  const poll = useRef(null);
  const step = route.step;

  // The analysis: while it waits or runs, asked again; done, its result, choices and command.
  const refresh = useCallback(async () => {
    clearTimeout(poll.current);
    const body = await api(`/analyses/${id}`);
    setEntry(body.entry);
    setLog(body.log ?? []);
    if (body.entry.state === 'done') {
      const [result, loadedChoices, cmd, catalog] = await Promise.all([api(`/analyses/${id}/result`), api(`/analyses/${id}/choices`), api(`/analyses/${id}/command`), api('/catalog')]);
      const byComponent = prepare(result, catalog);
      setData({ result, byComponent });
      const c = normalize(loadedChoices);
      setChoices(c);
      setSaved(clone(c));
      setCommand(cmd.command);
      loadAnalyses().catch(console.error);
    } else if (body.entry.state !== 'failed') poll.current = setTimeout(() => refresh().catch(console.error), 1500);
    else loadAnalyses().catch(console.error);
  }, [id, loadAnalyses]);

  useEffect(() => { refresh().catch(e => notify('error', e.message)); return () => clearTimeout(poll.current); }, [refresh, notify]);
  // Whether the site is made by the repository's own build: read from its files (① shows it, ⑤ starts from it).
  useEffect(() => { api(`/analyses/${id}/original-build-advice`).then(setAdvice).catch(() => {}); }, [id]);

  // The conversion: while it waits or runs, asked again (the steps show how far it is); the setup steps given last.
  const convPoll = useRef(null);
  const loadConversion = useCallback(async () => {
    clearTimeout(convPoll.current);
    const body = await api(`/analyses/${id}/conversion`);
    setConversion(body);
    setSavedSteps(body.originalSteps ?? []);
    const c = body.conversion;
    if (c && (c.state === 'queued' || c.state === 'running')) convPoll.current = setTimeout(() => loadConversion().catch(console.error), 2000);
    else if (c) loadAnalyses().catch(console.error);
  }, [id, loadAnalyses]);
  const done = entry?.state === 'done';
  useEffect(() => {
    if (done) loadConversion().catch(e => notify('error', e.message));
    return () => clearTimeout(convPoll.current);
  }, [done, loadConversion, notify]);
  // ⑥ (a link to it) without a built conversion: ⑤; ③ to ⑥ before the analysis is done: ②.
  useEffect(() => {
    if (conversion && step === 6 && conversion.conversion?.state !== 'done') open(id, 5);
  }, [conversion, step, id, open]);
  useEffect(() => {
    if (entry && entry.state !== 'done' && step > 2) open(id, 2);
  }, [entry, step, id, open]);

  const result = data?.result;
  const unsaved = changes(result, choices, saved);
  useEffect(() => { dirty.current = unsaved > 0; }, [unsaved, dirty]);

  const stepsText = stepsTyped ?? savedSteps.join('\n');
  const buildOriginal = buildOriginalChoice ?? (advice ? advice.needed : !!conversion?.conversion?.buildOriginal);
  const built = conversion?.conversion?.state === 'done';

  async function save() {
    setSaving(true);
    try {
      await api(`/analyses/${id}/choices`, { method: 'PUT', body: JSON.stringify(choices) });
      setSaved(clone(choices));
      setErrors([]);
      setCommand((await api(`/analyses/${id}/command`)).command);
      notify('success', t('choices.saved'));
      return true;
    } catch (e) {
      setErrors(e.body?.errors ?? [e.message]);
      return false;
    } finally {
      setSaving(false);
    }
  }

  async function del() {
    const running = entry && (entry.state === 'queued' || entry.state === 'running');
    if (!(await confirm(running ? t('analysis.cancelDelete', listed.name) : t('analysis.deleteQuestion', listed.name), { danger: true, confirmText: t('common.delete') }))) return;
    await remove(`/analyses/${id}`);
    dirty.current = false;
    const list = await loadAnalyses();
    open(list.find(a => a.id !== id)?.id ?? null);
    notify('success', running ? t('analysis.cancelledDeleted') : t('analysis.deleted'));
  }

  async function reanalyze() {
    const started = await post('/analyses', { project: entry.project, root: entry.root, configuration: entry.configuration, name: entry.name });
    await loadAnalyses();
    open(started.id);
  }

  const copyCommand = async conv => {
    await navigator.clipboard.writeText(conv ? commandLine(command, conv.buildOriginal, conv.originalSteps ?? []) : commandLine(command, buildOriginal, parseSteps(stepsText)));
    notify('success', t('command.copied'));
  };

  const header = (
    <Header variant="h1"
      description={entry && result
        ? <>{result.repository} / <code>{result.entry}</code> ・ {t('analysis.configuration', result.configuration)} ・ {when(result.analyzed)} ・ {t('analysis.catalog', result.catalogVersion)}</>
        : entry && <><code>{entry.project}</code> ・ {t('analysis.configuration', entry.configuration)}</>}
      actions={<SpaceBetween direction="horizontal" size="xs">
        {entry?.state === 'done' && <Button iconName="refresh" onClick={() => reanalyze().catch(e => notify('error', e.message))}>{t('analysis.reanalyze')}</Button>}
        <Button onClick={() => del().catch(e => notify('error', e.message))}>
          {entry && entry.state !== 'done' && entry.state !== 'failed' ? t('analysis.cancelAndDelete') : t('common.delete')}</Button>
      </SpaceBetween>}>
      {listed.name}
    </Header>
  );

  if (!entry) return <SpaceBetween size="l">{header}<ProgressBar status="in-progress" label={t('common.loading')} /></SpaceBetween>;

  const ready = entry.state === 'done' && data && choices;
  const ctx = {
    id, entry, result, byComponent: data?.byComponent, choices, setChoices, saved, notify, confirm, setSplit, step, advice,
    conversion, setConversion, buildOriginal, setBuildOriginalChoice, stepsText, setStepsTyped, setSavedSteps,
    command, copyCommand, unsaved, save, loadAnalyses, loadConversion, linked: route.component,
  };
  const bar = ready && <ChoicesBar ctx={ctx} errors={errors} saving={saving}
    onReset={() => { setChoices(empty()); setErrors([]); }} onDiscard={() => { setChoices(clone(saved)); setErrors([]); }} />;

  const go = async (n, reason) => {
    if (n === step) return;
    if (n > 2 && !ready) { notify('info', t('wizard.analyzeFirst')); return; }
    // Forward from ③ or ④ to the conversion, the choices are saved first (what is converted is what is saved).
    if ((step === 3 || step === 4) && n > 4 && unsaved > 0 && !(await save())) return;
    if (n === 6 && !built) { notify('info', t('wizard.buildFirst')); return; }
    open(id, n);
    if (reason !== 'step') scrollTo({ top: 0 });
  };

  const stepState = n => {
    const c = conversion?.conversion;
    if (n === 1) return t('state.done');
    if (n === 2) return entry.state === 'done' ? t('state.done') : entry.state === 'failed' ? t('state.failed') : entry.state === 'queued' ? t('state.queued') : t('state.analyzing');
    if (!ready) return t('wizard.afterAnalysis');
    if (n === 3 || n === 4) {
      const k = changes(result, choices, saved, n === 3 ? ['components', 'apis'] : ['settings']);
      return k > 0 ? t('wizard.unsaved', k) : t('wizard.saved');
    }
    if (n === 5) return c ? t(`conversion.${c.state}`) : t('wizard.notRun');
    return built ? t('wizard.ready') : t('wizard.afterBuild');
  };

  const failed = entry.state === 'failed';
  const running = (
    <Container header={<Header variant="h2" description={failed ? undefined : t('analysis.runningHint')}>
      {failed ? t('analysis.failed') : entry.state === 'queued' ? t('analysis.waiting') : t('analysis.running')}</Header>}>
      <SpaceBetween size="m">
        {failed ? <Alert type="error">{entry.error}</Alert> : <ProgressBar status="in-progress" />}
        <Log lines={log.slice(-60)} />
      </SpaceBetween>
    </Container>
  );
  const later = <Box color="text-body-secondary">{t('wizard.afterAnalysisHint')}</Box>;

  return (
    <SpaceBetween size="l">
      {header}
      <Wizard
        activeStepIndex={step - 1}
        allowSkipTo
        onNavigate={ev => go(ev.detail.requestedStepIndex + 1, ev.detail.reason).catch(e => notify('error', e.message))}
        onCancel={() => open(null)}
        onSubmit={() => { location.href = `/api/analyses/${id}/conversion/zip`; }}
        submitButtonText={t('wizard.download')}
        isLoadingNextStep={saving}
        i18nStrings={{
          stepNumberLabel: n => t('wizard.stepNumber', n),
          collapsedStepsLabel: (n, total) => t('wizard.collapsed', n, total),
          skipToButtonLabel: s => t('wizard.skipTo', s.title),
          navigationAriaLabel: t('wizard.steps'),
          cancelButton: t('common.close'),
          previousButton: t('wizard.previous'),
          nextButton: t('wizard.next'),
          optional: t('wizard.optional'),
        }}
        steps={STEPS.map((key, i) => ({
          title: t(`steps.${key}.title`), description: stepState(i + 1),
          content: key === 'load' ? <Load ctx={ctx} />
            : key === 'analysis' ? (ready ? <Overview ctx={ctx} /> : running)
              : !ready ? later
                : key === 'components' ? <SpaceBetween size="l">{bar}<ComponentList ctx={ctx} deciding /></SpaceBetween>
                  : key === 'settings' ? <SpaceBetween size="l">{bar}<Settings ctx={ctx} /></SpaceBetween>
                    : key === 'convert' ? <Convert ctx={ctx} onNext={() => go(6)} />
                      : <Deploy ctx={ctx} />,
        }))}
      />
    </SpaceBetween>
  );
}

export const STEPS = ['load', 'analysis', 'components', 'settings', 'convert', 'deploy'];
