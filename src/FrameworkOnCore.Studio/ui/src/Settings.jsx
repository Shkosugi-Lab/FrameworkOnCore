// ④ The application's settings: choices that are no API's (file names' case on Linux, IIS's features, the language packs'
// languages: the languages chosen, "en,ja,de"; by default English and the original server's language packs').
import Container from '@cloudscape-design/components/container';
import Header from '@cloudscape-design/components/header';
import SpaceBetween from '@cloudscape-design/components/space-between';
import FormField from '@cloudscape-design/components/form-field';
import Tiles from '@cloudscape-design/components/tiles';
import Multiselect from '@cloudscape-design/components/multiselect';
import Badge from '@cloudscape-design/components/badge';
import Box from '@cloudscape-design/components/box';
import { t, lang } from './i18n.js';
import { defaultOf } from './choices.js';

/** The languages a setting's value names ("en,ja,de"); its default option: the languages it stands for (defaultLanguages). */
export const languagesOf = (setting, value) =>
  setting.options.some(o => o.id === value) ? (setting.defaultLanguages ?? ['en'])
    : String(value ?? '').split(/[,\s]+/).filter(Boolean);

const sameSet = (a, b) => a.length === b.length && a.every(x => b.some(y => x.toLowerCase() === y.toLowerCase()));

// The languages, one list: chosen, else the default (English and the original server's language packs'). Choosing the
// default's languages again is the default.
function LanguageSetting({ s, chosen, choose }) {
  const languages = languagesOf(s, chosen);
  const defaults = s.defaultLanguages ?? ['en'];
  const title = id => s.languages.find(l => l.id.toLowerCase() === id.toLowerCase())?.title ?? id;
  const option = l => ({ value: l.id, label: l.title, tags: [l.id],
    description: l.id === 'en' ? t('step3.languages.original') : l.translated ? t('step3.languages.translated') : t('step3.languages.untranslated') });
  return (
    <FormField description={<>{s.options[0]?.description} {t('step3.languages.default', defaults.map(title).join(lang === 'ja' ? '、' : ', '))}</>}
      errorText={languages.length === 0 ? t('step3.languages.empty') : undefined}>
      <Multiselect filteringType="auto" placeholder={t('step3.languages.placeholder')}
        selectedOptions={s.languages.filter(l => languages.some(c => c.toLowerCase() === l.id.toLowerCase())).map(option)}
        options={s.languages.map(option)}
        onChange={ev => {
          const ids = ev.detail.selectedOptions.map(o => o.value);
          choose(sameSet(ids, defaults) ? s.options[0].id : ids.join(','));
        }} />
    </FormField>
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
