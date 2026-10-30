using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Ejemplar
    {
        public int Id { get; set; }
        public int IdLibro { get; set; }
        public string TituloLibro { get; set; }
        public string CodigoInventario { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }

        public override string ToString()
        {
            return CodigoInventario;
        }
    }
}