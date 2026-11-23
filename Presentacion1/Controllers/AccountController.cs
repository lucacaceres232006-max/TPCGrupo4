
using System.Web.Mvc;
using Dominio;
using Negocio;

namespace Presentacion1.Controllers
{
    public class AccountController : Controller
    {
        private UsuarioNegocio negocio = new UsuarioNegocio();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string nombreUsuario, string clave)
        {
            Usuario usuario = negocio.validarUsuario(nombreUsuario, clave);

            if (usuario == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos, o usuario inactivo.";
                return View();
            }

            Session["Usuario"] = usuario;
            Session["Perfil"] = usuario.Perfil;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Account");
        }
    }
}
