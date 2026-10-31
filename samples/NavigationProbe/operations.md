# NavigationProbe: the inventory of operations

Written by `inventory.py` from the same table as `operations.json` (edit the table there and run it). Each operation is exercised by the steps of
`parity-scenario.json` that name it (`op`); the golden data is recorded on the original on IIS and compared on Windows and Linux
(`experiments/wf4c/sample-parity.ps1 -Name NavigationProbe -Record / -Windows / -Linux`). Coverage: `inventory.py` checks every interactive
server control and page of the source belongs to an operation (static); ParityTest `discover` crawls the running site and lists what no
operation's keys cover (`sample-parity.ps1 ... -Discover`).

| category | operations | not runnable |
|---|---|---|
| index | 1 | 0 |
| transfer | 22 | 0 |
| cross-page | 10 | 0 |
| redirect | 48 | 0 |
| global | 13 | 0 |
| error | 20 | 0 |
| forms | 16 | 1 |
| postback | 16 | 0 |
| wizard | 11 | 0 |
| ajax | 15 | 0 |
| routing | 20 | 0 |
| navigation | 10 | 0 |
| async | 13 | 0 |
| windows | 6 | 0 |
| vb | 5 | 0 |
| **total** | **226** | **1** |

## index

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| index.home | / (Default.aspx) | the index of every case | the links | nothing: the starting point (and what discover crawls) |

## transfer

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| transfer.preserve-form | /T/Source.aspx -> ~/T/Sub/Target.aspx | Server.Transfer(path, true) from a click | the target in another folder runs; the address stays /T/Source.aspx; Request.Form and PreviousPage (FindControl, the typed property) are the source's | the target renders under the source's URL: ~ / relative URLs, the form's action, ResolveClientUrl and PreviousPage all depend on the executing vs the requested path (Context.Handler / PreviousHandler) |
| transfer.no-form | /T/Source.aspx | Server.Transfer(path, false) | the target sees no form; PreviousPage still set | preserveForm false clears Request.Form for the target only |
| transfer.query | /T/Source.aspx | Server.Transfer("...?q=from-transfer&q=again", true) | the target's query is the transfer's (repeated key) | the query string replaced by the transfer's while RawUrl stays the browser's |
| transfer.relative-links | /T/Sub/Target.aspx after a transfer | HyperLink NavigateUrl="Other.aspx", a href="Other.aspx" (plain and runat=server) | HyperLink / runat=server resolve against the target's folder (T/Sub); the plain link against the browser's (T/) - a 404 | the classic Transfer breakage: server-resolved URLs and browser-resolved URLs disagree |
| transfer.target-postback | /T/Sub/Target.aspx after a transfer | a postback from the transferred-to page | the form posts to the action the target rendered (relative to the source's URL) | the form action after Transfer: a relative "Target.aspx"-style action resolved by the browser against the old URL |
| transfer.handler | /T/Source.aspx | Server.Transfer(IHttpHandler, true) | the handler writes into the page's response; Context.Handler is the page | a handler object instead of a path: no page, no PreviousPage |
| transfer.ashx | /T/Source.aspx | Server.Transfer("~/T/Echo.ashx?via=transfer") | as .NET Framework: the .ashx through Transfer | Transfer to a non-page handler path (the handler factory of .ashx) |
| transfer.chain | /T/Chain1.aspx -> Chain2 -> Sub/Target | nested Server.Transfer from Page_Load | three pages in one request; PreviousPage the last one | each transfer nests a child request; the log and PreviousPage show the order |
| transfer.after-write | /T/Source.aspx | Response.Write then Server.Transfer | what was written stays before the target's output | Transfer does not clear the response |
| transfer.catch-threadabort | /T/Source.aspx | Server.Transfer inside try / catch (ThreadAbortException) | on .NET Framework the abort is re-raised at the end of the catch: the code after the try never runs (the session shows what ran) | .NET has no thread abort: code after a swallowed Response.End / Transfer runs unless the port re-raises it |
| transfer.catch-exception | /T/Source.aspx | Server.Transfer inside try / catch (Exception) | as above with catch (Exception): the type caught | the exception type a broad catch sees (ThreadAbortException) |
| transfer.transfer-request | /T/Source.aspx | Server.TransferRequest("~/T/Sub/Target.aspx?tr=1", true) | a new pipeline for the target: no PreviousPage; the form kept | IIS integrated mode's child request (the whole pipeline again, Global.asax events too) |
| transfer.missing | /T/Source.aspx | Server.Transfer to a page that does not exist | the HttpException (404 inside a transfer: 500? as the original) | the error of a child request |
| execute.load | /T/Exec.aspx?mode=load | Server.Execute (load) | the executed page's output first, before the page's own <html> | Execute writes straight into the response, at the time it is called |
| execute.render | /T/Exec.aspx?mode=render | Server.Execute (render) | the executed page's output inside the div, during Render | Execute from inline code, in the middle of the page's output |
| execute.writer | /T/Exec.aspx?mode=writer | Server.Execute (writer) | the output captured by a StringWriter, shown encoded | Execute(path, TextWriter): another writer for the child page |
| execute.ashx | /T/Exec.aspx?mode=ashx | Server.Execute (ashx) | an .ashx executed into a writer | Execute of a handler path |
| execute.redirect | /T/Exec.aspx?mode=redirect | Server.Execute (redirect) | the executed page's Response.Redirect redirects the whole request | Response.Redirect inside a child request (who ends?) |
| execute.transfer | /T/Exec.aspx?mode=transfer | Server.Execute (transfer) | the executed page's Server.Transfer | Transfer inside Execute |
| execute.throw | /T/Exec.aspx?mode=throw | Server.Execute (throw) | HttpException "Error executing child request" with the inner exception | the wrapping of a child request's exception |
| execute.missing | /T/Exec.aspx?mode=missing | Server.Execute (missing) | the HttpException of a missing page | Execute of a page that does not exist |
| execute.preserve-form | /T/Exec.aspx | Server.Execute(path, writer, preserveForm true / false) on a postback | the child sees the form or not | preserveForm on Execute |

## cross-page

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| cross.same-folder | /X/Source.aspx -> /X/Target.aspx | Button PostBackUrl; the target's PreviousPageType | the target: IsCrossPagePostBack of the source, the typed PreviousPage.NameTyped, IsValid | the source page is re-created (its LoadComplete) inside the target's request; PreviousPageType makes a typed property in generated code |
| cross.validation | /X/Source.aspx -> /X/Target.aspx | cross-page postback with an empty required field (server validation only) | PreviousPage.IsValid false: the target decides | validation runs on the source page when the target asks PreviousPage.IsValid |
| cross.no-validation | /X/Source.aspx | CausesValidation=false with PostBackUrl | IsValid without validation: the exception or true | Page.IsValid before Validate() |
| cross.other-folder | /X/Source.aspx -> /X/Other/Target.aspx | PostBackUrl to a page in another folder (no PreviousPageType) | FindControl on PreviousPage; relative URLs of the target | the browser's URL is the target's, unlike Transfer |
| cross.query | /X/Source.aspx | PostBackUrl with a query | the target sees via=query | WebForm_DoPostBackWithOptions sets the form's action with the query |
| cross.linkbutton | /X/Source.aspx | LinkButton PostBackUrl | the same as the button, through the script | __doPostBack with options and __PREVIOUSPAGE |
| cross.target-postback | /X/Target.aspx | a normal postback on the target after a cross-page postback | PreviousPage gone, IsPostBack true | the target's own view state after a cross-page arrival |
| cross.target-direct | /X/Target.aspx | GET of the target | no previous page | the typed PreviousPage null |
| cross.master | /X/MasterSource.aspx (master A) -> /X/MasterTarget.aspx (master B) | PostBackUrl between pages of different masters | PreviousPage.FindControl misses the content control; Master.FindControl(main).FindControl finds it; each master's relative URLs | naming containers of the content placeholder; masters in another folder |
| cross.plain-postback | /X/Source.aspx | a button without PostBackUrl on the source | an ordinary postback (the validator runs) | the same page without cross-page |

## redirect

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| redirect.at-preinit | /R/Life.aspx?at=PreInit | Response.Redirect in PreInit (endResponse true, then false) | true: 302, the rest of the page not run; false: 302 with the page's output after the "Object moved" body, the events after it run | each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone) |
| redirect.at-init | /R/Life.aspx?at=Init | Response.Redirect in Init (endResponse true, then false) | true: 302, the rest of the page not run; false: 302 with the page's output after the "Object moved" body, the events after it run | each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone) |
| redirect.at-initcomplete | /R/Life.aspx?at=InitComplete | Response.Redirect in InitComplete (endResponse true, then false) | true: 302, the rest of the page not run; false: 302 with the page's output after the "Object moved" body, the events after it run | each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone) |
| redirect.at-preload | /R/Life.aspx?at=PreLoad | Response.Redirect in PreLoad (endResponse true, then false) | true: 302, the rest of the page not run; false: 302 with the page's output after the "Object moved" body, the events after it run | each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone) |
| redirect.at-load | /R/Life.aspx?at=Load | Response.Redirect in Load (endResponse true, then false) | true: 302, the rest of the page not run; false: 302 with the page's output after the "Object moved" body, the events after it run | each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone) |
| redirect.at-click | /R/Life.aspx?at=Click | Response.Redirect in Click (endResponse true, then false) | true: 302, the rest of the page not run; false: 302 with the page's output after the "Object moved" body, the events after it run | each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone) |
| redirect.at-loadcomplete | /R/Life.aspx?at=LoadComplete | Response.Redirect in LoadComplete (endResponse true, then false) | true: 302, the rest of the page not run; false: 302 with the page's output after the "Object moved" body, the events after it run | each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone) |
| redirect.at-prerender | /R/Life.aspx?at=PreRender | Response.Redirect in PreRender (endResponse true, then false) | true: 302, the rest of the page not run; false: 302 with the page's output after the "Object moved" body, the events after it run | each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone) |
| redirect.at-prerendercomplete | /R/Life.aspx?at=PreRenderComplete | Response.Redirect in PreRenderComplete (endResponse true, then false) | true: 302, the rest of the page not run; false: 302 with the page's output after the "Object moved" body, the events after it run | each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone) |
| redirect.at-savestatecomplete | /R/Life.aspx?at=SaveStateComplete | Response.Redirect in SaveStateComplete (endResponse true, then false) | true: 302, the rest of the page not run; false: 302 with the page's output after the "Object moved" body, the events after it run | each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone) |
| redirect.at-render | /R/Life.aspx?at=Render | Response.Redirect in Render (endResponse true, then false) | true: 302, the rest of the page not run; false: 302 with the page's output after the "Object moved" body, the events after it run | each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone) |
| redirect.at-unload | /R/Life.aspx?at=Unload | Response.Redirect in Unload (endResponse true, then false) | true: 302, the rest of the page not run; false: 302 with the page's output after the "Object moved" body, the events after it run | each event of the page's lifecycle: before / after the form is read, during Render (headers not sent: buffered), in Unload (the response is gone) |
| redirect.tae | /R/Cases.aspx?c=tae | Redirect in try / catch (ThreadAbortException): the code after the try | the redirect; on .NET Framework neither the code after the try nor the rest runs (the session shows what did) | thread abort emulation |
| redirect.ex | /R/Cases.aspx?c=ex | Redirect in try / catch (Exception) | the type caught; the code after the try (the session) | a broad catch swallowing the end of the response |
| redirect.finally | /R/Cases.aspx?c=finally | Redirect in try / finally | the finally runs; the redirect | finally blocks during the abort |
| redirect.written | /R/Cases.aspx?c=written | Response.Write then Redirect | the redirect: what was written is cleared | a buffered response is cleared |
| redirect.flushed | /R/Cases.aspx?c=flushed | Response.Write, Flush, then Redirect | HttpException "Cannot redirect after HTTP headers have been sent" after the flushed text (200) | headers sent: the redirect fails mid-page |
| redirect.permanent | /R/Cases.aspx?c=permanent | RedirectPermanent | 301 | the permanent redirect's status |
| redirect.permanent-false | /R/Cases.aspx?c=permanent-false | RedirectPermanent(url, false) | 301 with the page after it | endResponse false on the permanent redirect |
| redirect.route | /R/Cases.aspx?c=route | RedirectToRoute("NavRoute", id=3) | 302 to /nav/3 | the route's URL from values |
| redirect.route-permanent | /R/Cases.aspx?c=route-permanent | RedirectToRoutePermanent with an extra value (space) | 301 to /nav/4?extra=x%20y | the extra value as a query, its encoding |
| redirect.route-name | /R/Cases.aspx?c=route-name | RedirectToRoute("FilesRoute", rest="a/b c") | the catch-all route's URL (the slash kept, the space escaped) | catch-all parameters' encoding |
| redirect.external | /R/Cases.aspx?c=external | Redirect to an absolute external URL (a space in the query) | 302 to http://external.example/... (the browser goes there: its own error page, .example names do not resolve) | absolute URLs left as they are |
| redirect.protocol-relative | /R/Cases.aspx?c=protocol-relative | Redirect to //external.example/relative | the scheme-relative Location | a URL that looks relative but leaves the site |
| redirect.absolute-path | /R/Cases.aspx?c=absolute-path | Redirect to /R/Landing.aspx | the path as given | an absolute path |
| redirect.unicode | /R/Cases.aspx?c=unicode | Redirect with Unicode, a space, % and & in the query | the encoding of the Location header (non-ASCII, space, a lone %) | UrlEncodeRedirect / UrlPathEncode of the query |
| redirect.encoded | /R/Cases.aspx?c=encoded | Redirect with an already escaped query (%26 %E6 %2B) | escapes kept, not escaped twice | double encoding |
| redirect.unicode-path | /R/Cases.aspx?c=unicode-path | Redirect to ~/R/着地.aspx | the path escaped (UTF-8) | non-ASCII file names (Linux file system, the escaping) |
| redirect.space-path | /R/Cases.aspx?c=space-path | Redirect to ~/R/With Space.aspx?from=space path | the space in the path and the query | spaces in paths |
| redirect.newline | /R/Cases.aspx?c=newline | Redirect to a URL with CR LF (header injection) | refused: ArgumentException / the URL sanitised, as the original | response splitting protection |
| redirect.empty | /R/Cases.aspx?c=empty | Redirect("") | as the original (to the folder? an exception?) | the edge of URL resolution |
| redirect.end | /R/Cases.aspx?c=end | Response.Write, Response.End, Write | only the text before End | Response.End without a redirect |
| redirect.complete | /R/Cases.aspx?c=complete | Write, CompleteRequest, the page goes on | the page still renders (CompleteRequest only skips the pipeline) | CompleteRequest is not End |
| redirect.false-complete | /R/Cases.aspx?c=false-complete | Redirect(url, false) + CompleteRequest (the recommended pattern) | 302 with the page rendered after the redirect's body | the documented alternative to Redirect(url) still renders the page |
| redirect.status200 | /R/Cases.aspx?c=status200 | Redirect(url, false), then StatusCode = 200 in PreRender | 200 with a Location header and the page | a status overridden after the redirect |
| redirect.status301 | /R/Cases.aspx?c=status301 | Redirect(url, false), then StatusCode = 301 | 301 with the Location | the status changed, the Location kept |
| redirect.status404 | /R/Cases.aspx?c=status404 | Redirect(url, false), then StatusCode = 404 (TrySkipIisCustomErrors) | 404 with a Location header | a redirect turned into an error |
| redirect.twice | /R/Cases.aspx?c=twice | Redirect(a, false) then Redirect(b) | the second wins | the Location replaced |
| redirect.clear-headers | /R/Cases.aspx?c=clear-headers | Redirect(url, false) then ClearHeaders | no Location: 200? as the original | the redirect's headers removed |
| redirect.relative-parent | /R/Sub/Rel.aspx?c=parent | Response.Redirect("../Landing.aspx") from R/Sub | the Location as the original made it absolute; the browser's address (the fragment kept) | relative URL resolution of Redirect (against the request's folder), fragments |
| redirect.relative-file | /R/Sub/Rel.aspx?c=file | Response.Redirect("Landing.aspx") from R/Sub | the Location as the original made it absolute; the browser's address (the fragment kept) | relative URL resolution of Redirect (against the request's folder), fragments |
| redirect.relative-dot | /R/Sub/Rel.aspx?c=dot | Response.Redirect("./Landing.aspx") from R/Sub | the Location as the original made it absolute; the browser's address (the fragment kept) | relative URL resolution of Redirect (against the request's folder), fragments |
| redirect.relative-tilde | /R/Sub/Rel.aspx?c=tilde | Response.Redirect("~/R/Landing.aspx") from R/Sub | the Location as the original made it absolute; the browser's address (the fragment kept) | relative URL resolution of Redirect (against the request's folder), fragments |
| redirect.relative-query | /R/Sub/Rel.aspx?c=query | Response.Redirect("?from=query-only") from R/Sub | the Location as the original made it absolute; the browser's address (the fragment kept) | relative URL resolution of Redirect (against the request's folder), fragments |
| redirect.relative-frag | /R/Sub/Rel.aspx?c=frag | Response.Redirect("#frag") from R/Sub | the Location as the original made it absolute; the browser's address (the fragment kept) | relative URL resolution of Redirect (against the request's folder), fragments |
| redirect.relative-query-frag | /R/Sub/Rel.aspx?c=query-frag | Response.Redirect("Landing.aspx?from=query-frag#section") from R/Sub | the Location as the original made it absolute; the browser's address (the fragment kept) | relative URL resolution of Redirect (against the request's folder), fragments |
| redirect.relative-tilde-frag | /R/Sub/Rel.aspx?c=tilde-frag | Response.Redirect("~/R/Landing.aspx#top") from R/Sub | the Location as the original made it absolute; the browser's address (the fragment kept) | relative URL resolution of Redirect (against the request's folder), fragments |
| redirect.relative-parent-parent | /R/Sub/Rel.aspx?c=parent-parent | Response.Redirect("../../Default.aspx") from R/Sub | the Location as the original made it absolute; the browser's address (the fragment kept) | relative URL resolution of Redirect (against the request's folder), fragments |

## global

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| global.begin-request | /G/Begin.aspx | Response.Redirect in Application_BeginRequest | 302 before anything else ran | the earliest event: no handler, no session |
| global.authenticate-request | /G/Auth.aspx | Response.Redirect in Application_AuthenticateRequest | 302 | during authentication |
| global.post-acquire-state | /G/Acquire.aspx | Response.Redirect in Application_PostAcquireRequestState (with the session) | 302; the session available | the session's event |
| global.end-request | /G/End.aspx | Response.Redirect in Application_EndRequest after the page ran | 302 replacing the page's output | a redirect at the very end (headers not sent yet) |
| global.module-redirect | /G/Module.aspx | an IHttpModule's Response.Redirect in PostAuthorizeRequest | 302 | a module of web.config's system.webServer/modules |
| global.module-complete | /G/ModuleEnd.aspx | a module answers and calls CompleteRequest | the module's answer; the page never runs | CompleteRequest from a module |
| global.error-redirect | /G/ErrRedirect.aspx | Application_Error: ClearError + Redirect | 302 to the landing with the exception type | error handling by redirect |
| global.error-redirect-kept | /G/ErrRedirectKeep.aspx | Application_Error: Redirect without ClearError | the redirect (or the error page?) as the original | the error still pending when redirecting |
| global.error-transfer | /G/ErrTransfer.aspx | Application_Error: ClearError + Server.Transfer | the handled page under the failing URL | Transfer outside the page's execution |
| global.error-execute | /G/ErrExecute.aspx | Application_Error: ClearError + Clear + Server.Execute | the handled page's output | Execute from an application event |
| global.error-clear | /G/ErrClear.aspx | Application_Error: ClearError + Write | 200 with only the written text | swallowing an error |
| global.redirect-loop | /G/LoopA.aspx <-> /G/LoopB.aspx | a redirect loop | each hop the same; the browser gives up (ERR_TOO_MANY_REDIRECTS) | relative Location values ("LoopB.aspx") resolved hop after hop |
| global.redirect-chain | /G/Chain.aspx?n=19 / ?n=25 | a chain of 19 redirects (the browser follows) and of 25 (it does not) | the end of the chain / the browser's error | the browser's redirect limit (20) |

## error

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| error.redirect-mode | /E/Redirect/Throw.aspx | customErrors On, ResponseRedirect: an exception | 302 to ~/E/Oops.aspx?aspxerrorpath=/E/Redirect/Throw.aspx | the error page by redirect |
| error.redirect-404 | /E/Redirect/Http404.aspx | HttpException(404) under ResponseRedirect with an <error statusCode="404"> | 302 to ~/E/NotFound.aspx?aspxerrorpath=... | per-status pages |
| error.redirect-403 | /E/Redirect/Http403.aspx | HttpException(403) without its own page | 302 to the default page | the status without an entry |
| error.redirect-missing | /E/Redirect/Missing.aspx | a missing .aspx under customErrors On | 302 to the 404 page | the page handler factory's 404 |
| error.rewrite-mode | /E/Rewrite/Throw.aspx | customErrors ResponseRewrite: an exception | 500 with Oops.aspx's content under the failing URL (GetLastError available) | Server.Execute of the error page |
| error.rewrite-404 | /E/Rewrite/Http404.aspx | HttpException(404) under ResponseRewrite | 404 with NotFound.aspx's content | the per-status page rewritten |
| error.rewrite-missing | /E/Rewrite/Missing.aspx | a missing page under ResponseRewrite | 404 with NotFound.aspx's content | rewrite for the 404 of the factory |
| error.error-page-throws | /E/Broken/Throw.aspx | the error page itself throws (ResponseRewrite) | the generic runtime error page | an error inside the error page |
| error.no-default | /E/Plain/Throw.aspx | customErrors On without defaultRedirect | the generic "Runtime Error" page (no details) | what a remote user sees |
| error.detailed | /E/Throw.aspx | customErrors Off: an exception | the detailed error page (500) | the yellow screen |
| error.httpexception-404 | /E/Http404.aspx | HttpException(404) with customErrors Off | the detailed 404 page | HttpException's status |
| error.missing-page | /E/NoSuchPage.aspx | a missing .aspx | ASP.NET's 404 | the resource cannot be found |
| error.compile | /E/Compile.aspx | a page whose inline code does not compile | Compilation Error (CS0029) | the server's compiler and its error page |
| error.parser | /E/Parser.aspx | a page with an unknown control | Parser Error | the parser's error page |
| error.bad-directive | /E/BadDirective.aspx | a page inheriting a class that does not exist | Parser Error (could not load type) | a broken Inherits |
| error.httperrors-replaced | /E/Http/Status404.aspx | httpErrors Custom / existingResponse Auto: the page sets 404 without TrySkipIisCustomErrors | IIS runs /E/NotFound.aspx (ExecuteURL) instead of the page's body | IIS's error pages over the application's answer |
| error.httperrors-skip | /E/Http/Skip404.aspx | the same with TrySkipIisCustomErrors | the page's own 404 body | TrySkipIisCustomErrors under Auto |
| error.httperrors-missing | /E/Http/NoSuch.aspx | a missing page under httpErrors Custom | as the original: ASP.NET's 404 or IIS's ExecuteURL | which layer answers the 404 |
| error.async | /E/Redirect/Async.aspx | an async postback that throws under customErrors On | the async error redirects the browser to the defaultRedirect (pageRedirect) | customErrors and the page request manager |
| error.asyncnoredirect | /E/Redirect/AsyncNoRedirect.aspx | an async postback that throws under customErrors On | the generic async error message (AllowCustomErrorsRedirect false) | customErrors and the page request manager |

## forms

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| forms.protected-redirect | /F/Secure/Page.aspx (anonymous) | a page of a folder denied to anonymous users | 302 to ~/F/Login.aspx?ReturnUrl=%2fF%2fSecure%2fPage.aspx | the authorization module's 401 turned into the login redirect (ReturnUrl's encoding) |
| forms.return-url-query-unicode | /F/Secure/Page.aspx?a=1&b=日本&c=x%26y | the ReturnUrl of a URL with a query, Unicode and an escaped & | the ReturnUrl escaped once, as the original | nested encoding of the ReturnUrl |
| forms.secure-route | /secure/5 (anonymous) | a route to a page of the protected folder | as the original: URL authorization checks the route's URL, not the page's file | the gap between URL authorization and routes |
| forms.public-page | /F/Secure/Public.aspx | a page allowed by its own <location> inside the denied folder | 200 for anonymous | location rules: the most specific wins |
| forms.login-wrong | /F/Login.aspx | a wrong password | the message; no cookie | FormsAuthentication.Authenticate against web.config's credentials |
| forms.login-return | /F/Login.aspx?ReturnUrl=... | RedirectFromLoginPage | back to the ReturnUrl (its query and Unicode intact); the session cookie of the ticket | the ReturnUrl decoded and followed |
| forms.admin-denied | /F/Admin/Page.aspx (user) | a signed-in user without the role | 302 to the login page (authenticated but not authorized) | a 401 for a signed-in user still goes to the login page |
| forms.redirect-to-login | /F/Secure/Page.aspx | FormsAuthentication.RedirectToLoginPage("extra=1") | the login page with ReturnUrl and extra | the extra query with the ReturnUrl |
| forms.logout | /F/Secure/Page.aspx | SignOut + Redirect to the secure page | the login page again; the ticket cookie removed | the expired cookie |
| forms.back-after-logout | /F/Secure/Page.aspx (history) | the browser's Back after logging out | as the original (the page from the cache or the login again) | cache headers of a protected page (private) |
| forms.return-url-external | /F/Login.aspx?ReturnUrl=http://evil.example/steal | a ReturnUrl to another host | refused: the default URL (or the original's answer) | the open redirect check of GetRedirectUrl |
| forms.return-url-scheme-relative | /F/Login.aspx?ReturnUrl=//evil.example/x | a scheme-relative ReturnUrl | refused as the original | "//host" passes a naive "starts with /" check |
| forms.default-url | /F/Login.aspx | a login without ReturnUrl, persistent | the defaultUrl (F/Welcome); a persistent cookie | createPersistentCookie |
| forms.admin-allowed | /F/Admin/Page.aspx (admin) | the Admin role (Application_PostAuthenticateRequest's GenericPrincipal) | 200; the site map shows the admin nodes | roles set in an application event; security trimming for a role |
| session.abandon | /F/Abandon.aspx | Session.Abandon, then a postback | the value gone; a new session (IsNewSession) with the view state still there | session expiry in the middle of a flow; the session id cookie reused |
| session.cookieless | (an application with sessionState cookieless="UseUri") | cookieless session URLs /(S(id))/ with Redirect, Transfer and relative links | not runnable: sessionState cookieless is per application (MachineToApplication): it needs a second application in a subfolder; sample-parity.ps1 hosts one application at the site root (and the converter converts one application). A sample of its own would be needed. | the session id in the path: every relative URL and Redirect must keep it |

## postback

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| postback.back-stale | /P/Counter.aspx | a postback, the browser's Back, a postback with the old view state | view=1 (the stale view state) session=2 | the history's page posts its own view state: view state and session disagree |
| postback.double-submit | /P/Counter.aspx | the same form posted twice (a double click) | both answers view=1; the session counted twice | a replayed postback: event validation and view state accept it |
| postback.no-viewstate | /P/Counter.aspx | a POST with the button but no __VIEWSTATE | IsPostBack true, the click raised, the state lost | what makes a request a postback |
| postback.event-validation | /P/Validate.aspx | an option added by script to a DropDownList, posted | Invalid postback or callback argument (500) | EnableEventValidation's check of posted values |
| postback.no-event-validation | /P/NoValidate.aspx | the same with EnableEventValidation="false" | accepted; the list keeps its own selection | a posted value the list does not have |
| postback.unknown-target | /P/Validate.aspx | __doPostBack of a control that does not exist | a postback without an event | __EVENTTARGET not found |
| postback.bad-argument | /P/Validate.aspx | __doPostBack(lnk, an argument it never registered) | Invalid postback or callback argument | event validation of the argument |
| postback.hidden-target | /P/Validate.aspx | __doPostBack of a control that is Visible=false | Invalid postback (it was never rendered) | a hidden button's event |
| postback.tampered-viewstate | /P/Counter.aspx | a __VIEWSTATE changed by the client | the view state MAC / state invalid error (500) | EnableViewStateMac |
| postback.dynamic-recreated | /P/Dynamic.aspx | controls added in OnInit on every request | the dynamic button's click with the typed text | dynamic controls re-created |
| postback.dynamic-lost | /P/Dynamic.aspx?recreate=0 | controls added on the first request only | the click lost; the text only in Request.Form | the classic dynamic-control mistake |
| postback.viewstate-order | /P/Order.aspx | the dynamic labels added the other way round on the postback | the labels' texts swapped (positional view state) | view state is matched by position |
| postback.tree-changed | /P/Changed.aspx | a Label where the DropDownList was | as the original: "Failed to load viewstate" or the state ignored | a control tree changed between requests |
| postback.chunked-viewstate | /P/Chunk/Big.aspx | maxPageStateFieldLength="64" (a <location>) | __VIEWSTATEFIELDCOUNT and __VIEWSTATE1..n; the value restored | the view state cut into fields |
| postback.encrypted-viewstate | /P/Encrypted.aspx | ViewStateEncryptionMode="Always" | __VIEWSTATEENCRYPTED; the label restored | the machine key's encryption |
| postback.get-viewstate | /P/GetState.aspx?__VIEWSTATE=... | a GET with __VIEWSTATE, __EVENTTARGET and __EVENTVALIDATION in the query | IsPostBack true on a GET; the click raised | the query string as the postback's values |

## wizard

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| wizard.validation | /W/Wizard.aspx | Next with an empty required field (server validation) | the step stays; the validator's text | validation per step |
| wizard.next | /W/Wizard.aspx | Next, Next | steps 2 and 3; NextButtonClick and ActiveStepChanged | the Wizard's templates' buttons |
| wizard.previous | /W/Wizard.aspx | step 3 has no Previous (step 2 has AllowReturn=false); the side bar back to Step 1, then Next into the visited step 2 | step 1 again; Next does not enter step 2 again (it stays on step 1) | AllowReturn hides the way back to a step; the history of steps |
| wizard.sidebar | /W/Wizard.aspx | the side bar's link to Step 3 | SideBarButtonClick; step 3 | the side bar's DataList of LinkButtons |
| wizard.finish | /W/Wizard.aspx | the side bar to Confirm, Finish | the Complete step | FinishButtonClick and the Complete step |
| wizard.jump | /W/Wizard.aspx | ActiveStepIndex = 2 in code | step 3 | the active step set by code |
| wizard.cancel | /W/Wizard.aspx | Cancel (CancelDestinationPageUrl) | redirect to W/Cancelled.aspx | the Wizard's own redirect |
| wizard.finish-destination | /W/WizardDest.aspx | Finish with FinishDestinationPageUrl | redirect to W/Finished.aspx?from=wizard | the Wizard's redirect after Finish |
| multiview.commands | /W/MultiView.aspx | NextView, nested SwitchViewByID, the inner PrevView, PrevView, NextView, SwitchViewByIndex | each view; the inner MultiView handles its own commands (bubbling to the nearest) | command bubbling through nested MultiViews |
| multiview.set-active | /W/MultiView.aspx | SetActiveView(v3) in code | view three | the active view set by code |
| wizard.ajax | /W/AjaxWizard.aspx | a Wizard inside an UpdatePanel: Next, Next, Finish | each step by an async postback | the Wizard's postbacks through the page request manager |

## ajax

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| ajax.update | /J/Panel.aspx | an async postback inside the panel | the panel updated (requests: 2); one more page load | the delta format and the panel's refresh |
| ajax.trigger-outside | /J/Panel.aspx | a button outside the panel registered as AsyncPostBackTrigger | an async postback | triggers |
| ajax.postback-trigger | /J/Panel.aspx | a button inside the panel registered as PostBackTrigger | a full postback | a full postback from inside a panel |
| ajax.outside-full | /J/Panel.aspx | a button outside the panel, not a trigger | a full postback | the default |
| ajax.redirect | /J/Panel.aspx | Response.Redirect in an async postback | the delta "pageRedirect"; the browser goes to J/Landing | the redirect turned into a delta by the ScriptModule |
| ajax.transfer | /J/Panel.aspx | Server.Transfer in an async postback | as the original (an error delta / the page) | Transfer during an async postback |
| ajax.error | /J/Panel.aspx | an exception in an async postback (AsyncPostBackError sets the message) | the error delta with the handled message, shown by the page's script | AsyncPostBackErrorMessage |
| ajax.timeout | /J/Panel.aspx | an async postback slower than AsyncPostBackTimeout (2 s) | the client's timeout error | the timeout written into the page's initialisation |
| ajax.response-write | /J/Panel.aspx | Response.Write in an async postback | the parser error ("The message received from the server could not be parsed") | raw output breaks the delta |
| ajax.response-end | /J/Panel.aspx | Response.End in an async postback | as the original | the end of an async response |
| ajax.nested | /J/Nested.aspx | nested conditional panels: the inner button, the outer, Update() of a third, a button of a panel whose children are not triggers | only the panels updated show the new request number | UpdateMode Conditional, ChildrenAsTriggers false, nesting |
| ajax.history | /J/History.aspx | AddHistoryPoint on two async postbacks, the browser's Back twice and Forward | the address's fragment; Navigate restores each step | history points (EnableSecureHistoryState false: the state in the fragment) |
| ajax.timer | /J/Timer.aspx | a Timer in a panel (600 ms), disabled after 3 ticks | ticks: 3 | postbacks the browser makes by itself |
| ajax.page-methods | /J/Methods.aspx | PageMethods: a result, an exception, a Response.Redirect inside | the result; the error (500, its type and message); the redirect's answer | static [WebMethod]s of a page (PathInfo /Echo on the .aspx) |
| ajax.script-service | /J/ServicePage.aspx | a [ScriptService] .asmx through its generated proxy | the result | the proxy (/js) and the JSON call |

## routing

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| route.constraint | /nav/5 | MapPageRoute nav/{id} (constraint \d+, default 1) | the page with id=5; GetRouteUrl, RouteUrl expression, relative links under /nav/ | routes and the URLs a routed page resolves |
| route.default | /nav | the default value | id=1 | defaults |
| route.constraint-fail | /nav/abc | a value the constraint refuses | 404 | constraints |
| route.catch-all | /files/a/b c, /files/x.txt | a catch-all {*rest}; one with a file extension | rest=a/b c; the .txt as the original (IIS's static file handler: 404) | which requests reach the routing module |
| route.relative-link | /nav/5 | the relative HyperLink on a routed page | resolved against /nav/ (a 404 as the original) | relative URLs on routed pages |
| route.postback | /nav/5 | a postback on a routed page | the form posts to the route's URL | the form action of a routed page |
| route.transfer | /go/somewhere | a routed page that transfers | U/Plain under /go/somewhere with the route's values | RouteData after Transfer |
| url.pathinfo | /U/Info.aspx/extra/more | PathInfo | PathInfo /extra/more; the form action and relative links under the path info | the path info that the browser takes as folders |
| url.pathinfo-encoded | /U/Info.aspx/a%20b/%E6%97%A5 | escaped path info | decoded as the original | decoding of the path info |
| url.default-document | /U/ | the folder's default document | U/Default.aspx | IIS's DefaultDocument module |
| url.courtesy-redirect | /U | a folder without its trailing slash | 301 to /U/ (IIS's courtesy redirect) | IIS answers before ASP.NET |
| url.rewrite-rebase | /U/Pretty/abc | RewritePath(path) (rebaseClientPath true) in BeginRequest, then a postback | Path / RawUrl / the form action; where the postback goes | the client path the form action is made from |
| url.rewrite-no-rebase | /U/PrettyNoRebase/abc | RewritePath(path, false) in BeginRequest, then a postback | Path / RawUrl / the form action; where the postback goes | the client path the form action is made from |
| url.rewrite-parts | /U/PrettyParts/abc | RewritePath(file, pathInfo, query, false) in BeginRequest, then a postback | Path / RawUrl / the form action; where the postback goes | the client path the form action is made from |
| url.query-repeated | /U/Query.aspx?a=1&a=2&a= | a repeated key | the collection as the original parses it | HttpValueCollection's parsing |
| url.query-empty | /U/Query.aspx?a=&b&=c&&d= | empty values, a key without =, an empty key | the collection as the original parses it | HttpValueCollection's parsing |
| url.query-encoded | /U/Query.aspx?a=x%26y&b=%E6%97%A5&c=a+b&d=%2B&e=%3D | escaped & = + and UTF-8 | the collection as the original parses it | HttpValueCollection's parsing |
| url.query-invalid | /U/Query.aspx?a=%ZZ&b=%u65E5&c=%E6%97 | an invalid escape, IIS's %u, a truncated UTF-8 | the collection as the original parses it | HttpValueCollection's parsing |
| url.case | /u/QUERY.aspx?a=1 | a URL in another case | the same page (Windows' file names) | case-sensitive file systems (Linux) |
| url.double-slash | //U//Query.aspx?a=1 | doubled slashes | as the original | path normalisation |

## navigation

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| nav.sitemap | /N/Deep/Page.aspx (anonymous) | SiteMapPath, Menu and TreeView of Web.sitemap with security trimming; a user control's URLs; the master's | the trimmed nodes (Secure, Admin gone; the roles node kept); URLs relative to the control's and the master's folders | security trimming by URL authorization and roles; TemplateSourceDirectory of controls and masters |
| nav.usercontrol-link | /N/Deep/Page.aspx | the user control's relative HyperLink and the plain link | resolved against N/Controls and N/Deep (404s as the original) | relative URLs in controls of other folders |
| nav.sitemap-admin | /N/Deep/Page.aspx (admin) | the same as the Admin role (done in forms.admin-allowed) | the Admin nodes shown | trimming by role |
| nav.menu | /N/Menu.aspx | Menu items: NavigateUrl, a postback item, a child postback item | the navigation; MenuItemClick with the value path | Menu's postback vs NavigateUrl, value paths with \ |
| nav.treeview | /N/Menu.aspx | TreeView without client script: expand by postback, select, a NavigateUrl node | TreeNodeExpanded, SelectedNodeChanged, the navigation | TreeView's postback events |
| nav.imagemap | /N/ImageMap.aspx | ImageMap hot spots: PostBack and Navigate | the click with its value; the navigation | <area> postbacks |
| nav.client-click | /N/Buttons.aspx | OnClientClick "return false" on a LinkButton and a Button; a confirm (accepted) | no postback; no postback; the postback | the client script before __doPostBack |
| nav.default-button | /N/Buttons.aspx | Enter in a Panel with DefaultButton; Enter in the form (form DefaultButton); DefaultFocus | the panel's button; the form's; the focus | WebForm_FireDefaultButton |
| nav.set-focus | /N/Buttons.aspx | SetFocus(txtAfter) in a click | the focus on txtAfter | WebForm_AutoFocus |
| nav.scroll | /N/Buttons.aspx | MaintainScrollPositionOnPostBack: the button at the bottom | the scroll position restored | __SCROLLPOSITIONY |

## async

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| async.redirect | /Y/Async.aspx?mode=redirect | Async="true", RegisterAsyncTask: redirect after an await | 302; the code after the redirect in the task does not run | navigation from a continuation (another thread; the synchronization context) |
| async.redirect-false | /Y/Async.aspx?mode=redirect-false | Async="true", RegisterAsyncTask: redirect-false after an await | 302 with the page | navigation from a continuation (another thread; the synchronization context) |
| async.transfer | /Y/Async.aspx?mode=transfer | Async="true", RegisterAsyncTask: transfer after an await | the landing under Y/Async.aspx | navigation from a continuation (another thread; the synchronization context) |
| async.throw | /Y/Async.aspx?mode=throw | Async="true", RegisterAsyncTask: throw after an await | the error page of the task's exception | navigation from a continuation (another thread; the synchronization context) |
| async.done | /Y/Async.aspx | Async="true", RegisterAsyncTask: no navigation after an await | the task finished | navigation from a continuation (another thread; the synchronization context) |
| async.void | /Y/AsyncVoid.aspx?mode=redirect | async void Page_Load: Redirect after an await | 302 as the original | an async void event handler (the page waits for it) |
| handler.redirect | /Y/Redirect.ashx | a handler's Response.Redirect | the redirect; a relative URL resolved against Y/ | a handler mapped in web.config (no file); Redirect outside a page |
| handler.redirect-permanent | /Y/Redirect.ashx?mode=permanent | a handler's Response.RedirectPermanent | the redirect; a relative URL resolved against Y/ | a handler mapped in web.config (no file); Redirect outside a page |
| handler.redirect-false | /Y/Redirect.ashx?mode=false | a handler's Response.Redirect(url, false) + Write | the redirect; a relative URL resolved against Y/ | a handler mapped in web.config (no file); Redirect outside a page |
| handler.transfer | /Y/Transfer.ashx | a handler's Server.Transfer to a page and Server.Execute of a page | the landing; the part between the handler's own lines | child requests from a handler |
| download.button | /Y/Download.aspx | a Button that sends a file (Content-Disposition attachment, Response.End), then a postback | the file; the next postback works | the page stays after a download |
| download.linkbutton | /Y/Download.aspx | a LinkButton download (__EVENTTARGET left set), then a Button and a LinkButton | the file; which event the next postbacks raise | __EVENTTARGET still holding the download's target |
| download.unicode | /Y/Download.aspx | filename* (RFC 5987, UTF-8) | the Unicode name | the header as written |

## windows

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| window.blank-link | /Y/Opener.aspx | a HyperLink with Target="_blank" | the popup page in a new window | new windows |
| window.form-target | /Y/Opener.aspx | OnClientClick sets the form's target to _blank; then an ordinary button | the postback in a new window; the next ordinary postback too (the target stays) | a form target left set by script |
| window.open-opener | /Y/Opener.aspx | window.open, a postback in the popup that writes into its opener | the opener's text changed | window.opener after a postback |
| frame.postback | /Y/Frames.aspx | postbacks and a Response.Redirect inside an iframe | only the frame changes | navigation inside frames |
| frame.target | /Y/Frames.aspx | a link of the page with target="f1"; a postback in the new frame page | Inner2 in the frame; its postback | named targets |
| frame.top | /Y/Frames.aspx | a target="_top" link and a postback whose script sets top.location | the whole window navigates | leaving the frame |

## vb

| id | page | element | expected | why it is nasty |
|---|---|---|---|---|
| vb.handles-redirect | /V/Handles.aspx | Handles Me.Load and Handles btnGo.Click (AutoEventWireup false): Response.Redirect | the landing; the Handles events logged | VB's event wiring (WithEvents fields of the generated class) |
| vb.catch-when | /V/Handles.aspx | Redirect in Try ... Catch ex As ThreadAbortException When filter() | the filter and the Catch run, the code after the Try does not (the session) | exception filters run before the stack unwinds; the abort re-raised after the Catch |
| vb.transfer | /V/Handles.aspx | Server.Transfer from a VB handler | the landing under V/Handles.aspx | Transfer in VB |
| vb.my | /V/My.aspx | My.Request / My.Response.Redirect | the path; the redirect | VB's My namespace for the web (_MYTYPE="Web") |
| vb.cross-page | /V/Source.aspx -> /V/Target.aspx | a cross-page postback between VB pages (PreviousPageType, a VB property) | the typed value | PreviousPageType in VB |
