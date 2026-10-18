// FrameworkOnCore Studio: the analyses (the side navigation), and one analysis as a wizard of five steps (Analysis.jsx).
// The AWS console's layout and parts (Cloudscape): the top navigation, the side navigation, the notifications (flash
// bar), the split panel (a source file, the conversion's report, a container's log). Everything comes from the HTTP API:
// the page holds no data of its own.
import { useCallback, useEffect, useRef, useState } from 'react';
import AppLayout from '@cloudscape-design/components/app-layout';
import TopNavigation from '@cloudscape-design/components/top-navigation';
import SideNavigation from '@cloudscape-design/components/side-navigation';
import Flashbar from '@cloudscape-design/components/flashbar';
import SplitPanel from '@cloudscape-design/components/split-panel';
import Modal from '@cloudscape-design/components/modal';
import Box from '@cloudscape-design/components/box';
import Button from '@cloudscape-design/components/button';
import SpaceBetween from '@cloudscape-design/components/space-between';
import StatusIndicator from '@cloudscape-design/components/status-indicator';
import { applyMode, Mode } from '@cloudscape-design/global-styles';
import { api } from './api.js';
import { t, fmt, when } from './i18n.js';
import Analysis from './Analysis.jsx';
import NewAnalysis from './NewAnalysis.jsx';
import Welcome from './Welcome.jsx';

const active = a => a.state === 'queued' || a.state === 'running';

// #/a/<analysis>, #/a/<analysis>/step/<n>, #/a/<analysis>/c/<component> (a component's link is of the third step).
function parseHash() {
  const m = location.hash.match(/^#\/a\/([^/]+)(?:\/step\/([1-6])|\/c\/([^/]+))?/);
  if (!m) return { id: null, step: 1, component: null };
  return { id: decodeURIComponent(m[1]), step: m[3] ? 3 : +(m[2] ?? 1), component: m[3] ? decodeURIComponent(m[3]) : null };
}

// An analysis' state in the side navigation: its icon (the name keeps the width), its words for a screen reader.
const STATE_TYPE = { done: 'success', failed: 'error', queued: 'pending' };
function analysisStatus(a) {
  const label = a.state === 'done' ? (a.summary ? t('nav.summary', fmt(a.summary.apis), fmt(a.summary.toDecide)) : t('state.done'))
    : t(`state.${a.state === 'running' ? 'analyzing' : a.state}`);
  return <span title={`${label} ・ ${when(a.created, { dateStyle: 'short', timeStyle: 'short' })}`}>
    <StatusIndicator type={STATE_TYPE[a.state] ?? 'in-progress'} iconAriaLabel={label} /></span>;
}

export default function App() {
  const [analyses, setAnalyses] = useState([]);
  const [loaded, setLoaded] = useState(false);
  const [route, setRoute] = useState(parseHash);
  const [flash, setFlash] = useState([]);
  const [split, setSplit] = useState(null);
  const [newOpen, setNewOpen] = useState(false);
  const [question, setQuestion] = useState(null);
  const [mode, setModeState] = useState(() => localStorage.getItem('foc-theme') ?? 'light');
  const dirty = useRef(false);
  const timer = useRef(null);

  useEffect(() => {
    applyMode(mode === 'dark' ? Mode.Dark : Mode.Light);
    localStorage.setItem('foc-theme', mode);
  }, [mode]);

  // A message at the top of the page: an error stays until dismissed, the others go by themselves.
  const notify = useCallback((type, content) => {
    const id = Math.random().toString(36).slice(2);
    const dismiss = () => setFlash(items => items.filter(i => i.id !== id));
    setFlash(items => [...items.slice(-2), { id, type, content, dismissible: true, onDismiss: dismiss, dismissLabel: t('common.close') }]);
    if (type !== 'error') setTimeout(dismiss, 4000);
  }, []);

  // A question to confirm (in place of the browser's confirm): resolves true or false.
  const confirm = useCallback((content, options = {}) =>
    new Promise(resolve => setQuestion({ content, ...options, resolve })), []);

  const loadAnalyses = useCallback(async () => {
    const list = await api('/analyses');
    setAnalyses(list);
    setLoaded(true);
    clearTimeout(timer.current);
    // While one waits or runs, the list is read again (its state changes whichever analysis is open).
    if (list.some(active)) timer.current = setTimeout(() => loadAnalyses().catch(console.error), 2000);
    return list;
  }, []);

  useEffect(() => {
    loadAnalyses().then(list => {
      if (!parseHash().id && list.length) location.hash = `#/a/${list[0].id}`;
    }).catch(() => notify('error', t('app.unreachable')));
    const onHash = () => setRoute(parseHash());
    addEventListener('hashchange', onHash);
    const onLeave = ev => { if (dirty.current) ev.preventDefault(); };
    addEventListener('beforeunload', onLeave);
    return () => { removeEventListener('hashchange', onHash); removeEventListener('beforeunload', onLeave); };
  }, [loadAnalyses, notify]);

  // To another analysis: unsaved choices are given up only when the user says so.
  const open = useCallback(async (id, step) => {
    if (id !== parseHash().id && dirty.current && !(await confirm(t('app.discardChanges')))) return;
    dirty.current = false;
    setSplit(null);
    location.hash = id ? `#/a/${id}${step && step > 1 ? `/step/${step}` : ''}` : '';
  }, [confirm]);

  async function removeFailed() {
    const failed = analyses.filter(a => a.state === 'failed');
    if (!failed.length || !(await confirm(t('app.deleteFailed', failed.length), { danger: true }))) return;
    const { deleted } = await api('/analyses?state=failed', { method: 'DELETE' });
    const list = await loadAnalyses();
    if (failed.some(a => a.id === route.id)) open(list[0]?.id ?? null);
    notify('success', t('app.deleted', deleted));
  }

  const current = analyses.find(a => a.id === route.id);
  const navItems = [
    ...analyses.map(a => ({
      type: 'link', text: a.name, href: `#/a/${a.id}`,
      info: analysisStatus(a),
    })),
  ];

  return (
    <>
      <div id="top-nav" style={{ position: 'sticky', top: 0, zIndex: 1002 }}>
        <TopNavigation
          identity={{ href: '#', title: 'FrameworkOnCore Studio', onFollow: ev => { ev.preventDefault(); open(null); } }}
          utilities={[
            { type: 'button', text: t('nav.new'), iconName: 'add-plus', onClick: () => setNewOpen(true) },
            ...(analyses.some(a => a.state === 'failed') ? [{ type: 'button', text: t('nav.deleteFailed'), iconName: 'remove', onClick: () => removeFailed().catch(e => notify('error', e.message)) }] : []),
            {
              type: 'menu-dropdown', iconName: 'settings', ariaLabel: t('nav.display'), title: t('nav.display'),
              items: [{ id: 'light', text: t('nav.light'), checked: mode === 'light' }, { id: 'dark', text: t('nav.dark'), checked: mode === 'dark' }],
              onItemClick: ev => setModeState(ev.detail.id),
            },
          ]}
          i18nStrings={{ overflowMenuTriggerText: t('nav.more'), overflowMenuTitleText: t('nav.all') }}
        />
      </div>
      <AppLayout
        headerSelector="#top-nav"
        toolsHide
        notifications={<Flashbar items={flash} />}
        navigation={
          <SideNavigation
            header={{ href: '#', text: t('nav.analyses') }}
            activeHref={route.id ? `#/a/${route.id}` : undefined}
            items={navItems.length ? navItems : [{ type: 'link', text: t('nav.none'), href: '#' }]}
            onFollow={ev => { ev.preventDefault(); if (ev.detail.href !== '#') open(ev.detail.href.slice(4)); }}
          />
        }
        splitPanelOpen={!!split}
        onSplitPanelToggle={ev => { if (!ev.detail.open) setSplit(null); }}
        splitPanelPreferences={{ position: 'side' }}
        splitPanel={split && (
          <SplitPanel header={split.header} closeBehavior="hide" hidePreferencesButton
            i18nStrings={{ closeButtonAriaLabel: t('common.close'), openButtonAriaLabel: t('common.open') }}>
            {split.description && <Box variant="small" margin={{ bottom: 's' }}>{split.description}</Box>}
            {split.content}
          </SplitPanel>
        )}
        content={
          !loaded ? null
            : current
              ? <Analysis key={current.id} listed={current} route={route} notify={notify} confirm={confirm} open={open}
                  setSplit={setSplit} dirty={dirty} loadAnalyses={loadAnalyses} />
              : <Welcome onNew={() => setNewOpen(true)} />
        }
        ariaLabels={{ navigation: t('nav.analyses'), navigationClose: t('common.close'), navigationToggle: t('nav.analyses') }}
      />
      <NewAnalysis visible={newOpen} onDismiss={() => setNewOpen(false)}
        onStarted={async entry => { setNewOpen(false); await loadAnalyses(); open(entry.id); }} />
      <Modal visible={!!question} onDismiss={() => { question?.resolve(false); setQuestion(null); }}
        header={question?.header ?? t('common.confirm')}
        footer={<Box float="right"><SpaceBetween direction="horizontal" size="xs">
          <Button variant="link" onClick={() => { question.resolve(false); setQuestion(null); }}>{t('common.cancel')}</Button>
          <Button variant="primary" onClick={() => { question.resolve(true); setQuestion(null); }}>{question?.confirmText ?? t('common.ok')}</Button>
        </SpaceBetween></Box>}>
        {question?.content}
      </Modal>
    </>
  );
}
