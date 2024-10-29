using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OHC.EAMI.WebUI.Controllers
{
    [EAMIAuthentication]
    //[AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
    public class ErrorHandlerController : Controller
    {
        string ViewName = "~/Views/ErrorHandler/Index.cshtml";
        string UnauthorizedView = "~/Views/ErrorHandler/Unauthorized.cshtml";
        string eErrorMsg = "An error occured while processing your request. EAMI admin has been notified.";

        public ActionResult Index()
        {
            throwHttpException(500, eErrorMsg); //used generic 500 server error code as catch-all.

            return View(ViewName);
        }

        public ActionResult SystemTimeout()
        {
            return View("~/Views/ErrorHandler/SystemTimeout.cshtml");
        }

        public ActionResult NotFound()
        {
            eErrorMsg = eErrorMsg + " The error is a 404 (File Not Found) error.";
            throwHttpException(404, eErrorMsg);

            return View(ViewName);
        }
        
        public ActionResult Unauthorized()
        {
            eErrorMsg = eErrorMsg + " The error is a 401 (Unauthorized Access) error.";
            throwHttpException(401, eErrorMsg);

            return View(UnauthorizedView);
        }

        private void throwHttpException(int errorCode, string emailErrorMsg)
        {
            try
            {
                throw new HttpException(errorCode, emailErrorMsg);
            }
            catch (Exception e)
            {
                ExceptionContext exceptionContext = new ExceptionContext();
                exceptionContext.Exception = e;
                OnException(exceptionContext);
            }
        }

        /// <summary>
        /// Check Fatal Exception.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="isFatal">The isFatal.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static void CheckFatalException(bool status, bool isFatal, string message)
        {
            if (!status && isFatal)
            {
                throw new Exception(message);
            }

        }
    }
}