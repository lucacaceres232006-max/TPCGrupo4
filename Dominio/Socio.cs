using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Socio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool Habilitado { get; set; }
        public PoliticaPrestamo PoliticaPrestamo { get; set; }

        public override string ToString()
        {
            return Apellido + ", " + Nombre + " (" + DNI + ")";
        }
    }
}