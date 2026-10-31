<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Wizard.aspx.cs" Inherits="NavigationProbe.W.Wizard" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>W/Wizard</title></head>
<body>
  <form id="form1" runat="server">
    <h1>W/Wizard</h1>
    <asp:Wizard ID="wiz" runat="server" DisplaySideBar="true" DisplayCancelButton="true" CancelDestinationPageUrl="~/W/Cancelled.aspx"
        OnNextButtonClick="Next" OnPreviousButtonClick="Previous" OnSideBarButtonClick="SideBar" OnFinishButtonClick="Finish"
        OnActiveStepChanged="Changed" OnCancelButtonClick="Cancel">
      <WizardSteps>
        <asp:WizardStep ID="s1" runat="server" Title="Step 1">
          name: <asp:TextBox ID="txtName" runat="server" />
          <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" EnableClientScript="false" Text="name required" />
        </asp:WizardStep>
        <asp:WizardStep ID="s2" runat="server" Title="Step 2" AllowReturn="false">step two (no return to it)</asp:WizardStep>
        <asp:WizardStep ID="s3" runat="server" Title="Step 3">step three</asp:WizardStep>
        <asp:WizardStep ID="s4" runat="server" Title="Confirm" StepType="Finish">confirm: <asp:Literal ID="summary" runat="server" /></asp:WizardStep>
        <asp:WizardStep ID="s5" runat="server" Title="Done" StepType="Complete">done</asp:WizardStep>
      </WizardSteps>
    </asp:Wizard>
    <asp:Button ID="btnJump" runat="server" Text="ActiveStepIndex = 2" OnClick="Jump" CausesValidation="false" />
    <p id="active">active: <%: wiz.ActiveStepIndex %> (<%: wiz.ActiveStep.Title %>)</p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
