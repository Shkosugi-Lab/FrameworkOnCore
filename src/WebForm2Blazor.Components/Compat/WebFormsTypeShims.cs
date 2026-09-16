namespace WebForm2Blazor.Components;

// Declaration surface for the System.Web types the corpora name but the compatibility
// layer did not have. Every one of them was a CS0246 in ported code.
//
// These are DECLARATIONS, not implementations: enums carry their real values (code
// branches on them), collections hold what is added to them, and the rest exists so the
// file compiles. Where a type drives behaviour it lives elsewhere - a control that renders
// is a .razor component, not an entry here.

// ---------------------------------------------------------------------------------------
// Enumerations. The values are the originals: ported code compares against them and, for
// several, persists the number.
// ---------------------------------------------------------------------------------------

/// <summary>System.Web.UI.WebControls.PagerPosition equivalent.</summary>
public enum PagerPosition
{
    Bottom,
    Top,
    TopAndBottom,
}

/// <summary>System.Web.UI.WebControls.ButtonType equivalent.</summary>
public enum ButtonType
{
    Button,
    Image,
    Link,
}

/// <summary>System.Web.UI.WebControls.TextAlign equivalent.</summary>
public enum TextAlign
{
    Left = 1,
    Right = 2,
}

/// <summary>System.Web.UI.WebControls.TableCaptionAlign equivalent.</summary>
public enum TableCaptionAlign
{
    NotSet,
    Top,
    Bottom,
    Left,
    Right,
}

/// <summary>System.Web.UI.WebControls.GridLines equivalent.</summary>
public enum GridLines
{
    None,
    Horizontal,
    Vertical,
    Both,
}

/// <summary>System.Web.UI.WebControls.FirstDayOfWeek equivalent (Default is 7, as there).</summary>
public enum FirstDayOfWeek
{
    Sunday = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Default = 7,
}

/// <summary>System.Web.UI.WebControls.DayNameFormat equivalent.</summary>
public enum DayNameFormat
{
    Full,
    Short,
    FirstLetter,
    FirstTwoLetters,
    Shortest,
}

/// <summary>System.Web.UI.WebControls.ValidationDataType equivalent.</summary>
public enum ValidationDataType
{
    String,
    Integer,
    Double,
    Date,
    Currency,
}

/// <summary>System.Web.HttpValidationStatus equivalent (output-cache validation callback).</summary>
public enum HttpValidationStatus
{
    Invalid = 1,
    IgnoreThisRequest = 2,
    Valid = 3,
}

// ---------------------------------------------------------------------------------------
// Delegates
// ---------------------------------------------------------------------------------------

/// <summary>System.Web.UI.WebControls.MonthChangedEventHandler equivalent.</summary>
public delegate void MonthChangedEventHandler(object sender, MonthChangedEventArgs e);

/// <summary>System.Web.UI.WebControls.MonthChangedEventArgs equivalent.</summary>
public sealed class MonthChangedEventArgs(DateTime newDate, DateTime previousDate) : EventArgs
{
    public DateTime NewDate { get; } = newDate;

    public DateTime PreviousDate { get; } = previousDate;
}

/// <summary>System.Web.UI.DataSourceViewSelectCallback equivalent.</summary>
public delegate void DataSourceViewSelectCallback(System.Collections.IEnumerable data);

/// <summary>System.Web.UI.DataSourceViewOperationCallback equivalent.</summary>
public delegate bool DataSourceViewOperationCallback(int affectedRecords, Exception ex);

// ---------------------------------------------------------------------------------------
// Attributes. Declared with the real AttributeTargets so that applying one where the
// original did still compiles; nothing reads them (design-time metadata).
// ---------------------------------------------------------------------------------------

/// <summary>System.Web.UI.WebControls.WebParts.PersonalizationScope equivalent.</summary>
public enum PersonalizationScope
{
    User,
    Shared,
}

/// <summary>System.Web.UI.WebControls.WebParts.PersonalizableAttribute equivalent.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class PersonalizableAttribute : Attribute
{
    public PersonalizableAttribute()
    {
    }

    public PersonalizableAttribute(bool isPersonalizable) => IsPersonalizable = isPersonalizable;

    public PersonalizableAttribute(object scope) => _ = scope;

    public bool IsPersonalizable { get; }
}

/// <summary>System.ComponentModel.WebBrowsableAttribute equivalent.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class WebBrowsableAttribute : Attribute
{
    public WebBrowsableAttribute()
    {
    }

    public WebBrowsableAttribute(bool browsable) => Browsable = browsable;

    public bool Browsable { get; } = true;
}

/// <summary>System.ComponentModel.WebDisplayNameAttribute equivalent.</summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class WebDisplayNameAttribute(string displayName) : Attribute
{
    public string DisplayName { get; } = displayName;
}

/// <summary>System.ComponentModel.WebDescriptionAttribute equivalent.</summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class WebDescriptionAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}

/// <summary>System.Web.UI.UrlPropertyAttribute equivalent.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class UrlPropertyAttribute : Attribute
{
    public UrlPropertyAttribute()
    {
    }

    public UrlPropertyAttribute(string filter) => Filter = filter;

    public string Filter { get; } = "*.*";
}

/// <summary>System.Web.UI.IDReferencePropertyAttribute equivalent.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class IDReferencePropertyAttribute : Attribute
{
    public IDReferencePropertyAttribute()
    {
    }

    public IDReferencePropertyAttribute(Type referencedControlType)
        => ReferencedControlType = referencedControlType;

    public Type ReferencedControlType { get; }
}

/// <summary>
/// System.Web.PreApplicationStartMethodAttribute equivalent. ASP.NET Core starts from
/// Program.cs, so nothing invokes the named method; the attribute is kept so the assembly
/// declaring it compiles.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class PreApplicationStartMethodAttribute(Type type, string methodName) : Attribute
{
    public Type Type { get; } = type;

    public string MethodName { get; } = methodName;
}

// ---------------------------------------------------------------------------------------
// Interfaces
// ---------------------------------------------------------------------------------------

/// <summary>System.Web.UI.WebControls.IButtonControl equivalent.</summary>
public interface IButtonControl
{
    bool CausesValidation { get; set; }

    string CommandArgument { get; set; }

    string CommandName { get; set; }

    string PostBackUrl { get; set; }

    string Text { get; set; }

    string ValidationGroup { get; set; }
}

/// <summary>System.Web.UI.WebControls.IEditableTextControl equivalent.</summary>
public interface IEditableTextControl
{
    string Text { get; set; }

    event EventHandler TextChanged;
}

/// <summary>System.Web.UI.INavigateUIData equivalent.</summary>
public interface INavigateUIData
{
    string Name { get; }

    string NavigateUrl { get; }

    string Value { get; }

    string Description { get; }
}

// ---------------------------------------------------------------------------------------
// Controls and control-shaped types
// ---------------------------------------------------------------------------------------

/// <summary>
/// System.Web.UI.WebControls.BaseCompareValidator equivalent - the base of the comparison
/// validators, which ported code derives from to add a comparison of its own.
/// </summary>
public abstract class BaseCompareValidator : ValidatorBase
{
    public ValidationDataType Type { get; set; } = ValidationDataType.String;

    public bool CultureInvariantValues { get; set; }

    /// <summary>WebForms BaseCompareValidator.CanConvert equivalent.</summary>
    public static bool CanConvert(string text, ValidationDataType type) => type switch
    {
        ValidationDataType.Integer => int.TryParse(text, out _),
        ValidationDataType.Double => double.TryParse(text, out _),
        ValidationDataType.Currency => decimal.TryParse(text, out _),
        ValidationDataType.Date => DateTime.TryParse(text, out _),
        _ => true,
    };
}

/// <summary>
/// System.Web.UI.WebControls.DataBoundControl equivalent. The two-step bind (PerformSelect
/// fetches, PerformDataBinding consumes) is what a ported control overrides.
/// </summary>
public class DataBoundControl : LegacyWebControl
{
    public virtual object DataSource { get; set; }

    public virtual string DataSourceID { get; set; }

    public virtual string DataMember { get; set; }

    protected virtual void PerformSelect()
    {
    }

    protected virtual void PerformDataBinding(System.Collections.IEnumerable data)
    {
    }

    protected virtual DataSourceView GetData() => null;
}

/// <summary>
/// System.Web.UI.WebControls.DataSourceControl equivalent.
/// </summary>
/// <remarks>
/// GetView / GetViewNames are PROTECTED, as in System.Web. Declaring them public here made
/// every ported data source (n2's ItemDataSource, mojoPortal's RssDataSource) fail with
/// CS0507 - an override cannot widen accessibility - which is a worse error than the
/// missing type it replaced.
/// </remarks>
public class DataSourceControl : LegacyWebControl
{
    protected virtual DataSourceView GetView(string viewName) => null;

    protected virtual System.Collections.ICollection GetViewNames() => Array.Empty<string>();
}

/// <summary>System.Web.UI.WebControls.HierarchicalDataSourceControl equivalent.</summary>
public class HierarchicalDataSourceControl : LegacyWebControl
{
}

/// <summary>System.Web.UI.HtmlControls.HtmlTextArea equivalent (declaration surface).</summary>
public class HtmlTextArea : LegacyWebControl
{
    /// <summary>
    /// WebForms HtmlTextArea.InnerText / InnerHtml. A textarea's content IS its value -
    /// the element has no separate children - so all three names read and write the same
    /// string, which is what HtmlTextArea did on 4.8. Code-behind uses whichever it
    /// happens to prefer: 186 of YAF's build errors were InnerText alone.
    /// </summary>
    public string InnerText
    {
        get => Value;
        set => Value = value;
    }

    public string InnerHtml
    {
        get => Value;
        set => Value = value;
    }

    public string Value { get; set; }

    public int Rows { get; set; }

    public int Cols { get; set; }

    public string Name { get; set; }

    public event EventHandler ServerChange;

    protected virtual void OnServerChange(EventArgs e) => ServerChange?.Invoke(this, e);
}

/// <summary>System.Web.UI.HtmlControls.HtmlInputImage equivalent (declaration surface).</summary>
public class HtmlInputImage : LegacyWebControl
{
    public string Src { get; set; }

    public string Alt { get; set; }

    public string Align { get; set; }

    public int Border { get; set; }

    public bool CausesValidation { get; set; } = true;

    public string ValidationGroup { get; set; }

    public event EventHandler ServerClick;

    protected virtual void OnServerClick(EventArgs e) => ServerClick?.Invoke(this, e);
}

// ---------------------------------------------------------------------------------------
// Collections and data plumbing
// ---------------------------------------------------------------------------------------

/// <summary>System.Web.UI.WebControls.GridViewRowCollection equivalent.</summary>
public sealed class GridViewRowCollection : List<GridViewRow>
{
}

/// <summary>System.Web.UI.WebControls.DataGridItemCollection equivalent.</summary>
public sealed class DataGridItemCollection : List<object>
{
}

/// <summary>System.Web.UI.WebControls.DataGridColumnCollection equivalent.</summary>
public sealed class DataGridColumnCollection : List<object>
{
}

/// <summary>System.Web.UI.DataBindingCollection equivalent (design-time data bindings).</summary>
public sealed class DataBindingCollection : List<object>
{
}

/// <summary>
/// System.Web.UI.WebControls.Parameter equivalent - a declarative data-source parameter.
/// Kept as data: ported code builds these to describe a query, and reads the values back.
/// </summary>
public class Parameter
{
    public Parameter()
    {
    }

    public Parameter(string name) => Name = name;

    public Parameter(string name, object defaultValue)
    {
        Name = name;
        DefaultValue = defaultValue?.ToString();
    }

    protected Parameter(Parameter original)
    {
        Name = original?.Name;
        DefaultValue = original?.DefaultValue;
        Type = original?.Type;
        Direction = original?.Direction;
    }

    public string Name { get; set; }

    public string DefaultValue { get; set; }

    public string Type { get; set; }

    public string ConvertEmptyStringToNull { get; set; }

    public string Direction { get; set; }

    /// <summary>
    /// WebForms Parameter.Clone / Evaluate. An application that declares a parameter of its
    /// own (n2's CurrentItemParameter, which reads the current content item) overrides both.
    /// </summary>
    protected virtual Parameter Clone() => new(this);

    protected virtual object Evaluate(HttpContext context, IWebFormsControl control) => DefaultValue;
}

/// <summary>System.Web.UI.OutputCacheParameters equivalent (declaration surface).</summary>
public sealed class OutputCacheParameters
{
    public int Duration { get; set; }

    public bool Enabled { get; set; } = true;

    public string CacheProfile { get; set; }

    public string VaryByParam { get; set; }

    public string VaryByHeader { get; set; }

    public string VaryByCustom { get; set; }

    public string VaryByControl { get; set; }

    public string SqlDependency { get; set; }

    public bool NoStore { get; set; }
}

/// <summary>System.Web.UI.PostBackOptions equivalent (declaration surface).</summary>
public sealed class PostBackOptions
{
    public PostBackOptions(object targetControl) => _ = targetControl;

    public PostBackOptions(object targetControl, string argument)
    {
        _ = targetControl;
        Argument = argument;
    }

    public string Argument { get; set; }

    public string ActionUrl { get; set; }

    public string ValidationGroup { get; set; }

    public bool AutoPostBack { get; set; }

    public bool PerformValidation { get; set; }

    public bool RequiresJavaScriptProtocol { get; set; }

    public bool TrackFocus { get; set; }

    public bool ClientSubmit { get; set; } = true;
}

/// <summary>System.Web.UI.ScriptReference equivalent (ASP.NET AJAX script registration).</summary>
public sealed class ScriptReference
{
    public ScriptReference()
    {
    }

    public ScriptReference(string path) => Path = path;

    public ScriptReference(string name, string assembly)
    {
        Name = name;
        Assembly = assembly;
    }

    public string Path { get; set; }

    public string Name { get; set; }

    public string Assembly { get; set; }
}

// ---------------------------------------------------------------------------------------
// Converters and style types
// ---------------------------------------------------------------------------------------

/// <summary>System.Web.UI.WebControls.UnitConverter equivalent.</summary>
public class UnitConverter : System.ComponentModel.TypeConverter
{
}

/// <summary>System.Web.UI.WebControls.ValidatedControlConverter equivalent.</summary>
public class ValidatedControlConverter : System.ComponentModel.TypeConverter
{
}

/// <summary>
/// System.Web.UI.WebControls.FontInfo equivalent. WebControl exposes Font-* as separate
/// properties here (the markup form), so this is the object shape ported code assigns to.
/// </summary>
public sealed class FontInfo
{
    public string Name { get; set; }

    public string[] Names { get; set; } = [];

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public bool Underline { get; set; }

    public bool Overline { get; set; }

    public bool Strikeout { get; set; }

    public string Size { get; set; }

    public void CopyFrom(FontInfo other)
    {
        if (other is null)
        {
            return;
        }
        Name = other.Name;
        Bold = other.Bold;
        Italic = other.Italic;
        Underline = other.Underline;
        Overline = other.Overline;
        Strikeout = other.Strikeout;
        Size = other.Size;
    }
}

// ---------------------------------------------------------------------------------------
// Site map
// ---------------------------------------------------------------------------------------

/// <summary>System.Web.StaticSiteMapProvider equivalent (declaration surface).</summary>
public abstract class StaticSiteMapProvider : SiteMapProvider
{
    protected virtual void AddNode(SiteMapNode node)
    {
    }

    protected virtual void AddNode(SiteMapNode node, SiteMapNode parentNode)
    {
    }

    protected virtual void RemoveNode(SiteMapNode node)
    {
    }

    protected virtual void Clear()
    {
    }
}

/// <summary>System.Web.UI.WebControls.SiteMapNodeItemType equivalent.</summary>
public enum SiteMapNodeItemType
{
    Root,
    Parent,
    Current,
    PathSeparator,
}

/// <summary>
/// System.Web.UI.WebControls.SiteMapNodeItem equivalent - one node in a SiteMapPath, which
/// a ported breadcrumb control creates and templates against (mojoPortal's
/// mojoSiteMapPath).
/// </summary>
public class SiteMapNodeItem : LegacyWebControl
{
    public SiteMapNodeItem()
    {
    }

    public SiteMapNodeItem(int itemIndex, SiteMapNodeItemType itemType)
    {
        ItemIndex = itemIndex;
        ItemType = itemType;
    }

    public int ItemIndex { get; }

    public SiteMapNodeItemType ItemType { get; }

    public SiteMapNode SiteMapNode { get; set; }
}

/// <summary>System.Web.UI.WebControls.SiteMapNodeItemEventArgs equivalent.</summary>
public sealed class SiteMapNodeItemEventArgs(SiteMapNodeItem item) : EventArgs
{
    public SiteMapNodeItem Item { get; } = item;
}

/// <summary>System.Web.UI.WebControls.SiteMapNodeItemEventHandler equivalent.</summary>
public delegate void SiteMapNodeItemEventHandler(object sender, SiteMapNodeItemEventArgs e);

// ---------------------------------------------------------------------------------------
// Membership / profile / mail
// ---------------------------------------------------------------------------------------

// ProfileAuthenticationOption, ProfileInfo and ProfileInfoCollection are in
// Compat/DeclarationShims.cs, next to the rest of the profile store.

/// <summary>
/// System.Web.Profile.ProfileProvider equivalent.
///
/// Every member is declared abstract, as in the original. A provider subclass is exactly
/// what an application ports (n2cms has ContentProfileProvider), and a base missing members
/// turns each override into a CS0115 on the subclass - the same failure that adding this
/// type was meant to remove.
/// </summary>
public abstract class ProfileProvider : System.Configuration.SettingsProvider
{
    public abstract int DeleteInactiveProfiles(
        ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate);

    public abstract int DeleteProfiles(ProfileInfoCollection profiles);

    public abstract int DeleteProfiles(string[] usernames);

    public abstract ProfileInfoCollection FindInactiveProfilesByUserName(
        ProfileAuthenticationOption authenticationOption, string usernameToMatch,
        DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords);

    public abstract ProfileInfoCollection FindProfilesByUserName(
        ProfileAuthenticationOption authenticationOption, string usernameToMatch,
        int pageIndex, int pageSize, out int totalRecords);

    public abstract ProfileInfoCollection GetAllInactiveProfiles(
        ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate,
        int pageIndex, int pageSize, out int totalRecords);

    public abstract ProfileInfoCollection GetAllProfiles(
        ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords);

    public abstract int GetNumberOfInactiveProfiles(
        ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate);
}

/// <summary>
/// System.Web.Security.SqlMembershipProvider equivalent. Applications derive from it to
/// change one or two methods, so every member of the abstract base is implemented here -
/// otherwise the subclass, not this class, is the one that fails to compile.
///
/// The SQL store itself is NOT reproduced. Each member throws rather than returning a
/// plausible default: silently reporting "no such user" from an authentication provider
/// would be a security answer this layer has no business inventing.
/// </summary>
public class SqlMembershipProvider : MembershipProvider
{
    private static Exception NotPorted([System.Runtime.CompilerServices.CallerMemberName] string member = "")
        => new NotSupportedException(
            $"SqlMembershipProvider.{member} は移植されていません。"
            + "ASP.NET Core Identity など、移行先の認証基盤へ置き換えてください。");

    public override string ApplicationName { get => throw NotPorted(); set => throw NotPorted(); }

    public override bool EnablePasswordReset => throw NotPorted();

    public override bool EnablePasswordRetrieval => throw NotPorted();

    public override int MaxInvalidPasswordAttempts => throw NotPorted();

    public override int MinRequiredNonAlphanumericCharacters => throw NotPorted();

    public override int MinRequiredPasswordLength => throw NotPorted();

    public override int PasswordAttemptWindow => throw NotPorted();

    public override MembershipPasswordFormat PasswordFormat => throw NotPorted();

    public override string PasswordStrengthRegularExpression => throw NotPorted();

    public override bool RequiresQuestionAndAnswer => throw NotPorted();

    public override bool RequiresUniqueEmail => throw NotPorted();

    public override bool ChangePassword(string username, string oldPassword, string newPassword)
        => throw NotPorted();

    public override bool ChangePasswordQuestionAndAnswer(
        string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        => throw NotPorted();

    public override MembershipUser CreateUser(
        string username, string password, string email, string passwordQuestion, string passwordAnswer,
        bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        => throw NotPorted();

    public override bool DeleteUser(string username, bool deleteAllRelatedData) => throw NotPorted();

    public override MembershipUserCollection FindUsersByEmail(
        string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        => throw NotPorted();

    public override MembershipUserCollection FindUsersByName(
        string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        => throw NotPorted();

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        => throw NotPorted();

    public override int GetNumberOfUsersOnline() => throw NotPorted();

    public override string GetPassword(string username, string answer) => throw NotPorted();

    public override MembershipUser GetUser(string username, bool userIsOnline) => throw NotPorted();

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) => throw NotPorted();

    public override string GetUserNameByEmail(string email) => throw NotPorted();

    public override string ResetPassword(string username, string answer) => throw NotPorted();

    public override bool UnlockUser(string userName) => throw NotPorted();

    public override void UpdateUser(MembershipUser user) => throw NotPorted();

    public override bool ValidateUser(string username, string password) => throw NotPorted();
}

/// <summary>System.Web.Security.ValidatePasswordEventArgs equivalent.</summary>
public sealed class ValidatePasswordEventArgs(string userName, string password, bool isNewUser) : EventArgs
{
    public string UserName { get; } = userName;

    public string Password { get; } = password;

    public bool IsNewUser { get; } = isNewUser;

    public bool Cancel { get; set; }

    public Exception FailureInformation { get; set; }
}

/// <summary>System.Web.UI.WebControls.CreateUserErrorEventArgs equivalent.</summary>
public sealed class CreateUserErrorEventArgs(object createUserError) : EventArgs
{
    public object CreateUserError { get; } = createUserError;
}

/// <summary>System.Web.UI.WebControls.SendMailErrorEventArgs equivalent.</summary>
public sealed class SendMailErrorEventArgs(Exception exception) : EventArgs
{
    public Exception Exception { get; } = exception;

    public bool Handled { get; set; }
}

/// <summary>System.Web.UI.WebControls.MailMessageEventArgs equivalent.</summary>
public sealed class MailMessageEventArgs(object message) : EventArgs
{
    public object Message { get; } = message;
}

// ---------------------------------------------------------------------------------------
// Configuration and health monitoring
// ---------------------------------------------------------------------------------------

/// <summary>
/// System.Web.Configuration.MachineKeySection equivalent. Key material is configuration in
/// ASP.NET Core (data protection), so the values are read back as configured and nothing
/// here signs or encrypts.
/// </summary>
public class MachineKeySection
{
    public string ValidationKey { get; set; } = "AutoGenerate";

    public string DecryptionKey { get; set; } = "AutoGenerate";

    public string Validation { get; set; } = "HMACSHA256";

    public string Decryption { get; set; } = "Auto";
}

/// <summary>
/// System.Web.Management.WebEventFormatter equivalent. Health monitoring has no ASP.NET
/// Core counterpart; a ported event that formats itself still compiles.
/// </summary>
public sealed class WebEventFormatter
{
    public int IndentationLevel { get; set; }

    public int TabSize { get; set; } = 4;

    public void AppendLine(string s) => _ = s;
}

/// <summary>System.Web.Management.WebRequestInformation equivalent.</summary>
public sealed class WebRequestInformation
{
    public string RequestUrl { get; internal set; }

    public string RequestPath { get; internal set; }

    public string UserHostAddress { get; internal set; }

    public System.Security.Principal.IPrincipal Principal { get; internal set; }

    public string ThreadAccountName { get; internal set; }
}

/// <summary>
/// System.Web.Management.WebBaseEvent equivalent - the root of the health-monitoring
/// events. Applications derive their own (mojoPortal raises one per sign-in attempt) and
/// override Raise / FormatCustomEventDetails, so both are declared virtual.
///
/// Raise is inert: there is no health-monitoring pipeline to raise into. The override still
/// runs whatever the application does before calling base.
/// </summary>
public class WebBaseEvent
{
    public object EventSource { get; protected set; }

    public int EventCode { get; protected set; }

    public int EventDetailCode { get; protected set; }

    public DateTime EventTime { get; } = DateTime.UtcNow;

    public string Message { get; protected set; }

    public virtual void Raise()
    {
    }

    public virtual void FormatCustomEventDetails(WebEventFormatter formatter) => _ = formatter;
}

/// <summary>System.Web.Management.WebAuthenticationSuccessAuditEvent equivalent.</summary>
public class WebAuthenticationSuccessAuditEvent : WebBaseEvent
{
    public string NameToAuthenticate { get; protected set; }

    public WebRequestInformation RequestInformation { get; } = new();
}

/// <summary>System.Web.Management.WebAuthenticationFailureAuditEvent equivalent.</summary>
public class WebAuthenticationFailureAuditEvent : WebBaseEvent
{
    public string NameToAuthenticate { get; protected set; }

    public WebRequestInformation RequestInformation { get; } = new();
}

// ---------------------------------------------------------------------------------------
// Web Parts. The whole personalization framework is out of scope; these are the
// declarations a ported web part names.
// ---------------------------------------------------------------------------------------

/// <summary>System.Web.UI.WebControls.WebParts.WebPart equivalent (declaration surface).</summary>
public class WebPart : LegacyWebControl
{
    public string Title { get; set; }

    public string Subtitle { get; set; }

    public string Description { get; set; }

    public string TitleIconImageUrl { get; set; }

    public string TitleUrl { get; set; }

    public bool AllowClose { get; set; } = true;

    public bool AllowMinimize { get; set; } = true;

    public bool AllowHide { get; set; } = true;
}

/// <summary>System.Web.UI.WebControls.WebParts.WebPartDescription equivalent.</summary>
public sealed class WebPartDescription
{
    public WebPartDescription(string id, string title, string description, string imageUrl)
    {
        ID = id;
        Title = title;
        Description = description;
        CatalogIconImageUrl = imageUrl;
    }

    public string ID { get; }

    public string Title { get; }

    public string Description { get; }

    public string CatalogIconImageUrl { get; }
}

/// <summary>System.Web.UI.WebControls.WebParts.WebPartDescriptionCollection equivalent.</summary>
public sealed class WebPartDescriptionCollection : List<WebPartDescription>
{
    public WebPartDescriptionCollection()
    {
    }

    public WebPartDescriptionCollection(System.Collections.ICollection descriptions)
    {
        foreach (var description in descriptions)
        {
            if (description is WebPartDescription typed)
            {
                Add(typed);
            }
        }
    }
}

/// <summary>System.Web.UI.WebControls.WebParts.CatalogPart equivalent (declaration surface).</summary>
public abstract class CatalogPart : LegacyWebControl
{
    public virtual string Title { get; set; }

    public abstract WebPartDescriptionCollection GetAvailableWebPartDescriptions();

    public abstract WebPart GetWebPart(WebPartDescription description);
}

// ---------------------------------------------------------------------------------------
// ASP.NET AJAX serialization
// ---------------------------------------------------------------------------------------

/// <summary>
/// System.Web.Script.Serialization.JavaScriptConverter equivalent. The converted app
/// serializes with System.Text.Json; a ported converter compiles and is never registered.
/// </summary>
public abstract class JavaScriptConverter
{
    public abstract IEnumerable<Type> SupportedTypes { get; }

    public abstract object Deserialize(
        IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer);

    public abstract IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer);
}
