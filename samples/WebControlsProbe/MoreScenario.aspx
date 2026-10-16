<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="MoreScenario.aspx.cs" Inherits="WebControlsProbe.MoreScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>More controls scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Table ID="tblPlain" runat="server" GridLines="Both" Caption="Plain table">
            <asp:TableHeaderRow ID="thrHead" runat="server">
                <asp:TableHeaderCell ID="thName" runat="server" Text="Name" Scope="Column" />
                <asp:TableHeaderCell ID="thBound" runat="server" Text='<%# BoundText %>' />
            </asp:TableHeaderRow>
            <asp:TableRow ID="trFirst" runat="server">
                <asp:TableCell ID="tdFirst" runat="server" Text="First" />
                <asp:TableCell ID="tdBound" runat="server" Text='<%# BoundText %>' />
            </asp:TableRow>
            <asp:TableRow ID="trCode" runat="server">
                <asp:TableCell ID="tdCode" runat="server" />
                <asp:TableCell ID="tdHidden" runat="server" Visible="false" />
            </asp:TableRow>
            <asp:TableFooterRow ID="tfrFoot" runat="server">
                <asp:TableCell ID="tdFoot" runat="server" Text="Foot" ColumnSpan="2" />
            </asp:TableFooterRow>
        </asp:Table>
        <asp:Table ID="tblCode" runat="server" GridLines="Both" />
        <asp:Table ID="tblHidden" runat="server" Visible="false" />
        <p>
            FileUpload:
            <asp:FileUpload ID="fupPlain" runat="server" />
            <asp:FileUpload ID="fupMulti" runat="server" AllowMultiple="true" />
            <asp:FileUpload ID="fupDisabled" runat="server" Enabled="false" />
            <asp:FileUpload ID="fupBound" runat="server" ToolTip='<%# BoundText %>' />
            <asp:FileUpload ID="fupHidden" runat="server" Visible="false" />
            <asp:RequiredFieldValidator ID="reqUpload" runat="server" ControlToValidate="fupPlain" ValidationGroup="g1" ErrorMessage="A file is required" EnableClientScript="false" />
            <asp:Button ID="btnUploadValidate" runat="server" Text="Validate g1" ValidationGroup="g1" />
        </p>
        <asp:MultiView ID="mvSteps" runat="server" ActiveViewIndex="0" OnActiveViewChanged="ViewChanged">
            <asp:View ID="vwFirst" runat="server" OnActivate="ViewActivated" OnDeactivate="ViewDeactivated">
                First view: <asp:TextBox ID="txtInFirst" runat="server" />
                <asp:Button ID="btnNext" runat="server" Text="Next view" CommandName="NextView" CausesValidation="false" />
            </asp:View>
            <asp:View ID="vwSecond" runat="server" OnActivate="ViewActivated" OnDeactivate="ViewDeactivated">
                Second view: <asp:Label ID="lblInSecond" runat="server" Text='<%# BoundText %>' />
                <asp:Button ID="btnPrevious" runat="server" Text="Previous view" CommandName="PrevView" CausesValidation="false" />
                <asp:Button ID="btnToThird" runat="server" Text="To the third" CommandName="SwitchViewByID" CommandArgument="vwThird" CausesValidation="false" />
            </asp:View>
            <asp:View ID="vwThird" runat="server" OnActivate="ViewActivated" OnDeactivate="ViewDeactivated">
                Third view
                <asp:Button ID="btnToFirst" runat="server" Text="To the first" CommandName="SwitchViewByIndex" CommandArgument="0" CausesValidation="false" />
            </asp:View>
        </asp:MultiView>
        <asp:MultiView ID="mvHidden" runat="server" ActiveViewIndex="0" Visible="false">
            <asp:View ID="vwInHidden" runat="server">In a hidden multi-view</asp:View>
        </asp:MultiView>
        <p>
            Localize:
            <asp:Localize ID="locPlain" runat="server" Text="<b>localized</b>" />
            <asp:Localize ID="locEncode" runat="server" Mode="Encode" Text="<b>encoded</b>" />
            <asp:Localize ID="locBound" runat="server" Text='<%# BoundText %>' />
            <asp:Localize ID="locCode" runat="server" />
            <asp:Localize ID="locHidden" runat="server" Visible="false" />
        </p>
        <p>
            Substitution: <asp:Substitution ID="subPlain" runat="server" MethodName="SubstitutionText" />
        </p>
        <asp:Xml ID="xmlPlain" runat="server" DocumentSource="~/App_Data/probe.xml" TransformSource="~/App_Data/probe.xslt" />
        <asp:Xml ID="xmlBound" runat="server" DocumentContent='<%# "<items><item>" + BoundText + "</item></items>" %>' TransformSource="~/App_Data/probe.xslt" />
        <asp:Xml ID="xmlCode" runat="server" TransformSource="~/App_Data/probe.xslt" />
        <p>
            AdRotator:
            <asp:AdRotator ID="adPlain" runat="server" AdvertisementFile="~/App_Data/ads.xml" OnAdCreated="AdCreated" />
            <asp:AdRotator ID="adBound" runat="server" ImageUrlField="Image" NavigateUrlField="Code" AlternateTextField="Name" OnAdCreated="AdCreated" />
            <asp:AdRotator ID="adHidden" runat="server" AdvertisementFile="~/App_Data/ads.xml" Visible="false" />
        </p>
        <asp:Calendar ID="calPlain" runat="server" SelectionMode="DayWeekMonth" SelectWeekText="Week" SelectMonthText="Month" OnSelectionChanged="DateSelected" OnVisibleMonthChanged="MonthChanged" OnDayRender="DayRender" />
        <asp:Calendar ID="calBound" runat="server" Caption='<%# BoundText %>' SelectionMode="None" />
        <asp:Calendar ID="calHidden" runat="server" Visible="false" />
        <p>
            ImageMap:
            <asp:ImageMap ID="imapPlain" runat="server" ImageUrl="~/Images/probe.png" AlternateText="Areas" HotSpotMode="PostBack" OnClick="HotSpotClicked">
                <asp:RectangleHotSpot Left="0" Top="0" Right="10" Bottom="10" PostBackValue="rect" AlternateText="Rectangle" />
                <asp:CircleHotSpot X="15" Y="5" Radius="5" HotSpotMode="Navigate" NavigateUrl="~/ButtonTarget.aspx" AlternateText="Circle" />
            </asp:ImageMap>
            <asp:ImageMap ID="imapBound" runat="server" ImageUrl="~/Images/probe.png" AlternateText='<%# BoundText %>' />
            <asp:ImageMap ID="imapHidden" runat="server" ImageUrl="~/Images/probe.png" Visible="false" />
        </p>
        <p>Dynamic: <asp:PlaceHolder ID="Dynamic" runat="server" /></p>
        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CausesValidation="false" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" CausesValidation="false" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
