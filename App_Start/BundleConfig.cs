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

            bundles.Add(new ScriptBundle("~/bundles/customjquery").Include(
             "~/Scripts/jquery-{version}.js",
             "~/assets/js/jquery.min.js",
             "~/assets/js/popper.min.js",
             "~/assets/js/bootstrap.min.js",
             "~/assets/lib/moment.js/moment.min.js",
             "~/assets/lib/fortawesome/all.min.js",
             "~/assets/lib/stickyfilljs/stickyfill.min.js",
             "~/assets/lib/sticky-kit/sticky-kit.min.js",
             "~/assets/lib/is_js/is.min.js",
             "~/assets/lib/lodash/lodash.min.js",
             "~/assets/lib/perfect-scrollbar/perfect-scrollbar.js",
             "~/assets/lib/anchor.min.js",
             "~/assets/lib/prismjs/prism.js",
             "~/assets/lib/flatpickr/flatpickr.min.js",
             "~/assets/lib/emojionearea/emojionearea.min.js",
             "~/assets/lib/jquery.fancybox.min.js",
             "~/assets/js/theme.js",
             "~/assets/lib/progressbar.js/progressbar.min.js",
             "~/assets/lib/select2/select2.min.js",
              "~/assets/js/wrapper.js"
    ));

            bundles.Add(new ScriptBundle("~/bundles/js/select").Include(
               "~/assets/lib/select2/select2.min.js"
               ));
            bundles.Add(new ScriptBundle("~/bundles/js/chart").Include(
               "~/assets/lib/echarts/echarts.min.js",
               "~/assets/lib/flatpickr/flatpickr.min.js"
               //"~/assets/lib/chart.js/Chart.bundle.min.js",
               //"~/Content/js/theme/Utils.js"
               //"~/assets/js/utils.js"
               ));
            bundles.Add(new ScriptBundle("~/bundles/js/table").Include(
              "~/assets/lib/datatables/js/jquery.dataTables.min.js",
              "~/assets/lib/datatables-bs4/dataTables.bootstrap4.min.js",
              "~/assets/lib/datatables.net-responsive/dataTables.responsive.js",
              "~/assets/lib/datatables.net-responsive-bs4/responsive.bootstrap4.js",
              "~/assets/lib/leaflet/leaflet.js",
              "~/assets/lib/leaflet.markercluster/leaflet.markercluster.js",
              "~/assets/lib/leaflet.tilelayer.colorfilter/leaflet-tilelayer-colorfilter.min.js"
              ));
            bundles.Add(new ScriptBundle("~/bundles/js/kanban").Include(
             "~/assets/lib/shopify-draggable/draggable.bundle.js",
             "~/assets/lib/lightbox2/js/lightbox.min.js",
             "~/assets/lib/fancybox/jquery.fancybox.min.js",
             "~/assets/lib/prismjs/prism.js",
             "~/assets/lib/list.min.js",
             "~/assets/lib/flatpickr/flatpickr.min.js",
             "~/assets/lib/dropzone/dropzone.min.js"
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

            bundles.Add(new Bundle("~/bundles/js/reports").Include("~/assets/js/reports.js"));
            #region FileUploader

            bundles.Add(new ScriptBundle("~/bundles/jQuery-File-Upload").Include(
                    //<!-- The Templates plugin is included to render the upload/download listings -->
                    "~/assets/lib/blueimp-file-upload/js/vendor/jquery.ui.widget.min.js",
                    //"~/assets/lib/blueimp-file-upload/js/tmpl.min.js",
                    //<!-- The Load Image plugin is included for the preview images and image resizing functionality -->
                    //"~/assets/lib/blueimp-file-upload/js/load-image.all.min.js",
                    //<!-- The Canvas to Blob plugin is included for image resizing functionality -->
                    //"~/assets/lib/blueimp-file-upload/js/canvas-to-blob.min.js",
                    //"~/Scripts/file-upload/jquery.blueimp-gallery.min.js",
                    //<!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
                    "~/assets/lib/blueimp-file-upload/js/jquery.iframe-transport.min.js",
                    //<!-- The basic File Upload plugin -->
                    "~/assets/lib/blueimp-file-upload/js/jquery.fileupload.min.js",
                    //<!-- The File Upload processing plugin -->
                    "~/assets/lib/blueimp-file-upload/js/jquery.fileupload-process.min.js",
                    //<!-- The File Upload image preview & resize plugin -->
                    "~/assets/lib/blueimp-file-upload/js/jquery.fileupload-image.min.js",
                    //<!-- The File Upload audio preview plugin -->
                    "~/assets/lib/blueimp-file-upload/js/jquery.fileupload-audio.min.js",
                    //<!-- The File Upload video preview plugin -->
                    "~/assets/lib/blueimp-file-upload/js/jquery.fileupload-video.min.js",
                    //<!-- The File Upload validation plugin -->
                    "~/assets/lib/blueimp-file-upload/js/jquery.fileupload-validate.min.js",
                    //!-- The File Upload user interface plugin -->
                    "~/assets/lib/blueimp-file-upload/js/jquery.fileupload-ui.min.js",
                    //Blueimp Gallery 2 
                    "~/assets/lib/blueimp-gallery/js/blueimp-gallery.min.js",
                    "~/assets/lib/blueimp-gallery/js/blueimp-gallery-video.min.js",
                    "~/assets/lib/blueimp-gallery/js/blueimp-gallery-indicator.min.js",
                    "~/assets/lib/blueimp-gallery/js/jquery.blueimp-gallery.min.js"

));

            bundles.Add(new ScriptBundle("~/bundles/Blueimp-Gallerry2").Include(//Blueimp Gallery 2 
                        "~/assets/lib/blueimp-gallery/js/blueimp-gallery.min.js",
                        "~/assets/lib/blueimp-gallery/js/blueimp-gallery-video.min.js",
                        "~/assets/lib/blueimp-gallery/js/blueimp-gallery-indicator.min.js",
                        "~/assets/lib/blueimp-gallery/js/jquery.blueimp-gallery.min.js"));

            #endregion
            //Css Start

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/theme").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/site.css"));

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
                     "~/assets/lib/emojionearea/emojionearea.min.css",
                     "~/assets/css/theme.css"
                    ));

            bundles.Add(new StyleBundle("~/bundles/css/kanban").Include(
                    "~/assets/lib/dropzone/dropzone.min.css",
                    "~/assets/lib/fancybox/jquery.fancybox.min.css",
                     "~/Style/NeedStyle.css"
                   ));

            #region FileUploader
            bundles.Add(new StyleBundle("~/Content/jQuery-File-Upload").Include(
                    "~/assets/lib/blueimp-file-upload/css/jquery.fileupload.min.css",
                   "~/assets/lib/blueimp-file-upload/css/jquery.fileupload-ui.min.css",
                   "~/assets/lib/blueimp-gallery/css/blueimp-gallery.css",
                    "~/assets/lib/blueimp-gallery/css/blueimp-gallery-video.css",
                    "~/assets/lib/blueimp-gallery/css/blueimp-gallery-indicator.css"
                   ));
            #endregion

            BundleTable.EnableOptimizations = false;

        }
    }
}
