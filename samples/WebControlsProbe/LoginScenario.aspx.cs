using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The login controls' and the wizards' scenario layer (scenarios/Login.json, LoginName.json, LoginStatus.json,
    /// LoginView.json, ChangePassword.json, PasswordRecovery.json, CreateUserWizard.json, CreateUserWizardStep.json,
    /// CompleteWizardStep.json, Wizard.json, WizardStep.json, TemplatedWizardStep.json): forms authentication against the
    /// users in memory (ProbeMembership.cs; ?reset=1 puts them back as they were), a failed and a successful login, what the
    /// status, name and view show to the user (and to a role), a password changed, a password recovered (its mail
    /// cancelled, its text logged), a user created, a wizard's steps; ones made in code; a log of what the server saw.
    /// </summary>
    public partial class LoginScenario : Page
    {
        readonly List<string> entries = new List<string>();
        Wizard dynamicWizard;

        protected string BoundText => "bound";

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (Request.QueryString["reset"] == "1" && !IsPostBack)
            {
                ProbeMembershipProvider.Reset();
                FormsAuthentication.SignOut();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Made on every request: a login status and a wizard of two steps.
            Dynamic.Controls.Add(new LoginStatus { ID = "lsDynamic" });
            dynamicWizard = new Wizard { ID = "wizDynamic" };
            dynamicWizard.ActiveStepChanged += StepChanged;
            var first = new WizardStep { ID = "wsDynamicFirst", Title = "Dynamic first" };
            first.Controls.Add(new TextBox { ID = "txtDynamic" });
            dynamicWizard.WizardSteps.Add(first);
            dynamicWizard.WizardSteps.Add(new WizardStep { ID = "wsDynamicLast", Title = "Dynamic last", StepType = WizardStepType.Finish });
            Dynamic.Controls.Add(dynamicWizard);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack + " User=" + (User.Identity.IsAuthenticated ? User.Identity.Name : "(anonymous)"));
            if (IsPostBack) return;
            lblLast.Text = "set in code";
            DataBind();
        }

        protected void LoggingIn(object sender, LoginCancelEventArgs e) => entries.Add("LoggingIn " + lgnMain.UserName);

        protected void Authenticate(object sender, AuthenticateEventArgs e)
        {
            e.Authenticated = Membership.ValidateUser(lgnMain.UserName, lgnMain.Password);
            entries.Add("Authenticate " + lgnMain.UserName + " Authenticated=" + e.Authenticated);
        }

        protected void LoggedInEvent(object sender, EventArgs e) => entries.Add("LoggedIn " + lgnMain.UserName);

        protected void LoginError(object sender, EventArgs e) => entries.Add("LoginError " + lgnMain.UserName);

        protected void LoggingOut(object sender, LoginCancelEventArgs e) => entries.Add("LoggingOut");

        protected void LoggedOut(object sender, EventArgs e) => entries.Add("LoggedOut");

        protected void ViewChanged(object sender, EventArgs e) => entries.Add("ViewChanged");

        protected void ChangingPassword(object sender, LoginCancelEventArgs e) => entries.Add("ChangingPassword " + cpwChange.UserName);

        protected void ChangedPassword(object sender, EventArgs e) => entries.Add("ChangedPassword " + cpwChange.UserName + " valid with the new one=" + Membership.ValidateUser(cpwChange.UserName, cpwChange.NewPassword));

        protected void ChangePasswordError(object sender, EventArgs e) => entries.Add("ChangePasswordError " + cpwChange.UserName);

        protected void VerifyingUser(object sender, LoginCancelEventArgs e) => entries.Add("VerifyingUser " + prRecover.UserName);

        protected void VerifyingAnswer(object sender, LoginCancelEventArgs e) => entries.Add("VerifyingAnswer " + prRecover.UserName + " Question=" + prRecover.Question);

        protected void UserLookupError(object sender, EventArgs e) => entries.Add("UserLookupError " + prRecover.UserName);

        protected void AnswerLookupError(object sender, EventArgs e) => entries.Add("AnswerLookupError " + prRecover.UserName);

        protected void SendingMail(object sender, MailMessageEventArgs e)
        {
            // Not sent (no mail server): what it would send, logged.
            entries.Add("SendingMail To=" + e.Message.To + " Subject=" + e.Message.Subject + " Body=" + e.Message.Body.Replace("\r\n", " ").Replace("\n", " ").Trim());
            e.Cancel = true;
        }

        protected void CreatingUser(object sender, LoginCancelEventArgs e) => entries.Add("CreatingUser " + cuwCreate.UserName);

        protected void CreatedUser(object sender, EventArgs e) => entries.Add("CreatedUser " + cuwCreate.UserName + " exists=" + (Membership.GetUser(cuwCreate.UserName) != null));

        protected void CreateUserError(object sender, CreateUserErrorEventArgs e) => entries.Add("CreateUserError " + e.CreateUserError);

        protected void StepChanged(object sender, EventArgs e)
        {
            var wizard = (Wizard)sender;
            entries.Add("ActiveStepChanged " + wizard.ID + " ActiveStepIndex=" + wizard.ActiveStepIndex);
        }

        protected void NextClicked(object sender, WizardNavigationEventArgs e) => entries.Add("NextButtonClick " + e.CurrentStepIndex + " -> " + e.NextStepIndex);

        protected void PreviousClicked(object sender, WizardNavigationEventArgs e) => entries.Add("PreviousButtonClick " + e.CurrentStepIndex + " -> " + e.NextStepIndex);

        protected void FinishClicked(object sender, WizardNavigationEventArgs e) => entries.Add("FinishButtonClick " + e.CurrentStepIndex + " -> " + e.NextStepIndex + " First=\"" + txtFirst.Text + "\"");

        protected void SideBarClicked(object sender, WizardNavigationEventArgs e) => entries.Add("SideBarButtonClick " + e.CurrentStepIndex + " -> " + e.NextStepIndex);

        protected void StepActivated(object sender, EventArgs e) => entries.Add("Activate " + ((Control)sender).ID);

        protected void StepDeactivated(object sender, EventArgs e) => entries.Add("Deactivate " + ((Control)sender).ID);

        protected void ShowHidden(object sender, EventArgs e)
        {
            wizHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            entries.Add("State users: " + string.Join(", ", Membership.GetAllUsers().Cast<MembershipUser>().Select(u => u.UserName)));
            entries.Add("State wizSteps ActiveStepIndex=" + wizSteps.ActiveStepIndex + " txtFirst=\"" + txtFirst.Text + "\"");
            entries.Add("State wizDynamic ActiveStepIndex=" + dynamicWizard.ActiveStepIndex);
            entries.Add("State cuwCreate ActiveStepIndex=" + cuwCreate.ActiveStepIndex);
            entries.Add("State User=" + (User.Identity.IsAuthenticated ? User.Identity.Name + " admins=" + User.IsInRole("admins") : "(anonymous)"));
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>Login scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
