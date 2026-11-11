
using System;
using System.Web.Mvc;
using Dominio;
using Negocio;

namespace Presentacion1.Controllers
{
    public class UsuariosController : Controller
    {
        private UsuarioNegocio negocio = new UsuarioNegocio();

        public ActionResult Index()
        {
            return View(negocio.listar());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Usuario usuario)
        {
            try
            {
                usuario.Activo = true;
                negocio.agregar(usuario);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(usuario);
            }
        }

        public ActionResult Edit(int id)
        {
            Usuario usuario = negocio.obtenerPorId(id);

            if (usuario == null)
                return HttpNotFound();

            return View(usuario);
        }

        [HttpPost]
        public ActionResult Edit(Usuario usuario)
        {
            try
            {
                negocio.modificar(usuario);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(usuario);
            }
        }

        public ActionResult CambiarEstado(int id, bool activo)
        {
            negocio.cambiarEstado(id, activo);
            return RedirectToAction("Index");
        }
    }
}
