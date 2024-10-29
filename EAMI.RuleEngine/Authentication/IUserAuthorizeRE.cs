using EAMI.Respository.EAMI_Dental_Repository;
using Microsoft.EntityFrameworkCore;
using EAMI.Common;
using EAMI.CommonEntity;

namespace EAMI.RuleEngine
{
    public interface IUserAuthorizeRE
    {
        public CommonStatusPayload<EAMIUser> GetEAMIUser(string userName, string domainName = null, string password = null, bool verifyPassword = false);
    }
}
