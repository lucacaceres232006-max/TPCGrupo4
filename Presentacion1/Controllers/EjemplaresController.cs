using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dominio;
using Negocio;

namespace Presentacion1.Controllers
{
    public class EjemplaresController : Controller
    {
        public ActionResult Index(string buscar, string estado)
        {
            EjemplarNegocio negocio = new EjemplarNegocio();
            var lista = negocio.listar();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                lista = lista
                    .Where(e =>
                        (!string.IsNullOrEmpty(e.CodigoInventario) &&
                         e.CodigoInventario.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0)
                        ||
                        (!string.IsNullOrEmpty(e.TituloLibro) &&
                         e.TituloLibro.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                lista = lista
                    .Where(e => string.Equals(e.Estado, estado, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.Buscar = buscar;
            ViewBag.EstadoSeleccionado = estado;

            return View(lista);
        }

        public ActionResult Create(int? idLibro)
        {
            cargarLibros();
            Ejemplar modelo = new Ejemplar
            {
                IdLibro = idLibro ?? 0,
                Estado = "Disponible"
            };
            return View(modelo);
        }

        [HttpPost]
        public ActionResult Create(Ejemplar nuevo)
        {
            try
            {
                if (nuevo.IdLibro <= 0)
                    throw new Exception("Debés seleccionar el libro al que pertenece el ejemplar.");

                EjemplarNegocio negocio = new EjemplarNegocio();
                negocio.agregar(nuevo);

                TempData["Exito"] = "Ejemplar registrado correctamente con el código de inventario: " + nuevo.CodigoInventario;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                cargarLibros();
                return View(nuevo);
            }
        }

        public ActionResult Edit(int id)
        {
            EjemplarNegocio negocio = new EjemplarNegocio();
            Ejemplar ejemplar = negocio.obtenerPorId(id);

            if (ejemplar == null)
                return HttpNotFound();

            cargarLibros();
            return View(ejemplar);
        }

        [HttpPost]
        public ActionResult Edit(Ejemplar ejemplar)
        {
            try
            {
                if (ejemplar.IdLibro <= 0)
                    throw new Exception("Debés seleccionar un libro válido.");

                if (string.IsNullOrWhiteSpace(ejemplar.CodigoInventario))
                    throw new Exception("El código de inventario es obligatorio.");

                EjemplarNegocio negocio = new EjemplarNegocio();
                Ejemplar actual = negocio.obtenerPorId(ejemplar.Id);

                if (actual == null)
                    return HttpNotFound();

                if (actual.Estado == "Prestado" && ejemplar.Estado != "Prestado")
                    throw new Exception("Este ejemplar se encuentra actualmente Prestado. Para cambiar su disponibilidad, registrá primero la devolución o extravío en la sección de Préstamos.");

                if (actual.Estado != "Prestado" && ejemplar.Estado == "Prestado")
                    throw new Exception("No se puede marcar como Prestado manualmente. Registrá la operación desde el módulo de Préstamos.");

                negocio.modificar(ejemplar);
                TempData["Exito"] = "Ejemplar " + ejemplar.CodigoInventario + " actualizado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                cargarLibros();
                return View(ejemplar);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                EjemplarNegocio negocio = new EjemplarNegocio();
                negocio.eliminar(id);
                TempData["Exito"] = "Ejemplar eliminado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        private void cargarLibros()
        {
            LibroNegocio libroNegocio = new LibroNegocio();
            ViewBag.Libros = libroNegocio.listar();
        }
    }
}