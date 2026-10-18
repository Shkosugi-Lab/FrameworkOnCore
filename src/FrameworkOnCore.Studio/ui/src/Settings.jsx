// ③ The application's settings: choices that are no API's (file names' case on Linux, IIS's features).
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import FormField from '@cloudscape-design/components/form-field';
import Tiles from '@cloudscape-design/components/tiles';
import Badge from '@cloudscape-design/components/badge';
import Box from '@cloudscape-design/components/box';
import { t } from './i18n.js';
import { defaultOf } from './choices.js';

export default function Settings({ ctx }) {
  const { result, choices, setChoices } = ctx;
  return (
    <SpaceBetween size="l">
      <Box variant="p">{t('step3.intro')}</Box>
      <Container header={<Header variant="h2">{t('step3.settings')}</Header>}>
        <SpaceBetween size="l">
          {result.settings.map(s => {
            const chosen = choices.settings[s.id] ?? defaultOf(s.options);
            return (
              <FormField key={s.id} stretch
                label={<>{s.title} <Box variant="code" display="inline" fontSize="body-s">{s.id}</Box> {chosen !== defaultOf(s.options) && <Badge color="severity-medium">{t('common.changed')}</Badge>}</>}>
                <Tiles value={chosen} columns={Math.min(s.options.length, 3)}
                  onChange={ev => setChoices({ ...choices, settings: { ...choices.settings, [s.id]: ev.detail.value } })}
                  items={s.options.map(o => ({ value: o.id, label: <>{o.title} {o.default && <Badge>{t('common.default')}</Badge>}</>, description: o.description, disabled: o.planned }))} />
              </FormField>
            );
          })}
        </SpaceBetween>
      </Container>
    </SpaceBetween>
  );
}
