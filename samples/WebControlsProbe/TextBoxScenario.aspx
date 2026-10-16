<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="TextBoxScenario.aspx.cs" Inherits="WebControlsProbe.TextBoxScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>TextBox scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <p>Plain: <asp:TextBox ID="txtPlain" runat="server" Text="Taro" OnTextChanged="Changed" /></p>
        <p>Auto: <asp:TextBox ID="txtAuto" runat="server" AutoPostBack="true" OnTextChanged="Changed" /></p>
        <p>ReadOnly: <asp:TextBox ID="txtReadOnly" runat="server" ReadOnly="true" Text="fixed" OnTextChanged="Changed" /></p>
        <p>Disabled: <asp:TextBox ID="txtDisabled" runat="server" Enabled="false" Text="disabled" OnTextChanged="Changed" /></p>
        <p>Password: <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" OnTextChanged="Changed" /></p>
        <p>Multi: <asp:TextBox ID="txtMulti" runat="server" TextMode="MultiLine" OnTextChanged="Changed" /></p>
        <p>NoViewState: <asp:TextBox ID="txtNoViewState" runat="server" EnableViewState="false" Text="initial" OnTextChanged="Changed" /></p>
        <p>Hidden: <asp:TextBox ID="txtHidden" runat="server" Visible="false" OnTextChanged="Changed" /></p>
        <p>Raw: <asp:TextBox ID="txtRaw" runat="server" ValidateRequestMode="Disabled" OnTextChanged="Changed" /></p>
        <p>Dynamic: <asp:PlaceHolder ID="Dynamic" runat="server" /></p>
        <p>
            Validated (g1): <asp:TextBox ID="txtValidated" runat="server" />
            <asp:RequiredFieldValidator ID="reqValidated" runat="server" ControlToValidate="txtValidated" ValidationGroup="g1" ErrorMessage="Required (g1)" EnableClientScript="false" />
            <asp:RegularExpressionValidator ID="rexValidated" runat="server" ControlToValidate="txtValidated" ValidationGroup="g1" ValidationExpression="\d+" ErrorMessage="Digits only (g1)" EnableClientScript="false" />
            <asp:Button ID="btnG1" runat="server" Text="Validate g1" ValidationGroup="g1" />
        </p>
        <p>
            Auto validated (g2): <asp:TextBox ID="txtAutoValidated" runat="server" AutoPostBack="true" CausesValidation="true" ValidationGroup="g2" OnTextChanged="Changed" />
            <asp:RequiredFieldValidator ID="reqAutoValidated" runat="server" ControlToValidate="txtAutoValidated" ValidationGroup="g2" ErrorMessage="Required (g2)" EnableClientScript="false" />
        </p>
        <p>
            Client validated (g3): <asp:TextBox ID="txtClient" runat="server" />
            <asp:RequiredFieldValidator ID="reqClient" runat="server" ControlToValidate="txtClient" ValidationGroup="g3" ErrorMessage="Required (g3, in the browser)" />
            <asp:Button ID="btnG3" runat="server" Text="Validate g3" ValidationGroup="g3" />
        </p>
        <asp:FormView ID="fvPerson" runat="server" DefaultMode="Edit" OnItemUpdating="Updating" OnModeChanging="ModeChanging">
            <EditItemTemplate>
                Bound: <asp:TextBox ID="txtBound" runat="server" Text='<%# Bind("Name") %>' />
                <asp:Button ID="btnUpdate" runat="server" CommandName="Update" Text="Update" CausesValidation="false" />
            </EditItemTemplate>
        </asp:FormView>
        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CausesValidation="false" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" CausesValidation="false" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
