using ActiveDs;
using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.Util
{
    public sealed class ActiveDirectoryAccess
    {
        private static volatile ActiveDirectoryAccess instance;
        private static object syncRoot = new object();

        private ActiveDirectoryAccess() { }

        public static ActiveDirectoryAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ActiveDirectoryAccess();
                    }
                }

                return instance;
            }
        }

        public bool ValidateCredentials(string userName, string password, string domainName)
        {
            bool isValid = false;

            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainName))
                {
                    //if you use just user-name and if it fails, it increases bad-logon-count by 2 due to kerberoes and NTLM.
                    isValid = pc.ValidateCredentials(domainName + @"\" + userName, password);
                }
            }
            catch (Exception ex)
            {
            }

            return isValid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        /// https://technet.microsoft.com/en-us/magazine/2007.12.securitywatch.aspx
        public string GetADPasswordGuidelines(string domainName)
        {
            long maxPwdAge = 0;
            long minPwdAge = 0;
            long lockOutObservationWindow = 0;
            long lockoutDuration = 0;
            int lockoutThreshold = 0;
            int minPwdLength = 0;
            int pwdHistoryLength = 0;
            int PasswordSettingsPrecedence = 0;
            bool PasswordComplexityEnabled = false;
            bool PasswordReversibleEncryptionEnabled = false;

            //domainName = "LDAP://intra.dhs.ca.gov:389/CN=Password Settings Container,CN=System,DC=intra,DC=dhs,DC=ca,DC=gov";

            DirectoryEntry entry = new DirectoryEntry(domainName);
            DirectorySearcher mySearcher = new DirectorySearcher(entry);
            //string filter = "(maxPwdAge=*)"; 
            string filter = "";
            mySearcher.Filter = filter;

            SearchResult results = mySearcher.FindOne();

            if (results != null)
            {
                Int64 pwdAge = (Int64)results.Properties["maxPwdAge"][0];
                maxPwdAge = pwdAge / -864000000000;

                Int64 _minPwdAge = (Int64)results.Properties["minPwdAge"][0];
                minPwdAge = _minPwdAge / -864000000000;

                Int64 _lockOutObservationWindow = (Int64)results.Properties["lockOutObservationWindow"][0];
                lockOutObservationWindow = _lockOutObservationWindow / -864000000000;

                Int64 _lockoutDuration = (Int64)results.Properties["lockoutDuration"][0];
                lockoutDuration = _lockoutDuration / -864000000000;

                lockoutThreshold = (int)results.Properties["lockoutThreshold"][0];

                minPwdLength = (int)results.Properties["minPwdLength"][0];

                pwdHistoryLength = (int)results.Properties["pwdHistoryLength"][0];

            }

            return "";
        }

        public bool ChangePassword(string domainName, string userName, string oldPassword, string newPassword)
        {
            bool wasItSuccessFul = false;

            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName))
                {
                    using (UserPrincipal upr = UserPrincipal.FindByIdentity(ctx, userName))
                    {
                        //upr.UnlockAccount();
                        upr.ChangePassword(oldPassword, newPassword);
                        wasItSuccessFul = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return wasItSuccessFul;

        }

        /// <summary>
        /// This call will be handled by an external webservice as it needs higher privilege
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool ResetPassword(string appName, string userName, string callerUserName, string callerPassword)
        {
            return true;
        }

        /// <summary>
        /// This call will be handled by an external webservice as it needs higher privilege
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="userName"></param>
        /// <param name="ADGroupName"></param>
        /// <returns></returns>
        public bool AssociateUserWithADGroup(string appName, string userName, string ADGroupName,
            string callerUserName, string callerPassword)
        {
            return true;
        }

        public string GetDomain(IIdentity identity)
        {
            string s = identity.Name;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(0, stop) : string.Empty;
        }

        public string GetLoginUserName(IIdentity identity)
        {
            string s = identity.Name;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(stop + 1, s.Length - stop - 1) : string.Empty;
        }

        public UserADProfileInfo GetUserADProfileInfo(string domainName, string userName)
        {
            UserADProfileInfo upi = new UserADProfileInfo();
            
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName))
                {
                    using (UserPrincipal upr = UserPrincipal.FindByIdentity(ctx, userName))
                    {
                        DirectoryEntry entry = new DirectoryEntry("LDAP://" + upr.DistinguishedName);
                        IADsUser native = (IADsUser)entry.NativeObject;

                        upi.BadLogonCount = upr.BadLogonCount;
                        upi.LastBadPasswordAttempt = upr.LastBadPasswordAttempt;
                        upi.LastLogOn = upr.LastLogon;// may be incorrect due to different Domain Controllers
                        upi.LastPasswordSet = upr.LastPasswordSet;

                        TimeSpan ts = native.PasswordExpirationDate - DateTime.Now;
                        upi.passExpiryDays = ts.Days;

                        upi.PasswordExpirationDate = native.PasswordExpirationDate;
                        upi.FullName = upr.Name;
                        upi.EmailAddress = upr.EmailAddress;
                        upi.LastName = upr.Surname;
                    }
                }
            }
            catch (Exception ex)
            {
                upi = null;
            }

            return upi;
        }

        public int GetUserPasswordExpiryDays(string domainName, string userName)
        {
            int passExpiryDays = 0;

            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName))
                {
                    using (UserPrincipal upr = UserPrincipal.FindByIdentity(ctx, userName))
                    {
                        DirectoryEntry entry = new DirectoryEntry("LDAP://" + upr.DistinguishedName);
                        IADsUser native = (IADsUser)entry.NativeObject;

                        TimeSpan ts = native.PasswordExpirationDate - DateTime.Now;

                        passExpiryDays = ts.Days;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return passExpiryDays;
        }


        /// <summary>
        /// Will return list of user AD groups
        /// </summary>
        /// <param name="OUName">This will be in format of "OU=ITSD,DC=intra,DC=dhs,DC=ca,DC=gov"</param>
        /// <param name="domainName">This will be like "intra.dhs.ca.gov"</param>
        /// <param name="userName">Like psmith</param>        
        /// <returns>Will return list of user AD groups</returns>
        public List<string> ValidateUserWithOUAndListGroups(string OUName, string domainName, string userName)
        {
            List<string> userGroups = new List<string>();

            // set up a "PrincipalContext" for that OU

            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName, OUName))
                {
                    if (ctx != null)
                    {
                        UserPrincipal user = UserPrincipal.FindByIdentity(ctx, userName);

                        if (user != null)
                        {

                            List<string> fgroups = user.GetGroups().Select(s => s.Name).ToList();
                            if (fgroups != null)
                                userGroups.AddRange(fgroups);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //log error                 
            }

            return userGroups;
        }

        /// <summary>
        /// Will return list of all groups
        /// </summary>
        /// <param name="OUName">This will be in format of "OU=ITSD,DC=intra,DC=dhs,DC=ca,DC=gov"</param>
        /// <param name="domainName">This will be like "intra.dhs.ca.gov"</param>                        
        public List<string> GetAllGroupsForGivenOU(string OUName, string domainName)
        {
            List<string> ougroups = new List<string>();

            // set up a "PrincipalContext" for that OU

            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName, OUName))
                {
                    if (ctx != null)
                    {
                        // define a "query-by-example" principal - here, we search for a GroupPrincipal  
                        var qbeGroup = new GroupPrincipal(ctx);

                        // create your principal searcher passing in the QBE principal     
                        var srch = new PrincipalSearcher(qbeGroup);

                        // find all matches 
                        foreach (Principal found in srch.FindAll())
                        {
                            var foundGroup = found as GroupPrincipal;

                            if (foundGroup != null && foundGroup.IsSecurityGroup == true)
                            {
                                ougroups.Add(foundGroup.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //log error
            }

            return ougroups;
        }

        /// <summary>
        /// Will return OK if given username is found in searched OU and has associated groupname else an error message
        /// </summary>
        /// <param name="OUName">This will be in format of "OU=ITSD,DC=intra,DC=dhs,DC=ca,DC=gov"</param>
        /// <param name="domainName">This will be like "intra.dhs.ca.gov"</param>
        /// <param name="userName">Like psmith</param>
        /// <param name="groupName">Like EAMI_USERS</param>
        /// <returns>OK/Error Message</returns>
        public string SearchUserByOUAndGroupName(string OUName, string domainName, string userName, string groupName)
        {
            string userValidityMessage = "ERROR";

            // set up a "PrincipalContext" for that OU

            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName, OUName))
                {
                    if (ctx != null)
                    {
                        UserPrincipal user = UserPrincipal.FindByIdentity(ctx, userName);

                        if (user != null)
                        {

                            Principal grp = user.GetGroups().ToList().Find(s => s.Name.Trim().ToUpper() == groupName.Trim().ToUpper());

                            if (grp != null)
                            {
                                userValidityMessage = "OK";
                            }
                            else
                            {
                                userValidityMessage = "VALID USER BUT NO GROUP";
                            }
                        }
                        else
                        {
                            userValidityMessage = "VALID OU BUT USER INVALID";
                        }
                    }
                    else
                    {
                        userValidityMessage = "INVALID OU";
                    }
                }
            }
            catch (Exception ex)
            {
                userValidityMessage = ex.Message;
            }

            return userValidityMessage;
        }

        public bool IsUserEnabled(string domainName, string userName)
        {
            bool isUserActive = false;

            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName))
                {
                    if (ctx != null)
                    {
                        using (UserPrincipal upr = UserPrincipal.FindByIdentity(ctx, userName))
                        {
                            if (upr != null && upr.Enabled != null && Convert.ToBoolean(upr.Enabled))
                                isUserActive = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return isUserActive;
        }

        public EAMIADUserStatus GetADUserStatus(string domainName, string userName)
        {
            EAMIADUserStatus currentUserStatus = EAMIADUserStatus.DISABLED;

            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName))
                {
                    using (UserPrincipal upr = UserPrincipal.FindByIdentity(ctx, userName))
                    {
                        DirectoryEntry entry = new DirectoryEntry("LDAP://" + upr.DistinguishedName);
                        IADsUser native = (IADsUser)entry.NativeObject;

                        int badLogonCount = upr.BadLogonCount;
                        DateTime? LastBadPasswordAttempt = upr.LastBadPasswordAttempt;
                        DateTime? LastLogOn = upr.LastLogon;// may be incorrect due to different Domain Controllers
                        DateTime? LastPasswordSet = upr.LastPasswordSet;
                        
                        if (upr.IsAccountLockedOut())
                            currentUserStatus = EAMIADUserStatus.LOCKED;

                        else if (upr.Enabled != null && Convert.ToBoolean(upr.Enabled))
                            currentUserStatus = EAMIADUserStatus.ENABLED;

                        else if (upr.Enabled != null && !Convert.ToBoolean(upr.Enabled))
                            currentUserStatus = EAMIADUserStatus.DISABLED;

                        else if (upr.PasswordNotRequired)
                            currentUserStatus = EAMIADUserStatus.PASSWORD_NOT_REQUIRED;

                        else if (upr.UserCannotChangePassword)
                            currentUserStatus = EAMIADUserStatus.PASSWORD_CAN_NOT_CHANGE;

                        else if (upr.PasswordNeverExpires)
                            currentUserStatus = EAMIADUserStatus.PASSWORD_WILL_NOT_EXPIRE;

                        else if (!upr.LastPasswordSet.HasValue && !upr.PasswordNeverExpires)
                            currentUserStatus = EAMIADUserStatus.PASSWORD_RESET_REQUIRED;

                        else if (native.PasswordExpirationDate <= DateTime.Now)
                            currentUserStatus = EAMIADUserStatus.PASSWORD_RESET_REQUIRED;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return currentUserStatus;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">This looks like intra.dhs.ca.gov</param>
        /// <param name="userName">This looks like uName without @ data</param>
        /// <returns></returns>
        public string GetUserFullName(string domainName, string userName)
        {
            string fullName = string.Empty;
            try
            {
                //DirectoryEntry userEntry = new DirectoryEntry("WinNT://" + domainName + "/" + userName + ",User");
                //return (string)userEntry.Properties["fullname"].Value;

                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName))
                {
                    using (UserPrincipal upr = UserPrincipal.FindByIdentity(ctx, userName))
                    {
                        fullName = upr.Name;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return fullName;
        }

        public string GetUserEmailAddr(string domainName, string userName)
        {
            string retValue = string.Empty;
            try
            {                
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName))
                {
                    using (UserPrincipal upr = UserPrincipal.FindByIdentity(ctx, userName))
                    {
                        retValue = upr.EmailAddress;
                    }
                }
            }
            catch
            { }
            return retValue;
        }

        public bool IsUserAMemberOfADGroup(string domainName, string userName, string groupName)
        {
            bool IsMember = false;

            try
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(new PrincipalContext(ContextType.Domain, domainName), IdentityType.SamAccountName, userName);
                Principal grp = user.GetGroups().ToList().Find(s => s.Name.Trim().ToUpper() == groupName.Trim().ToUpper());
                if (grp != null)
                    IsMember = true;
            }
            catch (Exception ex)
            { }

            return IsMember;
        }

        public UserPrincipal GetUserPrincipal(string domainName, string userName)
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName);
            UserPrincipal upr = UserPrincipal.FindByIdentity(ctx, userName);
            return upr;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ADuserName"></param>
        /// <param name="emailID"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public CommonStatus ValidateUserInformationWithAD(string ADuserName, string emailID, string lastName)
        {
            CommonStatus ret = new CommonStatus(false);

            try
            {
                string domainName = Util.Helper.GetConfigValue("DomainName");

                if (string.IsNullOrEmpty(domainName))
                {
                    ret.AddMessageDetail("Invalid domain");
                }
                else
                {
                    UserADProfileInfo up = this.GetUserADProfileInfo(domainName, ADuserName);

                    if (up != null)
                    {
                        if (string.IsNullOrEmpty(up.EmailAddress))
                            ret.AddMessageDetail("Email does not match with active directory");
                        else if (!up.EmailAddress.Equals(emailID, StringComparison.CurrentCultureIgnoreCase))
                            ret.AddMessageDetail("Email does not match with active directory");
                        else if (!up.LastName.Equals(lastName, StringComparison.CurrentCultureIgnoreCase))
                            ret.AddMessageDetail("Last name does not match with active directory");
                        else
                        {
                            ret.Status = true;
                            ret.AddMessageDetail("Valid User");
                        }
                    }
                    else
                    {
                        ret.AddMessageDetail("User name not found in active directory");
                    }
                }
            }
            catch (Exception ex)
            {
                ret.AddMessageDetail(ex.Message);
            }

            return ret;
        }
    }

}
