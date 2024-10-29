using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EAMI.WebAPI.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Produces("application/json")]
    [Route("api/")]
    [ApiController]
    public class BaseApiController : Controller
    {

        /// <summary>
        /// Gets the company identifier.
        /// </summary>
        /// <value>
        /// The company identifier.
        /// </value>
        public int ProgramId { get; private set; }
        public int UserId { get; private set; }
        public string userIp { get; private set; }
        public int UserTypeId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApiController"/> class.
        /// </summary>
        public BaseApiController()
        {
            SetUserDetails(Common.HttpContext_obsolete.CurrentContext);
        }

        private void SetUserDetails(IHttpContextAccessor actionContext)
        {
            if (actionContext == null)
            {
                return;
            }
            var userIdentity = actionContext.HttpContext.User.Identity;
            if (userIdentity.IsAuthenticated)
            {
                //var companyData = (userIdentity as ClaimsIdentity).Claims.Where(c => c.Type == "CompanyID");
                //if (companyData == null)
                //    throw new NotFoundException("Not Authorized to access this resource.");

                //var userData = (userIdentity as ClaimsIdentity).Claims.Where(c => c.Type == "UserID");
                //if (userData == null)
                //    throw new NotFoundException("Not Authorized to access this resource.");

                //var userTypeData = (userIdentity as ClaimsIdentity).Claims.Where(c => c.Type == "UserTypeId");
                //if (userTypeData == null)
                //    throw new NotFoundException("Not Authorized to access this resource.");

                //this.UserId = userData != null && userData.Count() > 0 ? Convert.ToInt32(userData.FirstOrDefault().Value) : 0;
                //this.CompanyId = companyData != null && companyData.Count() > 0 ? Convert.ToInt32(companyData.FirstOrDefault().Value) : 0;
                //this.UserTypeId = userTypeData != null && userTypeData.Count() > 0 ? Convert.ToInt32(userTypeData.FirstOrDefault().Value) : 0;
            }
        }

        //if (actionContext.HttpContext != null)
        //{
        //    this.userIp = actionContext.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString(); 
        //}

    }
}
