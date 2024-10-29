using OHC.EAMI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;

namespace OHC.EAMI.WebUI.Filters
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeResourceAttribute : AuthorizeAttribute
    {
        private string[] _roles;

        public AuthorizeResourceAttribute(params string[] roles)
        {
            _roles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string programIdToken = string.Empty;
            programIdToken = httpContext.Request.Headers.GetValues("ProgramChoiceId") != null ?
                        httpContext.Request.Headers.GetValues("ProgramChoiceId").FirstOrDefault() : "0";

            if (base.AuthorizeCore(httpContext))
            {
                if (programIdToken != "0" && programIdToken != httpContext.Session["ProgramChoiceId"].ToString())
                {
                    return false;
                }
                else
                {
                    if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                    {
                        if (_roles.Count() > 0)
                        {
                            try
                            {
                                var uid = (ClaimsIdentity)httpContext.User.Identity;
                                var roleList = uid.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                                if (_roles.Intersect(roleList).Count() == 0)
                                {
                                    return false;
                                }
                            }
                            catch (Exception ex)
                            {
                                EAMILogger.Instance.Error(ex);
                                return false;
                            }

                            return true;
                        }
                        else
                            return true;
                    }
                    else
                        return false;
                }
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                //for GET requests like Search and all popup requests...
                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest()
                    && filterContext.HttpContext.Request.HttpMethod == "GET"
                    && filterContext.HttpContext.Request.CurrentExecutionFilePath != "/ErrorHandler/UnAuthorizedProgramSession"
                    && filterContext.HttpContext.Request.CurrentExecutionFilePath != "/ErrorHandler/UnAuthorized")
                {
                    filterContext.Controller.ViewData.Model = filterContext.HttpContext.Session["ProgramChoiceId"].ToString();
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/ErrorHandler/UnAuthorizedProgramSession.cshtml", //this is done to avoid runtime error "Too many redirects"
                        ViewData = new ViewDataDictionary(filterContext.HttpContext.Session["ProgramChoiceId"].ToString())
                    };
                    return;
                }
                //for POST requests and not the Pop-up requests...
                else if (filterContext.HttpContext.Request.HttpMethod == "POST"
                    && filterContext.HttpContext.Request.CurrentExecutionFilePath != "/ErrorHandler/UnAuthorizedProgramSession"
                    && filterContext.HttpContext.Request.CurrentExecutionFilePath != "/ErrorHandler/UnAuthorized"
                    && filterContext.HttpContext.Request.CurrentExecutionFilePath != "/ErrorHandler")
                {
                    filterContext.Controller.ViewData.Model = filterContext.HttpContext.Session["ProgramChoiceId"].ToString();
                    filterContext.Result = new ViewResult
                    {

                        ViewName = "~/Views/ErrorHandler/Index.cshtml", //this is done to avoid runtime error "Too many redirects"
                        ViewData = new ViewDataDictionary(filterContext.HttpContext.Session["ProgramChoiceId"].ToString())
                    };
                    return;
                }
                // Any other unauthorized requests and not just session override.
                // This usually gives the console error => TOO_MANY_REDIRECTS. Hence the above 2 conditions applied.
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                                   new RouteValueDictionary
                                   {
                                       { "action", "UnAuthorized" },
                                       { "controller", "ErrorHandler" }
                                   });
                    return;
                }
            }
            else
                base.HandleUnauthorizedRequest(filterContext);
        }
    }

    public class ValidateAntiForgeryTokenOnPost : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string cookieToken = "";
            string formToken = "";
            IEnumerable<string> tokenHeaders;
            try
            {
                if (filterContext.HttpContext.Request.HttpMethod != "GET" && filterContext.HttpContext.Request.CurrentExecutionFilePath != "/" && filterContext.HttpContext.Request.CurrentExecutionFilePath != "/Administration/Landing"
                    && filterContext.HttpContext.Request.CurrentExecutionFilePath != "/ErrorHandler/UnAuthorized")
                {
                    tokenHeaders = filterContext.RequestContext.HttpContext.Request.Headers.GetValues("X-CSRF-Token");

                    if (tokenHeaders != null)
                    {
                        string[] tokens = tokenHeaders.First().Split(':');

                        if (tokens.Length >= 2)
                        {
                            cookieToken = tokens[0].Trim();
                            formToken = tokens[1].Trim();
                            //programIdToken = tokens.Count() > 2 ? tokens[2].Trim() : "0";
                        }
                    }
                    AntiForgery.Validate(cookieToken, formToken);
                }
            }
            catch
            {
                filterContext.Result = new RedirectToRouteResult(
                                        new RouteValueDictionary
                                        {
                                       { "action", "UnAuthorized" },
                                       { "controller", "ErrorHandler" }
                                        });
            }
        }
    }
}