using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;

namespace AuthProbe
{
    /// <summary>
    /// Forms authentication with SQL Server's membership, roles and profile (aspnet_regsql's schema): the users and roles
    /// made, signing in and out (the ticket, its cookie), the roles of the user, the profile saved, a password changed, a
    /// user locked out by wrong passwords. Writes what the request sees: the user, the ticket, the providers' settings and
    /// data (no dates, no keys: they differ from run to run).
    /// </summary>
    public partial class Default : Page
    {
        readonly List<string> entries = new List<string>();

        const string AlicePassword = "alice-pass1";
        const string BobPassword = "bob-pass1";

        protected void Setup(object sender, EventArgs e)
        {
            foreach (var name in new[] { "alice", "bob", "carol" })
                if (Membership.GetUser(name) != null) Membership.DeleteUser(name, true);
            foreach (var role in Roles.GetAllRoles()) Roles.DeleteRole(role, false);
            Roles.CreateRole("Admin");
            Roles.CreateRole("Editor");

            MembershipCreateStatus status;
            Membership.CreateUser("alice", AlicePassword, "alice@example.com", null, null, true, out status);
            entries.Add("Create alice: " + status);
            Membership.CreateUser("alice", AlicePassword, "alice2@example.com", null, null, true, out status);
            entries.Add("Create alice again: " + status);
            Membership.CreateUser("carol", "short", "carol@example.com", null, null, true, out status);
            entries.Add("Create carol with a short password: " + status);
            Membership.CreateUser("carol", "carol-pass1", "alice@example.com", null, null, true, out status);
            entries.Add("Create carol with alice's e-mail: " + status);
            Membership.CreateUser("bob", BobPassword, "bob@example.com", null, null, true, out status);
            entries.Add("Create bob: " + status);
            try
            {
                Membership.CreateUser("x", "x");
            }
            catch (MembershipCreateUserException ex)
            {
                entries.Add("CreateUser(x, x): " + ex.GetType().Name + " " + ex.StatusCode);
            }

            Roles.AddUserToRoles("alice", new[] { "Admin", "Editor" });
            Roles.AddUserToRole("bob", "Editor");
            try
            {
                Roles.AddUserToRole("bob", "Editor");
            }
            catch (Exception ex)
            {
                entries.Add("bob to Editor again: " + ex.GetType().Name);
            }
            try
            {
                Roles.AddUserToRole("bob", "Nobody");
            }
            catch (Exception ex)
            {
                entries.Add("bob to a role that is not: " + ex.GetType().Name);
            }
            entries.Add("Click Setup");
        }

        void SignIn(string name, string password, bool persistent)
        {
            var valid = Membership.ValidateUser(name, password);
            entries.Add("ValidateUser(" + name + "): " + valid);
            if (!valid) return;
            if (persistent)
            {
                var cookie = FormsAuthentication.GetAuthCookie(name, true);
                entries.Add("Persistent cookie: " + cookie.Name + " expires set " + (cookie.Expires != DateTime.MinValue) + " HttpOnly " + cookie.HttpOnly + " Path " + cookie.Path);
                Response.Cookies.Add(cookie);
            }
            else FormsAuthentication.SetAuthCookie(name, false);
        }

        protected void SignInAlice(object sender, EventArgs e) => SignIn("alice", AlicePassword, false);

        protected void SignInBob(object sender, EventArgs e) => SignIn("bob", BobPassword, false);

        protected void SignInPersistent(object sender, EventArgs e) => SignIn("alice", AlicePassword, true);

        protected void SignOut(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            var cookie = Response.Cookies[FormsAuthentication.FormsCookieName];
            entries.Add("SignOut: the cookie " + (cookie == null ? "not set" : "set, value empty " + (cookie.Value == "") + ", expired " + (cookie.Expires < DateTime.Now)));
        }

        protected void UpdateProfile(object sender, EventArgs e)
        {
            var profile = Context.Profile;
            entries.Add("Profile of " + profile.UserName + " anonymous " + profile.IsAnonymous);
            if (profile.IsAnonymous) return;
            profile.SetPropertyValue("Nickname", profile.UserName + "-nick");
            profile.SetPropertyValue("Visits", (int)profile.GetPropertyValue("Visits") + 1);
            profile.GetProfileGroup("Prefs").SetPropertyValue("Color", "green");
            profile.Save();
            entries.Add("Profile saved, dirty after " + profile.IsDirty);
        }

        protected void ChangePassword(object sender, EventArgs e)
        {
            var user = Membership.GetUser("alice");
            entries.Add("ChangePassword with a wrong one: " + user.ChangePassword("wrong", "alice-pass2"));
            entries.Add("ChangePassword: " + user.ChangePassword(AlicePassword, "alice-pass2"));
            entries.Add("ValidateUser(old): " + Membership.ValidateUser("alice", AlicePassword));
            entries.Add("ValidateUser(new): " + Membership.ValidateUser("alice", "alice-pass2"));
            try
            {
                user.ChangePassword("alice-pass2", "ab");
            }
            catch (Exception ex)
            {
                entries.Add("ChangePassword to a short one: " + ex.GetType().Name);
            }
            entries.Add("ChangePassword back: " + user.ChangePassword("alice-pass2", AlicePassword));
        }

        protected void LockOut(object sender, EventArgs e)
        {
            for (var i = 1; i <= 3; i++)
                entries.Add("ValidateUser(bob, wrong) " + i + ": " + Membership.ValidateUser("bob", "wrong") + ", locked out " + Membership.GetUser("bob").IsLockedOut);
            entries.Add("ValidateUser(bob) while locked out: " + Membership.ValidateUser("bob", BobPassword));
            entries.Add("UnlockUser: " + Membership.GetUser("bob").UnlockUser());
            entries.Add("ValidateUser(bob) unlocked: " + Membership.ValidateUser("bob", BobPassword));
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            var lines = new List<string>();
            lines.Add("IsPostBack=" + IsPostBack);
            lines.Add("User: " + User.GetType().Name + " " + User.Identity.GetType().Name + " IsAuthenticated=" + User.Identity.IsAuthenticated +
                " Name=" + User.Identity.Name + " AuthenticationType=" + User.Identity.AuthenticationType + " Request.IsAuthenticated=" + Request.IsAuthenticated);
            var identity = User.Identity as FormsIdentity;
            if (identity != null)
            {
                var ticket = identity.Ticket;
                lines.Add("Ticket: Name=" + ticket.Name + " Version=" + ticket.Version + " IsPersistent=" + ticket.IsPersistent + " Minutes=" +
                    (ticket.Expiration - ticket.IssueDate).TotalMinutes + " CookiePath=" + ticket.CookiePath + " UserData=" + ticket.UserData + " Expired=" + ticket.Expired);
            }
            lines.Add("IsInRole: Admin=" + User.IsInRole("Admin") + " Editor=" + User.IsInRole("Editor") + " Nobody=" + User.IsInRole("Nobody"));
            if (User.Identity.IsAuthenticated)
            {
                lines.Add("Roles.GetRolesForUser: " + string.Join(",", Roles.GetRolesForUser().OrderBy(r => r)));
                var profile = Context.Profile;
                lines.Add("Profile: Nickname=" + profile.GetPropertyValue("Nickname") + " Visits=" + profile.GetPropertyValue("Visits") +
                    " Prefs.Color=" + profile.GetProfileGroup("Prefs").GetPropertyValue("Color"));
            }
            lines.Add("Forms: CookieName=" + FormsAuthentication.FormsCookieName + " LoginUrl=" + FormsAuthentication.LoginUrl + " DefaultUrl=" +
                FormsAuthentication.DefaultUrl + " Timeout=" + FormsAuthentication.Timeout.TotalMinutes + " Sliding=" + FormsAuthentication.SlidingExpiration +
                " CookieMode=" + FormsAuthentication.CookieMode + " RequireSSL=" + FormsAuthentication.RequireSSL + " CookiesSupported=" + FormsAuthentication.CookiesSupported);
            var provider = (SqlMembershipProvider)Membership.Provider;
            lines.Add("Membership: " + provider.Name + " " + provider.ApplicationName + " PasswordFormat=" + provider.PasswordFormat + " MinLength=" +
                provider.MinRequiredPasswordLength + " MaxInvalid=" + provider.MaxInvalidPasswordAttempts + " UniqueEmail=" + provider.RequiresUniqueEmail +
                " Reset=" + provider.EnablePasswordReset + " Retrieval=" + provider.EnablePasswordRetrieval);
            lines.Add("Roles: " + Roles.Provider.Name + " Enabled=" + Roles.Enabled + " CacheRolesInCookie=" + Roles.CacheRolesInCookie + " CookieName=" + Roles.CookieName);
            lines.Add("ProfileManager: " + ProfileManager.Provider.Name + " Enabled=" + ProfileManager.Enabled + " Profiles=" + ProfileManager.GetNumberOfProfiles(ProfileAuthenticationOption.All));
            var users = Membership.GetAllUsers().Cast<MembershipUser>().OrderBy(u => u.UserName).ToList();
            lines.Add("Users: " + string.Join(", ", users.Select(u => u.UserName + " <" + u.Email + "> approved " + u.IsApproved + " locked " + u.IsLockedOut)));
            lines.Add("Roles: " + string.Join(", ", Roles.GetAllRoles().OrderBy(r => r).Select(r => r + "=" + string.Join("+", Roles.GetUsersInRole(r).OrderBy(u => u)))));
            lines.Add("FindUsersByEmail(%@example.com): " + string.Join(",", Membership.FindUsersByEmail("%@example.com").Cast<MembershipUser>().Select(u => u.UserName).OrderBy(n => n)));
            lines.Add("Request cookies: " + string.Join(",", Request.Cookies.AllKeys.OrderBy(k => k)));
            lines.AddRange(entries);
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", lines));
        }
    }
}
