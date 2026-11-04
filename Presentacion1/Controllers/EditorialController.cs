
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Dominio;
using Negocio;

namespace Presentacion1.Controllers
{
    public class EditorialController : Controller
    {
        public ActionResult Index()
        {
            EditorialNegocio negocio = new EditorialNegocio();
            List<Editorial> lista = negocio.listar();

            return View(lista);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Editorial nueva)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nueva.Nombre))
                {
                    ViewBag.Error = "El nombre de la editorial es obligatorio.";
                    return View(nueva);
                }

                EditorialNegocio negocio = new EditorialNegocio();
                negocio.agregar(nueva);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al guardar la editorial: " + ex.Message;
                return View(nueva);
            }
        }

        public ActionResult Edit(int id)
        {
            EditorialNegocio negocio = new EditorialNegocio();
            Editorial seleccionada = negocio.obtenerPorId(id);

            if (seleccionada == null)
                return RedirectToAction("Index");

            return View(seleccionada);
        }

        [HttpPost]
        public ActionResult Edit(Editorial editorial)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(editorial.Nombre))
                {
                    ViewBag.Error = "El nombre de la editorial es obligatorio.";
                    return View(editorial);
                }

                EditorialNegocio negocio = new EditorialNegocio();
                negocio.modificar(editorial);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al modificar la editorial: " + ex.Message;
                return View(editorial);
            }
        }

        public ActionResult Delete(int id)
        {
            EditorialNegocio negocio = new EditorialNegocio();
            Editorial seleccionada = negocio.obtenerPorId(id);

            if (seleccionada == null)
                return RedirectToAction("Index");

            return View(seleccionada);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                EditorialNegocio negocio = new EditorialNegocio();
                negocio.eliminar(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                EditorialNegocio negocio = new EditorialNegocio();
                Editorial seleccionada = negocio.obtenerPorId(id);

                ViewBag.Error = "No se puede eliminar la editorial porque tiene libros asociados. Detalle: " + ex.Message;

                return View("Delete", seleccionada);
            }
        }
    }
}
