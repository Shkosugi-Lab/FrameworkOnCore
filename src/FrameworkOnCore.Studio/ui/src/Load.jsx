// ① Loading: what is known of the application without its analysis (the project's files: its language, its .NET
// Framework version, the projects it references, its packages, its pages; the repository's build), and the original
// application run on this machine (its test run), which needs no analysis either.
import { useEffect, useState } from 'react';
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import KeyValuePairs from '@cloudscape-design/components/key-value-pairs';
import Box from '@cloudscape-design/components/box';
import Alert from '@cloudscape-design/components/alert';
import Badge from '@cloudscape-design/components/badge';
import { api } from './api.js';
import { t, fmt, when } from './i18n.js';
import { Original } from './Runners.jsx';

const Code = ({ children }) => <Box variant="code" fontSize="body-s">{children}</Box>;

export default function Load({ ctx }) {
  const { entry, advice } = ctx;
  const [project, setProject] = useState(null);
  const [missing, setMissing] = useState(false);
  useEffect(() => {
    api(`/analyses/${ctx.id}/project`).then(setProject).catch(() => setMissing(true));
  }, [ctx.id]);

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
      <Container header={<Header variant="h2" description={t('load.buildHint')}>{t('load.build')}</Header>}>
        {!advice ? <Box color="text-body-secondary">{t('common.loading')}</Box>
          : advice.needed
            ? <Alert type="info" header={t('conversion.adviceNeeded')}><ul style={{ margin: 0, paddingInlineStart: 20 }}>{advice.reasons.map(r => <li key={r}>{r}</li>)}</ul></Alert>
            : <Box>{t('conversion.adviceNotNeeded')}</Box>}
      </Container>
      <Original ctx={ctx} />
    </SpaceBetween>
  );
}
