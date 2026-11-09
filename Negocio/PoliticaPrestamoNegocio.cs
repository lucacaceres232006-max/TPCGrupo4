using System;
using System.Collections.Generic;
using Dominio;

namespace Negocio
{
    public class PoliticaPrestamoNegocio
    {
        public List<PoliticaPrestamo> listar()
        {
            List<PoliticaPrestamo> lista = new List<PoliticaPrestamo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, Descripcion, DiasDuracion, CantidadMaximaEjemplares, LimiteRenovaciones FROM POLITICAS_PRESTAMO ORDER BY Descripcion");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    PoliticaPrestamo aux = new PoliticaPrestamo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.DiasDuracion = (int)datos.Lector["DiasDuracion"];
                    aux.CantidadMaximaEjemplares = (int)datos.Lector["CantidadMaximaEjemplares"];
                    aux.LimiteRenovaciones = (int)datos.Lector["LimiteRenovaciones"];

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

        public PoliticaPrestamo obtenerPorId(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, Descripcion, DiasDuracion, CantidadMaximaEjemplares, LimiteRenovaciones FROM POLITICAS_PRESTAMO WHERE Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    PoliticaPrestamo aux = new PoliticaPrestamo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.DiasDuracion = (int)datos.Lector["DiasDuracion"];
                    aux.CantidadMaximaEjemplares = (int)datos.Lector["CantidadMaximaEjemplares"];
                    aux.LimiteRenovaciones = (int)datos.Lector["LimiteRenovaciones"];

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

        public void agregar(PoliticaPrestamo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("INSERT INTO POLITICAS_PRESTAMO (Descripcion, DiasDuracion, CantidadMaximaEjemplares, LimiteRenovaciones) VALUES (@descripcion, @dias, @cantidad, @limite)");
                datos.setearParametro("@descripcion", nuevo.Descripcion);
                datos.setearParametro("@dias", nuevo.DiasDuracion);
                datos.setearParametro("@cantidad", nuevo.CantidadMaximaEjemplares);
                datos.setearParametro("@limite", nuevo.LimiteRenovaciones);
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

        public void modificar(PoliticaPrestamo politica)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("UPDATE POLITICAS_PRESTAMO SET Descripcion = @descripcion, DiasDuracion = @dias, CantidadMaximaEjemplares = @cantidad, LimiteRenovaciones = @limite WHERE Id = @id");
                datos.setearParametro("@descripcion", politica.Descripcion);
                datos.setearParametro("@dias", politica.DiasDuracion);
                datos.setearParametro("@cantidad", politica.CantidadMaximaEjemplares);
                datos.setearParametro("@limite", politica.LimiteRenovaciones);
                datos.setearParametro("@id", politica.Id);
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
                    IF EXISTS (SELECT 1 FROM SOCIOS WHERE IdPoliticaPrestamo = @id)
                        THROW 50001, 'No se puede eliminar la política porque hay socios asignados a ella.', 1;

                    DELETE FROM POLITICAS_PRESTAMO WHERE Id = @id;");

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