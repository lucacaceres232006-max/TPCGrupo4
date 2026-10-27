using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string ISBN { get; set; }
        public Editorial Editorial { get; set; }
        public int AnioEdicion { get; set; }
        public List<Autor> Autores { get; set; }
        public List<Categoria> Categorias { get; set; }
        public List<Ejemplar> Ejemplares { get; set; }

        public Libro()
        {
            Autores = new List<Autor>();
            Categorias = new List<Categoria>();
            Ejemplares = new List<Ejemplar>();
        }

        public override string ToString()
        {
            return Titulo;
        }
    }
}