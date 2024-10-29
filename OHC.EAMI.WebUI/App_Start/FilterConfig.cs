using OHC.EAMI.WebUI.Filters;
using System.Web;
using System.Web.Mvc;

namespace OHC.EAMI.WebUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ValidateAntiForgeryTokenOnPost());
            filters.Add(new AuthorizeResourceAttribute());           
        }
    }
}
