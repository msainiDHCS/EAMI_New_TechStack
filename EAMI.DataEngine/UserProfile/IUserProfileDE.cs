

namespace EAMI.DataEngine
{
    public interface IUserProfileDE
    {
        List<UserProfileModelDE> GetEAMIUser(string userName);
    }
}
