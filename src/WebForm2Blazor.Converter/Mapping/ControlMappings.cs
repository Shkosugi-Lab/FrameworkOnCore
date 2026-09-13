namespace WebForm2Blazor.Converter.Mapping;

/// <summary>Conversion rule for one kind of WebForms server control.</summary>
public sealed record ControlMapping
{
    /// <summary>Name of the target Blazor compatibility component.</summary>
    public required string Component { get; init; }

    /// <summary>ASPX attribute name -> Blazor parameter name.</summary>
    public required Dictionary<string, string> Attributes { get; init; }

    /// <summary>Whether to generate a code-behind field when the control has an ID.</summary>
    public bool CreatesField { get; init; } = true;

    /// <summary>
    /// Whether the smoke test may assert "an element with this ID exists in the DOM".
    /// True only for controls that always render (validators render only on failure,
    /// GridView only when it has rows, so those are excluded).
    /// </summary>
    public bool AssertPresence { get; init; }

    /// <summary>Parameters always emitted regardless of markup (e.g. a legacy-rendering flag).</summary>
    public Dictionary<string, string> FixedParameters { get; init; } = [];
}

/// <summary>
/// The mapping table. Generalization proceeds by growing this table.
/// Even after the AI conversion layer is introduced, recurring patterns the AI finds
/// are "promoted" here to widen the deterministic conversion's coverage.
/// </summary>
public static class ControlMappings
{
    /// <summary>
    /// Attributes that have no meaning in Blazor (= dropping them does not change behavior).
    /// Reported as information, not as warnings.
    /// </summary>
    public static readonly HashSet<string> KnownNoOpAttributes = new(StringComparer.OrdinalIgnoreCase)
    {
        "runat", "AutoPostBack", "EnableViewState", "ViewStateMode", "ViewStateEncryptionMode",
        "ClientIDMode", "EnableClientScript", "EnableTheming", "SkinID", "meta:resourcekey",
        // Client-side validation functions are replaced by server-side OnServerValidate
        "ClientValidationFunction",
        // RadioButtonList: only the Table layout is implemented (the WebForms default)
        "RepeatLayout",
        // Focus-move / submit-behavior tweaks have no effect in Blazor
        "SetFocusOnError", "UseSubmitBehavior", "AutoCompleteType",
    };

    /// <summary>
    /// Styles written as child elements of GridView etc. (&lt;HeaderStyle CssClass=... /&gt;).
    /// The emitter flattens them into "HeaderStyle-CssClass"-style attributes.
    /// </summary>
    public static readonly HashSet<string> StyleChildElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "HeaderStyle", "RowStyle", "AlternatingRowStyle", "FooterStyle",
        "PagerStyle", "SelectedRowStyle", "EditRowStyle", "EmptyDataRowStyle",
        "ItemStyle", "ControlStyle", "PagerSettings",
        // DataGrid / DataList / DetailsView name the same slots differently. Missing one
        // is not a cosmetic gap: the element is then left as child markup and a component
        // that only accepts templates fails to compile (RZ9996).
        "AlternatingItemStyle", "EditItemStyle", "SelectedItemStyle", "SeparatorStyle",
        "InsertItemStyle", "InsertRowStyle", "CommandRowStyle", "FieldHeaderStyle",
        "EmptyItemStyle", "GroupSeparatorStyle",
    };

    /// <summary>Data-bound templates (RenderFragment with a context).</summary>
    public static readonly HashSet<string> DataBoundTemplates = new(StringComparer.OrdinalIgnoreCase)
    {
        "ItemTemplate", "AlternatingItemTemplate", "EditItemTemplate",
        "InsertItemTemplate", "SelectedItemTemplate",
    };

    /// <summary>
    /// Template child elements whose WebForms name is also the name of a COLLECTION that
    /// code-behind mutates (grid.Columns.Add(...)). The markup half is emitted under a
    /// different parameter so the collection can keep the original name.
    /// </summary>
    public static readonly Dictionary<string, string> TemplateParameterNames =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Columns"] = "ColumnsContent",
        };

    /// <summary>Templates / child elements that take no context.</summary>
    public static readonly HashSet<string> PlainTemplates = new(StringComparer.OrdinalIgnoreCase)
    {
        "HeaderTemplate", "FooterTemplate", "SeparatorTemplate", "ItemSeparatorTemplate",
        "EmptyDataTemplate", "ContentTemplate", "LayoutTemplate", "GroupTemplate",
        "EmptyItemTemplate", "Columns", "Items", "Fields",
    };

    /// <summary>
    /// Templates that may contain unclosed HTML.
    /// (In WebForms it is idiomatic to open a &lt;ul&gt; in HeaderTemplate and close it in
    ///  FooterTemplate, but Razor requires matching tags, so that cannot convert as-is.)
    /// </summary>
    public static readonly HashSet<string> MayBeUnbalancedTemplates = new(StringComparer.OrdinalIgnoreCase)
    {
        "HeaderTemplate", "FooterTemplate", "SeparatorTemplate",
    };

    private static Dictionary<string, string> Map(params string[] names)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in names)
        {
            result[name] = name;
        }
        return result;
    }

    /// <summary>Adds renames (ASPX name -> differently named Blazor parameter) to a map.</summary>
    private static Dictionary<string, string> Rename(
        Dictionary<string, string> attrs, params (string Aspx, string Blazor)[] renames)
    {
        foreach (var (aspx, blazor) in renames)
        {
            attrs[aspx] = blazor;
        }
        return attrs;
    }

    /// <summary>
    /// Style attributes shared by all WebControl-derived controls.
    /// WebFormsControlBase implements them in one place, so the mappings are added in bulk too.
    /// (Literal / Repeater / ListItem / BoundField are not WebControls and are excluded.)
    /// </summary>
    private static readonly (string Aspx, string Blazor)[] CommonWebControlAttrs =
    [
        ("Width", "Width"), ("Height", "Height"),
        ("ToolTip", "ToolTip"), ("TabIndex", "TabIndex"), ("AccessKey", "AccessKey"),
        ("BackColor", "BackColor"), ("ForeColor", "ForeColor"),
        ("BorderColor", "BorderColor"), ("BorderWidth", "BorderWidth"), ("BorderStyle", "BorderStyle"),
        ("Font-Bold", "FontBold"), ("Font-Italic", "FontItalic"), ("Font-Underline", "FontUnderline"),
        ("Font-Size", "FontSize"), ("Font-Names", "FontName"), ("Font-Name", "FontName"),
        // WebFormsControlBase implements Enabled for every WebControl; a disabled
        // validator in particular must not run (measured: it stays valid).
        ("Enabled", "Enabled"),
    ];

    private static Dictionary<string, string> WithCommon(Dictionary<string, string> attrs)
    {
        foreach (var (aspx, blazor) in CommonWebControlAttrs)
        {
            attrs[aspx] = blazor;
        }
        return attrs;
    }

    /// <summary>
    /// GridView row-style attributes ("HeaderStyle-CssClass" form).
    /// The child-element form (&lt;HeaderStyle CssClass=... /&gt;) is flattened by the
    /// emitter into the same keys.
    /// </summary>
    private static Dictionary<string, string> WithRowStyles(Dictionary<string, string> attrs)
    {
        foreach (var style in new[] { "HeaderStyle", "RowStyle", "AlternatingRowStyle" })
        {
            attrs[$"{style}-CssClass"] = $"{style}CssClass";
            attrs[$"{style}-BackColor"] = $"{style}BackColor";
            attrs[$"{style}-ForeColor"] = $"{style}ForeColor";
            attrs[$"{style}-Font-Bold"] = $"{style}FontBold";
        }
        attrs["PagerStyle-CssClass"] = "PagerStyleCssClass";
        attrs["FooterStyle-CssClass"] = "FooterStyleCssClass";
        return attrs;
    }

    /// <summary>
    /// Per-field cell styles on BoundField / TemplateField ("ItemStyle-Width" form).
    /// The child-element form (&lt;ItemStyle Width=... /&gt;) is flattened by the emitter
    /// into the same keys.
    /// </summary>
    private static Dictionary<string, string> WithFieldStyles(Dictionary<string, string> attrs)
    {
        foreach (var style in new[] { "ItemStyle", "HeaderStyle" })
        {
            attrs[$"{style}-Width"] = $"{style}Width";
            attrs[$"{style}-HorizontalAlign"] = $"{style}HorizontalAlign";
            attrs[$"{style}-CssClass"] = $"{style}CssClass";
        }
        return attrs;
    }

    /// <summary>DataList item styles applied to the item cells.</summary>
    private static Dictionary<string, string> WithDataListItemStyles(Dictionary<string, string> attrs)
    {
        attrs["ItemStyle-CssClass"] = "ItemStyleCssClass";
        attrs["ItemStyle-HorizontalAlign"] = "ItemStyleHorizontalAlign";
        attrs["ItemStyle-VerticalAlign"] = "ItemStyleVerticalAlign";
        return attrs;
    }

    private static readonly Dictionary<string, ControlMapping> Table = new(StringComparer.OrdinalIgnoreCase)
    {
        ["TextBox"] = new()
        {
            Component = "TextBox",
            Attributes = WithCommon(Map("ID", "Text", "CssClass", "TextMode", "ReadOnly", "MaxLength", "Rows", "Columns", "Wrap", "Enabled", "Visible", "ValidationGroup")),
            AssertPresence = true,
        },
        ["Button"] = new()
        {
            Component = "Button",
            Attributes = WithCommon(Map("ID", "Text", "CssClass", "OnClick", "CausesValidation", "Enabled", "Visible",
                "OnClientClick", "ValidationGroup", "CommandName", "CommandArgument", "OnCommand")),
            AssertPresence = true,
        },
        ["Label"] = new()
        {
            Component = "Label",
            Attributes = WithCommon(Map("ID", "Text", "CssClass", "Visible", "AssociatedControlID")),
            AssertPresence = true,
        },
        ["Literal"] = new()
        {
            Component = "Literal",
            Attributes = Map("ID", "Text", "Visible"),
        },
        ["HyperLink"] = new()
        {
            Component = "HyperLink",
            Attributes = WithCommon(Map("ID", "Text", "NavigateUrl", "Target", "ImageUrl", "CssClass", "Visible")),
            AssertPresence = true,
        },
        ["LinkButton"] = new()
        {
            Component = "LinkButton",
            Attributes = WithCommon(Map("ID", "Text", "OnClick", "CausesValidation", "CssClass", "Visible",
                "OnClientClick", "ValidationGroup", "CommandName", "CommandArgument", "OnCommand")),
            AssertPresence = true,
        },
        ["CheckBox"] = new()
        {
            Component = "CheckBox",
            Attributes = WithCommon(Map("ID", "Text", "Checked", "OnCheckedChanged", "CssClass", "Enabled", "Visible", "TextAlign", "ValidationGroup")),
            AssertPresence = true,
        },
        ["Panel"] = new()
        {
            Component = "Panel",
            Attributes = WithCommon(Map("ID", "CssClass", "Visible", "GroupingText", "DefaultButton")),
            AssertPresence = true,
        },
        ["DropDownList"] = new()
        {
            Component = "DropDownList",
            Attributes = WithCommon(Map("ID", "CssClass", "DataTextField", "DataValueField", "OnSelectedIndexChanged", "Enabled", "Visible", "AppendDataBoundItems", "ItemType", "SelectMethod", "SelectedValue")),
            AssertPresence = true,
        },
        ["ListItem"] = new()
        {
            Component = "ListItem",
            Attributes = Map("Text", "Value", "Selected"),
            CreatesField = false,
        },
        ["GridView"] = new()
        {
            Component = "GridView",
            Attributes = WithCommon(WithRowStyles(Map("ID", "CssClass", "AutoGenerateColumns", "EmptyDataText", "Visible",
                "AllowPaging", "PageSize", "OnPageIndexChanging",
                "AllowSorting", "OnSorting", "PageIndex", "OnRowCommand",
                "OnRowDataBound", "OnRowDeleting", "OnRowEditing",
                "GridLines", "CellPadding", "CellSpacing",
                "Caption", "ShowHeader", "ShowFooter", "UseAccessibleHeader", "EnableModelValidation",
                "DataKeyNames", "DataSourceID", "ItemType", "SelectMethod"))),
        },
        ["BoundField"] = new()
        {
            Component = "BoundField",
            Attributes = WithFieldStyles(Map("DataField", "HeaderText", "DataFormatString", "SortExpression", "ReadOnly")),
            CreatesField = false,
        },
        ["TemplateField"] = new()
        {
            Component = "TemplateField",
            Attributes = WithFieldStyles(Map("HeaderText", "SortExpression")),
            CreatesField = false,
        },
        ["RadioButtonList"] = new()
        {
            Component = "RadioButtonList",
            Attributes = WithCommon(Map("ID", "CssClass", "DataTextField", "DataValueField",
                "OnSelectedIndexChanged", "RepeatDirection", "RepeatColumns", "Enabled", "Visible", "AppendDataBoundItems", "ValidationGroup")),
            AssertPresence = true,
        },
        ["ListView"] = new()
        {
            // Renders no wrapper element, so the common WebControl (style) attributes do not apply
            Component = "ListView",
            Attributes = Map("ID", "Visible", "OnItemDataBound", "DataSourceID", "ItemType", "SelectMethod", "DataKeyNames", "GroupItemCount"),
        },
        ["FormView"] = new()
        {
            Component = "FormView",
            Attributes = WithCommon(Map("ID", "CssClass", "Visible", "ItemType", "SelectMethod", "RenderOuterTable")),
        },
        ["PlaceHolder"] = new()
        {
            Component = "PlaceHolder",
            Attributes = Map("ID", "Visible"),
        },
        // The Wizard family, the largest group of unimplemented standard controls (17
        // across the corpora). CreateUserWizard is a Wizard whose steps happened to create
        // an account - the account creation belonged to the membership provider, which is
        // Identity's job in the converted app, so what is reproduced is the step structure.
        ["Wizard"] = new()
        {
            Component = "Wizard",
            Attributes = Map(
                "ID", "Visible", "ActiveStepIndex", "DisplaySideBar",
                "FinishCompleteButtonText", "StartNextButtonText",
                "StepNextButtonText", "StepPreviousButtonText",
                // The navigation handlers. Dropping these leaves a wizard that moves
                // between steps while the page never learns that it did - YAF's installer
                // does all of its work in them.
                "OnActiveStepChanged", "OnNextButtonClick",
                "OnPreviousButtonClick", "OnFinishButtonClick"),
        },
        // CreateUserWizard / CreateUserWizardStep / CompleteWizardStep are NOT mapped onto
        // these. MEASURED: it costs 11 build errors in BlogEngine alone. The account
        // wizard is not a Wizard with different steps - its code-behind uses an API of its
        // own (CreateUserStep, ContinueDestinationPageUrl, UserName, Password), and its
        // markup nests named templates a plain WizardStep does not accept. Making it work
        // means a component of its own, not an alias for this one.
        ["WizardStep"] = new()
        {
            Component = "WizardStep",
            Attributes = Map("ID", "Visible", "Title", "StepType", "AllowReturn"),
        },
        // MultiView / View: "show one child of several". Without them the whole switched
        // region rendered as an unconverted-control comment - every pane of it gone.
        ["MultiView"] = new()
        {
            Component = "MultiView",
            Attributes = Map("ID", "Visible", "ActiveViewIndex"),
        },
        ["View"] = new()
        {
            Component = "View",
            Attributes = Map("ID", "Visible"),
        },
        ["Repeater"] = new()
        {
            Component = "Repeater",
            // DataSourceID is how WebForms bound most Repeaters - no code-behind call, just
            // the attribute. Dropping it rendered thirteen of them across the corpora as
            // nothing at all, which is a silent behaviour change, not a missing feature.
            Attributes = Map(
                "ID", "Visible", "OnItemCommand", "OnItemDataBound", "DataSourceID", "DataMember"),
        },
        ["RequiredFieldValidator"] = new()
        {
            Component = "RequiredFieldValidator",
            Attributes = WithCommon(Map("ID", "ControlToValidate", "ErrorMessage", "Text", "InitialValue", "CssClass", "Visible", "ValidationGroup", "Display")),
        },
        ["RangeValidator"] = new()
        {
            Component = "RangeValidator",
            Attributes = WithCommon(Map("ID", "ControlToValidate", "ErrorMessage", "Text", "CssClass", "Visible", "ValidationGroup", "Display",
                "MinimumValue", "MaximumValue", "Type")),
        },
        ["CompareValidator"] = new()
        {
            Component = "CompareValidator",
            Attributes = WithCommon(Map("ID", "ControlToValidate", "ErrorMessage", "Text", "CssClass", "Visible", "ValidationGroup", "Display",
                "ControlToCompare", "ValueToCompare", "Operator", "Type")),
        },
        ["RegularExpressionValidator"] = new()
        {
            Component = "RegularExpressionValidator",
            Attributes = WithCommon(Map("ID", "ControlToValidate", "ErrorMessage", "Text", "CssClass", "Visible", "ValidationGroup", "Display",
                "ValidationExpression")),
        },
        ["CustomValidator"] = new()
        {
            Component = "CustomValidator",
            Attributes = WithCommon(Map("ID", "ControlToValidate", "ErrorMessage", "Text", "CssClass", "Visible", "ValidationGroup", "Display",
                "OnServerValidate", "ValidateEmptyText")),
        },
        ["ValidationSummary"] = new()
        {
            Component = "ValidationSummary",
            Attributes = WithCommon(Map("ID", "CssClass", "Visible", "ValidationGroup",
                "HeaderText", "DisplayMode", "ShowSummary", "ShowMessageBox", "ShowModelStateErrors")),
        },
        ["HiddenField"] = new()
        {
            Component = "HiddenField",
            Attributes = Map("ID", "Value", "Visible"),
            AssertPresence = true,
        },
        ["Image"] = new()
        {
            Component = "Image",
            Attributes = WithCommon(Map("ID", "ImageUrl", "AlternateText", "ImageAlign", "CssClass", "Visible")),
            AssertPresence = true,
        },
        ["RadioButton"] = new()
        {
            Component = "RadioButton",
            Attributes = WithCommon(Map("ID", "Text", "Checked", "GroupName", "OnCheckedChanged", "CssClass", "Enabled", "Visible", "TextAlign", "ValidationGroup")),
            AssertPresence = true,
        },
        ["CheckBoxList"] = new()
        {
            Component = "CheckBoxList",
            Attributes = WithCommon(Map("ID", "CssClass", "DataTextField", "DataValueField",
                "OnSelectedIndexChanged", "RepeatDirection", "RepeatColumns", "Enabled", "Visible", "AppendDataBoundItems", "ValidationGroup")),
            AssertPresence = true,
        },
        ["FileUpload"] = new()
        {
            Component = "FileUpload",
            Attributes = WithCommon(Map("ID", "CssClass", "Enabled", "Visible")),
            AssertPresence = true,
        },
        ["DataList"] = new()
        {
            Component = "DataList",
            Attributes = WithDataListItemStyles(WithCommon(Map("ID", "CssClass", "Visible", "RepeatColumns", "RepeatDirection",
                "CellPadding", "CellSpacing", "OnItemCommand", "OnItemDataBound", "DataKeyField"))),
        },
        ["ImageButton"] = new()
        {
            Component = "ImageButton",
            Attributes = WithCommon(Map("ID", "ImageUrl", "AlternateText", "OnClick", "CausesValidation",
                "ValidationGroup", "OnClientClick", "CommandName", "CommandArgument", "OnCommand",
                "CssClass", "Enabled", "Visible")),
            AssertPresence = true,
        },
        ["ListBox"] = new()
        {
            Component = "ListBox",
            Attributes = WithCommon(Map("ID", "CssClass", "DataTextField", "DataValueField",
                "OnSelectedIndexChanged", "Rows", "SelectionMode", "Enabled", "Visible", "AppendDataBoundItems")),
            AssertPresence = true,
        },
        ["Table"] = new()
        {
            Component = "Table",
            Attributes = WithCommon(Map("ID", "CssClass", "Visible", "CellPadding", "CellSpacing")),
            AssertPresence = true,
        },
        ["TableRow"] = new()
        {
            Component = "TableRow",
            Attributes = WithCommon(Map("ID", "CssClass", "Visible")),
        },
        ["TableCell"] = new()
        {
            Component = "TableCell",
            Attributes = WithCommon(Map("ID", "CssClass", "Visible", "Text", "ColumnSpan", "RowSpan")),
        },
        ["TableHeaderCell"] = new()
        {
            Component = "TableHeaderCell",
            Attributes = WithCommon(Map("ID", "CssClass", "Visible", "Text", "ColumnSpan", "RowSpan")),
        },
        ["DetailsView"] = new()
        {
            Component = "DetailsView",
            Attributes = WithCommon(Map("ID", "CssClass", "Visible", "AutoGenerateRows",
                "GridLines", "CellPadding", "CellSpacing", "ItemType", "SelectMethod")),
        },
        // --- Legacy DataGrid family (the GridView predecessor), mapped onto GridView ---
        ["DataGrid"] = new()
        {
            Component = "GridView",
            // DataGrid names a single key column; GridView takes a comma-separated list,
            // and one name is a valid list.
            Attributes = Rename(
                WithRowStyles(WithCommon(Map("ID", "CssClass", "AutoGenerateColumns", "Visible",
                    "CellPadding", "CellSpacing", "GridLines", "ShowHeader", "ShowFooter",
                    "UseAccessibleHeader"))),
                ("DataKeyField", "DataKeyNames")),
            FixedParameters = new Dictionary<string, string> { ["DataGridMode"] = "true" },
        },
        ["BoundColumn"] = new()
        {
            Component = "BoundField",
            Attributes = WithFieldStyles(Map("DataField", "HeaderText", "DataFormatString", "SortExpression")),
            CreatesField = false,
        },
        ["TemplateColumn"] = new()
        {
            Component = "TemplateField",
            Attributes = WithFieldStyles(Map("HeaderText", "SortExpression")),
            CreatesField = false,
        },
        ["ObjectDataSource"] = new()
        {
            // Declarative data source: renders nothing; controls with a DataSourceID
            // resolve it via the host registry and call Select()
            Component = "ObjectDataSource",
            Attributes = Map("ID", "TypeName", "SelectMethod", "SelectCountMethod",
                "EnablePaging", "StartRowIndexParameterName", "MaximumRowsParameterName"),
        },
        ["SiteMapDataSource"] = new()
        {
            // Also non-visual. Its nodes come from the application's own SiteMapProvider,
            // so converting it cannot change the DOM - it only stops the tag being
            // reported as an unsupported control.
            Component = "SiteMapDataSource",
            Attributes = Map("ID", "ShowStartingNode", "StartingNodeUrl", "StartFromCurrentNode",
                "StartingNodeOffset", "SiteMapProvider"),
        },
        ["TableHeaderRow"] = new()
        {
            Component = "TableHeaderRow",
            Attributes = WithCommon(Map("ID", "CssClass", "Visible", "TableSection")),
        },
        // --- AjaxControlToolkit (prefix-qualified: matched as ajaxToolkit:TabContainer) ---
        ["ajaxToolkit:TabContainer"] = new()
        {
            Component = "TabContainer",
            Attributes = WithCommon(Map("ID", "CssClass", "ActiveTabIndex", "Visible", "Enabled")),
            AssertPresence = true,
        },
        ["ajaxToolkit:TabPanel"] = new()
        {
            Component = "TabPanel",
            Attributes = Map("ID", "HeaderText", "CssClass", "Visible", "Enabled"),
            AssertPresence = true,
        },
    };

    /// <summary>
    /// Loads user-supplied mappings from a JSON file (the --control-map option):
    /// [{ "tag": "nopCommerce:Pager", "component": "My.Ns.PagerCompat",
    ///    "attributes": { "PageSize": "PageSize" }, "createsField": true }]
    /// Lets hand-ported custom controls plug into the deterministic conversion
    /// without modifying the converter.
    /// </summary>
    public static int LoadExternal(string jsonPath)
    {
        using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(jsonPath));
        var count = 0;
        foreach (var entry in document.RootElement.EnumerateArray())
        {
            var tag = entry.GetProperty("tag").GetString();
            var component = entry.GetProperty("component").GetString();
            if (string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(component))
            {
                continue;
            }

            var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (entry.TryGetProperty("attributes", out var attributeMap))
            {
                foreach (var pair in attributeMap.EnumerateObject())
                {
                    attributes[pair.Name] = pair.Value.GetString() ?? pair.Name;
                }
            }

            Table[tag] = new ControlMapping
            {
                Component = component,
                Attributes = attributes,
                CreatesField = !entry.TryGetProperty("createsField", out var createsField) || createsField.GetBoolean(),
                AssertPresence = entry.TryGetProperty("assertPresence", out var assertPresence) && assertPresence.GetBoolean(),
            };
            count++;
        }
        return count;
    }

    public static ControlMapping? Find(string aspxControlName)
        => Table.GetValueOrDefault(aspxControlName);

    /// <summary>
    /// Prefix-aware lookup. A "prefix:name" entry wins (third-party controls such as
    /// ajaxToolkit:TabPanel); the name-only table applies only to asp: (standard) controls,
    /// so a third-party control that happens to share a name is not mis-mapped.
    /// </summary>
    public static ControlMapping? Find(string prefix, string name)
    {
        if (!string.IsNullOrEmpty(prefix) && Table.TryGetValue($"{prefix}:{name}", out var qualified))
        {
            return qualified;
        }
        return string.IsNullOrEmpty(prefix) || prefix.Equals("asp", StringComparison.OrdinalIgnoreCase)
            ? Table.GetValueOrDefault(name)
            : null;
    }

    public static int Count => Table.Count;

    /// <summary>For the coverage audit: enumerates all mappings.</summary>
    public static IReadOnlyDictionary<string, ControlMapping> All => Table;
}
