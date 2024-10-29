using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EAMI.WebApi.Models;
using EAMI.Helpers;

namespace EAMI.WebAPI.Common
{
    /// <summary>
    /// TokenController
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [AllowAnonymous]
    [Route("api")]
    [ApiController]
    public class TokenController : Controller
    {
        /*
        /// <summary>
        /// The configuration
        /// </summary>
        private IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Token Generate and Refresh Token 

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        [HttpPost("token")]
        public IActionResult CreateToken(TokenParameters parameters)
        {
            if (parameters == null)
            {
                return BadRequest();
            }

            if (!string.IsNullOrEmpty(parameters.Grant_type))
            {
                //Genrate Token
                return Json(GetTokenWithClaim(parameters));
            }
            else
            {
                return BadRequest();
            }
        }

        #region Private Method

        /// <summary>
        /// Gets the token with claim.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expires">The expires.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns></returns>
        private TokenDetails GetTokenWithClaim(TokenParameters parameters)
        {
            TokenDetails response = null;
            try
            {
                var refreshToken = Guid.NewGuid().ToString().Replace("-", "");

                var handler = new JwtSecurityTokenHandler();

                var requestedAt = DateTime.UtcNow;
                var claims = new Claim[]
                {
                new Claim("UserName", parameters.User_name),
                new Claim("UserID", parameters.User_id.ToString()),
                 new Claim("ProgramChoiceId", parameters.Program_Choice_Id),
                new Claim(JwtRegisteredClaimNames.Sub, parameters.User_id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,  requestedAt.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)

                };

                var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                double.TryParse(_configuration["JWT:tokenExpireInMinutes"], out double tokenExpire);

                var jwt = handler.CreateJwtSecurityToken(new SecurityTokenDescriptor()
                {
                    Issuer = _configuration["JWT:issuer"],
                    Audience = _configuration["JWT:audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"])), SecurityAlgorithms.HmacSha256),
                    Subject = identity,
                    NotBefore = requestedAt,
                    Expires = requestedAt.Add(TimeSpan.FromMinutes(tokenExpire))
                });

                var encodedJwt = handler.WriteToken(jwt);
                var guid = Guid.Parse(HtmlHelperExtensions.StringToGuid(encodedJwt)).ToString();
                response = new TokenDetails()
                {
                    Access_token = encodedJwt,
                    Expires_in = (int)TimeSpan.FromMinutes(tokenExpire).TotalSeconds,
                    Refresh_token = refreshToken,
                    Token_guid = guid
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return response;
        }

        #endregion

        #endregion
        */
    }
}