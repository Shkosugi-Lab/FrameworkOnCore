using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace FrameworkOnCore.WebControlsParity
{
    /// <summary>
    /// A component the parity covers: its type and the instance its API cases call members on (made fresh for each
    /// member, inside a sample page: Samples). The probe site (samples/WebControlsProbe, ProbeComponents) has the same
    /// list for the rendering and scenario layers.
    /// </summary>
    public sealed class Component
    {
        public Component(string name, Type type, Func<Control> make, Func<Control[]> siblings = null, Func<Control, Control> wrap = null)
        {
            Name = name;
            Type = type;
            Make = make;
            Siblings = siblings;
            Wrap = wrap;
        }

        public string Name { get; }
        public Type Type { get; }
        public Func<Control> Make { get; }

        /// <summary>Controls the sample page has beside the component (a validator's ControlToValidate), or null.</summary>
        public Func<Control[]> Siblings { get; }

        /// <summary>The parent the component is in, made around it (a view in a multi-view), or null: in the form.</summary>
        public Func<Control, Control> Wrap { get; }

        public static readonly IReadOnlyList<Component> All = new[]
        {
            new Component("TextBox", typeof(TextBox), () => new TextBox { ID = "txtName", Text = "Taro" }),
            new Component("Button", typeof(Button), () => new Button { ID = "btnGo", Text = "Go", CommandName = "go", CommandArgument = "1" }),
            List("DropDownList", new DropDownList { ID = "ddlSize" }),
            List("ListBox", new ListBox { ID = "lstSize" }),
            List("CheckBoxList", new CheckBoxList { ID = "cblSize" }),
            List("RadioButtonList", new RadioButtonList { ID = "rblSize" }),
            List("BulletedList", new BulletedList { ID = "bltSize" }),
            new Component("Label", typeof(Label), () => new Label { ID = "lblName", Text = "Name" }),
            new Component("CheckBox", typeof(CheckBox), () => new CheckBox { ID = "chkAgree", Text = "Agree" }),
            new Component("RadioButton", typeof(RadioButton), () => new RadioButton { ID = "rdoAgree", Text = "Agree", GroupName = "answer" }),
            new Component("Literal", typeof(Literal), () => new Literal { ID = "litText", Text = "<b>literal</b>" }),
            new Component("LinkButton", typeof(LinkButton), () => new LinkButton { ID = "lnkGo", Text = "Go", CommandName = "go", CommandArgument = "1" }),
            new Component("HyperLink", typeof(HyperLink), () => new HyperLink { ID = "hypGo", Text = "Go", NavigateUrl = "~/Target.aspx?a=1&b=2" }),
            new Component("Image", typeof(Image), () => new Image { ID = "imgLogo", ImageUrl = "~/Images/probe.png", AlternateText = "Logo" }),
            new Component("ImageButton", typeof(ImageButton), () => new ImageButton { ID = "ibtnGo", ImageUrl = "~/Images/probe.png", CommandName = "go", CommandArgument = "1" }),
            new Component("Panel", typeof(Panel), () =>
            {
                var panel = new Panel { ID = "pnlBox" };
                panel.Controls.Add(new Label { ID = "lblInside", Text = "Inside" });
                panel.Controls.Add(new Button { ID = "btnInside", Text = "Inside" });
                return panel;
            }),
            new Component("HiddenField", typeof(HiddenField), () => new HiddenField { ID = "hidValue", Value = "initial" }),
            new Component("PlaceHolder", typeof(PlaceHolder), () =>
            {
                var holder = new PlaceHolder { ID = "phArea" };
                holder.Controls.Add(new Label { ID = "lblInside", Text = "Inside" });
                return holder;
            }),
            Validator("RequiredFieldValidator", () => new RequiredFieldValidator()),
            Validator("RegularExpressionValidator", () => new RegularExpressionValidator { ValidationExpression = "[0-9]+" }),
            Validator("CompareValidator", () => new CompareValidator { ValueToCompare = "3", Type = ValidationDataType.Integer, Operator = ValidationCompareOperator.GreaterThan }),
            Validator("CustomValidator", () => new CustomValidator { ClientValidationFunction = "checkOk" }),
            Validator("RangeValidator", () => new RangeValidator { MinimumValue = "1", MaximumValue = "10", Type = ValidationDataType.Integer }),
            new Component("ValidationSummary", typeof(ValidationSummary), () => new ValidationSummary { ID = "sumErrors", HeaderText = "Errors" }, ValidatorTargets),
            new Component("GridView", typeof(GridView), () => new GridView { ID = "gvItems", DataSource = new[] { "Pen", "Bag" } }),
            new Component("Repeater", typeof(Repeater), () => new Repeater { ID = "rptItems", ItemTemplate = new SampleTemplate(), DataSource = new[] { "Pen", "Bag" } }),
            new Component("FormView", typeof(FormView), () => new FormView { ID = "fvItem", ItemTemplate = new SampleTemplate(), DataSource = new[] { "Pen", "Bag" } }),
            new Component("DetailsView", typeof(DetailsView), () => new DetailsView { ID = "dvItem", DataSource = new[] { "Pen", "Bag" } }),
            new Component("ListView", typeof(ListView), () => new ListView { ID = "lvItems", ItemTemplate = new SampleTemplate(), DataSource = new[] { "Pen", "Bag" } }),
            new Component("DataPager", typeof(DataPager), () =>
            {
                var pager = new DataPager { ID = "dpPager", PagedControlID = "lvPaged", PageSize = 1 };
                pager.Fields.Add(new NumericPagerField());
                return pager;
            }, () => new Control[] { new ListView { ID = "lvPaged", ItemTemplate = new SampleTemplate(), DataSource = new[] { "Pen", "Bag" } } }),
            new Component("DataList", typeof(DataList), () => new DataList { ID = "dlItems", ItemTemplate = new SampleTemplate(), DataSource = new[] { "Pen", "Bag" } }),
            new Component("DataGrid", typeof(DataGrid), () => new DataGrid { ID = "dgItems", DataSource = new[] { "Pen", "Bag" } }),
            new Component("HtmlGenericControl", typeof(HtmlGenericControl), () => new HtmlGenericControl("span") { ID = "genBox", InnerText = "Generic" }),
            new Component("HtmlAnchor", typeof(HtmlAnchor), () => new HtmlAnchor { ID = "ancGo", HRef = "~/Target.aspx?a=1&b=2", InnerText = "Go" }),
            new Component("HtmlForm", typeof(HtmlForm), () => new HtmlForm { ID = "frmProbe" }),
            new Component("HtmlHead", typeof(HtmlHead), () => new HtmlHead { ID = "hdProbe" }),
            new Component("HtmlLink", typeof(HtmlLink), () => new HtmlLink { ID = "lnkCss", Href = "~/Images/probe.css" }),
            new Component("HtmlImage", typeof(HtmlImage), () => new HtmlImage { ID = "imgHtml", Src = "~/Images/probe.png", Alt = "Logo" }),
            new Component("HtmlTable", typeof(HtmlTable), () =>
            {
                var table = new HtmlTable { ID = "tblHtml" };
                table.Rows.Add(HtmlRow());
                return table;
            }),
            new Component("HtmlTableRow", typeof(HtmlTableRow), HtmlRow),
            new Component("HtmlTableCell", typeof(HtmlTableCell), () => new HtmlTableCell { ID = "tdCell", InnerText = "Cell" }),
            new Component("HtmlInputText", typeof(HtmlInputText), () => new HtmlInputText { ID = "inpName", Value = "Taro" }),
            new Component("HtmlInputPassword", typeof(HtmlInputPassword), () => new HtmlInputPassword { ID = "inpSecret", Value = "secret" }),
            new Component("HtmlInputHidden", typeof(HtmlInputHidden), () => new HtmlInputHidden { ID = "inpHidden", Value = "initial" }),
            new Component("HtmlInputCheckBox", typeof(HtmlInputCheckBox), () => new HtmlInputCheckBox { ID = "inpCheck" }),
            new Component("HtmlInputRadioButton", typeof(HtmlInputRadioButton), () => new HtmlInputRadioButton { ID = "inpRadio", Name = "answer" }),
            new Component("HtmlInputButton", typeof(HtmlInputButton), () => new HtmlInputButton { ID = "inpButton", Value = "Go" }),
            new Component("HtmlInputSubmit", typeof(HtmlInputSubmit), () => new HtmlInputSubmit { ID = "inpSubmit", Value = "Go" }),
            new Component("HtmlInputReset", typeof(HtmlInputReset), () => new HtmlInputReset { ID = "inpReset", Value = "Reset" }),
            new Component("HtmlInputImage", typeof(HtmlInputImage), () => new HtmlInputImage { ID = "inpImage", Src = "~/Images/probe.png", Alt = "Go" }),
            new Component("HtmlInputFile", typeof(HtmlInputFile), () => new HtmlInputFile { ID = "inpFile" }),
            new Component("HtmlInputGenericControl", typeof(HtmlInputGenericControl), () => new HtmlInputGenericControl("email") { ID = "inpEmail", Value = "a@example.com" }),
            new Component("HtmlButton", typeof(HtmlButton), () => new HtmlButton { ID = "btnHtml", InnerText = "Go" }),
            new Component("HtmlSelect", typeof(HtmlSelect), () =>
            {
                var select = new HtmlSelect { ID = "selSize" };
                select.Items.Add(new ListItem("Small", "S"));
                select.Items.Add(new ListItem("Medium", "M") { Selected = true });
                select.Items.Add(new ListItem("Large", "L"));
                return select;
            }),
            new Component("HtmlTextArea", typeof(HtmlTextArea), () => new HtmlTextArea { ID = "txaNote", Value = "Line1" }),
            new Component("HtmlMeta", typeof(HtmlMeta), () => new HtmlMeta { ID = "metaProbe", Name = "description", Content = "probe" }),
            new Component("HtmlTitle", typeof(HtmlTitle), () => new HtmlTitle { ID = "ttlProbe", Text = "Probe" }),
            new Component("HtmlIframe", typeof(HtmlIframe), () => new HtmlIframe { ID = "ifrProbe", Src = "~/Target.aspx" }),
            new Component("HtmlArea", typeof(HtmlArea), () => new HtmlArea { ID = "areaProbe", Href = "~/Target.aspx" }),
            new Component("HtmlAudio", typeof(HtmlAudio), () => new HtmlAudio { ID = "audProbe", Src = "~/Media/probe.mp3" }),
            new Component("HtmlVideo", typeof(HtmlVideo), () => new HtmlVideo { ID = "vidProbe", Src = "~/Media/probe.mp4", Poster = "~/Images/probe.png" }),
            new Component("HtmlSource", typeof(HtmlSource), () => new HtmlSource { ID = "srcProbe", Src = "~/Media/probe.mp4" }),
            new Component("HtmlTrack", typeof(HtmlTrack), () => new HtmlTrack { ID = "trkProbe", Src = "~/Media/probe.vtt" }),
            new Component("HtmlEmbed", typeof(HtmlEmbed), () => new HtmlEmbed { ID = "embProbe", Src = "~/Media/probe.swf" }),
            new Component("HtmlElement", typeof(HtmlElement), () => new HtmlElement { ID = "elmProbe" }),
            Part("Table", () =>
            {
                var table = new Table { ID = "tblItems" };
                table.Rows.Add(TableRowOf(new TableRow()));
                table.Rows.Add(TableRowOf(new TableRow()));
                return table;
            }),
            Part("TableRow", () => TableRowOf(new TableRow())),
            Part("TableHeaderRow", () => TableRowOf(new TableHeaderRow())),
            Part("TableFooterRow", () => TableRowOf(new TableFooterRow())),
            Part("TableCell", () => new TableCell { ID = "tdCell", Text = "Cell" }),
            Part("TableHeaderCell", () => new TableHeaderCell { ID = "thHead", Text = "Header" }),
            new Component("FileUpload", typeof(FileUpload), () => new FileUpload { ID = "fupFile" }),
            new Component("MultiView", typeof(MultiView), () =>
            {
                var views = new MultiView { ID = "mvSteps" };
                views.Views.Add(new View { ID = "vwFirst" });
                views.Views.Add(new View { ID = "vwSecond" });
                views.ActiveViewIndex = 0;
                return views;
            }),
            new Component("View", typeof(View), () => new View { ID = "vwProbe" }, wrap: view =>
            {
                var views = new MultiView { ID = "mvHost" };
                views.Views.Add(new View { ID = "vwOther" });
                views.Views.Add((View)view);
                return views;
            }),
            new Component("Localize", typeof(Localize), () => new Localize { ID = "locText", Text = "<b>localized</b>" }),
            new Component("Substitution", typeof(Substitution), () => new Substitution { ID = "subText", MethodName = "SubstitutionText" }),
            new Component("Xml", typeof(Xml), () => new Xml { ID = "xmlItems", DocumentContent = "<items><item>Pen</item></items>" }),
            new Component("AdRotator", typeof(AdRotator), () => new AdRotator { ID = "adBanner", AdvertisementFile = "~/App_Data/ads.xml" }),
            new Component("Calendar", typeof(Calendar), () => new Calendar { ID = "calDate", TodaysDate = new DateTime(2020, 1, 15), VisibleDate = new DateTime(2020, 1, 15) }),
            new Component("ImageMap", typeof(ImageMap), () =>
            {
                var map = new ImageMap { ID = "imapAreas", ImageUrl = "~/Images/probe.png" };
                map.HotSpots.Add(new RectangleHotSpot { Left = 0, Top = 0, Right = 10, Bottom = 10, PostBackValue = "rect" });
                return map;
            }),
            new Component("LiteralControl", typeof(LiteralControl), () => new LiteralControl("<b>literal</b>") { ID = "litControl" }),
            new Component("ContentPlaceHolder", typeof(ContentPlaceHolder), () => new ContentPlaceHolder { ID = "cphMain" }),
            new Component("Content", typeof(Content), () => new Content { ID = "cntMain" }),
            new Component("ScriptManager", typeof(ScriptManager), () => new ScriptManager { ID = "smProbe" }),
            new Component("ScriptManagerProxy", typeof(ScriptManagerProxy), () => new ScriptManagerProxy { ID = "smpProbe" }, wrap: AfterScriptManager),
            new Component("UpdatePanel", typeof(UpdatePanel), () => new UpdatePanel { ID = "upProbe" }, wrap: AfterScriptManager),
            new Component("UpdateProgress", typeof(UpdateProgress), () => new UpdateProgress { ID = "uprProbe", AssociatedUpdatePanelID = "upTarget" }, wrap: AfterScriptManager),
            new Component("Timer", typeof(Timer), () => new Timer { ID = "tmrProbe", Interval = 60000 }, wrap: AfterScriptManager),
            new Component("TreeView", typeof(TreeView), () =>
            {
                var tree = new TreeView { ID = "tvNodes" };
                var root = new TreeNode("Root", "r");
                root.ChildNodes.Add(new TreeNode("Child", "c"));
                tree.Nodes.Add(root);
                return tree;
            }),
            new Component("Menu", typeof(Menu), () =>
            {
                var menu = new Menu { ID = "mnuItems" };
                var products = new MenuItem("Products", "p");
                products.ChildItems.Add(new MenuItem("Pen", "pen"));
                menu.Items.Add(products);
                return menu;
            }),
            new Component("SiteMapPath", typeof(SiteMapPath), () => new SiteMapPath { ID = "smpPath" }),
            new Component("SiteMapDataSource", typeof(SiteMapDataSource), () => new SiteMapDataSource { ID = "smdsSite" }),
            new Component("Login", typeof(Login), () => new Login { ID = "lgnMain" }),
            new Component("LoginName", typeof(LoginName), () => new LoginName { ID = "lnName" }),
            new Component("LoginStatus", typeof(LoginStatus), () => new LoginStatus { ID = "lsStatus" }),
            new Component("LoginView", typeof(LoginView), () => new LoginView { ID = "lvStatus" }),
            new Component("Wizard", typeof(Wizard), () =>
            {
                var wizard = new Wizard { ID = "wizSteps" };
                wizard.WizardSteps.Add(new WizardStep { ID = "wsFirst", Title = "First" });
                wizard.WizardSteps.Add(new WizardStep { ID = "wsLast", Title = "Last", StepType = WizardStepType.Finish });
                return wizard;
            }),
            new Component("WizardStep", typeof(WizardStep), () => new WizardStep { ID = "wsProbe", Title = "Probe step" }, wrap: InWizard),
            new Component("TemplatedWizardStep", typeof(TemplatedWizardStep), () => new TemplatedWizardStep { ID = "twsProbe", Title = "Templated" }, wrap: InWizard),
            new Component("CreateUserWizard", typeof(CreateUserWizard), () => new CreateUserWizard { ID = "cuwCreate" }),
            new Component("CreateUserWizardStep", typeof(CreateUserWizardStep), () => new CreateUserWizardStep { ID = "cuwsCreate" }, wrap: step =>
            {
                var wizard = new CreateUserWizard { ID = "cuwHost" };
                wizard.WizardSteps.Add((WizardStepBase)step);
                wizard.WizardSteps.Add(new CompleteWizardStep { ID = "cwsComplete" });
                return wizard;
            }),
            new Component("CompleteWizardStep", typeof(CompleteWizardStep), () => new CompleteWizardStep { ID = "cwsComplete" }, wrap: step =>
            {
                var wizard = new CreateUserWizard { ID = "cuwHost" };
                wizard.WizardSteps.Add(new CreateUserWizardStep { ID = "cuwsCreate" });
                wizard.WizardSteps.Add((WizardStepBase)step);
                return wizard;
            }),
            new Component("ChangePassword", typeof(ChangePassword), () => new ChangePassword { ID = "cpwChange" }),
            new Component("PasswordRecovery", typeof(PasswordRecovery), () => new PasswordRecovery { ID = "prRecover" }),
            new Component("ObjectDataSource", typeof(ObjectDataSource), () => new ObjectDataSource { ID = "odsProducts", TypeName = "System.Collections.Generic.List`1[System.String]", SelectMethod = "ToArray" }),
            new Component("SqlDataSource", typeof(SqlDataSource), () => new SqlDataSource { ID = "sdsProducts", ProviderName = "System.Data.SqlClient", SelectCommand = "SELECT 1" }),
            new Component("AccessDataSource", typeof(AccessDataSource), () => new AccessDataSource { ID = "adsProducts", DataFile = "~/App_Data/probe.mdb", SelectCommand = "SELECT 1" }),
            new Component("XmlDataSource", typeof(XmlDataSource), () => new XmlDataSource { ID = "xdsProducts", Data = "<products><product code=\"P1\" name=\"Pen\" /></products>", EnableCaching = false }),
            new Component("LinqDataSource", typeof(LinqDataSource), () => new LinqDataSource { ID = "ldsProducts", ContextTypeName = "System.Object", TableName = "Products" }),
            new Component("QueryExtender", typeof(QueryExtender), () => new QueryExtender { ID = "qeProducts", TargetControlID = "ldsTarget" },
                () => new Control[] { new LinqDataSource { ID = "ldsTarget", ContextTypeName = "System.Object", TableName = "Products" } }),
            new Component("ModelErrorMessage", typeof(ModelErrorMessage), () => new ModelErrorMessage { ID = "memName", ModelStateKey = "Name" }),
            new Component("WebPartManager", typeof(WebPartManager), () => new WebPartManager { ID = "wpmProbe" }),
            new Component("ProxyWebPartManager", typeof(ProxyWebPartManager), () => new ProxyWebPartManager { ID = "pwpmProbe" }, wrap: AfterWebPartManager),
            new Component("WebPartZone", typeof(WebPartZone), () => new WebPartZone { ID = "wpzProbe" }, wrap: AfterWebPartManager),
            new Component("CatalogZone", typeof(CatalogZone), () => new CatalogZone { ID = "czProbe" }, wrap: AfterWebPartManager),
            new Component("DeclarativeCatalogPart", typeof(DeclarativeCatalogPart), () => new DeclarativeCatalogPart { ID = "dcpProbe" }, wrap: AfterWebPartManager),
            new Component("PageCatalogPart", typeof(PageCatalogPart), () => new PageCatalogPart { ID = "pcpProbe" }, wrap: AfterWebPartManager),
            new Component("ImportCatalogPart", typeof(ImportCatalogPart), () => new ImportCatalogPart { ID = "icpProbe" }, wrap: AfterWebPartManager),
            new Component("EditorZone", typeof(EditorZone), () => new EditorZone { ID = "ezProbe" }, wrap: AfterWebPartManager),
            new Component("AppearanceEditorPart", typeof(AppearanceEditorPart), () => new AppearanceEditorPart { ID = "aepProbe" }, wrap: AfterWebPartManager),
            new Component("BehaviorEditorPart", typeof(BehaviorEditorPart), () => new BehaviorEditorPart { ID = "bepProbe" }, wrap: AfterWebPartManager),
            new Component("LayoutEditorPart", typeof(LayoutEditorPart), () => new LayoutEditorPart { ID = "lepProbe" }, wrap: AfterWebPartManager),
            new Component("PropertyGridEditorPart", typeof(PropertyGridEditorPart), () => new PropertyGridEditorPart { ID = "pgepProbe" }, wrap: AfterWebPartManager),
            new Component("ConnectionsZone", typeof(ConnectionsZone), () => new ConnectionsZone { ID = "cnzProbe" }, wrap: AfterWebPartManager),
        };

        static Component Part(string name, Func<WebControl> make) => new Component(name, make().GetType(), make);

        /// <summary>The web parts' controls: after the page's web part manager, as they must be.</summary>
        static Control AfterWebPartManager(Control control)
        {
            var holder = new PlaceHolder();
            holder.Controls.Add(new WebPartManager { ID = "wpmPage" });
            holder.Controls.Add(control);
            return holder;
        }

        /// <summary>A wizard step: the second of a wizard.</summary>
        static Control InWizard(Control step)
        {
            var wizard = new Wizard { ID = "wizHost" };
            wizard.WizardSteps.Add(new WizardStep { ID = "wsBefore", Title = "Before" });
            wizard.WizardSteps.Add((WizardStepBase)step);
            return wizard;
        }

        /// <summary>The ASP.NET AJAX controls (System.Web.Extensions): after the page's script manager, as they must be.</summary>
        static Control AfterScriptManager(Control control)
        {
            var holder = new PlaceHolder();
            holder.Controls.Add(new ScriptManager { ID = "smPage" });
            holder.Controls.Add(control);
            return holder;
        }

        /// <summary>A table row (of the type given) with two cells (A, B).</summary>
        static TableRow TableRowOf(TableRow row)
        {
            row.ID = "trRow";
            row.Cells.Add(new TableCell { Text = "A" });
            row.Cells.Add(new TableCell { Text = "B" });
            return row;
        }

        /// <summary>A table row with two cells (A, B).</summary>
        static HtmlTableRow HtmlRow()
        {
            var row = new HtmlTableRow { ID = "trRow" };
            row.Cells.Add(new HtmlTableCell { InnerText = "A" });
            row.Cells.Add(new HtmlTableCell { InnerText = "B" });
            return row;
        }

        /// <summary>A template made in code: a literal of its container's data item.</summary>
        sealed class SampleTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                var literal = new Literal();
                literal.DataBinding += (sender, e) => literal.Text = "[" + DataBinder.GetDataItem(container) + "]";
                container.Controls.Add(literal);
            }
        }

        /// <summary>The text boxes a validator validates and compares with (txtTarget "abc", txtOther "xyz").</summary>
        static Control[] ValidatorTargets() => new Control[] { new TextBox { ID = "txtTarget", Text = "abc" }, new TextBox { ID = "txtOther", Text = "xyz" } };

        /// <summary>A validator of txtTarget (ValidatorTargets beside it), with an error message.</summary>
        static Component Validator(string name, Func<BaseValidator> make) => new Component(name, make().GetType(), () =>
        {
            var validator = make();
            validator.ID = "valCheck";
            validator.ControlToValidate = "txtTarget";
            validator.ErrorMessage = "Error message";
            return validator;
        }, ValidatorTargets);

        /// <summary>A list control with three items (Small/S, Medium/M, Large/L), the second selected.</summary>
        static Component List(string name, ListControl prototype) => new Component(name, prototype.GetType(), () =>
        {
            var list = (ListControl)Activator.CreateInstance(prototype.GetType());
            list.ID = prototype.ID;
            list.Items.Add(new ListItem("Small", "S"));
            list.Items.Add(new ListItem("Medium", "M") { Selected = true });
            list.Items.Add(new ListItem("Large", "L"));
            return list;
        });

        public static Component Named(string name) =>
            All.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("no component " + name + " (" + string.Join(", ", All.Select(c => c.Name)) + ")");
    }
}
