using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio;
using Negocio;

namespace Presentacion.Controllers
{
    public class AutorController : Controller
    {
        // GET: Autor
        public ActionResult Index()
        {
            AutorNegocio negocio = new AutorNegocio();
            List<Autor> lista = negocio.listar();
            return View(lista);
        }

        // GET: Autor/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Autor nuevo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nuevo.Nombre) || string.IsNullOrWhiteSpace(nuevo.Apellido))
                {
                    ViewBag.Error = "El nombre y el apellido son obligatorios.";
                    return View(nuevo);
                }

                AutorNegocio negocio = new AutorNegocio();
                negocio.agregar(nuevo);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al guardar el autor: " + ex.Message;
                return View(nuevo);
            }
        }

        public ActionResult Edit(int id)
        {
            AutorNegocio negocio = new AutorNegocio();
            Autor seleccionado = negocio.obtenerPorId(id);

            if (seleccionado == null)
                return RedirectToAction("Index");

            return View(seleccionado);
        }

        [HttpPost]
        public ActionResult Edit(Autor autor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(autor.Nombre) || string.IsNullOrWhiteSpace(autor.Apellido))
                {
                    ViewBag.Error = "El nombre y el apellido son obligatorios.";
                    return View(autor);
                }

                AutorNegocio negocio = new AutorNegocio();
                negocio.modificar(autor);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al modificar el autor: " + ex.Message;
                return View(autor);
            }

        }

        [HttpPost]
        public ActionResult Desactivar(int id)
        {
            AutorNegocio negocio = new AutorNegocio();
            negocio.eliminar(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Activar(int id)
        {
            AutorNegocio negocio = new AutorNegocio();
            negocio.activar(id);

            return RedirectToAction("Index");
        }

    }
}