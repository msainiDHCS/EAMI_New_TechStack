using EAMI.DataEngine;

namespace EAMI.RuleEngine
{
    public class UserProfileRE : IUserProfileRE//, IComparer<EAMIUser>
    {
        private readonly IUserProfileDE _userProfileDE;
        private readonly string _prgId;
        public UserProfileRE(IUserProfileDE userProfileDE)
        {
            _userProfileDE = userProfileDE; 
           // _prgId = prgId;
        }
        public List<UserProfileModelRE> GetEAMIUser(string userName)
        {
            try
            {
               
                List<UserProfileModelRE> lstUserProfile = new List<UserProfileModelRE>();
                _userProfileDE.GetEAMIUser(userName).ForEach(userProfile =>
                {
                    UserProfileModelRE userProfileRE = new UserProfileModelRE
                    {
                        User_ID = userProfile.User_ID,
                        User_Name = userProfile.User_Name,
                        Display_Name = userProfile.Display_Name,
                        User_EmailAddr = userProfile.User_EmailAddr,
                        // Map other properties as needed
                    };
                    lstUserProfile.Add(userProfileRE);
                });

                return lstUserProfile;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                throw ex;
            }
        }
    }
}
