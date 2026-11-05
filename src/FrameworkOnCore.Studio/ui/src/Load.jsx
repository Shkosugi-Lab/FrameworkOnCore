// ① Loading: what is known of the application without its analysis (the project's files: its language, its .NET
// Framework version, the projects it references, its packages, its pages; the repository's build), and the original
// application run on this machine (its test run), which needs no analysis either.
import { useCallback, useEffect, useState } from 'react';
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import KeyValuePairs from '@cloudscape-design/components/key-value-pairs';
import Box from '@cloudscape-design/components/box';
import Alert from '@cloudscape-design/components/alert';
import Badge from '@cloudscape-design/components/badge';
import Button from '@cloudscape-design/components/button';
import FormField from '@cloudscape-design/components/form-field';
import RadioGroup from '@cloudscape-design/components/radio-group';
import Textarea from '@cloudscape-design/components/textarea';
import ProgressBar from '@cloudscape-design/components/progress-bar';
import StatusIndicator from '@cloudscape-design/components/status-indicator';
import { api, post } from './api.js';
import { t, fmt, when } from './i18n.js';
import { parseSteps } from './choices.js';
import { Original, useRun } from './Runners.jsx';

const Code = ({ children }) => <Box variant="code" fontSize="body-s">{children}</Box>;

export default function Load({ ctx }) {
  const { entry, advice } = ctx;
  const [project, setProject] = useState(null);
  const [missing, setMissing] = useState(false);
  useEffect(() => {
    api(`/analyses/${ctx.id}/project`).then(setProject).catch(() => setMissing(true));
  }, [ctx.id]);
  // The original application's build and test run: one state (the build that succeeded last opens the steps after ①).
  const pick = useCallback(b => b?.original, []);
  const [body, load] = useRun(`/analyses/${ctx.id}/original`, pick, ctx.notify);
  useEffect(() => {
    if (!body) return;
    ctx.setSavedSteps(body.originalSteps ?? []);
    ctx.setOriginal(body);
  }, [body]); // eslint-disable-line react-hooks/exhaustive-deps

  const files = project?.files ?? [];
  return (
    <SpaceBetween size="l">
      <Box variant="p">{t('load.intro')}</Box>
      <Container header={<Header variant="h2" description={t('load.projectHint')}>{t('load.project')}</Header>}>
        {missing ? <Alert type="warning">{t('load.missing')}</Alert> : (
          <KeyValuePairs columns={3} items={[
            { label: t('load.name'), value: entry.name },
            { label: t('load.file'), value: <Code>{entry.project}</Code> },
            { label: t('load.root'), value: <Code>{entry.root}</Code> },
            { label: t('load.configuration'), value: entry.configuration },
            { label: t('load.language'), value: project ? project.language : '…' },
            { label: t('load.framework'), value: project ? (project.targetFramework ?? '—') : '…' },
            { label: t('load.style'), value: project ? (project.sdkStyle ? t('load.sdkStyle') : t('load.oldStyle')) : '…' },
            { label: t('load.solution'), value: project ? (project.solution ? <Code>{project.solution}</Code> : t('load.noSolution')) : '…' },
            { label: 'web.config', value: project ? (project.webConfig ? t('load.yes') : t('load.no')) : '…' },
            { label: t('load.packages'), value: project ? fmt(project.packages) : '…' },
            {
              label: t('load.references'), value: project
                ? (project.projectReferences.length ? <SpaceBetween direction="horizontal" size="xxs">{project.projectReferences.map(r => <Badge key={r}>{r}</Badge>)}</SpaceBetween> : t('load.none'))
                : '…',
            },
            {
              label: t('load.pages'), value: project
                ? (files.length ? <SpaceBetween direction="horizontal" size="xs">{files.map(f => <span key={f.extension}>{f.extension} <b>{fmt(f.count)}</b></span>)}</SpaceBetween> : t('load.none'))
                : '…',
            },
            { label: t('load.created'), value: when(entry.created) },
          ]} />
        )}
      </Container>
      <OriginalBuild ctx={ctx} body={body} load={load} />
      <Original ctx={ctx} body={body} load={load} />
    </SpaceBetween>
  );
}

/**
 * The original application's build (its repository's build, the MSBuild chosen when both Visual Studio and its Build
 * Tools are installed, the setup steps after it): the steps after ① open once it has succeeded, and take the MSBuild of
 * the build that succeeded last.
 */
function OriginalBuild({ ctx, body, load }) {
  const { advice, lastBuild } = ctx;
  const k = body?.original?.state;
  const building = k === 'building';
  const build = () => post(`/analyses/${ctx.id}/original/build`, { originalSteps: parseSteps(ctx.stepsText), msbuild: ctx.msbuild })
    .then(() => { ctx.setStepsTyped(null); return load(); }).catch(e => ctx.notify('error', e.message));
  return (
    <Container header={<Header variant="h2" description={t('load.buildHint')}
      actions={<Button variant={lastBuild ? 'normal' : 'primary'} iconName={lastBuild ? 'refresh' : undefined} disabled={building || !body}
        onClick={build}>{lastBuild ? t('build.again') : t('build.start')}</Button>}>{t('load.build')}</Header>}>
      <SpaceBetween size="m">
        {!advice ? <Box color="text-body-secondary">{t('common.loading')}</Box>
          : advice.needed
            ? <Alert type="info" header={t('conversion.adviceNeeded')}><ul style={{ margin: 0, paddingInlineStart: 20 }}>{advice.reasons.map(r => <li key={r}>{r}</li>)}</ul></Alert>
            : <Box>{t('conversion.adviceNotNeeded')}</Box>}
        {ctx.msbuildBoth && <FormField label={t('build.msbuild')} description={t('build.msbuildHint')} stretch>
          <RadioGroup value={ctx.msbuild} onChange={ev => ctx.setMSBuild(ev.detail.value)}
            items={ctx.msbuildInstances.map(i => ({ value: i.installationPath, label: `${i.displayName} ${i.version}`, description: i.installationPath, disabled: building }))} />
        </FormField>}
        <FormField label={t('steps.label')} description={t('original.stepsHint')} stretch>
          <Textarea value={ctx.stepsText} onChange={ev => ctx.setStepsTyped(ev.detail.value)} rows={2} spellcheck={false} disabled={building} />
        </FormField>
        {building
          ? <ProgressBar status="in-progress" label={t('original.building')} />
          : lastBuild
            ? <StatusIndicator type="success">{t('build.done', when(lastBuild.built), ctx.msbuildBoth ? ctx.msbuildName(lastBuild.msBuild) : null)}</StatusIndicator>
            : <Alert type="info">{t('build.needed')}</Alert>}
        {k === 'failed' && <Alert type="error" header={t('build.failed')}>{body.original.error}</Alert>}
      </SpaceBetween>
    </Container>
  );
}
