using System;
using System.Collections.Generic;
using Dominio;

namespace Negocio
{
    public class SocioNegocio
    {
        public List<Socio> listar()
        {
            List<Socio> lista = new List<Socio>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
                    SELECT S.Id, S.Nombre, S.Apellido, S.DNI, S.Direccion, S.Telefono, S.Email, S.Habilitado,
                           P.Id AS IdPolitica, P.Descripcion AS DescripcionPolitica, P.DiasDuracion, P.CantidadMaximaEjemplares, P.LimiteRenovaciones
                    FROM SOCIOS S
                    INNER JOIN POLITICAS_PRESTAMO P ON S.IdPoliticaPrestamo = P.Id
                    ORDER BY S.Apellido, S.Nombre");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Socio aux = new Socio();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Apellido = (string)datos.Lector["Apellido"];
                    aux.DNI = (string)datos.Lector["DNI"];
                    aux.Habilitado = (bool)datos.Lector["Habilitado"];

                    if (!(datos.Lector["Direccion"] is DBNull))
                        aux.Direccion = (string)datos.Lector["Direccion"];

                    if (!(datos.Lector["Telefono"] is DBNull))
                        aux.Telefono = (string)datos.Lector["Telefono"];

                    if (!(datos.Lector["Email"] is DBNull))
                        aux.Email = (string)datos.Lector["Email"];

                    aux.PoliticaPrestamo = new PoliticaPrestamo
                    {
                        Id = (int)datos.Lector["IdPolitica"],
                        Descripcion = (string)datos.Lector["DescripcionPolitica"],
                        DiasDuracion = (int)datos.Lector["DiasDuracion"],
                        CantidadMaximaEjemplares = (int)datos.Lector["CantidadMaximaEjemplares"],
                        LimiteRenovaciones = (int)datos.Lector["LimiteRenovaciones"]
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

        public Socio obtenerPorId(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
                    SELECT S.Id, S.Nombre, S.Apellido, S.DNI, S.Direccion, S.Telefono, S.Email, S.Habilitado,
                           P.Id AS IdPolitica, P.Descripcion AS DescripcionPolitica, P.DiasDuracion, P.CantidadMaximaEjemplares, P.LimiteRenovaciones
                    FROM SOCIOS S
                    INNER JOIN POLITICAS_PRESTAMO P ON S.IdPoliticaPrestamo = P.Id
                    WHERE S.Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    Socio aux = new Socio();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Apellido = (string)datos.Lector["Apellido"];
                    aux.DNI = (string)datos.Lector["DNI"];
                    aux.Habilitado = (bool)datos.Lector["Habilitado"];

                    if (!(datos.Lector["Direccion"] is DBNull))
                        aux.Direccion = (string)datos.Lector["Direccion"];

                    if (!(datos.Lector["Telefono"] is DBNull))
                        aux.Telefono = (string)datos.Lector["Telefono"];

                    if (!(datos.Lector["Email"] is DBNull))
                        aux.Email = (string)datos.Lector["Email"];

                    aux.PoliticaPrestamo = new PoliticaPrestamo
                    {
                        Id = (int)datos.Lector["IdPolitica"],
                        Descripcion = (string)datos.Lector["DescripcionPolitica"],
                        DiasDuracion = (int)datos.Lector["DiasDuracion"],
                        CantidadMaximaEjemplares = (int)datos.Lector["CantidadMaximaEjemplares"],
                        LimiteRenovaciones = (int)datos.Lector["LimiteRenovaciones"]
                    };

                    return aux;
                }

                return null;
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

        public void agregar(Socio nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
                    IF EXISTS (SELECT 1 FROM SOCIOS WHERE DNI = @dni)
                        THROW 50001, 'Ya existe un socio registrado con el DNI ingresado.', 1;

                    INSERT INTO SOCIOS (Nombre, Apellido, DNI, Direccion, Telefono, Email, Habilitado, IdPoliticaPrestamo)
                    VALUES (@nombre, @apellido, @dni, @direccion, @telefono, @email, @habilitado, @idPolitica)");

                datos.setearParametro("@nombre", nuevo.Nombre);
                datos.setearParametro("@apellido", nuevo.Apellido);
                datos.setearParametro("@dni", nuevo.DNI);
                datos.setearParametro("@direccion", string.IsNullOrWhiteSpace(nuevo.Direccion) ? (object)DBNull.Value : nuevo.Direccion);
                datos.setearParametro("@telefono", string.IsNullOrWhiteSpace(nuevo.Telefono) ? (object)DBNull.Value : nuevo.Telefono);
                datos.setearParametro("@email", string.IsNullOrWhiteSpace(nuevo.Email) ? (object)DBNull.Value : nuevo.Email);
                datos.setearParametro("@habilitado", nuevo.Habilitado);
                datos.setearParametro("@idPolitica", nuevo.PoliticaPrestamo.Id);

                datos.ejecutarAccion();
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

        public void modificar(Socio socio)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
                    IF EXISTS (SELECT 1 FROM SOCIOS WHERE DNI = @dni AND Id <> @id)
                        THROW 50001, 'Ya existe otro socio registrado con el DNI ingresado.', 1;

                    UPDATE SOCIOS 
                    SET Nombre = @nombre, 
                        Apellido = @apellido, 
                        DNI = @dni, 
                        Direccion = @direccion, 
                        Telefono = @telefono, 
                        Email = @email, 
                        Habilitado = @habilitado, 
                        IdPoliticaPrestamo = @idPolitica 
                    WHERE Id = @id");

                datos.setearParametro("@id", socio.Id);
                datos.setearParametro("@nombre", socio.Nombre);
                datos.setearParametro("@apellido", socio.Apellido);
                datos.setearParametro("@dni", socio.DNI);
                datos.setearParametro("@direccion", string.IsNullOrWhiteSpace(socio.Direccion) ? (object)DBNull.Value : socio.Direccion);
                datos.setearParametro("@telefono", string.IsNullOrWhiteSpace(socio.Telefono) ? (object)DBNull.Value : socio.Telefono);
                datos.setearParametro("@email", string.IsNullOrWhiteSpace(socio.Email) ? (object)DBNull.Value : socio.Email);
                datos.setearParametro("@habilitado", socio.Habilitado);
                datos.setearParametro("@idPolitica", socio.PoliticaPrestamo.Id);

                datos.ejecutarAccion();
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

        public void eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
                    IF EXISTS (SELECT 1 FROM PRESTAMOS WHERE IdSocio = @id)
                        THROW 50001, 'No se puede eliminar el socio porque posee un historial de préstamos. Se recomienda suspenderlo en su lugar.', 1;

                    DELETE FROM SOCIOS WHERE Id = @id;");

                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
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