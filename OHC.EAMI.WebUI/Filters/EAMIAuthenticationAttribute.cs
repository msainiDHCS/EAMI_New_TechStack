using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.CommonEntity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Collections;
using System.Linq;
using System.Web.Security;
using OHC.EAMI.WebUI.Controllers;
using OHC.EAMI.WebUI.Helpers;

namespace OHC.EAMI.WebUI.Filters
{
    public class EAMIAuthenticationAttribute : FilterAttribute, IAuthenticationFilter
    {
        public EAMIAuthenticationAttribute()
        {
            _wcfService = new WcfServiceInvoker();
        }

        private readonly WcfServiceInvoker _wcfService;
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            GenericPrincipal genericPrincipal = null;

            string _authCacheKey = filterContext.HttpContext.Session.SessionID + "_UserAuth";

            string userName = Util.ActiveDirectoryAccess.Instance.GetLoginUserName(HttpContext.Current.User.Identity);
            string domainName = Util.ActiveDirectoryAccess.Instance.GetDomain(HttpContext.Current.User.Identity);

            bool isDatabasePollingNeed = true;
            genericPrincipal = CacheManager<GenericPrincipal>.Get(_authCacheKey, "AUTH");
            //genericPrincipal = WebSessionManager<GenericPrincipal>.GetWithCheckSessionStatus(_authCacheKey, "AUTH", filterContext);


            if (genericPrincipal != null && !genericPrincipal.FindFirst(ClaimTypes.Expired).Equals("0"))
                isDatabasePollingNeed = false;

            //check if cache has the values for the same user
            if (isDatabasePollingNeed)
            {
                CommonStatusPayload<EAMIUser> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<EAMIUser>>
                                        (svc => svc.GetEAMIUser(userName, domainName, null, false), false);

                if (cs != null && cs.Status)
                {

                    if ((cs.Payload.User_Roles != null && cs.Payload.User_Roles.Count() > 0) && (cs.Payload.User_Systems != null && cs.Payload.User_Systems.Count() > 0)
                        && (cs.Payload.Permissions != null && cs.Payload.Permissions.Count() > 0))
                    {

                        var windowsPrincipal = filterContext.HttpContext.User as WindowsPrincipal;

                        GenericIdentity genericIdentity = new GenericIdentity(cs.Payload.User_Name, filterContext.HttpContext.User.Identity.AuthenticationType);

                        var claims = new List<Claim>();

                        if (!string.IsNullOrWhiteSpace(domainName))
                        {
                            claims.Add(new Claim(ClaimTypes.GivenName, Util.ActiveDirectoryAccess.Instance.GetUserFullName(domainName, userName)));
                            claims.Add(new Claim(ClaimTypes.Expiration, Util.ActiveDirectoryAccess.Instance.GetUserPasswordExpiryDays(domainName, userName).ToString()));
                        }
                        else
                        {
                            claims.Add(new Claim(ClaimTypes.GivenName, cs.Payload.Display_Name));
                            claims.Add(new Claim(ClaimTypes.Expiration, ""));
                        }

                        claims.Add(new Claim("UserID", cs.Payload.User_ID.ToString()));
                        claims.Add(new Claim(ClaimTypes.WindowsAccountName, cs.Payload.User_Name));
                        claims.Add(new Claim(ClaimTypes.Hash, cs.Payload.User_Password));

                        claims.Add(new Claim(ClaimTypes.Expired, "0"));

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
                            claims.Add(new Claim(ClaimTypes.Role, role.Code));
                        }

                        genericIdentity.AddClaims(claims);

                        genericPrincipal = new GenericPrincipal(genericIdentity, cs.Payload.User_Roles.Select(a => a.Code).ToArray());

                        CacheManager<GenericPrincipal>.Set(genericPrincipal, _authCacheKey, "AUTH", 20);
                    }
                    else
                    {
                        genericPrincipal = SetExpiredPrincipal(filterContext, _authCacheKey);
                    }
                }
                else
                {
                    genericPrincipal = SetExpiredPrincipal(filterContext, _authCacheKey);
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }
            else
            {
                genericPrincipal = CacheManager<GenericPrincipal>.Get(_authCacheKey, "AUTH");
                //genericPrincipal = WebSessionManager<GenericPrincipal>.GetWithCheckSessionStatus(_authCacheKey, "AUTH", filterContext);
            }

            filterContext.HttpContext.User = genericPrincipal;

            WebSessionManager<GenericPrincipal>.CheckSessionStatus();
        }

        private GenericPrincipal SetExpiredPrincipal(AuthenticationContext filterContext, string _authCacheKey)
        {
            GenericPrincipal genericPrincipal;

            var windowsPrincipal = filterContext.HttpContext.User as WindowsPrincipal;

            GenericIdentity genericIdentity = new GenericIdentity(windowsPrincipal.Identity.Name, filterContext.HttpContext.User.Identity.AuthenticationType);

            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.WindowsAccountName, windowsPrincipal.Identity.Name));
            claims.Add(new Claim(ClaimTypes.Expired, "1"));

            genericIdentity.AddClaims(claims);

            genericPrincipal = new GenericPrincipal(genericIdentity, null);

            CacheManager<GenericPrincipal>.Set(genericPrincipal, _authCacheKey, "AUTH");

            return genericPrincipal;
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            // redirect the user to some form of log in
            if (filterContext.HttpContext.User == null || !filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            //else if (!filterContext.HttpContext.User.IsInRole("PROCESSOR") && !filterContext.HttpContext.User.IsInRole("ADMIN") && !filterContext.HttpContext.User.IsInRole("SUPERVISOR"))
            //{
            //    EAMILogger.Instance.Error("null user");
            //    filterContext.Result = new RedirectToRouteResult("ErrorHandler",
            //           new System.Web.Routing.RouteValueDictionary{
            //            {"controller", "ErrorHandler"},
            //            {"action", "Unauthorized"},
            //            {"returnUrl", filterContext.HttpContext.Request.RawUrl}
            //           });
            //}
            else
            {
                var userid = filterContext.HttpContext.User.Identity as ClaimsIdentity;

                if (userid.FindFirst(ClaimTypes.Expired).Value == "1")
                {
                    //filterContext.Result = new HttpUnauthorizedResult();

                    string _authCacheKey = filterContext.HttpContext.Session.SessionID + "_UserAuth";
                    CacheManager<GenericPrincipal>.Remove(_authCacheKey, "AUTH");
                    

                    //filterContext.Result = new RedirectToRouteResult("ErrorHandler",
                    //       new System.Web.Routing.RouteValueDictionary{
                    //        {"controller", "ErrorHandler"},
                    //        {"action", "Unauthorized"},
                    //        {"returnUrl", filterContext.HttpContext.Request.RawUrl}
                    //       });

                }
            }
        }
    }
}
