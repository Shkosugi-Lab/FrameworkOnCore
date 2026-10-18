// No analysis open: what Studio does, and the first step.
import ContentLayout from '@cloudscape-design/components/content-layout';
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import Button from '@cloudscape-design/components/button';
import Box from '@cloudscape-design/components/box';
import ColumnLayout from '@cloudscape-design/components/column-layout';
import { t } from './i18n.js';
import { STEPS } from './Analysis.jsx';

export default function Welcome({ onNew }) {
  return (
    <ContentLayout header={<Header variant="h1" description={t('welcome.description')}
      actions={<Button variant="primary" onClick={onNew}>{t('welcome.start')}</Button>}>{t('welcome.title')}</Header>}>
      <Container header={<Header variant="h2">{t('welcome.how')}</Header>}>
        <ColumnLayout columns={3} variant="text-grid">
          {STEPS.map((key, i) => (
            <SpaceBetween key={key} size="xxs">
              <Box variant="awsui-key-label">{t('wizard.stepNumber', i + 1)}</Box>
              <Box fontWeight="bold">{t(`steps.${key}.title`)}</Box>
              <Box variant="small" color="text-body-secondary">{t(`steps.${key}.sub`)}</Box>
            </SpaceBetween>
          ))}
        </ColumnLayout>
      </Container>
    </ContentLayout>
  );
}
