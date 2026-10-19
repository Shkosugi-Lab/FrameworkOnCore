// ④ The application's settings: choices that are no API's (file names' case on Linux, IIS's features, the language packs'
// languages: one of its options, or the languages chosen, "ja,de").
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import FormField from '@cloudscape-design/components/form-field';
import Tiles from '@cloudscape-design/components/tiles';
import Multiselect from '@cloudscape-design/components/multiselect';
import Badge from '@cloudscape-design/components/badge';
import Box from '@cloudscape-design/components/box';
import { t } from './i18n.js';
import { defaultOf } from './choices.js';

const CHOSEN_LANGUAGES = '__languages';

/** The languages a setting's value names ("ja,de"), or null when it is one of its options. */
export const languagesOf = (setting, value) =>
  !setting.languages?.length || setting.options.some(o => o.id === value) ? null
    : String(value ?? '').split(/[,\s]+/).filter(Boolean);

function LanguageSetting({ s, chosen, choose }) {
  const languages = languagesOf(s, chosen);
  const items = s.options.map(o => ({ value: o.id, label: <>{o.title} {o.default && <Badge>{t('common.default')}</Badge>}</>, description: o.description, disabled: o.planned }))
    .concat([{ value: CHOSEN_LANGUAGES, label: t('step3.languages.choose'), description: t('step3.languages.chooseDescription') }]);
  const option = l => ({ value: l.id, label: l.title, tags: [l.id], description: l.translated ? t('step3.languages.translated') : t('step3.languages.untranslated') });
  return (
    <SpaceBetween size="s">
      <Tiles value={languages ? CHOSEN_LANGUAGES : chosen} columns={3}
        onChange={ev => choose(ev.detail.value === CHOSEN_LANGUAGES
          // The languages FrameworkOnCore has translations in, to start with.
          ? (s.languages.filter(l => l.translated).map(l => l.id).join(',') || s.languages[0].id)
          : ev.detail.value)}
        items={items} />
      {languages && (
        <FormField label={t('step3.languages.label')} description={t('step3.languages.hint')}
          errorText={languages.length === 0 ? t('step3.languages.empty') : undefined}>
          <Multiselect filteringType="auto" placeholder={t('step3.languages.placeholder')}
            selectedOptions={s.languages.filter(l => languages.some(c => c.toLowerCase() === l.id.toLowerCase())).map(option)}
            options={s.languages.map(option)}
            onChange={ev => choose(ev.detail.selectedOptions.map(o => o.value).join(','))} />
        </FormField>
      )}
    </SpaceBetween>
  );
}

export default function Settings({ ctx }) {
  const { result, choices, setChoices } = ctx;
  return (
    <SpaceBetween size="l">
      <Box variant="p">{t('step3.intro')}</Box>
      <Container header={<Header variant="h2">{t('step3.settings')}</Header>}>
        <SpaceBetween size="l">
          {result.settings.map(s => {
            const chosen = choices.settings[s.id] ?? defaultOf(s.options);
            const choose = value => setChoices({ ...choices, settings: { ...choices.settings, [s.id]: value } });
            return (
              <FormField key={s.id} stretch
                label={<>{s.title} <Box variant="code" display="inline" fontSize="body-s">{s.id}</Box> {chosen !== defaultOf(s.options) && <Badge color="severity-medium">{t('common.changed')}</Badge>}</>}>
                {s.languages?.length
                  ? <LanguageSetting s={s} chosen={chosen} choose={choose} />
                  : <Tiles value={chosen} columns={Math.min(s.options.length, 3)}
                      onChange={ev => choose(ev.detail.value)}
                      items={s.options.map(o => ({ value: o.id, label: <>{o.title} {o.default && <Badge>{t('common.default')}</Badge>}</>, description: o.description, disabled: o.planned }))} />}
              </FormField>
            );
          })}
        </SpaceBetween>
      </Container>
    </SpaceBetween>
  );
}
