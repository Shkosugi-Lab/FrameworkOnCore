namespace Microsoft.AspNet.Identity;

// ASP.NET Identity is out of scope for this converter - signing a user in is ASP.NET
// Core's job after the migration, and the compatibility layer deliberately does not
// pretend otherwise (Membership.ValidateUser returns false rather than appearing to work).
//
// What IS in scope is the application's own DATA MODEL. YAF's AspNetUsers is a table POCO
// that implements IUser<TKey>, and that interface carries no behaviour at all - two
// properties the class already declares. Without it the model does not compile, which
// cascaded into BoardContext and, from there, into 1614 build errors.
//
// So the CONTRACT is declared and nothing else. There is no UserManager here, no password
// hasher, no sign-in: a model can name its interface, and any code that tries to actually
// authenticate still has nowhere to go, which is the honest state of an unmigrated login.

/// <summary>
/// Microsoft.AspNet.Identity.IUser&lt;TKey&gt;: the minimum a user model exposes. Declared, not
/// implemented - the interface has no members beyond these two.
/// </summary>
public interface IUser<out TKey>
{
    TKey Id { get; }

    string UserName { get; set; }
}

/// <summary>Microsoft.AspNet.Identity.IUser: the string-keyed form.</summary>
public interface IUser : IUser<string>
{
}

/// <summary>
/// Microsoft.AspNet.Identity.IRole&lt;TKey&gt;, the same shape for roles. Present for the same
/// reason: a role model is data, and its interface carries no behaviour.
/// </summary>
public interface IRole<out TKey>
{
    TKey Id { get; }

    string Name { get; set; }
}

/// <summary>Microsoft.AspNet.Identity.IRole: the string-keyed form.</summary>
public interface IRole : IRole<string>
{
}

/// <summary>
/// Microsoft.AspNet.Identity.IdentityResult: what an Identity operation returned. Pure
/// data - a flag and a list of messages - so the application's own signatures that mention
/// it compile. Nothing here performs an Identity operation.
/// </summary>
public class IdentityResult
{
    public IdentityResult()
    {
    }

    public IdentityResult(params string[] errors) => Errors = errors ?? [];

    public IdentityResult(bool success) => Succeeded = success;

    public bool Succeeded { get; protected set; }

    public System.Collections.Generic.IEnumerable<string> Errors { get; protected set; } = [];

    public static IdentityResult Success { get; } = new(true);

    public static IdentityResult Failed(params string[] errors) => new(errors);
}

/// <summary>
/// Microsoft.AspNet.Identity.UserLoginInfo: an external login, as data.
/// </summary>
public class UserLoginInfo(string loginProvider, string providerKey)
{
    public string LoginProvider { get; set; } = loginProvider;

    public string ProviderKey { get; set; } = providerKey;
}

/// <summary>Microsoft.AspNet.Identity.PasswordVerificationResult.</summary>
public enum PasswordVerificationResult
{
    Failed,
    Success,
    SuccessRehashNeeded,
}

/// <summary>
/// Microsoft.AspNet.Identity.IPasswordHasher - the CONTRACT only.
///
/// No implementation is supplied and none should be: hashing a password is the thing a
/// migration has to decide deliberately, and an inert default would silently accept or
/// reject logins. An application that implements this interface itself keeps its own
/// hashing; one that expected Identity's has nowhere to go, which is the honest state.
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);

    PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
}

/// <summary>Microsoft.AspNet.Identity.IPasswordHasher&lt;TUser&gt;: the generic form.</summary>
public interface IPasswordHasher<in TUser>
{
    string HashPassword(TUser user, string password);

    PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword);
}
