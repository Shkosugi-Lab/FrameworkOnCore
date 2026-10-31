# NavigationProbe

A .NET Framework 4.8 Web Forms site (C#, a few Visual Basic pages compiled by the server) full of page navigation that is likely to break on a port of System.Web:
Server.Transfer / Execute, cross-page postbacks, Response.Redirect in every event and with every kind of URL, redirects of Global.asax and a module, customErrors
and httpErrors, forms authentication, postback edge cases, Wizard / MultiView, ASP.NET AJAX, routes and URL rewriting, navigation controls, async pages,
handlers, downloads, windows and frames. Self-contained: no database, no external service (the browser's requests to another origin are stubbed by the
test; the target of a redirect to external.example is not seen by it: .example names do not resolve, the browser's own error page is compared, its text).

Every page shows where it thinks it is (`Code/Probe.cs`: the `Info` control - the page, Path / RawUrl / the executing path / PathInfo, the postback,
PreviousPage, the posted form, the query, the user, the session's side effects of the previous request, the event log of every page of the request).

- `inventory.py` writes `operations.json` / `operations.md` (the inventory), `parity-scenario.json` (the steps), `Default.aspx` (the index) and checks the
  inventory against the source.
- Record on IIS: `.\experiments\wf4c\sample-parity.ps1 -Name NavigationProbe -Record` (an administrator's shell; a throwaway site): `golden-webforms.json`.
- Compare: `.\experiments\wf4c\sample-parity.ps1 -Name NavigationProbe -Windows` / `-Linux` (Docker). Per-operation results: `results.<platform>.md` / `.json`
  (same / allowed / different).
- Documented differences: `allowed.json` (each with its reason).
- What a user can reach that the inventory does not name: ParityTest discover on the original (a site of the sample on IIS), `discovered.iis.json`:
  `dotnet tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll discover --url <site> --out samples\NavigationProbe\discovered.iis.json
  --inventory samples\NavigationProbe\operations.json --login-url /F/Login.aspx --persona anonymous --persona login:user:pass1 --persona login:admin:pass2`
  (nothing left: 663 pages, 309 keys).
- The pages are the sample's own files (written once by a generator during its making; edited since): change them here, then run `python inventory.py`.

## The cases (226 operations)

### index (1)

- **index.home** (/ (Default.aspx)): the index of every case. *Why:* nothing: the starting point (and what discover crawls).

### transfer (22)

- **transfer.preserve-form** (/T/Source.aspx -> ~/T/Sub/Target.aspx): Server.Transfer(path, true) from a click. *Why:* the target renders under the source's URL: ~ / relative URLs, the form's action, ResolveClientUrl and PreviousPage all depend on the executing vs the requested path (Context.Handler / PreviousHandler).
- **transfer.no-form** (/T/Source.aspx): Server.Transfer(path, false). *Why:* preserveForm false clears Request.Form for the target only.
- **transfer.query** (/T/Source.aspx): Server.Transfer("...?q=from-transfer&q=again", true). *Why:* the query string replaced by the transfer's while RawUrl stays the browser's.
- **transfer.relative-links** (/T/Sub/Target.aspx after a transfer): HyperLink NavigateUrl="Other.aspx", a href="Other.aspx" (plain and runat=server). *Why:* the classic Transfer breakage: server-resolved URLs and browser-resolved URLs disagree.
- **transfer.target-postback** (/T/Sub/Target.aspx after a transfer): a postback from the transferred-to page. *Why:* the form action after Transfer: a relative "Target.aspx"-style action resolved by the browser against the old URL.
- **transfer.handler** (/T/Source.aspx): Server.Transfer(IHttpHandler, true). *Why:* a handler object instead of a path: no page, no PreviousPage.
- **transfer.ashx** (/T/Source.aspx): Server.Transfer("~/T/Echo.ashx?via=transfer"). *Why:* Transfer to a non-page handler path (the handler factory of .ashx).
- **transfer.chain** (/T/Chain1.aspx -> Chain2 -> Sub/Target): nested Server.Transfer from Page_Load. *Why:* each transfer nests a child request; the log and PreviousPage show the order.
- **transfer.after-write** (/T/Source.aspx): Response.Write then Server.Transfer. *Why:* Transfer does not clear the response.
- **transfer.catch-threadabort** (/T/Source.aspx): Server.Transfer inside try / catch (ThreadAbortException). *Why:* .NET has no thread abort: code after a swallowed Response.End / Transfer runs unless the port re-raises it.
- **transfer.catch-exception** (/T/Source.aspx): Server.Transfer inside try / catch (Exception). *Why:* the exception type a broad catch sees (ThreadAbortException).
- **transfer.transfer-request** (/T/Source.aspx): Server.TransferRequest("~/T/Sub/Target.aspx?tr=1", true). *Why:* IIS integrated mode's child request (the whole pipeline again, Global.asax events too).
- **transfer.missing** (/T/Source.aspx): Server.Transfer to a page that does not exist. *Why:* the error of a child request.
- **execute.load** (/T/Exec.aspx?mode=load): Server.Execute (load). *Why:* Execute writes straight into the response, at the time it is called.
- **execute.render** (/T/Exec.aspx?mode=render): Server.Execute (render). *Why:* Execute from inline code, in the middle of the page's output.
- **execute.writer** (/T/Exec.aspx?mode=writer): Server.Execute (writer). *Why:* Execute(path, TextWriter): another writer for the child page.
- **execute.ashx** (/T/Exec.aspx?mode=ashx): Server.Execute (ashx). *Why:* Execute of a handler path.
- **execute.redirect** (/T/Exec.aspx?mode=redirect): Server.Execute (redirect). *Why:* Response.Redirect inside a child request (who ends?).
- **execute.transfer** (/T/Exec.aspx?mode=transfer): Server.Execute (transfer). *Why:* Transfer inside Execute.
- **execute.throw** (/T/Exec.aspx?mode=throw): Server.Execute (throw). *Why:* the wrapping of a child request's exception.
- **execute.missing** (/T/Exec.aspx?mode=missing): Server.Execute (missing). *Why:* Execute of a page that does not exist.
- **execute.preserve-form** (/T/Exec.aspx): Server.Execute(path, writer, preserveForm true / false) on a postback. *Why:* preserveForm on Execute.

### cross-page (10)

- **cross.same-folder** (/X/Source.aspx -> /X/Target.aspx): Button PostBackUrl; the target's PreviousPageType. *Why:* the source page is re-created (its LoadComplete) inside the target's request; PreviousPageType makes a typed property in generated code.
- **cross.validation** (/X/Source.aspx -> /X/Target.aspx): cross-page postback with an empty required field (server validation only). *Why:* validation runs on the source page when the target asks PreviousPage.IsValid.
- **cross.no-validation** (/X/Source.aspx): CausesValidation=false with PostBackUrl. *Why:* Page.IsValid before Validate().
- **cross.other-folder** (/X/Source.aspx -> /X/Other/Target.aspx): PostBackUrl to a page in another folder (no PreviousPageType). *Why:* the browser's URL is the target's, unlike Transfer.
- **cross.query** (/X/Source.aspx): PostBackUrl with a query. *Why:* WebForm_DoPostBackWithOptions sets the form's action with the query.
- **cross.linkbutton** (/X/Source.aspx): LinkButton PostBackUrl. *Why:* __doPostBack with options and __PREVIOUSPAGE.
- **cross.target-postback** (/X/Target.aspx): a normal postback on the target after a cross-page postback. *Why:* the target's own view state after a cross-page arrival.
- **cross.target-direct** (/X/Target.aspx): GET of the target. *Why:* the typed PreviousPage null.
- **cross.master** (/X/MasterSource.aspx (master A) -> /X/MasterTarget.aspx (master B)): PostBackUrl between pages of different masters. *Why:* naming containers of the content placeholder; masters in another folder.
- **cross.plain-postback** (/X/Source.aspx): a button without PostBackUrl on the source. *Why:* the same page without cross-page.

### redirect (48)

- **redirect.at-preinit** (/R/Life.aspx?at=PreInit): Response.Redirect in PreInit (endResponse true, then false). *Why:* each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone).
- **redirect.at-init** (/R/Life.aspx?at=Init): Response.Redirect in Init (endResponse true, then false). *Why:* each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone).
- **redirect.at-initcomplete** (/R/Life.aspx?at=InitComplete): Response.Redirect in InitComplete (endResponse true, then false). *Why:* each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone).
- **redirect.at-preload** (/R/Life.aspx?at=PreLoad): Response.Redirect in PreLoad (endResponse true, then false). *Why:* each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone).
- **redirect.at-load** (/R/Life.aspx?at=Load): Response.Redirect in Load (endResponse true, then false). *Why:* each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone).
- **redirect.at-click** (/R/Life.aspx?at=Click): Response.Redirect in Click (endResponse true, then false). *Why:* each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone).
- **redirect.at-loadcomplete** (/R/Life.aspx?at=LoadComplete): Response.Redirect in LoadComplete (endResponse true, then false). *Why:* each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone).
- **redirect.at-prerender** (/R/Life.aspx?at=PreRender): Response.Redirect in PreRender (endResponse true, then false). *Why:* each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone).
- **redirect.at-prerendercomplete** (/R/Life.aspx?at=PreRenderComplete): Response.Redirect in PreRenderComplete (endResponse true, then false). *Why:* each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone).
- **redirect.at-savestatecomplete** (/R/Life.aspx?at=SaveStateComplete): Response.Redirect in SaveStateComplete (endResponse true, then false). *Why:* each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone).
- **redirect.at-render** (/R/Life.aspx?at=Render): Response.Redirect in Render (endResponse true, then false). *Why:* each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone).
- **redirect.at-unload** (/R/Life.aspx?at=Unload): Response.Redirect in Unload (endResponse true, then false). *Why:* each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone).
- **redirect.tae** (/R/Cases.aspx?c=tae): Redirect in try / catch (ThreadAbortException): the code after the try. *Why:* thread abort emulation.
- **redirect.ex** (/R/Cases.aspx?c=ex): Redirect in try / catch (Exception). *Why:* a broad catch swallowing the end of the response.
- **redirect.finally** (/R/Cases.aspx?c=finally): Redirect in try / finally. *Why:* finally blocks during the abort.
- **redirect.written** (/R/Cases.aspx?c=written): Response.Write then Redirect. *Why:* a buffered response is cleared.
- **redirect.flushed** (/R/Cases.aspx?c=flushed): Response.Write, Flush, then Redirect. *Why:* headers sent: the redirect fails mid-page.
- **redirect.permanent** (/R/Cases.aspx?c=permanent): RedirectPermanent. *Why:* the permanent redirect's status.
- **redirect.permanent-false** (/R/Cases.aspx?c=permanent-false): RedirectPermanent(url, false). *Why:* endResponse false on the permanent redirect.
- **redirect.route** (/R/Cases.aspx?c=route): RedirectToRoute("NavRoute", id=3). *Why:* the route's URL from values.
- **redirect.route-permanent** (/R/Cases.aspx?c=route-permanent): RedirectToRoutePermanent with an extra value (space). *Why:* the extra value as a query, its encoding.
- **redirect.route-name** (/R/Cases.aspx?c=route-name): RedirectToRoute("FilesRoute", rest="a/b c"). *Why:* catch-all parameters' encoding.
- **redirect.external** (/R/Cases.aspx?c=external): Redirect to an absolute external URL (a space in the query). *Why:* absolute URLs left as they are.
- **redirect.protocol-relative** (/R/Cases.aspx?c=protocol-relative): Redirect to //external.example/relative. *Why:* a URL that looks relative but leaves the site.
- **redirect.absolute-path** (/R/Cases.aspx?c=absolute-path): Redirect to /R/Landing.aspx. *Why:* an absolute path.
- **redirect.unicode** (/R/Cases.aspx?c=unicode): Redirect with Unicode, a space, % and & in the query. *Why:* UrlEncodeRedirect / UrlPathEncode of the query.
- **redirect.encoded** (/R/Cases.aspx?c=encoded): Redirect with an already escaped query (%26 %E6 %2B). *Why:* double encoding.
- **redirect.unicode-path** (/R/Cases.aspx?c=unicode-path): Redirect to ~/R/着地.aspx. *Why:* non-ASCII file names (Linux file system, the escaping).
- **redirect.space-path** (/R/Cases.aspx?c=space-path): Redirect to ~/R/With Space.aspx?from=space path. *Why:* spaces in paths.
- **redirect.newline** (/R/Cases.aspx?c=newline): Redirect to a URL with CR LF (header injection). *Why:* response splitting protection.
- **redirect.empty** (/R/Cases.aspx?c=empty): Redirect(""). *Why:* the edge of URL resolution.
- **redirect.end** (/R/Cases.aspx?c=end): Response.Write, Response.End, Write. *Why:* Response.End without a redirect.
- **redirect.complete** (/R/Cases.aspx?c=complete): Write, CompleteRequest, the page goes on. *Why:* CompleteRequest is not End.
- **redirect.false-complete** (/R/Cases.aspx?c=false-complete): Redirect(url, false) + CompleteRequest (the recommended pattern). *Why:* the documented alternative to Redirect(url) still renders the page.
- **redirect.status200** (/R/Cases.aspx?c=status200): Redirect(url, false), then StatusCode = 200 in PreRender. *Why:* a status overridden after the redirect.
- **redirect.status301** (/R/Cases.aspx?c=status301): Redirect(url, false), then StatusCode = 301. *Why:* the status changed, the Location kept.
- **redirect.status404** (/R/Cases.aspx?c=status404): Redirect(url, false), then StatusCode = 404 (TrySkipIisCustomErrors). *Why:* a redirect turned into an error.
- **redirect.twice** (/R/Cases.aspx?c=twice): Redirect(a, false) then Redirect(b). *Why:* the Location replaced.
- **redirect.clear-headers** (/R/Cases.aspx?c=clear-headers): Redirect(url, false) then ClearHeaders. *Why:* the redirect's headers removed.
- **redirect.relative-parent** (/R/Sub/Rel.aspx?c=parent): Response.Redirect("../Landing.aspx") from R/Sub. *Why:* relative URL resolution of Redirect (against the request's folder), fragments.
- **redirect.relative-file** (/R/Sub/Rel.aspx?c=file): Response.Redirect("Landing.aspx") from R/Sub. *Why:* relative URL resolution of Redirect (against the request's folder), fragments.
- **redirect.relative-dot** (/R/Sub/Rel.aspx?c=dot): Response.Redirect("./Landing.aspx") from R/Sub. *Why:* relative URL resolution of Redirect (against the request's folder), fragments.
- **redirect.relative-tilde** (/R/Sub/Rel.aspx?c=tilde): Response.Redirect("~/R/Landing.aspx") from R/Sub. *Why:* relative URL resolution of Redirect (against the request's folder), fragments.
- **redirect.relative-query** (/R/Sub/Rel.aspx?c=query): Response.Redirect("?from=query-only") from R/Sub. *Why:* relative URL resolution of Redirect (against the request's folder), fragments.
- **redirect.relative-frag** (/R/Sub/Rel.aspx?c=frag): Response.Redirect("#frag") from R/Sub. *Why:* relative URL resolution of Redirect (against the request's folder), fragments.
- **redirect.relative-query-frag** (/R/Sub/Rel.aspx?c=query-frag): Response.Redirect("Landing.aspx?from=query-frag#section") from R/Sub. *Why:* relative URL resolution of Redirect (against the request's folder), fragments.
- **redirect.relative-tilde-frag** (/R/Sub/Rel.aspx?c=tilde-frag): Response.Redirect("~/R/Landing.aspx#top") from R/Sub. *Why:* relative URL resolution of Redirect (against the request's folder), fragments.
- **redirect.relative-parent-parent** (/R/Sub/Rel.aspx?c=parent-parent): Response.Redirect("../../Default.aspx") from R/Sub. *Why:* relative URL resolution of Redirect (against the request's folder), fragments.

### global (13)

- **global.begin-request** (/G/Begin.aspx): Response.Redirect in Application_BeginRequest. *Why:* the earliest event: no handler, no session.
- **global.authenticate-request** (/G/Auth.aspx): Response.Redirect in Application_AuthenticateRequest. *Why:* during authentication.
- **global.post-acquire-state** (/G/Acquire.aspx): Response.Redirect in Application_PostAcquireRequestState (with the session). *Why:* the session's event.
- **global.end-request** (/G/End.aspx): Response.Redirect in Application_EndRequest after the page ran. *Why:* a redirect at the very end (headers not sent yet).
- **global.module-redirect** (/G/Module.aspx): an IHttpModule's Response.Redirect in PostAuthorizeRequest. *Why:* a module of web.config's system.webServer/modules.
- **global.module-complete** (/G/ModuleEnd.aspx): a module answers and calls CompleteRequest. *Why:* CompleteRequest from a module.
- **global.error-redirect** (/G/ErrRedirect.aspx): Application_Error: ClearError + Redirect. *Why:* error handling by redirect.
- **global.error-redirect-kept** (/G/ErrRedirectKeep.aspx): Application_Error: Redirect without ClearError. *Why:* the error still pending when redirecting.
- **global.error-transfer** (/G/ErrTransfer.aspx): Application_Error: ClearError + Server.Transfer. *Why:* Transfer outside the page's execution.
- **global.error-execute** (/G/ErrExecute.aspx): Application_Error: ClearError + Clear + Server.Execute. *Why:* Execute from an application event.
- **global.error-clear** (/G/ErrClear.aspx): Application_Error: ClearError + Write. *Why:* swallowing an error.
- **global.redirect-loop** (/G/LoopA.aspx <-> /G/LoopB.aspx): a redirect loop. *Why:* relative Location values ("LoopB.aspx") resolved hop after hop.
- **global.redirect-chain** (/G/Chain.aspx?n=19 / ?n=25): a chain of 19 redirects (the browser follows) and of 25 (it does not). *Why:* the browser's redirect limit (20).

### error (20)

- **error.redirect-mode** (/E/Redirect/Throw.aspx): customErrors On, ResponseRedirect: an exception. *Why:* the error page by redirect.
- **error.redirect-404** (/E/Redirect/Http404.aspx): HttpException(404) under ResponseRedirect with an <error statusCode="404">. *Why:* per-status pages.
- **error.redirect-403** (/E/Redirect/Http403.aspx): HttpException(403) without its own page. *Why:* the status without an entry.
- **error.redirect-missing** (/E/Redirect/Missing.aspx): a missing .aspx under customErrors On. *Why:* the page handler factory's 404.
- **error.rewrite-mode** (/E/Rewrite/Throw.aspx): customErrors ResponseRewrite: an exception. *Why:* Server.Execute of the error page.
- **error.rewrite-404** (/E/Rewrite/Http404.aspx): HttpException(404) under ResponseRewrite. *Why:* the per-status page rewritten.
- **error.rewrite-missing** (/E/Rewrite/Missing.aspx): a missing page under ResponseRewrite. *Why:* rewrite for the 404 of the factory.
- **error.error-page-throws** (/E/Broken/Throw.aspx): the error page itself throws (ResponseRewrite). *Why:* an error inside the error page.
- **error.no-default** (/E/Plain/Throw.aspx): customErrors On without defaultRedirect. *Why:* what a remote user sees.
- **error.detailed** (/E/Throw.aspx): customErrors Off: an exception. *Why:* the yellow screen.
- **error.httpexception-404** (/E/Http404.aspx): HttpException(404) with customErrors Off. *Why:* HttpException's status.
- **error.missing-page** (/E/NoSuchPage.aspx): a missing .aspx. *Why:* the resource cannot be found.
- **error.compile** (/E/Compile.aspx): a page whose inline code does not compile. *Why:* the server's compiler and its error page.
- **error.parser** (/E/Parser.aspx): a page with an unknown control. *Why:* the parser's error page.
- **error.bad-directive** (/E/BadDirective.aspx): a page inheriting a class that does not exist. *Why:* a broken Inherits.
- **error.httperrors-replaced** (/E/Http/Status404.aspx): httpErrors Custom / existingResponse Auto: the page sets 404 without TrySkipIisCustomErrors. *Why:* IIS's error pages over the application's answer.
- **error.httperrors-skip** (/E/Http/Skip404.aspx): the same with TrySkipIisCustomErrors. *Why:* TrySkipIisCustomErrors under Auto.
- **error.httperrors-missing** (/E/Http/NoSuch.aspx): a missing page under httpErrors Custom. *Why:* which layer answers the 404.
- **error.async** (/E/Redirect/Async.aspx): an async postback that throws under customErrors On. *Why:* customErrors and the page request manager.
- **error.asyncnoredirect** (/E/Redirect/AsyncNoRedirect.aspx): an async postback that throws under customErrors On. *Why:* customErrors and the page request manager.

### forms (16)

- **forms.protected-redirect** (/F/Secure/Page.aspx (anonymous)): a page of a folder denied to anonymous users. *Why:* the authorization module's 401 turned into the login redirect (ReturnUrl's encoding).
- **forms.return-url-query-unicode** (/F/Secure/Page.aspx?a=1&b=日本&c=x%26y): the ReturnUrl of a URL with a query, Unicode and an escaped &. *Why:* nested encoding of the ReturnUrl.
- **forms.secure-route** (/secure/5 (anonymous)): a route to a page of the protected folder. *Why:* the gap between URL authorization and routes.
- **forms.public-page** (/F/Secure/Public.aspx): a page allowed by its own <location> inside the denied folder. *Why:* location rules: the most specific wins.
- **forms.login-wrong** (/F/Login.aspx): a wrong password. *Why:* FormsAuthentication.Authenticate against web.config's credentials.
- **forms.login-return** (/F/Login.aspx?ReturnUrl=...): RedirectFromLoginPage. *Why:* the ReturnUrl decoded and followed.
- **forms.admin-denied** (/F/Admin/Page.aspx (user)): a signed-in user without the role. *Why:* a 401 for a signed-in user still goes to the login page.
- **forms.redirect-to-login** (/F/Secure/Page.aspx): FormsAuthentication.RedirectToLoginPage("extra=1"). *Why:* the extra query with the ReturnUrl.
- **forms.logout** (/F/Secure/Page.aspx): SignOut + Redirect to the secure page. *Why:* the expired cookie.
- **forms.back-after-logout** (/F/Secure/Page.aspx (history)): the browser's Back after logging out. *Why:* cache headers of a protected page (private).
- **forms.return-url-external** (/F/Login.aspx?ReturnUrl=http://evil.example/steal): a ReturnUrl to another host. *Why:* the open redirect check of GetRedirectUrl.
- **forms.return-url-scheme-relative** (/F/Login.aspx?ReturnUrl=//evil.example/x): a scheme-relative ReturnUrl. *Why:* "//host" passes a naive "starts with /" check.
- **forms.default-url** (/F/Login.aspx): a login without ReturnUrl, persistent. *Why:* createPersistentCookie.
- **forms.admin-allowed** (/F/Admin/Page.aspx (admin)): the Admin role (Application_PostAuthenticateRequest's GenericPrincipal). *Why:* roles set in an application event; security trimming for a role.
- **session.abandon** (/F/Abandon.aspx): Session.Abandon, then a postback. *Why:* session expiry in the middle of a flow; the session id cookie reused.
- **session.cookieless** ((an application with sessionState cookieless="UseUri")): cookieless session URLs /(S(id))/ with Redirect, Transfer and relative links. *Why:* the session id in the path: every relative URL and Redirect must keep it. *Not runnable:* sessionState cookieless is per application (MachineToApplication): it needs a second application in a subfolder; sample-parity.ps1 hosts one application at the site root (and the converter converts one application). A sample of its own would be needed.

### postback (16)

- **postback.back-stale** (/P/Counter.aspx): a postback, the browser's Back, a postback with the old view state. *Why:* the history's page posts its own view state: view state and session disagree.
- **postback.double-submit** (/P/Counter.aspx): the same form posted twice (a double click). *Why:* a replayed postback: event validation and view state accept it.
- **postback.no-viewstate** (/P/Counter.aspx): a POST with the button but no __VIEWSTATE. *Why:* what makes a request a postback.
- **postback.event-validation** (/P/Validate.aspx): an option added by script to a DropDownList, posted. *Why:* EnableEventValidation's check of posted values.
- **postback.no-event-validation** (/P/NoValidate.aspx): the same with EnableEventValidation="false". *Why:* a posted value the list does not have.
- **postback.unknown-target** (/P/Validate.aspx): __doPostBack of a control that does not exist. *Why:* __EVENTTARGET not found.
- **postback.bad-argument** (/P/Validate.aspx): __doPostBack(lnk, an argument it never registered). *Why:* event validation of the argument.
- **postback.hidden-target** (/P/Validate.aspx): __doPostBack of a control that is Visible=false. *Why:* a hidden button's event.
- **postback.tampered-viewstate** (/P/Counter.aspx): a __VIEWSTATE changed by the client. *Why:* EnableViewStateMac.
- **postback.dynamic-recreated** (/P/Dynamic.aspx): controls added in OnInit on every request. *Why:* dynamic controls re-created.
- **postback.dynamic-lost** (/P/Dynamic.aspx?recreate=0): controls added on the first request only. *Why:* the classic dynamic-control mistake.
- **postback.viewstate-order** (/P/Order.aspx): the dynamic labels added the other way round on the postback. *Why:* view state is matched by position.
- **postback.tree-changed** (/P/Changed.aspx): a Label where the DropDownList was. *Why:* a control tree changed between requests.
- **postback.chunked-viewstate** (/P/Chunk/Big.aspx): maxPageStateFieldLength="64" (a <location>). *Why:* the view state cut into fields.
- **postback.encrypted-viewstate** (/P/Encrypted.aspx): ViewStateEncryptionMode="Always". *Why:* the machine key's encryption.
- **postback.get-viewstate** (/P/GetState.aspx?__VIEWSTATE=...): a GET with __VIEWSTATE, __EVENTTARGET and __EVENTVALIDATION in the query. *Why:* the query string as the postback's values.

### wizard (11)

- **wizard.validation** (/W/Wizard.aspx): Next with an empty required field (server validation). *Why:* validation per step.
- **wizard.next** (/W/Wizard.aspx): Next, Next. *Why:* the Wizard's templates' buttons.
- **wizard.previous** (/W/Wizard.aspx): step 3 has no Previous (step 2 has AllowReturn=false); the side bar back to Step 1, then Next into the visited step 2. *Why:* AllowReturn hides the way back to a step; the history of steps.
- **wizard.sidebar** (/W/Wizard.aspx): the side bar's link to Step 3. *Why:* the side bar's DataList of LinkButtons.
- **wizard.finish** (/W/Wizard.aspx): the side bar to Confirm, Finish. *Why:* FinishButtonClick and the Complete step.
- **wizard.jump** (/W/Wizard.aspx): ActiveStepIndex = 2 in code. *Why:* the active step set by code.
- **wizard.cancel** (/W/Wizard.aspx): Cancel (CancelDestinationPageUrl). *Why:* the Wizard's own redirect.
- **wizard.finish-destination** (/W/WizardDest.aspx): Finish with FinishDestinationPageUrl. *Why:* the Wizard's redirect after Finish.
- **multiview.commands** (/W/MultiView.aspx): NextView, nested SwitchViewByID, the inner PrevView, PrevView, NextView, SwitchViewByIndex. *Why:* command bubbling through nested MultiViews.
- **multiview.set-active** (/W/MultiView.aspx): SetActiveView(v3) in code. *Why:* the active view set by code.
- **wizard.ajax** (/W/AjaxWizard.aspx): a Wizard inside an UpdatePanel: Next, Next, Finish. *Why:* the Wizard's postbacks through the page request manager.

### ajax (15)

- **ajax.update** (/J/Panel.aspx): an async postback inside the panel. *Why:* the delta format and the panel's refresh.
- **ajax.trigger-outside** (/J/Panel.aspx): a button outside the panel registered as AsyncPostBackTrigger. *Why:* triggers.
- **ajax.postback-trigger** (/J/Panel.aspx): a button inside the panel registered as PostBackTrigger. *Why:* a full postback from inside a panel.
- **ajax.outside-full** (/J/Panel.aspx): a button outside the panel, not a trigger. *Why:* the default.
- **ajax.redirect** (/J/Panel.aspx): Response.Redirect in an async postback. *Why:* the redirect turned into a delta by the ScriptModule.
- **ajax.transfer** (/J/Panel.aspx): Server.Transfer in an async postback. *Why:* Transfer during an async postback.
- **ajax.error** (/J/Panel.aspx): an exception in an async postback (AsyncPostBackError sets the message). *Why:* AsyncPostBackErrorMessage.
- **ajax.timeout** (/J/Panel.aspx): an async postback slower than AsyncPostBackTimeout (2 s). *Why:* the timeout written into the page's initialisation.
- **ajax.response-write** (/J/Panel.aspx): Response.Write in an async postback. *Why:* raw output breaks the delta.
- **ajax.response-end** (/J/Panel.aspx): Response.End in an async postback. *Why:* the end of an async response.
- **ajax.nested** (/J/Nested.aspx): nested conditional panels: the inner button, the outer, Update() of a third, a button of a panel whose children are not triggers. *Why:* UpdateMode Conditional, ChildrenAsTriggers false, nesting.
- **ajax.history** (/J/History.aspx): AddHistoryPoint on two async postbacks, the browser's Back twice and Forward. *Why:* history points (EnableSecureHistoryState false: the state in the fragment).
- **ajax.timer** (/J/Timer.aspx): a Timer in a panel (600 ms), disabled after 3 ticks. *Why:* postbacks the browser makes by itself.
- **ajax.page-methods** (/J/Methods.aspx): PageMethods: a result, an exception, a Response.Redirect inside. *Why:* static [WebMethod]s of a page (PathInfo /Echo on the .aspx).
- **ajax.script-service** (/J/ServicePage.aspx): a [ScriptService] .asmx through its generated proxy. *Why:* the proxy (/js) and the JSON call.

### routing (20)

- **route.constraint** (/nav/5): MapPageRoute nav/{id} (constraint \d+, default 1). *Why:* routes and the URLs a routed page resolves.
- **route.default** (/nav): the default value. *Why:* defaults.
- **route.constraint-fail** (/nav/abc): a value the constraint refuses. *Why:* constraints.
- **route.catch-all** (/files/a/b c, /files/x.txt): a catch-all {*rest}; one with a file extension. *Why:* which requests reach the routing module.
- **route.relative-link** (/nav/5): the relative HyperLink on a routed page. *Why:* relative URLs on routed pages.
- **route.postback** (/nav/5): a postback on a routed page. *Why:* the form action of a routed page.
- **route.transfer** (/go/somewhere): a routed page that transfers. *Why:* RouteData after Transfer.
- **url.pathinfo** (/U/Info.aspx/extra/more): PathInfo. *Why:* the path info that the browser takes as folders.
- **url.pathinfo-encoded** (/U/Info.aspx/a%20b/%E6%97%A5): escaped path info. *Why:* decoding of the path info.
- **url.default-document** (/U/): the folder's default document. *Why:* IIS's DefaultDocument module.
- **url.courtesy-redirect** (/U): a folder without its trailing slash. *Why:* IIS answers before ASP.NET.
- **url.rewrite-rebase** (/U/Pretty/abc): RewritePath(path) (rebaseClientPath true) in BeginRequest, then a postback. *Why:* the client path the form action is made from.
- **url.rewrite-no-rebase** (/U/PrettyNoRebase/abc): RewritePath(path, false) in BeginRequest, then a postback. *Why:* the client path the form action is made from.
- **url.rewrite-parts** (/U/PrettyParts/abc): RewritePath(file, pathInfo, query, false) in BeginRequest, then a postback. *Why:* the client path the form action is made from.
- **url.query-repeated** (/U/Query.aspx?a=1&a=2&a=): a repeated key. *Why:* HttpValueCollection's parsing.
- **url.query-empty** (/U/Query.aspx?a=&b&=c&&d=): empty values, a key without =, an empty key. *Why:* HttpValueCollection's parsing.
- **url.query-encoded** (/U/Query.aspx?a=x%26y&b=%E6%97%A5&c=a+b&d=%2B&e=%3D): escaped & = + and UTF-8. *Why:* HttpValueCollection's parsing.
- **url.query-invalid** (/U/Query.aspx?a=%ZZ&b=%u65E5&c=%E6%97): an invalid escape, IIS's %u, a truncated UTF-8. *Why:* HttpValueCollection's parsing.
- **url.case** (/u/QUERY.aspx?a=1): a URL in another case. *Why:* case-sensitive file systems (Linux).
- **url.double-slash** (//U//Query.aspx?a=1): doubled slashes. *Why:* path normalisation.

### navigation (10)

- **nav.sitemap** (/N/Deep/Page.aspx (anonymous)): SiteMapPath, Menu and TreeView of Web.sitemap with security trimming; a user control's URLs; the master's. *Why:* security trimming by URL authorization and roles; TemplateSourceDirectory of controls and masters.
- **nav.usercontrol-link** (/N/Deep/Page.aspx): the user control's relative HyperLink and the plain link. *Why:* relative URLs in controls of other folders.
- **nav.sitemap-admin** (/N/Deep/Page.aspx (admin)): the same as the Admin role (done in forms.admin-allowed). *Why:* trimming by role.
- **nav.menu** (/N/Menu.aspx): Menu items: NavigateUrl, a postback item, a child postback item. *Why:* Menu's postback vs NavigateUrl, value paths with \.
- **nav.treeview** (/N/Menu.aspx): TreeView without client script: expand by postback, select, a NavigateUrl node. *Why:* TreeView's postback events.
- **nav.imagemap** (/N/ImageMap.aspx): ImageMap hot spots: PostBack and Navigate. *Why:* <area> postbacks.
- **nav.client-click** (/N/Buttons.aspx): OnClientClick "return false" on a LinkButton and a Button; a confirm (accepted). *Why:* the client script before __doPostBack.
- **nav.default-button** (/N/Buttons.aspx): Enter in a Panel with DefaultButton; Enter in the form (form DefaultButton); DefaultFocus. *Why:* WebForm_FireDefaultButton.
- **nav.set-focus** (/N/Buttons.aspx): SetFocus(txtAfter) in a click. *Why:* WebForm_AutoFocus.
- **nav.scroll** (/N/Buttons.aspx): MaintainScrollPositionOnPostBack: the button at the bottom. *Why:* __SCROLLPOSITIONY.

### async (13)

- **async.redirect** (/Y/Async.aspx?mode=redirect): Async="true", RegisterAsyncTask: redirect after an await. *Why:* navigation from a continuation (another thread; the synchronization context).
- **async.redirect-false** (/Y/Async.aspx?mode=redirect-false): Async="true", RegisterAsyncTask: redirect-false after an await. *Why:* navigation from a continuation (another thread; the synchronization context).
- **async.transfer** (/Y/Async.aspx?mode=transfer): Async="true", RegisterAsyncTask: transfer after an await. *Why:* navigation from a continuation (another thread; the synchronization context).
- **async.throw** (/Y/Async.aspx?mode=throw): Async="true", RegisterAsyncTask: throw after an await. *Why:* navigation from a continuation (another thread; the synchronization context).
- **async.done** (/Y/Async.aspx): Async="true", RegisterAsyncTask: no navigation after an await. *Why:* navigation from a continuation (another thread; the synchronization context).
- **async.void** (/Y/AsyncVoid.aspx?mode=redirect): async void Page_Load: Redirect after an await. *Why:* an async void event handler (the page waits for it).
- **handler.redirect** (/Y/Redirect.ashx): a handler's Response.Redirect. *Why:* a handler mapped in web.config (no file); Redirect outside a page.
- **handler.redirect-permanent** (/Y/Redirect.ashx?mode=permanent): a handler's Response.RedirectPermanent. *Why:* a handler mapped in web.config (no file); Redirect outside a page.
- **handler.redirect-false** (/Y/Redirect.ashx?mode=false): a handler's Response.Redirect(url, false) + Write. *Why:* a handler mapped in web.config (no file); Redirect outside a page.
- **handler.transfer** (/Y/Transfer.ashx): a handler's Server.Transfer to a page and Server.Execute of a page. *Why:* child requests from a handler.
- **download.button** (/Y/Download.aspx): a Button that sends a file (Content-Disposition attachment, Response.End), then a postback. *Why:* the page stays after a download.
- **download.linkbutton** (/Y/Download.aspx): a LinkButton download (__EVENTTARGET left set), then a Button and a LinkButton. *Why:* __EVENTTARGET still holding the download's target.
- **download.unicode** (/Y/Download.aspx): filename* (RFC 5987, UTF-8). *Why:* the header as written.

### windows (6)

- **window.blank-link** (/Y/Opener.aspx): a HyperLink with Target="_blank". *Why:* new windows.
- **window.form-target** (/Y/Opener.aspx): OnClientClick sets the form's target to _blank; then an ordinary button. *Why:* a form target left set by script.
- **window.open-opener** (/Y/Opener.aspx): window.open, a postback in the popup that writes into its opener. *Why:* window.opener after a postback.
- **frame.postback** (/Y/Frames.aspx): postbacks and a Response.Redirect inside an iframe. *Why:* navigation inside frames.
- **frame.target** (/Y/Frames.aspx): a link of the page with target="f1"; a postback in the new frame page. *Why:* named targets.
- **frame.top** (/Y/Frames.aspx): a target="_top" link and a postback whose script sets top.location. *Why:* leaving the frame.

### vb (5)

- **vb.handles-redirect** (/V/Handles.aspx): Handles Me.Load and Handles btnGo.Click (AutoEventWireup false): Response.Redirect. *Why:* VB's event wiring (WithEvents fields of the generated class).
- **vb.catch-when** (/V/Handles.aspx): Redirect in Try ... Catch ex As ThreadAbortException When filter(). *Why:* exception filters run before the stack unwinds; the abort re-raised after the Catch.
- **vb.transfer** (/V/Handles.aspx): Server.Transfer from a VB handler. *Why:* Transfer in VB.
- **vb.my** (/V/My.aspx): My.Request / My.Response.Redirect. *Why:* VB's My namespace for the web (_MYTYPE="Web").
- **vb.cross-page** (/V/Source.aspx -> /V/Target.aspx): a cross-page postback between VB pages (PreviousPageType, a VB property). *Why:* PreviousPageType in VB.
