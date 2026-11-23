using System;
using System.Linq;
using System.Web.Mvc;
using Dominio;
using Negocio;

namespace Presentacion1.Controllers
{
    public class PrestamosController : Controller
    {
        public ActionResult Index(string buscar, string estado)
        {
            PrestamoNegocio negocio = new PrestamoNegocio();
            PoliticaPrestamoNegocio politicaNegocio = new PoliticaPrestamoNegocio();

            var prestamos = negocio.listar();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                prestamos = prestamos.Where(p =>
                    (!string.IsNullOrEmpty(p.NumeroComprobante) && p.NumeroComprobante.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (p.Socio != null && (
                        (p.Socio.Nombre ?? "").IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (p.Socio.Apellido ?? "").IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (p.Socio.DNI ?? "").IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0
                    )) ||
                    (p.Ejemplar != null && (
                        (p.Ejemplar.CodigoInventario ?? "").IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (p.Ejemplar.TituloLibro ?? "").IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0
                    ))
                ).ToList();
            }

            if (estado == "Activo")
            {
                prestamos = prestamos.Where(p =>
                    p.Estado == "Activo" &&
                    p.FechaPrevistaDevolucion.Date >= DateTime.Today
                ).ToList();
            }
            else if (estado == "Vencido")
            {
                prestamos = prestamos.Where(p =>
                    p.Estado == "Activo" &&
                    p.FechaPrevistaDevolucion.Date < DateTime.Today
                ).ToList();
            }
            else if (estado == "Devuelto")
            {
                prestamos = prestamos.Where(p =>
                    p.Estado == "Devuelto"
                ).ToList();
            }

            ViewBag.Buscar = buscar;
            ViewBag.Estado = estado;
            ViewBag.Politicas = politicaNegocio.listar();

            return View(prestamos);
        }

        public ActionResult Create()
        {
            cargarDatos();
            return View(new Prestamo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int idSocio, int idEjemplar)
        {
            try
            {
                if (idSocio <= 0)
                    throw new Exception("Debés seleccionar un socio.");

                if (idEjemplar <= 0)
                    throw new Exception("Debés seleccionar un ejemplar.");

                Prestamo nuevo = new Prestamo
                {
                    Socio = new Socio { Id = idSocio },
                    Ejemplar = new Ejemplar { Id = idEjemplar }
                };

                PrestamoNegocio negocio = new PrestamoNegocio();
                negocio.agregar(nuevo);

                TempData["Exito"] = "Préstamo registrado correctamente. Comprobante: " + nuevo.NumeroComprobante;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                cargarDatos();

                return View(new Prestamo
                {
                    Socio = new Socio { Id = idSocio },
                    Ejemplar = new Ejemplar { Id = idEjemplar }
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Devolver(int id, string estadoEjemplar, string observaciones)
        {
            try
            {
                PrestamoNegocio negocio = new PrestamoNegocio();
                negocio.devolver(id, estadoEjemplar, observaciones);

                string estadoTexto = string.IsNullOrWhiteSpace(estadoEjemplar) ? "Disponible" : estadoEjemplar;
                TempData["Exito"] = "La devolución se registró correctamente. El ejemplar quedó en estado: " + estadoTexto + ".";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizarVencido(
            int id,
            string estadoEjemplar,
            string accionSocio,
            int? idNuevaPolitica,
            int diasDescuento,
            string observaciones)
        {
            try
            {
                PrestamoNegocio negocio = new PrestamoNegocio();
                negocio.finalizarVencido(id, estadoEjemplar, accionSocio, idNuevaPolitica, diasDescuento, observaciones);

                TempData["Exito"] = "Préstamo vencido finalizado correctamente. Ejemplar en estado: " + estadoEjemplar + ".";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Extender(int id)
        {
            try
            {
                PrestamoNegocio negocio = new PrestamoNegocio();
                negocio.extender(id);

                TempData["Exito"] = "El préstamo se extendió correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        private void cargarDatos()
        {
            SocioNegocio socioNegocio = new SocioNegocio();
            EjemplarNegocio ejemplarNegocio = new EjemplarNegocio();
            PrestamoNegocio prestamoNegocio = new PrestamoNegocio();

            var sociosHabilitados = socioNegocio.listar()
                .Where(s => s.Habilitado)
                .ToList();

            var prestamosActivos = prestamoNegocio.listar()
                .Where(p => p.Estado == "Activo" && p.Socio != null)
                .GroupBy(p => p.Socio.Id)
                .ToDictionary(g => g.Key, g => g.Count());

            ViewBag.Socios = sociosHabilitados;
            ViewBag.SociosDetalle = sociosHabilitados.Select(s => new
            {
                s.Id,
                s.Nombre,
                s.Apellido,
                s.DNI,
                Politica = s.PoliticaPrestamo != null ? s.PoliticaPrestamo.Descripcion : "Sin política",
                MaxPrestamos = s.PoliticaPrestamo != null ? s.PoliticaPrestamo.CantidadMaximaEjemplares : 0,
                DiasDuracion = s.PoliticaPrestamo != null ? s.PoliticaPrestamo.DiasDuracion : 0,
                PrestamosActivos = prestamosActivos.ContainsKey(s.Id) ? prestamosActivos[s.Id] : 0
            }).ToList();

            ViewBag.Ejemplares = ejemplarNegocio.listar()
                .Where(e => e.Estado == "Disponible")
                .ToList();
        }
    }
}