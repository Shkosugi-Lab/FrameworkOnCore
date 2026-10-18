// Running the applications: the original one on this machine (① its test run: IIS Express or IIS), the converted one
// natively (⑤ dotnet) or on Linux in Docker (⑤ the conversion's Dockerfile). Studio stops what it started when it stops.
import { useCallback, useEffect, useRef, useState } from 'react';
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import Button from '@cloudscape-design/components/button';
import FormField from '@cloudscape-design/components/form-field';
import Textarea from '@cloudscape-design/components/textarea';
import Alert from '@cloudscape-design/components/alert';
import Box from '@cloudscape-design/components/box';
import ProgressBar from '@cloudscape-design/components/progress-bar';
import KeyValuePairs from '@cloudscape-design/components/key-value-pairs';
import Link from '@cloudscape-design/components/link';
import { api, post, remove, text } from './api.js';
import { t, duration } from './i18n.js';
import { parseSteps } from './choices.js';
import { Code, LogSection, RunState } from './Parts.jsx';

const HOST = { iisexpress: 'IIS Express', iis: 'IIS', dotnet: 'dotnet' };
const busy = k => k === 'building' || k === 'starting';

/** A run's state from the API, asked again while it builds or starts (2 s) and while it runs (10 s). */
function useRun(path, pick, notify) {
  const [body, setBody] = useState(null);
  const timer = useRef(null);
  const load = useCallback(async () => {
    clearTimeout(timer.current);
    const b = await api(path);
    setBody(b);
    const k = pick(b)?.state;
    if (busy(k)) timer.current = setTimeout(() => load().catch(console.error), 2000);
    else if (k === 'running') timer.current = setTimeout(() => load().catch(console.error), 10000);
    return b;
  }, [path, pick]);
  useEffect(() => { load().catch(e => notify('error', e.message)); return () => clearTimeout(timer.current); }, [load, notify]);
  return [body, load];
}

/** One panel: what it runs, how far it is, its log; idle: the explanation and the fields and buttons to start. */
function RunnerPanel({ title, description, entry, log, busyText, onStop, idle, actions, runningActions }) {
  const k = entry?.state;
  const running = k === 'running';
  return (
    <Container header={<Header variant="h2" description={description} info={<RunState state={k} />}
      actions={<SpaceBetween direction="horizontal" size="xs">
        {running && runningActions}
        {running && <Button variant="primary" iconName="external" iconAlign="right" href={entry.url} target="_blank">{t('run.open')}</Button>}
        {(busy(k) || running) && <Button onClick={onStop}>{busy(k) ? t('common.cancel') : t('run.stop')}</Button>}
        {!busy(k) && !running && actions}
      </SpaceBetween>}>{title}</Header>}>
      <SpaceBetween size="m">
        {busy(k) && <ProgressBar status="in-progress" label={busyText(entry)}
          description={`${t('run.elapsed', duration(entry.started))}${k === 'starting' ? ` ・ ${t('run.firstSlow')}` : ''}`} />}
        {running && <>
          <KeyValuePairs columns={3} items={[
            { label: 'URL', value: <Link href={entry.url} external>{entry.url}</Link> },
            { label: t('run.host'), value: HOST[entry.host] ?? entry.host ?? '—' },
            { label: t('run.firstStatus'), value: String(entry.firstStatus ?? '—') },
          ]} />
          {entry.firstStatus >= 500 && <Alert type="warning">{t('run.failing')}</Alert>}
        </>}
        {!busy(k) && !running && <>
          {entry?.error && <Alert type="error">{entry.error}</Alert>}
          {idle}
        </>}
        <LogSection lines={log} open={busy(k) || k === 'failed'} />
      </SpaceBetween>
    </Container>
  );
}

const EnvField = ({ value, onChange, hint }) => (
  <FormField label={t('run.environment')} description={t('run.environmentHint')} constraintText={hint} stretch>
    <Textarea value={value} onChange={ev => onChange(ev.detail.value)} rows={5} spellcheck={false} />
  </FormField>
);

/** ① The original application (.NET Framework) run here: built as its repository builds it, run by IIS Express or IIS. */
export function Original({ ctx }) {
  const pick = useCallback(b => b?.original, []);
  const [body, load] = useRun(`/analyses/${ctx.id}/original`, pick, ctx.notify);
  const [env, setEnv] = useState(null);
  useEffect(() => { if (body) ctx.setSavedSteps(body.originalSteps ?? []); }, [body]); // eslint-disable-line react-hooks/exhaustive-deps
  if (!body) return null;
  const { original, built, environment, host, reason, log } = body;
  const start = rebuild => post(`/analyses/${ctx.id}/original`, { rebuild, environment: env ?? environment, originalSteps: parseSteps(ctx.stepsText) })
    .then(() => { setEnv(null); ctx.setStepsTyped(null); return load(); }).catch(e => ctx.notify('error', e.message));
  return (
    <RunnerPanel title={t('original.title')} description={t('original.description')} entry={original} log={log}
      busyText={e => e.state === 'building' ? t('original.building') : t('run.waiting', HOST[e.host] ?? '', e.url ?? '')}
      onStop={() => remove(`/analyses/${ctx.id}/original`).catch(e => ctx.notify('error', e.message)).then(load)}
      actions={host && <>
        {built && <Button iconName="refresh" onClick={() => start(true)}>{t('original.rebuild')}</Button>}
        <Button variant="primary" onClick={() => start(false)}>{built ? t('run.start') : t('original.buildAndStart')}</Button>
      </>}
      idle={!host
        ? <Alert type="warning">{reason}</Alert>
        : <SpaceBetween size="m">
            <Box variant="p" color="text-body-secondary">{t('original.how', HOST[host])}</Box>
            <EnvField value={env ?? environment ?? ''} onChange={setEnv} hint={t('original.environmentWebConfig')} />
            <FormField label={t('steps.label')} description={t('original.stepsHint')} stretch>
              <Textarea value={ctx.stepsText} onChange={ev => ctx.setStepsTyped(ev.detail.value)} rows={2} spellcheck={false} />
            </FormField>
          </SpaceBetween>} />
  );
}

/** ⑤ The converted application run here by the .NET 10 runtime (dotnet), without Docker. */
export function Native({ ctx }) {
  const pick = useCallback(b => b?.native, []);
  const [body, load] = useRun(`/analyses/${ctx.id}/native`, pick, ctx.notify);
  const [env, setEnv] = useState(null);
  if (!body) return null;
  const { native, environment, log } = body;
  return (
    <RunnerPanel title={t('native.title')} description={t('native.description')} entry={native} log={log}
      busyText={e => t('run.waiting', 'dotnet', e.url ?? '')}
      onStop={() => remove(`/analyses/${ctx.id}/native`).catch(e => ctx.notify('error', e.message)).then(load)}
      actions={<Button variant="primary" onClick={() => post(`/analyses/${ctx.id}/native`, { environment: env ?? environment })
        .then(() => { setEnv(null); return load(); }).catch(e => ctx.notify('error', e.message))}>{t('run.start')}</Button>}
      idle={<EnvField value={env ?? environment ?? ''} onChange={setEnv} hint={t('native.environmentHint')} />} />
  );
}

/** ⑤ The converted application on Linux in Docker: the conversion's Dockerfile built, run on a port of localhost. */
export function ContainerRun({ ctx }) {
  const pick = useCallback(b => b?.container, []);
  const [body, load] = useRun(`/analyses/${ctx.id}/container`, pick, ctx.notify);
  const [docker, setDocker] = useState(null);
  const [starting, setStarting] = useState(false);
  const [env, setEnv] = useState(null);
  const timer = useRef(null);

  // Docker's engine: while it does not answer (Docker Desktop starting), asked again.
  const loadDocker = useCallback(async () => {
    clearTimeout(timer.current);
    const d = await api('/docker');
    setDocker(d);
    if (d.available) setStarting(false);
    else timer.current = setTimeout(() => loadDocker().catch(console.error), 3000);
  }, []);
  useEffect(() => { loadDocker().catch(console.error); return () => clearTimeout(timer.current); }, [loadDocker]);

  if (!body) return null;
  const { container, environment, log } = body;
  const showLog = async () => {
    const content = await text(`/analyses/${ctx.id}/container/log`);
    ctx.setSplit({ header: t('container.logTitle'), description: container?.name, content: <Code lines={(content || t('container.noOutput')).split('\n')} /> });
  };
  return (
    <RunnerPanel title={t('container.title')} entry={container} log={log}
      description={docker?.available ? t('container.description', docker.version) : undefined}
      busyText={e => e.state === 'building' ? t('container.building') : t('run.waiting', 'Docker', e.url ?? '')}
      onStop={async () => {
        if (!(await ctx.confirm(t('container.stopQuestion')))) return;
        await remove(`/analyses/${ctx.id}/container`).catch(e => ctx.notify('error', e.message));
        await load();
        await loadDocker();
      }}
      runningActions={<Button onClick={() => showLog().catch(e => ctx.notify('error', e.message))}>{t('container.log')}</Button>}
      actions={<>
        <Button variant="primary" disabled={!docker?.available}
          onClick={() => post(`/analyses/${ctx.id}/container`, { environment: env ?? environment }).then(() => { setEnv(null); return load(); }).catch(e => ctx.notify('error', e.message))}>
          {t('container.buildAndStart')}</Button>
      </>}
      idle={<SpaceBetween size="m">
        {!docker ? <Box color="text-body-secondary">{t('container.checking')}</Box>
          : !docker.available && <Alert type="warning"
              action={docker.desktop && <Button disabled={starting} onClick={() => post('/docker/start').then(() => { setStarting(true); return loadDocker(); }).catch(e => ctx.notify('error', e.message))}>
                {starting ? t('container.waitingDesktop') : t('container.startDesktop')}</Button>}>
              {docker.desktop ? t('container.startIt') : t('container.install')}{docker.reason && <Box variant="small" color="text-body-secondary">{docker.reason}</Box>}
            </Alert>}
        <EnvField value={env ?? environment ?? ''} onChange={setEnv} hint={t('container.environmentHint')} />
      </SpaceBetween>} />
  );
}
