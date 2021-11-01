using ProvenCfoUI.Comman;
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
