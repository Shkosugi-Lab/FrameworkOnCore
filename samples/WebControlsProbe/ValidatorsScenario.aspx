<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ValidatorsScenario.aspx.cs" Inherits="WebControlsProbe.ValidatorsScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Validators scenario</title>
    <script type="text/javascript">
        function checkOk(sender, args) { args.IsValid = args.Value == "ok"; }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <fieldset>
            <legend>Server (group "server", no client script)</legend>
            Name: <asp:TextBox ID="txtName" runat="server" />
            <asp:RequiredFieldValidator ID="reqName" runat="server" ControlToValidate="txtName" ValidationGroup="server" EnableClientScript="false" ErrorMessage='<%# "Name is required" %>' Text="*" />
            Initial: <asp:TextBox ID="txtInitial" runat="server" Text="(none)" />
            <asp:RequiredFieldValidator ID="reqInitial" runat="server" ControlToValidate="txtInitial" InitialValue="(none)" ValidationGroup="server" EnableClientScript="false" ErrorMessage='<%# "Change the initial value" %>' />
            Code: <asp:TextBox ID="txtCode" runat="server" />
            <asp:RegularExpressionValidator ID="rexCode" runat="server" ControlToValidate="txtCode" ValidationExpression="[0-9]+" ValidationGroup="server" EnableClientScript="false" Display="Dynamic" ErrorMessage='<%# "Digits only" %>' />
            Age: <asp:TextBox ID="txtAge" runat="server" />
            <asp:RangeValidator ID="rngAge" runat="server" ControlToValidate="txtAge" MinimumValue="1" MaximumValue="120" Type="Integer" ValidationGroup="server" EnableClientScript="false" ErrorMessage='<%# "Age from 1 to 120" %>' />
            Password: <asp:TextBox ID="txtPassword" runat="server" /> Confirm: <asp:TextBox ID="txtConfirm" runat="server" />
            <asp:CompareValidator ID="cmpConfirm" runat="server" ControlToValidate="txtConfirm" ControlToCompare="txtPassword" ValidationGroup="server" EnableClientScript="false" ErrorMessage='<%# "Must match" %>' />
            Quantity: <asp:TextBox ID="txtQty" runat="server" />
            <asp:CompareValidator ID="cmpQty" runat="server" ControlToValidate="txtQty" ValueToCompare="3" Operator="GreaterThan" Type="Integer" ValidationGroup="server" EnableClientScript="false" ErrorMessage='<%# "More than 3" %>' />
            Date: <asp:TextBox ID="txtDate" runat="server" />
            <asp:CompareValidator ID="cmpDate" runat="server" ControlToValidate="txtDate" Operator="DataTypeCheck" Type="Date" ValidationGroup="server" EnableClientScript="false" ErrorMessage='<%# "A date" %>' />
            Custom: <asp:TextBox ID="txtCustom" runat="server" />
            <asp:CustomValidator ID="cusCustom" runat="server" ControlToValidate="txtCustom" OnServerValidate="MustBeOk" ValidationGroup="server" EnableClientScript="false" ErrorMessage='<%# "Must be ok" %>' />
            Custom (empty validated): <asp:TextBox ID="txtCustomEmpty" runat="server" />
            <asp:CustomValidator ID="cusEmpty" runat="server" ControlToValidate="txtCustomEmpty" ValidateEmptyText="true" OnServerValidate="NotEmpty" ValidationGroup="server" EnableClientScript="false" ErrorMessage='<%# "Required (custom)" %>' />
            <asp:CustomValidator ID="cusNoControl" runat="server" OnServerValidate="NameNotForbidden" ValidationGroup="server" EnableClientScript="false" ErrorMessage='<%# "Name is forbidden" %>' />
            Disabled: <asp:TextBox ID="txtDisabledTarget" runat="server" />
            <asp:RequiredFieldValidator ID="reqDisabled" runat="server" ControlToValidate="txtDisabledTarget" Enabled="false" ValidationGroup="server" EnableClientScript="false" ErrorMessage="Disabled validator" />
            Hidden: <asp:TextBox ID="txtHiddenTarget" runat="server" />
            <asp:RequiredFieldValidator ID="reqHidden" runat="server" ControlToValidate="txtHiddenTarget" Visible="false" ValidationGroup="server" EnableClientScript="false" ErrorMessage="Hidden validator" />
            Dynamic: <asp:TextBox ID="txtDynamicTarget" runat="server" /> <asp:PlaceHolder ID="Dynamic" runat="server" />
            <asp:ValidationSummary ID="sumServer" runat="server" ValidationGroup="server" HeaderText='<%# "Server errors" %>' EnableClientScript="false" />
            <asp:Button ID="btnServer" runat="server" Text="Validate on the server" ValidationGroup="server" OnClick="Clicked" />
            <asp:Button ID="btnManual" runat="server" Text="Validate the code only" CausesValidation="false" OnClick="ValidateCode" />
            <asp:Button ID="btnPageValidate" runat="server" Text="Page.Validate(server)" CausesValidation="false" OnClick="ValidateGroup" />
        </fieldset>
        <fieldset>
            <legend>In the browser (a group each)</legend>
            Name: <asp:TextBox ID="txtNameC" runat="server" />
            <asp:RequiredFieldValidator ID="reqNameC" runat="server" ControlToValidate="txtNameC" ValidationGroup="c-req" Display="Dynamic" ErrorMessage="Name is required (browser)" />
            <asp:Button ID="btnReqC" runat="server" Text="Required" ValidationGroup="c-req" OnClick="Clicked" />
            Code: <asp:TextBox ID="txtCodeC" runat="server" />
            <asp:RegularExpressionValidator ID="rexCodeC" runat="server" ControlToValidate="txtCodeC" ValidationExpression="[0-9]+" ValidationGroup="c-rex" ErrorMessage="Digits only (browser)" />
            <asp:Button ID="btnRexC" runat="server" Text="Expression" ValidationGroup="c-rex" OnClick="Clicked" />
            Age: <asp:TextBox ID="txtAgeC" runat="server" />
            <asp:RangeValidator ID="rngAgeC" runat="server" ControlToValidate="txtAgeC" MinimumValue="1" MaximumValue="120" Type="Integer" ValidationGroup="c-rng" ErrorMessage="Age from 1 to 120 (browser)" />
            <asp:Button ID="btnRngC" runat="server" Text="Range" ValidationGroup="c-rng" OnClick="Clicked" />
            Password: <asp:TextBox ID="txtPasswordC" runat="server" /> Confirm: <asp:TextBox ID="txtConfirmC" runat="server" />
            <asp:CompareValidator ID="cmpConfirmC" runat="server" ControlToValidate="txtConfirmC" ControlToCompare="txtPasswordC" ValidationGroup="c-cmp" ErrorMessage="Must match (browser)" />
            <asp:Button ID="btnCmpC" runat="server" Text="Compare" ValidationGroup="c-cmp" OnClick="Clicked" />
            Custom: <asp:TextBox ID="txtCustomC" runat="server" />
            <asp:CustomValidator ID="cusCustomC" runat="server" ControlToValidate="txtCustomC" ClientValidationFunction="checkOk" OnServerValidate="MustBeOk" ValidationGroup="c-cus" ErrorMessage="Must be ok (browser)" />
            <asp:Button ID="btnCusC" runat="server" Text="Custom" ValidationGroup="c-cus" OnClick="Clicked" />
            Summary: <asp:TextBox ID="txtSumC" runat="server" /> <asp:TextBox ID="txtSumC2" runat="server" />
            <asp:RequiredFieldValidator ID="reqSumC" runat="server" ControlToValidate="txtSumC" ValidationGroup="c-sum" Text="*" ErrorMessage="First is required (browser)" />
            <asp:RequiredFieldValidator ID="reqSumC2" runat="server" ControlToValidate="txtSumC2" ValidationGroup="c-sum" Display="None" ErrorMessage="Second is required (only in the summary)" />
            <asp:ValidationSummary ID="sumClient" runat="server" ValidationGroup="c-sum" HeaderText="Browser errors" DisplayMode="List" />
            <asp:Button ID="btnSumC" runat="server" Text="Summary" ValidationGroup="c-sum" OnClick="Clicked" />
        </fieldset>
        <p><asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" CausesValidation="false" /></p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
