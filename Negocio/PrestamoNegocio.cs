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

        public void agregar(Prestamo nuevo)
        {
            if (nuevo == null || nuevo.Socio == null || nuevo.Ejemplar == null)
                throw new Exception("Los datos del préstamo están incompletos.");

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.iniciarTransaccion();

                datos.setearConsulta(@"
            SELECT ISNULL(
                (SELECT PP.DiasDuracion
                 FROM SOCIOS S
                 INNER JOIN POLITICAS_PRESTAMO PP
                    ON S.IdPoliticaPrestamo = PP.Id
                 WHERE S.Id = @IdSocio
                 AND S.Habilitado = 1), 0)");

                datos.setearParametro("@IdSocio", nuevo.Socio.Id);

                int diasDuracion = datos.ejecutarAccionScalar();

                if (diasDuracion <= 0)
                    throw new Exception("El socio no está habilitado o no tiene una política válida.");

                datos.setearConsulta(@"
            SELECT ISNULL(
                (SELECT PP.CantidadMaximaEjemplares
                 FROM SOCIOS S
                 INNER JOIN POLITICAS_PRESTAMO PP
                    ON S.IdPoliticaPrestamo = PP.Id
                 WHERE S.Id = @IdSocio
                 AND S.Habilitado = 1), 0)");

                datos.setearParametro("@IdSocio", nuevo.Socio.Id);

                int cantidadMaxima = datos.ejecutarAccionScalar();

                datos.setearConsulta(@"
            SELECT COUNT(*)
            FROM PRESTAMOS WITH (UPDLOCK, HOLDLOCK)
            WHERE IdSocio = @IdSocio
            AND Estado = 'Activo'");

                datos.setearParametro("@IdSocio", nuevo.Socio.Id);

                int prestamosActivos = datos.ejecutarAccionScalar();

                if (prestamosActivos >= cantidadMaxima)
                    throw new Exception("El socio alcanzó el límite máximo de préstamos.");

                datos.setearConsulta(@"
            SELECT ISNULL(
                MAX(TRY_CONVERT(BIGINT, SUBSTRING(NumeroComprobante, 6, 45))),
                0
            ) + 1
            FROM PRESTAMOS WITH (UPDLOCK, HOLDLOCK)
            WHERE NumeroComprobante LIKE 'COMP-%'");

                int siguienteNumero = datos.ejecutarAccionScalar();

                string comprobante = "COMP-" + siguienteNumero.ToString("D4");

                datos.setearConsulta(@"
            UPDATE EJEMPLARES
            SET Estado = 'Prestado'
            WHERE Id = @IdEjemplar
            AND Estado = 'Disponible';

            SELECT @@ROWCOUNT;");

                datos.setearParametro("@IdEjemplar", nuevo.Ejemplar.Id);

                int ejemplarActualizado = datos.ejecutarAccionScalar();

                if (ejemplarActualizado != 1)
                    throw new Exception("El ejemplar no existe o no está disponible.");

                datos.setearConsulta(@"
            INSERT INTO PRESTAMOS
            (
                NumeroComprobante,
                IdSocio,
                IdEjemplar,
                FechaPrevistaDevolucion,
                CantidadRenovaciones,
                Estado
            )
            VALUES
            (
                @NumeroComprobante,
                @IdSocio,
                @IdEjemplar,
                DATEADD(DAY, @DiasDuracion, GETDATE()),
                0,
                'Activo'
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);");

                datos.setearParametro("@NumeroComprobante", comprobante);
                datos.setearParametro("@IdSocio", nuevo.Socio.Id);
                datos.setearParametro("@IdEjemplar", nuevo.Ejemplar.Id);
                datos.setearParametro("@DiasDuracion", diasDuracion);

                int idGenerado = datos.ejecutarAccionScalar();

                datos.confirmarTransaccion();

                nuevo.Id = idGenerado;
                nuevo.NumeroComprobante = comprobante;
                nuevo.Estado = "Activo";
                nuevo.CantidadRenovaciones = 0;
                nuevo.Ejemplar.Estado = "Prestado";
            }
            catch
            {
                datos.cancelarTransaccion();
                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void devolver(int idPrestamo, string estadoEjemplar, string observaciones)
        {
            if (idPrestamo <= 0)
                throw new Exception("El préstamo seleccionado no es válido.");

            string estadoFinal = string.IsNullOrWhiteSpace(estadoEjemplar)
                ? "Disponible"
                : estadoEjemplar.Trim();

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.iniciarTransaccion();

                datos.setearConsulta(@"
                    SELECT ISNULL(
                        (SELECT IdEjemplar
                         FROM PRESTAMOS WITH (UPDLOCK, HOLDLOCK)
                         WHERE Id = @IdPrestamo
                         AND Estado = 'Activo'), 0)");

                datos.setearParametro("@IdPrestamo", idPrestamo);

                int idEjemplar = datos.ejecutarAccionScalar();

                if (idEjemplar <= 0)
                    throw new Exception("El préstamo no existe o ya fue devuelto.");

                datos.setearConsulta(@"
                    UPDATE PRESTAMOS
                    SET Estado = 'Devuelto',
                        FechaDevolucionReal = GETDATE(),
                        ObservacionesDevolucion = @Observaciones
                    WHERE Id = @IdPrestamo
                    AND Estado = 'Activo';

                    SELECT @@ROWCOUNT;");

                datos.setearParametro("@IdPrestamo", idPrestamo);
                datos.setearParametro("@Observaciones", string.IsNullOrWhiteSpace(observaciones) ? (object)DBNull.Value : observaciones.Trim());

                int prestamosActualizados = datos.ejecutarAccionScalar();

                if (prestamosActualizados != 1)
                    throw new Exception("No se pudo registrar la devolución.");

                datos.setearConsulta(@"
                    UPDATE EJEMPLARES
                    SET Estado = @EstadoEjemplar,
                        Observaciones = CASE
                            WHEN @Observaciones IS NOT NULL THEN @Observaciones
                            ELSE Observaciones
                        END
                    WHERE Id = @IdEjemplar
                    AND Estado = 'Prestado';

                    SELECT @@ROWCOUNT;");

                datos.setearParametro("@EstadoEjemplar", estadoFinal);
                datos.setearParametro("@Observaciones", string.IsNullOrWhiteSpace(observaciones) ? (object)DBNull.Value : observaciones.Trim());
                datos.setearParametro("@IdEjemplar", idEjemplar);

                int ejemplaresActualizados = datos.ejecutarAccionScalar();

                if (ejemplaresActualizados != 1)
                    throw new Exception("No se pudo actualizar el estado del ejemplar.");

                datos.confirmarTransaccion();
            }
            catch
            {
                datos.cancelarTransaccion();
                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }


    }
}
