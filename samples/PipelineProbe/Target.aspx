<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Pipeline probe: target</title>
</head>
<body>
    <pre id="target"><%= HttpUtility.HtmlEncode(
        "from=" + Request.QueryString["from"] +
        "\nPath=" + Request.Path + " FilePath=" + Request.FilePath + " CurrentExecutionFilePath=" + Request.CurrentExecutionFilePath +
        " AppRelativeCurrentExecutionFilePath=" + Request.AppRelativeCurrentExecutionFilePath +
        "\nRawUrl=" + Request.RawUrl + " Url=" + Request.Url.PathAndQuery +
        "\nHandler=" + Context.Handler.GetType().Name + " CurrentHandler=" + Context.CurrentHandler.GetType().Name +
        " PreviousHandler=" + (Context.PreviousHandler == null ? "none" : Context.PreviousHandler.GetType().Name) +
        "\nPreviousPage=" + (PreviousPage == null ? "none" : PreviousPage.GetType().Name) + " IsCrossPagePostBack=" + IsCrossPagePostBack + " IsPostBack=" + IsPostBack +
        "\nForm txtValue=" + Request.Form["txtValue"] + " Form keys=" + Request.Form.Count) %></pre>
</body>
</html>
