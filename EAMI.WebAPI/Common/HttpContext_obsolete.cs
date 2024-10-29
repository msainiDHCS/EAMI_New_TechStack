namespace EAMI.WebAPI.Common
{
    public class HttpContext_obsolete
    {
        //public static IServiceProvider ServiceProvider { get; internal set; }
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static Microsoft.AspNetCore.Http.IHttpContextAccessor CurrentContext
        {
            get
            {
                return _httpContextAccessor;
            }
        }
    }
}
