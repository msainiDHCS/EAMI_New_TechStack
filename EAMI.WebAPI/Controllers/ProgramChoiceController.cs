using Microsoft.AspNetCore.Mvc;

namespace EAMI.Controllers
{
    [ApiController]
    [Route("api/ProgramChoice/")]
   // [EAMIAuthorize]
    public class ProgramChoiceController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProgramChoiceController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult ProgramChoice(string prgId)
        {
            var redirectedPage = RedirectToAction("Index", "ProgramChoice");
            int intProgramChoiceId = Convert.ToInt32(prgId);
            if (HttpContext.Session is null && intProgramChoiceId == 0)
            {
                return RedirectToAction("Index", "ProgramChoice");
            }
            else
            {
                //if (intProgramChoiceId != 0 && HttpContext.Session != null)
                //{
                //    HttpContext.Session.SetString("ProgramChoiceId", prgId); // intProgramChoiceId);
                //}
                if (!String.IsNullOrEmpty(prgId) && HttpContext.Session != null) // Ensure HttpContext.Session is not null
                {
                    string _authCacheKey = HttpContext.Session.Id + "_UserAuth";
                    //adding the Program id in http context session to be accessed globally...This is used to get the database context.
                    HttpContext.Session.SetString("ProgramChoiceId", prgId);
                    _httpContextAccessor.HttpContext!.Items["ProgramChoiceId"] = prgId; // Use prgId instead of programChoiceId
                    return RedirectToAction("Index", "Landing");
                }
                return RedirectToAction("Index", "ProgramChoice");
            }
        }
    }
}