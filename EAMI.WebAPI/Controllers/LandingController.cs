using EAMI.Filters;
using EAMI.RuleEngine;
using Microsoft.AspNetCore.Mvc;

namespace EAMI.Controllers
{
    [ApiController]
    [Route("Landing")]
    [EAMIAuthorize] 
    public class LandingController : Controller
    {
        //private readonly HttpClient _client;
        private readonly IUserProfileRE _userProfile;
        private readonly string prgId = string.Empty;
        public LandingController(IUserProfileRE userProfile) //HttpClient client
        {
            //_client = client ?? throw new ArgumentNullException(nameof(client));
            _userProfile = userProfile;
            // prgId = ProgramChoiceId;
        }
        /*

        [HttpGet("GetEAMIUser")]
        public IActionResult GetEAMIUser(string userName)
        {
            List<UserProfileModelRE> lstUser = _userProfile.GetEAMIUser(userName);
            return Json(lstUser);
        }

        */

        [HttpGet]
        public ActionResult Index()
        {
           // string prg2 = HttpContext.Session.GetString("ProgramChoiceId");
            var redirectedPage = RedirectToAction("GetEAMIUser", "Account", new { userName = "msaini" });

            // The roles are addessed in descending order.  Since a user can have multiple roles,
            // higher roles take precedence so they are higher up in the if else condition tree.
            if (User.IsInRole("SUPERVISOR") || User.IsInRole("ADMIN"))
            {
                //redirectedPage = RedirectToAction("Landing", "Administration");
                redirectedPage = RedirectToAction("GetEAMIUser", "Account", new { userName = "msaini" });
            }
            else if (User.IsInRole("PROCESSOR"))
            {
                //redirectedPage = RedirectToAction("Index", "PaymentProcessing");
            }
            else
            {
                //redirectedPage = RedirectToAction("Index", "PaymentProcessing");
            }

            return redirectedPage;
        }
    }
}