<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="LocaleProbe.Form" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>form</title></head>
<body>
    <form id="form1" runat="server">
        <asp:Calendar ID="cal" runat="server" OnSelectionChanged="DayChosen" ShowGridLines="true" />
        <asp:TextBox ID="txtAmount" runat="server" />
        <asp:CompareValidator ID="valAmount" runat="server" ControlToValidate="txtAmount" Type="Currency" Operator="DataTypeCheck" ErrorMessage="amount" Display="Dynamic" />
        <asp:RangeValidator ID="valRange" runat="server" ControlToValidate="txtAmount" Type="Double" MinimumValue="0.5" MaximumValue="1000.25" CultureInvariantValues="true" ErrorMessage="range" Display="Dynamic" />
        <asp:TextBox ID="txtDate" runat="server" />
        <asp:CompareValidator ID="valDate" runat="server" ControlToValidate="txtDate" Type="Date" Operator="GreaterThanEqual" ValueToCompare="2026-01-01" CultureInvariantValues="true" ErrorMessage="date" Display="Dynamic" />
        <asp:TextBox ID="txtCount" runat="server" />
        <asp:CompareValidator ID="valCount" runat="server" ControlToValidate="txtCount" Type="Integer" Operator="LessThan" ValueToCompare="1000" ErrorMessage="count" Display="Dynamic" />
        <asp:DropDownList ID="ddlMonth" runat="server" />
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="Submitted" />
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="When" HeaderText="d" DataFormatString="{0:d}" />
                <asp:BoundField DataField="When" HeaderText="MMMM" DataFormatString="{0:dddd, MMMM d, yyyy}" />
                <asp:BoundField DataField="Amount" HeaderText="C" DataFormatString="{0:C}" />
                <asp:BoundField DataField="Amount" HeaderText="N" DataFormatString="{0:N3}" />
                <asp:BoundField DataField="Ratio" HeaderText="P" DataFormatString="{0:P1}" />
                <asp:BoundField DataField="When" HeaderText="raw" />
                <asp:TemplateField HeaderText="eval"><ItemTemplate><%# Eval("When", "{0:g}") %> / <%# Eval("Amount") %></ItemTemplate></asp:TemplateField>
            </Columns>
        </asp:GridView>
        <pre id="out"><asp:Literal ID="Out" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>