using System.Configuration;
using System.Web;
using System.Web.Optimization;

namespace Cart.MVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            string StrTheme = ConfigurationManager.AppSettings["Theme"].ToString();
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/loginJs").Include(
                     "~/Scripts/bootstrap-waitingfor.js",
                     "~/Scripts/common-ui.js"));

            bundles.Add(new StyleBundle("~/Content/advantagecss").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/Site.css"));


            bundles.Add(new StyleBundle("~/Content/bootstrapCss").Include(
                       "~/Content/bootstrap-theme.min.css",
                     "~/Content/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/loginCss").Include(
                      "~/Content/Theme/theme-" + @StrTheme + ".css"));

            bundles.Add(new StyleBundle("~/Content/customCss").Include(
                     "~/Content/font-awesome.css",
                     "~/Content/animate.css",
                     "~/Content/Theme/theme-" + @StrTheme + ".css"));
        }
    }
}
