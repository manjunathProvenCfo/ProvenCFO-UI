using System.Web;
using System.Web.Optimization;

namespace ProvenCfoUI
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/customjquery").Include(

            //         "~/assets/js/jquery.min.js" ,
            //         "~/assets/js/popper.min.js", 
            //         "~/assets/js/bootstrap.min.js" ,
            //         "~/assets/lib/@fortawesome/all.min.js" ,
            //         "~/assets/lib/stickyfilljs/stickyfill.min.js",
            //         "~/assets/lib/sticky-kit/sticky-kit.min.js", 
            //         "~/assets/lib/is_js/is.min.js" ,
            //         "~/assets/lib/lodash/lodash.min.js" ,
            //         "~/assets/lib/perfect-scrollbar/perfect-scrollbar.js" ,    
            //        "~/assets/lib/echarts/echarts.min.js" ,  
            //        "~/assets/lib/progressbar.js/progressbar.min.js" ,
            //        "~/assets/js/theme.js" 
            //           ));

            bundles.Add(new Bundle("~/bundles/customjquery").Include(
             "~/assets/js/jquery.min.js",
             "~/assets/js/popper.min.js",
             "~/assets/js/bootstrap.min.js",
             "~/assets/lib/moment/dist/moment.min.js",
             "~/assets/lib/fortawesome/all.min.js",
             "~/assets/lib/stickyfilljs/stickyfill.min.js",
             "~/assets/lib/sticky-kit/sticky-kit.min.js",
             "~/assets/lib/is_js/is.min.js",
             "~/assets/lib/lodash/lodash.min.js",
             "~/assets/lib/perfect-scrollbar/perfect-scrollbar.js",
             "~/assets/lib/chart.js/Chart.min.js",
             "~/assets/lib/datatables/js/jquery.dataTables.min.js",
             "~/assets/lib/datatables-bs4/dataTables.bootstrap4.min.js",
             "~/assets/lib/datatables.net-responsive/dataTables.responsive.js",
             "~/assets/lib/datatables.net-responsive-bs4/responsive.bootstrap4.js",
             "~/assets/lib/leaflet/leaflet.js",
             "~/assets/lib/leaflet.markercluster/leaflet.markercluster.js",
             "~/assets/lib/leaflet.tilelayer.colorfilter/leaflet-tilelayer-colorfilter.min.js",
             "~/assets/lib/select2/select2.min.js",
             "~/assets/lib/shopify-draggable/draggable.bundle.js",
             "~/assets/lib/lightbox2/js/lightbox.min.js",
             "~/assets/lib/fancybox/jquery.fancybox.min.js",
             "~/assets/lib/echarts/echarts.min.js",
             "~/assets/lib/progressbar.js/progressbar.min.js",
             "~/assets/lib/anchor.min.js",
             "~/assets/lib/prismjs/prism.js",
             "~/assets/lib/list.min.js",
             "~/assets/lib/flatpickr/flatpickr.min.js",
              "~/assets/lib/dropzone/dropzone.min.js",
              "~/assets/lib/emojionearea/emojionearea.min.js",
             "~/assets/js/theme.js",
              "~/assets/js/wrapper.js"
    ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new Bundle("~/bundles/js/twilio-chat").Include(
                        "~/assets/js/chat.js",
                        "~/assets/js/twilio.js"));
            //Css Start

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/theme").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/site.css"));

            //bundles.Add(new StyleBundle("~/bundles/customcss").Include(
            //        "~/assets/js/config.navbar-vertical.js" ,
            //        "~/assets/lib/perfect-scrollbar/perfect-scrollbar.css",
            //        "~/assets/css/theme.css"


            //         ));

            bundles.Add(new StyleBundle("~/bundles/customcss").Include(
                  "~/assets/css/common.css",
                  "~/assets/js/config.navbar-vertical.js",
                   "~/assets/lib/perfect-scrollbar/perfect-scrollbar.css",
                    "~/assets/lib/datatables-bs4/dataTables.bootstrap4.min.css",
                    "~/assets/lib/datatables.net-responsive-bs4/responsive.bootstrap4.css",
                    "~/assets/lib/leaflet/leaflet.css",
                    "~/assets/lib/leaflet.markercluster/MarkerCluster.css",
                    "~/assets/lib/leaflet.markercluster/MarkerCluster.Default.css",

                     "~/assets/lib/select2/select2.min.css",
                     "~/assets/lib/flatpickr/flatpickr.min.css",
                     "~/assets/lib/dropzone/dropzone.min.css",
                     "~/assets/lib/fancybox/jquery.fancybox.min.css",
                     "~/assets/lib/emojionearea/emojionearea.min.css",
                "~/assets/css/theme.css"
                    ));

            BundleTable.EnableOptimizations = true;

        }
    }
}
