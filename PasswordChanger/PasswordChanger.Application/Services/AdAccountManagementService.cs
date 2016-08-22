namespace PasswordChanger.Application.Services
{
    using System;
    using System.Collections;
    using System.Text;
    using System.DirectoryServices.AccountManagement;
    using System.Data;
    using PasswordChanger.Application.Contracts;
    using System.Reflection;
    using System.Configuration;

    public class AdAccountManagementService : IAdAccountManagementService
    {
        #region Variables

        private readonly string domain; //= @"192.168.7.201";
        private readonly string defaultOU; //= "CN=Test User, OU = Users, OU = CASDM, DC = CASDM, DC = LOCAL";
        private readonly string serviceUser; //= @"CASDM\admin1";
        private readonly string servicePassword;// = @"qaz123wsx";

        public AdAccountManagementService()
        {
            string domain, defaultOU, serviceUser, servicePassword;
            GetValuesFromAppConfig(out domain, out defaultOU, out serviceUser, out servicePassword);
            ValidateValues(domain, defaultOU, serviceUser, servicePassword);
            this.domain = domain;
            this.defaultOU = defaultOU;
            this.serviceUser = serviceUser;
            this.servicePassword = servicePassword;
        }

        private static void ValidateValues(string domain, string defaultOU, string serviceUser, string servicePassword)
        {
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException(nameof(domain), $"The field {nameof(domain)} cannot be empty. Please check app.config");
            }
            if (string.IsNullOrEmpty(defaultOU))
            {
                throw new ArgumentNullException(nameof(defaultOU), $"The field {nameof(defaultOU)} cannot be empty. Please check app.config");
            }
            if (string.IsNullOrEmpty(serviceUser))
            {
                throw new ArgumentNullException(nameof(serviceUser), $"The field {nameof(serviceUser)} cannot be empty. Please check app.config");
            }
            if (string.IsNullOrEmpty(servicePassword))
            {
                throw new ArgumentNullException(nameof(servicePassword), $"The field {nameof(servicePassword)} cannot be empty. Please check app.config");
            }
        }

        private static void GetValuesFromAppConfig (out string domain, out string defaultOU, out string serviceUser, out string servicePassword)
        {
            var assembly = Assembly.GetEntryAssembly();
            Configuration configuration;
            var exeFileName = assembly.Location;
            configuration = ConfigurationManager.OpenExeConfiguration(exeFileName);
            ConfigurationSectionGroup appSettingsGroup = configuration.GetSectionGroup("userSettings");
            ConfigurationSection appSettingsSection = appSettingsGroup.Sections[0];
            ClientSettingsSection settings = appSettingsSection as ClientSettingsSection;
            domain = settings.Settings.Get("domain").Value.ValueXml.InnerText;
            defaultOU = settings.Settings.Get("defaultOU").Value.ValueXml.InnerText;
            serviceUser = settings.Settings.Get("serviceUser").Value.ValueXml.InnerText;
            var password = settings.Settings.Get("servicePassword").Value.ValueXml.InnerText;
            byte[] newBytes = Convert.FromBase64String(password);
            servicePassword = new string(System.Text.Encoding.UTF8.GetString(newBytes).ToCharArray());
        }

        public AdAccountManagementService(string domain, string defaultOU, string serviceUser, string servicePassword)
        {
            this.domain = domain;
            this.defaultOU = defaultOU;
            this.serviceUser = serviceUser;
            this.servicePassword = servicePassword;
        }

        #endregion

        #region Validate Methods

        /// <summary>
        /// Validates the User name and password of a given user
        /// </summary>
        /// <param name="sUserName">The User name to validate</param>
        /// <param name="sPassword">The password of the User name to validate</param>
        /// <returns>Returns True of user is valid</returns>
        public bool ValidateCredentials(string userName, string sPassword)
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();
            return oPrincipalContext.ValidateCredentials(userName, sPassword);
        }

        /// <summary>
        /// Checks if the User Account is Expired
        /// </summary>
        /// <param name="userName">The User name to check</param>
        /// <returns>Returns true if Expired</returns>
        public bool IsUserExpired(string userName)
        {
            UserPrincipal oUserPrincipal = GetUser(userName);
            if (oUserPrincipal.AccountExpirationDate != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Checks if user exists on AD
        /// </summary>
        /// <param name="userName">The User name to check</param>
        /// <returns>Returns true if User name Exists</returns>
        public bool IsUserExisiting(string userName)
        {
            if (GetUser(userName) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Checks if user account is locked
        /// </summary>
        /// <param name="userName">The user name to check</param>
        /// <returns>Returns true of Account is locked</returns>
        public bool IsAccountLocked(string userName)
        {
            UserPrincipal oUserPrincipal = GetUser(userName);
            return oUserPrincipal.IsAccountLockedOut();
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// Gets a certain user on Active Directory
        /// </summary>
        /// <param name="userName">The user name to get</param>
        /// <returns>Returns the UserPrincipal Object</returns>
        public UserPrincipal GetUser(string userName)
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();

            UserPrincipal oUserPrincipal =
                UserPrincipal.FindByIdentity(oPrincipalContext, userName);
            return oUserPrincipal;
        }

        /// <summary>
        /// Gets a certain group on Active Directory
        /// </summary>
        /// <param name="groutName">The group to get</param>
        /// <returns>Returns the GroupPrincipal Object</returns>
        public GroupPrincipal GetGroup(string groutName)
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();

            GroupPrincipal oGroupPrincipal =
                GroupPrincipal.FindByIdentity(oPrincipalContext, groutName);
            return oGroupPrincipal;
        }

        #endregion

        #region User Account Methods

        /// <summary>
        /// Sets the user password
        /// </summary>
        /// <param name="userName">The user name to set</param>
        /// <param name="newPassword">The new password to use</param>
        /// <param name="message">Any output messages</param>
        public void SetUserPassword(string userName, string newPassword, out string message)
        {
            try
            {
                UserPrincipal oUserPrincipal = GetUser(userName);
                oUserPrincipal.SetPassword(newPassword);
                message = "";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        /// <summary>
        /// Enables a disabled user account
        /// </summary>
        /// <param name="userName">The user name to enable</param>
        public void EnableUserAccount(string userName)
        {
            UserPrincipal oUserPrincipal = GetUser(userName);
            oUserPrincipal.Enabled = true;
            oUserPrincipal.Save();
        }

        /// <summary>
        /// Force disabling of a user account
        /// </summary>
        /// <param name="userName">The user name to disable</param>
        public void DisableUserAccount(string userName)
        {
            UserPrincipal oUserPrincipal = GetUser(userName);
            oUserPrincipal.Enabled = false;
            oUserPrincipal.Save();
        }

        /// <summary>
        /// Force expire password of a user
        /// </summary>
        /// <param name="userName">The user name to expire the password</param>
        public void ExpireUserPassword(string userName)
        {
            UserPrincipal oUserPrincipal = GetUser(userName);
            oUserPrincipal.ExpirePasswordNow();
            oUserPrincipal.Save();
        }

        /// <summary>
        /// Unlocks a locked user account
        /// </summary>
        /// <param name="userName">The user name to unlock</param>
        public void UnlockUserAccount(string userName)
        {
            UserPrincipal oUserPrincipal = GetUser(userName);
            oUserPrincipal.UnlockAccount();
            oUserPrincipal.Save();
        }

        /// <summary>
        /// Creates a new user on Active Directory
        /// </summary>
        /// <param name="usersOU">The OU location you want to save your user</param>
        /// <param name="userName">The user name of the new user</param>
        /// <param name="password">The password of the new user</param>
        /// <param name="givenName">The given name of the new user</param>
        /// <param name="surname">The surname of the new user</param>
        /// <returns>returns the UserPrincipal object</returns>
        public UserPrincipal CreateNewUser(string usersOU,
                                           string userName,
                                           string password,
                                           string givenName,
                                           string surname)
        {
            if (!IsUserExisiting(userName))
            {
                PrincipalContext oPrincipalContext = GetPrincipalContext(usersOU);

                UserPrincipal oUserPrincipal = new UserPrincipal(oPrincipalContext, userName, password, true /*Enabled or not*/);

                //User Log on Name
                oUserPrincipal.UserPrincipalName = userName;
                oUserPrincipal.GivenName = givenName;
                oUserPrincipal.Surname = surname;
                oUserPrincipal.Save();

                return oUserPrincipal;
            }
            else
            {
                return GetUser(userName);
            }
        }

        /// <summary>
        /// Deletes a user in Active Directory
        /// </summary>
        /// <param name="userName">The user name you want to delete</param>
        /// <returns>Returns true if successfully deleted</returns>
        public bool DeleteUser(string userName)
        {
            try
            {
                UserPrincipal oUserPrincipal = GetUser(userName);

                oUserPrincipal.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Group Methods

        /// <summary>
        /// Creates a new group in Active Directory
        /// </summary>
        /// <param name="groupsOU">The OU location you want to save your new Group</param>
        /// <param name="groupName">The name of the new group</param>
        /// <param name="description">The description of the new group</param>
        /// <param name="groupScope">The scope of the new group</param>
        /// <param name="securityGroup">True is you want this group
        /// to be a security group, false if you want this as a distribution group</param>
        /// <returns>Returns the GroupPrincipal object</returns>
        public GroupPrincipal CreateNewGroup(string groupsOU,
                                             string groupName,
                                             string description,
                                             GroupScope groupScope,
                                             bool securityGroup)
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext(groupsOU);

            GroupPrincipal oGroupPrincipal = new GroupPrincipal(oPrincipalContext, groupName);
            oGroupPrincipal.Description = description;
            oGroupPrincipal.GroupScope = groupScope;
            oGroupPrincipal.IsSecurityGroup = securityGroup;
            oGroupPrincipal.Save();

            return oGroupPrincipal;
        }

        /// <summary>
        /// Adds the user for a given group
        /// </summary>
        /// <param name="userName">The user you want to add to a group</param>
        /// <param name="groupName">The group you want the user to be added in</param>
        /// <returns>Returns true if successful</returns>
        public bool AddUserToGroup(string userName, string groupName)
        {
            try
            {
                UserPrincipal oUserPrincipal = GetUser(userName);
                GroupPrincipal oGroupPrincipal = GetGroup(groupName);
                if (oUserPrincipal == null || oGroupPrincipal == null)
                {
                    if (!IsUserGroupMember(userName, groupName))
                    {
                        oGroupPrincipal.Members.Add(oUserPrincipal);
                        oGroupPrincipal.Save();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes user from a given group
        /// </summary>
        /// <param name="userName">The user you want to remove from a group</param>
        /// <param name="groupName">The group you want the user to be removed from</param>
        /// <returns>Returns true if successful</returns>
        public bool RemoveUserFromGroup(string userName, string groupName)
        {
            try
            {
                UserPrincipal oUserPrincipal = GetUser(userName);
                GroupPrincipal oGroupPrincipal = GetGroup(groupName);
                if (oUserPrincipal == null || oGroupPrincipal == null)
                {
                    if (IsUserGroupMember(userName, groupName))
                    {
                        oGroupPrincipal.Members.Remove(oUserPrincipal);
                        oGroupPrincipal.Save();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if user is a member of a given group
        /// </summary>
        /// <param name="userName">The user you want to validate</param>
        /// <param name="groupName">The group you want to check the
        /// membership of the user</param>
        /// <returns>Returns true if user is a group member</returns>
        public bool IsUserGroupMember(string userName, string groupName)
        {
            UserPrincipal oUserPrincipal = GetUser(userName);
            GroupPrincipal oGroupPrincipal = GetGroup(groupName);

            if (oUserPrincipal == null || oGroupPrincipal == null)
            {
                return oGroupPrincipal.Members.Contains(oUserPrincipal);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a list of the users group memberships
        /// </summary>
        /// <param name="userName">The user you want to get the group memberships</param>
        /// <returns>Returns an array list of group memberships</returns>
        public ArrayList GetUserGroups(string userName)
        {
            ArrayList myItems = new ArrayList();
            UserPrincipal oUserPrincipal = GetUser(userName);

            PrincipalSearchResult<Principal> oPrincipalSearchResult = oUserPrincipal.GetGroups();

            foreach (Principal oResult in oPrincipalSearchResult)
            {
                myItems.Add(oResult.Name);
            }
            return myItems;
        }

        /// <summary>
        /// Gets a list of the users authorization groups
        /// </summary>
        /// <param name="userName">The user you want to get authorization groups</param>
        /// <returns>Returns an array list of group authorization memberships</returns>
        public ArrayList GetUserAuthorizationGroups(string userName)
        {
            ArrayList myItems = new ArrayList();
            UserPrincipal oUserPrincipal = GetUser(userName);

            PrincipalSearchResult<Principal> oPrincipalSearchResult =
                oUserPrincipal.GetAuthorizationGroups();

            foreach (Principal oResult in oPrincipalSearchResult)
            {
                myItems.Add(oResult.Name);
            }
            return myItems;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the base principal context
        /// </summary>
        /// <returns>Returns the PrincipalContext object</returns>
        public PrincipalContext GetPrincipalContext()
        {
            PrincipalContext oPrincipalContext = new PrincipalContext(ContextType.Domain, domain, defaultOU, ContextOptions.SimpleBind,
    serviceUser, servicePassword);
            return oPrincipalContext;
        }

        /// <summary>
        /// Gets the principal context on specified OU
        /// </summary>
        /// <param name="ou">The OU you want your Principal Context to run on</param>
        /// <returns>Returns the PrincipalContext object</returns>
        public PrincipalContext GetPrincipalContext(string ou)
        {
            PrincipalContext oPrincipalContext =
                new PrincipalContext(ContextType.Domain, domain, ou,
                    ContextOptions.SimpleBind, serviceUser, servicePassword);
            return oPrincipalContext;
        }

        #endregion
    }
}