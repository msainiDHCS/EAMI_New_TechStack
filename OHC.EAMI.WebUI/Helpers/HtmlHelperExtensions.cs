using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace OHC.EAMI.WebUI.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static string AntiForgeryTokenAjax(this HtmlHelper htmlHelper)
        {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            return cookieToken + ":" + formToken;
        }
    }
}