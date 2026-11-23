
using System;
using System.Web;
using System.Web.Mvc;

namespace Presentacion1.Filters
{
    public class AutorizacionAttribute : AuthorizeAttribute
    {
        private readonly string[] perfilesPermitidos;

        public AutorizacionAttribute(params string[] perfiles)
        {
            perfilesPermitidos = perfiles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["Usuario"] == null)
                return false;

            string perfil = httpContext.Session["Perfil"] as string;

            if (string.IsNullOrEmpty(perfil))
                return false;

            // Si no se especifican perfiles, alcanza con estar autenticado.
            if (perfilesPermitidos == null || perfilesPermitidos.Length == 0)
                return true;

            foreach (string perfilPermitido in perfilesPermitidos)
            {
                if (string.Equals(perfil, perfilPermitido,
                    StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(
            AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["Usuario"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Account" },
                        { "action", "Login" }
                    });
            }
            else
            {
                filterContext.Result = new HttpStatusCodeResult(403);
            }
        }
    }
}
