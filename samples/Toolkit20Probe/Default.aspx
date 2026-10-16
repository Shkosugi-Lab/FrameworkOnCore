<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
    // The toolkit's controls, each what its scripts make of the page: tabs, an accordion, a watermark, a masked edit, a
    // calendar popup (a date of its own: not today's month), numeric up/down, a modal popup, a collapsible panel, a confirm
    // button (the dialog answered OK), an UpdatePanel's asynchronous postback, the server seeing what they posted.
    protected void Confirmed(object sender, EventArgs e)
    {
        Result.Text = "Confirmed; posted: watermark box '" + txtWatermark.Text + "', masked '" + txtMasked.Text + "', date '" + txtDate.Text +
            "', number '" + txtNumber.Text + "', active tab " + tabs.ActiveTabIndex + ", accordion pane " + accordion.SelectedIndex;
    }

    protected void Greet(object sender, EventArgs e)
    {
        lblGreeting.Text = "Hello, " + txtName.Text + " (asynchronous postback " + sm1.IsInAsyncPostBack + ")";
    }
</script>
<!DOCTYPE html>
<html>
<head runat="server">
  <title>Toolkit 20 probe</title>
</head>
<body>
  <form id="form1" runat="server">
    <asp:ScriptManager runat="server" ID="sm1" />
    <ajaxToolkit:TabContainer ID="tabs" runat="server" ActiveTabIndex="0">
      <ajaxToolkit:TabPanel ID="tabFirst" runat="server" HeaderText="First"><ContentTemplate>The first tab's content.</ContentTemplate></ajaxToolkit:TabPanel>
      <ajaxToolkit:TabPanel ID="tabSecond" runat="server" HeaderText="Second"><ContentTemplate>The second tab's content.</ContentTemplate></ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
    <ajaxToolkit:Accordion ID="accordion" runat="server" SelectedIndex="0" FadeTransitions="false" TransitionDuration="1" RequireOpenedPane="true">
      <Panes>
        <ajaxToolkit:AccordionPane ID="paneOne" runat="server">
          <Header><span id="headerOne">Pane one</span></Header>
          <Content>The first pane.</Content>
        </ajaxToolkit:AccordionPane>
        <ajaxToolkit:AccordionPane ID="paneTwo" runat="server">
          <Header><span id="headerTwo">Pane two</span></Header>
          <Content>The second pane.</Content>
        </ajaxToolkit:AccordionPane>
      </Panes>
    </ajaxToolkit:Accordion>
    <asp:TextBox ID="txtWatermark" runat="server" />
    <ajaxToolkit:TextBoxWatermarkExtender ID="wm" runat="server" TargetControlID="txtWatermark" WatermarkText="Type here" />
    <asp:TextBox ID="txtMasked" runat="server" />
    <ajaxToolkit:MaskedEditExtender ID="mask" runat="server" TargetControlID="txtMasked" Mask="999-9999" MaskType="Number" ClearMaskOnLostFocus="false" />
    <asp:TextBox ID="txtDate" runat="server" Text="2020-03-04" />
    <ajaxToolkit:CalendarExtender ID="cal" runat="server" TargetControlID="txtDate" Format="yyyy-MM-dd" />
    <asp:TextBox ID="txtNumber" runat="server" Text="5" />
    <ajaxToolkit:NumericUpDownExtender ID="updown" runat="server" TargetControlID="txtNumber" Width="80" Minimum="0" Maximum="10" />
    <asp:Button ID="btnShowModal" runat="server" Text="Show the modal" OnClientClick="return false;" />
    <asp:Panel ID="pnlModal" runat="server" style="display:none">The modal popup. <asp:Button ID="btnCloseModal" runat="server" Text="Close" OnClientClick="return false;" /></asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="modal" runat="server" TargetControlID="btnShowModal" PopupControlID="pnlModal" CancelControlID="btnCloseModal" />
    <asp:Panel ID="pnlHeader" runat="server">Details</asp:Panel>
    <asp:Panel ID="pnlBody" runat="server">The panel the header opens.</asp:Panel>
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeBody" runat="server" TargetControlID="pnlBody" ExpandControlID="pnlHeader" CollapseControlID="pnlHeader" Collapsed="true" />
    <asp:UpdatePanel ID="upGreeting" runat="server">
      <ContentTemplate>
        <asp:TextBox ID="txtName" runat="server" />
        <asp:Button ID="btnGreet" runat="server" Text="Greet" OnClick="Greet" />
        <p><asp:Label ID="lblGreeting" runat="server" /></p>
      </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnConfirm" runat="server" Text="Confirm" OnClick="Confirmed" />
    <ajaxToolkit:ConfirmButtonExtender ID="confirm" runat="server" TargetControlID="btnConfirm" ConfirmText="Sure?" />
    <p id="result"><asp:Literal ID="Result" runat="server" EnableViewState="false" /></p>
  </form>
</body>
</html>
