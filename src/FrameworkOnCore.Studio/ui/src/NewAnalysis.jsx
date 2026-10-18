// A new analysis: the web project (.csproj / .vbproj), its repository's folder (by default the topmost one with a .sln),
// the configuration and a name. Projects and folders are chosen through the server, which lists them (a page gets no
// path from the browser's own dialog).
import { useEffect, useState } from 'react';
import Modal from '@cloudscape-design/components/modal';
import Form from '@cloudscape-design/components/form';
import FormField from '@cloudscape-design/components/form-field';
import Input from '@cloudscape-design/components/input';
import Button from '@cloudscape-design/components/button';
import SpaceBetween from '@cloudscape-design/components/space-between';
import ColumnLayout from '@cloudscape-design/components/column-layout';
import Box from '@cloudscape-design/components/box';
import BreadcrumbGroup from '@cloudscape-design/components/breadcrumb-group';
import TextFilter from '@cloudscape-design/components/text-filter';
import Table from '@cloudscape-design/components/table';
import Link from '@cloudscape-design/components/link';
import Badge from '@cloudscape-design/components/badge';
import Icon from '@cloudscape-design/components/icon';
import Grid from '@cloudscape-design/components/grid';
import { api, post } from './api.js';
import { t } from './i18n.js';

export default function NewAnalysis({ visible, onDismiss, onStarted }) {
  const [form, setForm] = useState({ project: '', root: '', configuration: 'Debug', name: '' });
  const [rootHint, setRootHint] = useState('C:\\src\\MyApp');
  const [error, setError] = useState(null);
  const [busy, setBusy] = useState(false);
  const [browsing, setBrowsing] = useState(null);   // { mode, start }
  const set = (k, v) => setForm(f => ({ ...f, [k]: v }));

  // The repository folder the analysis takes when none is given, shown in the empty field.
  async function suggestRoot(project) {
    try { setRootHint(t('new.rootDefault', (await api('/browse/root?project=' + encodeURIComponent(project))).root)); }
    catch { setRootHint('C:\\src\\MyApp'); }
  }

  async function submit() {
    setBusy(true);
    setError(null);
    try {
      const entry = await post('/analyses', form);
      setForm({ project: '', root: '', configuration: 'Debug', name: '' });
      await onStarted(entry);
    } catch (e) {
      setError(e.message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <>
      <Modal visible={visible && !browsing} onDismiss={onDismiss} size="large" header={t('new.title')}
        footer={<Box float="right"><SpaceBetween direction="horizontal" size="xs">
          <Button variant="link" onClick={onDismiss}>{t('common.cancel')}</Button>
          <Button variant="primary" loading={busy} disabled={!form.project} onClick={submit}>{t('new.analyze')}</Button>
        </SpaceBetween></Box>}>
        <Form errorText={error}>
          <SpaceBetween size="m">
            <Box variant="p" color="text-body-secondary">{t('new.description')}</Box>
            <FormField label={t('new.project')} stretch>
              <Grid gridDefinition={[{ colspan: 9 }, { colspan: 3 }]}>
                <Input value={form.project} placeholder="C:\src\MyApp\Web\MyApp.Web.csproj" autoComplete={false}
                  onChange={ev => set('project', ev.detail.value)} onBlur={() => form.project && suggestRoot(form.project)} />
                <Button iconName="folder-open" onClick={() => setBrowsing({ mode: 'project', start: form.project })}>{t('new.browse')}</Button>
              </Grid>
            </FormField>
            <FormField label={t('new.root')} description={t('new.rootHint')} stretch>
              <Grid gridDefinition={[{ colspan: 9 }, { colspan: 3 }]}>
                <Input value={form.root} placeholder={rootHint} autoComplete={false} onChange={ev => set('root', ev.detail.value)} />
                <Button iconName="folder-open" onClick={() => setBrowsing({ mode: 'root', start: form.root || form.project })}>{t('new.browse')}</Button>
              </Grid>
            </FormField>
            <ColumnLayout columns={2}>
              <FormField label={t('new.configuration')}><Input value={form.configuration} onChange={ev => set('configuration', ev.detail.value)} /></FormField>
              <FormField label={t('new.name')} description={t('common.optional')}><Input value={form.name} onChange={ev => set('name', ev.detail.value)} /></FormField>
            </ColumnLayout>
          </SpaceBetween>
        </Form>
      </Modal>
      {browsing && <Browser mode={browsing.mode} start={browsing.start} onClose={path => {
        if (path) { set(browsing.mode, path); if (browsing.mode === 'project') suggestRoot(path); }
        setBrowsing(null);
      }} />}
    </>
  );
}

// Choosing a project or a folder: the server lists a folder's folders and projects (no path: the drives).
function Browser({ mode, start, onClose }) {
  const [listing, setListing] = useState(null);
  const [selected, setSelected] = useState(null);
  const [filter, setFilter] = useState('');
  const [typed, setTyped] = useState('');
  const [loading, setLoading] = useState(false);

  async function navigate(path) {
    setLoading(true);
    let l;
    try { l = await api('/browse?path=' + encodeURIComponent(path ?? '')); }
    catch (e) { l = { path, parent: null, entries: [], places: listing?.places ?? [], error: e.message }; }
    setLoading(false);
    setListing(l);
    setFilter('');
    if (l.path || !l.error) setTyped(l.path ?? '');
    // A project file given (the field's value): it is selected in its folder.
    setSelected(path && l.entries.find(e => e.kind === 'project' && e.path.toLowerCase() === String(path).toLowerCase()) || null);
    if (l.path && !l.error) localStorage.setItem('foc-browse', l.path);
  }
  useEffect(() => { navigate(start || localStorage.getItem('foc-browse') || ''); }, []); // eslint-disable-line react-hooks/exhaustive-deps

  // What "Choose" takes: the project selected; for a folder, the one selected or else the folder open.
  const choice = mode === 'project'
    ? (selected?.kind === 'project' ? selected.path : null)
    : selected && (selected.kind === 'folder' || selected.kind === 'drive') ? selected.path : listing?.path ?? null;

  const entries = (listing?.entries ?? []).filter(e => !filter || e.name.toLowerCase().includes(filter.trim().toLowerCase()));
  const open = e => {
    if (e.kind === 'folder' || e.kind === 'drive') navigate(e.path);
    else if (e.kind === 'project' && mode === 'project') onClose(e.path);
  };

  // Breadcrumbs: every folder up to the drive, each one a link.
  const crumbs = [{ text: 'PC', href: '' }];
  if (listing?.path) {
    const unix = listing.path.startsWith('/');
    let acc = unix ? '/' : '';
    listing.path.split(/[\\/]/).filter(Boolean).forEach((part, i) => {
      acc = unix ? (acc === '/' ? '/' + part : acc + '/' + part) : (i === 0 ? part + '\\' : acc.replace(/\\?$/, '\\') + part);
      crumbs.push({ text: part, href: acc });
    });
  }

  return (
    <Modal visible size="max" onDismiss={() => onClose(null)} header={mode === 'project' ? t('browse.project') : t('browse.folder')}
      footer={<SpaceBetween direction="horizontal" size="m" alignItems="center">
        <Box>{choice ? <>{t('browse.choice')} <Box variant="code" display="inline">{choice}</Box></> : <Box color="text-body-secondary">{t('browse.none')}</Box>}</Box>
        <SpaceBetween direction="horizontal" size="xs">
          <Button variant="link" onClick={() => onClose(null)}>{t('common.cancel')}</Button>
          <Button variant="primary" disabled={!choice} onClick={() => onClose(choice)}>
            {mode === 'root' && !(selected && selected.kind !== 'project') ? t('browse.thisFolder') : t('browse.choose')}</Button>
        </SpaceBetween>
      </SpaceBetween>}>
      <SpaceBetween size="m">
        <Box color="text-body-secondary">{mode === 'project' ? t('browse.projectHint') : t('browse.folderHint')}</Box>
        <Grid gridDefinition={[{ colspan: 1 }, { colspan: 11 }]}>
          <Button iconName="arrow-up" ariaLabel={t('browse.up')} disabled={!listing?.path} onClick={() => navigate(listing?.parent ?? '')} />
          <Input value={typed} placeholder={t('browse.typePath')} onChange={ev => setTyped(ev.detail.value)}
            onKeyDown={ev => { if (ev.detail.key === 'Enter') navigate(typed); }} />
        </Grid>
        <BreadcrumbGroup items={crumbs} ariaLabel={t('browse.where')} onFollow={ev => { ev.preventDefault(); navigate(ev.detail.href); }} />
        <Grid gridDefinition={[{ colspan: 3 }, { colspan: 9 }]}>
          <SpaceBetween size="xs">
            <Box variant="awsui-key-label">{t('browse.places')}</Box>
            {(listing?.places ?? []).map(p => <div key={p.path}><Link onFollow={() => navigate(p.path)}>{p.label}</Link></div>)}
            <div><Link onFollow={() => navigate('')}>{t('browse.drives')}</Link></div>
          </SpaceBetween>
          <Table variant="embedded" loading={loading} loadingText={t('common.loading')} items={entries} trackBy="path"
            selectionType="single" selectedItems={selected ? [selected] : []}
            isItemDisabled={e => e.kind === 'solution'}
            onSelectionChange={ev => setSelected(ev.detail.selectedItems[0] ?? null)}
            onRowClick={ev => { const e = ev.detail.item; if (e.kind !== 'solution') setSelected(e); }}
            filter={<TextFilter filteringText={filter} onChange={ev => setFilter(ev.detail.filteringText)} filteringPlaceholder={t('browse.filter')} />}
            empty={<Box textAlign="center" color="text-body-secondary">{listing?.error ?? (filter ? t('browse.noMatch') : t('browse.empty'))}</Box>}
            columnDefinitions={[{
              id: 'name', header: t('browse.name'), cell: e => (
                <SpaceBetween direction="horizontal" size="xs" alignItems="center">
                  <Icon name={e.kind === 'project' || e.kind === 'solution' ? 'file' : 'folder'} />
                  {e.kind === 'folder' || e.kind === 'drive' || (e.kind === 'project' && mode === 'project')
                    ? <Link onFollow={() => open(e)}>{e.name}</Link> : <span>{e.name}</span>}
                  {e.kind === 'project' && <Badge color="blue">{e.name.toLowerCase().endsWith('.vbproj') ? 'VB' : 'C#'}</Badge>}
                  {e.kind === 'solution' && <Badge>SLN</Badge>}
                  {e.hasProject && <Badge color="green">{t('browse.hasProject')}</Badge>}
                </SpaceBetween>
              ),
            }]} />
        </Grid>
      </SpaceBetween>
    </Modal>
  );
}
