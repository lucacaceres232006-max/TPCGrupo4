
using System.Linq;
using System.Web.Mvc;
using Negocio;

namespace Presentacion1.Controllers
{
    public class EjemplaresController : Controller
    {
        public ActionResult Index(string buscar)
        {
            EjemplarNegocio negocio = new EjemplarNegocio();

            var lista = negocio.listar();


            if (!string.IsNullOrWhiteSpace(buscar))
            {
                lista = lista
                    .Where(e =>
                        (!string.IsNullOrEmpty(e.CodigoInventario) &&
                         e.CodigoInventario.IndexOf(
                             buscar,
                             System.StringComparison.OrdinalIgnoreCase) >= 0)
                        ||
                        (!string.IsNullOrEmpty(e.TituloLibro) &&
                         e.TituloLibro.IndexOf(
                             buscar,
                             System.StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }


            return View(lista);
        }
    }
}
