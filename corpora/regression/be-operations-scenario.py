# Writes corpora/regression/be.operations.scenario.json, BlogEngine.NET's operations suite (ParityTest's scenario: 765 steps,
# 473 snapshots), from the steps below: each operation of the inventory (be.operations.json, be-operations-inventory.py)
# done in the order the phases give, with the state checks (App_Data's files, the API's answers) after them. Edit the
# steps and run:  python corpora/regression/be-operations-scenario.py  (then record the golden data again on IIS:
# corpora/record-be-operations.ps1; tests/README.md). The output is deterministic: the same JSON from the same steps.
import json, os, sys

STEPS = []
# Every file of App_Data but the conversion's own (the converted application's only).
# settings.xml and the widgets' settings are written from a Hashtable (StringDictionary): their elements in no order of the
# application's ('~': compared in order of their lines).
UNORDERED = '~settings.xml;~**/datastore/widgets/????????-????-????-????-????????????.xml;~**/datastore/widgets/parity-linklist.xml'
# The log (logger.txt) is the logs API's step's (its messages' wording: be.operations.allowed.json).
APP_DATA_ALL = '**;!culture-profile.json;!machine.config;!logger.txt;' + UNORDERED
WELCOME = 'f39c3289-e861-48f7-a4b7-edaaffe6f6dc'
C_VISITOR1 = '61c640cb-7782-4a1d-ab79-25bd4613fc4e'
PAGE_ABOUT = '50d4bec7-c5aa-442a-92a2-b5c7e87da64f'
PAGE_EXAMPLE = 'd9347918-136c-4538-ad50-9bce25fa170f'
CAT_BE = '19770e74-9ec9-4cde-b2ab-e5051aaaf348'
W_TEXTBOX = '2fa2f989-9d2b-4c8e-911e-2e1bf967d5e1'
W_POSTLIST = 'cb9e0c01-4491-4010-8015-0ee38cbcab51'
W_NEWSLETTER = 'b5c8b621-d6d4-4adb-b59c-b330a00dd81d'
W_TAGCLOUD = 'fd1a8078-fbdc-4908-b328-f74e340e6ef3'
W_BLOGROLL = '4e3d1239-9e6b-40b4-9b5b-192b79c3b759'


def NG(expr):
    """An AngularJS button by its click expression (ng-click or data-ng-click)."""
    return f"css=[data-ng-click='{expr}'], [ng-click='{expr}']"


def phase(text):
    STEPS.append({'_phase': text})


def s(action, op=None, **kw):
    step = {'action': action}
    step.update({k: v for k, v in kw.items() if v is not None})
    if op:
        step['op'] = op
    STEPS.append(step)


def goto(path, name=None, op=None, error=False):
    s('goto', op, path=path)
    if name:
        s('snapshot', name=name, value='error' if error else None)


def snap(name, error=False):
    s('snapshot', name=name, value='error' if error else None)


def fetch(path, name, op=None, **kw):
    s('fetch', op, path=path, name=name, **kw)


def api(name, method, url, body=None, op=None):
    """A Web API call as JSON: its status, its body a line a value."""
    kw = {}
    if method != 'GET':
        kw['method'] = method
    if body is not None:
        kw['body'] = json.dumps(body, ensure_ascii=False)
        kw['headers'] = {'Content-Type': 'application/json'}
    elif method in ('PUT', 'POST', 'DELETE'):
        kw['headers'] = {'Content-Type': 'application/json'}
    fetch(url, name, op, content='json', **kw)


def script(name, js, op=None):
    s('script', op, name=name, value=js)


def files(name, pattern, op=None):
    s('files', op, name=name, value=pattern)


def login(user, password, remember=False, op=None, path='/Account/login.aspx'):
    goto(path)
    s('fill', target='UserName', value=user)
    s('fill', target='Password', value=password)
    if remember:
        s('check', target='RememberMe')
    s('click', op, target='LoginButton')


# The comment form: name, e-mail, website, content, the captcha's answer; Save.
def comment(name, email, content, captcha='10', website='', op=None):
    s('fill', target='txtName', value=name)
    s('fill', target='txtEmail', value=email)
    if website:
        s('fill', target='txtWebsite', value=website)
    s('fill', target='txtContent', value=content)
    if captcha is not None:
        s('fill', target='simpleCaptchaValue', value=captcha)
    s('click', op, target='btnSaveAjax')
    # The client callback's answer (the button is disabled while the comment is saved).
    s('waitfor', value="!document.getElementById('btnSaveAjax') || !document.getElementById('btnSaveAjax').disabled")


# A Package / item list's checked action: the items found by a predicate, checked, PUT to the url.
def checked_js(list_url, predicate, put_url, extra=''):
    return (f"const all = (await api('GET', '{list_url}')).body; const items = all.filter(x => {predicate}).map(x => Object.assign(x, {{ IsChecked: true }}{extra})); "
            f"const r = await api('PUT', '{put_url}', items); return {{ checked: items.length, status: r.status, body: r.body }};")


def rpc_js(calls):
    """XML-RPC calls to /metaweblog.axd in order: each one's status and answer (text). calls: JS expressions of [method, [params xml]];
    the answers before are in ans[] (the text), id(n) the first <string> of answer n."""
    return ("const v = x => '<value>' + x + '</value>'; const str = x => v('<string>' + x + '</string>'); const b = x => v('<boolean>' + (x ? 1 : 0) + '</boolean>'); "
            "const st = o => v('<struct>' + Object.entries(o).map(([k, x]) => '<member><name>' + k + '</name>' + x + '</member>').join('') + '</struct>'); "
            "const arr = xs => v('<array><data>' + xs.join('') + '</data></array>'); "
            "const ans = []; const id = n => ((ans[n] || '').match(/<string>([^<]*)<\\/string>/) || [])[1] || ''; "
            "const call = async (m, ps) => { const r = await fetch('/metaweblog.axd', { method: 'POST', headers: { 'Content-Type': 'text/xml' }, body: '<?xml version=\"1.0\"?><methodCall><methodName>' + m + '</methodName><params>' + ps.map(p => '<param>' + p + '</param>').join('') + '</params></methodCall>' }); const t = await r.text(); ans.push(t); const e = t.match(/<b>\\s*(?:Exception Details|例外の詳細)\\s*:\\s*<\\/b>\\s*([\\w.`]+)/); return m + ' ' + r.status + '\\n' + (e ? '(ASP.NET error page) ' + e[1] : t.replace(/></g, '>\\n<')); }; "
            "const out = []; " + ' '.join(f"out.push(await call({c}));" for c in calls) + " return out.join('\\n');")


# ---------------------------------------------------------------------------------------------------------------------
phase('1. Anonymous browsing of the release\'s blog (a new browser: no cookies)')
goto('/', 'home', 'page.home,widget.postlist-tagcloud-textbox,page.external-links')
s('assets', 'static.page-assets', name='home-assets')
s('cookies', 'state.cookies-anonymous', name='cookies-anonymous')
files('app-data-after-start', APP_DATA_ALL, 'state.app-data-initial')
goto('/?page=2', 'home-page-2', 'page.home-paging')
goto('/')
s('clicktext', 'page.post,page.related-navigation,comment.form', value='Welcome to BlogEngine.NET')
snap('post-welcome')
s('assets', 'static.page-assets', name='post-assets')
goto('/post/2015/09/23/welcome-to-blogengine-net', 'post-dated', 'page.post-dated')
goto('/post/no-such-post', 'post-unknown', 'page.post-unknown')
fetch(f'/post.aspx?id={WELCOME}', 'post-by-id', 'page.post-by-id')
fetch('/post.aspx', 'post-no-id', 'page.post-no-id')
goto('/category/BlogEngineNET', 'category', 'page.category')
goto('/?tag=welcome', 'tag-welcome', 'page.tag')
goto('/author/Admin', 'author', 'page.author')
goto('/2018/05/default', 'date-month', 'page.date')
goto('/2018/05/20/default', 'date-day', 'page.date')
fetch('/?year=2018&month=5', 'date-year-query', 'page.date')
goto('/calendar', 'calendar', 'page.calendar')
goto('/blog', 'blog', 'page.blog')
goto('/page/about', 'page-about', 'page.page')
goto('/page/example-page', 'page-example', 'page.page')
goto('/page/no-such-page', 'page-unknown', 'page.page-unknown')
goto('/archive', 'archive', 'page.archive')
goto('/search?q=welcome', 'search-welcome', 'page.search')
goto('/search?q=nothing-matches', 'search-nothing', 'page.search')
goto('/search?q=visitor&comment=true', 'search-comments', 'page.search-comments')
goto('/search?q=the&page=2', 'search-page-2', 'page.search-paging')
goto('/search?q=http%3A%2F%2F127.0.0.1%3A9%2Fapml.xml', 'search-apml', 'page.search-apml')
goto('/contact', 'contact', 'page.contact-page')
goto('/contact?name=Parity&email=parity%40example.com&subject=Hello', 'contact-prefill', 'page.contact-prefill')
goto('/FrontPageExample.aspx', 'front-page-example', 'page.front-page-example')
goto('/', None)
s('fill', target=f'txt-{"fd2436bb-68a7-47d1-9171-2753af201fa6"}', value='welcome')
s('script', 'widget.search', value="document.getElementById('searchbutton').click();")
snap('search-widget')

phase('2. Themes, languages, crawlers')
goto('/?theme=Standard-2017', 'theme-standard-2017', 'page.theme-query,widget.postlist-tagcloud-textbox')
goto(f'/post/welcome-to-blogengine-net?theme=Standard-2017', 'theme-standard-2017-post', 'page.theme-query')
goto('/?theme=RazorHost', 'theme-razorhost', 'page.theme-query')
goto('/?theme=NoSuchTheme', 'theme-unknown', 'page.theme-query')
goto('/', 'theme-back-to-standard', 'page.theme-query')
s('acceptlanguage', value='de-DE,de;q=0.9')
goto('/', 'culture-de-home', 'page.culture')
goto('/post/welcome-to-blogengine-net', 'culture-de-post', 'page.culture')
fetch('/de.res.axd', 'culture-de-res', 'page.culture,h.res-axd', hash=True)
s('acceptlanguage', value='en-US,en;q=0.9')
goto('/archive', 'culture-en-archive', 'page.culture')
goto('/contact', 'culture-en-contact', 'page.culture')
s('acceptlanguage', value='ja-JP,ja;q=0.9')
goto('/archive', 'culture-ja-archive', 'page.culture')
fetch('/', 'culture-none', 'page.culture', headers={'Accept-Language': ''}, bodyMatch='<title>[^<]*</title>|<span id="ctl00_aLoginText">[^<]*</span>')
s('newcontext', value='Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)')
fetch('/category/BlogEngineNET', 'crawler-category', 'page.crawler')
fetch('/?tag=welcome', 'crawler-tag', 'page.crawler')
fetch('/?page=2', 'crawler-page', 'page.crawler')
fetch('/post/welcome-to-blogengine-net', 'crawler-post', 'page.crawler', bodyMatch='<title>[^<]*</title>')
s('newcontext')

phase('3. Feeds, handlers, static content, request filtering, error pages (anonymous)')
fetch('/syndication.axd', 'feed-rss', 'h.rss', content='xml')
fetch('/syndication.axd?format=atom', 'feed-atom', 'h.atom', content='xml')
fetch('/category/feed/BlogEngineNET', 'feed-category', 'h.feed-category', content='xml')
fetch(f'/syndication.axd?category={CAT_BE}', 'feed-category-id', 'h.feed-category', content='xml')
fetch('/post/feed/welcome-to-blogengine-net', 'feed-post', 'h.feed-post', content='xml')
fetch(f'/syndication.axd?post={WELCOME}', 'feed-post-id', 'h.feed-post', content='xml')
fetch('/syndication.axd?tag=welcome', 'feed-tag', 'h.feed-tag-author', content='xml')
fetch('/syndication.axd?author=Admin', 'feed-author', 'h.feed-tag-author', content='xml')
fetch('/syndication.axd?comments=true', 'feed-comments', 'h.feed-comments', content='xml')
fetch('/syndication.axd?q=welcome', 'feed-search', 'h.feed-search', content='xml')
fetch('/syndication.axd?category=not-a-guid', 'feed-category-bad', 'h.feed-category', content='text')
fetch('/sitemap.axd', 'sitemap', 'h.sitemap', content='xml')
fetch('/opensearch.axd', 'opensearch', 'h.opensearch', content='xml')
fetch('/rsd.axd', 'rsd', 'h.rsd', content='xml')
fetch('/opml.axd', 'opml', 'h.opml', content='xml')
fetch('/apml.axd', 'apml', 'h.apml', content='xml')
fetch('/sioc.axd', 'sioc', 'h.sioc', content='xml')
fetch(f'/sioc.axd?sioc_type=post&sioc_id={WELCOME}', 'sioc-post', 'h.sioc', content='xml')
fetch('/sioc.axd?sioc_type=user&sioc_id=Admin', 'sioc-user', 'h.sioc', content='xml')
fetch('/foaf.axd', 'foaf', 'h.foaf', content='xml')
fetch('/foaf_Admin.axd', 'foaf-admin', 'h.foaf', content='xml')
fetch('/file.axd?file=/blogengine.png', 'file-axd', 'h.file', hash=True, show=['Content-Disposition'])
fetch('/file.axd?file=/../users.xml', 'file-axd-dotdot', 'h.file', content='text')
fetch('/file.axd?file=/missing.txt', 'file-axd-missing', 'h.file')
fetch('/image.axd?picture=/logo.png', 'image-axd', 'h.image', hash=True, show=['Cache-Control'])
fetch('/image.axd?picture=/missing.png', 'image-axd-missing', 'h.image')
fetch('/images/logo.jpgx', 'image-rewrite', 'h.image-rewrite')
fetch('/files/blogengine.png.axdx', 'file-rewrite', 'h.image-rewrite')
fetch('/en.res.axd', 'res-en', 'h.res-axd', content='text')
fetch('/ja.res.axd', 'res-ja', 'h.res-axd', content='text')
fetch('/admin.res.axd', 'res-admin', 'h.res-axd', content='text')
goto('/')
script('js-axd', "const s = [...document.querySelectorAll('script[src]')].map(e => e.getAttribute('src')).filter(u => /\\.js\\.axd/.test(u)); const out = []; for (const u of s) { const r = await fetch(u); out.push(u.replace(/res-[^.]*/, 'res-{hash}') + ' ' + r.status + ' ' + (r.headers.get('content-type') || '') + ' ' + (await r.text()).length); } return out.length ? out.join('\\n') : 'no .js.axd on the page';", 'h.js-axd')
fetch('/blogml.axd', 'blogml-anonymous', 'h.blogml')
fetch('/rating.axd?id=' + WELCOME + '&rating=4', 'rating', 'comment.rating', content='text')
fetch('/rating.axd?id=' + WELCOME + '&rating=4', 'rating-again', 'comment.rating', content='text')
s('cookies', 'comment.rating', name='cookies-after-rating')
fetch('/rating.axd?id=' + WELCOME + '&rating=9', 'rating-out-of-range', 'comment.rating', content='text')
fetch('/rating.axd?id=bad&rating=3', 'rating-bad-id', 'comment.rating', content='text')
fetch('/trackback.axd', 'trackback-no-post', 'h.trackback', content='text')
fetch('/trackback.axd?id=' + WELCOME, 'trackback-no-url', 'h.trackback', method='POST', body='title=Parity&excerpt=x&blog_name=Parity', content='text')
fetch('/trackback.axd?id=' + WELCOME, 'trackback-unreachable', 'h.trackback', method='POST', body='title=Parity&excerpt=x&blog_name=Parity&url=http%3A%2F%2F127.0.0.1%3A9%2Fparity', content='text')
script('pingback-unreachable', "const r = await fetch('/pingback.axd', { method: 'POST', headers: { 'Content-Type': 'text/xml' }, body: '<?xml version=\"1.0\"?><methodCall><methodName>pingback.ping</methodName><params><param><value><string>http://127.0.0.1:9/parity</string></value></param><param><value><string>' + location.origin + '/post/welcome-to-blogengine-net</string></value></param></params></methodCall>' }); const t = await r.text(); const m = t.match(/<b>\\s*(?:Exception Details|例外の詳細)\\s*:\\s*<\\/b>\\s*([\\w.`]+)/); return r.status + '\\n' + (m ? '(ASP.NET error page) ' + m[1] : t.replace(/></g, '>\\n<'));", 'h.pingback')
fetch('/pingback.axd', 'pingback-get', 'h.pingback', content='text')
fetch('/robots.txt', 'static-robots', 'static.files', hash=True, show=['Cache-Control'])
fetch('/wlwmanifest.xml', 'static-wlwmanifest', 'static.files', hash=True)
fetch('/Content/images/blog/rssButton.png', 'static-png', 'static.files', hash=True, show=['Cache-Control'])
fetch('/Custom/Themes/Standard/src/css/styles.min.css', 'static-css', 'static.files', hash=True, show=['Cache-Control'])
fetch('/Custom/Themes/Standard/src/fonts/fontawesome-webfont.woff', 'static-woff', 'static.files', hash=True)
fetch('/favicon.ico', 'static-favicon', 'static.not-found')
fetch('/Content/missing.css', 'static-missing', 'static.not-found')
for path, name in [('/Web.Config', 'web-config'), ('/App_Data/users.xml', 'app-data-users'), ('/App_Data/', 'app-data-folder'), ('/bin/BlogEngine.Core.dll', 'bin'),
                   ('/AppCode/WebUtils.cs', 'appcode-cs'), ('/Custom/Themes/Standard/site.master', 'master'), ('/Custom/Controls/PostList.ascx', 'ascx'),
                   ('/packages.config', 'packages-config'), ('/Global.asax', 'global-asax'), ('/Account/login.aspx.cs', 'code-behind')]:
    fetch(path, 'filtered-' + name, 'static.request-filtering')
goto('/nosuch.aspx', 'not-found-aspx', 'err.not-found-aspx', error=True)
goto('/no/such/path', 'not-found-extensionless', 'err.not-found-extensionless')
goto('/error404.aspx?aspxerrorpath=/post/welcome', 'error404-suggestions', 'err.error404-page')
goto('/error404.aspx', 'error404', 'err.error404-page')
goto('/error.aspx', 'error-page', 'err.error-page')
goto('/post.aspx?id=bad', 'post-bad-id', 'err.exception')
fetch('/a*b', 'invalid-path-chars', 'sec.invalid-path-chars')
goto('/search?q=%3Cscript%3Ex%3C/script%3E', 'request-validation-query', 'sec.request-validation')
fetch('/admin/', 'admin-anonymous', 'acct.admin-anonymous')
fetch('/admin/app/editor/editpost.cshtml', 'admin-editor-anonymous', 'acct.admin-anonymous')
fetch('/setup/upgrade/Updater.asmx', 'setup-anonymous', 'h.web-services')
fetch('/admin/Extensions/Settings.aspx?ext=BBCode', 'extension-settings-anonymous', 'acct.admin-anonymous')
fetch('/Account/register.aspx', 'register-disabled', 'acct.register-disabled')
fetch('/Account/create-blog.aspx', 'create-blog-disabled', 'acct.create-blog-disabled')
fetch('/Account/change-password.aspx', 'change-password-anonymous', 'acct.change-password-anonymous')

phase('4. The Web API without signing in')
for url, name in [('/api/posts', 'posts'), (f'/api/posts/{WELCOME}', 'post'), ('/api/pages', 'pages'), ('/api/categories', 'categories'), ('/api/tags', 'tags'),
                  ('/api/comments', 'comments'), ('/api/users', 'users'), ('/api/roles', 'roles'), ('/api/settings', 'settings'), ('/api/dashboard', 'dashboard'),
                  ('/api/lookups', 'lookups'), ('/api/blogroll', 'blogroll'), ('/api/pingservices', 'pingservices'), ('/api/filemanager', 'filemanager'),
                  ('/api/trash', 'trash'), ('/api/customfields', 'customfields'), ('/api/blogs', 'blogs'), ('/api/stats', 'stats'), ('/api/packages?filter=extensions', 'packages-extensions'),
                  ('/api/commentfilter', 'commentfilter'), ('/api/logs/getlog/file', 'logs'), ('/api/pager', 'pager'), ('/api/tools/check1', 'tools')]:
    api('api-anonymous-' + name, 'GET', url, op='sec.api-anonymous')
api('api-anonymous-post-create', 'POST', '/api/posts', {'Title': 'x', 'Content': 'x', 'DateCreated': '2025-01-01 10:00'}, op='sec.api-anonymous')
api('api-anonymous-comment-create', 'POST', '/api/comments', {'PostId': WELCOME, 'Content': 'x'}, op='sec.api-anonymous')
api('api-anonymous-settings-save', 'PUT', '/api/settings', {'Name': 'x'}, op='sec.api-anonymous')
fetch('/api/upload?action=image', 'api-anonymous-upload', 'api.upload-unauthorized', method='POST', upload='file=parity.png:base64:iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==')

phase('5. Signing in (the login page) and the anonymous widgets')
goto('/Account/login.aspx', 'login-page', 'acct.login-page')
s('click', 'acct.login-required', target='LoginButton')
snap('login-required')
s('fill', target='UserName', value='admin')
s('fill', target='Password', value='wrong')
s('click', 'acct.login-wrong', target='LoginButton')
snap('login-wrong')
s('postraw', 'sec.viewstate-mac', target='LoginButton', body='__VIEWSTATE=AAAA' + 'B' * 40, name='login-viewstate-tampered', bodyMatch='<title>[^<]*</title>')
s('postraw', 'sec.event-validation', target='LoginButton', body='__EVENTVALIDATION=AAAA' + 'B' * 40, name='login-eventvalidation-tampered', bodyMatch='<title>[^<]*</title>')
s('postraw', 'sec.request-validation', target='LoginButton', body='ctl00%24MainContent%24LoginUser%24UserName=%3Cscript%3Ex%3C%2Fscript%3E&ctl00%24MainContent%24LoginUser%24Password=x', name='login-request-validation', bodyMatch='<title>[^<]*</title>')
goto('/Account/login.aspx?ReturnURL=http%3A%2F%2Fevil.example%2F')
s('fill', target='UserName', value='admin')
s('fill', target='Password', value='admin')
s('click', 'acct.login-return-url-external,acct.login', target='LoginButton')
snap('login-return-url-external')
s('cookies', 'state.cookies-authenticated', name='cookies-signed-in')
goto('/', 'home-signed-in', 'widget.administration')
fetch('/Account/login.aspx?logoff', 'logoff', 'acct.logoff', headers={'Referer': 'http://127.0.0.1:9/parity-referer'})
fetch('/Account/login.aspx?logoff', 'logoff-no-referer', 'acct.logoff')
s('cookies', 'acct.logoff', name='cookies-logged-off')
goto('/', 'home-logged-off', 'acct.logoff')
goto('/Account/login.aspx?ReturnURL=/admin/')
s('fill', target='UserName', value='admin')
s('fill', target='Password', value='admin')
s('check', target='RememberMe')
s('click', 'acct.login-return-url,acct.login-remember-me', target='LoginButton')
snap('login-return-url-admin')
s('cookies', 'acct.login-remember-me', name='cookies-remember-me')

phase('6. The administrator makes the blog deterministic (each one an operation): no ping services, no SMTP server, no comment filters that ask the Internet')
goto('/admin/#/settings/controls/pings', 'admin-pings', 'admin.screens,api.ping-services')
api('pingservices-list', 'GET', '/api/pingservices', op='api.ping-services')
api('pingservices-add', 'POST', '/api/pingservices', {'ServiceUrl': 'http://127.0.0.1:9/parity-ping'}, op='api.ping-services')
script('pingservices-delete-all', checked_js('/api/pingservices', 'true', '/api/pingservices/processchecked/delete'), 'api.ping-services')
files('pingservices-file', 'pingservices.xml', 'api.ping-services')
goto('/admin/#/settings/email', 'admin-settings-email', 'admin.screens,api.settings-get')
s('fill', target='txtSmtpServer', value='127.0.0.1')
s('fill', target='txtSmtpServerPort', value='9')
s('click', 'api.settings-save', target=NG('save()'))
s('waitms', ms=1000)
api('settings-after-email', 'GET', '/api/settings', op='api.settings-save')
script('settings-test-email', "const vm = (await api('GET', '/api/settings')).body; return await api('PUT', '/api/settings?action=testEmail', vm.Settings);", 'api.settings-test-email')
goto('/admin/#/custom/plugins', 'admin-plugins', 'admin.screens,api.plugins')
api('plugins-list', 'GET', '/api/packages?take=0&skip=0&filter=extensions&order=LastUpdated%20desc', op='api.plugins')
script('plugins-disable-external-filters', checked_js('/api/packages?take=0&skip=0&filter=extensions', "x.Id == 'StopForumSpam' || x.Id == 'AkismetFilter'", '/api/packages/processchecked/disable'), 'api.plugins,comment.post-anonymous')
files('plugins-files', 'datastore/extensions/StopForumSpam.xml;datastore/extensions/AkismetFilter.xml', 'api.plugins')

phase('7. Comments of visitors (a new browser)')
s('newcontext')
goto('/post/welcome-to-blogengine-net')
s('click', 'comment.validation', target='btnSaveAjax')
snap('comment-required-name')
s('fill', target='txtName', value='Parity Visitor')
s('click', 'comment.validation', target='btnSaveAjax')
snap('comment-required-email')
comment('Parity Visitor', 'visitor@example.com', 'A comment with the wrong answer.', captcha='3', op='comment.captcha-wrong')
snap('comment-captcha-wrong')
comment('Parity Visitor', 'visitor@example.com', 'A first comment from the parity test.', captcha='10', website='http://example.com/visitor', op='comment.post-anonymous')
snap('comment-posted-pending')
files('comment-file-pending', f'posts/{WELCOME}.xml', 'comment.post-anonymous')
s('cookies', 'comment.cookie', name='cookies-after-comment')
goto('/post/welcome-to-blogengine-net', 'comment-cookie-fills-form', 'comment.cookie')
s('script', 'comment.post-reply', value=f"BlogEngine.replyToComment('{C_VISITOR1}');")
comment('Parity Replier', 'replier@example.com', 'A reply to visitor1 with [b]BBCode[/b], <i>markup</i> and a smiley :)', captcha='10', op='comment.post-reply,comment.post-markup')
snap('comment-reply-posted')
goto(f'/post/welcome-to-blogengine-net?approvecomment={C_VISITOR1}', 'moderate-query-anonymous', 'comment.moderate-query-anonymous')
fetch('/post/welcome-to-blogengine-net?unsubscribe-email=visitor@example.com', 'unsubscribe', 'comment.unsubscribe')
fetch(f'/?deletepost={WELCOME}', 'delete-post-query-anonymous', 'page.delete-post-query')
files('comments-file-after-visitors', f'posts/{WELCOME}.xml', 'comment.post-reply,comment.post-markup')
goto('/', None)
s('fill', target='txtNewsletterEmail', value='not-an-address')
s('script', 'widget.newsletter-invalid', value="document.getElementById('btnSave').click();")
snap('newsletter-invalid')
s('fill', target='txtNewsletterEmail', value='subscriber@example.com')
s('script', 'widget.newsletter', value="document.getElementById('btnSave').click();")
snap('newsletter-subscribed')
files('newsletter-file', 'newsletter.xml', 'widget.newsletter')
goto(f'/Custom/Widgets/TextBox/edit.cshtml?id={W_TEXTBOX}&zone=sidebar-Post', 'widget-edit-anonymous', 'sec.widget-edit-anonymous')
goto(f'/Custom/Widgets/BlogRoll/edit.cshtml?id={W_BLOGROLL}&zone=sidebar-Post', 'widget-edit-blogroll-anonymous', 'sec.widget-edit-anonymous')

phase('8. The administrator: the dashboard and the screens')
s('newcontext')
login('admin', 'admin', op='acct.login')
goto('/admin/', 'admin-shell', 'admin.shell,admin.dashboard,admin.dashboard-external')
s('fill', target='txtDraftTitle', value='Parity quick draft')
s('fill', target='txtDraftText', value='Drafted on the dashboard.')
s('click', 'admin.dashboard-quick-draft', target=NG('postDraft()'))
s('waitms', ms=1500)
snap('dashboard-quick-draft')
fetch('/admin/about.cshtml', 'admin-about-fetch', 'admin.about', bodyMatch='<title>[^<]*</title>')
goto('/admin/about.cshtml', 'admin-about', 'admin.about')
api('dashboard', 'GET', '/api/dashboard', op='admin.dashboard')
api('newsfeed', 'GET', '/api/newsfeed?take=5&skip=0', op='admin.dashboard-external')
api('gallery-packages', 'GET', '/api/packages?take=5&skip=0', op='admin.dashboard-external,api.packages-gallery')
api('setup-version', 'GET', '/api/setup?version=3.3.8.0', op='admin.dashboard-external')
for route in ['content/posts', 'content/pages', 'content/categories', 'content/tags', 'content/comments', 'content/comments/filters', 'content/blogs',
              'custom/themes', 'custom/widgets', 'custom/plugins/gallery', 'custom/themes/gallery', 'custom/widgets/gallery',
              'security/profile', 'security/users', 'security/roles',
              'settings/basic', 'settings/feed', 'settings/comments', 'settings/controls', 'settings/advanced', 'settings/controls/blogroll', 'settings/tools']:
    goto(f'/admin/#/{route}', 'admin-' + route.replace('/', '-'), 'admin.screens')

phase('9. Posts: the editor, the API, the list\'s actions')
goto('/admin/app/editor/editpost.cshtml', 'editor-new-post', 'admin.post-editor')
s('fill', target='txtTitle', value='Parity post from the editor')
s('fill', target='txtDate', value='2025-03-10 10:00')
s('script', value="editorSetHtml('<p>Written in the <strong>editor</strong>.</p>');")
s('check', target=f'cat-{CAT_BE}')
s('click', 'api.posts-create,admin.post-editor', target=NG('publish(true)'))
s('waitms', ms=2000)
snap('editor-post-saved')
goto('/post/parity-post-from-the-editor', 'post-from-editor', 'api.posts-create')
api('post-create', 'POST', '/api/posts', {'Id': '', 'Title': 'Parity API post', 'Author': '', 'Content': '<p>Posted with the API. welcome</p>', 'DateCreated': '2025-03-11 09:30', 'Slug': '',
                                          'Categories': [{'Id': '', 'Title': 'Parity category'}], 'Tags': [{'TagName': 'parity'}, {'TagName': 'api'}, {'TagName': 'parity'}],
                                          'HasCommentsEnabled': True, 'IsPublished': True}, op='api.posts-create')
api('post-create-same-title', 'POST', '/api/posts', {'Id': '', 'Title': 'Parity API post', 'Content': '<p>The same title: the slug made unique.</p>', 'DateCreated': '2025-03-12 08:00', 'Slug': '',
                                                     'Categories': [], 'Tags': [], 'HasCommentsEnabled': True, 'IsPublished': True}, op='api.posts-create')
api('post-create-draft', 'POST', '/api/posts', {'Id': '', 'Title': 'Parity draft', 'Content': '<p>A draft.</p>', 'DateCreated': '2025-03-13 08:00', 'Slug': 'parity-draft',
                                                'Categories': [], 'Tags': [], 'HasCommentsEnabled': False, 'IsPublished': False}, op='api.posts-create-draft')
api('post-create-bad-date', 'POST', '/api/posts', {'Id': '', 'Title': 'Parity bad date', 'Content': '<p>x</p>', 'DateCreated': '10.03.2025', 'Slug': '', 'Categories': [], 'Tags': [], 'IsPublished': True}, op='api.posts-bad-date')
api('posts-list', 'GET', '/api/posts?take=0&skip=0', op='api.posts-list')
api('posts-list-filtered', 'GET', '/api/posts?take=10&skip=0&filter=IsPublished%20%3D%3D%20true&order=Title', op='api.posts-filter-order')
api('posts-list-bad-filter', 'GET', '/api/posts?take=10&skip=0&filter=NoSuchField%20%3D%3D%201', op='api.posts-filter-order')
api('post-get', 'GET', f'/api/posts/{WELCOME}', op='api.posts-get')
api('post-get-unknown', 'GET', '/api/posts/00000000-0000-0000-0000-000000000001', op='api.posts-get')
api('post-get-bad', 'GET', '/api/posts/not-a-guid', op='api.posts-get')
files('posts-files-created', 'posts/*.xml;categories.xml', 'api.posts-create,api.posts-create-draft')
goto('/', 'home-with-new-posts', 'api.posts-create')
goto('/post/Parity-API-post', 'post-same-slug', 'api.posts-create')
goto('/category/parity-category', 'category-created-by-post', 'api.posts-create')
goto('/?tag=parity', 'tag-created-by-post', 'api.posts-create')
goto('/post/parity-draft', 'draft-signed-in', 'api.posts-create-draft')
script('post-update', "const p = (await api('GET', '/api/posts?take=0')).body.find(x => x.Title == 'Parity API post' && x.DateCreated.indexOf('2025-03-11') == 0); const d = (await api('GET', '/api/posts/' + p.Id)).body; d.Title = 'Parity API post, changed'; d.Content = '<p>Changed with the API. welcome</p>'; return await api('PUT', '/api/posts/update/foo', d);", 'api.posts-update')
goto('/post/Parity-API-post', 'post-updated', 'api.posts-update')
goto('/admin/app/editor/editpost.cshtml?id=' + WELCOME, 'editor-welcome', 'admin.post-editor')
goto('/admin/#/content/posts', 'admin-posts-list', 'admin.screens,api.posts-list')
script('posts-unpublish', checked_js('/api/posts?take=0', "x.Title == 'Parity post from the editor'", '/api/posts/processchecked/unpublish'), 'api.posts-process')
goto('/post/parity-post-from-the-editor', 'post-unpublished-signed-in', 'api.posts-process')
script('posts-publish', checked_js('/api/posts?take=0', "x.Title == 'Parity post from the editor'", '/api/posts/processchecked/publish'), 'api.posts-process')
script('posts-delete-checked', checked_js('/api/posts?take=0', "x.Title == 'Parity post from the editor'", '/api/posts/processchecked/delete'), 'api.posts-process')
api('posts-process-empty', 'PUT', '/api/posts/processchecked/publish', [], op='api.posts-process')
script('post-delete', "const p = (await api('GET', '/api/posts?take=0')).body.find(x => x.Title == 'Parity API post' && x.DateCreated.indexOf('2025-03-12') == 0); return await api('DELETE', '/api/posts/' + p.Id);", 'api.posts-delete')
files('posts-files-after-actions', 'posts/*.xml', 'api.posts-process,api.posts-delete,api.posts-update')
api('trash-list', 'GET', '/api/trash?type=0&take=10&skip=0&filter=1%3D%3D1&order=DateCreated%20descending', op='api.trash')
script('trash-restore', "const t = (await api('GET', '/api/trash?type=2&take=10&skip=0&filter=1==1&order=DateCreated descending')).body.Items.find(x => x.Title == 'Parity post from the editor'); return await api('PUT', '/api/trash/restore/x', t);", 'api.trash')
goto('/post/parity-post-from-the-editor', 'post-restored', 'api.trash')
script('trash-purge', "const t = (await api('GET', '/api/trash?type=2&take=10&skip=0&filter=1==1&order=DateCreated descending')).body.Items.find(x => x.Title == 'Parity API post'); return await api('PUT', '/api/trash/purge/x', t);", 'api.trash')
files('posts-files-after-trash', 'posts/*.xml', 'api.trash')

phase('10. Pages, categories, tags')
goto('/admin/app/editor/editpage.cshtml', 'editor-new-page', 'admin.page-editor')
s('fill', target='txtTitle', value='Parity page from the editor')
s('script', value="editorSetHtml('<p>A page written in the editor.</p>');")
s('click', 'api.pages-create,admin.page-editor', target=NG('publish(true)'))
s('waitms', ms=2000)
snap('editor-page-saved')
goto('/page/parity-page-from-the-editor', 'page-from-editor', 'api.pages-create')
api('page-create', 'POST', '/api/pages', {'Id': '', 'Title': 'Parity child page', 'Content': '<p>A child of About.</p>', 'Description': 'child', 'Keywords': 'parity', 'DateCreated': '2025-03-14 10:00',
                                          'Slug': '', 'ShowInList': True, 'IsPublished': True, 'IsFrontPage': False, 'SortOrder': 5, 'Parent': {'OptionValue': PAGE_ABOUT}}, op='api.pages-create')
api('page-create-no-parent', 'POST', '/api/pages', {'Id': '', 'Title': 'Parity no parent', 'Content': '<p>x</p>', 'DateCreated': '2025-03-14 10:00', 'Slug': '', 'IsPublished': True}, op='api.pages-create')
api('pages-list', 'GET', '/api/pages?take=0&skip=0', op='api.pages-list')
api('page-get', 'GET', f'/api/pages/{PAGE_ABOUT}', op='api.pages-get')
goto('/page/parity-child-page', 'page-child', 'api.pages-create')
script('page-update', "const p = (await api('GET', '/api/pages?take=0')).body.find(x => x.Title == 'Parity child page'); const d = (await api('GET', '/api/pages/' + p.Id)).body; d.Title = 'Parity child page, changed'; return await api('PUT', '/api/pages/update/foo', d);", 'api.pages-update')
script('pages-delete-with-children', checked_js('/api/pages?take=0', "x.Id == '" + PAGE_ABOUT + "'", '/api/pages/processchecked/delete'), 'api.pages-process')
script('pages-unpublish', checked_js('/api/pages?take=0', "x.Title == 'Parity page from the editor'", '/api/pages/processchecked/unpublish'), 'api.pages-process')
goto('/', 'home-page-unpublished', 'api.pages-process')
script('pages-publish', checked_js('/api/pages?take=0', "x.Title == 'Parity page from the editor'", '/api/pages/processchecked/publish'), 'api.pages-process')
script('page-front', "const p = (await api('GET', '/api/pages?take=0')).body.find(x => x.Title == 'Parity page from the editor'); const d = (await api('GET', '/api/pages/' + p.Id)).body; d.IsFrontPage = true; if (!d.Parent) d.Parent = { OptionValue: 'none' }; return await api('PUT', '/api/pages/update/foo', d);", 'page.front-page,api.pages-update')
goto('/', 'home-front-page', 'page.front-page')
goto('/blog', 'blog-with-front-page', 'page.front-page,page.blog')
script('page-front-off', "const p = (await api('GET', '/api/pages?take=0')).body.find(x => x.Title == 'Parity page from the editor'); const d = (await api('GET', '/api/pages/' + p.Id)).body; d.IsFrontPage = false; if (!d.Parent) d.Parent = { OptionValue: 'none' }; return await api('PUT', '/api/pages/update/foo', d);", 'page.front-page')
script('page-delete', "const p = (await api('GET', '/api/pages?take=0')).body.find(x => x.Title == 'Parity child page, changed'); return p ? await api('DELETE', '/api/pages/' + p.Id) : 'no such page';", 'api.pages-delete')
goto(f'/page/example-page?deletepage={PAGE_EXAMPLE}', 'delete-page-query', 'page.delete-page-query')
files('pages-files', 'pages/*.xml', 'api.pages-create,api.pages-update,api.pages-process,api.pages-delete,page.delete-page-query')
api('categories-list', 'GET', '/api/categories?take=0&skip=0', op='api.categories-list')
api('category-create', 'POST', '/api/categories', {'Title': 'Parity child category', 'Description': 'made with the API', 'Parent': {'OptionValue': CAT_BE}}, op='api.categories-create')
api('category-create-duplicate', 'POST', '/api/categories', {'Title': 'Parity child category', 'Description': 'again', 'Parent': {'OptionValue': ''}}, op='api.categories-create')
api('category-get', 'GET', f'/api/categories?Id={CAT_BE}', op='api.categories-list')
script('category-update', "const c = (await api('GET', '/api/categories?take=0')).body.find(x => x.Title == 'Parity child category'); c.Title = 'Parity renamed category'; c.Description = 'renamed'; return await api('PUT', '/api/categories/update/' + c.Id, c);", 'api.categories-update')
goto('/category/parity-renamed-category', 'category-renamed', 'api.categories-update')
script('categories-delete-checked', checked_js('/api/categories?take=0', "x.Title == 'Parity renamed category'", '/api/categories/processchecked/delete'), 'api.categories-delete')
script('category-delete', "const c = (await api('GET', '/api/categories?take=0')).body.find(x => x.Title == 'Parity category'); return await api('DELETE', '/api/categories/' + c.Id);", 'api.categories-delete')
files('categories-file', 'categories.xml', 'api.categories-create,api.categories-update,api.categories-delete')
api('tags-list', 'GET', '/api/tags?take=0&skip=0', op='api.tags-list')
api('tags-of-post', 'GET', f'/api/tags?take=0&skip=0&postId={WELCOME}', op='api.tags-list')
api('tag-rename', 'PUT', '/api/tags/update/welcome', {'OldTag': 'welcome', 'NewTag': 'greeting'}, op='api.tags-update')
goto('/?tag=greeting', 'tag-renamed', 'api.tags-update')
api('tag-delete', 'DELETE', '/api/tags/api', op='api.tags-delete')
api('tags-delete-checked', 'PUT', '/api/tags/processchecked/delete', [{'IsChecked': True, 'TagName': 'parity', 'TagCount': 1}], op='api.tags-delete')
api('tags-after', 'GET', '/api/tags?take=0&skip=0', op='api.tags-update,api.tags-delete')

phase('11. Comments: moderation in the admin and on the post page')
api('comments-list', 'GET', '/api/comments', op='api.comments-list')
goto('/admin/#/content/comments', 'admin-comments', 'admin.screens,api.comments-list')
script('comment-get', "const c = (await api('GET', '/api/comments')).body.Items.find(x => x.Author == 'Parity Visitor'); return await api('GET', '/api/comments/' + c.Id);", 'api.comments-get')
script('comments-approve', checked_js('/api/comments', "x.Author == 'Parity Visitor'", '/api/comments/processchecked/approve').replace("(await api('GET', '/api/comments')).body", "(await api('GET', '/api/comments')).body.Items"), 'api.comments-process')
goto('/post/welcome-to-blogengine-net', 'post-comment-approved', 'api.comments-process')
script('comments-unapprove', checked_js('/api/comments', "x.Author == 'Parity Replier'", '/api/comments/processchecked/unapprove').replace("(await api('GET', '/api/comments')).body", "(await api('GET', '/api/comments')).body.Items"), 'api.comments-process')
script('comment-reply', "const c = (await api('GET', '/api/comments')).body.Items.find(x => x.Author == 'Parity Visitor'); const d = (await api('GET', '/api/comments/' + c.Id)).body; return await api('POST', '/api/comments', { ParentId: d.Id, PostId: d.PostId, Content: 'The administrator answers.' });", 'api.comments-reply')
goto('/post/welcome-to-blogengine-net')
comment('Admin', 'post@example.com', 'A comment of the signed-in administrator.', captcha=None, op='comment.post-authenticated')
snap('comment-authenticated')
goto('/post/welcome-to-blogengine-net', 'post-comments-signed-in', 'comment.post-authenticated,comment.moderate-query')
script('comment-moderate-query', f"const c = (await api('GET', '/api/comments')).body.Items.find(x => x.Author == 'Parity Replier'); const r = await fetch('/post/welcome-to-blogengine-net?approvecomment=' + c.Id, {{ redirect: 'manual' }}); return r.status + ' ' + r.type;", 'comment.moderate-query')
script('comment-delete-query', f"const c = (await api('GET', '/api/comments')).body.Items.find(x => x.Author == 'Parity Replier'); const r = await fetch('/post/welcome-to-blogengine-net?deletecomment=' + c.Id, {{ redirect: 'manual' }}); return r.status + ' ' + r.type;", 'comment.moderate-query')
fetch('/post/welcome-to-blogengine-net?approveallcomments=true', 'comment-approve-all-query', 'comment.moderate-query')
api('comments-delete-all-spam', 'PUT', '/api/comments/deleteall/spam', {}, op='api.comments-delete-all')
api('comments-delete-all-pending', 'PUT', '/api/comments/deleteall/pending', {}, op='api.comments-delete-all')
script('comment-delete', "const c = (await api('GET', '/api/comments')).body.Items.find(x => x.Title && x.Title.indexOf('administrator answers') >= 0); return c ? await api('DELETE', '/api/comments/' + c.Id) : 'not found';", 'api.comments-delete')
files('comments-file-after-moderation', f'posts/{WELCOME}.xml', 'api.comments-process,api.comments-reply,api.comments-delete,comment.moderate-query')
api('comment-filters-list', 'GET', '/api/commentfilter?take=0&skip=0', op='api.comment-filters')
api('comment-filter-add', 'POST', '/api/commentfilter', {'Action': 'Block', 'Subject': 'Email', 'Operation': 'Contains', 'Filter': 'spammer.example'}, op='api.comment-filters')
api('comment-filter-add-duplicate', 'POST', '/api/commentfilter', {'Action': 'Block', 'Subject': 'Email', 'Operation': 'Contains', 'Filter': 'spammer.example'}, op='api.comment-filters')
api('comment-filters-after-add', 'GET', '/api/commentfilter?take=0&skip=0', op='api.comment-filters')
script('comment-filters-delete-checked', checked_js('/api/commentfilter?take=0&skip=0', "x.Filter == 'spammer.example'", '/api/commentfilter/processchecked/delete'), 'api.comment-filters')
api('comment-filters-delete-all', 'PUT', '/api/commentfilter/deleteall/foo', {}, op='api.comment-filters')
files('comment-filters-file', 'datastore/extensions/MetaExtension.xml', 'api.comment-filters')

phase('12. Users, roles, profile; an editor')
api('users-list', 'GET', '/api/users?take=0&skip=0&filter=1%20%3D%3D%201&order=UserName', op='api.users-list')
files('profiles-after-users-list', 'profiles/*.xml', 'api.users-list')
api('roles-of-new-user', 'GET', '/api/roles/getuserroles/fakeusername', op='api.roles-rights')
api('user-create', 'POST', '/api/users', {'UserName': 'parity-editor', 'Email': 'editor@example.com', 'Password': 'Parity-1',
                                          'Profile': {'DisplayName': 'Parity Editor', 'FirstName': 'Parity', 'LastName': 'Editor', 'EmailAddress': 'editor@example.com', 'Birthday': '1990-01-02', 'Private': False, 'AboutMe': 'The editor of the parity test.'},
                                          'Roles': [{'RoleName': 'Editors', 'IsChecked': True}]}, op='api.users-create')
api('user-create-no-password', 'POST', '/api/users', {'UserName': 'parity-nopassword', 'Email': 'nop@example.com', 'Password': '', 'Roles': []}, op='api.users-create-invalid')
api('user-create-existing', 'POST', '/api/users', {'UserName': 'parity-editor', 'Email': 'editor2@example.com', 'Password': 'Parity-1', 'Profile': {'DisplayName': 'x'}, 'Roles': []}, op='api.users-create-invalid')
api('user-create-other', 'POST', '/api/users', {'UserName': 'parity-other', 'Email': 'other@example.com', 'Password': 'Parity-2', 'Profile': {'DisplayName': 'Parity Other'}, 'Roles': []}, op='api.users-create')
api('user-get', 'GET', '/api/users/parity-editor', op='api.users-list')
api('user-roles', 'GET', '/api/roles/getuserroles/parity-editor', op='api.roles-rights')
script('user-update', "const u = (await api('GET', '/api/users/parity-other')).body; u.Email = 'other-changed@example.com'; u.OldPassword = 'Parity-2'; u.Password = 'Parity-3'; u.Roles = [{ RoleName: 'Editors', IsChecked: true }]; return await api('PUT', '/api/users/update/item', u);", 'api.users-update')
files('users-files', 'users.xml;roles.xml;profiles/*.xml', 'api.users-create,api.users-update')
api('user-create-to-delete', 'POST', '/api/users', {'UserName': 'parity-temp', 'Email': 'temp@example.com', 'Password': 'Parity-9', 'Profile': {'DisplayName': 'Temp'}, 'Roles': []}, op='api.users-delete')
api('user-delete', 'DELETE', '/api/users/parity-temp', op='api.users-delete')
api('user-delete-last-admin', 'DELETE', '/api/users/Admin', op='api.users-delete')
api('users-delete-checked', 'PUT', '/api/users/processchecked/delete', [{'IsChecked': True, 'UserName': 'parity-nopassword'}], op='api.users-delete')
files('users-files-after-delete', 'users.xml;roles.xml;profiles/*.xml', 'api.users-delete')
api('roles-list', 'GET', '/api/roles?take=0&skip=0', op='api.roles')
api('role-create', 'POST', '/api/roles', {'RoleName': 'Parity Authors'}, op='api.roles')
api('role-create-existing', 'POST', '/api/roles', {'RoleName': 'Editors'}, op='api.roles')
api('role-get', 'GET', '/api/roles/get/Editors', op='api.roles')
api('role-rights', 'GET', '/api/roles/getrights/Parity%20Authors', op='api.roles-rights')
script('role-save-rights', "const g = (await api('GET', '/api/roles/getrights/Parity%20Authors')).body; g.forEach(x => x.Permissions.forEach(p => p.IsChecked = ['ViewPublicPosts', 'CreateNewPosts', 'EditOwnPosts', 'AccessAdminPages'].includes(p.Id))); return await api('PUT', '/api/roles/saverights/Parity%20Authors', g);", 'api.roles-rights')
files('rights-after-save', 'rights.xml;roles.xml', 'api.roles-rights,api.roles')
api('role-delete-system', 'DELETE', '/api/roles/Administrators', op='api.roles')
api('role-delete', 'DELETE', '/api/roles/Parity%20Authors', op='api.roles')
files('rights-after-delete', 'rights.xml;roles.xml', 'api.roles')
api('profile-get', 'GET', '/api/users/Admin', op='api.profile')
script('profile-save', "const u = (await api('GET', '/api/users/Admin')).body; u.Profile.DisplayName = 'Parity Administrator'; u.Profile.AboutMe = 'Edited on the profile screen.'; u.Profile.CityTown = 'Tokyo'; return await api('PUT', '/api/users/saveprofile/item', u);", 'api.profile')
files('profile-file', 'profiles/admin.xml', 'api.profile')
fetch('/foaf_Admin.axd', 'foaf-after-profile', 'api.profile,h.foaf', content='xml')
s('newcontext')
login('parity-editor', 'Parity-1')
goto('/', 'editor-home', 'sec.api-editor,widget.administration')
goto('/admin/', 'editor-admin-shell', 'sec.api-editor,admin.shell')
for url, name in [('/api/posts?take=0', 'posts'), ('/api/dashboard', 'dashboard'), ('/api/users?take=0&skip=0&filter=1%20%3D%3D%201&order=UserName', 'users'), ('/api/roles', 'roles'),
                  ('/api/settings', 'settings'), ('/api/comments', 'comments'), ('/api/lookups', 'lookups'), ('/api/trash?type=0&take=10&skip=0&filter=1%3D%3D1&order=DateCreated%20descending', 'trash'),
                  ('/api/packages?filter=extensions', 'plugins'), ('/api/blogs', 'blogs')]:
    api('editor-api-' + name, 'GET', url, op='sec.api-editor')
api('editor-post-create', 'POST', '/api/posts', {'Id': '', 'Title': 'Parity editor post', 'Content': '<p>By the editor.</p>', 'DateCreated': '2025-03-15 10:00', 'Slug': '', 'Categories': [], 'Tags': [], 'IsPublished': True}, op='sec.api-editor')
api('editor-delete-others-post', 'DELETE', f'/api/posts/{WELCOME}', op='sec.api-editor')
api('editor-user-create', 'POST', '/api/users', {'UserName': 'x', 'Email': 'x@example.com', 'Password': 'x', 'Roles': []}, op='sec.api-editor')
api('editor-role-create', 'POST', '/api/roles', {'RoleName': 'x'}, op='sec.api-editor')
goto('/post/welcome-to-blogengine-net', 'editor-post-page', 'sec.api-editor')
fetch('/blogml.axd', 'editor-blogml', 'sec.api-editor,h.blogml')
files('posts-files-after-editor', 'posts/*.xml', 'sec.api-editor')
s('newcontext')
login('admin', 'admin')

phase('13. Settings, lookups, custom fields, blog roll, tools, logs, blogs')
goto('/admin/#/settings/basic', None)
s('fill', target='txtName', value='Parity Blog')
s('fill', target='txtDescription', value='Recorded by the parity test')
s('fill', target='txtPostsPerPage', value='2')
s('click', 'api.settings-save', target=NG('save()'))
s('waitms', ms=1000)
snap('settings-basic-saved')
goto('/', 'home-after-settings', 'api.settings-save,page.home-paging')
goto('/?page=2', 'home-page-2-after-settings', 'page.home-paging,api.settings-save')
script('settings-self-registration', "const vm = (await api('GET', '/api/settings')).body; vm.Settings.EnableSelfRegistration = true; vm.Settings.Culture = 'Auto'; return await api('PUT', '/api/settings', vm.Settings);", 'api.settings-save')
files('settings-file', '~settings.xml', 'api.settings-save')
api('settings-clear-cache', 'PUT', '/api/settings?action=clearCache', {}, op='api.settings-clear-cache')
api('lookups', 'GET', '/api/lookups', op='api.lookups')
api('lookups-editor-options', 'PUT', '/api/lookups/update/foo', {'OptionType': 'Post', 'ShowSlug': True, 'ShowDescription': True, 'ShowCustomFields': True}, op='api.lookups')
api('custom-fields-theme', 'GET', '/api/customfields?filter=CustomType%20%3D%3D%20%22THEME%22', op='api.custom-fields')
api('custom-field-add', 'POST', '/api/customfields', {'CustomType': 'POST', 'ObjectId': WELCOME, 'Key': 'parity-key', 'Value': 'parity value'}, op='api.custom-fields')
api('custom-fields-of-post', 'GET', f'/api/customfields?filter=CustomType%20%3D%3D%20%22POST%22%20%26%26%20ObjectId%20%3D%3D%20%22{WELCOME}%22', op='api.custom-fields')
api('custom-fields-replace', 'PUT', '/api/customfields', [{'CustomType': 'POST', 'ObjectId': WELCOME, 'BlogId': '96d5b379-7e1d-4dac-a6ba-1e50db561b04', 'Key': 'parity-key', 'Value': 'changed value'}], op='api.custom-fields')
api('custom-field-delete', 'DELETE', '/api/customfields', {'CustomType': 'POST', 'ObjectId': WELCOME, 'Key': 'parity-key'}, op='api.custom-fields')
files('custom-fields-file', 'customfields.xml', 'api.custom-fields')
api('blogroll-list', 'GET', '/api/blogroll', op='api.blogroll')
api('blogroll-add', 'POST', '/api/blogroll', {'Title': 'Parity blog', 'Description': 'a local feed that does not answer', 'BlogUrl': 'http://127.0.0.1:9/', 'FeedUrl': 'http://127.0.0.1:9/feed', 'Xfn': 'friend', 'SortIndex': 4}, op='api.blogroll')
script('blogroll-update', "const b = (await api('GET', '/api/blogroll')).body.find(x => x.Title == 'Parity blog'); b.Description = 'changed'; const r = await api('PUT', '/api/blogroll/update/item', b); const g = await api('GET', '/api/blogroll/' + b.Id); return { update: r, get: g };", 'api.blogroll')
api('blogroll-delete-broken', 'PUT', '/api/blogroll/processchecked/delete', [], op='api.blogroll')
files('blogroll-file', 'blogroll.xml', 'api.blogroll')
fetch('/opml.axd', 'opml-after-blogroll', 'api.blogroll,h.opml', content='xml')
for t in ['check1', 'trust', 'data', 'root', 'Custom']:
    api('tools-' + t.lower(), 'GET', '/api/tools/' + t, op='api.tools')
api('logs', 'GET', '/api/logs/getlog/file', op='api.logs')
api('logs-purge', 'PUT', '/api/logs/purgelog/file', {}, op='api.logs')
api('logs-after-purge', 'GET', '/api/logs/getlog/file', op='api.logs')
api('blogs-list', 'GET', '/api/blogs?take=0&skip=0&filter=1%20%3D%3D%201&order=Name', op='api.blogs')
api('blog-get', 'GET', '/api/blogs/96d5b379-7e1d-4dac-a6ba-1e50db561b04', op='api.blogs')
api('blog-create', 'POST', '/api/blogs', {'Name': 'parityblog', 'UserName': 'parity-blogger', 'Email': 'blogger@example.com', 'Password': 'Parity-4', 'ConfirmPassword': 'Parity-4'}, op='api.blogs')
files('blogs-files', 'blogs.xml;blogs/parityblog/**;' + UNORDERED, 'api.blogs')
goto('/parityblog/', 'child-blog-home', 'api.blogs')
script('blog-update', "const b = (await api('GET', '/api/blogs?take=0&skip=0&filter=1 == 1&order=Name')).body.find(x => x.Name == 'parityblog'); b.IsActive = false; return await api('PUT', '/api/blogs/update/item', b);", 'api.blogs')
script('blogs-delete-checked', checked_js('/api/blogs?take=0&skip=0&filter=1 == 1&order=Name', "x.Name == 'parityblog'", '/api/blogs/processchecked/delete'), 'api.blogs')
files('blogs-files-after-delete', 'blogs.xml;blogs/**', 'api.blogs')
api('stats', 'GET', '/api/stats', op='api.stats-pager')
api('pager', 'GET', '/api/pager', op='api.stats-pager')

phase('14. Widgets, themes, plugins')
api('widgets', 'GET', '/api/widgets', op='api.widgets-get')
goto(f'/Custom/Widgets/TextBox/edit.cshtml?id={W_TEXTBOX}&zone=sidebar-Post', 'widget-edit-textbox', 'widget.edit-page')
fetch(f'/Custom/Widgets/TextBox/edit.cshtml?id={W_TEXTBOX}&zone=sidebar-Post', 'widget-save-textbox', 'widget.edit-textbox', method='POST', body='txtContent=%3Cp%3EParity+%3Cb%3Etext%3C%2Fb%3E+box%3C%2Fp%3E', bodyMatch='<textarea[^>]*>[^<]*')
goto(f'/Custom/Widgets/PostList/edit.cshtml?id={W_POSTLIST}&zone=sidebar-Post', 'widget-edit-postlist', 'widget.edit-page')
fetch(f'/Custom/Widgets/PostList/edit.cshtml?id={W_POSTLIST}&zone=sidebar-Post', 'widget-save-postlist', 'widget.edit-settings', method='POST', body='txtNumberOfPosts=2&ddlCategories=&ddlAuthors=All&ddlSortBy=Alphabetical&cbShowImg=on&cbShowDate=on', content='text')
goto(f'/Custom/Widgets/TagCloud/edit.cshtml?id={W_TAGCLOUD}&zone=sidebar-Post', 'widget-edit-tagcloud', 'widget.edit-page')
goto(f'/Custom/Widgets/Newsletter/edit.cshtml?id={W_NEWSLETTER}&zone=sidebar-Post', 'widget-edit-newsletter', 'widget.edit-page,widget.edit-newsletter')
fetch(f'/Custom/Widgets/Newsletter/edit.cshtml?id={W_NEWSLETTER}&zone=sidebar-Post', 'widget-save-newsletter', 'widget.edit-newsletter', method='POST', body='hdnEmail=subscriber%40example.com', content='text')
files('newsletter-file-after-edit', 'newsletter.xml', 'widget.edit-newsletter')
goto(f'/Custom/Widgets/BlogRoll/edit.cshtml?id={W_BLOGROLL}&zone=sidebar-Post', 'widget-edit-blogroll', 'widget.edit-page,widget.edit-blogroll')
goto('/Custom/Widgets/LinkList/edit.cshtml?id=parity-linklist&zone=sidebar-Post', 'widget-edit-linklist', 'widget.edit-page')
fetch('/Custom/Widgets/LinkList/edit.cshtml?id=parity-linklist&zone=sidebar-Post', 'widget-linklist-add', 'widget.edit-linklist', method='POST', body='txtNumberOfLinks=5&txtTitle=Parity+link&txtUrl=http%3A%2F%2Fexample.com%2Fparity&btnAdd=', content='text')
files('widget-files', 'datastore/widgets/*.xml;' + UNORDERED, 'widget.edit-textbox,widget.edit-settings,widget.edit-linklist')
script('widgets-update', "const vm = (await api('GET', '/api/widgets')).body; const zone = vm.WidgetZones.find(z => z.Id == 'sidebar-Post'); zone.Widgets = zone.Widgets.filter(w => w.Name != 'BlogRoll'); zone.Widgets.push({ Id: '1234', Name: 'LinkList', Title: 'Parity links', ShowTitle: true }); zone.Widgets.push({ Id: '5678', Name: 'MonthList', Title: 'Parity months', ShowTitle: true }); return await api('PUT', '/api/widgets/update/item', [zone]);", 'api.widgets-update')
files('widget-zone-file', 'datastore/widgets/sidebar-Post.xml', 'api.widgets-update')
goto('/', 'home-after-widgets', 'api.widgets-update,widget.edit-textbox,widget.edit-settings')
api('themes', 'GET', '/api/packages?take=0&skip=0&filter=themes&order=LastUpdated%20desc', op='api.themes')
api('widgets-installed', 'GET', '/api/packages?take=0&skip=0&filter=widgets&order=LastUpdated%20desc', op='api.themes')
api('theme-set', 'PUT', '/api/packages/settheme/Standard-2017', {}, op='api.themes')
goto('/', 'home-theme-standard-2017', 'api.themes')
goto('/post/welcome-to-blogengine-net', 'post-theme-standard-2017', 'api.themes')
api('theme-set-back', 'PUT', '/api/packages/settheme/Standard', {}, op='api.themes')
goto('/admin/Extensions/Settings.aspx?ext=BBCode', 'extension-settings-bbcode', 'api.plugins')
script('plugins-enable', checked_js('/api/packages?take=0&skip=0&filter=extensions', "x.Id == 'AkismetFilter'", '/api/packages/processchecked/enable'), 'api.plugins')
script('plugins-disable-again', checked_js('/api/packages?take=0&skip=0&filter=extensions', "x.Id == 'AkismetFilter'", '/api/packages/processchecked/disable'), 'api.plugins')
api('gallery-all', 'GET', '/api/packages?take=0&skip=0&filter=all&order=LastUpdated%20desc', op='api.packages-gallery')
api('gallery-package', 'GET', '/api/packages/BBCode', op='api.packages-gallery')

phase('15. Files: the file manager, uploads, BlogML import and export')
api('file-manager-list', 'GET', '/api/filemanager?take=0&skip=0', op='api.file-manager')
goto('/admin/app/editor/filemanager.cshtml', 'file-manager-page', 'api.file-manager')
api('file-manager-add-folder', 'PUT', '/api/filemanager/addfolder/add', {'Name': 'parity-folder', 'FullPath': ''}, op='api.file-manager')
fetch('/api/upload?action=filemgr&dirPath=/parity-folder', 'upload-filemgr', 'api.upload', method='POST', content='json', upload='file=parity-notes.txt:Notes of the parity test.')
fetch('/api/upload?action=image', 'upload-image', 'api.upload', method='POST', content='json', upload='file=parity.png:base64:iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==')
fetch('/api/upload?action=file', 'upload-file', 'api.upload', method='POST', content='json', upload='file=parity-document.txt:A document.')
fetch('/api/upload?action=image', 'upload-empty', 'api.upload', method='POST', content='json', upload='file=:')
api('file-manager-folder', 'GET', '/api/filemanager?take=0&skip=0&path=/parity-folder', op='api.file-manager')
fetch('/file.axd?file=/parity-folder/parity-notes.txt', 'upload-served', 'api.upload,h.file', content='text')
files('files-after-uploads', 'files/**', 'api.upload,api.file-manager')
script('file-manager-delete', checked_js('/api/filemanager?take=0&skip=0&path=/parity-folder', "x.Name == 'parity-notes.txt'", '/api/filemanager/processchecked/delete'), 'api.file-manager')
fetch('/api/upload?action=profile', 'upload-profile', 'api.upload', method='POST', content='json', upload='file=parity-avatar.png:base64:iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==')
files('files-after-delete', 'files/**', 'api.file-manager,api.upload')
s('fetch', 'h.blogml', path='/blogml.axd', name='blogml-export', content='xml', show=['Content-Disposition'])
fetch('/api/upload?action=import', 'blogml-import', 'api.upload-import', method='POST', content='json',
      upload='file=parity-blogml.xml:<?xml version="1.0" encoding="utf-8"?><blog xmlns="http://www.blogml.com/2006/09/BlogML" date-created="2025-01-01T00:00:00" root-url="/"><title type="text"><![CDATA[Imported]]></title><sub-title type="text"><![CDATA[]]></sub-title><authors><author id="1" date-created="2025-01-01T00:00:00" date-modified="2025-01-01T00:00:00" approved="true" email="imported@example.com"><title type="text"><![CDATA[Admin]]></title></author></authors><categories><category id="9" date-created="2025-01-01T00:00:00" date-modified="2025-01-01T00:00:00" approved="true" parentref="0"><title type="text"><![CDATA[Imported category]]></title></category></categories><posts><post id="77" date-created="2025-02-01T10:00:00" date-modified="2025-02-01T10:00:00" approved="true" post-url="/post/imported" type="normal" hasexcerpt="false" views="0" is-published="True"><title type="text"><![CDATA[Imported post]]></title><content type="text"><![CDATA[<p>From a BlogML document.</p>]]></content><post-name type="text"><![CDATA[imported-post]]></post-name><categories><category ref="9" /></categories><authors><author ref="1" /></authors></post></posts></blog>')
goto('/post/imported', 'imported-post', 'api.upload-import')

phase('16. MetaWeblog (XML-RPC), as Windows Live Writer uses it')
goto('/')
script('metaweblog-auth', rpc_js(["'blogger.getUsersBlogs', [str('app'), str('admin'), str('wrong')]", "'no.such.method', [str('x')]", "'metaWeblog.getPost', [str('x'), str('admin'), str('admin')]"]), 'h.metaweblog-auth')
script('metaweblog-read', rpc_js(["'blogger.getUsersBlogs', [str('app'), str('admin'), str('admin')]", "'metaWeblog.getUsersBlogs', [str('app'), str('admin'), str('admin')]",
                                  "'metaWeblog.getCategories', [str('1'), str('admin'), str('admin')]", "'wp.getTags', [str('1'), str('admin'), str('admin')]", "'wp.getAuthors', [str('1'), str('admin'), str('admin')]",
                                  "'metaWeblog.getRecentPosts', [str('1'), str('admin'), str('admin'), v('<int>3</int>')]", f"'metaWeblog.getPost', [str('{WELCOME}'), str('admin'), str('admin')]",
                                  "'blogger.getUserInfo', [str('app'), str('admin'), str('admin')]"]), 'h.metaweblog-users-blogs,h.metaweblog-categories,h.metaweblog-recent,h.metaweblog-user-info')
script('metaweblog-posts', rpc_js(["'metaWeblog.newPost', [str('1'), str('admin'), str('admin'), st({ title: str('Parity XML-RPC post'), description: str('<p>Written over XML-RPC.</p>'), categories: arr([str('BlogEngine.NET')]), mt_keywords: str('rpc, parity'), dateCreated: v('<dateTime.iso8601>20250316T10:00:00</dateTime.iso8601>') }), b(true)]",
                                   "'metaWeblog.editPost', [str(id(0)), str('admin'), str('admin'), st({ title: str('Parity XML-RPC post, edited'), description: str('<p>Edited over XML-RPC.</p>'), dateCreated: v('<dateTime.iso8601>20250316T10:00:00</dateTime.iso8601>') }), b(true)]",
                                   "'metaWeblog.getPost', [str(id(0)), str('admin'), str('admin')]",
                                   "'metaWeblog.newMediaObject', [str('1'), str('admin'), str('admin'), st({ name: str('parity-rpc.png'), type: str('image/png'), bits: v('<base64>iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==</base64>') })]",
                                   "'blogger.deletePost', [str('app'), str(id(0)), str('admin'), str('admin'), b(true)]"]), 'h.metaweblog-new-post,h.metaweblog-delete-post,h.metaweblog-media')
script('metaweblog-pages', rpc_js(["'wp.newPage', [str('1'), str('admin'), str('admin'), st({ title: str('Parity XML-RPC page'), description: str('<p>A page over XML-RPC.</p>'), wp_page_parent_id: str('0'), dateCreated: v('<dateTime.iso8601>20250316T10:00:00</dateTime.iso8601>') }), b(true)]",
                                   "'wp.getPageList', [str('1'), str('admin'), str('admin')]", "'wp.getPages', [str('1'), str('admin'), str('admin')]",
                                   "'wp.getPage', [str('1'), str(id(0)), str('admin'), str('admin')]",
                                   "'wp.editPage', [str('1'), str(id(0)), str('admin'), str('admin'), st({ title: str('Parity XML-RPC page, edited'), description: str('<p>Edited.</p>'), wp_page_parent_id: str('0') }), b(true)]",
                                   "'wp.deletePage', [str('1'), str('admin'), str('admin'), str(id(0))]"]), 'h.metaweblog-pages')
files('files-after-metaweblog', 'posts/*.xml;pages/*.xml;files/parity-rpc.png', 'h.metaweblog-new-post,h.metaweblog-pages,h.metaweblog-media')

phase('17. Account: change the password, sign out, register, the password retrieval')
goto('/Account/change-password.aspx', 'change-password-page', 'acct.change-password-page')
s('click', 'acct.change-password-validation', target="css=input[type=submit]")
snap('change-password-required')
s('fill', target='CurrentPassword', value='wrong')
s('fill', target='NewPassword', value='Parity-5')
s('fill', target='ConfirmNewPassword', value='Parity-5')
s('click', 'acct.change-password-wrong', target="css=input[type=submit]")
snap('change-password-wrong')
goto('/Account/change-password.aspx')
s('fill', target='CurrentPassword', value='admin')
s('fill', target='NewPassword', value='Parity-5')
s('fill', target='ConfirmNewPassword', value='Parity-6')
s('click', 'acct.change-password-validation', target="css=input[type=submit]")
snap('change-password-mismatch')
goto('/Account/change-password.aspx')
s('fill', target='CurrentPassword', value='admin')
s('fill', target='NewPassword', value='Parity-5')
s('fill', target='ConfirmNewPassword', value='Parity-5')
s('click', 'acct.change-password,acct.change-password-success', target="css=input[type=submit]")
snap('change-password-done')
files('users-file-after-password', 'users.xml', 'acct.change-password')
fetch('/Account/Logout.cshtml?ReturnUrl=%2Farchive', 'logout-cshtml', 'acct.logout-cshtml')
s('cookies', 'acct.logout-cshtml', name='cookies-after-logout')
login('admin', 'admin', op='acct.change-password')
snap('login-old-password')
login('admin', 'Parity-5', op='acct.change-password')
snap('login-new-password')
s('newcontext')
goto('/Account/register.aspx', 'register-page', 'acct.register-page')
s('click', 'acct.register-validation', target='CreateUserButton')
snap('register-required')
s('fill', target='UserName', value='parity-reader')
s('fill', target='Email', value='reader@example.com')
s('fill', target='Password', value='Parity-7')
s('fill', target='ConfirmPassword', value='Parity-8')
s('click', 'acct.register-validation', target='CreateUserButton')
snap('register-mismatch')
s('fill', target='ConfirmPassword', value='Parity-7')
s('click', 'acct.register', target='CreateUserButton')
snap('registered')
s('cookies', 'acct.register', name='cookies-registered')
files('users-files-after-register', 'users.xml;roles.xml;profiles/parity-reader.xml', 'acct.register')
s('newcontext')
goto('/Account/register.aspx')
s('fill', target='UserName', value='parity-reader')
s('fill', target='Email', value='reader2@example.com')
s('fill', target='Password', value='Parity-7')
s('fill', target='ConfirmPassword', value='Parity-7')
s('click', 'acct.register-duplicate', target='CreateUserButton')
snap('register-duplicate')
goto('/Account/password-retrieval.aspx', 'password-retrieval-page', 'acct.password-retrieval-page')
s('fill', target='txtUser', value='nobody')
s('click', 'acct.password-retrieval-unknown', target='LoginButton')
snap('password-retrieval-unknown')
s('fill', target='txtUser', value='parity-other')
s('click', 'acct.password-retrieval-send', target='LoginButton')
snap('password-retrieval-send')
goto('/contact')
s('fill', target='txtName', value='Parity Visitor')
s('fill', target='txtEmail', value='visitor@example.com')
s('fill', target='txtSubject', value='Hello')
s('fill', target='txtMessage', value='A message from the parity test.')
s('click', 'page.contact-page', target='btnSend')
s('waitms', ms=2000)
snap('contact-sent')
goto('/contact')
s('click', 'page.contact-page', target='btnSend')
snap('contact-required')

phase('18. The state App_Data was left in')
files('app-data-final', APP_DATA_ALL, 'state.app-data-initial')

here = os.path.dirname(os.path.abspath(__file__))
scenario = {
    '_comment': 'BlogEngine.NET: every user-facing operation of corpora/regression/be.operations.json (be.operations.md), each step\'s op naming the operations it exercises. Golden: corpora/parity/be.operations.golden.json, recorded on the original on IIS by corpora/record-be-operations.ps1; verified by experiments/wf4c/verify-windows.ps1 -Name be -Suite operations and experiments/wf4c/run-linux.ps1 ... -Suite operations. Every run starts from the release\'s App_Data (corpora/be-app-data.ps1); the files steps read it (--files). The administrator is the release\'s admin/admin; the scenario makes an editor (parity-editor / Parity-1). What would ask the Internet is first turned off by the administrator (the ping services removed, the SMTP server set to 127.0.0.1:9, the StopForumSpam and Akismet comment filters disabled: each one an operation compared); URLs the server is given to read are local ones that do not answer (127.0.0.1:9). External requests of the browser are stubbed (stubExternal).',
    'operations': 'be.operations.json',
    'allowed': '../parity/be.operations.allowed.json',
    'textMasks': [
        '{today}',
        # The time of an ISO date and time (2026-11-01T17:26:48: the run's).
        '(?<=T)\\d{2}:\\d{2}:\\d{2}',
        '\\b\\d{1,2}:\\d{2}(:\\d{2}(\\.\\d+)?)?( ?[AP]M)?\\b',
        '[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}',
        '(?<=© )20\\d\\d',
        # The process's identity (the IIS pool's, the converted run's user).
        '(?<=Identity is: ).*',
        '(?<=Identity: ).*',
        # An upload's folder (files/yyyy/MM) in a URL.
        '(?<=%2f)20\\d\\d%2f\\d\\d(?=%2f)',
        # The file manager's dates of the files: when the site's files were copied.
        '(?<=\\.Created: ")[^"]*',
    ],
    'ignoreSelectors': ['.widget.blogroll ul.xoxo li ul'],
    'maskIds': ['[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}'],
    'unorderedRows': ['table.table-item-comments'],
    'waitMs': 300,
    'quietMs': 600,
    'continueOnError': True,
    'stubExternal': True,
    'steps': STEPS,
}
out = sys.argv[1] if len(sys.argv) > 1 else os.path.join(here, 'be.operations.scenario.json')
with open(out, 'w', encoding='utf-8', newline='\n') as f:
    json.dump(scenario, f, ensure_ascii=False, indent=1)
    f.write('\n')
print(len(STEPS), 'steps', sum(1 for x in STEPS if x.get('action') in ('snapshot', 'fetch', 'files', 'cookies', 'assets') or (x.get('action') == 'script' and x.get('name'))), 'snapshots')
