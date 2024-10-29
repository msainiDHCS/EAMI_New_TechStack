
using EAMI.Respository.EAMI_Dental_Repository;
using EAMI.Respository.EAMI_MC_Repository;
using EAMI.Respository.EAMI_RX_Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace EAMI.DataEngine
{
    public class BaseMiddlewareServiceDE
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string? _strPogramChoiceId;
        private const string _eamiDBContext_default = "EAMIData";
        private readonly IConfiguration _configuration;
        private readonly EAMI_Dental _dentalContext;
        private readonly EAMI_MC _mcContext;
        private readonly EAMI_RX _rxContext;
        //private readonly RequestDelegate _next;

        public BaseMiddlewareServiceDE(IHttpContextAccessor httpContextAccessor
            , IConfiguration configuration
            , EAMI_Dental dentalContext
            , EAMI_MC mcContext
            , EAMI_RX rxContext)//,RequestDelegate next)
        {
            _configuration = configuration;
            _dentalContext = dentalContext;
            _mcContext = mcContext;
            _rxContext = rxContext;
            _httpContextAccessor = httpContextAccessor;
           // _next = next;

            if (_httpContextAccessor.HttpContext == null)
            {
                // Log or handle the null HttpContext case
                throw new InvalidOperationException("HttpContext is null");
            }

           // bool flag = TryGetSessionValue("ProgramChoiceId", out strPogramChoiceId);

            if (_httpContextAccessor.HttpContext.Items.ContainsKey("ProgramChoiceId"))
            {
                _strPogramChoiceId = _httpContextAccessor.HttpContext.Items["ProgramChoiceId"]?.ToString();
            }
            else
            {
                // Handle the case where "ProgramChoiceId" is not present
                _strPogramChoiceId = string.Empty; // or any default value
            }
        }

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    if (!String.IsNullOrWhiteSpace(_strPogramChoiceId) && _httpContextAccessor.HttpContext != null)
        //    {
        //        _httpContextAccessor.HttpContext.Items["ProgramChoiceId"] = _strPogramChoiceId;
        //        //_httpContextAccessor.HttpContext.Session.SetString("ProgramChoiceId", programChoiceId);
        //    }
        //    await _next(context);
        //}

        public bool TryGetSessionValue(string key, out string? value)
        {
            if (_httpContextAccessor.HttpContext?.Session.TryGetValue(key, out byte[]? valueBytes) == true)
            {
                value = Encoding.UTF8.GetString(valueBytes);
                return true;
            }
            value = null;
            return false;
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

        public string GetDatabaseName()
        {
            return _strPogramChoiceId switch
            {
                "1" => "ManagedCareDB",
                "2" => "DentalDB",
                "3" => "PharmacyDB",
                _ => _eamiDBContext_default,
            };
        }

        public string GetConnectionString()
        {
            string dbName = GetDatabaseName();
            //DbContext dbContext = Context(dbName);
            string? dbConnectionString = _configuration.GetConnectionString(dbName);
            if (dbConnectionString == null)
            {
                throw new InvalidOperationException("Connection string is null");
            }
            return dbConnectionString;
        }
    }
}
