using OHC.EAMI.Common;
using OHC.EAMI.WebUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace OHC.EAMI.WebUI.Controllers
{
    public class ProgramChoiceController : Controller
    {
        public ActionResult Index(string ProgramChoiceId)
        {            
            var redirectedPage = RedirectToAction("Index", "ProgramChoice");
            int intProgramChoiceId = Convert.ToInt32(ProgramChoiceId);            
            if (Convert.ToInt32(Session["ProgramChoiceId"]) == 0 && intProgramChoiceId == 0)
            {
                return View();                
            }
            else
            {
                if (intProgramChoiceId != 0)
                {
                    Session["ProgramChoiceId"] = intProgramChoiceId;
                }
                redirectedPage = RedirectToAction("Index", "Landing");
                string _authCacheKey = HttpContext.Session.SessionID + "_UserAuth";
                CacheManager<GenericPrincipal>.Remove(_authCacheKey, "AUTH");
                HttpContext.User = null;

                return redirectedPage;
            }

        }

    }
}