using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio;
using Negocio;

namespace Presentacion.Controllers
{
    public class CategoriaController : Controller
    {
        // GET: Categoria
        public ActionResult Index()
        {
            CategoriaNegocio negocio = new CategoriaNegocio();
            List<Categoria> lista = negocio.listar();
            return View(lista);
        }

        // GET: Categoria/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Categoria nueva)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nueva.Descripcion))
                {
                    ViewBag.Error = "El nombre del género es obligatorio.";
                    return View(nueva);
                }

                CategoriaNegocio negocio = new CategoriaNegocio();
                negocio.agregar(nueva);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al registrar el género: " + ex.Message;
                return View(nueva);
            }
        }

        public ActionResult Edit(int id)
        {
            CategoriaNegocio negocio = new CategoriaNegocio();
            Categoria seleccionada = negocio.obtenerPorId(id);

            if (seleccionada == null)
                return RedirectToAction("Index");

            return View(seleccionada);
        }

        [HttpPost]
        public ActionResult Edit(Categoria categoria)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoria.Descripcion))
                {
                    ViewBag.Error = "El nombre del género es obligatorio.";
                    return View(categoria);
                }

                CategoriaNegocio negocio = new CategoriaNegocio();
                negocio.modificar(categoria);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al modificar el género: " + ex.Message;
                return View(categoria);
            }
        }

        public ActionResult Delete(int id)
        {
            CategoriaNegocio negocio = new CategoriaNegocio();
            Categoria seleccionada = negocio.obtenerPorId(id);

            if (seleccionada == null)
                return RedirectToAction("Index");

            return View(seleccionada);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CategoriaNegocio negocio = new CategoriaNegocio();
                negocio.eliminar(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                CategoriaNegocio negocio = new CategoriaNegocio();
                Categoria seleccionada = negocio.obtenerPorId(id);
                ViewBag.Error = "No se puede eliminar el género porque tiene libros asociados en el catálogo. Detalle: " + ex.Message;
                return View(seleccionada);
            }
        }
    }
}