# NavigationProbe's operations: the one table that writes
#   operations.json      the inventory ParityTest reads (each operation, its keys for ParityTest discover)
#   operations.md        the same, to read
#   parity-scenario.json the steps that perform every operation and take its snapshots (sample-parity.ps1)
#   Default.aspx         the index: a link to every entry URL (what discover crawls)
#   README.md            the cases and why each one is nasty
# and checks the inventory against the sample's source (static analysis: every interactive server control and handler
# of every page belongs to an operation). Edit the table and run: python samples/NavigationProbe/inventory.py
import html, json, os, re, sys
from collections import OrderedDict

HERE = os.path.dirname(os.path.abspath(__file__))
OPS = []
STEPS = []

# What a fetch / postraw shows of a body: the title, the "Object moved" text, the page's event log and side effects.
BODY = r'<title>[^<]*</title>|<h2>.*?</h2>|log: [^\n<]*|side: [^\n<]*|<p id="(?:flushed|before-end|after-end|before-complete|written|cleared|module|own)">[^<]*'
# What an async postback's answer shows: its delta's panels, redirect, form action, title, error.
DELTA = r'\|(?:updatePanel\|[^|]*|(?:pageRedirect|formAction|pageTitle)\|\|[^|]*|error\|\d+\|[^|]*)\|'
ASYNC = {'X-MicrosoftAjax': 'Delta=true', 'X-Requested-With': 'XMLHttpRequest'}


def goto(path): return {'action': 'goto', 'path': path}
# The snapshots of an error page (ASP.NET's yellow screen, IIS's detailed error): what the error is (the title, the status, the
# text before the source, the stack trace and the versions), not where: the paths and the stack are the machine's and the
# runtime's.
ERROR_SNAPS = {'transfer-handler', 'transfer-ashx', 'redirect-at-unload', 'redirect-flushed', 'redirect-newline', 'redirect-relative-query',
               'redirect-relative-frag', 'error-httperrors-missing', 'postback-tree-changed'}
def snap(name, error=False): return {'action': 'snapshot', 'name': name, **({'value': 'error'} if error or name in ERROR_SNAPS else {})}
def fetch(path, name, **more): return {'action': 'fetch', 'path': path, 'name': name, 'raw': True, 'bodyMatch': BODY, **more}
def click(target): return {'action': 'click', 'target': target}
def clicktext(text): return {'action': 'clicktext', 'value': text}
def fill(target, value): return {'action': 'fill', 'target': target, 'value': value}
def check(target): return {'action': 'check', 'target': target}
def press(target, key='Enter'): return {'action': 'press', 'target': target, 'value': key}
def script(code): return {'action': 'script', 'value': code}
def evaluate(target, code): return {'action': 'evaluate', 'target': target, 'value': code}
def setvalue(target, value): return {'action': 'setvalue', 'target': target, 'value': value}
def wait(ms): return {'action': 'waitMs', 'ms': ms}
def back(): return {'action': 'back'}
def forward(): return {'action': 'forward'}
def frame(name=''): return {'action': 'frame', 'value': name}
def popup(name): return {'action': 'popup', 'value': name}
def tab(name): return {'action': 'tab', 'value': name}
def download(target, name): return {'action': 'download', 'target': target, 'name': name}
def cookies(name): return {'action': 'cookies', 'name': name}
def newcontext(): return {'action': 'newcontext'}
def phase(text): return {'_phase': text}
def postraw(target, name, body=None, headers=None, match=BODY, **more):
    step = {'action': 'postraw', 'target': target, 'name': name, 'raw': True, 'bodyMatch': match, **more}
    if body: step['body'] = body
    if headers: step['headers'] = headers
    return step
def asyncpost(target, name, panel='up1', manager='sm'):
    return postraw(target, name, body=f'{manager}={panel}|{target}&__ASYNCPOST=true', headers=ASYNC, match=DELTA)


def op(id, category, page, element, expected, steps, why, keys=(), controls=(), role='any', pre=None, inputs=None,
       runnable=True, reason=None):
    OPS.append({k: v for k, v in OrderedDict(id=id, category=category, page=page, element=element, preconditions=pre, role=role,
                                             inputs=inputs, expected=expected, why=why, keys=list(keys) or None,
                                             controls=list(controls) or None, runnable=runnable, reason=reason).items() if v is not None})
    for step in steps:
        if 'action' in step:
            step = dict(step)
            step['op'] = id if 'op' not in step else step['op'] + ',' + id
        STEPS.append(step)


def k(page, *items):
    """Discover's keys of a page: GET, and 'submit:name', 'input:name type', 'postback:target', 'validator:id fn', 'link:/path'."""
    keys = ['GET ' + page]
    for item in items:
        kind, _, rest = item.partition(':')
        keys.append({'submit': 'SUBMIT', 'input': 'INPUT', 'postback': 'POSTBACK', 'validator': 'VALIDATOR'}[kind] + f' {page} {rest}'
                    if kind != 'link' else 'GET ' + rest)
    return keys


# ======================================================================= index
op('index.home', 'index', '/ (Default.aspx)', 'the index of every case', 'the links', [
    phase('0. The index (a new browser)'), goto('/'), snap('index'),
], 'nothing: the starting point (and what discover crawls)', keys=['GET /', 'GET /default.aspx'], controls=['Default.aspx'])

# ======================================================================= T: Server.Transfer / Execute
TS = '/t/source.aspx'
op('transfer.preserve-form', 'transfer', '/T/Source.aspx -> ~/T/Sub/Target.aspx', 'Server.Transfer(path, true) from a click',
   'the target in another folder runs; the address stays /T/Source.aspx; Request.Form and PreviousPage (FindControl, the typed property) are the source\'s',
   [phase('1. Server.Transfer, Server.Execute'), goto('/T/Source.aspx'), snap('transfer-source'), fill('txt', 'hello'), click('btnTrue'), snap('transfer-preserve-form')],
   'the target renders under the source\'s URL: ~ / relative URLs, the form\'s action, ResolveClientUrl and PreviousPage all depend on '
   'the executing vs the requested path (Context.Handler / PreviousHandler)', keys=k(TS, 'submit:btnTrue', 'input:txt text'), controls=['T/Source.aspx#txt', 'T/Source.aspx#btnTrue', 'T/Sub/Target.aspx'])
op('transfer.no-form', 'transfer', '/T/Source.aspx', 'Server.Transfer(path, false)', 'the target sees no form; PreviousPage still set',
   [goto('/T/Source.aspx'), fill('txt', 'not kept'), click('btnFalse'), snap('transfer-no-form')],
   'preserveForm false clears Request.Form for the target only', keys=k(TS, 'submit:btnFalse'), controls=['T/Source.aspx#btnFalse'])
op('transfer.query', 'transfer', '/T/Source.aspx', 'Server.Transfer("...?q=from-transfer&q=again", true)', 'the target\'s query is the transfer\'s (repeated key)',
   [goto('/T/Source.aspx'), click('btnQuery'), snap('transfer-query')],
   'the query string replaced by the transfer\'s while RawUrl stays the browser\'s', keys=k(TS, 'submit:btnQuery'), controls=['T/Source.aspx#btnQuery'])
op('transfer.relative-links', 'transfer', '/T/Sub/Target.aspx after a transfer', 'HyperLink NavigateUrl="Other.aspx", a href="Other.aspx" (plain and runat=server)',
   'HyperLink / runat=server resolve against the target\'s folder (T/Sub); the plain link against the browser\'s (T/) - a 404',
   [goto('/T/Source.aspx'), click('btnTrue'), click('lnkRelative'), snap('transfer-hyperlink-relative'),
    goto('/T/Source.aspx'), click('btnTrue'), click('aRelative'), snap('transfer-plain-relative', error=True),
    goto('/T/Source.aspx'), click('btnTrue'), click('aServer'), snap('transfer-server-anchor')],
   'the classic Transfer breakage: server-resolved URLs and browser-resolved URLs disagree', keys=['GET /t/sub/other.aspx'],
   controls=['T/Sub/Target.aspx#lnkRelative', 'T/Sub/Target.aspx#lnkTilde', 'T/Sub/Target.aspx#aServer', 'T/Sub/Target.aspx#img'])
op('transfer.target-postback', 'transfer', '/T/Sub/Target.aspx after a transfer', 'a postback from the transferred-to page',
   'the form posts to the action the target rendered (relative to the source\'s URL)',
   [goto('/T/Source.aspx'), click('btnTrue'), click('btnPost'), snap('transfer-target-postback')],
   'the form action after Transfer: a relative "Target.aspx"-style action resolved by the browser against the old URL', controls=['T/Sub/Target.aspx#btnPost'])
op('transfer.handler', 'transfer', '/T/Source.aspx', 'Server.Transfer(IHttpHandler, true)', 'the handler writes into the page\'s response; Context.Handler is the page',
   [goto('/T/Source.aspx'), click('btnHandler'), snap('transfer-handler')],
   'a handler object instead of a path: no page, no PreviousPage', keys=k(TS, 'submit:btnHandler'), controls=['T/Source.aspx#btnHandler'])
op('transfer.ashx', 'transfer', '/T/Source.aspx', 'Server.Transfer("~/T/Echo.ashx?via=transfer")', 'as .NET Framework: the .ashx through Transfer',
   [goto('/T/Source.aspx'), click('btnAshx'), snap('transfer-ashx')],
   'Transfer to a non-page handler path (the handler factory of .ashx)', keys=k(TS, 'submit:btnAshx'), controls=['T/Source.aspx#btnAshx', 'T/Echo.ashx'])
op('transfer.chain', 'transfer', '/T/Chain1.aspx -> Chain2 -> Sub/Target', 'nested Server.Transfer from Page_Load', 'three pages in one request; PreviousPage the last one',
   [goto('/T/Source.aspx'), click('btnChain'), snap('transfer-chain'), goto('/T/Chain1.aspx'), snap('transfer-chain-get'),
    click('btnPost'), snap('transfer-chain-postback')],
   'each transfer nests a child request; the log and PreviousPage show the order', keys=k(TS, 'submit:btnChain') + k('/t/chain1.aspx', 'submit:btnPost'),
   controls=['T/Source.aspx#btnChain', 'T/Chain1.aspx', 'T/Chain2.aspx'])
op('transfer.after-write', 'transfer', '/T/Source.aspx', 'Response.Write then Server.Transfer', 'what was written stays before the target\'s output',
   [goto('/T/Source.aspx'), click('btnWritten'), snap('transfer-after-write')],
   'Transfer does not clear the response', keys=k(TS, 'submit:btnWritten'), controls=['T/Source.aspx#btnWritten'])
op('transfer.catch-threadabort', 'transfer', '/T/Source.aspx', 'Server.Transfer inside try / catch (ThreadAbortException)',
   'on .NET Framework the abort is re-raised at the end of the catch: the code after the try never runs (the session shows what ran)',
   [goto('/T/Source.aspx'), click('btnCatch'), snap('transfer-catch-threadabort'), goto('/T/Sub/Other.aspx'), snap('transfer-catch-threadabort-side')],
   '.NET has no thread abort: code after a swallowed Response.End / Transfer runs unless the port re-raises it', keys=k(TS, 'submit:btnCatch'), controls=['T/Source.aspx#btnCatch'])
op('transfer.catch-exception', 'transfer', '/T/Source.aspx', 'Server.Transfer inside try / catch (Exception)', 'as above with catch (Exception): the type caught',
   [goto('/T/Source.aspx'), click('btnCatchAll'), snap('transfer-catch-exception'), goto('/T/Sub/Other.aspx'), snap('transfer-catch-exception-side')],
   'the exception type a broad catch sees (ThreadAbortException)', keys=k(TS, 'submit:btnCatchAll'), controls=['T/Source.aspx#btnCatchAll'])
op('transfer.transfer-request', 'transfer', '/T/Source.aspx', 'Server.TransferRequest("~/T/Sub/Target.aspx?tr=1", true)', 'a new pipeline for the target: no PreviousPage; the form kept',
   [goto('/T/Source.aspx'), fill('txt', 'request'), click('btnRequest'), snap('transfer-request')],
   'IIS integrated mode\'s child request (the whole pipeline again, Global.asax events too)', keys=k(TS, 'submit:btnRequest'), controls=['T/Source.aspx#btnRequest'])
op('transfer.missing', 'transfer', '/T/Source.aspx', 'Server.Transfer to a page that does not exist', 'the HttpException (404 inside a transfer: 500? as the original)',
   [goto('/T/Source.aspx'), click('btnMissing'), snap('transfer-missing', error=True)],
   'the error of a child request', keys=k(TS, 'submit:btnMissing'), controls=['T/Source.aspx#btnMissing'])
TE = '/t/exec.aspx'
for mode, expected, why in (
        ('load', 'the executed page\'s output first, before the page\'s own <html>', 'Execute writes straight into the response, at the time it is called'),
        ('render', 'the executed page\'s output inside the div, during Render', 'Execute from inline code, in the middle of the page\'s output'),
        ('writer', 'the output captured by a StringWriter, shown encoded', 'Execute(path, TextWriter): another writer for the child page'),
        ('ashx', 'an .ashx executed into a writer', 'Execute of a handler path'),
        ('redirect', 'the executed page\'s Response.Redirect redirects the whole request', 'Response.Redirect inside a child request (who ends?)'),
        ('transfer', 'the executed page\'s Server.Transfer', 'Transfer inside Execute'),
        ('throw', 'HttpException "Error executing child request" with the inner exception', 'the wrapping of a child request\'s exception'),
        ('missing', 'the HttpException of a missing page', 'Execute of a page that does not exist')):
    steps = [fetch(f'/T/Exec.aspx?mode={mode}', f'execute-{mode}-raw'), goto(f'/T/Exec.aspx?mode={mode}'), snap(f'execute-{mode}')]
    if mode == 'redirect':
        steps += [goto('/T/Sub/Other.aspx'), snap('execute-redirect-side')]
    op(f'execute.{mode}', 'transfer', f'/T/Exec.aspx?mode={mode}', f'Server.Execute ({mode})', expected, steps, why,
       keys=['GET /t/exec.aspx?mode'], controls=['T/Exec.aspx', 'T/Part.aspx', 'T/PartRedirect.aspx', 'T/PartTransfer.aspx', 'T/PartThrow.aspx'])
op('execute.preserve-form', 'transfer', '/T/Exec.aspx', 'Server.Execute(path, writer, preserveForm true / false) on a postback', 'the child sees the form or not',
   [goto('/T/Exec.aspx'), fill('txt', 'posted'), click('btnPreserve'), snap('execute-preserve-true'), click('btnNoPreserve'), snap('execute-preserve-false')],
   'preserveForm on Execute', keys=k(TE, 'submit:btnPreserve', 'submit:btnNoPreserve', 'input:txt text'),
   controls=['T/Exec.aspx#txt', 'T/Exec.aspx#btnPreserve', 'T/Exec.aspx#btnNoPreserve'])

# ======================================================================= X: cross-page postback
XS = '/x/source.aspx'
op('cross.same-folder', 'cross-page', '/X/Source.aspx -> /X/Target.aspx', 'Button PostBackUrl; the target\'s PreviousPageType', 'the target: IsCrossPagePostBack of the source, the typed PreviousPage.NameTyped, IsValid',
   [phase('2. Cross-page postbacks'), goto('/X/Source.aspx'), snap('cross-source'), fill('txtName', 'Ann'), click('btnSame'), snap('cross-same-folder')],
   'the source page is re-created (its LoadComplete) inside the target\'s request; PreviousPageType makes a typed property in generated code',
   keys=k(XS, 'submit:btnSame', 'input:txtName text'), controls=['X/Source.aspx#txtName', 'X/Source.aspx#btnSame', 'X/Target.aspx'])
op('cross.validation', 'cross-page', '/X/Source.aspx -> /X/Target.aspx', 'cross-page postback with an empty required field (server validation only)', 'PreviousPage.IsValid false: the target decides',
   [goto('/X/Source.aspx'), click('btnSame'), snap('cross-validation-failed')],
   'validation runs on the source page when the target asks PreviousPage.IsValid', controls=['X/Source.aspx#rfvName'])
op('cross.no-validation', 'cross-page', '/X/Source.aspx', 'CausesValidation=false with PostBackUrl', 'IsValid without validation: the exception or true',
   [goto('/X/Source.aspx'), click('btnNoValidation'), snap('cross-no-validation')],
   'Page.IsValid before Validate()', keys=k(XS, 'submit:btnNoValidation'), controls=['X/Source.aspx#btnNoValidation'])
op('cross.other-folder', 'cross-page', '/X/Source.aspx -> /X/Other/Target.aspx', 'PostBackUrl to a page in another folder (no PreviousPageType)', 'FindControl on PreviousPage; relative URLs of the target',
   [goto('/X/Source.aspx'), fill('txtName', 'Bob'), click('btnOther'), snap('cross-other-folder'), click('lnkRel'), snap('cross-other-folder-link')],
   'the browser\'s URL is the target\'s, unlike Transfer', keys=k(XS, 'submit:btnOther'), controls=['X/Source.aspx#btnOther', 'X/Other/Target.aspx#lnkRel', 'X/Other/Target.aspx#aRel'])
op('cross.query', 'cross-page', '/X/Source.aspx', 'PostBackUrl with a query', 'the target sees via=query',
   [goto('/X/Source.aspx'), fill('txtName', 'Cy'), click('btnQuery'), snap('cross-query')],
   'WebForm_DoPostBackWithOptions sets the form\'s action with the query', keys=k(XS, 'submit:btnQuery'), controls=['X/Source.aspx#btnQuery'])
op('cross.linkbutton', 'cross-page', '/X/Source.aspx', 'LinkButton PostBackUrl', 'the same as the button, through the script',
   [goto('/X/Source.aspx'), fill('txtName', 'Di'), click('lnkCross'), snap('cross-linkbutton')],
   '__doPostBack with options and __PREVIOUSPAGE', keys=k(XS, 'postback:lnkCross'), controls=['X/Source.aspx#lnkCross'])
op('cross.target-postback', 'cross-page', '/X/Target.aspx', 'a normal postback on the target after a cross-page postback', 'PreviousPage gone, IsPostBack true',
   [goto('/X/Source.aspx'), fill('txtName', 'Ed'), click('btnSame'), click('btnBack'), snap('cross-target-postback')],
   'the target\'s own view state after a cross-page arrival', keys=k('/x/target.aspx', 'submit:btnBack'), controls=['X/Target.aspx#btnBack'])
op('cross.target-direct', 'cross-page', '/X/Target.aspx', 'GET of the target', 'no previous page', [goto('/X/Target.aspx'), snap('cross-target-direct')],
   'the typed PreviousPage null', keys=['GET /x/target.aspx'])
op('cross.master', 'cross-page', '/X/MasterSource.aspx (master A) -> /X/MasterTarget.aspx (master B)', 'PostBackUrl between pages of different masters',
   'PreviousPage.FindControl misses the content control; Master.FindControl(main).FindControl finds it; each master\'s relative URLs',
   [goto('/X/MasterSource.aspx'), snap('cross-master-source'), click('btnCross'), snap('cross-master')],
   'naming containers of the content placeholder; masters in another folder', keys=['GET /x/mastersource.aspx', 'SUBMIT /x/mastersource.aspx ctl00$main$btnCross', 'INPUT /x/mastersource.aspx ctl00$main$txtName text',
                                                                                     'GET /masters/other.aspx', 'GET /default.aspx'],
   controls=['X/MasterSource.aspx#txtName', 'X/MasterSource.aspx#btnCross', 'X/MasterSource.aspx#lnkRel', 'X/MasterTarget.aspx', 'Masters/A.master#lnkHome',
             'Masters/A.master#aMasterRel', 'Masters/A.master#imgMaster', 'Masters/B.master#lnkHome', 'Masters/B.master#aMasterRel', 'Masters/B.master#imgMaster'])
op('cross.plain-postback', 'cross-page', '/X/Source.aspx', 'a button without PostBackUrl on the source', 'an ordinary postback (the validator runs)',
   [goto('/X/Source.aspx'), fill('txtName', 'Fay'), click('btnHere'), snap('cross-plain-postback')], 'the same page without cross-page',
   keys=k(XS, 'submit:btnHere'), controls=['X/Source.aspx#btnHere'])

# ======================================================================= R: Response.Redirect
STAGES = ['PreInit', 'Init', 'InitComplete', 'PreLoad', 'Load', 'Click', 'LoadComplete', 'PreRender', 'PreRenderComplete', 'SaveStateComplete', 'Render', 'Unload']
for i, stage in enumerate(STAGES):
    s = stage.lower()
    if stage == 'Click':
        steps = [goto('/R/Life.aspx?at=Click'), postraw('btnGo', 'redirect-at-click-raw'), click('btnGo'), snap('redirect-at-click'),
                 goto('/R/Life.aspx?at=Click&end=false'), postraw('btnGo', 'redirect-at-click-false-raw'), click('btnGo'), snap('redirect-at-click-false')]
    else:
        steps = [fetch(f'/R/Life.aspx?at={stage}', f'redirect-at-{s}-raw'), fetch(f'/R/Life.aspx?at={stage}&end=false', f'redirect-at-{s}-false-raw'),
                 goto(f'/R/Life.aspx?at={stage}'), snap(f'redirect-at-{s}')]
    if i == 0:
        steps = [phase('3. Response.Redirect in each event, then every kind of redirect')] + steps
    op(f'redirect.at-{s}', 'redirect', f'/R/Life.aspx?at={stage}', f'Response.Redirect in {stage} (endResponse true, then false)',
       'true: 302, the rest of the page not run; false: 302 with the page\'s output after the "Object moved" body, the events after it run',
       steps, 'each event of the page\'s lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone)',
       keys=['GET /r/life.aspx?at', 'GET /r/life.aspx?at&end'] + (['SUBMIT /r/life.aspx btnGo'] if stage == 'Click' else []),
       controls=['R/Life.aspx#btnGo', 'R/Life.aspx', 'R/Landing.aspx'] if stage == 'Click' else ['R/Life.aspx'])

CASES = [
    ('tae', 'Redirect in try / catch (ThreadAbortException): the code after the try', 'the redirect; on .NET Framework neither the code after the try nor the rest runs (the session shows what did)', 'thread abort emulation'),
    ('ex', 'Redirect in try / catch (Exception)', 'the type caught; the code after the try (the session)', 'a broad catch swallowing the end of the response'),
    ('finally', 'Redirect in try / finally', 'the finally runs; the redirect', 'finally blocks during the abort'),
    ('written', 'Response.Write then Redirect', 'the redirect: what was written is cleared', 'a buffered response is cleared'),
    ('flushed', 'Response.Write, Flush, then Redirect', 'HttpException "Cannot redirect after HTTP headers have been sent" after the flushed text (200)', 'headers sent: the redirect fails mid-page'),
    ('permanent', 'RedirectPermanent', '301', 'the permanent redirect\'s status'),
    ('permanent-false', 'RedirectPermanent(url, false)', '301 with the page after it', 'endResponse false on the permanent redirect'),
    ('route', 'RedirectToRoute("NavRoute", id=3)', '302 to /nav/3', 'the route\'s URL from values'),
    ('route-permanent', 'RedirectToRoutePermanent with an extra value (space)', '301 to /nav/4?extra=x%20y', 'the extra value as a query, its encoding'),
    ('route-name', 'RedirectToRoute("FilesRoute", rest="a/b c")', 'the catch-all route\'s URL (the slash kept, the space escaped)', 'catch-all parameters\' encoding'),
    ('external', 'Redirect to an absolute external URL (a space in the query)', '302 to http://external.example/... (the browser goes there: its own error page, .example names do not resolve)', 'absolute URLs left as they are'),
    ('protocol-relative', 'Redirect to //external.example/relative', 'the scheme-relative Location', 'a URL that looks relative but leaves the site'),
    ('absolute-path', 'Redirect to /R/Landing.aspx', 'the path as given', 'an absolute path'),
    ('unicode', 'Redirect with Unicode, a space, % and & in the query', 'the encoding of the Location header (non-ASCII, space, a lone %)', 'UrlEncodeRedirect / UrlPathEncode of the query'),
    ('encoded', 'Redirect with an already escaped query (%26 %E6 %2B)', 'escapes kept, not escaped twice', 'double encoding'),
    ('unicode-path', 'Redirect to ~/R/着地.aspx', 'the path escaped (UTF-8)', 'non-ASCII file names (Linux file system, the escaping)'),
    ('space-path', 'Redirect to ~/R/With Space.aspx?from=space path', 'the space in the path and the query', 'spaces in paths'),
    ('newline', 'Redirect to a URL with CR LF (header injection)', 'refused: ArgumentException / the URL sanitised, as the original', 'response splitting protection'),
    ('empty', 'Redirect("")', 'as the original (to the folder? an exception?)', 'the edge of URL resolution'),
    ('end', 'Response.Write, Response.End, Write', 'only the text before End', 'Response.End without a redirect'),
    ('complete', 'Write, CompleteRequest, the page goes on', 'the page still renders (CompleteRequest only skips the pipeline)', 'CompleteRequest is not End'),
    ('false-complete', 'Redirect(url, false) + CompleteRequest (the recommended pattern)', '302 with the page rendered after the redirect\'s body', 'the documented alternative to Redirect(url) still renders the page'),
    ('status200', 'Redirect(url, false), then StatusCode = 200 in PreRender', '200 with a Location header and the page', 'a status overridden after the redirect'),
    ('status301', 'Redirect(url, false), then StatusCode = 301', '301 with the Location', 'the status changed, the Location kept'),
    ('status404', 'Redirect(url, false), then StatusCode = 404 (TrySkipIisCustomErrors)', '404 with a Location header', 'a redirect turned into an error'),
    ('twice', 'Redirect(a, false) then Redirect(b)', 'the second wins', 'the Location replaced'),
    ('clear-headers', 'Redirect(url, false) then ClearHeaders', 'no Location: 200? as the original', 'the redirect\'s headers removed'),
]
for case, element, expected, why in CASES:
    path = f'/R/Cases.aspx?c={case}'
    steps = [fetch(path, f'redirect-{case}-raw'), goto(path), snap(f'redirect-{case}')]
    if case in ('tae', 'ex', 'finally', 'flushed', 'end', 'complete'):
        steps += [goto('/R/Landing.aspx?from=side'), snap(f'redirect-{case}-side')]
    op(f'redirect.{case}', 'redirect', path, element, expected, steps, why, keys=['GET /r/cases.aspx?c'], controls=['R/Cases.aspx'])
for case, target in (('parent', '../Landing.aspx'), ('file', 'Landing.aspx'), ('dot', './Landing.aspx'), ('tilde', '~/R/Landing.aspx'), ('query', '?from=query-only'),
                     ('frag', '#frag'), ('query-frag', 'Landing.aspx?from=query-frag#section'), ('tilde-frag', '~/R/Landing.aspx#top'), ('parent-parent', '../../Default.aspx')):
    path = f'/R/Sub/Rel.aspx?c={case}'
    op(f'redirect.relative-{case}', 'redirect', path, f'Response.Redirect("{target}") from R/Sub', 'the Location as the original made it absolute; the browser\'s address (the fragment kept)',
       [fetch(path, f'redirect-relative-{case}-raw'), goto(path), snap(f'redirect-relative-{case}')],
       'relative URL resolution of Redirect (against the request\'s folder), fragments', keys=['GET /r/sub/rel.aspx?c'], controls=['R/Sub/Rel.aspx', 'R/Sub/Landing.aspx'])

# ======================================================================= G: Global.asax, module, Application_Error, loops
for id, path, element, expected, why in (
        ('begin-request', '/G/Begin.aspx', 'Response.Redirect in Application_BeginRequest', '302 before anything else ran', 'the earliest event: no handler, no session'),
        ('authenticate-request', '/G/Auth.aspx', 'Response.Redirect in Application_AuthenticateRequest', '302', 'during authentication'),
        ('post-acquire-state', '/G/Acquire.aspx', 'Response.Redirect in Application_PostAcquireRequestState (with the session)', '302; the session available', 'the session\'s event'),
        ('end-request', '/G/End.aspx', 'Response.Redirect in Application_EndRequest after the page ran', '302 replacing the page\'s output', 'a redirect at the very end (headers not sent yet)'),
        ('module-redirect', '/G/Module.aspx', 'an IHttpModule\'s Response.Redirect in PostAuthorizeRequest', '302', 'a module of web.config\'s system.webServer/modules'),
        ('module-complete', '/G/ModuleEnd.aspx', 'a module answers and calls CompleteRequest', 'the module\'s answer; the page never runs', 'CompleteRequest from a module'),
        ('error-redirect', '/G/ErrRedirect.aspx', 'Application_Error: ClearError + Redirect', '302 to the landing with the exception type', 'error handling by redirect'),
        ('error-redirect-kept', '/G/ErrRedirectKeep.aspx', 'Application_Error: Redirect without ClearError', 'the redirect (or the error page?) as the original', 'the error still pending when redirecting'),
        ('error-transfer', '/G/ErrTransfer.aspx', 'Application_Error: ClearError + Server.Transfer', 'the handled page under the failing URL', 'Transfer outside the page\'s execution'),
        ('error-execute', '/G/ErrExecute.aspx', 'Application_Error: ClearError + Clear + Server.Execute', 'the handled page\'s output', 'Execute from an application event'),
        ('error-clear', '/G/ErrClear.aspx', 'Application_Error: ClearError + Write', '200 with only the written text', 'swallowing an error')):
    op(f'global.{id}', 'global', path, element, expected,
       ([phase('4. Global.asax, a module, Application_Error, loops')] if id == 'begin-request' else []) +
       [fetch(path, f'global-{id}-raw'), goto(path), snap(f'global-{id}')], why, keys=['GET ' + path.lower()],
       controls=['Global.asax', 'G/' + path.split('/')[-1], 'G/Landing.aspx', 'G/Handled.aspx'])
op('global.redirect-loop', 'global', '/G/LoopA.aspx <-> /G/LoopB.aspx', 'a redirect loop', 'each hop the same; the browser gives up (ERR_TOO_MANY_REDIRECTS)',
   [{**fetch('/G/LoopA.aspx', 'global-redirect-loop-hops'), 'follow': True}, goto('/G/LoopA.aspx'), snap('global-redirect-loop')],
   'relative Location values ("LoopB.aspx") resolved hop after hop', keys=['GET /g/loopa.aspx'], controls=['G/LoopA.aspx', 'G/LoopB.aspx'])
op('global.redirect-chain', 'global', '/G/Chain.aspx?n=19 / ?n=25', 'a chain of 19 redirects (the browser follows) and of 25 (it does not)', 'the end of the chain / the browser\'s error',
   [{**fetch('/G/Chain.aspx?n=25', 'global-redirect-chain-hops'), 'follow': True}, goto('/G/Chain.aspx?n=19'), snap('global-redirect-chain-19'),
    goto('/G/Chain.aspx?n=25'), snap('global-redirect-chain-25')],
   'the browser\'s redirect limit (20)', keys=['GET /g/chain.aspx?n'], controls=['G/Chain.aspx'])

# ======================================================================= E: errors
for id, path, element, expected, why, error in (
        ('redirect-mode', '/E/Redirect/Throw.aspx', 'customErrors On, ResponseRedirect: an exception', '302 to ~/E/Oops.aspx?aspxerrorpath=/E/Redirect/Throw.aspx', 'the error page by redirect', False),
        ('redirect-404', '/E/Redirect/Http404.aspx', 'HttpException(404) under ResponseRedirect with an <error statusCode="404">', '302 to ~/E/NotFound.aspx?aspxerrorpath=...', 'per-status pages', False),
        ('redirect-403', '/E/Redirect/Http403.aspx', 'HttpException(403) without its own page', '302 to the default page', 'the status without an entry', False),
        ('redirect-missing', '/E/Redirect/Missing.aspx', 'a missing .aspx under customErrors On', '302 to the 404 page', 'the page handler factory\'s 404', False),
        ('rewrite-mode', '/E/Rewrite/Throw.aspx', 'customErrors ResponseRewrite: an exception', '500 with Oops.aspx\'s content under the failing URL (GetLastError available)', 'Server.Execute of the error page', False),
        ('rewrite-404', '/E/Rewrite/Http404.aspx', 'HttpException(404) under ResponseRewrite', '404 with NotFound.aspx\'s content', 'the per-status page rewritten', False),
        ('rewrite-missing', '/E/Rewrite/Missing.aspx', 'a missing page under ResponseRewrite', '404 with NotFound.aspx\'s content', 'rewrite for the 404 of the factory', False),
        ('error-page-throws', '/E/Broken/Throw.aspx', 'the error page itself throws (ResponseRewrite)', 'the generic runtime error page', 'an error inside the error page', True),
        ('no-default', '/E/Plain/Throw.aspx', 'customErrors On without defaultRedirect', 'the generic "Runtime Error" page (no details)', 'what a remote user sees', True),
        ('detailed', '/E/Throw.aspx', 'customErrors Off: an exception', 'the detailed error page (500)', 'the yellow screen', True),
        ('httpexception-404', '/E/Http404.aspx', 'HttpException(404) with customErrors Off', 'the detailed 404 page', 'HttpException\'s status', True),
        ('missing-page', '/E/NoSuchPage.aspx', 'a missing .aspx', 'ASP.NET\'s 404', 'the resource cannot be found', True),
        ('compile', '/E/Compile.aspx', 'a page whose inline code does not compile', 'Compilation Error (CS0029)', 'the server\'s compiler and its error page', True),
        ('parser', '/E/Parser.aspx', 'a page with an unknown control', 'Parser Error', 'the parser\'s error page', True),
        ('bad-directive', '/E/BadDirective.aspx', 'a page inheriting a class that does not exist', 'Parser Error (could not load type)', 'a broken Inherits', True),
        ('httperrors-replaced', '/E/Http/Status404.aspx', 'httpErrors Custom / existingResponse Auto: the page sets 404 without TrySkipIisCustomErrors', 'IIS runs /E/NotFound.aspx (ExecuteURL) instead of the page\'s body', 'IIS\'s error pages over the application\'s answer', False),
        ('httperrors-skip', '/E/Http/Skip404.aspx', 'the same with TrySkipIisCustomErrors', 'the page\'s own 404 body', 'TrySkipIisCustomErrors under Auto', False),
        ('httperrors-missing', '/E/Http/NoSuch.aspx', 'a missing page under httpErrors Custom', 'as the original: ASP.NET\'s 404 or IIS\'s ExecuteURL', 'which layer answers the 404', False)):
    op(f'error.{id}', 'error', path, element, expected,
       ([phase('5. Errors: customErrors, httpErrors, error pages')] if id == 'redirect-mode' else []) +
       [fetch(path, f'error-{id}-raw'), goto(path), snap(f'error-{id}', error=error)], why, keys=['GET ' + path.lower()],
       controls=['E/' + path[3:], 'E/Oops.aspx', 'E/NotFound.aspx'] if 'Missing' not in path and 'NoSuch' not in path else ['E/Oops.aspx', 'E/NotFound.aspx'])
for name, expected in (('Async', 'the async error redirects the browser to the defaultRedirect (pageRedirect)'), ('AsyncNoRedirect', 'the generic async error message (AllowCustomErrorsRedirect false)')):
    op(f'error.{name.lower()}', 'error', f'/E/Redirect/{name}.aspx', 'an async postback that throws under customErrors On', expected,
       [goto(f'/E/Redirect/{name}.aspx'), asyncpost('btnThrow', f'error-{name.lower()}-raw', panel='up'), click('btnThrow'), snap(f'error-{name.lower()}')],
       'customErrors and the page request manager', keys=k(f'/e/redirect/{name.lower()}.aspx', 'submit:btnThrow'), controls=[f'E/Redirect/{name}.aspx#btnThrow'])

# ======================================================================= F: forms authentication, session
op('forms.protected-redirect', 'forms', '/F/Secure/Page.aspx (anonymous)', 'a page of a folder denied to anonymous users', '302 to ~/F/Login.aspx?ReturnUrl=%2fF%2fSecure%2fPage.aspx',
   [phase('6. Forms authentication, the session (a new browser)'), newcontext(), fetch('/F/Secure/Page.aspx', 'forms-protected-raw'), goto('/F/Secure/Page.aspx'), snap('forms-protected-login')],
   'the authorization module\'s 401 turned into the login redirect (ReturnUrl\'s encoding)', keys=['GET /f/secure/page.aspx', 'GET /f/login.aspx?returnurl'])
op('forms.return-url-query-unicode', 'forms', '/F/Secure/Page.aspx?a=1&b=日本&c=x%26y', 'the ReturnUrl of a URL with a query, Unicode and an escaped &', 'the ReturnUrl escaped once, as the original',
   [fetch('/F/Secure/Page.aspx?a=1&b=%E6%97%A5%E6%9C%AC&c=x%26y', 'forms-return-url-unicode-raw'), goto('/F/Secure/Page.aspx?a=1&b=%E6%97%A5%E6%9C%AC&c=x%26y'), snap('forms-return-url-unicode')],
   'nested encoding of the ReturnUrl')
op('forms.secure-route', 'forms', '/secure/5 (anonymous)', 'a route to a page of the protected folder', 'as the original: URL authorization checks the route\'s URL, not the page\'s file',
   [fetch('/secure/5', 'forms-secure-route-raw'), goto('/secure/5'), snap('forms-secure-route')],
   'the gap between URL authorization and routes', keys=['GET /secure/*'])
op('forms.public-page', 'forms', '/F/Secure/Public.aspx', 'a page allowed by its own <location> inside the denied folder', '200 for anonymous',
   [goto('/F/Secure/Public.aspx'), snap('forms-public')], 'location rules: the most specific wins', keys=['GET /f/secure/public.aspx'], controls=['F/Secure/Public.aspx'])
op('forms.login-wrong', 'forms', '/F/Login.aspx', 'a wrong password', 'the message; no cookie',
   [goto('/F/Secure/Page.aspx?a=1&b=%E6%97%A5%E6%9C%AC'), fill('UserName', 'user'), fill('Password', 'wrong'), click('btnLogin'), snap('forms-login-wrong')],
   'FormsAuthentication.Authenticate against web.config\'s credentials', keys=k('/f/login.aspx', 'submit:btnLogin', 'input:UserName text', 'input:Password password', 'input:Persist checkbox'),
   controls=['F/Login.aspx#UserName', 'F/Login.aspx#Password', 'F/Login.aspx#Persist', 'F/Login.aspx#btnLogin'])
op('forms.login-return', 'forms', '/F/Login.aspx?ReturnUrl=...', 'RedirectFromLoginPage', 'back to the ReturnUrl (its query and Unicode intact); the session cookie of the ticket',
   [fill('UserName', 'user'), fill('Password', 'pass1'), click('btnLogin'), snap('forms-login-return'), cookies('forms-login-cookies')],
   'the ReturnUrl decoded and followed', controls=['F/Secure/Page.aspx'])
op('forms.admin-denied', 'forms', '/F/Admin/Page.aspx (user)', 'a signed-in user without the role', '302 to the login page (authenticated but not authorized)',
   [fetch('/F/Admin/Page.aspx', 'forms-admin-denied-raw'), goto('/F/Admin/Page.aspx'), snap('forms-admin-denied')], 'a 401 for a signed-in user still goes to the login page',
   keys=['GET /f/admin/page.aspx'], controls=['F/Admin/Page.aspx'])
op('forms.redirect-to-login', 'forms', '/F/Secure/Page.aspx', 'FormsAuthentication.RedirectToLoginPage("extra=1")', 'the login page with ReturnUrl and extra',
   [goto('/F/Secure/Page.aspx'), click('btnToLogin'), snap('forms-redirect-to-login')], 'the extra query with the ReturnUrl',
   keys=k('/f/secure/page.aspx', 'submit:btnToLogin', 'submit:btnLogout'), controls=['F/Secure/Page.aspx#btnToLogin'])
op('forms.logout', 'forms', '/F/Secure/Page.aspx', 'SignOut + Redirect to the secure page', 'the login page again; the ticket cookie removed',
   [goto('/F/Secure/Page.aspx'), click('btnLogout'), snap('forms-logout'), cookies('forms-logout-cookies')], 'the expired cookie', controls=['F/Secure/Page.aspx#btnLogout'])
op('forms.back-after-logout', 'forms', '/F/Secure/Page.aspx (history)', 'the browser\'s Back after logging out', 'as the original (the page from the cache or the login again)',
   [back(), snap('forms-back-after-logout'), goto('/F/Secure/Page.aspx'), snap('forms-after-logout-reload')], 'cache headers of a protected page (private)')
op('forms.return-url-external', 'forms', '/F/Login.aspx?ReturnUrl=http://evil.example/steal', 'a ReturnUrl to another host', 'refused: the default URL (or the original\'s answer)',
   [newcontext(), goto('/F/Login.aspx?ReturnUrl=http%3A%2F%2Fevil.example%2Fsteal'), snap('forms-return-url-external-login'), fill('UserName', 'user'), fill('Password', 'pass1'),
    click('btnLogin'), snap('forms-return-url-external')], 'the open redirect check of GetRedirectUrl')
op('forms.return-url-scheme-relative', 'forms', '/F/Login.aspx?ReturnUrl=//evil.example/x', 'a scheme-relative ReturnUrl', 'refused as the original',
   [newcontext(), goto('/F/Login.aspx?ReturnUrl=%2F%2Fevil.example%2Fx'), fill('UserName', 'user'), fill('Password', 'pass1'), click('btnLogin'), snap('forms-return-url-scheme-relative')],
   '"//host" passes a naive "starts with /" check')
op('forms.default-url', 'forms', '/F/Login.aspx', 'a login without ReturnUrl, persistent', 'the defaultUrl (F/Welcome); a persistent cookie',
   [newcontext(), goto('/F/Login.aspx'), fill('UserName', 'user'), fill('Password', 'pass1'), check('Persist'), click('btnLogin'), snap('forms-default-url'), cookies('forms-persistent-cookies')],
   'createPersistentCookie', controls=['F/Welcome.aspx'])
op('forms.admin-allowed', 'forms', '/F/Admin/Page.aspx (admin)', 'the Admin role (Application_PostAuthenticateRequest\'s GenericPrincipal)', '200; the site map shows the admin nodes',
   [newcontext(), goto('/F/Admin/Page.aspx'), fill('UserName', 'admin'), fill('Password', 'pass2'), click('btnLogin'), snap('forms-admin-allowed'),
    {**goto('/N/Deep/Page.aspx'), 'op': 'nav.sitemap-admin'}, {**snap('nav-sitemap-admin'), 'op': 'nav.sitemap-admin'}], 'roles set in an application event; security trimming for a role', role='admin')
op('session.abandon', 'forms', '/F/Abandon.aspx', 'Session.Abandon, then a postback', 'the value gone; a new session (IsNewSession) with the view state still there',
   [newcontext(), goto('/F/Abandon.aspx'), click('btnSet'), snap('session-set'), click('btnAbandon'), snap('session-abandoned'), click('btnRead'), snap('session-after-abandon')],
   'session expiry in the middle of a flow; the session id cookie reused', keys=k('/f/abandon.aspx', 'submit:btnSet', 'submit:btnAbandon', 'submit:btnRead', 'input:txt text'),
   controls=['F/Abandon.aspx#btnSet', 'F/Abandon.aspx#btnAbandon', 'F/Abandon.aspx#btnRead', 'F/Abandon.aspx#txt'])
op('session.cookieless', 'forms', '(an application with sessionState cookieless="UseUri")', 'cookieless session URLs /(S(id))/ with Redirect, Transfer and relative links', 'not run',
   [], 'the session id in the path: every relative URL and Redirect must keep it', runnable=False,
   reason='sessionState cookieless is per application (MachineToApplication): it needs a second application in a subfolder; sample-parity.ps1 hosts one application at the site root (and the converter converts one application). A sample of its own would be needed.')

# ======================================================================= P: postback edge cases
op('postback.back-stale', 'postback', '/P/Counter.aspx', 'a postback, the browser\'s Back, a postback with the old view state', 'view=1 (the stale view state) session=2',
   [phase('7. Postback edge cases (a new browser)'), newcontext(), goto('/P/Counter.aspx'), click('btnInc'), snap('postback-counter-1'), back(), snap('postback-back'), click('btnInc'), snap('postback-back-stale')],
   'the history\'s page posts its own view state: view state and session disagree', keys=k('/p/counter.aspx', 'submit:btnInc'), controls=['P/Counter.aspx#btnInc'])
op('postback.double-submit', 'postback', '/P/Counter.aspx', 'the same form posted twice (a double click)', 'both answers view=1; the session counted twice',
   [goto('/P/Counter.aspx'), postraw('btnInc', 'postback-double-1'), postraw('btnInc', 'postback-double-2'), click('btnInc'), snap('postback-double-after')],
   'a replayed postback: event validation and view state accept it')
op('postback.no-viewstate', 'postback', '/P/Counter.aspx', 'a POST with the button but no __VIEWSTATE', 'IsPostBack true, the click raised, the state lost',
   [{**fetch('/P/Counter.aspx', 'postback-no-viewstate'), 'body': 'btnInc=%2B1', 'bodyMatch': r'view=\d+ session=\d+|log: [^\n<]*|post: [^\n<]*'}], 'what makes a request a postback')
op('postback.event-validation', 'postback', '/P/Validate.aspx', 'an option added by script to a DropDownList, posted', 'Invalid postback or callback argument (500)',
   [goto('/P/Validate.aspx'), snap('postback-validate'), evaluate('ddl', "e.add(new Option('X', 'x')); e.value = 'x';"), click('btnPost'), snap('postback-event-validation', error=True)],
   'EnableEventValidation\'s check of posted values', keys=k('/p/validate.aspx', 'submit:btnPost', 'input:ddl select', 'postback:lnk'),
   controls=['P/Validate.aspx#ddl', 'P/Validate.aspx#btnPost', 'P/Validate.aspx#lnk', 'P/Validate.aspx#btnHidden'])
op('postback.no-event-validation', 'postback', '/P/NoValidate.aspx', 'the same with EnableEventValidation="false"', 'accepted; the list keeps its own selection',
   [goto('/P/NoValidate.aspx'), evaluate('ddl', "e.add(new Option('X', 'x')); e.value = 'x';"), click('btnPost'), snap('postback-no-event-validation')],
   'a posted value the list does not have', keys=k('/p/novalidate.aspx', 'submit:btnPost', 'input:ddl select', 'postback:lnk'),
   controls=['P/NoValidate.aspx#ddl', 'P/NoValidate.aspx#btnPost', 'P/NoValidate.aspx#lnk', 'P/NoValidate.aspx#btnHidden'])
op('postback.unknown-target', 'postback', '/P/Validate.aspx', '__doPostBack of a control that does not exist', 'a postback without an event',
   [goto('/P/Validate.aspx'), script("__doPostBack('nope', '')"), snap('postback-unknown-target')], '__EVENTTARGET not found')
op('postback.bad-argument', 'postback', '/P/Validate.aspx', '__doPostBack(lnk, an argument it never registered)', 'Invalid postback or callback argument',
   [goto('/P/Validate.aspx'), script("__doPostBack('lnk', 'bogus')"), snap('postback-bad-argument', error=True)], 'event validation of the argument')
op('postback.hidden-target', 'postback', '/P/Validate.aspx', '__doPostBack of a control that is Visible=false', 'Invalid postback (it was never rendered)',
   [goto('/P/Validate.aspx'), script("__doPostBack('btnHidden', '')"), snap('postback-hidden-target', error=True)], 'a hidden button\'s event')
op('postback.tampered-viewstate', 'postback', '/P/Counter.aspx', 'a __VIEWSTATE changed by the client', 'the view state MAC / state invalid error (500)',
   [goto('/P/Counter.aspx'), setvalue('__VIEWSTATE', 'dGFtcGVyZWQgdmlldyBzdGF0ZQ=='), click('btnInc'), snap('postback-tampered', error=True)], 'EnableViewStateMac')
op('postback.dynamic-recreated', 'postback', '/P/Dynamic.aspx', 'controls added in OnInit on every request', 'the dynamic button\'s click with the typed text',
   [goto('/P/Dynamic.aspx'), fill('dynTxt', 'dyn'), click('dynBtn'), snap('postback-dynamic-recreated')], 'dynamic controls re-created',
   keys=k('/p/dynamic.aspx', 'submit:dynBtn', 'submit:btnStatic', 'input:dynTxt text') + k('/p/dynamic.aspx?recreate'), controls=['P/Dynamic.aspx#btnStatic'])
op('postback.dynamic-lost', 'postback', '/P/Dynamic.aspx?recreate=0', 'controls added on the first request only', 'the click lost; the text only in Request.Form',
   [goto('/P/Dynamic.aspx?recreate=0'), fill('dynTxt', 'lost'), click('dynBtn'), snap('postback-dynamic-lost'), click('btnStatic'), snap('postback-dynamic-static')],
   'the classic dynamic-control mistake')
op('postback.viewstate-order', 'postback', '/P/Order.aspx', 'the dynamic labels added the other way round on the postback', 'the labels\' texts swapped (positional view state)',
   [goto('/P/Order.aspx'), check('chkFlip'), click('btnPost'), snap('postback-viewstate-order')], 'view state is matched by position',
   keys=k('/p/order.aspx', 'submit:btnPost', 'input:chkFlip checkbox'), controls=['P/Order.aspx#chkFlip', 'P/Order.aspx#btnPost'])
op('postback.tree-changed', 'postback', '/P/Changed.aspx', 'a Label where the DropDownList was', 'as the original: "Failed to load viewstate" or the state ignored',
   [goto('/P/Changed.aspx'), check('chkSwap'), click('btnPost'), snap('postback-tree-changed')], 'a control tree changed between requests',
   keys=k('/p/changed.aspx', 'submit:btnPost', 'input:chkSwap checkbox', 'input:slot select'), controls=['P/Changed.aspx#chkSwap', 'P/Changed.aspx#btnPost'])
op('postback.chunked-viewstate', 'postback', '/P/Chunk/Big.aspx', 'maxPageStateFieldLength="64" (a <location>)', '__VIEWSTATEFIELDCOUNT and __VIEWSTATE1..n; the value restored',
   [goto('/P/Chunk/Big.aspx'), snap('postback-chunked'), click('btnPost'), snap('postback-chunked-post')], 'the view state cut into fields',
   keys=k('/p/chunk/big.aspx', 'submit:btnPost'), controls=['P/Chunk/Big.aspx#btnPost'])
op('postback.encrypted-viewstate', 'postback', '/P/Encrypted.aspx', 'ViewStateEncryptionMode="Always"', '__VIEWSTATEENCRYPTED; the label restored',
   [goto('/P/Encrypted.aspx'), snap('postback-encrypted'), click('btnPost'), snap('postback-encrypted-post')], 'the machine key\'s encryption',
   keys=k('/p/encrypted.aspx', 'submit:btnPost'), controls=['P/Encrypted.aspx#btnPost', 'P/Encrypted.aspx#lbl'])
op('postback.get-viewstate', 'postback', '/P/GetState.aspx?__VIEWSTATE=...', 'a GET with __VIEWSTATE, __EVENTTARGET and __EVENTVALIDATION in the query', 'IsPostBack true on a GET; the click raised',
   [goto('/P/GetState.aspx'), script("var q = ['__VIEWSTATE', '__VIEWSTATEGENERATOR', '__EVENTVALIDATION'].map(function (n) { var e = document.getElementById(n); return e ? n + '=' + encodeURIComponent(e.value) : ''; }).filter(function (s) { return s; }); location.href = 'GetState.aspx?__EVENTTARGET=lnk&__EVENTARGUMENT=&' + q.join('&');"),
    snap('postback-get-viewstate')], 'the query string as the postback\'s values', keys=k('/p/getstate.aspx', 'postback:lnk'), controls=['P/GetState.aspx#lnk', 'P/GetState.aspx#lbl'])

# ======================================================================= W: Wizard, MultiView
WZ = '/w/wizard.aspx'
op('wizard.validation', 'wizard', '/W/Wizard.aspx', 'Next with an empty required field (server validation)', 'the step stays; the validator\'s text',
   [phase('8. Wizard, MultiView'), goto('/W/Wizard.aspx'), snap('wizard-start'), click('StartNextButton'), snap('wizard-validation')], 'validation per step',
   keys=k(WZ, 'submit:wiz$*', 'input:wiz$txtName text', 'postback:wiz$SideBarContainer$SideBarList$*', 'submit:btnJump'),
   controls=['W/Wizard.aspx#wiz', 'W/Wizard.aspx#txtName', 'W/Wizard.aspx#rfvName'])
op('wizard.next', 'wizard', '/W/Wizard.aspx', 'Next, Next', 'steps 2 and 3; NextButtonClick and ActiveStepChanged',
   [fill('txtName', 'Bob'), click('StartNextButton'), snap('wizard-step-2'), click('StepNextButton'), snap('wizard-step-3')], 'the Wizard\'s templates\' buttons')
op('wizard.previous', 'wizard', '/W/Wizard.aspx', 'step 3 has no Previous (step 2 has AllowReturn=false); the side bar back to Step 1, then Next into the visited step 2',
   'step 1 again; Next does not enter step 2 again (it stays on step 1)',
   [clicktext('Step 1'), snap('wizard-sidebar-back'), click('StartNextButton'), snap('wizard-step-2-again')],
   'AllowReturn hides the way back to a step; the history of steps')
op('wizard.sidebar', 'wizard', '/W/Wizard.aspx', 'the side bar\'s link to Step 3', 'SideBarButtonClick; step 3', [clicktext('Step 3'), snap('wizard-sidebar')], 'the side bar\'s DataList of LinkButtons')
op('wizard.finish', 'wizard', '/W/Wizard.aspx', 'the side bar to Confirm, Finish', 'the Complete step', [clicktext('Confirm'), snap('wizard-confirm'), click('FinishButton'), snap('wizard-finish')],
   'FinishButtonClick and the Complete step')
op('wizard.jump', 'wizard', '/W/Wizard.aspx', 'ActiveStepIndex = 2 in code', 'step 3', [goto('/W/Wizard.aspx'), click('btnJump'), snap('wizard-jump')], 'the active step set by code', controls=['W/Wizard.aspx#btnJump'])
op('wizard.cancel', 'wizard', '/W/Wizard.aspx', 'Cancel (CancelDestinationPageUrl)', 'redirect to W/Cancelled.aspx', [click('CancelButton'), snap('wizard-cancel')],
   'the Wizard\'s own redirect', controls=['W/Cancelled.aspx'])
op('wizard.finish-destination', 'wizard', '/W/WizardDest.aspx', 'Finish with FinishDestinationPageUrl', 'redirect to W/Finished.aspx?from=wizard',
   [goto('/W/WizardDest.aspx'), click('StartNextButton'), snap('wizard-dest-finish-step'), click('FinishButton'), snap('wizard-finish-destination')], 'the Wizard\'s redirect after Finish',
   keys=k('/w/wizarddest.aspx', 'submit:wiz$*'), controls=['W/WizardDest.aspx#wiz', 'W/Finished.aspx'])
MV = '/w/multiview.aspx'
op('multiview.commands', 'wizard', '/W/MultiView.aspx', 'NextView, nested SwitchViewByID, the inner PrevView, PrevView, NextView, SwitchViewByIndex',
   'each view; the inner MultiView handles its own commands (bubbling to the nearest)',
   [goto('/W/MultiView.aspx'), snap('multiview-1'), click('next1'), snap('multiview-2'), click('toI2'), snap('multiview-inner-2'), click('innerPrev'), snap('multiview-inner-1'),
    click('prev2'), snap('multiview-prev'), click('next1'), click('next2'), snap('multiview-3'), click('toFirst'), snap('multiview-by-index')],
   'command bubbling through nested MultiViews', keys=k(MV, 'submit:next1', 'submit:next2', 'submit:prev2', 'submit:toI2', 'submit:innerPrev', 'submit:toFirst', 'submit:btnToThree'),
   controls=['W/MultiView.aspx#mv', 'W/MultiView.aspx#mvInner', 'W/MultiView.aspx#next1', 'W/MultiView.aspx#next2', 'W/MultiView.aspx#prev2', 'W/MultiView.aspx#toI2',
             'W/MultiView.aspx#innerPrev', 'W/MultiView.aspx#toFirst'])
op('multiview.set-active', 'wizard', '/W/MultiView.aspx', 'SetActiveView(v3) in code', 'view three', [click('btnToThree'), snap('multiview-set-active')], 'the active view set by code',
   controls=['W/MultiView.aspx#btnToThree'])
op('wizard.ajax', 'wizard', '/W/AjaxWizard.aspx', 'a Wizard inside an UpdatePanel: Next, Next, Finish', 'each step by an async postback',
   [goto('/W/AjaxWizard.aspx'), click('StartNextButton'), snap('wizard-ajax-b'), click('StepNextButton'), snap('wizard-ajax-c'), click('FinishButton'), snap('wizard-ajax-finish')],
   'the Wizard\'s postbacks through the page request manager', keys=k('/w/ajaxwizard.aspx', 'submit:wiz$*', 'postback:wiz$SideBarContainer$SideBarList$*'),
   controls=['W/AjaxWizard.aspx#wiz', 'W/AjaxWizard.aspx#up', 'W/AjaxWizard.aspx#sm'])

# ======================================================================= J: ASP.NET AJAX
JP = '/j/panel.aspx'
J_CONTROLS = ['J/Panel.aspx#sm', 'J/Panel.aspx#up1']
op('ajax.update', 'ajax', '/J/Panel.aspx', 'an async postback inside the panel', 'the panel updated (requests: 2); one more page load',
   [phase('9. ASP.NET AJAX'), goto('/J/Panel.aspx'), snap('ajax-panel'), asyncpost('btnUpdate', 'ajax-update-raw'), click('btnUpdate'), snap('ajax-update')],
   'the delta format and the panel\'s refresh', keys=k(JP, 'submit:btnUpdate', 'submit:btnRedirect', 'submit:btnTransfer', 'submit:btnThrow', 'submit:btnSlow', 'submit:btnWrite',
                                                       'submit:btnEnd', 'submit:btnFull', 'submit:btnOutside', 'submit:btnPlain'),
   controls=J_CONTROLS + ['J/Panel.aspx#btnUpdate'])
op('ajax.trigger-outside', 'ajax', '/J/Panel.aspx', 'a button outside the panel registered as AsyncPostBackTrigger', 'an async postback', [click('btnOutside'), snap('ajax-trigger-outside')],
   'triggers', controls=['J/Panel.aspx#btnOutside'])
op('ajax.postback-trigger', 'ajax', '/J/Panel.aspx', 'a button inside the panel registered as PostBackTrigger', 'a full postback', [click('btnFull'), snap('ajax-postback-trigger')],
   'a full postback from inside a panel', controls=['J/Panel.aspx#btnFull'])
op('ajax.outside-full', 'ajax', '/J/Panel.aspx', 'a button outside the panel, not a trigger', 'a full postback', [click('btnPlain'), snap('ajax-outside-full')], 'the default', controls=['J/Panel.aspx#btnPlain'])
op('ajax.redirect', 'ajax', '/J/Panel.aspx', 'Response.Redirect in an async postback', 'the delta "pageRedirect"; the browser goes to J/Landing',
   [goto('/J/Panel.aspx'), asyncpost('btnRedirect', 'ajax-redirect-raw'), click('btnRedirect'), snap('ajax-redirect')], 'the redirect turned into a delta by the ScriptModule',
   controls=['J/Panel.aspx#btnRedirect', 'J/Landing.aspx'])
op('ajax.transfer', 'ajax', '/J/Panel.aspx', 'Server.Transfer in an async postback', 'as the original (an error delta / the page)',
   [goto('/J/Panel.aspx'), asyncpost('btnTransfer', 'ajax-transfer-raw'), click('btnTransfer'), snap('ajax-transfer')], 'Transfer during an async postback', controls=['J/Panel.aspx#btnTransfer'])
op('ajax.error', 'ajax', '/J/Panel.aspx', 'an exception in an async postback (AsyncPostBackError sets the message)', 'the error delta with the handled message, shown by the page\'s script',
   [goto('/J/Panel.aspx'), asyncpost('btnThrow', 'ajax-error-raw'), click('btnThrow'), snap('ajax-error')], 'AsyncPostBackErrorMessage', controls=['J/Panel.aspx#btnThrow'])
op('ajax.timeout', 'ajax', '/J/Panel.aspx', 'an async postback slower than AsyncPostBackTimeout (2 s)', 'the client\'s timeout error',
   [goto('/J/Panel.aspx'), click('btnSlow'), wait(4500), snap('ajax-timeout')], 'the timeout written into the page\'s initialisation', controls=['J/Panel.aspx#btnSlow'])
op('ajax.response-write', 'ajax', '/J/Panel.aspx', 'Response.Write in an async postback', 'the parser error ("The message received from the server could not be parsed")',
   [goto('/J/Panel.aspx'), asyncpost('btnWrite', 'ajax-write-raw'), click('btnWrite'), snap('ajax-write')], 'raw output breaks the delta', controls=['J/Panel.aspx#btnWrite'])
op('ajax.response-end', 'ajax', '/J/Panel.aspx', 'Response.End in an async postback', 'as the original', [goto('/J/Panel.aspx'), asyncpost('btnEnd', 'ajax-end-raw'), click('btnEnd'), snap('ajax-end')],
   'the end of an async response', controls=['J/Panel.aspx#btnEnd'])
JN = '/j/nested.aspx'
op('ajax.nested', 'ajax', '/J/Nested.aspx', 'nested conditional panels: the inner button, the outer, Update() of a third, a button of a panel whose children are not triggers',
   'only the panels updated show the new request number',
   [goto('/J/Nested.aspx'), snap('ajax-nested'), click('btnInner'), snap('ajax-nested-inner'), click('btnOuter'), snap('ajax-nested-outer'), click('btnUpdate3'), snap('ajax-nested-update3'),
    click('btnThree'), snap('ajax-nested-three')], 'UpdateMode Conditional, ChildrenAsTriggers false, nesting',
   keys=k(JN, 'submit:btnInner', 'submit:btnOuter', 'submit:btnUpdate3', 'submit:btnThree'),
   controls=['J/Nested.aspx#sm', 'J/Nested.aspx#upOuter', 'J/Nested.aspx#upInner', 'J/Nested.aspx#up3', 'J/Nested.aspx#btnInner', 'J/Nested.aspx#btnOuter', 'J/Nested.aspx#btnUpdate3', 'J/Nested.aspx#btnThree'])
op('ajax.history', 'ajax', '/J/History.aspx', 'AddHistoryPoint on two async postbacks, the browser\'s Back twice and Forward', 'the address\'s fragment; Navigate restores each step',
   [goto('/J/History.aspx'), click('btnStep2'), snap('ajax-history-2'), click('btnStep3'), snap('ajax-history-3'), back(), snap('ajax-history-back'), back(), snap('ajax-history-back-2'),
    forward(), snap('ajax-history-forward')], 'history points (EnableSecureHistoryState false: the state in the fragment)', keys=k('/j/history.aspx', 'submit:btnStep2', 'submit:btnStep3'),
   controls=['J/History.aspx#sm', 'J/History.aspx#up', 'J/History.aspx#btnStep2', 'J/History.aspx#btnStep3'])
op('ajax.timer', 'ajax', '/J/Timer.aspx', 'a Timer in a panel (600 ms), disabled after 3 ticks', 'ticks: 3', [goto('/J/Timer.aspx'), wait(4000), snap('ajax-timer')],
   'postbacks the browser makes by itself', keys=['GET /j/timer.aspx'], controls=['J/Timer.aspx#tm', 'J/Timer.aspx#sm', 'J/Timer.aspx#up'])
op('ajax.page-methods', 'ajax', '/J/Methods.aspx', 'PageMethods: a result, an exception, a Response.Redirect inside', 'the result; the error (500, its type and message); the redirect\'s answer',
   [goto('/J/Methods.aspx'), click('btnEcho'), wait(800), snap('ajax-page-method'), click('btnFails'), wait(800), snap('ajax-page-method-fails'), click('btnRedirects'), wait(800), snap('ajax-page-method-redirects'),
    {**fetch('/J/Methods.aspx/Echo', 'ajax-page-method-raw'), 'method': 'POST', 'body': '{"text":"raw"}', 'headers': {'Content-Type': 'application/json; charset=utf-8'}, 'bodyMatch': r'\{.*\}'}],
   'static [WebMethod]s of a page (PathInfo /Echo on the .aspx)', keys=k('/j/methods.aspx', 'submit:#btnEcho', 'submit:#btnFails', 'submit:#btnRedirects'),
   controls=['J/Methods.aspx#sm'])
op('ajax.script-service', 'ajax', '/J/ServicePage.aspx', 'a [ScriptService] .asmx through its generated proxy', 'the result',
   [goto('/J/ServicePage.aspx'), click('btnHello'), wait(800), snap('ajax-script-service'), fetch('/J/Service.asmx/js', 'ajax-script-service-proxy')],
   'the proxy (/js) and the JSON call', keys=k('/j/servicepage.aspx', 'submit:#btnHello'), controls=['J/ServicePage.aspx#sm', 'J/Service.asmx'])

# ======================================================================= U: routes, URLs
op('route.constraint', 'routing', '/nav/5', 'MapPageRoute nav/{id} (constraint \\d+, default 1)', 'the page with id=5; GetRouteUrl, RouteUrl expression, relative links under /nav/',
   [phase('10. Routes and URLs'), goto('/nav/5'), snap('route-constraint')], 'routes and the URLs a routed page resolves', keys=['GET /nav/*'], controls=['Global.asax', 'U/RoutePage.aspx#lnkRouteUrl', 'U/RoutePage.aspx#img', 'U/RoutePage.aspx#aRelative'])
op('route.default', 'routing', '/nav', 'the default value', 'id=1', [goto('/nav'), snap('route-default')], 'defaults', keys=['GET /nav'])
op('route.constraint-fail', 'routing', '/nav/abc', 'a value the constraint refuses', '404', [fetch('/nav/abc', 'route-constraint-fail-raw'), goto('/nav/abc'), snap('route-constraint-fail', error=True)], 'constraints')
op('route.catch-all', 'routing', '/files/a/b c, /files/x.txt', 'a catch-all {*rest}; one with a file extension', 'rest=a/b c; the .txt as the original (IIS\'s static file handler: 404)',
   [goto('/files/a/b%20c'), snap('route-catch-all'), click('btnPost'), snap('route-catch-all-postback'), fetch('/files/x.txt', 'route-catch-all-extension')],
   'which requests reach the routing module', keys=['GET /files/*', 'SUBMIT /files/* btnPost'])
op('route.relative-link', 'routing', '/nav/5', 'the relative HyperLink on a routed page', 'resolved against /nav/ (a 404 as the original)',
   [goto('/nav/5'), click('lnkRelative'), snap('route-relative-link', error=True)], 'relative URLs on routed pages', controls=['U/RoutePage.aspx#lnkRelative'])
op('route.postback', 'routing', '/nav/5', 'a postback on a routed page', 'the form posts to the route\'s URL', [goto('/nav/5'), click('btnPost'), snap('route-postback')],
   'the form action of a routed page', keys=['SUBMIT /nav/5 btnPost', 'SUBMIT /nav btnPost', 'SUBMIT /nav/* btnPost'], controls=['U/RoutePage.aspx#btnPost'])
op('route.transfer', 'routing', '/go/somewhere', 'a routed page that transfers', 'U/Plain under /go/somewhere with the route\'s values',
   [goto('/go/somewhere'), snap('route-transfer')], 'RouteData after Transfer', keys=['GET /go/*'], controls=['U/RouteTransfer.aspx', 'U/Plain.aspx'])
op('url.pathinfo', 'routing', '/U/Info.aspx/extra/more', 'PathInfo', 'PathInfo /extra/more; the form action and relative links under the path info',
   [goto('/U/Info.aspx/extra/more'), snap('url-pathinfo'), click('btnPost'), snap('url-pathinfo-postback'), goto('/U/Info.aspx/extra/more'), click('lnkRelative'), snap('url-pathinfo-link', error=True)],
   'the path info that the browser takes as folders', keys=['GET /u/info.aspx/*', 'SUBMIT /u/info.aspx/* btnPost', 'GET /u/info.aspx/extra/plain.aspx'],
   controls=['U/Info.aspx#btnPost', 'U/Info.aspx#lnkRelative', 'U/Info.aspx#aRelative'])
op('url.pathinfo-encoded', 'routing', '/U/Info.aspx/a%20b/%E6%97%A5', 'escaped path info', 'decoded as the original', [goto('/U/Info.aspx/a%20b/%E6%97%A5'), snap('url-pathinfo-encoded')], 'decoding of the path info')
op('url.default-document', 'routing', '/U/', 'the folder\'s default document', 'U/Default.aspx', [goto('/U/'), snap('url-default-document')], 'IIS\'s DefaultDocument module', keys=['GET /u'], controls=['U/Default.aspx'])
op('url.courtesy-redirect', 'routing', '/U', 'a folder without its trailing slash', '301 to /U/ (IIS\'s courtesy redirect)', [fetch('/U', 'url-courtesy-redirect-raw'), goto('/U'), snap('url-courtesy-redirect')],
   'IIS answers before ASP.NET')
for id, path, what in (('rebase', '/U/Pretty/abc', 'RewritePath(path) (rebaseClientPath true)'), ('no-rebase', '/U/PrettyNoRebase/abc', 'RewritePath(path, false)'),
                       ('parts', '/U/PrettyParts/abc', 'RewritePath(file, pathInfo, query, false)')):
    op(f'url.rewrite-{id}', 'routing', path, f'{what} in BeginRequest, then a postback', 'Path / RawUrl / the form action; where the postback goes',
       [goto(path), snap(f'url-rewrite-{id}'), click('btnPost'), snap(f'url-rewrite-{id}-postback')],
       'the client path the form action is made from', keys=['GET ' + path.lower(), f'SUBMIT {path.lower()} btnPost', 'GET /u/pretty/plain.aspx', 'GET /u/prettynorebase/plain.aspx', 'GET /u/prettyparts/plain.aspx', 'GET /u/plain.aspx'],
       controls=['U/Rewritten.aspx#btnPost', 'U/Rewritten.aspx#lnkRelative', 'U/Rewritten.aspx#img'])
for id, query, what in (('repeated', 'a=1&a=2&a=', 'a repeated key'), ('empty', 'a=&b&=c&&d=', 'empty values, a key without =, an empty key'),
                        ('encoded', 'a=x%26y&b=%E6%97%A5&c=a+b&d=%2B&e=%3D', 'escaped & = + and UTF-8'), ('invalid', 'a=%ZZ&b=%u65E5&c=%E6%97', 'an invalid escape, IIS\'s %u, a truncated UTF-8')):
    op(f'url.query-{id}', 'routing', f'/U/Query.aspx?{query}', what, 'the collection as the original parses it', [goto(f'/U/Query.aspx?{query}'), snap(f'url-query-{id}')], 'HttpValueCollection\'s parsing',
       keys=['GET /u/query.aspx*'], controls=['U/Query.aspx'])
op('url.case', 'routing', '/u/QUERY.aspx?a=1', 'a URL in another case', 'the same page (Windows\' file names)', [goto('/u/QUERY.aspx?a=1'), snap('url-case')], 'case-sensitive file systems (Linux)')
op('url.double-slash', 'routing', '//U//Query.aspx?a=1', 'doubled slashes', 'as the original', [fetch('//U//Query.aspx?a=1', 'url-double-slash')], 'path normalisation')

# ======================================================================= N: navigation controls
op('nav.sitemap', 'navigation', '/N/Deep/Page.aspx (anonymous)', 'SiteMapPath, Menu and TreeView of Web.sitemap with security trimming; a user control\'s URLs; the master\'s',
   'the trimmed nodes (Secure, Admin gone; the roles node kept); URLs relative to the control\'s and the master\'s folders',
   [phase('11. Navigation controls (a new browser)'), newcontext(), goto('/N/Deep/Page.aspx'), snap('nav-sitemap')],
   'security trimming by URL authorization and roles; TemplateSourceDirectory of controls and masters', keys=['GET /n/deep/page.aspx', 'GET /n/controls/sibling.aspx', 'GET /n/deep/sibling.aspx',
                                                                                                         'GET /masters/other.aspx', 'GET /n/*', 'GET /t/source.aspx', 'GET /default.aspx', 'EXTERNAL http://external.example/sitemap',
                                                                                                         'POSTBACK /n/* ctl00$tvMap', 'POSTBACK /n/* ctl00$menuMap'],
   controls=['Masters/Nav.master#smds', 'Masters/Nav.master#smp', 'Masters/Nav.master#menuMap', 'Masters/Nav.master#tvMap', 'Masters/Nav.master#aMasterRel', 'Masters/Nav.master#imgMaster',
             'N/Deep/Page.aspx#links', 'N/Deep/Page.aspx#lnkPage', 'N/Controls/Links.ascx#lnkTilde', 'N/Controls/Links.ascx#lnkRelative', 'N/Controls/Links.ascx#aServer',
             'N/Controls/Links.ascx#icon', 'N/RolesOnly.aspx', 'N/Default.aspx', 'Web.sitemap'])
op('nav.usercontrol-link', 'navigation', '/N/Deep/Page.aspx', 'the user control\'s relative HyperLink and the plain link', 'resolved against N/Controls and N/Deep (404s as the original)',
   [goto('/N/Deep/Page.aspx'), click('links_lnkRelative'), snap('nav-usercontrol-link', error=True), goto('/N/Deep/Page.aspx'), click('aPlain'), snap('nav-usercontrol-plain', error=True)],
   'relative URLs in controls of other folders')
op('nav.sitemap-admin', 'navigation', '/N/Deep/Page.aspx (admin)', 'the same as the Admin role (done in forms.admin-allowed)', 'the Admin nodes shown', [], 'trimming by role', role='admin')
NM = '/n/menu.aspx'
op('nav.menu', 'navigation', '/N/Menu.aspx', 'Menu items: NavigateUrl, a postback item, a child postback item', 'the navigation; MenuItemClick with the value path',
   [goto('/N/Menu.aspx'), snap('nav-menu'), clicktext('Postback B'), snap('nav-menu-postback'), script("__doPostBack('menuItems', 'a\\\\a1')"), snap('nav-menu-child'),
    goto('/N/Menu.aspx'), clicktext('Go to Deep'), snap('nav-menu-navigate')],
   'Menu\'s postback vs NavigateUrl, value paths with \\', keys=k(NM, 'postback:menuItems', 'postback:tvItems', 'link:/n/deep/page.aspx', 'link:/n/default.aspx'), controls=['N/Menu.aspx#menuItems'])
op('nav.treeview', 'navigation', '/N/Menu.aspx', 'TreeView without client script: expand by postback, select, a NavigateUrl node', 'TreeNodeExpanded, SelectedNodeChanged, the navigation',
   [goto('/N/Menu.aspx'), script("__doPostBack('tvItems', 'troot')"), snap('nav-treeview-expand'), clicktext('Child select'), snap('nav-treeview-select'), clicktext('Child link'),
    snap('nav-treeview-navigate'), goto('/N/Menu.aspx'), clicktext('Root'), snap('nav-treeview-root-text')],
   'TreeView\'s postback events', controls=['N/Menu.aspx#tvItems'])
op('nav.imagemap', 'navigation', '/N/ImageMap.aspx', 'ImageMap hot spots: PostBack and Navigate', 'the click with its value; the navigation',
   [goto('/N/ImageMap.aspx'), snap('nav-imagemap'), script("document.querySelector('area[alt=\"post back left\"]').click()"), snap('nav-imagemap-postback'),
    script("document.querySelector('area[alt=\"navigate\"]').click()"), snap('nav-imagemap-navigate')], '<area> postbacks', keys=k('/n/imagemap.aspx', 'postback:im', 'link:/n/default.aspx'),
   controls=['N/ImageMap.aspx#im'])
NB = '/n/buttons.aspx'
op('nav.client-click', 'navigation', '/N/Buttons.aspx', 'OnClientClick "return false" on a LinkButton and a Button; a confirm (accepted)', 'no postback; no postback; the postback',
   [goto('/N/Buttons.aspx'), snap('nav-buttons'), click('lnkFalse'), snap('nav-linkbutton-false'), click('btnFalse'), snap('nav-button-false'), click('lnkConfirm'), snap('nav-linkbutton-confirm')],
   'the client script before __doPostBack', keys=k(NB, 'postback:lnkFalse', 'postback:lnkConfirm', 'submit:btnFalse', 'submit:btnPanel', 'submit:btnForm', 'submit:btnFocus', 'submit:btnBottom',
                                                    'input:txtPanel text', 'input:txtFocus text', 'input:txtAfter text'),
   controls=['N/Buttons.aspx#lnkFalse', 'N/Buttons.aspx#btnFalse', 'N/Buttons.aspx#lnkConfirm'])
op('nav.default-button', 'navigation', '/N/Buttons.aspx', 'Enter in a Panel with DefaultButton; Enter in the form (form DefaultButton); DefaultFocus', 'the panel\'s button; the form\'s; the focus',
   [goto('/N/Buttons.aspx'), fill('txtPanel', 'enter'), press('txtPanel'), snap('nav-panel-default-button'), goto('/N/Buttons.aspx'), fill('txtFocus', 'enter'), press('txtFocus'), snap('nav-form-default-button')],
   'WebForm_FireDefaultButton', controls=['N/Buttons.aspx#pnl', 'N/Buttons.aspx#txtPanel', 'N/Buttons.aspx#btnPanel', 'N/Buttons.aspx#txtFocus', 'N/Buttons.aspx#btnForm'])
op('nav.set-focus', 'navigation', '/N/Buttons.aspx', 'SetFocus(txtAfter) in a click', 'the focus on txtAfter', [goto('/N/Buttons.aspx'), click('btnFocus'), snap('nav-set-focus')], 'WebForm_AutoFocus',
   controls=['N/Buttons.aspx#btnFocus', 'N/Buttons.aspx#txtAfter'])
op('nav.scroll', 'navigation', '/N/Buttons.aspx', 'MaintainScrollPositionOnPostBack: the button at the bottom', 'the scroll position restored', [goto('/N/Buttons.aspx'), click('btnBottom'), snap('nav-scroll')],
   '__SCROLLPOSITIONY', controls=['N/Buttons.aspx#btnBottom'])

# ======================================================================= Y: async pages, handlers, downloads, windows, frames
for mode, expected in (('redirect', '302; the code after the redirect in the task does not run'), ('redirect-false', '302 with the page'), ('transfer', 'the landing under Y/Async.aspx'),
                       ('throw', 'the error page of the task\'s exception'), ('', 'the task finished')):
    path = '/Y/Async.aspx' + (f'?mode={mode}' if mode else '')
    name = mode or 'done'
    steps = [fetch(path, f'async-{name}-raw'), goto(path), snap(f'async-{name}', error=(mode == 'throw'))]
    if mode == 'redirect':
        steps = [phase('12. Async pages, handlers, downloads, windows, frames')] + steps + [goto('/Y/Landing.aspx'), snap('async-redirect-side')]
    op(f'async.{name}', 'async', path, f'Async="true", RegisterAsyncTask: {mode or "no navigation"} after an await', expected, steps,
       'navigation from a continuation (another thread; the synchronization context)', keys=['GET /y/async.aspx', 'GET /y/async.aspx?mode'], controls=['Y/Async.aspx', 'Y/Landing.aspx'])
op('async.void', 'async', '/Y/AsyncVoid.aspx?mode=redirect', 'async void Page_Load: Redirect after an await', '302 as the original',
   [fetch('/Y/AsyncVoid.aspx?mode=redirect', 'async-void-raw'), goto('/Y/AsyncVoid.aspx?mode=redirect'), snap('async-void'), goto('/Y/AsyncVoid.aspx'), snap('async-void-plain')],
   'an async void event handler (the page waits for it)', keys=['GET /y/asyncvoid.aspx', 'GET /y/asyncvoid.aspx?mode'], controls=['Y/AsyncVoid.aspx'])
for mode in ('', 'permanent', 'false'):
    path = '/Y/Redirect.ashx' + (f'?mode={mode}' if mode else '')
    op(f'handler.redirect{"-" + mode if mode else ""}', 'async', path, f'a handler\'s Response.Redirect{"Permanent" if mode == "permanent" else ""}{"(url, false) + Write" if mode == "false" else ""}',
       'the redirect; a relative URL resolved against Y/', [fetch(path, f'handler-redirect{"-" + mode if mode else ""}-raw'), goto(path), snap(f'handler-redirect{"-" + mode if mode else ""}')],
       'a handler mapped in web.config (no file); Redirect outside a page', keys=['GET /y/redirect.ashx', 'GET /y/redirect.ashx?mode'], controls=['Web.config#RedirectHandler'])
op('handler.transfer', 'async', '/Y/Transfer.ashx', 'a handler\'s Server.Transfer to a page and Server.Execute of a page', 'the landing; the part between the handler\'s own lines',
   [goto('/Y/Transfer.ashx'), snap('handler-transfer'), goto('/Y/Transfer.ashx?mode=execute'), snap('handler-execute')], 'child requests from a handler',
   keys=['GET /y/transfer.ashx', 'GET /y/transfer.ashx?mode'], controls=['Y/Transfer.ashx'])
YD = '/y/download.aspx'
op('download.button', 'async', '/Y/Download.aspx', 'a Button that sends a file (Content-Disposition attachment, Response.End), then a postback', 'the file; the next postback works',
   [goto('/Y/Download.aspx'), download('btnDownload', 'download-button'), click('btnAfter'), snap('download-button-after')], 'the page stays after a download',
   keys=k(YD, 'submit:btnDownload', 'submit:btnUnicode', 'submit:btnAfter', 'postback:lnkDownload', 'postback:lnkAfter'), controls=['Y/Download.aspx#btnDownload', 'Y/Download.aspx#btnAfter'])
op('download.linkbutton', 'async', '/Y/Download.aspx', 'a LinkButton download (__EVENTTARGET left set), then a Button and a LinkButton', 'the file; which event the next postbacks raise',
   [goto('/Y/Download.aspx'), download('lnkDownload', 'download-linkbutton'), click('btnAfter'), snap('download-linkbutton-button'), download('lnkDownload', 'download-linkbutton-2'),
    click('lnkAfter'), snap('download-linkbutton-link')], '__EVENTTARGET still holding the download\'s target', controls=['Y/Download.aspx#lnkDownload', 'Y/Download.aspx#lnkAfter'])
op('download.unicode', 'async', '/Y/Download.aspx', 'filename* (RFC 5987, UTF-8)', 'the Unicode name', [goto('/Y/Download.aspx'), download('btnUnicode', 'download-unicode')], 'the header as written',
   controls=['Y/Download.aspx#btnUnicode'])
YO = '/y/opener.aspx'
op('window.blank-link', 'windows', '/Y/Opener.aspx', 'a HyperLink with Target="_blank"', 'the popup page in a new window', [goto('/Y/Opener.aspx'), click('lnkBlank'), popup('p1'), snap('window-blank-link'), tab('main')],
   'new windows', keys=k(YO, 'link:/y/popup.aspx?via', 'submit:btnBlank', 'submit:btnNormal', 'submit:#btnOpen'), controls=['Y/Opener.aspx#lnkBlank', 'Y/Popup.aspx'])
op('window.form-target', 'windows', '/Y/Opener.aspx', 'OnClientClick sets the form\'s target to _blank; then an ordinary button', 'the postback in a new window; the next ordinary postback too (the target stays)',
   [goto('/Y/Opener.aspx'), click('btnBlank'), popup('p2'), snap('window-form-target-popup'), tab('main'), snap('window-form-target-main'), click('btnNormal'), popup('p3'), snap('window-form-target-sticky'), tab('main')],
   'a form target left set by script', controls=['Y/Opener.aspx#btnBlank', 'Y/Opener.aspx#btnNormal'])
op('window.open-opener', 'windows', '/Y/Opener.aspx', 'window.open, a postback in the popup that writes into its opener', 'the opener\'s text changed',
   [goto('/Y/Opener.aspx'), click('btnOpen'), popup('pw'), snap('window-open'), click('btnTell'), snap('window-open-postback'), tab('main'), snap('window-opener-told')],
   'window.opener after a postback', keys=k('/y/popup.aspx', 'submit:btnTell'), controls=['Y/Popup.aspx#btnTell'])
op('frame.postback', 'windows', '/Y/Frames.aspx', 'postbacks and a Response.Redirect inside an iframe', 'only the frame changes',
   [goto('/Y/Frames.aspx'), snap('frame-page'), frame('f1'), snap('frame-inner'), click('btnInc'), snap('frame-postback'), click('btnRedirect'), snap('frame-redirect'), frame(''), snap('frame-page-after')],
   'navigation inside frames', keys=['GET /y/frames.aspx', 'GET /y/inner2.aspx?via'], controls=['Y/Frames.aspx', 'Y/Inner.aspx#btnInc', 'Y/Inner.aspx#btnRedirect', 'Y/Inner2.aspx#btnPost'])
op('frame.target', 'windows', '/Y/Frames.aspx', 'a link of the page with target="f1"; a postback in the new frame page', 'Inner2 in the frame; its postback',
   [goto('/Y/Frames.aspx'), click('lnkFrame'), frame('f1'), snap('frame-target'), click('btnPost'), snap('frame-target-postback'), frame('')], 'named targets')
op('frame.top', 'windows', '/Y/Frames.aspx', 'a target="_top" link and a postback whose script sets top.location', 'the whole window navigates',
   [goto('/Y/Frames.aspx'), frame('f1'), click('lnkTop'), frame(''), snap('frame-top-link'), goto('/Y/Frames.aspx'), frame('f1'), click('btnTop'), frame(''), wait(1000), snap('frame-top-script')],
   'leaving the frame', controls=['Y/Inner.aspx#lnkTop', 'Y/Inner.aspx#btnTop'])

# ======================================================================= V: Visual Basic pages
op('vb.handles-redirect', 'vb', '/V/Handles.aspx', 'Handles Me.Load and Handles btnGo.Click (AutoEventWireup false): Response.Redirect', 'the landing; the Handles events logged',
   [phase('13. Visual Basic pages (compiled by the server)'), goto('/V/Handles.aspx'), snap('vb-handles'), click('btnGo'), snap('vb-handles-redirect')],
   'VB\'s event wiring (WithEvents fields of the generated class)', keys=k('/v/handles.aspx', 'submit:btnGo', 'submit:btnCatch', 'submit:btnTransfer'), controls=['V/Handles.aspx#btnGo', 'V/Landing.aspx'])
op('vb.catch-when', 'vb', '/V/Handles.aspx', 'Redirect in Try ... Catch ex As ThreadAbortException When filter()', 'the filter and the Catch run, the code after the Try does not (the session)',
   [goto('/V/Handles.aspx'), click('btnCatch'), snap('vb-catch-when'), goto('/V/Landing.aspx'), snap('vb-catch-when-side')],
   'exception filters run before the stack unwinds; the abort re-raised after the Catch', controls=['V/Handles.aspx#btnCatch'])
op('vb.transfer', 'vb', '/V/Handles.aspx', 'Server.Transfer from a VB handler', 'the landing under V/Handles.aspx', [goto('/V/Handles.aspx'), click('btnTransfer'), snap('vb-transfer')], 'Transfer in VB',
   controls=['V/Handles.aspx#btnTransfer'])
op('vb.my', 'vb', '/V/My.aspx', 'My.Request / My.Response.Redirect', 'the path; the redirect', [goto('/V/My.aspx'), snap('vb-my'), fetch('/V/My.aspx?go=1', 'vb-my-redirect-raw'), goto('/V/My.aspx?go=1'), snap('vb-my-redirect')],
   'VB\'s My namespace for the web (_MYTYPE="Web")', keys=['GET /v/my.aspx', 'GET /v/my.aspx?go'], controls=['V/My.aspx'])
op('vb.cross-page', 'vb', '/V/Source.aspx -> /V/Target.aspx', 'a cross-page postback between VB pages (PreviousPageType, a VB property)', 'the typed value',
   [goto('/V/Source.aspx'), click('btnCross'), snap('vb-cross-page')], 'PreviousPageType in VB', keys=k('/v/source.aspx', 'submit:btnCross', 'input:txt text') + ['GET /v/target.aspx'],
   controls=['V/Source.aspx#btnCross', 'V/Source.aspx#txt', 'V/Target.aspx'])

# ======================================================================= the files
INDEX_EXTRA = ['/T/Sub/Other.aspx', '/T/Sub/Target.aspx', '/X/Other/Target.aspx', '/R/Life.aspx', '/R/Cases.aspx', '/R/Sub/Rel.aspx', '/G/Landing.aspx', '/E/Oops.aspx', '/E/NotFound.aspx',
               '/F/Welcome.aspx', '/F/Secure/Page.aspx?a=1&b=%E6%97%A5%E6%9C%AC&c=x%26y', '/W/Cancelled.aspx', '/W/Finished.aspx', '/J/Landing.aspx', '/U/Plain.aspx', '/U/Rewritten.aspx',
               '/N/Default.aspx', '/N/RolesOnly.aspx', '/Y/Landing.aspx', '/Y/Inner.aspx', '/Y/Inner2.aspx', '/Y/Popup.aspx', '/V/Landing.aspx', '/V/Target.aspx', '/T/Part.aspx']


def entry_urls():
    urls = []
    for step in STEPS:
        if step.get('action') in ('goto', 'fetch') and step.get('method') is None and step.get('body') is None:
            path = step['path']
            if path.startswith('//') or path == '/':
                continue
            if path not in urls:
                urls.append(path)
    return urls + [u for u in INDEX_EXTRA if u not in urls]


def write(name, text):
    with open(os.path.join(HERE, name), 'w', encoding='utf-8', newline='\r\n') as f:
        f.write(text)


SCENARIO = OrderedDict([
    ('_comment', 'NavigationProbe: every operation of operations.json (operations.md, README.md: why each case is nasty), each step\'s op naming the operation it exercises. Written by '
                 'inventory.py - edit the table there. Golden: golden-webforms.json, recorded on IIS by experiments/wf4c/sample-parity.ps1 -Name NavigationProbe -Record; '
                 'compared by sample-parity.ps1 -Windows / -Linux. Snapshots: the page (compareSource: its HTML too, the view state masked; captureUrl: the address the browser shows), '
                 'fetch / postraw: one answer, its redirect not followed (status, Location as sent, the body\'s title, event log and side effects; follow: every hop), downloads, cookies. '
                 'External requests of the browser are stubbed (not the target of a redirect: a route does not see it; .example names do not resolve: the browser\'s own '
                 'error page, its text only). The view state and event validation in a URL (a GET postback) are masked.'),
    ('operations', 'operations.json'),
    ('allowed', 'allowed.json'),
    ('compareSource', True),
    ('captureUrl', True),
    ('stubExternal', True),
    ('continueOnError', True),
    ('waitMs', 300),
    ('quietMs', 400),
    ('textMasks', [r'(?<=sha256 )[0-9A-F]{16}(?= \(masked\))', r'(?<=(__VIEWSTATE\w*|__EVENTVALIDATION)=)[^&\s"<]+']),
    ('sourceMasks', [r'(?<=(__VIEWSTATE\w*|__EVENTVALIDATION)=)[^&\s"<]+']),
    ('ignorePatterns', []),
    ('steps', STEPS),
    ('crawl', OrderedDict([('depth', 1), ('max', 400), ('urls', ['/']), ('exclude', [r'(?i)/G/Loop', r'(?i)/G/Chain', r'(?i)logout'])])),
])


# Pages and controls of operations made in loops.
EXTRA_CONTROLS = {
    'error.error-page-throws': ['E/Broken/ErrorThrows.aspx'],
    'error.async': ['E/Redirect/Async.aspx#sm', 'E/Redirect/Async.aspx#up'],
    'error.asyncnoredirect': ['E/Redirect/AsyncNoRedirect.aspx#sm', 'E/Redirect/AsyncNoRedirect.aspx#up'],
    'redirect.unicode-path': ['R/着地.aspx'],
    'redirect.space-path': ['R/With Space.aspx'],
    'transfer.relative-links': ['T/Sub/Other.aspx'],
}


# The pages an operation lands on (a redirect's, a transfer's, a link's target): discover reaches them from the index
# (INDEX_EXTRA) or from a page's links; their keys belong to the operation that lands there.
LANDING_KEYS = {
    'error.redirect-mode': ['GET /e/oops.aspx', 'GET /e/oops.aspx?aspxerrorpath'],
    'error.redirect-404': ['GET /e/notfound.aspx', 'GET /e/notfound.aspx?aspxerrorpath'],
    'forms.return-url-query-unicode': ['GET /f/secure/page.aspx?a&b', 'GET /f/secure/page.aspx?a&b&c', 'GET /f/login.aspx?a&b&returnurl',
                                       'GET /f/login.aspx?a&b&c&returnurl'],
    'global.post-acquire-state': ['GET /g/landing.aspx?from&session'],
    'global.error-redirect': ['GET /g/landing.aspx?from&type'],
    'redirect.relative-parent-parent': ['GET /default.aspx?from'],
    'redirect.relative-tilde-frag': ['GET /r/landing.aspx'],
    'redirect.encoded': ['GET /r/landing.aspx?a&b&c'],
    'redirect.unicode': ['GET /r/landing.aspx?amp&name&pct&plus&y'],
    'redirect.relative-query': ['GET /r/sub?from'],
    'redirect.relative-frag': ['GET /r/sub'],
    'redirect.relative-file': ['GET /r/sub/landing.aspx?from'],
    'redirect.space-path': ['GET /r/with space.aspx?from'],
    'redirect.unicode-path': ['GET /r/着地.aspx?from'],
    'forms.default-url': ['GET /f/welcome.aspx'],
    'global.begin-request': ['GET /g/landing.aspx', 'GET /g/landing.aspx?from'],
    'ajax.redirect': ['GET /j/landing.aspx'],
    'ajax.script-service': ['GET /j/service.asmx/js'],
    'redirect.tae': ['GET /r/cases.aspx'],
    'redirect.at-preinit': ['GET /r/life.aspx', 'GET /r/landing.aspx?from'],
    'redirect.relative-parent': ['GET /r/sub/rel.aspx'],
    'transfer.chain': ['GET /t/other.aspx'],
    'transfer.target-postback': ['GET /t/sub/target.aspx', 'SUBMIT /t/sub/target.aspx btnPost'],
    'execute.load': ['GET /t/part.aspx'],
    'execute.redirect': ['GET /t/sub/other.aspx?from'],
    'url.rewrite-rebase': ['GET /u/rewritten.aspx', 'SUBMIT /u/rewritten.aspx btnPost', 'GET /u/pretty/*', 'SUBMIT /u/pretty/* btnPost'],
    'url.rewrite-no-rebase': ['GET /u/prettynorebase/*', 'SUBMIT /u/prettynorebase/* btnPost'],
    'url.rewrite-parts': ['GET /u/prettyparts/*', 'SUBMIT /u/prettyparts/* btnPost'],
    # The relative HyperLink "Plain.aspx" of /nav (the default value) resolved by the browser against the root.
    'route.relative-link': ['GET /plain.aspx', 'GET /nav/plain.aspx', 'GET /go/plain.aspx', 'GET /files/*'],
    # The links of the error pages (ASP.NET's and IIS's detailed errors) to Microsoft's documentation.
    'error.detailed': ['EXTERNAL http://go.microsoft.com/*', 'EXTERNAL https://go.microsoft.com/*'],
    # The protected page under its route (signed in): its buttons.
    'forms.secure-route': ['SUBMIT /secure/* btnLogout', 'SUBMIT /secure/* btnToLogin'],
    'vb.handles-redirect': ['GET /v/landing.aspx', 'GET /v/landing.aspx?from'],
    'wizard.cancel': ['GET /w/cancelled.aspx'],
    'wizard.finish-destination': ['GET /w/finished.aspx'],
    'cross.master': ['GET /x/other.aspx'],
    'cross.other-folder': ['GET /x/other/target.aspx'],
    'frame.postback': ['GET /y/inner.aspx', 'SUBMIT /y/inner.aspx btnInc', 'SUBMIT /y/inner.aspx btnRedirect'],
    'frame.top': ['SUBMIT /y/inner.aspx btnTop'],
    'frame.target': ['GET /y/inner2.aspx', 'SUBMIT /y/inner2.aspx btnPost'],
    'async.redirect': ['GET /y/landing.aspx', 'GET /y/landing.aspx?from'],
}


def main():
    ids = {o['id'] for o in OPS}
    assert set(LANDING_KEYS) <= ids, set(LANDING_KEYS) - ids
    for o in OPS:
        o.setdefault('controls', []).extend(EXTRA_CONTROLS.get(o['id'], []))
        if o['id'] in LANDING_KEYS:
            o['keys'] = (o.get('keys') or []) + [key for key in LANDING_KEYS[o['id']] if key not in (o.get('keys') or [])]
    ids = [o['id'] for o in OPS]
    dupes = {i for i in ids if ids.count(i) > 1}
    assert not dupes, dupes
    names = [s['name'] for s in STEPS if s.get('name')]
    dupes = {n for n in names if names.count(n) > 1}
    assert not dupes, dupes
    inventory = {'_comment': 'Written by inventory.py (edit the table there). The operations of NavigationProbe; parity-scenario.json exercises them (its steps\' op).',
                 'operations': [{k: v for k, v in o.items() if k != 'controls'} for o in OPS]}
    write('operations.json', json.dumps(inventory, ensure_ascii=False, indent=2) + '\n')
    write('parity-scenario.json', json.dumps(SCENARIO, ensure_ascii=False, indent=2) + '\n')

    # The index: every entry URL, by folder.
    links = '\n'.join(f'      <li><a href="{html.escape(u)}">{html.escape(u)}</a></li>' for u in entry_urls())
    write('Default.aspx', '<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>\n<!DOCTYPE html>\n<html>\n<head runat="server"><title>NavigationProbe</title></head>\n<body>\n'
          '  <form id="form1" runat="server">\n    <h1>NavigationProbe</h1>\n    <p>Page navigation that is likely to break on a port: one link a case (README.md).</p>\n'
          f'    <ul id="cases">\n{links}\n    </ul>\n    <nav:Info runat="server" />\n  </form>\n</body>\n</html>\n')

    categories = OrderedDict()
    for o in OPS:
        categories.setdefault(o['category'], []).append(o)
    md = ['# NavigationProbe: the inventory of operations', '',
          'Written by `inventory.py` from the same table as `operations.json` (edit the table there and run it). Each operation is exercised by the steps of',
          '`parity-scenario.json` that name it (`op`); the golden data is recorded on the original on IIS and compared on Windows and Linux',
          '(`experiments/wf4c/sample-parity.ps1 -Name NavigationProbe -Record / -Windows / -Linux`). Coverage: `inventory.py` checks every interactive',
          'server control and page of the source belongs to an operation (static); ParityTest `discover` crawls the running site and lists what no',
          'operation\'s keys cover (`sample-parity.ps1 ... -Discover`).', '', '| category | operations | not runnable |', '|---|---|---|']
    for c, ops in categories.items():
        md.append(f'| {c} | {len(ops)} | {sum(1 for o in ops if not o["runnable"])} |')
    md.append(f'| **total** | **{len(OPS)}** | **{sum(1 for o in OPS if not o["runnable"])}** |')
    for c, ops in categories.items():
        md += ['', f'## {c}', '', '| id | page | element | expected | why it is nasty |', '|---|---|---|---|---|']
        for o in ops:
            cell = lambda t: (t or '').replace('|', '\\|')
            md.append(f'| {o["id"]} | {cell(o["page"])} | {cell(o["element"])} | {cell(o["expected"] if o["runnable"] else "not runnable: " + o["reason"])} | {cell(o["why"])} |')
    write('operations.md', '\n'.join(md) + '\n')

    readme = ['# NavigationProbe', '',
              'A .NET Framework 4.8 Web Forms site (C#, a few Visual Basic pages compiled by the server) full of page navigation that is likely to break on a port of System.Web:',
              'Server.Transfer / Execute, cross-page postbacks, Response.Redirect in every event and with every kind of URL, redirects of Global.asax and a module, customErrors',
              'and httpErrors, forms authentication, postback edge cases, Wizard / MultiView, ASP.NET AJAX, routes and URL rewriting, navigation controls, async pages,',
              'handlers, downloads, windows and frames. Self-contained: no database, no external service (the browser\'s requests to another origin are stubbed by the',
              'test; the target of a redirect to external.example is not seen by it: .example names do not resolve, the browser\'s own error page is compared, its text).', '',
              'Every page shows where it thinks it is (`Code/Probe.cs`: the `Info` control - the page, Path / RawUrl / the executing path / PathInfo, the postback,',
              'PreviousPage, the posted form, the query, the user, the session\'s side effects of the previous request, the event log of every page of the request).', '',
              '- `inventory.py` writes `operations.json` / `operations.md` (the inventory), `parity-scenario.json` (the steps), `Default.aspx` (the index) and checks the',
              '  inventory against the source.',
              '- Record on IIS: `.\\experiments\\wf4c\\sample-parity.ps1 -Name NavigationProbe -Record` (an administrator\'s shell; a throwaway site): `golden-webforms.json`.',
              '- Compare: `.\\experiments\\wf4c\\sample-parity.ps1 -Name NavigationProbe -Windows` / `-Linux` (Docker). Per-operation results: `results.<platform>.md` / `.json`',
              '  (same / allowed / different).',
              '- Documented differences: `allowed.json` (each with its reason).',
              '- What a user can reach that the inventory does not name: ParityTest discover on the original (a site of the sample on IIS), `discovered.iis.json`:',
              '  `dotnet tools\\FrameworkOnCore.ParityTest\\bin\\alt\\FrameworkOnCore.ParityTest.dll discover --url <site> --out samples\\NavigationProbe\\discovered.iis.json',
              '  --inventory samples\\NavigationProbe\\operations.json --login-url /F/Login.aspx --persona anonymous --persona login:user:pass1 --persona login:admin:pass2`',
              '  (nothing left: 663 pages, 309 keys).',
              '- The pages are the sample\'s own files (written once by a generator during its making; edited since): change them here, then run `python inventory.py`.',
              '', f'## The cases ({len(OPS)} operations)', '']
    for c, ops in categories.items():
        readme += [f'### {c} ({len(ops)})', '']
        for o in ops:
            readme.append(f'- **{o["id"]}** ({o["page"]}): {o["element"]}. *Why:* {o["why"]}.' + ('' if o['runnable'] else f' *Not runnable:* {o["reason"]}'))
        readme.append('')
    write('README.md', '\n'.join(readme))

    # Static analysis: every interactive server control, every page and handler of the source in an operation.
    referenced = set()
    for o in OPS:
        for c in o.get('controls', []):
            referenced.add(c.lower())
    interactive = re.compile(r'<(asp:(?:Button|LinkButton|ImageButton|HyperLink|TextBox|DropDownList|CheckBox|Menu|TreeView|ImageMap|Wizard|MultiView|Timer|UpdatePanel|ScriptManager|'
                             r'Image|SiteMapPath|SiteMapDataSource|RequiredFieldValidator|Panel|Label)|probe:\w+|a|img)\b[^>]*\bID="([^"]+)"[^>]*runat="server"|'
                             r'<(asp:(?:Button|LinkButton|HyperLink|TextBox|DropDownList|CheckBox|Menu|TreeView|ImageMap|Wizard|MultiView|Timer|UpdatePanel|ScriptManager|Image|SiteMapPath|'
                             r'SiteMapDataSource|RequiredFieldValidator|Panel|Label)|a|img)\b[^>]*runat="server"[^>]*\bID="([^"]+)"', re.I)
    missing = []
    for folder, dirs, files in os.walk(HERE):
        dirs[:] = [d for d in dirs if d not in ('bin', 'obj')]
        for name in files:
            if not name.endswith(('.aspx', '.ascx', '.master', '.ashx', '.asmx')):
                continue
            rel = os.path.relpath(os.path.join(folder, name), HERE).replace('\\', '/')
            text = open(os.path.join(folder, name), encoding='utf-8').read()
            page_listed = any(c.split('#')[0] == rel.lower() for c in referenced)
            if not page_listed:
                missing.append(rel)
            for m in interactive.finditer(text):
                cid = m.group(2) or m.group(4)
                if f'{rel}#{cid}'.lower() not in referenced and cid not in ('form1', 'info'):
                    missing.append(f'{rel}#{cid}')
    print(f'{len(OPS)} operations ({sum(1 for o in OPS if not o["runnable"])} not runnable), {len([s for s in STEPS if "action" in s])} steps, '
          f'{len(entry_urls())} index links')
    if missing:
        print('in the source, in no operation:')
        for m in sorted(set(missing)):
            print('  ' + m)
        sys.exit(1)
    print('static coverage: every page and interactive control of the source is in an operation')


main()
