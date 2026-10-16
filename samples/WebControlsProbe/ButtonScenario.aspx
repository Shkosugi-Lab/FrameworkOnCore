<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ButtonScenario.aspx.cs" Inherits="WebControlsProbe.ButtonScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Button scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CommandName="save" CommandArgument="1" OnClick="Clicked" OnCommand="Commanded" CausesValidation="false" />
            <asp:Button ID="btnScript" runat="server" Text="Script" UseSubmitBehavior="false" OnClick="Clicked" CausesValidation="false" />
            <asp:Button ID="btnConfirmNo" runat="server" Text="Cancelled" OnClientClick="return false;" OnClick="Clicked" CausesValidation="false" />
            <asp:Button ID="btnConfirmYes" runat="server" Text="Confirmed" OnClientClick="return true;" OnClick="Clicked" CausesValidation="false" />
            <asp:Button ID="btnDisabled" runat="server" Text="Disabled" Enabled="false" OnClick="Clicked" />
        </p>
        <p>
            Required (g1): <asp:TextBox ID="txtRequired" runat="server" />
            <asp:RequiredFieldValidator ID="reqRequired" runat="server" ControlToValidate="txtRequired" ValidationGroup="g1" ErrorMessage="Required (g1)" EnableClientScript="false" />
            <asp:Button ID="btnValidate" runat="server" Text="Validate g1" ValidationGroup="g1" OnClick="Clicked" />
            <asp:Button ID="btnNoValidate" runat="server" Text="Skip validation" ValidationGroup="g1" CausesValidation="false" OnClick="Clicked" />
        </p>
        <p>
            Required (g2, in the browser): <asp:TextBox ID="txtClient" runat="server" />
            <asp:RequiredFieldValidator ID="reqClient" runat="server" ControlToValidate="txtClient" ValidationGroup="g2" ErrorMessage="Required (g2, in the browser)" />
            <asp:Button ID="btnClient" runat="server" Text="Validate g2" ValidationGroup="g2" OnClick="Clicked" />
        </p>
        <asp:Repeater ID="rptItems" runat="server" OnItemCommand="ItemCommand">
            <ItemTemplate>
                <span><%# Container.DataItem %></span>
                <asp:Button ID="btnPick" runat="server" Text="Pick" CommandName="pick" CommandArgument='<%# Container.DataItem %>' CausesValidation="false" />
            </ItemTemplate>
        </asp:Repeater>
        <asp:Panel ID="pnlDefault" runat="server" DefaultButton="btnDefault">
            Default button: <asp:TextBox ID="txtDefault" runat="server" />
            <asp:Button ID="btnOther" runat="server" Text="Other" OnClick="Clicked" CausesValidation="false" />
            <asp:Button ID="btnDefault" runat="server" Text="Default" OnClick="Clicked" CausesValidation="false" />
        </asp:Panel>
        <p>Dynamic: <asp:PlaceHolder ID="Dynamic" runat="server" /></p>
        <p>
            <asp:Button ID="btnHidden" runat="server" Text="Hidden" Visible="false" OnClick="Clicked" CausesValidation="false" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" CausesValidation="false" />
        </p>
        <p>
            Cross-page: <asp:TextBox ID="txtCross" runat="server" />
            <asp:Button ID="btnCross" runat="server" Text="To the target" PostBackUrl="~/ButtonTarget.aspx" CausesValidation="false" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
