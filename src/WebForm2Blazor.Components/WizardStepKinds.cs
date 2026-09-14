namespace WebForm2Blazor.Components;

/// <summary>
/// System.Web.UI.WebControls.CreateUserWizardStep equivalent.
///
/// A WizardStep. WebForms distinguished it so the membership provider knew which step
/// collected the account details; here that is CreateUserWizard's job, and what is left is
/// an ordinary step.
///
/// A type of its own rather than an alias, because a designer file declares the field with
/// this exact name - n2's Users/New.aspx.designer.cs does - and the name has to resolve.
/// </summary>
public class CreateUserWizardStep : WizardStep
{
}

/// <summary>
/// System.Web.UI.WebControls.CompleteWizardStep equivalent: the confirmation step.
/// StepType is Complete by construction, which is what WebForms fixed it to - a parameter
/// the markup does not supply keeps the value set here.
/// </summary>
public class CompleteWizardStep : WizardStep
{
    public CompleteWizardStep() => StepType = "Complete";
}
