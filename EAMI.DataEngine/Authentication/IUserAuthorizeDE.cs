using EAMI.Common;
using EAMI.CommonEntity;

namespace EAMI.DataEngine
{
    public interface IUserAuthorizeDE
    {
        public CommonStatusPayload<EAMIUser> GetEAMIUser(string userName, string domainName = null, string password = null, bool verifyPassword = false);
        public CommonStatusPayload<EAMIUser> GetEAMIUserByID(long userID);
        public CommonStatus DeactivateEAMIUser(EAMIUser inputUser, string loginUserName);
        public CommonStatus GetActivestatusEAMIUserInfo(long inputUser);
        public CommonStatus AddUpdateEAMIUser(EAMIUser inputUser, string loginUserName);
        public CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>> GetEAMIAuthorizationLookUps(string lookUpType = null);
        public CommonStatus CheckEAMIUserValidity(string userName, long userType);
        public CommonStatusPayload<List<EAMIUser>> GetAllEAMIUsers();
        public CommonStatus AddUpdateEAMIMasterData(EAMIMasterData inputData, string loginUserName);
        public CommonStatusPayload<List<EAMIMasterData>> GetAllEAMIMasterData(string DataType);
        public CommonStatusPayload<EAMIMasterData> GetEAMIMasterDataByID(string DataType, long DataTID);
        public CommonStatusPayload<List<Tuple<EAMIDateType, string, DateTime>>> GetYearlyCalendarEntries(int activeYear, string loginUserName);
        public CommonStatus UpdateYearlyCalendarEntry(List<Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>> dates, string loginUserName);

    }
}
