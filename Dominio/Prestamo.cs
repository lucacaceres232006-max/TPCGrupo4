using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Prestamo
    {
        public int Id { get; set; }
        public string NumeroComprobante { get; set; }
        public Socio Socio { get; set; }
        public Ejemplar Ejemplar { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaPrevistaDevolucion { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public int CantidadRenovaciones { get; set; }
        public string Estado { get; set; }
        public string ObservacionesDevolucion { get; set; }

        public override string ToString()
        {
            return NumeroComprobante;
        }
    }
}