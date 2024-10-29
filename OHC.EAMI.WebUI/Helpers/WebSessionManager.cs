using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Net;
using System.Configuration;
using OHC.EAMI.Common;
using System.Security.Principal;
using OHC.EAMI.WebUI.Controllers;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace OHC.EAMI.WebUI.Helpers
{
    public abstract class WebSessionManager<T>
    {
        //public static T GetWithCheckSessionStatus(string cacheKey, string cacheScope, AuthenticationContext filterContext)
        //{
        //    CheckSessionStatus(filterContext);
        //    return CacheManager<T>.Get(cacheKey, cacheScope);
        //}

        public static void CheckSessionStatus()
        {
            if (IsSessionExpired())
            {
                Configuration conf = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                SessionStateSection section = (SessionStateSection)conf.GetSection("system.web/sessionState");
                int timeout = (int)section.Timeout.TotalMinutes;

                throw new System.Net.WebException("Session Timeout Has Occurred.  Session Time Limit of " +
                    timeout + " Minutes Has Been Exceeded!", WebExceptionStatus.Timeout);
            }
        }

        private static bool IsSessionExpired()
        {
            if (HttpContext.Current.Session != null)
            {
                if (HttpContext.Current.Session.IsNewSession)
                {
                    string CookieHeaders = HttpContext.Current.Request.Headers["Cookie"];

                    if ((null != CookieHeaders) && (CookieHeaders.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                        //HttpContext.Current.Session.Clear();

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
    }
}