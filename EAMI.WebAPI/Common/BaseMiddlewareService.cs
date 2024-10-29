using Microsoft.AspNetCore.Mvc;

namespace EAMI.WebAPI.Common
{
    public class BaseMiddlewareServiceBE
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private object? strPogramChoiceId;

        public BaseMiddlewareServiceBE(IHttpContextAccessor httpContextAccessor) //(RequestDelegate next)
        {
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor.HttpContext == null)
            {
                // Log or handle the null HttpContext case
                throw new InvalidOperationException("HttpContext is null");
            }

            if (_httpContextAccessor.HttpContext.Items.ContainsKey("ProgramChoiceId"))
            {
                strPogramChoiceId = _httpContextAccessor.HttpContext.Items["ProgramChoiceId"]?.ToString();
            }
            else
            {
                // Handle the case where "ProgramChoiceId" is not present
                strPogramChoiceId = string.Empty; // or any default value
            }
        }
    }
}
