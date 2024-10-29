using System.Web;
using System.Web.Optimization;

namespace OHC.EAMI.WebUI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"                        
                        ));

            bundles.Add(new ScriptBundle("~/bundles/datatablejs").Include(
                        "~/Scripts/DataTables/jquery.dataTables.min.js.js",
                        "~/Scripts/DataTables/dataTables.bootstrap4.min.js"
                        ));

            //bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
            //"~/Scripts/kendo/kendo.all.min.js",
            //    //"~/Scripts/kendo/kendo.all.min.intellisense.js",
            //    // "~/Scripts/kendo/kendo.timezones.min.js", // uncomment if using the Scheduler
            //"~/Scripts/kendo/kendo.aspnetmvc.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*", "~/Scripts/_extensions.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryvalunobtrusive").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.js"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/umd/popper.min.js",
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.sticky.js",
                        //"~/Scripts/custom.js",
                        "~/Scripts/App/Common/common.js"));

            bundles.Add(new ScriptBundle("~/bundles/JMask").Include(
                        "~/Scripts/jquery.mask.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/custom.css",
                "~/Content/EAMI.css",
                "~/Content/bootstrap.min.css",
                "~/Content/themes/base/jquery-ui.min.css"));

            //bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
            //"~/Content/kendo/kendo.common-bootstrap.min.css",
            //            "~/Content/kendo/kendo.bootstrap.min.css"));


            bundles.Add(new StyleBundle("~/Content/FontAwesome").Include(
                "~/Content/font-awesome.min.css"));

            bundles.Add(new StyleBundle("~/Content/Font.Awesome").Include(
                "~/Content/fontawesome/all.css"));

            bundles.Add(new ScriptBundle("~/bundles/eamijs").Include(
                "~/Scripts/App/Common/eami-namespace.js",
                "~/Scripts/App/Commom/eami-init.js"
                //));
                //,
                //"~/Scripts/App/Commom/EAMI_Common.js"
                ));

            BundleTable.EnableOptimizations = false;
        }
    }
}
