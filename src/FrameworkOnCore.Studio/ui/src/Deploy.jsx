// ⑤ The deployment: the conversion's output as a zip (the site, the Dockerfile, the systemd scripts), the converted
// application run here (dotnet) or on Linux in Docker; publishing to Amazon ECR is planned.
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import ColumnLayout from '@cloudscape-design/components/column-layout';
import Button from '@cloudscape-design/components/button';
import Alert from '@cloudscape-design/components/alert';
import Badge from '@cloudscape-design/components/badge';
import Box from '@cloudscape-design/components/box';
import { t } from './i18n.js';
import { ContainerRun, Native } from './Runners.jsx';

export default function Deploy({ ctx }) {
  const c = ctx.conversion?.conversion;
  return (
    <SpaceBetween size="l">
      <Box variant="p">{t('step5.intro')}</Box>
      <ColumnLayout columns={2}>
        <Container header={<Header variant="h2" description={t('zip.description')}
          actions={<SpaceBetween direction="horizontal" size="xs">
            {ctx.command && <Button iconName="copy" onClick={() => ctx.copyCommand(c).catch(e => ctx.notify('error', e.message))}>{t('command.copy')}</Button>}
            {c?.state === 'done' && <Button variant="primary" iconName="download" href={`/api/analyses/${ctx.id}/conversion/zip`}>
              {t('zip.download', (c.zipSize / 1048576).toFixed(1))}</Button>}
          </SpaceBetween>}>{t('zip.title')}</Header>}>
          {ctx.conversion?.stale ? <Alert type="warning">{t('conversion.stale')}</Alert> : <Box color="text-body-secondary">{t('zip.readme')}</Box>}
        </Container>
        <Container header={<Header variant="h2" description={t('ecr.description')} info={<Badge>{t('common.planned')}</Badge>}
          actions={<Button disabled>{t('ecr.publish')}</Button>}>{t('ecr.title')}</Header>}>
          <Box color="text-body-secondary">{t('ecr.preparing')}</Box>
        </Container>
      </ColumnLayout>
      <Native ctx={ctx} />
      <ContainerRun ctx={ctx} />
    </SpaceBetween>
  );
}
