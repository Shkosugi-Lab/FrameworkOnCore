<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Download.aspx.cs" Inherits="NavigationProbe.Y.Download" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>Y/Download</title></head>
<body>
  <form id="form1" runat="server">
    <h1>Y/Download</h1>
    <asp:Button ID="btnDownload" runat="server" Text="download (Button)" OnClick="SendReport" />
    <asp:LinkButton ID="lnkDownload" runat="server" Text="download (LinkButton)" OnClick="SendReport" />
    <asp:Button ID="btnUnicode" runat="server" Text="download (Unicode name)" OnClick="DownloadUnicode" />
    <asp:Button ID="btnAfter" runat="server" Text="postback after the download" OnClick="After" />
    <asp:LinkButton ID="lnkAfter" runat="server" Text="link postback after the download" OnClick="After" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
