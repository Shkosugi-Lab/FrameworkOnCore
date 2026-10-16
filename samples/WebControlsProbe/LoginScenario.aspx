<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="LoginScenario.aspx.cs" Inherits="WebControlsProbe.LoginScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Login controls scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <p>
            Status: <asp:LoginStatus ID="lsStatus" runat="server" LogoutAction="Refresh" OnLoggingOut="LoggingOut" OnLoggedOut="LoggedOut" />
            Name: <asp:LoginName ID="lnName" runat="server" FormatString="Hello, {0}" />
        </p>
        <asp:LoginView ID="lvStatus" runat="server" OnViewChanged="ViewChanged">
            <AnonymousTemplate><p>Anonymous view</p></AnonymousTemplate>
            <LoggedInTemplate><p>Logged-in view: <asp:LoginName ID="lnInView" runat="server" /></p></LoggedInTemplate>
            <RoleGroups>
                <asp:RoleGroup Roles="admins"><ContentTemplate><p>Admins' view: <asp:LoginName ID="lnInRole" runat="server" /></p></ContentTemplate></asp:RoleGroup>
            </RoleGroups>
        </asp:LoginView>
        <asp:Login ID="lgnMain" runat="server" DestinationPageUrl="~/LoginScenario.aspx" OnLoggingIn="LoggingIn" OnAuthenticate="Authenticate" OnLoggedIn="LoggedInEvent" OnLoginError="LoginError" />
        <asp:Login ID="lgnTemplated" runat="server" DestinationPageUrl="~/LoginScenario.aspx">
            <LayoutTemplate>
                <p>Templated login: <asp:TextBox ID="UserName" runat="server" /> <asp:TextBox ID="Password" runat="server" TextMode="Password" />
                <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Sign in" /> <asp:Literal ID="FailureText" runat="server" /></p>
            </LayoutTemplate>
        </asp:Login>
        <asp:ChangePassword ID="cpwChange" runat="server" OnChangingPassword="ChangingPassword" OnChangedPassword="ChangedPassword" OnChangePasswordError="ChangePasswordError">
            <SuccessTemplate><p>The password changed. <asp:Button ID="ContinuePushButton" runat="server" CommandName="Continue" Text="Continue" /></p></SuccessTemplate>
        </asp:ChangePassword>
        <asp:PasswordRecovery ID="prRecover" runat="server" MailDefinition-From="probe@example.com" MailDefinition-Subject="Your password" OnVerifyingUser="VerifyingUser" OnVerifyingAnswer="VerifyingAnswer" OnSendingMail="SendingMail" OnUserLookupError="UserLookupError" OnAnswerLookupError="AnswerLookupError">
            <SuccessTemplate><p>The recovery mail prepared.</p></SuccessTemplate>
        </asp:PasswordRecovery>
        <asp:CreateUserWizard ID="cuwCreate" runat="server" LoginCreatedUser="false" OnCreatingUser="CreatingUser" OnCreatedUser="CreatedUser" OnCreateUserError="CreateUserError" OnActiveStepChanged="StepChanged">
            <WizardSteps>
                <asp:CreateUserWizardStep ID="cuwsCreate" runat="server" Title="Create">
                    <CustomNavigationTemplate><asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Text="Create the user" /></CustomNavigationTemplate>
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep ID="cwsComplete" runat="server" Title="Complete">
                    <ContentTemplate><p>The user was created. <asp:Button ID="ContinueButton" runat="server" CommandName="Continue" Text="Continue" /></p></ContentTemplate>
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
        <asp:Wizard ID="wizSteps" runat="server" OnActiveStepChanged="StepChanged" OnNextButtonClick="NextClicked" OnPreviousButtonClick="PreviousClicked" OnFinishButtonClick="FinishClicked" OnSideBarButtonClick="SideBarClicked">
            <HeaderTemplate>[wizard header]</HeaderTemplate>
            <WizardSteps>
                <asp:WizardStep ID="wsFirst" runat="server" Title="First" OnActivate="StepActivated" OnDeactivate="StepDeactivated">
                    First: <asp:TextBox ID="txtFirst" runat="server" />
                </asp:WizardStep>
                <asp:TemplatedWizardStep ID="twsSecond" runat="server" Title="Second" OnActivate="StepActivated" OnDeactivate="StepDeactivated">
                    <ContentTemplate>Second (templated): <asp:Label ID="lblSecond" runat="server" Text='<%# BoundText %>' /></ContentTemplate>
                </asp:TemplatedWizardStep>
                <asp:WizardStep ID="wsLast" runat="server" Title="Last" StepType="Finish" OnActivate="StepActivated" OnDeactivate="StepDeactivated">
                    Last: <asp:Label ID="lblLast" runat="server" />
                </asp:WizardStep>
                <asp:WizardStep ID="wsDone" runat="server" Title="Done" StepType="Complete" OnActivate="StepActivated">Done.</asp:WizardStep>
            </WizardSteps>
        </asp:Wizard>
        <asp:Wizard ID="wizHidden" runat="server" Visible="false">
            <WizardSteps><asp:WizardStep ID="wsHidden" runat="server" Title="Hidden">Hidden wizard</asp:WizardStep></WizardSteps>
        </asp:Wizard>
        <p>Dynamic: <asp:PlaceHolder ID="Dynamic" runat="server" /></p>
        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CausesValidation="false" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" CausesValidation="false" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
