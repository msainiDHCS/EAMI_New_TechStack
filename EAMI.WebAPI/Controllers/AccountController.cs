using EAMI.Filters;
using EAMI.RuleEngine;
using Microsoft.AspNetCore.Mvc;

namespace EAMI.Controllers
{
    [ApiController]
    [Route("api/Account/")]
    [EAMIAuthorize]
    // [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
    public class AccountController : Controller
    {
       // private readonly WcfServiceInvoker _wcfService;
        private readonly IUserProfileRE _userProfile;
       // private readonly string _strPrgId = string.Empty;

        public AccountController(IUserProfileRE userProfile)
        {
            // _wcfService = new WcfServiceInvoker();
            _userProfile = userProfile;
        }

       
        [HttpGet]
        public IActionResult GetEAMIUser(string userName) //, string prgID)  
        {
            List<UserProfileModelRE> lstUser = _userProfile.GetEAMIUser(userName);
            return new JsonResult(lstUser); //View(lstUser);
        }
        /*
        [HttpGet]
        [Route("UserDetails")]
        public IActionResult UserDetails(string userDetails)
        {
            var decodedUserDetails = HttpUtility.UrlDecode(userDetails);
            var userDetailsModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserProfileModelRE>>(decodedUserDetails);
            return View(userDetailsModel);
        }
        */
        /*
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            /*
            if (ModelState.IsValid)
            {
                GenericPrincipal genericPrincipal = null;

                string _authCacheKey = HttpContext.Session.Id + "_UserAuth";

                string userName = model.UserName;
                string password = model.Password;

                CommonStatusPayload<EAMIUser> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<EAMIUser>>
                                        (svc => svc.GetEAMIUser(userName, null, password, true), false);
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());

                if (cs != null && cs.Status)
                {

                    var windowsPrincipal = HttpContext.User as WindowsPrincipal;

                    GenericIdentity genericIdentity = new GenericIdentity(cs.Payload.User_Name, HttpContext.User.Identity.AuthenticationType);

                    var claims = new List<Claim>();

                    claims.Add(new Claim(ClaimTypes.GivenName, cs.Payload.Display_Name));
                    claims.Add(new Claim(ClaimTypes.Expiration, ""));

                    claims.Add(new Claim("UserID", cs.Payload.User_ID.ToString()));
                    claims.Add(new Claim(ClaimTypes.WindowsAccountName, cs.Payload.User_Name));
                    claims.Add(new Claim(ClaimTypes.Hash, cs.Payload.User_Password));

                    claims.Add(new Claim(ClaimTypes.Expired, "0"));

                    claims.Add(new Claim(ClaimTypes.Authentication, "UNP"));

                    foreach (EAMIAuthBase permission in cs.Payload.Permissions)
                    {
                        claims.Add(new Claim("Permission", permission.Code));
                    }

                    foreach (EAMIAuthBase system in cs.Payload.User_Systems)
                    {
                        claims.Add(new Claim("System", system.Code));
                    }

                    foreach (EAMIAuthBase role in cs.Payload.User_Roles)
                    {
                        claims.Add(new System.Security.Claims.Claim(ClaimTypes.Role, role.Code));
                    }

                    genericIdentity.AddClaims(claims);

                    genericPrincipal = new GenericPrincipal(genericIdentity, cs.Payload.User_Roles.Select(a => a.Code).ToArray());

                    CacheManager<GenericPrincipal>.Set(genericPrincipal, _authCacheKey, "AUTH", 20);

                    HttpContext.User = genericPrincipal;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid user-name or password");
                }
            }
            return View(); // Ensure this method returns IActionResult
        }
        */
    }
}