
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Presentacion1.Filters
{
    // Autenticación general: exige una sesión iniciada.
    public class AutenticacionAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session["Usuario"] != null;
        }

        protected override void HandleUnauthorizedRequest(
            AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Account" },
                    { "action", "Login" }
                });
        }
    }

    // Autorización por perfil: conserva los permisos anteriores.
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
                    new RouteValueDictionary
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
