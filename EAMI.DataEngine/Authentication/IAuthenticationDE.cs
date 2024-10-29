using Microsoft.EntityFrameworkCore;

namespace EAMI.DataEngine
{
    public interface IAuthenticationDE : IAuthorizeDE
    {
       //  TB_USER GetUserDetails(AuthenticationModel model);
        DbContext Context(string dbName);
    }
}
