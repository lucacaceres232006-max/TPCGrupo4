
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

            var prestamos = negocio.listar();

            // Buscar por nombre, apellido o DNI del socio.
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                prestamos = prestamos.Where(p =>
                    p.Socio != null &&
                    (
                        (p.Socio.Nombre ?? "").IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (p.Socio.Apellido ?? "").IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (p.Socio.DNI ?? "").IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0
                    )
                ).ToList();
            }

            // Filtrar por estado.
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

                TempData["Exito"] =
                    "Préstamo registrado correctamente. Comprobante: "
                    + nuevo.NumeroComprobante;

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

        private void cargarDatos()
        {
            SocioNegocio socioNegocio = new SocioNegocio();
            EjemplarNegocio ejemplarNegocio = new EjemplarNegocio();

            ViewBag.Socios = socioNegocio.listar()
                .Where(s => s.Habilitado)
                .ToList();

            ViewBag.Ejemplares = ejemplarNegocio.listar()
                .Where(e => e.Estado == "Disponible")
                .ToList();
        }
    }
}
