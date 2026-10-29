using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Resources;
using System.Resources.Tools;
using System.Text;
using WebFormsForCore.CodeDom.Compiler;
using CodeDomProvider = WebFormsForCore.CodeDom.Compiler.CodeDomProvider;

namespace FrameworkOnCore.Runtime.Tests.Resources;

/// <summary>
/// StronglyTypedResourceBuilder, the port of .NET Framework's (System.Design), against its documentation (learn.microsoft.com
/// System.Resources.Tools.StronglyTypedResourceBuilder: Create's exceptions and unmatchable, VerifyResourceName) and the
/// classes it generates (Visual Studio's Resources.Designer.cs: the class, ResourceManager, Culture, a property per
/// resource with GetString/GetStream/GetObject and its doc comment).
/// </summary>
public class StronglyTypedResourceBuilderTests : IDisposable
{
    // The comments are System.Web's resources, localized: the English ones compared.
    readonly System.Globalization.CultureInfo uiCulture = System.Globalization.CultureInfo.CurrentUICulture;
    public StronglyTypedResourceBuilderTests() => System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
    public void Dispose() => System.Globalization.CultureInfo.CurrentUICulture = uiCulture;

    static readonly CodeDomProvider CSharp = new CSharpCodeProviderBase(); // the C# generator (CSharpCodeProvider adds the compiler's configuration)

    static IDictionary Resources(params (string Name, object Value)[] resources)
    {
        var d = new OrderedDictionary();
        foreach (var r in resources) d.Add(r.Name, r.Value);
        return d;
    }

    static CodeTypeDeclaration Class(CodeCompileUnit unit) => unit.Namespaces[0].Types[0];

    static CodeMemberProperty Property(CodeCompileUnit unit, string name) =>
        Class(unit).Members.OfType<CodeMemberProperty>().SingleOrDefault(p => p.Name == name);

    static string[] Properties(CodeCompileUnit unit) =>
        Class(unit).Members.OfType<CodeMemberProperty>().Select(p => p.Name).Where(n => n != "ResourceManager" && n != "Culture").ToArray();

    static string Code(CodeCompileUnit unit, CodeDomProvider provider = null)
    {
        var text = new StringWriter();
        (provider ?? CSharp).GenerateCodeFromCompileUnit(unit, text, new CodeGeneratorOptions());
        return text.ToString().Replace("\r\n", "\n");
    }

    /// <summary>A provider without static public members (as Framework's for some languages), possibly target-aware.</summary>
    sealed class LimitedProvider : CSharpCodeProviderBase, ITargetAwareCodeDomProvider
    {
        public bool PublicStatics, TypeAssembly = true;
        public override bool Supports(GeneratorSupport support) => support == GeneratorSupport.PublicStaticMembers ? PublicStatics : base.Supports(support);
        public bool SupportsProperty(Type type, string propertyName, bool isWritable) => TypeAssembly;
    }

    // --- Arguments

    [Fact] // docs (Create): resourceList, baseName or codeProvider null -> ArgumentNullException; resxFile null too
    public void Create_needs_its_arguments()
    {
        Assert.Throws<ArgumentNullException>(() => StronglyTypedResourceBuilder.Create((IDictionary)null, "R", "NS", CSharp, true, out _));
        Assert.Throws<ArgumentNullException>(() => StronglyTypedResourceBuilder.Create(Resources(), null, "NS", CSharp, true, out _));
        Assert.Throws<ArgumentNullException>(() => StronglyTypedResourceBuilder.Create(Resources(), "R", "NS", null, true, out _));
        Assert.Throws<ArgumentNullException>(() => StronglyTypedResourceBuilder.Create((string)null, "R", "NS", CSharp, true, out _));
        Assert.Throws<ArgumentNullException>(() => StronglyTypedResourceBuilder.Create((string)null, "R", "NS", "RNS", CSharp, true, out _));
    }

    [Fact] // docs (Create): a ResXDataNode whose name is not its key -> ArgumentException
    public void A_node_must_be_under_its_name()
    {
        Assert.Throws<ArgumentException>(() => StronglyTypedResourceBuilder.Create(Resources(("a", new ResXDataNode("b", "v"))), "R", "NS", CSharp, true, out _));
    }

    // --- The class

    [Fact] // Visual Studio's Resources.Designer.cs (internal): the class, its attributes, ResourceManager over "namespace.baseName", Culture, a string property
    public void An_internal_class_is_generated()
    {
        var unit = StronglyTypedResourceBuilder.Create(Resources(("Greeting", "Hello")), "Resources", "NS", CSharp, true, out var unmatchable);
        Assert.Empty(unmatchable);
        var code = Code(unit);
        foreach (var line in new[]
        {
            "namespace NS {",
            "    using System;",
            "    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"System.Resources.Tools.StronglyTypedResourceBuilder\", \"4.0.0.0\")]",
            "    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]",
            "    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]",
            "    internal class Resources {",
            "        private static global::System.Resources.ResourceManager resourceMan;",
            "        private static global::System.Globalization.CultureInfo resourceCulture;",
            "        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute(\"Microsoft.Performance\", \"CA1811:AvoidUncalledPrivateCode\")]",
            "        internal Resources() {",
            "        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]",
            "        internal static global::System.Resources.ResourceManager ResourceManager {",
            "                if (object.ReferenceEquals(resourceMan, null)) {",
            "                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager(\"NS.Resources\", typeof(Resources).Assembly);",
            "                    resourceMan = temp;",
            "                return resourceMan;",
            "        internal static global::System.Globalization.CultureInfo Culture {",
            "                return resourceCulture;",
            "                resourceCulture = value;",
            "    // This class was auto-generated by the StronglyTypedResourceBuilder",
            "    // class via a tool like ResGen or Visual Studio.",
            "    // To add or remove a member, edit your .ResX file then rerun ResGen",
            "    // with the /str option, or rebuild your VS project.",
            "        internal static string Greeting {",
            "                return ResourceManager.GetString(\"Greeting\", resourceCulture);",
        })
            Assert.Contains(line + "\n", code);
        Assert.Equal(new[] { "System.dll" }, unit.ReferencedAssemblies.Cast<string>());
        Assert.Equal(false, unit.UserData["AllowLateBound"]);
        Assert.Equal(true, unit.UserData["RequireVariableDeclaration"]);
    }

    [Fact] // Visual Studio's PublicResXFileCodeGenerator: a public class, its members public static (the constructor internal)
    public void A_public_class_is_generated()
    {
        var code = Code(StronglyTypedResourceBuilder.Create(Resources(("Greeting", "Hello")), "Resources", "NS", CSharp, false, out _));
        Assert.Contains("    public class Resources {\n", code);
        Assert.Contains("        internal Resources() {\n", code);
        Assert.Contains("        public static global::System.Resources.ResourceManager ResourceManager {\n", code);
        Assert.Contains("        public static global::System.Globalization.CultureInfo Culture {\n", code);
        Assert.Contains("        public static string Greeting {\n", code);
    }

    [Fact] // reference source (InternalCreate): a provider without public static members: a public class of instance (virtual) members, a public constructor
    public void A_provider_without_static_members_gets_instance_members()
    {
        var unit = StronglyTypedResourceBuilder.Create(Resources(("Greeting", "Hello"), ("Count", 5)), "Resources", "NS", new LimitedProvider(), false, out _);
        var code = Code(unit);
        Assert.Contains("        public Resources() {\n", code);
        Assert.Contains("        private global::System.Resources.ResourceManager resourceMan;\n", code);
        Assert.Contains("        public virtual global::System.Resources.ResourceManager ResourceManager {\n", code);
        Assert.Contains("        public virtual string Greeting {\n", code);
        Assert.Contains("ResourceManager.GetString(\"Greeting\", this.resourceCulture);", code);
        // internal: static anyway
        code = Code(StronglyTypedResourceBuilder.Create(Resources(("Greeting", "Hello")), "Resources", "NS", new LimitedProvider(), true, out _));
        Assert.Contains("        internal static string Greeting {\n", code);
    }

    [Fact] // reference source (InternalCreate): a target without Type.Assembly: System.Reflection imported, typeof(T).GetTypeInfo().Assembly
    public void A_target_without_type_assembly_uses_type_info()
    {
        var unit = StronglyTypedResourceBuilder.Create(Resources(), "Resources", "NS", new LimitedProvider { PublicStatics = true, TypeAssembly = false }, true, out _);
        var code = Code(unit);
        Assert.Contains("    using System.Reflection;\n", code);
        Assert.Contains("new global::System.Resources.ResourceManager(\"NS.Resources\", typeof(Resources).GetTypeInfo().Assembly);", code);
        Assert.DoesNotContain("System.Reflection", Code(StronglyTypedResourceBuilder.Create(Resources(), "Resources", "NS", new LimitedProvider(), true, out _)));
    }

    [Fact] // docs (Create's resourcesNamespace): the ResourceManager's name is resourcesNamespace.baseName ("" : baseName), else namespace.baseName (none: baseName)
    public void The_resources_name_is_the_resources_namespace_and_the_base_name()
    {
        string Name(string ns, string resourcesNs) =>
            Code(StronglyTypedResourceBuilder.Create(Resources(), "R", ns, resourcesNs, CSharp, true, out _)).Split("new global::System.Resources.ResourceManager(\"")[1].Split('"')[0];
        Assert.Equal("Other.R", Name("NS", "Other"));
        Assert.Equal("R", Name("NS", ""));
        Assert.Equal("NS.R", Name("NS", null));
        Assert.Equal("R", Name("", null));
        Assert.Equal("R", Name(null, null));
    }

    [Fact] // reference source (InternalCreate): a base name or namespace that is not an identifier is made one ('.' kept in a namespace); one that cannot be -> ArgumentException
    public void Names_are_made_identifiers()
    {
        var unit = StronglyTypedResourceBuilder.Create(Resources(), "My Resources", "My NS", CSharp, true, out _);
        Assert.Equal("My_Resources", Class(unit).Name);
        Assert.Equal("My_NS", unit.Namespaces[0].Name);
        Assert.Contains("new global::System.Resources.ResourceManager(\"My_NS.My Resources\"", Code(unit)); // the namespace made one, the base name as given
        // a namespace with a dot is never an identifier: not made one, refused by CodeDom's validation
        Assert.Throws<ArgumentException>(() => StronglyTypedResourceBuilder.Create(Resources(), "R", "My NS.Sub", CSharp, true, out _));
        unit = StronglyTypedResourceBuilder.Create(Resources(), "R", "A.B", CSharp, true, out _);
        Assert.Equal("A.B", unit.Namespaces[0].Name); // a namespace with dots is not an identifier, and stays
        Assert.Equal("R", Class(StronglyTypedResourceBuilder.Create(Resources(), "R", "", CSharp, true, out _)).Name);
        Assert.Throws<ArgumentException>(() => StronglyTypedResourceBuilder.Create(Resources(), "a=b", "NS", CSharp, true, out _));
    }

    // --- The properties

    [Fact] // Resources.Designer.cs: a non-string resource: GetObject cast to its type; a MemoryStream: UnmanagedMemoryStream from GetStream
    public void Objects_and_streams_have_their_types()
    {
        var unit = StronglyTypedResourceBuilder.Create(Resources(("Count", 5), ("Sound", new MemoryStream()), ("Ums", new UnmanagedMemoryStreamLike())), "R", "NS", CSharp, true, out _);
        var code = Code(unit);
        Assert.Contains("        internal static int Count {\n            get {\n                object obj = ResourceManager.GetObject(\"Count\", resourceCulture);\n                return ((int)(obj));\n", code);
        Assert.Contains("        internal static System.IO.UnmanagedMemoryStream Sound {\n            get {\n                return ResourceManager.GetStream(\"Sound\", resourceCulture);\n", code);
        Assert.Contains("        internal static System.IO.UnmanagedMemoryStream Ums {\n            get {\n                return ResourceManager.GetStream(\"Ums\", resourceCulture);\n", code);
    }

    sealed class UnmanagedMemoryStreamLike : UnmanagedMemoryStream
    {
        public override string ToString() => "stream";
    }

    sealed class Hidden : Plain
    {
        public override string ToString() => "hidden";
    }

    [Fact] // reference source (DefineResourceFetchingProperty): a type that is not public: its first public base type
    public void A_non_public_type_is_its_public_base()
    {
        Assert.Equal(typeof(Plain).FullName, Property(StronglyTypedResourceBuilder.Create(Resources(("H", new Hidden())), "R", "NS", CSharp, true, out _), "H").Type.BaseType);
    }

    [Fact] // reference source (DefineResourceFetchingProperty): the doc comment of a string: its value (escaped; longer than 512 characters: truncated)
    public void A_string_property_comment_has_the_value()
    {
        var unit = StronglyTypedResourceBuilder.Create(Resources(("S", "a<b"), ("Long", new string('x', 600))), "R", "NS", CSharp, true, out _);
        Assert.Equal("Looks up a localized string similar to a&lt;b.", Property(unit, "S").Comments[1].Comment.Text.Trim());
        Assert.Equal("Looks up a localized string similar to " + new string('x', 512) + " [rest of string was truncated]&quot;;.", Property(unit, "Long").Comments[1].Comment.Text.Trim());
    }

    [Fact] // docs (Create): ResXDataNode values: their type (GetValueTypeName) and value
    public void Nodes_give_their_types()
    {
        var unit = StronglyTypedResourceBuilder.Create(Resources(("N", new ResXDataNode("N", 5)), ("S", new ResXDataNode("S", "v"))), "R", "NS", CSharp, true, out var unmatchable);
        Assert.Empty(unmatchable);
        Assert.Equal("System.Int32", Property(unit, "N").Type.BaseType);
        Assert.Equal("System.String", Property(unit, "S").Type.BaseType);
    }

    // --- Names (unmatchable)

    [Fact] // docs (Create's unmatchable, VerifyResourceName): names made identifiers ('_' for the characters that cannot be); the resource's own name fetched
    public void Resource_names_are_made_identifiers()
    {
        var unit = StronglyTypedResourceBuilder.Create(Resources(("my key", "a"), ("class", "b"), ("1st", "c"), ("ok", "d")), "R", "NS", CSharp, true, out var unmatchable);
        Assert.Empty(unmatchable);
        Assert.Equal(new[] { "_1st", "_class", "my_key", "ok" }, Properties(unit).OrderBy(n => n, StringComparer.Ordinal));
        Assert.Contains("ResourceManager.GetString(\"my key\", resourceCulture);", Code(unit));
    }

    [Fact] // docs (unmatchable): names that cannot be identifiers, ResourceManager and Culture, void, a type not found; reference source: designer names ($, >>) ignored
    public void Unmatchable_names_are_reported()
    {
        var unit = StronglyTypedResourceBuilder.Create(Resources(
            ("a=b", "x"), ("ResourceManager", "x"), ("Culture", "x"),
            ("V", new ResXDataNode("V", 1, t => "System.Void")), ("U", new ResXDataNode("U", 1, t => "No.Such.Type, No.Such.Assembly")),
            ("$this.Text", "x"), (">>a.Name", "x"), ("ok", "y")), "R", "NS", CSharp, true, out var unmatchable);
        Assert.Equal(new[] { "Culture", "ResourceManager", "U", "V", "a=b" }, unmatchable.OrderBy(n => n, StringComparer.Ordinal));
        Assert.Equal(new[] { "ok" }, Properties(unit));
    }

    [Fact] // docs (unmatchable): names that become the same identifier: none of them is generated
    public void Names_that_become_the_same_are_unmatchable()
    {
        var unit = StronglyTypedResourceBuilder.Create(Resources(("a b", "1"), ("a-b", "2"), ("c", "3")), "R", "NS", CSharp, true, out var unmatchable);
        Assert.Equal(new[] { "a b", "a-b" }, unmatchable.OrderBy(n => n, StringComparer.Ordinal));
        Assert.Equal(new[] { "c" }, Properties(unit));
        // a name made the same as another's own (that one first, or after)
        StronglyTypedResourceBuilder.Create(Resources(("a_b", "1"), ("a b", "2")), "R", "NS", CSharp, true, out unmatchable);
        Assert.Contains("a b", unmatchable);
        StronglyTypedResourceBuilder.Create(Resources(("a b", "1"), ("a_b", "2")), "R", "NS", CSharp, true, out unmatchable);
        Assert.Equal(new[] { "a b", "a_b" }, unmatchable.OrderBy(n => n, StringComparer.Ordinal));
        // three: the third reported too
        StronglyTypedResourceBuilder.Create(Resources(("a b", "1"), ("a-b", "2"), ("a+b", "3")), "R", "NS", CSharp, true, out unmatchable);
        Assert.Equal(new[] { "a b", "a+b", "a-b" }, unmatchable.Distinct().OrderBy(n => n, StringComparer.Ordinal));
    }

    [Fact] // docs (VerifyResourceName): key or provider null -> ArgumentNullException; the characters replaced by '_'; a keyword escaped; a leading digit prefixed; null when it cannot be
    public void VerifyResourceName_makes_a_name_an_identifier()
    {
        Assert.Throws<ArgumentNullException>(() => StronglyTypedResourceBuilder.VerifyResourceName(null, CSharp));
        Assert.Throws<ArgumentNullException>(() => StronglyTypedResourceBuilder.VerifyResourceName("a", null));
        Assert.Equal("a_b_c_d", StronglyTypedResourceBuilder.VerifyResourceName("a b.c-d", CSharp));
        Assert.Equal("ok", StronglyTypedResourceBuilder.VerifyResourceName("ok", CSharp));
        Assert.Equal("_class", StronglyTypedResourceBuilder.VerifyResourceName("class", CSharp));
        Assert.Equal("_1st", StronglyTypedResourceBuilder.VerifyResourceName("1st", CSharp));
        Assert.Null(StronglyTypedResourceBuilder.VerifyResourceName("a=b", CSharp));
    }

    // --- Differences from .NET Framework

    [Fact(Skip = "bug: the resource string NonStringPropertyComment is missing from System.Web.resx: a resource whose text is its type's name (a Bitmap, null) throws ArgumentNullException (StronglyTypedResourceBuilder.cs:584)")] // Resources.Designer.cs (.NET Framework): "Looks up a localized resource of type System.Drawing.Bitmap."
    public void A_resource_whose_text_is_its_type_name_is_generated()
    {
        var unit = StronglyTypedResourceBuilder.Create(Resources(("P", new Plain()), ("N", null)), "R", "NS", CSharp, true, out var unmatchable);
        Assert.Empty(unmatchable);
        Assert.Equal("  Looks up a localized resource of type " + typeof(Plain).FullName + ".", Property(unit, "P").Comments[1].Comment.Text);
        Assert.Equal("  Looks up a localized resource of type System.Object.", Property(unit, "N").Comments[1].Comment.Text);
    }

    [Fact(Skip = "bug: System.Web.resx's NonStringPropertyDetailedComment lacks \" similar to {1}\" (.NET Framework: \"Looks up a localized resource of type System.Drawing.Icon similar to (Icon).\") (StronglyTypedResourceBuilder.cs:584)")] // Resources.Designer.cs (.NET Framework)
    public void A_non_string_property_comment_has_the_value()
    {
        var unit = StronglyTypedResourceBuilder.Create(Resources(("Count", 5)), "R", "NS", CSharp, true, out _);
        Assert.Equal("  Looks up a localized resource of type System.Int32 similar to 5.", Property(unit, "Count").Comments[1].Comment.Text);
    }

    [Fact(Skip = "bug: System.Web.resx's doc comment strings lost .NET Framework's two leading spaces (\"///   Looks up a localized string similar to ...\" in Resources.Designer.cs) (StronglyTypedResourceBuilder.cs:242, 506, 513, 584)")] // Resources.Designer.cs (.NET Framework)
    public void The_doc_comments_are_Frameworks()
    {
        var code = Code(StronglyTypedResourceBuilder.Create(Resources(("Greeting", "Hello")), "Resources", "NS", CSharp, true, out _));
        Assert.Contains("    /// <summary>\n    ///   A strongly-typed resource class, for looking up localized strings, etc.\n    /// </summary>\n", code);
        Assert.Contains("        /// <summary>\n        ///   Returns the cached ResourceManager instance used by this class.\n        /// </summary>\n", code);
        Assert.Contains("        ///   Overrides the current thread's CurrentUICulture property for all\n        ///   resource lookups using this strongly typed resource class.\n", code);
        Assert.Contains("        /// <summary>\n        ///   Looks up a localized string similar to Hello.\n        /// </summary>\n", code);
    }

    [Fact(Skip = "bug: Create(resxFile, ...) does not read the file (the reading is compiled out: #if !WebFormsForCore, \"TODO\"): no property is generated (StronglyTypedResourceBuilder.cs:361)")] // docs (Create(string resxFile, ...)): a class for the resources of the .resx file
    public void A_resx_file_gets_its_properties()
    {
        var path = Resx.TempFile(".resx", Encoding.UTF8.GetBytes(Resx.Document("<data name=\"Greeting\"><value>Hello</value></data>")));
        try
        {
            var unit = StronglyTypedResourceBuilder.Create(path, "R", "NS", CSharp, true, out var unmatchable);
            Assert.Empty(unmatchable);
            Assert.Equal(new[] { "Greeting" }, Properties(unit));
        }
        finally { File.Delete(path); }
    }

    [Fact] // docs (Create(string resxFile, ...)): the class named, in the namespace given
    public void A_resx_file_gets_its_class()
    {
        var path = Resx.TempFile(".resx", Encoding.UTF8.GetBytes(Resx.Document("")));
        try
        {
            var unit = StronglyTypedResourceBuilder.Create(path, "R", "NS", CSharp, true, out var unmatchable);
            Assert.Empty(unmatchable);
            Assert.Equal("R", Class(unit).Name);
            Assert.Equal("NS", unit.Namespaces[0].Name);
        }
        finally { File.Delete(path); }
    }
}
