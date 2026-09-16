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
