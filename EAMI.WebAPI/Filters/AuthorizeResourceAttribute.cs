using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using OHC.EAMI.Common;
using System.Security.Claims;

namespace EAMI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeResourceAttribute : AuthorizeAttribute
    {
        private string[] _roles;

        public AuthorizeResourceAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            //_httpContextAccessor.HttpContext.Items["ProgramChoiceId"]?.ToString();
            string programIdToken = httpContext.Request.Headers["ProgramChoiceId"].FirstOrDefault() ?? "0";

            if (AuthorizeCore(httpContext))
            {
                if (programIdToken != "0" && programIdToken != httpContext.Session.GetString("ProgramChoiceId"))
                {
                    context.Result = new ForbidResult();
                    return;
                }
                else
                {
                    if (httpContext.User?.Identity?.IsAuthenticated == true)
                    {
                        if (_roles.Length > 0)
                        {
                            try
                            {
                                var uid = (ClaimsIdentity)httpContext.User.Identity;
                                var roleList = uid.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                                if (_roles.Intersect(roleList).Count() == 0)
                                {
                                    context.Result = new ForbidResult();
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                //EAMILogger.Instance.Error(ex);
                                context.Result = new ForbidResult();
                                return;
                            }
                        }
                    }
                    else
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }
            else
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        private bool AuthorizeCore(HttpContext httpContext)
        {
            // Implement your core authorization logic here
            // For now, we'll just return true to indicate authorization is successful
            return true;
        }

        private void HandleUnauthorizedRequest(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                if (IsAjaxRequest(context.HttpContext.Request)
                    && context.HttpContext.Request.Method == "GET"
                    && context.HttpContext.Request.Path != "/ErrorHandler/UnAuthorizedProgramSession"
                    && context.HttpContext.Request.Path != "/ErrorHandler/UnAuthorized")
                {
                    context.Result = new ViewResult
                    {
                        ViewName = "~/Views/ErrorHandler/UnAuthorizedProgramSession.cshtml",
                        ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
                        {
                            Model = context.HttpContext.Session.GetString("ProgramChoiceId")
                        }
                    };
                    return;
                }
                else if (context.HttpContext.Request.Method == "POST"
                    && context.HttpContext.Request.Path != "/ErrorHandler/UnAuthorizedProgramSession"
                    && context.HttpContext.Request.Path != "/ErrorHandler/UnAuthorized"
                    && context.HttpContext.Request.Path != "/ErrorHandler")
                {
                    context.Result = new ViewResult
                    {
                        ViewName = "~/Views/ErrorHandler/Index.cshtml",
                        ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
                        {
                            Model = context.HttpContext.Session.GetString("ProgramChoiceId")
                        }
                    };
                    return;
                }
                else
                {
                    context.Result = new RedirectToRouteResult(
                                   new RouteValueDictionary
                                   {
                                           { "action", "UnAuthorized" },
                                           { "controller", "ErrorHandler" }
                                   });
                    return;
                }
            }
            else
                context.Result = new ForbidResult();
        }

        private bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }

    public class ValidateAntiForgeryTokenOnPost : IAuthorizationFilter
    {
        private readonly IAntiforgery _antiforgery;

        public ValidateAntiForgeryTokenOnPost(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                if (context.HttpContext.Request.Method != "GET" && context.HttpContext.Request.Path != "/" && context.HttpContext.Request.Path != "/Administration/Landing"
                    && context.HttpContext.Request.Path != "/ErrorHandler/UnAuthorized")
                {
                    _antiforgery.ValidateRequestAsync(context.HttpContext).GetAwaiter().GetResult();
                }
            }
            catch
            {
                context.Result = new RedirectToRouteResult(
                                        new RouteValueDictionary
                                        {
                                               { "action", "UnAuthorized" },
                                               { "controller", "ErrorHandler" }
                                        });
            }
        }
    }
}