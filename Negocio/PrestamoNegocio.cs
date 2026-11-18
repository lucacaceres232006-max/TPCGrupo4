using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace Negocio
{
    public class PrestamoNegocio
    {
        public List<Prestamo> listar()
        {
            List<Prestamo> lista = new List<Prestamo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
                    SELECT P.Id, P.NumeroComprobante, P.FechaInicio, P.FechaPrevistaDevolucion, 
                           P.FechaDevolucionReal, P.CantidadRenovaciones, P.Estado, P.ObservacionesDevolucion,
                           S.Id AS IdSocio, S.Nombre AS NombreSocio, S.Apellido AS ApellidoSocio, S.DNI, S.Email,
                           PP.Id AS IdPolitica, PP.Descripcion AS DescripcionPolitica, PP.DiasDuracion, PP.CantidadMaximaEjemplares, PP.LimiteRenovaciones,
                           E.Id AS IdEjemplar, E.IdLibro, E.CodigoInventario, E.Estado AS EstadoEjemplar,
                           L.Titulo AS TituloLibro
                    FROM PRESTAMOS P
                    INNER JOIN SOCIOS S ON P.IdSocio = S.Id
                    INNER JOIN POLITICAS_PRESTAMO PP ON S.IdPoliticaPrestamo = PP.Id
                    INNER JOIN EJEMPLARES E ON P.IdEjemplar = E.Id
                    INNER JOIN LIBROS L ON E.IdLibro = L.Id
                    ORDER BY 
                        CASE WHEN P.Estado = 'Activo' THEN 0 ELSE 1 END,
                        P.FechaPrevistaDevolucion ASC");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Prestamo aux = new Prestamo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.NumeroComprobante = datos.Lector["NumeroComprobante"] is DBNull ? "" : (string)datos.Lector["NumeroComprobante"];
                    aux.FechaInicio = (DateTime)datos.Lector["FechaInicio"];
                    aux.FechaPrevistaDevolucion = (DateTime)datos.Lector["FechaPrevistaDevolucion"];
                    aux.CantidadRenovaciones = (int)datos.Lector["CantidadRenovaciones"];
                    aux.Estado = (string)datos.Lector["Estado"];

                    if (!(datos.Lector["FechaDevolucionReal"] is DBNull))
                        aux.FechaDevolucionReal = (DateTime)datos.Lector["FechaDevolucionReal"];

                    if (!(datos.Lector["ObservacionesDevolucion"] is DBNull))
                        aux.ObservacionesDevolucion = (string)datos.Lector["ObservacionesDevolucion"];

                    aux.Socio = new Socio
                    {
                        Id = (int)datos.Lector["IdSocio"],
                        Nombre = (string)datos.Lector["NombreSocio"],
                        Apellido = (string)datos.Lector["ApellidoSocio"],
                        DNI = (string)datos.Lector["DNI"],
                        Email = datos.Lector["Email"] is DBNull ? "" : (string)datos.Lector["Email"],
                        PoliticaPrestamo = new PoliticaPrestamo
                        {
                            Id = (int)datos.Lector["IdPolitica"],
                            Descripcion = (string)datos.Lector["DescripcionPolitica"],
                            DiasDuracion = (int)datos.Lector["DiasDuracion"],
                            CantidadMaximaEjemplares = (int)datos.Lector["CantidadMaximaEjemplares"],
                            LimiteRenovaciones = (int)datos.Lector["LimiteRenovaciones"]
                        }
                    };

                    aux.Ejemplar = new Ejemplar
                    {
                        Id = (int)datos.Lector["IdEjemplar"],
                        IdLibro = (int)datos.Lector["IdLibro"],
                        TituloLibro = (string)datos.Lector["TituloLibro"],
                        CodigoInventario = (string)datos.Lector["CodigoInventario"],
                        Estado = (string)datos.Lector["EstadoEjemplar"]
                    };

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

    }
}
