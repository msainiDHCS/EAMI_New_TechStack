using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Filters;
using OHC.EAMI.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace OHC.EAMI.WebUI.Controllers
{
    [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
    public class AccountController : Controller
    {
        private readonly WcfServiceInvoker _wcfService;

        public AccountController()
        {
            _wcfService = new WcfServiceInvoker();
        }

        // GET: Account
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                GenericPrincipal genericPrincipal = null;

                string _authCacheKey = HttpContext.Session.SessionID + "_UserAuth";

                string userName = model.UserName;
                string password = model.Password;

                Common.CommonStatusPayload<EAMIUser> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<EAMIUser>>
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

            return View();
        }
    }
}