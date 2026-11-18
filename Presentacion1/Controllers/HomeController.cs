
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio;
using Negocio;

namespace Presentacion1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string buscar, string filtroEstado)
        {
            PrestamoNegocio negocio = new PrestamoNegocio();
            List<Prestamo> lista = negocio.listar();

            if (string.IsNullOrWhiteSpace(filtroEstado))
                filtroEstado = "Activos";

            if (filtroEstado == "Activos")
            {
                lista = lista.Where(p => p.Estado == "Activo" && p.FechaPrevistaDevolucion.Date >= DateTime.Today).ToList();
            }
            else if (filtroEstado == "Vencidos")
            {
                lista = lista.Where(p => p.Estado == "Activo" && p.FechaPrevistaDevolucion.Date < DateTime.Today).ToList();
            }
            else if (filtroEstado == "Devueltos")
            {
                lista = lista.Where(p => p.Estado == "Devuelto").ToList();
            }
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                lista = lista.Where(p =>
                    (!string.IsNullOrEmpty(p.NumeroComprobante) && p.NumeroComprobante.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (p.Socio != null && (
                        p.Socio.DNI.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        p.Socio.Apellido.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        p.Socio.Nombre.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0)) ||
                    (p.Ejemplar != null && (
                        p.Ejemplar.CodigoInventario.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (!string.IsNullOrEmpty(p.Ejemplar.TituloLibro) && p.Ejemplar.TituloLibro.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0)))
                ).ToList();
            }

            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            return View(lista);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
