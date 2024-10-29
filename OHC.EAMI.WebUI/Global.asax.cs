using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Diagnostics;
using OHC.EAMI.Common;

namespace OHC.EAMI.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();            
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {         
            var app = sender as HttpApplication;
            if (app != null && app.Context != null)
            {
                app.Context.Response.Headers.Remove("Server");
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.Buffer = true;
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1D);
            Response.Expires = -1500;
            Response.CacheControl = "no-cache";
        }

        protected void Application_Error(object sender, EventArgs e)
        {   
            //Log exception
            Exception exception = Server.GetLastError();
            //Logger.LogError(System.Reflection.MethodBase.GetCurrentMethod().Name, exception.Message, exception);

            //Write to nlog
            EAMILogger.Instance.Error(exception);
            //Send Error Email to EAMI Distribution List
            Data.Notification.SendErrorEmail(exception);

            //Clear error from response stream
            Response.Clear();
            //Server.ClearError();

            //Redirect user
            //if (Context.User.Identity.IsAuthenticated)
            //    Context.Server.TransferRequest("~/Portal/Error");
            //else
            //    Context.Server.TransferRequest("~/Anonymous/Error");
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            Session["init"] = 0;
            Session["ProgramChoiceId"] = 0;
        }
    }
}
