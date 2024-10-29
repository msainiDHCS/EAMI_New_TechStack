using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Filters;
using System.Web.Mvc;

namespace OHC.EAMI.WebUI.Controllers
{
    [EAMIAuthentication]

    [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
    public class LandingController : BaseController
    {              

        // GET: Landing

        [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
        public ActionResult Index()
        {
            var redirectedPage = RedirectToAction("Index", "PaymentProcessing");

            // The roles are addessed in descending order.  Since a user can have multiple roles,
            // higher roles take precedence so they are higher up in the if else condition tree.
            if (User.IsInRole("SUPERVISOR") || User.IsInRole("ADMIN"))
            {
                redirectedPage = RedirectToAction("Landing", "Administration");
            }
            else if (User.IsInRole("PROCESSOR"))
            {
                redirectedPage = RedirectToAction("Index", "PaymentProcessing");
            }
            else
            {
                redirectedPage = RedirectToAction("Index", "PaymentProcessing");
            }

            return redirectedPage;
        }
    }
}