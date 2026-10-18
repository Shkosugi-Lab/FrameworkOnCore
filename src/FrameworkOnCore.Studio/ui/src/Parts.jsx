// Small parts the steps share: an API's status, a log, lines of code (the split panel), a runner's state.
import { useEffect, useRef } from 'react';
import Box from '@cloudscape-design/components/box';
import StatusIndicator from '@cloudscape-design/components/status-indicator';
import ExpandableSection from '@cloudscape-design/components/expandable-section';
import * as tokens from '@cloudscape-design/design-tokens';
import { t, fmt } from './i18n.js';

// An API's status: Cloudscape's status types (never the color alone: the icon and the label too).
const STATUS_TYPE = { Missing: 'error', Throws: 'error', WindowsOnly: 'warning', Behavior: 'warning', Obsolete: 'info', Available: 'success' };
export const Status = ({ status }) => <StatusIndicator type={STATUS_TYPE[status] ?? 'info'}>{t(`status.${status}`)}</StatusIndicator>;

// A run's state (the original application, the native run, the container) and the conversion's.
const RUN_TYPE = { building: 'in-progress', starting: 'in-progress', running: 'success', stopped: 'stopped', failed: 'error', queued: 'pending', done: 'success', cancelled: 'stopped' };
export const RunState = ({ state, prefix = 'run' }) => state ? <StatusIndicator type={RUN_TYPE[state] ?? 'info'}>{t(`${prefix}.${state}`)}</StatusIndicator> : null;

const codeStyle = {
  margin: 0, maxHeight: 320, overflow: 'auto', padding: '8px 12px', whiteSpace: 'pre-wrap', wordBreak: 'break-all',
  fontFamily: tokens.fontFamilyMonospace, fontSize: 12, lineHeight: '18px',
  background: tokens.colorBackgroundContainerHeader, border: `1px solid ${tokens.colorBorderDividerDefault}`, borderRadius: 8,
};

/** Lines of a log, scrolled to their end. */
export function Log({ lines, height = 320 }) {
  const ref = useRef(null);
  useEffect(() => { if (ref.current) ref.current.scrollTop = ref.current.scrollHeight; }, [lines]);
  return <pre ref={ref} style={{ ...codeStyle, maxHeight: height }}>{lines?.length ? lines.join('\n') : '…'}</pre>;
}

/** A log that can be opened (open while something runs or failed). */
export const LogSection = ({ lines, open }) => lines?.length
  ? <ExpandableSection headerText={t('common.log', fmt(lines.length))} defaultExpanded={open}><Log lines={lines} /></ExpandableSection>
  : null;

/** Lines of a file in the split panel: numbered from start, the line hit marked. */
export function Code({ lines, start = 1, hit, heading }) {
  const ref = useRef(null);
  useEffect(() => { ref.current?.querySelector('[data-hit]')?.scrollIntoView({ block: 'center' }); }, [lines, hit]);
  return (
    <pre ref={ref} style={{ ...codeStyle, maxHeight: 'none', whiteSpace: 'pre' }}>
      {lines.map((text, i) => {
        const no = start + i;
        const marked = no === hit || (heading && text.startsWith('## '));
        return <div key={i} data-hit={no === hit ? '' : undefined}
          style={marked ? { background: tokens.colorBackgroundItemSelected, fontWeight: heading ? 700 : undefined } : undefined}>
          {hit !== undefined && <span style={{ display: 'inline-block', width: 48, opacity: 0.55, userSelect: 'none' }}>{no}</span>}{text}
        </div>;
      })}
    </pre>
  );
}

export const Muted = ({ children }) => <Box variant="small" color="text-body-secondary">{children}</Box>;
