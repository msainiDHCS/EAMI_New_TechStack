using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace EAMI.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        /*
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TestController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("username")]
        public IActionResult GetUserName()
        {       
        var userName = Util.ActiveDirectoryAccess.Instance.GetLoginUserName(HttpContext.User.Identity);
            return Ok(new { UserName = userName });
        }

        [HttpGet("claims")]
        public IActionResult GetUserClaims()
        {
            WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            var username = wp.Identity.Name;

            string userName = Util.ActiveDirectoryAccess.Instance.GetLoginUserName(wp.Identity);
            string domainName = Util.ActiveDirectoryAccess.Instance.GetDomain(wp.Identity);

            var claims = _httpContextAccessor.HttpContext.User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });

            return Ok(claims);
        }
        */
    }
}