
using System.Collections.Generic;
using System.Web.Mvc;
using Dominio;
using Negocio;

namespace Presentacion1.Controllers
{
    public class LibrosController : Controller
    {
        public ActionResult Index()
        {
            LibroNegocio negocio = new LibroNegocio();
            List<Libro> lista = negocio.listar();

            return View(lista);
        }
    }
}
