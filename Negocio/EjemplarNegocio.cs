
using System.Collections.Generic;
using Dominio;

namespace Negocio
{
    public class EjemplarNegocio
    {
        public List<Ejemplar> listar()
        {
            List<Ejemplar> lista = new List<Ejemplar>();

            LibroNegocio negocio = new LibroNegocio();
            List<Libro> libros = negocio.listar();

            foreach (Libro libro in libros)
            {
                foreach (Ejemplar ejemplar in libro.Ejemplares)
                {
                    ejemplar.TituloLibro = libro.Titulo;
                    lista.Add(ejemplar);
                }
            }

            return lista;
        }
    }
}
