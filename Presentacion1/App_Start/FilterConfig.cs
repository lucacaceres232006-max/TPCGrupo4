
using System.Web.Mvc;
using Presentacion1.Filters;

namespace Presentacion1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AutenticacionAttribute());
        }
    }
}
