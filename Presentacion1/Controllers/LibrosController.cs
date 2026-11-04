using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Dominio;
using Negocio;

namespace Presentacion1.Controllers
{
    public class LibrosController : Controller
    {

        public ActionResult Index(string buscar)
        {
            LibroNegocio negocio = new LibroNegocio();
            List<Libro> lista = negocio.listar();

            if (!string.IsNullOrEmpty(buscar))
            {
                lista = lista.FindAll(x =>
                    x.Titulo.ToLower().Contains(buscar.ToLower()));
            }

            ViewBag.Buscar = buscar;

            return View(lista);
        }


        public ActionResult Create()
        {
            EditorialNegocio editorialNegocio = new EditorialNegocio();
            AutorNegocio autorNegocio = new AutorNegocio();
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();

            ViewBag.Editoriales = editorialNegocio.listar();
            ViewBag.Autores = autorNegocio.listar();
            ViewBag.Categorias = categoriaNegocio.listar();

            return View();
        }
        [HttpPost]
        public ActionResult Create(
    string Titulo,
    string ISBN,
    int IdEditorial,
    int AnioEdicion,
    int[] IdsAutores,
    int[] IdsCategorias)
        {
            try
            {
                Libro libro = new Libro();

                libro.Titulo = Titulo;
                libro.ISBN = ISBN;
                libro.AnioEdicion = AnioEdicion;

                libro.Editorial = new Editorial();
                libro.Editorial.Id = IdEditorial;

                AutorNegocio autorNegocio = new AutorNegocio();
                CategoriaNegocio categoriaNegocio = new CategoriaNegocio();

                foreach (int id in IdsAutores)
                {
                    libro.Autores.Add(autorNegocio.obtenerPorId(id));
                }

                foreach (int id in IdsCategorias)
                {
                    libro.Categorias.Add(categoriaNegocio.obtenerPorId(id));
                }

                LibroNegocio negocio = new LibroNegocio();
                negocio.agregar(libro);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

                EditorialNegocio editorialNegocio = new EditorialNegocio();
                AutorNegocio autorNegocio = new AutorNegocio();
                CategoriaNegocio categoriaNegocio = new CategoriaNegocio();

                ViewBag.Editoriales = editorialNegocio.listar();
                ViewBag.Autores = autorNegocio.listar();
                ViewBag.Categorias = categoriaNegocio.listar();

                return View();
            }
        }
        public ActionResult Edit(int id)
        {
            LibroNegocio negocio = new LibroNegocio();
            List<Libro> lista = negocio.listar();

            Libro libro = lista.Find(x => x.Id == id);

            if (libro == null)
                return HttpNotFound();

            EditorialNegocio editorialNegocio = new EditorialNegocio();
            AutorNegocio autorNegocio = new AutorNegocio();
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();

            ViewBag.Editoriales = editorialNegocio.listar();
            ViewBag.Autores = autorNegocio.listar();
            ViewBag.Categorias = categoriaNegocio.listar();

            return View(libro);
        }
        [HttpPost]
        public ActionResult Edit(
    int Id,
    string Titulo,
    string ISBN,
    int IdEditorial,
    int AnioEdicion,
    int[] IdsAutores,
    int[] IdsCategorias)
        {
            try
            {
                Libro libro = new Libro();

                libro.Id = Id;
                libro.Titulo = Titulo;
                libro.ISBN = ISBN;
                libro.AnioEdicion = AnioEdicion;

                libro.Editorial = new Editorial();
                libro.Editorial.Id = IdEditorial;

                AutorNegocio autorNegocio = new AutorNegocio();
                CategoriaNegocio categoriaNegocio = new CategoriaNegocio();

                foreach (int id in IdsAutores)
                {
                    libro.Autores.Add(autorNegocio.obtenerPorId(id));
                }

                foreach (int id in IdsCategorias)
                {
                    libro.Categorias.Add(categoriaNegocio.obtenerPorId(id));
                }

                LibroNegocio negocio = new LibroNegocio();
                negocio.modificar(libro);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

                EditorialNegocio editorialNegocio = new EditorialNegocio();
                AutorNegocio autorNegocio = new AutorNegocio();
                CategoriaNegocio categoriaNegocio = new CategoriaNegocio();

                ViewBag.Editoriales = editorialNegocio.listar();
                ViewBag.Autores = autorNegocio.listar();
                ViewBag.Categorias = categoriaNegocio.listar();

           
                Libro libro = new Libro
                {
                    Id = Id,
                    Titulo = Titulo,
                    ISBN = ISBN,
                    AnioEdicion = AnioEdicion,
                    Editorial = new Editorial { Id = IdEditorial }
                };

                return View(libro);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                LibroNegocio negocio = new LibroNegocio();
                negocio.eliminar(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public ActionResult Details(int id)
        {
            LibroNegocio negocio = new LibroNegocio();

            List<Libro> lista = negocio.listar();

            Libro libro = lista.Find(x => x.Id == id);

            if (libro == null)
                return HttpNotFound();

            return View(libro);
        }


    }
}