using System;
using System.Net;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace EAMI.Helpers
{
    public abstract class WebSessionManager<T>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public WebSessionManager(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void CheckSessionStatus()
        {
            if (IsSessionExpired())
            {
                var sessionTimeout = _configuration.GetSection("AppSettings:SessionTimeout").Value;
                if (!int.TryParse(sessionTimeout, out int timeout))
                {
                    throw new InvalidOperationException("Invalid session timeout value in configuration.");
                }

                throw new WebException("Session Timeout Has Occurred. Session Time Limit of " +
                    timeout + " Minutes Has Been Exceeded!", WebExceptionStatus.Timeout);
            }
        }

        private bool IsSessionExpired()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Session != null)
            {
                if (IsNewSession(context))
                {
                    string cookieHeaders = context.Request.Headers["Cookie"];
                    if (!string.IsNullOrEmpty(cookieHeaders) && cookieHeaders.Contains("ASP.NET_SessionId"))
                    {
                        // IsNewSession is true, but session cookie exists,
                        // so, ASP.NET session is expired
                        return true;
                    }
                }
            }
            // Session is not expired and function will return false,
            // could be new session, or existing active session
            return false;
        }

        private bool IsNewSession(HttpContext context)
        {
           // var val = context.Session.Keys.Any(); //.TryGetValue("IsUserSessionExpired", out _);

            // Check if the session is new by using a custom session key
            if (!context.Session.TryGetValue("IsUserSessionExpired", out _))
            {
                // Mark the session as not new for future requests
                context.Session.SetString("IsUserSessionExpired", "false");
                return true;
            }
            return false;
        }
    }

    public class ConcreteWebSessionManager : WebSessionManager<GenericPrincipal>
    {
        public ConcreteWebSessionManager(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
        }
    }
}