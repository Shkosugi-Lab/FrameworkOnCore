<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Ajax.aspx.cs" Inherits="WcfProbe.AjaxPage" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>WCF AJAX probe</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scripts" runat="server">
            <Services>
                <asp:ServiceReference Path="~/Ajax.svc" />
            </Services>
        </asp:ScriptManager>
        <h1>Ajax.svc through ScriptManager (enableWebScript)</h1>
        <p>Hello (GET): <span id="hello"></span></p>
        <p>Lookup(3): <span id="lookup"></span></p>
        <p>Cheaper(3): <span id="cheaper"></span></p>
        <p>When(2026): <span id="when"></span></p>
        <p>Nothing: <span id="nothing"></span></p>
        <p>Fail: <span id="fail"></span></p>
        <h2>On the wire</h2>
        <p>POST Lookup: <span id="rawLookup"></span></p>
        <p>GET Hello: <span id="rawHello"></span></p>
        <p>POST Fail: <span id="rawFail"></span></p>
    </form>
    <script type="text/javascript">
        function show(id, text) { document.getElementById(id).appendChild(document.createTextNode(text)); }
        function failed(id) { return function (error) { show(id, "failed: " + error.get_message() + " (" + error.get_statusCode() + ", " + error.get_exceptionType() + ")"); }; }
        // The answer as it is; a fault's parts (its stack trace is the server's paths and lines).
        function raw(id, method, url, body) {
            var request = new XMLHttpRequest();
            request.open(method, url, true);
            if (body) request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            request.onreadystatechange = function () {
                if (request.readyState !== 4) return;
                var text = request.responseText;
                if (request.getResponseHeader("jsonerror")) {
                    var fault = JSON.parse(text);
                    text = "Message=" + fault.Message + "; ExceptionType=" + fault.ExceptionType + "; StackTrace=" + (fault.StackTrace ? "yes" : "no")
                        + "; ExceptionDetail.Type=" + (fault.ExceptionDetail && fault.ExceptionDetail.Type) + "; ExceptionDetail.Message=" + (fault.ExceptionDetail && fault.ExceptionDetail.Message)
                        + "; keys=" + Object.keys(fault).join(",") + "; detail keys=" + (fault.ExceptionDetail ? Object.keys(fault.ExceptionDetail).join(",") : "");
                }
                show(id, request.status + " " + (request.getResponseHeader("jsonerror") || "-") + " " + (request.getResponseHeader("Content-Type") || "") + " " + text);
            };
            request.send(body || null);
        }
        function pageLoad() {
            Ajax.Hello("Taro", function (result) { show("hello", result); }, failed("hello"));
            Ajax.Lookup(3, function (item) { show("lookup", item.Id + " " + item.Name + " " + item.Price + " " + item.InStock + " " + (item.__type || "")); }, failed("lookup"));
            Ajax.Cheaper(3, function (items) { show("cheaper", items.length + ": " + items.map(function (i) { return i.Name + " " + i.Price; }).join(", ")); }, failed("cheaper"));
            Ajax.When(2026, function (date) { show("when", date.toISOString()); }, failed("when"));
            Ajax.Nothing(function (result) { show("nothing", "done " + result); }, failed("nothing"));
            Ajax.Fail("on purpose", function (result) { show("fail", "no failure " + result); }, failed("fail"));
            raw("rawLookup", "POST", "Ajax.svc/Lookup", "{\"id\":2}");
            raw("rawHello", "GET", "Ajax.svc/Hello?name=%22Hanako%22");
            raw("rawFail", "POST", "Ajax.svc/Fail", "{\"why\":\"wire\"}");
        }
    </script>
</body>
</html>
