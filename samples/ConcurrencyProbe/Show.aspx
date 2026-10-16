<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>Concurrency probe: a run</title></head>
<body><p id="run">Run <%= HttpUtility.HtmlEncode(Request.QueryString["run"]) %>: <%= HttpUtility.HtmlEncode(ConcurrencyProbe.Timeline.Describe(Request.QueryString["run"])) %></p></body>
</html>
