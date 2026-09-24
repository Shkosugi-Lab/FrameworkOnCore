namespace WebForm2Blazor.Components;

// System.Web.Compilation - the WebForms compilation pipeline.
//
// This namespace used to be treated as unportable, which excluded every file importing it
// AND everything downstream: DNN's Framework/Reflection.cs uses one method of BuildManager
// and took nine more files with it. Measured across the corpora, 28 files import this
// namespace; 13 of them derive an ExpressionBuilder and 3 call BuildManager.
//
// The declarations live here instead, following the rule already stated for the other
// library-level Framework namespaces: port with local errors rather than exclude, because
// an exclusion cascade does more damage than the errors do.
//
// NOTHING RUNS. A custom expression builder is invoked by the WebForms page compiler,
// which does not exist here - the converter handles <%$ Prefix:Value %> at conversion
// time instead, through --expression-map. These types let the application's builders
// compile so that the rest of the file ports; the builders themselves are inert.

/// <summary>
/// System.Web.Compilation.BuildManager equivalent.
///
/// GetType is the one member ported code actually calls, and it is type resolution by
/// name - which .NET does perfectly well. The others exist for compilation only.
/// </summary>
public static class BuildManager
{
    /// <summary>
    /// WebForms BuildManager.GetType: resolves a type by name across the application's
    /// assemblies. Searches the loaded assemblies, which is the same set the original
    /// searched (its "top-level assemblies" were the app's own plus its references).
    /// </summary>
    public static Type GetType(string typeName, bool throwOnError) => GetType(typeName, throwOnError, false);

    public static Type GetType(string typeName, bool throwOnError, bool ignoreCase)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return throwOnError ? throw new ArgumentNullException(nameof(typeName)) : null;
        }

        var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var resolved = Type.GetType(typeName, throwOnError: false, ignoreCase);
        if (resolved is not null)
        {
            return resolved;
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                foreach (var candidate in assembly.GetTypes())
                {
                    if (string.Equals(candidate.FullName, typeName, comparison)
                        || string.Equals(candidate.Name, typeName, comparison))
                    {
                        return candidate;
                    }
                }
            }
            catch (System.Reflection.ReflectionTypeLoadException)
            {
                // A partially loadable assembly contributes whatever it can; skip it.
            }
        }

        return throwOnError
            ? throw new TypeLoadException($"型 {typeName} は見つかりませんでした。")
            : null;
    }

    /// <summary>WebForms BuildManager.GetReferencedAssemblies equivalent.</summary>
    public static System.Collections.ICollection GetReferencedAssemblies()
        => AppDomain.CurrentDomain.GetAssemblies();

    /// <summary>
    /// WebForms BuildManager.CreateInstanceFromVirtualPath equivalent. Compiling a .aspx
    /// at runtime has no counterpart here - the converter turned those into components at
    /// conversion time - so this reports rather than returns something wrong.
    /// </summary>
    public static object CreateInstanceFromVirtualPath(string virtualPath, Type requiredBaseType)
        => throw new NotSupportedException(
            $"仮想パス {virtualPath} からの実行時コンパイルは変換後のアプリには存在しません。"
            + "対象の .aspx/.ascx は変換時に Blazor コンポーネントになっています。");

    /// <summary>
    /// WebForms BuildManager.GetCompiledType: the type a virtual path compiles to.
    ///
    /// A .cshtml the application renders itself (BlogEngine's widgets, through
    /// RazorHelpers.ParseRazor) is compiled at BUILD time here - the converter hands the
    /// templates its code names to the Razor SDK - and the SDK records each one's path on
    /// the assembly ([RazorCompiledItem]). This looks the path up there, so the template
    /// resolves by the same "~/Custom/Widgets/Search/widget.cshtml" the original compiled.
    /// A template copied under wwwroot is recorded with that prefix, which is accepted too.
    ///
    /// A path with nothing compiled for it (an .aspx: those became components) still
    /// throws, as before - there is no runtime compiler to fall back to.
    /// </summary>
    public static Type GetCompiledType(string virtualPath)
    {
        var path = (virtualPath ?? string.Empty).Replace('\\', '/');
        path = path.StartsWith("~/", StringComparison.Ordinal) ? path[1..] : path;
        path = path.StartsWith('/') ? path : "/" + path;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic)
            {
                continue;
            }

            foreach (var item in assembly.GetCustomAttributes(typeof(Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute), false)
                         .Cast<Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute>())
            {
                if (string.Equals(item.Identifier, path, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(item.Identifier, "/wwwroot" + path, StringComparison.OrdinalIgnoreCase))
                {
                    return item.Type;
                }
            }
        }

        throw new NotSupportedException(
            $"仮想パス {virtualPath} の実行時コンパイルは変換後のアプリには存在しません"
            + "(ビルド時にコンパイルされたテンプレートにもありません)。");
    }
}

/// <summary>
/// System.Web.Compilation.ExpressionBuilder equivalent - the base an application derives
/// to add its own &lt;%$ Prefix:Value %&gt; syntax.
///
/// Declared, never invoked. Expression builders are resolved AT CONVERSION TIME: the
/// converter reads the builder's own declared output format and rewrites the markup
/// (see --expression-map and corpora/README.md). A builder that reached runtime here
/// would have nothing to plug into.
/// </summary>
public abstract class ExpressionBuilder
{
    /// <summary>WebForms ExpressionBuilder.SupportsEvaluate equivalent.</summary>
    public virtual bool SupportsEvaluate => false;

    /// <summary>
    /// WebForms ExpressionBuilder.EvaluateExpression - used in no-compile pages. Ported
    /// builders override it and it is the one member that could sensibly run, so the base
    /// returns null rather than throwing.
    /// </summary>
    public virtual object EvaluateExpression(
        object target, BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        => null;

    /// <summary>WebForms ExpressionBuilder.ParseExpression equivalent.</summary>
    public virtual object ParseExpression(string expression, Type propertyType, ExpressionBuilderContext context)
        => expression;

    /// <summary>
    /// WebForms ExpressionBuilder.GetCodeExpression - emits the CodeDom the page compiler
    /// would have compiled. There is no page compiler here; the override compiles and is
    /// never called.
    /// </summary>
    public virtual CodeExpression GetCodeExpression(
        BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        => null;
}

/// <summary>
/// System.Web.HttpCompileException equivalent. Raised by the WebForms page compiler, which
/// is not here; ported code catches it around a dynamic compile, and that catch has to
/// name a type. Nothing throws it.
/// </summary>
public class HttpCompileException : Exception
{
    public HttpCompileException()
    {
    }

    public HttpCompileException(string message) : base(message)
    {
    }

    public HttpCompileException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public string SourceCode => string.Empty;
}

/// <summary>
/// System.Web.Compilation.ExpressionPrefixAttribute equivalent - declares the prefix a
/// builder answers to (&lt;%$ Reflect:... %&gt;).
///
/// The converter reads the SOURCE form of this to build --expression-map entries; the
/// attribute is kept so the ported builder still carries its own declaration.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ExpressionPrefixAttribute(string expressionPrefix) : Attribute
{
    public string ExpressionPrefix { get; } = expressionPrefix;
}

/// <summary>System.Web.Compilation.ExpressionEditorAttribute equivalent (design-time only).</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ExpressionEditorAttribute(string typeName) : Attribute
{
    public string EditorTypeName { get; } = typeName;
}

/// <summary>System.Web.Compilation.ExpressionBuilderContext equivalent.</summary>
public class ExpressionBuilderContext
{
    public ExpressionBuilderContext()
    {
    }

    public ExpressionBuilderContext(string virtualPath) => VirtualPath = virtualPath;

    public string VirtualPath { get; }

    public IWebFormsControl TemplateControl => null;
}

/// <summary>
/// System.Web.UI.BoundPropertyEntry equivalent - the property an expression is bound to.
/// </summary>
public class BoundPropertyEntry
{
    public string Name { get; set; }

    public string PropertyName { get; set; }

    public Type Type { get; set; }

    public string Expression { get; set; }

    public string ExpressionPrefix { get; set; }

    public bool Generated { get; set; }

    public object ParsedExpressionData { get; set; }

    public bool UseSetAttribute { get; set; }

    public Type ControlType { get; set; }

    public Type DeclaringType { get; set; }

    public System.Reflection.PropertyInfo PropertyInfo { get; set; }
}

// ---------------------------------------------------------------------------------------
// System.CodeDom, the shape an expression builder returns.
//
// The real System.CodeDom exists as a NuGet package, but nothing here compiles a CodeDom
// graph - the builders are inert - so a package reference would add weight for types that
// are only ever constructed and dropped. These carry the values so that a builder's own
// code (which sometimes reads back what it built) still works.
// ---------------------------------------------------------------------------------------

/// <summary>System.CodeDom.CodeExpression equivalent.</summary>
public class CodeExpression
{
}

/// <summary>System.CodeDom.CodeSnippetExpression equivalent - literal code, emitted as-is.</summary>
public class CodeSnippetExpression : CodeExpression
{
    public CodeSnippetExpression()
    {
    }

    public CodeSnippetExpression(string value) => Value = value;

    public string Value { get; set; } = string.Empty;
}

/// <summary>System.CodeDom.CodePrimitiveExpression equivalent.</summary>
public class CodePrimitiveExpression : CodeExpression
{
    public CodePrimitiveExpression()
    {
    }

    public CodePrimitiveExpression(object value) => Value = value;

    public object Value { get; set; }
}

/// <summary>System.CodeDom.CodeTypeReference equivalent.</summary>
public class CodeTypeReference
{
    public CodeTypeReference()
    {
    }

    public CodeTypeReference(string typeName) => BaseType = typeName;

    public CodeTypeReference(Type type) => BaseType = type?.FullName;

    public string BaseType { get; set; }
}

/// <summary>System.CodeDom.CodeTypeReferenceExpression equivalent.</summary>
public class CodeTypeReferenceExpression : CodeExpression
{
    public CodeTypeReferenceExpression()
    {
    }

    public CodeTypeReferenceExpression(string type) => Type = new CodeTypeReference(type);

    public CodeTypeReferenceExpression(Type type) => Type = new CodeTypeReference(type);

    public CodeTypeReferenceExpression(CodeTypeReference type) => Type = type;

    public CodeTypeReference Type { get; set; }
}

/// <summary>System.CodeDom.CodeTypeOfExpression equivalent.</summary>
public class CodeTypeOfExpression : CodeExpression
{
    public CodeTypeOfExpression()
    {
    }

    public CodeTypeOfExpression(string type) => Type = new CodeTypeReference(type);

    public CodeTypeOfExpression(Type type) => Type = new CodeTypeReference(type);

    public CodeTypeReference Type { get; set; }
}

/// <summary>System.CodeDom.CodeMethodReferenceExpression equivalent.</summary>
public class CodeMethodReferenceExpression : CodeExpression
{
    public CodeMethodReferenceExpression()
    {
    }

    public CodeMethodReferenceExpression(CodeExpression targetObject, string methodName)
    {
        TargetObject = targetObject;
        MethodName = methodName;
    }

    public CodeExpression TargetObject { get; set; }

    public string MethodName { get; set; } = string.Empty;
}

/// <summary>System.CodeDom.CodeMethodInvokeExpression equivalent.</summary>
public class CodeMethodInvokeExpression : CodeExpression
{
    public CodeMethodInvokeExpression()
    {
    }

    public CodeMethodInvokeExpression(CodeMethodReferenceExpression method, params CodeExpression[] parameters)
    {
        Method = method;
        Parameters.AddRange(parameters ?? []);
    }

    public CodeMethodInvokeExpression(
        CodeExpression targetObject, string methodName, params CodeExpression[] parameters)
        : this(new CodeMethodReferenceExpression(targetObject, methodName), parameters)
    {
    }

    public CodeMethodReferenceExpression Method { get; set; }

    public List<CodeExpression> Parameters { get; } = [];
}

/// <summary>System.CodeDom.CodePropertyReferenceExpression equivalent.</summary>
public class CodePropertyReferenceExpression : CodeExpression
{
    public CodePropertyReferenceExpression()
    {
    }

    public CodePropertyReferenceExpression(CodeExpression targetObject, string propertyName)
    {
        TargetObject = targetObject;
        PropertyName = propertyName;
    }

    public CodeExpression TargetObject { get; set; }

    public string PropertyName { get; set; } = string.Empty;
}

/// <summary>System.CodeDom.CodeCastExpression equivalent.</summary>
public class CodeCastExpression : CodeExpression
{
    public CodeCastExpression()
    {
    }

    public CodeCastExpression(Type targetType, CodeExpression expression)
    {
        TargetType = new CodeTypeReference(targetType);
        Expression = expression;
    }

    public CodeCastExpression(string targetType, CodeExpression expression)
    {
        TargetType = new CodeTypeReference(targetType);
        Expression = expression;
    }

    public CodeTypeReference TargetType { get; set; }

    public CodeExpression Expression { get; set; }
}
