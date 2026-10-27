using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class PoliticaPrestamo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int DiasDuracion { get; set; }
        public int CantidadMaximaEjemplares { get; set; }
        public int LimiteRenovaciones { get; set; }

        public override string ToString()
        {
            return Descripcion;
        }
    }
}