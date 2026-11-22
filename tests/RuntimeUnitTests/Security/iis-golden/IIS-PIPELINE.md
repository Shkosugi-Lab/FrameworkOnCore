# IIS's request pipeline, as its own traces show it

Made by `trace.ps1` from IIS's failed request tracing of the attack tests' cases (1160 cases; IIS Version 10.0 on Microsoft Windows NT 10.0.26200.0). Do not edit: record again.

IIS answers a request by notifying its modules, notification after notification; within a notification, in the order below. The port's steps (UseWebFormsIis's summary) follow this order.

## The modules of each notification, in order

| Notification | Modules |
|---|---|
| PreBeginRequest | IsapiFilterModule |
| BeginRequest | HttpCacheModule → RequestFilteringModule → ConfigurationValidationModule → RewriteModule → FailedRequestsTracingModule → ProtocolSupportModule |
| AuthenticateRequest | IsapiFilterModule → ScriptModule-4.0 → WindowsAuthentication → FormsAuthentication → AnonymousAuthenticationModule → DefaultAuthentication → ServiceModel-4.0 |
| AuthorizeRequest | UrlAuthorization → FileAuthorization |
| ResolveRequestCache | HttpCacheModule → OutputCache → UrlRoutingModule-4.0 |
| MapRequestHandler | StaticCompressionModule → HttpRedirectionModule → ManagedPipelineHandler |
| AcquireRequestState | Session → Profile → ScriptModule-4.0 |
| ExecuteRequestHandler | StaticFileModule → ManagedPipelineHandler → ProtocolSupportModule |
| ReleaseRequestState | StaticCompressionModule → Session → DynamicCompressionModule → RewriteModule |
| UpdateRequestCache | AspNetFilterModule → HttpCacheModule → OutputCache |
| LogRequest | AspNetFilterModule |
| EndRequest | ScriptModule-4.0 → Session → FormsAuthentication → Profile → ManagedPipelineHandler |
| SendResponse | RequestFilteringModule → ProtocolSupportModule → HttpCacheModule → HttpLoggingModule → IsapiFilterModule → DynamicCompressionModule → IsapiModule → RewriteModule → CustomErrorModule |

## Which module sets each status, and when

Every status a module set, in the request or in a child request of it; where several did, the last one's is the answer's.

| Status | Module | Notification | Times | For example |
|---|---|---|---|---|
| 200 | ManagedPipelineHandler | ExecuteRequestHandler | 2 | `GET /status.ashx?v=OK%0D%0AX-Injected:%201` |
| 301 | RewriteModule | BeginRequest | 28 | `GET /go/y.secret%20` |
| 301 | DefaultDocumentModule | ExecuteRequestHandler | 8 | `GET /sub` |
| 302 | FormsAuthentication | EndRequest | 28 | `GET /secure/x.aspx` |
| 302 | HttpRedirectionModule | MapRequestHandler | 10 | `GET /old/x` |
| 302 | ManagedPipelineHandler | ExecuteRequestHandler | 4 | `GET /redirect.ashx?to=//evil.example/` |
| 400.0 | ScriptModule-4.0 | AuthenticateRequest | 14 | `GET /(S(abcdefghijklmnopqrstuvwx))/a.txt.` |
| 400.0 | ScriptModule-4.0 | EndRequest | 6 | `POST /(S(abcdefghijklmnopqrstuvwx))/App_Data/%75sers.xml.` |
| 401.0 | UrlAuthorization | AuthorizeRequest | 28 | `GET /secure/x.aspx` |
| 401.3 | IIS Web Core | AuthenticateRequest | 1 | `GET a file in a folder the server cannot read` |
| 403.0 | ManagedPipelineHandler | ExecuteRequestHandler | 5 | `GET /x.aspq` |
| 403.14 | DirectoryListingModule | ExecuteRequestHandler | 4 | `GET /empty/` |
| 404.0 | IIS Web Core | MapRequestHandler | 157 | `GET /none.txt` |
| 404.0 | ScriptModule-4.0 | AuthenticateRequest | 63 | `GET /SUB/%2e%2e/web.config.` |
| 404.0 | ManagedPipelineHandler | MapRequestHandler | 41 | `GET /none.aspx` |
| 404.0 | ScriptModule-4.0 | EndRequest | 39 | `GET /private/x.txt.` |
| 404.0 | ManagedPipelineHandler | EndRequest | 6 | `GET /go/sub/%20` |
| 404.0 | ManagedPipelineHandler | ExecuteRequestHandler | 1 | `GET /WebResource.axd` |
| 404.11 | RequestFilteringModule | BeginRequest | 7 | `GET /%252e%252e/web.config` |
| 404.14 | RequestFilteringModule | BeginRequest | 2 | `GET a URL of 4,227 characters` |
| 404.15 | RequestFilteringModule | BeginRequest | 2 | `GET a query string of 2,049 characters` |
| 404.17 | StaticFileModule | ExecuteRequestHandler | 1 | `GET /.git/config` |
| 404.3 | StaticFileModule | ExecuteRequestHandler | 11 | `GET /x.asa` |
| 404.5 | RequestFilteringModule | BeginRequest | 44 | `GET /badword/from` |
| 404.7 | RequestFilteringModule | BeginRequest | 111 | `GET /global.asax` |
| 404.8 | RequestFilteringModule | BeginRequest | 273 | `GET /App_Data/users.xml` |
| 405.0 | StaticFileModule | ExecuteRequestHandler | 3 | `an unknown method` |
| 413.1 | RequestFilteringModule | BeginRequest | 6 | `POST 300 KB (over maxAllowedContentLength 200,000)` |
| 500.0 | IIS Web Core | AuthenticateRequest | 36 | `GET /a.txt/` |
| 500.0 | ManagedPipelineHandler | ExecuteRequestHandler | 23 | `GET /trace.axd` |
| 500.0 | ScriptModule-4.0 | AuthenticateRequest | 1 | `GET a folder the server cannot read` |
| 500.50 | RewriteModule | BeginRequest | 1 | `OPTIONS the asterisk form` |
| 501.0 | ProtocolSupportModule | ExecuteRequestHandler | 4 | `TRACE` |

## A status set after another

A later module's status replaces an earlier one's (URL Rewrite's redirect, then ASP.NET's 404 as its modules end the request).

| The statuses, in order | Cases | For example |
|---|---|---|
| UrlAuthorization@AuthorizeRequest=401.0 > FormsAuthentication@EndRequest=302 | 28 | `GET /secure/x.aspx` |
| RequestFilteringModule@BeginRequest=404.8 > ScriptModule-4.0@EndRequest=404.0 | 19 | `GET /private/x.txt.` |
| RequestFilteringModule@BeginRequest=404.8 > ScriptModule-4.0@EndRequest=400.0 | 5 | `POST /(S(abcdefghijklmnopqrstuvwx))/App_Data/%75sers.xml.` |
| RequestFilteringModule@BeginRequest=404.5 > ScriptModule-4.0@EndRequest=404.0 | 5 | `GET /go/badword.txt.` |
| RewriteModule@BeginRequest=301 > ManagedPipelineHandler@EndRequest=404.0 | 5 | `GET /go/sub/%20` |
| RewriteModule@BeginRequest=301 > ScriptModule-4.0@EndRequest=404.0 | 1 | `GET /go/a.txt.` |
| RequestFilteringModule@BeginRequest=404.8 > ManagedPipelineHandler@EndRequest=404.0 | 1 | `GET /go/App_Data/from%20` |
| RequestFilteringModule@BeginRequest=404.5 > ScriptModule-4.0@EndRequest=400.0 | 1 | `GET /(S(abcdefghijklmnopqrstuvwx))/badword.txt.` |

## The handlers the requests were mapped to

The handler is mapped before BeginRequest, for the whole URL as sent and the verb (IIS has no path info for a static file: "/a.txt/x" is an extensionless URL). A child request maps again: URL Rewrite's rewritten URL runs as one, from BeginRequest; the extensionless handler (ASP.NET's TransferRequestHandler) hands a URL no route takes to a child request, which the static file handler answers.

| Handlers, in order | Cases | For example |
|---|---|---|
| StaticFile | 452 | `GET /a.txt`, `GET /sub/a.txt` |
| ExtensionlessUrlHandler-Integrated-4.0 | 266 | `GET /App_Data/`, `GET /App_Data` |
| PageHandlerFactory-Integrated-4.0 | 197 | `GET /default.aspx`, `GET /vs.aspx` |
| SimpleHandlerFactory-Integrated-4.0 | 103 | `GET /echo.ashx?q=1`, `GET /echo.ashx/x/y?q=2` |
| ExtensionlessUrlHandler-Integrated-4.0 > [child] StaticFile | 99 | `GET /empty/`, `GET /.git/config` |
| OPTIONSVerbHandler | 22 | `OPTIONS /APP_DATA/USERS.XML/x.aspx`, `OPTIONS /(S(abcdefghijklmnopqrstuvwx))/echo.ashx` |
| ExtensionlessUrlHandler-Integrated-4.0 > [child] StaticFile > [child] PageHandlerFactory-Integrated-4.0 | 7 | `GET /`, `GET /sub/` |
| TRACEVerbHandler | 6 | `TRACE`, `TRACE a page` |
| TraceHandler-Integrated-4.0 | 1 | `GET /trace.axd` |
| AssemblyResourceLoader-Integrated-4.0 | 1 | `GET /WebResource.axd` |
| rules-Integrated-4.0 | 1 | `GET /x.rules` |
| aspq-Integrated-4.0 | 1 | `GET /x.aspq` |
| cshtm-Integrated-4.0 | 1 | `GET /x.cshtm` |
| cshtml-Integrated-4.0 | 1 | `GET /x.cshtml` |
| vbhtm-Integrated-4.0 | 1 | `GET /x.vbhtm` |
| vbhtml-Integrated-4.0 | 1 | `GET /x.vbhtml` |

## URL Rewrite: when its rules are evaluated

Rules matched in 132 cases. A request a module refused before RewriteModule's BeginRequest has no rule evaluated:

| Refused by | Cases whose URL a rule's pattern would have matched ("/rw/", "/go/", "/to-") | For example |
|---|---|---|
| RequestFilteringModule@BeginRequest=404.8 | 52 | `HEAD /rw/APP_DATA/USERS.XML` |
| RequestFilteringModule@BeginRequest=404.7 | 10 | `OPTIONS /go/x.cs?x=1` |
| RequestFilteringModule@BeginRequest=404.5 | 9 | `POST /rw/%62adword.txt?x=1` |
| RewriteModule@BeginRequest=301 | 3 | `GET /go/a.txt` |

A rewritten URL's child request, refused in its own BeginRequest (the request filtering runs again for it):

| Rule | Child request | Refused by | For example |
|---|---|---|---|
| rw: Rewrite /a.txt. | /a.txt. | ScriptModule-4.0@EndRequest=404.0 | `GET /rw/a.txt.` |
| rw: Rewrite /A.TXT. | /A.TXT. | ScriptModule-4.0@EndRequest=404.0 | `GET /rw/A.TXT.` |
| rw: Rewrite /default.aspx. | /default.aspx. | ScriptModule-4.0@EndRequest=404.0 | `GET /rw/default.aspx.` |
| rw: Rewrite /echo.ashx. | /echo.ashx. | ScriptModule-4.0@EndRequest=404.0 | `GET /rw/echo.ashx.` |
| rw: Rewrite /web.config. | /web.config. | ScriptModule-4.0@EndRequest=404.0 | `GET /rw/web.config.` |
| rw: Rewrite /sub/web.config. | /sub/web.config. | ScriptModule-4.0@EndRequest=404.0 | `GET /rw/sub/web.config.` |
| rw: Rewrite /y.secret. | /y.secret. | ScriptModule-4.0@EndRequest=404.0 | `GET /rw/y.secret.` |
| rw: Rewrite /x.cs. | /x.cs. | ScriptModule-4.0@EndRequest=404.0 | `GET /rw/x.cs.` |
| rw: Rewrite /global.asax. | /global.asax. | ScriptModule-4.0@EndRequest=404.0 | `GET /rw/global.asax.` |
| rw: Rewrite /appsettings.json. | /appsettings.json. | ScriptModule-4.0@EndRequest=404.0 | `GET /rw/appsettings.json.` |
| rw: Rewrite /secure/x.aspx. | /secure/x.aspx. | ScriptModule-4.0@EndRequest=404.0 | `GET /rw/secure/x.aspx.` |
| rw: Rewrite /x.json. | /x.json. | ScriptModule-4.0@EndRequest=404.0 | `GET /rw/x.json.` |
