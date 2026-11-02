# Writes corpora/regression/be.operations.json (the inventory ParityTest reads) and be.operations.md (the same, to read)
# from the table below: every user-facing operation of BlogEngine.NET 3.3.8, found by reading the original's source (the
# pages, controls, themes, widgets, the HTTP modules - UrlRewrite, Security, Right -, the handlers of Web.Config, the Web
# API controllers and the repositories behind them, the admin single-page application's screens and their calls) and by
# crawling the running application (ParityTest discover as the anonymous visitor, the administrator and an editor:
# corpora/regression/be.discovered.*.json, the links, forms, inputs, the admin's routes and the Web API calls its screens
# make). Edit the table and run: python corpora/regression/be-operations-inventory.py
import json, os

A, ADM, ED, ANY = 'anonymous', 'administrator', 'editor', 'any'
OPS = []


def op(id, category, page, element, expected, role=ANY, pre=None, inputs=None, keys=None, runnable=True, reason=None, partial=None):
    OPS.append({k: v for k, v in dict(id=id, category=category, page=page, element=element, preconditions=pre, role=role,
                                       inputs=inputs, expected=expected, keys=keys, runnable=runnable, reason=reason,
                                       partial=partial).items() if v is not None})


EXT = 'the outbound request goes to the Internet'

# --- the public pages (UrlRewrite: Core/Web/HttpModules/UrlRewrite.cs, UrlRules.cs) ---
op('page.home', 'page', '/ (default.aspx)', 'GET: the posts (PostList), the theme Standard, the sidebar zone (widgets)', 'the welcome post, the widgets', keys=['GET /', 'GET /default*'])
op('page.home-paging', 'page', '/?page=N', 'PostPager: page=N links (postsperpage)', 'the page N of the posts; an empty page past the last', inputs=['page=2', 'page=99'], keys=['GET /?page'])
op('page.post', 'page', '/post/{slug}', 'UrlRewrite /post/: post.aspx?id= (PostView, RelatedPosts, PostNavigation, CommentList, CommentForm)', 'the post with its comments and the comment form',
   keys=['GET /post/*'])
op('page.post-dated', 'page', '/post/yyyy/mm/dd/{slug}', 'a post\'s link with a date (TimeStampPostLinks off: only the slug counts)', 'the post')
op('page.post-unknown', 'page', '/post/no-such-post', 'a slug no post has', 'the 404 page (error404.aspx)')
op('page.post-by-id', 'page', '/post.aspx?id={guid}', 'post.aspx with the id in the raw URL', '301 to the post\'s canonical link')
op('page.post-no-id', 'page', '/post.aspx', 'post.aspx without a post', 'redirect to error404')
op('page.category', 'page', '/category/{slug}', 'UrlRewrite /category/: default.aspx?id={category}', 'the category\'s posts', keys=['GET /category/*'])
op('page.tag', 'page', '/?tag={tag}', 'default.aspx ?tag= (the tags\' links)', 'the posts with the tag', keys=['GET /?tag'])
op('page.author', 'page', '/author/{name}', 'UrlRewrite /author/: default.aspx?name=', 'the author\'s posts', keys=['GET /author/*'])
op('page.date', 'page', '/yyyy/mm/default, /yyyy/mm/dd/default, /?year=', 'UrlRewrite of the date URLs: default.aspx?year=&month= / date=; a raw year= 301', 'the posts of the month / day; year= redirects',
   keys=['GET /20*'])
op('page.calendar', 'page', '/calendar', 'default.aspx?calendar=show (PostCalendar)', 'the calendar of the posts', keys=['GET /calendar*'])
op('page.blog', 'page', '/blog', 'default.aspx?blog=true', 'the posts (the blog page when a page is the front page)', keys=['GET /blog*'])
op('page.page', 'page', '/page/{slug}', 'UrlRewrite /page/: page.aspx?id=', 'the page', keys=['GET /page/*'])
op('page.page-unknown', 'page', '/page/no-such-page', 'a slug no page has', 'the 404 page')
op('page.archive', 'page', '/archive', 'archive.aspx: the posts by category, the totals', 'the archive', keys=['GET /archive*'])
op('page.search', 'page', '/search?q=', 'search.aspx: the posts and pages that have the words', 'the results', inputs=['welcome', 'nothing-matches'], keys=['GET /search*'])
op('page.search-comments', 'page', '/search?q=&comment=true', 'search in the comments too (EnableCommentSearch)', 'the posts whose comments have the words')
op('page.search-paging', 'page', '/search?q=&page=N', 'the results\' pages (20 a page)', 'the page N')
op('page.search-apml', 'page', '/search?q=http://...', 'a URL as the search term: the server reads it as an APML document', 'a document that cannot be read: the search for the text',
   partial='a local URL that does not answer (127.0.0.1:9): no request to the Internet')
op('page.contact-page', 'page', '/contact', 'contact.aspx: the form (name, e-mail, subject, message), its validators, Send (btnSend: the mail, which fails as the SMTP server refuses)', "the form; the validators' messages; after Send the page's answer",
   keys=['GET /contact*', 'INPUT /contact *', 'SUBMIT /contact *', 'VALIDATOR /contact *'])
op('page.contact-prefill', 'page', '/contact?name=&email=&subject=', 'the form filled in from the query', 'the values in the fields')
op('page.front-page-example', 'page', '/FrontPageExample.aspx', 'the sample page', 'the page')
op('page.theme-query', 'page', '/?theme={name}', 'BlogSettings.Theme: ?theme= overrides the setting for that request', 'the page in the theme asked (Standard-2017, RazorHost)', inputs=['Standard-2017', 'RazorHost', 'NoSuchTheme'],
   keys=['GET /?theme'])
op('page.culture', 'page', 'any page', 'globalization culture="auto": the browser\'s Accept-Language (labels.*.resx)', 'the labels in the browser\'s language; the server\'s (ja-JP) without one', inputs=['de-DE', 'en-US', 'ja-JP', '(none)'])
op('page.crawler', 'page', '/category/, /?tag=, /?page=', 'a crawler\'s User-Agent (Browser.Crawler)', '301 to / (default.aspx\'s rule for crawlers)')
op('page.delete-post-query', 'page', '/?deletepost={guid}', 'BlogBasePage: a GET that deletes a post the user can delete', 'anonymous: nothing; the administrator: the post deleted', ADM)
op('page.delete-page-query', 'page', '/page/{slug}?deletepage={guid}', 'page.aspx: a GET that deletes a page', 'the page deleted (administrator)', ADM)
op('page.related-navigation', 'page', '/post/{slug}', 'the related posts and the next / previous post links', 'the links of the other posts')
op('page.external-links', 'page', 'every page', 'links to other sites (blogengine.io, the blog roll, social, share)', 'present as on the original (not followed)', keys=['EXTERNAL *'])

# --- comments (CommentList.ascx: ASP.NET client callback; CommentHandlers: moderation) ---
op('comment.form', 'comment', '/post/{slug}', 'CommentForm: name, e-mail, website, content, the simple captcha "5+5 ="', 'the form', keys=['INPUT /post/* *', 'SUBMIT /post/* *'])
op('comment.validation', 'comment', '/post/{slug}', 'BlogEngine.validateAndSubmitCommentForm (blog.js): required name, e-mail, content', 'the messages; nothing sent')
op('comment.captcha-wrong', 'comment', '/post/{slug}', 'SimpleCaptcha: a wrong answer', '"The captcha value you provided is incorrect."; nothing saved')
op('comment.post-anonymous', 'comment', '/post/{slug}', 'WebForm_DoCallback (CommentList.RaiseCallbackEvent): a visitor\'s comment', 'saved in App_Data/posts/{id}.xml, pending moderation (EnableCommentsModeration), the message for it',
   A, partial='the comment filters that ask the Internet (StopForumSpam, Akismet) disabled first (plugin.disable): ' + EXT)
op('comment.post-reply', 'comment', '/post/{slug}', 'a reply (BlogEngine.replyToComment: hiddenReplyTo)', 'the comment nested under its parent')
op('comment.post-authenticated', 'comment', '/post/{slug}', 'a comment of a logged-in user (TrustAuthenticatedUsers)', 'approved at once, shown', ADM)
op('comment.post-markup', 'comment', '/post/{slug}', 'markup in a comment (HTML-encoded; BBCode, Smilies extensions)', 'encoded, BBCode turned into HTML')
op('comment.cookie', 'comment', '/post/{slug}', 'the comment cookie (the name, e-mail, website for the next time)', 'the fields filled in on the next post')
op('comment.moderate-query', 'comment', '/post/{slug}?approvecomment=, ?deletecomment=, ?approveallcomments=, ?deletecommentandchildren=', 'moderation links of the post page (ModerateComments)', 'the comment approved / deleted',
   ADM, keys=['GET /post/*?approve*', 'GET /post/*?delete*'])
op('comment.moderate-query-anonymous', 'comment', '/post/{slug}?approvecomment=', 'the same without the right', 'nothing changes', A)
op('comment.unsubscribe', 'comment', '/post/{slug}?unsubscribe-email=', 'NotificationEmails: the address removed', 'the post saved without the address')
op('comment.rating', 'comment', '/rating.axd?id=&rating=', 'RatingHandler (BlogEngine.rate): a rating of a post (EnableRating); the rating cookie', 'OK and the post\'s rating saved; a second time NHASRATED; a bad value FAIL',
   inputs=['rating=4', 'again', 'rating=9', 'id=bad'], keys=['GET /rating.axd*'])
op('comment.notify-mail', 'comment', 'comment events', 'SendCommentMail and the notification of the post\'s subscribers', 'mails', runnable=False,
   reason='e-mail: the blog\'s SMTP settings are placeholders (smtp.gmail.com, a made-up account); the sending is tried and fails on both (the SMTP server is set to an address that refuses first, settings.email)')

# --- account pages (Account/*.aspx, account.master, Logout.cshtml) ---
op('acct.login-page', 'account', '/Account/login.aspx', 'GET: asp:Login (UserName, Password, RememberMe), the forgot-password link', 'the form', A,
   keys=['GET /account/login.aspx*', 'INPUT /account/login.aspx *', 'SUBMIT /account/login.aspx *'])
op('acct.login-required', 'account', '/Account/login.aspx', 'ValidateLogin (account.js): empty user name or password', 'the client\'s message; not posted', A)
op('acct.login-wrong', 'account', '/Account/login.aspx', 'Security.AuthenticateUser fails', 'the Login control\'s failure text', A)
op('acct.login', 'account', '/Account/login.aspx', 'LoginUser_OnAuthenticate: the cookie .AUXBLOGENGINE-{blog}', 'signed in, redirect to /', A)
op('acct.login-return-url', 'account', '/Account/login.aspx?ReturnURL=/admin/', 'the ReturnURL followed when it is local', 'redirect to /admin/', A)
op('acct.login-return-url-external', 'account', '/Account/login.aspx?ReturnURL=http://evil.example/', 'a ReturnURL to another site', 'not followed: /', A)
op('acct.login-remember-me', 'account', '/Account/login.aspx', 'RememberMe', 'a persistent cookie', A)
op('acct.logoff', 'account', '/Account/login.aspx?logoff', 'the Administration widget\'s and the admin\'s "log off" (a GET): SignOut, redirect to the Referer', 'signed out, back to the page', ADM)
op('acct.logout-cshtml', 'account', '/Account/Logout.cshtml?ReturnUrl=', 'Logout.cshtml: SignOut, RedirectLocal', 'signed out, redirect', ADM)
op('acct.register-disabled', 'account', '/Account/register.aspx', 'EnableSelfRegistration off (the default)', 'redirect to the login page', A)
op('acct.register-page', 'account', '/Account/register.aspx', 'CreateUserWizard (UserName, Email, Password, ConfirmPassword), self registration on (settings)', 'the form', A, pre='settings: EnableSelfRegistration on',
   keys=['GET /account/register.aspx*', 'INPUT /account/register.aspx *'])
op('acct.register-validation', 'account', '/Account/register.aspx', 'ValidateNewUser (account.js): missing fields, passwords that differ', 'the messages', A)
op('acct.register', 'account', '/Account/register.aspx', 'Membership.CreateUser (XmlMembershipProvider), the role SelfRegistrationInitialRole, signed in', 'users.xml, roles.xml, the profile; signed in', A,
   keys=['SUBMIT /account/register.aspx *'])
op('acct.register-duplicate', 'account', '/Account/register.aspx', 'a user name that exists', 'the wizard\'s message', A)
op('acct.password-retrieval-page', 'account', '/Account/password-retrieval.aspx', 'GET (EnablePasswordReset)', 'the form', A, keys=['GET /account/password-retrieval.aspx*', 'INPUT /account/password-retrieval.aspx *', 'SUBMIT /account/password-retrieval.aspx *'])
op('acct.password-retrieval-unknown', 'account', '/Account/password-retrieval.aspx', 'an unknown user', 'the message', A)
op('acct.password-retrieval-send', 'account', '/Account/password-retrieval.aspx', 'a user: a code mailed', 'the mail tried (fails: SMTP), the message', A, partial='e-mail: the sending fails as the SMTP server refuses')
op('acct.password-retrieval-code', 'account', '/Account/password-retrieval.aspx', 'the mailed 4-digit code entered: a new password mailed', 'the password reset', A, runnable=False,
   reason='e-mail: the code (Random) is only in the mail; the blog\'s SMTP settings are placeholders')
op('acct.change-password-page', 'account', '/Account/change-password.aspx', 'asp:ChangePassword', 'the form', ADM, keys=['GET /account/change-password.aspx*', 'INPUT /account/change-password.aspx *', 'SUBMIT /account/change-password.aspx *'])
op('acct.change-password-anonymous', 'account', '/Account/change-password.aspx', 'not signed in', 'redirect to the login page', A)
op('acct.change-password-validation', 'account', '/Account/change-password.aspx', 'ValidateChangePassword (account.js): empty, too short, not the same', 'the messages', ADM)
op('acct.change-password-wrong', 'account', '/Account/change-password.aspx', 'a wrong current password', 'the control\'s failure text', ADM)
op('acct.change-password', 'account', '/Account/change-password.aspx', 'ChangePassword: users.xml', 'redirect to change-password-success.aspx; the new password works', ADM)
op('acct.change-password-success', 'account', '/Account/change-password-success.aspx', 'GET', 'the page', ADM, keys=['GET /account/change-password-success.aspx*'])
op('acct.create-blog-disabled', 'account', '/Account/create-blog.aspx', 'CreateBlogOnSelfRegistration off', 'redirect to the login page', A)
op('acct.admin-anonymous', 'account', '/admin/', 'admin.master / layout.cshtml: AccessAdminPages', 'redirect to the login page with the ReturnURL', A, keys=['GET /admin*'])

# --- handlers (Web.Config) ---
op('h.rss', 'handler', '/syndication.axd', 'SyndicationHandler: the feed (RSS 2.0, PostsPerFeed)', 'the latest posts', keys=['GET /syndication.axd*'])
op('h.atom', 'handler', '/syndication.axd?format=atom', 'the Atom feed', 'the latest posts as Atom')
op('h.feed-category', 'handler', '/category/feed/{slug}, /syndication.axd?category=', 'a category\'s feed', 'its posts', keys=['GET /category/feed/*'])
op('h.feed-post', 'handler', '/post/feed/{slug}, /syndication.axd?post=', 'a post\'s comments as a feed', 'the comments', keys=['GET /post/feed/*'])
op('h.feed-tag-author', 'handler', '/syndication.axd?tag=, ?author=', 'the feeds of a tag, an author', 'their posts')
op('h.feed-comments', 'handler', '/syndication.axd?comments=true', 'the latest comments', 'the approved comments')
op('h.feed-search', 'handler', '/syndication.axd?q=', 'the search as a feed', 'the results')
op('h.sitemap', 'handler', '/sitemap.axd', 'SiteMap handler', 'the posts, pages, contact, blog as a sitemap', keys=['GET /sitemap.axd*'])
op('h.opensearch', 'handler', '/opensearch.axd', 'OpenSearch description', 'the search templates', keys=['GET /opensearch.axd*'])
op('h.rsd', 'handler', '/rsd.axd', 'Really Simple Discovery', 'the MetaWeblog endpoint', keys=['GET /rsd.axd*'])
op('h.opml', 'handler', '/opml.axd', 'the blog roll as OPML', 'the three entries', keys=['GET /opml.axd*'])
op('h.apml', 'handler', '/apml.axd', 'APML: the tags and categories', 'the document', keys=['GET /apml.axd*'])
op('h.sioc', 'handler', '/sioc.axd?sioc_type=', 'SIOC: site, post, comment, user', 'RDF documents', inputs=['(none)', 'post', 'user'], keys=['GET /sioc.axd*'])
op('h.foaf', 'handler', '/foaf.axd, /foaf_{user}.axd', 'FOAF of the first user / a user', 'RDF document', keys=['GET /foaf*'])
op('h.file', 'handler', '/file.axd?file=', 'FileHandler: a file of App_Data/files', 'the file; .. refused; a missing one 404', inputs=['/blogengine.png', '/../users.xml', '/missing.txt'], keys=['GET /file.axd*'])
op('h.image', 'handler', '/image.axd?picture=', 'ImageHandler: an image of App_Data/files', 'the image', inputs=['/logo.png', '/missing.png'], keys=['GET /image.axd*'])
op('h.image-rewrite', 'handler', '/images/{name}.jpgx, /files/{name}.axdx', 'UrlRewrite to image.axd / file.axd', 'the same file')
op('h.js-axd', 'handler', '/{name}.js.axd', 'JavaScriptHandler: the pages\' combined scripts', 'the script', keys=['GET /*.js.axd*'])
op('h.res-axd', 'handler', '/{lang}.res.axd, /admin.res.axd', 'ResourceHandler: BlogEngineRes (the labels in the language, the web roots)', 'the script', inputs=['en', 'ja', 'admin'], keys=['GET /*.res.axd*'])
op('h.blogml', 'handler', '/blogml.axd', 'BlogMLExportHandler: the whole blog (administrator)', 'anonymous 403; the administrator the BlogML document', keys=['GET /blogml.axd*'])
op('h.trackback', 'handler', '/trackback.axd', 'TrackbackHandler without a post / without a URL that links back', 'the error answers; nothing saved', partial='a source URL that does not answer (127.0.0.1:9): ' + EXT + ' otherwise',
   keys=['GET /trackback.axd*'])
op('h.pingback', 'handler', '/pingback.axd', 'PingbackHandler (XML-RPC pingback.ping) with a source that does not answer', 'the fault answer', partial='a local source URL that does not answer', keys=['GET /pingback.axd*'])
op('h.metaweblog-users-blogs', 'metaweblog', '/metaweblog.axd', 'blogger.getUsersBlogs / metaWeblog.getUsersBlogs', 'the blog', ADM)
op('h.metaweblog-auth', 'metaweblog', '/metaweblog.axd', 'a wrong password; a bad request', 'the faults', A)
op('h.metaweblog-categories', 'metaweblog', '/metaweblog.axd', 'metaWeblog.getCategories, wp.getTags, wp.getAuthors', 'the lists', ADM)
op('h.metaweblog-recent', 'metaweblog', '/metaweblog.axd', 'metaWeblog.getRecentPosts, metaWeblog.getPost', 'the posts', ADM)
op('h.metaweblog-new-post', 'metaweblog', '/metaweblog.axd', 'metaWeblog.newPost (and editPost)', 'App_Data/posts/{id}.xml; the post shown', ADM)
op('h.metaweblog-delete-post', 'metaweblog', '/metaweblog.axd', 'blogger.deletePost', 'the post deleted', ADM)
op('h.metaweblog-pages', 'metaweblog', '/metaweblog.axd', 'wp.newPage, wp.getPageList, wp.getPages, wp.getPage, wp.editPage, wp.deletePage', 'App_Data/pages', ADM)
op('h.metaweblog-media', 'metaweblog', '/metaweblog.axd', 'metaWeblog.newMediaObject', 'App_Data/files/{name}; its URL', ADM)
op('h.metaweblog-user-info', 'metaweblog', '/metaweblog.axd', 'blogger.getUserInfo (not implemented)', 'fault 10', ADM)
op('h.web-services', 'handler', '/setup/upgrade/Updater.asmx', 'the upgrade\'s web service (script services): its description; setup/Web.config denies anonymous', 'anonymous: the login page', A, keys=['GET /setup*'])
op('h.setup-upgrade', 'handler', '/setup/upgrade/', 'the upgrade page (AccessAdminPages): downloads a release', 'the page', ADM, runnable=False,
   reason='the upgrade downloads releases from dnbe.net and overwrites the site: ' + EXT + ' and it changes the application itself')

# --- static content, request filtering, errors ---
op('static.page-assets', 'static', 'every page', 'the theme\'s style sheets, scripts, fonts, images, the bundles', 'each one 200, the same bytes', keys=['GET /content/*', 'GET /scripts/*', 'GET /custom/*'])
op('static.files', 'static', '/Custom/Themes/..., /Content/..., /Scripts/..., /robots.txt, /wlwmanifest.xml, /favicon.ico', 'static files (staticContent clientCache, mimeMap)', '200, the same bytes and type, Cache-Control', keys=['GET /robots.txt', 'GET /wlwmanifest.xml'])
op('static.not-found', 'static', '/Content/missing.css', 'a missing static file', '404')
op('static.request-filtering', 'static', 'Web.Config, App_Data/users.xml, bin/, App_Code, *.cs, .master, .ascx, packages.config', 'files the server refuses', 'IIS 404.8 / ASP.NET 403 as the original')
op('err.not-found-aspx', 'error', '/nosuch.aspx', 'a missing .aspx (customErrors RemoteOnly, 404 -> error404.aspx, a local request)', 'the error page as the original')
op('err.not-found-extensionless', 'error', '/no/such/path', 'an extensionless URL with no page (the rewrite\'s default.aspx)', 'as the original')
op('err.error404-page', 'error', '/error404.aspx', 'error404.aspx: suggestions from aspxerrorpath / the Referer', 'the 404 page with the suggestions', inputs=['aspxerrorpath=/post/welcome'], keys=['GET /error404*'])
op('err.error-page', 'error', '/error.aspx', 'error.aspx (details for ViewDetailedErrorMessages)', 'the page', keys=['GET /error.aspx*'])
op('err.exception', 'error', '/post.aspx?id=bad', 'an exception in a page', 'the error page (customErrors RemoteOnly: details for a local request)')

# --- security ---
op('sec.viewstate-mac', 'security', '/Account/login.aspx', 'a tampered __VIEWSTATE (enableViewStateMac, the fixed machineKey)', '500 as the original')
op('sec.event-validation', 'security', '/Account/login.aspx', 'a tampered __EVENTVALIDATION', '500 as the original')
op('sec.request-validation', 'security', '/Account/login.aspx, /search?q=', 'markup posted / in the query (requestValidationMode 2.0; post.aspx ValidateRequest off)', 'HttpRequestValidationException as the original; post.aspx accepts it')
op('sec.invalid-path-chars', 'security', '/a*b', 'requestPathInvalidCharacters (<,>,*,\\)', '400 as the original')
op('sec.api-anonymous', 'security', '/api/*', 'the API without signing in (Security.IsAuthorizedTo, UnauthorizedAccessExceptionFilterAttribute)', '401, or what anonymous may read', A)
op('sec.api-editor', 'security', '/api/*', 'the API as an editor (rights.xml: Editors)', 'the editor\'s rights: 401 for the administrator\'s', ED)
op('sec.widget-edit-anonymous', 'security', '/Custom/Widgets/{name}/edit.cshtml', 'a widget\'s editor without signing in (no check but BlogRoll\'s)', 'as the original (the page and its save)', A)

# --- widgets (Custom/Widgets: WidgetZone, widget.cshtml / edit.cshtml) ---
op('widget.search', 'widget', 'sidebar', 'Search widget: BlogEngine.search -> /search?q=', 'the search page', keys=['SUBMIT * #searchbutton', 'INPUT * #txt-*'])
op('widget.newsletter', 'widget', 'sidebar', 'Newsletter widget: txtNewsletterEmail + "Notify me" (a POST of the page)', 'the address in App_Data/newsletter.xml, the thanks', keys=['INPUT * txtNewsletterEmail text', 'SUBMIT * btnSave'])
op('widget.newsletter-invalid', 'widget', 'sidebar', 'an address that is not one', 'the message; nothing saved')
op('widget.administration', 'widget', 'sidebar', 'Administration widget: the admin\'s links, log off', 'the links for the signed-in user', ADM)
op('widget.postlist-tagcloud-textbox', 'widget', 'sidebar', 'PostList, TagCloud, TextBox, BlogRoll (with its feeds: not compared, ' + EXT + '), CategoryList, MonthList, CommentList (Standard-2017)', 'their content')
op('widget.edit-page', 'widget', '/Custom/Widgets/{name}/edit.cshtml?id=&zone=', 'a widget\'s editor (the admin\'s widgets screen opens it in a frame)', 'the form', ADM, keys=['GET /custom/widgets/*'])
op('widget.edit-textbox', 'widget', '/Custom/Widgets/TextBox/edit.cshtml', 'TextBox: the HTML saved', 'App_Data/datastore/widgets/{id}.xml; the sidebar shows it', ADM)
op('widget.edit-settings', 'widget', '/Custom/Widgets/{PostList,TagCloud,CategoryList}/edit.cshtml', 'display settings saved', 'the widget\'s file; the sidebar', ADM)
op('widget.edit-linklist', 'widget', '/Custom/Widgets/LinkList/edit.cshtml', 'LinkList: a link added, ?delid= deleted', 'the widget\'s file', ADM)
op('widget.edit-newsletter', 'widget', '/Custom/Widgets/Newsletter/edit.cshtml', 'the subscribers; one removed (hdnEmail)', 'newsletter.xml', ADM)
op('widget.edit-blogroll', 'widget', '/Custom/Widgets/BlogRoll/edit.cshtml', 'the blog roll\'s entries (administrator)', 'blogroll.xml', ADM)

# --- the admin (admin/index.cshtml: AngularJS, hash routes; the Web API: AppCode/Api) ---
SCREENS = ['#/dashboard', '#/content/posts', '#/content/pages', '#/content/categories', '#/content/tags', '#/content/comments', '#/content/comments/filters',
           '#/content/blogs', '#/custom/plugins', '#/custom/themes', '#/custom/widgets', '#/security/profile', '#/security/users', '#/security/roles',
           '#/settings/basic', '#/settings/feed', '#/settings/email', '#/settings/comments', '#/settings/controls', '#/settings/advanced',
           '#/settings/controls/blogroll', '#/settings/controls/pings', '#/settings/tools']
op('admin.shell', 'admin', '/admin/', 'admin/index.cshtml: the shell, the menu by the user\'s rights (UserVars.Rights), admin.res.axd', 'the menu', ADM,
   keys=['GET /admin', 'GET /admin/index.cshtml', 'GET /admin/about.cshtml', 'API GET /admin/app/*', 'SUBMIT /admin* *'])
op('admin.screens', 'admin', '/admin/#/...', 'each screen of the menu: ' + ', '.join(SCREENS), 'each one with its data', ADM,
   # The screens' fields (AngularJS: what they change is each screen's API operation below).
   keys=['ROUTE /admin *', 'INPUT /admin *', 'INPUT /admin/index.cshtml *'])
op('admin.about', 'admin', '/admin/about.cshtml', 'the about page', 'the versions', ADM)
op('admin.dashboard', 'admin', '#/dashboard, GET /api/dashboard', 'DashboardVM: drafts, counts, the latest comments, trash, the log', 'the dashboard\'s numbers', ADM, keys=['API GET /api/dashboard*'])
op('admin.dashboard-quick-draft', 'admin', '#/dashboard', 'the quick draft (POST /api/posts, IsPublished false)', 'a draft post', ADM)
op('admin.dashboard-external', 'admin', '#/dashboard', 'the news feed (/api/newsfeed: blogengine.io), the gallery\'s packages (/api/packages), the version check (/api/setup: dnbe.net)', 'what the services answer', ADM,
   keys=['API GET /api/newsfeed*', 'API GET /api/setup*', 'API GET /api/packages?skip&take', 'API GET /api/packages?filter&order&skip&take'], partial='the requests are made and their answers compared (empty: the services no longer answer); ' + EXT)
# posts
op('api.posts-list', 'api', 'GET /api/posts?take=&skip=&filter=&order=', 'the posts (the admin\'s list; anonymous gets only its own: none)', 'PostItem[]', ADM, keys=['API GET /api/posts*'])
op('api.posts-get', 'api', 'GET /api/posts/{id}', 'a post\'s detail (also an unknown id: 404, a bad one: 500)', 'PostDetail', ADM)
op('api.posts-filter-order', 'api', 'GET /api/posts?filter=IsPublished == true&order=Title', 'System.Linq.Dynamic filter and order (a bad expression: 500)', 'the filtered list', ADM)
op('admin.post-editor', 'admin', '/admin/app/editor/editpost.cshtml?id=', 'the post editor: title, content (TinyMCE), slug, date, categories, tags, comments on, author', 'the post\'s fields', ADM,
   keys=['GET /admin/app/editor/editpost.cshtml*', 'INPUT /admin/app/editor/editpost.cshtml *', 'ROUTE /admin/app/editor/editpost.cshtml *'])
op('api.posts-create', 'api', 'POST /api/posts', 'a new post (the editor\'s save): slug made unique, a category made from a new title, tags', '201; App_Data/posts/{id}.xml; the post on the blog', ADM)
op('api.posts-create-draft', 'api', 'POST /api/posts', 'IsPublished false', 'not on the blog; anonymous: 404 page', ADM)
op('api.posts-update', 'api', 'PUT /api/posts/update/foo', 'the editor\'s save of a post', 'the post changed (title, content, slug)', ADM)
op('api.posts-process', 'api', 'PUT /api/posts/processchecked/{publish,unpublish,delete}', 'the list\'s buttons on the checked posts', 'published / unpublished / in the trash', ADM)
op('api.posts-delete', 'api', 'DELETE /api/posts/{id}', 'delete (soft: IsDeleted)', 'in the trash', ADM)
op('api.posts-bad-date', 'api', 'POST /api/posts', 'a DateCreated that is not yyyy-MM-dd HH:mm', '500', ADM)
# pages
op('api.pages-list', 'api', 'GET /api/pages', 'the pages', 'PageItem[]', ADM, keys=['API GET /api/pages*'])
op('api.pages-get', 'api', 'GET /api/pages/{id}', 'a page\'s detail', 'PageDetail', ADM)
op('admin.page-editor', 'admin', '/admin/app/editor/editpage.cshtml?id=', 'the page editor: title, content, slug, parent, front page, show in list, keywords, description, sort order', 'the page\'s fields', ADM,
   keys=['GET /admin/app/editor/editpage.cshtml*', 'INPUT /admin/app/editor/editpage.cshtml *'])
op('api.pages-create', 'api', 'POST /api/pages', 'a new page (with a parent, as the front page)', '201; App_Data/pages/{id}.xml; on the blog', ADM)
op('api.pages-update', 'api', 'PUT /api/pages/update/foo', 'a page saved', 'changed', ADM)
op('api.pages-process', 'api', 'PUT /api/pages/processchecked/{publish,unpublish,delete}', 'the list\'s buttons; a page with children cannot be deleted (500)', 'published / unpublished / in the trash', ADM)
op('api.pages-delete', 'api', 'DELETE /api/pages/{id}', 'delete', 'in the trash', ADM)
op('page.front-page', 'page', '/', 'a page marked IsFrontPage: the home page shows it, /blog the posts', 'the page as the home page')
# categories, tags
op('api.categories-list', 'api', 'GET /api/categories', 'the categories with their counts', 'CategoryItem[]', ADM, keys=['API GET /api/categories*'])
op('api.categories-create', 'api', 'POST /api/categories', 'a category (a parent); a duplicate title: 500', '201; categories.xml', ADM)
op('api.categories-update', 'api', 'PUT /api/categories/update/{id}', 'renamed', 'categories.xml; the category\'s page', ADM)
op('api.categories-delete', 'api', 'PUT /api/categories/processchecked/delete, DELETE /api/categories/{id}', 'deleted (from the posts too)', 'categories.xml', ADM)
op('api.tags-list', 'api', 'GET /api/tags?take=&skip=&postId=', 'the tags with their counts', 'TagItem[]', ADM, keys=['API GET /api/tags*'])
op('api.tags-update', 'api', 'PUT /api/tags/update/{tag}', 'a tag renamed in every post', 'the posts\' files', ADM)
op('api.tags-delete', 'api', 'DELETE /api/tags/{tag}, PUT /api/tags/processchecked/delete', 'a tag removed from every post', 'the posts\' files', ADM)
# comments
op('api.comments-list', 'api', 'GET /api/comments', 'every comment (the admin\'s comments screen: pending, approved, spam)', 'CommentsVM', ADM, keys=['API GET /api/comments*'])
op('api.comments-get', 'api', 'GET /api/comments/{id}', 'a comment\'s detail (the edit dialog)', 'CommentDetail', ADM)
op('api.comments-reply', 'api', 'POST /api/comments', 'the admin\'s reply', '201; the reply in the post\'s file', ADM)
op('api.comments-process', 'api', 'PUT /api/comments/processchecked/{approve,unapprove,delete}', 'moderation of the checked comments', 'the flags in the post\'s file', ADM)
op('api.comments-delete-all', 'api', 'PUT /api/comments/deleteall/{pending,spam}', 'all pending / spam comments deleted', 'in the trash', ADM)
op('api.comments-delete', 'api', 'DELETE /api/comments/{id}', 'a comment deleted', 'in the trash', ADM)
op('api.comment-filters', 'api', 'GET/POST /api/commentfilter, PUT .../processchecked/delete, .../deleteall/foo', 'the comment filters (MetaExtension.xml)', 'the list; added; deleted', ADM, keys=['API GET /api/commentfilter*'])
# trash, logs
op('api.trash', 'api', 'GET /api/trash, PUT /api/trash/{restore,purge,purgeall}/...', 'the trash: restore a post, purge one, purge all', 'the files restored / removed', ADM, keys=['API GET /api/trash*'])
op('api.logs', 'api', 'GET /api/logs/getlog/file, PUT /api/logs/purgelog/file', 'the log (App_Data/logger.txt)', 'its lines; removed', ADM)
# users, roles, profile
op('api.users-list', 'api', 'GET /api/users?filter=1 == 1&order=UserName', 'the users (profiles made for users without one)', 'BlogUser[]', ADM, keys=['API GET /api/users*'])
op('api.users-create', 'api', 'POST /api/users', 'a new user with a role (the users screen): an editor', 'users.xml, roles.xml, profiles/{user}.xml', ADM)
op('api.users-create-invalid', 'api', 'POST /api/users', 'a user without a password / an existing name', '500 / 304', ADM)
op('api.users-update', 'api', 'PUT /api/users/update/item', 'e-mail, password, roles changed', 'users.xml, roles.xml', ADM)
op('api.users-delete', 'api', 'DELETE /api/users/{name}, PUT /api/users/processchecked/delete', 'a user deleted (the last administrator refused)', 'users.xml, the profile removed', ADM)
op('api.profile', 'api', 'GET /api/users/{name}, PUT /api/users/saveprofile/item', 'the profile screen: display name, names, about, private', 'profiles/{user}.xml', ADM, keys=['ROUTE /admin #/security/profile'])
op('api.roles', 'api', 'GET /api/roles, POST /api/roles, DELETE /api/roles/{name}, PUT .../processchecked/delete', 'the roles (a system role cannot be deleted)', 'roles.xml, rights.xml', ADM, keys=['API GET /api/roles*'])
op('api.roles-rights', 'api', 'GET /api/roles/getrights/{role}, PUT /api/roles/saverights/{role}, GET /api/roles/getuserroles/{user}', 'a role\'s rights', 'rights.xml', ADM)
# settings, lookups
op('api.settings-get', 'api', 'GET /api/settings', 'the settings screens\' data (basic, feed, email, comments, controls, advanced)', 'SettingsVM', ADM, keys=['API GET /api/settings*'])
op('api.settings-save', 'api', 'PUT /api/settings', 'a settings screen saved: the blog\'s name and description, posts a page, comments, feed, e-mail (the SMTP server), self registration, culture', 'settings.xml; the blog shows them', ADM)
op('api.settings-test-email', 'api', 'PUT /api/settings?action=testEmail', 'the e-mail settings\' test', 'the error text (the SMTP server refuses)', ADM, partial='e-mail: tried and failed (no SMTP server)')
op('api.settings-clear-cache', 'api', 'PUT /api/settings?action=clearCache', 'the cache cleared', '200', ADM)
op('api.lookups', 'api', 'GET /api/lookups, PUT /api/lookups/update/foo', 'the editors\' lists (cultures, authors, pages, categories, themes); the editor options', 'the lookups; settings.xml', ADM, keys=['API GET /api/lookups*'])
op('api.custom-fields', 'api', 'GET/POST/PUT/DELETE /api/customfields', 'custom fields of a post, a page, a profile, a theme', 'customfields.xml', ADM, keys=['API GET /api/customfields*'])
op('api.blogroll', 'api', 'GET /api/blogroll, POST, PUT .../update/item, GET /api/blogroll/{id}', 'the blog roll screen (its edit-load and delete calls are broken in the original: 404)', 'blogroll.xml', ADM, keys=['API GET /api/blogroll*'])
op('api.ping-services', 'api', 'GET/POST /api/pingservices, PUT .../processchecked/delete', 'the ping services (removed first: their pings go to the Internet)', 'pingservices.xml', ADM, keys=['API GET /api/pingservices*'])
op('api.tools', 'api', 'GET /api/tools/{check1,trust,data,root,Custom}', 'the tools screen\'s checks (the identity, the trust level, the write permissions)', 'the answers', ADM, keys=['API GET /api/tools*'])
op('api.file-manager', 'api', 'GET /api/filemanager, PUT .../addfolder/add, PUT .../processchecked/delete', 'the file manager (App_Data/files): list, a folder made, deleted', 'the folders', ADM,
   keys=['API GET /api/filemanager*', 'GET /admin/app/editor/filemanager.cshtml*'])
op('api.upload', 'api', 'POST /api/upload?action={image,file,filemgr,profile}', 'an upload (the editor\'s image and file buttons, the file manager, the profile\'s photo)', '201 with the URL; App_Data/files/...; served by image.axd / file.axd', ADM)
op('api.upload-import', 'api', 'POST /api/upload?action=import', 'a BlogML import (advanced settings)', 'the posts of the document', ADM)
op('api.upload-unauthorized', 'api', 'POST /api/upload', 'without the right', 'redirect to the login page', A)
# widgets, themes, plugins, packages
op('api.widgets-get', 'api', 'GET /api/widgets', 'the widgets screen: the zones (the server asks its own pages for them) and the widgets available', 'WidgetsVM', ADM, keys=['API GET /api/widgets*'])
op('api.widgets-update', 'api', 'PUT /api/widgets/update/item', 'the zones saved (a widget added, moved, removed)', 'datastore/widgets/{zone}.xml; the sidebar', ADM)
op('api.themes', 'api', 'GET /api/packages?filter=themes, PUT /api/packages/settheme/{id}', 'the themes screen: the installed themes, one made the blog\'s', 'settings.xml; the blog in the theme', ADM)
op('api.plugins', 'api', 'GET /api/packages?filter=extensions, PUT /api/packages/processchecked/{enable,disable}, /admin/Extensions/Settings.aspx?ext=', 'the plugins screen: enable, disable, an extension\'s settings page', 'datastore/extensions/{name}.xml', ADM,
   keys=['GET /admin/extensions/settings.aspx*'])
op('api.packages-gallery', 'api', 'GET /api/packages?filter=all, GET /api/packages/{id}', 'the gallery screens of plugins, themes, widgets', 'the gallery\'s list', ADM, partial='the requests are made (the gallery at dnbe.net no longer answers: an empty list on both); ' + EXT)
op('api.packages-install', 'api', 'PUT /api/packages/install/{id}, uninstall, rate, /api/packageextra', 'install a package from the gallery, rate it', 'files of the package in the site', ADM, runnable=False,
   reason='the package gallery (dnbe.net) is on the Internet (and no longer answers): there is nothing to install')
op('api.blogs', 'api', 'GET /api/blogs, POST /api/blogs, PUT .../update/item, DELETE', 'the blogs screen (several blogs: UsageScenario singleblog)', 'blogs.xml, App_Data/blogs/{name}', ADM, keys=['API GET /api/blogs*'])
op('api.stats-pager', 'api', 'GET /api/stats, GET /api/pager', 'calls the admin does not make', 'the answers', ADM)

# --- state ---
op('state.cookies-anonymous', 'state', 'any page', 'the cookies of a visit', 'none of the application\'s but the rating / comment ones after those operations')
op('state.cookies-authenticated', 'state', 'after login', 'the authentication cookie .AUXBLOGENGINE-{blog id}', 'HttpOnly; session or persistent (RememberMe)')
op('state.app-data-initial', 'state', 'App_Data', 'what the application\'s start writes (the extensions\' settings: datastore/extensions)', 'the same files and contents')

here = os.path.dirname(os.path.abspath(__file__))
ids = [o['id'] for o in OPS]
assert len(ids) == len(set(ids)), 'duplicate ids'
with open(os.path.join(here, 'be.operations.json'), 'w', encoding='utf-8', newline='\n') as f:
    json.dump({'_comment': 'Generated by be-operations-inventory.py (edit the table there). The operations of BlogEngine.NET; corpora/regression/be.operations.scenario.json exercises them (its steps\' op).',
               'operations': OPS}, f, ensure_ascii=False, indent=2)
    f.write('\n')

cats = {}
for o in OPS:
    cats.setdefault(o['category'], []).append(o)
lines = ['# BlogEngine.NET: the inventory of operations', '',
         'Generated by `be-operations-inventory.py` from the same table as `be.operations.json` (edit the table there and run it).',
         'Each operation is exercised by the steps of `be.operations.scenario.json` that name it (`op`); the golden data is recorded on the',
         'original on IIS (`corpora/record-be-operations.ps1`) and compared on Windows and Linux (`verify-windows.ps1 -Name be -Suite operations`,',
         '`run-linux.ps1 ... -Suite operations`). Every run starts from the release\'s App_Data (`corpora/be-app-data.ps1`).', '',
         '| category | operations | not runnable | partial |', '|---|---|---|---|']
for c, items in cats.items():
    lines.append(f'| {c} | {len(items)} | {sum(1 for i in items if not i["runnable"])} | {sum(1 for i in items if i.get("partial"))} |')
lines.append(f'| **total** | **{len(OPS)}** | **{sum(1 for i in OPS if not i["runnable"])}** | **{sum(1 for i in OPS if i.get("partial"))}** |')
for c, items in cats.items():
    lines += ['', f'## {c}', '', '| id | page | element | role / preconditions | inputs | expected |', '|---|---|---|---|---|---|']
    for o in items:
        role = o.get('role', '') + ('; ' + o['preconditions'] if o.get('preconditions') else '')
        exp = o.get('expected', '')
        if not o['runnable']:
            exp = f'**not runnable**: {o["reason"]}'
        elif o.get('partial'):
            exp += f' (partial: {o["partial"]})'
        cells = [o['id'], o['page'], o['element'], role, ', '.join(o.get('inputs', [])), exp]
        lines.append('| ' + ' | '.join(str(x).replace('|', '\\|') for x in cells) + ' |')
with open(os.path.join(here, 'be.operations.md'), 'w', encoding='utf-8', newline='\n') as f:
    f.write('\n'.join(lines) + '\n')
print(len(OPS), 'operations')
