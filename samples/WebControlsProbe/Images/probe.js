// The script a ScriptManagerProxy references (AjaxScenario.aspx): it shows it ran.
window.addEventListener('load', function () {
    var output = document.getElementById('scriptOutput');
    if (output) output.textContent = 'probe.js ran';
});
if (typeof Sys !== 'undefined' && Sys.Application) Sys.Application.notifyScriptLoaded();
