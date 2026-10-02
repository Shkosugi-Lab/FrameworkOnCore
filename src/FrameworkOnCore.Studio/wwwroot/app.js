// FrameworkOnCore Studio: the analyses (sidebar), and one analysis as a wizard of four steps: ① the analysis (the
// original application run here, the numbers, the components and their APIs), ② the decisions (each component's
// option, the application's settings, the DLLs' references retargeted; saved through PUT /api/analyses/{id}/choices),
// ③ the conversion and its build (its report), ④ the deployment (the zip, the converted application run here or in a
// container). Everything comes from the HTTP API: the page holds no data of its own.

// Statuses: the reserved status colors, always with an icon and a label (never the color alone).
const STATUS = {
  Missing: { label: '.NET に無い', icon: '✕', color: 'var(--status-critical)' },
  Throws: { label: '例外(全 OS)', icon: 'ϟ', color: 'var(--status-critical)', texture: true },
  WindowsOnly: { label: '例外(Linux)', icon: '⊘', color: 'var(--status-serious)' },
  Behavior: { label: '動きが違う', icon: '≈', color: 'var(--status-warning)' },
  Obsolete: { label: '廃止予定(動く)', icon: '◷', color: 'var(--status-neutral)' },
  Available: { label: 'そのまま', icon: '✓', color: 'var(--status-good)' },
};
const ORDER = ['Missing', 'Throws', 'WindowsOnly', 'Behavior', 'Obsolete', 'Available'];
const ROWS = 60;

const state = {
  analyses: [], catalog: null, id: null, entry: null, result: null, byComponent: new Map(),
  choices: null, saved: null, command: null, errors: [],
  filter: { q: '', statuses: new Set(ORDER.slice(0, 5)) }, expanded: new Set(), rows: new Map(), poll: null, step: 1,
};

// The wizard's steps.
const STEPS = [
  { title: '分析', sub: '読み込み・テスト起動・結果' },
  { title: '方針決定', sub: '部品の対応・設定・DLL の参照' },
  { title: '変換・ビルド', sub: '変換の設定と実行・レポート' },
  { title: 'デプロイ', sub: 'ZIP・ネイティブ・コンテナ' },
];

const $ = (s, root = document) => root.querySelector(s);
const esc = s => String(s ?? '').replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
const fmt = n => Number(n ?? 0).toLocaleString('ja-JP');
const clone = o => JSON.parse(JSON.stringify(o));

async function api(path, options = {}) {
  const response = await fetch('/api' + path, { headers: { 'Content-Type': 'application/json' }, ...options });
  const text = await response.text();
  const body = text ? JSON.parse(text) : null;
  if (!response.ok) throw Object.assign(new Error(body?.error ?? response.statusText), { body, status: response.status });
  return body;
}

function pill(status) {
  const s = STATUS[status];
  return `<span class="pill" style="--c:${s.color}"><span class="ic" aria-hidden="true">${s.icon}</span>${esc(s.label)}</span>`;
}

function toast(text) {
  const t = $('#toast');
  t.textContent = text;
  t.hidden = false;
  clearTimeout(toast.timer);
  toast.timer = setTimeout(() => (t.hidden = true), 2400);
}

// ---------------------------------------------------------------------------------------------------------------------
// Sidebar

// The list; while one waits or runs, it is read again (its state changes whichever analysis is open).
async function loadAnalyses() {
  state.analyses = await api('/analyses');
  renderSidebar();
  clearTimeout(loadAnalyses.timer);
  if (state.analyses.some(a => a.state === 'queued' || a.state === 'running'))
    loadAnalyses.timer = setTimeout(() => loadAnalyses().catch(console.error), 2000);
}

const active = a => a.state === 'queued' || a.state === 'running';

function renderSidebar() {
  const list = $('#analyses');
  $('#clear-failed').hidden = !state.analyses.some(a => a.state === 'failed');
  if (state.analyses.length === 0) {
    list.innerHTML = '<div class="muted" style="padding:6px 10px;font-size:13px">まだ解析がありません</div>';
    return;
  }
  list.innerHTML = state.analyses.map(a => {
    const meta = a.state === 'done' && a.summary
      ? `${fmt(a.summary.apis)} API ・ 部品 ${fmt(a.summary.toDecide)}`
      : a.state === 'failed' ? '失敗' : a.state === 'queued' ? '待機中' : '解析中…';
    return `<div class="analysis-row">
      <button class="analysis-item ${a.id === state.id ? 'active' : ''}" data-id="${esc(a.id)}">
        <div class="name"><span class="dot ${esc(a.state)}"></span>${esc(a.name)}</div>
        <div class="meta">${esc(meta)} ・ ${new Date(a.created).toLocaleString('ja-JP', { dateStyle: 'short', timeStyle: 'short' })}</div>
      </button>
      <button class="row-delete" data-delete="${esc(a.id)}" title="${active(a) ? '中止して削除' : '削除'}" aria-label="${esc(a.name)} を${active(a) ? '中止して' : ''}削除">✕</button>
    </div>`;
  }).join('');
  list.querySelectorAll('.analysis-item').forEach(b => b.addEventListener('click', () => select(b.dataset.id)));
  list.querySelectorAll('.row-delete').forEach(b => b.addEventListener('click', ev => {
    ev.stopPropagation();
    removeAnalysis(b.dataset.delete).catch(e => toast(e.message));
  }));
}

// ---------------------------------------------------------------------------------------------------------------------
// One analysis

async function select(id) {
  if (state.id !== id && isDirty() && !confirm('保存していない選択があります。破棄して移りますか?')) return;
  clearTimeout(state.poll);
  clearTimeout(state.convertPoll);
  clearTimeout(state.containerPoll);
  clearTimeout(state.dockerPoll);
  clearTimeout(state.originalPoll);
  clearTimeout(state.nativePoll);
  // The step of the link (#/a/<analysis>/step/<n>; a component's link is of ②), else the first.
  const linked = location.hash.startsWith(`#/a/${id}/`) ? location.hash : '';
  const step = /\/c\//.test(linked) ? 2 : +(linked.match(/\/step\/([1-4])/)?.[1] ?? 1);
  Object.assign(state, {
    id, entry: null, result: null, choices: null, saved: null, command: null, errors: [], conversion: null,
    container: null, runOpen: false, runEnv: null, docker: null, dockerStarting: false, step,
    original: null, originalEnv: null, native: null, nativeEnv: null, report: null, advice: null, buildOriginalChoice: null,
  });
  state.expanded.clear();
  state.rows.clear();
  if (!location.hash.startsWith(`#/a/${id}`)) location.hash = id ? `#/a/${id}` : '';
  renderSidebar();
  $('#welcome').hidden = !!id;
  $('#view').hidden = !id;
  $('#savebar').hidden = true;
  if (!id) return;
  $('#view').innerHTML = '<div class="progress"></div>';
  await refresh();
}

async function refresh() {
  const id = state.id;
  const { entry, log } = await api(`/analyses/${id}`);
  if (id !== state.id) return;
  state.entry = entry;
  if (entry.state === 'done') {
    const [result, choices, command] = await Promise.all([api(`/analyses/${id}/result`), api(`/analyses/${id}/choices`), api(`/analyses/${id}/command`)]);
    if (id !== state.id) return;
    state.result = result;
    state.command = command.command;
    state.byComponent = new Map();
    for (const a of result.apis) {
      if (!state.byComponent.has(a.component)) state.byComponent.set(a.component, []);
      state.byComponent.get(a.component).push(a);
    }
    state.choices = normalize(choices);
    state.saved = clone(state.choices);
    // A link to a component (#/a/<analysis>/c/<component>): opened, in view.
    const linked = decodeURIComponent(location.hash.match(/\/c\/([^/]+)$/)?.[1] ?? '');
    if (linked) state.expanded.add(linked);
    renderResult();
    if (linked) document.getElementById(`c-${linked}`)?.scrollIntoView({ block: 'start' });
    await Promise.all([loadAnalyses(), loadConversion(), loadOriginal(), loadNative(), loadAdvice()]);
  } else {
    renderRunning(entry, log);
    if (entry.state !== 'failed') state.poll = setTimeout(() => refresh().catch(console.error), 1500);
    else await loadAnalyses();
  }
}

function renderRunning(entry, log) {
  const failed = entry.state === 'failed';
  $('#view').innerHTML = `
    <div class="running">
      <div class="header"><div>
        <h1>${esc(entry.name)}</h1>
        <div class="sub"><code>${esc(entry.project)}</code><span>構成 ${esc(entry.configuration)}</span></div>
      </div>
      <div class="header-actions"><button class="btn ghost danger" id="delete">${failed ? '削除' : '中止して削除'}</button></div></div>
      ${stepper(false)}
      <div class="card card-pad">
        <h2>${failed ? '解析できませんでした' : entry.state === 'queued' ? 'ほかの解析が終わるのを待っています' : 'アプリを読み込んで、API を数えています'}</h2>
        <div class="hint">${failed ? esc(entry.error ?? '') : 'ソースを .NET Framework 4.8 の参照アセンブリでコンパイルし、名前を一つずつ .NET 10 と照らし合わせます。大きなアプリでは数十秒かかります。'}</div>
        ${failed ? '' : '<div class="progress"></div>'}
        <pre class="log">${esc((log ?? []).slice(-60).join('\n')) || '…'}</pre>
      </div>
    </div>`;
  $('#delete').addEventListener('click', () => removeAnalysis(entry.id).catch(e => toast(e.message)));
  const pre = $('.log');
  pre.scrollTop = pre.scrollHeight;
}

// ---------------------------------------------------------------------------------------------------------------------
// The result

function renderResult() {
  const r = state.result;
  const e = state.entry;
  $('#view').innerHTML = `
    <div class="header">
      <div>
        <h1>${esc(e.name)}</h1>
        <div class="sub">
          <span>${esc(r.repository)} / <code>${esc(r.entry)}</code></span>
          <span>構成 ${esc(r.configuration)}</span>
          <span>${new Date(r.analyzed).toLocaleString('ja-JP')}</span>
          <span>カタログ v${esc(r.catalogVersion)}</span>
        </div>
      </div>
      <div class="header-actions">
        <button class="btn ghost" id="reanalyze" title="同じプロジェクトをもう一度解析する">↻ 再解析</button>
        <button class="btn ghost danger" id="delete">削除</button>
      </div>
    </div>
    <div id="stepper"></div>
    <div id="step" class="step"></div>`;
  $('#delete').addEventListener('click', () => removeAnalysis(e.id).catch(err => toast(err.message)));
  $('#reanalyze').addEventListener('click', () => startAnalysis({ project: e.project, root: e.root, configuration: e.configuration, name: e.name }));
  renderStepper();
  renderStep();
}

// ---------------------------------------------------------------------------------------------------------------------
// The wizard: the steps, where each one is

// What each step has come to (shown under its title).
function stepState(n) {
  const c = state.conversion?.conversion;
  if (n === 1) return state.original?.original?.state === 'running' ? { text: '元のアプリ実行中', on: true } : { text: '完了', done: true };
  if (n === 2) {
    const k = changes();
    return k > 0 ? { text: `未保存 ${k} 件`, warn: true } : { text: '保存済み', done: !!c };
  }
  if (n === 3) return c ? { text: CONVERSION[c.state].label, done: c.state === 'done', warn: c.state === 'failed', on: c.state === 'queued' || c.state === 'running' } : { text: '未実行' };
  const running = [state.native?.native, state.container?.container].filter(x => x?.state === 'running').length;
  return running ? { text: `${running} 件実行中`, on: true } : c?.state === 'done' ? { text: '準備完了' } : { text: 'ビルド後' };
}

// Which steps can be opened: ① always, ②③ once the analysis is done, ④ once the conversion is built.
function stepOpen(n) {
  if (n === 1) return true;
  if (state.entry?.state !== 'done') return false;
  return n < 4 || state.conversion?.conversion?.state === 'done';
}

function stepper(withStates = true) {
  return `<nav class="stepper" aria-label="手順">${STEPS.map((s, i) => {
    const n = i + 1;
    const st = withStates ? stepState(n) : n === 1 ? { text: '解析中', on: true } : { text: '' };
    const open = withStates && stepOpen(n);
    const current = n === (withStates ? state.step : 1);
    return `<button class="step-tab ${current ? 'current' : ''} ${st.done ? 'done' : ''}" data-step="${n}" ${open ? '' : 'disabled'} ${current ? 'aria-current="step"' : ''}>
      <span class="step-no" aria-hidden="true">${st.done && !current ? '✓' : n}</span>
      <span class="step-text"><span class="step-title">${esc(s.title)}</span><span class="step-sub">${esc(s.sub)}</span>
        ${st.text ? `<span class="step-state ${st.warn ? 'warn' : ''} ${st.on ? 'on' : ''}">${esc(st.text)}</span>` : ''}</span>
    </button>`;
  }).join('<span class="step-line" aria-hidden="true"></span>')}</nav>`;
}

function renderStepper() {
  const root = $('#stepper');
  if (!root) return;
  root.innerHTML = stepper();
  root.querySelectorAll('.step-tab').forEach(b => b.addEventListener('click', () => goStep(+b.dataset.step).catch(e => toast(e.message))));
}

// To a step: forward from ② the choices are saved first (what is converted is what is saved).
async function goStep(n) {
  if (n === state.step || !stepOpen(n)) return;
  if (state.step === 2 && n > 2 && isDirty()) {
    await save();
    if (state.errors.length) return;
  }
  state.step = n;
  history.replaceState(null, '', `#/a/${state.id}/step/${n}`);
  renderStepper();
  renderStep();
  scrollTo({ top: 0 });
}

function stepNav(back, next) {
  return `<div class="step-nav">
    ${back ? `<button class="btn ghost" data-go="${back}">← ${esc(STEPS[back - 1].title)}に戻る</button>` : '<span></span>'}
    ${next ? `<button class="btn primary" data-go="${next.step}" ${stepOpen(next.step) ? '' : 'disabled'}>${esc(next.label)} →</button>` : ''}
  </div>`;
}

function renderStep() {
  const root = $('#step');
  if (!root) return;
  const r = state.result;
  state.runOpen = state.step === 4;
  if (state.step === 1) {
    const uses = r.apis.reduce((s, a) => s + a.count, 0);
    const frameworkUses = r.projects.reduce((s, p) => s + p.frameworkCalls, 0);
    const unresolved = r.projects.reduce((s, p) => s + p.unresolved, 0);
    const files = new Set(r.apis.flatMap(a => a.files.map(f => f.file))).size;
    const attention = r.components.filter(c => c.status !== 'Available');
    const count = status => attention.filter(c => c.status === status).length;
    const resolvedRate = frameworkUses + unresolved === 0 ? 100 : (100 * frameworkUses / (frameworkUses + unresolved));
    root.innerHTML = `
      <p class="step-intro">アプリが使う .NET Framework の API を数え、.NET 10 でどうなるかを調べた結果です。変換の前に、元のアプリをこの PC で動かして確かめることもできます。</p>
      <div class="card card-pad runner" id="original"></div>
      <div class="tiles">
        ${tile('対応を選ぶ部品', attention.length, '件', `.NET に無い ${count('Missing')} ・ 例外 ${count('Throws') + count('WindowsOnly')} ・ 動きの違い ${count('Behavior')}`)}
        ${tile('.NET Framework の API', fmt(r.apis.length), '種類', `ソースの ${fmt(r.projects.filter(p => !p.skipped).length)} プロジェクト`)}
        ${tile('使用回数', fmt(uses), '回', `${fmt(files)} ファイル`)}
        ${tile('名前の解決率', resolvedRate.toFixed(1), '%', `解決できなかった名前 ${fmt(unresolved)}`)}
      </div>
      <div class="card card-pad breakdown">
        <h2>そのまま動かない API の使用(状態別)</h2>
        <div class="hint" id="breakdown-hint"></div>
        <div class="stack" id="stack" role="img" aria-label="状態別の使用回数"></div>
        <div class="legend" id="legend"></div>
      </div>
      <h2 class="step-h">部品と API</h2>
      <div class="hint">部品ごとの対応は、次の「方針決定」で選びます。</div>
      ${componentList()}
      ${stepNav(0, { step: 2, label: '次へ: 方針決定' })}`;
    renderOriginal();
    renderBreakdown();
    wireComponentList();
  } else if (state.step === 2) {
    root.innerHTML = `
      <p class="step-intro">変換で使う選択です。既定のままでも変換できます。選んだら保存して、変換に進みます。</p>
      <h2 class="step-h">部品ごとの対応</h2>
      <div class="hint">選べる対応がある部品だけを並べています。API ごとに変えることもできます(「API を見る」)。</div>
      ${componentList()}
      <div class="card card-pad settings">
        <h2>アプリの設定</h2>
        <div class="hint">API ではなく、アプリ全体に対する選択です。</div>
        <div id="settings"></div>
      </div>
      ${retargetCard(r) || '<div class="card card-pad retarget"><h2><span class="retarget-ic" aria-hidden="true">↪</span> DLL の参照の付け替え</h2><div class="hint">付け替えるものはありません(ソースの無い DLL が、.NET 10 で別のアセンブリにある型を参照していません)。</div></div>'}
      ${stepNav(1, { step: 3, label: 'saveNext' })}`;
    renderSettings();
    wireComponentList();
  } else if (state.step === 3) {
    root.innerHTML = `
      <p class="step-intro">保存した選択で .NET 10 のプロジェクトに変換し、そのままビルドします。</p>
      <div class="card card-pad convert" id="convert"></div>
      <div id="report"></div>
      ${stepNav(2, { step: 4, label: '次へ: デプロイ' })}`;
    renderConversion();
    renderReport();
  } else {
    root.innerHTML = `
      <p class="step-intro">変換・ビルドしたアプリを、ZIP で持ち出す、この PC で動かす、Linux のコンテナで動かす、のいずれかで確かめます。</p>
      <div class="deploy">
        <div class="card card-pad deploy-card" id="zip"></div>
        <div class="card card-pad deploy-card planned">
          <h3><span class="deploy-ic" aria-hidden="true">☁</span> ECR 発行 <span class="badge planned">予定</span></h3>
          <div class="hint">コンテナのイメージを Amazon ECR に発行します(準備中)。</div>
          <div class="deploy-actions"><button class="btn ghost" disabled>ECR に発行</button></div>
        </div>
      </div>
      <div class="card card-pad runner" id="native"></div>
      <div class="card card-pad runner" id="run"></div>
      ${stepNav(3, null)}`;
    renderZip();
    renderNative();
    renderContainer();
    if (!state.docker?.available) loadDocker().catch(console.error);
  }
  root.querySelectorAll('.step-nav [data-go]').forEach(b => b.addEventListener('click', () => goStep(+b.dataset.go).catch(e => toast(e.message))));
  renderStepNext();
  renderSavebar();
}

// ② forward: "save and go on" while there are changes.
function renderStepNext() {
  const next = $('#step .step-nav [data-go="3"]');
  if (next && state.step === 2) next.textContent = `${isDirty() ? '保存して次へ' : '次へ'}: 変換・ビルド →`;
}

// The components (① to look at them, ② to choose): the search, the statuses, the cards.
function componentList() {
  return `<div class="toolbar">
      <label class="search"><span aria-hidden="true">⌕</span>
        <input id="search" placeholder="部品・API を検索(例: Drawing, Encoding.Default)" value="${esc(state.filter.q)}">
      </label>
      <div class="chips" id="chips"></div>
    </div>
    <div class="components" id="components"></div>`;
}

function wireComponentList() {
  $('#search').addEventListener('input', ev => { state.filter.q = ev.target.value; renderComponents(); });
  renderChips();
  renderComponents();
}

function tile(label, value, unit, foot) {
  return `<div class="card tile"><div class="label">${esc(label)}</div>
    <div class="value">${esc(value)}<small>${esc(unit)}</small></div><div class="foot">${esc(foot)}</div></div>`;
}

// Part-to-whole of the uses that do not work as they were, by status: one bar, a legend with icons, a hover tooltip.
function renderBreakdown() {
  const totals = Object.fromEntries(ORDER.map(s => [s, 0]));
  for (const a of state.result.apis) totals[a.status] += a.count;
  const all = ORDER.reduce((s, k) => s + totals[k], 0);
  const shown = ORDER.slice(0, 5).filter(s => totals[s] > 0);
  const sum = shown.reduce((s, k) => s + totals[k], 0);
  $('#breakdown-hint').textContent = all === 0 ? '' :
    `使用回数の ${(100 * totals.Available / all).toFixed(1)}% はそのまま動きます。残りの ${fmt(sum)} 回の内訳:`;
  const stack = $('#stack');
  stack.innerHTML = shown.map(s => {
    const st = STATUS[s];
    const bg = st.texture
      ? `repeating-linear-gradient(45deg, ${st.color} 0 5px, color-mix(in srgb, ${st.color} 55%, black) 5px 7px)` : st.color;
    return `<div class="seg" data-status="${s}" style="flex:${totals[s]};background:${bg}"></div>`;
  }).join('') || '<div class="muted" style="font-size:13px">対応が必要な API はありません</div>';
  stack.querySelectorAll('.seg').forEach(seg => {
    const s = seg.dataset.status;
    seg.addEventListener('mousemove', ev => showTip(ev, `${STATUS[s].icon} <b>${esc(STATUS[s].label)}</b><br>${fmt(totals[s])} 回 ・ ${(100 * totals[s] / sum).toFixed(1)}%`));
    seg.addEventListener('mouseleave', hideTip);
  });
  $('#legend').innerHTML = shown.map(s => `<div class="legend-item">
      <span class="swatch" style="background:${STATUS[s].color}"></span>${STATUS[s].icon} ${esc(STATUS[s].label)} <b>${fmt(totals[s])}</b></div>`).join('');
}

function showTip(ev, html) {
  const tip = $('#tooltip');
  tip.innerHTML = html;
  tip.hidden = false;
  const x = Math.min(ev.clientX + 14, innerWidth - tip.offsetWidth - 10);
  tip.style.left = `${x}px`;
  tip.style.top = `${ev.clientY + 16}px`;
}
function hideTip() { $('#tooltip').hidden = true; }

function renderChips() {
  const counts = Object.fromEntries(ORDER.map(s => [s, pool().filter(c => c.status === s).length]));
  $('#chips').innerHTML = ORDER.filter(s => counts[s] > 0).map(s => `<button class="chip" data-status="${s}" style="--c:${STATUS[s].color}"
      aria-pressed="${state.filter.statuses.has(s)}"><span class="ic">${STATUS[s].icon}</span>${esc(STATUS[s].label)} ${counts[s]}</button>`).join('');
  $('#chips').querySelectorAll('.chip').forEach(chip => chip.addEventListener('click', () => {
    const s = chip.dataset.status;
    state.filter.statuses.has(s) ? state.filter.statuses.delete(s) : state.filter.statuses.add(s);
    chip.setAttribute('aria-pressed', state.filter.statuses.has(s));
    renderComponents();
  }));
}

// ---------------------------------------------------------------------------------------------------------------------
// Settings and components: the options, chosen or default

function defaultOf(options) { return (options.find(o => o.default) ?? options[0]).id; }

// A component with options to choose from (one only, or one with others planned, is not a decision).
function choosable(c) {
  const options = c.options ?? [];
  return options.filter(o => !o.planned).length > 1 || options.some(o => o.planned);
}

// The components of the step: in the decisions those to decide, all of them in the analysis.
const pool = () => state.result.components.filter(c => state.step !== 2 || choosable(c));

function renderSettings() {
  $('#settings').innerHTML = state.result.settings.map(s => {
    const chosen = state.choices.settings[s.id] ?? defaultOf(s.options);
    return `<div class="setting-row"><div><b>${esc(s.title)}</b> <span class="cid">${esc(s.id)}</span>
        ${chosen !== defaultOf(s.options) ? '<span class="changed">変更</span>' : ''}</div>
      <div class="options" style="padding:0">${s.options.map(o => optionCard(`setting:${s.id}`, o, chosen)).join('')}</div></div>`;
  }).join('');
  bindOptions($('#settings'));
}

function optionCard(name, o, chosen) {
  return `<label class="option ${o.id === chosen ? 'selected' : ''} ${o.planned ? 'planned' : ''}" title="${o.planned ? 'まだありません(予定)' : ''}">
    <input type="radio" name="${esc(name)}" value="${esc(o.id)}" ${o.id === chosen ? 'checked' : ''} ${o.planned ? 'disabled' : ''}>
    <span class="radio"></span>
    <div class="t">${esc(o.title)}${o.default ? '<span class="badge default">既定</span>' : ''}${o.planned ? '<span class="badge planned">予定</span>' : ''}</div>
    ${o.description ? `<div class="d">${esc(o.description)}</div>` : ''}
  </label>`;
}

function bindOptions(root) {
  root.querySelectorAll('input[type=radio]').forEach(input => input.addEventListener('change', () => {
    const [kind, id] = input.name.split(/:(.*)/s);
    if (kind === 'setting') state.choices.settings[id] = input.value;
    else state.choices.components[id] = input.value;
    kind === 'setting' ? renderSettings() : renderComponents();
    renderSavebar();
  }));
}

// Those the sources use first (a component only DLLs reference after them), the worst status first, the most used first.
function visibleComponents() {
  const q = state.filter.q.trim().toLowerCase();
  const rank = c => [c.attentionCount > 0 || c.status === 'Available' ? 0 : 1, ORDER.indexOf(c.status), -c.attentionCount, -c.binaryReferences];
  const compare = (a, b) => { const x = rank(a), y = rank(b); for (let i = 0; i < x.length; i++) if (x[i] !== y[i]) return x[i] - y[i]; return 0; };
  return pool().slice().sort(compare).filter(c => {
    if (!state.filter.statuses.has(c.status)) return false;
    if (!q) return true;
    if (`${c.title} ${c.id} ${c.note ?? ''}`.toLowerCase().includes(q)) return true;
    return (state.byComponent.get(c.id) ?? []).some(a => a.name.toLowerCase().includes(q) || a.id.toLowerCase().includes(q));
  });
}

function renderComponents() {
  const list = visibleComponents();
  const root = $('#components');
  if (list.length === 0) {
    root.innerHTML = '<div class="card card-pad muted">条件に合う部品はありません</div>';
    return;
  }
  root.innerHTML = list.map(componentCard).join('');
  bindOptions(root);
  root.querySelectorAll('.expander').forEach(b => b.addEventListener('click', () => {
    const id = b.dataset.id;
    state.expanded.has(id) ? state.expanded.delete(id) : state.expanded.add(id);
    renderComponents();
  }));
  root.querySelectorAll('.place').forEach(b => b.addEventListener('click', () => openSource(b.dataset.file, +b.dataset.line, b.dataset.api)));
  root.querySelectorAll('select.override').forEach(sel => sel.addEventListener('change', () => {
    if (sel.value) state.choices.apis[sel.dataset.api] = sel.value; else delete state.choices.apis[sel.dataset.api];
    renderComponents();
    renderSavebar();
  }));
  root.querySelectorAll('.show-more').forEach(b => b.addEventListener('click', () => {
    state.rows.set(b.dataset.id, (state.rows.get(b.dataset.id) ?? ROWS) + ROWS * 3);
    renderComponents();
  }));
}

function componentCard(c) {
  const s = STATUS[c.status];
  const options = c.options ?? [];
  const chosen = state.choices.components[c.id] ?? defaultOf(options);
  const changed = chosen !== defaultOf(options) || apisOf(c).some(a => state.choices.apis[a.id]);
  const deciding = state.step === 2;  // ① shows what is chosen; it is chosen in ②
  const expanded = state.expanded.has(c.id);
  const attentionApis = apisOf(c);
  return `<article class="card component" id="c-${esc(c.id)}" style="--c:${s.color}">
    <div class="component-head">
      <div>
        <div class="component-title"><h3>${esc(c.title)}</h3><span class="cid">${esc(c.id)}</span>${pill(c.status)}
          ${c.attentionCount === 0 && c.binaryReferences > 0 ? '<span class="badge planned" title="ソースでは使われず、ソースのない DLL だけが参照している">DLL だけ</span>' : ''}
          ${c.retargetedApis ? `<span class="badge retarget-badge" title="DLL の参照先を変換器が付け替える API">↪ 付け替え ${fmt(c.retargetedApis)}</span>` : ''}
          ${changed ? '<span class="changed">変更</span>' : ''}</div>
        ${c.note ? `<p class="note">${esc(c.note)}</p>` : ''}
      </div>
      <div class="numbers">
        ${num(c.attentionApis, '要対応 API')}${num(c.attentionCount, '回数')}${num(c.files, 'ファイル')}${c.binaryReferences ? num(c.binaryReferences, 'DLL の参照') : ''}
      </div>
    </div>
    ${deciding && choosable(c)
      ? `<div class="options">${options.map(o => optionCard(`component:${c.id}`, o, chosen)).join('')}</div>`
      : choosable(c)
        ? `<div class="single">対応: <b>${esc(options.find(o => o.id === chosen)?.title ?? '')}</b>${chosen === defaultOf(options) ? ' <span class="muted">(既定 ・ 「方針決定」で選べます)</span>' : ''}</div>`
        : `<div class="single" title="${esc(options[0]?.description ?? '')}">選べる対応はありません(${esc(options[0]?.title ?? '')})</div>`}
    <button class="expander" data-id="${esc(c.id)}" aria-expanded="${expanded}"><span class="chev">▸</span>
      API を見る(${fmt(attentionApis.length)} 件)</button>
    ${expanded ? apiTable(c, attentionApis, deciding ? options : []) : ''}
  </article>`;
}

// The DLLs' references the converter retargets: .NET has the type, not in the assembly the DLL names (FrameworkOnCore's
// CallContext is in its System.Web, the DLL looks in mscorlib). Grouped by from -> to.
function retargetCard(r) {
  const apis = r.apis.filter(a => a.retargetedTo);
  if (!apis.length) return '';
  const groups = new Map();
  for (const a of apis) {
    const key = `${a.assembly} → ${a.retargetedTo}${a.callReplaced ? ' call' : ''}`;
    if (!groups.has(key)) groups.set(key, { from: a.assembly, to: a.retargetedTo, call: a.callReplaced, apis: [], dlls: new Set() });
    const g = groups.get(key);
    g.apis.push(a);
    for (const b of a.binaries ?? []) g.dlls.add(b.file.split('/').pop());
  }
  const rows = [...groups.values()].map(g => {
    // A call replaced: the members themselves (the type is .NET's); retargeted: the types (and how many members).
    const types = g.apis.filter(a => g.call || a.kind === 'Type').map(a => a.name);  // broken after a dot, not within a name
    const members = g.apis.length - types.length;
    return `<tr>
      <td><span class="retarget-flow"><code>${esc(g.from)}</code><span class="arrow" aria-hidden="true">→</span><code>${esc(g.to)}</code></span>
        ${g.call ? '<div class="api-detail">呼び出しを置き換え(.NET の型に無いメンバー)</div>' : ''}</td>
      <td>${types.map(t => `<div class="api-name">${esc(t).replaceAll('.', '.<wbr>')}</div>`).join('')}${members ? `<div class="api-detail">そのメンバー ${fmt(members)} 件</div>` : ''}</td>
      <td class="dlls">${[...g.dlls].map(d => `<div>${esc(d)}</div>`).join('')}</td>
    </tr>`;
  }).join('');
  return `<div class="card card-pad retarget">
    <h2><span class="retarget-ic" aria-hidden="true">↪</span> DLL の参照の付け替え <span class="muted">(${fmt(apis.length)} API)</span></h2>
    <div class="hint">ソースの無い DLL は型を「アセンブリ + 型名」で参照します。次の型は .NET 10 にありますが、DLL が参照するアセンブリには無いので、変換器が DLL の参照先を付け替えます(そのまま動きます。変換レポートの「references retargeted」)。</div>
    <div class="api-wrap"><table class="api-table">
      <thead><tr><th>参照先 → 付け替え先</th><th>型</th><th>DLL</th></tr></thead>
      <tbody>${rows}</tbody></table></div>
  </div>`;
}

function num(n, label) { return `<div class="num"><div class="n">${fmt(n)}</div><div class="l">${esc(label)}</div></div>`; }

// A component's APIs that do not work as they were (all of them for one that does), the most used first.
function apisOf(c) {
  const all = state.byComponent.get(c.id) ?? [];
  const attention = all.filter(a => a.status !== 'Available');
  return (attention.length ? attention : all).slice().sort((a, b) => ORDER.indexOf(a.status) - ORDER.indexOf(b.status) || b.count - a.count);
}

function apiTable(c, apis, options) {
  const limit = state.rows.get(c.id) ?? ROWS;
  const overridable = options.filter(o => !o.planned).length > 1;
  const rows = apis.slice(0, limit).map(a => {
    const place = a.places?.[0];
    const override = state.choices.apis[a.id] ?? '';
    const dlls = (a.binaries ?? []).map(b => b.file.split('/').pop());
    return `<tr>
      <td><div class="api-name">${esc(a.name)}</div>${a.obsolete || a.note ? `<div class="api-detail">${esc(a.obsolete ?? a.note)}</div>` : ''}
        ${a.retargetedTo ? `<div class="api-retarget">↪ ${a.callReplaced ? 'DLL の呼び出しを置き換え' : 'DLL の参照を付け替え'}: <code>${esc(a.assembly)}</code> → <code>${esc(a.retargetedTo)}</code></div>` : ''}</td>
      <td>${pill(a.status)}</td>
      <td class="r">${fmt(a.count)}</td>
      <td>${place ? `<button class="place" data-file="${esc(place.file)}" data-line="${place.line}" data-api="${esc(a.name)}">${esc(place.file)}:${place.line}</button>
        ${a.files.length > 1 ? `<div class="api-detail">ほか ${a.files.length - 1} ファイル</div>` : ''}` : '<span class="muted">—</span>'}
        ${dlls.length ? `<div class="dlls">DLL: ${esc(dlls.slice(0, 3).join(', '))}${dlls.length > 3 ? ` ほか ${dlls.length - 3}` : ''}</div>` : ''}</td>
      ${overridable ? `<td><select class="override ${override ? 'set' : ''}" data-api="${esc(a.id)}" aria-label="この API の対応">
          <option value="">部品の選択に従う</option>
          ${options.filter(o => !o.planned).map(o => `<option value="${esc(o.id)}" ${o.id === override ? 'selected' : ''}>${esc(o.title)}</option>`).join('')}
        </select></td>` : ''}
    </tr>`;
  }).join('');
  return `<div class="api-wrap"><table class="api-table">
    <thead><tr><th>API</th><th>状態</th><th class="r">回数</th><th>主な場所</th>${overridable ? '<th>この API だけ</th>' : ''}</tr></thead>
    <tbody>${rows}</tbody></table>
    ${apis.length > limit ? `<div class="more"><button class="btn ghost small show-more" data-id="${esc(c.id)}">さらに表示(残り ${fmt(apis.length - limit)} 件)</button></div>` : ''}
  </div>`;
}

// ---------------------------------------------------------------------------------------------------------------------
// Converting and building in Studio: the saved choices; when built, the output as a zip.

const CONVERSION = {
  queued: { label: '待機中', icon: '…', color: 'var(--accent-2)' },
  running: { label: '変換・ビルド中', icon: '⟳', color: 'var(--accent-2)' },
  done: { label: 'ビルド成功', icon: '✓', color: 'var(--status-good)' },
  failed: { label: '失敗', icon: '✕', color: 'var(--status-critical)' },
  cancelled: { label: '中止', icon: '■', color: 'var(--status-neutral)' },
};

async function loadConversion() {
  const id = state.id;
  clearTimeout(state.convertPoll);
  const body = await api(`/analyses/${id}/conversion`);
  if (id !== state.id) return;
  state.conversion = body;
  // ④ (a link to it) without a built conversion: ③.
  if (state.step === 4 && !stepOpen(4)) {
    state.step = 3;
    history.replaceState(null, '', `#/a/${id}/step/3`);
    renderStep();
  }
  renderConversion();
  renderZip();
  renderStepper();
  renderStepNext();
  $('#step .step-nav [data-go="4"]')?.toggleAttribute('disabled', !stepOpen(4));
  const c = body.conversion;
  if (c && (c.state === 'queued' || c.state === 'running'))
    state.convertPoll = setTimeout(() => loadConversion().catch(console.error), 2000);
  else if (c) {
    loadAnalyses().catch(console.error);
    await loadReport();
  }
  if (c?.state === 'done') await loadContainer();
}

function duration(from, to) {
  const s = Math.max(0, Math.round(((to ? new Date(to) : new Date()) - new Date(from)) / 1000));
  return s < 60 ? `${s} 秒` : `${Math.floor(s / 60)} 分 ${s % 60} 秒`;
}

function renderConversion() {
  const root = $('#convert');
  if (!root) return;
  const body = state.conversion;
  const c = body?.conversion;
  const active = c && (c.state === 'queued' || c.state === 'running');
  const log = (body?.log ?? []).join('\n');
  const logBlock = open => log ? `<details class="convert-log" ${open ? 'open' : ''}><summary>ログ(${fmt(body.log.length)} 行)</summary><pre class="log">${esc(log)}</pre></details>` : '';
  const sections = c?.sections?.length
    ? `<div class="convert-sections">${c.sections.map(s => `<span class="sec ${s.count ? 'has' : ''}" title="${esc(s.title)}"><b>${fmt(s.count)}</b> ${esc(s.title.replace(/\(.*$/, ''))}</span>`).join('')}</div>`
    : '';
  // Checked as the repository says (the advice), unless the user chose otherwise.
  const advice = state.advice;
  const buildOriginal = state.buildOriginalChoice ?? (advice ? advice.needed : !!c?.buildOriginal);
  const reasons = advice?.reasons ?? [];
  const adviceLine = !advice ? ''
    : advice.needed
      ? `<div class="advice need">自動判定: <b>必要</b> ・ ${reasons.slice(0, 3).map(esc).join(' ・ ')}${reasons.length > 3 ? ` ・ ほか ${reasons.length - 3} 件` : ''}</div>`
      : '<div class="advice">自動判定: 不要(ビルドでサイトを組み立てる仕組みは見つかりません。Web プロジェクトのフォルダーがそのままサイトです)</div>';
  const origin = `<label class="check"><input type="checkbox" id="build-original" ${buildOriginal ? 'checked' : ''}>
      元のアプリをそのビルド手順でビルドし、配置されるサイトから組み立てる <span class="muted">(--build-original。Windows と Visual Studio の MSBuild が必要。DNN など、ビルドでサイトを作るアプリ向け)</span></label>${adviceLine}`;
  const pill = s => { const k = CONVERSION[s]; return `<span class="pill" style="--c:${k.color}"><span class="ic" aria-hidden="true">${k.icon}</span>${esc(k.label)}</span>`; };

  let html;
  if (!c) {
    html = `<div class="convert-head"><div><h2>変換してビルド</h2>
        <div class="hint">保存した選択で .NET 10 のプロジェクトに変換し、そのままビルドします。ビルドできたら、Linux に配置できる形(Dockerfile と systemd 用のスクリプト付き)を ZIP でダウンロードできます。</div></div>
        <button class="btn primary" id="convert-start">▶ 変換してビルド</button></div>${origin}`;
  } else if (active) {
    html = `<div class="convert-head"><div><h2>変換してビルド ${pill(c.state)}</h2>
        <div class="hint">${c.state === 'queued' ? 'ほかの解析・変換が終わるのを待っています' : `${duration(c.started)} 経過。プロジェクトの変換、パッケージの復元、ビルドの順に進みます(数分かかります)`}</div></div>
        <button class="btn ghost danger" id="convert-cancel">中止</button></div>
      <div class="progress"></div>${logBlock(true)}`;
  } else {
    const done = c.state === 'done';
    html = `<div class="convert-head"><div><h2>変換してビルド ${pill(c.state)}</h2>
        <div class="hint">${esc(new Date(c.finished ?? c.started).toLocaleString('ja-JP'))} ・ ${duration(c.started, c.finished)}${c.buildOriginal ? ' ・ 元のビルドから' : ''}${c.error ? ` ・ ${esc(c.error)}` : ''}</div>
        ${body.stale ? '<div class="stale">⚠ この変換のあとに選択を保存しています。今の選択にするには、もう一度変換してください。</div>' : ''}</div>
        <div class="convert-actions">
          ${done ? '<button class="btn primary" id="convert-next">次へ: デプロイ →</button>' : ''}
          <button class="btn ghost" id="convert-start">↻ もう一度変換</button>
        </div></div>
      ${state.report?.text ? '' : sections}${origin}${logBlock(!done && !c.sections)}`;
  }
  root.innerHTML = html;
  root.style.setProperty('--c', c ? CONVERSION[c.state].color : 'var(--accent)');
  $('#build-original')?.addEventListener('change', ev => (state.buildOriginalChoice = ev.target.checked));
  $('#convert-start')?.addEventListener('click', () => startConversion().catch(e => toast(e.message)));
  $('#convert-cancel')?.addEventListener('click', () => cancelConversion().catch(e => toast(e.message)));
  $('#convert-next')?.addEventListener('click', () => goStep(4).catch(e => toast(e.message)));
  const pre = root.querySelector('.log');
  if (pre) pre.scrollTop = pre.scrollHeight;
}

// Whether the site is made by building the application (the "build the original" checkbox's default, and why).
async function loadAdvice() {
  const id = state.id;
  try {
    const advice = await api(`/analyses/${id}/original-build-advice`);
    if (id !== state.id) return;
    state.advice = advice;
  } catch {
    return;
  }
  renderConversion();
}

// The conversion's report (CONVERSION-REPORT.md), in ③ under the conversion: its sections, each one opened on demand.
async function loadReport() {
  const c = state.conversion?.conversion;
  const key = c?.sections ? `${state.id}:${c.finished ?? c.started}` : null;
  if (!key) { state.report = null; renderReport(); return; }
  if (state.report?.key === key) return;
  const response = await fetch(`/api/analyses/${state.id}/conversion/report`);
  state.report = { key, text: response.ok ? await response.text() : null };
  renderReport();
  renderConversion();  // its counts are the report's then
}

function renderReport() {
  const root = $('#report');
  if (!root) return;
  const text = state.report?.text;
  if (!text) { root.innerHTML = ''; return; }
  // "## <title>(<n> 件)" then "- **<subject>**: <text>" lines.
  const sections = [];
  for (const line of text.split('\n')) {
    const heading = line.match(/^## (.+?)\((\d+) 件\)\s*$/);
    if (heading) sections.push({ title: heading[1], count: +heading[2], items: [] });
    else if (line.startsWith('- ') && sections.length) sections.at(-1).items.push(line.slice(2));
  }
  const LIMIT = 300;
  const item = s => `<li>${esc(s).replace(/\*\*(.+?)\*\*/, '<b>$1</b>').replace(/`([^`]+)`/g, '<code>$1</code>')}</li>`;
  root.innerHTML = `<div class="card card-pad report">
      <div class="convert-head"><div><h2>変換レポート</h2>
        <div class="hint">変換が変えたところ、変えられなかったところ。「未解決」があれば、ビルドや実行の前に見てください。</div></div>
        <button class="btn ghost" id="report-full">レポート全体(CONVERSION-REPORT.md)</button></div>
      ${sections.map((s, i) => `<details class="report-section ${i === 0 && s.count ? 'attention' : ''}" ${i === 0 && s.count ? 'open' : ''}>
        <summary><b>${fmt(s.count)}</b> ${esc(s.title)}</summary>
        ${s.items.length ? `<ul>${s.items.slice(0, LIMIT).map(item).join('')}</ul>${s.items.length > LIMIT ? `<div class="muted">ほか ${fmt(s.items.length - LIMIT)} 件(レポート全体で)</div>` : ''}` : '<div class="muted">ありません</div>'}
      </details>`).join('')}
    </div>`;
  $('#report-full').addEventListener('click', () => showReport().catch(e => toast(e.message)));
}

// ④: the conversion's output to download.
function renderZip() {
  const root = $('#zip');
  if (!root) return;
  const c = state.conversion?.conversion;
  root.innerHTML = `<h3><span class="deploy-ic" aria-hidden="true">⤓</span> ZIP ダウンロード</h3>
    <div class="hint">変換の出力(サイト、Dockerfile、systemd 用のスクリプト)。配置の方法は ZIP の deploy/README.md にあります。</div>
    ${state.conversion?.stale ? '<div class="stale">⚠ この変換のあとに選択を保存しています。今の選択にするには、もう一度変換してください。</div>' : ''}
    <div class="deploy-actions">
      ${c?.state === 'done' ? `<a class="btn primary" href="/api/analyses/${esc(state.id)}/conversion/zip" download>⤓ ダウンロード <span class="size">${(c.zipSize / 1048576).toFixed(1)} MB</span></a>` : ''}
      ${state.command ? '<button class="btn ghost small" id="copy-command2" title="Studio の外で同じ変換をするコマンド">変換のコマンドをコピー</button>' : ''}
    </div>`;
  $('#copy-command2')?.addEventListener('click', async () => {
    await navigator.clipboard.writeText(state.command);
    toast('コマンドをコピーしました(--out の出力先を書き換えて実行)');
  });
}

// ---------------------------------------------------------------------------------------------------------------------
// Running on Linux in Docker: the conversion's Dockerfile built and run on a port of localhost.

const CONTAINER = {
  building: { label: 'イメージを作成中', icon: '⟳', color: 'var(--accent-2)' },
  starting: { label: '起動中', icon: '⟳', color: 'var(--accent-2)' },
  running: { label: 'Linux で実行中', icon: '●', color: 'var(--status-good)' },
  stopped: { label: '停止', icon: '■', color: 'var(--status-neutral)' },
  failed: { label: '失敗', icon: '✕', color: 'var(--status-critical)' },
};

async function loadContainer() {
  const id = state.id;
  clearTimeout(state.containerPoll);
  const body = await api(`/analyses/${id}/container`);
  if (id !== state.id) return;
  state.container = body;
  const k = body.container?.state;
  renderContainer();
  renderStepper();
  if (k === 'building' || k === 'starting') state.containerPoll = setTimeout(() => loadContainer().catch(console.error), 2000);
  else if (k === 'running') state.containerPoll = setTimeout(() => loadContainer().catch(console.error), 10000);
}

async function loadDocker() {
  clearTimeout(state.dockerPoll);
  const before = state.docker;
  state.docker = await api('/docker');
  if (state.docker.available) state.dockerStarting = false;
  // Drawn again only when it changed (the environment being typed in is not disturbed while Docker is asked again).
  const d = state.docker;
  if (!before || before.available !== d.available || before.reason !== d.reason || before.desktop !== d.desktop || state.dockerStarting) renderContainer();
  // While the panel is open and the engine does not answer (Docker Desktop starting, or started outside Studio): asked
  // again until it does.
  if (!d.available && (state.runOpen || state.dockerStarting)) state.dockerPoll = setTimeout(() => loadDocker().catch(console.error), 3000);
}

function renderContainer() {
  const root = $('#run');
  if (!root) return;
  const c = state.container?.container;
  const k = c?.state;
  const busy = k === 'building' || k === 'starting';
  const kind = k ? CONTAINER[k] : null;
  const head = `<div class="run-head"><h3><span class="deploy-ic" aria-hidden="true">⬢</span> コンテナ起動(Linux / Docker) ${kind ? `<span class="pill" style="--c:${kind.color}"><span class="ic" aria-hidden="true">${kind.icon}</span>${esc(kind.label)}</span>` : ''}</h3>`;
  const log = state.container?.log ?? [];
  const logBlock = log.length ? `<details class="convert-log" ${busy || k === 'failed' ? 'open' : ''}><summary>ログ(${fmt(log.length)} 行)</summary><pre class="log">${esc(log.join('\n'))}</pre></details>` : '';

  let html;
  if (busy) {
    html = `${head}<button class="btn ghost danger" id="run-stop">中止</button></div>
      <div class="hint">${k === 'building' ? 'Dockerfile からイメージを作っています(初回はベースイメージの取得で数分かかります)' : `コンテナを起動し、サイトが応答するのを待っています(${esc(c.url)}) ・ ${duration(c.started)} 経過<br>最初の表示は、ページのコンパイルやデータベースの作成で数分かかることがあります(10 分で打ち切ります)。`}</div>
      <div class="progress"></div>${logBlock}`;
  } else if (k === 'running') {
    const failing = c.firstStatus >= 500;
    html = `${head}<div class="convert-actions">
        <a class="btn primary" href="${esc(c.url)}" target="_blank" rel="noopener">↗ 開く</a>
        <button class="btn ghost" id="run-log">コンテナのログ</button>
        <button class="btn ghost danger" id="run-stop">停止</button></div></div>
      <div class="run-url"><code>${esc(c.url)}</code> <span class="muted">(${esc(c.image)} ・ 最初の応答 ${c.firstStatus})</span></div>
      ${failing ? '<div class="stale">⚠ サイトは動いていますが、エラー(500 番台)を返しています。データベースの接続文字列などを確かめてください(コンテナのログに詳細があります)。</div>' : ''}
      ${logBlock}`;
  } else {
    const d = state.docker;
    const docker = !d ? '<div class="hint">Docker を確認しています…</div>'
      : d.available ? `<div class="hint">Docker ${esc(d.version)} ・ 変換の出力の Dockerfile でイメージを作り、コンテナを localhost のポートで起動します(Studio のコンテナは一度に一つ)。</div>`
      : `<div class="stale">⚠ Docker のエンジンに接続できません。${d.desktop ? 'Docker Desktop を起動してください(起動すると自動で切り替わります)。' : 'Docker をインストールして起動してください。'}${d.reason ? `<br><span class="muted">${esc(d.reason)}</span>` : ''}</div>
         ${d.desktop ? `<button class="btn ghost small" id="docker-start" ${state.dockerStarting ? 'disabled' : ''}>${state.dockerStarting ? '起動を待っています…' : 'Docker Desktop を起動'}</button>` : ''}`;
    html = `${head}</div>
      ${c?.error ? `<div class="stale">${esc(c.error)}</div>` : ''}
      ${docker}
      <label class="env"><span>環境変数 <span class="muted">(<code>SQLCONNSTR_&lt;名前&gt;</code> で web.config の接続文字列、<code>APPSETTING_&lt;キー&gt;</code> で appSettings を置き換え)</span></span>
        <textarea id="run-env" rows="6" spellcheck="false">${esc(state.runEnv ?? state.container?.environment ?? '')}</textarea>
        <span class="muted">コンテナの中から見えるデータベースを指定します(Windows の LocalDB やこのマシンの localhost は見えません。このマシンの SQL Server なら host.docker.internal)。</span>
      </label>
      <div class="convert-actions left"><button class="btn primary" id="run-start" ${d?.available ? '' : 'disabled'}>▶ ビルドして起動</button></div>
      ${logBlock}`;
  }
  root.innerHTML = html;
  $('#run-env')?.addEventListener('input', ev => (state.runEnv = ev.target.value));
  $('#run-start')?.addEventListener('click', () => startContainer().catch(e => toast(e.message)));
  $('#run-stop')?.addEventListener('click', () => stopContainer().catch(e => toast(e.message)));
  $('#run-log')?.addEventListener('click', () => showContainerLog().catch(e => toast(e.message)));
  $('#docker-start')?.addEventListener('click', async () => {
    await api('/docker/start', { method: 'POST' });
    state.dockerStarting = true;
    await loadDocker();
  });
  const pre = root.querySelector('.log');
  if (pre) pre.scrollTop = pre.scrollHeight;
}

async function startContainer() {
  await api(`/analyses/${state.id}/container`, { method: 'POST', body: JSON.stringify({ environment: $('#run-env')?.value ?? null }) });
  state.runEnv = null;
  await loadContainer();
}

async function stopContainer() {
  if (!confirm('コンテナを止めますか?')) return;
  await api(`/analyses/${state.id}/container`, { method: 'DELETE' });
  await loadContainer();
  await loadDocker();
}

async function showContainerLog() {
  const text = await (await fetch(`/api/analyses/${state.id}/container/log`)).text();
  $('#drawer-title').textContent = 'docker logs';
  $('#drawer-sub').textContent = `${state.entry.name} のコンテナの出力(最後の 300 行)`;
  $('#drawer-code').innerHTML = (text || '(出力はありません)').split('\n').map(line => `<span class="ln">${esc(line)}</span>`).join('');
  $('#drawer').classList.add('open');
  $('#drawer').setAttribute('aria-hidden', 'false');
}

// ---------------------------------------------------------------------------------------------------------------------
// Running on this machine: the original application (① its test run: IIS Express or IIS) and the converted one (④ with
// dotnet). Both are drawn by one panel; Studio stops them when it stops.

const RUN = {
  building: { label: 'ビルド中', icon: '⟳', color: 'var(--accent-2)' },
  starting: { label: '起動中', icon: '⟳', color: 'var(--accent-2)' },
  running: { label: '実行中', icon: '●', color: 'var(--status-good)' },
  stopped: { label: '停止', icon: '■', color: 'var(--status-neutral)' },
  failed: { label: '失敗', icon: '✕', color: 'var(--status-critical)' },
};
const HOST = { iisexpress: 'IIS Express', iis: 'IIS', dotnet: 'dotnet' };

// One panel: what it runs (title, icon), how far it is, its log; idle: the explanation and the buttons to start.
function runnerPanel(prefix, title, icon, entry, log, idle, busyText) {
  const k = entry?.state;
  const busy = k === 'building' || k === 'starting';
  const kind = k ? RUN[k] : null;
  const head = `<div class="run-head"><h3><span class="deploy-ic" aria-hidden="true">${icon}</span> ${esc(title)} ${kind ? `<span class="pill" style="--c:${kind.color}"><span class="ic" aria-hidden="true">${kind.icon}</span>${esc(kind.label)}</span>` : ''}</h3>`;
  const logBlock = log?.length ? `<details class="convert-log" ${busy || k === 'failed' ? 'open' : ''}><summary>ログ(${fmt(log.length)} 行)</summary><pre class="log">${esc(log.join('\n'))}</pre></details>` : '';
  if (busy) {
    return `${head}<button class="btn ghost danger" id="${prefix}-stop">中止</button></div>
      <div class="hint">${busyText(entry)} ・ ${duration(entry.started)} 経過${k === 'starting' ? '<br>最初の表示は、ページのコンパイルやデータベースの作成で数分かかることがあります(10 分で打ち切ります)。' : ''}</div><div class="progress"></div>${logBlock}`;
  }
  if (k === 'running') {
    return `${head}<div class="convert-actions">
        <a class="btn primary" href="${esc(entry.url)}" target="_blank" rel="noopener">↗ 開く</a>
        <button class="btn ghost danger" id="${prefix}-stop">停止</button></div></div>
      <div class="run-url"><code>${esc(entry.url)}</code> <span class="muted">(${esc(HOST[entry.host] ?? entry.host ?? '')} ・ 最初の応答 ${entry.firstStatus} ・ ${esc(entry.site ?? '')})</span></div>
      ${entry.firstStatus >= 500 ? '<div class="stale">⚠ サイトは動いていますが、エラー(500 番台)を返しています。データベースの接続文字列などを確かめてください(ログに詳細があります)。</div>' : ''}
      ${logBlock}`;
  }
  return `${head}</div>${entry?.error ? `<div class="stale">${esc(entry.error)}</div>` : ''}${idle}${logBlock}`;
}

function poller(name, load, entryOf) {
  clearTimeout(state[name]);
  const k = entryOf()?.state;
  if (k === 'building' || k === 'starting') state[name] = setTimeout(() => load().catch(console.error), 2000);
  else if (k === 'running') state[name] = setTimeout(() => load().catch(console.error), 10000);
}

async function loadOriginal() {
  const id = state.id;
  const body = await api(`/analyses/${id}/original`);
  if (id !== state.id) return;
  state.original = body;
  renderOriginal();
  renderStepper();
  poller('originalPoll', loadOriginal, () => state.original?.original);
}

function renderOriginal() {
  const root = $('#original');
  if (!root || !state.original) return;
  const { original, built, environment, host, reason, log } = state.original;
  const idle = !host
    ? `<div class="hint">変換の前に、元のアプリ(.NET Framework)をこの PC で動かして、変換前の動きを確かめます。</div>
       <div class="stale">⚠ ${esc(reason ?? '')}</div>`
    : `<div class="hint">変換の前に、元のアプリ(.NET Framework)をこの PC で動かして、変換前の動きを確かめます。リポジトリのコピーをそのビルド手順(Visual Studio の MSBuild)でビルドし、${esc(HOST[host])} で起動します(Studio を止めると止まります)。</div>
       <label class="env"><span>環境変数 <span class="muted">(<code>SQLCONNSTR_&lt;名前&gt;</code> で web.config の接続文字列、<code>APPSETTING_&lt;キー&gt;</code> で appSettings を置き換え)</span></span>
         <textarea id="original-env" rows="5" spellcheck="false">${esc(state.originalEnv ?? environment ?? '')}</textarea>
         <span class="muted">.NET Framework は環境変数から設定を読まないので、接続文字列と appSettings はビルドしたサイトの web.config に書き込みます(元の web.config は残し、起動のたびにそこから作り直します)。そのほかの変数はプロセスに渡します。値の無いものは web.config のまま使います。</span>
       </label>
       <div class="convert-actions left">
         <button class="btn primary" id="original-start">▶ ${built ? '起動' : 'ビルドして起動'}</button>
         ${built ? '<button class="btn ghost" id="original-rebuild">↻ ビルドし直して起動</button>' : ''}
       </div>`;
  root.innerHTML = runnerPanel('original', '元のアプリを起動(テスト起動)', '▶', original, log, idle,
    e => e.state === 'building' ? '元のアプリをビルドしています(初回はパッケージの復元で数分かかります)' : `${esc(HOST[e.host] ?? '')} で起動し、サイトが応答するのを待っています(${esc(e.url ?? '')})`);
  $('#original-env')?.addEventListener('input', ev => (state.originalEnv = ev.target.value));
  const start = rebuild => api(`/analyses/${state.id}/original`, { method: 'POST', body: JSON.stringify({ rebuild, environment: $('#original-env')?.value ?? null }) })
    .then(() => { state.originalEnv = null; return loadOriginal(); }).catch(e => toast(e.message));
  $('#original-start')?.addEventListener('click', () => start(false));
  $('#original-rebuild')?.addEventListener('click', () => start(true));
  $('#original-stop')?.addEventListener('click', async () => {
    await api(`/analyses/${state.id}/original`, { method: 'DELETE' }).catch(e => toast(e.message));
    await loadOriginal();
  });
  const pre = root.querySelector('.log');
  if (pre) pre.scrollTop = pre.scrollHeight;
}

async function loadNative() {
  const id = state.id;
  const body = await api(`/analyses/${id}/native`);
  if (id !== state.id) return;
  state.native = body;
  renderNative();
  renderStepper();
  poller('nativePoll', loadNative, () => state.native?.native);
}

function renderNative() {
  const root = $('#native');
  if (!root || !state.native) return;
  const { native, environment, log } = state.native;
  const idle = `<div class="hint">変換したアプリを、この PC の .NET 10(dotnet)でそのまま起動します。Docker は使いません。</div>
    <label class="env"><span>環境変数 <span class="muted">(<code>SQLCONNSTR_&lt;名前&gt;</code> で web.config の接続文字列、<code>APPSETTING_&lt;キー&gt;</code> で appSettings を置き換え)</span></span>
      <textarea id="native-env" rows="5" spellcheck="false">${esc(state.nativeEnv ?? environment ?? '')}</textarea>
      <span class="muted">値の無いものは web.config のまま使います。この PC から見えるデータベースを指定します(LocalDB、<code>.\\SQLEXPRESS</code>、Docker の SQL Server なら <code>localhost,&lt;ポート&gt;</code>)。</span>
    </label>
    <div class="convert-actions left"><button class="btn primary" id="native-start">▶ 起動</button></div>`;
  root.innerHTML = runnerPanel('native', 'ネイティブ起動(この Windows)', '◆', native, log, idle,
    e => `dotnet で起動し、サイトが応答するのを待っています(${esc(e.url ?? '')})`);
  $('#native-env')?.addEventListener('input', ev => (state.nativeEnv = ev.target.value));
  $('#native-start')?.addEventListener('click', () =>
    api(`/analyses/${state.id}/native`, { method: 'POST', body: JSON.stringify({ environment: $('#native-env')?.value ?? null }) })
      .then(() => { state.nativeEnv = null; return loadNative(); }).catch(e => toast(e.message)));
  $('#native-stop')?.addEventListener('click', async () => {
    await api(`/analyses/${state.id}/native`, { method: 'DELETE' }).catch(e => toast(e.message));
    await loadNative();
  });
  const pre = root.querySelector('.log');
  if (pre) pre.scrollTop = pre.scrollHeight;
}

async function startConversion() {
  // What is converted is what is saved: unsaved changes are saved first.
  if (isDirty()) {
    await save();
    if (state.errors.length) return;
  }
  const buildOriginal = $('#build-original')?.checked ?? state.conversion?.conversion?.buildOriginal ?? false;
  await api(`/analyses/${state.id}/conversion`, { method: 'POST', body: JSON.stringify({ buildOriginal }) });
  await loadConversion();
  await loadAnalyses();
}

async function cancelConversion() {
  if (!confirm('変換を中止しますか?')) return;
  await api(`/analyses/${state.id}/conversion`, { method: 'DELETE' });
  await loadConversion();
}

async function showReport() {
  const response = await fetch(`/api/analyses/${state.id}/conversion/report`);
  if (!response.ok) throw new Error('レポートがありません');
  const text = await response.text();
  $('#drawer-title').textContent = 'CONVERSION-REPORT.md';
  $('#drawer-sub').textContent = `${state.entry.name} の変換の結果`;
  $('#drawer-code').innerHTML = text.split('\n').map(line => `<span class="ln ${line.startsWith('## ') ? 'hit' : ''}">${esc(line)}</span>`).join('');
  $('#drawer').classList.add('open');
  $('#drawer').setAttribute('aria-hidden', 'false');
}

// ---------------------------------------------------------------------------------------------------------------------
// Source

async function openSource(file, line, apiName) {
  const drawer = $('#drawer');
  $('#drawer-title').textContent = `${file}:${line}`;
  $('#drawer-sub').textContent = apiName;
  $('#drawer-code').innerHTML = '<span class="ln muted">読み込み中…</span>';
  drawer.classList.add('open');
  drawer.setAttribute('aria-hidden', 'false');
  try {
    const src = await api(`/analyses/${state.id}/source?file=${encodeURIComponent(file)}&line=${line}`);
    $('#drawer-code').innerHTML = src.lines.map((text, i) => {
      const no = src.start + i;
      return `<span class="ln ${no === line ? 'hit' : ''}"><span class="no">${no}</span>${esc(text)}</span>`;
    }).join('');
    $('.ln.hit')?.scrollIntoView({ block: 'center' });
  } catch {
    $('#drawer-code').innerHTML = '<span class="ln muted">ソースを読めませんでした(リポジトリが移動した可能性があります)</span>';
  }
}

function closeDrawer() {
  $('#drawer').classList.remove('open');
  $('#drawer').setAttribute('aria-hidden', 'true');
}

// ---------------------------------------------------------------------------------------------------------------------
// Choices: what changed, saving, the command

function normalize(c) {
  return { schema: c?.schema ?? 1, components: { ...(c?.components ?? {}) }, apis: { ...(c?.apis ?? {}) }, settings: { ...(c?.settings ?? {}) } };
}

// The choices as the conversion reads them: an option at its default is the same as none.
function effective(c) {
  const out = { components: {}, apis: { ...c.apis }, settings: {} };
  for (const comp of state.result?.components ?? []) {
    const option = c.components[comp.id];
    if (option && option !== defaultOf(comp.options ?? [])) out.components[comp.id] = option;
  }
  for (const s of state.result?.settings ?? []) {
    const option = c.settings[s.id];
    if (option && option !== defaultOf(s.options)) out.settings[s.id] = option;
  }
  return out;
}

function changes() {
  if (!state.choices || !state.saved) return 0;
  const a = effective(state.choices), b = effective(state.saved);
  let n = 0;
  for (const part of ['components', 'apis', 'settings']) {
    for (const k of new Set([...Object.keys(a[part]), ...Object.keys(b[part])])) if (a[part][k] !== b[part][k]) n++;
  }
  return n;
}
const isDirty = () => changes() > 0;

// The bar of ② (the choices are made there); the steps show what is saved.
function renderSavebar() {
  const bar = $('#savebar');
  renderStepper();
  renderStepNext();
  if (!state.result || state.step !== 2) { bar.hidden = true; return; }
  bar.hidden = false;
  const n = changes();
  const e = effective(state.choices);
  const nonDefault = Object.keys(e.components).length + Object.keys(e.apis).length + Object.keys(e.settings).length;
  // Saved (the defaults count: saving them as they are is a choice too): the conversion's command, to copy.
  $('#save-state').innerHTML = state.errors.length
    ? `<span class="errors">保存できません: ${esc(state.errors.join(' / '))}</span>`
    : n > 0
      ? `<b>未保存の変更 ${n} 件</b> ・ 既定と違う選択 ${nonDefault} 件`
      : `保存済み ・ 既定と違う選択 ${nonDefault} 件 ${state.command ? '・ <button class="btn ghost small" id="copy-command">変換のコマンドをコピー</button>' : ''}`;
  $('#save').textContent = n > 0 ? '選択を保存' : 'この選択で保存';
  $('#discard').disabled = n === 0;
  $('#copy-command')?.addEventListener('click', async () => {
    await navigator.clipboard.writeText(state.command);
    toast('コマンドをコピーしました(--out の出力先を書き換えて実行)');
  });
}

async function save() {
  try {
    await api(`/analyses/${state.id}/choices`, { method: 'PUT', body: JSON.stringify(state.choices) });
    state.saved = clone(state.choices);
    state.errors = [];
    state.command = (await api(`/analyses/${state.id}/command`)).command;
    toast('選択を保存しました');
  } catch (e) {
    state.errors = e.body?.errors ?? [e.message];
  }
  renderSavebar();
}

// ---------------------------------------------------------------------------------------------------------------------
// New analysis, delete

function openNew() {
  $('#new-error').hidden = true;
  $('#new-form').elements.root.placeholder = 'C:\\src\\MyApp';
  $('#new-dialog').showModal();
}

// ---------------------------------------------------------------------------------------------------------------------
// Choosing a project or a folder: the server lists the folders (a page gets no path from the browser's own dialog).

const KIND = {
  drive: { badge: '', cls: 'drive' },
  folder: { badge: '', cls: 'folder' },
  project: { badge: '', cls: 'project' },
  solution: { badge: 'SLN', cls: 'solution' },
};
const browse = { mode: 'project', listing: null, selected: null, filter: '', resolve: null };

function projectBadge(name) {
  return name.toLowerCase().endsWith('.vbproj') ? 'VB' : 'C#';
}

// Opens the chooser; resolves with the path chosen, or null.
function openBrowser(mode, start) {
  browse.mode = mode;
  $('#browse-title').textContent = mode === 'project' ? 'プロジェクトを選ぶ' : 'フォルダーを選ぶ';
  $('#browse-sub').textContent = mode === 'project'
    ? '.csproj / .vbproj をダブルクリック、または選んで「選ぶ」。プロジェクトのあるフォルダーには印が付きます。'
    : 'フォルダーを開いて「このフォルダーを選ぶ」。一覧のフォルダーを選んでから選ぶこともできます。';
  $('#browse-dialog').showModal();
  navigate(start || localStorage.getItem('foc-browse') || '');
  return new Promise(resolve => (browse.resolve = resolve));
}

function closeBrowser(path) {
  if ($('#browse-dialog').open) $('#browse-dialog').close();
  browse.resolve?.(path ?? null);
  browse.resolve = null;
}

async function navigate(path) {
  const list = $('#browse-list');
  list.classList.add('loading');
  try {
    browse.listing = await api('/browse?path=' + encodeURIComponent(path ?? ''));
  } catch (e) {
    browse.listing = { path, parent: null, entries: [], places: browse.listing?.places ?? [], error: e.message };
  }
  list.classList.remove('loading');
  browse.filter = '';
  $('#browse-filter').value = '';
  // A project file given (the field's value): it is selected in its folder.
  const given = path && browse.listing.entries.find(e => e.kind === 'project' && e.path.toLowerCase() === String(path).toLowerCase());
  browse.selected = given ?? null;
  if (browse.listing.path && !browse.listing.error) localStorage.setItem('foc-browse', browse.listing.path);
  renderBrowser();
  list.focus();
}

function visibleEntries() {
  const q = browse.filter.trim().toLowerCase();
  return (browse.listing?.entries ?? []).filter(e => !q || e.name.toLowerCase().includes(q));
}

function renderBrowser() {
  const listing = browse.listing;
  if (listing.path || !listing.error) $('#browse-path').value = listing.path ?? '';  // a path that failed stays to fix
  $('#browse-up').disabled = !listing.path;

  // Breadcrumbs: every folder up to the drive, each one a link.
  const crumbs = [];
  if (listing.path) {
    const parts = listing.path.split(/[\\/]/).filter(Boolean);
    const unix = listing.path.startsWith('/');
    let acc = unix ? '/' : '';
    parts.forEach((part, i) => {
      acc = unix ? (acc === '/' ? '/' + part : acc + '/' + part) : (i === 0 ? part + '\\' : acc.replace(/\\?$/, '\\') + part);
      crumbs.push(`<button type="button" class="crumb" data-path="${esc(acc)}">${esc(part)}</button>`);
    });
  }
  $('#browse-crumbs').innerHTML = `<button type="button" class="crumb" data-path="">PC</button>` + crumbs.map(c => `<span class="sep" aria-hidden="true">›</span>${c}`).join('');

  $('#browse-places').innerHTML = `<div class="places-label">場所</div>` + listing.places.map(p =>
    `<button type="button" class="place${listing.path?.toLowerCase() === p.path.toLowerCase() ? ' on' : ''}" data-path="${esc(p.path)}" title="${esc(p.path)}">${esc(p.label)}</button>`).join('')
    + `<button type="button" class="place${!listing.path ? ' on' : ''}" data-path="">ドライブ</button>`;

  const entries = visibleEntries();
  const list = $('#browse-list');
  if (listing.error) list.innerHTML = `<div class="browse-empty">${esc(listing.error)}</div>`;
  else if (entries.length === 0) list.innerHTML = `<div class="browse-empty">${browse.filter ? '一致するものがありません' : 'フォルダーもプロジェクトもありません'}</div>`;
  else list.innerHTML = entries.map((e, i) => {
    const kind = KIND[e.kind];
    const badge = e.kind === 'project' ? projectBadge(e.name) : kind.badge;
    const selectable = e.kind !== 'solution';
    return `<div class="entry ${kind.cls}${browse.selected?.path === e.path ? ' selected' : ''}${selectable ? '' : ' dim'}" role="option"
      aria-selected="${browse.selected?.path === e.path}" data-i="${i}">
      <span class="entry-ic" aria-hidden="true">${badge ? `<b>${esc(badge)}</b>` : ''}</span>
      <span class="entry-name">${esc(e.name)}</span>
      ${e.hasProject ? '<span class="entry-tag">プロジェクトあり</span>' : ''}
      ${e.kind === 'folder' || e.kind === 'drive' ? '<span class="entry-go" aria-hidden="true">›</span>' : ''}
    </div>`;
  }).join('');
  renderChoice();
}

// What "選ぶ" takes: the project selected; for a folder, the one selected or else the folder open.
function choice() {
  if (browse.mode === 'project') return browse.selected?.kind === 'project' ? browse.selected.path : null;
  if (browse.selected && (browse.selected.kind === 'folder' || browse.selected.kind === 'drive')) return browse.selected.path;
  return browse.listing?.path ?? null;
}

function renderChoice() {
  const path = choice();
  $('#browse-ok').disabled = !path;
  $('#browse-ok').textContent = browse.mode === 'root' && !(browse.selected && browse.selected.kind !== 'project') ? 'このフォルダーを選ぶ' : '選ぶ';
  $('#browse-choice').innerHTML = path ? `<span class="muted">選択:</span> <code>${esc(path)}</code>` : '<span class="muted">未選択</span>';
}

function selectEntry(entry) {
  if (!entry || entry.kind === 'solution') return;
  browse.selected = entry;
  $('#browse-list').querySelectorAll('.entry').forEach(el => {
    const on = visibleEntries()[+el.dataset.i]?.path === entry.path;
    el.classList.toggle('selected', on);
    el.setAttribute('aria-selected', on);
    if (on) el.scrollIntoView({ block: 'nearest' });
  });
  renderChoice();
}

function openEntry(entry) {
  if (!entry) return;
  if (entry.kind === 'folder' || entry.kind === 'drive') navigate(entry.path);
  else if (entry.kind === 'project' && browse.mode === 'project') closeBrowser(entry.path);
}

function wireBrowser() {
  const list = $('#browse-list');
  const entryOf = ev => {
    const el = ev.target.closest('.entry');
    return el ? visibleEntries()[+el.dataset.i] : null;
  };
  list.addEventListener('click', ev => selectEntry(entryOf(ev)));
  list.addEventListener('dblclick', ev => openEntry(entryOf(ev)));
  list.addEventListener('keydown', ev => {
    const entries = visibleEntries().filter(e => e.kind !== 'solution');
    const at = entries.findIndex(e => e.path === browse.selected?.path);
    if (ev.key === 'ArrowDown' || ev.key === 'ArrowUp') {
      ev.preventDefault();
      const next = ev.key === 'ArrowDown' ? Math.min(entries.length - 1, at + 1) : Math.max(0, at - 1);
      selectEntry(entries[next]);
    } else if (ev.key === 'Enter') {
      ev.preventDefault();
      if (browse.selected) openEntry(browse.selected);
    } else if (ev.key === 'Backspace') {
      ev.preventDefault();
      if (browse.listing?.path) navigate(browse.listing.parent ?? '');
    } else if (ev.key.length === 1 && !ev.ctrlKey && !ev.metaKey && !ev.altKey) {
      $('#browse-filter').focus();  // typing filters
    }
  });
  const go = ev => { const b = ev.target.closest('[data-path]'); if (b) navigate(b.dataset.path); };
  $('#browse-crumbs').addEventListener('click', go);
  $('#browse-places').addEventListener('click', go);
  $('#browse-up').addEventListener('click', () => navigate(browse.listing?.parent ?? ''));
  $('#browse-path').addEventListener('keydown', ev => { if (ev.key === 'Enter') { ev.preventDefault(); navigate(ev.target.value); } });
  $('#browse-filter').addEventListener('input', ev => { browse.filter = ev.target.value; renderBrowser(); });
  $('#browse-filter').addEventListener('keydown', ev => {
    if (ev.key === 'ArrowDown' || ev.key === 'Enter') {
      ev.preventDefault();
      const first = visibleEntries().find(e => e.kind !== 'solution');
      if (ev.key === 'Enter' && first && visibleEntries().filter(e => e.kind !== 'solution').length === 1) return openEntry(first);
      if (first) selectEntry(first);
      list.focus();
    }
  });
  $('#browse-ok').addEventListener('click', () => { const path = choice(); if (path) closeBrowser(path); });
  $('#browse-cancel').addEventListener('click', () => closeBrowser(null));
  $('#browse-close').addEventListener('click', () => closeBrowser(null));
  $('#browse-dialog').addEventListener('close', () => closeBrowser(null));

  // The pickers of the new analysis.
  document.querySelectorAll('[data-browse]').forEach(button => button.addEventListener('click', async () => {
    const form = $('#new-form');
    const field = form.elements[button.dataset.browse];
    const start = field.value || (button.dataset.browse === 'root' ? form.elements.project.value : '');
    const path = await openBrowser(button.dataset.browse, start);
    if (!path) return;
    field.value = path;
    if (button.dataset.browse === 'project') await suggestRoot(path);
  }));
  $('#new-form').elements.project.addEventListener('change', ev => suggestRoot(ev.target.value));
}

// The repository folder the analysis takes when none is given, shown in the empty field.
async function suggestRoot(project) {
  const root = $('#new-form').elements.root;
  try {
    const { root: found } = await api('/browse/root?project=' + encodeURIComponent(project));
    root.placeholder = `既定: ${found}`;
  } catch {
    root.placeholder = 'C:\\src\\MyApp';
  }
}

async function startAnalysis(request) {
  const entry = await api('/analyses', { method: 'POST', body: JSON.stringify(request) });
  await loadAnalyses();
  await select(entry.id);
  return entry;
}

async function removeAnalysis(id) {
  const a = state.analyses.find(x => x.id === id) ?? state.entry;
  const question = active(a)
    ? `「${a.name}」の解析を中止して削除しますか?`
    : `「${a.name}」を削除しますか?(解析の結果と選択のファイルが消えます)`;
  if (!confirm(question)) return;
  await api(`/analyses/${id}`, { method: 'DELETE' });
  await afterDelete([id]);
  toast(active(a) ? '解析を中止して削除しました' : '削除しました');
}

async function removeFailed() {
  const failed = state.analyses.filter(a => a.state === 'failed');
  if (!failed.length || !confirm(`失敗した解析 ${failed.length} 件をすべて削除しますか?`)) return;
  const { deleted } = await api('/analyses?state=failed', { method: 'DELETE' });
  await afterDelete(failed.map(a => a.id));
  toast(`${deleted} 件削除しました`);
}

// The list again; the analysis open, if it was deleted, gives way to the next one.
async function afterDelete(ids) {
  const current = ids.includes(state.id);
  if (current) {
    clearTimeout(state.poll);
    state.saved = state.choices;  // nothing left to save
  }
  await loadAnalyses();
  if (current) await select(state.analyses[0]?.id ?? null);
}

// ---------------------------------------------------------------------------------------------------------------------

function applyTheme(theme) {
  document.documentElement.dataset.theme = theme;
  localStorage.setItem('foc-theme', theme);
}

async function init() {
  applyTheme(localStorage.getItem('foc-theme') ?? 'dark');
  $('#theme-toggle').addEventListener('click', () => applyTheme(document.documentElement.dataset.theme === 'dark' ? 'light' : 'dark'));
  $('#new-analysis').addEventListener('click', openNew);
  $('#welcome-new').addEventListener('click', openNew);
  $('#drawer-close').addEventListener('click', closeDrawer);
  wireBrowser();
  $('#clear-failed').addEventListener('click', () => removeFailed().catch(e => toast(e.message)));
  document.addEventListener('keydown', ev => { if (ev.key === 'Escape') closeDrawer(); });
  $('#save').addEventListener('click', save);
  $('#discard').addEventListener('click', () => { state.choices = clone(state.saved); state.errors = []; renderResult(); });
  $('#reset-defaults').addEventListener('click', () => {
    state.choices = { schema: 1, components: {}, apis: {}, settings: {} };
    state.errors = [];
    renderResult();
  });
  $('#new-form').addEventListener('submit', async ev => {
    if (ev.submitter?.value !== 'ok') return;
    ev.preventDefault();
    const data = Object.fromEntries(new FormData(ev.target));
    try {
      $('#new-submit').disabled = true;
      await startAnalysis(data);
      $('#new-dialog').close();
      ev.target.reset();
    } catch (e) {
      $('#new-error').textContent = e.message;
      $('#new-error').hidden = false;
    } finally {
      $('#new-submit').disabled = false;
    }
  });
  addEventListener('beforeunload', ev => { if (isDirty()) ev.preventDefault(); });

  state.catalog = await api('/catalog');
  await loadAnalyses();
  const fromHash = location.hash.match(/^#\/a\/([^/]+)/)?.[1];
  if (fromHash && state.analyses.some(a => a.id === fromHash)) await select(fromHash);
  else if (state.analyses.length) await select(state.analyses[0].id);
}

init().catch(e => { console.error(e); toast('Studio に接続できません'); });
