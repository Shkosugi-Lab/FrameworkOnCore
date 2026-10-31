<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AjaxWizard.aspx.cs" Inherits="NavigationProbe.W.AjaxWizard" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>W/AjaxWizard</title></head>
<body>
  <form id="form1" runat="server">
    <h1>W/AjaxWizard</h1>
    <asp:ScriptManager ID="sm" runat="server" />
    <asp:UpdatePanel ID="up" runat="server">
      <ContentTemplate>
        <asp:Wizard ID="wiz" runat="server" DisplaySideBar="true" OnActiveStepChanged="Changed">
          <WizardSteps>
            <asp:WizardStep runat="server" Title="A">step A</asp:WizardStep>
            <asp:WizardStep runat="server" Title="B">step B</asp:WizardStep>
            <asp:WizardStep runat="server" Title="C" StepType="Finish">step C</asp:WizardStep>
          </WizardSteps>
        </asp:Wizard>
        <nav:Info ID="panelInfo" runat="server" />
      </ContentTemplate>
    </asp:UpdatePanel>
    <nav:Info runat="server" />
  </form>
</body>
</html>
