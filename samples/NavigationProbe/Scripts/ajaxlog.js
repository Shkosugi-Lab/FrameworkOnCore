// The page request manager's outcome, in the page: each page load counted, an async postback's error shown (and handled:
// no alert), the browser's history point.
Sys.Application.add_init(function () {
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    var loads = 0;
    prm.add_pageLoaded(function () {
        loads++;
        var l = document.getElementById('loads');
        if (l) l.innerHTML = 'page loads: ' + loads;
    });
    prm.add_endRequest(function (sender, args) {
        var error = args.get_error();
        if (error) {
            var t = document.getElementById('ajaxerr');
            if (t) t.innerHTML = ('error: ' + error.name + ': ' + error.message).replace(/</g, '&lt;');
            args.set_errorHandled(true);
        }
    });
});
