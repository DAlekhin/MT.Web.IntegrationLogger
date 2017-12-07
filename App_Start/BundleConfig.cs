using System.Web;
using System.Web.Optimization;

namespace MT.Web.IntegrationLogger
{
    public class BundleConfig
    {
        // Дополнительные сведения о Bundling см. по адресу http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/pre-js").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/SyntaxHighlighter/shCore.js",
                        "~/Scripts/SyntaxHighlighter/shBrushXml.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/pace.min.js",
                        "~/Scripts/jasny-bootstrap.min.js",
                        "~/Scripts/jquery.slimscroll.min.js",
                        "~/Scripts/jquery.nanoscroller.min.js",
                        "~/Scripts/DataTables/jquery.dataTables.min.js",
                        "~/Scripts/DataTables/dataTables.responsive.min.js",
                        "~/Scripts/moment-with-locales.min.js",
                        "~/Scripts/bootstrap-datetimepicker.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/post-js").Include(
                        "~/Scripts/metisMenu.min.js",
                        "~/Scripts/custom.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/ie").Include(
                        "~/Scripts/html5shiv.min.js",
                        "~/Scripts/respond.min.js"

            ));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/bootstrap-datetimepicker.min.css"
                        ));

            bundles.Add(new StyleBundle("~/Content/datatable").Include(
                    "~/Content/DataTables/css/jquery.dataTables.css",
                    "~/Content/DataTables/css/responsive.bootstrap.css"
            ));

            bundles.Add(new StyleBundle("~/Content/other").Include(
                        "~/Content/simple-line-icons.css",
                        "~/Content/font-awesome.min.css",
                        "~/Content/pace/pace.css",
                        "~/Content/jasny-bootstrap.min.css",
                        "~/Content/nanoscroller.css",
                        "~/Content/metisMenu.min.css",
                        "~/Content/style.css",
                        "~/Content/SyntaxHighlighter/shCore.css"
            ));

            bundles.Add(new StyleBundle("~/Content/custom").Include(
                        "~/Content/style.css"
            ));

        }
    }
}