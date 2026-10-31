# 動作パリティレポート

- 正解の採取元: http://localhost:5096
- 検証対象: http://localhost:5096(linux)

- ✅ index
- ✅ transfer-source
- ✅ transfer-preserve-form
- ✅ transfer-no-form
- ✅ transfer-query
- ✅ transfer-hyperlink-relative
- ✅ transfer-plain-relative
- ✅ transfer-server-anchor
- ✅ transfer-target-postback
- ✅ transfer-handler
- ✅ transfer-ashx
- ✅ transfer-chain
- ✅ transfer-chain-get
- ✅ transfer-chain-postback
- ✅ transfer-after-write
- ✅ transfer-catch-threadabort
- ✅ transfer-catch-threadabort-side
- ✅ transfer-catch-exception
- ✅ transfer-catch-exception-side
- ✅ transfer-request
- ✅ transfer-missing
- ✅ execute-load-raw
- ✅ execute-load
- ✅ execute-render-raw
- ✅ execute-render
- ✅ execute-writer-raw
- ✅ execute-writer
- ✅ execute-ashx-raw
- ✅ execute-ashx
- ✅ execute-redirect-raw
- ✅ execute-redirect
- ✅ execute-redirect-side
- ✅ execute-transfer-raw
- ✅ execute-transfer
- ✅ execute-throw-raw
- ✅ execute-throw
- ✅ execute-missing-raw
- ✅ execute-missing
- ✅ execute-preserve-true
- ✅ execute-preserve-false
- ✅ cross-source
- ✅ cross-same-folder
- ✅ cross-validation-failed
- ✅ cross-no-validation
- ✅ cross-other-folder
- ✅ cross-other-folder-link
- ✅ cross-query
- ✅ cross-linkbutton
- ✅ cross-target-postback
- ✅ cross-target-direct
- ✅ cross-master-source
- ✅ cross-master
- ✅ cross-plain-postback
- ✅ redirect-at-preinit-raw
- ✅ redirect-at-preinit-false-raw
- ✅ redirect-at-preinit
- ✅ redirect-at-init-raw
- ✅ redirect-at-init-false-raw
- ✅ redirect-at-init
- ✅ redirect-at-initcomplete-raw
- ✅ redirect-at-initcomplete-false-raw
- ✅ redirect-at-initcomplete
- ✅ redirect-at-preload-raw
- ✅ redirect-at-preload-false-raw
- ✅ redirect-at-preload
- ✅ redirect-at-load-raw
- ✅ redirect-at-load-false-raw
- ✅ redirect-at-load
- ✅ redirect-at-click-raw
- ✅ redirect-at-click
- ✅ redirect-at-click-false-raw
- ✅ redirect-at-click-false
- ✅ redirect-at-loadcomplete-raw
- ✅ redirect-at-loadcomplete-false-raw
- ✅ redirect-at-loadcomplete
- ✅ redirect-at-prerender-raw
- ✅ redirect-at-prerender-false-raw
- ✅ redirect-at-prerender
- ✅ redirect-at-prerendercomplete-raw
- ✅ redirect-at-prerendercomplete-false-raw
- ✅ redirect-at-prerendercomplete
- ✅ redirect-at-savestatecomplete-raw
- ✅ redirect-at-savestatecomplete-false-raw
- ✅ redirect-at-savestatecomplete
- ✅ redirect-at-render-raw
- ✅ redirect-at-render-false-raw
- ✅ redirect-at-render
- ✅ redirect-at-unload-raw
- ✅ redirect-at-unload-false-raw
- ✅ redirect-at-unload
- ✅ redirect-tae-raw
- ✅ redirect-tae
- ✅ redirect-tae-side
- ✅ redirect-ex-raw
- ✅ redirect-ex
- ✅ redirect-ex-side
- ✅ redirect-finally-raw
- ✅ redirect-finally
- ✅ redirect-finally-side
- ✅ redirect-written-raw
- ✅ redirect-written
- ✅ redirect-flushed-raw
- ✅ redirect-flushed
- ✅ redirect-flushed-side
- ✅ redirect-permanent-raw
- ✅ redirect-permanent
- ✅ redirect-permanent-false-raw
- ✅ redirect-permanent-false
- ✅ redirect-route-raw
- ✅ redirect-route
- ✅ redirect-route-permanent-raw
- ✅ redirect-route-permanent
- ✅ redirect-route-name-raw
- ✅ redirect-route-name
- ✅ redirect-external-raw
- ✅ redirect-external
- ✅ redirect-protocol-relative-raw
- ✅ redirect-protocol-relative
- ✅ redirect-absolute-path-raw
- ✅ redirect-absolute-path
- ✅ redirect-unicode-raw
- ✅ redirect-unicode
- ✅ redirect-encoded-raw
- ✅ redirect-encoded
- ✅ redirect-unicode-path-raw
- ✅ redirect-unicode-path
- ✅ redirect-space-path-raw
- ✅ redirect-space-path
- ✅ redirect-newline-raw
- ✅ redirect-newline
- ✅ redirect-empty-raw
- ✅ redirect-empty
- ✅ redirect-end-raw
- ✅ redirect-end
- ✅ redirect-end-side
- ✅ redirect-complete-raw
- ✅ redirect-complete
- ✅ redirect-complete-side
- ✅ redirect-false-complete-raw
- ✅ redirect-false-complete
- ✅ redirect-status200-raw
- ✅ redirect-status200
- ✅ redirect-status301-raw
- ✅ redirect-status301
- ✅ redirect-status404-raw
- ✅ redirect-status404
- ✅ redirect-twice-raw
- ✅ redirect-twice
- ✅ redirect-clear-headers-raw
- ✅ redirect-clear-headers
- ✅ redirect-relative-parent-raw
- ✅ redirect-relative-parent
- ✅ redirect-relative-file-raw
- ✅ redirect-relative-file
- ✅ redirect-relative-dot-raw
- ✅ redirect-relative-dot
- ✅ redirect-relative-tilde-raw
- ✅ redirect-relative-tilde
- ✅ redirect-relative-query-raw
- 🟡 redirect-relative-query(許容される差: IIS's own detailed error page (403.14 of a folder without a default document, 404.0 of a URL no handler answers; a local request) is a simple page in FrameworkOnCore (the user's decision when httpErrors was made IIS-compatible, README 'httpErrors を IIS 互換で実装した'; wt's not-found-extensionless the same): the same status, substatus and title, the requested URL and physical path; not IIS's 'Most likely causes', 'Things you can try', module, handler, logon lines. The physical path is the site's folder (the original's iis, the conversion's converted, /app in the container).)
  - 本文テキスト 3 行目から相違: 期待 'The Web server is configured to not list the contents of this directory.' / 実際 'Requested URL {root}/R/Sub/?from=query-only'(期待 24 行 / 実際 4 行)
  -   正解側にのみある行: 'The Web server is configured to not list the contents of this directory.' / 'Most likely causes:' / 'A default document is not configured for the requested URL, and directory browsing is not enabled on the server.' / 'Things you can try:' / 'If you do not want to enable directory browsing, ensure that a default document is configured and that the file exists.'
  -   実測側にのみある行: 'Physical Path /app/R/Sub/'
- ✅ redirect-relative-frag-raw
- 🟡 redirect-relative-frag(許容される差: IIS's own detailed error page (403.14 of a folder without a default document, 404.0 of a URL no handler answers; a local request) is a simple page in FrameworkOnCore (the user's decision when httpErrors was made IIS-compatible, README 'httpErrors を IIS 互換で実装した'; wt's not-found-extensionless the same): the same status, substatus and title, the requested URL and physical path; not IIS's 'Most likely causes', 'Things you can try', module, handler, logon lines. The physical path is the site's folder (the original's iis, the conversion's converted, /app in the container).)
  - 本文テキスト 3 行目から相違: 期待 'The Web server is configured to not list the contents of this directory.' / 実際 'Requested URL {root}/R/Sub/'(期待 24 行 / 実際 4 行)
  -   正解側にのみある行: 'The Web server is configured to not list the contents of this directory.' / 'Most likely causes:' / 'A default document is not configured for the requested URL, and directory browsing is not enabled on the server.' / 'Things you can try:' / 'If you do not want to enable directory browsing, ensure that a default document is configured and that the file exists.'
  -   実測側にのみある行: 'Physical Path /app/R/Sub/'
- ✅ redirect-relative-query-frag-raw
- ✅ redirect-relative-query-frag
- ✅ redirect-relative-tilde-frag-raw
- ✅ redirect-relative-tilde-frag
- ✅ redirect-relative-parent-parent-raw
- ✅ redirect-relative-parent-parent
- ✅ global-begin-request-raw
- ✅ global-begin-request
- ✅ global-authenticate-request-raw
- ✅ global-authenticate-request
- ✅ global-post-acquire-state-raw
- ✅ global-post-acquire-state
- ✅ global-end-request-raw
- ✅ global-end-request
- ✅ global-module-redirect-raw
- ✅ global-module-redirect
- ✅ global-module-complete-raw
- ✅ global-module-complete
- ✅ global-error-redirect-raw
- ✅ global-error-redirect
- ✅ global-error-redirect-kept-raw
- ✅ global-error-redirect-kept
- ✅ global-error-transfer-raw
- ✅ global-error-transfer
- ✅ global-error-execute-raw
- ✅ global-error-execute
- ✅ global-error-clear-raw
- ✅ global-error-clear
- ✅ global-redirect-loop-hops
- ✅ global-redirect-loop
- ✅ global-redirect-chain-hops
- ✅ global-redirect-chain-19
- ✅ global-redirect-chain-25
- ✅ error-redirect-mode-raw
- ✅ error-redirect-mode
- ✅ error-redirect-404-raw
- ✅ error-redirect-404
- ✅ error-redirect-403-raw
- ✅ error-redirect-403
- ✅ error-redirect-missing-raw
- ✅ error-redirect-missing
- ✅ error-rewrite-mode-raw
- ✅ error-rewrite-mode
- ✅ error-rewrite-404-raw
- ✅ error-rewrite-404
- ✅ error-rewrite-missing-raw
- ✅ error-rewrite-missing
- ✅ error-error-page-throws-raw
- ✅ error-error-page-throws
- ✅ error-no-default-raw
- 🟡 error-no-default(許容される差: Request.IsLocal (the test's environment): the golden was recorded from IIS's own machine, so customErrors On without a defaultRedirect wrote its local wording ('prevent the details ... from being viewed', set mode to RemoteOnly); the Linux container is reached through Docker's port forwarding, the application sees the client as Docker's gateway and Request.IsLocal is false: the remote wording ('viewed remotely (for security reasons)', set mode to Off). On Windows (a local client) the same page as IIS's. The same cause as wt's error-page on Linux.)
  - 本文テキスト 4 行目から相違: 期待 'Description: An application error occurred on the server. The current custom error settings for this application prevent the details of the application error from being viewed.' / 実際 'Description: An application error occurred on the server. The current custom error settings for this application prevent the details of the application error from being viewed remotely (for security reasons). It could, however, be viewed by browsers running on the local server machine.'(期待 18 行 / 実際 18 行)
  -   正解側にのみある行: 'Description: An application error occurred on the server. The current custom error settings for this application prevent the details of the application error from being viewed.' / 'Details: To enable the details of this specific error message to be viewable on the local server machine, please create a <customErrors> tag within a "web.config" configuration file located in the root directory of the current web application. This <customErrors> tag should then have its "mode" attribute set to "RemoteOnly". To enable the details to be viewable on remote machines, please set "mode" to "Off".' / '<customErrors mode="RemoteOnly"/>' / '<customErrors mode="On" defaultRedirect="mycustompage.htm"/>'
  -   実測側にのみある行: 'Description: An application error occurred on the server. The current custom error settings for this application prevent the details of the application error from being viewed remotely (for security reasons). It could, however, be viewed by browsers running on the local server machine.' / 'Details: To enable the details of this specific error message to be viewable on remote machines, please create a <customErrors> tag within a "web.config" configuration file located in the root directory of the current web application. This <customErrors> tag should then have its "mode" attribute set to "Off".' / '<customErrors mode="Off"/>' / '<customErrors mode="RemoteOnly" defaultRedirect="mycustompage.htm"/>'
- ✅ error-detailed-raw
- ✅ error-detailed
- ✅ error-httpexception-404-raw
- ✅ error-httpexception-404
- ✅ error-missing-page-raw
- ✅ error-missing-page
- ✅ error-compile-raw
- 🟡 error-compile(許容される差: the compiler's language: .NET Framework's ASP.NET ran csc.exe, whose messages are in Windows' display language (Japanese here: 'CS0029: 型 'string' を型 'int' に暗黙的に変換できません。') while the page's own text is the application's uiCulture (en-US); FrameworkOnCore compiles with Roslyn in the process, in the application's culture (English, as the rest of the page). The same error number, the same page, status and title.)
  - 本文テキスト 5 行目から相違: 期待 'Compiler Error Message: CS0029: 型 'string' を型 'int' に暗黙的に変換できません。' / 実際 'Compiler Error Message: CS0029: Cannot implicitly convert type 'string' to 'int''(期待 5 行 / 実際 5 行)
  -   正解側にのみある行: 'Compiler Error Message: CS0029: 型 'string' を型 'int' に暗黙的に変換できません。'
  -   実測側にのみある行: 'Compiler Error Message: CS0029: Cannot implicitly convert type 'string' to 'int''
- ✅ error-parser-raw
- ✅ error-parser
- ✅ error-bad-directive-raw
- ✅ error-bad-directive
- ✅ error-httperrors-replaced-raw
- ✅ error-httperrors-replaced
- ✅ error-httperrors-skip-raw
- ✅ error-httperrors-skip
- ✅ error-httperrors-missing-raw
- ✅ error-httperrors-missing
- ✅ error-async-raw
- ✅ error-async
- ✅ error-asyncnoredirect-raw
- ✅ error-asyncnoredirect
- ✅ forms-protected-raw
- ✅ forms-protected-login
- ✅ forms-return-url-unicode-raw
- ✅ forms-return-url-unicode
- ✅ forms-secure-route-raw
- ✅ forms-secure-route
- ✅ forms-public
- ✅ forms-login-wrong
- ✅ forms-login-return
- ✅ forms-login-cookies
- ✅ forms-admin-denied-raw
- ✅ forms-admin-denied
- ✅ forms-redirect-to-login
- ✅ forms-logout
- ✅ forms-logout-cookies
- ✅ forms-back-after-logout
- ✅ forms-after-logout-reload
- ✅ forms-return-url-external-login
- ✅ forms-return-url-external
- ✅ forms-return-url-scheme-relative
- ✅ forms-default-url
- ✅ forms-persistent-cookies
- ✅ forms-admin-allowed
- ✅ nav-sitemap-admin
- ✅ session-set
- ✅ session-abandoned
- ✅ session-after-abandon
- ✅ postback-counter-1
- ✅ postback-back
- ✅ postback-back-stale
- ✅ postback-double-1
- ✅ postback-double-2
- ✅ postback-double-after
- ✅ postback-no-viewstate
- ✅ postback-validate
- ✅ postback-event-validation
- ✅ postback-no-event-validation
- ✅ postback-unknown-target
- ✅ postback-bad-argument
- ✅ postback-hidden-target
- ✅ postback-tampered
- ✅ postback-dynamic-recreated
- ✅ postback-dynamic-lost
- ✅ postback-dynamic-static
- ✅ postback-viewstate-order
- ✅ postback-tree-changed
- ✅ postback-chunked
- ✅ postback-chunked-post
- ✅ postback-encrypted
- ✅ postback-encrypted-post
- ✅ postback-get-viewstate
- ✅ wizard-start
- ✅ wizard-validation
- ✅ wizard-step-2
- ✅ wizard-step-3
- ✅ wizard-sidebar-back
- ✅ wizard-step-2-again
- ✅ wizard-sidebar
- ✅ wizard-confirm
- ✅ wizard-finish
- ✅ wizard-jump
- ✅ wizard-cancel
- ✅ wizard-dest-finish-step
- ✅ wizard-finish-destination
- ✅ multiview-1
- ✅ multiview-2
- ✅ multiview-inner-2
- ✅ multiview-inner-1
- ✅ multiview-prev
- ✅ multiview-3
- ✅ multiview-by-index
- ✅ multiview-set-active
- ✅ wizard-ajax-b
- ✅ wizard-ajax-c
- ✅ wizard-ajax-finish
- ✅ ajax-panel
- ✅ ajax-update-raw
- ✅ ajax-update
- ✅ ajax-trigger-outside
- ✅ ajax-postback-trigger
- ✅ ajax-outside-full
- ✅ ajax-redirect-raw
- ✅ ajax-redirect
- ✅ ajax-transfer-raw
- ✅ ajax-transfer
- ✅ ajax-error-raw
- ✅ ajax-error
- ✅ ajax-timeout
- ✅ ajax-write-raw
- ✅ ajax-write
- ✅ ajax-end-raw
- ✅ ajax-end
- ✅ ajax-nested
- ✅ ajax-nested-inner
- ✅ ajax-nested-outer
- ✅ ajax-nested-update3
- ✅ ajax-nested-three
- ✅ ajax-history-2
- ✅ ajax-history-3
- ✅ ajax-history-back
- ✅ ajax-history-back-2
- ✅ ajax-history-forward
- 🟡 ajax-timer(許容される差: compilation debug="true": .NET Framework serves the debug ASP.NET AJAX scripts (MicrosoftAjax.debug.js: Sys.Debug.isDebug true), whose script loader leaves the startup scripts of each async postback in the head (Sys._ScriptLoaderTask._clearScript removes them only when not debugging); FrameworkOnCore's System.Web.Extensions has the release scripts only (its .debug.js are copies of the release ones), which remove them once run. The same deltas from the server (compared with a client of their own: every async postback's panel, hidden fields and the Timer's $create with enabled false after the third tick), the same ticks and page; only the executed script elements left in the document differ.)
  - HTML 5 行目から相違: 期待 '<script type="text/javascript">Sys.Application.add_init(function() {' / 実際 '</head>'(期待 96 行 / 実際 84 行)
  -   正解側にのみある行: '<script type="text/javascript">Sys.Application.add_init(function() {' / '$create(Sys.UI._Timer, {"enabled":false,"interval":600,"uniqueID":"tm"}, null, null, $get("tm"));'
- ✅ ajax-page-method
- ✅ ajax-page-method-fails
- ✅ ajax-page-method-redirects
- ✅ ajax-page-method-raw
- ✅ ajax-script-service
- ✅ ajax-script-service-proxy
- ✅ route-constraint
- ✅ route-default
- ✅ route-constraint-fail-raw
- 🟡 route-constraint-fail(許容される差: IIS's own detailed error page (403.14 of a folder without a default document, 404.0 of a URL no handler answers; a local request) is a simple page in FrameworkOnCore (the user's decision when httpErrors was made IIS-compatible, README 'httpErrors を IIS 互換で実装した'; wt's not-found-extensionless the same): the same status, substatus and title, the requested URL and physical path; not IIS's 'Most likely causes', 'Things you can try', module, handler, logon lines. The physical path is the site's folder (the original's iis, the conversion's converted, /app in the container).)
  - 本文テキスト 3 行目から相違: 期待 'The resource you are looking for has been removed, had its name changed, or is temporarily unavailable.' / 実際 'Requested URL {root}/nav/abc'(期待 23 行 / 実際 4 行)
  -   正解側にのみある行: 'The resource you are looking for has been removed, had its name changed, or is temporarily unavailable.' / 'Most likely causes:' / 'The directory or file specified does not exist on the Web server.' / 'The URL contains a typographical error.' / 'A custom filter or module, such as URLScan, restricts access to the file.'
  -   実測側にのみある行: 'Physical Path /app/nav/abc'
- ✅ route-catch-all
- ✅ route-catch-all-postback
- ✅ route-catch-all-extension
- ✅ route-relative-link
- ✅ route-postback
- ✅ route-transfer
- ✅ url-pathinfo
- ✅ url-pathinfo-postback
- ✅ url-pathinfo-link
- ✅ url-pathinfo-encoded
- ✅ url-default-document
- ✅ url-courtesy-redirect-raw
- ✅ url-courtesy-redirect
- ✅ url-rewrite-rebase
- ✅ url-rewrite-rebase-postback
- ✅ url-rewrite-no-rebase
- ✅ url-rewrite-no-rebase-postback
- ✅ url-rewrite-parts
- ✅ url-rewrite-parts-postback
- ✅ url-query-repeated
- ✅ url-query-empty
- ✅ url-query-encoded
- ✅ url-query-invalid
- ✅ url-case
- ✅ url-double-slash
- ✅ nav-sitemap
- ✅ nav-usercontrol-link
- ✅ nav-usercontrol-plain
- ✅ nav-menu
- ✅ nav-menu-postback
- ✅ nav-menu-child
- ✅ nav-menu-navigate
- ✅ nav-treeview-expand
- ✅ nav-treeview-select
- ✅ nav-treeview-navigate
- ✅ nav-treeview-root-text
- ✅ nav-imagemap
- ✅ nav-imagemap-postback
- ✅ nav-imagemap-navigate
- ✅ nav-buttons
- ✅ nav-linkbutton-false
- ✅ nav-button-false
- ✅ nav-linkbutton-confirm
- ✅ nav-panel-default-button
- ✅ nav-form-default-button
- ✅ nav-set-focus
- ✅ nav-scroll
- ✅ async-redirect-raw
- ✅ async-redirect
- ✅ async-redirect-side
- ✅ async-redirect-false-raw
- ✅ async-redirect-false
- ✅ async-transfer-raw
- ✅ async-transfer
- ✅ async-throw-raw
- ✅ async-throw
- ✅ async-done-raw
- ✅ async-done
- ✅ async-void-raw
- ✅ async-void
- ✅ async-void-plain
- ✅ handler-redirect-raw
- ✅ handler-redirect
- ✅ handler-redirect-permanent-raw
- ✅ handler-redirect-permanent
- ✅ handler-redirect-false-raw
- ✅ handler-redirect-false
- ✅ handler-transfer
- ✅ handler-execute
- ✅ download-button
- ✅ download-button-after
- ✅ download-linkbutton
- ✅ download-linkbutton-button
- ✅ download-linkbutton-2
- ✅ download-linkbutton-link
- ✅ download-unicode
- ✅ window-blank-link
- ✅ window-form-target-popup
- ✅ window-form-target-main
- ✅ window-form-target-sticky
- ✅ window-open
- ✅ window-open-postback
- ✅ window-opener-told
- ✅ frame-page
- ✅ frame-inner
- ✅ frame-postback
- ✅ frame-redirect
- ✅ frame-page-after
- ✅ frame-target
- ✅ frame-target-postback
- ✅ frame-top-link
- ✅ frame-top-script
- ✅ vb-handles
- ✅ vb-handles-redirect
- ✅ vb-catch-when
- ✅ vb-catch-when-side
- ✅ vb-transfer
- ✅ vb-my
- ✅ vb-my-redirect-raw
- ✅ vb-my-redirect
- ✅ vb-cross-page
- ✅ リンク 284 件

## 操作ごとの結果

| 操作 | 分類 | 結果 | スナップショット | 備考 |
|---|---|---|---|---|
| index.home | index | same | index |  |
| transfer.preserve-form | transfer | same | transfer-source, transfer-preserve-form |  |
| transfer.no-form | transfer | same | transfer-no-form |  |
| transfer.query | transfer | same | transfer-query |  |
| transfer.relative-links | transfer | same | transfer-hyperlink-relative, transfer-plain-relative, transfer-server-anchor |  |
| transfer.target-postback | transfer | same | transfer-target-postback |  |
| transfer.handler | transfer | same | transfer-handler |  |
| transfer.ashx | transfer | same | transfer-ashx |  |
| transfer.chain | transfer | same | transfer-chain, transfer-chain-get, transfer-chain-postback |  |
| transfer.after-write | transfer | same | transfer-after-write |  |
| transfer.catch-threadabort | transfer | same | transfer-catch-threadabort, transfer-catch-threadabort-side |  |
| transfer.catch-exception | transfer | same | transfer-catch-exception, transfer-catch-exception-side |  |
| transfer.transfer-request | transfer | same | transfer-request |  |
| transfer.missing | transfer | same | transfer-missing |  |
| execute.load | transfer | same | execute-load-raw, execute-load |  |
| execute.render | transfer | same | execute-render-raw, execute-render |  |
| execute.writer | transfer | same | execute-writer-raw, execute-writer |  |
| execute.ashx | transfer | same | execute-ashx-raw, execute-ashx |  |
| execute.redirect | transfer | same | execute-redirect-raw, execute-redirect, execute-redirect-side |  |
| execute.transfer | transfer | same | execute-transfer-raw, execute-transfer |  |
| execute.throw | transfer | same | execute-throw-raw, execute-throw |  |
| execute.missing | transfer | same | execute-missing-raw, execute-missing |  |
| execute.preserve-form | transfer | same | execute-preserve-true, execute-preserve-false |  |
| cross.same-folder | cross-page | same | cross-source, cross-same-folder |  |
| cross.validation | cross-page | same | cross-validation-failed |  |
| cross.no-validation | cross-page | same | cross-no-validation |  |
| cross.other-folder | cross-page | same | cross-other-folder, cross-other-folder-link |  |
| cross.query | cross-page | same | cross-query |  |
| cross.linkbutton | cross-page | same | cross-linkbutton |  |
| cross.target-postback | cross-page | same | cross-target-postback |  |
| cross.target-direct | cross-page | same | cross-target-direct |  |
| cross.master | cross-page | same | cross-master-source, cross-master |  |
| cross.plain-postback | cross-page | same | cross-plain-postback |  |
| redirect.at-preinit | redirect | same | redirect-at-preinit-raw, redirect-at-preinit-false-raw, redirect-at-preinit |  |
| redirect.at-init | redirect | same | redirect-at-init-raw, redirect-at-init-false-raw, redirect-at-init |  |
| redirect.at-initcomplete | redirect | same | redirect-at-initcomplete-raw, redirect-at-initcomplete-false-raw, redirect-at-initcomplete |  |
| redirect.at-preload | redirect | same | redirect-at-preload-raw, redirect-at-preload-false-raw, redirect-at-preload |  |
| redirect.at-load | redirect | same | redirect-at-load-raw, redirect-at-load-false-raw, redirect-at-load |  |
| redirect.at-click | redirect | same | redirect-at-click-raw, redirect-at-click, redirect-at-click-false-raw, redirect-at-click-false |  |
| redirect.at-loadcomplete | redirect | same | redirect-at-loadcomplete-raw, redirect-at-loadcomplete-false-raw, redirect-at-loadcomplete |  |
| redirect.at-prerender | redirect | same | redirect-at-prerender-raw, redirect-at-prerender-false-raw, redirect-at-prerender |  |
| redirect.at-prerendercomplete | redirect | same | redirect-at-prerendercomplete-raw, redirect-at-prerendercomplete-false-raw, redirect-at-prerendercomplete |  |
| redirect.at-savestatecomplete | redirect | same | redirect-at-savestatecomplete-raw, redirect-at-savestatecomplete-false-raw, redirect-at-savestatecomplete |  |
| redirect.at-render | redirect | same | redirect-at-render-raw, redirect-at-render-false-raw, redirect-at-render |  |
| redirect.at-unload | redirect | same | redirect-at-unload-raw, redirect-at-unload-false-raw, redirect-at-unload |  |
| redirect.tae | redirect | same | redirect-tae-raw, redirect-tae, redirect-tae-side |  |
| redirect.ex | redirect | same | redirect-ex-raw, redirect-ex, redirect-ex-side |  |
| redirect.finally | redirect | same | redirect-finally-raw, redirect-finally, redirect-finally-side |  |
| redirect.written | redirect | same | redirect-written-raw, redirect-written |  |
| redirect.flushed | redirect | same | redirect-flushed-raw, redirect-flushed, redirect-flushed-side |  |
| redirect.permanent | redirect | same | redirect-permanent-raw, redirect-permanent |  |
| redirect.permanent-false | redirect | same | redirect-permanent-false-raw, redirect-permanent-false |  |
| redirect.route | redirect | same | redirect-route-raw, redirect-route |  |
| redirect.route-permanent | redirect | same | redirect-route-permanent-raw, redirect-route-permanent |  |
| redirect.route-name | redirect | same | redirect-route-name-raw, redirect-route-name |  |
| redirect.external | redirect | same | redirect-external-raw, redirect-external |  |
| redirect.protocol-relative | redirect | same | redirect-protocol-relative-raw, redirect-protocol-relative |  |
| redirect.absolute-path | redirect | same | redirect-absolute-path-raw, redirect-absolute-path |  |
| redirect.unicode | redirect | same | redirect-unicode-raw, redirect-unicode |  |
| redirect.encoded | redirect | same | redirect-encoded-raw, redirect-encoded |  |
| redirect.unicode-path | redirect | same | redirect-unicode-path-raw, redirect-unicode-path |  |
| redirect.space-path | redirect | same | redirect-space-path-raw, redirect-space-path |  |
| redirect.newline | redirect | same | redirect-newline-raw, redirect-newline |  |
| redirect.empty | redirect | same | redirect-empty-raw, redirect-empty |  |
| redirect.end | redirect | same | redirect-end-raw, redirect-end, redirect-end-side |  |
| redirect.complete | redirect | same | redirect-complete-raw, redirect-complete, redirect-complete-side |  |
| redirect.false-complete | redirect | same | redirect-false-complete-raw, redirect-false-complete |  |
| redirect.status200 | redirect | same | redirect-status200-raw, redirect-status200 |  |
| redirect.status301 | redirect | same | redirect-status301-raw, redirect-status301 |  |
| redirect.status404 | redirect | same | redirect-status404-raw, redirect-status404 |  |
| redirect.twice | redirect | same | redirect-twice-raw, redirect-twice |  |
| redirect.clear-headers | redirect | same | redirect-clear-headers-raw, redirect-clear-headers |  |
| redirect.relative-parent | redirect | same | redirect-relative-parent-raw, redirect-relative-parent |  |
| redirect.relative-file | redirect | same | redirect-relative-file-raw, redirect-relative-file |  |
| redirect.relative-dot | redirect | same | redirect-relative-dot-raw, redirect-relative-dot |  |
| redirect.relative-tilde | redirect | same | redirect-relative-tilde-raw, redirect-relative-tilde |  |
| redirect.relative-query | redirect | allowed | redirect-relative-query-raw, redirect-relative-query |  |
| redirect.relative-frag | redirect | allowed | redirect-relative-frag-raw, redirect-relative-frag |  |
| redirect.relative-query-frag | redirect | same | redirect-relative-query-frag-raw, redirect-relative-query-frag |  |
| redirect.relative-tilde-frag | redirect | same | redirect-relative-tilde-frag-raw, redirect-relative-tilde-frag |  |
| redirect.relative-parent-parent | redirect | same | redirect-relative-parent-parent-raw, redirect-relative-parent-parent |  |
| global.begin-request | global | same | global-begin-request-raw, global-begin-request |  |
| global.authenticate-request | global | same | global-authenticate-request-raw, global-authenticate-request |  |
| global.post-acquire-state | global | same | global-post-acquire-state-raw, global-post-acquire-state |  |
| global.end-request | global | same | global-end-request-raw, global-end-request |  |
| global.module-redirect | global | same | global-module-redirect-raw, global-module-redirect |  |
| global.module-complete | global | same | global-module-complete-raw, global-module-complete |  |
| global.error-redirect | global | same | global-error-redirect-raw, global-error-redirect |  |
| global.error-redirect-kept | global | same | global-error-redirect-kept-raw, global-error-redirect-kept |  |
| global.error-transfer | global | same | global-error-transfer-raw, global-error-transfer |  |
| global.error-execute | global | same | global-error-execute-raw, global-error-execute |  |
| global.error-clear | global | same | global-error-clear-raw, global-error-clear |  |
| global.redirect-loop | global | same | global-redirect-loop-hops, global-redirect-loop |  |
| global.redirect-chain | global | same | global-redirect-chain-hops, global-redirect-chain-19, global-redirect-chain-25 |  |
| error.redirect-mode | error | same | error-redirect-mode-raw, error-redirect-mode |  |
| error.redirect-404 | error | same | error-redirect-404-raw, error-redirect-404 |  |
| error.redirect-403 | error | same | error-redirect-403-raw, error-redirect-403 |  |
| error.redirect-missing | error | same | error-redirect-missing-raw, error-redirect-missing |  |
| error.rewrite-mode | error | same | error-rewrite-mode-raw, error-rewrite-mode |  |
| error.rewrite-404 | error | same | error-rewrite-404-raw, error-rewrite-404 |  |
| error.rewrite-missing | error | same | error-rewrite-missing-raw, error-rewrite-missing |  |
| error.error-page-throws | error | same | error-error-page-throws-raw, error-error-page-throws |  |
| error.no-default | error | allowed | error-no-default-raw, error-no-default |  |
| error.detailed | error | same | error-detailed-raw, error-detailed |  |
| error.httpexception-404 | error | same | error-httpexception-404-raw, error-httpexception-404 |  |
| error.missing-page | error | same | error-missing-page-raw, error-missing-page |  |
| error.compile | error | allowed | error-compile-raw, error-compile |  |
| error.parser | error | same | error-parser-raw, error-parser |  |
| error.bad-directive | error | same | error-bad-directive-raw, error-bad-directive |  |
| error.httperrors-replaced | error | same | error-httperrors-replaced-raw, error-httperrors-replaced |  |
| error.httperrors-skip | error | same | error-httperrors-skip-raw, error-httperrors-skip |  |
| error.httperrors-missing | error | same | error-httperrors-missing-raw, error-httperrors-missing |  |
| error.async | error | same | error-async-raw, error-async |  |
| error.asyncnoredirect | error | same | error-asyncnoredirect-raw, error-asyncnoredirect |  |
| forms.protected-redirect | forms | same | forms-protected-raw, forms-protected-login |  |
| forms.return-url-query-unicode | forms | same | forms-return-url-unicode-raw, forms-return-url-unicode |  |
| forms.secure-route | forms | same | forms-secure-route-raw, forms-secure-route |  |
| forms.public-page | forms | same | forms-public |  |
| forms.login-wrong | forms | same | forms-login-wrong |  |
| forms.login-return | forms | same | forms-login-return, forms-login-cookies |  |
| forms.admin-denied | forms | same | forms-admin-denied-raw, forms-admin-denied |  |
| forms.redirect-to-login | forms | same | forms-redirect-to-login |  |
| forms.logout | forms | same | forms-logout, forms-logout-cookies |  |
| forms.back-after-logout | forms | same | forms-back-after-logout, forms-after-logout-reload |  |
| forms.return-url-external | forms | same | forms-return-url-external-login, forms-return-url-external |  |
| forms.return-url-scheme-relative | forms | same | forms-return-url-scheme-relative |  |
| forms.default-url | forms | same | forms-default-url, forms-persistent-cookies |  |
| forms.admin-allowed | forms | same | forms-admin-allowed, nav-sitemap-admin |  |
| session.abandon | forms | same | session-set, session-abandoned, session-after-abandon |  |
| session.cookieless | forms | not runnable |  | sessionState cookieless is per application (MachineToApplication): it needs a second application in a subfolder; sample-parity.ps1 hosts one application at the site root (and the converter converts one application). A sample of its own would be needed. |
| postback.back-stale | postback | same | postback-counter-1, postback-back, postback-back-stale |  |
| postback.double-submit | postback | same | postback-double-1, postback-double-2, postback-double-after |  |
| postback.no-viewstate | postback | same | postback-no-viewstate |  |
| postback.event-validation | postback | same | postback-validate, postback-event-validation |  |
| postback.no-event-validation | postback | same | postback-no-event-validation |  |
| postback.unknown-target | postback | same | postback-unknown-target |  |
| postback.bad-argument | postback | same | postback-bad-argument |  |
| postback.hidden-target | postback | same | postback-hidden-target |  |
| postback.tampered-viewstate | postback | same | postback-tampered |  |
| postback.dynamic-recreated | postback | same | postback-dynamic-recreated |  |
| postback.dynamic-lost | postback | same | postback-dynamic-lost, postback-dynamic-static |  |
| postback.viewstate-order | postback | same | postback-viewstate-order |  |
| postback.tree-changed | postback | same | postback-tree-changed |  |
| postback.chunked-viewstate | postback | same | postback-chunked, postback-chunked-post |  |
| postback.encrypted-viewstate | postback | same | postback-encrypted, postback-encrypted-post |  |
| postback.get-viewstate | postback | same | postback-get-viewstate |  |
| wizard.validation | wizard | same | wizard-start, wizard-validation |  |
| wizard.next | wizard | same | wizard-step-2, wizard-step-3 |  |
| wizard.previous | wizard | same | wizard-sidebar-back, wizard-step-2-again |  |
| wizard.sidebar | wizard | same | wizard-sidebar |  |
| wizard.finish | wizard | same | wizard-confirm, wizard-finish |  |
| wizard.jump | wizard | same | wizard-jump |  |
| wizard.cancel | wizard | same | wizard-cancel |  |
| wizard.finish-destination | wizard | same | wizard-dest-finish-step, wizard-finish-destination |  |
| multiview.commands | wizard | same | multiview-1, multiview-2, multiview-inner-2, multiview-inner-1, multiview-prev, multiview-3, multiview-by-index |  |
| multiview.set-active | wizard | same | multiview-set-active |  |
| wizard.ajax | wizard | same | wizard-ajax-b, wizard-ajax-c, wizard-ajax-finish |  |
| ajax.update | ajax | same | ajax-panel, ajax-update-raw, ajax-update |  |
| ajax.trigger-outside | ajax | same | ajax-trigger-outside |  |
| ajax.postback-trigger | ajax | same | ajax-postback-trigger |  |
| ajax.outside-full | ajax | same | ajax-outside-full |  |
| ajax.redirect | ajax | same | ajax-redirect-raw, ajax-redirect |  |
| ajax.transfer | ajax | same | ajax-transfer-raw, ajax-transfer |  |
| ajax.error | ajax | same | ajax-error-raw, ajax-error |  |
| ajax.timeout | ajax | same | ajax-timeout |  |
| ajax.response-write | ajax | same | ajax-write-raw, ajax-write |  |
| ajax.response-end | ajax | same | ajax-end-raw, ajax-end |  |
| ajax.nested | ajax | same | ajax-nested, ajax-nested-inner, ajax-nested-outer, ajax-nested-update3, ajax-nested-three |  |
| ajax.history | ajax | same | ajax-history-2, ajax-history-3, ajax-history-back, ajax-history-back-2, ajax-history-forward |  |
| ajax.timer | ajax | allowed | ajax-timer |  |
| ajax.page-methods | ajax | same | ajax-page-method, ajax-page-method-fails, ajax-page-method-redirects, ajax-page-method-raw |  |
| ajax.script-service | ajax | same | ajax-script-service, ajax-script-service-proxy |  |
| route.constraint | routing | same | route-constraint |  |
| route.default | routing | same | route-default |  |
| route.constraint-fail | routing | allowed | route-constraint-fail-raw, route-constraint-fail |  |
| route.catch-all | routing | same | route-catch-all, route-catch-all-postback, route-catch-all-extension |  |
| route.relative-link | routing | same | route-relative-link |  |
| route.postback | routing | same | route-postback |  |
| route.transfer | routing | same | route-transfer |  |
| url.pathinfo | routing | same | url-pathinfo, url-pathinfo-postback, url-pathinfo-link |  |
| url.pathinfo-encoded | routing | same | url-pathinfo-encoded |  |
| url.default-document | routing | same | url-default-document |  |
| url.courtesy-redirect | routing | same | url-courtesy-redirect-raw, url-courtesy-redirect |  |
| url.rewrite-rebase | routing | same | url-rewrite-rebase, url-rewrite-rebase-postback |  |
| url.rewrite-no-rebase | routing | same | url-rewrite-no-rebase, url-rewrite-no-rebase-postback |  |
| url.rewrite-parts | routing | same | url-rewrite-parts, url-rewrite-parts-postback |  |
| url.query-repeated | routing | same | url-query-repeated |  |
| url.query-empty | routing | same | url-query-empty |  |
| url.query-encoded | routing | same | url-query-encoded |  |
| url.query-invalid | routing | same | url-query-invalid |  |
| url.case | routing | same | url-case |  |
| url.double-slash | routing | same | url-double-slash |  |
| nav.sitemap | navigation | same | nav-sitemap |  |
| nav.usercontrol-link | navigation | same | nav-usercontrol-link, nav-usercontrol-plain |  |
| nav.sitemap-admin | navigation | same | nav-sitemap-admin |  |
| nav.menu | navigation | same | nav-menu, nav-menu-postback, nav-menu-child, nav-menu-navigate |  |
| nav.treeview | navigation | same | nav-treeview-expand, nav-treeview-select, nav-treeview-navigate, nav-treeview-root-text |  |
| nav.imagemap | navigation | same | nav-imagemap, nav-imagemap-postback, nav-imagemap-navigate |  |
| nav.client-click | navigation | same | nav-buttons, nav-linkbutton-false, nav-button-false, nav-linkbutton-confirm |  |
| nav.default-button | navigation | same | nav-panel-default-button, nav-form-default-button |  |
| nav.set-focus | navigation | same | nav-set-focus |  |
| nav.scroll | navigation | same | nav-scroll |  |
| async.redirect | async | same | async-redirect-raw, async-redirect, async-redirect-side |  |
| async.redirect-false | async | same | async-redirect-false-raw, async-redirect-false |  |
| async.transfer | async | same | async-transfer-raw, async-transfer |  |
| async.throw | async | same | async-throw-raw, async-throw |  |
| async.done | async | same | async-done-raw, async-done |  |
| async.void | async | same | async-void-raw, async-void, async-void-plain |  |
| handler.redirect | async | same | handler-redirect-raw, handler-redirect |  |
| handler.redirect-permanent | async | same | handler-redirect-permanent-raw, handler-redirect-permanent |  |
| handler.redirect-false | async | same | handler-redirect-false-raw, handler-redirect-false |  |
| handler.transfer | async | same | handler-transfer, handler-execute |  |
| download.button | async | same | download-button, download-button-after |  |
| download.linkbutton | async | same | download-linkbutton, download-linkbutton-button, download-linkbutton-2, download-linkbutton-link |  |
| download.unicode | async | same | download-unicode |  |
| window.blank-link | windows | same | window-blank-link, window-form-target-popup |  |
| window.form-target | windows | same | window-form-target-popup, window-form-target-main, window-form-target-sticky, window-open |  |
| window.open-opener | windows | same | window-open, window-open-postback, window-opener-told |  |
| frame.postback | windows | same | frame-page, frame-inner, frame-postback, frame-redirect, frame-page-after |  |
| frame.target | windows | same | frame-target, frame-target-postback, frame-top-link |  |
| frame.top | windows | same | frame-top-link, frame-top-script |  |
| vb.handles-redirect | vb | same | vb-handles, vb-handles-redirect |  |
| vb.catch-when | vb | same | vb-catch-when, vb-catch-when-side |  |
| vb.transfer | vb | same | vb-transfer |  |
| vb.my | vb | same | vb-my, vb-my-redirect-raw, vb-my-redirect |  |
| vb.cross-page | vb | same | vb-cross-page |  |

**結果: 一致(449 スナップショット、許容される差 6、リンク 284 件)**
