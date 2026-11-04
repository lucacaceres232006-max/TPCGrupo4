using System;
using System.Collections.Generic;
using Dominio;

namespace Negocio
{
    public class EjemplarNegocio
    {
        public List<Ejemplar> listar()
        {
            List<Ejemplar> lista = new List<Ejemplar>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
                    SELECT E.Id, E.IdLibro, L.Titulo AS TituloLibro, E.CodigoInventario, E.Estado, E.Observaciones 
                    FROM EJEMPLARES E 
                    INNER JOIN LIBROS L ON E.IdLibro = L.Id
                    ORDER BY L.Titulo, E.CodigoInventario");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Ejemplar aux = new Ejemplar();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.IdLibro = (int)datos.Lector["IdLibro"];
                    aux.TituloLibro = (string)datos.Lector["TituloLibro"];
                    aux.CodigoInventario = (string)datos.Lector["CodigoInventario"];
                    aux.Estado = (string)datos.Lector["Estado"];

                    if (!(datos.Lector["Observaciones"] is DBNull))
                        aux.Observaciones = (string)datos.Lector["Observaciones"];

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

        public Ejemplar obtenerPorId(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT E.Id, E.IdLibro, L.Titulo AS TituloLibro, E.CodigoInventario, E.Estado, E.Observaciones 
                    FROM EJEMPLARES E 
                    INNER JOIN LIBROS L ON E.IdLibro = L.Id
                    WHERE E.Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    Ejemplar aux = new Ejemplar();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.IdLibro = (int)datos.Lector["IdLibro"];
                    aux.TituloLibro = (string)datos.Lector["TituloLibro"];
                    aux.CodigoInventario = (string)datos.Lector["CodigoInventario"];
                    aux.Estado = (string)datos.Lector["Estado"];

                    if (!(datos.Lector["Observaciones"] is DBNull))
                        aux.Observaciones = (string)datos.Lector["Observaciones"];

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

        public void agregar(Ejemplar nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                if (string.IsNullOrWhiteSpace(nuevo.CodigoInventario))
                {
                    nuevo.CodigoInventario = generarCodigoInventario(nuevo.IdLibro);
                }
                else
                {
                    nuevo.CodigoInventario = nuevo.CodigoInventario.Trim().ToUpper();
                }

                datos.setearConsulta(@"
                    IF EXISTS (SELECT 1 FROM EJEMPLARES WHERE CodigoInventario = @codigo)
                        THROW 50001, 'Ya existe un ejemplar registrado con ese código de inventario.', 1;

                    INSERT INTO EJEMPLARES (IdLibro, CodigoInventario, Estado, Observaciones)
                    VALUES (@idLibro, @codigo, @estado, @observaciones);");

                datos.setearParametro("@idLibro", nuevo.IdLibro);
                datos.setearParametro("@codigo", nuevo.CodigoInventario);
                datos.setearParametro("@estado", string.IsNullOrWhiteSpace(nuevo.Estado) ? "Disponible" : nuevo.Estado);
                datos.setearParametro("@observaciones", nuevo.Observaciones);

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

        public string generarCodigoInventario(int idLibro)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    DECLARE @prefijo VARCHAR(3);
                    DECLARE @numero INT;
                    DECLARE @codigoCandidato VARCHAR(50);

                    SELECT @prefijo = UPPER(LEFT(REPLACE(Titulo, ' ', ''), 3))
                    FROM LIBROS
                    WHERE Id = @idLibro;

                    IF @prefijo IS NULL OR LEN(@prefijo) < 3
                        SET @prefijo = 'LIB';

                    SELECT @numero = COUNT(*) + 1
                    FROM EJEMPLARES
                    WHERE IdLibro = @idLibro;

                    SET @codigoCandidato = 'INV-' + @prefijo + '-' + RIGHT('000' + CAST(@numero AS VARCHAR(10)), 3);

                    WHILE EXISTS (SELECT 1 FROM EJEMPLARES WHERE CodigoInventario = @codigoCandidato)
                    BEGIN
                        SET @numero = @numero + 1;
                        SET @codigoCandidato = 'INV-' + @prefijo + '-' + RIGHT('000' + CAST(@numero AS VARCHAR(10)), 3);
                    END

                    SELECT @codigoCandidato AS CodigoGenerado;
                ");

                datos.setearParametro("@idLibro", idLibro);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    return (string)datos.Lector["CodigoGenerado"];
                }

                return "INV-" + idLibro + "-" + DateTime.Now.Ticks.ToString().Substring(10);
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

        public void modificar(Ejemplar ejemplar)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                ejemplar.CodigoInventario = ejemplar.CodigoInventario.Trim().ToUpper();

                datos.setearConsulta(@"
                    IF EXISTS (SELECT 1 FROM EJEMPLARES WHERE CodigoInventario = @codigo AND Id <> @id)
                        THROW 50001, 'Ya existe otro ejemplar con ese código de inventario.', 1;

                    UPDATE EJEMPLARES
                    SET IdLibro = @idLibro,
                        CodigoInventario = @codigo,
                        Estado = @estado,
                        Observaciones = @observaciones
                    WHERE Id = @id;");

                datos.setearParametro("@id", ejemplar.Id);
                datos.setearParametro("@idLibro", ejemplar.IdLibro);
                datos.setearParametro("@codigo", ejemplar.CodigoInventario);
                datos.setearParametro("@estado", ejemplar.Estado);
                datos.setearParametro("@observaciones", ejemplar.Observaciones);

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
                    IF EXISTS (SELECT 1 FROM PRESTAMOS WHERE IdEjemplar = @id)
                        THROW 50001, 'No se puede eliminar el ejemplar porque posee historial de préstamos. Si se perdió o dañó, cambie su estado a Extraviado o En reparación.', 1;

                    DELETE FROM EJEMPLARES WHERE Id = @id;");

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