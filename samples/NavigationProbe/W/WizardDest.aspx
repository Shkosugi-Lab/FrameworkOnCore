<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>W/WizardDest</title></head>
<body>
  <form id="form1" runat="server">
    <h1>W/WizardDest</h1>
    <asp:Wizard ID="wiz" runat="server" FinishDestinationPageUrl="~/W/Finished.aspx?from=wizard" DisplaySideBar="false">
      <WizardSteps>
        <asp:WizardStep runat="server" Title="Only step" StepType="Start">start</asp:WizardStep>
        <asp:WizardStep runat="server" Title="Finish" StepType="Finish">finish here</asp:WizardStep>
      </WizardSteps>
    </asp:Wizard>
    <nav:Info runat="server" />
  </form>
</body>
</html>
