// The choices' state in ② (the analysis) and ③ (the settings): unsaved changes, how many differ from the defaults, and the actions (back to the
// defaults, discard, save; once saved, the conversion's command to copy).
import Alert from '@cloudscape-design/components/alert';
import Button from '@cloudscape-design/components/button';
import SpaceBetween from '@cloudscape-design/components/space-between';
import { t } from './i18n.js';
import { effective } from './choices.js';

export default function ChoicesBar({ ctx, errors, saving, onReset, onDiscard }) {
  const { result, choices, unsaved } = ctx;
  const e = effective(result, choices);
  const nonDefault = Object.keys(e.components).length + Object.keys(e.apis).length + Object.keys(e.settings).length;
  return (
    <Alert
      type={errors.length ? 'error' : unsaved > 0 ? 'warning' : 'info'}
      header={errors.length ? t('choices.cannotSave') : unsaved > 0 ? t('choices.unsaved', unsaved) : t('choices.savedState')}
      action={<SpaceBetween direction="horizontal" size="xs">
        <Button onClick={onReset}>{t('choices.reset')}</Button>
        <Button onClick={onDiscard} disabled={unsaved === 0}>{t('choices.discard')}</Button>
        {unsaved === 0 && ctx.command && <Button iconName="copy" onClick={() => ctx.copyCommand().catch(err => ctx.notify('error', err.message))}>{t('command.copy')}</Button>}
        <Button variant="primary" loading={saving} onClick={() => ctx.save()}>{unsaved > 0 ? t('choices.save') : t('choices.saveAsIs')}</Button>
      </SpaceBetween>}>
      {errors.length ? errors.join(' / ') : t('choices.nonDefault', nonDefault)}
    </Alert>
  );
}
