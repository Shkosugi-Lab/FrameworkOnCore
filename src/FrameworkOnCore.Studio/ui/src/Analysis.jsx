// One analysis: its header, and a wizard of five steps: ① loading (what the project's files say, the original application
// run here), ② the analysis and what to do with it (its progress; done, the numbers, what the conversion leaves to the
// developers, each component's option with the DLLs' references among them, every API), ③ the application's settings
// (② and ③ saved through PUT /api/analyses/{id}/choices), ④ the conversion and its build (its report), ⑤ the deployment
// (the zip, the converted application run here or in a container). ③ to ⑤ open once the analysis is done.
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
import { changes, clone, commandLine, normalize, parseSteps, prepare } from './choices.js';
import Overview from './Overview.jsx';
import Load from './Load.jsx';
import Settings from './Settings.jsx';
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
  // The Visual Studio and Build Tools installed (null: not known yet): ①'s build runs the kind chosen there.
  const [msbuildInstances, setMSBuildInstances] = useState(null);
  const [msbuildTyped, setMSBuildTyped] = useState(null);
  useEffect(() => { api('/msbuild').then(setMSBuildInstances).catch(() => setMSBuildInstances([])); }, []);
  // ①'s build and test run (its build that succeeded last opens the steps after ①): ① keeps it up to date.
  const [original, setOriginal] = useState(null);
  useEffect(() => {
    setOriginal(null);
    setMSBuildTyped(null);
    api(`/analyses/${id}/original`).then(setOriginal).catch(() => {});
  }, [id]);
  const lastBuild = original?.lastBuild ?? null;

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
  // ⑤ (a link to it) without a built conversion: ④; ③ to ⑤ before the analysis is done: ②.
  useEffect(() => {
    if (conversion && step === DEPLOY && conversion.conversion?.state !== 'done') open(id, CONVERT);
  }, [conversion, step, id, open]);
  useEffect(() => {
    if (entry && entry.state !== 'done' && step > 2) open(id, 2);
    // A step after ① (a link to it) before ①'s build has succeeded: ①.
    if (original && !original.lastBuild && step > 1) open(id, 1);
  }, [entry, original, step, id, open]);

  const result = data?.result;
  const unsaved = changes(result, choices, saved);
  useEffect(() => { dirty.current = unsaved > 0; }, [unsaved, dirty]);
  const saveRef = useRef(null);
  const saveFailed = useRef(null);
  saveRef.current = save;
  useEffect(() => {
    // Not again for choices that could not be saved: once they change.
    if (!unsaved || saving || choices === saveFailed.current) return undefined;
    const timer = setTimeout(() => saveRef.current().catch(console.error), 600);
    return () => clearTimeout(timer);
  }, [choices, unsaved, saving]);

  const stepsText = stepsTyped ?? savedSteps.join('\n');
  const built = conversion?.conversion?.state === 'done';
  // ①'s choice, a kind (visualstudio or buildtools: the converter's --msbuild takes its newest): the one chosen (for its
  // next build), else the last build's, else Visual Studio when it is installed. One chosen before as an installation's
  // folder is taken as its kind. The steps after ① take the last successful build's.
  const instances = msbuildInstances ?? [];
  const msbuildKind = v => v === 'visualstudio' || v === 'buildtools' ? v : instances.find(i => i.installationPath === v)?.kind ?? null;
  const installed = kind => instances.some(i => i.kind === kind);
  const msbuild = [msbuildTyped, original?.msbuild, lastBuild?.msBuild].map(msbuildKind).find(k => k && installed(k))
    ?? (installed('visualstudio') ? 'visualstudio' : installed('buildtools') ? 'buildtools' : null);
  const msbuildName = value => {
    const kind = msbuildKind(value);
    const i = instances.find(m => m.kind === kind);  // the newest first
    return i ? `${i.displayName} ${i.version}` : value ?? '';
  };

  // The choices are saved as they change (a moment after the last change); going on to the conversion saves what is not yet.
  async function save() {
    setSaving(true);
    try {
      await api(`/analyses/${id}/choices`, { method: 'PUT', body: JSON.stringify(choices) });
      setSaved(clone(choices));
      setErrors([]);
      setCommand((await api(`/analyses/${id}/command`)).command);
      return true;
    } catch (e) {
      setErrors(e.body?.errors ?? [e.message]);
      saveFailed.current = choices;
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

  // A new analysis of the same project (② only: what it changes is the analysis).
  async function reanalyze() {
    const started = await post('/analyses', { project: entry.project, root: entry.root, configuration: entry.configuration, name: entry.name });
    await loadAnalyses();
    open(started.id);
  }

  const copyCommand = async conv => {
    await navigator.clipboard.writeText(conv ? commandLine(command, conv.buildOriginal, conv.originalSteps ?? [], conversion?.msbuild) : commandLine(command, true, savedSteps, lastBuild?.msBuild));
    notify('success', t('command.copied'));
  };

  const header = (
    <Header variant="h1"
      description={entry && result
        ? <>{result.repository} / <code>{result.entry}</code> ・ {t('analysis.configuration', result.configuration)} ・ {when(result.analyzed)} ・ {t('analysis.catalog', result.catalogVersion)}</>
        : entry && <><code>{entry.project}</code> ・ {t('analysis.configuration', entry.configuration)}</>}
      actions={<SpaceBetween direction="horizontal" size="xs">
        <Button onClick={() => del().catch(e => notify('error', e.message))}>
          {entry && entry.state !== 'done' && entry.state !== 'failed' ? t('analysis.cancelAndDelete') : t('common.delete')}</Button>
      </SpaceBetween>}>
      {listed.name}
    </Header>
  );

  if (!entry) return <SpaceBetween size="l">{header}<ProgressBar status="in-progress" label={t('common.loading')} /></SpaceBetween>;

  const ready = entry.state === 'done' && data && choices;
  const ctx = {
    id, entry, result, byComponent: data?.byComponent, choices, setChoices, saved, notify, confirm, setSplit, step,
    conversion, setConversion, stepsText, setStepsTyped, setSavedSteps,
    msbuildInstances, msbuild, setMSBuild: setMSBuildTyped, msbuildName, lastBuild, setOriginal,
    command, copyCommand, unsaved, save, loadAnalyses, loadConversion, linked: route.component,
  };
  const reanalyzeButton = <Button iconName="refresh" onClick={() => reanalyze().catch(e => notify('error', e.message))}>{t('analysis.reanalyze')}</Button>;
  // Saved by itself; what cannot be saved is told here (the choices stay as they are, to be changed).
  const bar = ready && errors.length > 0 && <Alert type="error" header={t('choices.cannotSave')}>{errors.join(' / ')}</Alert>;

  const go = async (n, reason) => {
    if (n === step) return;
    // The steps after ① open once its build has succeeded.
    if (n > 1 && !lastBuild) { notify('info', t('wizard.originalBuildFirst')); return; }
    if (n > 2 && !ready) { notify('info', t('wizard.analyzeFirst')); return; }
    // Forward from ② or ③ to the conversion, the choices are saved first (what is converted is what is saved).
    if ((step === 2 || step === SETTINGS) && n >= CONVERT && unsaved > 0 && !(await save())) return;
    if (n === DEPLOY && !built) { notify('info', t('wizard.buildFirst')); return; }
    open(id, n);
    if (reason !== 'step') scrollTo({ top: 0 });
  };

  const stepState = n => {
    const c = conversion?.conversion;
    if (n === 1) return lastBuild ? t('state.done') : t('wizard.buildHere');
    if (!lastBuild) return t('wizard.afterOriginalBuild');
    if (n === 2 && entry.state !== 'done') return entry.state === 'failed' ? t('state.failed') : entry.state === 'queued' ? t('state.queued') : t('state.analyzing');
    if (!ready) return n === 2 ? t('state.done') : t('wizard.afterAnalysis');
    if (n === 2 || n === SETTINGS) {
      const k = changes(result, choices, saved, n === 2 ? ['components', 'apis'] : ['settings']);
      return k > 0 ? t('wizard.unsaved', k) : n === 2 ? t('state.done') : t('wizard.saved');
    }
    if (n === CONVERT) return c ? t(`conversion.${c.state}`) : t('wizard.notRun');
    return built ? t('wizard.ready') : t('wizard.afterBuild');
  };

  const failed = entry.state === 'failed';
  const running = (
    <Container header={<Header variant="h2" description={failed ? undefined : t('analysis.runningHint')} actions={failed && reanalyzeButton}>
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
            : key === 'analysis' ? (ready ? <Overview ctx={ctx} actions={reanalyzeButton} bar={bar} /> : running)
              : !ready ? later
                : key === 'settings' ? <SpaceBetween size="l">{bar}<Settings ctx={ctx} /></SpaceBetween>
                  : key === 'convert' ? <Convert ctx={ctx} onNext={() => go(DEPLOY)} />
                    : <Deploy ctx={ctx} />,
        }))}
      />
    </SpaceBetween>
  );
}

export const STEPS = ['load', 'analysis', 'settings', 'convert', 'deploy'];
const [SETTINGS, CONVERT, DEPLOY] = ['settings', 'convert', 'deploy'].map(k => STEPS.indexOf(k) + 1);
