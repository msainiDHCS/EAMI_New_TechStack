using EAMI.Common;
using EAMI.CommonEntity;
using EAMI.DataEngine;

namespace EAMI.RuleEngine
{
    public class UserAuthorizeRE : IUserAuthorizeRE
    {
        private IUserAuthorizeDE _userAuthorizeDE;
        public UserAuthorizeRE(IUserAuthorizeDE userAuthorizeDE)
        {
            _userAuthorizeDE = userAuthorizeDE;
        }

        public CommonStatusPayload<EAMIUser> GetEAMIUser(string userName, string domainName = null, string password = null, bool verifyPassword = false)
        {
            return _userAuthorizeDE.GetEAMIUser(userName, domainName, password, verifyPassword);
        }

        //public CommonStatusPayload<List<UserProfileModelRE>> UserAuthenticate(string programId)
        //{
        //    CommonStatusPayload<List<UserProfileModelRE>> commonStatusPayload = new CommonStatusPayload<List<UserProfileModelRE>>(new List<UserProfileModelRE>(), true);
        //    List<UserProfileModelRE> userData = _userProfileRE.GetEAMIUser(programId);
        //    if (userData != null && userData.Count > 0)
        //    {
        //        commonStatusPayload.Payload.AddRange(userData);
        //        commonStatusPayload.Status = true;
        //        commonStatusPayload.MessageDetailList.Add("User authenticated successfully");
        //    }
        //    else
        //    {
        //        commonStatusPayload.Status = false;
        //        commonStatusPayload.MessageDetailList.Add("User not authenticated");
        //    }
        //    //var userData = _authenticationDE.
        //    throw new NotImplementedException();
        //}
    }
}
