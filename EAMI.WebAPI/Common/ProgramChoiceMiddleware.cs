using EAMI.DataEngine;
using Microsoft.AspNetCore.Http;

namespace EAMI.WebAPI.Common
{
    public class ProgramChoiceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProgramChoiceMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("ProgramChoiceId", out var programChoiceId))
            {
                // Set the ProgramChoiceId in HttpContext.Items
                if (!string.IsNullOrWhiteSpace(programChoiceId) && _httpContextAccessor.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.Items["ProgramChoiceId"] = programChoiceId.ToString();
                }
            }

            // Log the authentication status
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                Console.WriteLine("User is authenticated.");
            }
            else
            {
                Console.WriteLine("User is not authenticated.");
            }

            //// Set the ProgramChoiceId in HttpContext.Items
            //var programChoiceId = context.Session.GetString("ProgramChoiceId");

            //if (!String.IsNullOrWhiteSpace(programChoiceId) && _httpContextAccessor.HttpContext != null)
            //{
            //    _httpContextAccessor.HttpContext.Items["ProgramChoiceId"] = programChoiceId;
            //    //_httpContextAccessor.HttpContext.Session.SetString("ProgramChoiceId", programChoiceId);
            //}            

            //// Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
