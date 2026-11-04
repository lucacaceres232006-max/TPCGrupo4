using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Dominio;
using Negocio;

namespace Presentacion1.Controllers
{
    public class LibrosController : Controller
    {
        // GET: Libros
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

        // GET: Libros/Create
        public ActionResult Create()
        {
            cargarListasDesplegables();
            return View(new Libro());
        }

        // POST: Libros/Create
        [HttpPost]
        public ActionResult Create(
            string Titulo,
            string ISBN,
            int? IdEditorial,
            int? AnioEdicion,
            int[] IdsAutores,
            int[] IdsCategorias)
        {
            Libro libro = new Libro
            {
                Titulo = Titulo,
                ISBN = ISBN,
                AnioEdicion = AnioEdicion ?? 0,
                Editorial = new Editorial { Id = IdEditorial ?? 0 }
            };

            try
            {
                if (IdsAutores != null)
                {
                    foreach (int id in IdsAutores)
                        libro.Autores.Add(new Autor { Id = id });
                }

                if (IdsCategorias != null)
                {
                    foreach (int id in IdsCategorias)
                        libro.Categorias.Add(new Categoria { Id = id });
                }

                if (string.IsNullOrWhiteSpace(Titulo) || string.IsNullOrWhiteSpace(ISBN))
                    throw new Exception("El título y el ISBN son obligatorios.");

                if (IdEditorial == null || IdEditorial <= 0)
                    throw new Exception("Debés seleccionar una editorial.");

                if (libro.Autores.Count == 0)
                    throw new Exception("Debés seleccionar al menos un autor.");

                if (libro.Categorias.Count == 0)
                    throw new Exception("Debés seleccionar al menos un género.");

                LibroNegocio negocio = new LibroNegocio();
                negocio.agregar(libro);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                cargarListasDesplegables();
                return View(libro);
            }
        }

        // GET: Libros/Edit/5
        public ActionResult Edit(int id)
        {
            LibroNegocio negocio = new LibroNegocio();
            List<Libro> lista = negocio.listar();

            Libro libro = lista.Find(x => x.Id == id);

            if (libro == null)
                return HttpNotFound();

            cargarListasDesplegables();
            return View(libro);
        }

        // POST: Libros/Edit/5
        [HttpPost]
        public ActionResult Edit(
            int Id,
            string Titulo,
            string ISBN,
            int? IdEditorial,
            int? AnioEdicion,
            int[] IdsAutores,
            int[] IdsCategorias)
        {
            Libro libro = new Libro
            {
                Id = Id,
                Titulo = Titulo,
                ISBN = ISBN,
                AnioEdicion = AnioEdicion ?? 0,
                Editorial = new Editorial { Id = IdEditorial ?? 0 }
            };

            try
            {
                if (IdsAutores != null)
                {
                    foreach (int id in IdsAutores)
                        libro.Autores.Add(new Autor { Id = id });
                }

                if (IdsCategorias != null)
                {
                    foreach (int id in IdsCategorias)
                        libro.Categorias.Add(new Categoria { Id = id });
                }

                if (string.IsNullOrWhiteSpace(Titulo) || string.IsNullOrWhiteSpace(ISBN))
                    throw new Exception("El título y el ISBN son obligatorios.");

                if (IdEditorial == null || IdEditorial <= 0)
                    throw new Exception("Debés seleccionar una editorial.");

                if (libro.Autores.Count == 0)
                    throw new Exception("Debés seleccionar al menos un autor.");

                if (libro.Categorias.Count == 0)
                    throw new Exception("Debés seleccionar al menos un género.");

                LibroNegocio negocio = new LibroNegocio();
                negocio.modificar(libro);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                cargarListasDesplegables();
                return View(libro);
            }
        }

        // GET: Libros/Delete/5
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

        // GET: Libros/Details/5
        public ActionResult Details(int id)
        {
            LibroNegocio negocio = new LibroNegocio();
            List<Libro> lista = negocio.listar();

            Libro libro = lista.Find(x => x.Id == id);

            if (libro == null)
                return HttpNotFound();

            return View(libro);
        }

        private void cargarListasDesplegables()
        {
            EditorialNegocio editorialNegocio = new EditorialNegocio();
            AutorNegocio autorNegocio = new AutorNegocio();
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();

            ViewBag.Editoriales = editorialNegocio.listar();
            ViewBag.Autores = autorNegocio.listar();
            ViewBag.Categorias = categoriaNegocio.listar();
        }
    }
}