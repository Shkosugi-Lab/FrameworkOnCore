<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Methods.aspx.cs" Inherits="ServicesProbe.Methods" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Services probe: from the page's scripts</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="sm" runat="server" EnablePageMethods="true">
            <Services>
                <asp:ServiceReference Path="~/ProbeService.asmx" />
            </Services>
        </asp:ScriptManager>
        <pre id="out">calling</pre>
    </form>
    <script>
        // A page method (PageMethods) and the service's proxy (ServiceReference), as a page's scripts call them; the results
        // written in order.
        var results = [];
        function done(text) { results.push(text); document.getElementById('out').textContent = results.join('\n'); }
        function failed(error) { done('error: ' + error.get_exceptionType() + ': ' + error.get_message() + ' (' + error.get_statusCode() + ')'); }
        Sys.Application.add_load(function () {
            PageMethods.Echo('from PageMethods', function (r) {
                done('PageMethods.Echo: ' + r);
                ServicesProbe.ProbeService.Hello('proxy', function (r2) {
                    done('proxy Hello: ' + r2);
                    ServicesProbe.ProbeService.Items(2, function (items) {
                        done('proxy Items: ' + items.map(function (i) { return i.Name + ' ' + i.Price + ' [' + i.Tags.join(',') + '] ' + i.__type; }).join('; '));
                        ServicesProbe.ProbeService.When(function (d) {
                            done('proxy When: ' + d.toISOString());
                            ServicesProbe.ProbeService.Fail(function () { done('Fail returned'); }, failed);
                        }, failed);
                    }, failed);
                }, failed);
            }, failed);
        });
    </script>
</body>
</html>
