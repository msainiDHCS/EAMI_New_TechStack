using EAMI.RuleEngine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAMI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Authentication/")]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class AuthenticationController : Controller
    {
        private IConfiguration _configuration;
        private IUserAuthorizeRE _userAuthorizeRE;
        private IHttpContextAccessor _httpContextAccessor;

        public AuthenticationController(IConfiguration configuration, IUserAuthorizeRE userAuthorizeRE, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _userAuthorizeRE = userAuthorizeRE;
            _httpContextAccessor = httpContextAccessor;
        }

        //[HttpPost("authenticate")]
        //public IActionResult Authenticate()
        //{
        //    //string webUIBaseUrl = _configuration["ConnectionStrings:"] ?? string.Empty;
        //    //return View();
        //    //string authBaseUrl = _configuration["AuthServer:BaseUrl"];
        //    //string authTokenEndPoint = _configuration["AuthServer:TokenEndPoint"];
        //    //string baseAddress = _configuration["PEServer:BaseUrl"];

        //    // Get programId from request Header
        //    string strPogramChoiceId = string.Empty;
        //    if (_httpContextAccessor == null)
        //    {
        //        // Log or handle the null HttpContext case
        //        throw new InvalidOperationException("HttpContext is null");
        //    }

        //    if (_httpContextAccessor?.HttpContext?.Items?.ContainsKey("ProgramChoiceId") == true)
        //    {
        //        strPogramChoiceId = _httpContextAccessor.HttpContext.Items["ProgramChoiceId"]?.ToString() ?? string.Empty;
        //    }
        //    else
        //    {
        //        // Handle the case where "ProgramChoiceId" is not present
        //        strPogramChoiceId = string.Empty; // or any default value
        //    }

        //    //Logger.LogInfo("current user Ip addres for Authenticate  is" + userIP);

        //    var result = _userAuthorizeRE.GetEAMIUser(); //.UserAuthenticate(strPogramChoiceId);

        //    return Ok(result);
        //}
    }
}
