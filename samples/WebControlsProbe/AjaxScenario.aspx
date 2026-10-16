<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="AjaxScenario.aspx.cs" Inherits="WebControlsProbe.AjaxScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>AJAX controls scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="smPage" runat="server" EnableHistory="true" EnableSecureHistoryState="false" OnNavigate="Navigated" />
        <asp:ScriptManagerProxy ID="smpPage" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Images/probe.js" />
            </Scripts>
        </asp:ScriptManagerProxy>
        <p>Script: <span id="scriptOutput">(not run)</span></p>
        <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <p>
                    Main panel, rendered by request <asp:Label ID="lblMainRequest" runat="server" />
                    <asp:TextBox ID="txtInPanel" runat="server" />
                    <asp:Button ID="btnInPanel" runat="server" Text="Post the panel" OnClick="PanelClicked" />
                    <asp:Button ID="btnSlow" runat="server" Text="Slow" OnClick="SlowClicked" />
                    <asp:Button ID="btnHistory" runat="server" Text="History point" OnClick="HistoryClicked" />
                    <asp:Button ID="btnFull" runat="server" Text="Full postback" OnClick="PanelClicked" />
                </p>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnFull" />
            </Triggers>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="uprMain" runat="server" AssociatedUpdatePanelID="upMain" DisplayAfter="0">
            <ProgressTemplate><span id="progressText">Working...</span></ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="upTriggered" runat="server" UpdateMode="Conditional" RenderMode="Inline">
            <ContentTemplate>Triggered panel, rendered by request <asp:Label ID="lblTriggeredRequest" runat="server" /></ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnOutside" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <asp:Button ID="btnOutside" runat="server" Text="Outside, triggers the second panel" OnClick="OutsideClicked" />
        <asp:UpdatePanel ID="upBound" runat="server">
            <ContentTemplate><asp:Label ID="lblBound" runat="server" Text='<%# BoundText %>' /></ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="upHidden" runat="server" Visible="false">
            <ContentTemplate>Hidden panel <asp:Label ID="lblInHidden" runat="server" /></ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="upTimer" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                Timer panel, ticks: <asp:Label ID="lblTicks" runat="server" Text="0" />
                <asp:Timer ID="tmrMain" runat="server" Interval="1500" Enabled="false" OnTick="Ticked" />
                <asp:Button ID="btnStartTimer" runat="server" Text="Start the timer" OnClick="StartTimer" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <p>Dynamic: <asp:PlaceHolder ID="Dynamic" runat="server" /></p>
        <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" />
        <asp:UpdatePanel ID="upLog" runat="server" UpdateMode="Always">
            <ContentTemplate><pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre></ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
