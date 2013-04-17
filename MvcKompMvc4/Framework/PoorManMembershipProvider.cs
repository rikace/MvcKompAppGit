using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MvcKompApp.Framework
{
    public class PoorManMembershipProvider : MembershipProvider
    {
        public override bool ValidateUser(String username, String password)
        {
            // Validate credentials against your data store
            var response = ValidateUserInternal(username, password);
            return response;
        }

        public override MembershipUser GetUser(String username, Boolean userIsOnline)
        {
            // Query for specific user
            // ...

            // Wrap retrieved data to the ASP.NET data structure
            var user = new MembershipUser(ApplicationName, username, username, username + "@contoso.com", "", "", true, true, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            return user;
        }

        public override MembershipUser CreateUser(String username, String password, String email, String passwordQuestion, String passwordAnswer, Boolean isApproved, Object providerUserKey, out MembershipCreateStatus status)
        {
            // Add a new user
            // ...

            // Retrieve and return a MembershipUser object for the just created user
            var user = new MembershipUser(ApplicationName, username, providerUserKey, username + "@contoso.com", "", "", isApproved, true, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            status = MembershipCreateStatus.Success;
            return user;
        }

        public override Int32 MinRequiredPasswordLength
        {
            get { return 1; }
        }

        public override Boolean ChangePassword(String username, String oldPassword, String newPassword)
        {
            // Update password in your data store
            // ...

            return true;
        }

        #region Unused membership members
        public override string ApplicationName
        {
            get { return "PoorManMembershipProvider"; }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Data store wrappers
        private static Boolean ValidateUserInternal(string username, string password)
        {
            return !String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password);
        }
        #endregion
    }

}