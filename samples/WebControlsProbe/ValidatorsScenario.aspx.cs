using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The validators' scenario layer (scenarios/RequiredFieldValidator.json, RegularExpressionValidator.json,
    /// CompareValidator.json, CustomValidator.json, RangeValidator.json, ValidationSummary.json, all on this page): the
    /// server's validation in a group (no client script), a group in the browser for each kind, validators disabled,
    /// hidden, made dynamically, with data-bound messages; and a log of what the server saw (the request's number, kept in
    /// view state: a postback the browser's validation held back is no request; each ServerValidate, the page's IsValid,
    /// every validator's IsValid).
    /// </summary>
    public partial class ValidatorsScenario : Page
    {
        readonly List<string> entries = new List<string>();
        BaseValidator[] dynamicValidators;
        ValidationSummary dynamicSummary;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Made on every request, before validation: one of each kind, validating txtDynamicTarget, in the server's group.
            dynamicValidators = new BaseValidator[]
            {
                new RequiredFieldValidator { ID = "reqDynamic", ErrorMessage = "Dynamic: required" },
                new RegularExpressionValidator { ID = "rexDynamic", ValidationExpression = "[0-9]+", ErrorMessage = "Dynamic: digits" },
                new RangeValidator { ID = "rngDynamic", MinimumValue = "1", MaximumValue = "9", Type = ValidationDataType.Integer, ErrorMessage = "Dynamic: 1 to 9" },
                new CompareValidator { ID = "cmpDynamic", ValueToCompare = "5", Operator = ValidationCompareOperator.LessThan, Type = ValidationDataType.Integer, ErrorMessage = "Dynamic: less than 5" },
                new CustomValidator { ID = "cusDynamic", ErrorMessage = "Dynamic: must be ok" },
            };
            ((CustomValidator)dynamicValidators[4]).ServerValidate += MustBeOk;
            foreach (var validator in dynamicValidators)
            {
                validator.ControlToValidate = "txtDynamicTarget";
                validator.ValidationGroup = "server";
                validator.EnableClientScript = false;
                validator.Display = ValidatorDisplay.Dynamic;
                Dynamic.Controls.Add(validator);
            }
            dynamicSummary = new ValidationSummary { ID = "sumDynamic", ValidationGroup = "server", HeaderText = "Dynamic summary", DisplayMode = ValidationSummaryDisplayMode.SingleParagraph, EnableClientScript = false };
            Dynamic.Controls.Add(dynamicSummary);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack);
            if (!IsPostBack) DataBind();
        }

        protected void Clicked(object sender, EventArgs e)
        {
            entries.Add("Click " + ((Control)sender).ID);
        }

        protected void MustBeOk(object source, ServerValidateEventArgs args)
        {
            args.IsValid = args.Value == "ok";
            entries.Add("ServerValidate " + ((Control)source).ID + ": \"" + args.Value + "\" -> " + args.IsValid);
        }

        protected void NotEmpty(object source, ServerValidateEventArgs args)
        {
            args.IsValid = args.Value.Length > 0;
            entries.Add("ServerValidate " + ((Control)source).ID + ": \"" + args.Value + "\" -> " + args.IsValid);
        }

        protected void NameNotForbidden(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtName.Text != "forbidden";
            entries.Add("ServerValidate " + ((Control)source).ID + ": (no control, value \"" + args.Value + "\") -> " + args.IsValid);
        }

        protected void ValidateCode(object sender, EventArgs e)
        {
            rexCode.Validate();
            entries.Add("rexCode.Validate(): IsValid=" + rexCode.IsValid);
        }

        protected void ValidateGroup(object sender, EventArgs e)
        {
            Validate("server");
            entries.Add("Page.Validate(\"server\"): IsValid=" + IsValid);
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            reqHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (IsPostBack)
            {
                string valid;
                try { valid = IsValid.ToString(); }
                catch (HttpException) { valid = "(not validated)"; }
                entries.Add("IsValid: " + valid);
            }
            entries.Add("Page.Validators: " + Validators.Count);
            foreach (var validator in Validators.Cast<IValidator>().OfType<BaseValidator>())
                entries.Add("Validator " + validator.ID + ": IsValid=" + validator.IsValid + (validator.Enabled ? "" : " (disabled)") + (validator.Visible ? "" : " (hidden)"));
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>Validators scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
