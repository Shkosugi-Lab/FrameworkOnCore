<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ListControlsScenario.aspx.cs" Inherits="WebControlsProbe.ListControlsScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>List controls scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <p>
            DropDownList:
            <asp:DropDownList ID="ddlSize" runat="server" OnSelectedIndexChanged="Changed">
                <asp:ListItem Value="S">Small</asp:ListItem>
                <asp:ListItem Value="M">Medium</asp:ListItem>
                <asp:ListItem Value="L">Large</asp:ListItem>
            </asp:DropDownList>
            Auto: <asp:DropDownList ID="ddlAuto" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Changed">
                <asp:ListItem Value="S">Small</asp:ListItem>
                <asp:ListItem Value="M">Medium</asp:ListItem>
                <asp:ListItem Value="L">Large</asp:ListItem>
            </asp:DropDownList>
            Bound: <asp:DropDownList ID="ddlBound" runat="server" DataTextField="Name" DataValueField="Code" AppendDataBoundItems="true" OnSelectedIndexChanged="Changed">
                <asp:ListItem Value="">(choose)</asp:ListItem>
            </asp:DropDownList>
            No view state: <asp:DropDownList ID="ddlNoViewState" runat="server" EnableViewState="false" OnSelectedIndexChanged="Changed">
                <asp:ListItem Value="S">Small</asp:ListItem>
                <asp:ListItem Value="M">Medium</asp:ListItem>
            </asp:DropDownList>
        </p>
        <p>
            ListBox (multiple): <asp:ListBox ID="lstMulti" runat="server" SelectionMode="Multiple" Rows="3" OnSelectedIndexChanged="Changed">
                <asp:ListItem Value="R">Red</asp:ListItem>
                <asp:ListItem Value="G">Green</asp:ListItem>
                <asp:ListItem Value="B">Blue</asp:ListItem>
            </asp:ListBox>
            Auto: <asp:ListBox ID="lstAuto" runat="server" AutoPostBack="true" Rows="3" OnSelectedIndexChanged="Changed">
                <asp:ListItem Value="R">Red</asp:ListItem>
                <asp:ListItem Value="G">Green</asp:ListItem>
            </asp:ListBox>
        </p>
        <p>
            CheckBoxList: <asp:CheckBoxList ID="cblColors" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="Changed">
                <asp:ListItem Value="R">Red</asp:ListItem>
                <asp:ListItem Value="G" Enabled="false">Green</asp:ListItem>
                <asp:ListItem Value="B">Blue</asp:ListItem>
            </asp:CheckBoxList>
            Auto: <asp:CheckBoxList ID="cblAuto" runat="server" AutoPostBack="true" RepeatLayout="Flow" OnSelectedIndexChanged="Changed">
                <asp:ListItem Value="X">X</asp:ListItem>
                <asp:ListItem Value="Y">Y</asp:ListItem>
            </asp:CheckBoxList>
        </p>
        <p>
            RadioButtonList: <asp:RadioButtonList ID="rblSize" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="Changed">
                <asp:ListItem Value="S">Small</asp:ListItem>
                <asp:ListItem Value="M">Medium</asp:ListItem>
                <asp:ListItem Value="L">Large</asp:ListItem>
            </asp:RadioButtonList>
            <asp:RequiredFieldValidator ID="reqSize" runat="server" ControlToValidate="rblSize" ValidationGroup="g2" ErrorMessage="Choose a size (g2)" EnableClientScript="false" />
            Auto: <asp:RadioButtonList ID="rblAuto" runat="server" AutoPostBack="true" RepeatLayout="Flow" OnSelectedIndexChanged="Changed">
                <asp:ListItem Value="X">X</asp:ListItem>
                <asp:ListItem Value="Y">Y</asp:ListItem>
            </asp:RadioButtonList>
        </p>
        <p>
            Validated (g1, server): <asp:DropDownList ID="ddlValidated" runat="server">
                <asp:ListItem Value="">(choose)</asp:ListItem>
                <asp:ListItem Value="A">A</asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="reqValidated" runat="server" ControlToValidate="ddlValidated" ValidationGroup="g1" ErrorMessage="Choose one (g1)" EnableClientScript="false" />
            <asp:ListBox ID="lstValidated" runat="server" Rows="2">
                <asp:ListItem Value="A">A</asp:ListItem>
                <asp:ListItem Value="B">B</asp:ListItem>
            </asp:ListBox>
            <asp:RequiredFieldValidator ID="reqListValidated" runat="server" ControlToValidate="lstValidated" ValidationGroup="g1" ErrorMessage="Choose from the list (g1)" EnableClientScript="false" />
            <asp:CheckBoxList ID="cblValidated" runat="server" RepeatLayout="Flow">
                <asp:ListItem Value="A">A</asp:ListItem>
            </asp:CheckBoxList>
            <%-- A CheckBoxList has no ValidationProperty: a RequiredFieldValidator cannot validate it (an HttpException); a CustomValidator does. --%>
            <asp:CustomValidator ID="cusChecks" runat="server" ValidationGroup="g1" ErrorMessage="Check A (g1)" OnServerValidate="ChecksValidate" EnableClientScript="false" />
            <asp:Button ID="btnG1" runat="server" Text="Validate g1" ValidationGroup="g1" />
            <asp:Button ID="btnG2" runat="server" Text="Validate g2" ValidationGroup="g2" />
        </p>
        <p>
            Validated (g3, in the browser): <asp:DropDownList ID="ddlClient" runat="server">
                <asp:ListItem Value="">(choose)</asp:ListItem>
                <asp:ListItem Value="A">A</asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="reqClient" runat="server" ControlToValidate="ddlClient" ValidationGroup="g3" ErrorMessage="Choose one (g3, in the browser)" />
            <asp:ListBox ID="lstClient" runat="server" Rows="2">
                <asp:ListItem Value="A">A</asp:ListItem>
            </asp:ListBox>
            <asp:RequiredFieldValidator ID="reqListClient" runat="server" ControlToValidate="lstClient" ValidationGroup="g3" ErrorMessage="Choose from the list (g3, in the browser)" />
            <asp:RadioButtonList ID="rblClient" runat="server" RepeatLayout="Flow">
                <asp:ListItem Value="A">A</asp:ListItem>
            </asp:RadioButtonList>
            <asp:RequiredFieldValidator ID="reqRadioClient" runat="server" ControlToValidate="rblClient" ValidationGroup="g3" ErrorMessage="Choose a radio (g3, in the browser)" />
            <asp:Button ID="btnG3" runat="server" Text="Validate g3" ValidationGroup="g3" />
        </p>
        <p>
            BulletedList: <asp:BulletedList ID="bltLinks" runat="server" DisplayMode="LinkButton" CausesValidation="false" OnClick="BulletClicked">
                <asp:ListItem Value="one">First</asp:ListItem>
                <asp:ListItem Value="two">Second</asp:ListItem>
                <asp:ListItem Value="three" Enabled="false">Third</asp:ListItem>
            </asp:BulletedList>
            <asp:BulletedList ID="bltValidated" runat="server" DisplayMode="LinkButton" ValidationGroup="g1" OnClick="BulletClicked">
                <asp:ListItem Value="v">Validate g1 by a bullet</asp:ListItem>
            </asp:BulletedList>
            <asp:BulletedList ID="bltBound" runat="server" DataTextField="Name" DataValueField="Code" BulletStyle="Numbered" />
        </p>
        <p>Dynamic: <asp:PlaceHolder ID="Dynamic" runat="server" /></p>
        <p>
            Hidden: <asp:DropDownList ID="ddlHidden" runat="server" Visible="false" OnSelectedIndexChanged="Changed" />
            <asp:ListBox ID="lstHidden" runat="server" Visible="false" />
            <asp:CheckBoxList ID="cblHidden" runat="server" Visible="false" />
            <asp:RadioButtonList ID="rblHidden" runat="server" Visible="false" />
            <asp:BulletedList ID="bltHidden" runat="server" Visible="false" />
        </p>
        <asp:FormView ID="fvProduct" runat="server" DefaultMode="Edit" OnItemUpdating="Updating" OnModeChanging="ModeChanging">
            <EditItemTemplate>
                <asp:DropDownList ID="ddlBind" runat="server" SelectedValue='<%# Bind("Ddl") %>'>
                    <asp:ListItem Value="P1">Pen</asp:ListItem>
                    <asp:ListItem Value="B2">Bag</asp:ListItem>
                </asp:DropDownList>
                <asp:ListBox ID="lstBind" runat="server" Rows="2" SelectedValue='<%# Bind("Lst") %>'>
                    <asp:ListItem Value="P1">Pen</asp:ListItem>
                    <asp:ListItem Value="B2">Bag</asp:ListItem>
                </asp:ListBox>
                <asp:RadioButtonList ID="rblBind" runat="server" RepeatLayout="Flow" SelectedValue='<%# Bind("Rbl") %>'>
                    <asp:ListItem Value="P1">Pen</asp:ListItem>
                    <asp:ListItem Value="B2">Bag</asp:ListItem>
                </asp:RadioButtonList>
                <asp:CheckBoxList ID="cblBind" runat="server" RepeatLayout="Flow" SelectedValue='<%# Bind("Cbl") %>'>
                    <asp:ListItem Value="P1">Pen</asp:ListItem>
                    <asp:ListItem Value="B2">Bag</asp:ListItem>
                </asp:CheckBoxList>
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
