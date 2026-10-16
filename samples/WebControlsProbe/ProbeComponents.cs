using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The rendering layer's cases of a component (tests/WebControlsParity/README.md), made by reflection over its type, so
    /// that every property a page can set is covered, not the ones someone thought of:
    ///   (default)               the component as made (Make), nothing set
    ///   Property=value          each public settable property of a simple type (text, numbers, enums, units, colors;
    ///                           a sub-object's: Font.Bold), with each sample value of its type (SampleValues)
    ///   Attributes[..], Style[..]  the attribute and style collections
    ///   A=x+B=y                 the component's known interactions (Interactions), written by hand
    ///   [context]...            the same in a context: in a naming container (ClientIDMode), with a theme (skins)
    /// The same code makes the list on .NET Framework (the recording) and on the port; a name the port does not make
    /// (a property it lacks) is a case that says so.
    /// </summary>
    public sealed class ProbeComponent
    {
        public ProbeComponent(string name, Type type, Func<Control> make)
        {
            Name = name;
            Type = type;
            Make = make;
        }

        public string Name { get; private set; }
        public Type Type { get; private set; }
        public Func<Control> Make { get; private set; }

        /// <summary>More values for a property than its type's samples (TextBox.Text: lines, a leading line break).</summary>
        public Dictionary<string, List<Sample>> ExtraValues = new Dictionary<string, List<Sample>>();

        /// <summary>The data every case binds to in Load (a data control's), or null: only the "data-bound" context binds.</summary>
        public Func<object> Data;

        /// <summary>What a context does to the component before the case's settings (a grid's columns, a repeater's templates).</summary>
        public Dictionary<string, Action<Control>> ContextSetups = new Dictionary<string, Action<Control>>();

        /// <summary>Controls added after the component, beside it (a Label's AssociatedControlID's text box).</summary>
        public Func<Control[]> Siblings;

        /// <summary>The parent the component is in, made around it (a view in a multi-view), or null: in the form.</summary>
        public Func<Control, Control> Wrap;

        /// <summary>The data the "[data-bound]" cases bind to, or null: ProbeData.Products().</summary>
        public Func<object> BoundData;

        /// <summary>An item of a list the component has ("Items[1]"): each of the item's settable properties is a case too.</summary>
        public string ItemPath;

        /// <summary>Properties set together, as "A=label+B=label": the interactions known to matter.</summary>
        public List<string> Interactions = new List<string>();

        /// <summary>
        /// Cases in a context, each a list of case bodies ("ClientIDMode=Static"): "naming container" (in a naming
        /// container), "themed" (with a theme), "data-bound" (DataSource: ProbeData.Products(), then DataBind in Load).
        /// </summary>
        public Dictionary<string, List<string>> Contexts = new Dictionary<string, List<string>>();

        List<RenderCase> cases;

        public IReadOnlyList<RenderCase> Cases()
        {
            if (cases != null) return cases;
            var all = new List<RenderCase> { new RenderCase(Name + ".(default)", null, new List<Assignment>()) };
            foreach (var property in Settable(Type))
                foreach (var sample in property.Path.Contains("Style.") ? Samples(property.Path, property.Type).Take(1) : Samples(property.Path, property.Type))
                    all.Add(new RenderCase(Name + "." + property.Path + "=" + sample.Label, null, new List<Assignment> { new Assignment(property.Path, sample) }));
            if (ItemPath != null)
                foreach (var property in Settable(Assignment.StepType(Type, ItemPath)))
                    foreach (var sample in Samples(ItemPath + "." + property.Path, property.Type))
                        all.Add(new RenderCase(Name + "." + ItemPath + "." + property.Path + "=" + sample.Label, null, new List<Assignment> { new Assignment(ItemPath + "." + property.Path, sample) }));
            foreach (var sample in typeof(IAttributeAccessor).IsAssignableFrom(Type) ? SampleValues.Strings.Take(2) : Enumerable.Empty<Sample>())
            {
                all.Add(new RenderCase(Name + ".Attributes[data-probe]=" + sample.Label, null, new List<Assignment> { new Assignment("Attributes[data-probe]", sample) }));
                all.Add(new RenderCase(Name + ".Attributes[onfocus]=" + sample.Label, null, new List<Assignment> { new Assignment("Attributes[onfocus]", sample) }));
            }
            foreach (var sample in typeof(WebControl).IsAssignableFrom(Type) || typeof(HtmlControl).IsAssignableFrom(Type) ? SampleValues.StyleValues : new Sample[0])
            {
                all.Add(new RenderCase(Name + ".Style[color]=" + sample.Label, null, new List<Assignment> { new Assignment("Style[color]", sample) }));
                all.Add(new RenderCase(Name + ".Style[margin-left]=" + sample.Label, null, new List<Assignment> { new Assignment("Style[margin-left]", sample) }));
            }
            foreach (var interaction in Interactions) all.Add(Parse(null, interaction));
            foreach (var context in Contexts)
            {
                // A type that cannot be themed has no skin (a skin of it is a parse error of the whole theme).
                if (context.Key == "themed" && !ThemeableAttribute.IsTypeThemeable(Type)) continue;
                foreach (var body in context.Value) all.Add(Parse(context.Key, body));
            }
            var duplicates = all.GroupBy(c => c.Name).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicates.Count > 0) throw new InvalidOperationException("cases named twice: " + string.Join(", ", duplicates));
            return cases = all;
        }

        public RenderCase Case(string name) => Cases().FirstOrDefault(c => c.Name == name);

        /// <summary>A case from its body: "A=label+B=label", or "(default)".</summary>
        RenderCase Parse(string context, string body)
        {
            var assignments = new List<Assignment>();
            if (body != "(default)")
                foreach (var part in body.Split('+'))
                {
                    var equals = part.IndexOf('=');
                    var path = part.Substring(0, equals);
                    var label = part.Substring(equals + 1);
                    var sample = Samples(path, PathType(path)).FirstOrDefault(s => s.Label == label);
                    if (sample == null) throw new InvalidOperationException(Name + ": no sample '" + label + "' for " + path);
                    assignments.Add(new Assignment(path, sample));
                }
            return new RenderCase(Name + (context == null ? "" : "[" + context + "]") + "." + body, context, assignments);
        }

        Type PathType(string path)
        {
            if (path.StartsWith("Attributes[", StringComparison.Ordinal) || Assignment.Keyed.IsMatch(path)) return typeof(string);
            if (path.StartsWith("Style[", StringComparison.Ordinal)) return typeof(CssValue);
            var type = Type;
            foreach (var part in path.Split('.')) type = Assignment.StepType(type, part);
            return type;
        }

        IEnumerable<Sample> Samples(string path, Type type)
        {
            List<Sample> extra;
            var samples = SampleValues.For(type);
            return ExtraValues.TryGetValue(path, out extra) ? samples.Concat(extra) : samples;
        }

        /// <summary>System.Web's, or System.Web.Extensions' (ListView, DataPager).</summary>
        static bool IsWebAssembly(Assembly assembly) => assembly == typeof(Control).Assembly || assembly.GetName().Name == "System.Web.Extensions";

        static readonly string[] StyleParts = { "BackColor", "ForeColor", "CssClass", "Font.Bold", "HorizontalAlign", "VerticalAlign", "Wrap", "Height" };

        /// <summary>
        /// The component's public settable properties of System.Web, of a type with sample values, by name; a sub-object's
        /// (Font: FontInfo) the same, as Font.Bold.
        /// </summary>
        public static IEnumerable<(string Path, Type Type)> Settable(Type type)
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name, StringComparer.Ordinal))
            {
                if (property.GetIndexParameters().Length > 0 || !IsWebAssembly(property.DeclaringType.Assembly)) continue;
                var setter = property.GetSetMethod();
                if (setter != null && SampleValues.Has(property.PropertyType))
                {
                    yield return (property.Name, property.PropertyType);
                    continue;
                }
                if (setter == null && property.PropertyType == typeof(FontInfo))
                    foreach (var sub in Settable(property.PropertyType)) yield return (property.Name + "." + sub.Path, sub.Type);
                // A data control's styles (HeaderStyle, RowStyle...): their main properties, one sample each (Cases); the pager's settings.
                if (setter == null && property.Name != "ControlStyle" && typeof(Style).IsAssignableFrom(property.PropertyType))
                    foreach (var sub in Settable(property.PropertyType).Where(x => StyleParts.Contains(x.Path))) yield return (property.Name + "." + sub.Path, sub.Type);
                if (setter == null && property.PropertyType == typeof(PagerSettings))
                    foreach (var sub in Settable(property.PropertyType)) yield return (property.Name + "." + sub.Path, sub.Type);
            }
        }

        public static readonly IReadOnlyList<ProbeComponent> All = new[]
        {
            TextBox(),
            Button(),
            List("DropDownList", () => new DropDownList { ID = "ddlSize" }),
            List("ListBox", () => new ListBox { ID = "lstSize" }),
            List("CheckBoxList", () => new CheckBoxList { ID = "cblSize" }),
            List("RadioButtonList", () => new RadioButtonList { ID = "rblSize" }),
            List("BulletedList", () => new BulletedList { ID = "bltSize" }),
            Label(),
            Toggle("CheckBox", () => new CheckBox { ID = "chkAgree" }),
            Toggle("RadioButton", () => new RadioButton { ID = "rdoAgree" }),
            Literal(),
            Link("LinkButton", () => new LinkButton { ID = "lnkGo" }),
            HyperLink(),
            Picture("Image", () => new System.Web.UI.WebControls.Image { ID = "imgLogo" }),
            Link("ImageButton", () => new ImageButton { ID = "ibtnGo" }),
            Panel(),
            HiddenField(),
            PlaceHolder(),
            Validator("RequiredFieldValidator", () => new RequiredFieldValidator()),
            Validator("RegularExpressionValidator", () => new RegularExpressionValidator { ValidationExpression = "[0-9]+" }),
            Validator("CompareValidator", () => new CompareValidator { ValueToCompare = "3", Type = ValidationDataType.Integer, Operator = ValidationCompareOperator.GreaterThan }),
            Validator("CustomValidator", () => new CustomValidator { ClientValidationFunction = "checkOk" }),
            Validator("RangeValidator", () => new RangeValidator { MinimumValue = "1", MaximumValue = "10", Type = ValidationDataType.Integer }),
            Summary(),
            GridView(),
            Repeater(),
            FormView(),
            DetailsView(),
            ListView(),
            DataPager(),
            DataList(),
            DataGrid(),
            Html("HtmlGenericControl", () => new HtmlGenericControl("span") { ID = "genBox", InnerText = "Generic" }, "TagName=div+InnerHtml=markup", "InnerText=markup+Disabled=true", "TagName=section+Visible=false"),
            Html("HtmlAnchor", () => new HtmlAnchor { ID = "ancGo", InnerText = "Go" }, "HRef=app-relative+Target=blank", "HRef=app-relative+Disabled=true", "HRef=absolute+Title=text", "CausesValidation=false+ValidationGroup=text", "InnerHtml=markup+HRef=app-relative"),
            Html("HtmlForm", () => new HtmlForm { ID = "frmProbe" }, "DefaultButton=text+DefaultFocus=text", "Method=text+Enctype=text", "SubmitDisabledControls=true+Disabled=true"),
            Html("HtmlHead", () => new HtmlHead { ID = "hdProbe" }, "Title=markup+Description=text", "Keywords=markup+Title=text"),
            Html("HtmlLink", () => new HtmlLink { ID = "lnkCss", Href = "~/Images/probe.css" }, "Href=absolute+Attributes[rel]=text"),
            Html("HtmlImage", () => new HtmlImage { ID = "imgHtml", Alt = "Logo" }, "Src=app-relative+Alt=markup", "Src=app-relative+Width=3+Height=3+Border=0", "Src=absolute+Align=text"),
            Html("HtmlTable", () =>
            {
                var table = new HtmlTable { ID = "tblHtml" };
                table.Rows.Add(HtmlRow("A", "B"));
                return table;
            }, "Border=3+CellPadding=3+CellSpacing=0", "Width=text+Height=text+Align=text", "BgColor=text+BorderColor=text"),
            Html("HtmlTableRow", () => HtmlRow("A", "B"), "Align=text+VAlign=text", "BgColor=text+BorderColor=text+Height=text"),
            Html("HtmlTableCell", () => new HtmlTableCell { ID = "tdCell", InnerText = "Cell" }, "ColSpan=3+RowSpan=3", "NoWrap=true+Width=text", "Align=text+VAlign=text+BgColor=text"),
            Html("HtmlInputText", () => new HtmlInputText { ID = "inpName", Value = "Taro" }, InputInteractions.Concat(new[] { "MaxLength=3+Size=3", "Value=japanese+MaxLength=3" }).ToArray()),
            Html("HtmlInputPassword", () => new HtmlInputPassword { ID = "inpSecret", Value = "secret" }, InputInteractions.Concat(new[] { "MaxLength=3+Size=3" }).ToArray()),
            Html("HtmlInputHidden", () => new HtmlInputHidden { ID = "inpHidden", Value = "initial" }, InputInteractions),
            Html("HtmlInputCheckBox", () => new HtmlInputCheckBox { ID = "inpCheck" }, InputInteractions.Concat(new[] { "Checked=true+Disabled=true", "Checked=true+EnableViewState=false" }).ToArray()),
            Html("HtmlInputRadioButton", () => new HtmlInputRadioButton { ID = "inpRadio", Name = "answer" }, InputInteractions.Concat(new[] { "Checked=true+Disabled=true", "Checked=true+Name=group" }).ToArray()),
            Html("HtmlInputButton", () => new HtmlInputButton { ID = "inpButton", Value = "Go" }, InputInteractions.Concat(new[] { "CausesValidation=false+ValidationGroup=text", "Disabled=true+CausesValidation=false" }).ToArray()),
            Html("HtmlInputSubmit", () => new HtmlInputSubmit { ID = "inpSubmit", Value = "Go" }, InputInteractions.Concat(new[] { "CausesValidation=false+ValidationGroup=text", "Disabled=true+CausesValidation=false" }).ToArray()),
            Html("HtmlInputReset", () => new HtmlInputReset { ID = "inpReset", Value = "Reset" }, InputInteractions.Concat(new[] { "CausesValidation=false+ValidationGroup=text" }).ToArray()),
            Html("HtmlInputImage", () => new HtmlInputImage { ID = "inpImage", Alt = "Go" }, InputInteractions.Concat(new[] { "Src=app-relative+Alt=markup", "Src=app-relative+Align=text+Border=3", "CausesValidation=false+ValidationGroup=text" }).ToArray()),
            Html("HtmlInputFile", () => new HtmlInputFile { ID = "inpFile" }, "Accept=text+MaxLength=3+Size=3", "Disabled=true+Accept=text"),
            Html("HtmlInputGenericControl", () => new HtmlInputGenericControl("email") { ID = "inpEmail", Value = "a@example.com" }, InputInteractions),
            Html("HtmlButton", () => new HtmlButton { ID = "btnHtml", InnerText = "Go" }, "InnerHtml=markup+Disabled=true", "CausesValidation=false+ValidationGroup=text", "InnerText=markup+Visible=false"),
            HtmlSelect(),
            Html("HtmlTextArea", () => new HtmlTextArea { ID = "txaNote", Value = "Line1" }, InputInteractions.Concat(new[] { "Rows=3+Cols=3", "Value=markup+Rows=3" }).ToArray()),
            Html("HtmlMeta", () => new HtmlMeta { ID = "metaProbe", Name = "description", Content = "probe" }, "HttpEquiv=text+Content=markup", "Scheme=text+Name=group"),
            Html("HtmlTitle", () => new HtmlTitle { ID = "ttlProbe", Text = "Probe" }, "Text=markup+Visible=false"),
            Html("HtmlIframe", () => new HtmlIframe { ID = "ifrProbe" }, "Src=app-relative+Attributes[name]=text", "Src=absolute+Disabled=true"),
            Html("HtmlArea", () => new HtmlArea { ID = "areaProbe" }, "Href=app-relative+Attributes[alt]=markup", "Href=absolute+Attributes[shape]=text"),
            Html("HtmlAudio", () => new HtmlAudio { ID = "audProbe" }, "Src=app-relative+Attributes[controls]=text"),
            Html("HtmlVideo", () => new HtmlVideo { ID = "vidProbe" }, "Src=app-relative+Poster=app-relative", "Src=absolute+Attributes[controls]=text"),
            Html("HtmlSource", () => new HtmlSource { ID = "srcProbe" }, "Src=app-relative+Attributes[type]=text"),
            Html("HtmlTrack", () => new HtmlTrack { ID = "trkProbe" }, "Src=app-relative+Attributes[kind]=text"),
            Html("HtmlEmbed", () => new HtmlEmbed { ID = "embProbe" }, "Src=app-relative+Attributes[type]=text"),
            Html("HtmlElement", () => new HtmlElement { ID = "elmProbe" }, "Manifest=app-relative+Attributes[lang]=text"),
            TablePart("Table", Grid, "Rows[1]", "Caption=text+CaptionAlign=Bottom", "GridLines=Both+CellPadding=3+CellSpacing=0", "HorizontalAlign=Center+Width=120px", "BackImageUrl=app-relative+GridLines=Both"),
            TablePart("TableRow", () => Row(new TableRow()), "Cells[1]", "TableSection=TableHeader+HorizontalAlign=Center", "VerticalAlign=Top+Height=2em"),
            TablePart("TableHeaderRow", () => Row(new TableHeaderRow()), "Cells[1]", "TableSection=TableBody+HorizontalAlign=Center"),
            TablePart("TableFooterRow", () => Row(new TableFooterRow()), "Cells[1]", "TableSection=TableBody+HorizontalAlign=Center"),
            TablePart("TableCell", () => new TableCell { ID = "tdCell", Text = "Cell" }, null, "ColumnSpan=3+RowSpan=3", "Text=markup+Wrap=false", "HorizontalAlign=Center+VerticalAlign=Top", "AssociatedHeaderCellID=header+Text=text"),
            TablePart("TableHeaderCell", () => new TableHeaderCell { ID = "thHead", Text = "Header" }, null, "Scope=Column+AbbreviatedText=text", "CategoryText=Arial,MS-Gothic+Scope=Row", "ColumnSpan=3+Wrap=false"),
            FileUpload(),
            MultiView(),
            View(),
            Localize(),
            Substitution(),
            Xml(),
            AdRotator(),
            Calendar(),
            ImageMap(),
            LiteralControl(),
            ContentPlaceHolder(),
            Content(),
            ScriptManager(),
            ScriptManagerProxy(),
            UpdatePanel(),
            UpdateProgress(),
            Timer(),
            TreeView(),
            Menu(),
            SiteMapPath(),
            SiteMapDataSource(),
            Login(),
            LoginName(),
            LoginStatus(),
            LoginView(),
            Wizard(),
            Step("WizardStep", () =>
            {
                var step = new WizardStep { ID = "wsProbe", Title = "Probe step" };
                step.Controls.Add(new Label { ID = "lblInStep", Text = "In the step" });
                return step;
            }),
            Step("TemplatedWizardStep", () => new TemplatedWizardStep { ID = "twsProbe", Title = "Templated", ContentTemplate = new ProbePlaceholderTemplate("<span>Templated step</span>", "phStep", "") }),
            CreateUserWizard(),
            CreateUserStep("CreateUserWizardStep", () => new CreateUserWizardStep { ID = "cuwsCreate" }),
            CreateUserStep("CompleteWizardStep", () => new CompleteWizardStep { ID = "cwsComplete" }),
            ChangePassword(),
            PasswordRecovery(),
            DataSource("ObjectDataSource", () => new ObjectDataSource { ID = "odsProducts", TypeName = "WebControlsProbe.ProductStore", SelectMethod = "SelectAll" },
                "EnablePaging=true+SelectCountMethod=text", "EnableCaching=true+CacheDuration=3", "TypeName=missing+SelectMethod=SelectAll", "ConflictDetection=CompareAllValues+OldValuesParameterFormatString=text"),
            DataSource("SqlDataSource", () => new SqlDataSource { ID = "sdsProducts", ProviderName = "WebControlsProbe.Data", ConnectionString = "memory", SelectCommand = "select" },
                "DataSourceMode=DataReader+EnableCaching=true", "ProviderName=missing+SelectCommand=select", "CancelSelectOnNullParameter=false+FilterExpression=text"),
            DataSource("AccessDataSource", () => new AccessDataSource { ID = "adsProducts", DataFile = "~/App_Data/probe.mdb", SelectCommand = "SELECT * FROM Products" },
                "DataSourceMode=DataReader+EnableCaching=true"),
            DataSource("XmlDataSource", () => new XmlDataSource { ID = "xdsProducts", DataFile = "~/App_Data/products.xml" },
                "XPath=products+TransformFile=app-relative", "DataFile=missing+XPath=products", "EnableCaching=false+CacheDuration=3"),
            DataSource("LinqDataSource", () => new LinqDataSource { ID = "ldsProducts", ContextTypeName = "WebControlsProbe.ProbeLinqContext", TableName = "Products" },
                "Where=text+OrderBy=text", "AutoPage=false+AutoSort=false", "EnableUpdate=true+EnableDelete=true+EnableInsert=true", "StoreOriginalValuesInViewState=false+Select=text"),
            QueryExtenderComponent(),
            ModelErrorMessageComponent(),
            WebPart("WebPartManager", () => new System.Web.UI.WebControls.WebParts.WebPartManager { ID = "wpmProbe" }, null, "EnableClientScript=false+CloseProviderWarning=markup", "DeleteWarning=markup+ExportSensitiveDataWarning=text"),
            WebPart("ProxyWebPartManager", () => new System.Web.UI.WebControls.WebParts.ProxyWebPartManager { ID = "pwpmProbe" }, null),
            WebPart("WebPartZone", () => new System.Web.UI.WebControls.WebParts.WebPartZone
            {
                ID = "wpzProbe", HeaderText = "Zone",
                ZoneTemplate = new ProbeControlsTemplate(() => new Control[] { new Label { ID = "lblPart", Text = "A label web part" } }),
            }, null, "HeaderText=markup+EmptyZoneText=markup", "LayoutOrientation=Horizontal+ShowTitleIcons=false", "WebPartVerbRenderMode=TitleBar+MenuLabelText=text",
               "PartChromeType=BorderOnly+PartTitleStyle.BackColor=Red", "HeaderStyle.BackColor=Red+PartStyle.ForeColor=#3366CC", "AllowLayoutChange=false+DragHighlightColor=Red"),
            WebPart("CatalogZone", () => new System.Web.UI.WebControls.WebParts.CatalogZone { ID = "czProbe", HeaderText = "Catalog" }, null, "HeaderText=markup+InstructionText=markup", "AddVerb.Text=text+CloseVerb.Text=markup"),
            WebPart("DeclarativeCatalogPart", () => new System.Web.UI.WebControls.WebParts.DeclarativeCatalogPart
            {
                ID = "dcpProbe", Title = "Declared", WebPartsTemplate = new ProbeControlsTemplate(() => new Control[] { new Label { ID = "lblCatalogPart", Text = "From the catalog" } }),
            }, InCatalogZone, "Title=markup+Description=text"),
            WebPart("PageCatalogPart", () => new System.Web.UI.WebControls.WebParts.PageCatalogPart { ID = "pcpProbe" }, InCatalogZone, "Title=markup+Description=text"),
            WebPart("ImportCatalogPart", () => new System.Web.UI.WebControls.WebParts.ImportCatalogPart { ID = "icpProbe" }, InCatalogZone, "BrowseHelpText=markup+UploadHelpText=markup", "PartImportErrorLabelText=text+UploadButtonText=text"),
            WebPart("EditorZone", () => new System.Web.UI.WebControls.WebParts.EditorZone { ID = "ezProbe", HeaderText = "Editor" }, null, "HeaderText=markup+InstructionText=markup", "ApplyVerb.Text=text+OKVerb.Text=markup"),
            WebPart("AppearanceEditorPart", () => new System.Web.UI.WebControls.WebParts.AppearanceEditorPart { ID = "aepProbe" }, InEditorZone, "Title=markup+Description=text"),
            WebPart("BehaviorEditorPart", () => new System.Web.UI.WebControls.WebParts.BehaviorEditorPart { ID = "bepProbe" }, InEditorZone, "Title=markup+Description=text"),
            WebPart("LayoutEditorPart", () => new System.Web.UI.WebControls.WebParts.LayoutEditorPart { ID = "lepProbe" }, InEditorZone, "Title=markup+Description=text"),
            WebPart("PropertyGridEditorPart", () => new System.Web.UI.WebControls.WebParts.PropertyGridEditorPart { ID = "pgepProbe" }, InEditorZone, "Title=markup+Description=text"),
            WebPart("ConnectionsZone", () => new System.Web.UI.WebControls.WebParts.ConnectionsZone { ID = "cnzProbe" }, null, "HeaderText=markup+InstructionText=markup", "NoExistingConnectionTitle=markup+NewConnectionErrorMessage=text"),
        };

        public static ProbeComponent Named(string name) =>
            All.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));

        static readonly DateTime Today = new DateTime(2020, 1, 15);

        /// <summary>A table of two rows of two cells (A B / C D).</summary>
        static Table Grid()
        {
            var table = new Table { ID = "tblItems" };
            foreach (var texts in new[] { new[] { "A", "B" }, new[] { "C", "D" } })
            {
                var row = new TableRow();
                foreach (var text in texts) row.Cells.Add(new TableCell { Text = text });
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>The table parts (Table, TableRow and its header and footer rows, TableCell, TableHeaderCell).</summary>
        static ProbeComponent TablePart(string name, Func<WebControl> make, string itemPath, params string[] interactions)
        {
            var component = new ProbeComponent(name, make().GetType(), make);
            component.ItemPath = itemPath;
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["BackImageUrl"] = new List<Sample> { AppRelativeImage };
            component.ExtraValues["AssociatedHeaderCellID"] = new List<Sample> { new Sample("header", new[] { "thHead" }) };
            component.Interactions.AddRange(interactions);
            component.Interactions.AddRange(new[] { "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red", "CssClass=text+Visible=false", "CssClass=text+EnableViewState=false" });
            AddContexts(component);
            return component;
        }

        static TableRow Row(TableRow row)
        {
            row.ID = "trRow";
            row.Cells.Add(new TableCell { Text = "A" });
            row.Cells.Add(new TableCell { Text = "B" });
            return row;
        }

        static ProbeComponent FileUpload()
        {
            var component = new ProbeComponent("FileUpload", typeof(FileUpload), () => new FileUpload { ID = "fupFile" });
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.Interactions.AddRange(new[]
            {
                "AllowMultiple=true+Enabled=false", "AllowMultiple=true+ToolTip=text", "Enabled=false+CssClass=text", StyleCombination,
                "ForeColor=#3366CC+Style[color]=red", "AllowMultiple=true+Visible=false", "AllowMultiple=true+EnableViewState=false",
            });
            AddContexts(component, "AllowMultiple=true");
            return component;
        }

        /// <summary>A multi-view of three views (each a label), the first active.</summary>
        static MultiView Views()
        {
            var views = new MultiView { ID = "mvSteps", ActiveViewIndex = 0 };
            foreach (var name in new[] { "First", "Second", "Third" })
            {
                var view = new View { ID = "vw" + name };
                view.Controls.Add(new Label { ID = "lbl" + name, Text = name + " view" });
                views.Views.Add(view);
            }
            return views;
        }

        static ProbeComponent MultiView()
        {
            var component = new ProbeComponent("MultiView", typeof(MultiView), Views);
            component.ExtraValues["ActiveViewIndex"] = new List<Sample> { new Sample("1", 1), new Sample("2", 2) };
            component.Interactions.AddRange(new[] { "ActiveViewIndex=1+EnableViewState=false", "ActiveViewIndex=1+Visible=false", "ActiveViewIndex=-1+Visible=true" });
            AddContexts(component, "ActiveViewIndex=1");
            return component;
        }

        /// <summary>A view: the second of a multi-view, active (a view shows only in its multi-view).</summary>
        static ProbeComponent View()
        {
            var component = new ProbeComponent("View", typeof(View), () =>
            {
                var view = new View { ID = "vwProbe" };
                view.Controls.Add(new Label { ID = "lblInView", Text = "In the view" });
                return view;
            });
            component.Wrap = control =>
            {
                var views = new MultiView { ID = "mvHost" };
                views.Views.Add(new View { ID = "vwOther" });
                views.Views.Add((View)control);
                views.ActiveViewIndex = 1;
                return views;
            };
            component.Interactions.AddRange(new[] { "Visible=false+EnableViewState=false" });
            AddContexts(component);
            return component;
        }

        static ProbeComponent Localize()
        {
            var component = new ProbeComponent("Localize", typeof(Localize), () => new Localize { ID = "locText" });
            component.Interactions.AddRange(new[] { "Mode=Encode+Text=markup", "Mode=PassThrough+Text=markup", "Mode=Transform+Text=markup", "Text=text+EnableViewState=false", "Text=text+Visible=false" });
            AddContexts(component);
            return component;
        }

        static ProbeComponent Substitution()
        {
            var component = new ProbeComponent("Substitution", typeof(Substitution), () => new Substitution { ID = "subText", MethodName = "SubstitutionText" });
            component.ExtraValues["MethodName"] = new List<Sample> { new Sample("SubstitutionText", "SubstitutionText"), new Sample("missing", "NoSuchMethod") };
            component.Interactions.AddRange(new[] { "MethodName=SubstitutionText+Visible=false", "MethodName=SubstitutionText+EnableViewState=false" });
            AddContexts(component);
            return component;
        }

        static ProbeComponent Xml()
        {
            var component = new ProbeComponent("Xml", typeof(System.Web.UI.WebControls.Xml), () => new System.Web.UI.WebControls.Xml { ID = "xmlItems", DocumentContent = "<items><item>Pen</item><item>Bag &amp; box</item></items>" });
            component.ExtraValues["DocumentContent"] = new List<Sample> { new Sample("items", "<items><item>Cup</item></items>"), new Sample("invalid", "<items>") };
            component.ExtraValues["DocumentSource"] = new List<Sample> { new Sample("app-relative", "~/App_Data/probe.xml"), new Sample("missing", "~/App_Data/none.xml") };
            component.ExtraValues["TransformSource"] = new List<Sample> { new Sample("app-relative", "~/App_Data/probe.xslt"), new Sample("missing", "~/App_Data/none.xslt") };
            component.Interactions.AddRange(new[]
            {
                "TransformSource=app-relative+DocumentContent=items", "TransformSource=app-relative+DocumentSource=app-relative", "DocumentSource=app-relative+DocumentContent=items",
                "TransformSource=app-relative+Visible=false", "TransformSource=app-relative+EnableViewState=false",
            });
            AddContexts(component, "TransformSource=app-relative");
            return component;
        }

        /// <summary>An ad rotator of one advertisement (App_Data/ads.xml: a rotator picks one of several at random).</summary>
        static ProbeComponent AdRotator()
        {
            var component = new ProbeComponent("AdRotator", typeof(AdRotator), () => new AdRotator { ID = "adBanner", AdvertisementFile = "~/App_Data/ads.xml" });
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["AdvertisementFile"] = new List<Sample> { new Sample("app-relative", "~/App_Data/ads.xml"), new Sample("missing", "~/App_Data/none.xml") };
            component.ExtraValues["KeywordFilter"] = new List<Sample> { new Sample("pen", "pen") };
            component.ExtraValues["Target"] = new List<Sample> { new Sample("blank", "_blank") };
            component.ExtraValues["ImageUrlField"] = new List<Sample> { new Sample("Image", "Image") };
            component.ExtraValues["NavigateUrlField"] = new List<Sample> { new Sample("Code", "Code") };
            component.ExtraValues["AlternateTextField"] = new List<Sample> { new Sample("Name", "Name") };
            component.BoundData = () => ProbeData.Many().Take(1).ToList();
            component.Interactions.AddRange(new[]
            {
                "KeywordFilter=pen+Target=blank", "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red",
                "Target=blank+Visible=false", "Target=blank+EnableViewState=false",
            });
            AddContexts(component, "Target=blank");
            component.Contexts["data-bound"] = new List<string>
            {
                "(default)", "ImageUrlField=Image", "ImageUrlField=Image+NavigateUrlField=Code+AlternateTextField=Name", "AlternateTextField=Name+Target=blank",
            };
            return component;
        }

        static ProbeComponent Calendar()
        {
            var component = new ProbeComponent("Calendar", typeof(System.Web.UI.WebControls.Calendar), () => new System.Web.UI.WebControls.Calendar { ID = "calDate", TodaysDate = Today, VisibleDate = Today });
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["NextMonthText"] = new List<Sample> { new Sample("gt", "&gt;") };
            component.Interactions.AddRange(new[]
            {
                "SelectedDate=2020-01-20+SelectedDayStyle.BackColor=Red", "SelectionMode=DayWeek+SelectedDate=2020-01-20", "SelectionMode=DayWeekMonth+SelectWeekText=text+SelectMonthText=markup",
                "SelectionMode=None+SelectedDate=2020-01-20", "ShowGridLines=true+CellPadding=3+CellSpacing=0", "ShowTitle=false+ShowDayHeader=false",
                "ShowNextPrevMonth=false+TitleFormat=Month", "NextPrevFormat=FullMonth+DayNameFormat=Full", "FirstDayOfWeek=Monday+DayNameFormat=FirstTwoLetters",
                "TodayDayStyle.BackColor=Red+WeekendDayStyle.BackColor=#3366CC", "OtherMonthDayStyle.ForeColor=Red+TitleStyle.BackColor=#3366CC",
                "UseAccessibleHeader=false+Caption=text+CaptionAlign=Bottom", "Enabled=false+SelectionMode=DayWeekMonth", "Enabled=false+CssClass=text",
                StyleCombination, "ForeColor=#3366CC+Style[color]=red", "SelectedDate=2020-01-20+Visible=false", "SelectedDate=2020-01-20+EnableViewState=false",
            });
            AddContexts(component, "SelectionMode=DayWeekMonth");
            return component;
        }

        static ProbeComponent ImageMap()
        {
            var component = new ProbeComponent("ImageMap", typeof(ImageMap), () =>
            {
                var map = new ImageMap { ID = "imapAreas", ImageUrl = "~/Images/probe.png", AlternateText = "Areas" };
                map.HotSpots.Add(new RectangleHotSpot { Left = 0, Top = 0, Right = 10, Bottom = 10, NavigateUrl = "~/ButtonTarget.aspx", AlternateText = "Rectangle", PostBackValue = "rect" });
                map.HotSpots.Add(new CircleHotSpot { X = 15, Y = 5, Radius = 5, AlternateText = "Circle", PostBackValue = "circle", HotSpotMode = HotSpotMode.PostBack });
                map.HotSpots.Add(new PolygonHotSpot { Coordinates = "0,0,20,0,20,10", AlternateText = "Polygon", HotSpotMode = HotSpotMode.Inactive });
                return map;
            });
            component.ItemPath = "HotSpots[0]";
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["ImageUrl"] = new List<Sample> { AppRelativeImage, AbsoluteImage };
            component.ExtraValues["HotSpots[0].NavigateUrl"] = new List<Sample> { new Sample("app-relative", "~/ButtonTarget.aspx?a=1&b=2") };
            component.ExtraValues["HotSpots[0].Target"] = new List<Sample> { new Sample("blank", "_blank") };
            component.ExtraValues["Target"] = new List<Sample> { new Sample("blank", "_blank") };
            component.Interactions.AddRange(new[]
            {
                "HotSpotMode=PostBack+HotSpots[0].PostBackValue=text", "HotSpotMode=Navigate+HotSpots[0].NavigateUrl=app-relative", "HotSpotMode=Inactive+Target=blank",
                "HotSpots[0].HotSpotMode=PostBack+HotSpots[0].PostBackValue=markup", "HotSpotMode=PostBack+Enabled=false",
                "HotSpots[0].Target=blank+HotSpots[0].AccessKey=text", "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red",
                "AlternateText=text+Visible=false", "AlternateText=text+EnableViewState=false",
            });
            AddContexts(component, "HotSpotMode=PostBack");
            return component;
        }

        static ProbeComponent LiteralControl()
        {
            var component = new ProbeComponent("LiteralControl", typeof(LiteralControl), () => new LiteralControl("<b>literal</b>") { ID = "litControl" });
            component.Interactions.AddRange(new[] { "Text=markup+EnableViewState=false", "Text=text+Visible=false" });
            AddContexts(component);
            return component;
        }

        /// <summary>A content place holder (a master page's), with a label; outside a master page it is a container.</summary>
        static ProbeComponent ContentPlaceHolder()
        {
            var component = new ProbeComponent("ContentPlaceHolder", typeof(ContentPlaceHolder), () =>
            {
                var holder = new ContentPlaceHolder { ID = "cphMain" };
                holder.Controls.Add(new Label { ID = "lblDefault", Text = "Default content" });
                return holder;
            });
            component.Interactions.AddRange(new[] { "Visible=false+EnableViewState=false" });
            AddContexts(component);
            return component;
        }

        /// <summary>A content control (a content page's), with a label; outside a content page it is a container.</summary>
        static ProbeComponent Content()
        {
            var component = new ProbeComponent("Content", typeof(Content), () =>
            {
                var content = new Content { ID = "cntMain" };
                content.Controls.Add(new Label { ID = "lblContent", Text = "Content" });
                return content;
            });
            component.ExtraValues["ContentPlaceHolderID"] = new List<Sample> { new Sample("main", "cphMain") };
            component.Interactions.AddRange(new[] { "Visible=false+EnableViewState=false" });
            AddContexts(component);
            return component;
        }

        /// <summary>The ASP.NET AJAX controls (System.Web.Extensions): after the page's script manager (smPage), as they must be.</summary>
        static Control AfterScriptManager(Control control, params Control[] before)
        {
            var holder = new PlaceHolder();
            holder.Controls.Add(new ScriptManager { ID = "smPage" });
            foreach (var other in before) holder.Controls.Add(other);
            holder.Controls.Add(control);
            return holder;
        }

        static ProbeComponent ScriptManager()
        {
            var component = new ProbeComponent("ScriptManager", typeof(ScriptManager), () => new ScriptManager { ID = "smProbe" });
            component.ExtraValues["ScriptPath"] = new List<Sample> { new Sample("app-relative", "~/Scripts") };
            component.ExtraValues["EmptyPageUrl"] = new List<Sample> { new Sample("app-relative", "~/ButtonTarget.aspx") };
            component.ExtraValues["ClientNavigateHandler"] = new List<Sample> { new Sample("handler", "onNavigate") };
            component.Interactions.AddRange(new[]
            {
                "EnablePartialRendering=false+EnablePageMethods=true", "EnableHistory=true+EnableSecureHistoryState=false", "EnableHistory=true+ClientNavigateHandler=handler",
                "ScriptMode=Debug+LoadScriptsBeforeUI=false", "ScriptMode=Release+EnableScriptGlobalization=true", "EnableCdn=true+EnableCdnFallback=false",
                "AjaxFrameworkMode=Disabled+EnablePartialRendering=false", "AsyncPostBackTimeout=3+AsyncPostBackErrorMessage=text", "EnablePartialRendering=false+Visible=false",
                "EnablePageMethods=true+EnableViewState=false",
            });
            AddContexts(component, "EnableHistory=true");
            return component;
        }

        static ProbeComponent ScriptManagerProxy()
        {
            var component = new ProbeComponent("ScriptManagerProxy", typeof(ScriptManagerProxy), () =>
            {
                var proxy = new ScriptManagerProxy { ID = "smpProbe" };
                proxy.Scripts.Add(new ScriptReference("~/Images/probe.js"));
                return proxy;
            });
            component.Wrap = control => AfterScriptManager(control);
            component.Interactions.AddRange(new[] { "Visible=false+EnableViewState=false" });
            AddContexts(component);
            return component;
        }

        static ProbeComponent UpdatePanel()
        {
            var component = new ProbeComponent("UpdatePanel", typeof(UpdatePanel), () => new UpdatePanel
            {
                ID = "upProbe", ContentTemplate = new ProbePlaceholderTemplate("<span>In the panel</span>", "phInPanel", ""),
            });
            component.Wrap = control => AfterScriptManager(control);
            component.Interactions.AddRange(new[]
            {
                "UpdateMode=Conditional+ChildrenAsTriggers=false", "UpdateMode=Always+ChildrenAsTriggers=false", "RenderMode=Inline+UpdateMode=Conditional",
                "RenderMode=Inline+Visible=false", "UpdateMode=Conditional+EnableViewState=false",
            });
            AddContexts(component, "RenderMode=Inline");
            return component;
        }

        static ProbeComponent UpdateProgress()
        {
            var component = new ProbeComponent("UpdateProgress", typeof(UpdateProgress), () => new UpdateProgress
            {
                ID = "uprProbe", ProgressTemplate = new ProbePlaceholderTemplate("<span>Working...</span>", "phInProgress", ""),
            });
            component.Wrap = control => AfterScriptManager(control, new UpdatePanel { ID = "upTarget", ContentTemplate = new ProbePlaceholderTemplate("<span>Target</span>", "phInTarget", "") });
            component.ExtraValues["AssociatedUpdatePanelID"] = new List<Sample> { new Sample("target", "upTarget"), new Sample("missing", "upMissing") };
            component.Interactions.AddRange(new[]
            {
                "AssociatedUpdatePanelID=target+DisplayAfter=0", "DynamicLayout=false+DisplayAfter=3", "AssociatedUpdatePanelID=target+DynamicLayout=false",
                "DisplayAfter=3+Visible=false", "DisplayAfter=3+EnableViewState=false",
            });
            AddContexts(component, "AssociatedUpdatePanelID=target");
            return component;
        }

        static ProbeComponent Timer()
        {
            var component = new ProbeComponent("Timer", typeof(Timer), () => new Timer { ID = "tmrProbe", Interval = 60000 });
            component.Wrap = control => AfterScriptManager(control);
            component.ExtraValues["Interval"] = new List<Sample> { new Sample("1000", 1000) };
            component.Interactions.AddRange(new[] { "Interval=1000+Enabled=false", "Interval=1000+Visible=false", "Interval=1000+EnableViewState=false" });
            AddContexts(component, "Interval=1000");
            return component;
        }

        /// <summary>Hierarchical data in memory (the navigation controls' "[data-bound]" cases): nodes of nodes.</summary>
        public static XmlDataSource Nodes() => new XmlDataSource
        {
            ID = "xdsNodes", EnableCaching = false,
            Data = "<nodes><node text=\"One\" value=\"1\"><node text=\"One A\" value=\"1a\" /><node text=\"One &amp; B\" value=\"1b\" /></node><node text=\"Two\" value=\"2\" /></nodes>",
        };

        static ProbeComponent TreeView()
        {
            var component = new ProbeComponent("TreeView", typeof(TreeView), () =>
            {
                var tree = new TreeView { ID = "tvNodes" };
                var root = new TreeNode("Root", "r");
                root.ChildNodes.Add(new TreeNode("Child A", "a"));
                root.ChildNodes.Add(new TreeNode("Child <B>", "b"));
                tree.Nodes.Add(root);
                tree.Nodes.Add(new TreeNode("Second", "s"));
                return tree;
            });
            component.ItemPath = "Nodes[0]";
            component.BoundData = Nodes;
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["Nodes[0].NavigateUrl"] = new List<Sample> { new Sample("app-relative", "~/ButtonTarget.aspx?a=1&b=2") };
            component.ExtraValues["Nodes[0].ImageUrl"] = new List<Sample> { AppRelativeImage };
            component.ExtraValues["CollapseImageUrl"] = new List<Sample> { AppRelativeImage };
            component.ExtraValues["ExpandImageUrl"] = new List<Sample> { AppRelativeImage };
            component.ExtraValues["NoExpandImageUrl"] = new List<Sample> { AppRelativeImage };
            component.ExtraValues["LineImagesFolder"] = new List<Sample> { new Sample("app-relative", "~/Images") };
            component.ExtraValues["Target"] = new List<Sample> { new Sample("blank", "_blank") };
            component.ExtraValues["ExpandDepth"] = new List<Sample> { new Sample("1", 1) };
            component.Interactions.AddRange(new[]
            {
                "ShowCheckBoxes=All+Nodes[0].Checked=true", "ShowCheckBoxes=Leaf+ExpandDepth=1", "ShowLines=true+ExpandDepth=1", "ImageSet=XPFileExplorer+ShowLines=true",
                "EnableClientScript=false+ExpandDepth=0", "ShowExpandCollapse=false+ExpandDepth=1", "Nodes[0].Selected=true+SelectedNodeStyle.BackColor=Red",
                "Nodes[0].NavigateUrl=app-relative+Target=blank", "Nodes[0].SelectAction=Expand+ExpandDepth=0", "Nodes[0].SelectAction=None+Nodes[0].ToolTip=text",
                "Nodes[0].PopulateOnDemand=true+PopulateNodesFromClient=true", "Nodes[0].PopulateOnDemand=true+EnableClientScript=false", "NodeIndent=3+NodeWrap=true",
                "NodeStyle.ForeColor=Red+HoverNodeStyle.BackColor=#3366CC", "RootNodeStyle.BackColor=Red+LeafNodeStyle.BackColor=#3366CC", "Enabled=false+ExpandDepth=1",
                "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red", "ExpandDepth=1+Visible=false", "ExpandDepth=1+EnableViewState=false",
            });
            AddContexts(component, "ExpandDepth=1", "ShowCheckBoxes=All");
            component.Contexts["data-bound"] = new List<string> { "(default)", "ExpandDepth=0", "AutoGenerateDataBindings=false", "ShowCheckBoxes=All+ExpandDepth=1", "MaxDataBindDepth=0" };
            return component;
        }

        static ProbeComponent Menu()
        {
            var component = new ProbeComponent("Menu", typeof(Menu), () =>
            {
                var menu = new Menu { ID = "mnuItems" };
                menu.Items.Add(new MenuItem("Home", "h"));
                var products = new MenuItem("Products", "p");
                products.ChildItems.Add(new MenuItem("Pen", "pen"));
                products.ChildItems.Add(new MenuItem("Bag & box", "bag"));
                menu.Items.Add(products);
                menu.Items.Add(new MenuItem("About", "a"));
                return menu;
            });
            component.ItemPath = "Items[1]";
            component.BoundData = Nodes;
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["Items[1].NavigateUrl"] = new List<Sample> { new Sample("app-relative", "~/ButtonTarget.aspx?a=1&b=2") };
            component.ExtraValues["Items[1].ImageUrl"] = new List<Sample> { AppRelativeImage };
            component.ExtraValues["StaticPopOutImageUrl"] = new List<Sample> { AppRelativeImage };
            component.ExtraValues["DynamicPopOutImageUrl"] = new List<Sample> { AppRelativeImage };
            component.ExtraValues["ScrollDownImageUrl"] = new List<Sample> { AppRelativeImage };
            component.ExtraValues["Target"] = new List<Sample> { new Sample("blank", "_blank") };
            component.ExtraValues["StaticDisplayLevels"] = new List<Sample> { new Sample("2", 2) };
            component.Interactions.AddRange(new[]
            {
                "Orientation=Horizontal+StaticDisplayLevels=2", "RenderingMode=Table+Orientation=Horizontal", "RenderingMode=List+IncludeStyleBlock=false",
                "RenderingMode=Table+StaticDisplayLevels=2", "MaximumDynamicDisplayLevels=0+StaticDisplayLevels=2", "Items[1].Selected=true+StaticSelectedStyle.BackColor=Red",
                "Items[1].Enabled=false+StaticDisplayLevels=2", "Items[1].Selectable=false+Items[1].ToolTip=text", "Items[1].NavigateUrl=app-relative+Target=blank",
                "StaticMenuItemStyle.ForeColor=Red+DynamicMenuItemStyle.BackColor=#3366CC", "StaticEnableDefaultPopOutImage=false+DynamicEnableDefaultPopOutImage=false",
                "SkipLinkText=empty+Orientation=Horizontal", "DisappearAfter=3+DynamicHorizontalOffset=3", "Enabled=false+StaticDisplayLevels=2", "Enabled=false+CssClass=text",
                StyleCombination, "ForeColor=#3366CC+Style[color]=red", "StaticDisplayLevels=2+Visible=false", "StaticDisplayLevels=2+EnableViewState=false",
            });
            AddContexts(component, "StaticDisplayLevels=2", "RenderingMode=Table");
            component.Contexts["data-bound"] = new List<string> { "(default)", "StaticDisplayLevels=2", "MaximumDynamicDisplayLevels=0", "RenderingMode=Table" };
            return component;
        }

        /// <summary>A site map path: the page's place in Web.sitemap (RenderProbe.aspx is one of its nodes).</summary>
        static ProbeComponent SiteMapPath()
        {
            var component = new ProbeComponent("SiteMapPath", typeof(SiteMapPath), () => new SiteMapPath { ID = "smpPath" });
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["SiteMapProvider"] = new List<Sample> { new Sample("default", "AspNetXmlSiteMapProvider") };
            component.ExtraValues["PathSeparator"] = new List<Sample> { new Sample("arrow", " &gt; ") };
            component.ExtraValues["ParentLevelsDisplayed"] = new List<Sample> { new Sample("1", 1) };
            component.ContextSetups["templates"] = control =>
            {
                var path = (SiteMapPath)control;
                path.NodeTemplate = new ProbeTemplate("node");
                path.CurrentNodeTemplate = new ProbeTemplate("current");
                path.RootNodeTemplate = new ProbeTemplate("root");
                path.PathSeparatorTemplate = new ProbeTemplate("separator");
            };
            component.Interactions.AddRange(new[]
            {
                "PathDirection=CurrentToRoot+PathSeparator=arrow", "RenderCurrentNodeAsLink=true+ShowToolTips=false", "ParentLevelsDisplayed=0+PathSeparator=arrow",
                "ParentLevelsDisplayed=1+RenderCurrentNodeAsLink=true", "SkipLinkText=empty+PathSeparator=arrow", "CurrentNodeStyle.ForeColor=Red+NodeStyle.BackColor=#3366CC",
                "RootNodeStyle.ForeColor=Red+PathSeparatorStyle.BackColor=#3366CC", "Enabled=false+RenderCurrentNodeAsLink=true", "Enabled=false+CssClass=text",
                StyleCombination, "ForeColor=#3366CC+Style[color]=red", "PathSeparator=arrow+Visible=false", "PathSeparator=arrow+EnableViewState=false",
            });
            AddContexts(component, "RenderCurrentNodeAsLink=true");
            component.Contexts["templates"] = new List<string> { "(default)", "PathDirection=CurrentToRoot" };
            return component;
        }

        static ProbeComponent SiteMapDataSource()
        {
            var component = new ProbeComponent("SiteMapDataSource", typeof(SiteMapDataSource), () => new SiteMapDataSource { ID = "smdsSite" });
            component.ExtraValues["SiteMapProvider"] = new List<Sample> { new Sample("default", "AspNetXmlSiteMapProvider") };
            component.ExtraValues["StartingNodeUrl"] = new List<Sample> { new Sample("app-relative", "~/NavigationScenario.aspx"), new Sample("missing", "~/None.aspx") };
            component.Interactions.AddRange(new[] { "ShowStartingNode=false+StartFromCurrentNode=true", "StartingNodeOffset=-1+StartFromCurrentNode=true", "ShowStartingNode=false+Visible=false" });
            AddContexts(component);
            return component;
        }

        static Sample AppRelativePage => new Sample("app-relative", "~/ButtonTarget.aspx");

        /// <summary>The request's user: taro, in the role admins (the "[logged in]" context: what the login controls show to a user).</summary>
        static void LoggedIn(Control control) =>
            System.Web.HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity("taro", "Forms"), new[] { "admins" });

        /// <summary>The login controls' and the wizards' URL properties, an app-relative page (or image).</summary>
        static void Urls(ProbeComponent component)
        {
            foreach (var property in component.Type.GetProperties().Where(p => p.CanWrite && p.PropertyType == typeof(string) && p.Name.EndsWith("Url", StringComparison.Ordinal)))
                component.ExtraValues[property.Name] = new List<Sample> { property.Name.EndsWith("ImageUrl", StringComparison.Ordinal) ? AppRelativeImage : AppRelativePage };
        }

        static ProbeComponent Login()
        {
            var component = new ProbeComponent("Login", typeof(Login), () => new Login { ID = "lgnMain" });
            Urls(component);
            component.ContextSetups["logged in"] = LoggedIn;
            component.ContextSetups["templates"] = control => ((Login)control).LayoutTemplate = new ProbeControlsTemplate(() => new Control[]
            {
                new TextBox { ID = "UserName" }, new TextBox { ID = "Password", TextMode = TextBoxMode.Password }, new CheckBox { ID = "RememberMe" },
                new Literal { ID = "FailureText" }, new Button { ID = "LoginButton", CommandName = "Login", Text = "Sign in" },
            });
            component.Interactions.AddRange(new[]
            {
                "Orientation=Horizontal+TextLayout=TextOnTop", "DisplayRememberMe=false+RememberMeSet=true", "LoginButtonType=Link+LoginButtonText=markup",
                "LoginButtonType=Image+LoginButtonImageUrl=app-relative", "CreateUserText=text+CreateUserUrl=app-relative", "HelpPageText=text+HelpPageUrl=app-relative+HelpPageIconUrl=app-relative",
                "PasswordRecoveryText=text+PasswordRecoveryUrl=app-relative", "TitleText=markup+InstructionText=markup", "UserNameLabelText=markup+PasswordLabelText=markup",
                "UserName=text+UserNameRequiredErrorMessage=markup", "RenderOuterTable=false+Orientation=Horizontal", "TitleTextStyle.BackColor=Red+LoginButtonStyle.ForeColor=#3366CC",
                "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red", "UserName=text+Visible=false", "UserName=text+EnableViewState=false",
            });
            AddContexts(component, "Orientation=Horizontal");
            component.Contexts["logged in"] = new List<string> { "(default)", "VisibleWhenLoggedIn=false" };
            component.Contexts["templates"] = new List<string> { "(default)", "UserName=text" };
            return component;
        }

        static ProbeComponent LoginName()
        {
            var component = new ProbeComponent("LoginName", typeof(LoginName), () => new LoginName { ID = "lnName" });
            component.ContextSetups["logged in"] = LoggedIn;
            component.ExtraValues["FormatString"] = new List<Sample> { new Sample("welcome", "Welcome, {0}!"), new Sample("invalid", "{1}") };
            component.Interactions.AddRange(new[] { "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red", "FormatString=welcome+Visible=false" });
            AddContexts(component);
            component.Contexts["logged in"] = new List<string> { "(default)", "FormatString=welcome", "FormatString=markup", "FormatString=invalid", "FormatString=welcome+CssClass=text", "FormatString=welcome+EnableViewState=false" };
            return component;
        }

        static ProbeComponent LoginStatus()
        {
            var component = new ProbeComponent("LoginStatus", typeof(LoginStatus), () => new LoginStatus { ID = "lsStatus" });
            Urls(component);
            component.ContextSetups["logged in"] = LoggedIn;
            component.Interactions.AddRange(new[]
            {
                "LoginText=markup+LogoutText=markup", "LoginImageUrl=app-relative+LoginText=text", "LogoutAction=Redirect+LogoutPageUrl=app-relative",
                "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red", "LoginText=text+Visible=false", "LoginText=text+EnableViewState=false",
            });
            AddContexts(component);
            component.Contexts["logged in"] = new List<string>
            {
                "(default)", "LogoutText=markup", "LogoutImageUrl=app-relative+LogoutText=text", "LogoutAction=RedirectToLoginPage", "Enabled=false", "LogoutText=text+Visible=false",
            };
            return component;
        }

        static ProbeComponent LoginView()
        {
            var component = new ProbeComponent("LoginView", typeof(LoginView), () =>
            {
                var view = new LoginView
                {
                    ID = "lvStatus",
                    AnonymousTemplate = new ProbePlaceholderTemplate("<span>Anonymous</span>", "phAnonymous", ""),
                    LoggedInTemplate = new ProbePlaceholderTemplate("<span>Logged in</span>", "phLoggedIn", ""),
                };
                view.RoleGroups.Add(new RoleGroup { Roles = new[] { "admins" }, ContentTemplate = new ProbePlaceholderTemplate("<span>An admin</span>", "phAdmin", "") });
                return view;
            });
            component.ContextSetups["logged in"] = LoggedIn;
            component.ContextSetups["no templates"] = control =>
            {
                var view = (LoginView)control;
                view.AnonymousTemplate = null;
                view.LoggedInTemplate = null;
                view.RoleGroups.Clear();
            };
            component.Interactions.AddRange(new[] { "Visible=false+EnableViewState=false" });
            AddContexts(component);
            component.Contexts["logged in"] = new List<string> { "(default)", "Visible=false", "EnableViewState=false" };
            component.Contexts["no templates"] = new List<string> { "(default)" };
            return component;
        }

        /// <summary>A wizard of three steps: a step (a label), a templated step, a finish step.</summary>
        static Wizard Steps(Wizard wizard)
        {
            var first = new WizardStep { ID = "wsFirst", Title = "First" };
            first.Controls.Add(new Label { ID = "lblFirst", Text = "First step" });
            wizard.WizardSteps.Add(first);
            wizard.WizardSteps.Add(new TemplatedWizardStep { ID = "wsTemplated", Title = "Templated", ContentTemplate = new ProbePlaceholderTemplate("<span>Templated step</span>", "phTemplated", "") });
            var last = new WizardStep { ID = "wsLast", Title = "Last & final", StepType = WizardStepType.Finish };
            last.Controls.Add(new Label { ID = "lblLast", Text = "Last step" });
            wizard.WizardSteps.Add(last);
            return wizard;
        }

        static void WizardInteractions(ProbeComponent component)
        {
            Urls(component);
            component.ExtraValues["ActiveStepIndex"] = new List<Sample> { new Sample("1", 1), new Sample("2", 2) };
            component.Interactions.AddRange(new[]
            {
                "ActiveStepIndex=1+DisplaySideBar=false", "ActiveStepIndex=2+FinishCompleteButtonType=Link", "DisplayCancelButton=true+CancelButtonText=markup",
                "DisplayCancelButton=true+CancelButtonType=Image+CancelButtonImageUrl=app-relative", "StartNextButtonType=Link+StartNextButtonText=markup",
                "ActiveStepIndex=1+StepNextButtonType=Image+StepNextButtonImageUrl=app-relative", "HeaderText=markup+HeaderStyle.BackColor=Red",
                "DisplaySideBar=false+HeaderText=text", "SideBarStyle.BackColor=Red+SideBarButtonStyle.ForeColor=#3366CC", "ActiveStepIndex=1+CellPadding=3+CellSpacing=3",
                "Enabled=false+ActiveStepIndex=1", "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red",
                "ActiveStepIndex=1+Visible=false", "ActiveStepIndex=1+EnableViewState=false",
            });
            AddContexts(component, "ActiveStepIndex=1");
        }

        static ProbeComponent Wizard()
        {
            var component = new ProbeComponent("Wizard", typeof(Wizard), () => Steps(new Wizard { ID = "wizSteps" }));
            component.ItemPath = "WizardSteps[0]";
            component.ContextSetups["templates"] = control =>
            {
                var wizard = (Wizard)control;
                wizard.HeaderTemplate = new ProbePlaceholderTemplate("<span>Header</span>", "phHeader", "");
                wizard.StartNavigationTemplate = new ProbeControlsTemplate(() => new Control[] { new Button { ID = "btnStartNext", CommandName = "MoveNext", Text = "Go on" } });
                wizard.SideBarTemplate = new ProbeControlsTemplate(() => new Control[] { new DataList { ID = "SideBarList", ItemTemplate = new ProbeControlsTemplate(() => new Control[] { new LinkButton { ID = "SideBarButton" } }) } });
            };
            component.ContextSetups["layout"] = control => ((Wizard)control).LayoutTemplate = new ProbeControlsTemplate(() => new Control[]
            {
                new LiteralControl("<div class=\"layout\">"), new PlaceHolder { ID = "headerPlaceHolder" }, new PlaceHolder { ID = "sideBarPlaceHolder" },
                new PlaceHolder { ID = "WizardStepPlaceHolder" }, new PlaceHolder { ID = "navigationPlaceHolder" }, new LiteralControl("</div>"),
            });
            WizardInteractions(component);
            component.Contexts["templates"] = new List<string> { "(default)", "ActiveStepIndex=1", "HeaderText=text" };
            component.Contexts["layout"] = new List<string> { "(default)", "ActiveStepIndex=1" };
            return component;
        }

        /// <summary>A wizard step (WizardStep, TemplatedWizardStep): the second of a wizard, active.</summary>
        static ProbeComponent Step(string name, Func<WizardStepBase> make)
        {
            var component = new ProbeComponent(name, make().GetType(), make);
            component.Wrap = control =>
            {
                var wizard = new Wizard { ID = "wizHost" };
                var first = new WizardStep { ID = "wsBefore", Title = "Before" };
                first.Controls.Add(new Label { ID = "lblBefore", Text = "Before" });
                wizard.WizardSteps.Add(first);
                wizard.WizardSteps.Add((WizardStepBase)control);
                wizard.WizardSteps.Add(new WizardStep { ID = "wsAfter", Title = "After" });
                wizard.ActiveStepIndex = 1;
                return wizard;
            };
            component.Interactions.AddRange(new[] { "StepType=Finish+Title=markup", "StepType=Start+AllowReturn=false", "Title=text+Visible=false", "Title=text+EnableViewState=false" });
            AddContexts(component);
            return component;
        }

        static ProbeComponent CreateUserWizard()
        {
            var component = new ProbeComponent("CreateUserWizard", typeof(CreateUserWizard), () => new CreateUserWizard { ID = "cuwCreate" });
            component.ContextSetups["templates"] = control =>
            {
                var wizard = (CreateUserWizard)control;
                wizard.HeaderTemplate = new ProbePlaceholderTemplate("<span>Header</span>", "phHeader", "");
            };
            component.ContextSetups["logged in"] = LoggedIn;
            WizardInteractions(component);
            component.Interactions.AddRange(new[]
            {
                "RequireEmail=false+AutoGeneratePassword=true", "CreateUserButtonType=Link+CreateUserButtonText=markup", "UserName=text+Email=text+Question=text",
                "DisplayCancelButton=true+CancelDestinationPageUrl=app-relative", "InstructionText=markup+CompleteSuccessText=markup", "LoginCreatedUser=false+DisableCreatedUser=true",
                "TextBoxStyle.BackColor=Red+LabelStyle.ForeColor=#3366CC", "UserNameRequiredErrorMessage=markup+PasswordRequiredErrorMessage=text",
            });
            component.Contexts["templates"] = new List<string> { "(default)", "HeaderText=text" };
            component.Contexts["logged in"] = new List<string> { "(default)", "LoginCreatedUser=false" };
            return component;
        }

        /// <summary>A create user wizard's step (CreateUserWizardStep, CompleteWizardStep), in a create user wizard (active).</summary>
        static ProbeComponent CreateUserStep(string name, Func<WizardStepBase> make)
        {
            var component = new ProbeComponent(name, make().GetType(), make);
            component.Wrap = control =>
            {
                var wizard = new CreateUserWizard { ID = "cuwHost" };
                if (control is CompleteWizardStep)
                {
                    wizard.WizardSteps.Add(new CreateUserWizardStep { ID = "cuwsCreate" });
                    wizard.WizardSteps.Add((WizardStepBase)control);
                    wizard.ActiveStepIndex = 1;
                }
                else
                {
                    wizard.WizardSteps.Add((WizardStepBase)control);
                    wizard.WizardSteps.Add(new CompleteWizardStep { ID = "cwsComplete" });
                }
                return wizard;
            };
            component.Interactions.AddRange(new[] { "Title=markup+AllowReturn=false", "Title=text+Visible=false", "Title=text+EnableViewState=false" });
            AddContexts(component);
            return component;
        }

        static ProbeComponent ChangePassword()
        {
            var component = new ProbeComponent("ChangePassword", typeof(ChangePassword), () => new ChangePassword { ID = "cpwChange" });
            Urls(component);
            component.ContextSetups["logged in"] = LoggedIn;
            component.ContextSetups["templates"] = control =>
            {
                var change = (ChangePassword)control;
                change.ChangePasswordTemplate = new ProbeControlsTemplate(() => new Control[]
                {
                    new TextBox { ID = "CurrentPassword", TextMode = TextBoxMode.Password }, new TextBox { ID = "NewPassword", TextMode = TextBoxMode.Password },
                    new TextBox { ID = "ConfirmNewPassword", TextMode = TextBoxMode.Password }, new Literal { ID = "FailureText" },
                    new Button { ID = "ChangePasswordPushButton", CommandName = "ChangePassword", Text = "Change it" },
                });
                change.SuccessTemplate = new ProbePlaceholderTemplate("<span>Changed</span>", "phSuccess", "");
            };
            component.Interactions.AddRange(new[]
            {
                "DisplayUserName=true+UserName=text", "ChangePasswordButtonType=Link+ChangePasswordButtonText=markup", "CancelButtonType=Image+CancelButtonImageUrl=app-relative",
                "ChangePasswordTitleText=markup+InstructionText=markup", "PasswordHintText=markup+NewPasswordRegularExpression=text", "CreateUserText=text+CreateUserUrl=app-relative",
                "HelpPageText=text+HelpPageUrl=app-relative", "PasswordRecoveryText=text+PasswordRecoveryUrl=app-relative", "RenderOuterTable=false+DisplayUserName=true",
                "TextBoxStyle.BackColor=Red+TitleTextStyle.ForeColor=#3366CC", "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red",
                "DisplayUserName=true+Visible=false", "DisplayUserName=true+EnableViewState=false",
            });
            AddContexts(component, "DisplayUserName=true");
            component.Contexts["logged in"] = new List<string> { "(default)", "DisplayUserName=true" };
            component.Contexts["templates"] = new List<string> { "(default)" };
            return component;
        }

        static ProbeComponent PasswordRecovery()
        {
            var component = new ProbeComponent("PasswordRecovery", typeof(PasswordRecovery), () => new PasswordRecovery { ID = "prRecover" });
            Urls(component);
            component.ContextSetups["templates"] = control =>
            {
                var recovery = (PasswordRecovery)control;
                recovery.UserNameTemplate = new ProbeControlsTemplate(() => new Control[]
                {
                    new TextBox { ID = "UserName" }, new Literal { ID = "FailureText" }, new Button { ID = "SubmitButton", CommandName = "Submit", Text = "Send" },
                });
                recovery.SuccessTemplate = new ProbePlaceholderTemplate("<span>Sent</span>", "phSent", "");
            };
            component.Interactions.AddRange(new[]
            {
                "UserName=text+UserNameTitleText=markup", "SubmitButtonType=Link+SubmitButtonText=markup", "SubmitButtonType=Image+SubmitButtonImageUrl=app-relative",
                "UserNameInstructionText=markup+UserNameLabelText=markup", "HelpPageText=text+HelpPageUrl=app-relative", "RenderOuterTable=false+UserName=text",
                "TextBoxStyle.BackColor=Red+TitleTextStyle.ForeColor=#3366CC", "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red",
                "UserName=text+Visible=false", "UserName=text+EnableViewState=false",
            });
            AddContexts(component, "UserName=text");
            component.Contexts["templates"] = new List<string> { "(default)" };
            return component;
        }

        /// <summary>A data source control (it renders nothing: its properties and its view state are the cases).</summary>
        static ProbeComponent DataSource(string name, Func<Control> make, params string[] interactions)
        {
            var component = new ProbeComponent(name, make().GetType(), make);
            component.ExtraValues["TypeName"] = new List<Sample> { new Sample("store", "WebControlsProbe.ProductStore"), new Sample("missing", "WebControlsProbe.NoSuchType") };
            component.ExtraValues["SelectMethod"] = new List<Sample> { new Sample("SelectAll", "SelectAll") };
            component.ExtraValues["ProviderName"] = new List<Sample> { new Sample("probe", "WebControlsProbe.Data"), new Sample("missing", "No.Such.Provider") };
            component.ExtraValues["SelectCommand"] = new List<Sample> { new Sample("select", "select") };
            component.ExtraValues["DataFile"] = new List<Sample> { new Sample("app-relative", "~/App_Data/products.xml"), new Sample("missing", "~/App_Data/none.xml") };
            component.ExtraValues["TransformFile"] = new List<Sample> { new Sample("app-relative", "~/App_Data/probe.xslt") };
            component.ExtraValues["XPath"] = new List<Sample> { new Sample("products", "products/product") };
            component.ExtraValues["ContextTypeName"] = new List<Sample> { new Sample("context", "WebControlsProbe.ProbeLinqContext") };
            component.ExtraValues["TableName"] = new List<Sample> { new Sample("Products", "Products") };
            component.ExtraValues["TargetControlID"] = new List<Sample> { new Sample("linq", "ldsTarget") };
            component.ExtraValues["ModelStateKey"] = new List<Sample> { new Sample("Name", "Name") };
            component.ExtraValues["AssociatedControlID"] = new List<Sample> { new Sample("target", "txtTarget") };
            component.Interactions.AddRange(interactions);
            component.Interactions.AddRange(new[] { "Visible=false+EnableViewState=false" });
            AddContexts(component);
            return component;
        }

        static ProbeComponent QueryExtenderComponent()
        {
            var component = DataSource("QueryExtender", () =>
            {
                var extender = new QueryExtender { ID = "qeProducts", TargetControlID = "ldsTarget" };
                var search = new System.Web.UI.WebControls.Expressions.SearchExpression { DataFields = "Name", SearchType = System.Web.UI.WebControls.Expressions.SearchType.Contains };
                search.Parameters.Add(new Parameter("name", TypeCode.String, "e"));
                extender.Expressions.Add(search);
                return extender;
            }, "TargetControlID=linq+Visible=false");
            component.Siblings = () => new Control[] { new LinqDataSource { ID = "ldsTarget", ContextTypeName = "WebControlsProbe.ProbeLinqContext", TableName = "Products" } };
            return component;
        }

        static ProbeComponent ModelErrorMessageComponent()
        {
            var component = DataSource("ModelErrorMessage", () => new ModelErrorMessage { ID = "memName", ModelStateKey = "Name" },
                "SetFocusOnError=true+AssociatedControlID=target", "ModelStateKey=Name+CssClass=text");
            component.Siblings = () => new Control[] { new TextBox { ID = "txtTarget" } };
            component.ContextSetups["error"] = control => control.Init += (sender, args) => ((Control)sender).Page.ModelState.AddModelError("Name", "The name <b>is</b> required.");
            component.Contexts["error"] = new List<string> { "(default)", "ModelStateKey=Name+CssClass=text", "SetFocusOnError=true+AssociatedControlID=target", "ModelStateKey=text", "Enabled=false", "Visible=false", StyleCombination };
            return component;
        }

        /// <summary>The web parts' controls: after the page's web part manager (wpmPage), as they must be; a part in its zone.</summary>
        static Control AfterWebPartManager(Control control, Func<Control, Control> zone = null)
        {
            var holder = new PlaceHolder();
            holder.Controls.Add(new System.Web.UI.WebControls.WebParts.WebPartManager { ID = "wpmPage" });
            holder.Controls.Add(zone == null ? control : zone(control));
            return holder;
        }

        static ProbeComponent WebPart(string name, Func<Control> make, Func<Control, Control> zone, params string[] interactions)
        {
            var component = new ProbeComponent(name, make().GetType(), make);
            if (name != "WebPartManager") component.Wrap = control => AfterWebPartManager(control, zone);
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.Interactions.AddRange(interactions);
            component.Interactions.AddRange(new[] { "Visible=false+EnableViewState=false" });
            if (typeof(WebControl).IsAssignableFrom(component.Type))
                component.Interactions.AddRange(new[] { "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red" });
            AddContexts(component);
            return component;
        }

        static Control InCatalogZone(Control part) =>
            new System.Web.UI.WebControls.WebParts.CatalogZone { ID = "czHost", ZoneTemplate = new ProbeControlsTemplate(() => new[] { part }) };

        static Control InEditorZone(Control part) =>
            new System.Web.UI.WebControls.WebParts.EditorZone { ID = "ezHost", ZoneTemplate = new ProbeControlsTemplate(() => new[] { part }) };

        // The HTML server controls (System.Web.UI.HtmlControls): an element of the markup with runat="server".
        static ProbeComponent Html(string name, Func<HtmlControl> make, params string[] interactions)
        {
            var component = new ProbeComponent(name, make().GetType(), make);
            component.ExtraValues["HRef"] = new List<Sample> { new Sample("app-relative", "~/ButtonTarget.aspx?a=1&b=2"), new Sample("absolute", "http://example.com/a b?x=<y>") };
            component.ExtraValues["Href"] = new List<Sample> { new Sample("app-relative", "~/Images/probe.css"), new Sample("absolute", "http://example.com/a b.css?x=1&y=2") };
            component.ExtraValues["Src"] = new List<Sample> { AppRelativeImage, AbsoluteImage };
            component.ExtraValues["Target"] = new List<Sample> { new Sample("blank", "_blank") };
            component.ExtraValues["Name"] = new List<Sample> { new Sample("group", "group") };
            component.ExtraValues["Poster"] = new List<Sample> { AppRelativeImage };
            component.ExtraValues["Manifest"] = new List<Sample> { new Sample("app-relative", "~/Images/probe.appcache") };
            component.ExtraValues["TagName"] = new List<Sample> { new Sample("div", "div"), new Sample("section", "section") };
            component.Interactions.AddRange(interactions);
            component.Interactions.AddRange(new[] { "Disabled=true+Attributes[data-probe]=text", "Attributes[class]=text+Style[color]=red", "Visible=false+EnableViewState=false" });
            AddContexts(component);
            return component;
        }

        static HtmlTableRow HtmlRow(string first, string second)
        {
            var row = new HtmlTableRow { ID = "trRow" };
            row.Cells.Add(new HtmlTableCell { InnerText = first });
            row.Cells.Add(new HtmlTableCell { InnerText = second });
            return row;
        }

        static ProbeComponent HtmlSelect()
        {
            var component = Html("HtmlSelect", () =>
            {
                var select = new HtmlSelect { ID = "selSize" };
                select.Items.Add(new ListItem("Small", "S"));
                select.Items.Add(new ListItem("Medium", "M"));
                select.Items.Add(new ListItem("Large", "L"));
                return select;
            }, "Multiple=true+Size=3", "Multiple=true+Items[0].Selected=true+Items[2].Selected=true", "SelectedIndex=1+Items[1].Enabled=false",
               "Items[1].Text=markup+Items[1].Value=markup", "SelectedIndex=1+Disabled=true", "SelectedIndex=1+EnableViewState=false");
            component.ItemPath = "Items[1]";
            component.ExtraValues["SelectedIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["Value"] = new List<Sample> { new Sample("M", "M") };
            component.ExtraValues["DataTextField"] = new List<Sample> { new Sample("Name", "Name") };
            component.ExtraValues["DataValueField"] = new List<Sample> { new Sample("Code", "Code") };
            component.Contexts["data-bound"] = new List<string> { "(default)", "DataTextField=Name", "DataValueField=Code", "DataTextField=Name+DataValueField=Code", "DataTextField=Name+DataValueField=Code+Value=M" };
            return component;
        }

        // A property: All (above) is made before the fields below it are.
        static string[] InputInteractions => new[] { "Value=markup+Disabled=true", "Value=text+Visible=false", "Value=markup+EnableViewState=false", "Name=group+Value=text" };

        static ProbeComponent TextBox()
        {
            var component = new ProbeComponent("TextBox", typeof(TextBox), () => new TextBox { ID = "txtName" });
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["Text"] = new List<Sample>
            {
                new Sample("lines", "Line1\r\nLine2"),
                new Sample("leading-newline", "\r\nabc"),
                new Sample("spaces", "  a  b  "),
            };
            component.Interactions.AddRange(new[]
            {
                "TextMode=MultiLine+Text=lines",
                "TextMode=MultiLine+Text=leading-newline",
                "TextMode=MultiLine+Text=markup",
                "TextMode=MultiLine+Rows=3+Columns=3",
                "TextMode=MultiLine+Wrap=false",
                "TextMode=MultiLine+MaxLength=3",
                "TextMode=MultiLine+ReadOnly=true",
                "TextMode=MultiLine+AutoPostBack=true",
                "TextMode=Password+Text=text",
                "TextMode=Password+AutoCompleteType=Disabled",
                "TextMode=Number+MaxLength=3",
                "TextMode=Date+Text=text",
                "ReadOnly=true+Enabled=false",
                "Enabled=false+CssClass=text",
                "AutoPostBack=true+CausesValidation=true",
                "AutoPostBack=true+CausesValidation=true+ValidationGroup=text",
                "AutoPostBack=true+Enabled=false",
                "AutoPostBack=true+Visible=false",
                "Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true",
                "Columns=3+Width=120px",
                "ToolTip=text+Attributes[data-probe]=text",
                "ForeColor=#3366CC+Style[color]=red",
                "Text=markup+EnableViewState=false",
                "Text=text+Visible=false",
            });
            component.Contexts["naming container"] = new List<string>
            {
                "(default)", "ClientIDMode=AutoID", "ClientIDMode=Static", "ClientIDMode=Predictable", "ClientIDMode=Inherit",
                "AutoPostBack=true", "ID=text",
            };
            component.Contexts["themed"] = new List<string>
            {
                "(default)", "SkinID=text", "EnableTheming=false", "CssClass=text", "Width=120px",
            };
            return component;
        }

        static ProbeComponent Button()
        {
            var component = new ProbeComponent("Button", typeof(Button), () => new Button { ID = "btnGo" });
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["OnClientClick"] = new List<Sample> { new Sample("confirm", "return confirm('OK?');") };
            component.ExtraValues["PostBackUrl"] = new List<Sample>
            {
                new Sample("app-relative", "~/ButtonTarget.aspx"),
                new Sample("absolute", "http://example.com/target.aspx?a=1&b=2"),
            };
            component.Interactions.AddRange(new[]
            {
                "UseSubmitBehavior=false+Text=text",
                "UseSubmitBehavior=false+CausesValidation=false",
                "UseSubmitBehavior=false+ValidationGroup=text",
                "UseSubmitBehavior=false+OnClientClick=confirm",
                "UseSubmitBehavior=false+PostBackUrl=app-relative",
                "UseSubmitBehavior=false+Enabled=false",
                "UseSubmitBehavior=false+Visible=false",
                "OnClientClick=confirm+CausesValidation=false",
                "OnClientClick=confirm+ValidationGroup=text",
                "OnClientClick=confirm+Enabled=false",
                "PostBackUrl=app-relative+ValidationGroup=text",
                "PostBackUrl=app-relative+CausesValidation=false",
                "PostBackUrl=app-relative+OnClientClick=confirm",
                "PostBackUrl=app-relative+Enabled=false",
                "CommandName=text+CommandArgument=markup",
                "Enabled=false+CssClass=text",
                "Text=markup+ToolTip=markup",
                "Text=text+Visible=false",
                "Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true",
                "ForeColor=#3366CC+Style[color]=red",
                "Text=markup+EnableViewState=false",
            });
            component.Contexts["naming container"] = new List<string>
            {
                "(default)", "ClientIDMode=AutoID", "ClientIDMode=Static", "ClientIDMode=Predictable", "ClientIDMode=Inherit",
                "UseSubmitBehavior=false", "PostBackUrl=app-relative", "ValidationGroup=text", "ID=text",
            };
            component.Contexts["themed"] = new List<string>
            {
                "(default)", "SkinID=text", "EnableTheming=false", "CssClass=text", "Width=120px",
            };
            return component;
        }

        const string StyleCombination = "Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true";

        // Properties, not fields: All (above) is made before the fields below it are.
        static Sample AppRelativeImage => new Sample("app-relative", "~/Images/probe.png");
        static Sample AbsoluteImage => new Sample("absolute", "http://example.com/images/a b.png?x=1&y=2");

        /// <summary>The naming container and theme contexts every component has, with the component's own bodies.</summary>
        static void AddContexts(ProbeComponent component, params string[] naming)
        {
            component.Contexts["naming container"] = new[] { "(default)", "ClientIDMode=AutoID", "ClientIDMode=Static", "ClientIDMode=Predictable", "ClientIDMode=Inherit", "ID=text" }.Concat(naming).ToList();
            component.Contexts["themed"] = new List<string> { "(default)", "SkinID=text", "EnableTheming=false" };
            if (component.Type.GetProperty("CssClass") != null) component.Contexts["themed"].Add("CssClass=text");
        }

        static ProbeComponent Label()
        {
            var component = new ProbeComponent("Label", typeof(Label), () => new Label { ID = "lblName" });
            component.Siblings = () => new Control[] { new TextBox { ID = "txtTarget" } };
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["AssociatedControlID"] = new List<Sample> { new Sample("target", "txtTarget") };
            component.Interactions.AddRange(new[]
            {
                "AssociatedControlID=target+Text=markup", "AssociatedControlID=target+Enabled=false", "AssociatedControlID=target+AccessKey=k",
                "Text=markup+Enabled=false", "Enabled=false+CssClass=text", "Text=empty+ToolTip=text", StyleCombination,
                "ForeColor=#3366CC+Style[color]=red", "Text=text+EnableViewState=false", "Text=text+Visible=false",
            });
            AddContexts(component, "AssociatedControlID=target");
            return component;
        }

        /// <summary>CheckBox and RadioButton (a RadioButton is a CheckBox with a group).</summary>
        static ProbeComponent Toggle(string name, Func<CheckBox> make)
        {
            var type = make().GetType();
            var component = new ProbeComponent(name, type, make);
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.Interactions.AddRange(new[]
            {
                "Checked=true+Enabled=false", "Text=text+TextAlign=Left", "Text=markup+Checked=true",
                "AutoPostBack=true+CausesValidation=true", "AutoPostBack=true+CausesValidation=true+ValidationGroup=text", "AutoPostBack=true+Enabled=false",
                "InputAttributes[data-probe]=text", "LabelAttributes[data-probe]=text", "InputAttributes[data-probe]=text+Text=text", "LabelAttributes[data-probe]=text+Text=text",
                "Text=text+CssClass=text", "Enabled=false+CssClass=text", "Text=text+ToolTip=text", StyleCombination,
                "ForeColor=#3366CC+Style[color]=red", "Checked=true+EnableViewState=false", "Checked=true+Visible=false",
            });
            if (type == typeof(RadioButton))
                component.Interactions.AddRange(new[] { "GroupName=text+Checked=true", "GroupName=text+AutoPostBack=true" });
            AddContexts(component, "AutoPostBack=true", "Text=text");
            if (type == typeof(RadioButton)) component.Contexts["naming container"].Add("GroupName=text");
            return component;
        }

        static ProbeComponent Literal()
        {
            var component = new ProbeComponent("Literal", typeof(Literal), () => new Literal { ID = "litText" });
            component.Interactions.AddRange(new[]
            {
                "Mode=Encode+Text=markup", "Mode=PassThrough+Text=markup", "Mode=Transform+Text=markup", "Mode=Encode+Text=japanese",
                "Text=text+EnableViewState=false", "Text=text+Visible=false",
            });
            AddContexts(component);
            return component;
        }

        /// <summary>LinkButton and ImageButton: the buttons other than Button (IButtonControl).</summary>
        static ProbeComponent Link(string name, Func<WebControl> make)
        {
            var type = make().GetType();
            var component = new ProbeComponent(name, type, make);
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["OnClientClick"] = new List<Sample> { new Sample("confirm", "return confirm('OK?');") };
            component.ExtraValues["PostBackUrl"] = new List<Sample> { new Sample("app-relative", "~/ButtonTarget.aspx"), new Sample("absolute", "http://example.com/target.aspx?a=1&b=2") };
            component.ExtraValues["ImageUrl"] = new List<Sample> { AppRelativeImage, AbsoluteImage };
            component.Interactions.AddRange(new[]
            {
                "OnClientClick=confirm+CausesValidation=false", "OnClientClick=confirm+ValidationGroup=text", "OnClientClick=confirm+Enabled=false",
                "PostBackUrl=app-relative+ValidationGroup=text", "PostBackUrl=app-relative+CausesValidation=false", "PostBackUrl=app-relative+OnClientClick=confirm",
                "PostBackUrl=app-relative+Enabled=false", "CommandName=text+CommandArgument=markup", "Enabled=false+CssClass=text", StyleCombination,
                "ForeColor=#3366CC+Style[color]=red",
            });
            if (type == typeof(LinkButton))
                component.Interactions.AddRange(new[] { "Text=markup+ToolTip=markup", "Text=text+Visible=false", "Text=markup+EnableViewState=false", "Text=text+Enabled=false" });
            else
                component.Interactions.AddRange(new[] { "ImageUrl=app-relative+AlternateText=text", "ImageUrl=app-relative+Visible=false", "ImageUrl=app-relative+EnableViewState=false", "ImageUrl=app-relative+Enabled=false" });
            AddContexts(component, "PostBackUrl=app-relative", "ValidationGroup=text");
            return component;
        }

        static ProbeComponent HyperLink()
        {
            var component = new ProbeComponent("HyperLink", typeof(HyperLink), () => new HyperLink { ID = "hypGo" });
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["NavigateUrl"] = new List<Sample>
            {
                new Sample("app-relative", "~/ButtonTarget.aspx?a=1&b=2"), new Sample("absolute", "http://example.com/a b?x=<y>"), new Sample("script", "javascript:alert('x')"),
            };
            component.ExtraValues["ImageUrl"] = new List<Sample> { AppRelativeImage, AbsoluteImage };
            component.ExtraValues["Target"] = new List<Sample> { new Sample("blank", "_blank") };
            component.Interactions.AddRange(new[]
            {
                "NavigateUrl=app-relative+Text=text", "NavigateUrl=app-relative+Enabled=false", "ImageUrl=app-relative+Text=text",
                "ImageUrl=app-relative+ImageWidth=120px+ImageHeight=2em", "ImageUrl=app-relative+ToolTip=text", "NavigateUrl=absolute+Target=blank",
                "Text=markup+NavigateUrl=script", "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red",
                "Text=text+Visible=false", "Text=text+EnableViewState=false",
            });
            AddContexts(component, "NavigateUrl=app-relative");
            return component;
        }

        static ProbeComponent Picture(string name, Func<System.Web.UI.WebControls.Image> make)
        {
            var component = new ProbeComponent(name, make().GetType(), make);
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["ImageUrl"] = new List<Sample> { AppRelativeImage, AbsoluteImage };
            component.ExtraValues["DescriptionUrl"] = new List<Sample> { new Sample("app-relative", "~/ButtonTarget.aspx") };
            component.Interactions.AddRange(new[]
            {
                "ImageUrl=app-relative+AlternateText=text", "ImageUrl=app-relative+GenerateEmptyAlternateText=true", "AlternateText=empty+GenerateEmptyAlternateText=true",
                "ImageUrl=app-relative+ImageAlign=Left", "ImageUrl=app-relative+Enabled=false", "ImageUrl=app-relative+" + StyleCombination,
                "Enabled=false+CssClass=text", "ForeColor=#3366CC+Style[color]=red", "ImageUrl=app-relative+Visible=false", "ImageUrl=app-relative+EnableViewState=false",
            });
            AddContexts(component, "ImageUrl=app-relative");
            return component;
        }

        static ProbeComponent Panel()
        {
            var component = new ProbeComponent("Panel", typeof(Panel), () =>
            {
                var panel = new Panel { ID = "pnlBox" };
                panel.Controls.Add(new Label { ID = "lblInside", Text = "Inside" });
                panel.Controls.Add(new Button { ID = "btnInside", Text = "Inside" });
                return panel;
            });
            component.ExtraValues["DefaultButton"] = new List<Sample> { new Sample("inside", "btnInside") };
            component.ExtraValues["BackImageUrl"] = new List<Sample> { AppRelativeImage };
            component.Interactions.AddRange(new[]
            {
                "GroupingText=text+Direction=RightToLeft", "DefaultButton=inside+Enabled=false", "ScrollBars=Auto+Height=2em", "HorizontalAlign=Center+Wrap=false",
                "BackImageUrl=app-relative+Width=120px", "GroupingText=text+CssClass=text", "Enabled=false+CssClass=text", StyleCombination,
                "ForeColor=#3366CC+Style[color]=red", "GroupingText=text+Visible=false", "GroupingText=text+EnableViewState=false",
            });
            AddContexts(component, "DefaultButton=inside");
            return component;
        }

        static ProbeComponent HiddenField()
        {
            var component = new ProbeComponent("HiddenField", typeof(HiddenField), () => new HiddenField { ID = "hidValue" });
            component.Interactions.AddRange(new[] { "Value=markup+EnableViewState=false", "Value=text+Visible=false" });
            AddContexts(component);
            return component;
        }

        static ProbeComponent PlaceHolder()
        {
            var component = new ProbeComponent("PlaceHolder", typeof(PlaceHolder), () =>
            {
                var holder = new PlaceHolder { ID = "phArea" };
                holder.Controls.Add(new Label { ID = "lblInside", Text = "Inside" });
                holder.Controls.Add(new Literal { Text = "<b>literal</b>" });
                return holder;
            });
            component.Interactions.AddRange(new[] { "Visible=false+EnableViewState=false" });
            AddContexts(component);
            return component;
        }

        /// <summary>The text boxes a validator validates and compares with.</summary>
        static Control[] ValidatorTargets() => new Control[] { new TextBox { ID = "txtTarget", Text = "abc" }, new TextBox { ID = "txtOther", Text = "xyz" } };

        /// <summary>A validator of txtTarget, with an error message; its type's own samples and interactions.</summary>
        static ProbeComponent Validator(string name, Func<BaseValidator> make)
        {
            var type = make().GetType();
            var component = new ProbeComponent(name, type, () =>
            {
                var validator = make();
                validator.ID = "valCheck";
                validator.ControlToValidate = "txtTarget";
                validator.ErrorMessage = "Error message";
                return validator;
            });
            component.Siblings = ValidatorTargets;
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["ControlToValidate"] = new List<Sample> { new Sample("target", "txtTarget"), new Sample("other", "txtOther") };
            component.ExtraValues["ControlToCompare"] = new List<Sample> { new Sample("other", "txtOther") };
            component.ExtraValues["ValidationExpression"] = new List<Sample> { new Sample("digits", "[0-9]+"), new Sample("letters", "[a-z]+") };
            component.ExtraValues["ClientValidationFunction"] = new List<Sample> { new Sample("checkOk", "checkOk") };
            foreach (var property in new[] { "ValueToCompare", "MinimumValue", "MaximumValue" })
                component.ExtraValues[property] = new List<Sample> { new Sample("3", "3"), new Sample("10", "10"), new Sample("date", "2020-01-02"), new Sample("decimal", "1,234.5") };
            component.Interactions.AddRange(new[]
            {
                "IsValid=false+Display=Dynamic", "IsValid=false+Display=None", "IsValid=false+Text=text", "IsValid=false+ErrorMessage=markup",
                "IsValid=false+Text=markup+ErrorMessage=text", "IsValid=false+EnableClientScript=false", "IsValid=false+ForeColor=#3366CC",
                "IsValid=false+Enabled=false", "IsValid=false+Visible=false", "EnableClientScript=false+Display=Dynamic",
                "SetFocusOnError=true+EnableClientScript=false", "ValidationGroup=text+EnableClientScript=false",
                "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red",
                "ErrorMessage=text+EnableViewState=false",
            });
            if (type == typeof(RegularExpressionValidator))
                component.Interactions.AddRange(new[] { "ValidationExpression=letters+IsValid=false", "ValidationExpression=digits+EnableClientScript=false" });
            if (type == typeof(CompareValidator))
                component.Interactions.AddRange(new[]
                {
                    "ControlToCompare=other+Operator=Equal", "Type=Date+ValueToCompare=date", "Type=Double+ValueToCompare=decimal",
                    "Type=Double+ValueToCompare=decimal+CultureInvariantValues=true", "Operator=DataTypeCheck+Type=Integer", "Type=Integer+ValueToCompare=text",
                    "Type=Currency+ValueToCompare=decimal",
                });
            if (type == typeof(RangeValidator))
                component.Interactions.AddRange(new[]
                {
                    "MinimumValue=10+MaximumValue=3", "Type=Date+MinimumValue=date+MaximumValue=date", "Type=String+MinimumValue=text+MaximumValue=text",
                    "Type=Double+MinimumValue=3+MaximumValue=decimal+CultureInvariantValues=true", "Type=Integer+MinimumValue=text",
                });
            if (type == typeof(CustomValidator))
                component.Interactions.AddRange(new[] { "ClientValidationFunction=checkOk+ValidateEmptyText=true", "ControlToValidate=empty+ClientValidationFunction=checkOk" });
            AddContexts(component, "IsValid=false", "ValidationGroup=text");
            return component;
        }

        /// <summary>A validation summary; beside it, two failed validators (one in group g2) and one that passed.</summary>
        static ProbeComponent Summary()
        {
            var component = new ProbeComponent("ValidationSummary", typeof(ValidationSummary), () => new ValidationSummary { ID = "sumErrors" });
            component.Siblings = () => new Control[]
            {
                new TextBox { ID = "txtTarget" },
                new RequiredFieldValidator { ID = "reqFirst", ControlToValidate = "txtTarget", ErrorMessage = "First error", Text = "*", IsValid = false },
                new RequiredFieldValidator { ID = "reqSecond", ControlToValidate = "txtTarget", ErrorMessage = "Second <b>error</b>", ValidationGroup = "g2", IsValid = false },
                new RequiredFieldValidator { ID = "reqPassed", ControlToValidate = "txtTarget", ErrorMessage = "Passed" },
            };
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["ValidationGroup"] = new List<Sample> { new Sample("g2", "g2") };
            component.Interactions.AddRange(new[]
            {
                "DisplayMode=List+HeaderText=markup", "DisplayMode=SingleParagraph+HeaderText=text", "DisplayMode=BulletList+HeaderText=text",
                "ShowMessageBox=true+ShowSummary=false", "ShowMessageBox=true+EnableClientScript=false", "EnableClientScript=false+HeaderText=text",
                "ValidationGroup=g2+HeaderText=text", "ShowValidationErrors=false+HeaderText=text", "Enabled=false+CssClass=text", StyleCombination,
                "ForeColor=#3366CC+Style[color]=red", "HeaderText=text+Visible=false", "HeaderText=text+EnableViewState=false",
            });
            AddContexts(component, "ValidationGroup=g2", "EnableClientScript=false");
            return component;
        }

        /// <summary>A grid of ProbeData.Many(): its columns generated, or (context "fields") one of each kind.</summary>
        static ProbeComponent GridView()
        {
            var component = new ProbeComponent("GridView", typeof(GridView), () => new GridView { ID = "gvItems" });
            component.Data = ProbeData.Many;
            Action<Control> fields = control =>
            {
                var grid = (GridView)control;
                grid.AutoGenerateColumns = false;
                grid.Columns.Add(new BoundField { DataField = "Name", HeaderText = "Name", SortExpression = "Name" });
                grid.Columns.Add(new BoundField { DataField = "Price", HeaderText = "Price", DataFormatString = "{0:N2}", SortExpression = "Price" });
                grid.Columns.Add(new CheckBoxField { DataField = "Active", HeaderText = "Active" });
                grid.Columns.Add(new HyperLinkField { Text = "Open", DataNavigateUrlFields = new[] { "Code" }, DataNavigateUrlFormatString = "~/ButtonTarget.aspx?code={0}" });
                grid.Columns.Add(new ImageField { DataImageUrlField = "Image", DataAlternateTextField = "Name" });
                grid.Columns.Add(new ButtonField { CommandName = "pick", Text = "Pick" });
                grid.Columns.Add(new CommandField { ShowSelectButton = true, ShowEditButton = true, ShowDeleteButton = true });
                grid.Columns.Add(new TemplateField { HeaderText = "Template", ItemTemplate = new ProbeTemplate("item"), FooterTemplate = new ProbeTemplate("footer") });
            };
            component.ContextSetups["fields"] = fields;
            component.ContextSetups["empty"] = fields;
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["EditIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["SelectedIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["PageIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["DataKeyNames"] = new List<Sample> { new Sample("Code", new[] { "Code" }) };
            component.ExtraValues["RowHeaderColumn"] = new List<Sample> { new Sample("Name", "Name") };
            component.Interactions.AddRange(new[]
            {
                "AllowPaging=true+PageSize=3", "AllowPaging=true+PageSize=3+PageIndex=1", "AllowPaging=true+PageSize=3+PageIndex=3",
                "AllowPaging=true+PageSize=3+PagerSettings.Position=TopAndBottom", "AllowPaging=true+PageSize=3+PagerSettings.Mode=NextPrevious",
                "AllowPaging=true+PageSize=3+PagerSettings.Mode=NumericFirstLast", "AllowPaging=true+PageSize=3+PagerSettings.Visible=false",
                "AllowPaging=true+PageSize=3+PagerStyle.BackColor=Red", "AllowSorting=true+AllowPaging=true",
                "AutoGenerateEditButton=true+AutoGenerateDeleteButton=true+AutoGenerateSelectButton=true", "SelectedIndex=1+SelectedRowStyle.BackColor=Red",
                "EditIndex=1+EditRowStyle.BackColor=Red", "DataKeyNames=Code+EditIndex=1", "DataKeyNames=Code+SelectedIndex=1", "GridLines=None+CellPadding=3",
                "ShowFooter=true+FooterStyle.BackColor=Red", "ShowHeader=false+ShowFooter=true", "Caption=text+CaptionAlign=Bottom",
                "UseAccessibleHeader=true+RowHeaderColumn=Name", "AlternatingRowStyle.BackColor=Red+RowStyle.BackColor=#3366CC",
                "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red",
                "AllowPaging=true+PageSize=3+EnableViewState=false", "AllowPaging=true+Visible=false",
            });
            AddContexts(component, "AllowPaging=true+PageSize=3", "SelectedIndex=1", "AutoGenerateSelectButton=true");
            component.Contexts["fields"] = new List<string>
            {
                "(default)", "AllowSorting=true", "AllowPaging=true+PageSize=3", "AllowPaging=true+PageSize=3+PageIndex=1", "EditIndex=1",
                "EditIndex=1+DataKeyNames=Code", "SelectedIndex=1", "ShowFooter=true", "ShowHeader=false", "DataKeyNames=Code",
                "UseAccessibleHeader=true+RowHeaderColumn=Name", "AllowSorting=true+AllowPaging=true+PageSize=3", "Enabled=false",
            };
            component.Contexts["empty"] = new List<string> { "(default)", "EmptyDataText=text", "EmptyDataText=markup", "ShowHeaderWhenEmpty=true", "ShowHeaderWhenEmpty=true+EmptyDataText=text" };
            return component;
        }

        /// <summary>A repeater of ProbeData.Many() with an item template; (context "templates") header, footer, separator, alternating ones.</summary>
        static ProbeComponent Repeater()
        {
            var component = new ProbeComponent("Repeater", typeof(Repeater), () => new Repeater { ID = "rptItems", ItemTemplate = new ProbeTemplate("item") });
            component.Data = ProbeData.Many;
            Action<Control> templates = control =>
            {
                var repeater = (Repeater)control;
                repeater.HeaderTemplate = new ProbeTemplate("header");
                repeater.FooterTemplate = new ProbeTemplate("footer");
                repeater.SeparatorTemplate = new ProbeTemplate("separator");
                repeater.AlternatingItemTemplate = new ProbeTemplate("alternating");
            };
            component.ContextSetups["templates"] = templates;
            component.ContextSetups["empty"] = templates;
            component.ContextSetups["no template"] = control => ((Repeater)control).ItemTemplate = null;
            component.Interactions.AddRange(new[] { "EnableViewState=false+Visible=false" });
            AddContexts(component);
            component.Contexts["templates"] = new List<string> { "(default)", "EnableViewState=false", "Visible=false" };
            component.Contexts["empty"] = new List<string> { "(default)" };
            component.Contexts["no template"] = new List<string> { "(default)" };
            return component;
        }

        static ProbeComponent FormView()
        {
            var component = new ProbeComponent("FormView", typeof(FormView), () => new FormView
            {
                ID = "fvItem", ItemTemplate = new ProbeTemplate("item"), EditItemTemplate = new ProbeTemplate("edit"), InsertItemTemplate = new ProbeTemplate("insert"),
            });
            component.Data = ProbeData.Many;
            Action<Control> templates = control =>
            {
                var form = (FormView)control;
                form.HeaderTemplate = new ProbeTemplate("header");
                form.FooterTemplate = new ProbeTemplate("footer");
                form.PagerTemplate = new ProbeTemplate("pager");
            };
            component.ContextSetups["templates"] = templates;
            component.ContextSetups["empty"] = control => ((FormView)control).EmptyDataTemplate = new ProbeTemplate("empty");
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["PageIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["DataKeyNames"] = new List<Sample> { new Sample("Code", new[] { "Code" }) };
            component.Interactions.AddRange(new[]
            {
                "AllowPaging=true+PageIndex=1", "AllowPaging=true+PageIndex=3", "AllowPaging=true+PagerSettings.Mode=NumericFirstLast",
                "AllowPaging=true+PagerSettings.Mode=NextPreviousFirstLast", "AllowPaging=true+PagerSettings.Position=Top", "AllowPaging=true+PagerStyle.BackColor=Red",
                "DefaultMode=Edit+DataKeyNames=Code", "HeaderText=text+FooterText=markup", "RenderOuterTable=false+AllowPaging=true",
                "GridLines=Both+CellPadding=3", "Caption=text+CaptionAlign=Bottom", "DefaultMode=Edit+EditRowStyle.BackColor=Red", "DefaultMode=Insert+InsertRowStyle.BackColor=Red",
                "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red", "AllowPaging=true+EnableViewState=false", "AllowPaging=true+Visible=false",
            });
            AddContexts(component, "AllowPaging=true");
            component.Contexts["templates"] = new List<string> { "(default)", "AllowPaging=true", "DefaultMode=Edit", "HeaderText=text+FooterText=text" };
            component.Contexts["empty"] = new List<string> { "(default)", "EmptyDataText=text", "DefaultMode=Insert" };
            return component;
        }

        static ProbeComponent DetailsView()
        {
            var component = new ProbeComponent("DetailsView", typeof(DetailsView), () => new DetailsView { ID = "dvItem" });
            component.Data = ProbeData.Many;
            component.ContextSetups["fields"] = control =>
            {
                var details = (DetailsView)control;
                details.AutoGenerateRows = false;
                details.Fields.Add(new BoundField { DataField = "Name", HeaderText = "Name" });
                details.Fields.Add(new BoundField { DataField = "Price", HeaderText = "Price", DataFormatString = "{0:N2}" });
                details.Fields.Add(new CheckBoxField { DataField = "Active", HeaderText = "Active" });
                details.Fields.Add(new HyperLinkField { Text = "Open", DataNavigateUrlFields = new[] { "Code" }, DataNavigateUrlFormatString = "~/ButtonTarget.aspx?code={0}" });
                details.Fields.Add(new TemplateField { HeaderText = "Template", ItemTemplate = new ProbeTemplate("item"), EditItemTemplate = new ProbeTemplate("edit"), InsertItemTemplate = new ProbeTemplate("insert") });
                details.Fields.Add(new CommandField { ShowEditButton = true, ShowInsertButton = true, ShowDeleteButton = true });
            };
            component.ContextSetups["templates"] = control =>
            {
                var details = (DetailsView)control;
                details.HeaderTemplate = new ProbeTemplate("header");
                details.FooterTemplate = new ProbeTemplate("footer");
            };
            component.ContextSetups["empty"] = control => ((DetailsView)control).EmptyDataTemplate = new ProbeTemplate("empty");
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["PageIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["DataKeyNames"] = new List<Sample> { new Sample("Code", new[] { "Code" }) };
            component.Interactions.AddRange(new[]
            {
                "AllowPaging=true+PageIndex=1", "AllowPaging=true+PagerSettings.Mode=NextPrevious", "AllowPaging=true+EnablePagingCallbacks=true",
                "AutoGenerateEditButton=true+AutoGenerateInsertButton=true+AutoGenerateDeleteButton=true", "DefaultMode=Edit+DataKeyNames=Code",
                "HeaderText=text+FooterText=markup", "GridLines=None+CellPadding=3", "Caption=text+CaptionAlign=Bottom", "Enabled=false+CssClass=text",
                StyleCombination, "ForeColor=#3366CC+Style[color]=red", "AllowPaging=true+EnableViewState=false", "AllowPaging=true+Visible=false",
            });
            AddContexts(component, "AllowPaging=true");
            component.Contexts["fields"] = new List<string> { "(default)", "AllowPaging=true", "AllowPaging=true+PageIndex=1", "DefaultMode=Edit", "DefaultMode=Insert", "DefaultMode=Edit+DataKeyNames=Code" };
            component.Contexts["templates"] = new List<string> { "(default)", "AllowPaging=true" };
            component.Contexts["empty"] = new List<string> { "(default)", "EmptyDataText=text", "DefaultMode=Insert" };
            return component;
        }

        static ProbeComponent ListView()
        {
            var component = new ProbeComponent("ListView", typeof(ListView), () => new ListView { ID = "lvItems", ItemTemplate = new ProbeTemplate("item") });
            component.Data = ProbeData.Many;
            component.ContextSetups["layout"] = control =>
            {
                var list = (ListView)control;
                list.LayoutTemplate = new ProbePlaceholderTemplate("<ul class=\"layout\">", "itemPlaceholder", "</ul>");
                list.ItemSeparatorTemplate = new ProbeTemplate("separator");
                list.AlternatingItemTemplate = new ProbeTemplate("alternating");
            };
            component.ContextSetups["groups"] = control =>
            {
                var list = (ListView)control;
                list.LayoutTemplate = new ProbePlaceholderTemplate("<table class=\"groups\">", "groupPlaceholder", "</table>");
                list.GroupTemplate = new ProbePlaceholderTemplate("<tr>", "itemPlaceholder", "</tr>");
                list.ItemTemplate = new ProbeTemplate("cell");
                list.EmptyItemTemplate = new ProbeTemplate("empty item");
                list.GroupSeparatorTemplate = new ProbeTemplate("group separator");
                list.GroupItemCount = 3;
            };
            component.ContextSetups["editing"] = control =>
            {
                var list = (ListView)control;
                list.EditItemTemplate = new ProbeTemplate("edit");
                list.InsertItemTemplate = new ProbeTemplate("insert");
                list.SelectedItemTemplate = new ProbeTemplate("selected");
                list.InsertItemPosition = InsertItemPosition.LastItem;
            };
            component.ContextSetups["empty"] = control => ((ListView)control).EmptyDataTemplate = new ProbeTemplate("empty");
            component.ExtraValues["EditIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["GroupItemCount"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["SelectedIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["DataKeyNames"] = new List<Sample> { new Sample("Code", new[] { "Code" }) };
            component.Interactions.AddRange(new[] { "EnableViewState=false+SelectedIndex=1", "Visible=false+SelectedIndex=1" });
            AddContexts(component);
            component.Contexts["layout"] = new List<string> { "(default)", "ItemPlaceholderID=text" };
            component.Contexts["groups"] = new List<string> { "(default)", "GroupItemCount=1", "GroupPlaceholderID=text" };
            component.Contexts["editing"] = new List<string>
            {
                "(default)", "EditIndex=1", "SelectedIndex=1", "InsertItemPosition=FirstItem", "InsertItemPosition=None", "EditIndex=1+DataKeyNames=Code", "SelectedIndex=1+DataKeyNames=Code",
            };
            component.Contexts["empty"] = new List<string> { "(default)", "InsertItemPosition=FirstItem" };
            return component;
        }

        /// <summary>A pager of a list view beside it (bound by an ObjectDataSource), numeric; other kinds of fields in contexts.</summary>
        static ProbeComponent DataPager()
        {
            var component = new ProbeComponent("DataPager", typeof(DataPager), () =>
            {
                var pager = new DataPager { ID = "dpPager", PagedControlID = "lvPaged", PageSize = 3 };
                pager.Fields.Add(new NumericPagerField());
                return pager;
            });
            component.Siblings = () => new Control[]
            {
                new ObjectDataSource { ID = "odsMany", TypeName = "WebControlsProbe.ProductStore", SelectMethod = "SelectMany" },
                new ListView { ID = "lvPaged", DataSourceID = "odsMany", ItemTemplate = new ProbeTemplate("item") },
            };
            component.ContextSetups["next-previous"] = control =>
            {
                var pager = (DataPager)control;
                pager.Fields.Clear();
                pager.Fields.Add(new NextPreviousPagerField { ShowFirstPageButton = true, ShowLastPageButton = true });
            };
            component.ContextSetups["template"] = control =>
            {
                var pager = (DataPager)control;
                pager.Fields.Clear();
                pager.Fields.Add(new TemplatePagerField { PagerTemplate = new ProbeTemplate("pager") });
            };
            component.ExtraValues["PagedControlID"] = new List<Sample> { new Sample("list", "lvPaged") };
            component.Interactions.AddRange(new[] { "QueryStringField=text+PageSize=3", "PageSize=3+Visible=false", "PageSize=3+EnableViewState=false" });
            AddContexts(component);
            component.Contexts["next-previous"] = new List<string> { "(default)", "PageSize=3" };
            component.Contexts["template"] = new List<string> { "(default)" };
            return component;
        }

        static ProbeComponent DataList()
        {
            var component = new ProbeComponent("DataList", typeof(DataList), () => new DataList { ID = "dlItems", ItemTemplate = new ProbeTemplate("item") });
            component.Data = ProbeData.Many;
            component.ContextSetups["templates"] = control =>
            {
                var list = (DataList)control;
                list.HeaderTemplate = new ProbeTemplate("header");
                list.FooterTemplate = new ProbeTemplate("footer");
                list.SeparatorTemplate = new ProbeTemplate("separator");
                list.AlternatingItemTemplate = new ProbeTemplate("alternating");
                list.SelectedItemTemplate = new ProbeTemplate("selected");
                list.EditItemTemplate = new ProbeTemplate("edit");
            };
            component.ContextSetups["empty"] = component.ContextSetups["templates"];
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["SelectedIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["EditItemIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["DataKeyField"] = new List<Sample> { new Sample("Code", "Code") };
            component.Interactions.AddRange(new[]
            {
                "RepeatColumns=3+RepeatDirection=Horizontal", "RepeatColumns=3+RepeatDirection=Vertical", "RepeatLayout=Flow+RepeatColumns=3",
                "RepeatLayout=UnorderedList+RepeatColumns=3", "RepeatLayout=Flow+GridLines=Both", "Caption=text+CaptionAlign=Bottom", "GridLines=Both+CellPadding=3",
                "SelectedIndex=1+SelectedItemStyle.BackColor=Red", "Enabled=false+CssClass=text", StyleCombination, "ForeColor=#3366CC+Style[color]=red",
                "RepeatColumns=3+EnableViewState=false", "RepeatColumns=3+Visible=false",
            });
            AddContexts(component, "RepeatColumns=3");
            component.Contexts["templates"] = new List<string>
            {
                "(default)", "SelectedIndex=1", "EditItemIndex=1", "ShowHeader=false", "ShowFooter=false", "ExtractTemplateRows=true", "UseAccessibleHeader=true",
                "RepeatColumns=3", "RepeatLayout=Flow", "RepeatLayout=UnorderedList", "DataKeyField=Code+SelectedIndex=1",
            };
            component.Contexts["empty"] = new List<string> { "(default)", "ShowHeader=false" };
            return component;
        }

        static ProbeComponent DataGrid()
        {
            var component = new ProbeComponent("DataGrid", typeof(DataGrid), () => new DataGrid { ID = "dgItems" });
            component.Data = ProbeData.Many;
            component.ContextSetups["columns"] = control =>
            {
                var grid = (DataGrid)control;
                grid.AutoGenerateColumns = false;
                grid.Columns.Add(new BoundColumn { DataField = "Name", HeaderText = "Name", SortExpression = "Name" });
                grid.Columns.Add(new BoundColumn { DataField = "Price", HeaderText = "Price", DataFormatString = "{0:N2}", SortExpression = "Price" });
                grid.Columns.Add(new HyperLinkColumn { Text = "Open", DataNavigateUrlField = "Code", DataNavigateUrlFormatString = "~/ButtonTarget.aspx?code={0}" });
                grid.Columns.Add(new ButtonColumn { CommandName = "pick", Text = "Pick" });
                grid.Columns.Add(new EditCommandColumn { EditText = "Edit", UpdateText = "Update", CancelText = "Cancel" });
                grid.Columns.Add(new TemplateColumn { HeaderText = "Template", ItemTemplate = new ProbeTemplate("item"), EditItemTemplate = new ProbeTemplate("edit"), FooterTemplate = new ProbeTemplate("footer") });
            };
            component.ContextSetups["empty"] = component.ContextSetups["columns"];
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["CurrentPageIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["EditItemIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["SelectedIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["DataKeyField"] = new List<Sample> { new Sample("Code", "Code") };
            component.Interactions.AddRange(new[]
            {
                "AllowPaging=true+PageSize=3", "AllowPaging=true+PageSize=3+CurrentPageIndex=1", "AllowPaging=true+PageSize=3+PagerStyle.Mode=NumericPages",
                "AllowPaging=true+PageSize=3+PagerStyle.Position=TopAndBottom", "AllowCustomPaging=true+AllowPaging=true+PageSize=3+VirtualItemCount=3",
                "AllowSorting=true+AllowPaging=true", "EditItemIndex=1+EditItemStyle.BackColor=Red", "SelectedIndex=1+SelectedItemStyle.BackColor=Red",
                "ShowFooter=true+FooterStyle.BackColor=Red", "Caption=text+CaptionAlign=Bottom", "GridLines=None+CellPadding=3", "Enabled=false+CssClass=text",
                StyleCombination, "ForeColor=#3366CC+Style[color]=red", "AllowPaging=true+PageSize=3+EnableViewState=false", "AllowPaging=true+Visible=false",
            });
            AddContexts(component, "AllowPaging=true+PageSize=3");
            component.Contexts["columns"] = new List<string>
            {
                "(default)", "AllowSorting=true", "EditItemIndex=1", "SelectedIndex=1", "AllowPaging=true+PageSize=3", "ShowFooter=true", "DataKeyField=Code+EditItemIndex=1",
            };
            component.Contexts["empty"] = new List<string> { "(default)", "ShowHeader=false" };
            return component;
        }

        /// <summary>A list control with three items (Small/S, Medium/M, Large/L), none selected.</summary>
        static ProbeComponent List(string name, Func<ListControl> make)
        {
            var type = make().GetType();
            var component = new ProbeComponent(name, type, () =>
            {
                var list = make();
                list.Items.Add(new ListItem("Small", "S"));
                list.Items.Add(new ListItem("Medium", "M"));
                list.Items.Add(new ListItem("Large", "L"));
                return list;
            });
            component.ItemPath = "Items[1]";
            component.ExtraValues["AccessKey"] = new List<Sample> { new Sample("k", "k") };
            component.ExtraValues["SelectedIndex"] = new List<Sample> { new Sample("1", 1) };
            component.ExtraValues["SelectedValue"] = new List<Sample> { new Sample("M", "M"), new Sample("B2", "B2") };
            component.ExtraValues["DataTextField"] = new List<Sample> { new Sample("Name", "Name"), new Sample("Price", "Price") };
            component.ExtraValues["DataValueField"] = new List<Sample> { new Sample("Code", "Code") };
            component.ExtraValues["DataTextFormatString"] = new List<Sample> { new Sample("N2", "{0:N2}") };
            component.ExtraValues["Items[1].Text"] = new List<Sample> { new Sample("spaces", "  a  b  ") };
            var interactions = new List<string>
            {
                "SelectedIndex=1+Items[1].Enabled=false",
                "Items[1].Selected=true+Enabled=false",
                "SelectedValue=M+SelectedIndex=0",
                "Items[1].Text=markup+Items[1].Value=markup",
                "Items[1].Text=empty+Items[1].Value=text",
                "Enabled=false+CssClass=text",
                "Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true",
                "ForeColor=#3366CC+Style[color]=red",
                "SelectedIndex=1+EnableViewState=false",
                "SelectedIndex=1+Visible=false",
            };
            var naming = new List<string> { "(default)", "ClientIDMode=AutoID", "ClientIDMode=Static", "ClientIDMode=Predictable", "ClientIDMode=Inherit", "ID=text" };
            var bound = new List<string>
            {
                "(default)", "DataTextField=Name", "DataValueField=Code", "DataTextField=Name+DataValueField=Code",
                "DataTextField=Price+DataTextFormatString=N2", "DataTextField=Name+AppendDataBoundItems=true",
                "DataTextField=text", "DataTextField=Name+DataValueField=Code+SelectedValue=B2",
                "DataTextField=Name+DataValueField=Code+SelectedValue=M", "DataTextField=Name+DataValueField=Code+SelectedIndex=1",
            };
            if (type == typeof(BulletedList))
            {
                component.ExtraValues["Target"] = new List<Sample> { new Sample("blank", "_blank") };
                interactions.AddRange(new[]
                {
                    "DisplayMode=LinkButton+Items[1].Enabled=false",
                    "DisplayMode=HyperLink+Items[1].Enabled=false",
                    "DisplayMode=HyperLink+Target=blank",
                    "DisplayMode=LinkButton+CausesValidation=false",
                    "DisplayMode=LinkButton+ValidationGroup=text",
                    "DisplayMode=LinkButton+Enabled=false",
                    "BulletStyle=Numbered+FirstBulletNumber=3",
                    "BulletStyle=CustomImage+BulletImageUrl=text",
                    "DisplayMode=LinkButton+Items[1].Text=markup",
                });
                naming.Add("DisplayMode=LinkButton");
            }
            else
            {
                interactions.AddRange(new[]
                {
                    "AutoPostBack=true+CausesValidation=true",
                    "AutoPostBack=true+CausesValidation=true+ValidationGroup=text",
                    "AutoPostBack=true+Enabled=false",
                    "AutoPostBack=true+Items[1].Enabled=false",
                });
                naming.Add("AutoPostBack=true");
            }
            if (type == typeof(DropDownList))
                interactions.AddRange(new[] { "SelectedIndex=1+Items[2].Selected=true", "Items[0].Selected=true+Items[2].Selected=true" });
            if (type == typeof(ListBox))
                interactions.AddRange(new[]
                {
                    "SelectionMode=Multiple+Items[0].Selected=true+Items[2].Selected=true",
                    "SelectionMode=Single+Items[0].Selected=true+Items[2].Selected=true",
                    "SelectionMode=Multiple+Rows=3",
                    "SelectionMode=Multiple+AutoPostBack=true",
                });
            if (type == typeof(CheckBoxList) || type == typeof(RadioButtonList))
                interactions.AddRange(new[]
                {
                    "Items[0].Selected=true+Items[2].Selected=true",
                    "RepeatColumns=3+RepeatDirection=Horizontal",
                    "RepeatColumns=3+RepeatDirection=Vertical",
                    "RepeatLayout=Flow+RepeatColumns=3",
                    "RepeatLayout=UnorderedList+RepeatColumns=3",
                    "RepeatLayout=OrderedList+RepeatDirection=Horizontal",
                    "RepeatLayout=Flow+TextAlign=Left",
                    "RepeatLayout=Table+CellPadding=3+CellSpacing=3",
                    "Items[1].Enabled=false+Enabled=false",
                });
            component.Interactions.AddRange(interactions);
            component.Contexts["naming container"] = naming;
            component.Contexts["themed"] = new List<string> { "(default)", "SkinID=text", "EnableTheming=false", "CssClass=text" };
            component.Contexts["data-bound"] = bound;
            return component;
        }
    }

    /// <summary>The data the "[data-bound]" cases and the scenario pages bind to.</summary>
    public static class ProbeData
    {
        // Serializable: the data controls' scenarios keep it in the session, which the converted application's session
        // provider (ASP.NET Core's) serializes; InProc on IIS does not (a difference of the conversion's session, not of the controls).
        [Serializable]
        public sealed class Product
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public decimal Price { get; set; }
            public bool Active { get; set; }
            public string Image { get; set; }
        }

        public static List<Product> Products() => new List<Product>
        {
            new Product { Name = "Pen", Code = "P1", Price = 1.25m },
            new Product { Name = "Bag", Code = "B2", Price = 24.99m },
            new Product { Name = "Cup <&>", Code = "C3", Price = 1234.5m },
        };

        /// <summary>Seven products (the data controls': paging by 3 makes three pages), every property set.</summary>
        public static List<Product> Many() => new List<Product>
        {
            new Product { Name = "Pen", Code = "P1", Price = 1.25m, Active = true, Image = "~/Images/probe.png" },
            new Product { Name = "Bag", Code = "B2", Price = 24.99m, Active = false, Image = "~/Images/probe.png" },
            new Product { Name = "Cup <&>", Code = "C3", Price = 1234.5m, Active = true, Image = "~/Images/probe.png" },
            new Product { Name = "Desk", Code = "D4", Price = 150m, Active = true, Image = "~/Images/probe.png" },
            new Product { Name = "Eraser", Code = "E5", Price = 0.5m, Active = false, Image = "~/Images/probe.png" },
            new Product { Name = "File", Code = "F6", Price = 3m, Active = true, Image = "~/Images/probe.png" },
            new Product { Name = "Glue", Code = "G7", Price = 2.75m, Active = false, Image = "~/Images/probe.png" },
        };
    }

    /// <summary>A template made in code of the controls given (a login's layout: the controls it looks for by ID).</summary>
    public sealed class ProbeControlsTemplate : ITemplate
    {
        readonly Func<Control[]> make;

        public ProbeControlsTemplate(Func<Control[]> make)
        {
            this.make = make;
        }

        public void InstantiateIn(Control container)
        {
            foreach (var control in make()) container.Controls.Add(control);
        }
    }

    /// <summary>A template made in code with a placeholder of an ID (a list view's layout and group templates: itemPlaceholder, groupPlaceholder).</summary>
    public sealed class ProbePlaceholderTemplate : ITemplate
    {
        readonly string before;
        readonly string id;
        readonly string after;

        public ProbePlaceholderTemplate(string before, string id, string after)
        {
            this.before = before;
            this.id = id;
            this.after = after;
        }

        public void InstantiateIn(Control container)
        {
            container.Controls.Add(new LiteralControl(before));
            container.Controls.Add(new PlaceHolder { ID = id });
            container.Controls.Add(new LiteralControl(after));
        }
    }

    /// <summary>
    /// A template made in code (as the page parser makes one from markup): a literal written when the container binds,
    /// "kind: Name (Code)" of the data item (kind: item, header...; header, footer and separator have no data item).
    /// </summary>
    public sealed class ProbeTemplate : ITemplate
    {
        readonly string kind;

        public ProbeTemplate(string kind)
        {
            this.kind = kind;
        }

        public void InstantiateIn(Control container)
        {
            var literal = new Literal();
            literal.DataBinding += (sender, e) =>
            {
                var item = DataBinder.GetDataItem(((Control)sender).NamingContainer);
                literal.Text = "[" + kind + (item == null ? "" : ": " + System.Web.HttpUtility.HtmlEncode(DataBinder.Eval(item, "Name")) + " (" + DataBinder.Eval(item, "Code") + ")") + "]";
            };
            container.Controls.Add(literal);
        }
    }

    /// <summary>The type of a style's value (Style[color]), for its samples.</summary>
    public sealed class CssValue
    {
    }

    /// <summary>A case: its name, its context (null: in the form), what it sets.</summary>
    public sealed class RenderCase
    {
        public RenderCase(string name, string context, List<Assignment> assignments)
        {
            Name = name;
            Context = context;
            Assignments = assignments;
        }

        public string Name { get; private set; }
        public string Context { get; private set; }
        public List<Assignment> Assignments { get; private set; }

        /// <summary>What the case does, as C#: "txtName.MaxLength = 3;".</summary>
        public string Description =>
            (Context == null ? "" : "[" + Context + "] ") + (Assignments.Count == 0 ? "(nothing set)" : string.Join(" ", Assignments.Select(a => a.Description)));
    }

    /// <summary>A property (path: "Font.Bold", "Attributes[data-probe]", "Style[color]") set to a sample value.</summary>
    public sealed class Assignment
    {
        public Assignment(string path, Sample sample)
        {
            Path = path;
            Sample = sample;
        }

        public string Path { get; private set; }
        public Sample Sample { get; private set; }

        public string Description => "c." + Path + " = " + Sample.Code + ";";

        public void Apply(Control control)
        {
            if (Path.StartsWith("Attributes[", StringComparison.Ordinal))
            {
                ((IAttributeAccessor)control).SetAttribute(Key(), (string)Sample.Value);
                return;
            }
            if (Path.StartsWith("Style[", StringComparison.Ordinal))
            {
                var style = control is HtmlControl html ? html.Style : ((WebControl)control).Style;
                style[Key()] = (string)Sample.Value;
                return;
            }
            var keyed = Keyed.Match(Path);
            if (keyed.Success)
            {
                var attributes = (System.Web.UI.AttributeCollection)control.GetType().GetProperty(keyed.Groups[1].Value, BindingFlags.Public | BindingFlags.Instance).GetValue(control, null);
                attributes[keyed.Groups[2].Value] = (string)Sample.Value;
                return;
            }
            object target = control;
            var parts = Path.Split('.');
            for (var i = 0; i < parts.Length - 1; i++) target = Step(target, parts[i]);
            var property = target.GetType().GetProperty(parts[parts.Length - 1], BindingFlags.Public | BindingFlags.Instance);
            try { property.SetValue(target, Sample.Value, null); }
            catch (TargetInvocationException e) { throw e.InnerException ?? e; }
        }

        string Key() => Path.Substring(Path.IndexOf('[') + 1).TrimEnd(']');

        /// <summary>An attribute collection's entry by key, as InputAttributes[data-probe] (not an index).</summary>
        public static readonly System.Text.RegularExpressions.Regex Keyed = new System.Text.RegularExpressions.Regex(@"^(\w+Attributes)\[([^\]0-9][^\]]*)\]$");

        static readonly System.Text.RegularExpressions.Regex Indexed = new System.Text.RegularExpressions.Regex(@"^(\w+)\[(\d+)\]$");

        /// <summary>A part of a path: a property ("Font"), or an item of a property's list ("Items[1]").</summary>
        public static object Step(object target, string part)
        {
            var indexed = Indexed.Match(part);
            var name = indexed.Success ? indexed.Groups[1].Value : part;
            var value = target.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance).GetValue(target, null);
            return indexed.Success ? ((System.Collections.IList)value)[int.Parse(indexed.Groups[2].Value, CultureInfo.InvariantCulture)] : value;
        }

        /// <summary>The type a part of a path gives ("Items[1]": the list's item type, its indexer's).</summary>
        public static Type StepType(Type type, string part)
        {
            var indexed = Indexed.Match(part);
            if (!indexed.Success) return type.GetProperty(part, BindingFlags.Public | BindingFlags.Instance).PropertyType;
            var list = type.GetProperty(indexed.Groups[1].Value, BindingFlags.Public | BindingFlags.Instance).PropertyType;
            return list.GetProperty("Item", new[] { typeof(int) }).PropertyType;
        }
    }

    /// <summary>A sample value: its label (in case names), the value, how C# writes it.</summary>
    public sealed class Sample
    {
        public Sample(string label, object value, string code = null)
        {
            Label = label;
            Value = value;
            Code = code ?? Literal(value);
        }

        public string Label { get; private set; }
        public object Value { get; private set; }
        public string Code { get; private set; }

        static string Literal(object value)
        {
            if (value == null) return "null";
            if (value is string s) return "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n") + "\"";
            if (value is bool b) return b ? "true" : "false";
            if (value is Enum) return value.GetType().Name + "." + value;
            if (value is string[] array) return "new[] { " + string.Join(", ", array.Select(Literal)) + " }";
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>The sample values of each type a property can have.</summary>
    public static class SampleValues
    {
        public static readonly Sample[] Strings =
        {
            new Sample("text", "probe"),
            new Sample("markup", "<b>\"&'</b>"),
            new Sample("japanese", "\u65E5\u672C\u8A9E\u30C6\u30AD\u30B9\u30C8"),   // 日本語テキスト, escaped: the source's encoding is the compiler's guess
            new Sample("empty", ""),
        };

        static readonly Dictionary<Type, Sample[]> samples = new Dictionary<Type, Sample[]>
        {
            [typeof(string)] = Strings,
            [typeof(bool)] = new[] { new Sample("true", true), new Sample("false", false) },
            [typeof(int)] = new[] { new Sample("0", 0), new Sample("3", 3), new Sample("-1", -1) },
            [typeof(short)] = new[] { new Sample("0", (short)0), new Sample("3", (short)3), new Sample("-1", (short)-1) },
            [typeof(Unit)] = new[] { new Sample("120px", Unit.Pixel(120), "Unit.Pixel(120)"), new Sample("50%", Unit.Percentage(50), "Unit.Percentage(50)"), new Sample("2em", new Unit(2, UnitType.Em), "new Unit(2, UnitType.Em)") },
            [typeof(Color)] = new[] { new Sample("Red", Color.Red, "Color.Red"), new Sample("#3366CC", Color.FromArgb(0x33, 0x66, 0xCC), "Color.FromArgb(0x33, 0x66, 0xCC)"), new Sample("Transparent", Color.Transparent, "Color.Transparent") },
            [typeof(FontUnit)] = new[] { new Sample("12pt", FontUnit.Point(12), "FontUnit.Point(12)"), new Sample("Large", FontUnit.Large, "FontUnit.Large"), new Sample("150%", new FontUnit(Unit.Percentage(150)), "new FontUnit(Unit.Percentage(150))") },
            [typeof(string[])] = new[] { new Sample("Arial,MS-Gothic", new[] { "Arial", "MS Gothic" }) },
            [typeof(DateTime)] = new[] { new Sample("2020-01-20", new DateTime(2020, 1, 20), "new DateTime(2020, 1, 20)"), new Sample("2020-02-29", new DateTime(2020, 2, 29), "new DateTime(2020, 2, 29)"), new Sample("MinValue", DateTime.MinValue, "DateTime.MinValue") },
        };

        /// <summary>A CSS value (Style[color]): its own samples, not text's.</summary>
        public static readonly Sample[] StyleValues = { new Sample("red", "red"), new Sample("1em", "1em") };

        public static bool Has(Type type) => type.IsEnum || samples.ContainsKey(type);

        /// <summary>A type's samples; an enum's every value.</summary>
        public static IEnumerable<Sample> For(Type type)
        {
            if (type.IsEnum) return Enum.GetNames(type).Select(n => new Sample(n, Enum.Parse(type, n)));
            if (type == typeof(CssValue)) return StyleValues;
            Sample[] values;
            return samples.TryGetValue(type, out values) ? values : Enumerable.Empty<Sample>();
        }
    }
}
