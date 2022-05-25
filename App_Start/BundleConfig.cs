using System.Web;
using System.Web.Optimization;

namespace Aisger
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new StyleBundle("~/Content/reportCss").Include(
                "~/Content/kendostyles/kendo.common.min.css",
				"~/Content/kendostyles/kendo.blueopal.min.css",
		
                "~/Content/reportAnalyse.css"
                ));

            bundles.Add(new StyleBundle("~/Content/kendoui").Include(
              "~/Content/kendostyles/kendo.common.min.css",
			  "~/Content/kendostyles/kendo.blueopal.min.css"
              ));

            bundles.Add(new ScriptBundle("~/bundles/kendoui").Include(
                "~/Scripts/kendojs/kendo.core.min.js",
                "~/Scripts/kendojs/kendo.web.min.js",
                "~/Scripts/kendojs/kendo.window.min.js",
                "~/Scripts/kendojs/kendo.grid.min.js",
                "~/Scripts/kendojs/kendo.notification.min.js",
                "~/Scripts/kendojs/cultures/kendo.culture.ru-RU.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/reportJs").Include(
                "~/Scripts/rhelper.js",
                "~/Scripts/jinqjs.js",
                "~/Scripts/highcharts2.js",
                "~/Scripts/highcharts-more.js",
                "~/Scripts/exporting.js",
                "~/Scripts/interfaceLang.js"
                ));

        }
    }
}