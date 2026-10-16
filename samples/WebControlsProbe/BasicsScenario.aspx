<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="BasicsScenario.aspx.cs" Inherits="WebControlsProbe.BasicsScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Basic controls scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <p>
            Label:
            <asp:Label ID="lblPlain" runat="server" Text="Plain" />
            <asp:Label ID="lblBound" runat="server" Text='<%# BoundText %>' />
            <asp:Label ID="lblAssoc" runat="server" AssociatedControlID="txtAssoc" Text="Associated" /> <asp:TextBox ID="txtAssoc" runat="server" />
            <asp:Label ID="lblCode" runat="server" />
            <asp:Label ID="lblNoViewState" runat="server" EnableViewState="false" />
            <asp:Label ID="lblHidden" runat="server" Visible="false" />
        </p>
        <p>
            Literal:
            <asp:Literal ID="litEncode" runat="server" Mode="Encode" Text="<b>encoded</b>" />
            <asp:Literal ID="litPass" runat="server" Mode="PassThrough" Text="<b>pass</b>" />
            <asp:Literal ID="litBound" runat="server" Text='<%# BoundText %>' />
            <asp:Literal ID="litCode" runat="server" />
            <asp:Literal ID="litNoViewState" runat="server" EnableViewState="false" />
            <asp:Literal ID="litHidden" runat="server" Visible="false" />
        </p>
        <p>
            CheckBox:
            <asp:CheckBox ID="chkPlain" runat="server" Text="Plain" OnCheckedChanged="Changed" />
            <asp:CheckBox ID="chkAuto" runat="server" Text="Auto" AutoPostBack="true" OnCheckedChanged="Changed" />
            <asp:CheckBox ID="chkCauses" runat="server" Text="Auto validates g1" AutoPostBack="true" CausesValidation="true" ValidationGroup="g1" OnCheckedChanged="Changed" />
            <asp:CheckBox ID="chkDisabled" runat="server" Text="Disabled" Enabled="false" Checked="true" OnCheckedChanged="Changed" />
            <asp:CheckBox ID="chkBound" runat="server" Text="Bound" Checked='<%# BoundFlag %>' OnCheckedChanged="Changed" />
            <asp:CheckBox ID="chkNoViewState" runat="server" Text="No view state" EnableViewState="false" OnCheckedChanged="Changed" />
            <asp:CheckBox ID="chkHidden" runat="server" Text="Hidden" Visible="false" OnCheckedChanged="Changed" />
        </p>
        <p>
            RadioButton:
            <asp:RadioButton ID="rdoSmall" runat="server" Text="Small" GroupName="size" OnCheckedChanged="Changed" />
            <asp:RadioButton ID="rdoLarge" runat="server" Text="Large" GroupName="size" OnCheckedChanged="Changed" />
            <asp:RadioButton ID="rdoAutoA" runat="server" Text="Auto A" GroupName="auto" AutoPostBack="true" OnCheckedChanged="Changed" />
            <asp:RadioButton ID="rdoAutoB" runat="server" Text="Auto B" GroupName="auto" AutoPostBack="true" OnCheckedChanged="Changed" />
            <asp:RadioButton ID="rdoCauses" runat="server" Text="Auto validates g1" GroupName="causes" AutoPostBack="true" CausesValidation="true" ValidationGroup="g1" OnCheckedChanged="Changed" />
            <asp:RadioButton ID="rdoBound" runat="server" Text="Bound" GroupName="bound" Checked='<%# BoundFlag %>' OnCheckedChanged="Changed" />
            <asp:RadioButton ID="rdoHidden" runat="server" Text="Hidden" GroupName="hidden" Visible="false" OnCheckedChanged="Changed" />
        </p>
        <p>
            HiddenField:
            <asp:HiddenField ID="hidPlain" runat="server" Value="initial" OnValueChanged="Changed" />
            <asp:HiddenField ID="hidBound" runat="server" Value='<%# BoundText %>' OnValueChanged="Changed" />
            <asp:HiddenField ID="hidNoViewState" runat="server" EnableViewState="false" OnValueChanged="Changed" />
            <asp:HiddenField ID="hidHidden" runat="server" Visible="false" OnValueChanged="Changed" />
        </p>
        <p>
            LinkButton:
            <asp:LinkButton ID="lnkSubmit" runat="server" Text="Submit link" CommandName="save" CommandArgument="1" OnClick="Clicked" OnCommand="Commanded" CausesValidation="false" />
            <asp:LinkButton ID="lnkConfirmNo" runat="server" Text="Cancelled link" OnClientClick="return false;" OnClick="Clicked" CausesValidation="false" />
            <asp:LinkButton ID="lnkValidate" runat="server" Text="Validate g1 by a link" ValidationGroup="g1" OnClick="Clicked" />
            <asp:LinkButton ID="lnkDisabled" runat="server" Text="Disabled link" Enabled="false" OnClick="Clicked" />
            <asp:LinkButton ID="lnkBound" runat="server" Text='<%# BoundText %>' OnClick="Clicked" CausesValidation="false" />
            <asp:LinkButton ID="lnkCross" runat="server" Text="Link to the target" PostBackUrl="~/ButtonTarget.aspx" CausesValidation="false" />
            <asp:LinkButton ID="lnkHidden" runat="server" Text="Hidden link" Visible="false" OnClick="Clicked" CausesValidation="false" />
        </p>
        <p>
            HyperLink:
            <asp:HyperLink ID="hypTarget" runat="server" NavigateUrl="~/ButtonTarget.aspx" Text="Go to the target" />
            <asp:HyperLink ID="hypBound" runat="server" NavigateUrl='<%# "~/ButtonTarget.aspx?id=" + 7 %>' Text='<%# BoundText %>' />
            <asp:HyperLink ID="hypImage" runat="server" NavigateUrl="~/ButtonTarget.aspx" ImageUrl="~/Images/probe.png" Text="Image link" />
            <asp:HyperLink ID="hypDisabled" runat="server" NavigateUrl="~/ButtonTarget.aspx" Text="Disabled hyperlink" Enabled="false" />
            <asp:HyperLink ID="hypHidden" runat="server" Visible="false" />
        </p>
        <p>
            Image:
            <asp:Image ID="imgPlain" runat="server" ImageUrl="~/Images/probe.png" AlternateText="Logo" />
            <asp:Image ID="imgBound" runat="server" ImageUrl='<%# "~/Images/probe.png" %>' AlternateText='<%# BoundText %>' />
            <asp:Image ID="imgCode" runat="server" ImageUrl="~/Images/probe.png" />
            <asp:Image ID="imgHidden" runat="server" ImageUrl="~/Images/probe.png" Visible="false" />
        </p>
        <p>
            ImageButton:
            <asp:ImageButton ID="ibtnGo" runat="server" ImageUrl="~/Images/probe.png" AlternateText="Go" CommandName="go" CommandArgument="1" OnClick="ImageClicked" OnCommand="Commanded" CausesValidation="false" />
            <asp:ImageButton ID="ibtnConfirmNo" runat="server" ImageUrl="~/Images/probe.png" AlternateText="Cancelled image" OnClientClick="return false;" OnClick="ImageClicked" CausesValidation="false" />
            <asp:ImageButton ID="ibtnValidate" runat="server" ImageUrl="~/Images/probe.png" AlternateText="Validate g1 by an image" ValidationGroup="g1" OnClick="ImageClicked" />
            <asp:ImageButton ID="ibtnBound" runat="server" ImageUrl="~/Images/probe.png" AlternateText='<%# BoundText %>' OnClick="ImageClicked" CausesValidation="false" />
            <asp:ImageButton ID="ibtnCross" runat="server" ImageUrl="~/Images/probe.png" AlternateText="Image to the target" PostBackUrl="~/ButtonTarget.aspx" CausesValidation="false" />
            <asp:ImageButton ID="ibtnHidden" runat="server" ImageUrl="~/Images/probe.png" AlternateText="Hidden image" Visible="false" OnClick="ImageClicked" CausesValidation="false" />
        </p>
        <asp:Panel ID="pnlGroup" runat="server" GroupingText="Group">
            In a panel: <asp:TextBox ID="txtInPanel" runat="server" />
        </asp:Panel>
        <asp:Panel ID="pnlBound" runat="server" GroupingText='<%# BoundText %>'>Bound panel</asp:Panel>
        <asp:Panel ID="pnlHidden" runat="server" Visible="false">
            In a hidden panel: <asp:TextBox ID="txtInHiddenPanel" runat="server" />
        </asp:Panel>
        <asp:Panel ID="pnlDefaultLink" runat="server" DefaultButton="lnkDefault">
            Default link: <asp:TextBox ID="txtDefaultLink" runat="server" />
            <asp:LinkButton ID="lnkOther" runat="server" Text="Other link" OnClick="Clicked" CausesValidation="false" />
            <asp:LinkButton ID="lnkDefault" runat="server" Text="Default link" OnClick="Clicked" CausesValidation="false" />
        </asp:Panel>
        <asp:Panel ID="pnlDefaultImage" runat="server" DefaultButton="ibtnDefault">
            Default image: <asp:TextBox ID="txtDefaultImage" runat="server" />
            <asp:ImageButton ID="ibtnDefault" runat="server" ImageUrl="~/Images/probe.png" AlternateText="Default image" OnClick="ImageClicked" CausesValidation="false" />
        </asp:Panel>
        <p>
            PlaceHolder:
            <asp:PlaceHolder ID="phCreated" runat="server" />
            <asp:PlaceHolder ID="phBound" runat="server"><asp:Literal ID="litInBoundHolder" runat="server" Text='<%# BoundText %>' /></asp:PlaceHolder>
            <asp:PlaceHolder ID="phHidden" runat="server" Visible="false"><asp:Label ID="lblInHiddenHolder" runat="server" /></asp:PlaceHolder>
        </p>
        <p>Dynamic: <asp:PlaceHolder ID="Dynamic" runat="server" /></p>
        <asp:Repeater ID="rptItems" runat="server" OnItemCommand="ItemCommand">
            <ItemTemplate>
                <asp:Label ID="lblItem" runat="server" Text='<%# Container.DataItem %>' />
                <asp:LinkButton ID="lnkPick" runat="server" Text='<%# "Pick " + Container.DataItem %>' CommandName="pick" CommandArgument='<%# Container.DataItem %>' CausesValidation="false" />
                <asp:ImageButton ID="ibtnPick" runat="server" ImageUrl="~/Images/probe.png" AlternateText='<%# "Image pick " + Container.DataItem %>' CommandName="pick-image" CommandArgument='<%# Container.DataItem %>' CausesValidation="false" />
            </ItemTemplate>
        </asp:Repeater>
        <asp:FormView ID="fvBasics" runat="server" DefaultMode="Edit" OnItemUpdating="Updating" OnModeChanging="ModeChanging">
            <EditItemTemplate>
                <asp:Label ID="lblBind" runat="server" Text='<%# Bind("Name") %>' />
                <asp:CheckBox ID="chkBind" runat="server" Text="Flag" Checked='<%# Bind("Flag") %>' />
                <asp:RadioButton ID="rdoBind" runat="server" Text="Choice" GroupName="bind" Checked='<%# Bind("Choice") %>' />
                <asp:HiddenField ID="hidBind" runat="server" Value='<%# Bind("Secret") %>' />
                <asp:Button ID="btnUpdate" runat="server" CommandName="Update" Text="Update" CausesValidation="false" />
            </EditItemTemplate>
        </asp:FormView>
        <p>
            Required (g1, server): <asp:TextBox ID="txtRequired" runat="server" />
            <asp:RequiredFieldValidator ID="reqRequired" runat="server" ControlToValidate="txtRequired" ValidationGroup="g1" ErrorMessage="Required (g1)" EnableClientScript="false" />
            Cross-page: <asp:TextBox ID="txtCross" runat="server" />
        </p>
        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CausesValidation="false" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" CausesValidation="false" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
