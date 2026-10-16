<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Default.aspx.cs" Inherits="AuthProbe.Default" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Auth probe</title>
</head>
<body>
    <form id="form1" runat="server">
        <p>
            <asp:Button ID="btnSetup" runat="server" Text="Setup" OnClick="Setup" />
            <asp:Button ID="btnLoginAlice" runat="server" Text="Sign in alice" OnClick="SignInAlice" />
            <asp:Button ID="btnLoginBob" runat="server" Text="Sign in bob" OnClick="SignInBob" />
            <asp:Button ID="btnPersistent" runat="server" Text="Sign in alice, persistent" OnClick="SignInPersistent" />
            <asp:Button ID="btnLogout" runat="server" Text="Sign out" OnClick="SignOut" />
            <asp:Button ID="btnProfile" runat="server" Text="Profile" OnClick="UpdateProfile" />
            <asp:Button ID="btnChangePassword" runat="server" Text="Change password" OnClick="ChangePassword" />
            <asp:Button ID="btnBadLogin" runat="server" Text="Lock out" OnClick="LockOut" />
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
        </p>
        <p><a href="Members/Default.aspx">Members</a> <a href="Admin/Default.aspx">Admin</a></p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
