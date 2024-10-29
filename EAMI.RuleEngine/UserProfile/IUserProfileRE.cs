

namespace EAMI.RuleEngine
{
    public interface IUserProfileRE
    {
        List<UserProfileModelRE> GetEAMIUser(string userName);
    }
}
