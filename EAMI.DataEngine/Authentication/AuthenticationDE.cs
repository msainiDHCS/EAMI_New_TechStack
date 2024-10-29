using EAMI.Respository.EAMI_Dental_Repository;
using EAMI.Respository.EAMI_MC_Repository;
using EAMI.Respository.EAMI_RX_Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EAMI.DataEngine
{
    public class AuthenticationDE : IAuthenticationDE
    {
        public int RoleId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ProgramId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private readonly IConfiguration _configuration;
        private readonly EAMI_Dental _dentalContext;
        private readonly EAMI_MC _mcContext;
        private readonly EAMI_RX _rxContext;

        public AuthenticationDE(IConfiguration configuration, EAMI_Dental dentalContext, EAMI_MC mcContext, EAMI_RX rxContext) // Pass configuration to base classbase class
        {
            _configuration = configuration;
            _dentalContext = dentalContext;
            _mcContext = mcContext;
            _rxContext = rxContext;
        }

        public DbContext Context(string dbName)
        {
            return dbName switch
            {
                "DentalDB" => _dentalContext,
                "ManagedCareDB" => _mcContext,
                "PharmacyDB" => _rxContext,
                _ => throw new ArgumentException("Invalid database name", nameof(dbName)),
            };
        }
    }
}
