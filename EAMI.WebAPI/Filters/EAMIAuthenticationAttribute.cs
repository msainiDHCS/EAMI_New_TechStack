using EAMI.Common;
using EAMI.CommonEntity;
using EAMI.Helpers;
using EAMI.RuleEngine;
using EAMI.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Security.Principal;

namespace EAMI.Filters
{
    public class EAMIAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public EAMIAuthorizeAttribute()
        {
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            Console.WriteLine("EAMIAuthorizeAttribute invoked for: " + context.HttpContext.Request.Path);
            //set bool session for sessiontimeout. to be used in WebSessionManager.
            context.HttpContext.Session.SetString("IsUserSessionExpired", "false");

            #region this code to pull the user identity from the windows principal is needed if you run code with http/https profiles in launchSettings.json...

            //WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            //var userIdentity = wp.Identity.Name;
            //bool isAuthenticatd = wp.Identity.IsAuthenticated;

            //if (userIdentity == null || !isAuthenticatd)
            //{
            //    context.Result = new ForbidResult();
            //    return;
            //}

            #endregion

            #region this pulls the user identity if you run code in IISExpress profile in launchSettings.json...
            //the AuthenticationType is different in different profiles in launchSettings.json. We need "Negotiate", so run under IISExpress profile
            var authenticationScheme = context.HttpContext.User.Identity?.AuthenticationType;

            if (authenticationScheme == "Kerberos")
            {
                context.Result = new ForbidResult();
                return;
            }
            #endregion

            await Task.Run(() => ValidateAuthorization(context));
        }

        private void ValidateAuthorization(AuthorizationFilterContext context)
        {
            bool status = false;
            long userId = 0;
            string uName = string.Empty;
            string strProgramChoiceId = string.Empty;
            Guid token;
            EAMIUser emaiUserPayload = new EAMIUser();

            if (context.HttpContext.Request.Headers.TryGetValue("ProgramChoiceId", out var programChoiceId))
            {
                strProgramChoiceId = programChoiceId.ToString();
                Console.WriteLine($"ProgramChoiceId: {strProgramChoiceId}");
            }
            else
            {
                Console.WriteLine("ProgramChoiceId header is missing.");
            }

            CommonStatusPayload<EAMIUser> response = new CommonStatusPayload<EAMIUser>(emaiUserPayload, status);

            #region to be implemented later...
            /*
            var claim = context.HttpContext.User.Claims.ToList();
            //EAMILogger.LogInfo("Filter: R6AuthorizeAttribute: Method: ValidateAuthorization, claimCount: " + claim.Count);
            if (claim.Count > 0)
            {

            }
            else
            {
                var AutorizationToken = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                var message = string.Format("From RevFlowAuthorize Claims Not Found in both Owin context and context " +
                   "claims:= Controller Name : {0} , Action Name : {1}, UserId : {2} , CompanyId : {3} ," +
                   " Authentication Token {4} ",
                  context.RouteData.Values["Controller"].ToString() ??
                   "N/A", context.RouteData.Values["Action"].ToString() ?? "N/A", userId.ToString(), companyId.ToString(), AutorizationToken);

                //EAMILogger.LogInfo(message, ErrorCode.ClaimsErrorCode);
                response.Status = false;
            }
            */
            #endregion

            GenericPrincipal genericPrincipal = null;

            string _authCacheKey = context.HttpContext.Session.Id + "_UserAuth";
            var lstClaims = context.HttpContext.User.Claims.ToList();
            if (lstClaims.Count == 0)
            {
                context.Result = new ForbidResult();
                return;
            }

           var header = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            
            string userName = context.HttpContext.User.Identity != null
                ? ActiveDirectoryAccess.Instance.GetLoginUserName(context.HttpContext.User.Identity)
                : throw new InvalidOperationException("User identity is null");
            string domainName = ActiveDirectoryAccess.Instance.GetDomain(context.HttpContext.User.Identity);
            
            //This will resolve the dependency per HTTP request 
            var _userAuthorizeRE = context.HttpContext.RequestServices.GetService(typeof(IUserAuthorizeRE)) as IUserAuthorizeRE;
            var _configuration = context.HttpContext.RequestServices.GetService(typeof(IConfigurationManager)) as IConfigurationManager;
            bool isDatabasePollingNeed = true;
            genericPrincipal = CacheManager<GenericPrincipal>.Get(_authCacheKey, "AUTH");

            if (genericPrincipal != null && genericPrincipal.FindFirst(ClaimTypes.Expired)?.Value != "0")
             //   if (genericPrincipal != null && !genericPrincipal.FindFirst(ClaimTypes.Expired).Equals("0"))
                isDatabasePollingNeed = false;

            if (isDatabasePollingNeed)
            {
                CommonStatusPayload<EAMIUser> cs = _userAuthorizeRE.GetEAMIUser(userName, domainName, null, false);

                if (cs != null && cs.Status)
                {
                    if ((cs.Payload.User_Roles != null && cs.Payload.User_Roles.Count() > 0) && (cs.Payload.User_Systems != null && cs.Payload.User_Systems.Count() > 0)
                        && (cs.Payload.Permissions != null && cs.Payload.Permissions.Count() > 0))
                    {
                        var windowsPrincipal = context.HttpContext.User as WindowsPrincipal;

                        GenericIdentity genericIdentity = new GenericIdentity(cs.Payload.User_Name, context.HttpContext.User.Identity.AuthenticationType);
                        var claimType = "myNewClaim";

                        var claims = new List<Claim>();                        
                        if (!string.IsNullOrWhiteSpace(domainName))
                        {
                            claims.Add(new Claim(ClaimTypes.GivenName, ActiveDirectoryAccess.Instance.GetUserFullName(domainName, userName)));
                            claims.Add(new Claim(ClaimTypes.Expiration, ActiveDirectoryAccess.Instance.GetUserPasswordExpiryDays(domainName, userName).ToString()));
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
                        if (!String.IsNullOrWhiteSpace(strProgramChoiceId))
                        {
                            claims.Add(new Claim("ProgramChoiceId", strProgramChoiceId));
                        }
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
                        genericPrincipal = SetExpiredPrincipal(context, _authCacheKey);
                    }
                }
                else
                {
                    genericPrincipal = SetExpiredPrincipal(context, _authCacheKey);
                }
                //ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }
            else
            {
                genericPrincipal = CacheManager<GenericPrincipal>.Get(_authCacheKey, "AUTH");
            }

            context.HttpContext.User = genericPrincipal;
            //WebSessionManager<GenericPrincipal>.CheckSessionStatus();

            // Instantiate the ConcreteWebSessionManager
            // Call the CheckSessionStatus method            
            var httpContextAccessor = context.HttpContext.RequestServices.GetService<IHttpContextAccessor>();
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

            if (httpContextAccessor == null || configuration == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var webSessionManager = new ConcreteWebSessionManager(httpContextAccessor, configuration);
            webSessionManager.CheckSessionStatus();
        }

        private GenericPrincipal SetExpiredPrincipal(AuthorizationFilterContext context, string _authCacheKey)
        {
            GenericPrincipal genericPrincipal;

            var windowsPrincipal = context.HttpContext.User;// as WindowsPrincipal;

            GenericIdentity genericIdentity = new GenericIdentity(windowsPrincipal.Identity.Name, context.HttpContext.User.Identity.AuthenticationType);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.WindowsAccountName, windowsPrincipal.Identity.Name),
                new Claim(ClaimTypes.Expired, "1")
            };

            genericIdentity.AddClaims(claims);

            genericPrincipal = new GenericPrincipal(genericIdentity, null);

            CacheManager<GenericPrincipal>.Set(genericPrincipal, _authCacheKey, "AUTH");

            return genericPrincipal;
        }
    }
}
