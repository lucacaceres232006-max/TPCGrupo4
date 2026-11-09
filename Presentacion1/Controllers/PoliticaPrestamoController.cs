using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Dominio;
using Negocio;

namespace Presentacion.Controllers
{
    public class PoliticaPrestamoController : Controller
    {
        public ActionResult Index()
        {
            PoliticaPrestamoNegocio negocio = new PoliticaPrestamoNegocio();
            List<PoliticaPrestamo> lista = negocio.listar();

            return View(lista);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(PoliticaPrestamo nueva)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nueva.Descripcion))
                    throw new Exception("La descripción de la política es obligatoria.");

                if (nueva.DiasDuracion <= 0)
                    throw new Exception("Los días de duración deben ser mayores a cero.");

                if (nueva.CantidadMaximaEjemplares <= 0)
                    throw new Exception("La cantidad máxima de ejemplares debe ser mayor a cero.");

                PoliticaPrestamoNegocio negocio = new PoliticaPrestamoNegocio();
                negocio.agregar(nueva);

                TempData["Exito"] = "Política de préstamo registrada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(nueva);
            }
        }

        public ActionResult Edit(int id)
        {
            PoliticaPrestamoNegocio negocio = new PoliticaPrestamoNegocio();
            PoliticaPrestamo seleccionada = negocio.obtenerPorId(id);

            if (seleccionada == null)
                return HttpNotFound();

            return View(seleccionada);
        }

        [HttpPost]
        public ActionResult Edit(PoliticaPrestamo politica)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(politica.Descripcion))
                    throw new Exception("La descripción de la política es obligatoria.");

                if (politica.DiasDuracion <= 0)
                    throw new Exception("Los días de duración deben ser mayores a cero.");

                if (politica.CantidadMaximaEjemplares <= 0)
                    throw new Exception("La cantidad máxima de ejemplares debe ser mayor a cero.");

                PoliticaPrestamoNegocio negocio = new PoliticaPrestamoNegocio();
                negocio.modificar(politica);

                TempData["Exito"] = "Política de préstamo actualizada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(politica);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                PoliticaPrestamoNegocio negocio = new PoliticaPrestamoNegocio();
                negocio.eliminar(id);

                TempData["Exito"] = "Política eliminada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}