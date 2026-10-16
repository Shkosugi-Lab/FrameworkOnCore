using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Security;

namespace WebControlsProbe
{
    /// <summary>
    /// The probe site's users, in memory (web.config's membership provider: the login controls' scenario, LoginScenario.aspx):
    /// taro (Passw0rd!, the question "color", the answer "blue", in the role "admins"). Reset() puts them back as they were
    /// (each scenario starts from it). A reset password is always NewPass1! (the same on every run).
    /// </summary>
    public sealed class ProbeMembershipProvider : MembershipProvider
    {
        sealed class Account
        {
            public string Name;
            public string Password;
            public string Email;
            public string Question;
            public string Answer;
            public bool Approved = true;
            public DateTime Created = new DateTime(2020, 1, 15, 9, 0, 0);
        }

        static readonly object gate = new object();
        static Dictionary<string, Account> accounts = Initial();

        static Dictionary<string, Account> Initial() => new Dictionary<string, Account>(StringComparer.OrdinalIgnoreCase)
        {
            ["taro"] = new Account { Name = "taro", Password = "Passw0rd!", Email = "taro@example.com", Question = "color", Answer = "blue" },
        };

        public static void Reset()
        {
            lock (gate) accounts = Initial();
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name ?? "Probe", config);
        }

        public override string ApplicationName { get; set; } = "/";
        public override bool EnablePasswordRetrieval => false;
        public override bool EnablePasswordReset => true;
        public override bool RequiresQuestionAndAnswer => true;
        public override int MaxInvalidPasswordAttempts => 5;
        public override int PasswordAttemptWindow => 10;
        public override bool RequiresUniqueEmail => false;
        public override MembershipPasswordFormat PasswordFormat => MembershipPasswordFormat.Hashed;
        public override int MinRequiredPasswordLength => 6;
        public override int MinRequiredNonAlphanumericCharacters => 0;
        public override string PasswordStrengthRegularExpression => "";

        MembershipUser User(Account account) =>
            new MembershipUser(Name, account.Name, account.Name, account.Email, account.Question, "", account.Approved, false,
                               account.Created, account.Created, account.Created, account.Created, DateTime.MinValue);

        public override bool ValidateUser(string username, string password)
        {
            lock (gate) return accounts.TryGetValue(username ?? "", out var account) && account.Approved && account.Password == password;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            lock (gate) return accounts.TryGetValue(username ?? "", out var account) ? User(account) : null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) => GetUser(providerUserKey as string, userIsOnline);

        public override string GetUserNameByEmail(string email)
        {
            lock (gate) return accounts.Values.FirstOrDefault(a => string.Equals(a.Email, email, StringComparison.OrdinalIgnoreCase))?.Name;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer,
                                                  bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            lock (gate)
            {
                if (string.IsNullOrEmpty(username)) { status = MembershipCreateStatus.InvalidUserName; return null; }
                if (accounts.ContainsKey(username)) { status = MembershipCreateStatus.DuplicateUserName; return null; }
                if (password == null || password.Length < MinRequiredPasswordLength) { status = MembershipCreateStatus.InvalidPassword; return null; }
                var account = new Account { Name = username, Password = password, Email = email, Question = passwordQuestion, Answer = passwordAnswer, Approved = isApproved };
                accounts[username] = account;
                status = MembershipCreateStatus.Success;
                return User(account);
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            lock (gate)
            {
                if (!accounts.TryGetValue(username ?? "", out var account) || account.Password != oldPassword) return false;
                if (newPassword == null || newPassword.Length < MinRequiredPasswordLength) throw new ArgumentException("The new password is too short.", nameof(newPassword));
                account.Password = newPassword;
                return true;
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            lock (gate)
            {
                if (!accounts.TryGetValue(username ?? "", out var account)) throw new ProviderException("The user was not found.");
                if (RequiresQuestionAndAnswer && account.Answer != answer) throw new MembershipPasswordException("The answer is wrong.");
                account.Password = "NewPass1!";
                return account.Password;
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) => false;

        public override string GetPassword(string username, string answer) => throw new NotSupportedException("Passwords are not retrievable.");

        public override void UpdateUser(MembershipUser user)
        {
            lock (gate)
                if (accounts.TryGetValue(user.UserName, out var account)) { account.Email = user.Email; account.Approved = user.IsApproved; }
        }

        public override bool UnlockUser(string userName) => true;

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            lock (gate) return accounts.Remove(username ?? "");
        }

        MembershipUserCollection Users(IEnumerable<Account> found, int pageIndex, int pageSize, out int totalRecords)
        {
            var list = found.OrderBy(a => a.Name, StringComparer.Ordinal).ToList();
            totalRecords = list.Count;
            var users = new MembershipUserCollection();
            foreach (var account in list.Skip(pageIndex * pageSize).Take(pageSize)) users.Add(User(account));
            return users;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            lock (gate) return Users(accounts.Values, pageIndex, pageSize, out totalRecords);
        }

        public override int GetNumberOfUsersOnline() => 0;

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            lock (gate) return Users(accounts.Values.Where(a => a.Name.IndexOf(usernameToMatch ?? "", StringComparison.OrdinalIgnoreCase) >= 0), pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            lock (gate) return Users(accounts.Values.Where(a => (a.Email ?? "").IndexOf(emailToMatch ?? "", StringComparison.OrdinalIgnoreCase) >= 0), pageIndex, pageSize, out totalRecords);
        }
    }

    /// <summary>The probe site's roles, in memory: taro is in "admins".</summary>
    public sealed class ProbeRoleProvider : RoleProvider
    {
        static readonly Dictionary<string, string[]> roles = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase) { ["admins"] = new[] { "taro" } };

        public override string ApplicationName { get; set; } = "/";

        public override bool IsUserInRole(string username, string roleName) =>
            roles.TryGetValue(roleName ?? "", out var users) && users.Contains(username ?? "", StringComparer.OrdinalIgnoreCase);

        public override string[] GetRolesForUser(string username) =>
            roles.Where(r => r.Value.Contains(username ?? "", StringComparer.OrdinalIgnoreCase)).Select(r => r.Key).ToArray();

        public override void CreateRole(string roleName) => throw new NotSupportedException();
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) => throw new NotSupportedException();
        public override bool RoleExists(string roleName) => roles.ContainsKey(roleName ?? "");
        public override void AddUsersToRoles(string[] usernames, string[] roleNames) => throw new NotSupportedException();
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) => throw new NotSupportedException();
        public override string[] GetUsersInRole(string roleName) => roles.TryGetValue(roleName ?? "", out var users) ? users : new string[0];
        public override string[] GetAllRoles() => roles.Keys.ToArray();
        public override string[] FindUsersInRole(string roleName, string usernameToMatch) => GetUsersInRole(roleName).Where(u => u.Contains(usernameToMatch ?? "")).ToArray();
    }
}
