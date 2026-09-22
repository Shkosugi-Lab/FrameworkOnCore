using Microsoft.AspNetCore.Components;

namespace WebForm2Blazor.Components;

/// <summary>
/// System.Web.UI.WebControls.CreateUserWizard equivalent.
///
/// A Wizard whose steps happen to create an account, so it derives from Wizard and renders
/// exactly as one - the steps, the sidebar and the navigation are the same control. What it
/// adds is the surface the page talks to: CreateUserStep, the account fields, and the
/// events.
///
/// It does NOT create the account. Membership is gone and the converted application signs
/// people in through ASP.NET Core, so CreatingUser / CreatedUser are raised and the page
/// decides - BlogEngine's register page does all of its work in CreatedUser (roles,
/// authentication cookie, redirect) and that code keeps working unchanged.
///
/// Mapping it onto a plain Wizard was tried and reverted: the page names CreateUserStep,
/// ContinueDestinationPageUrl, UserName and Password, none of which a Wizard has, and that
/// alone was 11 build errors in BlogEngine.
/// </summary>
public class CreateUserWizard : Wizard
{
    /// <summary>
    /// WebForms CreateUserStep: the step that collects the account details. The first
    /// step, as it is in the markup - the account wizard's first step IS the create step.
    /// </summary>
    public WizardStep CreateUserStep => WizardSteps.Count > 0 ? WizardSteps[0] : null;

    /// <summary>WebForms CompleteStep: the confirmation step, if the markup declares one.</summary>
    public WizardStep CompleteStep
        => WizardSteps.FirstOrDefault(step => step.StepType is "Complete")
           ?? (WizardSteps.Count > 1 ? WizardSteps[^1] : null);

    [Parameter] public string UserName { get; set; } = string.Empty;
    [Parameter] public string Password { get; set; } = string.Empty;
    [Parameter] public string Email { get; set; } = string.Empty;
    [Parameter] public string Question { get; set; } = string.Empty;
    [Parameter] public string Answer { get; set; } = string.Empty;
    [Parameter] public string MembershipProvider { get; set; } = string.Empty;
    [Parameter] public string ContinueDestinationPageUrl { get; set; } = string.Empty;
    [Parameter] public bool DisableCreatedUser { get; set; }
    [Parameter] public bool LoginCreatedUser { get; set; } = true;
    [Parameter] public bool RequireEmail { get; set; } = true;

    [Parameter] public EventHandler<LoginCancelEventArgs> OnCreatingUser { get; set; }
    [Parameter] public EventHandler OnCreatedUser { get; set; }
    [Parameter] public EventHandler OnCreateUserError { get; set; }
    [Parameter] public EventHandler OnContinueButtonClick { get; set; }

    public event EventHandler<LoginCancelEventArgs> CreatingUser
    {
        add => OnCreatingUser += value;
        remove => OnCreatingUser -= value;
    }

    public event EventHandler CreatedUser
    {
        add => OnCreatedUser += value;
        remove => OnCreatedUser -= value;
    }

    public event EventHandler CreateUserError
    {
        add => OnCreateUserError += value;
        remove => OnCreateUserError -= value;
    }

    /// <summary>
    /// Finishing the create step is what WebForms turned into the account creation, so
    /// that is where the events go. The base still raises FinishButtonClick and moves on.
    /// </summary>
    protected override void RaiseActiveStepChanged(object source, EventArgs e)
    {
        base.RaiseActiveStepChanged(source, e);

        if (ActiveStep is null || ActiveStep != CompleteStep || _created)
        {
            return;
        }

        _created = true;
        var cancel = new LoginCancelEventArgs();
        OnCreatingUser?.Invoke(this, cancel);
        if (cancel.Cancel)
        {
            OnCreateUserError?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnCreatedUser?.Invoke(this, EventArgs.Empty);
    }

    private bool _created;

    // --- The rest of the WebForms CreateUserWizard declaration surface.
    //     Same terms as ChangePassword: carried for the adapter that does the rendering,
    //     and the DEFAULTS are the substance - each one below is what 4.8 declares. ---
    public string HeaderText { get; set; } = "Sign Up for Your New Account";
    public string InstructionText { get; set; } = string.Empty;
    public string CompleteSuccessText { get; set; } = "Your account has been successfully created.";

    public string UserNameLabelText { get; set; } = "User Name:";
    public string PasswordLabelText { get; set; } = "Password:";
    public string ConfirmPasswordLabelText { get; set; } = "Confirm Password:";
    public string EmailLabelText { get; set; } = "E-mail:";
    public string QuestionLabelText { get; set; } = "Security Question:";
    public string AnswerLabelText { get; set; } = "Security Answer:";
    public string PasswordHintText { get; set; } = string.Empty;

    public string CreateUserButtonText { get; set; } = "Create User";
    public string CancelButtonText { get; set; } = "Cancel";
    public string ContinueButtonText { get; set; } = "Continue";
    public bool DisplayCancelButton { get; set; }

    public ButtonType CreateUserButtonType { get; set; } = ButtonType.Button;
    public ButtonType CancelButtonType { get; set; } = ButtonType.Button;
    public ButtonType ContinueButtonType { get; set; } = ButtonType.Button;

    public string CreateUserButtonImageUrl { get; set; } = string.Empty;
    public string CancelButtonImageUrl { get; set; } = string.Empty;
    public string ContinueButtonImageUrl { get; set; } = string.Empty;

    public string FinishDestinationPageUrl { get; set; } = string.Empty;
    public string EditProfileText { get; set; } = string.Empty;
    public string EditProfileUrl { get; set; } = string.Empty;
    public string EditProfileIconUrl { get; set; } = string.Empty;
    public string HelpPageText { get; set; } = string.Empty;
    public string HelpPageUrl { get; set; } = string.Empty;
    public string HelpPageIconUrl { get; set; } = string.Empty;

    public string PasswordRegularExpression { get; set; } = string.Empty;
    public string PasswordRegularExpressionErrorMessage { get; set; } = string.Empty;
    public string EmailRegularExpression { get; set; } = string.Empty;
    public string EmailRegularExpressionErrorMessage { get; set; } = string.Empty;

    public string UserNameRequiredErrorMessage { get; set; } = "User Name is required.";
    public string PasswordRequiredErrorMessage { get; set; } = "Password is required.";
    public string ConfirmPasswordRequiredErrorMessage { get; set; } = "Confirm Password is required.";
    public string ConfirmPasswordCompareErrorMessage { get; set; }
        = "The Password and Confirmation Password must match.";
    public string EmailRequiredErrorMessage { get; set; } = "E-mail is required.";
    public string QuestionRequiredErrorMessage { get; set; } = "Security question is required.";
    public string AnswerRequiredErrorMessage { get; set; } = "Security answer is required.";

    public string DuplicateUserNameErrorMessage { get; set; }
        = "Please enter a different user name.";
    public string DuplicateEmailErrorMessage { get; set; }
        = "Please enter a different e-mail address.";
    public string InvalidPasswordErrorMessage { get; set; }
        = "Password length minimum: {0}. Non-alphanumeric characters required: {1}.";
    public string InvalidEmailErrorMessage { get; set; } = "Please enter a valid e-mail address.";
    public string InvalidQuestionErrorMessage { get; set; } = "Please enter a different security question.";
    public string InvalidAnswerErrorMessage { get; set; } = "Please enter a different security answer.";
    public string UnknownErrorMessage { get; set; }
        = "Your account was not created. Please try again.";

    /// <summary>
    /// WebForms CreateUserWizard.LayoutTemplate. Null, and the compat wizard renders its
    /// own layout when it is - the branch an application that supplies none would take.
    /// </summary>
    public ITemplate LayoutTemplate { get; set; }

    // --- Style slots. The compat wizard renders none of them; mojoPortal's
    //     CreateUserWizardAdapter does its own rendering and reads the CssClass off each.
    //     Plain objects with the original's empty default - see ChangePassword for why
    //     these are not views over parameters. ---
    public TableItemStyle LabelStyle { get; } = new();
    public TableItemStyle TextBoxStyle { get; } = new();
    public TableItemStyle ValidatorTextStyle { get; } = new();
    public TableItemStyle TitleTextStyle { get; } = new();
    public TableItemStyle InstructionTextStyle { get; } = new();
    public TableItemStyle HeaderStyle { get; } = new();
    public TableItemStyle ErrorMessageStyle { get; } = new();
    public TableItemStyle HyperLinkStyle { get; } = new();
    public TableItemStyle CompleteSuccessTextStyle { get; } = new();
    public TableItemStyle CreateUserButtonStyle { get; } = new();
    public TableItemStyle ContinueButtonStyle { get; } = new();
    public TableItemStyle CancelButtonStyle { get; } = new();
    public TableItemStyle PasswordHintStyle { get; } = new();

    /// <summary>
    /// WebForms CreateUserWizard.NavigationStyle.HorizontalAlign (markup attribute form
    /// NavigationStyle-HorizontalAlign). Flattened rather than a TableItemStyle slot like
    /// the others above because it is the only sub-property of NavigationStyle seen in the
    /// corpora; carried on the same terms - the compat wizard renders none of them.
    /// </summary>
    [Parameter] public string NavigationStyleHorizontalAlign { get; set; }
}
