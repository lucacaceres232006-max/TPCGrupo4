
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dominio;
using Negocio;
using Presentacion1.Filters;

namespace Presentacion.Controllers
{
    public class SociosController : Controller
    {
        [Autorizacion("Usuario", "Bibliotecario", "Administrador")]
        public ActionResult Index(string buscar)
        {
            SocioNegocio negocio = new SocioNegocio();
            List<Socio> lista = negocio.listar();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                lista = lista
                    .Where(s =>
                        (!string.IsNullOrEmpty(s.Nombre) && s.Nombre.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrEmpty(s.Apellido) && s.Apellido.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrEmpty(s.DNI) && s.DNI.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }

            ViewBag.Buscar = buscar;
            return View(lista);
        }

        [Autorizacion("Bibliotecario", "Administrador")]
        public ActionResult Create()
        {
            cargarPoliticas();
            Socio modelo = new Socio { Habilitado = true };
            return View(modelo);
        }

        [HttpPost]
        [Autorizacion("Bibliotecario", "Administrador")]
        public ActionResult Create(Socio nuevo, int idPolitica)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nuevo.Nombre) || string.IsNullOrWhiteSpace(nuevo.Apellido) || string.IsNullOrWhiteSpace(nuevo.DNI))
                    throw new Exception("El nombre, apellido y DNI son obligatorios.");

                nuevo.PoliticaPrestamo = new PoliticaPrestamo { Id = idPolitica };

                SocioNegocio negocio = new SocioNegocio();
                negocio.agregar(nuevo);

                TempData["Exito"] = "Socio registrado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                cargarPoliticas();
                return View(nuevo);
            }
        }

        [Autorizacion("Bibliotecario", "Administrador")]
        public ActionResult Edit(int id)
        {
            SocioNegocio negocio = new SocioNegocio();
            Socio socio = negocio.obtenerPorId(id);

            if (socio == null)
                return HttpNotFound();

            cargarPoliticas();
            return View(socio);
        }

        [HttpPost]
        [Autorizacion("Bibliotecario", "Administrador")]
        public ActionResult Edit(Socio socio, int idPolitica)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(socio.Nombre) || string.IsNullOrWhiteSpace(socio.Apellido) || string.IsNullOrWhiteSpace(socio.DNI))
                    throw new Exception("El nombre, apellido y DNI son obligatorios.");

                socio.PoliticaPrestamo = new PoliticaPrestamo { Id = idPolitica };

                SocioNegocio negocio = new SocioNegocio();
                negocio.modificar(socio);

                TempData["Exito"] = "Datos del socio actualizados correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                cargarPoliticas();
                return View(socio);
            }
        }

        [HttpPost]
        [Autorizacion("Bibliotecario", "Administrador")]
        public ActionResult CambiarEstado(int id, bool habilitado)
        {
            try
            {
                SocioNegocio negocio = new SocioNegocio();
                negocio.cambiarEstado(id, habilitado);

                TempData["Exito"] = habilitado
                    ? "Socio habilitado correctamente."
                    : "Socio deshabilitado correctamente. No podrá retirar nuevos préstamos.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        private void cargarPoliticas()
        {
            PoliticaPrestamoNegocio politicaNegocio = new PoliticaPrestamoNegocio();
            ViewBag.Politicas = politicaNegocio.listar();
        }
    }
}
